﻿using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bookie.Common;
using Bookie.Common.EventArgs;
using Bookie.Common.Model;
using Bookie.ViewModels;


namespace Bookie.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Brush _gridBrushColor = new SolidColorBrush(Color.FromArgb(255, 43, 43, 43));
        private readonly Brush _shelfBrushColor = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

        private double _offsetPosition;

        private ScrollViewer _scroll;
        private int _columnCount = 8;

        private MainPageViewModel _viewmodel;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
            booksGridView.Visibility = Visibility.Visible;
            MessagingService.Register(this, MessagingService_messages);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["OffsetPosition"] = _offsetPosition;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            DetermineVisualState();
            if (DataContext == null)
            {
                _viewmodel = new MainPageViewModel();
            }
            _viewmodel = DataContext as MainPageViewModel;
            if (_viewmodel != null)
            {
                _viewmodel.ShelfVisibility = Visibility.Collapsed;
            }
            UpdateLettersWidths();
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
            UpdateLettersWidths();
        }

        private void UpdateLettersWidths()
        {
            if (_viewmodel == null) return;
            if (ActualWidth > 20)
            {
                var s = ActualWidth - 20;
                _viewmodel.LetterWidth = s/27;
            }
            else
            {
                // We are in the Viewer
                var s = 20;
                _viewmodel.LetterWidth = s/27;
            }
        }

        private void DetermineVisualState()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            _columnCount = applicationView.Orientation == ApplicationViewOrientation.Landscape ? 8 : 4;
            UpdateLettersWidths();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLettersWidths();
        }

        private void AppBarButton_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            ShellViewModel.ShowMessageDialog("Test message");
            _viewmodel.ShelfHeight = _viewmodel.ShelfHeight == 0 ? 198 : 0;
        }

        private void MessagingService_messages(object sender, BookieMessageEventArgs e)
        {
            throw new NotImplementedException("Messaging Service");
        }

        private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _viewmodel.BookDetailsVisibility = Visibility.Visible;
            _viewmodel.EditBookVisibility = Visibility.Collapsed;
            await BookDetailsPopup.ShowAsync();
        }

        private void EditPopup_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (cbox.IsChecked == false)
            {
                _viewmodel.SelectedBook.DatePublished = null;
            }

            _viewmodel.UpdateBook(_viewmodel.SelectedBook);
            var a = new BookEventArgs
            {
                Book = _viewmodel.SelectedBook,
                State = BookEventArgs.BookState.Updated
            };
            _viewmodel.BookChanged(this, a);
            tbutton.IsChecked = false;
        }

        private void GridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var item = e.Items.FirstOrDefault();
            if (item == null)
            {
                e.Cancel = true;
                return;
            }
            e.Data.Properties.Add("item", item);
            e.Data.Properties.Add("gridSource", sender);
        }

        private void gview_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var item = e.Items.FirstOrDefault() as Book;
            if (item == null)
            {
                e.Cancel = true;
                return;
            }
            e.Data.Properties.Add("item", item);
            e.Data.Properties.Add("gridSource", sender);
        }

        private void ggview_DragEnter_1(object sender, DragEventArgs e)
        {
            DisableScroll();
            // Drag over shelf
            object sourceItem;
            object s;

            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;

            e.Data.Properties.TryGetValue("gridSource", out s);
            // If it is being dragged back onto the shelf, do nothing an return
            if (sender == s)
            {
                _viewmodel.ShelfBrush = _shelfBrushColor;
                e.Handled = true;
                return;
            }

            e.Data.Properties.TryGetValue("item", out sourceItem);
            var existsInShelf =
                _viewmodel.ShelfBooks.FirstOrDefault(
                    x => x.FullPathAndFileName == ((Book) sourceItem).FullPathAndFileName);
            if (existsInShelf != null)
            {
                // Item already exists on shelf
                _viewmodel.ShelfBrush = new SolidColorBrush(Colors.Red);
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }
            // Doesnt exist on shelf so add, and show green highlight
            e.AcceptedOperation = DataPackageOperation.Copy;
            _viewmodel.ShelfBrush = new SolidColorBrush(Colors.DarkOliveGreen);
        }

        private void gview_DragEnter(object sender, DragEventArgs e)
        {
            DisableScroll();
            //Drag from shelf back to main
            object sourceItem;
            object s;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;

            e.Data.Properties.TryGetValue("gridSource", out s);
            // If it is being dragged back onto the shelf, do nothing an return
            if (sender == s)
            {
                _viewmodel.ShelfBrush = _shelfBrushColor;
                _viewmodel.GridBrush = _gridBrushColor;
                e.Handled = true;
                return;
            }

            e.Data.Properties.TryGetValue("item", out sourceItem);
            e.AcceptedOperation = DataPackageOperation.Move;
            _viewmodel.GridBrush = new SolidColorBrush(Colors.DarkOliveGreen);
        }

        private void DisableScroll()
        {
            _viewmodel.BooksScroll = ScrollMode.Disabled;
        }

        private void EnableScroll()
        {
            _viewmodel.BooksScroll = ScrollMode.Enabled;
        }

        private void gview_DragLeave(object sender, DragEventArgs e)
        {
            _viewmodel.BooksScroll = ScrollMode.Enabled;
            _viewmodel.GridBrush = _gridBrushColor;
        }

        private void ggview_DragLeave(object sender, DragEventArgs e)
        {
            _viewmodel.BooksScroll = ScrollMode.Enabled;
            _viewmodel.ShelfBrush = _shelfBrushColor;
        }

        private void gview_Drop(object sender, DragEventArgs e)
        {
            EnableScroll();
            object gridSource;
            e.Data.Properties.TryGetValue("gridSource", out gridSource);

            if (gridSource == sender)
                return;

            object sourceItem;
            e.Data.Properties.TryGetValue("item", out sourceItem);
            if (sourceItem == null)
                return;

            _viewmodel.ShelfBrush = _shelfBrushColor;
            _viewmodel.GridBrush = _gridBrushColor;

            //Remove it from shelf
            var book = (Book) sourceItem;
            _viewmodel.ShelfBooks.Remove(book);
            book.Shelf = false;

            var a = new BookEventArgs
            {
                Book = book,
                State = BookEventArgs.BookState.Updated
            };
            _viewmodel.BookChanged(this, a);
            _viewmodel.UpdateBook(book);
            ShellViewModel.ShowMessage("Removed from shelf", null);
        }

        private void ggview_Drop(object sender, DragEventArgs e)
        {
            EnableScroll();
            object gridSource;
            e.Data.Properties.TryGetValue("gridSource", out gridSource);

            if (gridSource == sender)
                return;

            object sourceItem;
            e.Data.Properties.TryGetValue("item", out sourceItem);
            if (sourceItem == null)
                return;

            _viewmodel.ShelfBrush = _shelfBrushColor;
            _viewmodel.GridBrush = _gridBrushColor;

            var book = (Book) sourceItem;
            _viewmodel.ShelfBooks.Add(book);
            book.Shelf = true;
            var a = new BookEventArgs
            {
                Book = book,
                State = BookEventArgs.BookState.Updated
            };
            _viewmodel.BookChanged(this, a);
            _viewmodel.UpdateBook(book);
            ShellViewModel.ShowMessage("Added to shelf", null);
        }

        private void gview_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            EnableScroll();
            _viewmodel.ShelfBrush = _shelfBrushColor;
            _viewmodel.GridBrush = _gridBrushColor;
        }

        private void ggview_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            EnableScroll();
            _viewmodel.ShelfBrush = _shelfBrushColor;
            _viewmodel.GridBrush = _gridBrushColor;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid == null) return;
            var book = (Book) grid.DataContext;
            _viewmodel.SelectedBook = book;
            booksGridView.ScrollIntoView(_viewmodel.SelectedBook);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (booksGridView.Items == null) return;
            _scroll = (ScrollViewer) sender;
            var offset = _scroll.VerticalOffset;
            offset -= 1;

            _offsetPosition = offset;

            var index = ((int) offset - 1)*_columnCount;
            var item = booksGridView.Items[index];
            if (item == null) return;
            var titleLetterArray = (item as Book).Title.ToCharArray();
            var letterLowerCase = titleLetterArray[0].ToString().ToLower();
            _viewmodel?.SelectLetter(letterLowerCase);
        }

        private void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid == null) return; 
            var letter = grid.DataContext as Letter;
            if (letter == null) return;
            var lower = letter.Name.ToLower();
            var found =
                _viewmodel.FilteredBooks.FirstOrDefault(x => x.Title.ToCharArray()[0].ToString().ToLower() == lower);
            if (found == null) return;
            _viewmodel.SelectLetter(lower);
            booksGridView.ScrollIntoView(found);
        }

        private void Flyout_Closed(object sender, object e)
        {
            _viewmodel.Filter();
        }  

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Shell.GoToViewer();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            _viewmodel.BookDetailsVisibility = Visibility.Collapsed;
            _viewmodel.EditBookVisibility = Visibility.Visible;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewmodel.BookDetailsVisibility = Visibility.Visible;
            _viewmodel.EditBookVisibility = Visibility.Collapsed;
        }

        private void BookDetailsPopup_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            tbutton.IsChecked = false;
            args.Cancel = false;
        }
    }
}