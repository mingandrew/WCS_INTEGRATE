using resource;
using System;
using System.Globalization;
using System.Windows.Data;

namespace wcs.Tools.Converter
{
    public class TrackId2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is uint trackid)
            {
                return PubMaster.Track.GetTrackName(trackid);
            }

            if(value is ushort strackid)
            {
                return PubMaster.Track.GetTrackName(strackid);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
