namespace enums
{
    public enum DevLifterNeedE
    {
        无,
        有
    }

    public enum DevLifterLoadE
    {
        无砖,
        有砖,
        满砖
    }

    public enum DevLifterInvolE
    {
        离开,
        介入,
        清除需求
    }

    public enum DevLifterCmdTypeE
    {
        查询,
        介入1,
        介入2,
        转产,
        模式,
        等级,
        复位转产,//砖机主动触发的转产，执行后进行复位
        开关灯,

        接收回复 = 0x88
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
        补砖,
        无 = 255
    }

    /// <summary>
    /// 平板
    /// </summary>
    public enum RfTileWorkModeE
    {
        过砖,
        上砖 = 1,
        下砖 = 2,
        补砖,
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

    public enum TileAlertShiftE
    {
        复位,
        收到转产,
    }

    public enum TileLightShiftE
    {
        灯关,
        灯开
    }

    public enum TileConfigUpdateE
    {
        Goods,
        Alert_Dev_Id,
        NoWorkTrack,
        LastTrack,
        WorkMode,
        Strategey,

    }
}
