using module.device;
using System.Collections.Generic;

namespace module.rf
{
    public class CheckBroTrackPack
    {
        public uint trackid { get; set; }
        public bool brotrack { get; set; }
        public List<RfDevTileLifter> tilelist { get; set; }

        public void AddTileLifter(RfDevTileLifter item)
        {
            if (tilelist == null)
            {
                tilelist = new List<RfDevTileLifter>();
            }
            tilelist.Add(item);
        }
    }
}
