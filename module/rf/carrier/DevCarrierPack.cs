using System.Collections.Generic;

namespace module.rf
{
    public class DevCarrierPack
    {
        public List<RfDevCarrier> DevList { set; get; }
        public void AddDev(RfDevCarrier carrier)
        {
            if(DevList == null)
            {
                DevList = new List<RfDevCarrier>();
            }

            DevList.Add(carrier);
        }
    }
}
