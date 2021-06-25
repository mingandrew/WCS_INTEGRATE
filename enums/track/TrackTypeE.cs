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
        摆渡车_入,
        摆渡车_出
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
        UpCount
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
        下砖摆渡点 = 10,
        入轨口定位点 = 11,
        入轨口复位点 = 12,
        轨道中点 = 50,
        出轨口复位点 = 98,
        出轨口定位点 = 99,
        上砖摆渡点 = 100
    }

}
