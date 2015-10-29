using System;
using Windows.UI;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bookie
{
    public class BookmarkToggle : ToggleButton
    {

        private ImageBrush i;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Checked += BookmarkToggle_Checked;
            Unchecked += BookmarkToggle_Unchecked;
            Tapped += BookmarkToggle_Tapped;
            i = new ImageBrush();
            i.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bookmark.png"));
            Background = i;

            if (IsChecked == true)
            {
                Background = i;

                Opacity = 1;

            }
            else
            {
                Background = i;

                Opacity = 0.1;

            }

        }

        private void BookmarkToggle_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
          
                if (IsChecked == true)
                {
                Background = i;

                Opacity = 0.9;
                }
                else
                {
                Background = i;

                Opacity = 0.1;

                }
            
        }

        private void BookmarkToggle_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (IsChecked == true)
            {
                Background = i;

                Opacity = 0.9;
            }
            else
            {
                Background = i;

                Opacity = 0.1;

            }
        }

        private void BookmarkToggle_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            if (IsChecked == true)
            {
                Background = i;

                Opacity = 0.9;
            }
            else
            {
                Background = i;

                Opacity = 0.1;

            }
        }
    }
}