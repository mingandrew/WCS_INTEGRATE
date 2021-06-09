using System;
using System.Collections.Generic;

namespace tool.appconfig
{
    /// <summary>
    /// 
    /// </summary>
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
        public bool IsFreeUpFerry(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.FreeUpFerry ?? false;
        }

        /// <summary>
        /// 释放释放下砖摆渡车
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public bool IsFreeDownFerry(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.FreeDownFerry ?? false;
        }

        /// <summary>
        /// 上砖任务新分配逻辑
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public bool IsUpTaskNewAllocate(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.UpTaskNewAllocate ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public BigConfigItem GetItem(uint areaid, ushort lineid)
        {
            BigConfigItem item = BigConfigList.Find(c=>c.AreaId == areaid && c.LineId == lineid);
            if (item == null)
            {
                item = new BigConfigItem()
                {
                    AreaId = areaid,
                    LineId = lineid,
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
        public uint AreaId { set; get; }//区域
        public ushort LineId { set; get; }//线路
        public bool FreeUpFerry { set; get; }//是否释放上砖摆渡车
        public bool FreeDownFerry { set; get; }//是否释放下砖摆渡车
        public bool UpTaskNewAllocate { set; get; }//上砖任务新分配逻辑
    }
}
