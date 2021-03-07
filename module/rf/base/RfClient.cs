using enums;
using System;
using System.Collections.Generic;

namespace module.rf
{
    public class RfClient
    {
        public string rfid { set; get; }
        public string name { set; get; }
        public string ip { set; get; }
        public DateTime? conn_time { set; get; }
        public DateTime? disconn_time { set; get; }
        public RfConnectE Status { set; get; }
        public bool filter_area { set; get; }//过滤指定区域
        public string filter_areaids { set; get; }
        public List<uint> AreaIds { set; get; }
        public bool filter_type { set; get; }//过滤指定设备类型
        public string filter_typevalues { set; get; }
        public List<byte> DevTypeValues { set; get; }
        public bool filter_dev { set; get; }//过滤指定设备
        public string filter_devids { set; get; }
        public List<uint> DevIds { set; get; }
        public void SetupFilterArea()
        {
            AreaIds = new List<uint>();
            if (!string.IsNullOrEmpty(filter_areaids))
            {
                string[] ids = filter_areaids.Split(':');
                foreach (var item in ids)
                {
                    if (uint.TryParse(item, out uint id))
                    {
                        AreaIds.Add(id);
                    }
                }
            }
        }

        public void SetupFilterType()
        {
            DevTypeValues = new List<byte>();
            if (!string.IsNullOrEmpty(filter_typevalues))
            {
                string[] types = filter_typevalues.Split(':');
                foreach (var item in types)
                {
                    if (byte.TryParse(item, out byte type))
                    {
                        DevTypeValues.Add(type);
                    }
                }
            }
        }

        public void SetupFilterDevice()
        {
            DevIds = new List<uint>();
            if (!string.IsNullOrEmpty(filter_devids))
            {
                string[] ids = filter_devids.Split(':');
                foreach (var item in ids)
                {
                    if (uint.TryParse(item, out uint id))
                    {
                        DevIds.Add(id);
                    }
                }
            }
        }

    }
}
