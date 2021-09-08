namespace enums
{
    public static class TimerTag
    {
        /// <summary>
        /// 设备通讯信息刷新
        /// </summary>
        public const string DevTcpDateRefresh = nameof(DevTcpDateRefresh);

        /// <summary>
        /// 设备信息刷新超时
        /// </summary>
        public const string DevRefreshTimeOut = nameof(DevRefreshTimeOut);

        #region[砖机]

        /// <summary>
        /// 砖机存在需求
        /// </summary>
        public const string TileInTaskStatus = nameof(TileInTaskStatus);

        /// <summary>
        /// 砖机无需求但介入
        /// </summary>
        public const string TileInvoNotNeed = nameof(TileInvoNotNeed);

        /// <summary>
        /// 下砖-不满足 满砖需求
        /// </summary>
        public const string DownTileHaveLoadNoNeed = nameof(DownTileHaveLoadNoNeed);

        /// <summary>
        /// 上砖-恢复 空砖需求
        /// </summary>
        public const string UpTileReStoreEmtpyNeed = nameof(UpTileReStoreEmtpyNeed);

        /// <summary>
        /// 上砖-不满足 空砖需求
        /// </summary>
        public const string UpTileDonotHaveEmtpyAndNeed = nameof(UpTileDonotHaveEmtpyAndNeed);

        /// <summary>
        /// 砖机取消需求
        /// </summary>
        public const string TileNeedCancel = nameof(TileNeedCancel);

        /// <summary>
        /// 砖机品种异常
        /// </summary>
        public const string TileGoodsErr = nameof(TileGoodsErr);
        #endregion

        #region[运输车]

        /// <summary>
        /// 任务无分配运输车
        /// </summary>
        public const string TransCancelNoCar = nameof(TransCancelNoCar);

        /// <summary>
        /// 任务分配运输车失败
        /// </summary>
        public const string FailAllocateCarrier = nameof(FailAllocateCarrier);

        /// <summary>
        /// 运输车载货-砖机轨道空砖
        /// </summary>
        public const string CarrierLoadNotInTileTrack = nameof(CarrierLoadNotInTileTrack);
        #endregion

        #region[摆渡车]

        /// <summary>
        /// 任务分配摆渡车失败
        /// </summary>
        public const string FailAllocateFerry = nameof(FailAllocateFerry);

        /// <summary>
        /// 摆渡车：发送平板信息-平板离线
        /// </summary>
        public const string RfFerrySiteUpdateSendOffline = nameof(RfFerrySiteUpdateSendOffline);
        #endregion

    }
}
