using Bookie.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Bookie.Views
{
    public sealed partial class Shell : Page
    {
        public Shell()
        {
            this.InitializeComponent();

            // Navigate to the first page (optionally)
            var type = (DataContext as ShellViewModel).Menu.First().NavigationDestination;
            SplitViewFrame.Navigate(type); // return MainPageView
            SplitViewFrame.DataContext = (DataContext as ShellViewModel).MainViewModel;
        }

        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            var menuItem = e.AddedItems.First() as MenuItem;
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
    }
}