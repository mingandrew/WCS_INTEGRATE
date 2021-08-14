namespace enums.track
{
    /// <summary>
    /// 轨道类型
    /// </summary>
    public enum TrackTypeE
    {
        上砖轨道,
        下砖轨道,
        储砖_入,
        储砖_出,
        储砖_出入,
        后置摆渡轨道,
        前置摆渡轨道
    }

    /// <summary>
    /// 轨道出入类型
    /// （常规-通用；中转方案时-区分出入）
    /// </summary>
    public enum TrackType2E
    {
        通用,
        入库,
        出库,
    }

    /// <summary>
    /// 轨道类型
    /// </summary>
    public enum RfFilterTrackTypeE
    {
        入轨 = 2,
        出轨,
        出入轨,
    }

    public enum TrackStockStatusE
    {
        空砖,
        有砖,
        满砖,
    }

    public enum TrackStatusE
    {
        停用,
        启用,
        倒库中,
        仅上砖,
        仅下砖
    }

    /// <summary>
    /// RF轨道修改状态
    /// </summary>
    public enum TrackRfStatusE
    {
        停用,
        启用,
        仅上砖 = 3,
        仅下砖 = 4
    }

    public enum TrackUpdateE
    {
        StockStatus,
        TrackStatus,
        Common,
        Size,
        Ferry,
        Track,
        RfId,
        Order,
        RecentGoodId,
        RecentTileId,
        Alert,
        EarlyFull,
        Point,
        UpCount,
        TGtype,
        SortAble
    }

    public enum TrackAlertE
    {
        正常,
        小车读点故障,
    }

    public enum TrackLogE
    {
        空轨道 = 1,
        满轨道 = 2
    }

    /// <summary>
    /// 运输车复位点位类型
    /// </summary>
    public enum CarrierPosE
    {
        下砖机复位点 = 8,

        后置摆渡复位点 = 10,

        轨道后侧定位点 = 11,
        轨道后侧复位点 = 12,

        轨道中间复位点 = 50,

        轨道前侧复位点 = 98,
        轨道前侧定位点 = 99,

        前置摆渡复位点 = 100,

        上砖机复位点 = 102,
    }

    /// <summary>
    /// 运输车允许发送初始化的点位（平板）
    /// </summary>
    public enum CarrierInitPoint
    {
        后置摆渡复位点 = 10,

        //轨道后侧定位点 = 11,
        轨道后侧复位点 = 12,

        //轨道中间复位点 = 50,

        轨道前侧复位点 = 98,
        //轨道前侧定位点 = 99,

        前置摆渡复位点 = 100
    }

}
