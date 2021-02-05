﻿namespace enums
{
    public enum DevLifterNeedE
    {
        无,
        有
    }

    public enum DevLifterLoadE
    {
        无砖,
        有砖
    }

    public enum DevLifterInvolE
    {
        离开,
        介入
    }

    public enum DevLifterCmdTypeE
    {
        查询,
        介入1,
        介入2,
        转产,
        模式,
        等级
    }

    public enum TileShiftCmdE
    {
        复位,
        变更品种,
        执行转产
    }

    public enum TileShiftStatusE
    {
        复位,
        转产中,
        完成
    }

    public enum TileWorkModeE
    {
        过砖,
        上砖,
        下砖,
        无 = 255
    }

    /// <summary>
    /// 平板
    /// </summary>
    public enum RfTileWorkModeE
    {
        上砖 = 1,
        下砖 = 2
    }


    public enum TileFullE
    {
        忽略,
        设为满砖
    }

    public enum TileLifterTypeE
    {
        前进放砖,
        后退放砖,
    }

    //需求列表的 20210121
    public enum TileNeedStatusE
    {
        Trans = 0, //生成任务
        Finish = 1, //任务完成
        UpdateCreateTime = 2, //更新需求生成时间
    }
}
