namespace enums
{
    public static class TimerTag
    {
        public const string DevRefreshTimeOut = nameof(DevRefreshTimeOut);
        public const string DevTcpDateRefresh = nameof(DevTcpDateRefresh);
        public const string TileInTaskStatus = nameof(TileInTaskStatus);

        #region[上下砖机]

        public const string DownTileLifterHaveGoods = nameof(DownTileLifterHaveGoods);//下砖机有砖
        public const string UpTileLifterEmpty = nameof(UpTileLifterEmpty);//上砖机无砖
        public const string TileIsInvolint = nameof(TileIsInvolint);//砖机正在介入中  错开查询/介入发送时间

        #endregion

        #region[运输车]

        public const string CarrierGotLoad = nameof(CarrierGotLoad);//小车取货
        public const string CarrierBeenUnLoad = nameof(CarrierBeenUnLoad);//小车卸货

        public const string CarrierAllocate = nameof(CarrierAllocate);//分配运输车

        public const string CarrierNotTaskStop = nameof(CarrierNotTaskStop);//小车停止并且没有任务
        public const string CarrierOnFerry = nameof(CarrierOnFerry);//小车到达摆渡车

        public const string CarrierOnTask = nameof(CarrierOnTask);//小车处于任务持续时间
        #endregion

        #region[摆渡车]

        public const string FerryOnPosition = nameof(FerryOnPosition);
        public const string RfFerrySiteUpdateSendOffline = nameof(RfFerrySiteUpdateSendOffline);//摆渡车发送平板信息，平板离线
        #endregion

        #region[取消任务]

        public const string TransCancelNoCar = nameof(TransCancelNoCar);//任务还没分配小车

        #endregion

        public const string TileToSurface = nameof(TileToSurface);
        public const string FerryToSurface = nameof(FerryToSurface);
        public const string CarrierToSurface = nameof(CarrierToSurface);
        public const string CarrierLoadNotInTileTrack = nameof(CarrierLoadNotInTileTrack);

        public const string TileInvoNotNeed = nameof(TileInvoNotNeed);
        public const string TileHaveLoadTaskStatusMoveOn = nameof(TileHaveLoadTaskStatusMoveOn);
        public const string GiveTaskMeaningless = nameof(GiveTaskMeaningless);
        public const string DownTileHaveLoadNoNeed = nameof(DownTileHaveLoadNoNeed);
        public const string UpTileReStoreEmtpyNeed = nameof(UpTileReStoreEmtpyNeed);
        public const string UpTileDonotHaveEmtpyAndNeed = nameof(UpTileDonotHaveEmtpyAndNeed);
        public const string CarrierGiveMissTrack = nameof(CarrierGiveMissTrack);
        public const string CarrierSortTakeGive = nameof(CarrierSortTakeGive);
    }
}
