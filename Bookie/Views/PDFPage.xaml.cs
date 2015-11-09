using Bookie.Common;
using Bookie.Common.Model;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace Bookie.Views
{
    public sealed partial class PdfPage
    {
        private Dictionary<string, PdfLoadedBookmark> _bookmarksDictionary;

        private ViewModels.PdfViewModel _viewmodel;
        private DispatcherTimer timer;

        public PdfPage()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_viewmodel.CurrentPageNumber != 1 && _viewmodel.CurrentPageNumber != pdfViewer1.PageCount)
            {
                _viewmodel.SelectedBook.CurrentPage = _viewmodel.CurrentPageNumber;
                _viewmodel.UpdateBook();
            }
            else
            {
                _viewmodel.SelectedBook.CurrentPage = null;
                _viewmodel.UpdateBook();
            }
        }

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
                if (!_bookmarksDictionary.ContainsKey(bookmark.Title))
                {
                    _bookmarksDictionary.Add(bookmark.Title, bookmark);

                }

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

        private void ShowMessage(string text)
        {
            flyoutText.Text = text;
            FlyoutBase.ShowAttachedFlyout(cbar as FrameworkElement);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var f = FlyoutBase.GetAttachedFlyout(cbar);
            f.Hide();
            timer.Stop();
        }

        private void GetBookmarks()
        {
            var bookmarks = _viewmodel.Doc.Bookmarks;
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

       

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewmodel = DataContext as ViewModels.PdfViewModel;
            if (_viewmodel == null) return;
            _viewmodel.PdfControl = pdfViewer1;
            _viewmodel.LoadingFinished += _viewmodel_LoadingFinished;
            _viewmodel.LoadDefaultFile();
            SetPageNumber(1);
        }

        private void SetPageNumber(int pageNumber)
        {
            cpagenumber.Text = "Page: " + pageNumber;
        }

        private void _viewmodel_LoadingFinished(object sender, EventArgs e)
        {
            GetBookmarks();
        }

        private void pdfViewer1_PageChanged(object sender, Syncfusion.Windows.PdfViewer.PageChangedEventArgs e)
        {
            SetPageNumber(e.NewPageNumber);
            _viewmodel.CurrentPageNumber = e.NewPageNumber;
            _viewmodel.CheckPageForBookMark(e.NewPageNumber);
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem.ToString();
            var bookmark = _bookmarksDictionary[clickedItem];
            if (bookmark.Destination == null) return;
            var page = bookmark.Destination.Page;
            var pageIndex = 0;
            foreach (PdfPageBase pageBase in _viewmodel.Doc.Pages)
            {
                if (pageBase == page)
                {
                    pdfViewer1.GotoPage(pageIndex + 1);
                    break;
                }
                pageIndex++;
            }
        }

        private void AppBarButton_Tapped_3(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var added =_viewmodel.ToggleBookMark();
            if (added)
            {
                ShowMessage("BookMark Added: Page " + _viewmodel.CurrentPageNumber);
            }
            else
            {
                ShowMessage("BookMark Removed: Page " + _viewmodel.CurrentPageNumber);
            }

        }

        private void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            var bookMark = e.ClickedItem as BookMark;
            if (bookMark == null) return;
            pdfViewer1.GotoPage(bookMark.PageNumber);
        }

        private void TextBlock_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var tappedTextBlock = sender as TextBlock;
            if (tappedTextBlock == null) return;
            pdfViewer1.GotoPage(((BookMarkBase)tappedTextBlock.DataContext).PageNumber);
        }
    }
}