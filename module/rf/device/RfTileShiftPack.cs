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

            TileShift.Add(new RfTileGoodPack()
            {
                tile_id = dev.id,
                area = dev.area,
                type = dev.type,
                good_id = cfg.goods_id,
                oldgood_id = cfg.old_goodid,
                pregood_id = cfg.pre_goodid,
                shiftstatus = status
            });
        }
    }
}
