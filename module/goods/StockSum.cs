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
        public byte track_type2 { set; get; }
        public DateTime? last_produce_time { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        public byte sum_level { set; get; }

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
        
        public TrackType2E TrackType2
        {
            get => (TrackType2E)track_type2;
        }
        
        /// <summary>
        /// 判断品种和等级是否跟这个库存的信息相等
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="lev"></param>
        /// <returns></returns>
        public bool EqualGoodAndLevel(uint gid, byte lev)
        {
            return goods_id == gid && sum_level == lev;
        }
    }
}
