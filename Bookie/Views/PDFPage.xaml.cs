using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Bookie.Common.Model;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using static System.Int32;
using static System.String;

namespace Bookie.Views
{
    public sealed partial class PdfPage : Page
    {
        private Dictionary<string, PdfLoadedBookmark> _bookmarksDictionary;

        public PdfPage()
        {
            InitializeComponent();
            if (DataContext == null)
            {
                DataContext = new ViewModels.PdfViewModel();
            }
            ScrollViewer.Visibility = Visibility.Visible;

            ViewModel = DataContext as ViewModels.PdfViewModel;
            ScrollViewer.Visibility = Visibility.Collapsed;
            SizeChanged += PdfPage_SizeChanged;
            ViewModel.PdfLoadedEvent += _viewmodel_PdfLoadedEvent;
            ViewModel.LoadDefaultFile();
        }

        public ViewModels.PdfViewModel ViewModel { get; }

        private void AddBookMark(PdfLoadedBookmark bookmark)
        {
            if (bookmark.Count == 0)
            {
                listView.Items?.Add(bookmark.Title);
                _bookmarksDictionary.Add(bookmark.Title, bookmark);
            }
            else
            {
                if (listView.Items == null) return;
                listView.Items.Add(bookmark.Title);
                _bookmarksDictionary.Add(bookmark.Title, bookmark);
                foreach (PdfLoadedBookmark value in bookmark)
                {
                    if (value.Count == 0)
                    {
                        listView.Items.Add(bookmark.Title + " : " + value.Title);
                        _bookmarksDictionary.Add(bookmark.Title + " : " + value.Title, value);
                    }
                    else
                    {
                        AddBookMark(value);
                    }
                }
            }
        }

        private void GetBookmarks()
        {
            var bookmarks = ViewModel.doc.Bookmarks;
            if (bookmarks != null && bookmarks.Count > 0)
            {
                _bookmarksDictionary = new Dictionary<string, PdfLoadedBookmark>();
            }
            if (bookmarks != null)
                foreach (PdfLoadedBookmark bookmark in bookmarks)
                {
                    AddBookMark(bookmark);
                }
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
                    ViewModel.PageOrientation = Orientation.Horizontal;
                    break;
                case ApplicationViewOrientation.Portrait:
                    ViewModel.PageOrientation = Orientation.Vertical;
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
            if (ViewModel.SelectedBook == null) return;
            if (ViewModel.CurrentPage != 0 && ViewModel.CurrentPage != 1 &&
                ViewModel.CurrentPage != ViewModel.PageCount)
            {
                ViewModel.SelectedBook.CurrentPage = ViewModel.CurrentPage;
                ViewModel.UpdateBook();
            }
            else
            {
                ViewModel.SelectedBook.CurrentPage = null;
                ViewModel.UpdateBook();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.FullScreen = false;
        }

        private void EventHandlerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate) return;
            var scrollViewer = sender as ScrollViewer;
            if
                (scrollViewer != null)
            {
                ViewModel.PdfPages.UpdatePages(scrollViewer.ZoomFactor);
            }
        }


        private void _viewmodel_PdfLoadedEvent(object sender, EventArgs e)
        {
            ScrollViewer.Visibility = Visibility.Visible;
            GetBookmarks();
        }

        private Size GetScreenSize()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width*scaleFactor, bounds.Height*scaleFactor);
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
                        var pp2 = size.Width*ViewModel.PageCount/ViewModel.PageCount;
                        var result2 = pp2*pageNumber - pp2;
                        return result2;
                    }

                    var pp1 = ScrollViewer.ExtentWidth/ViewModel.PageCount;
                    var result1 = pp1*pageNumber - pp1;
                    return result1;

                case ApplicationViewOrientation.Portrait:
                    // If Scrollviewer hasnt been loaded with pages yet
                    if ((int) ScrollViewer.ExtentHeight == 0)
                    {
                        var pp2 = (size.Height - 72)*ViewModel.PageCount/ViewModel.PageCount;
                        var result2 = pp2*pageNumber - pp2;
                        return result2;
                    }

                    var pp = ScrollViewer.ExtentHeight/ViewModel.PageCount;
                    var result = pp*pageNumber - pp;
                    return result;
            }

            return 1;
        }

        private int OffsetToPageNumber(double offset)
        {
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    var offsetToPage = ScrollViewer.ExtentWidth/ViewModel.PageCount;
                    var pageNumber = offset/offsetToPage;
                    return Convert.ToInt32(pageNumber + 1);

                case ApplicationViewOrientation.Portrait:
                    var offsetToPage2 = ScrollViewer.ExtentHeight/ViewModel.PageCount;
                    var pageNumber2 = offset/offsetToPage2;
                    return Convert.ToInt32(pageNumber2 + 1);
            }
            return 1;
        }

        public void ScrollToPage(double offset)
        {
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    //  ScrollViewer.ScrollToHorizontalOffset(offset);

                    ScrollViewer.ChangeView(offset, ScrollViewer.VerticalOffset, ScrollViewer.ZoomFactor);
                    break;

                case ApplicationViewOrientation.Portrait:
                    //  ScrollViewer.ScrollToVerticalOffset(offset);
                    ScrollViewer.ChangeView(ScrollViewer.HorizontalOffset, offset, ScrollViewer.ZoomFactor);

                    break;
            }
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            switch (GetOrientation())
            {
                case ApplicationViewOrientation.Landscape:
                    ViewModel.CurrentPage = OffsetToPageNumber(ScrollViewer.HorizontalOffset);
                    break;


                case ApplicationViewOrientation.Portrait:
                    ViewModel.CurrentPage = OffsetToPageNumber(ScrollViewer.VerticalOffset);
                    break;
            }
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
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

        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var s = "t";
        }

        private void listView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem.ToString();


            var bookmark = _bookmarksDictionary[clickedItem];
            if (bookmark.Destination == null) return;
            var page = bookmark.Destination.Page;
            var pageIndex = 0;


            foreach (PdfPageBase pageBase in ViewModel.doc.Pages)
            {
                if (pageBase == page)
                {
                    var pageNumber = pageIndex + 1;
                    ScrollToPage(PageNumberToOffset(pageNumber));
                    break;
                }
                pageIndex++;
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.BookMarksVisibility = Visibility.Collapsed;
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.FullScreen)
            {
                ViewModel.FullScreen = false;
            }
            else
            { ViewModel.FullScreen = true; }


        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
          
            var text = JumpToPage?.Text;
            if (IsNullOrEmpty(text)) return;
            int page;
            var result = TryParse(text, out page);
            if (result)
            {
                ScrollToPage(PageNumberToOffset(Convert.ToInt32(text)));
            }
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var s = "t";
        }
    }
}