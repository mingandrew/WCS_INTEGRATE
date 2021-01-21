using enums;
using System;
using System.Collections.Generic;

namespace module.rf
{
    public class RfClient
    {
        public RfClient()
        {
            AreaIds = new List<uint>();
        }

        public string rfid { set; get; }
        public string name { set; get; }
        public string ip { set; get; }
        public DateTime? conn_time { set; get; }
        public DateTime? disconn_time { set; get; }

        public RfConnectE Status { set; get; }
        public bool filter_area { set; get; }
        public string filter_areaids { set; get; }
        public List<uint> AreaIds { set; get; }

        public void SetupFilterArea()
        {
            if (!string.IsNullOrEmpty(filter_areaids))
            {
                string[] ids = filter_areaids.Split(':');
                foreach (var item in ids)
                {
                    if(uint.TryParse(item ,out uint id))
                    {
                        AreaIds.Add(id);
                    }
                }
            }
        }

        public bool CanDoInArea(uint id)
        {
            if (!filter_area) return true;
            return AreaIds.Contains(id);
        }
    }
}
