using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Bookie.ViewModels;

namespace Bookie.Views
{
    public sealed partial class Shell : Page
    {

        private DispatcherTimer timer;

        public Shell()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;

            // Navigate to the first page (optionally)
            var type = (DataContext as ShellViewModel).Menu.First().NavigationDestination;
            SplitViewFrame.Navigate(type); // return MainPageView
            SplitViewFrame.DataContext = (DataContext as ShellViewModel).MainViewModel;


            ShellViewModel.ShowMessageEvent += ShellViewModel_ShowMessageEvent1;



        }

        private void ShellViewModel_ShowMessageEvent1(Common.EventArgs.BookieMessageEventArgs e)
        {
            ShowMessage(e.Message);
        }


        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            var menuItem = e.AddedItems.First() as MenuItem;
            if (menuItem.NavigationDestination == typeof (PdfPage) && ShellViewModel.SelectedBook == null)
            {
                return;
            }
            if (menuItem != null && menuItem.IsNavigation)
            {
                SplitViewFrame.Navigate(menuItem.NavigationDestination);
                SplitViewFrame.DataContext = menuItem.ViewModel;
            }
            else
            {
                if (menuItem != null) menuItem.Command.Execute(null);
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            var f = FlyoutBase.GetAttachedFlyout(SplitViewFrame);
            f.Hide();
            timer.Stop();
        }

        private void SplitViewOpener_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 50)
            {
                MySplitView.IsPaneOpen = true;
            }
        }

        private void SplitViewPane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < -50)
            {
                MySplitView.IsPaneOpen = false;
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        public void ShowMessage(string text)
        {
            flyoutText.Text = text;
            FlyoutBase.ShowAttachedFlyout(SplitViewFrame as FrameworkElement);
            timer.Start();
        }
    }
}