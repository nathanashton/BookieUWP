using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    public class FavouriteVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return (bool)value == false ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}