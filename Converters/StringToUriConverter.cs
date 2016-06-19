/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Windows.UI.Xaml.Data;

namespace AzureStorage.Converters
{
    /// <summary>
    /// Value converter that translates the string to a URI.
    /// </summary>
    public class StringToUriConverter : IValueConverter
    {
        /// <summary>
        /// Convert boolean value to Visibility type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting string).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (string.IsNullOrEmpty(value.ToString()) ? null : new Uri(value.ToString()));
        }

        /// <summary>
        /// Convert visibility value to boolean type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Boolean).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Uri) ? ((Uri)value).OriginalString : null;
        }
    }
}

