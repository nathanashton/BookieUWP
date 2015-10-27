// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bookie.Views
{
    public sealed partial class PdfPage : Page
    {
        private ViewModels.PdfViewModel viewmodel;

        private int currentPage;

        public PdfPage()
        {
            InitializeComponent();
            var s = this.Width;
            var p = this.ActualWidth;
        }

        private void Lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewmodel.Start = (sender as ListView).SelectedIndex.ToString();
        }

        private void ScrollViewer_ViewChanged1(object sender, ScrollViewerViewChangedEventArgs e)
        {
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
           
        }

     

        private void EventHandlerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate)
            {
                var scrollViewer = sender as ScrollViewer;
                if (scrollViewer != null)
                {
                    viewmodel.V.UpdatePages(scrollViewer.ZoomFactor);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewmodel = DataContext as ViewModels.PdfViewModel;
            viewmodel.LoadDefaultFile();
            viewmodel.Notes = Visibility.Collapsed;
        }

        private void V_VectorChanged(Windows.UI.Xaml.Interop.IBindableObservableVector vector, object e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewmodel.Notes == Visibility.Collapsed)
            {
                viewmodel.Notes = Visibility.Visible;
            }
            else
            {
                viewmodel.Notes = Visibility.Collapsed;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var s = this.ActualWidth;
        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
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