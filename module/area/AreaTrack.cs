using enums.track;

namespace module.area
{
    public class AreaTrack
    {
        public uint id { set; get; }
        public uint area_id { set; get; }
        public uint track_id { set; get; }
        public byte track_type { set; get; }

        public TrackTypeE TrackType
        {
            get => (TrackTypeE)track_type;
            set => track_type = (byte)value;
        }
    }
}
