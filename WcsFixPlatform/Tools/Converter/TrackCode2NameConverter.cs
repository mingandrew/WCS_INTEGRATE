using resource;
using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class TrackCode2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string devidtrackcode)
            {
                string[] idcode = devidtrackcode.Split(':');
                if (idcode.Length > 1
                    && uint.TryParse(idcode[0], out uint devid)
                    && ushort.TryParse(idcode[1], out ushort trackcode))
                {
                    return PubMaster.Track.GetTrackName(devid, trackcode);
                }

            }else if (value is ushort trackcode)
            {
                return PubMaster.Track.GetTrackNameByCode(trackcode);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
