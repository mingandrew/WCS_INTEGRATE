using enums;
using resource;
using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class LevelValue2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte levelbyte )
            {
                return PubMaster.Dic.GetDtlStrCode(DicTag.TileLevel, DicTag.TileSite, levelbyte);
            }
            else  if (value is int levelint)
            {
                return PubMaster.Dic.GetDtlStrCode(DicTag.TileLevel, DicTag.TileSite, levelint);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
