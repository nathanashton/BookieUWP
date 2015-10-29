﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    public class NullableValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              string culture)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  string culture)
        {
            string s = value as string;

            int result;
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out result))
            {
                return result;
            }

            return null;
        }
    }
}
