using resource;
using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class AreaId2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is uint araid)
            {
                return PubMaster.Area.GetAreaName(araid);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
