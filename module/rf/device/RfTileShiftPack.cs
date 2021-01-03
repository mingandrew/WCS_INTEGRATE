using enums;
using module.device;
using System.Collections.Generic;

namespace module.rf.device
{
    public class RfTileShiftPack
    {
        public List<RfTileGoodPack> TileShift { set; get; }

        public void AddTileShift(Device dev, TileShiftStatusE status)
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
                good_id = dev.goods_id,
                oldgood_id = dev.old_goodid,
                pregood_id = dev.pre_goodid,
                shiftstatus = status
            });
        }
    }
}
