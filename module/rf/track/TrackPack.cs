using module.track;
using System.Collections.Generic;

namespace module.rf
{
    public class TrackPack
    {
        public List<RfTrack> TrackList { set; get; }

        public void AddTrackList(List<Track> list)
        {
            if (TrackList == null)
            {
                TrackList = new List<RfTrack>();
            }
            foreach (Track track in list)
            {
                TrackList.Add(new RfTrack(track));
            }
        }
    }
}
