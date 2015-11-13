using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bookie.Converters
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class PageNumberConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return " of " + value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}