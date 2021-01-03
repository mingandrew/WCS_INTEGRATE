namespace enums
{

    public static class FunTag
    {
        #region[基础信息]
        public const string DisConnect = nameof(DisConnect);
        public const string HeartBeat = nameof(HeartBeat);
        public const string Empty = nameof(Empty);
        public const string Login = nameof(Login);
        public const string ModuleView = nameof(ModuleView);
        public const string QueryVersion = nameof(QueryVersion);
        public const string InitVersion = nameof(InitVersion);
        public const string UserCheck = nameof(UserCheck);
        #endregion

        #region[数字字典]
        public const string QueryDicAll = nameof(QueryDicAll);
        public const string UpdateGoodDic = nameof(UpdateGoodDic);
        #endregion

        #region[查询]
        public const string QueryDevStatus = nameof(QueryDevStatus);
        public const string QueryWarn = nameof(QueryWarn);

        public const string QueryGoods = nameof(QueryGoods);
        public const string QueryStock = nameof(QueryStock);
        public const string QueryTrack = nameof(QueryTrack);
        public const string QuerySingleTrack = nameof(QuerySingleTrack);

        public const string QueryTileGood = nameof(QueryTileGood);
        #endregion

        #region[规格]

        public const string AddGoods = nameof(AddGoods);

        #endregion

        #region[摆渡对位]

        public const string StartFerryPos = nameof(StartFerryPos);//开始对位
        public const string StopFerryPos = nameof(StopFerryPos);//结束对位
        public const string QueryFerryPos = nameof(QueryFerryPos);

        public const string FerrySiteUpdate = nameof(FerrySiteUpdate);//当前站点，光电状态
        public const string FerryPosUpdate = nameof(FerryPosUpdate);//当前地标，
        public const string UpdateFerryPos = nameof(UpdateFerryPos);//更新摆渡车

        #endregion

        #region[库存]
        public const string QueryStockSum = nameof(QueryStockSum);
        public const string AddTrackStock = nameof(AddTrackStock);
        public const string QueryTrackStock = nameof(QueryTrackStock);
        public const string ShiftTrackStock = nameof(ShiftTrackStock);
        public const string UpdateStockGood = nameof(UpdateStockGood);
        public const string DeleteTrackStock = nameof(DeleteTrackStock);
        #endregion

        #region[操作]
        public const string UpdateGood = nameof(UpdateGood);
        public const string UpdateTrackStatus = nameof(UpdateTrackStatus);
        public const string UpdateTileGood = nameof(UpdateTileGood);
        public const string QueryTileStockGood = nameof(QueryTileStockGood);//获取库存规格

        public const string UpdateAbmSet = nameof(UpdateAbmSet);
        public const string UpdateRgvSet = nameof(UpdateRgvSet);
        #endregion

        #region[任务]
        public const string TaskFerryToPos = nameof(TaskFerryToPos);
        public const string TaskFerryStop = nameof(TaskFerryStop);
        public const string TaskFerryReset = nameof(TaskFerryReset);
        #endregion

        #region[警告]

        public const string FatalErrorNotice = nameof(FatalErrorNotice);

        #endregion

        #region[任务开关]

        public const string QueryTaskSwitch = nameof(QueryTaskSwitch);
        public const string UpdateTaskSwitch = nameof(UpdateTaskSwitch);

        #endregion

        #region[设备信息]
        public const string QueryDevice = nameof(QueryDevice);
        public const string UpdateDevWorking = nameof(UpdateDevWorking);
        public const string QueryDevFerry = nameof(QueryDevFerry);
        public const string DoDevFerryTask = nameof(DoDevFerryTask);

        public const string QueryDevCarrier = nameof(QueryDevCarrier);
        public const string DoDevCarrierTask = nameof(DoDevCarrierTask);

        public const string QueryDevTileLifter = nameof(QueryDevTileLifter);
        public const string DoDevTileLifterTask = nameof(DoDevTileLifterTask);

        #endregion

        #region[任务信息]

        public const string QueryTrans = nameof(QueryTrans);
        public const string ForseTransFinish = nameof(ForseTransFinish);
        public const string CancelTrans = nameof(CancelTrans);

        #endregion

        #region[按轨出库]

        public const string QueryTileTrack = nameof(QueryTileTrack);
        public const string UpdateTileTrack = nameof(UpdateTileTrack);

        #endregion


        #region[转规格]
        public const string QueryTileShift = nameof(QueryTileShift);//查询砖机转产信息

        public const string UpdatePreGood = nameof(UpdatePreGood);//更新预设品种
        public const string ShiftTileGood = nameof(ShiftTileGood);//转规格

        #endregion
    }
}
