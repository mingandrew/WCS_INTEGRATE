using enums;
using enums.track;
using enums.warning;
using module.device;
using module.deviceconfig;
using module.track;
using resource;
using socket.tcp;
using task.task;

namespace task.device
{
    public class CarrierTask : TaskBase
    {
        #region[属性]
        private uint currenttrackid;
        /// <summary>
        /// 当前运输车所在轨道ID
        /// </summary>
        public uint CurrentTrackId 
        {
            get => currenttrackid;
            set
            {
                if (currenttrackid != value)
                {
                    try
                    {
                        string log = string.Format("【切换轨道】源[ {0} ], 新[ {1} ], 当前[ {2}^{3} ]",
                            PubMaster.Track.GetTrackLogInfo(currenttrackid),
                            PubMaster.Track.GetTrackLogInfo(value),
                            DevStatus?.CurrentSite,
                            DevStatus?.CurrentPoint);
                        DevTcp.AddStatusLog(log);
                    }
                    catch { }
                    currenttrackid = value;
                }
            }
        }

        /// <summary>
        /// 运输车目的轨道ID
        /// </summary>
        public uint TargetTrackId { set; get; }

        /// <summary>
        /// 上一次的摆渡车轨道id
        /// </summary>
        public uint LastTrackId { set; get; }

        /// <summary>
        /// 【地标需要转移】小车卸货在摆渡车轨道
        /// </summary>
        public bool IsUnloadInFerry { set; get; }
        /// <summary>
        /// 小车类型
        /// </summary>
        public CarrierTypeE CarrierType
        {
            get => Device.CarrierType;
        }

        #region 位置信息

        /// <summary>
        /// 所在位置
        /// </summary>
        public DevCarrierPositionE Position
        {
            get => DevStatus?.Position ?? DevCarrierPositionE.异常;
        }

        /// <summary>
        /// 当前RFID
        /// </summary>
        public ushort CurrentSite
        {
            get => DevStatus?.CurrentSite ?? 0;
        }

        /// <summary>
        /// 当前坐标
        /// </summary>
        public ushort CurrentPoint
        {
            get => DevStatus?.CurrentPoint ?? 0;
        }

        /// <summary>
        /// 目的RFID
        /// </summary>
        public ushort TargetSite
        {
            get => DevStatus?.TargetSite ?? 0;
        }

        /// <summary>
        /// 目的坐标
        /// </summary>
        public ushort TargetPoint
        {
            get => DevStatus?.TargetPoint ?? 0;
        }

        /// <summary>
        /// 取货RFID
        /// </summary>
        public ushort TakePoint
        {
            get => DevStatus?.TakeSite ?? 0;
        }

        /// <summary>
        /// 取货坐标
        /// </summary>
        public ushort TakeSite
        {
            get => DevStatus?.TakePoint ?? 0;
        }

        /// <summary>
        /// 卸货RFID
        /// </summary>
        public ushort GivePoint
        {
            get => DevStatus?.GiveSite ?? 0;
        }

        /// <summary>
        /// 卸货坐标
        /// </summary>
        public ushort GiveSite
        {
            get => DevStatus?.GivePoint ?? 0;
        }

        #endregion

        #region 状态信息

        /// <summary>
        /// 设备状态
        /// </summary>
        public DevCarrierStatusE Status
        {
            get => DevStatus?.DeviceStatus ?? DevCarrierStatusE.异常;
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public DevOperateModeE OperateMode
        {
            get => DevStatus?.OperateMode ?? DevOperateModeE.手动;
        }

        /// <summary>
        /// 载货状态
        /// </summary>
        public DevCarrierLoadE Load
        {
            get => DevStatus?.LoadStatus ?? DevCarrierLoadE.异常;
        }

        /// <summary>
        /// 综合判断有货状态
        /// </summary>
        /// <returns></returns>
        public bool IsLoad()
        {
            return Load == DevCarrierLoadE.有货
                || (Load == DevCarrierLoadE.异常 
                    && TakePoint > 0 
                    && TakeSite > 0);
        }

        /// <summary>
        /// 综合判断无货状态
        /// </summary>
        /// <returns></returns>
        public bool IsNotLoad()
        {
            return Load == DevCarrierLoadE.无货
                || (Load == DevCarrierLoadE.异常
                    && GivePoint > 0
                    && GiveSite > 0);
        }

        /// <summary>
        /// 完成指令
        /// </summary>
        public DevCarrierOrderE FinishOrder
        {
            get => DevStatus?.FinishOrder ?? DevCarrierOrderE.异常;
        }

        /// <summary>
        /// 当前指令
        /// </summary>
        public DevCarrierOrderE CurrentOrder
        {
            get => DevStatus?.CurrentOrder ?? DevCarrierOrderE.异常;
        }

        public bool IsConnect
        {
            get => DevTcp?.IsConnected ?? false;
        }
        #endregion

        #endregion

        #region[构造/启动/停止]

        public CarrierTcp DevTcp { set; get; }
        public DevCarrier DevStatus { set; get; }
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

        /// <summary>
        /// 清空信息
        /// </summary>
        internal void ClearDevStatus()
        {
            if (DevStatus != null)
            {
                DevStatus.CurrentSite = 0;
                DevStatus.CurrentPoint = 0;
                DevStatus.TargetSite = 0;
                DevStatus.TargetPoint = 0;
                DevStatus.CurrentOrder = DevCarrierOrderE.无;
                DevStatus.FinishOrder = DevCarrierOrderE.无;
                CurrentTrackId = 0;
                TargetTrackId = 0;
            }
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
        internal void DoOrder(CarrierActionOrder cao)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.执行指令, cao.Order, cao.CheckTra, cao.ToRFID, cao.ToSite, cao.OverRFID, cao.OverSite, cao.MoveCount);
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
            DevTcp?.SendCmd(DevCarrierCmdE.终止指令);
        }

        internal void DoStopNow()
        {
            DevTcp?.SendCmdNow(DevCarrierCmdE.终止指令);
        }

        internal void DoTask(DevCarrierTaskE task, DevCarrierSizeE oversize)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.执行指令);
        }

        #endregion

        #region[更新轨道信息]

        internal void UpdateInfo()
        {
            CurrentTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, CurrentSite, CurrentPoint);

            TargetTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, TargetSite, TargetPoint);

            DevStatus.CurrentTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, CurrentSite, CurrentPoint);

            DevStatus.TargetTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, TargetSite, TargetPoint);
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
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X0, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X0, (ushort)ID);

            if (On(DevStatus.Aler1, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X1, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X1, (ushort)ID);

            if (On(DevStatus.Aler1, 2))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X2, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X2, (ushort)ID);


            if (On(DevStatus.Aler1, 3))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X3, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X3, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X4, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X4, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X5, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X5, (ushort)ID);
            }


            if (On(DevStatus.Aler1, 6))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X6, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X6, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 7))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA1X7, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X7, (ushort)ID);
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
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X0, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X0, (ushort)ID);

            if (On(DevStatus.Aler2, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X1, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X1, (ushort)ID);

            if (On(DevStatus.Aler2, 2))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X2, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X2, (ushort)ID);


            if (On(DevStatus.Aler2, 3))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X3, (ushort)ID, 2);
                //1.将大于运输车当前坐标值的库存的实际坐标值删除掉
                //2.判断是否还有库存，没有则轨道状态变为空
                //3.运输车发终止任务
                PubMaster.Goods.DeleteStockBySite(CurrentTrackId, CurrentPoint);
                if (PubMaster.Goods.IsTrackStockEmpty(CurrentTrackId))
                {
                    PubMaster.Track.UpdateStockStatus(CurrentTrackId, TrackStockStatusE.空砖, "运输车检测无砖,自动调整轨道为空");
                    PubMaster.Goods.ClearTrackEmtpy(CurrentTrackId);
                    PubTask.TileLifter.ReseTileCurrentTake(CurrentTrackId);
                    PubMaster.Track.AddTrackLog((ushort)AreaId, ID, CurrentTrackId, TrackLogE.空轨道, "运输车检测无砖");
                }
                //DoStop();
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X3, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X4, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X4, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X5, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X5, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 6))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X6, (ushort)ID, 2);
                //1.将大于运输车当前坐标值的库存的实际坐标值删除掉
                //2.判断是否还有库存，没有则轨道状态变为空
                //3.运输车发终止任务
                PubMaster.Goods.DeleteStockBySite(CurrentTrackId, CurrentPoint);
                if (PubMaster.Goods.IsTrackStockEmpty(CurrentTrackId))
                {
                    PubMaster.Track.UpdateStockStatus(CurrentTrackId, TrackStockStatusE.空砖, "运输车倒库无砖,自动调整轨道为空");
                    PubMaster.Goods.ClearTrackEmtpy(CurrentTrackId);
                    PubTask.TileLifter.ReseTileCurrentTake(CurrentTrackId);
                    PubMaster.Track.AddTrackLog((ushort)AreaId, ID, CurrentTrackId, TrackLogE.空轨道, "运输车倒库无砖");
                }
                //DoStop();
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X6, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 7))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA2X7, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X7, (ushort)ID);
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
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X0, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X0, (ushort)ID);

            if (On(DevStatus.Aler3, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X1, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X1, (ushort)ID);

            if (On(DevStatus.Aler3, 2))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X2, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X2, (ushort)ID);


            if (On(DevStatus.Aler3, 3))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X3, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X3, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X4, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X4, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X5, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X5, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 6))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X6, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X6, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 7))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA3X7, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X7, (ushort)ID);
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
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X0, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X0, (ushort)ID);

            if (On(DevStatus.Aler4, 1))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X1, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X1, (ushort)ID);

            if (On(DevStatus.Aler4, 2))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X2, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X2, (ushort)ID);


            if (On(DevStatus.Aler4, 3))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X3, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X3, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 4))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X4, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X4, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 5))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X5, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X5, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 6))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X6, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X6, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 7))
            {
                PubMaster.Warn.AddCarrierWarn(CarrierWarnE.WarningA4X7, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X7, (ushort)ID);
            }
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
                if (PubMaster.Track.IsStoreType(CurrentTrackId))
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
