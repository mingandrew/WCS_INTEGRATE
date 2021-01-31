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

        /// <summary>
        /// 判断库存是否在给定误差内
        /// </summary>
        /// <param name="stocksite">脉冲位置</param>
        /// <param name="difrange">误差范围</param>
        /// <returns></returns>
        public bool IsInLocation(ushort stocksite, ushort difrange)
        {
            if (location == 0 || stocksite ==0) return false;
            return Math.Abs(location - stocksite) <= difrange;
        }

        public override string ToString()
        {
            return string.Format("id:{0},good_id:{1},track_id:{2},produce_time:{3}," +
                "pos:{4},tile_id:{5},area:{6},track_type:{7}",id, goods_id, track_id, produce_time, pos,
                tilelifter_id, area, TrackType);
        }
    }
}
