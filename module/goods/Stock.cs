using enums;
using enums.track;
using System;

namespace module.goods
{
    public class Stock
    {
        public uint id { set; get; }
        public uint goods_id { set; get; }
        public byte stack { set; get; }
        public ushort pieces { set; get; }
        public uint track_id { set; get; }
        public DateTime? produce_time{set;get;}
        public short pos { set; get; }
        public byte pos_type { set; get; }
        public uint tilelifter_id { set; get; }//库存来源下砖机
        public uint area { set; get; }
        public byte track_type { set; get; }

        /// <summary>
        /// 库存实际坐标（脉冲）
        /// </summary>
        public ushort location { set; get; }
        /// <summary>
        /// 库存计算坐标（脉冲）
        /// </summary>
        public ushort location_cal { set; get; }

        public StockPosE PosType
        {
            get => (StockPosE)pos_type;
            set => pos_type = (byte)value;
        }
        public TrackTypeE TrackType
        {
            get => (TrackTypeE)track_type;
            set => track_type = (byte)value;
        }
    }
}
