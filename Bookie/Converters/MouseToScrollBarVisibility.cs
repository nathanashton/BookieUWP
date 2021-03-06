﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Bookie.Converters
{
    internal sealed class MouseOverToScrollBarVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return (bool) value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotSupportedException();
        }
    }
}