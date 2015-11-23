using System;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    public class NullBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            string culture)
        {
            if (value == null)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string culture)
        {
            var s = (bool) value;
            if (!s)
            {
                return null;
            }
            return value;
        }
    }
}