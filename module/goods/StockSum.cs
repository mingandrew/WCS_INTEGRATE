using enums.track;
using System;

namespace module.goods
{
    public class StockSum
    {
        public uint goods_id { set; get; }
        public uint track_id { set; get; }
        public DateTime? produce_time { set; get; }
        public int count { set; get; }
        public int stack { set; get; }
        public int pieces { set; get; }
        public uint area { set; get; }
        public ushort line { set; get; }
        public byte track_type { set; get; }

        public int CompareProduceTime(DateTime? time)
        {
            if (produce_time is DateTime dtime && time is DateTime ctime)
            {
                return dtime.CompareTo(ctime);
            }
            return 0;
        }

        public TrackTypeE TrackType
        {
            get => (TrackTypeE)track_type;
        }
    }
}
