using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class BooleanToSpaceVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bv)
            {
                return bv ? Visibility.Visible: Visibility.Hidden;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
