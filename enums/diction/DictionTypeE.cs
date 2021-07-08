namespace enums
{

    public enum DictionTypeE
    {
        设备,
        任务,
        开关,
        用户
    }

    public enum DicAreaTaskE
    {
        上砖,
        下砖,
        倒库
    }

    /// <summary>
    /// 字典编号
    /// </summary>
    public static class DicTag
    {
        #region[创建标识]
        /// <summary>
        /// 库存ID
        /// </summary>
        public const string NewStockId = nameof(NewStockId);

        /// <summary>
        /// 任务ID
        /// </summary>
        public const string NewTranId = nameof(NewTranId);

        /// <summary>
        /// 警告ID
        /// </summary>
        public const string NewWarnId = nameof(NewWarnId);

        /// <summary>
        /// 品种ID
        /// </summary>
        public const string NewGoodId = nameof(NewGoodId);

        /// <summary>
        /// 砖机轨道ID
        /// </summary>
        public const string NewTileTrackId = nameof(NewTileTrackId);

        /// <summary>
        /// 交管ID
        /// </summary>
        public const string NewTrafficCtlId = nameof(NewTrafficCtlId);

        public const string NewTranDtlId = nameof(NewTranDtlId);

        #endregion

        #region[平板]
        /// <summary>
        /// 是否开启登陆功能
        /// </summary>
        public const string UserLoginFunction = nameof(UserLoginFunction);

        /// <summary>
        /// PDA基础字典版本数据
        /// </summary>
        public const string PDA_INIT_VERSION = nameof(PDA_INIT_VERSION);

        /// <summary>
        /// PDA品种字典版本信息
        /// </summary>
        public const string PDA_GOOD_VERSION = nameof(PDA_GOOD_VERSION);

        #endregion

        #region[其他]
        /// <summary>
        /// 品种等级
        /// </summary>
        public static string GoodLevel = nameof(GoodLevel);

        /// <summary>
        /// 最小库存存放时间 小时数
        /// </summary>
        public static string MinStockTime = nameof(MinStockTime);

        /// <summary>
        /// 摆渡车安全距离 轨道数
        /// </summary>
        public static string FerryAvoidNumber = nameof(FerryAvoidNumber);

        /// <summary>
        /// 下砖机转产差值 层数
        /// </summary>
        public static string TileLifterShiftCount = nameof(TileLifterShiftCount);

        /// <summary>
        /// 计算库位置使用的一垛间距
        /// </summary>
        public static string StackPluse = nameof(StackPluse);

        /// <summary>
        /// 1脉冲=厘米
        /// </summary>
        public static string Pulse2CM = nameof(Pulse2CM);

        /// <summary>
        /// 品种列表数量上限
        /// </summary>
        public static string GoodsListLimit = nameof(GoodsListLimit);

        #endregion

        #region[任务逻辑开关]
        /// <summary>
        /// 开关-砖机需转产信号
        /// </summary>
        public static string TileNeedSysShiftFunc = nameof(TileNeedSysShiftFunc);

        /// <summary>
        /// 开关-备用砖机切换备用
        /// </summary>
        public static string AutoBackupTileFunc = nameof(AutoBackupTileFunc);

        /// <summary>
        /// 开关-无缝上摆渡
        /// </summary>
        public static string SeamlessMoveToFerry = nameof(SeamlessMoveToFerry);

        /// <summary>
        /// 开关-启用砖机的-满砖信号
        /// </summary>
        public static string UseTileFullSign = nameof(UseTileFullSign);

        /// <summary>
        /// 开关-启用运输车交管摆渡车
        /// </summary>
        public static string EnableCarrierTraffic = nameof(EnableCarrierTraffic);

        /// <summary>
        /// 开关-启用下砖入库极限混砖
        /// </summary>
        public static string EnableLimitAllocate = nameof(EnableLimitAllocate);

        /// <summary>
        /// 开关-启用上砖库存时间限制（品种库存最早时间在入库侧-停止上砖且报警）
        /// </summary>
        public static string EnableStockTimeForUp = nameof(EnableStockTimeForUp);

        /// <summary>
        /// 开关-启用下砖库存时间限制（不得连续下满同一条轨道-仅剩最后一条轨道时停止下砖且报警）
        /// </summary>
        public static string EnableStockTimeForDown = nameof(EnableStockTimeForDown);

        /// <summary>
        /// 开关-启用下砖顺序存放（下砖时按轨道顺序存放）
        /// </summary>
        public static string EnableDownTrackOrder = nameof(EnableDownTrackOrder);

        /// <summary>
        /// 开关-允许清除任务
        /// </summary>
        public static string AllowClearTask = nameof(AllowClearTask);
        #endregion

        #region[接力倒库、倒库同时上砖开关]
        /// <summary>
        /// 开关-允许接力倒库时可以上砖
        /// </summary>
        public static string UpTaskIgnoreSortTask = nameof(UpTaskIgnoreSortTask);

        /// <summary>
        /// 开关-启用上砖侧分割点坐标逻辑
        /// </summary>
        public static string UseUpSplitPoint = nameof(UseUpSplitPoint);

        /// <summary>
        /// 开关-限制直接使用上砖侧分割点后的库存
        /// </summary>
        public static string CannotUseUpSplitStock = nameof(CannotUseUpSplitStock);

        /// <summary>
        /// 开关-接力倒库使用倒库最大数量【在线路上配置】
        /// </summary>
        public static string UpSortUseMaxNumber = nameof(UpSortUseMaxNumber);

        /// <summary>
        /// 开关-允许出入倒库时可以上砖
        /// </summary>
        public static string UpTaskIgnoreInoutSortTask = nameof(UpTaskIgnoreInoutSortTask);

        /// <summary>
        /// 开关-启用反抛任务
        /// </summary>
        public static string EnableSecondUpTask = nameof(EnableSecondUpTask);

        #endregion

        #region[分析服务开关]
        /// <summary>
        /// 开关-启用分析服务
        /// </summary>
        public static string EnableDiagnose = nameof(EnableDiagnose);

        /// <summary>
        /// 开关-启用倒库分析服务
        /// </summary>
        public static string EnableSortDiagnose = nameof(EnableSortDiagnose);

        /// <summary>
        /// 开关-启用移车分析服务
        /// </summary>
        public static string EnableMoveCarDiagnose = nameof(EnableMoveCarDiagnose);

        #endregion

        #region[流程超时]

        /// <summary>
        /// 除【倒库中】，其他流程的超时时间（秒）
        /// </summary>
        public static string StepOverTime = nameof(StepOverTime);

        /// <summary>
        /// 倒库中流程的超时时间（秒）
        /// </summary>
        public static string SortingStockStepOverTime = nameof(SortingStockStepOverTime);

        #endregion
    }

    public static class DicSwitchTag
    {
        public const string Area = nameof(Area);
        public const string Down = nameof(Down);
        public const string Up = nameof(Up);
        public const string Sort = nameof(Sort);

        public const string Area1Down = nameof(Area1Down);
        public const string Area1Up = nameof(Area1Up);
        public const string Area1Sort = nameof(Area1Sort);

        public const string Area2Down = nameof(Area2Down);
        public const string Area2Up = nameof(Area2Up);
        public const string Area2Sort = nameof(Area2Sort);

        public const string Area3Down = nameof(Area3Down);
        public const string Area3Up = nameof(Area3Up);
        public const string Area3Sort = nameof(Area3Sort);

        public const string Area4Down = nameof(Area4Down);
        public const string Area4Up = nameof(Area4Up);
        public const string Area4Sort = nameof(Area4Sort);

        public const string Area5Down = nameof(Area5Down);
        public const string Area5Up = nameof(Area5Up);
        public const string Area5Sort = nameof(Area5Sort);
    }
}
