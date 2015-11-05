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
        private List<GroupInfoList<Book>> _groupedbooks;
        private Book _selectedBook;
        private string _filterQuery;
        private ObservableCollection<Book> _filteredBooks;
        private List<Book> _allBooks;

        public CollectionViewSource CollectionView
        {
            get { return _collectionView; }
            set
            {
                _collectionView = value;
                NotifyPropertyChanged("CollectionView");
            }
        }

        public List<GroupInfoList<Book>> GroupedBooks
        {
            get { return _groupedbooks; }
            set
            {
                _groupedbooks = value;
                NotifyPropertyChanged("GroupedBooks");
            }
        }



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
                GroupedBooks = FilterByTitleGrouped(value);
                FilteredBooks = FilterByTitleNotGrouped(value);
                NotifyPropertyChanged("FilteredBooks");
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

        public MainPageViewModel()
        {
            ProgressService.RegisterSubscriber(this);
            CollectionView = new CollectionViewSource();
            AllBooks = new List<Book>();
            Grouped();
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
            GroupedBooks = GetGroupsByLetter();
            CountResults(GroupedBooks);
        }

        internal List<GroupInfoList<Book>> GetGroupsByLetter()
        {
            var groups = new List<GroupInfoList<Book>>();

            var query =
                AllBooks.OrderBy(item => item.Title.ToLower())
                    .GroupBy(item => item.Title[0].ToString().ToLower())
                    .Select(g => new { GroupName = g.Key, Items = g }); // not finding any results
            foreach (var g in query)
            {
                var info = new GroupInfoList<Book>();
                info.Key = g.GroupName;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }
            CountResults(groups);

            return groups;
        }

        private void EditBook()
        {
            
        }

        private void CountResults(List<GroupInfoList<Book>> group)
        {
            var count = 0;
            foreach (var g in group)
            {
                count += g.Count;
            }
            FilterCount = count.ToString();
        }

        internal ObservableCollection<Book> FilterByTitleNotGrouped(string filterQuery)
        {
            var result = FilteredBooks.Where(x => x.Title.Contains(filterQuery)).ToList();
            return new ObservableCollection<Book>(result);
        }

        internal List<GroupInfoList<Book>> FilterByTitleGrouped(string filterQuery)
        {
            var groups = new List<GroupInfoList<Book>>();

            var query =
                AllBooks.OrderBy(item => item.Title)
                    .Where(item => item.Title.ToLower().Contains(filterQuery.ToLower()))
                    .GroupBy(item => item.Title[0])
                    .Select(g => new { GroupName = g.Key, Items = g });
            foreach (var g in query)
            {
                var info = new GroupInfoList<Book> { Key = g.GroupName };
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }
            CountResults(groups);
            return groups;
        }

        public void Grouped()
        {
            CollectionView.IsSourceGrouped = true;
            CollectionView.Source = GroupedBooks;
            RefreshBooks();
            NotifyPropertyChanged("CollectionView");
        }

        
        public void NotGrouped()
        {
            CollectionView.IsSourceGrouped = false;
            CollectionView.Source = AllBooks;
            RefreshBooks();
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
            GroupedBooks = GetGroupsByLetter();
            CountResults(GroupedBooks);
        }

        public  void UpdateBook(Book book)
        {
            _bookService.Update(book);
        }

        public Brush FilterColor => IsNullOrEmpty(FilterQuery) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Yellow);
    }
}