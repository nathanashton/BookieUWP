using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Bookie.Common;
using Bookie.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bookie.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly SettingsPageViewModel _viewmodel;

        public SettingsPage()
        {
            InitializeComponent();
            if (DataContext == null)
            {
                DataContext = new SettingsPageViewModel();
            }
            _viewmodel = DataContext as SettingsPageViewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            BookieSettings.SaveSettings();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewmodel.Load();
        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ResourceDictionary current = App.Current.Resources.MergedDictionaries.FirstOrDefault();
            if (current != null)
            {
                App.Current.Resources.MergedDictionaries.Remove(current);
            }
            var blue = new ResourceDictionary();
            blue.Source = new System.Uri("ms-appx:///Themes/Blue.xaml");
            App.Current.Resources.MergedDictionaries.Add(blue);


      
        }

        private void Button_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ResourceDictionary current = App.Current.Resources.MergedDictionaries.FirstOrDefault();
            var t = App.Current.Resources.MergedDictionaries.ToList();


            if (current != null)
            {
                App.Current.Resources.MergedDictionaries.Remove(current);
            }
            var blue = new ResourceDictionary();
            blue.Source = new System.Uri("ms-appx:///Themes/Black.xaml");
            App.Current.Resources.MergedDictionaries.Add(blue);


           
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var theme = ((sender as ComboBox).SelectedItem) as Theme;
            //if (theme == null) return;
            //if (_viewmodel == null) return;
            //_viewmodel.ChangeTheme(theme);
        }
    }
}