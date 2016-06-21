/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using System;
using Windows.UI.Xaml.Data;

namespace AzureStorage.Converters
{
    /// <summary>
    /// Value converter that translates the ContainerType enum to a string.
    /// </summary>
    public class ContainerTypeToStringConverter : IValueConverter
    {
        #region Private consts

        private const string BlobContainer = "azure blob container";
        private const string Queue = "azure queue";
        private const string Table = "azure table";

        #endregion

        /// <summary>
        /// Convert container type to string type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting string).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ContainerType)
            {
                switch ((ContainerType)value)
                {
                    case ContainerType.BlobContainer:
                        return BlobContainer;

                    case ContainerType.Queue:
                        return Queue;

                    case ContainerType.Table:
                        return Table;
                }
            }

            return string.Empty;
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
            var text = value.ToString();

            if (text.Equals(BlobContainer)) return ContainerType.BlobContainer;
            if (text.Equals(Queue)) return ContainerType.Queue;

            return ContainerType.Table;
        }
    }
}

