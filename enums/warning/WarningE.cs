namespace enums.warning
{
    /// <summary>
    /// 警告状态
    /// </summary>
    public enum WaringStatusE
    {
        Create,
        Resolve
    }

    /// <summary>
    /// 警告类型
    /// </summary>
    public enum WarningTypeE
    {
        DeviceOffline,//设备离线
        TrackFullButNoneStock,//轨道满砖但没库存
        CarrierLoadSortTask,//小车倒库中但是小车有货
        CarrierLoadNotSortTask,//小车倒库中任务清除
        TileNoneStrategy,//砖机没有设置策略
        CarrierFullSignalFullNotOnStoreTrack,//小车满砖信号不在储砖轨道
        CarrierGiveMissTrack,//小车前进放货没有扫到地标
        DownTileHaveNotTrackToStore,//砖机找不到空闲轨道存放
        UpTileHaveNotStockToOut,//砖机找不到库存出库
        TrackEarlyFull,//轨道提前满砖报警
        UpTileHaveNoTrackToOut,//砖机找不到有砖轨道上砖
        CarrierLoadNeedTakeCare,//小车没任务，有货需要手动处理
        HaveOtherCarrierInSortTrack,//有别的小车在倒库轨道，倒库车已经停止
        CarrierSortButStop,//倒库小车任务终止，需要手动发送倒库
        TileMixLastTrackInTrans,//砖机混砖作业，轨道被占用

        TileGoodsIsZero,  //砖机工位品种反馈异常
        TileGoodsIsNull,  //砖机工位品种没有配置

        TransHaveNotTheGiveTrack, // 任务没有合适的轨道卸砖
        UpTilePreGoodNotSet,//上砖机预设品种没有设置

        DeviceSortRunOutTrack,//运输车倒库没有扫到定位点冲出轨道

        FerryNoLocation,  //摆渡车没有位置信息

        FailAllocateCarrier, //分配运输车失败
        FailAllocateFerry, //分配摆渡车失败
    }

    /// <summary>
    /// 运输车反馈报警信息
    /// </summary>
    public enum CarrierWarnE
    {
        WarningA1X0 = 100,
        WarningA1X1,
        WarningA1X2,
        WarningA1X3,
        WarningA1X4,
        WarningA1X5,
        WarningA1X6,
        WarningA1X7,

        WarningA2X0,
        WarningA2X1,
        WarningA2X2,
        WarningA2X3,
        WarningA2X4,
        WarningA2X5,
        WarningA2X6,
        WarningA2X7,

        WarningA3X0,
        WarningA3X1,
        WarningA3X2,
        WarningA3X3,
        WarningA3X4,
        WarningA3X5,
        WarningA3X6,
        WarningA3X7,

        WarningA4X0,
        WarningA4X1,
        WarningA4X2,
        WarningA4X3,
        WarningA4X4,
        WarningA4X5,
        WarningA4X6,
        WarningA4X7
    }

}
