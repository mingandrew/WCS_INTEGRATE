using System;
using System.Collections.Generic;

namespace tool.appconfig
{
    public class BigConifg
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\BigConifg.json";
        public static readonly string SavePath = $"{Path}{FileName}";

        public List<BigConfigItem> BigConfigList { set; get; } = new List<BigConfigItem>();

        public bool IsFreeUpFerry(uint areaid)
        {
            return GetItem(areaid)?.FreeUpFerry ?? false;
        }

        public bool IsFreeDownFerry(uint areaid)
        {
            return GetItem(areaid)?.FreeDownFerry ?? false;
        }

        public bool IsUpTaskNotTake(uint areaid)
        {
            return GetItem(areaid)?.UpTaskBackNotTake ?? false;
        }

        public BigConfigItem GetItem(uint areaid)
        {
            BigConfigItem item = BigConfigList.Find(c=>c.AreaId == areaid);
            if (item == null)
            {
                item = new BigConfigItem()
                {
                    AreaId = areaid,
                    FreeDownFerry = false,
                    FreeUpFerry = false,
                    UpTaskBackNotTake = false
                };
                BigConfigList.Add(item);
                GlobalWcsDataConfig.SaveBigConifg();
            }
            return item;
        }

    }

    public class BigConfigItem
    {
        public uint AreaId { set; get; }
        public bool FreeUpFerry { set; get; }
        public bool FreeDownFerry { set; get; }
        public bool UpTaskBackNotTake { set; get; }
    }
}
