using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This converter converts a null value to be collapsed visibility and a non-null to be visible.
    /// </summary>
    public class NullToInverseVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert a null value to collapsed and non-null to visible.
        /// </summary>
        /// <returns>
        /// A value of true if false, otherwise false.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        /// <returns>
        /// A value of true if false, otherwise false.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
