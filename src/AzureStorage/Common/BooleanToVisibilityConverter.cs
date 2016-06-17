﻿/*
 *  Copyright © 2016, Russell Libby 
 */
 
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AzureStorage.Common
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>, or the reverse if the parameter is "Reverse".
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((value is bool && (bool)value) ^
                (parameter as string ?? string.Empty).Equals("Reverse")) ?
                Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible) ^
                (parameter as string ?? string.Empty).Equals("Reverse");
        }
    }
}
