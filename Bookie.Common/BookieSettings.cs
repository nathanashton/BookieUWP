using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Bookie.Common
{
    public static class BookieSettings
    {
        public static List<Theme> Themes { get; set; }
        public static Theme Theme { get; set; }


        public static void LoadSettings()
        {
            // Load all available themes
            Themes = new List<Theme>();
            var blue = new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Blue.xaml") };
            var blueTheme = new Theme { Resource = blue };
            var black = new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Black.xaml") };
            var blackTheme = new Theme { Resource = black };
            var red = new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Red.xaml") };
            var redTheme = new Theme { Resource = red };
            Themes.Add(blueTheme);
            Themes.Add(blackTheme);
            Themes.Add(redTheme);

            // Load Theme
            var setTheme = new Theme();
            var localSettings = ApplicationData.Current.LocalSettings;
            var theme = localSettings.Values["Theme"];
            if (theme == null)
            {
                // Theme not set so default to Black
                var resource = new ResourceDictionary { Source = new System.Uri("ms-appx:///Themes/Black.xaml") };
                setTheme.Resource = resource;
            }

            else
            {
                var resource = new ResourceDictionary { Source = new System.Uri(theme.ToString()) };
                setTheme.Resource = resource;
            }
            Theme = setTheme;


        }

        public static void SaveSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var t = Theme;
            localSettings.Values["Theme"] = Theme.Resource.Source.ToString();
        }


    }
}
