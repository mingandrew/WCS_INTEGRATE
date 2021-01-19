using enums;
using enums.track;
using enums.warning;
using module.device;
using module.deviceconfig;
using resource;
using socket.tcp;
using task.task;

namespace task.device
{
    public class CarrierTask : TaskBase
    {
        #region[属性]

        public uint TrackId { set; get; }

        /// <summary>
        /// 上一次的摆渡车轨道id
        /// </summary>
        public uint LastTrackId { set; get; }

        public CarrierTypeE CarrierType
        {
            get => Device.CarrierType;
        }

        public ushort Site
        {
            get => DevStatus?.CurrentSite ?? 0;
        }

        public DevCarrierStatusE Status
        {
            get => DevStatus?.DeviceStatus ?? DevCarrierStatusE.异常;
        }

        public DevOperateModeE OperateMode
        {
            get => DevStatus?.OperateMode ?? DevOperateModeE.手动;
        }

        public DevCarrierPositionE CarrierPosition
        {
            get => DevStatus?.Position ?? DevCarrierPositionE.异常;
        }

        public DevCarrierLoadE Load
        {
            get => DevStatus?.LoadStatus ?? DevCarrierLoadE.异常;
        }

        public DevCarrierTaskE FinishTask
        {
            get => DevStatus?.FinishTask ?? DevCarrierTaskE.其他;
        }

        public DevCarrierTaskE Task
        {
            get => DevStatus?.CurrentTask ?? DevCarrierTaskE.其他;
        }

        public DevCarrierOrderE FinishOrder
        {
            get => DevStatus?.FinishOrder ?? DevCarrierOrderE.异常;
        }

        public DevCarrierOrderE CurrentOrder
        {
            get => DevStatus?.CurrentOrder ?? DevCarrierOrderE.异常;
        }

        #endregion

        #region[构造/启动/停止]

        public CarrierTcp DevTcp { set; get; }
        public DevCarrier DevStatus { set; get; }
        public DevCarrierResetE DevReset { set; get; }
        public ConfigCarrier DevConfig { set; get; }

        public CarrierTask() : base()
        {
            DevStatus = new DevCarrier();
            DevConfig = new ConfigCarrier();
        }

        public void Start(string memo = "开始连接")
        {
            if (!IsEnable) return;

            if (DevTcp == null)
            {
                DevTcp = new CarrierTcp(Device);
            }

            if (!DevTcp.m_Working)
            {
                DevTcp.Start(memo);
            }
        }

        public void Stop(string memo)
        {
            DevTcp?.Stop(memo);
        }
        #endregion

        #region[发送指令]

        /// <summary>
        /// 查询指令
        /// </summary>
        internal void DoQuery()
        {
            DevTcp?.SendCmd(DevCarrierCmdE.查询);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="order">指令类型</param>
        /// <param name="checkTrack">校验轨道号</param>
        /// <param name="toRFID">定位RFID</param>
        /// <param name="toSite">定位坐标</param>
        /// <param name="overRFID">结束RFID</param>
        /// <param name="overSite">结束坐标</param>
        /// <param name="moveCount">倒库数量</param>
        internal void DoOrder(DevCarrierOrderE order, ushort checkTra, ushort toRFID = 0, ushort toSite = 0, ushort overRFID = 0, ushort overSite = 0, byte moveCount = 0)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.执行任务, order, checkTra, toRFID, toSite, overRFID, overSite, moveCount);
        }

        /// <summary>
        /// 设置复位点
        /// </summary>
        /// <param name="RFID">RFID</param>
        /// <param name="Site">坐标</param>
        internal void DoResetSite(ushort RFID, ushort Site)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.设复位点, 0, 0, RFID, Site, 0, 0, 0);
        }

        /// <summary>
        /// 终止指令
        /// </summary>
        internal void DoStop()
        {
            DevTcp?.SendCmd(DevCarrierCmdE.终止任务);
        }

        internal void DoTask(DevCarrierTaskE task, DevCarrierSizeE oversize)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.执行任务);
        }

        #endregion

        #region[更新轨道信息]

        internal void UpdateInfo()
        {
            if (Site != 0)
            {
                TrackId = PubMaster.Track.GetTrackId(Site);
            }
        }

        #endregion

        #region[检查报警]

        public void CheckAlert()
        {
            Alert1();
            Alert2();
            Alert3();
            Alert4();
        }
        private void Alert1()
        {
            if (DevStatus.Aler1 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 1);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler1, 0))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.ReadConBreakenCheckWire, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.ReadConBreakenCheckWire, (ushort)ID);

            if (On(DevStatus.Aler1, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.StoreSlowOverTimeCheckLight, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.StoreSlowOverTimeCheckLight, (ushort)ID);

            if (On(DevStatus.Aler1, 2) && mTimer.IsOver(12, 15, 10))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.FrontAvoidAlert, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.FrontAvoidAlert, (ushort)ID);


            if (On(DevStatus.Aler1, 3) && mTimer.IsOver(13, 15, 10))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.BackAvoidAlert, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.BackAvoidAlert, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.BackTakeOverTime, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.BackTakeOverTime, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.FrontGiveOverTime, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.FrontGiveOverTime, (ushort)ID);
            }


            if (On(DevStatus.Aler1, 6))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Front2PointCannotDo, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Front2PointCannotDo, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 7))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Back2PointCannotDo, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Back2PointCannotDo, (ushort)ID);
            }
        }

        internal void ClearDevStatus()
        {
            if (DevStatus != null)
            {
                DevStatus.CurrentSite = 0;
                DevStatus.CurrentTask = DevCarrierTaskE.其他;
                TrackId = 0;
            }
        }

        private void Alert2()
        {
            if (DevStatus.Aler2 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 2);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler2, 0))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Back2FerryOverTime, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Back2FerryOverTime, (ushort)ID);

            if (On(DevStatus.Aler2, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Front2FerryOverTime, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Front2FerryOverTime, (ushort)ID);

            if (On(DevStatus.Aler2, 2))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.GoUpOverTime, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.GoUpOverTime, (ushort)ID);


            if (On(DevStatus.Aler2, 3))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.GoDownOverTime, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.GoDownOverTime, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.BackTakeCannotDo, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.BackTakeCannotDo, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.FrontGiveCannotDo, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.FrontGiveCannotDo, (ushort)ID);
            }


            if (On(DevStatus.Aler2, 6))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Back2FerryCannotDo, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Back2FerryCannotDo, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 7))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Front2FerryCannotDo, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Front2FerryCannotDo, (ushort)ID);
            }
        }
        private void Alert3()
        {
            if (DevStatus.Aler3 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 3);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler3, 0))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Back2SortCannotDo, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Back2SortCannotDo, (ushort)ID);

            if (On(DevStatus.Aler3, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Front2PointCannotDo, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Front2PointCannotDo, (ushort)ID);

            if (On(DevStatus.Aler3, 2))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.Back2PointCannotDo, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.Back2PointCannotDo, (ushort)ID);


            if (On(DevStatus.Aler3, 3))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.NotGoodToGoUp, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.NotGoodToGoUp, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.SortTaskOverTime, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.SortTaskOverTime, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.TriggerEmergencyStop, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.TriggerEmergencyStop, (ushort)ID);
            }
        }

        private void Alert4()
        {
            if (DevStatus.Aler4 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 4);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler4, 0))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.CheckUpAndLoadIsNormal, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.CheckUpAndLoadIsNormal, (ushort)ID);

            if (On(DevStatus.Aler4, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.CheckGoDecelerateIsNormal, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.CheckGoDecelerateIsNormal, (ushort)ID);

        }

        private bool On(byte b, byte p)
        {
            return (b >> p) % 2 == 1;
        }

        #endregion

        #region[运输车-逻辑警告]

        internal void SetAlert(CarrierAlertE type, uint trackid, bool isalert)
        {
            switch (type)
            {
                case CarrierAlertE.GiveMissTrack:
                    DevConfig.a_givemisstrack = isalert;
                    break;
                case CarrierAlertE.TakeMissTrack:
                    DevConfig.a_takemisstrack = isalert;
                    break;
            }
            DevConfig.a_alert_track = trackid;
            PubMaster.Mod.DevConfigSql.EditCarrierAlert(DevConfig, type);
        }

        //特殊逻辑报警
        internal void CheckLogicAlert()
        {
            if (DevConfig.a_givemisstrack)
            {
                if (PubMaster.Track.IsStoreType(TrackId))
                {
                    PubMaster.Track.SetTrackStatus(DevConfig.a_alert_track, TrackStatusE.启用, out string _);
                    PubMaster.Track.SetTrackAlert(DevConfig.a_alert_track, 0, 0, TrackAlertE.正常);
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierGiveMissTrack, (ushort)ID);
                    DevConfig.a_givemisstrack = false;
                    DevConfig.a_alert_track = 0;
                    PubMaster.Mod.DevConfigSql.EditCarrierAlert(DevConfig, CarrierAlertE.GiveMissTrack);
                }
            }

            if (DevConfig.a_takemisstrack)
            {

            }

        }

        internal bool IsLogicAlert()
        {
            return DevConfig.a_givemisstrack || DevConfig.a_takemisstrack;
        }

        #endregion
    }
}
