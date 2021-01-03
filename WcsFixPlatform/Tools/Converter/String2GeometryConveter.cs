using HandyControl.Tools;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace wcs.Tools.Converter
{
    public class String2GeometryConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string str ? ResourceHelper.GetResource<Geometry>(str) : default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
