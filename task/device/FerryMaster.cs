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
using System.Threading;
using task.task;
using tool.mlog;
using tool.timer;

namespace task.device
{
    /// <summary>
    /// 摆渡车
    /// </summary>
    public class FerryMaster
    {
        #region[字段]
        private object _objmsg;
        private MsgAction mMsg;
        private List<FerryTask> DevList { set; get; }

        private readonly object _obj;
        private readonly object _posobj; //用于锁定轨道坐标值的poslist

        private Thread _mRefresh;
        private bool Refreshing = true;
        private MTimer mTimer;

        #region[摆渡对位]
        private bool _IsSetting;
        private bool _IsRefreshPos;
        private List<FerryPosSet> _FerryPosSetList;
        private Log mlog;
        private bool isWcsStoping = false;
        private List<FerryPos> PosList { set; get; }
        #endregion

        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public FerryMaster()
        {
            mlog = (Log)new LogFactory().GetLog("Ferry", false);
            mTimer = new MTimer();
            _objmsg = new object();
            mMsg = new MsgAction();
            _obj = new object();
            _posobj = new object();
            DevList = new List<FerryTask>();
            _FerryPosSetList = new List<FerryPosSet>();
            PosList = new List<FerryPos>();

            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.FerryMsgUpdate, FerryMsgUpdate);
        }

        public void Start()
        {
            List<Device> ferrys = PubMaster.Device.GetFerrys();
            foreach (Device dev in ferrys)
            {
                FerryTask task = new FerryTask
                {
                    Device = dev,
                    DevConfig = PubMaster.DevConfig.GetFerry(dev.id)
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
            foreach (FerryTask task in DevList)
            {
                task.Stop("调度关闭连接停止");
            }
        }

        /// <summary>
        /// 停止模拟的设备连接
        /// </summary>
        internal void StockSimDevice()
        {
            List<FerryTask> tasks = DevList.FindAll(c => c.IsConnect && c.Device.ip.Equals("127.0.0.1"));
            foreach (FerryTask task in tasks)
            {
                if (task.IsEnable)
                {
                    task.Device.enable = false;
                }
                task.Stop("模拟停止");
            }
        }

        public void ReStart()
        {

        }

        private void Refresh()
        {
            while (Refreshing)
            {
                try
                {
                    foreach (FerryTask task in DevList)
                    {
                        try
                        {
                            if (task.IsEnable && _IsSetting && _FerryPosSetList.Find(c => c.FerryId == task.ID) is FerryPosSet set)
                            {
                                task.DoSiteQuery(set.QueryPos);
                                Thread.Sleep(1000);
                            }

                            if (_IsRefreshPos && Monitor.TryEnter(_posobj, TimeSpan.FromSeconds(1)))
                            {
                                try
                                {
                                    foreach (FerryPos fp in PosList)
                                    {
                                        PubTask.Ferry.QueryPosList(fp.device_id, fp.ferry_code);
                                        Thread.Sleep(100);

                                    }
                                }
                                finally
                                {
                                    EndRefreshPosList();
                                    Monitor.Exit(_posobj);
                                }

                            }

                            // 摆渡车反馈的报警
                            task.CheckAlert();

                            if (task.IsConnect && task.Status == DevFerryStatusE.停止 && task.DevStatus.CurrentTask == DevFerryTaskE.定位)
                            {
                                //上砖测轨道ID 或 下砖测轨道ID
                                if (task.IsUpLight && task.UpTrackId == PubMaster.Track.GetAreaTrack(task.AreaId, (ushort)task.AreaId, task.Type, task.DevStatus.TargetSite))
                                {
                                    task.DoStop();
                                    Thread.Sleep(1000);
                                }

                                if (task.IsDownLight && task.DownTrackId == PubMaster.Track.GetAreaTrack(task.AreaId, (ushort)task.AreaId, task.Type, task.DevStatus.TargetSite))
                                {
                                    task.DoStop();
                                    Thread.Sleep(1000);
                                }
                            }

                            #region 上砖待命点 (单摆渡对多上砖机)

                            //if (task.Type == DeviceTypeE.上摆渡 && task.Status == DevFerryStatusE.停止 &&
                            //    task.DevStatus.CurrentTask == DevFerryTaskE.终止 && task.DevStatus.TargetSite == 0)
                            //{
                            //    // 当上砖机都有货，摆渡车 空车无锁定 时，移至待命点（原点 12&13 之间）
                            //    if (task.IsFerryFree() &&
                            //        !PubTask.Trans.IsExistsTask(task.AreaId, TransTypeE.出库) &&
                            //        !PubTask.TileLifter.IsAnyoneNeeds(task.AreaId, DeviceTypeE.上砖机))
                            //    {
                            //        short trackOrder = PubMaster.Track.GetTrack(task.DownTrackId)?.order ?? 0;
                            //        if (trackOrder != 0)
                            //        {
                            //            // 513
                            //            if (trackOrder < 12 || trackOrder > 14)
                            //            {
                            //                task.DoLocate(513);
                            //            }
                            //        }
                            //    }
                            //}

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

                        }
                        catch (Exception e)
                        {
                            mlog.Error(true, e.Message, e);
                        }
                        finally
                        {
                            if (task.IsEnable && task.IsConnect)
                            {
                                task.DoQuery();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    mlog.Error(true, e.Message, e);
                }
                Thread.Sleep(2000);
            }
        }


        public bool DoClearOtherTrackPos(uint id, out string rs)
        {
            rs = "";
            FerryTask task = DevList.Find(c => c.ID == id);
            if (task != null)
            {
                if (!task.IsConnect)
                {
                    rs = "设备离线！";
                    return false;
                }

                if (!task._cleaning)
                {
                    List<FerryPos> ferryPos = PubMaster.Track.GetFerryPos(id);
                    task.StartClearOtherTrackPos(ferryPos);
                    return true;
                }

                rs = "正在清除中！";
            }
            return false;
        }

        public void StartStopFerry(uint ferryid, bool isstart)
        {
            FerryTask task = DevList.Find(c => c.ID == ferryid);
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

        #endregion

        #region[获取信息]

        /// <summary>
        /// 判断摆渡车是否在线
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool IsOnline(uint devid)
        {
            return DevList.Exists(c => c.ID == devid && c.IsConnect);
        }


        public void GetAllFerry()
        {
            foreach (FerryTask task in DevList)
            {
                MsgSend(task, task.DevStatus);
            }
        }

        internal List<FerryTask> GetDevFerrys()
        {
            return DevList;
        }

        internal List<FerryTask> GetDevFerrys(List<uint> areaids)
        {
            return DevList.FindAll(c => areaids.Contains(c.AreaId));
        }

        internal FerryTask GetFerry(uint devid)
        {
            return DevList.Find(c => c.ID == devid);
        }

        internal List<FerryTask> GetDevFerrys(List<DeviceTypeE> types)
        {
            return DevList.FindAll(c => types.Contains(c.Type));
        }

        internal List<FerryTask> GetDevFerrys(List<uint> areaids, List<DeviceTypeE> types)
        {
            return DevList.FindAll(c => types.Contains(c.Type) && areaids.Contains(c.AreaId));
        }

        /// <summary>
        /// 摆渡轨道ID
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetFerryTrackId(uint devid)
        {
            return DevList.Find(c => c.ID == devid)?.FerryTrackId ?? 0;
        }

        /// <summary>
        /// 摆渡车当前对应轨道ID
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetFerryCurrentTrackId(uint devid)
        {
            return DevList.Find(c => c.ID == devid)?.GetFerryCurrentTrackId() ?? 0;
        }

        public void UpdateFerryWithTrackId(uint trackid, DevFerryLoadE devFerryLoadE)
        {
            FerryTask ferry = DevList.Find(c => c.DevConfig.track_id == trackid);
            if (ferry != null && ferry.DevStatus.LoadStatus != devFerryLoadE)
            {
                ferry.DevStatus.LoadStatus = devFerryLoadE;
                MsgSend(ferry, ferry.DevStatus);
            }
        }

        /// <summary>
        /// 获取摆渡车锁定的任务ID
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetFerryTransId(uint devid)
        {
            return DevList.Find(c => c.ID == devid && c.IsLock)?.TransId ?? 0;
        }

        #endregion

        #region[数据更新]

        private void FerryMsgUpdate(SocketMsgMod mod)
        {
            if (isWcsStoping) return;
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        FerryTask task = DevList.Find(c => c.ID == mod.ID);
                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is DevFerry ferry)
                            {
                                task.ReSetRefreshTime();
                                if (task.DevStatus != null)
                                {
                                    ferry.LoadStatus = task.DevStatus.LoadStatus;
                                }
                                task.DevStatus = ferry;
                                task.UpdateInfo();
                                if (ferry.IsUpdate || mTimer.IsTimeOutAndReset(TimerTag.DevRefreshTimeOut, ferry.ID, 5))
                                {
                                    MsgSend(task, ferry);
                                }

                                ///摆渡车对位中
                                if (_IsSetting)
                                {
                                    if (_FerryPosSetList.Find(c => c.FerryId == task.ID && !c.IsRF) is FerryPosSet set)
                                    {
                                        PosMsgSend(task, ferry);
                                    }

                                    if (_FerryPosSetList.Find(c => c.FerryId == task.ID && c.IsRF) is FerryPosSet rfset)
                                    {
                                        RfPosMsgSend(rfset, ferry);
                                    }
                                }
                            }

                            if (mod.Device is DevFerrySite site)
                            {
                                task.DevSite = site;

                                if (_FerryPosSetList.Find(c => c.FerryId == task.ID && !c.IsRF) is FerryPosSet fset)
                                {
                                    PosMsgSend(task, site);
                                }

                                if (_FerryPosSetList.Find(c => c.FerryId == task.ID && c.IsRF) is FerryPosSet rfset)
                                {
                                    RfPosSiteMsgSend(rfset, task.ID, site);
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

        private void CheckConn(FerryTask task)
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

        #region[执行任务]

        internal bool TryLock(StockTrans trans, uint ferryid, uint carriertrackid)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return false;
            }
            try
            {
                FerryTask task = DevList.Find(c => c.ID == ferryid);
                if (task == null) return false;

                //小车在摆渡车上，直接锁定
                if (task.DevConfig.track_id == carriertrackid)
                {
                    if (!task.IsStillLockInTrans(trans.id))
                    {
                        task.SetFerryLock(trans.id);
                    }
                    return true;
                }

                if (task.IsStillLockInTrans(trans.id))
                {
                    return true;
                }

                if (task.IsFerryFree())
                {
                    task.SetFerryLock(trans.id);
                }

                return false;
            }
            finally { Monitor.Exit(_obj); }
        }

        public bool StopFerry(uint id, out string result)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                result = "稍后再试！";
                return false;
            }
            try
            {
                FerryTask task = DevList.Find(c => c.ID == id);
                if (!CheckFerryStatus(task, out result))
                {
                    return false;
                }
                task.DoStop();
                mlog.Info(true, string.Format(@"摆渡车[ {0} ],  手动终止", task.ID));
                return true;
            }
            finally
            {
                Monitor.Exit(_obj);
            }
        }

        /// <summary>
        /// 手动摆渡车定位
        /// </summary>
        /// <param name="ferryid">摆渡车ID</param>
        /// <param name="trackid">轨道ID</param>
        /// <param name="isdownferry">是否是下砖摆渡</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        public bool DoManualLocate(uint ferryid, uint trackid, bool isdownferry, out string result)
        {
            Track tra = PubMaster.Track.GetTrack(trackid);
            if (tra == null)
            {
                result = "找不到目的轨道信息！";
                return false;
            }

            if (!PubMaster.Area.IsFerrySetTrack(ferryid, tra.id))
            {
                result = "该摆渡车未配置" + tra.name + "！";
                return false;
            }

            ushort ferrycode = tra.ferry_down_code;
            if (isdownferry)
            {
                if (tra.Type == TrackTypeE.上砖轨道 || tra.Type == TrackTypeE.储砖_出)
                {
                    result = "请选择下砖区域的轨道";
                    return false;
                }
            }
            else
            {   //上砖摆渡
                if (tra.Type == TrackTypeE.下砖轨道 || tra.Type == TrackTypeE.储砖_入)
                {
                    result = "请选择上砖区域的轨道";
                    return false;
                }
            }

            switch (tra.Type)
            {
                case TrackTypeE.上砖轨道:
                    ferrycode = tra.ferry_up_code;
                    break;
                case TrackTypeE.下砖轨道:
                    ferrycode = tra.ferry_down_code;
                    break;
                case TrackTypeE.储砖_入:
                    ferrycode = tra.ferry_up_code;
                    break;
                case TrackTypeE.储砖_出:
                    ferrycode = tra.ferry_down_code;
                    break;
                case TrackTypeE.储砖_出入:
                    ferrycode = isdownferry ? tra.ferry_up_code : tra.ferry_down_code;
                    break;
                case TrackTypeE.摆渡车_入:
                case TrackTypeE.摆渡车_出:
                    result = "请重新选择其他轨道";
                    return false;
            }

            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    FerryTask task = DevList.Find(c => c.ID == ferryid);

                    if (!IsAllowToMove(task, out result))
                    {
                        task.DoStop();
                        return false;
                    }

                    // 避让不让发指令
                    if (ExistsAvoid(task, trackid, out result, false))
                    {
                        return false;
                    }

                    task.DoLocate(ferrycode, task.DevConfig.track_id, trackid);
                    mlog.Info(true, string.Format(@"摆渡车[ {0} ],  手动定位[ {1} ]", task.ID, ferrycode));
                    return true;
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
            result = "稍后再试！";
            return false;
        }

        /// <summary>
        /// 终止摆渡车
        /// </summary>
        /// <param name="trackid"></param>
        internal void StopFerryByFerryTrackId(uint trackid)
        {
            uint ferryid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(trackid);
            if (ferryid > 0)
            {
                StopFerry(ferryid, out string _);
            }
        }

        public bool SetFerryPos(uint id, ushort ferry_code, int intpos, out string result)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                result = "稍后再试！";
                return false;
            }
            try
            {
                FerryTask task = DevList.Find(c => c.ID == id);
                if (!CheckFerryStatus(task, out result))
                {
                    return false;
                }
                task.DoSiteUpdate(ferry_code, intpos);
                //_FerrySiteCode = ferry_code;
                return true;
            }
            finally
            {
                Monitor.Exit(_obj);
            }
        }

        /// <summary>
        /// 摆渡车复位原点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resettype"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool ReSetFerry(uint id, DevFerryResetPosE resettype, out string result)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                result = "稍后再试！";
                return false;
            }
            try
            {
                FerryTask task = DevList.Find(c => c.ID == id);

                if (!IsAllowToMove(task, out result))
                {
                    task.DoStop();
                    return false;
                }

                task.DoReSet(resettype);
                mlog.Info(true, string.Format(@"摆渡车[ {0} ],  手动复位[ {1} ]", task.ID, resettype));
                return true;
            }
            finally
            {
                Monitor.Exit(_obj);
            }
        }

        public bool ReSendAllFerryPose(uint id, out string result)
        {
            result = "";
            FerryTask task = DevList.Find(c => c.ID == id);
            if (task != null)
            {
                if (!task.IsSendAll)
                {
                    task.DoSendAllPose();
                    return true;
                }
            }
            else
            {
                result = "找不到摆渡车信息";
            }
            return false;
        }


        /// <summary>
        /// 自动流程中摆渡车定位
        /// </summary>
        /// <param name="ferryid"></param>
        /// <param name="to_track_id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool DoLocateFerry(uint ferryid, uint to_track_id, out string result)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                result = "稍后再试！";
                return false;
            }

            FerryTask task = DevList.Find(c => c.ID == ferryid);
            try
            {
                if (!IsAllowToMove(task, out result))
                {
                    return false;
                }

                if (task.Status == DevFerryStatusE.停止
                    && (task.DevStatus.CurrentTask == task.DevStatus.FinishTask // 当前&完成 一致
                        || task.DevStatus.CurrentTask == DevFerryTaskE.无 // 当前无指令就当它没作业
                        || task.DevStatus.CurrentTask == DevFerryTaskE.终止 // 当前终止就当它没作业
                        //|| (task.DevStatus.CurrentTask == DevFerryTaskE.定位 && task.DevStatus.FinishTask == DevFerryTaskE.无) // 当前定位无完成就当它没作业
                        ))
                {
                    uint trid = PubMaster.Track.GetTrackId(ferryid, (ushort)task.AreaId, task.DevStatus.TargetSite);
                    if (task.DevStatus.TargetSite != 0
                        && trid != to_track_id
                        && trid != task.RecordTraId)
                    {
                        Thread.Sleep(500);
                        task.DoStop();
                        result = string.Format("[ {0} & {1} ]: 消除残留目标点", task.ID, task.Device.name);
                        return false;
                    }

                    // 上砖测轨道ID 前侧
                    if (task.UpTrackId == to_track_id && task.IsUpLight)
                    {
                        if (task.DevStatus.CurrentTask == DevFerryTaskE.终止)
                        {
                            result = string.Format("[ {0} & {1} ]: 定位完成", task.ID, task.Device.name);
                            return true;
                        }
                        else
                        {
                            Thread.Sleep(500);
                            task.DoStop();
                            result = string.Format("[ {0} & {1} ]: 到位执行终止", task.ID, task.Device.name);
                        }

                        return false;
                    }
                    // 下砖测轨道ID 后侧
                    if (task.DownTrackId == to_track_id && task.IsDownLight)
                    {
                        if (task.DevStatus.CurrentTask == DevFerryTaskE.终止)
                        {
                            result = string.Format("[ {0} & {1} ]: 定位完成", task.ID, task.Device.name);
                            return true;
                        }
                        else
                        {
                            Thread.Sleep(500);
                            task.DoStop();
                            result = string.Format("[ {0} & {1} ]: 到位执行终止", task.ID, task.Device.name);
                        }

                        return false;
                    }

                    #region 交管 

                    if (ExistsAvoid(task, to_track_id, out result))
                    {
                        mlog.Info(true, string.Format(@"定位车[ {0} & {1} ], 存在避让[ {2} ]", task.ID, task.Device.name, result));
                        return false;
                    }
                    mlog.Info(true, string.Format(@"定位车[ {0} & {1} ], 无需避让[ {2} ]", task.ID, task.Device.name, result));

                    #endregion

                    #region 定位前检查同轨道的摆渡车 - 停用，改启用楼上的 交管
                    //List<AreaDevice> areatras = PubMaster.Area.GetAreaDevList(task.AreaId, task.Type);
                    //if (areatras != null && areatras.Count > 0)
                    //{
                    //    uint taskTrackId;
                    //    short trackOrder;
                    //    short takeTrackOrder = PubMaster.Track.GetTrack(to_track_id)?.order ?? 0;
                    //    int safedis = PubMaster.Dic.GetDtlIntCode(DicTag.FerryAvoidNumber);
                    //    foreach (AreaDevice ferry in areatras)
                    //    {
                    //        if (ferry.device_id != ferryid)
                    //        {
                    //            //同区域另一台摆渡车
                    //            FerryTask taskB = DevList.Find(c => c.ID == ferry.device_id);
                    //            if (!CheckFerryStatus(taskB, out string r))
                    //            {
                    //                continue;
                    //            }

                    //            //另一台摆渡车对着的轨道id
                    //            //uint taskBTrackId = task.Type == DeviceTypeE.上摆渡 ? taskB.DownTrackId : taskB.UpTrackId;
                    //            uint taskBTrackId = taskB.GetFerryCurrentTrackId();

                    //            short trackBOrder = PubMaster.Track.GetTrack(taskBTrackId)?.order ?? 0;

                    //            //另一台摆渡车的目的轨道的顺序
                    //            //uint anotherTarget = PubMaster.Track.GetTrackId(taskB.DevStatus.TargetSite);
                    //            short taskBTargetOrder = PubMaster.Track.GetTrackByPoint((ushort)taskB.AreaId, taskB.DevStatus.TargetSite)?.order ?? 0;

                    //            //当前摆渡车对着的轨道id
                    //            //taskTrackId = task.Type == DeviceTypeE.上摆渡 ? task.DownTrackId : task.UpTrackId;
                    //            taskTrackId = task.GetFerryCurrentTrackId();

                    //            //当前摆渡车对着的轨道的顺序
                    //            trackOrder = PubMaster.Track.GetTrack(taskTrackId)?.order ?? 0;

                    //            if (trackBOrder == 0 || trackOrder == 0)
                    //            {
                    //                return false;
                    //            }

                    //            int leftCompare, rightCompare;
                    //            if (trackOrder >= takeTrackOrder)
                    //            {
                    //                leftCompare = takeTrackOrder - safedis;
                    //                rightCompare = trackOrder + safedis;
                    //            }
                    //            else
                    //            {
                    //                leftCompare = trackOrder - safedis;
                    //                rightCompare = takeTrackOrder + safedis;
                    //            }
                    //            leftCompare = leftCompare < 0 ? 0 : leftCompare;
                    //            switch (taskB.Status)
                    //            {
                    //                case DevFerryStatusE.停止:
                    //                    //当前摆渡车要前进
                    //                    if ((leftCompare < trackBOrder && trackBOrder < rightCompare)
                    //                           || (leftCompare < taskBTargetOrder && taskBTargetOrder < rightCompare))
                    //                    {
                    //                        if (taskB.IsFerryLock() || !IsAllowToMove(taskB, out result))
                    //                        {
                    //                            return false;
                    //                        }
                    //                        uint avoidTrackId;
                    //                        if (trackOrder < takeTrackOrder)
                    //                        {
                    //                            avoidTrackId = PubMaster.Track.GetTrackIdByDifference(to_track_id, safedis, true);
                    //                        }
                    //                        else
                    //                        {
                    //                            avoidTrackId = PubMaster.Track.GetTrackIdByDifference(to_track_id, safedis, false);
                    //                        }
                    //                        if (PubMaster.Track.GetTrackFerryCode(avoidTrackId, task.Type, out ushort newtrackferrycode, out result))
                    //                        {
                    //                            taskB.DoLocate(newtrackferrycode, task.DevConfig.track_id);
                    //                            return false;
                    //                        }
                    //                    }
                    //                    break;
                    //                case DevFerryStatusE.前进:
                    //                case DevFerryStatusE.后退:
                    //                    //当前摆渡车在另一台摆渡车的后面
                    //                    if ((leftCompare < trackBOrder && trackBOrder < rightCompare)
                    //                           || (leftCompare < taskBTargetOrder && taskBTargetOrder < rightCompare))
                    //                    {
                    //                        return false;
                    //                    }
                    //                    break;
                    //                default:
                    //                    return false;
                    //            }

                    //        }
                    //    }

                    //}
                    #endregion

                    // 发送定位
                    if (PubMaster.Track.GetTrackFerryCode(to_track_id, task.Type, out ushort trackferrycode, out result))
                    {
                        task.DoLocate(trackferrycode, task.DevConfig.track_id, to_track_id);
                    }
                }
            }
            finally
            {
                Monitor.Exit(_obj);
            }

            result = string.Format("[ {0} & {1} ]: 移动中", task.ID, task.Device.name);
            return false;
        }

        internal bool UnlockFerry(StockTrans trans, uint ferryid)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return false;
            }
            try
            {
                FerryTask task = DevList.Find(c => c.ID == ferryid);
                if (task != null)
                {
                    task.SetFerryUnlock(trans.id);
                }
                return true;
            }
            finally
            {
                Monitor.Exit(_obj);
            }
        }

        /// <summary>
        /// 是否存在避让
        /// </summary>
        public bool ExistsAvoid(FerryTask task, uint to_track_id, out string msg, bool isAdd = true)
        {
            // 确认是否已被交管
            if (PubTask.TrafficControl.ExistsRestricted(task.ID))
            {
                msg = string.Format("[ {0} & {1} ]: 被交管中,不可移动", task.ID, task.Device.name);
                return true;
            }

            // 同区域内其他同类型的摆渡车
            List<FerryTask> ferries = DevList.FindAll(c => c.AreaId == task.AreaId && c.Type == task.Type && c.ID != task.ID);
            if (ferries == null || ferries.Count == 0)
            {
                msg = string.Format("[ {0} & {1} ]: 同区域内无车干扰", task.ID, task.Device.name);
                return false;
            }

            #region 【定位车移动位置信息】

            // 当前轨道ID
            uint TrackId = task.GetFerryCurrentTrackId();

            #region 失去位置信息
            Track currentTrack = PubMaster.Track.GetTrack(TrackId);
            if (currentTrack == null || TrackId == 0 || TrackId.Equals(0) || TrackId.CompareTo(0) == 0)
            {
                msg = string.Format("[ {0} & {1} ]: 没有当前位置信息[ {2} ]", task.ID, task.Device.name, TrackId);
                return true;
            }

            Track toTrack = PubMaster.Track.GetTrack(to_track_id);
            if (toTrack == null || to_track_id == 0 || to_track_id.Equals(0) || to_track_id.CompareTo(0) == 0)
            {
                msg = string.Format("[ {0} & {1} ]: 没有目的位置信息[ {2} ]", task.ID, task.Device.name, to_track_id);
                return true;
            }
            #endregion

            // 当前摆渡车对着的轨道的顺序
            short fromOrder = currentTrack?.order ?? 0;
            // 目的轨道顺序
            short toOrder = toTrack?.order ?? 0;

            #region 没有相对位置序号
            if (fromOrder == 0 || fromOrder.Equals(0) || fromOrder.CompareTo(0) == 0)
            {
                msg = string.Format("[ {0} & {1} ]: 未获取到当前轨道相对位置顺序用于避让[ {2} ]", task.ID, task.Device.name, TrackId);
                return true;
            }

            if (toOrder == 0 || toOrder.Equals(0) || toOrder.CompareTo(0) == 0)
            {
                msg = string.Format("[ {0} & {1} ]: 未获取到目的轨道相对位置顺序用于避让[ {2} ]", task.ID, task.Device.name, to_track_id);
                return true;
            }
            #endregion

            if (fromOrder == toOrder)
            {
                msg = string.Format("[ {0} & {1} ]: 移动前后相对位置一致", task.ID, task.Device.name);
                return false;
            }

            // 摆渡间安全距离（轨道数）
            int safedis = PubMaster.Dic.GetDtlIntCode(DicTag.FerryAvoidNumber);
            // 摆渡轨道最小值（默认相对位置顺序以 1 记起）
            int mindis = PubMaster.Track.GetMinOrder((ushort)task.AreaId, task.Type);
            // 摆渡轨道最大值
            int maxdis = PubMaster.Track.GetMaxOrder((ushort)task.AreaId, task.Type);

            // 当前摆渡车移动区间 
            int limitMin, limitMax, standbyOrder;
            if (fromOrder > toOrder)
            {
                // 后退的安全范围
                limitMin = (toOrder - safedis) < mindis ? mindis : (toOrder - safedis);
                limitMax = fromOrder;
                // 待命点
                standbyOrder = limitMin;
            }
            else
            {
                // 前进的安全范围
                limitMin = fromOrder;
                limitMax = (toOrder + safedis) > maxdis ? maxdis : (toOrder + safedis);
                // 待命点
                standbyOrder = limitMax;
            }

            if (standbyOrder == 0 || standbyOrder.Equals(0) || standbyOrder.CompareTo(0) == 0)
            {
                msg = string.Format("[ {0} & {1} ]: 无法得到移动前其他车需避让到的安全待命点", task.ID, task.Device.name);
                return true;
            }
            #endregion

            //循环判断 
            foreach (FerryTask other in ferries)
            {
                // 断开通讯并且停用摆渡车
                if (!other.IsWorking && !other.IsEnable) continue;

                #region 【同坑内另一台车的移动位置信息】

                // 其一摆渡当前轨道ID
                uint otherTrackId = other.GetFerryCurrentTrackId();

                #region 失去位置信息
                Track otherTrack = PubMaster.Track.GetTrack(otherTrackId);
                if (otherTrack == null || otherTrackId == 0 || otherTrackId.Equals(0) || otherTrackId.CompareTo(0) == 0)
                {
                    msg = string.Format("同坑内另一台车[ {0} & {1} ]: 没有当前位置信息[ {2} ]", other.ID, other.Device.name, otherTrackId);
                    return true;
                }
                #endregion

                // 其一摆渡当前轨道顺序
                short otherOrder = otherTrack?.order ?? 0;

                #region 没有相对位置序号
                if (otherOrder == 0 || otherOrder.Equals(0) || otherOrder.CompareTo(0) == 0)
                {
                    msg = string.Format("同坑内另一台车[ {0} & {1} ]: 未获取到当前轨道相对位置顺序用于避让[ {2} ]", other.ID, other.Device.name, otherTrackId);
                    return true;
                }
                #endregion

                // 其一摆渡目的轨道顺序
                short otherToOrder = PubMaster.Track.GetTrackBySite((ushort)other.AreaId, other.Type, other.DevStatus.TargetSite)?.order ?? 0;

                // 使用 记录目标点
                if (otherToOrder == 0 || otherToOrder.Equals(0) || otherToOrder.CompareTo(0) == 0)
                {
                    otherToOrder = PubMaster.Track.GetTrackOrder(other.RecordTraId);
                }

                #endregion

                // 记录
                mlog.Info(true, string.Format(@"定位车[ {0} & {1} ], _移序[ {2} - {3} ]_安全范围[ {4} ~ {5} ], 同坑内另一车[ {6} & {7} ]_移序[ {8} - {9} ]",
                    task.ID, task.Device.name, fromOrder, toOrder, limitMin, limitMax, 
                    other.ID, other.Device.name, otherOrder, otherToOrder));

                #region 【交管判断】
                bool isOtherRunning = true; // other车在移动
                if (otherToOrder == 0 || otherToOrder.Equals(0) || otherToOrder.CompareTo(0) == 0) isOtherRunning = false; // other车停止

                // 1. other车的当前位置在安全范围内？
                if (limitMin < otherOrder && otherOrder < limitMax)
                {
                    // 范围内则考虑交管
                    if (isOtherRunning)
                    {
                        // 移动中不能生成交管，跳过
                        msg = string.Format("同坑内另一台车[ {0} & {1} ]: 是阻碍并在移动中", other.ID, other.Device.name);
                        return true;
                    }
                    else
                    {
                        // 确认是否已被同类型交管
                        if (PubTask.TrafficControl.ExistsTrafficControl(TrafficControlTypeE.摆渡车交管摆渡车, other.ID))
                        {
                            msg = string.Format("同坑内另一台车[ {0} & {1} ]: 已被交管中", other.ID, other.Device.name);
                            return true;
                        }
                        
                        // 安全待命点不变
                        if (standbyOrder == otherOrder)
                        {
                            msg = string.Format("同坑内另一台车[ {0} & {1} ]: 已到位安全待命点", other.ID, other.Device.name);
                            return true;
                        }

                        // 没有移动则生成交管
                        if (isAdd)
                        {
                            uint standbyTraID = PubMaster.Track.GetTrackIDByOrder((ushort)other.AreaId, other.Type, standbyOrder);
                            // 加入交管
                            PubTask.TrafficControl.AddTrafficControl(new TrafficControl()
                            {
                                area = (ushort)task.AreaId,
                                TrafficControlType = TrafficControlTypeE.摆渡车交管摆渡车,
                                restricted_id = task.ID,
                                control_id = other.ID,
                                from_track_id = otherTrackId,
                                to_track_id = standbyTraID
                            }, out msg);
                            return true; // 限制仅生成一个交管
                        }
                        else
                        {
                            // 只提示，不加入交管
                            msg = string.Format("同坑内另一台车[ {0} & {1} ]: 是阻碍", other.ID, other.Device.name);
                            return true;
                        }
                    }
                }
                // 2. 移动中的other车，目的位置在安全范围内？
                else
                {
                    if (isOtherRunning && limitMin < otherToOrder && otherToOrder < limitMax)
                    {
                        // 移动中不能生成交管，跳过
                        msg = string.Format("同坑内另一台车[ {0} & {1} ]: 是阻碍并在移动中", other.ID, other.Device.name);
                        return true;
                    }
                }
                #endregion

            }
            msg = "未检测到需要避让，可以移动";
            return false;
        }

        #endregion

        #region[发送信息]
        private void MsgSend(FerryTask task, DevFerry ferry)
        {
            if (Monitor.TryEnter(_objmsg, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    mMsg.ID = task.ID;
                    mMsg.Name = task.Device.name;
                    mMsg.o1 = ferry;
                    mMsg.o2 = task.ConnStatus;
                    mMsg.o3 = task.IsWorking;
                    mMsg.o4 = task.UpTrackId;
                    mMsg.o5 = task.DownTrackId;
                    Messenger.Default.Send(mMsg, MsgToken.FerryStatusUpdate);
                }
                finally
                {
                    Monitor.Exit(_objmsg);
                }
            }
        }

        private void PosMsgSend(FerryTask task, DevFerrySite site)
        {
            mMsg.ID = task.ID;
            mMsg.Name = task.Device.name;
            mMsg.o1 = site;
            Messenger.Default.Send(mMsg, MsgToken.FerrySiteUpdate);
        }

        private void PosMsgSend(FerryTask task, DevFerry ferry)
        {
            mMsg.ID = task.ID;
            mMsg.Name = task.Device.name;
            mMsg.o1 = ferry;
            Messenger.Default.Send(mMsg, MsgToken.FerrySiteUpdate);
        }

        /// <summary>
        /// 对位的光电状态
        /// </summary>
        /// <param name="set"></param>
        /// <param name="ferry"></param>
        private void RfPosMsgSend(FerryPosSet set, DevFerry ferry)
        {
            if (!PubTask.Rf.SendFerryLightPos(set.MEID, ferry.UpSite, ferry.DownSite, ferry.UpLight, ferry.DownLight)
                && mTimer.IsOver(TimerTag.RfFerrySiteUpdateSendOffline, ferry.DeviceID, 60))
            {
                StopRfPosSet(set.MEID);
            }
        }

        private void RfPosSiteMsgSend(FerryPosSet set, uint devid, DevFerrySite site)
        {
            //if (site.TrackCode !=0 && site.TrackPos != 0)
            if (site.TrackCode != 0) // 坐标值允许设0
            {
                PubMaster.Track.UpdateFerryPos(devid, site.TrackCode, site.TrackPos);
                //PubTask.Rf.SendFerryPos(devid, set.IP);

                if (!PubTask.Rf.SendSucc2Rf(set.MEID, FunTag.UpdateFerryPos, "ok")
                    && mTimer.IsOver(TimerTag.RfFerrySiteUpdateSendOffline, devid, 60, 10))
                {
                    StopRfPosSet(set.MEID);
                }
            }
            PubTask.Rf.SendFerrySitePos(set.MEID, devid, site);
        }

        #endregion

        #region[摆渡对位]

        public void StartFerryPosSetting(uint id, ushort code)
        {
            StopFerryPosSetting();
            if (!_FerryPosSetList.Exists(c => c.FerryId == id && !c.IsRF))
            {
                FerryPosSet set = new FerryPosSet
                {
                    FerryId = id,
                    IsRF = false
                };
                //set.QueryPos = code;
                _FerryPosSetList.Add(set);
            }
            _IsSetting = true;
        }

        public void StopFerryPosSetting()
        {
            _FerryPosSetList.RemoveAll(c => !c.IsRF);
            _IsSetting = _FerryPosSetList.Count != 0;
        }

        public void StartRfPosSet(string meid, uint ferryid)
        {
            FerryPosSet set = _FerryPosSetList.Find(c => meid.Equals(c.MEID));
            if (set == null)
            {
                set = new FerryPosSet
                {
                    FerryId = ferryid,
                    IsRF = true,
                    MEID = meid
                };
                _FerryPosSetList.Add(set);
            }
            else
            {
                set.FerryId = ferryid;
            }
            _IsSetting = true;
        }

        public void StopRfPosSet(string meid)
        {
            _FerryPosSetList.RemoveAll(c => meid.Equals(c.MEID));
            _IsSetting = _FerryPosSetList.Count != 0;
        }

        public void AutoPosMsgSend(uint ferryid, DevFerryAutoPosE posside, ushort starttrack, byte tracknumber)
        {
            FerryTask task = DevList.Find(c => c.ID == ferryid);
            task.DoAutoPos(posside, starttrack, tracknumber);
        }

        public void QueryPosList(uint ferryid, ushort trackcode)
        {
            FerryTask task = DevList.Find(c => c.ID == ferryid);
            task.DoSiteQuery(trackcode);
        }

        public void RefreshPosList(uint ferryid)
        {
            if (_IsRefreshPos) return;
            if (Monitor.TryEnter(_posobj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    EndRefreshPosList();
                    PosList.AddRange(PubMaster.Mod.TraSql.QueryFerryPosList(ferryid));
                    _IsRefreshPos = true;
                }
                finally
                {
                    Monitor.Exit(_posobj);
                }
            }
        }

        public void EndRefreshPosList()
        {
            PosList.Clear();
            _IsRefreshPos = false;
        }

        #endregion

        #region[分配-摆渡车]

        /// <summary>
        /// 根据交易信息分配摆渡车
        /// 1.取货轨道是否有车
        /// 2.卸货轨道是否有车
        /// 3.摆渡车上是否有车
        /// 4.根据上下砖机轨道优先级逐轨道是否有车
        /// 5.对面储砖区域(上下砖机轨道对应的兄弟轨道是否有车)
        /// 6.对面区域摆渡车是否有车
        /// 7.对面砖机轨道是否有车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferryid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool AllocateFerry(StockTrans trans, DeviceTypeE ferrytype, uint priortrackid, out uint ferryid, out string result)
        {
            result = "";
            ferryid = 0;
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    //3.1获取能到达[取货/卸货]轨道的摆渡车的ID
                    List<uint> ferryids;
                    Track carrierTrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    bool isCarInFerry = false;
                    if (carrierTrack.Type == enums.track.TrackTypeE.摆渡车_入 || carrierTrack.Type == enums.track.TrackTypeE.摆渡车_出)
                    {
                        isCarInFerry = true;
                    }

                    if (trans.TransType == TransTypeE.倒库任务)
                    {
                        ferryids = PubMaster.Area.GetFerryWithTrackInOut(ferrytype, trans.area_id, 0, trans.give_track_id, isCarInFerry ? 0 : carrierTrack.id, false);
                    }
                    else
                    {
                        ferryids = PubMaster.Area.GetFerryWithTrackInOut(ferrytype, trans.area_id, trans.take_track_id, trans.give_track_id, isCarInFerry ? 0 : carrierTrack.id, false);
                    }

                    //3.2摆渡车上是否有车[空闲，无货]
                    List<FerryTask> ferrys = DevList.FindAll(c => ferryids.Contains(c.ID) && c.IsWorking);

                    if (isCarInFerry)
                    {
                        ferryid = DevList.Find(c => c.DevConfig.track_id == carrierTrack.id && c.IsWorking)?.ID ?? 0;
                        return true;
                    }

                    short carrierTrackOrder = carrierTrack.order;

                    if (ferrys.Count > 0)
                    {
                        int safedis = PubMaster.Dic.GetDtlIntCode(DicTag.FerryAvoidNumber);
                        //判断是否存在有摆渡车已被锁
                        if (ferrys.Exists(c => c.IsFerryLock()) && ferrys.Exists(c => !c.IsFerryLock()))
                        {
                            List<FerryTask> ferryLockeds = ferrys.FindAll(c => c.IsFerryLock());
                            List<FerryTask> ferryUnLockeds = ferrys.FindAll(c => !c.IsFerryLock());

                            foreach (FerryTask fUnLocked in ferryUnLockeds)
                            {
                                if (CheckFerryStatus(fUnLocked) && (fUnLocked.IsStillLockInTrans(trans.id) || fUnLocked.IsFerryFree()))
                                {
                                    //摆渡车所对着的轨道id
                                    //uint taskUnLockedTrackId = ferrytype == DeviceTypeE.上摆渡 ? fUnLocked.DownTrackId : fUnLocked.UpTrackId;
                                    uint taskUnLockedTrackId = fUnLocked.GetFerryCurrentTrackId();

                                    //摆渡车的当前轨道的顺序
                                    short taskUnLockedCurrentOrder = PubMaster.Track.GetTrackOrder(taskUnLockedTrackId);

                                    int leftCompare, rightCompare;
                                    if (taskUnLockedCurrentOrder >= carrierTrackOrder)
                                    {
                                        leftCompare = carrierTrackOrder - safedis;
                                        rightCompare = taskUnLockedCurrentOrder + safedis;
                                    }
                                    else
                                    {
                                        leftCompare = taskUnLockedCurrentOrder - safedis;
                                        rightCompare = carrierTrackOrder + safedis;
                                    }
                                    leftCompare = leftCompare < 0 ? 0 : leftCompare;
                                    bool isChosen = true;
                                    foreach (FerryTask fLocked in ferryLockeds)
                                    {
                                        if (!CheckFerryStatus(fLocked, out string r))
                                        {
                                            continue;
                                        }

                                        if (CheckFerryStatus(fLocked) && fLocked.IsStillLockInTrans(trans.id))
                                        {
                                            ferryid = fLocked.ID;
                                            return true;
                                        }
                                        //上锁摆渡车所对着的轨道id
                                        //uint taskLockedTrackId = ferrytype == DeviceTypeE.上摆渡 ? fLocked.DownTrackId : fLocked.UpTrackId;
                                        uint taskLockedTrackId = fLocked.GetFerryCurrentTrackId();

                                        //上锁摆渡车的当前轨道的顺序
                                        short taskLockedCurrentOrder = PubMaster.Track.GetTrackOrder(taskLockedTrackId);

                                        //上锁摆渡车的目的轨道的位置顺序
                                        short taskLockedTargetOrder = PubMaster.Track.GetTrackBySite((ushort)fLocked.AreaId, fLocked.Type, fLocked.DevStatus.TargetSite)?.order ?? 0;

                                        if ((leftCompare < taskLockedCurrentOrder && taskLockedCurrentOrder < rightCompare)
                                               || (leftCompare < taskLockedTargetOrder && taskLockedTargetOrder < rightCompare))
                                        {
                                            isChosen = false;
                                            break;
                                        }
                                    }
                                    if (isChosen)
                                    {
                                        ferryid = fUnLocked.ID;
                                        return true;
                                    }
                                }
                                continue; //当前空闲摆渡车分配不了，就分配下一辆摆渡车
                            }
                        }

                        long distance = 999;
                        //如何判断哪个摆渡车最好储砖
                        foreach (FerryTask ferry in ferrys)
                        {
                            if (CheckFerryStatus(ferry) && (ferry.IsStillLockInTrans(trans.id) || ferry.IsFerryFree()))
                            {
                                // 摆渡车对应轨道号
                                Track ferryTrack = PubMaster.Track.GetTrack(ferry.GetFerryCurrentTrackId());

                                if (ferryTrack == null || ferryTrack.order == 0)
                                {
                                    continue;

                                }
                                //摆渡车跟运输车轨道的差绝对值,   数据库 在录入的轨道order时，砖机轨道的顺序是对着的那条储砖轨道的顺序
                                long d = Math.Abs(ferryTrack.order - carrierTrack.order);
                                if (distance > d)
                                {
                                    distance = d;
                                    ferryid = ferry.ID;
                                }
                            }
                            
                            result = result + ferry.Device.name + ",";
                        }
                        result = result + "摆渡车不符合状态，不能分配，分配条件：【启用】【通讯正常】【没载车】【停止】【自动】【没有被分配到其他任务】";
                        return ferryid != 0;
                    }
                    result = result.Equals("") ? "没有能够去取/卸货轨道的摆渡车" : result;
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        /// <summary>
        /// 检查摆渡车状态
        /// </summary>
        /// <param name="ferry"></param>
        /// <returns></returns>
        private bool CheckFerryStatus(FerryTask ferry)
        {
            if (ferry.ConnStatus == SocketConnectStatusE.通信正常
                    && ferry.OperateMode == DevOperateModeE.自动
                    && ferry.Status == DevFerryStatusE.停止
                    && ferry.Load == DevFerryLoadE.空)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region[条件判断]

        private bool CheckFerryStatus(FerryTask task, out string result)
        {
            if (task == null)
            {
                result = " 找不到对应摆渡车信息";
                return false;
            }

            if (task.ConnStatus != SocketConnectStatusE.通信正常)
            {
                result = " 摆渡车设备未连接";
                return false;
            }

            if (task.OperateMode == DevOperateModeE.手动)
            {
                result = " 摆渡车手动操作中";
                return false;
            }

            result = "";
            return true;
        }

        internal bool IsLoadOrEmpty(FerryTask task, out string result)
        {
            if (task.Load == DevFerryLoadE.异常 || task.Load == DevFerryLoadE.非空)
            {
                result = " 摆渡车非空非载车";
                return false;
            }
            result = "";
            return true;
        }

        internal bool IsLoad(uint ferryid)
        {
            return DevList.Exists(c => c.ID == ferryid
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.Load == DevFerryLoadE.载车);
        }

        internal bool IsUnLoad(uint ferryid)
        {
            return DevList.Exists(c => c.ID == ferryid
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.Load == DevFerryLoadE.空);
        }

        internal bool IsStop(uint ferryid)
        {
            return DevList.Exists(c => c.ID == ferryid
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.Status == DevFerryStatusE.停止);
        }

        public bool IsStopAndSiteOnTrack(uint id, bool isferryupsite, out uint intrackid, out string result)
        {
            intrackid = 0;
            FerryTask task = DevList.Find(c => c.DevConfig.track_id == id);
            if (task == null)
            {
                result = "找不到摆渡车设备";
                return false;
            }

            if (!CheckFerryStatus(task, out result))
            {
                return false;
            }

            if (task.Status != DevFerryStatusE.停止)
            {
                result = "摆渡车非停止状态！";
                return false;
            }

            if (isferryupsite && !task.IsUpLight)
            {
                result = "摆渡车前侧光电未亮！";
                return false;
            }

            if (!isferryupsite && !task.IsDownLight)
            {
                result = "摆渡车后侧光电未亮！";
                return false;
            }

            intrackid = isferryupsite ? task.UpTrackId : task.DownTrackId;
            result = "";
            return true;
        }

        /// <summary>
        /// 是否有到位摆渡车可用
        /// </summary>
        /// <param name="carriertask">小车执行指令</param>
        /// <param name="dt">摆渡类型</param>
        /// <param name="trackid">判断摆渡车是否对上轨道</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        public bool HaveFerryInPlace(DevCarrierTaskE carriertask, DeviceTypeE dt, uint trackid, out uint ferryTrackid, out string result)
        {
            ferryTrackid = 0;
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                result = "稍后再试！";
                return false;
            }
            try
            {
                //后退至摆渡车 则 判断判断摆渡车的上砖测光电
                bool checkuplight = carriertask == DevCarrierTaskE.后退至摆渡车;
                FerryTask task = DevList.Find(c => c.Type == dt && c.GetFerryCurrentTrackId(checkuplight) == trackid);

                if (!CheckFerryStatus(task, out result))
                {
                    return false;
                }

                if (task.Status == DevFerryStatusE.停止)
                {
                    ferryTrackid = task.FerryTrackId;
                    return true;
                }
            }
            finally
            {
                Monitor.Exit(_obj);
            }
            result = "没有符合条件的摆渡车!";
            return false;
        }

        /// <summary>
        /// 目的摆渡车是否到位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="result"></param>
        /// <param name="onferryboolvalue"></param>
        /// <returns></returns>
        public bool IsTargetFerryInPlace(ushort area, ushort from, ushort to, out string result, bool onferryboolvalue)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                result = "稍后再试！";
                return false;
            }
            try
            {
                Track ft = PubMaster.Track.GetTrackBySite(area, from);
                Track tt = PubMaster.Track.GetTrackBySite(area, to);
                if (ft != null)
                {
                    if (ft.Type == TrackTypeE.摆渡车_入 || ft.Type == TrackTypeE.摆渡车_出)
                    {
                        result = "小车已经在摆渡车上了！";
                        return onferryboolvalue;
                    }

                    FerryTask task = DevList.Find(c => c.FerryTrackId == tt.id);
                    if (!task.IsOnSite(ft.ferry_up_code) && !task.IsOnSite(ft.ferry_down_code))
                    {
                        result = "摆渡车没到位！";
                        return false;
                    }

                    if (!CheckFerryStatus(task, out result))
                    {
                        return false;
                    }

                    if (task.Status == DevFerryStatusE.停止)
                    {
                        return true;
                    }
                }
            }
            finally
            {
                Monitor.Exit(_obj);
            }
            result = "没有符合条件的摆渡车!";
            return false;
        }

        /// <summary>
        /// 摆渡车是否可移动
        /// </summary>
        public bool IsAllowToMove(FerryTask task, out string result)
        {
            // 检查摆渡车状态
            if (!CheckFerryStatus(task, out result))
            {
                return false;
            }
            if (!IsLoadOrEmpty(task, out result))
            {
                return false;
            }

            // 检查是否有对应运输车作业
            if (PubTask.Carrier.HaveTaskForFerry(task.DevConfig.track_id))
            {
                result = " 存在运输车上下摆渡中";
                return false;
            }

            return true;
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
                FerryTask task = DevList.Find(c => c.ID == devId);
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

    public class FerryPosSet
    {
        public bool IsRF { set; get; }
        public string MEID { set; get; }
        public uint FerryId { set; get; }
        public ushort QueryPos { set; get; } = 0;
    }
}
