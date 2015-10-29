using Bookie.Common;
using Bookie.Common.Model;
using Bookie.ViewModels;
using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bookie.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewmodel;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
            gview.Visibility = Visibility.Visible;
            lview.Visibility = Visibility.Collapsed;
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
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }

        private void HamburgerButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }

        private void MenuButton1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //   _viewModel.AddFolder();
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

        private void MenuButton5_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PdfPage));
        }

        private async void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var op = new FileOpenPicker();
            op.FileTypeFilter.Add("*");
            op.ViewMode = PickerViewMode.List;

            var file = await op.PickSingleFileAsync();

            if (file != null)

            {
                token = StorageApplicationPermissions.FutureAccessList.Add(file);
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["id"] = token;
            }
        }

        private async void TextBlock_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var t = localSettings.Values["id"].ToString();

            if (t != null)

            {
                StorageFile fileFromList = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(t);

                await (new Windows.UI.Popups.MessageDialog(fileFromList.DisplayName)).ShowAsync();
            }
        }

        private void AppBarButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //  _viewModel.AddFolder();
        }

        private void AppBarButton_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            gview.Visibility = Visibility.Visible;
            //   BooksCollectionViewSource.Source = viewmodel.AllBooks;
            // BooksCollectionViewSource.IsSourceGrouped = false;
            lview.Visibility = Visibility.Collapsed;
        }

        private void AppBarButton_Tapped_2(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            gview.Visibility = Visibility.Collapsed;

            lview.Visibility = Visibility.Collapsed;
        }

        private void AppBarButton_Tapped_3(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            gview.Visibility = Visibility.Collapsed;

            lview.Visibility = Visibility.Visible;
        }

        private void Button_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void Grid_OnRightTapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var t = (Book)(e.OriginalSource as Image).DataContext;
            viewmodel.SelectedBook = viewmodel.FilteredBooks.FirstOrDefault(x => x.Id == t.Id);

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void MessagingService_messages(object sender, BookieMessageEventArgs e)
        {
        }

        private void Grid_RightTapped_1(object sender, RightTappedRoutedEventArgs e)
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

        private void ComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gview.ItemsSource = BooksCollectionViewSource.View;
            lview.ItemsSource = BooksCollectionViewSource.View;
        }

        private void ComboBoxItem_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            gview.ItemsSource = BooksCollectionViewSourceNotgrouped.View;
            lview.ItemsSource = BooksCollectionViewSourceNotgrouped.View;
        }

        private void Grid_DoubleTapped_1(object sender, DoubleTappedRoutedEventArgs e)
        {
            var t = (Book)(e.OriginalSource as TextBlock).DataContext;
            viewmodel.SelectedBook = viewmodel.FilteredBooks.FirstOrDefault(x => x.Id == t.Id);
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
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
    }
}