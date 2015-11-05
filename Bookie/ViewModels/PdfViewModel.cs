using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Bookie.Common;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Bookie.ViewModels
{
    public class PdfViewModel : NotifyBase
    {

        private ObservableCollection<BookMarkBase> _bookmarks;
        private CollectionViewSource _bookmarksCollection;
        private SfPdfViewerControl _pdfControl;
        private StorageFile _pdfFile;
        private StorageFile _loadedFile;
        private readonly BookMarkService _bookMarkService;
        private Visibility _bookMarkVisibility;
        private Stream _pdfStream;
        private int _currentPageNumber;
        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set { _currentPageNumber = value;
                NotifyPropertyChanged("CurrentPageNumber");
            }
        }

        public ObservableCollection<BookMarkBase> BookMarks
        {
            get { return _bookmarks; }
            set { _bookmarks = value;
                NotifyPropertyChanged("BookMarks");
            }
        }


        public CollectionViewSource BookMarksCollection
        {
            get { return _bookmarksCollection; }
            set
            {
                _bookmarksCollection = value;
                NotifyPropertyChanged("BookMarksCollection");
            }
        }



        public SfPdfViewerControl pdfControl
        {
           get { return _pdfControl; }
            set { _pdfControl = value;
                NotifyPropertyChanged("pdfControl");
            }
        }

       public  PdfLoadedDocument doc { get; set; }



        public Visibility BookMarkVisibility
        {
            get { return _bookMarkVisibility; }
            set
            {
                _bookMarkVisibility = value;
                NotifyPropertyChanged("BookMarkVisibility");
            }
        }


        public StorageFile PdfFile
        {
            get { return _pdfFile; }
            set
            {
                _pdfFile = value;
                NotifyPropertyChanged("PdfFile");
            }
        }





        public RelayCommand FitWidthCommand => new RelayCommand(FitWidth);
        public RelayCommand SinglePageCommand => new RelayCommand(SinglePage);
        public RelayCommand NormalCommand => new RelayCommand(Normal);


        public event EventHandler LoadingFinished;



        public PdfViewModel()
        {
            _bookMarkService = new BookMarkService(new BookMarkRepository());
        }


        private IconElement _bookmarkIcon;

        public IconElement BookMarkIcon
        {
            get { return _bookmarkIcon; }
            set { _bookmarkIcon = value;
                NotifyPropertyChanged("BookMarkIcon");
            }
        }

        public void CheckPageForBookMark(int pageNumber)
        {
            var exists = SelectedBook.BookMarks.FirstOrDefault(x => x.PageNumber == pageNumber);
            if (exists == null)
            {
                BookMarkIcon = new SymbolIcon(Symbol.Add);
                BookMarkVisibility = Visibility.Collapsed;
            }
            else
            {
                BookMarkIcon = new SymbolIcon(Symbol.Remove);
                BookMarkVisibility = Visibility.Visible;
            }
        }


        private void FitWidth(object parameter)
        {
            pdfControl.ViewMode = PageViewMode.FitWidth;
        }

        private void SinglePage(object parameter)
        {
            pdfControl.ViewMode = PageViewMode.OnePage;
        }

        private void Normal(object parameter)
        {
            pdfControl.ViewMode = PageViewMode.Normal;
        }


    

        private void AddBookMarkToPage(int pageNumber)
        {
            var bookmark = new BookMark
            {
                Book = SelectedBook,
                PageNumber = pageNumber
            };
            var exists = SelectedBook.BookMarks.FirstOrDefault(x => x.PageNumber == pageNumber);
            if (exists != null) return;
            SelectedBook.BookMarks.Add(bookmark);
            _bookMarkService.Add(bookmark);
            SelectedBook.BookMarks = _bookMarkService.GetAllForBook(SelectedBook);

            NotifyPropertyChanged("SelectedBook");
        }

        private void RemoveBookMarkFromPage(int pageNumber)
        {
            var exists = SelectedBook.BookMarks.FirstOrDefault(x => x.PageNumber == pageNumber);
            if (exists == null) return;
            _bookMarkService.Remove(exists);
            SelectedBook.BookMarks.Remove(exists);
            SelectedBook.BookMarks = _bookMarkService.GetAllForBook(SelectedBook);

            NotifyPropertyChanged("SelectedBook");
        }


        public bool ToggleBookMark()
        {
            bool added;
            var pageHasBookMark = SelectedBook.BookMarks.FirstOrDefault(x => x.PageNumber == CurrentPageNumber);
            if (pageHasBookMark == null)
            {
                AddBookMarkToPage(CurrentPageNumber);
                added = true;
            }
            else
            {
                RemoveBookMarkFromPage(CurrentPageNumber);
                added = false;
            }
            CheckPageForBookMark(CurrentPageNumber);
            UpdateBookMarks();
            return added;
        }








        public Book SelectedBook => ShellViewModel.SelectedBook;

     

       

        private async void LoadPdf(StorageFile pdfFile)
        {
            if (pdfFile != null)
            {
                var stream = await pdfFile.OpenAsync(FileAccessMode.Read);
                Stream fileStream = stream.AsStreamForRead();

                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                doc = new PdfLoadedDocument(buffer);
                pdfControl.LoadDocument(doc);

            }
            BookMarks = new ObservableCollection<BookMarkBase>();


            BookMarksCollection = new CollectionViewSource();
            BookMarksCollection.Source = BookMarks;

            LoadingFinished?.Invoke(this, null);
            CheckPageForBookMark(1);
            CurrentPageNumber = 1;
            UpdateBookMarks();
        }


        private void UpdateBookMarks()
        {
            SelectedBook.BookMarks = _bookMarkService.GetAllForBook(SelectedBook);
            SelectedBook.BookMarks = SelectedBook.BookMarks.OrderBy(x => x.PageNumber).ToList();
            NotifyPropertyChanged("SelectedBook");
        }



        public async void LoadDefaultFile()
        {
            if (ShellViewModel.SelectedBook == null) return;
            var storageFolder = await
            StorageApplicationPermissions.FutureAccessList.GetFolderAsync(ShellViewModel.SelectedBook.Source.Token);
            var file = await storageFolder.GetFileAsync(ShellViewModel.SelectedBook.FileName);
            _loadedFile = file ?? null;

            LoadPdf(_loadedFile);
        }


        public Stream PdfStream
        {
            get { return _pdfStream; }
            set { _pdfStream = value;
                NotifyPropertyChanged("PdfStream");
            }
        }

    }
}