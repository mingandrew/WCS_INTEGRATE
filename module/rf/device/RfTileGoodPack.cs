using enums;
using System;
using System.Collections.Generic;

namespace module.rf.device
{
    public class RfTileGoodPack
    {
        public uint tile_id { set; get; }
        public byte type { set; get; }
        public ushort area { set; get; }
        public uint good_id { set; get; }
        public uint oldgood_id { set; get; }
        public uint pregood_id { set; get; }

        public int now_good_qty { set; get; }//当前品种数量
        public int pre_good_qty { set; get; }//预约品种数量
        public bool now_good_all { set; get; }//当前使用全部库存
        public bool pre_good_all { set; get; }//预约使用全部库存

        public int now_good_level { set; get; }//当前品种的等级
        public int pre_good_level { set; get; }//预设品种的等级

        public bool prior { get; set; }//急单
        public DateTime last_shift_time { set; get; }//上一次转产时间

        public TileShiftStatusE shiftstatus { set; get; }

        public List<uint> syn_tile_ids { set; get; } //同步 转产的砖机id
    }
}
