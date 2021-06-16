using System;
using System.Collections.Generic;

namespace tool.appconfig
{
    public class DevLightConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\DevLightConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";

        public List<DevLight> DevList { set; get; } = new List<DevLight>();

        public DevLightConfig() 
        { 

        }

        public DevLight GetDevLight(uint devid)
        {
            DevLight dev = DevList.Find(c => c.DevId == devid);
            if(dev == null)
            {
                dev = new DevLight()
                {
                    DevId = devid,
                    HaveLight = false,
                    OnlyMyself = false,
                    OnlyArea = true,
                    OnlyLine = false,
                    WarnLevel = 3
                };
                DevList.Add(dev);
                GlobalWcsDataConfig.SaveAlertLightConfig();
            }

            if(dev.HaveLight)
                return dev;
            return null;
        }
    }

    public class DevLight
    {
        public uint DevId { set; get; }
        public bool HaveLight { set; get; }
        /// <summary>
        /// 只管自己的报警
        /// </summary>
        public bool OnlyMyself { set; get; }
        /// <summary>
        /// 是否只报当前区域的
        /// </summary>
        public bool OnlyArea { set; get; }
        /// <summary>
        /// 是否只报当前线路
        /// </summary>
        public bool OnlyLine { set; get; }

        /// <summary>
        /// 报警等级
        /// </summary>
        public ushort WarnLevel { set; get; }
    }
}
