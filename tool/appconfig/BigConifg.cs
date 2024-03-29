﻿using System;
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

        /// <summary>
        /// 轨道前空出默认5个位置
        /// </summary>
        public int TrackSortFrontCount { set; get; } = 5;
        /// <summary>
        /// 轨道中间空出默认5个位置
        /// </summary>
        public int TrackSortMidCount { set; get; } = 5;
        /// <summary>
        /// 执行中间位置检测和倒库
        /// </summary>
        public bool TrackSortMid { set; get; } = true;
        /// <summary>
        /// 轨道后空出默认5个位置
        /// </summary>
        public int TrackSortBackCount { set; get; } = 5;

        public List<BigConfigItem> BigConfigList { set; get; } = new List<BigConfigItem>();
        public bool UseSortV2 { get; set; }

        /// <summary>
        /// 库存整理使用相隔多少车的距离
        /// </summary>
        public float MoveStockSeperateCarCount { set; get; } = 2;

        /// <summary>
        /// 是否释放上砖摆渡车
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public bool IsFreeUpFerry(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.FreeUpFerry ?? false;
        }

        /// <summary>
        /// 释放释放下砖摆渡车
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public bool IsFreeDownFerry(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.FreeDownFerry ?? false;
        }

        /// <summary>
        /// 上砖任务新分配逻辑
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public bool IsUpTaskNewAllocate(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.UpTaskNewAllocate ?? false;
        }

        /// <summary>
        /// 是否优先使用上砖机轨道的运输车，即使取砖轨道有运输车也需要用这个
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public bool IsUseUpTileLifterCar(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.UseUpTileLifterCar ?? false;
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
        /// 使用使用单车每次接力执行接力任务
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public bool IsOut2OutSingleStack(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.Out2OutSinglelStack ?? false;
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
        /// 检测出轨道尾部库存脉冲 距离 最后一车脉冲 需要多少倍的 安全距离，默认1.5
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public double GetIn2OutCheckButtomSafeRate(uint areaid, ushort lineid)
        {
            return GetItem(areaid, lineid)?.In2OutCheckButtomSafeRate ?? 1.5;
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
                    NotNeedSortToSplitUpPlace = true,
                    UserAutoBackDevVersion2 = false,
                    Out2OutSinglelStack = false,
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

        /// <summary>
        /// 优先使用停在砖机轨道的运输车，即使取砖轨道有车也先找砖机轨道的运输车
        /// </summary>
        public bool UseUpTileLifterCar { set; get; }

        /// <summary>
        /// 使用单次库存进行接力任务
        /// </summary>
        public bool Out2OutSinglelStack { set; get; }

        /// <summary>
        /// 检测出轨道尾部库存脉冲 距离 最后一车脉冲 需要多少倍的 安全距离，默认1.5，必须大于1
        /// </summary>
        public double In2OutCheckButtomSafeRate { set; get; } = 1.5;
    }
}
