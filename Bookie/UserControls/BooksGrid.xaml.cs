using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Bookie.UserControls
{
    public sealed partial class BooksGrid : UserControl
    {

        public MainPageViewModel viewmodel => (MainPageViewModel)Resources["ViewModel"];

        public BooksGrid()
        {
            this.InitializeComponent();
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
               //viewmodel.ShelfBrush = _shelfBrushColor;
              //  viewmodel.GridBrush = _gridBrushColor;
                e.Handled = true;
                return;
            }

            e.Data.Properties.TryGetValue("item", out sourceItem);
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            viewmodel.GridBrush = new SolidColorBrush(Colors.DarkOliveGreen);
        }


        private void gview_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;
         //   viewmodel.ShelfBrush = _shelfBrushColor;
          //  viewmodel.GridBrush = _gridBrushColor;
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

           // viewmodel.ShelfBrush = _shelfBrushColor;
           // viewmodel.GridBrush = _gridBrushColor;


            //Remove it from shelf
            var book = (Book)sourceItem;
            viewmodel.ShelfBooks.Remove(book);
            book.Shelf = false;


            var a = new BookEventArgs();
            a.Book = book;
            a.State = BookEventArgs.BookState.Updated;
            viewmodel.BookChanged(this, a);
            viewmodel.UpdateBook(book);
        }

        private void gview_DragLeave(object sender, DragEventArgs e)
        {
            viewmodel.BooksScroll = ScrollMode.Enabled;
          //  viewmodel.GridBrush = _gridBrushColor;

        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private void Grid_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {

        }
    }
}
