using Bookie.Common;
using Bookie.Common.Model;
using Bookie.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bookie.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private MainPageViewModel viewmodel;
        private Brush _shelfBrushColor = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
        private Brush _gridBrushColor = new SolidColorBrush(Color.FromArgb(255, 43, 43, 43));


        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
            booksGridView.Visibility = Visibility.Visible;
            MessagingService.Register(this, MessagingService_messages);
        }

        private string token = null;

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            DetermineVisualState();
            //    cflow.SelectedItemChanged += Cflow_SelectedItemChanged;

            viewmodel = DataContext as MainPageViewModel;
            viewmodel.ShelfVisibility = Visibility.Collapsed;
            UpdateLettersWidths();



        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
            UpdateLettersWidths();
        


        }


        private void UpdateLettersWidths()
        {
            var s = ActualWidth - 20;
            viewmodel.LetterWidth = s / 27;
        }

        private void DetermineVisualState()
        {
            //var state = string.Empty;
            //var applicationView = ApplicationView.GetForCurrentView();
            //var size = Window.Current.Bounds;

            //    if (applicationView.Orientation == ApplicationViewOrientation.Landscape)
            //        state = "Landscape";
            //    else
            //        state = "Portrait";

            //System.Diagnostics.Debug.WriteLine("Width: {0}, New VisulState: {1}",
            //    size.Width, state);

            //VisualStateManager.GoToState(this, state, true);
        }

        private void Page_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }





        private void AppBarButton_Tapped_3(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (viewmodel.ShelfVisibility == Visibility.Visible)
            {
                viewmodel.ShelfVisibility = Visibility.Collapsed;
            }
            else
            {
                viewmodel.ShelfVisibility = Visibility.Visible;
            }


        }

 

        private void MessagingService_messages(object sender, BookieMessageEventArgs e)
        {
        }


        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                Book t = (Book) (e.OriginalSource as Image).DataContext;
                viewmodel.SelectedBook = viewmodel.FilteredBooks.FirstOrDefault(x => x.Id == t.Id);
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }
            catch (Exception)
            {
                Book t = (Book)(e.OriginalSource as TextBlock).DataContext;
                viewmodel.SelectedBook = viewmodel.FilteredBooks.FirstOrDefault(x => x.Id == t.Id);
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }

        }


   
        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await EditPopup.ShowAsync();
        }

        private void EditPopup_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (cbox.IsChecked == false)
            {
                viewmodel.SelectedBook.DatePublished = null;
            }

            viewmodel.UpdateBook(viewmodel.SelectedBook);
            var a = new BookEventArgs();
            a.Book = viewmodel.SelectedBook;
            a.State = BookEventArgs.BookState.Updated;
            viewmodel.BookChanged(this, a);
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
            viewmodel.BooksScroll = ScrollMode.Disabled;

            // Drag over shelf
            object sourceItem;
            object s;

            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;

            e.Data.Properties.TryGetValue("gridSource", out s);
            // If it is being dragged back onto the shelf, do nothing an return
            if (sender == s)
            {
                viewmodel.ShelfBrush= _shelfBrushColor;
                e.Handled = true;
                return;
            }

            e.Data.Properties.TryGetValue("item", out sourceItem);
            var existsInShelf = viewmodel.ShelfBooks.FirstOrDefault(x => x.FullPathAndFileName == ((Book)sourceItem).FullPathAndFileName);
            if (existsInShelf != null)
            {
                // Item already exists on shelf
                viewmodel.ShelfBrush = new SolidColorBrush(Colors.Red);
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
                return;
            }
            // Doesnt exist on shelf so add, and show green highlight
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            viewmodel.ShelfBrush = new SolidColorBrush(Colors.DarkOliveGreen);

        }



        private void gview_DragEnter(object sender, DragEventArgs e)
        {
            viewmodel.BooksScroll = ScrollMode.Disabled;

            //Drag from shelf back to main
            object sourceItem;
            object s;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;
               
            e.Data.Properties.TryGetValue("gridSource", out s);
            // If it is being dragged back onto the shelf, do nothing an return
            if (sender == s)
            {
                viewmodel.ShelfBrush = _shelfBrushColor;
                viewmodel.GridBrush = _gridBrushColor;
                e.Handled = true;
                return;
            }

            e.Data.Properties.TryGetValue("item", out sourceItem);
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            viewmodel.GridBrush = new SolidColorBrush(Colors.DarkOliveGreen);
        }



        private void gview_DragLeave(object sender, DragEventArgs e)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;
            viewmodel.GridBrush = _gridBrushColor;

        }

        private void ggview_DragLeave(object sender, DragEventArgs e)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;
            viewmodel.ShelfBrush = _shelfBrushColor;

        }









        private void gview_Drop(object sender, DragEventArgs e)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;



            object gridSource;
            e.Data.Properties.TryGetValue("gridSource", out gridSource);

            if (gridSource == sender)
                return;

            object sourceItem;
            e.Data.Properties.TryGetValue("item", out sourceItem);
            if (sourceItem == null)
                return;

            viewmodel.ShelfBrush = _shelfBrushColor;
            viewmodel.GridBrush = _gridBrushColor;


            //Remove it from shelf
            var book = (Book) sourceItem;
            viewmodel.ShelfBooks.Remove(book);
            book.Shelf = false;


            var a = new BookEventArgs();
            a.Book = book;
            a.State = BookEventArgs.BookState.Updated;
            viewmodel.BookChanged(this, a);
            viewmodel.UpdateBook(book);
        }




        private void ggview_Drop(object sender, DragEventArgs e)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;

            object gridSource;
            e.Data.Properties.TryGetValue("gridSource", out gridSource);

            if (gridSource == sender)
                return;

            object sourceItem;
            e.Data.Properties.TryGetValue("item", out sourceItem);
            if (sourceItem == null)
                return;

            viewmodel.ShelfBrush = _shelfBrushColor;
            viewmodel.GridBrush = _gridBrushColor;

            var book = (Book)sourceItem;
            viewmodel.ShelfBooks.Add(book);
            book.Shelf = true;
            var a = new BookEventArgs();
            a.Book = book;
            a.State = BookEventArgs.BookState.Updated;
            viewmodel.BookChanged(this, a);
            viewmodel.UpdateBook(book);
        }

        private void gview_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;
            viewmodel.ShelfBrush = _shelfBrushColor;
            viewmodel.GridBrush = _gridBrushColor;
        }

        private void ggview_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;
           viewmodel.ShelfBrush = _shelfBrushColor;
            viewmodel.GridBrush = _gridBrushColor;

        }


        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {


            List<GridViewItem> it = new List<GridViewItem>();

            var t = booksGridView.Items;
            foreach (GridViewItem i in t)
            {
                if (i.Visibility == Visibility.Visible)
                {
                    it.Add(i);

                }
            }
            var tt = "s";

            var book = (Book)(sender as Grid).DataContext;
            viewmodel.SelectedBook = book;
            booksGridView.ScrollIntoView(viewmodel.SelectedBook);
        }
    }
}