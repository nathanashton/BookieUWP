using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain.Services;
using Bookie.Mvvm;
using static System.String;

namespace Bookie.ViewModels
{
    public class MainPageViewModel : NotifyBase, IProgressSubscriber
    {
        private ObservableCollection<Letter> _letters;
        private readonly BookService _bookService = new BookService(new BookRepository());
        private List<Book> _allBooks;

        private string _filterCount;
        private ObservableCollection<Book> _filteredBooks;


        private bool _filterFavourites;
        private string _filterQuery;
        private bool _filterReading;


        private bool _filterScraped;

        private ImportProgress _importProgress;

        private bool _pageHasBookMark;

        private bool _filterDescription;

        public ObservableCollection<Letter> Letters
        {
            get {return _letters;}
            set { _letters = value;
                NotifyPropertyChanged("Letters");
            }
            
        }


        public bool FilterDescription
        {
            get { return _filterDescription; }
            set { _filterDescription = value;
                NotifyPropertyChanged("FilterDescription");
                Filter();

            }
        }

        private bool _filterBookmarks;

        public bool FilterBookmarks
        {
            get { return _filterBookmarks; }
            set
            {
                _filterBookmarks = value;
                NotifyPropertyChanged("FilterBookmarks");
                Filter();

            }
        }

        private List<int> _ratings;
        private Book _selectedBook;

        private double _letterWidth;

        public double LetterWidth
        {
            get { return _letterWidth; }
            set { _letterWidth = value;
                NotifyPropertyChanged("LetterWidth");
                UpdateLetterWidths();
            }
        }


        public MainPageViewModel()
        {
            Letters = new ObservableCollection<Letter>();
            var zero = new Letter { Name = "0", LWidth = LetterWidth };

            var a = new Letter {Name = "A", LWidth = LetterWidth};
            var b = new Letter { Name = "B", LWidth = LetterWidth };
            var c = new Letter { Name = "C", LWidth = LetterWidth };
            var d = new Letter { Name = "D", LWidth = LetterWidth };
            var e = new Letter { Name = "E", LWidth = LetterWidth };
            var f = new Letter { Name = "F", LWidth = LetterWidth };
            var g = new Letter { Name = "G", LWidth = LetterWidth };
            var h = new Letter { Name = "H", LWidth = LetterWidth };
            var i = new Letter { Name = "I", LWidth = LetterWidth };
            var j = new Letter { Name = "J", LWidth = LetterWidth };
            var k = new Letter { Name = "K", LWidth = LetterWidth };
            var l = new Letter { Name = "L", LWidth = LetterWidth };
            var m = new Letter { Name = "M", LWidth = LetterWidth };
            var n = new Letter { Name = "N", LWidth = LetterWidth };
            var o = new Letter { Name = "O", LWidth = LetterWidth };
            var p = new Letter { Name = "P", LWidth = LetterWidth };
            var q = new Letter { Name = "Q", LWidth = LetterWidth };
            var r = new Letter { Name = "R", LWidth = LetterWidth };
            var s = new Letter { Name = "S", LWidth = LetterWidth };
            var t = new Letter { Name = "T", LWidth = LetterWidth };
            var u = new Letter { Name = "U", LWidth = LetterWidth };
            var v = new Letter { Name = "V", LWidth = LetterWidth };
            var w = new Letter { Name = "W", LWidth = LetterWidth };
            var x = new Letter { Name = "X", LWidth = LetterWidth };
            var y = new Letter { Name = "Y", LWidth = LetterWidth };
            var z = new Letter { Name = "Z", LWidth = LetterWidth };
            Letters.Add(zero);
            Letters.Add(a);
            Letters.Add(b);
            Letters.Add(c);
            Letters.Add(d);
            Letters.Add(e);
            Letters.Add(f);
            Letters.Add(g);
            Letters.Add(h);
            Letters.Add(i);
            Letters.Add(j);
            Letters.Add(k);
            Letters.Add(l);
            Letters.Add(m);
            Letters.Add(n);
            Letters.Add(o);
            Letters.Add(p);
            Letters.Add(q);
            Letters.Add(r);
            Letters.Add(s);
            Letters.Add(t);
            Letters.Add(u);
            Letters.Add(v);
            Letters.Add(w);
            Letters.Add(x);
            Letters.Add(y);
            Letters.Add(z);



            ProgressService.RegisterSubscriber(this);
            AllBooks = new List<Book>();
            RefreshBooksFromDb();
            Ratings = new List<int>();
            Ratings.Add(0);
            Ratings.Add(1);
            Ratings.Add(2);
            Ratings.Add(3);
            Ratings.Add(4);
            Ratings.Add(5);
            BooksScroll = ScrollMode.Enabled;
           
        }

        private Brush _shelfBrush;

        public Brush ShelfBrush
        {
            get { return _shelfBrush; }
            set { _shelfBrush = value;
                NotifyPropertyChanged("ShelfBrush");
            }
        }

        private Brush _gridBrush;

        public Brush GridBrush
        {
            get { return _gridBrush; }
            set
            {
                _gridBrush = value;
                NotifyPropertyChanged("GridBrush");
            }
        }

        public void UpdateLetterWidths()
        {
            foreach (var l in Letters)
            {
                l.LWidth = LetterWidth;
            }

        }



        public RelayCommand OpenBookCommand
        {
            get { return new RelayCommand(args => { OpenBook(); }); }
        }

        public Book SelectedBook
        {
            get { return _selectedBook; }
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
            set
            {
                _filteredBooks = value;
                NotifyPropertyChanged("FilteredBooks");
            }
        }

        private ObservableCollection<Book> _shelfBooks;

        public ObservableCollection<Book> ShelfBooks
        {
            get { return _shelfBooks; }
            set { _shelfBooks = value;
                NotifyPropertyChanged("ShelfBooks");
            }
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

        public bool FilterFavourites
        {
            get { return _filterFavourites; }
            set
            {
                _filterFavourites = value;
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

        public bool FilterScraped
        {
            get { return _filterScraped; }
            set
            {
                _filterScraped = value;
                NotifyPropertyChanged("FilterScraped");
                Filter();
            }
        }

        public string FilterCount
        {
            get { return _filterCount; }
            set
            {
                _filterCount = value;
                NotifyPropertyChanged("FilterCount");
            }
        }

        public List<int> Ratings
        {
            get { return _ratings; }
            set
            {
                _ratings = value;
                NotifyPropertyChanged("Ratings");
            }
        }

        public bool PageHasBookMark
        {
            get { return _pageHasBookMark; }
            set
            {
                _pageHasBookMark = value;
                NotifyPropertyChanged("PageHasBookMark");
            }
        }

        private Visibility _shelfVisibility;

        public Visibility ShelfVisibility
        {
            get { return _shelfVisibility; }
            set { _shelfVisibility = value;
                NotifyPropertyChanged("ShelfVisibility");
            }
        }

        public Brush FilterColor
        {
            get
            {
                if (!IsNullOrEmpty(FilterQuery) || FilterScraped ||FilterReading || FilterFavourites)
                {
                    return new SolidColorBrush(Colors.Yellow);
                }
                return new SolidColorBrush(Colors.White);
            }
        }

        public void UpdateShelfBooks()
        {
            ShelfBooks = new ObservableCollection<Book>(AllBooks.Where(x => x.Shelf == true));
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
            RefreshBooksFromDb();
        }

        private void OpenBook()
        {
            if (SelectedBook?.FileName != null)
            {
                ShellViewModel.SelectedBook = SelectedBook;
            }
        }

        public void SelectLetter(string letter)
        {
            foreach (var l in Letters)
            {
                if (l.Name.ToLower() == letter)
                {
                    l.Selected = true;
                }
                else
                {
                    l.Selected = false;

                }
            }

        }

        public void Filter()
        {
            var f = new ObservableCollection<Book>(AllBooks);
            if (!IsNullOrEmpty(FilterQuery))
            {
                var result = f.Where(x => x.Title.ToLower().Contains(FilterQuery.ToLower()));
                f = new ObservableCollection<Book>(result);
            }
            if (FilterScraped)
            {
                var result = f.Where(x => x.Scraped);
                f = new ObservableCollection<Book>(result);
            }
            if (FilterFavourites)
            {
                var result = f.Where(x => x.Favourite);
                f = new ObservableCollection<Book>(result);
            }
            if (FilterReading)
            {
                var result = f.Where(x => x.CurrentPage != null);
                f = new ObservableCollection<Book>(result);
            }
            if (FilterDescription)
            {
                var result = f.Where(x => x.Abstract != null && !IsNullOrEmpty(FilterQuery) && x.Abstract.ToLower().Contains(FilterQuery.ToLower()));
                f = new ObservableCollection<Book>(result);
            }
            if (FilterBookmarks)
            {
                var result = f.Where(x => x.BookMarks != null && x.BookMarks.Count > 0);
                f = new ObservableCollection<Book>(result);
            }

            FilteredBooks = new ObservableCollection<Book>(f);
            FilterCount = "Found " + FilteredBooks.Count;
            NotifyPropertyChanged("FilterColor");

        }

        private ScrollMode _booksScroll;

        public ScrollMode BooksScroll
        {
            get { return _booksScroll; }
            set { _booksScroll = value;
                NotifyPropertyChanged("BooksScroll");
            }
        }  

        private async void RefreshBooksFromDb()
        {
            AllBooks = await _bookService.GetAll();
            FilteredBooks = new ObservableCollection<Book>(AllBooks);
            UpdateShelfBooks();
            FilterCount = "Found " + FilteredBooks.Count;
        }


        internal ObservableCollection<Book> FilterByTitleNotGrouped(string filterQuery)
        {
            var result = FilteredBooks.Where(x => x.Title.Contains(filterQuery)).ToList();
            return new ObservableCollection<Book>(result);
        }

        public void BookChanged(object sender, BookEventArgs e)
        {
            if (e.Book == null)
            {
                return;
            }
            switch (e.State)
            {
                case BookEventArgs.BookState.Added:
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

                case BookEventArgs.BookState.Removed:
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

                case BookEventArgs.BookState.Updated: //Remove book from list and re-add it
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

        private Book _draggedBook;

        public Book DraggedBook
        {
            get { return _draggedBook; }
            set { _draggedBook = value;
                NotifyPropertyChanged("DraggedBook");
            }
        }

        public void UpdateBook(Book book)
        {
            _bookService.Update(book);
         //   RefreshBooksFromDb();
        }
    }
}