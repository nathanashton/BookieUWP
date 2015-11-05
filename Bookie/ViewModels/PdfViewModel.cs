using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;

namespace Bookie.ViewModels
{
    public class PdfViewModel : NotifyBase
    {
        private readonly BookMarkService _bookMarkService;


        private IconElement _bookmarkIcon;

        private ObservableCollection<BookMarkBase> _bookmarks;
        private CollectionViewSource _bookmarksCollection;
        private Visibility _bookMarkVisibility;
        private int _currentPageNumber;
        private StorageFile _loadedFile;
        private SfPdfViewerControl _pdfControl;
        private StorageFile _pdfFile;
        private Stream _pdfStream;


        public PdfViewModel()
        {
            _bookMarkService = new BookMarkService(new BookMarkRepository());
        }

        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set
            {
                _currentPageNumber = value;
                NotifyPropertyChanged("CurrentPageNumber");
            }
        }

        public ObservableCollection<BookMarkBase> BookMarks
        {
            get { return _bookmarks; }
            set
            {
                _bookmarks = value;
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


        public SfPdfViewerControl PdfControl
        {
            get { return _pdfControl; }
            set
            {
                _pdfControl = value;
                NotifyPropertyChanged("PdfControl");
            }
        }

        public PdfLoadedDocument Doc { get; set; }


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

        public IconElement BookMarkIcon
        {
            get { return _bookmarkIcon; }
            set
            {
                _bookmarkIcon = value;
                NotifyPropertyChanged("BookMarkIcon");
            }
        }

        public void UpdateBook()
        {
            new BookService(new BookRepository()).Update(SelectedBook);
        }


        public Book SelectedBook => ShellViewModel.SelectedBook;


        public Stream PdfStream
        {
            get { return _pdfStream; }
            set
            {
                _pdfStream = value;
                NotifyPropertyChanged("PdfStream");
            }
        }


        public event EventHandler LoadingFinished;

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
            PdfControl.ViewMode = PageViewMode.FitWidth;
        }

        private void SinglePage(object parameter)
        {
            PdfControl.ViewMode = PageViewMode.OnePage;
        }

        private void Normal(object parameter)
        {
            PdfControl.ViewMode = PageViewMode.Normal;
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


        private async void LoadPdf(StorageFile pdfFile)
        {
            if (pdfFile != null)
            {
                var stream = await pdfFile.OpenAsync(FileAccessMode.Read);
                var fileStream = stream.AsStreamForRead();

                var buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                Doc = new PdfLoadedDocument(buffer);
                PdfControl.LoadDocument(Doc);
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
            _loadedFile = file;
            LoadPdf(_loadedFile);
        }
    }
}