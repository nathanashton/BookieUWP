using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Core;
using Bookie.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Bookie.Repository;
using static System.String;

namespace Bookie.ViewModels
{
    public class MainPageViewModel : NotifyBase, IProgressSubscriber
    {

        private BookRepository _repository = new BookRepository();


        private CollectionViewSource _collectionView;
        private ImportProgress importProgress;
        private List<GroupInfoList<Book>> _groupedbooks;
        private Book _selectedBook;
        private string _filterQuery;
        private RelayCommand _openBookCommand;
        private RelayCommand _updateCommand;
        private ObservableCollection<Book> _books;
        private List<Book> _b;

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

        public RelayCommand UpdateCommand
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    UpdateBooksFromSources();
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
            if (SelectedBook != null && SelectedBook.FileName != null)
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
                Books = FilterByTitleNotGrouped(value);
                NotifyPropertyChanged("Books");
                NotifyPropertyChanged("FilterColor");
                NotifyPropertyChanged("FilterQuery");
            }
        }

        public ObservableCollection<Book> Books
        {
            get { return _books; }
            set { _books = value; NotifyPropertyChanged("Books"); }
        }

        public List<Book> B
        {
            get { return _b; }
            set
            {
                _b = value;
                NotifyPropertyChanged("B");
            }
        }

        public MainPageViewModel()
        {
            CollectionView = new CollectionViewSource();
            B = new List<Book>();
            ProgressService.RegisterSubscriber(this);
            Grouped();
            RefreshBooks();




            //var b = new BookMark();
            //b.PageNumber = 35;
            //if (B.Count > 1)
            //{
            //    b.Book = B[0];
            //    var bookmark = new BookMarkRepository().Add(b);
            //    B[0].BookMarks = new List<BookMark>();
            //    B[0].BookMarks.Add(bookmark);
            //    _repository.Update(B[0]);
            //}





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

        private void UpdateBooksFromSources()
        {
            var importer = new Importer();

            importer.UpdateBooksFromSources();
            RefreshBooks();
        }

        private  void RefreshBooks()
        {

            B =  _repository.GetAll();



            Books = new ObservableCollection<Book>(B);
            GroupedBooks = GetGroupsByLetter();
            CountResults(GroupedBooks);

            //if (GroupedBooks != null && GroupedBooks[0][0] != null)
            //{
            //    SelectedBook  = (Book)GroupedBooks[0][0];
            //}
        }

        internal List<GroupInfoList<Book>> GetGroupsByLetter()
        {
            var groups = new List<GroupInfoList<Book>>();

            var query =
                B.OrderBy(item => item.Title.ToLower())
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
            var result = Books.Where(x => x.Title.Contains(filterQuery)).ToList();
            return new ObservableCollection<Book>(result);
        }

        internal List<GroupInfoList<Book>> FilterByTitleGrouped(string filterQuery)
        {
            var groups = new List<GroupInfoList<Book>>();

            var query =
                B.OrderBy(item => item.Title)
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
            CollectionView.Source = B;
            RefreshBooks();
        }

        public void _progress_ProgressChanged(object sender, ProgressWindowEventArgs e)
        {
            importProgress.ViewModel.Title = e.OperationName;
            importProgress.ViewModel.Operation = e.ProgressText;
            importProgress.ViewModel.Progress = e.ProgressPercentage;
        }

        public async void _progress_ProgressStarted(object sender, EventArgs e)
        {
            importProgress = new ImportProgress();
            await importProgress.ShowAsync();
        }

        public void _progress_ProgressCompleted(object sender, EventArgs e)
        {
            importProgress.Hide();
            RefreshBooks();
        }

        public Brush FilterColor
        {
            get {
                return IsNullOrEmpty(FilterQuery) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Yellow);
            }
        }
    }
}