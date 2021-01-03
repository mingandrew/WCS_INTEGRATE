using enums.track;
using System;

namespace module.track
{
    public class TrackLog
    {
        public uint id { set; get; }
        public uint track_id { set; get; }
        public byte type { set; get; }
        public uint dev_id { set; get; }
        public ushort stock_count { set; get; }
        public DateTime? log_time { set; get; }
        public string memo { set; get; }
        public ushort area { set; get; }

        public TrackLogE Type
        {
            get => (TrackLogE)type;
            set => type = (byte)value;
        }
    }
}
