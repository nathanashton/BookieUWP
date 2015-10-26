using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bookie
{
    public class BookmarkToggle : ToggleButton
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Checked += BookmarkToggle_Checked;
            Unchecked += BookmarkToggle_Unchecked;
            Tapped += BookmarkToggle_Tapped;
        }

        private void BookmarkToggle_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (IsChecked == true)
            {
                if (IsChecked == true)
                {
                    var i = new ImageBrush();
                    i.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bookmark.png"));
                    Background = i;
                }
                else
                {
                    Background = new SolidColorBrush(Colors.Transparent);
                }
            } }

        private void BookmarkToggle_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            
        }

        private void BookmarkToggle_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

      
          

        }
    }
}
