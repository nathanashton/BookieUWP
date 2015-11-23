using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bookie.Converters
{
    public class BoolImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var i = new ImageBrush
            {
                Opacity = 1,
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bookmark.png"))
            };
            var f = new ImageBrush
            {
                Opacity = 0.1,
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bookmark.png"))
            };
            var s = (bool) value;
            return s ? i : f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}