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
        FerryTargetUnconfigured,  //摆渡车没有目的位置对位坐标值

        FailAllocateCarrier, //分配运输车失败
        FailAllocateFerry, //分配摆渡车失败

        UpTileEmptyNeedAndNoBack, //任务中上砖机没有需求信号且小车无轨可回

        CarrierFreeButMoveInFerry, //运输车空闲状态(停止/指令完成)，但处于上下摆渡中
        CarrierFreeInFerryButLocErr, //运输车空闲状态(停止/指令完成)在摆渡上，但当前轨道有误


        TheEarliestStockInDown,// 以先进先出为原则，发现最早的库存在下砖入库侧轨道，暂无法上砖
        PreventTimeConflict,// 不允许下砖连续下满同一条轨道，需变更轨道下砖，防止时间冲突

        DownTaskSwitchClosed,//【下砖任务开关】关闭
        UpTaskSwitchClosed,//【上砖任务开关】关闭
        SortTaskSwitchClosed,//【倒库任务开关】关闭

        SortFinishButDownExistStock,//倒库指令完成，但入库轨道还有库存
        GetStockButNull,//取砖指令完成后，没有取到砖

        Warning34,//【反抛未执行】，等待上砖机工位空砖
        Warning35,//【反抛未执行】，上砖侧库存里有反抛任务的品种可上
        Warning36,//【流程超时】
        Warning37,//当前品种设定的上砖数量为零

        CarrierIsInResetWork,//运输车初始化/寻点指令中，已暂停相关作业，请确认完成发送终止指令
        CarrierNoLocation,  //运输车没有位置信息

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
        WarningA4X7,

        WarningA5X0,
        WarningA5X1,
        WarningA5X2,
        WarningA5X3,
        WarningA5X4,
        WarningA5X5,
        WarningA5X6,
        WarningA5X7,

        WarningA6X0,
        WarningA6X1,
        WarningA6X2,
        WarningA6X3,
        WarningA6X4,
        WarningA6X5,
        WarningA6X6,
        WarningA6X7,

        WarningA7X0,
        WarningA7X1,
        WarningA7X2,
        WarningA7X3,
        WarningA7X4,
        WarningA7X5,
        WarningA7X6,
        WarningA7X7,

        WarningA8X0,
        WarningA8X1,
        WarningA8X2,
        WarningA8X3,
        WarningA8X4,
        WarningA8X5,
        WarningA8X6,
        WarningA8X7,

        WarningA9X0,
        WarningA9X1,
        WarningA9X2,
        WarningA9X3,
        WarningA9X4,
        WarningA9X5,
        WarningA9X6,
        WarningA9X7,

        WarningA10X0,
        WarningA10X1,
        WarningA10X2,
        WarningA10X3,
        WarningA10X4,
        WarningA10X5,
        WarningA10X6,
        WarningA10X7,
    }

    /// <summary>
    /// 摆渡车反馈报警信息
    /// </summary>
    public enum FerryWarnE
    {
        WarningF_A1X0 = 180,
        WarningF_A1X1,
        WarningF_A1X2,
        WarningF_A1X3,
        WarningF_A1X4,
        WarningF_A1X5,
        WarningF_A1X6,
        WarningF_A1X7,
    }
}
