using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using PdfViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Bookie.Domain;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;

namespace Bookie.ViewModels
{
    public class PdfViewModel : NotifyBase
    {
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