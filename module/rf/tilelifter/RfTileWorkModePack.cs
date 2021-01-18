using module.rf.device;
using System.Collections.Generic;

namespace module.rf
{
    public class RfTileWorkModePack
    {
        public List<RfTileModePack> TileWorkMode { set; get; }

        public void AddDevice(RfTileModePack tile)
        {
            if(TileWorkMode == null)
            {
                TileWorkMode = new List<RfTileModePack>();
            }

            TileWorkMode.Add(tile);
        }
    }
}
