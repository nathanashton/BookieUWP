// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bookie.Views
{
    public sealed partial class PdfPage : Page
    {
        private ViewModels.PdfViewModel _viewmodel;

        private int _currentPage;

        public PdfPage()
        {
            InitializeComponent();
            var s = this.Width;
            var p = this.ActualWidth;
        }

        private void EventHandlerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate)
            {
                var scrollViewer = sender as ScrollViewer;
                if (scrollViewer != null)
                {
                    _viewmodel.V.UpdatePages(scrollViewer.ZoomFactor);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewmodel = DataContext as ViewModels.PdfViewModel;
            _viewmodel.LoadDefaultFile();
            _viewmodel.Notes = Visibility.Collapsed;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var s = this.ActualWidth;
        }

        private void AppBarButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var factor = Convert.ToSingle(ScrollViewer.ZoomFactor + 0.4);

            ScrollViewer.ChangeView(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset, factor);
        }

        private void AppBarButton_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var factor = Convert.ToSingle(ScrollViewer.ZoomFactor - 0.4);

            ScrollViewer.ChangeView(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset, factor);
        }
    }
}