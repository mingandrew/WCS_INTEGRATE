﻿using enums;
using enums.track;
using enums.warning;
using module.device;
using module.deviceconfig;
using module.track;
using resource;
using socket.tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task.task;

namespace task.device
{
    public class CarrierTask : TaskBase
    {
        #region[属性]
        public ushort CurrentTrackLine { set; get; }
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
                    CurrentTrackLine = PubMaster.Track.GetTrackLine(value);
                }
            }
        }

        /// <summary>
        /// 运输车目的轨道ID
        /// </summary>
        public uint TargetTrackId { set; get; }

        /// <summary>
        /// 即将前往的轨道ID
        /// </summary>
        public uint OnGoingTrackId
        {
            get => ongoingtrackid;
            set
            {
                if (ongoingtrackid != value && value == 0)
                {
                    try
                    {
                        DevTcp.AddStatusLog("【到达目标轨道】");
                    }
                    catch { }
                }
                ongoingtrackid = value;
            }
        }
        private uint ongoingtrackid;
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

        /// <summary>
        /// 是否复位脉冲写入中（初始化）
        /// </summary>
        public bool IsResetWriting { set; get; }

        #region 位置信息

        /// <summary>
        /// 所在位置
        /// </summary>
        public DevCarrierPositionE Position
        {
            get => DevStatus?.Position ?? DevCarrierPositionE.异常;
        }

        /// <summary>
        /// 当前RFID（轨道编号）
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
        /// 目的RFID（轨道编号）
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
        /// 取货RFID（轨道编号）
        /// </summary>
        public ushort TakeSite
        {
            get => DevStatus?.TakeSite ?? 0;
        }

        /// <summary>
        /// 取货坐标
        /// </summary>
        public ushort TakePoint
        {
            get => DevStatus?.TakePoint ?? 0;
        }

        /// <summary>
        /// 卸货RFID（轨道编号）
        /// </summary>
        public ushort GiveSite
        {
            get => DevStatus?.GiveSite ?? 0;
        }

        /// <summary>
        /// 卸货坐标
        /// </summary>
        public ushort GivePoint
        {
            get => DevStatus?.GivePoint ?? 0;
        }

        /// <summary>
        /// 运输车正在执行的任务
        /// </summary>
        public DevCarrierOrderE OnGoingOrder
        {
            get => _ongoingorder;
            set => _ongoingorder = value;
        }

        /// <summary>
        /// 更新运输车当前任务指令
        /// </summary>
        /// <param name="order"></param>
        /// <param name="transid"></param>
        /// <param name="memo"></param>
        public void SetOnGoingOrderWithMemo(DevCarrierOrderE order, uint transid, string memo = "")
        {
            if (_ongoingorder != order)
            {
                if (order != DevCarrierOrderE.无)
                {
                    DevTcp.AddStatusLog(string.Format("【发送任务】任务[ {0} ], 指令[ {1} ], 备注[ {2} ]", transid, order, memo));
                }
                else
                {
                    DevTcp.AddStatusLog(string.Format("【完成重置】[ {0} ]", order));
                }
            }
            _ongoingorder = order;
        }



        private DevCarrierOrderE _ongoingorder = DevCarrierOrderE.无;

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
            return Load == DevCarrierLoadE.有货;
                //|| (Load == DevCarrierLoadE.异常
                //    && TakeSite > 0
                //    && TakePoint > 0);
        }

        /// <summary>
        /// 综合判断无货状态
        /// </summary>
        /// <returns></returns>
        public bool IsNotLoad()
        {
            return Load == DevCarrierLoadE.无货;
                //|| (Load == DevCarrierLoadE.异常
                //    && GiveSite > 0
                //    && GivePoint > 0);
        }

        /// <summary>
        /// 最后完成的指令
        /// </summary>
        public DevCarrierOrderE FinishOrder
        {
            get => DevStatus?.FinishOrder ?? DevCarrierOrderE.异常;
        }

        /// <summary>
        /// 正在执行的指令
        /// </summary>
        public DevCarrierOrderE CurrentOrder
        {
            get => DevStatus?.CurrentOrder ?? DevCarrierOrderE.异常;
        }

        /// <summary>
        /// 判断运输车是否无执行的指令（空闲状态）
        /// </summary>
        public bool IsNotDoingTask
        {
            get => (OnGoingOrder == DevCarrierOrderE.无 || OnGoingOrder == DevCarrierOrderE.终止指令)
                && (CurrentOrder == DevCarrierOrderE.无 || CurrentOrder == DevCarrierOrderE.终止指令);
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
        public DevCarrierAlert DevAlert { set; get; }

        public CarrierTask() : base()
        {
            DevStatus = new DevCarrier();
            DevConfig = new ConfigCarrier();
            DevAlert = new DevCarrierAlert();
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
                DoQuery(); // 开始连接查询一次
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
                DevStatus.Position = DevCarrierPositionE.异常;
                CurrentTrackId = 0;
                TargetTrackId = 0;
                LastTrackId = 0;
                OnGoingTrackId = 0;
                OnGoingOrder = DevCarrierOrderE.无;
            }
        }

        #endregion

        #region[发送指令]

        /// <summary>
        /// 查询指令
        /// </summary>
        internal void DoQuery()
        {
            if (IsResetWriting) return;

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
        internal void DoOrder(CarrierActionOrder cao, uint transid, string memo = null)
        {
            if (IsResetWriting) return;

            OnGoingTrackId = cao.ToTrackId;
            SetOnGoingOrderWithMemo(cao.Order, transid, memo);

            if (cao.Order == DevCarrierOrderE.往前倒库 || cao.Order == DevCarrierOrderE.往后倒库)
            {
                if (cao.MoveCount == 0)
                {
                    cao.MoveCount = 1;
                }
            }
            DevTcp?.SendCmd(DevCarrierCmdE.执行指令, (byte)cao.Order, cao.CheckTra, cao.ToRFID, cao.ToPoint, cao.OverRFID, cao.OverPoint, cao.MoveCount);
        }

        /// <summary>
        /// 设置复位点 by ResetID
        /// </summary>
        /// <param name="point">复位序号</param>
        /// <param name="pos">坐标</param>
        internal void DoResetSiteByPoint(ushort point, ushort pos)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.复位操作, (byte)CarrierResetE.写入, 0, 0, pos, 0, 0, (byte)point);
        }

        /// <summary>
        /// 查询复位点
        /// </summary>
        /// <param name="point">复位序号</param>
        internal void DoSelectResetSite(ushort point)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.复位操作, (byte)CarrierResetE.查询, 0, 0, 0, 0, 0, (byte)point);
        }

        /// <summary>
        /// 回复清除复位点
        /// </summary>
        /// <param name="ID">复位序号</param>
        internal void DoClearReset()
        {
            if (IsResetWriting) return;

            DevTcp?.SendCmd(DevCarrierCmdE.复位操作);
        }

        /// <summary>
        /// 移至复位标志点
        /// </summary>
        /// <param name="ID">复位序号</param>
        internal void DoMoveToResetPoint(CarrierResetE cr)
        {
            if (IsResetWriting) return;

            DevTcp?.SendCmd(DevCarrierCmdE.复位操作, (byte)cr, 0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="point">复位号码</param>
        /// <param name="Code">轨道编号</param>
        /// <param name="cr">复位操作</param>
        internal void DoRenew(ushort point, ushort Code, CarrierResetE cr)
        {
            DevTcp?.SendCmd(DevCarrierCmdE.复位操作, (byte)cr, 0, Code, 0, 0, 0, (byte)point);
        }

        /// <summary>
        /// 终止指令
        /// </summary>
        internal void DoStop(uint tranid, string memo)
        {
            OnGoingTrackId = 0;
            SetOnGoingOrderWithMemo(DevCarrierOrderE.终止指令, tranid, memo);

            DevTcp?.SendCmd(DevCarrierCmdE.执行指令, (byte)DevCarrierOrderE.终止指令, 0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// 接收-回复
        /// </summary>
        internal void DoReply()
        {
            DevTcp?.SendCmd(DevCarrierCmdE.接收回复, DevStatus.MarkCode);
        }

        #endregion

        #region[更新轨道信息]

        internal void UpdateInfo()
        {
            CurrentTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, CurrentSite, CurrentPoint);

            TargetTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, TargetSite, TargetPoint);

            DevStatus.CurrentTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, CurrentSite, CurrentPoint);

            DevStatus.TargetTrackId = PubMaster.Track.GetTrackIdForCarrier((ushort)AreaId, TargetSite, TargetPoint);

            //重置小车执行任务
            if (OnGoingOrder != DevCarrierOrderE.无
                && ((Status == DevCarrierStatusE.停止 && CurrentOrder == DevCarrierOrderE.无) // ∵同类型会终止 ∴不会连续2个同指令
                    || (OperateMode == DevOperateModeE.手动 && (CurrentOrder == DevCarrierOrderE.无 || CurrentOrder == DevCarrierOrderE.终止指令))) // 仅判断手动情况的终止
                )
            {
                OnGoingTrackId = 0;
                OnGoingOrder = DevCarrierOrderE.无;
            }

            if (CurrentTrackId == OnGoingTrackId)
            {
                OnGoingTrackId = 0;
            }
            else if (OnGoingTrackId != 0
                && InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库)
                && PubMaster.Track.IsBrotherTrack(CurrentTrackId, OnGoingTrackId))
            {
                OnGoingTrackId = 0;
            }
        }

        #endregion

        #region[检查报警]

        /// <summary>
        /// 检查报警
        /// </summary>
        public void CheckAlert()
        {
            Alert1();
            Alert2();
            Alert3();
            Alert4();
            Alert5();
            Alert6();
            Alert7();
            Alert8();
            Alert9();
            Alert10();

            SetAlertForSystem();
        }

        private void Alert1()
        {
            if (DevStatus.Aler1 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 1);
                DevAlert.ResetAler1();
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler1, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X0, (ushort)ID, 1);
                DevAlert.SetAlert(0, 0, true);
            }
            else 
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X0, (ushort)ID);
                DevAlert.SetAlert(0, 0, false);
            }

            if (On(DevStatus.Aler1, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X1, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X1, (ushort)ID);

            if (On(DevStatus.Aler1, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X2, (ushort)ID, 1);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X2, (ushort)ID);


            if (On(DevStatus.Aler1, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X3, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X3, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X4, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X4, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X5, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X5, (ushort)ID);
            }


            if (On(DevStatus.Aler1, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X6, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA1X6, (ushort)ID);
            }

            if (On(DevStatus.Aler1, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA1X7, (ushort)ID, 1);
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
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X0, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X0, (ushort)ID);

            if (On(DevStatus.Aler2, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X1, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X1, (ushort)ID);

            if (On(DevStatus.Aler2, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X2, (ushort)ID, 2);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X2, (ushort)ID);


            if (On(DevStatus.Aler2, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X3, (ushort)ID, 2);

                //PubMaster.Track.AddTrackLog((ushort)AreaId, ID, CurrentTrackId, TrackLogE.空轨道, Device.name + "运输车取砖无砖");

                //1.将大于运输车当前坐标值的库存的实际坐标值删除掉
                //2.判断是否还有库存，没有则轨道状态变为空
                //3.运输车发终止任务
                //PubMaster.Goods.DeleteStockBySite(CurrentTrackId, CurrentPoint, Device.name + "运输车检测无砖,自动调整轨道为空");
                //if (PubMaster.Goods.IsTrackStockEmpty(CurrentTrackId))
                //{
                //    PubMaster.Track.UpdateStockStatus(CurrentTrackId, TrackStockStatusE.空砖, Device.name + "运输车检测无砖,自动调整轨道为空");
                //    PubMaster.Goods.ClearTrackEmtpy(CurrentTrackId);
                //    PubTask.TileLifter.ReseUpTileCurrentTake(CurrentTrackId);
                //    PubMaster.Track.AddTrackLog((ushort)AreaId, ID, CurrentTrackId, TrackLogE.空轨道, Device.name + "运输车检测无砖");
                //}
                //else
                //{
                //    PubMaster.Goods.CheckStockTop(CurrentTrackId);
                //}
                //DoStop();
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X3, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X4, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X4, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X5, (ushort)ID, 2);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X5, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X6, (ushort)ID, 2);

                //PubMaster.Track.AddTrackLog((ushort)AreaId, ID, CurrentTrackId, TrackLogE.空轨道, Device.name + "运输车倒库无砖");

                //1.将大于运输车当前坐标值的库存的实际坐标值删除掉
                //2.判断是否还有库存，没有则轨道状态变为空
                //3.运输车发终止任务

                //PubMaster.Goods.DeleteStockBySite(CurrentTrackId, CurrentPoint, Device.name + "运输车倒库无砖,自动调整轨道为空");
                //if (PubMaster.Goods.IsTrackStockEmpty(CurrentTrackId))
                //{
                //    PubMaster.Track.UpdateStockStatus(CurrentTrackId, TrackStockStatusE.空砖, Device.name + "运输车倒库无砖,自动调整轨道为空");
                //    PubMaster.Goods.ClearTrackEmtpy(CurrentTrackId);
                //    PubTask.TileLifter.ReseUpTileCurrentTake(CurrentTrackId);
                //    PubMaster.Track.AddTrackLog((ushort)AreaId, ID, CurrentTrackId, TrackLogE.空轨道, Device.name + "运输车倒库无砖");
                //}
                //else
                //{
                //    PubMaster.Goods.CheckStockTop(CurrentTrackId);
                //}

                //DoStop();
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA2X6, (ushort)ID);
            }

            if (On(DevStatus.Aler2, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA2X7, (ushort)ID, 2);
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
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X0, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X0, (ushort)ID);

            if (On(DevStatus.Aler3, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X1, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X1, (ushort)ID);

            if (On(DevStatus.Aler3, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X2, (ushort)ID, 3);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X2, (ushort)ID);


            if (On(DevStatus.Aler3, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X3, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X3, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X4, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X4, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X5, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X5, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X6, (ushort)ID, 3);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X6, (ushort)ID);
            }

            if (On(DevStatus.Aler3, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA3X7, (ushort)ID, 3);
                DevAlert.SetAlert(2, 7, true);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA3X7, (ushort)ID);
                DevAlert.SetAlert(2, 7, true);
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
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X0, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X0, (ushort)ID);

            if (On(DevStatus.Aler4, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X1, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X1, (ushort)ID);

            if (On(DevStatus.Aler4, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X2, (ushort)ID, 4);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X2, (ushort)ID);


            if (On(DevStatus.Aler4, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X3, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X3, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X4, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X4, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X5, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X5, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X6, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X6, (ushort)ID);
            }

            if (On(DevStatus.Aler4, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA4X7, (ushort)ID, 4);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA4X7, (ushort)ID);
            }
        }

        private void Alert5()
        {
            if (DevStatus.Aler5 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 5);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler5, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X0, (ushort)ID, 5);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X0, (ushort)ID);

            if (On(DevStatus.Aler5, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X1, (ushort)ID, 5);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X1, (ushort)ID);

            if (On(DevStatus.Aler5, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X2, (ushort)ID, 5);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X2, (ushort)ID);


            if (On(DevStatus.Aler5, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X3, (ushort)ID, 5);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X3, (ushort)ID);
            }

            if (On(DevStatus.Aler5, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X4, (ushort)ID, 5);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X4, (ushort)ID);
            }

            if (On(DevStatus.Aler5, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X5, (ushort)ID, 5);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X5, (ushort)ID);
            }

            if (On(DevStatus.Aler5, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X6, (ushort)ID, 5);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X6, (ushort)ID);
            }

            if (On(DevStatus.Aler5, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA5X7, (ushort)ID, 5);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA5X7, (ushort)ID);
            }
        }

        private void Alert6()
        {
            if (DevStatus.Aler6 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 6);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler6, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X0, (ushort)ID, 6);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X0, (ushort)ID);

            if (On(DevStatus.Aler6, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X1, (ushort)ID, 6);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X1, (ushort)ID);

            if (On(DevStatus.Aler6, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X2, (ushort)ID, 6);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X2, (ushort)ID);


            if (On(DevStatus.Aler6, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X3, (ushort)ID, 6);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X3, (ushort)ID);
            }

            if (On(DevStatus.Aler6, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X4, (ushort)ID, 6);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X4, (ushort)ID);
            }

            if (On(DevStatus.Aler6, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X5, (ushort)ID, 6);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X5, (ushort)ID);
            }

            if (On(DevStatus.Aler6, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X6, (ushort)ID, 6);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X6, (ushort)ID);
            }

            if (On(DevStatus.Aler6, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA6X7, (ushort)ID, 6);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA6X7, (ushort)ID);
            }
        }

        private void Alert7()
        {
            if (DevStatus.Aler7 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 7);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler7, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X0, (ushort)ID, 7);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X0, (ushort)ID);

            if (On(DevStatus.Aler7, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X1, (ushort)ID, 7);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X1, (ushort)ID);

            if (On(DevStatus.Aler7, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X2, (ushort)ID, 7);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X2, (ushort)ID);


            if (On(DevStatus.Aler7, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X3, (ushort)ID, 7);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X3, (ushort)ID);
            }

            if (On(DevStatus.Aler7, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X4, (ushort)ID, 7);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X4, (ushort)ID);
            }

            if (On(DevStatus.Aler7, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X5, (ushort)ID, 7);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X5, (ushort)ID);
            }

            if (On(DevStatus.Aler7, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X6, (ushort)ID, 7);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X6, (ushort)ID);
            }

            if (On(DevStatus.Aler7, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA7X7, (ushort)ID, 7);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA7X7, (ushort)ID);
            }
        }

        private void Alert8()
        {
            if (DevStatus.Aler8 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 8);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler8, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X0, (ushort)ID, 8);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X0, (ushort)ID);

            if (On(DevStatus.Aler8, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X1, (ushort)ID, 8);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X1, (ushort)ID);

            if (On(DevStatus.Aler8, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X2, (ushort)ID, 8);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X2, (ushort)ID);


            if (On(DevStatus.Aler8, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X3, (ushort)ID, 8);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X3, (ushort)ID);
            }

            if (On(DevStatus.Aler8, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X4, (ushort)ID, 8);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X4, (ushort)ID);
            }

            if (On(DevStatus.Aler8, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X5, (ushort)ID, 8);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X5, (ushort)ID);
            }

            if (On(DevStatus.Aler8, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X6, (ushort)ID, 8);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X6, (ushort)ID);
            }

            if (On(DevStatus.Aler8, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA8X7, (ushort)ID, 8);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA8X7, (ushort)ID);
            }
        }

        private void Alert9()
        {
            if (DevStatus.Aler9 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 9);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler9, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X0, (ushort)ID, 9);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X0, (ushort)ID);

            if (On(DevStatus.Aler9, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X1, (ushort)ID, 9);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X1, (ushort)ID);

            if (On(DevStatus.Aler9, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X2, (ushort)ID, 9);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X2, (ushort)ID);


            if (On(DevStatus.Aler9, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X3, (ushort)ID, 9);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X3, (ushort)ID);
            }

            if (On(DevStatus.Aler9, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X4, (ushort)ID, 9);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X4, (ushort)ID);
            }

            if (On(DevStatus.Aler9, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X5, (ushort)ID, 9);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X5, (ushort)ID);
            }

            if (On(DevStatus.Aler9, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X6, (ushort)ID, 9);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X6, (ushort)ID);
            }

            if (On(DevStatus.Aler9, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA9X7, (ushort)ID, 9);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA9X7, (ushort)ID);
            }
        }

        private void Alert10()
        {
            if (DevStatus.Aler10 == 0)
            {
                PubMaster.Warn.RemoveCarrierWarn((ushort)ID, 10);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Aler10, 0))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X0, (ushort)ID, 10);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X0, (ushort)ID);

            if (On(DevStatus.Aler10, 1))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X1, (ushort)ID, 10);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X1, (ushort)ID);

            if (On(DevStatus.Aler10, 2))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X2, (ushort)ID, 10);
            }
            else PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X2, (ushort)ID);


            if (On(DevStatus.Aler10, 3))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X3, (ushort)ID, 10);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X3, (ushort)ID);
            }

            if (On(DevStatus.Aler10, 4))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X4, (ushort)ID, 10);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X4, (ushort)ID);
            }

            if (On(DevStatus.Aler10, 5))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X5, (ushort)ID, 10);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X5, (ushort)ID);
            }

            if (On(DevStatus.Aler10, 6))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X6, (ushort)ID, 10);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X6, (ushort)ID);
            }

            if (On(DevStatus.Aler10, 7))
            {
                PubMaster.Warn.AddCarrierWarn(AreaId, Line, CarrierWarnE.WarningA10X7, (ushort)ID, 10);
            }
            else
            {
                PubMaster.Warn.RemoveCarrierWarn(CarrierWarnE.WarningA10X7, (ushort)ID);
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

        internal bool InTask(params DevCarrierOrderE[] order)
        {
            //if (CurrentOrder == FinishOrder || CurrentOrder == DevCarrierOrderE.无)
            if (CurrentOrder == DevCarrierOrderE.无) // 无 就是完成
            {
                return order.Contains(OnGoingOrder);
            }
            return order.Contains(CurrentOrder) || order.Contains(OnGoingOrder);
        }

        internal bool NotInTask(params DevCarrierOrderE[] order)
        {
            return !InTask(order);
        }

        // c.TargetTrackId == trackid || c.CurrentTrackId == trackid || c.OnGoingTrackId == trackid
        internal bool InTrack(uint trackid)
        {
            return CurrentTrackId == trackid || TargetTrackId == trackid || OnGoingTrackId == trackid;
        }

        internal bool NotInTrack(uint trackid)
        {
            return !InTrack(trackid);
        }

        /// <summary>
        /// 是否处于 初始化/寻点 指令中
        /// </summary>
        /// <returns></returns>
        internal bool IsResetWork()
        {
            return IsResetWriting || InTask(DevCarrierOrderE.初始化, DevCarrierOrderE.寻点);
        }

        /// <summary>
        /// 系统判断 报警提示
        /// </summary>
        internal void SetAlertForSystem()
        {
            // 失去位置
            Track currentTrack = PubMaster.Track.GetTrack(CurrentTrackId);
            if (currentTrack == null || CurrentTrackId == 0 || CurrentTrackId.Equals(0) || CurrentTrackId.CompareTo(0) == 0)
            {
                PubMaster.Warn.AddDevWarn(AreaId, Line, WarningTypeE.CarrierNoLocation, (ushort)ID);
            }
            else
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierNoLocation, (ushort)ID);
            }

            // 初始化/寻点指令
            if (IsResetWork())
            {
                PubMaster.Warn.AddDevWarn(AreaId, Line, WarningTypeE.CarrierIsInResetWork, (ushort)ID);
            }
            else
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierIsInResetWork, (ushort)ID);
            }
        }
        #endregion

        #region [数据信息]

        /// <summary>
        /// 获取当前运输车信息
        /// 运动/指令/位置
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            return string.Format("运输车[ {0} ], 设备状态[ {1} ], 位置状态[ {2} ], 正执行指令[ {3} ], 已完成指令[ {4} ], 记录指令[ {5} ], 当前轨道[ {6} ], 目的轨道[ {7} ], 记录轨道[ {8} ]",
                Device.name, Status, Position, CurrentOrder, FinishOrder, OnGoingOrder,
                PubMaster.Track.GetTrackName(CurrentTrackId), 
                PubMaster.Track.GetTrackName(TargetTrackId), 
                PubMaster.Track.GetTrackName(OnGoingTrackId));
        }

        #endregion

        #region [条件判断]

        /// <summary>
        /// 检查运输车是否可分配使用
        /// </summary>
        /// <param name="carrier"></param>
        /// <param name="gsize"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CheckCarrierIsUsable(uint gsize, out string result)
        {
            if (PubTask.Trans.HaveInCarrier(ID))
            {
                result = string.Format("{0}已被任务锁定-等待空闲解锁；", Device.name);
                return false;
            }

            if (ConnStatus != SocketConnectStatusE.通信正常)
            {
                result = string.Format("{0}通信不正常-等待恢复通讯；", Device.name);
                return false;
            }

            if (OperateMode == DevOperateModeE.手动)
            {
                result = string.Format("{0}被手动操作中-等待恢复自动；", Device.name);
                return false;
            }

            if (!DevConfig.IsUseGoodsSize(gsize))
            {
                result = string.Format("{0}无法作业{1}的砖；", Device.name, PubMaster.Goods.GetSizeName(gsize));
                return false;
            }

            if (!IsEnable)
            {
                result = string.Format("{0}已被断开通讯-请操作连接通讯；", Device.name);
                return false;
            }

            if (!IsWorking)
            {
                result = string.Format("{0}已被停用-等待恢复启用；", Device.name);
                return false;
            }

            result = "";
            return true;
        }

        #endregion

    }

}
