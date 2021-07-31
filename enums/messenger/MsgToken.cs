using System;

namespace enums
{
    public static class MsgToken
    {
        public const string AllowShow = nameof(AllowShow); //授权显示
        public const string ActiveTab = nameof(ActiveTab);

        public static string LangUpdated = nameof(LangUpdated);

        public static string MainDialog = nameof(MainDialog);
        public const string OperateGrandUpdate = nameof(OperateGrandUpdate);

        public const string WarningUpdate = nameof(WarningUpdate);

        public const string TabItemSelected = nameof(TabItemSelected);
        public const string ResourceInitFinish = nameof(ResourceInitFinish);
        public const string SocketMsgUpdate = nameof(SocketMsgUpdate);

        public const string CarrierMsgUpdate = nameof(CarrierMsgUpdate);
        public const string CarrierStatusUpdate = nameof(CarrierStatusUpdate);

        public const string TileLifterMsgUpdate = nameof(TileLifterMsgUpdate);
        public const string TileLifterStatusUpdate = nameof(TileLifterStatusUpdate);

        public const string FerryMsgUpdate = nameof(FerryMsgUpdate);
        public const string FerryStatusUpdate = nameof(FerryStatusUpdate);
        public const string FerrySiteUpdate = nameof(FerrySiteUpdate);

        public const string TrackStatusUpdate = nameof(TrackStatusUpdate);

        public const string RfMsgUpdate = nameof(RfMsgUpdate);
        public const string RfStatusUpdate = nameof(RfStatusUpdate);

        public const string StockSumeUpdate = nameof(StockSumeUpdate);
        public const string TrackStockQtyUpdate = nameof(TrackStockQtyUpdate);
        public const string SetStockSelectTrack = nameof(SetStockSelectTrack);

        public const string TransUpdate = nameof(TransUpdate);
        public const string GoodsUpdate = nameof(GoodsUpdate);
        public const string TaskSwitchUpdate = nameof(TaskSwitchUpdate);
        public const string LineSwitchUpdate = nameof(LineSwitchUpdate);

        public const string RefreshRfTrackPos = nameof(RefreshRfTrackPos);
        public const string StockTransDtlUpdate = nameof(StockTransDtlUpdate);//库存细单状态更新

        public const string AutoSearchGood = nameof(AutoSearchGood);//自动搜索品种
        public const string AutoSearchStockGood = nameof(AutoSearchStockGood);//自动搜索品种

        #region[模拟系统]

        public const string SimCarrierMsgUpdate = nameof(SimCarrierMsgUpdate);//通讯
        public const string SimFerryMsgUpdate = nameof(SimFerryMsgUpdate);//通讯
        public const string SimTileLifterMsgUpdate = nameof(SimTileLifterMsgUpdate);//通讯

        public const string SimDeviceStatusUpdate = nameof(SimDeviceStatusUpdate);//界面
        public const string SimFerryStatusUpdate = nameof(SimFerryStatusUpdate);//界面
        public const string SimTileLifterStatusUpdate = nameof(SimTileLifterStatusUpdate);//界面
        #endregion
    }
}
