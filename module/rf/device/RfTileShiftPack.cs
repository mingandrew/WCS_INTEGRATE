using enums;
using module.device;
using module.deviceconfig;
using System.Collections.Generic;

namespace module.rf.device
{
    public class RfTileShiftPack
    {
        public List<RfTileGoodPack> TileShift { set; get; }

        public void AddTileShift(Device dev, ConfigTileLifter cfg, TileShiftStatusE status)
        {
            if(TileShift == null)
            {
                TileShift = new List<RfTileGoodPack>();
            }
            if (dev.Type == DeviceTypeE.砖机)
            {
                TileShift.Add(new RfTileGoodPack()
                {
                    tile_id = dev.id,
                    area = dev.area,
                    type = (cfg.WorkMode == TileWorkModeE.上砖)?(byte)DeviceTypeE.上砖机:(byte)DeviceTypeE.下砖机,
                    good_id = cfg.goods_id,
                    oldgood_id = cfg.old_goodid,
                    pregood_id = cfg.pre_goodid,
                    shiftstatus = status
                });
            }
            else
            {
                TileShift.Add(new RfTileGoodPack()
                {
                    tile_id = dev.id,
                    area = dev.area,
                    type = dev.type,
                    good_id = cfg.goods_id,
                    oldgood_id = cfg.old_goodid,
                    pregood_id = cfg.pre_goodid,
                    shiftstatus = status,

                    pre_good_all = cfg.pre_good_all,
                    pre_good_qty = cfg.pre_good_qty,
                    now_good_all = cfg.now_good_all,
                    now_good_qty = cfg.now_good_qty,

                    last_shift_time = cfg.last_shift_time,
                });

            }
        }
    }
}
