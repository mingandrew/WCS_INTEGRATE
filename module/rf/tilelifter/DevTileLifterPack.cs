using System.Collections.Generic;

namespace module.rf
{
    public class DevTileLifterPack
    {
        public List<RfDevTileLifter> DevList { set; get; }
        public void AddDev(RfDevTileLifter tilelifter)
        {
            if(DevList == null)
            {
                DevList = new List<RfDevTileLifter>();
            }

            DevList.Add(tilelifter);
        }
    }
}
