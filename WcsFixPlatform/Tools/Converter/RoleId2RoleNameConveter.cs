using resource;
using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class RoleId2RoleNameConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int roleid)
            {
                return PubMaster.Role.GetRoleName(roleid);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
