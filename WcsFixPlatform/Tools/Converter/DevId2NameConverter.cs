using resource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class DevId2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint devid)
            {
                return PubMaster.Device.GetDeviceName(devid);
            }
            if(value is ushort sdevid)
            {
                return PubMaster.Device.GetDeviceName((uint)sdevid);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}