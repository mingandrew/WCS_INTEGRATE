using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.area;
using module.device;
using module.goods;
using module.msg;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using tool.mlog;
using tool.timer;

namespace task.device
{
    public class CarrierMaster
    {
        #region[字段]

        private object _objmsg;
        private readonly MsgAction mMsg;
        private List<CarrierTask> DevList { set; get; }
        private readonly object _obj;
        private Thread _mRefresh;
        private bool Refreshing = true;
        private MTimer mTimer;
        private Log mlog;
        private bool isWcsStoping = false;
        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public CarrierMaster()
        {
            mlog = (Log)new LogFactory().GetLog("Carrier", false);
            mTimer = new MTimer();
            _objmsg = new object();
            mMsg = new MsgAction();
            _obj = new object();
            DevList = new List<CarrierTask>();
            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.CarrierMsgUpdate, CarrierMsgUpdate);
        }

        public void Start()
        {
            List<Device> carriers = PubMaster.Device.GetDeviceList(DeviceTypeE.运输车);
            foreach (Device dev in carriers)
            {
                CarrierTask task = new CarrierTask
                {
                    Device = dev,
                    DevConfig = PubMaster.DevConfig.GetCarrier(dev.id)
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
            foreach (CarrierTask task in DevList)
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
                        foreach (CarrierTask task in DevList)
                        {
                            try
                            {
                                if (task.IsEnable)
                                {
                                    task.DoQuery();

                                    if (task.Task != task.FinishTask)
                                    {
                                        switch (task.Task)
                                        {
                                            case DevCarrierTaskE.后退至摆渡车:
                                            case DevCarrierTaskE.前进至摆渡车:
                                                #region[判断是否有摆渡车]
                                                //ushort trackcode = PubTask.Carrier.GetCarrierSite(task.ID);
                                                ushort trackcode = task.DevStatus.CurrentSite;
                                                if (!PubTask.Ferry.HaveFerryOnTrack(trackcode, task.Task, out string result, true))
                                                {
                                                    DoTask(task.ID, DevCarrierTaskE.终止);
                                                }
                                                #endregion
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                }

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

                            }
                            catch (Exception e)
                            {
                                mlog.Error(true, e.Message, e);
                            }
                        }
                    }
                    finally { Monitor.Exit(_obj); }
                }
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// 清空设备信息
        /// </summary>
        /// <param name="iD"></param>
        public void ClearTaskStatus(uint carrierid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == carrierid);
                    if (task != null)
                    {
                        task.ClearDevStatus();
                        MsgSend(task, task.DevStatus);
                    }
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
        }

        public void DoReset(uint iD)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    DevList.Find(c => c.ID == iD).DevReset = DevCarrierResetE.复位;
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void StartStopCarrier(uint carrierid, bool isstart)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == carrierid);
                    if (task != null)
                    {
                        if (isstart)
                        {
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
                            task.Stop("手动停止");
                            PubMaster.Warn.RemoveDevWarn((ushort)task.ID);
                            PubTask.Ping.RemovePing(task.Device.ip);
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }

        }

        #endregion

        #region[获取信息]

        public void GetAllCarrier()
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    foreach (CarrierTask task in DevList)
                    {
                        MsgSend(task, task.DevStatus);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid)
        {
            return DevList.Exists(c => c.TrackId == trackid);
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道载货
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrackAndLoad(uint trackid)
        {
            return DevList.Exists(c => c.TrackId == trackid && c.Load == DevCarrierLoadE.有货);
        }

        internal bool HaveInTrackButCarrier(uint trackid, uint trackid2, uint cid, out uint carrierid)
        {
            CarrierTask task = DevList.Find(c => (c.TrackId == trackid || c.TrackId == trackid2) && c.ID != cid);
            if (task != null)
            {
                carrierid = task.ID;
                return true;
            }
            carrierid = 0;
            return false;
        }

        internal bool OnlyOneCarrierInTrack(uint trackid)
        {
            return DevList.Count(c => c.TrackId == trackid) == 1;
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道
        /// 1.ID对应的轨道
        /// 2.轨道的兄弟轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid, out uint carrierid)
        {
            //Track track = PubMaster.Track.GetTrack(trackid);
            CarrierTask carrier = DevList.Find(c => c.TrackId == trackid);
            carrierid = carrier?.ID ?? 0;
            return carrier != null;
        }

        internal bool HaveDifTypeInTrack(uint trackid, CarrierTypeE carriertype, out uint carrierid)
        {
            CarrierTask carrier = DevList.Find(c => c.TrackId == trackid);
            if (carrier != null && carrier.CarrierType != carriertype)
            {
                carrierid = carrier.ID;
                return true;
            }
            carrierid = 0;
            return false;
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道
        /// 1.ID对应的轨道
        /// 2.轨道的兄弟轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid, uint carrierid)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            return DevList.Exists(c => c.ID != carrierid && track.IsInTrack(c.Site));
        }

        /// <summary>
        /// 判断小车在指定的地标
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="carrierid"></param>
        /// <param name="isdownsite"></param>
        /// <returns></returns>
        internal bool HaveInTrackTopSide(uint trackid, uint carrierid, bool isdownsite)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            return DevList.Exists(c => c.ID != carrierid
            && ((isdownsite && c.Site == track.ferry_down_code)
                    || (!isdownsite && c.Site == track.ferry_up_code)));
        }

        internal bool IsOperateAuto(uint carrierid)
        {
            return DevList.Exists(c => c.OperateMode == DevOperateModeE.自动);
        }

        /// <summary>
        /// 小车完成任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsStopFTask(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && c.OperateMode == DevOperateModeE.自动
                        && c.Status == DevCarrierStatusE.停止
                        && (c.Task == c.FinishTask || c.Task == DevCarrierTaskE.无)
                        && (c.CarrierPosition != DevCarrierPositionE.上下摆渡中 || c.CarrierPosition != DevCarrierPositionE.未知));
        }

        /// <summary>
        /// 停止但是任务未完成
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsStopHaveTask(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && c.OperateMode == DevOperateModeE.自动
                        && c.Status == DevCarrierStatusE.停止
                        && c.Task != c.FinishTask
                        && c.Task != DevCarrierTaskE.无);
        }

        internal Track GetCarrierTrack(uint carrier_id)
        {
            uint trackid = DevList.Find(c => c.ID == carrier_id)?.TrackId ?? 0;
            return trackid > 0 ? PubMaster.Track.GetTrack(trackid) : null;
        }

        internal bool IsLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && c.Load == DevCarrierLoadE.有货);
        }

        internal bool IsLoadInFerry(uint ltrack)
        {
            return DevList.Exists(c => c.TrackId == ltrack
                         && c.ConnStatus == SocketConnectStatusE.通信正常
                         && c.Load == DevCarrierLoadE.有货
                         && c.CarrierPosition == DevCarrierPositionE.在摆渡上);
        }


        internal bool IsNotLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && c.Load == DevCarrierLoadE.无货);
        }

        #endregion

        #region[数据更新]

        private void CarrierMsgUpdate(SocketMsgMod mod)
        {
            if (isWcsStoping) return;
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        CarrierTask task = DevList.Find(c => c.ID == mod.ID);
                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is DevCarrier carrier)
                            {
                                task.ReSetRefreshTime();
                                task.DevStatus = carrier;
                                task.UpdateInfo();
                                CheckDev(task);

                                if (carrier.IsUpdate || mTimer.IsTimeOutAndReset(TimerTag.DevRefreshTimeOut, carrier.ID, 5))
                                {
                                    MsgSend(task, carrier);
                                }
                            }

                            CheckConn(task);
                        }
                    }
                    catch (Exception e)
                    {
                        mlog.Error(true, e.Message, e);
                    }
                    finally { Monitor.Exit(_obj); }
                }
            }
        }

        private void CheckConn(CarrierTask task)
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
            if (task.MConChange)
            {
                MsgSend(task, task.DevStatus);
            }
        }

        #endregion

        #region[检查设备状态]

        /// <summary>
        /// 1.运输车任务状态
        /// 2.满砖/空砖/正常取货卸货
        /// </summary>
        /// <param name="task"></param>
        private void CheckDev(CarrierTask task)
        {
            task.CheckAlert();
            if (task.OperateMode == DevOperateModeE.手动)
            {
                if (task.Task != DevCarrierTaskE.终止 && task.Task != DevCarrierTaskE.无)
                {
                    DoTask(task.ID, DevCarrierTaskE.终止);
                }

                Track track = PubMaster.Track.GetTrack(task.TrackId);
                if (track != null)
                {
                    if (track.Type == TrackTypeE.摆渡车_入 || track.Type == TrackTypeE.摆渡车_出)
                    {
                        PubTask.Ferry.StopFerryByFerryTrackId(track.id);
                    }
                }
            }
            UpdateFerryLoadStatus(task);
            if (task.Status != DevCarrierStatusE.停止) return;
            //if (task.OperateMode == DevOperateModeE.无)
            //{
            //    task.DoModeUpdate(DevCarrierWorkModeE.生产);
            //}
            if (task.DevReset == DevCarrierResetE.复位
                && task.DevStatus.ActionType == DevCarrierSignalE.复位)
            {
                task.DevReset = DevCarrierResetE.无动作;
            }

            //if (task.OperateMode == DevOperateModeE.手动模式) return;

            //if (task.WorkMode != DevCarrierWorkModeE.生产模式) return;

            #region[检查任务]

            #region[倒库完成]
            //倒库完成后小车回到空轨道今天等待，倒库任务完成，同时有空轨道信息
            //if (task.Task == DevCarrierTaskE.后退至轨道倒库
            //    && task.FinishTask == DevCarrierTaskE.后退至轨道倒库)
            //{
            //    if(task.Signal == DevCarrierSignalE.空轨道)
            //    {
            //        PubTask.Trans.ShiftTrans(task.ID, task.TrackId);
            //    }
            //}
            #endregion

            #region[空砖]

            if (task.Task == DevCarrierTaskE.后退取砖
                && task.FinishTask == DevCarrierTaskE.后退取砖)
            {
                Track track = PubMaster.Track.GetTrack(task.TrackId);
                if (track.Type == TrackTypeE.储砖_出 || track.Type == TrackTypeE.储砖_出入)
                {
                    switch (task.Signal)
                    {
                        case DevCarrierSignalE.空轨道:
                            if (task.Load == DevCarrierLoadE.无货)
                            {
                                PubMaster.Track.UpdateStockStatus(track.id, TrackStockStatusE.空砖, "上砖取空");
                                PubMaster.Goods.ClearTrackEmtpy(track.id);
                                PubTask.TileLifter.ReseTileCurrentTake(track.id);
                                PubMaster.Track.AddTrackLog((ushort)task.AreaId, task.ID, track.id, TrackLogE.空轨道, "无货");
                            }
                            else
                            {
                                PubMaster.Track.AddTrackLog((ushort)task.AreaId, task.ID, track.id, TrackLogE.空轨道, "有货");
                            }
                            break;
                        default:
                            break;
                    }

                    task.DevReset = DevCarrierResetE.复位;
                }
            }

            if (task.Signal == DevCarrierSignalE.空轨道 && task.Task != DevCarrierTaskE.后退取砖)
            {
                task.DevReset = DevCarrierResetE.复位;
            }

            #endregion

            #region[非空非满]

            if (task.Task == DevCarrierTaskE.前进放砖
                && task.FinishTask == DevCarrierTaskE.前进放砖
                && task.Signal == DevCarrierSignalE.非空非满)
            {
                task.DevReset = DevCarrierResetE.复位;
            }

            #endregion

            #region[满砖]
            if (task.Signal == DevCarrierSignalE.满轨道
                && task.Task == task.FinishTask)
            {
                Track givetrack = PubMaster.Track.GetTrackByCode(task.GiveTrackCode);
                if (givetrack != null
                    && (givetrack.Type == TrackTypeE.储砖_入 || givetrack.Type == TrackTypeE.储砖_出入))
                {
                    ushort storecount = PubMaster.Track.AddTrackLog((ushort)task.AreaId, task.ID, givetrack.id, TrackLogE.满轨道, "运输车反馈信号-满");
                    ushort areafullqty = PubMaster.Area.GetAreaFullQty(task.AreaId);
                    if (storecount >= (areafullqty - 1)) // 少一个 安全保底
                    {
                        //PubMaster.Track.SetTrackEaryFull(givetrack.id, true, DateTime.Now);

                        PubMaster.Track.UpdateStockStatus(givetrack.id, TrackStockStatusE.满砖, "下砖放满");
                        PubMaster.Track.UpdateRecentGood(givetrack.id, 0);
                        PubMaster.Track.UpdateRecentTile(givetrack.id, 0);
                    }
                }
                else if (givetrack != null)
                {
                    PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierFullSignalFullNotOnStoreTrack, (ushort)task.ID);
                    PubMaster.Track.AddTrackLog((ushort)task.AreaId, task.ID, givetrack.id, TrackLogE.满轨道, "非储砖轨道");
                }

                task.DevReset = DevCarrierResetE.复位;
            }

            #endregion

            #region[倒库]

            if (task.Task == DevCarrierTaskE.后退至轨道倒库
                && task.Signal == DevCarrierSignalE.非空非满
                && task.Status == DevCarrierStatusE.停止
                && mTimer.IsOver(TimerTag.CarrierSortTakeGive, (ushort)task.ID, 5, 10))
            {
                task.DevReset = DevCarrierResetE.复位;
            }

            #endregion

            #region[逻辑警告]

            task.CheckLogicAlert();

            #endregion

            #endregion

        }

        #region[更新摆渡车载货状态]

        private static void UpdateFerryLoadStatus(CarrierTask task)
        {
            Track tt = PubMaster.Track.GetTrack(task.TrackId);
            if (tt != null)
            {
                switch (tt.Type)
                {
                    case TrackTypeE.摆渡车_入:
                    case TrackTypeE.摆渡车_出:
                        PubTask.Ferry.UpdateFerryWithTrackId(task.TrackId, DevFerryLoadE.载车);
                        task.LastTrackId = task.TrackId;
                        break;
                    default:
                        if (task.LastTrackId != 0)
                        {
                            PubTask.Ferry.UpdateFerryWithTrackId(task.LastTrackId, DevFerryLoadE.空);
                            task.LastTrackId = 0;
                        }
                        break;
                }
            }
        }

        #endregion

        internal bool HaveCarrierTaskInTrack(uint trackid, DevCarrierTaskE carriertype)
        {
            return DevList.Exists(c => c.TrackId == trackid
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && (c.OperateMode == DevOperateModeE.自动 || c.OperateMode == DevOperateModeE.手动)
                                    && ((c.Task == carriertype
                                    && c.Task != c.FinishTask) || c.CarrierPosition == DevCarrierPositionE.上下摆渡中));
        }

        internal bool HaveCarrierTaskInFerry(uint trackid)
        {
            return DevList.Exists(c => c.TrackId == trackid
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && (c.OperateMode == DevOperateModeE.自动 || c.OperateMode == DevOperateModeE.手动)
                                    && (c.Task != c.FinishTask || c.CarrierPosition == DevCarrierPositionE.上下摆渡中));
        }

        internal bool CheckStatus(uint carrier_id)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == carrier_id);
                    if (task != null)
                    {
                        return task.OperateMode != DevOperateModeE.自动;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        internal bool IsCarrierInTrack(StockTrans trans)
        {
            if (!trans.IsReleaseGiveFerry)
            {
                CarrierTask carrier = DevList.Find(c => c.ID == trans.carrier_id);
                if (carrier.Task == DevCarrierTaskE.前进放砖)
                {
                    mlog.Error(true, "没有读到" + trans.give_track_id + "储砖入轨道地标");
                }
                else if (carrier.Task == DevCarrierTaskE.后退取砖)
                {
                    mlog.Error(true, "没有读到" + trans.finish_track_id + "储砖出轨道地标");
                }
            }
            return DevList.Exists(c => c.ID == trans.carrier_id
                                    && c.CarrierPosition == DevCarrierPositionE.在轨道上
                                    && PubMaster.Track.IsFerryTrackType(c.TrackId));
        }


        #endregion

        #region[发送信息]
        private void MsgSend(CarrierTask task, DevCarrier carrier)
        {
            if (Monitor.TryEnter(_objmsg, TimeSpan.FromSeconds(5)))
            {
                try
                {
                    mMsg.ID = task.ID;
                    mMsg.Name = task.Device.name;
                    mMsg.o1 = carrier;
                    mMsg.o2 = task.ConnStatus;
                    mMsg.o3 = task.IsWorking;
                    Messenger.Default.Send(mMsg, MsgToken.CarrierStatusUpdate);
                }
                finally
                {
                    Monitor.Exit(_objmsg);
                }
            }
        }
        #endregion

        #region[执行任务]

        public bool DoManualTask(uint devid, DevCarrierTaskE carriertask, out string result, bool isoversize = false)
        {
            bool isferryupsite = false;
            ushort trackcode = PubTask.Carrier.GetCarrierSite(devid);
            if (trackcode == 0)
            {
                result = "小车当前站点为0";
                return false;
            }
            switch (carriertask)
            {
                case DevCarrierTaskE.后退取砖:
                    if (IsLoad(devid))
                    {
                        result = "运输车有货不能后退取砖！";
                        return false;
                    }
                    break;
                case DevCarrierTaskE.前进放砖:
                    if (IsNotLoad(devid))
                    {
                        result = "运输车没有货不能前进放货！";
                        return false;
                    }
                    isferryupsite = true;
                    break;
                case DevCarrierTaskE.后退至摆渡车:
                case DevCarrierTaskE.前进至摆渡车:
                    #region[判断是否有摆渡车]
                    if (carriertask == DevCarrierTaskE.后退至摆渡车
                        && !((trackcode >= 200 && trackcode < 300) || (trackcode >= 600 && trackcode < 700)))
                    {
                        result = "小车需要在入库轨道头或者上砖轨道";
                        return false;
                    }
                    if (carriertask == DevCarrierTaskE.前进至摆渡车
                        && !((trackcode >= 500 && trackcode < 600) || (trackcode >= 100 && trackcode < 200)))
                    {
                        result = "小车需要在出库轨道头或者下砖轨道";
                        return false;
                    }
                    if (!PubTask.Ferry.HaveFerryOnTrack(trackcode, carriertask, out result, false))
                    {
                        return false;
                    }
                    #endregion
                    break;
                case DevCarrierTaskE.后退至轨道倒库:
                    result = "当前无法使用！";
                    return false;
                case DevCarrierTaskE.前进至点:
                    if (!((trackcode >= 100 && trackcode < 400) || (trackcode >= 700 && trackcode < 740)))
                    {
                        result = "小车需要在入库轨道或者下砖摆渡车";
                        return false;
                    }
                    isferryupsite = true;
                    break;
                case DevCarrierTaskE.后退至点:
                    if (!((trackcode >= 300 && trackcode < 600) || trackcode >= 740))
                    {
                        result = "小车需要在出库轨道或者上砖摆渡车";
                        return false;
                    }
                    break;
                case DevCarrierTaskE.顶升取货:
                    break;
                case DevCarrierTaskE.下降放货:
                    break;
                case DevCarrierTaskE.终止:
                    break;
            }

            //小车在摆渡车上-离开摆渡车时，需要判断摆渡车是否停止、并且对准轨道
            if (carriertask != DevCarrierTaskE.终止
                && carriertask != DevCarrierTaskE.下降放货
                && carriertask != DevCarrierTaskE.顶升取货
                && carriertask != DevCarrierTaskE.前进至摆渡车
                && carriertask != DevCarrierTaskE.后退至摆渡车)
            {
                Track track = GetCarrierTrack(devid);
                if (track != null
                    && (track.Type == TrackTypeE.摆渡车_入 || track.Type == TrackTypeE.摆渡车_出))
                {
                    if (!PubTask.Ferry.IsStopAndSiteOnTrack(track.id, isferryupsite, out result))
                    {
                        return false;
                    }
                }
            }

            DoTask(devid, carriertask, isoversize);
            result = "";
            return true;
        }

        public void DoTask(uint devid, DevCarrierTaskE carriertask, bool isoversize = false)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == devid);
                    if (task != null)
                    {
                        if (task.OperateMode == DevOperateModeE.自动)
                        {
                            if (task.Status == DevCarrierStatusE.停止 && task.Task == carriertask && task.FinishTask == carriertask)
                            {
                                carriertask = DevCarrierTaskE.终止;
                            }
                        }
                        else
                        {
                            carriertask = DevCarrierTaskE.终止;
                        }

                        task.DoTask(carriertask, isoversize ? DevCarrierSizeE.超限 : DevCarrierSizeE.非超限);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        //public void DoSetMode(uint devid, DevCarrierWorkModeE mode)
        //{
        //    if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
        //    {
        //        try
        //        {
        //            DevList.Find(c => c.ID == devid)?.DoModeUpdate(mode);
        //        }
        //        finally { Monitor.Exit(_obj); }
        //    }
        //}

        #endregion

        #region[分配-运输车]

        public bool AllocateCarrier(StockTrans trans, out uint carrierid, out string result)
        {
            result = "";
            carrierid = 0;
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    #region 分配职责

                    //if (DevList.Exists(c => c.IsEnable && c.CarrierDuty == CarrierDutyE.未知))
                    //{
                    //    foreach (CarrierTask car in DevList.FindAll(c => c.IsEnable))
                    //    {
                    //        Track t = PubTask.Carrier.GetCarrierTrack(car.ID);
                    //        if (t == null)
                    //        {
                    //            PubMaster.Device.SetDuty(car.ID, CarrierDutyE.未知);
                    //        }
                    //        else
                    //        {
                    //            switch (t.Type)
                    //            {
                    //                case TrackTypeE.上砖轨道:
                    //                case TrackTypeE.储砖_出:
                    //                case TrackTypeE.摆渡车_出:
                    //                    PubMaster.Device.SetDuty(car.ID, CarrierDutyE.上砖);
                    //                    break;
                    //                case TrackTypeE.下砖轨道:
                    //                case TrackTypeE.储砖_入:
                    //                case TrackTypeE.摆渡车_入:
                    //                    PubMaster.Device.SetDuty(car.ID, CarrierDutyE.下砖);
                    //                    break;
                    //                default:
                    //                    break;
                    //            }
                    //        }
                    //    }
                    //}

                    #endregion

                    switch (trans.TransType)
                    {
                        case TransTypeE.下砖任务:
                        case TransTypeE.手动入库:
                            return GetTransInOutCarrier(trans, DeviceTypeE.下摆渡, out carrierid, out result);
                        case TransTypeE.上砖任务:
                        case TransTypeE.手动出库:
                            return GetTransInOutCarrier(trans, DeviceTypeE.上摆渡, out carrierid, out result);
                        case TransTypeE.倒库任务:
                            return GetTransSortCarrier(trans, out carrierid, out result);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        /// <summary>
        /// 分配倒库小车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="carrierid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool GetTransSortCarrier(StockTrans trans, out uint carrierid, out string result)
        {
            result = "";
            carrierid = 0;
            if (trans.goods_id == 0) return false;
            CarrierTypeE needtype = PubMaster.Goods.GetGoodsCarrierType(trans.goods_id);

            // 1.倒库空轨道是否有车[空闲，无货]
            CarrierTask carrier = DevList.Find(c => c.TrackId == trans.give_track_id && c.CarrierType == needtype);
            #region[2.摆渡车上是否有车[空闲，无货]
            if (carrier == null)
            {
                //3.1获取能到达[空轨道]轨道的上砖摆渡车的轨道ID
                List<uint> ferrytrackids = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.上摆渡, trans.area_id, 0, trans.give_track_id, 0, true);
                //3.2获取在摆渡轨道上的车[空闲，无货]
                List<CarrierTask> carriers = DevList.FindAll(c => ferrytrackids.Contains(c.TrackId) && c.CarrierType == needtype);
                if (carriers.Count > 0)
                {
                    //如何判断哪个摆渡车最右
                    foreach (CarrierTask car in carriers)
                    {
                        //小车:没有任务绑定
                        if (!PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            //空闲,没货，没任务
                            if (CheckCarrierFreeNotLoad(car))
                            {
                                carrierid = car.ID;
                                return true;
                            }
                        }
                    }
                }

            }

            #region[2.满砖轨道是否有车[空闲，无货]]
            //if (carrier == null)
            //{
            //    carrier = DevList.Find(c => c.TrackId == trans.take_track_id);
            //}
            #endregion

            #endregion

            //前面找到车了，如果空闲则分配，否则等待
            if (carrier != null)
            {
                if (CheckCarrierFreeNotLoad(carrier))
                {
                    carrierid = carrier.ID;
                    return true;
                }
            }
            #region[直接找车]
            else
            {
                if (trans.area_id != 0)
                {
                    List<AreaDevice> areatras = PubMaster.Area.GetAreaDevList(trans.area_id, DeviceTypeE.运输车);
                    foreach (AreaDevice areatra in areatras)
                    {
                        CarrierTask task = DevList.Find(c => c.ID == areatra.device_id && c.CarrierType == needtype);
                        if (task != null && CheckCarrierFreeNotLoad(task)
                            && PubMaster.Track.IsTrackType(task.TrackId, TrackTypeE.储砖_出))
                        {
                            carrierid = task.ID;
                            return true;
                        }
                    }
                }
            }
            #endregion

            return false;
        }


        /// <summary>
        /// 根据交易信息分配运输车
        /// 1.取货轨道是否有车
        /// 2.卸货轨道是否有车
        /// 3.摆渡车上是否有车
        /// 4.根据上下砖机轨道优先级逐轨道是否有车
        /// 5.对面储砖区域(上下砖机轨道对应的兄弟轨道是否有车)
        /// 6.对面区域摆渡车是否有车
        /// 7.对面砖机轨道是否有车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        private bool GetTransInOutCarrier(StockTrans trans, DeviceTypeE ferrytype, out uint carrierid, out string result)
        {
            result = "";
            carrierid = 0;
            if (trans.goods_id == 0)
            {
                result = "任务没有规格id";
                return false;
            }
            CarrierTypeE needtype = PubMaster.Goods.GetGoodsCarrierType(trans.goods_id);
            //CarrierDutyE needduty = ferrytype == DeviceTypeE.上摆渡 ? CarrierDutyE.上砖 : CarrierDutyE.下砖;
            // 1.取货轨道是否有车[空闲，无货]
            CarrierTask carrier = DevList.Find(c => c.TrackId == trans.take_track_id && c.CarrierType == needtype);
            //CarrierTask carrier = DevList.Find(c => c.TrackId == trans.take_track_id && c.CarrierType == needtype && c.CarrierDuty == needduty);

            if (carrier == null && (trans.TransType == TransTypeE.上砖任务 || trans.TransType == TransTypeE.手动出库))
            {
                uint brothertra = PubMaster.Track.GetBrotherTrackId(trans.take_track_id);
                carrier = DevList.Find(c => c.TrackId == brothertra
                                        && c.CarrierType == needtype
                                        && c.Task == DevCarrierTaskE.后退取砖);
            }

            if (carrier == null)
            {
                #region[2.卸货轨道是否有车[空闲，无货]]
                carrier = DevList.Find(c => c.TrackId == trans.give_track_id && c.CarrierType == needtype);
                #endregion

                #region[3.摆渡车上是否有车[空闲，无货]
                if (carrier == null)
                {
                    //3.1获取能到达[取货/卸货]轨道的摆渡车的ID
                    List<uint> ferrytrackids = PubMaster.Area.GetFerryWithTrackInOut(ferrytype, trans.area_id, trans.take_track_id, trans.give_track_id, 0, true);

                    List<uint> loadcarferryids = new List<uint>();
                    foreach (uint fetraid in ferrytrackids)
                    {
                        uint fid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(fetraid);
                        if (PubTask.Ferry.IsLoad(fid))
                        {
                            loadcarferryids.Add(fetraid);
                        }
                    }

                    //3.2获取在摆渡车上的车[空闲，无货]
                    List<CarrierTask> carriers = DevList.FindAll(c => loadcarferryids.Contains(c.TrackId) && c.CarrierType == needtype);
                    if (carriers.Count > 0)
                    {
                        //如何判断哪个摆渡车最右
                        foreach (CarrierTask car in carriers)
                        {
                            //小车:没有任务绑定
                            if (!PubTask.Trans.HaveInCarrier(car.ID))
                            {
                                switch (trans.TransType)
                                {
                                    case TransTypeE.下砖任务:
                                        //空闲,没货，没任务
                                        if (CheckCarrierFreeNotLoad(car))
                                        {
                                            carrierid = car.ID;
                                            return true;
                                        }
                                        break;
                                    case TransTypeE.上砖任务:
                                        //空闲，没任务
                                        if (CheckCarrierFreeNoTask(car))
                                        {
                                            carrierid = car.ID;
                                            return true;
                                        }
                                        break;
                                    case TransTypeE.倒库任务:
                                        break;
                                    case TransTypeE.其他:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                }
                #endregion
            }
            //前面找到车了，如果空闲则分配，否则等待
            if (carrier != null)
            {
                switch (trans.TransType)
                {
                    case TransTypeE.下砖任务:
                    case TransTypeE.手动入库:
                        if (CheckCarrierFreeNotLoad(carrier))
                        {
                            carrierid = carrier.ID;
                            return true;
                        }
                        break;
                    case TransTypeE.上砖任务:
                    case TransTypeE.手动出库:
                        if (!carrier.IsWorking)
                        {
                            result = "运输车没有作业";
                            return false;
                        }
                        if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                            && carrier.OperateMode == DevOperateModeE.自动)
                        {
                            if (carrier.Status == DevCarrierStatusE.停止
                                //&& carrier.WorkMode == DevCarrierWorkModeE.生产模式
                                && (carrier.Task == carrier.FinishTask || carrier.Task == DevCarrierTaskE.无))
                            {
                                carrierid = carrier.ID;
                                return true;
                            }

                            if (carrier.Task == DevCarrierTaskE.后退取砖 && carrier.FinishTask == DevCarrierTaskE.无)
                            {
                                carrierid = carrier.ID;
                                return true;
                            }
                        }

                        if (CheckCarrierFreeNoTask(carrier))
                        {
                            carrierid = carrier.ID;
                            return true;
                        }
                        break;
                    case TransTypeE.倒库任务:
                        break;
                    case TransTypeE.其他:
                        break;
                    default:
                        break;
                }
            }
            #region[找其他轨道]
            else
            {
                List<uint> loadcarrerid = new List<uint>();
                List<uint> unloadcarrierid = new List<uint>();
                List<uint> trackids = PubMaster.Area.GetTileTrackIds(trans);
                foreach (uint traid in trackids)
                {
                    if (!PubMaster.Track.IsStoreType(traid)) continue;
                    List<CarrierTask> tasks = DevList.FindAll(c => c.TrackId == traid);
                    if (tasks.Count > 0)
                    {
                        if (tasks.Count > 1) continue;
                        if (tasks[0] == null) continue;
                        if (!tasks[0].IsWorking) continue;
                        if (tasks[0].ConnStatus == SocketConnectStatusE.通信正常
                                && tasks[0].Status == DevCarrierStatusE.停止
                                && tasks[0].OperateMode == DevOperateModeE.自动
                                //&& tasks[0].WorkMode == DevCarrierWorkModeE.生产模式
                                && (tasks[0].Task == tasks[0].FinishTask || tasks[0].Task == DevCarrierTaskE.无)
                                //&& tasks[0].Load == DevCarrierLoadE.无货
                                && tasks[0].CarrierType == needtype
                                )
                        {
                            if (tasks[0].Load == DevCarrierLoadE.无货)
                            {
                                unloadcarrierid.Add(tasks[0].ID);
                            }
                            else if (tasks[0].Load == DevCarrierLoadE.有货)
                            {
                                loadcarrerid.Add(tasks[0].ID);
                            }
                        }
                    }
                }

                if (loadcarrerid.Count > 0 || unloadcarrierid.Count > 0)
                {
                    foreach (uint carid in unloadcarrierid)
                    {
                        CarrierTask task = DevList.Find(c => c.ID == carid);

                        if (CheckCarrierFreeNotLoad(task)
                               && task.CarrierType == needtype
                               && !PubTask.Trans.HaveInCarrier(task.ID))
                        {
                            carrierid = task.ID;
                            return true;
                        }
                    }

                    foreach (uint carid in loadcarrerid)
                    {
                        CarrierTask task = DevList.Find(c => c.ID == carid);

                        if (CheckCarrierFreeNotLoad(task)
                               && task.CarrierType == needtype
                               && !PubTask.Trans.HaveInCarrier(task.ID))
                        {
                            carrierid = task.ID;
                            return true;
                        }
                    }
                }
            }
            #endregion

            return false;
        }

        internal DevCarrierSignalE GetCarrierSignal(uint carrier_id)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    return DevList.Find(c => c.ID == carrier_id)?.Signal ?? DevCarrierSignalE.其他;
                }
                finally { Monitor.Exit(_obj); }
            }
            return DevCarrierSignalE.其他;
        }

        /// <summary>
        /// 小车当前是否状态符合
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        private bool CheckCarrierFreeNotLoad(CarrierTask carrier)
        {
            if (carrier == null) return false;
            if (!carrier.IsWorking) return false;
            if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                    && carrier.Status == DevCarrierStatusE.停止
                    && carrier.OperateMode == DevOperateModeE.自动
                    //&& carrier.WorkMode == DevCarrierWorkModeE.生产模式
                    && (carrier.Task == carrier.FinishTask || carrier.Task == DevCarrierTaskE.无)
                    //&& carrier.Load == DevCarrierLoadE.无货
                    )
            {
                return true;
            }
            return false;
        }

        internal List<CarrierTask> GetDevCarriers()
        {
            return DevList;
        }

        /// <summary>
        /// 小车当前是否状态符合/不管有没有货
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        private bool CheckCarrierFreeNoTask(CarrierTask carrier)
        {
            if (!carrier.IsWorking) return false;
            if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                    && carrier.Status == DevCarrierStatusE.停止
                    && carrier.OperateMode == DevOperateModeE.自动
                    //&& carrier.WorkMode == DevCarrierWorkModeE.生产模式
                    && (carrier.Task == carrier.FinishTask || carrier.Task == DevCarrierTaskE.无)
                    //&& carrier.Load == DevCarrierLoadE.无货
                    )
            {
                return true;
            }
            return false;
        }

        public bool IsCarrierFree(uint carrierid)
        {
            CarrierTask carrier = DevList.Find(c => c.ID == carrierid);
            return CheckCarrierFreeNotLoad(carrier);
        }

        private bool GetCarrierInTrack(uint trackid)
        {
            Track track = PubMaster.Track.GetTrack(trackid);

            CarrierTask carrier = DevList.Find(c => track.IsInTrack(c.Site));
            if (carrier != null)
            {

            }

            return false;
        }

        #endregion

        #region[判断条件]

        /// <summary>
        /// 判断小车是否可以前往轨道
        /// 1.该轨道是否已经有小车
        /// 2.该轨道的兄弟轨道是否有车
        /// 3.两轨道间是否存在库存大于3
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool CanCarrierEnterTrack(uint trackid)
        {
            CarrierTask carrier = DevList.Find(c => c.TrackId == trackid);
            if (carrier != null) return false;

            uint brotherid = PubMaster.Track.GetBrotherTrackId(trackid);
            if (brotherid == 0)
            {
                //没有兄弟轨道，不会碰撞？？(可能也是配置漏了)
                return true;
            }
            carrier = DevList.Find(c => c.TrackId == brotherid);
            if (carrier != null)
            {
                if ((PubMaster.Goods.GetTrackStock(trackid)
                    + PubMaster.Goods.GetTrackStock(brotherid)) > 3)
                {
                    return true;
                }
            }

            return true;
        }

        internal ushort GetCarrierSite(uint devId)
        {
            return DevList.Find(c => c.ID == devId)?.DevStatus?.CurrentSite ?? 0;
        }

        /// <summary>
        /// 判断小车是否完成了取砖任务
        /// 1.在指定的轨道
        /// 2.并且
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsCarrierFinishLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.Load == DevCarrierLoadE.有货
                                    //&& c.FinishTask == DevCarrierTaskE.后退取砖
                                    //&& c.Task == DevCarrierTaskE.后退取砖
                                    && (c.Task == c.FinishTask || c.Task == DevCarrierTaskE.无)
                                    );
        }

        /// <summary>
        /// 判断小车是否完成了取砖任务
        /// 1.在指定的轨道
        /// 2.并且
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsCarrierFinishUnLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.Load == DevCarrierLoadE.无货
                                    //&& c.FinishTask == DevCarrierTaskE.前进放砖
                                    //&& c.Task == DevCarrierTaskE.前进放砖
                                    && (c.Task == c.FinishTask || c.Task == DevCarrierTaskE.无)
                                    );
        }

        internal bool IsCarrierInTask(uint carrier_id, DevCarrierTaskE task)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Task == task
                                    && c.Task != c.FinishTask);
        }

        internal bool IsCarrierFinishTask(uint carrier_id, DevCarrierTaskE task)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.Task == task
                                    && c.FinishTask == task);
        }

        /// <summary>
        /// 判断小车是否处于任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="carriertask"></param>
        /// <returns></returns>
        internal bool IsTaskAndDoTask(uint carrier_id, DevCarrierTaskE carriertask)
        {
            CarrierTask task = DevList.Find(c => c.ID == carrier_id);
            if (task != null)
            {
                if (task.Task == carriertask && task.FinishTask == DevCarrierTaskE.终止)
                {
                    return true;
                }
                task.DoTask(carriertask, DevCarrierSizeE.非超限);
            }
            return false;
        }

        internal bool IsCarrierMoveInTrack(uint trackid)
        {
            List<CarrierTask> carriers = DevList.FindAll(c => c.TrackId == trackid
                                && (c.Status != DevCarrierStatusE.停止 || c.OperateMode == DevOperateModeE.手动 ||
                                (c.Task != c.FinishTask && c.Task != DevCarrierTaskE.无) || c.CarrierPosition == DevCarrierPositionE.上下摆渡中));

            return carriers.Count > 0;
        }

        #endregion

        #region[小车逻辑警告]

        public void SetCarrierAlert(uint carrierid, uint trackid, CarrierAlertE type, bool isalert)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == carrierid);
                    if (task != null)
                    {
                        task.SetAlert(type, trackid, isalert);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }
        #endregion

        #region[启动/停止]

        public void UpdateWorking(uint devId, bool working)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                CarrierTask task = DevList.Find(c => c.ID == devId);
                if (task != null)
                {
                    task.IsWorking = working;
                    MsgSend(task, task.DevStatus);
                }
            }
            finally { Monitor.Exit(_obj); }

        }

        #endregion
    }
}
