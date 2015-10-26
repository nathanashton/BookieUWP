namespace Bookie.Converters
{
    using System;
    using Windows.UI.Xaml.Data;

    public class StarRatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (String.IsNullOrEmpty(value.ToString()) || value.ToString() == "0")
            {
                return "";
            }
            if (value.ToString() == "1")
            {
                return "\uE1CF";
            }
            if (value.ToString() == "2")
            {
                return "\uE1CF" + " " + "\uE1CF";
            }
            if (value.ToString() == "3")
            {
                return "\uE1CF" + " " + "\uE1CF" + " " + "\uE1CF";
            }
            if (value.ToString() == "4")
            {
                return "\uE1CF" + " " + "\uE1CF" + " " + "\uE1CF" + " " + "\uE1CF";
            }
            if (value.ToString() == "5")
            {
                return "\uE1CF" + " " + "\uE1CF" + " " + "\uE1CF" + " " + "\uE1CF" + " " + "\uE1CF";
            }

            return "\uE1CE" + "\uE1CE" + "\uE1CE" + "\uE1CE" + "\uE1CE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}