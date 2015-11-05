using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Data;
using Bookie.Domain;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Bookie.Data.Repositories;
using static System.String;

namespace Bookie.ViewModels
{
    public class MainPageViewModel : NotifyBase, IProgressSubscriber
    {
        private readonly BookService _bookService = new BookService(new BookRepository());

        private CollectionViewSource _collectionView;
        private ImportProgress _importProgress;
        private Book _selectedBook;
        private string _filterQuery;
        private ObservableCollection<Book> _filteredBooks;
        private List<Book> _allBooks;


        
        public RelayCommand EditCommand
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    EditBook();
                });
            }
        }

        public RelayCommand OpenBookCommand
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    OpenBook();
                });
            }
        }

        private void OpenBook()
        {
            if (SelectedBook?.FileName != null)
            {
                ShellViewModel.SelectedBook = SelectedBook;
            }
        }

        public Book SelectedBook
        {
            get
            {
                return _selectedBook;
            }
            set
            {
                _selectedBook = value;
                OpenBook();
                NotifyPropertyChanged("SelectedBook");
            }
        }

        public string FilterQuery
        {
            get { return _filterQuery; }
            set
            {
                _filterQuery = value;
                Filter();
                NotifyPropertyChanged("FilterColor");
                NotifyPropertyChanged("FilterQuery");
            }
        }

        public ObservableCollection<Book> FilteredBooks
        {
            get { return _filteredBooks; }
            set { _filteredBooks = value; NotifyPropertyChanged("FilteredBooks"); }
        }

        public List<Book> AllBooks
        {
            get { return _allBooks; }
            set
            {
                _allBooks = value;
                NotifyPropertyChanged("AllBooks");
            }
        }


        private bool _filterFavourites;
        private bool _filterReading;

        public bool FilterFavourites
        {
            get { return _filterFavourites; }
            set { _filterFavourites = value;
                NotifyPropertyChanged("FilterFavourites");
                Filter();
            }
        }

        public bool FilterReading
        {
            get { return _filterReading; }
            set
            {
                _filterReading = value;
                NotifyPropertyChanged("FilterReading");
                Filter();
            }
        }



        private bool _filterScraped;

        public bool FilterScraped
        {
            get { return _filterScraped; }
            set { _filterScraped = value;
                NotifyPropertyChanged("FilterScraped");
                Filter();
            }
        }

        public void Filter()
        {
            var f = new ObservableCollection<Book>(AllBooks);
            if (!String.IsNullOrEmpty(FilterQuery))
            {
                var result = f.Where(x => x.Title.ToLower().Contains(FilterQuery.ToLower()));
                f = new ObservableCollection<Book>(result);
            }
            if (FilterScraped)
            {
                var result = f.Where(x => x.Scraped == true);
                f = new ObservableCollection<Book>(result);
            }
            if (FilterFavourites)
            {
                var result = f.Where(x => x.Favourite == true);
                f = new ObservableCollection<Book>(result);
            }
            if (FilterReading)
            {
                var result = f.Where(x => x.CurrentPage != null);
                f = new ObservableCollection<Book>(result);
            }

            FilteredBooks = new ObservableCollection<Book>(f);
            FilterCount = "Found " + FilteredBooks.Count;
        }


        public MainPageViewModel()
        {
            ProgressService.RegisterSubscriber(this);
            AllBooks = new List<Book>();
            RefreshBooks();
            Ratings = new List<int>();
            Ratings.Add(0);
            Ratings.Add(1);
            Ratings.Add(2);
            Ratings.Add(3);
            Ratings.Add(4);
            Ratings.Add(5);
        }

        private string _filterCount;

        public string FilterCount
        {
            get { return _filterCount; }
            set
            {
                _filterCount = value;
                NotifyPropertyChanged("FilterCount");
            }
        }



        private List<int> _ratings;

        public List<int> Ratings
        {
            get { return _ratings; }
            set { _ratings = value;
                NotifyPropertyChanged("Ratings");
            }
        }

        private bool _pageHasBookMark;

        public bool PageHasBookMark
        {
            get { return _pageHasBookMark; }
            set { _pageHasBookMark = value;
                NotifyPropertyChanged("PageHasBookMark");
            }
        }


        private void RefreshBooks()
        {
            AllBooks = _bookService.GetAll();
            FilteredBooks = new ObservableCollection<Book>(AllBooks);
        }

     

        private void EditBook()
        {
            
        }

  
        internal ObservableCollection<Book> FilterByTitleNotGrouped(string filterQuery)
        {
            var result = FilteredBooks.Where(x => x.Title.Contains(filterQuery)).ToList();
            return new ObservableCollection<Book>(result);
        }

      
        public void _progress_ProgressChanged(object sender, ProgressWindowEventArgs e)
        {
            _importProgress.ViewModel.Title = e.OperationName;
            _importProgress.ViewModel.Operation = e.ProgressText;
            _importProgress.ViewModel.Progress = e.ProgressPercentage;
        }

        public async void _progress_ProgressStarted(object sender, EventArgs e)
        {
            _importProgress = new ImportProgress();
            await _importProgress.ShowAsync();
        }

        public void _progress_ProgressCompleted(object sender, EventArgs e)
        {
            _importProgress.Hide();
            RefreshBooks();
        }

        public void BookChanged(object sender, BookEventArgs e)
        {
            if (e.Book == null)
            {
                return;
            }
            switch (e.State)
            {
                case (BookEventArgs.BookState.Added):
                    var bookExistsAdded =
                        AllBooks.Any(
                            b =>
                                b.Id
                                == e.Book.Id);
                    if (!bookExistsAdded)
                    {
                        AllBooks.Add(e.Book);
                    }
                    NotifyPropertyChanged("FilterCount");
                    break;

                case (BookEventArgs.BookState.Removed):
                    var bookExistsRemoved =
                        AllBooks.Any(
                            b =>
                                b.Id
                                == e.Book.Id);
                    if (bookExistsRemoved)
                    {
                        AllBooks.Remove(e.Book);
                    }
                    NotifyPropertyChanged("FilterCount");
                    break;

                case (BookEventArgs.BookState.Updated): //Remove book from list and re-add it
                    var bookExistsUpdated =
                        AllBooks.FirstOrDefault(
                            b =>
                                b.FullPathAndFileName
                                == e.Book.FullPathAndFileName);
                    if (bookExistsUpdated != null)
                    {
                        var index = AllBooks.IndexOf(bookExistsUpdated);
                        AllBooks.Remove(bookExistsUpdated);
                        AllBooks.Insert(index, e.Book);
                    }
                    else
                    {
                        AllBooks.Add(e.Book);
                    }
                    NotifyPropertyChanged("FilterCount");
                    break;
            }
            FilteredBooks = new ObservableCollection<Book>(AllBooks);
        }

        public  void UpdateBook(Book book)
        {
            _bookService.Update(book);
            RefreshBooks();
        }

        public Brush FilterColor => IsNullOrEmpty(FilterQuery) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Yellow);
    }
}