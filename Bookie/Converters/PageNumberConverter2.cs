using System;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    public class PageNumberConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return " of " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}