using resource;
using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class GoodId2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint goodid)
            {
                return PubMaster.Goods.GetGoodsName(goodid);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
