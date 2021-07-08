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

        /// <summary>
        /// 车辆转移服务检测超时任务时间
        /// </summary>
        public int MoveCarWaitOverTime { set; get; } = 60;
        public int TileRefreshTime { set; get; } = 1000;
        public int TileLiveTime { set; get; } = 1000;
        public int TileInvaTime { set; get; } = 1000;
        public int TileOtherTime { set; get; } = 1000;
        public int TileNeedRefreshTime { set; get; } = 1000;

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
        /// 判断是否启用轨道满砖则移到空轨道
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsMoveWhenFull(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.InMoveWhenFull ?? false;
        }

        /// <summary>
        /// 1.不需要接力运输车把库存放下才取<br/>
        /// 2.接力前面没有砖都可以进去取砖<br/>
        /// 3.让上砖的车与接力车防撞触发停止，接力车放下砖，取货车取砖<br/>
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public bool IsNotNeedSortToSplitUpPlace(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.NotNeedSortToSplitUpPlace ?? false;
        }

        /// <summary>
        /// 使用自动转备用机第二种方式：砖机选备用机
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public bool IsUserAutoBackDevVersion2(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.UserAutoBackDevVersion2 ?? false;
        }


        /// <summary>
        /// 接力暂停，运输车停止在放货点的几个车身位置
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public byte GetSortWaitNumberCarSpace(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.SortWaitNumberCarSpace ?? 3;
        }

        /// <summary>
        /// 不需要接力运输车把库存放下才取
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
                    UpTaskNewAllocate = false,
                    InMoveWhenFull = false,
                    NotNeedSortToSplitUpPlace = false,
                    UserAutoBackDevVersion2 = false,
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
        public bool InMoveWhenFull { set; get; }//出入库轨道，满砖移车
        /// <summary>
        /// 1.不需要接力运输车把库存放下才取<br/>
        /// 2.接力前面没有砖都可以进去取砖<br/>
        /// 3.让上砖的车与接力车防撞触发停止，接力车放下砖，取货车取砖<br/>
        /// </summary>
        public bool NotNeedSortToSplitUpPlace { set; get; }
        /// <summary>
        /// 接力暂停，运输车停止在放货点的几个车身位置
        /// </summary>
        public byte SortWaitNumberCarSpace { set; get; } = 3;

        /// <summary>
        /// 使用自动转备用机第二种方式：砖机选备用机
        /// </summary>
        public bool UserAutoBackDevVersion2 { set; get; }
    }
}
