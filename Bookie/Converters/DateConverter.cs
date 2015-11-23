using System;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var date = (DateTime) value;
                return new DateTimeOffset(date);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var dto = (DateTimeOffset) value;
                return dto.DateTime;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}