using enums;
using enums.track;
using System;
using System.Linq;

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
        public uint last_track_id { set; get; }//库存上一个轨道ID

        /// <summary>
        /// 库存实际坐标（脉冲）
        /// </summary>
        public ushort location { set; get; }

        /// <summary>
        /// 急单类型
        /// </summary>
        public byte prior_num { set; get; }

        /// <summary>
        /// 库存计算坐标（脉冲）
        /// </summary>
        public ushort location_cal { set; get; }

        /// <summary>
        /// 砖机设定等级
        /// </summary>
        public byte level { set; get; }

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

        /// <summary>
        /// 判断库存是否在给定范围内
        /// </summary>
        /// <param name="point"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool IsWithinRange(ushort min, ushort max)
        {
            if (location == 0) return false;
            return min < location && location < max;
        }

        public bool InTrack(params uint[] tracks)
        {
            return tracks.Contains(track_id);
        }

        public override string ToString()
        {
            return string.Format("标识[ {0} ]，品种[ {1}^{9} ]，轨道[ {2}^{3} ]，生产[ {4} ]，" +
                "位置[ {5}^{6} ]，实际[ {7} ]，计算[ {8} ]", id, goods_id, TrackType, track_id, produce_time,
                PosType, pos, location, location_cal, level);
        }

        public string ToSmalString()
        {
            return string.Format("标识[ {0} ]，轨道[ {1}^{2} ]，位置[ {3}^{4} ]", id, TrackType, track_id,
                PosType, pos);
        }

        /// <summary>
        /// 是否为相同的品种&等级
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="lvl"></param>
        /// <returns></returns>
        public bool EqualGoodAndLevel(uint gid, byte lvl)
        {
            return goods_id == gid && level == lvl;
        }
    }
}
