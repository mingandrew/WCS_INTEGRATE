using module.area;
using module.goods;
using module.tiletrack;
using module.track;
using System.Collections.Generic;

namespace module.rf
{
    public class RfTileTrackPack
    {
        public uint TileId { set; get; }
        public List<TileTrack> TrackList { set; get; }
        public List<TileTrack> DeleteList { set; get; }
        public List<AreaDeviceTrack> AreaTrackList { set; get; }
        public List<StockSum> StockSumList { set; get; }
        public List<RfTrack> TileTrackStatusList { set; get; }


        public void SetTrackList(List<TileTrack> list)
        {
            if (TrackList == null)
            {
                TrackList = new List<TileTrack>();
            }

            TrackList.AddRange(list);
        }

        public void SetAreaTrackList(List<AreaDeviceTrack> list)
        {
            if (AreaTrackList == null)
            {
                AreaTrackList = new List<AreaDeviceTrack>();
            }
            list.Sort((x, y) => x.track_id.CompareTo(y.track_id));

            AreaTrackList.AddRange(list);
        }

        public void SetStockSumList(List<StockSum> list)
        {
            if (StockSumList == null)
            {
                StockSumList = new List<StockSum>();
            }
            StockSumList.AddRange(list);
        }

        public void SetTileTrackStatus(List<Track> list)
        {
            if (TileTrackStatusList == null)
            {
                TileTrackStatusList = new List<RfTrack>();
            }

            foreach (Track track in list)
            {
                TileTrackStatusList.Add(new RfTrack(track));
            }
        }
    }
}
