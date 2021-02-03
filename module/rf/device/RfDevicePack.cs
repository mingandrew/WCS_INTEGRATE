using System.Collections.Generic;

namespace module.rf.device
{
    public class RfDevicePack
    {
        public List<RfDevice> DevList { set; get; }

        public void AddDevs(List<RfDevice> list)
        {
            if(DevList == null)
            {
                DevList = new List<RfDevice>();
            }

            DevList.AddRange(list);
        }

        public void AddDevs(RfDevice dev)
        {
            if (DevList == null)
            {
                DevList = new List<RfDevice>();
            }

            DevList.Add(dev);
        }
    }
}
