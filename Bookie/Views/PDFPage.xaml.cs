using Bookie.Common.Model;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Bookie.Views
{
    public sealed partial class PdfPage : Page
    {
        private ViewModels.PdfViewModel _viewmodel;
        public ViewModels.PdfViewModel ViewModel => _viewmodel;

        public PdfPage()
        {
            InitializeComponent();
            ScrollViewer.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_viewmodel.CurrentPage != 0 && _viewmodel.CurrentPage != 1 &&
                _viewmodel.CurrentPage != _viewmodel.PageCount)
            {
                _viewmodel.SelectedBook.CurrentPage = (int) _viewmodel.CurrentPage;
                _viewmodel.UpdateBook();
            }
            else
            {
                _viewmodel.SelectedBook.CurrentPage = null;
                _viewmodel.UpdateBook();
            }
        }

        private void EventHandlerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate) return;
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                _viewmodel.PdfPages.UpdatePages(scrollViewer.ZoomFactor);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewmodel = DataContext as ViewModels.PdfViewModel;
            _viewmodel.PdfLoadedEvent += _viewmodel_PdfLoadedEvent;
            _viewmodel.LoadDefaultFile();
        }



        private void _viewmodel_PdfLoadedEvent(object sender, EventArgs e)
        {
            ScrollViewer.Visibility = Visibility.Visible;

            var page = _viewmodel.SelectedBook.CurrentPage;

            if (page == null) return;
            var pageAsInt = (int)page;
            
            // Doesnt work as the ScrollViewer hasnt been populated yet. Needs a delay or something
            var pageToffset = PageNumberToOffset(pageAsInt);
            ScrollToPage(pageToffset);
        }

        private double PageNumberToOffset(int pageNumber)
        {
            if ((int) ScrollViewer.ExtentHeight == 0)
            {
                double pp2 = (921 * _viewmodel.PageCount) / ((double)_viewmodel.PageCount);
                var result2 = pp2 * (pageNumber) - pp2;
                return result2;
            }


            double pp = ScrollViewer.ExtentHeight/((double)_viewmodel.PageCount);
            var result = pp * (pageNumber) - pp;
            return result;
        }

        private int OffsetToPageNumber(double offset)
        {
            var to = (ScrollViewer.ExtentHeight / (_viewmodel.PageCount)); // how many points to a page
            var result = offset / to;
            return Convert.ToInt32(result +1);
        }

        private void ScrollToPage(double offset)
        {
            ScrollViewer.ScrollToVerticalOffset(offset);
           // ScrollViewer.ChangeView(null, offset, null);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
  
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            _viewmodel.CurrentPage = OffsetToPageNumber(ScrollViewer.VerticalOffset);
        }

        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            //if (_viewmodel != null)
            //{
            //    ScrollToPage(PageNumberToOffset((int)_viewmodel.CurrentPage));
            //}
        }

        private bool IsScrollViewerReady()
        {
            var ready = false;
            while (!ready)
            {
                if ((int)ScrollViewer.ExtentHeight == 0)
                {
                    ready = false;
                }
                else
                {
                    ready = true;
                }
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            }

            return true;
            
        }

        private void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //var t = sender as StackPanel;
            //var s = (BookMark) t.DataContext;

                        //ScrollToPage(PageNumberToOffset(s.PageNumber));

        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var s = (BookMark) e.ClickedItem;
            if (s == null) return;
            ScrollToPage(PageNumberToOffset(s.PageNumber));

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var t = (sender as TextBox).Text;
            if (!String.IsNullOrEmpty(t))
            {
                int page;
                var result = Int32.TryParse(t, out page);
                if (result)
                {
                    ScrollToPage(PageNumberToOffset(Convert.ToInt32(t)));
                }
            }

        }
    }
}