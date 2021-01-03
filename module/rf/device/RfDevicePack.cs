using module.device;
using System.Collections.Generic;

namespace module.rf.device
{
    public class RfDevicePack
    {
        public List<Device> DevList { set; get; }

        public void AddDevs(List<Device> list)
        {
            if(DevList == null)
            {
                DevList = new List<Device>();
            }

            DevList.AddRange(list);
        }
    }
}
