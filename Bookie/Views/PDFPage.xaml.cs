using Bookie.Common.Model;
using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static System.Int32;
using static System.String;

namespace Bookie.Views
{
    public sealed partial class PdfPage : Page
    {
        private readonly ViewModels.PdfViewModel _viewmodel;
        public ViewModels.PdfViewModel ViewModel => _viewmodel;

        public PdfPage()
        {
            InitializeComponent();
            if (DataContext == null)
            {
                DataContext = new ViewModels.PdfViewModel();
            } 

            _viewmodel = DataContext as ViewModels.PdfViewModel;
            ScrollViewer.Visibility = Visibility.Collapsed;
            SizeChanged += PdfPage_SizeChanged;
            _viewmodel.PdfLoadedEvent += _viewmodel_PdfLoadedEvent;
            _viewmodel.LoadDefaultFile();

        }

        private void PdfPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

        private void DetermineVisualState()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            switch (applicationView.Orientation)
            {
                case ApplicationViewOrientation.Landscape:
                    _viewmodel.PageOrientation = Orientation.Horizontal;
                    break;
                case ApplicationViewOrientation.Portrait:
                    _viewmodel.PageOrientation = Orientation.Vertical;
                    break;
            }
        }

        private ApplicationViewOrientation GetOrientation()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            return applicationView.Orientation;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_viewmodel.CurrentPage != 0 && _viewmodel.CurrentPage != 1 &&
                _viewmodel.CurrentPage != _viewmodel.PageCount)
            {
                _viewmodel.SelectedBook.CurrentPage = _viewmodel.CurrentPage;
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

        }

        private void _viewmodel_PdfLoadedEvent(object sender, EventArgs e)
        {
            ScrollViewer.Visibility = Visibility.Visible;

            var page = _viewmodel.SelectedBook.CurrentPage;

            if (page == null) return;
            var pageAsInt = (int)page;
            
            // Doesnt work as the PDF hasnt been loaded yet so the pagecount is null.
            var pageToffset = PageNumberToOffset(pageAsInt);
            ScrollToPage(pageToffset);
        }

        private Size GetScreenSize()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
            return size;
        }

        public double PageNumberToOffset(int pageNumber)
        {
            var size = GetScreenSize();
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    if ((int) ScrollViewer.ExtentWidth == 0)
                    {
                        double pp2 = (size.Width * _viewmodel.PageCount)/((double) _viewmodel.PageCount);
                        var result2 = pp2*(pageNumber) - pp2;
                        return result2;
                    }

                    double pp1 = ScrollViewer.ExtentWidth/_viewmodel.PageCount;
                    var result1 = pp1*(pageNumber) - pp1;
                    return result1;

                case ApplicationViewOrientation.Portrait:
                    // If Scrollviewer hasnt been loaded with pages yet
                    if ((int) ScrollViewer.ExtentHeight == 0)
                    {
                        double pp2 = ((size.Height - 72)*_viewmodel.PageCount)/((double) _viewmodel.PageCount);
                        var result2 = pp2*(pageNumber) - pp2;
                        return result2;
                    }

                    double pp = ScrollViewer.ExtentHeight/_viewmodel.PageCount;
                    var result = pp*(pageNumber) - pp;
                    return result;
            }

            return 1;
        }

        private int OffsetToPageNumber(double offset)
        {
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    var offsetToPage = (ScrollViewer.ExtentWidth / (_viewmodel.PageCount));
                    var pageNumber = offset / offsetToPage;
                    return Convert.ToInt32(pageNumber + 1);

                case ApplicationViewOrientation.Portrait:
                    var offsetToPage2 = (ScrollViewer.ExtentHeight/(_viewmodel.PageCount));
                    var pageNumber2 = offset / offsetToPage2;
                    return Convert.ToInt32(pageNumber2 + 1);
            }
            return 1;
        }

        public void ScrollToPage(double offset)
        {
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    ScrollViewer.ScrollToHorizontalOffset(offset);
                    break;

                case ApplicationViewOrientation.Portrait:
                    ScrollViewer.ScrollToVerticalOffset(offset);
                    break;
            }
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    _viewmodel.CurrentPage = OffsetToPageNumber(ScrollViewer.HorizontalOffset);
                    break;


                case ApplicationViewOrientation.Portrait:
                    _viewmodel.CurrentPage = OffsetToPageNumber(ScrollViewer.VerticalOffset);
                    break;
            }
        }

        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            //if (_viewmodel != null)
            //{
            //    ScrollToPage(PageNumberToOffset((int)_viewmodel.CurrentPage));
            //}
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var s = (BookMark) e.ClickedItem;
            if (s == null) return;
            ScrollToPage(PageNumberToOffset(s.PageNumber));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var text = textBox?.Text;
            if (IsNullOrEmpty(text)) return;
            int page;
            var result = TryParse(text, out page);
            if (result)
            {
                ScrollToPage(PageNumberToOffset(Convert.ToInt32(text)));
            }
        }
    }
}