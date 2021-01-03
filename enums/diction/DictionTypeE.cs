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
        public const string PDA_INIT_VERSION = nameof(PDA_INIT_VERSION);//PDA基础字典版本数据
        public const string PDA_GOOD_VERSION = nameof(PDA_GOOD_VERSION);//PDA规格字典版本信息

        public const string UserLoginFunction = nameof(UserLoginFunction);//是否开启登陆功能

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
