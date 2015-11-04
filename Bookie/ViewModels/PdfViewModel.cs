using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using PdfViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Bookie.Common;
using Bookie.Domain;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;
using PdfDocument = Windows.Data.Pdf.PdfDocument;

namespace Bookie.ViewModels
{
    public class PdfViewModel : NotifyBase
    {

        private ObservableCollection<BookMarkBase> _bookmarks;

        public ObservableCollection<BookMarkBase> BookMarks
        {
            get { return _bookmarks; }
            set { _bookmarks = value;
                NotifyPropertyChanged("BookMarks");
            }
        }

        private CollectionViewSource _bookmarksCollection;

        public CollectionViewSource BookMarksCollection
        {
            get { return _bookmarksCollection; }
            set
            {
                _bookmarksCollection = value;
                NotifyPropertyChanged("BookMarksCollection");
            }
        }


        private SfPdfViewerControl _pdfControl;

        public SfPdfViewerControl pdfControl
        {
           get { return _pdfControl; }
            set { _pdfControl = value;
                NotifyPropertyChanged("pdfControl");
            }
        }

       public  PdfLoadedDocument doc { get; set; }

        private StorageFile _pdfFile;
        private StorageFile _loadedFile;
        private PdfDocViewModel _p;
        private PdfDocument _pdfDocument;
        private readonly BookMarkService _bookMarkService;

    


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

        public PdfViewModel()
        {
            _bookMarkService = new BookMarkService(new BookMarkRepository());

        }



        public bool CanE(object parameter)
        {
            return true;
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


    

        public void BookMarkPage(BookMark bookmark)
        {
            bookmark.Book = SelectedBook;
 
                    
                    SelectedBook.BookMarks.Add(bookmark);

                    NotifyPropertyChanged("SelectedBook");
                    _bookMarkService.Add(bookmark);

            SelectedBook.BookMarks = _bookMarkService.GetAllForBook(SelectedBook);

        }




        private Visibility _bookMarksVisibility;

        public Visibility BookMarksVisibility
        {
            get { return _bookMarksVisibility; }
            set { _bookMarksVisibility = value;
                NotifyPropertyChanged("BookMarksVisibility");
            }
        }


    

        public Book SelectedBook => ShellViewModel.SelectedBook;

     

       

        private async void LoadPdf(StorageFile pdfFile)
        {
            if (pdfFile != null)
            {
                var stream = await pdfFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                Stream fileStream = stream.AsStreamForRead();

                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                doc = new PdfLoadedDocument(buffer);
                pdfControl.LoadDocument(doc);

            }
            if (LoadingFinished != null)
            {
                LoadingFinished(this, null);

            }
            BookMarks = new ObservableCollection<BookMarkBase>();


            foreach (PdfLoadedBookmark bookmark in doc.Bookmarks)
            {
                var bmark1 = new BookMarkBase();
                bmark1.Title = bookmark.Title;
                var page = bookmark.Destination.Page;
                int pageIndex = 0;
                foreach (PdfPageBase pageBase in doc.Pages)
                {
                    if (page == pageBase)
                    {
                        bmark1.PageNumber = pageIndex + 1;
                    }
                    pageIndex++;
                }

                if (bookmark.Count > 0)
                {
                    bmark1.BookMarks = new ObservableCollection<BookMarkBase>();
                    foreach (PdfLoadedBookmark bookmark2 in bookmark)
                    {
                        var bmark2 = new BookMarkBase();
                        bmark2.Title = bookmark2.Title;
                        var page2 = bookmark2.Destination.Page;
                        int pageIndex2 = 0;
                        foreach (PdfPageBase pageBase in doc.Pages)
                        {
                            if (page2 == pageBase)
                            {
                                bmark2.PageNumber = pageIndex2 + 1;
                            }
                            pageIndex2++;
                        }
                        if (bookmark2.Count > 0)
                        {
                            bmark2.BookMarks = new ObservableCollection<BookMarkBase>();
                            foreach (PdfLoadedBookmark bookmark3 in bookmark2)
                            {
                                var bmark3 = new BookMarkBase();
                                bmark3.Title = bookmark3.Title;
                                var page3 = bookmark3.Destination.Page;
                                int pageIndex3 = 0;
                                foreach (PdfPageBase pageBase in doc.Pages)
                                {
                                    if (page3 == pageBase)
                                    {
                                        bmark3.PageNumber = pageIndex3 + 1;
                                    }
                                    pageIndex3++;
                                }
                                bmark2.BookMarks.Add(bmark3);
                            }
                        }
                        bmark1.BookMarks.Add(bmark2);
                    }
                }

                BookMarks.Add(bmark1);
            }

            






            BookMarksCollection = new CollectionViewSource();
         //   BookMarksCollection.IsSourceGrouped = true;
         //   BookMarksCollection.ItemsPath = new PropertyPath("Title");
            BookMarksCollection.Source = BookMarks;
        }

        public event EventHandler LoadingFinished;


        public async void LoadDefaultFile()
        {
            if (ShellViewModel.SelectedBook == null) return;
            var storageFolder = await
            StorageApplicationPermissions.FutureAccessList.GetFolderAsync(ShellViewModel.SelectedBook.Source.Token);
            var file = await storageFolder.GetFileAsync(ShellViewModel.SelectedBook.FileName);
            _loadedFile = file ?? null;

            LoadPdf(_loadedFile);
        }

        private Stream _pdfStream;

        public Stream PdfStream
        {
            get { return _pdfStream; }
            set { _pdfStream = value;
                NotifyPropertyChanged("PdfStream");
            }
        }

    }
}