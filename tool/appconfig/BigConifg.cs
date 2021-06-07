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

        /// <summary>
        /// 是否释放上砖摆渡车
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public bool IsFreeUpFerry(uint areaid)
        {
            return GetItem(areaid)?.FreeUpFerry ?? false;
        }

        /// <summary>
        /// 释放释放下砖摆渡车
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public bool IsFreeDownFerry(uint areaid)
        {
            return GetItem(areaid)?.FreeDownFerry ?? false;
        }

        /// <summary>
        /// 上砖任务新分配逻辑
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public bool IsUpTaskNewAllocate(uint areaid)
        {
            return GetItem(areaid)?.UpTaskNewAllocate ?? false;
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
                    UpTaskNewAllocate = false
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
        public bool FreeUpFerry { set; get; }//是否释放上砖摆渡车
        public bool FreeDownFerry { set; get; }//是否释放下砖摆渡车
        public bool UpTaskNewAllocate { set; get; }//上砖任务新分配逻辑
    }
}
