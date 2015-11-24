using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    public class InvertBoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var v = (bool) value;
            if (v)
            {
                return 1;
            }
            else
            {
                return 0.1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}