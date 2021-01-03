using module.area;
using module.tiletrack;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace module.rf
{
    public class RfTileTrackPack
    {
        public uint TileId { set; get; }
        public List<TileTrack> TrackList { set; get; }
        public List<TileTrack> DeleteList { set; get; }
        public List<AreaDeviceTrack> AreaTrackList { set; get; }

        public void SetTrackList(List<TileTrack> list)
        {
            if(TrackList== null)
            {
                TrackList = new List<TileTrack>();
            }

            TrackList.AddRange(list);
        }

        public void SetAreaTrackList(List<AreaDeviceTrack> list)
        {
            if(AreaTrackList == null)
            {
                AreaTrackList = new List<AreaDeviceTrack>();
            }
            list.Sort((x, y) => x.track_id.CompareTo(y.track_id));

            AreaTrackList.AddRange(list);
        }


    }
}
