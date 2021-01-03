using module.track;

namespace module.rf
{
    public class RfTrack
    {

        public uint id { set; get; }
        public string name { set; get; }
        public ushort area { set; get; }
        public byte type { set; get; }
        public byte stock_status { set; get; }//库存状态
        public byte track_status { set; get; }//轨道使用状态
        public string memo { set; get; }
        public short order { set; get; }

        public RfTrack()
        {

        }
        public RfTrack(Track track)
        {
            id = track.id;
            name = track.name;
            area = track.area;
            type = track.type;
            stock_status = track.stock_status;
            track_status = track.track_status;
            memo = track.memo;
            order = track.order;
        }
    }
}
