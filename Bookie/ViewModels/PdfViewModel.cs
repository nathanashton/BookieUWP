using System;
using System.IO;
using System.Linq;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using PdfViewModel;
using Syncfusion.Pdf.Parsing;
using PdfPage = Bookie.Views.PdfPage;

namespace Bookie.ViewModels
{
    public class PdfViewModel : NotifyBase
    {
        private bool _fullScreen;

        public bool FullScreen
        {
            get { return _fullScreen; }
            set { _fullScreen = value;
                NotifyPropertyChanged("FullScreen");
            }
        }
        private readonly BookMarkService _bookMarkService;
        private readonly BookService _bookService;
        private Visibility _bookMarksVisibility;
        private int _currentPage;
        private PdfLoadedDocument _doc;
        private StorageFile _loadedFile;
        private int _pageCount;

        private Orientation _pageOrientation;
        private PdfDocument _pdfDocument;
        private StorageFile _pdfFile;
        private PdfDocViewModel _pdfPages;
        private Visibility _progress;

        private BookMark _selectedBookMark;
        private double _sliderMinimum;
        public PdfPage scroll;

        public PdfViewModel()
        {
            _bookMarkService = new BookMarkService(new BookMarkRepository());
            _bookService = new BookService(new BookRepository());
            BookMarksVisibility = Visibility.Collapsed;
        }

        public PdfLoadedDocument doc
        {
            get { return _doc; }
            set
            {
                _doc = value;
                NotifyPropertyChanged("doc");
            }
        }

        public Visibility BookMarksVisibility
        {
            get { return _bookMarksVisibility; }
            set
            {
                _bookMarksVisibility = value;
                NotifyPropertyChanged("BookMarksVisibility");
            }
        }

        public Orientation PageOrientation
        {
            get { return _pageOrientation; }
            set
            {
                _pageOrientation = value;
                NotifyPropertyChanged("PageOrientation");
            }
        }


        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
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

        public Book SelectedBook => ShellViewModel.SelectedBook;

        public PdfDocViewModel PdfPages
        {
            get { return _pdfPages; }
            set
            {
                _pdfPages = value;
                NotifyPropertyChanged("PdfPages");
            }
        }

        public Visibility Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                _pageCount = value;
                NotifyPropertyChanged("PageCount");
            }
        }

        public double SliderMinimum
        {
            get { return _sliderMinimum; }
            set
            {
                _sliderMinimum = value;
                NotifyPropertyChanged("SliderMinimum");
            }
        }

        public RelayCommand BookMarkCommand => new RelayCommand(BookMarkPage);

        public RelayCommand ToggleBookMarksCommand => new RelayCommand(ToggleBookMarks);

        public BookMark SelectedBookMark
        {
            get { return _selectedBookMark; }
            set
            {
                _selectedBookMark = value;
                NotifyPropertyChanged("SelectedBookMark");
            }
        }

        public event EventHandler PdfLoadedEvent;

        private void ToggleBookMarks(object parameter)
        {
            BookMarksVisibility = BookMarksVisibility == Visibility.Collapsed
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private void BookMarkPage(object parameter)
        {
            var page = PdfPages[Convert.ToInt32(parameter)] as PdfPageViewModel;
            if (page == null) return;

            if (page.BookMark)
            {
                page.BookMark = false;
                //If bookmark exists, delete it and update
                var existingBookMark = SelectedBook.BookMarks.FirstOrDefault(x => x.PageNumber == page.PageNumber);
                if (existingBookMark != null)
                {
                    _bookMarkService.Remove(existingBookMark);
                    SelectedBook.BookMarks.Remove(existingBookMark);
                    NotifyPropertyChanged("SelectedBook");
                }
                ShellViewModel.ShowMessage("Bookmark Removed", null);
            }
            else
            {
                page.BookMark = true;
                //if bookmark doesnt exists, add it and update.
                var existingBookMark = SelectedBook.BookMarks.FirstOrDefault(x => x.PageNumber == page.PageNumber);
                if (existingBookMark == null)
                {
                    var bookmark = new BookMark
                    {
                        Book = SelectedBook,
                        PageNumber = Convert.ToInt32(page.PageNumber)
                    };
                    SelectedBook.BookMarks.Add(bookmark);

                    NotifyPropertyChanged("SelectedBook");
                    _bookMarkService.Add(bookmark);
                }
                ShellViewModel.ShowMessage("Bookmark Added", null);

            }
            UpdateBookmarks();
        }

        private void UpdateBookmarks()
        {
            SelectedBook.BookMarks = _bookMarkService.GetAllForBook(SelectedBook);
            foreach (var bookmark in ShellViewModel.SelectedBook.BookMarks)
            {
                var page = PdfPages.GetPage(Convert.ToUInt32(bookmark.PageNumber)) as PdfPageViewModel;
                if (page != null) page.BookMark = true;
            }

            SelectedBook.BookMarks = SelectedBook.BookMarks.OrderBy(x => x.PageNumber).ToList();
        }

        public void UpdateBook()
        {
            _bookService.Update(SelectedBook);
        }


        private async void LoadPdf(StorageFile pdfFile)
        {
            if (pdfFile != null)
            {
                Progress = Visibility.Visible;
                try
                {
                }
                catch (Exception)
                {
                    Progress = Visibility.Collapsed;

                    return;
                }
            }

            // if (_pdfDocument == null) return;
            Size pageSize;
            pageSize.Width = Window.Current.Bounds.Width;
            pageSize.Height = Window.Current.Bounds.Height;
            try
            {
                _pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
                PageCount = (int) _pdfDocument.PageCount;
                PdfPages = new PdfDocViewModel(_pdfDocument, pageSize, SurfaceType.VirtualSurfaceImageSource,
                    PdfPages_pdfLoaded);

                PdfLoaded();
            }
            catch (Exception)
            {
                Progress = Visibility.Collapsed;

                return;
            }
            UpdateBookmarks();
            NotifyPropertyChanged("SelectedBook");
            SliderMinimum = 1;
            CurrentPage = 1;
        }

        private void PdfPages_pdfLoaded()
        {
        }

        public async void LoadDefaultFile()
        {
            if (ShellViewModel.SelectedBook == null) return;
            var storageFolder = await
                StorageApplicationPermissions.FutureAccessList.GetFolderAsync(ShellViewModel.SelectedBook.Source.Token);
            var file = await storageFolder.GetFileAsync(ShellViewModel.SelectedBook.FileName);
            _loadedFile = file;
            LoadPdf(_loadedFile);
            if (file != null)
            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                var fileStream = stream.AsStreamForRead();

                var buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                doc = new PdfLoadedDocument(buffer);
            }
        }

        private void PdfLoaded()
        {
            if (PdfLoadedEvent != null)
            {
                Progress = Visibility.Collapsed;

                PdfLoadedEvent(this, null);
            }
        }
    }
}