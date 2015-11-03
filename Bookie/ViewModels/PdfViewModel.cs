using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using PdfViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Bookie.Domain;

namespace Bookie.ViewModels
{
    public class PdfViewModel : NotifyBase
    {
        private StorageFile _pdfFile;
        private StorageFile _loadedFile;
        private PdfDocViewModel _p;
        private PdfDocument _pdfDocument;
        private readonly BookMarkService _bookMarkService;

        private int _currentPage;

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
            }
        }

        public PdfViewModel()
        {
            _bookMarkService = new BookMarkService(new BookMarkRepository());
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

        private Visibility _notes;

        public Visibility Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged("Notes");
            }
        }

        public RelayCommand BookMarkCommand => new RelayCommand(BookMarkPage);
        public RelayCommand ToggleBookMarksCommand => new RelayCommand(ToggleBookMarks);


        public bool CanE(object parameter)
        {
            return true;
        }

        private void ToggleBookMarks(object parameter)
        {
            BookMarksVisibility = BookMarksVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }



        private void BookMarkPage(object parameter)
        {
            var page = V[Convert.ToInt32(parameter)] as PdfPageViewModel;

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
            }
            UpdateBookmarks();
        }


        private Visibility _bookMarksVisibility;

        public Visibility BookMarksVisibility
        {
            get { return _bookMarksVisibility; }
            set { _bookMarksVisibility = value;
                NotifyPropertyChanged("BookMarksVisibility");
            }
        }


        private void UpdateBookmarks()
        {
            SelectedBook.BookMarks = _bookMarkService.GetAllForBook(SelectedBook);
            foreach (var bookmark in ShellViewModel.SelectedBook.BookMarks)
            {
                var page = V.GetPage(Convert.ToUInt32(bookmark.PageNumber)) as PdfPageViewModel;
                page.BookMark = true;
            }

            SelectedBook.BookMarks = SelectedBook.BookMarks.OrderBy(x => x.PageNumber).ToList();

        }

        public Book SelectedBook => ShellViewModel.SelectedBook;

        private string _start;
        private string _finish;

        public string Start
        {
            get { return _start; }
            set
            {
                _start = value;
                NotifyPropertyChanged("Start");
            }
        }

        public string Finish
        {
            get { return _finish; }
            set
            {
                _finish = value;
                NotifyPropertyChanged("Finish");
            }
        }

        public PdfDocViewModel V
        {
            get
            {
                return _p;
            }
            set
            {
                _p = value;
                NotifyPropertyChanged("V");
            }
        }

        private Visibility _progress;

        public Visibility Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        private async void LoadPdf(StorageFile pdfFile)
        {
            if (pdfFile != null)
            {
                Progress = Visibility.Visible;
                _pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);

                Progress = Visibility.Collapsed;
            }

            if (_pdfDocument == null) return;
            Size pageSize;

            pageSize.Width = Window.Current.Bounds.Width;
            pageSize.Height = Window.Current.Bounds.Height;

            try
            {
                V = new PdfDocViewModel(_pdfDocument, pageSize, SurfaceType.VirtualSurfaceImageSource);
                UpdateBookmarks();
                CurrentPage = 1;
                NotifyPropertyChanged("SelectedBook");
                BookMarksVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                //Log error
            }
        }

        public void SaveInk(IRandomAccessStream stream)
        {
            //  V[0].Ink.SaveAsync(stream);
            //    var all = V.Where(x => x.Ink.GetStrokes().Count > 0);
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

        public void PdfDocViewModel()
        {
            Notes = Visibility.Collapsed;
        }
    }
}