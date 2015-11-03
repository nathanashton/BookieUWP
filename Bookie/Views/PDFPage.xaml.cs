// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Bookie.Common.Model;
using PdfViewModel;
using System.Linq;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Windows.PdfViewer;

namespace Bookie.Views
{
    public sealed partial class PdfPage : Page
    {

        Dictionary<string, PdfLoadedBookmark> bookmarksDictionary;

        private ViewModels.PdfViewModel _viewmodel;

        public ViewModels.PdfViewModel ViewModel
        {
            get { return _viewmodel; }
        }


        private int _currentPage;

        public PdfPage()
        {
            InitializeComponent();
            var s = this.Width;
            var p = this.ActualWidth;
          
        }

      

        private void EventHandlerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
         
        }

        private void AddBookMark(PdfLoadedBookmark bookmark)
        {
            if (bookmark.Count == 0)
            {
                listView.Items.Add(bookmark.Title);
                bookmarksDictionary.Add(bookmark.Title, bookmark);
            }
            else
            {
                listView.Items.Add(bookmark.Title);
                bookmarksDictionary.Add(bookmark.Title, bookmark);
                // loop to retieve thw child elemts of the bookmark
                foreach (PdfLoadedBookmark value in bookmark)
                {
                    if (value.Count == 0)
                    {
                        //BookMark child items added with Parent Name
                        listView.Items.Add(bookmark.Title + " : " + value.Title);
                        bookmarksDictionary.Add(bookmark.Title + " : " + value.Title, value);
                    }
                    else
                    {
                        // Calls this method recursively to add all the bookmarks child
                        AddBookMark(value);
                    }
                }
            }
        }


        private void GetBookmarks()
        {
            //Gets the BookMark collection.
            PdfBookmarkBase bookmarks = _viewmodel.doc.Bookmarks;
            if (bookmarks != null && bookmarks.Count > 0)
                bookmarksDictionary = new Dictionary<string, PdfLoadedBookmark>();
            //Iterate through Pdf bookmarks and adds it in List view.
            foreach (PdfLoadedBookmark bookmark in bookmarks)
            {
                AddBookMark(bookmark);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewmodel = DataContext as ViewModels.PdfViewModel;
            _viewmodel.pdfControl = pdfViewer1;
            _viewmodel.LoadingFinished += _viewmodel_LoadingFinished;
            _viewmodel.LoadDefaultFile();
        }

        private void _viewmodel_LoadingFinished(object sender, EventArgs e)
        {
            GetBookmarks();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var s = this.ActualWidth;
        }

        private void AppBarButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
          
        }

        private void AppBarButton_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
          
        }

        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
          
        }

        private void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
           // lv.ScrollIntoView(_viewmodel.SelectedPage);
        }

        private void pdfViewer1_PageChanged(object sender, Syncfusion.Windows.PdfViewer.PageChangedEventArgs e)
        {
            cpagenumber.Text = "Vertical: " + pdfViewer1.VerticalOffset.ToString() + " Page: " + e.NewPageNumber.ToString();
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string clickedItem = e.ClickedItem.ToString();
            // Bookmark selected from the ListView
            PdfLoadedBookmark bookmark = bookmarksDictionary[clickedItem];
            // Destination page
            PdfPageBase page;
            // Checks the destion of the selected bookmark
            if (bookmark.Destination != null)
            {
                // assigns the destination page
                page = bookmark.Destination.Page;
                int pageIndex = 0;
                // Find the Page Number by comparing the destination page with all the pages 
                foreach (PdfPageBase pageBase in _viewmodel.doc.Pages)
                {
                    // checks whether the iterated page equals the destination page
                    if (pageBase == page)
                    {
                        // Goes to particular page using the Page Index
                        pdfViewer1.GotoPage(pageIndex + 1);
                        break;
                    }
                    pageIndex++;
                }
            }
        }

        private void AppBarButton_Tapped_2(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GetBookmarks();

        }

        private void AppBarButton_Tapped_3(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var bookMark = new BookMark();
            bookMark.PageNumber = pdfViewer1.PageNumber;
            _viewmodel.BookMarkPage(bookMark);
        }

        private void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            pdfViewer1.GotoPage((e.ClickedItem as BookMark).PageNumber
 );

        }
    }
}