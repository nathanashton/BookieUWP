using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Bookie.Converters
{
    public class FoundForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var s = (bool) value;
            if (s)
            {
                return App.Current.Resources["LettersForegroundHighlighted"] as Brush;
            }
            return App.Current.Resources["LettersForeground"] as Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}