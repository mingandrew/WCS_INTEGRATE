using module.area;
using module.device;
using System.Collections.Generic;

namespace module.rf
{
    public class TilePack
    {
        public List<Device> TileList { set; get; }

        public void AddTileList(List<Device> list)
        {
            if(TileList == null)
            {
                TileList = new List<Device>();
            }
            TileList.AddRange(list);
        }
    }
}
