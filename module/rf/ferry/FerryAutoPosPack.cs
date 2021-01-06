using enums;

namespace module.rf
{
    public class FerryAutoPosPack
    {
        public uint DevId { get; set; }
        public int StartTrack { get; set; }
        public int TrackQty { get; set; }
        public DevFerryAutoPosE PosSide { get; set; }


    }
}
