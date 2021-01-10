using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.goods;
using module.msg;
using module.tiletrack;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using task.task;
using tool.mlog;
using tool.timer;

namespace task.device
{
    public class TileLifterMaster
    {
        #region[字段]
        private MsgAction mMsg;
        private object _objmsg;
        private List<TileLifterTask> DevList { set; get; }
        private readonly object _obj;
        private const byte Site_1 = 1, Site_2 = 2;
        private Thread _mRefresh;
        private bool Refreshing = true;
        private MTimer mTimer;
        private Log mlog;
        private bool isWcsStoping;
        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public TileLifterMaster()
        {
            mlog = (Log)new LogFactory().GetLog("TileLifter", false);
            mTimer = new MTimer();
            _objmsg = new object();
            mMsg = new MsgAction();
            _obj = new object();
            DevList = new List<TileLifterTask>();
            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.TileLifterMsgUpdate, TileLifterMsgUpdate);
        }

        public void Start()
        {
            List<Device> tilelifters = PubMaster.Device.GetTileLifters();
            foreach (Device dev in tilelifters)
            {
                TileLifterTask task = new TileLifterTask
                {
                    Device = dev,
                    DevConfig = PubMaster.DevConfig.GetTileLifter(dev.id)
                };
                task.Start("调度启动开始连接");
                DevList.Add(task);
            }

            if (_mRefresh == null || !_mRefresh.IsAlive || _mRefresh.ThreadState == ThreadState.Aborted)
            {
                _mRefresh = new Thread(Refresh)
                {
                    IsBackground = true
                };
            }

            _mRefresh.Start();

        }

        public void Stop()
        {
            Refreshing = false;
            _mRefresh?.Abort();
            isWcsStoping = true;
            foreach (TileLifterTask task in DevList)
            {
                task.Stop("调度关闭连接停止");
            }
        }

        public void ReStart()
        {

        }

        private void Refresh()
        {
            while (Refreshing)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
                {
                    try
                    {
                        foreach (TileLifterTask task in DevList)
                        {
                            try
                            {
                                if (task.IsEnable)
                                {
                                    task.DoQuery();
                                }

                                #region 同步当前品种/等级
                                if (task.DevConfig.goods_id != task.DevStatus.SetGoods)
                                {
                                    Thread.Sleep(500);
                                    task.DoShift(TileShiftCmdE.变更品种, 0, task.DevConfig.goods_id);
                                }

                                byte level = PubMaster.Goods.GetGoodsLevel(task.DevConfig.goods_id);
                                if (level != task.DevStatus.SetLevel)
                                {
                                    Thread.Sleep(500);
                                    task.DoUpdateLevel(level);
                                }
                                #endregion

                                #region 断线重连

                                ///离线住够长时间，自动断开重连
                                if (task.IsEnable
                                    && task.ConnStatus != SocketConnectStatusE.通信正常
                                    && task.ConnStatus != SocketConnectStatusE.连接成功)
                                {
                                    //离线超过20秒并且没有在主动断开
                                    if (!task.IsDevOfflineInBreak
                                        && task.IsOfflineTimeOver())
                                    {
                                        task.SetDevConnOnBreak(true);
                                        task.Stop("休息5秒断开连接");
                                    }

                                    //主动断开时间超过后，开始重连
                                    if (task.IsDevOfflineInBreak && task.IsInBreakOver())
                                    {
                                        task.SetDevConnOnBreak(false);
                                        task.Start("休息5秒后开始连接");
                                    }
                                }

                                #endregion

                                #region 下砖-转产

                                if (task.Type != DeviceTypeE.下砖机) continue;

                                int count = PubMaster.Dic.GetDtlIntCode("TileLifterShiftCount");
                                switch (task.DevStatus.ShiftStatus)
                                {
                                    case TileShiftStatusE.复位:
                                        #region [复位]
                                        if (task.DevConfig.do_shift)
                                        {
                                            if (!task.DevStatus.ShiftAccept)
                                            {
                                                Thread.Sleep(500);
                                                task.DoShift(TileShiftCmdE.执行转产, (byte)count, task.DevConfig.goods_id);
                                                break;
                                            }
                                        }

                                        if (!task.DevConfig.do_shift)
                                        {
                                            if (task.DevStatus.ShiftAccept)
                                            {
                                                Thread.Sleep(500);
                                                task.DoShift(TileShiftCmdE.复位);
                                                break;
                                            }
                                        }
                                        #endregion
                                        break;
                                    case TileShiftStatusE.转产中:
                                        #region [转产中]
                                        if (task.DevConfig.do_shift)
                                        {
                                            if (!task.DevStatus.ShiftAccept)
                                            {
                                                Thread.Sleep(500);
                                                task.DoShift(TileShiftCmdE.执行转产, (byte)count, task.DevConfig.goods_id);
                                                break;
                                            }
                                        }

                                        if (!task.DevConfig.do_shift)
                                        {
                                            if (task.DevStatus.ShiftAccept)
                                            {
                                                Thread.Sleep(500);
                                                task.DoShift(TileShiftCmdE.复位);
                                                break;
                                            }
                                        }
                                        #endregion
                                        break;
                                    case TileShiftStatusE.完成:
                                        #region [完成]
                                        if (task.DevConfig.do_shift)
                                        {
                                            Thread.Sleep(500);
                                            task.DoShift(TileShiftCmdE.复位);

                                            task.DevConfig.do_shift = false;
                                            task.DevConfig.old_goodid = 0;
                                            PubMaster.DevConfig.SetTileLifterGoods(task.ID, task.DevConfig.goods_id);
                                            break;
                                        }
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }

                                #endregion

                            }
                            catch (Exception e)
                            {
                                mlog.Error(true, e.Message, e);
                            }
                        }
                    }
                    finally { Monitor.Exit(_obj); }
                }
                Thread.Sleep(1000);
            }
        }

        public void DoInv(uint devid, bool isone, DevLifterInvolE type)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    if (isone)
                    {
                        DevList.Find(c => c.ID == devid)?.Do1Invo(type);
                    }
                    else
                    {
                        DevList.Find(c => c.ID == devid)?.Do2Invo(type);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        /// <summary>
        /// 忽略兄弟砖机工位介入
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="isone"></param>
        /// <returns></returns>
        public void DoIgnore(uint devid, bool isone)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == devid);
                    if (task == null) return;
                    if (isone)
                    {
                        task.Ignore_1 = true;
                    }
                    else
                    {
                        task.Ignore_2 = true;
                    }


                }
                finally { Monitor.Exit(_obj); }
            }
        }

        /// <summary>
        /// 发送给砖机离开工位
        /// 1.如果兄弟砖机需要，则不离开
        /// 2.如果兄弟砖机离开，则全部离开
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <param name="leavetrackid"></param>
        /// <returns></returns>
        internal bool DoInvLeave(uint tilelifter_id, uint leavetrackid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == tilelifter_id);
                    if (task != null)
                    {
                        TileLifterTask bro;
                        //离开砖机并且同时离开兄弟砖机
                        if (task.HaveBrother)
                        {
                            bro = DevList.Find(c => c.ID == task.BrotherId);
                        }
                        else
                        {
                            bro = DevList.Find(c => c.BrotherId == task.ID);
                        }

                        if (task.DevConfig.left_track_id == leavetrackid)
                        {
                            if (task.HaveBrother)
                            {
                                if (task.IsInvo_1)
                                {
                                    task.Do1Invo(DevLifterInvolE.离开);
                                }

                                if (bro.IsInvo_1)
                                {
                                    bro.Do1Invo(DevLifterInvolE.离开);
                                }

                                if (!task.IsInvo_1 && !bro.IsInvo_1)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (bro != null && bro.IsNeed_1)
                                {
                                    return true;
                                }

                                if (task.IsInvo_1)
                                {
                                    task.Do1Invo(DevLifterInvolE.离开);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }


                        if (task.DevConfig.right_track_id == leavetrackid)
                        {
                            if (task.HaveBrother)
                            {
                                if (task.IsInvo_2)
                                {
                                    task.Do2Invo(DevLifterInvolE.离开);
                                }

                                if (bro.IsInvo_2)
                                {
                                    bro.Do2Invo(DevLifterInvolE.离开);
                                }

                                if (!task.IsInvo_2 && !bro.IsInvo_2)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (bro != null && bro.IsNeed_2)
                                {
                                    return true;
                                }

                                if (task.IsInvo_2)
                                {
                                    task.Do2Invo(DevLifterInvolE.离开);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        internal bool IsTrackEmtpy(uint tileid, uint trackid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == tileid);
                    if (task != null)
                    {
                        if (task.DevConfig.left_track_id == trackid && task.IsEmpty_1)
                        {
                            return true;
                        }

                        if (task.DevConfig.right_track_id == trackid && task.IsEmpty_2)
                        {
                            return true;
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        public void StartStopTileLifter(uint tileid, bool isstart)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == tileid);
                    if (task != null)
                    {
                        if (isstart)
                        {
                            task.ReSetOffLineTime();
                            if (!task.IsEnable)
                            {
                                task.SetEnable(isstart);
                            }
                            task.Start("手动启动");
                        }
                        else
                        {
                            if (task.IsEnable)
                            {
                                task.SetEnable(isstart);
                            }
                            task.ReSetOfflineBreakTime();
                            task.Stop("手动停止");
                            PubMaster.Warn.RemoveDevWarn((ushort)task.ID);

                            PubTask.Ping.RemovePing(task.Device.ip);
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }

        }

        public void UpdateTileInStrategry(uint id, StrategyInE instrategy, DevWorkTypeE worktype)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == id);
                    if (task != null)
                    {
                        task.InStrategy = instrategy;
                        task.WorkType = worktype;
                        MsgSend(task, task.DevStatus);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void UpdateTileOutStrategry(uint id, StrategyOutE outstrategy, DevWorkTypeE worktype)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == id);
                    if (task != null)
                    {
                        task.OutStrategy = outstrategy;
                        task.WorkType = worktype;
                        MsgSend(task, task.DevStatus);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        #endregion

        #region[获取信息]

        public void GetAllTileLifter()
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                foreach (TileLifterTask task in DevList)
                {
                    MsgSend(task, task.DevStatus);
                }
            }
            finally { Monitor.Exit(_obj); }
        }

        public TileLifterTask GetTileLifter(uint id)
        {
            return DevList.Find(c => c.ID == id);
        }

        public uint GetTileCurrentTake(uint id)
        {
            uint take = 0;
            TileLifterTask task = DevList.Find(c => c.ID == id);
            if (task != null)
            {
                if (!PubMaster.Track.IsEmtpy(task.DevConfig.last_track_id) &&
                    !PubMaster.Track.IsStopUsing(task.DevConfig.last_track_id, TransTypeE.上砖任务))
                {
                    take = task.DevConfig.last_track_id;
                }
                else
                {
                    PubMaster.DevConfig.SetLastTrackId(id, 0);
                }
            }
            return take;
        }

        #endregion

        #region[数据更新]

        private void TileLifterMsgUpdate(SocketMsgMod mod)
        {
            if (isWcsStoping) return;
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        TileLifterTask task = DevList.Find(c => c.ID == mod.ID);
                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is DevTileLifter tilelifter)
                            {
                                task.ReSetRefreshTime();
                                task.DevStatus = tilelifter;
                                CheckDev(task);

                                if (tilelifter.IsUpdate
                                    || mTimer.IsTimeOutAndReset(TimerTag.DevRefreshTimeOut, tilelifter.ID, 10))
                                    MsgSend(task, tilelifter);
                            }
                            CheckConn(task);
                        }
                    }
                    catch (Exception e)
                    {
                        mlog.Error(true, e.Message, e);
                    }
                    finally
                    {
                        Monitor.Exit(_obj);
                    }
                }
            }
        }

        private void CheckConn(TileLifterTask task)
        {
            switch (task.ConnStatus)
            {
                case SocketConnectStatusE.连接成功:
                case SocketConnectStatusE.通信正常:
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.DeviceOffline, (ushort)task.ID);
                    PubTask.Ping.RemovePing(task.Device.ip);
                    break;
                case SocketConnectStatusE.连接中:
                case SocketConnectStatusE.连接断开:
                case SocketConnectStatusE.主动断开:
                    if (task.IsEnable) PubMaster.Warn.AddDevWarn(WarningTypeE.DeviceOffline, (ushort)task.ID);
                    PubTask.Ping.AddPing(task.Device.ip, task.Device.name);
                    break;
            }

            if (task.MConChange || mTimer.IsTimeOutAndReset("ConnRefresh", (int)task.ID, 3))
            {
                MsgSend(task, task.DevStatus);
            }
        }

        public void ReseTileCurrentTake(uint takeid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    foreach (TileLifterTask t in DevList.FindAll(c => c.DevConfig.last_track_id == takeid))
                    {
                        PubMaster.DevConfig.SetLastTrackId(t.ID, 0);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        #endregion

        #region[检查状态/生成交易]

        /// <summary>
        /// 检查上下砖机状态
        /// 1.需求信号
        /// 2.满砖/空砖信号
        /// </summary>
        /// <param name="task"></param>
        private void CheckDev(TileLifterTask task)
        {
            task.SetInTaskStatus(task.IsNeed_1 || task.IsNeed_2);

            if (!task.IsWorking) return;

            #region[检查基础信息]

            if(task.DevConfig == null)
            {
                return;
            }

            if (task.DevConfig.goods_id == 0)
            {
                return;
            }

            //品种是空品种
            if (PubMaster.Goods.IsGoodEmpty(task.DevConfig.goods_id))
            {
                return;
            }

            if (task.DevConfig.left_track_id == 0)
            {
                return;
            }

            #endregion

            #region[工位1有需求]

            if (task.IsNeed_1)
            {
                #region[下砖机-满砖]

                if (task.Type == DeviceTypeE.下砖机)
                {
                    if (task.IsEmpty_1 && task.IsInvo_1)
                    {
                        Thread.Sleep(1000);
                        task.Do1Invo(DevLifterInvolE.离开);
                        return;
                    }

                    if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.下砖)) return;

                    if (PubTask.Trans.HaveInTileTrack(task.DevConfig.left_track_id)) return;

                    #region[介入]

                    if (!task.IsInvo_1)
                    {
                        Thread.Sleep(1000);
                        task.Do1Invo(DevLifterInvolE.介入);
                        return;
                    }

                    #endregion

                    if (!CheckBrotherIsReady(task, false, true)) return;

                    #region[生成入库交易]

                    if (!IsAllowToBeTaskGoods(task.ID, task.DevStatus.Goods1)) return;

                    uint gid = task.DevStatus.Goods1;

                    bool iseffect = CheckInStrategy(task, task.DevConfig.left_track_id, gid);

                    if (!iseffect)
                    {
                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.规格作业:
                                AddAndGetStockId(task.ID, task.DevConfig.left_track_id, gid, task.FullQty, out uint stockid);
                                TileAddInTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, gid, stockid);
                                break;
                            case DevWorkTypeE.轨道作业:

                                break;
                            case DevWorkTypeE.混砖作业:
                                AddAndGetStockId(task.ID, task.DevConfig.left_track_id, gid, task.FullQty, out stockid);
                                AddMixTrackTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, gid, stockid);
                                break;
                        }
                    }

                    #endregion
                }

                #endregion

                #region[上砖机-空砖]

                else if (task.Type == DeviceTypeE.上砖机 && task.IsEmpty_1)
                {
                    if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.上砖)) return;

                    #region[介入]

                    if (!task.IsInvo_1)
                    {
                        Thread.Sleep(1000);
                        task.Do1Invo(DevLifterInvolE.介入);
                        return;
                    }

                    #endregion

                    if (!CheckUpBrotherIsReady(task, true, false)) return;

                    #region[生成出库交易]

                    bool iseffect = CheckOutStrategy(task, task.DevConfig.left_track_id);

                    if (!iseffect)
                    {
                        #region[清空轨道上砖轨道库存]

                        PubMaster.Goods.ClearTrackEmtpy(task.DevConfig.left_track_id);

                        #endregion

                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.规格作业:
                                TileAddOutTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, task.DevConfig.goods_id, task.DevConfig.last_track_id);
                                break;
                            case DevWorkTypeE.轨道作业:
                                TileAddTrackOutTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, task.DevConfig.goods_id);
                                break;
                        }
                    }

                    #endregion

                }

                #endregion

            }
            else
            {
                bool isOK = false;
                switch (task.Type)
                {
                    case DeviceTypeE.上砖机:
                        isOK = !PubTask.Carrier.HaveInTrackAndLoad(task.DevConfig.left_track_id);
                        break;
                    case DeviceTypeE.下砖机:
                        isOK = !PubTask.Carrier.HaveInTrack(task.DevConfig.left_track_id);
                        break;
                    default:
                        break;
                }
                //没有需求但是介入状态 同时:轨道没有车/有车无货
                if (task.IsInvo_1 && isOK
                    && mTimer.IsOver(TimerTag.TileInvoNotNeed, task.ID, Site_1, 15, 10))
                {
                    if (task.HaveBrother)
                    {
                        Thread.Sleep(1000);
                        task.Do1Invo(DevLifterInvolE.离开);
                    }
                    else
                    {
                        TileLifterTask bro = DevList.Find(c => c.BrotherId == task.ID);
                        if (bro == null || (bro != null && !bro.IsNeed_1))
                        {
                            Thread.Sleep(1000);
                            task.Do1Invo(DevLifterInvolE.离开);
                        }
                    }
                }
            }
            #endregion

            #region[工位2有需求]

            if (task.IsNeed_2 && task.IsTwoTrack)
            {
                if (task.DevConfig.right_track_id == 0) return;

                #region[下砖机-满砖]

                if (task.Type == DeviceTypeE.下砖机)
                {
                    if (task.IsEmpty_2 && task.IsInvo_2)
                    {
                        Thread.Sleep(1000);
                        task.Do2Invo(DevLifterInvolE.离开);
                        return;
                    }

                    if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.下砖)) return;

                    if (PubTask.Trans.HaveInTileTrack(task.DevConfig.right_track_id)) return;

                    #region[介入]

                    if (!task.IsInvo_2)
                    {
                        Thread.Sleep(1000);
                        task.Do2Invo(DevLifterInvolE.介入);
                        return;
                    }

                    #endregion

                    if (!CheckBrotherIsReady(task, false, false)) return;

                    #region[生成入库交易]

                    if (!IsAllowToBeTaskGoods(task.ID, task.DevStatus.Goods2)) return;

                    uint gid = task.DevStatus.Goods2;

                    bool iseffect = CheckInStrategy(task, task.DevConfig.right_track_id, gid);

                    if (!iseffect)
                    {
                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.规格作业:
                                AddAndGetStockId(task.ID, task.DevConfig.right_track_id, gid, task.FullQty, out uint stockid);
                                TileAddInTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, gid, stockid);
                                break;
                            case DevWorkTypeE.轨道作业:

                                break;
                            case DevWorkTypeE.混砖作业:
                                AddAndGetStockId(task.ID, task.DevConfig.right_track_id, gid, task.FullQty, out stockid);
                                AddMixTrackTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, gid, stockid);
                                break;
                        }
                    }

                    #endregion
                }

                #endregion

                #region[上砖机-空砖]
                else if (task.Type == DeviceTypeE.上砖机 && task.IsEmpty_2)
                {
                    if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.上砖)) return;

                    #region[介入]

                    if (!task.IsInvo_2)
                    {
                        Thread.Sleep(1000);
                        task.Do2Invo(DevLifterInvolE.介入);
                        return;
                    }

                    #endregion

                    if (!CheckUpBrotherIsReady(task, true, false)) return;

                    #region[生成出库交易]

                    bool iseffect = CheckOutStrategy(task, task.DevConfig.right_track_id);

                    if (!iseffect)
                    {
                        #region[清空轨道上砖轨道库存]

                        PubMaster.Goods.ClearTrackEmtpy(task.DevConfig.right_track_id);

                        #endregion

                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.规格作业:
                                TileAddOutTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, task.DevConfig.goods_id, task.DevConfig.last_track_id);
                                break;
                            case DevWorkTypeE.轨道作业:
                                TileAddTrackOutTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, task.DevConfig.goods_id);
                                break;
                        }
                    }

                    #endregion
                }

                #endregion
            }
            else if (task.IsInvo_2)
            {
                bool isOK = false;
                if (task.DevConfig.right_track_id == 0)
                {
                    isOK = true;
                }
                else
                {
                    switch (task.Type)
                    {
                        case DeviceTypeE.上砖机:
                            isOK = !PubTask.Carrier.HaveInTrackAndLoad(task.DevConfig.right_track_id);
                            break;
                        case DeviceTypeE.下砖机:
                            isOK = !PubTask.Carrier.HaveInTrack(task.DevConfig.right_track_id);
                            break;
                        default:
                            break;
                    }
                }
                //没有需求但是介入状态 同时:轨道没有车/有车无货
                if (task.DevConfig.right_track_id == 0
                    || (isOK && mTimer.IsOver(TimerTag.TileInvoNotNeed, task.ID, Site_2, 15, 10)))
                {
                    if (task.HaveBrother)
                    {
                        Thread.Sleep(1000);
                        task.Do2Invo(DevLifterInvolE.离开);
                    }
                    else
                    {
                        TileLifterTask bro = DevList.Find(c => c.BrotherId == task.ID);
                        if (bro == null || (bro != null && !bro.IsNeed_2))
                        {
                            Thread.Sleep(1000);
                            task.Do2Invo(DevLifterInvolE.离开);
                        }
                    }
                }
            }

            #endregion

        }
    
        /// <summary>
        /// 是否允许作为任务品种
        /// </summary>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        private bool IsAllowToBeTaskGoods(uint tileid, uint goodsid)
        {
            // 无品种反馈
            if (goodsid == 0)
            {
                PubMaster.Warn.AddDevWarn(WarningTypeE.TileGoodsIsZero, (ushort)tileid);
                return false;
            }

            // 无品种数据
            if (PubMaster.Goods.GetGoods(goodsid) == null)
            {
                PubMaster.Warn.AddDevWarn(WarningTypeE.TileGoodsIsNull, (ushort)tileid);
                return false;
            }

            PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileGoodsIsZero, (ushort)tileid);
            PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileGoodsIsNull, (ushort)tileid);
            return true;
        }

        /// <summary>
        /// 混砖下砖策略
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="tileid"></param>
        /// <param name="tiletrackid"></param>
        /// <param name="goodid"></param>
        /// <param name="stockid"></param>
        private void AddMixTrackTransTask(uint areaid, uint tileid, uint tiletrackid, uint goodid, uint stockid)
        {
            if (stockid == 0) return;
            uint lasttrack = PubMaster.DevConfig.GetLastTrackId(tileid);
            if (lasttrack != 0 && PubMaster.Track.IsStatusOkToGive(lasttrack))
            {
                if (PubTask.Trans.IsTraInTransWithLock(lasttrack))
                {
                    PubMaster.Warn.AddDevWarn(WarningTypeE.TileMixLastTrackInTrans, (ushort)tileid, 0, lasttrack);
                    return;
                }

                PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileMixLastTrackInTrans, (ushort)tileid);

                PubMaster.Track.UpdateRecentGood(lasttrack, goodid);
                PubMaster.Track.UpdateRecentTile(lasttrack, tileid);
                //生成入库交易
                PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.下砖任务, goodid, stockid, tiletrackid, lasttrack);
            }
            else
            {
                TileAddInTransTask(areaid, tileid, tiletrackid, goodid, stockid);

                PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileMixLastTrackInTrans, (ushort)tileid);
            }
        }

        /// <summary>
        /// 新增库存
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="tiletrackid"></param>
        /// <param name="goodid"></param>
        /// <param name="fullqty"></param>
        /// <param name="stockid"></param>
        private void AddAndGetStockId(uint tileid, uint tiletrackid, uint goodid, byte fullqty, out uint stockid)
        {
            //[已有库存]
            if (!PubMaster.Goods.HaveStockInTrack(tiletrackid, goodid, out stockid))
            {
                ////[生成库存]
                stockid = PubMaster.Goods.AddStock(tileid, tiletrackid, goodid, fullqty);
                if (stockid > 0)
                {
                    PubMaster.Track.UpdateStockStatus(tiletrackid, TrackStockStatusE.有砖, "下砖");
                    PubMaster.Goods.AddStockInLog(stockid);
                }
            }
        }

        /// <summary>
        /// 添加入库任务
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="tileid">砖机ID</param>
        /// <param name="tiletrackid">砖机轨道ID</param>
        /// <param name="goodid">砖机规格</param>
        /// <param name="fullqty">砖机满砖数量</param>
        private void TileAddInTransTask(uint areaid, uint tileid, uint tiletrackid, uint goodid, uint stockid)
        {
            //分配放货点
            if (stockid != 0 && PubMaster.Goods.AllocateGiveTrack(areaid, tileid, goodid, out List<uint> traids))
            {
                uint givetrackid = 0;
                foreach (uint traid in traids)
                {
                    if (!PubTask.Trans.IsTraInTransWithLock(traid))
                    {
                        givetrackid = traid;
                        break;
                    }
                }

                if (givetrackid != 0)
                {
                    PubMaster.DevConfig.SetLastTrackId(tileid, givetrackid);
                    PubMaster.Track.UpdateRecentGood(givetrackid, goodid);
                    PubMaster.Track.UpdateRecentTile(givetrackid, tileid);
                    //生成入库交易
                    PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.下砖任务, goodid, stockid, tiletrackid, givetrackid);
                }

                PubMaster.Warn.RemoveDevWarn(WarningTypeE.DownTileHaveNotTrackToStore, (ushort)tileid);
            }
            else if (stockid != 0)
            {
                PubMaster.Warn.AddDevWarn(WarningTypeE.DownTileHaveNotTrackToStore, (ushort)tileid);
            }
        }

        /// <summary>
        /// 添加出库任务
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="tileid">砖机ID</param>
        /// <param name="tiletrackid">砖机轨道ID</param>
        /// <param name="goodid">砖机规格</param>
        /// <param name="currentid">设置优先轨道</param>
        private void TileAddOutTransTask(uint areaid, uint tileid, uint tiletrackid, uint goodid, uint currentid)
        {
            bool isallocate = false;

            // 1.查看当前作业轨道是否能作业
            if (PubMaster.Track.HaveTrackInGoodFrist(areaid, tileid, goodid, currentid, out uint trackid))
            {
                if (!PubTask.Trans.HaveInTileTrack(trackid))
                {
                    uint stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
                    //有库存但是不是砖机需要的品种
                    if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goodid))
                    {
                        PubMaster.Track.UpdateRecentTile(trackid, 0);
                        PubMaster.Track.UpdateRecentGood(trackid, 0);
                        PubTask.TileLifter.ReseTileCurrentTake(trackid);
                        return;
                    }
                    //生成出库交易
                    PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stockid, trackid, tiletrackid);
                    PubMaster.Goods.AddStockOutLog(stockid, tiletrackid, tileid);
                    isallocate = true;
                }
            }

            // 2.查看是否存在未空砖但无库存的轨道
            else if (PubMaster.Track.HaveTrackInGoodButNotStock(areaid, tileid, goodid, out List<uint> trackids))
            {
                foreach (uint tra in trackids)
                {
                    if (!PubTask.Trans.HaveInTileTrack(tra))
                    {
                        uint stockid = PubMaster.Goods.GetTrackTopStockId(tra);
                        //有库存但是不是砖机需要的品种
                        if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goodid))
                        {
                            PubMaster.Track.UpdateRecentTile(tra, 0);
                            PubMaster.Track.UpdateRecentGood(tra, 0);
                            return;
                        }
                        //生成出库交易
                        PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stockid, tra, tiletrackid);
                        PubMaster.Goods.AddStockOutLog(stockid, tiletrackid, tileid);
                        isallocate = true;
                        break;
                    }
                }
            }

            // 3.分配库存
            else if (PubMaster.Goods.GetStock(areaid, tileid, goodid, out List<Stock> allocatestocks))
            {
                foreach (Stock stock in allocatestocks)
                {
                    if (!PubTask.Trans.IsStockInTrans(stock.id, stock.track_id))
                    {
                        PubMaster.Track.UpdateRecentGood(stock.track_id, goodid);
                        PubMaster.Track.UpdateRecentTile(stock.track_id, tileid);
                        //生成出库交易
                        PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stock.id, stock.track_id, tiletrackid);
                        PubMaster.Goods.AddStockOutLog(stock.id, tiletrackid, tileid);
                        isallocate = true;
                        break;
                    }
                }
            }

            if (isallocate)
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileHaveNotStockToOut, (ushort)tileid);
            }
            else
            {
                PubMaster.Warn.AddDevWarn(WarningTypeE.UpTileHaveNotStockToOut, (ushort)tileid);
            }

        }

        /// <summary>
        /// 添加上砖机按轨道的出库任务
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="tileid"></param>
        /// <param name="tiletrackid"></param>
        private void TileAddTrackOutTransTask(uint areaid, uint tileid, uint tiletrackid, uint tilegoodid)
        {
            bool isallocate = false;
            List<TileTrack> tracks = PubMaster.TileTrack.GetTileTrack2Out(tileid);
            foreach (TileTrack tt in tracks)
            {
                Track track = PubMaster.Track.GetTrack(tt.track_id);
                if (track.StockStatus == TrackStockStatusE.空砖 || track.AlertStatus != TrackAlertE.正常 ||
                    (track.TrackStatus != TrackStatusE.启用 && track.TrackStatus != TrackStatusE.仅上砖))
                {
                    PubMaster.TileTrack.DeleteTileTrack(tt);
                    continue;
                }

                Stock stock = PubMaster.Goods.GetTrackTopStock(tt.track_id);
                uint goodid = 0;
                uint stockid = 0;
                if (stock != null)
                {
                    stockid = stock.id;
                    goodid = stock.goods_id;

                    PubMaster.Track.UpdateRecentGood(stock.track_id, goodid);
                }
                else
                {
                    goodid = track.recent_goodid;
                    if (goodid == 0)
                    {
                        goodid = tilegoodid;
                    }
                }

                //生成出库交易
                PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stockid, tt.track_id, tiletrackid);
                PubMaster.Goods.AddStockOutLog(stockid, tiletrackid, tileid);
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileHaveNoTrackToOut, (ushort)tileid);
                isallocate = true;
                break;
            }

            if (!isallocate)
            {
                PubMaster.Warn.AddDevWarn(WarningTypeE.UpTileHaveNoTrackToOut, (ushort)tileid);
            }
        }

        internal List<TileLifterTask> GetDevTileLifters()
        {
            return DevList;
        }

        private bool IsTileLoadAndNeed(uint tileid, uint track, bool isload, bool isneed)
        {
            TileLifterTask tile = DevList.Find(c => c.ID == tileid);
            if (tile != null)
            {
                if (tile.DevConfig.left_track_id == track)
                {
                    return (isload ? tile.IsLoad_1 : tile.IsEmpty_1) && (isneed ? tile.IsNeed_1 : !tile.IsNeed_1);
                }

                if (tile.DevConfig.right_track_id == track)
                {
                    return (isload ? tile.IsLoad_2 : tile.IsEmpty_2) && (isneed ? tile.IsNeed_2 : !tile.IsNeed_2);
                }
            }

            return false;
        }

        public bool IsHaveLoadNoNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, true, false);
        }

        public bool IsHaveEmptyNoNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, false, false);
        }

        public bool IsHaveLoadNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, true, true);
        }

        public bool IsHaveEmptyNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, false, true);
        }

        internal byte GetTileFullQty(uint devid, uint goodid)
        {
            TileLifterTask tile = DevList.Find(c => c.ID == devid);
            if (tile != null && tile.FullQty > 0)
            {
                return tile.FullQty;
            }
            return PubMaster.Goods.GetGoodsPieces(goodid);
        }

        /// <summary>
        /// 检查入库策略
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckInStrategy(TileLifterTask task, uint trackid, uint goodsId)
        {
            bool iseffect = false;

            switch (task.InStrategy)
            {

                case StrategyInE.无:
                    iseffect = true;
                    PubMaster.Warn.AddDevWarn(WarningTypeE.TileNoneStrategy, (ushort)task.ID);
                    break;
                case StrategyInE.同机同轨://同一砖机同时只派发一个任务【间接限制了会下不同轨道】
                    iseffect = PubTask.Trans.HaveInLifter(task.ID);
                    break;
                case StrategyInE.同轨同轨://双下砖机，同时只作业一台砖机作业【间接限制了会下不同轨道】
                    iseffect = PubTask.Trans.HaveInTileTrack(task.DevConfig.left_track_id, task.DevConfig.right_track_id);
                    break;
                case StrategyInE.优先下砖:
                    iseffect = PubTask.Trans.ExistInTileTrack(task.ID, trackid);
                    break;
                case StrategyInE.同规同轨:
                    iseffect = PubTask.Trans.HaveInGoods(task.AreaId, goodsId, TransTypeE.下砖任务);
                    break;
            }
            return iseffect;
        }

        internal bool IsInSideTileNeed(uint tileid, uint trackid)
        {
            TileLifterTask task = DevList.Find(c => c.BrotherId == tileid);
            if (task != null)
            {
                if (task.ConnStatus == SocketConnectStatusE.通信正常)
                {
                    if (task.DevConfig.left_track_id == trackid)
                    {
                        return task.IsNeed_1 && task.IsInvo_1 && !task.IsLoad_1;
                    }
                    return task.IsNeed_2 && task.IsInvo_2 && !task.IsLoad_2;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查出库策略
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckOutStrategy(TileLifterTask task, uint trackid)
        {
            bool iseffect = false;

            switch (task.OutStrategy)
            {
                case StrategyOutE.无:
                    iseffect = true;
                    break;
                case StrategyOutE.同机同轨:
                    iseffect = PubTask.Trans.HaveInLifter(task.ID);
                    break;
                case StrategyOutE.同规同轨:
                    iseffect = PubTask.Trans.HaveInGoods(task.AreaId, task.DevConfig.goods_id, TransTypeE.上砖任务);
                    break;
                case StrategyOutE.优先上砖:
                    iseffect = PubTask.Trans.ExistInTileTrack(task.ID, trackid);
                    break;
                case StrategyOutE.同轨同轨://双下砖机，同时只作业一台砖机作业【间接限制了会下不同轨道】
                    iseffect = PubTask.Trans.HaveOutTileTrack(task.DevConfig.left_track_id, task.DevConfig.right_track_id);
                    break;
                default:
                    break;
            }
            return iseffect;
        }

        /// <summary>
        /// 检查下砖机兄弟砖机是否满砖状态
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckBrotherIsReady(TileLifterTask task, bool checkfull, bool checkleft)
        {
            //没有干预设备
            if (!task.HaveBrother) return true;

            TileLifterTask brotask = DevList.Find(c => c.ID == task.BrotherId);
            if (brotask == null) return false;
            if (brotask.ConnStatus == SocketConnectStatusE.通信正常)
            {
                if (checkleft)
                {
                    if (brotask.IsNeed_1) return false;
                    if (!brotask.IsInvo_1 && (checkfull ? brotask.IsLoad_1 : brotask.IsEmpty_1))
                    {
                        Thread.Sleep(1000);
                        brotask.Do1Invo(DevLifterInvolE.介入);
                    }
                    return brotask.IsInvo_1 && (checkfull ? brotask.IsLoad_1 : brotask.IsEmpty_1);
                }

                if (!brotask.IsInvo_2 && (checkfull ? brotask.IsLoad_2 : brotask.IsEmpty_2))
                {
                    if (brotask.IsNeed_2) return false;
                    Thread.Sleep(1000);
                    brotask.Do2Invo(DevLifterInvolE.介入);
                }
                return brotask.IsInvo_2 && (checkfull ? brotask.IsLoad_2 : brotask.IsEmpty_2);
            }
            else
            {
                if (checkleft)
                {
                    return brotask.Ignore_1;
                }
                else
                {
                    return brotask.Ignore_2;
                }
            }
        }

        /// <summary>
        /// 检查上砖机兄弟砖机是否满砖状态
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckUpBrotherIsReady(TileLifterTask task, bool checkfull, bool checkleft)
        {
            //外侧上砖机          
            if (!task.HaveBrother)
            {
                TileLifterTask brotaskin = DevList.Find(c => c.BrotherId == task.ID);//查找有BrotherId是该砖机ID的砖机(及并联内侧上砖机)
                if (brotaskin == null) return true;
                if (task.IsWorking && brotaskin.ConnStatus == SocketConnectStatusE.通信正常)
                {
                    if (checkleft)
                    {
                        if (brotaskin.IsNeed_1) return false;//左轨道检查需求1
                    }
                    else
                    {
                        if (brotaskin.IsNeed_2) return false;//右轨道检查需求2
                    }
                    return true;
                }
                return true;
            }

            TileLifterTask brotask = DevList.Find(c => c.ID == task.BrotherId);//DevList.Find(c => c.BrotherId == task.ID)
            if (brotask == null) return false;
            if (brotask.ConnStatus == SocketConnectStatusE.通信正常)
            {
                if (checkleft)
                {
                    if ((brotask.IsNeed_1 || brotask.IsInvo_1) && brotask.IsEmpty_1) return true;//如果兄弟砖机左工位有需求或介入且无砖，就可以生成出库任务                       
                }
                else
                {
                    if ((brotask.IsNeed_2 || brotask.IsInvo_2) && brotask.IsEmpty_2) return true;//如果兄弟砖机右工位有需求或介入且无砖，就可以生成出库任务
                }
                return false;
            }
            else
            {
                if (checkleft)
                {
                    return brotask.Ignore_1;
                }
                else
                {
                    return brotask.Ignore_2;
                }
            }
        }

        #endregion

        #region[发送信息]
        private void MsgSend(TileLifterTask task, DevTileLifter tilelifter)
        {
            if (Monitor.TryEnter(_objmsg, TimeSpan.FromSeconds(5)))
            {
                try
                {
                    mMsg.ID = task.ID;
                    mMsg.Name = task.Device.name;
                    mMsg.o1 = tilelifter;
                    mMsg.o2 = task.ConnStatus;
                    mMsg.o3 = task.DevConfig?.goods_id;
                    mMsg.o4 = task.InStrategy;
                    mMsg.o5 = task.OutStrategy;
                    mMsg.o6 = task.IsWorking;
                    mMsg.o7 = PubMaster.Track.GetTrackName(task.DevConfig.last_track_id);
                    mMsg.o8 = task.WorkType;
                    Messenger.Default.Send(mMsg, MsgToken.TileLifterStatusUpdate);
                }
                finally
                {
                    Monitor.Exit(_objmsg);
                }
            }
        }
        #endregion 

        #region[判断状态]
        private bool CheckTileLifterStatus(TileLifterTask task, out string result)
        {
            if (task == null)
            {
                result = "找不到砖机设备信息";
                return false;
            }

            if (task.ConnStatus != SocketConnectStatusE.通信正常)
            {
                result = "砖机设备未连接";
                return false;
            }

            result = "";
            return true;
        }

        /// <summary>
        /// 判断下砖机是否可以取货
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <param name="taketrackid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool IsTakeReady(uint tilelifter_id, uint taketrackid, out string result)
        {
            result = "";
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return false;
            }
            try
            {
                TileLifterTask task = DevList.Find(c => c.ID == tilelifter_id);
                if (!CheckTileLifterStatus(task, out result))
                {
                    return false;
                }

                if (!CheckBrotherIsReady(task, false, task.DevConfig.left_track_id == taketrackid))
                {
                    return false;
                }

                if (task.DevConfig.left_track_id == taketrackid)
                {
                    return task.IsNeed_1 && task.IsLoad_1 && task.IsInvo_1;
                }

                if (task.DevConfig.right_track_id == taketrackid)
                {
                    return task.IsNeed_2 && task.IsLoad_2 && task.IsInvo_2;
                }

            }
            finally { Monitor.Exit(_obj); }
            return false;
        }

        /// <summary>
        /// 判断上砖机是否可以放砖
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <param name="givetrackid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool IsGiveReady(uint tilelifter_id, uint givetrackid, out string result)
        {
            result = "";
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return false;
            }
            try
            {
                TileLifterTask task = DevList.Find(c => c.ID == tilelifter_id);
                if (!CheckTileLifterStatus(task, out result))
                {
                    return false;
                }

                if (!CheckUpBrotherIsReady(task, false, task.DevConfig.left_track_id == givetrackid))
                {
                    return false;
                }

                if (task.DevConfig.left_track_id == givetrackid)
                {
                    if (!task.IsInvo_1 && task.IsNeed_1 && task.IsEmpty_1)
                    {
                        task.Do1Invo(DevLifterInvolE.介入);
                    }
                    return task.IsNeed_1 && task.IsEmpty_1 && task.IsInvo_1;
                }

                if (task.DevConfig.right_track_id == givetrackid)
                {
                    if (!task.IsInvo_2 && task.IsNeed_2 && task.IsEmpty_2)
                    {
                        task.Do2Invo(DevLifterInvolE.介入);
                    }
                    return task.IsNeed_2 && task.IsEmpty_2 && task.IsInvo_2;
                }

            }
            finally { Monitor.Exit(_obj); }
            return false;
        }

        internal bool IsAnyoneNeeds(uint area, DeviceTypeE dt)
        {
            return DevList.Exists(c => c.AreaId == area && c.Type == dt && (c.IsNeed_1 || c.IsNeed_2));
        }

        #endregion

        #region[更新规格信息]
        public void UpdateTileLifterGoods(uint devid, uint goodid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(3)))
            {
                try
                {
                    TileLifterTask task = DevList.Find(c => c.ID == devid);
                    if (task != null)
                    {
                        task.DevConfig.goods_id = goodid;

                        #region 同步当前品种/等级
                        if (task.DevConfig.goods_id != task.DevStatus.SetGoods)
                        {
                            Thread.Sleep(500);
                            task.DoShift(TileShiftCmdE.变更品种, 0, task.DevConfig.goods_id);
                        }

                        byte level = PubMaster.Goods.GetGoodsLevel(task.DevConfig.goods_id);
                        if (level != task.DevStatus.SetLevel)
                        {
                            Thread.Sleep(500);
                            task.DoUpdateLevel(level);
                        }
                        #endregion

                        MsgSend(task, task.DevStatus);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        #endregion

        #region[启动/停止]

        public void UpdateWorking(uint devId, bool working, byte worktype)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                TileLifterTask task = DevList.Find(c => c.ID == devId);
                if (task != null)
                {
                    task.IsWorking = working;
                    if (worktype != 255)
                        task.WorkType = (DevWorkTypeE)worktype;
                    MsgSend(task, task.DevStatus);
                }
            }
            finally { Monitor.Exit(_obj); }

        }

        #endregion
    }
}
