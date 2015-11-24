using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Bookie.Common.Model;

namespace Bookie.Converters
{
    public class HasBookmarksVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var book = (List<BookMark>)value;
            if (book != null && book.Count > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}