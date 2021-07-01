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

        public const string NewStockId = nameof(NewStockId);
        public const string NewTranId = nameof(NewTranId);
        public const string NewWarnId = nameof(NewWarnId);
        public const string NewGoodId = nameof(NewGoodId);
        public const string NewTileTrackId = nameof(NewTileTrackId);
        public const string NewTrafficCtlId = nameof(NewTrafficCtlId);

        public const string NewTranDtlId = nameof(NewTranDtlId);

        #endregion

        #region[平板]

        public const string UserLoginFunction = nameof(UserLoginFunction);//是否开启登陆功能
        public const string PDA_INIT_VERSION = nameof(PDA_INIT_VERSION);//PDA基础字典版本数据
        public const string PDA_GOOD_VERSION = nameof(PDA_GOOD_VERSION);//PDA品种字典版本信息

        #endregion

        #region[其他]

        public static string GoodLevel = nameof(GoodLevel);                             //品种等级
        public static string MinStockTime = nameof(MinStockTime);               //最小库存存放时间 小时数
        public static string FerryAvoidNumber = nameof(FerryAvoidNumber);//摆渡车安全距离 轨道数
        public static string TileLifterShiftCount = nameof(TileLifterShiftCount);//下砖机转产差值 层数
        public static string StackPluse = nameof(StackPluse);                              //计算库位置使用的一垛间距
        public static string Pulse2CM = nameof(Pulse2CM);                              //1脉冲=厘米

        #endregion

        #region[任务逻辑开关]

        public static string TileNeedSysShiftFunc = nameof(TileNeedSysShiftFunc);       //开关-砖机需转产信号
        public static string AutoBackupTileFunc = nameof(AutoBackupTileFunc);           //开关-备用砖机切换备用
        public static string SeamlessMoveToFerry = nameof(SeamlessMoveToFerry);     //开关-无缝上摆渡
        public static string UseTileFullSign = nameof(UseTileFullSign);                             //开关-启用砖机的-满砖信号
        public static string EnableCarrierTraffic = nameof(EnableCarrierTraffic);               //开关-启用运输车交管摆渡车
        public static string EnableLimitAllocate = nameof(EnableLimitAllocate);               //开关-启用下砖入库极限混砖

        public static string EnableStockTimeForUp = nameof(EnableStockTimeForUp);               //开关-启用上砖库存时间限制（品种库存最早时间在入库侧-停止上砖且报警）
        public static string EnableStockTimeForDown = nameof(EnableStockTimeForDown);     //开关-启用下砖库存时间限制（不得连续下满同一条轨道-仅剩最后一条轨道时停止下砖且报警）

        public static string EnableDownTrackOrder = nameof(EnableDownTrackOrder);     //开关-启用下砖顺序存放（下砖时按轨道顺序存放）

        public static string AllowClearTask = nameof(AllowClearTask);     //开关-允许清除任务
        #endregion

        #region[接力倒库、倒库同时上砖开关]

        public static string UpTaskIgnoreSortTask = nameof(UpTaskIgnoreSortTask);          //开关-允许接力倒库时可以上砖
        public static string UseUpSplitPoint = nameof(UseUpSplitPoint);                             //开关-启用上砖侧分割点坐标逻辑
        public static string CannotUseUpSplitStock = nameof(CannotUseUpSplitStock);    //开关-限制直接使用上砖侧分割点后的库存
        public static string UpSortUseMaxNumber = nameof(UpSortUseMaxNumber);     //开关-接力倒库使用倒库最大数量【在线路上配置】

        public static string UpTaskIgnoreInoutSortTask = nameof(UpTaskIgnoreInoutSortTask);          //开关-允许出入倒库时可以上砖

        public static string EnableSecondUpTask = nameof(EnableSecondUpTask);          //开关-启用反抛任务
        #endregion

        #region[分析服务开关]

        public static string EnableDiagnose = nameof(EnableDiagnose);                   //开关-启用分析服务
        public static string EnableSortDiagnose = nameof(EnableSortDiagnose);   //开关-启用倒库分析服务
        public static string EnableMoveCarDiagnose = nameof(EnableMoveCarDiagnose);   //开关-启用移车分析服务

        #endregion

        #region[流程超时]

        public static string StepOverTime = nameof(StepOverTime);               //除【倒库中】，其他流程的超时时间（秒）
        public static string SortingStockStepOverTime = nameof(SortingStockStepOverTime);               //倒库中流程的超时时间（秒）

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
