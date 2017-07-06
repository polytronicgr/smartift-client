using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This converter converts a boolean value to the opposite of what a BooleanToVisibilityConverter would.
    /// </summary>
    public class BoolToInverseVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert a value from Boolean to the inverse of itself.
        /// </summary>
        /// <returns>
        /// A value of true if false, otherwise false.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
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