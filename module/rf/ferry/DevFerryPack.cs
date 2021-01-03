using module.device;
using System.Collections.Generic;

namespace module.rf
{
    public class DevFerryPack
    {
        public List<RfDevFerry> DevList { set; get; }
        public void AddDev(RfDevFerry ferry)
        {
            if(DevList == null)
            {
                DevList = new List<RfDevFerry>();
            }

            DevList.Add(ferry);
        }
    }
}
