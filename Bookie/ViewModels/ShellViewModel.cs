using Bookie.Common.Model;
using Bookie.Views;
using Windows.UI.Xaml;

namespace Bookie.ViewModels
{
    internal class ShellViewModel : ViewModelBase
    {
        public MainPageViewModel MainViewModel { get; set; }
        public PdfViewModel PdfViewModel { get; set; }
        public SettingsPageViewModel SettingsViewModel { get; set; }

        public static Book SelectedBook { get; set; }

        public ShellViewModel()
        {
            MainViewModel = new MainPageViewModel();
            PdfViewModel = new PdfViewModel();
            SettingsViewModel = new SettingsPageViewModel();

            Menu.Add(new MenuItem() { Glyph = "", Text = "Library", NavigationDestination = typeof(MainPage), ViewModel = MainViewModel });
            Menu.Add(new MenuItem() { Glyph = "", Text = "Viewer", NavigationDestination = typeof(PdfPage), ViewModel = PdfViewModel });
            Menu.Add(new MenuItem() { Glyph = "", Text = "Settings", NavigationDestination = typeof(SettingsPage), ViewModel = SettingsViewModel });
        }

        public ElementTheme Theme
        {
            get
            {
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    return ElementTheme.Light;
                }
                else
                {
                    return ElementTheme.Dark;
                }
            }
        }
    }
}