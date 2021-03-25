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

        TileMultipleLastTrackInTrans,//砖机并联轨道作业，轨道被占用

        TileGoodsIsZero,  //砖机工位品种反馈异常
        TileGoodsIsNull,  //砖机工位品种没有配置
    }

    public enum CarrierWarnE
    {
        ReadConBreakenCheckWire = 100,//RFID阅读器故障
        StoreSlowOverTimeCheckLight,
        FrontAvoidAlert,//前防撞
        BackAvoidAlert,//后防撞
        BackTakeOverTime,//后退取货超时
        FrontGiveOverTime,//前进放货超时
        FrontPointOverTime,
        BackPointOverTime,
        Back2FerryOverTime,
        Front2FerryOverTime,
        GoUpOverTime,
        GoDownOverTime,
        BackTakeCannotDo,
        FrontGiveCannotDo,
        Back2FerryCannotDo,
        Front2FerryCannotDo,
        Back2SortCannotDo,
        Front2PointCannotDo,
        Back2PointCannotDo,
        NotGoodToGoUp,
        SortTaskOverTime,
        FunctinSwitchOverTime,
        CheckUpAndLoadIsNormal, //检查上位和有砖信号是否正常
        CheckGoDecelerateIsNormal,  //检查前进存砖减速信号是否正常
        TriggerEmergencyStop //急停触发
    }

}
