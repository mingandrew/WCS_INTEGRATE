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

    public static class DicTag
    {
        public const string NewStockId = nameof(NewStockId);
        public const string NewTranId = nameof(NewTranId);
        public const string NewWarnId = nameof(NewWarnId);
        public const string NewGoodId = nameof(NewGoodId);
        public const string NewTileTrackId = nameof(NewTileTrackId);
        public const string NewTrafficCtlId = nameof(NewTrafficCtlId);

        public const string PDA_INIT_VERSION = nameof(PDA_INIT_VERSION);//PDA基础字典版本数据
        public const string PDA_GOOD_VERSION = nameof(PDA_GOOD_VERSION);//PDA品种字典版本信息

        public const string UserLoginFunction = nameof(UserLoginFunction);//是否开启登陆功能

        public static string GoodLevel = nameof(GoodLevel);//品种等级

        public static string MinStockTime = nameof(MinStockTime);//最小库存存放时间 小时数
        public static string FerryAvoidNumber = nameof(FerryAvoidNumber);//摆渡车安全距离 轨道数
        public static string TileLifterShiftCount = nameof(TileLifterShiftCount);//下砖机转产差值 层数

        #region[任务逻辑开关]

        public static string StackPluse = nameof(StackPluse);                           //计算库位置使用的一垛间距
        public static string TileNeedSysShiftFunc = nameof(TileNeedSysShiftFunc);       //开关-砖机需转产信号
        public static string AutoBackupTileFunc = nameof(AutoBackupTileFunc);           //开关-计算库位置使用的一垛间距
        public static string SeamlessMoveToFerry = nameof(SeamlessMoveToFerry);         //开关-无缝上摆渡
        public static string UpTaskIgnoreSortTask = nameof(UpTaskIgnoreSortTask);       //开关-允许倒库时可以上砖
        public static string UseUpSplitPoint = nameof(UseUpSplitPoint);                 //开关-启用上砖侧分割点坐标逻辑
        public static string CannotUseUpSplitStock = nameof(CannotUseUpSplitStock);     //开关-限制直接使用上砖侧分割点后的库存
        public static string EnableDiagnose = nameof(EnableDiagnose);                   //开关-启用分析服务
        public static string UseTileFullSign = nameof(UseTileFullSign);                   //开关-启用砖机的-满砖信号
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
