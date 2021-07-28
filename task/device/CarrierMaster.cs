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
using tool.appconfig;
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
        private Log mlog, mErrorLog;
        private bool isWcsStoping = false;
        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public CarrierMaster()
        {
            mlog = (Log)new LogFactory().GetLog("小车日志", false);
            mErrorLog = (Log)new LogFactory().GetLog("放砖地标警告", false);

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
                try
                {
                    foreach (CarrierTask task in DevList)
                    {
                        try
                        {
                            if (task.IsEnable && task.IsConnect)
                            {
                                #region [上下摆渡时终止摆渡车]
                                Track track = PubMaster.Track.GetTrack(task.CurrentTrackId);

                                // 运输车有执行的指令，并且在摆渡上
                                if (track != null && !task.IsNotDoingTask)
                                {
                                    if (track.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                                    {
                                        PubTask.Ferry.StopFerryByFerryTrackId(track.id, string.Format("运输车[ {0} ], 在摆渡车上[ {1} ], 执行[ {2} ]中", task.Device.name, track.name, task.OnGoingOrder), "锁定");
                                    }
                                }

                                // 目的前往摆渡车
                                Track targettrack = PubMaster.Track.GetTrack(task.TargetTrackId);
                                if (targettrack != null && targettrack.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出) )
                                {
                                    // 判断是否有摆渡车
                                    if (!PubTask.Ferry.IsTargetFerryInPlace((ushort)task.AreaId, task.CurrentSite, task.TargetSite, out string result, true))
                                    {
                                        task.DoStop(0, string.Format("【自动终止小车】, 触发[ {0} ], 位置[ {1} ], 其他[ {2} ]", "摆渡车状态不对", track?.Type, result));
                                        Thread.Sleep(500);
                                    }

                                    if (task.OnGoingOrder == DevCarrierOrderE.定位指令)
                                    {
                                        PubTask.Ferry.StopFerryByFerryTrackId(targettrack.id, string.Format("运输车[ {0} ], 定位[ {1} -> {2} ]", task.Device.name, track?.name, targettrack.name), "锁定");
                                    }
                                }

                                // 运输车作业中，上下摆渡状态，对上轨道的摆渡车全停
                                //if (track != null 
                                //    && task.Position == DevCarrierPositionE.上下摆渡中 
                                //    && task.NotInTask(DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
                                //{
                                //    PubTask.Ferry.StopFerryByTrackId(track.id, string.Format("运输车[ {0} ], 轨道[ {1} ], 上下摆渡中", task.Device.name, track.name));
                                //}

                                // 是否存在被运输车交管的摆渡车
                                if (PubTask.TrafficControl.ExistsTrafficControl(TrafficControlTypeE.运输车交管摆渡车, task.ID, out uint ferryid))
                                {
                                    PubTask.Ferry.StopFerry(0, ferryid, "运输车交管", "逻辑", out string result);
                                }

                                #endregion

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
                Thread.Sleep(1000);
            }
        }


        /// <summary>
        /// 停止模拟的设备连接
        /// </summary>
        internal void StockSimDevice()
        {
            List<CarrierTask> tasks = DevList.FindAll(c => c.IsConnect && c.Device.ip.Equals("127.0.0.1"));
            foreach (CarrierTask task in tasks)
            {
                if (task.IsEnable)
                {
                    task.Device.enable = false;
                }
                task.Stop("模拟停止");
            }
        }


        /// <summary>
        /// 清空设备信息
        /// </summary>
        /// <param name="iD"></param>
        public string ClearTaskStatus(uint carrierid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == carrierid);
                    if (task != null && !task.IsWorking && !task.IsEnable)
                    {
                        PubTask.Ferry.UpdateFerryWithTrackId(task.CurrentTrackId, task.Device.name, DevFerryLoadE.空);
                        task.ClearDevStatus();
                        MsgSend(task, task.DevStatus);
                        return "清除成功";
                    }
                    else
                    {
                        return "请确认小车已断开通讯且停用";
                    }
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
            return "稍后再试";
        }

        /// <summary>
        /// 连接/断开通讯
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="isstart"></param>
        public void StartStopCarrier(uint carrierid, bool isstart)
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
            catch (Exception ex)
            {
                mlog.Error(true, ex.StackTrace);
            }
        }

        #endregion

        #region[获取信息]

        public void GetAllCarrier()
        {
            foreach (CarrierTask task in DevList)
            {
                MsgSend(task, task.DevStatus);
            }
        }

        /// <summary>
        /// 查找是否存在运输车 当前/目的 在指定的轨道且载货
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrackAndLoad(uint trackid)
        {
            return DevList.Exists(c => c.InTrack(trackid) && c.IsLoad());
        }

        /// <summary>
        /// 存在其他运输车在当前任务双轨道中
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="trackid2"></param>
        /// <param name="cid"></param>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        internal bool HaveInTrackButCarrier(uint trackid, uint trackid2, uint cid, out uint carrierid)
        {
            CarrierTask task = DevList.Find(c => (c.CurrentTrackId == trackid || c.CurrentTrackId == trackid2) && c.ID != cid);
            if (task != null)
            {
                carrierid = task.ID;
                return true;
            }
            carrierid = 0;
            return false;
        }


        internal bool HaveDifTypeInTrack(uint trackid, CarrierTypeE carriertype, out uint carrierid)
        {
            CarrierTask carrier = DevList.Find(c => c.CurrentTrackId == trackid);
            if (carrier != null && carrier.CarrierType != carriertype)
            {
                carrierid = carrier.ID;
                return true;
            }
            carrierid = 0;
            return false;
        }

        /// <summary>
        /// 是否有负责不同规格的车在轨道内
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodssizeID"></param>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        internal bool HaveDifGoodsSizeInTrack(uint trackid, uint goodssizeID, out uint carrierid)
        {
            CarrierTask carrier = DevList.Find(c => c.CurrentTrackId == trackid);
            if (carrier != null && !carrier.DevConfig.IsUseGoodsSize(goodssizeID))
            {
                carrierid = carrier.ID;
                return true;
            }
            carrierid = 0;
            return false;
        }


        /// <summary>
        /// 查找是否存在运输车 当前/目的 在指定的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid)
        {
            return DevList.Exists(c => c.InTrack(trackid));
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
            CarrierTask carrier = DevList.Find(c => c.CurrentTrackId == trackid);
            carrierid = carrier?.ID ?? 0;
            return carrier != null;
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid, uint carrierid)
        {
            return DevList.Exists(c => c.ID != carrierid && c.CurrentTrackId == trackid);
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid, uint carrierid, out uint othercarrierid)
        {
            CarrierTask carrier = DevList.Find(c => c.ID != carrierid && c.CurrentTrackId == trackid);
            othercarrierid = carrier?.ID ?? 0;
            return carrier != null;
        }

        /// <summary>
        /// 小车完成任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsStopFTask(uint carrier_id,  Track track = null)
        {
            if(track != null)
            {
                if(track.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                {
                    return DevList.Exists(c => c.ID == carrier_id
                           && c.ConnStatus == SocketConnectStatusE.通信正常
                           && c.OperateMode == DevOperateModeE.自动
                           && c.Status == DevCarrierStatusE.停止
                           && c.IsNotDoingTask
                           && c.Position == DevCarrierPositionE.在摆渡上
                           && c.CurrentSite == track.rfid_1);
                }
            }
            return DevList.Exists(c => c.ID == carrier_id
                           && c.ConnStatus == SocketConnectStatusE.通信正常
                           && c.OperateMode == DevOperateModeE.自动
                           && c.Status == DevCarrierStatusE.停止
                           && c.IsNotDoingTask
                           //&& (c.CurrentOrder == c.FinishOrder || c.CurrentOrder == DevCarrierOrderE.无)
                           //&& (c.Position != DevCarrierPositionE.上下摆渡中 && c.Position != DevCarrierPositionE.异常) //小车冲过头？
                           );
        }

        internal Track GetCarrierTrack(uint carrier_id)
        {
            uint trackid = DevList.Find(c => c.ID == carrier_id)?.CurrentTrackId ?? 0;
            return trackid > 0 ? PubMaster.Track.GetTrack(trackid) : null;
        }

        /// <summary>
        /// 获取运输车当前所在轨道ID
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal uint GetCarrierTrackID(uint carrier_id)
        {
            return DevList.Find(c => c.ID == carrier_id)?.CurrentTrackId ?? 0;
        }

        internal bool IsLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && c.IsLoad());
        }

        internal bool IsLoadInFerry(uint ltrack)
        {
            return DevList.Exists(c => c.CurrentTrackId == ltrack
                         && c.ConnStatus == SocketConnectStatusE.通信正常
                         && c.IsLoad()
                         && c.Position == DevCarrierPositionE.在摆渡上);
        }

        internal bool IsNotLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && c.IsNotLoad());
        }
        
        /// <summary>
        /// 获取小车当前位置状态
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal DevCarrierPositionE GetPosition(uint carrier_id)
        {
            return DevList.Find(c => c.ID == carrier_id)?.Position?? DevCarrierPositionE.异常;
        }

        /// <summary>
        /// 获取当前在下砖侧工作的运输车数量
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="isUp"></param>
        /// <param name="trackTypes"></param>
        /// <returns></returns>
        public uint GetCurrentCarCount(uint area_id, bool isUp, params TrackTypeE[] trackTypes)
        {
            List<uint> trackids = PubMaster.Track.GetAreaTrackIdList(area_id, trackTypes);
            uint count = 0;
            if (isUp)
            {
                // 倒库的运输车算上砖的
                count = (uint)DevList.Count(c => trackids.Contains(c.CurrentTrackId) || PubTask.Trans.IsCarrierInTrans(c.ID, TransTypeE.倒库任务));
            }
            else
            {
                // 在下砖侧倒库的算上砖侧的
                count = (uint)DevList.Count(c => trackids.Contains(c.CurrentTrackId) && !PubTask.Trans.IsCarrierInTrans(c.ID, TransTypeE.倒库任务));
            }
            return count;
        }
        #endregion

        #region[数据更新]

        private void CarrierMsgUpdate(SocketMsgMod mod)
        {
            if (isWcsStoping) return;
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromMilliseconds(500)))
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
                    if (task.IsEnable) PubMaster.Warn.AddDevWarn(task.AreaId, task.Line, WarningTypeE.DeviceOffline, (ushort)task.ID);
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

            Track track = PubMaster.Track.GetTrack(task.CurrentTrackId);

            #region[手动操作]
            if (task.OperateMode == DevOperateModeE.手动)
            {
                if (task.CurrentOrder != DevCarrierOrderE.终止指令 && task.CurrentOrder != DevCarrierOrderE.无)
                {
                    task.DoStop(0, string.Format("【自动终止小车】, 触发[ {0} ], 位置[ {1} ], 指令[ {2} ]", "手动操作小车", track?.Type, task.CurrentOrder));
                }
            }
            #endregion

            #region[更新摆渡车载货状态]
            if (track != null)
            {
                switch (track.Type)
                {
                    case TrackTypeE.摆渡车_入:
                    case TrackTypeE.摆渡车_出:
                        PubTask.Ferry.UpdateFerryWithTrackId(task.CurrentTrackId, task.Device.name, DevFerryLoadE.载车);
                        task.LastTrackId = task.CurrentTrackId;
                        break;
                    default:
                        if (task.LastTrackId != 0)
                        {
                            PubTask.Ferry.UpdateFerryWithTrackId(task.LastTrackId, task.Device.name, DevFerryLoadE.空);
                            task.LastTrackId = 0;
                        }
                        break;
                }
            }
            #endregion

            //if (task.Status != DevCarrierStatusE.停止) return;

            #region[检查任务]

            #region [取卸货]

            //放货动作
            if (task.DevConfig.stock_id != 0 && task.IsNotLoad() && track != null)
            {
                if(task.DevStatus.GivePoint != 0)
                {
                    PubMaster.Goods.UpdateStockLocation(task.DevConfig.stock_id, task.DevStatus.GivePoint);
                }
                else
                {
                    PubMaster.Goods.UpdateStockLocation(task.DevConfig.stock_id, task.DevStatus.CurrentPoint);
                }

                //判断放下砖的时候轨道是否是能否放砖的轨道
                if (track.NotInType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                {
                    //将库存转移到轨道的位置
                    PubMaster.Goods.MoveStock(task.DevConfig.stock_id, track.id, false, "", task.ID);

                    if (task.IsUnloadInFerry)
                    {
                        task.IsUnloadInFerry = false;
                        mErrorLog.Error(true, string.Format("【放砖】轨道[ {0} ], 需要调整极限地标,否则影响倒库; 小车[ {1} ]",
                            track.GetLog(), task.Device.name));
                    }

                    try
                    {
                        PubMaster.Goods.AddStockLog(string.Format("【解绑】设备[ {0} ], 轨道[ {1} ], 库存[ {2} ], 运输车[ {3} ]",
                            task.Device.name,
                            track?.name ?? task.GivePoint + "",
                            PubMaster.Goods.GetStockInfo(task.DevConfig.stock_id),
                            task.DevStatus.GetGiveString()));
                    }
                    catch { }


                    task.DevConfig.stock_id = 0;

                    PubMaster.Mod.DevConfigSql.EditConfigCarrier(task.DevConfig);

                }
                else
                {
                    if (!task.IsUnloadInFerry)
                    {
                        task.IsUnloadInFerry = true;

                        mErrorLog.Error(true, string.Format("【放砖】小车[ {0} ], 尝试在轨道[ {1} ]上卸货; 状态[ {2} ]",
                            task.Device.name, track.GetLog(), task.DevStatus.GetGiveString()));
                    }
                }
            }

            //取货动作
            if (task.DevConfig.stock_id == 0 && task.IsLoad())
            {
                //1.根据轨道当前地标查看是否有库存在轨道的地标上
                //2.找不到则拿轨道上的库存(先不考虑方向)
                //3.都没有，则报警
                if (track != null)
                {
                    switch (track.Type)
                    {
                        case TrackTypeE.上砖轨道:

                            uint tileid = PubMaster.DevConfig.GetTileInPoint(track.id, task.CurrentSite);
                            uint gid = PubTask.TileLifter.GetTileTrackGid(tileid, track.id);
                            task.DevConfig.stock_id = PubMaster.Goods.GetStockInTileTrack(track.id, tileid, gid, true);
                            break;

                        case TrackTypeE.下砖轨道:

                            tileid = PubMaster.DevConfig.GetTileInPoint(track.id, task.CurrentSite);
                            gid = PubTask.TileLifter.GetTileTrackGid(tileid, track.id);
                            task.DevConfig.stock_id = PubMaster.Goods.GetStockInTileTrack(track.id, tileid, gid) ;
                            if(task.DevConfig.stock_id == 0)
                            {
                                if(PubTask.TileLifter.AddTileStockInTrack(track.id, task.CurrentSite, out uint stockid))
                                {
                                    task.DevConfig.stock_id = stockid;
                                    try
                                    {
                                        mlog.Status(true, string.Format("运输车在下砖轨道[ {0} ]，找不到轨道库存，则新增库存[ {1} ]", track.name, stockid));
                                    }
                                    catch { }
                                }
                            }
                            break;
                        case TrackTypeE.储砖_入:
                        case TrackTypeE.储砖_出:
                        case TrackTypeE.储砖_出入:
                            task.DevConfig.stock_id = PubMaster.Goods.GetStockInStoreTrack(track, task.DevStatus.TakePoint);
                            break;
                        case TrackTypeE.摆渡车_入:
                        case TrackTypeE.摆渡车_出:
                            task.DevConfig.stock_id = PubMaster.Goods.GetStockInFerryTrack(track.id);
                            break;
                    }
                    if (task.DevConfig.stock_id != 0)
                    {
                        PubMaster.Mod.DevConfigSql.EditConfigCarrier(task.DevConfig);
                        try
                        {
                            PubMaster.Goods.AddStockLog(string.Format("【绑定】设备[ {0} ], 轨道[ {1} ], 库存[ {2} ], 运输车[ {3} ]",
                                    task.Device.name,
                                    track?.name ?? task.TakePoint + "",
                                    PubMaster.Goods.GetStockSmallInfo(task.DevConfig.stock_id),
                                    task.DevStatus.GetTakeString()));
                        }
                        catch { }
                    }
                }

                if (task.DevConfig.stock_id == 0)
                {
                    //报警
                    mErrorLog.Error(true, string.Format("【放砖】小车[ {0} ]尝试在轨道[ {1} ]上放砖; 状态[ {2} ]",
                        task.Device.name, track?.GetLog() ?? task.DevStatus.CurrentSite+"", task.DevStatus.GetGiveString()));
                }
            }

            #endregion

            #region[运输车切换轨道]


            if (task.DevConfig.stock_id != 0)
            {
                //根据小车当前的位置更新库存对应所在的轨道
                PubMaster.Goods.MoveStock(task.DevConfig.stock_id, task.CurrentTrackId, false, task.CurrentOrder + "", task.ID);

                if(!task.IsNotDoingTask 
                    && task.IsLoad()
                    && (task.CurrentOrder == DevCarrierOrderE.往前倒库 || task.CurrentOrder == DevCarrierOrderE.往后倒库))
                {
                    PubMaster.Goods.UpdateStockLocation(task.DevConfig.stock_id, task.DevStatus.CurrentPoint);
                }
            }

            #endregion

            #region[逻辑警告]

            task.CheckLogicAlert();

            #endregion

            #endregion

        }

        /// <summary>
        /// 是否有运输车在上下摆渡相关任务
        /// </summary>
        /// <param name="ferrytraid"></param>
        /// <returns></returns>
        internal bool HaveTaskForFerry(uint ferrytraid)
        {
            Track track = PubMaster.Track.GetTrack(ferrytraid);
            if (track != null && track.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
            {
                return DevList.Exists(c => c.InTrack(ferrytraid)
                                       && (c.Status != DevCarrierStatusE.停止 
                                               || c.Position != DevCarrierPositionE.在摆渡上
                                               || !c.IsNotDoingTask
                                               || c.CurrentSite != track.rfid_1 //不在摆渡车的点上
                                               // || c.InTask(DevCarrierOrderE.定位指令, DevCarrierOrderE.取砖指令,
                                               //           DevCarrierOrderE.放砖指令, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库)
                                               )
                                       );
            }
            return DevList.Exists(c => c.InTrack(ferrytraid)
                                    //&& c.ConnStatus == SocketConnectStatusE.通信正常
                                    //&& (c.OperateMode == DevOperateModeE.自动 || c.OperateMode == DevOperateModeE.手动)
                                    //&& c.Status != DevCarrierStatusE.异常
                                    //&& c.CurrentOrder != c.FinishOrder
                                    && (c.Status != DevCarrierStatusE.停止 || c.Position != DevCarrierPositionE.在摆渡上 
                                            || c.InTask(DevCarrierOrderE.定位指令, DevCarrierOrderE.取砖指令, 
                                                        DevCarrierOrderE.放砖指令, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
                                    );
        }

        internal bool IsCarrierInTrack(StockTrans trans)
        {
            //当前任务的运输车是否是否站点在摆渡车上，但所在位置是在轨道上
            bool isWrongStatus = DevList.Exists(c => c.ID == trans.carrier_id
                                    && c.Position == DevCarrierPositionE.在轨道上
                                    && PubMaster.Track.IsFerryTrackType(c.CurrentTrackId));
            if (!trans.IsReleaseGiveFerry && isWrongStatus)
            {
                CarrierTask carrier = DevList.Find(c => c.ID == trans.carrier_id);
                if (carrier.CurrentOrder == DevCarrierOrderE.放砖指令)
                {
                    mErrorLog.Error(true, string.Format("【读点】小车[ {0} ]没有读到[ {1} ]轨道地标",
                        carrier.Device.name,
                        PubMaster.Track.GetTrackName(trans.give_track_id)));
                }
                else if (carrier.CurrentOrder == DevCarrierOrderE.取砖指令)
                {
                    mErrorLog.Error(true, string.Format("【读点】小车[ {0} ]没有读到[ {1} ]轨道地标",
                        carrier.Device.name,
                        PubMaster.Track.GetTrackName(trans.finish_track_id)));
                }
            }
            return isWrongStatus;
        }

        /// <summary>
        /// 判断砖机是否停在在砖机轨道对应的地标上<br/>
        /// 1.用于上砖停在在砖机位置时，可以释放摆渡车
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="tilelifterid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool IsCarrierStockInTileSite(uint carrierid, uint tilelifterid, uint trackid)
        {
            ushort site = PubMaster.DevConfig.GetTileSite(tilelifterid, trackid);
            return DevList.Exists(c => c.ID == carrierid && c.Status == DevCarrierStatusE.停止 && c.CurrentSite == site);
        }

        #endregion

        #region[发送信息]
        private void MsgSend(CarrierTask task, DevCarrier carrier)
        {
            if (Monitor.TryEnter(_objmsg, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    mMsg.ID = task.ID;
                    mMsg.Name = task.Device.name;
                    mMsg.o1 = carrier;
                    mMsg.o2 = task.ConnStatus;
                    mMsg.o3 = task.IsWorking;
                    mMsg.o4 = task.CurrentTrackId;
                    mMsg.o5 = task.TargetTrackId;
                    mMsg.o6 = task.CurrentTrackLine;
                    Messenger.Default.Send(mMsg, MsgToken.CarrierStatusUpdate);
                }
                finally
                {
                    Monitor.Exit(_objmsg);
                }
            }
        }
        #endregion

        #region[执行指令]

        /// <summary>
        /// 手动指令
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="carriertask"></param>
        /// <param name="result"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public bool DoManualNewTask(uint devid, DevCarrierTaskE carriertask, out string result, string memo = "", ushort srfid = 0)
        {
            try
            {
                bool isferryupsite = false;
                Track track = GetCarrierTrack(devid);
                if (track == null)
                {
                    result = "未能获取到小车位置相关信息！";
                    return false;
                }
                //小车当前所在RF点
                ushort site = GetCurrentSite(devid);

                DevCarrierOrderE order = DevCarrierOrderE.终止指令;
                ushort checkTra = 0;//校验轨道号
                ushort toRFID = 0;//目标点
                ushort toPoint = 0;//目标脉冲
                ushort overRFID = 0;//结束点
                ushort overPoint = 0;//结束脉冲
                byte moveCount = 0;//倒库数量
                uint ferryTraid = 0;//摆渡轨道ID
                uint toTrackid = 0;//目标轨道Id
                switch (carriertask)
                {
                    case DevCarrierTaskE.后退取砖:
                        #region 后退取砖
                        if (IsLoad(devid))
                        {
                            result = "运输车有货不能后退取砖！";
                            return false;
                        }
                        if (track.NotInType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                        {
                            result = "须在摆渡车上执行！";
                            return false;
                        }
                        order = DevCarrierOrderE.取砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.前进放砖:
                        #region 前进放砖
                        if (IsNotLoad(devid))
                        {
                            result = "运输车没有货不能前进放货！";
                            return false;
                        }
                        if (track.NotInType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                        {
                            result = "须在摆渡车上执行！";
                            return false;
                        }
                        order = DevCarrierOrderE.放砖指令;
                        isferryupsite = true;

                        #endregion
                        break;

                    case DevCarrierTaskE.后退至摆渡车:
                        #region 后退至摆渡车
                        //&& site != track.rfid_1//最小定位RFID
                        //if (track.NotInType(TrackTypeE.上砖轨道, TrackTypeE.储砖_入, TrackTypeE.储砖_出入)) 
                        //{
                        //    result = "小车需要在入库轨道头或者上砖轨道";
                        //    return false;
                        //}
                        if (track.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                        {
                            result = "小车在摆渡车时不能发上摆渡的指令";
                            return false;
                        }
                        if (!PubTask.Ferry.HaveFerryInPlace(carriertask, track.Type == TrackTypeE.上砖轨道 ? DeviceTypeE.上摆渡 : DeviceTypeE.下摆渡,
                            track.id, out ferryTraid, out uint ferryid, out result))
                        {
                            return false;
                        }

                        if (PubTask.Ferry.IsLoad(ferryid))
                        {
                            result = "摆渡车上有运输车";
                            return false;
                        }

                        checkTra = PubMaster.Track.GetTrackDownCode(ferryTraid);
                        toRFID = PubMaster.Track.GetTrackRFID1(ferryTraid);
                        order = DevCarrierOrderE.定位指令;
                        toTrackid = ferryTraid;

                        #endregion
                        break;

                    case DevCarrierTaskE.前进至摆渡车:
                        #region 前进至摆渡车
                        //if (track.NotInType(TrackTypeE.下砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入))
                        //{
                        //    result = "小车需要在出库轨道头或者下砖轨道";
                        //    return false;
                        //}
                        if (track.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
                        {
                            result = "小车在摆渡车时不能发上摆渡的指令";
                            return false;
                        }
                        if (!PubTask.Ferry.HaveFerryInPlace(carriertask, track.Type == TrackTypeE.下砖轨道 ? DeviceTypeE.下摆渡 : DeviceTypeE.上摆渡,
                            track.id, out ferryTraid, out ferryid, out result))
                        {
                            return false;
                        }

                        if (PubTask.Ferry.IsLoad(ferryid))
                        {
                            result = "摆渡车上有运输车";
                            return false;
                        }

                        checkTra = PubMaster.Track.GetTrackUpCode(ferryTraid);
                        toRFID = PubMaster.Track.GetTrackRFID2(ferryTraid);
                        order = DevCarrierOrderE.定位指令;
                        toTrackid = ferryTraid;

                        #endregion
                        break;

                    case DevCarrierTaskE.前进至点:
                        //常规：摆渡车出前进至点到达储砖轨道，在储砖轨道前进至点
                        //新增：在摆渡车入前进至点到达上砖机轨道

                        #region 前进至点
                        if (track.NotInType(TrackTypeE.摆渡车_入,TrackTypeE.摆渡车_出, TrackTypeE.储砖_入, TrackTypeE.储砖_出入,TrackTypeE.储砖_出))
                        {
                            result = "须在摆渡车或储砖轨道上执行！";
                            return false;
                        }

                        if (track.InType(TrackTypeE.储砖_出, TrackTypeE.储砖_出入)
                            && site >= track.rfid_2) //最大定位RFID

                        {
                            result = "当前储砖轨道位置不能再前进了！";
                            return false;
                        }

                        if (ExistCarInFront(devid, track.id))
                        {
                            result = "当前轨道前面还有其他运输车，不能执行前进至点的指令！";
                            return false;
                        }

                        order = DevCarrierOrderE.定位指令;
                        if (track.InType(TrackTypeE.储砖_入))
                        {
                            if (track.brother_track_id != 0 && HaveInTrack(track.brother_track_id)) 
                            {
                                result = "对应的出库轨道还有其他运输车，不能执行前进至点的指令！";
                                return false;
                            }
                            checkTra = PubMaster.Track.GetTrackDownCode(track.brother_track_id);
                            toRFID = PubMaster.Track.GetTrackRFID2(track.brother_track_id);
                            toTrackid = track.brother_track_id;
                        }
                        if (track.InType(TrackTypeE.储砖_出, TrackTypeE.储砖_出入))
                        {
                            checkTra = track.ferry_down_code;
                            toRFID = track.rfid_2;
                        }
                        isferryupsite = true;

                        #endregion

                        break;

                    case DevCarrierTaskE.后退至点:
                        //常规：摆渡车出后退至点到达储砖轨道，在储砖轨道后退至点
                        //新增：在摆渡车入后退至点到达下砖机轨道

                        #region 后退至点
                        if (track.NotInType(TrackTypeE.摆渡车_出,TrackTypeE.摆渡车_入, TrackTypeE.储砖_入, TrackTypeE.储砖_出入, TrackTypeE.储砖_出))
                        {
                            result = "须在摆渡车或储砖轨道上执行！";
                            return false;
                        }

                        if (site == track.rfid_1 //最小定位RFID
                            &&  (track.Type == TrackTypeE.储砖_入 || track.Type == TrackTypeE.储砖_出入))
                        {
                            result = "当前储砖轨道位置不能再后退了！";
                            return false;
                        }

                        if (ExistCarBehind(devid, track.id))
                        {
                            result = "当前轨道后面还有其他运输车，不能执行后退至点的指令！";
                            return false;
                        }

                        order = DevCarrierOrderE.定位指令;
                        if (track.InType(TrackTypeE.储砖_出))
                        {
                            if (track.brother_track_id != 0 && HaveInTrack(track.brother_track_id))
                            {
                                result = "对应的入库轨道还有其他运输车，不能执行后退至点的指令！";
                                return false;
                            }
                            checkTra = PubMaster.Track.GetTrackDownCode(track.brother_track_id);
                            toRFID = PubMaster.Track.GetTrackRFID1(track.brother_track_id);
                            toTrackid = track.brother_track_id;
                        }
                        if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入))
                        {
                            checkTra = track.ferry_up_code;
                            toRFID = track.rfid_1;
                        }

                        isferryupsite = false;

                        #endregion
                        break;

                    case DevCarrierTaskE.倒库:
                        #region 倒库
                        if (track.NotInType(TrackTypeE.储砖_出)) //最大定位RFID
                        {
                            result = "须在出库轨道上执行！";
                            return false;
                        }

                        if (!PubMaster.Goods.ExistStockInTrack(track.brother_track_id))
                        {
                            result = "对应的入库轨道并没有库存信息！";
                            return false;
                        }

                        if (!PubMaster.Track.IsTrackFull(track.brother_track_id))
                        {
                            result = "对应的入库轨道还没有满砖！";
                            return false;
                        }

                        if (!PubTask.Trans.CheckTrackCanDoSort(track.id, track.brother_track_id, devid, out result))
                        {
                            return false;
                        }

                        order = DevCarrierOrderE.往前倒库;
                        checkTra = track.ferry_down_code;
                        moveCount = (byte)PubMaster.Goods.GetTrackStockCount(track.brother_track_id);

                        if (PubMaster.Goods.ExistStockInTrack(track.id))
                        {
                            byte UpSortCount = (byte)PubMaster.Goods.GetTrackStockCount(track.id);
                            moveCount += UpSortCount;
                        }

                        memo = string.Format("[ {0} ], 倒库数量[ {1} ]", memo, moveCount);
                        #endregion
                        break;

                    case DevCarrierTaskE.顶升取货:
                        #region 顶升取货
                        if (track.InType(TrackTypeE.摆渡车_出, TrackTypeE.摆渡车_入))
                        {
                            result = "不能在摆渡车上执行！";
                            return false;
                        }
                        order = DevCarrierOrderE.取砖指令;

                        #endregion
                        break;

                    case DevCarrierTaskE.下降放货:
                        #region 下降放货
                        if (track.InType(TrackTypeE.摆渡车_出, TrackTypeE.摆渡车_入))
                        {
                            result = "不能在摆渡车上执行！";
                            return false;
                        }
                        order = DevCarrierOrderE.放砖指令;

                        #endregion
                        break;

                    case DevCarrierTaskE.前进至中间地标:
                        #region 前进至中间地标
                        if (track.NotInType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入))
                        {
                            result = "须在入库轨道上执行！";
                            return false;
                        }

                        if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入)
                            && site >= track.rfid_2) //最大定位RFID

                        {
                            result = "当前储砖轨道位置不能再前进了！";
                            return false;
                        }

                        order = DevCarrierOrderE.定位指令;
                        checkTra = track.ferry_down_code;
                        toRFID = track.rfid_2; // 中间地标 最大定位RFID
                        #endregion
                        break;

                    case DevCarrierTaskE.终止:
                        order = DevCarrierOrderE.终止指令;
                        break;
                }

                //小车在摆渡车上-离开摆渡车时，需要判断摆渡车是否停止、并且对准轨道
                if (carriertask != DevCarrierTaskE.终止
                    && carriertask != DevCarrierTaskE.倒库
                    && carriertask != DevCarrierTaskE.下降放货
                    && carriertask != DevCarrierTaskE.顶升取货
                    && carriertask != DevCarrierTaskE.前进至摆渡车
                    && carriertask != DevCarrierTaskE.后退至摆渡车)
                {
                    if (track.InType(TrackTypeE.摆渡车_出, TrackTypeE.摆渡车_入))
                    {
                        if (!PubTask.Ferry.IsStopAndSiteOnTrack(track.id, isferryupsite, out uint intrackid, out result))
                        {
                            return false;
                        }
                        Track tt = PubMaster.Track.GetTrack(intrackid);
                        if (tt == null)
                        {
                            try
                            {
                                mErrorLog.Error(true, string.Format("[DoManualNewTask]获取不到轨道信息[{0}]", intrackid));
                            }
                            catch { }
                            result = "获取不到摆渡车对上的轨道信息";
                            return false;
                        }
                        switch (carriertask)
                        {
                            case DevCarrierTaskE.后退取砖:
                                checkTra = tt.ferry_down_code;
                                if (tt.InType(TrackTypeE.下砖轨道, TrackTypeE.储砖_出入))
                                {
                                    toRFID = tt.rfid_1;
                                }
                                if (tt.InType(TrackTypeE.储砖_出))
                                {
                                    toPoint = tt.split_point;
                                }
                                overRFID = tt.rfid_2;
                                toTrackid = tt.id;
                                break;
                            case DevCarrierTaskE.前进放砖:
                                checkTra = tt.ferry_up_code;
                                if (tt.InType(TrackTypeE.上砖轨道))
                                {
                                    if (srfid != 0)
                                    {
                                        toRFID = srfid;
                                    }
                                    else
                                    {
                                        toRFID = tt.rfid_2;
                                    }
                                }
                                if (tt.InType(TrackTypeE.储砖_出入))
                                {
                                    toRFID = tt.rfid_2;
                                    overRFID = tt.rfid_1;
                                }
                                if (tt.InType(TrackTypeE.储砖_入))
                                {
                                    toPoint = tt.split_point;
                                    overRFID = tt.rfid_1;
                                }

                                toTrackid = tt.id;
                                break;
                            case DevCarrierTaskE.前进至点:
                                checkTra = tt.ferry_up_code;
                                toRFID = tt.rfid_1;
                                toTrackid = tt.id;
                                break;
                            case DevCarrierTaskE.后退至点:
                                checkTra = tt.ferry_down_code;
                                toRFID = tt.rfid_2;
                                toTrackid = tt.id;
                                break;
                        }
                    }
                }

                // 发送指令
                DoOrder(devid, 0, new CarrierActionOrder()
                {
                    Order = order,
                    CheckTra = checkTra,
                    ToRFID = toRFID,
                    ToPoint = toPoint,
                    OverRFID = overRFID,
                    OverPoint = overPoint,
                    MoveCount = moveCount,
                    ToTrackId = toTrackid
                },string.Format("【手动指令】[ {0} ], 备注[ {1} ]", order, memo));

                try
                {
                    mlog.Status(true, string.Format("运输车[ {0} ], 任务[ {1} ], 备注[ {2} ]",
                        PubMaster.Device.GetDeviceName(devid, devid + ""), carriertask, memo));
                }
                catch { }

                result = "";
                return true;
            }
            catch (Exception e)
            {
                mlog.Error(true, e.StackTrace);
                result = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 发送执行指令
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="cao"></param>
        /// <param name="memo">备注：非空则记录信息</param>
        public void DoOrder(uint devid, uint transid, CarrierActionOrder cao, string memo = "")
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == devid);
                    if (task != null)
                    {
                        // 手动中的直接终止
                        if (task.OperateMode == DevOperateModeE.手动 || cao.Order == DevCarrierOrderE.终止指令)
                        {
                            task.DoStop(transid, string.Format("【自动终止小车】, 触发[ {0} ], 模式[ {1} ], 指令[ {2} ], 备注[ {3} ]", "手动/终止指令", task.OperateMode, cao.Order, memo));
                            return;
                        }

                        // 连续同类型指令 需要先终止 - 待 PLC 后续优化
                        if (task.CurrentOrder == cao.Order)
                        {
                            task.DoStop(transid, string.Format("【自动终止小车】, 触发[ {0} ], 指令[ {1} ], 备注[ {2} ]", "发送同指令", cao.Order, memo));
                            return;
                        }

                        // 定位与结束相同时，不发结束
                        if (cao.ToRFID == cao.OverRFID)
                        {
                            cao.OverRFID = 0;
                        }
                        if (cao.ToPoint == cao.OverPoint)
                        {
                            cao.OverPoint = 0;
                        }

                        #region 交管摆渡车
                        if (!IsAllowToSend(task, cao.ToRFID, cao.OverRFID, out string result))
                        {
                            mlog.Info(true, string.Format(@"[ {0} ], 交管摆渡异常：{1}", task.Device.name, result));
                            return;
                        }
                        mlog.Info(true, string.Format(@"[ {0} ], 交管摆渡判断：{1}", task.Device.name, result));
                        #endregion

                        task.DoOrder(cao, transid, memo);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        /// <summary>
        /// 是否允许发送指令
        /// </summary>
        /// <param name="task"></param>
        /// <param name="toRFID"></param>
        /// <param name="overRFID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool IsAllowToSend(CarrierTask task, ushort toRFID, ushort overRFID, out string result)
        {
            // 当前/目的/结束 为摆渡车 - 交管
            Track track = new Track();
            result = "";
            if (task.CurrentTrackId > 0)
            {
                track = PubMaster.Track.GetTrack(task.CurrentTrackId);
                if (track == null)
                {
                    result = string.Format("找不到当前轨道ID[ {0} ]相关轨道数据", task.CurrentTrackId);
                    return false;
                }

                if (!IsAddTrafficControl(task, track, out string msg))
                {
                    result = msg;
                    return false;
                }
                if(!string.IsNullOrEmpty(msg)) result = result + "\n\t" + msg;
            }

            if (toRFID > 0)
            {
                track = PubMaster.Track.GetTrackBySite((ushort)task.AreaId, toRFID);
                if (track == null)
                {
                    result = string.Format("找不到指令中定位RFID[ {0} ]相关轨道数据", toRFID);
                    return false;
                }
                if (!IsAddTrafficControl(task, track, out string msg))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(msg)) result = result + "\n\t" + msg;
            }

            if (overRFID > 0)
            {
                track = PubMaster.Track.GetTrackBySite((ushort)task.AreaId, overRFID);
                if (track == null)
                {
                    result = string.Format("找不到指令中结束RFID[ {0} ]相关轨道数据", overRFID);
                    return false;
                }
                if (!IsAddTrafficControl(task, track, out string msg))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(msg)) result = result + "\n\t" + msg;
            }

            return true;
        }

        /// <summary>
        /// 是否加入运输交管摆渡
        /// </summary>
        /// <param name="task"></param>
        /// <param name="track"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool IsAddTrafficControl(CarrierTask task, Track track, out string msg)
        {
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.EnableCarrierTraffic))
            {
                //msg = string.Format("未打开运输车交管开关");
                msg = "";
                return true;
            }

            if (track.NotInType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出))
            {
                //msg = string.Format("指令涉及轨道[ {0} ]不是摆轨，不用交管摆渡", track.name);
                msg = "";
                return true;
            }

            uint ferryid = PubTask.Ferry.GetFerryByTrackid(track.id)?.ID ?? 0;
            if (ferryid == 0)
            {
                msg = string.Format("找不到摆轨[ {0} ]对应的摆渡车数据", track.id);
                return false;
            }

            // 是否存在被运输车交管
            if (PubTask.TrafficControl.ExistsTrafficControl(TrafficControlTypeE.运输车交管摆渡车, ferryid, out uint carid))
            {
                if (task.ID == carid)
                {
                    //msg = "小车连续交管相同的摆渡车，直接放行";
                    msg = "";
                    return true;
                }

                msg = string.Format("摆渡车[ {0} ]已被运输车[ {0} ]交管", 
                    PubMaster.Device.GetDeviceName(ferryid),
                    PubMaster.Device.GetDeviceName(carid));
                return false;
            }

            // 加入交管
            return PubTask.TrafficControl.AddTrafficControl(new TrafficControl()
            {
                area = track.area,
                TrafficControlType = TrafficControlTypeE.运输车交管摆渡车,
                restricted_id = ferryid,
                control_id = task.ID,
                from_track_id = task.CurrentTrackId,
                to_track_id = track.id
            }, out msg);
        }

        /// <summary>
        /// 发送设置复位点坐标指令
        /// </summary>
        /// <param name="devid">指定设备</param>
        /// <param name="rfid">复位地标</param>
        /// <param name="site">复位脉冲</param>
        public void DoResetSite(uint devid, ushort rfid, ushort site)
        {
            CarrierTask task = DevList.Find(c => c.ID == devid);
            if (task != null)
            {
                task.DoResetSite(rfid, site);
            }
        }

        /// <summary>
        /// 发送设置复位点坐标指令
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="rfid">复位地标</param>
        /// <param name="site">复位脉冲</param>
        public void DoAreaResetSite(uint areaid, ushort rfid, ushort site)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (CarrierTask task in DevList.FindAll(c => c.AreaId == areaid))
                    {
                        if (!task.IsConnect) continue;
                        task.DoResetSite(rfid, site);
                    }
                }
                catch { }
            })
            {
                IsBackground = true
            }.Start();

        }

        #endregion

        #region[分配-运输车]

        /// <summary>
        /// 根据任务分配运输车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="carrierid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool AllocateCarrier(StockTrans trans, out uint carrierid, out string result, List<uint> ferryids = null)
        {
            result = "";
            carrierid = 0;
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    bool IsGetCarrier = false;
                    switch (trans.TransType)
                    {
                        case TransTypeE.下砖任务:
                        case TransTypeE.手动下砖:
                            IsGetCarrier = GetTransInOutCarrier(trans, DeviceTypeE.下摆渡, out carrierid, out result, ferryids);
                            break;
                        case TransTypeE.上砖任务:
                        case TransTypeE.手动上砖:
                        case TransTypeE.反抛任务:
                            IsGetCarrier = GetTransInOutCarrier(trans, DeviceTypeE.上摆渡, out carrierid, out result, ferryids);
                            break;
                        case TransTypeE.上砖侧倒库:
                        case TransTypeE.倒库任务:
                            IsGetCarrier = GetTransSortCarrier(trans, out carrierid, out result);
                            break;
                        case TransTypeE.同向上砖:
                            IsGetCarrier = GetTransInOutCarrier(trans, DeviceTypeE.下摆渡, out carrierid, out result, ferryids);
                            break;
                        case TransTypeE.同向下砖:
                            IsGetCarrier = GetTransInOutCarrier(trans, DeviceTypeE.上摆渡, out carrierid, out result, ferryids);
                            break;
                    }
                    if (IsGetCarrier)
                    {
                        PubMaster.Warn.RemoveTaskWarn(WarningTypeE.FailAllocateCarrier, trans.id);
                    }
                    else if (!IsGetCarrier && mTimer.IsOver(TimerTag.FailAllocateCarrier, trans.id, 10, 5))
                    {
                        PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.FailAllocateCarrier, (ushort)trans.tilelifter_id, trans.id, result);
                    }
                    return IsGetCarrier;
                }
                catch (Exception e)
                {
                    mErrorLog.Error(true, e.Message + e.StackTrace + e.Source + e.Data.ToString());
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

            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);

            // 1.倒库空轨道是否有车[空闲，无货]
            CarrierTask carrier = DevList.Find(c => c.CurrentTrackId == trans.give_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
            #region[2.摆渡车上是否有车[空闲，无货]
            if (carrier == null)
            {
                //3.1获取能到达[空轨道]轨道的上砖摆渡车的轨道ID
                List<uint> ferrytrackids = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.上摆渡, trans.area_id, 0, trans.give_track_id, 0, true);
                //3.2获取在摆渡轨道上的车[空闲，无货]
                List<CarrierTask> carriers = DevList.FindAll(c => ferrytrackids.Contains(c.CurrentTrackId) && c.DevConfig.IsUseGoodsSize(goodssizeID));
                if (carriers.Count > 0)
                {
                    //如何判断哪个摆渡车最右
                    foreach (CarrierTask car in carriers)
                    {
                        //小车:没有任务绑定
                        if (!PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            //空闲,没货，没任务
                            if (CheckCarrierIsFree(car))
                            {
                                carrierid = car.ID;
                                return true;
                            }
                        }
                    }
                }
            }

            #region[2.满砖轨道是否有车[空闲，无货]]
            if (carrier == null)
            {
                carrier = DevList.Find(c => c.CurrentTrackId == trans.take_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
            }
            #endregion

            #endregion
            if (carrier != null
                   && carrier.ConnStatus == SocketConnectStatusE.通信正常
                   && carrier.OperateMode == DevOperateModeE.自动
                   && carrier.InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
            {
                carrierid = carrier.ID;
                return true;
            }

            //前面找到车了，如果空闲则分配，否则等待
            if (carrier != null)
            {
                if (CheckCarrierIsFree(carrier))
                {
                    carrierid = carrier.ID;
                    return true;
                }
                result = string.Format("取/卸货轨道上有运输车{0}，但运输车不符合状态，不能分配", carrier.Device.name);
                return false;
            }

            #region [5.找其他轨道]
            if (carrier == null)
            {
                // 优先车：同侧无砖
                List<CarrierTask> first_allocate_cars = new List<CarrierTask>();
                // 次级车：同侧载砖
                List<CarrierTask> second_allocate_cars = new List<CarrierTask>();
                // 末级车：远测无砖
                List<CarrierTask> third_allocate_cars = new List<CarrierTask>();
                // 末级车：远测载砖
                List<CarrierTask> fourth_allocate_cars = new List<CarrierTask>();

                // 获取任务砖机所有可作业轨道
                List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                // 按离取货点近远排序
                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.give_track_id, PubMaster.Track.GetTrackOrder(trans.give_track_id));

                //能去这个倒库轨道所有配置的摆渡车轨道信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(DeviceTypeE.上摆渡, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                string carNames = "";

                foreach (uint traid in tids)
                {
                    if (!PubMaster.Track.IsStoreType(traid)) continue;
                    List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == traid);
                    if (tasks.Count > 0)
                    {
                        // 每条轨道只拿一车出来加以选择
                        CarrierTask tracar = null;
                        ushort dis = 0;
                        foreach (CarrierTask item in tasks)
                        {
                            if (item == null) continue;
                            if (!item.IsWorking) continue;
                            if (item.ConnStatus == SocketConnectStatusE.通信正常
                                && item.Status == DevCarrierStatusE.停止
                                && item.IsNotDoingTask
                                && item.DevConfig.IsUseGoodsSize(goodssizeID))
                            {
                                //是否有能够到达该轨道的摆渡车
                                if (!PubMaster.Area.ExistFerryWithTrack(ferryids, item.CurrentTrackId))
                                {
                                    continue;
                                }

                                // 需要小车前进作业的，以最大脉冲为准
                                if (dis == 0 || dis < item.CurrentPoint)
                                {
                                    tracar = item;
                                    dis = item.CurrentPoint;
                                }
                            }
                        }

                        if (tracar == null || dis == 0) continue;
                        carNames += string.Format("[{0}]", tracar.Device.name);
                        // 上砖侧的RFID位数 [3XX99,3XX98,3XX96,3XX94]
                        if (tracar.CurrentSite % 100 > 90)
                        {
                            if (tracar.IsNotLoad())
                            {
                                first_allocate_cars.Add(tracar);
                            }
                            else
                            {
                                second_allocate_cars.Add(tracar);
                            }
                        }
                        else
                        {
                            if (tracar.IsNotLoad())
                            {
                                third_allocate_cars.Add(tracar);
                            }
                            else
                            {
                                fourth_allocate_cars.Add(tracar);
                            }
                        }
                    }
                }

                if (first_allocate_cars != null && first_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in first_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (second_allocate_cars != null && second_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in second_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (third_allocate_cars != null && third_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in third_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (fourth_allocate_cars != null && fourth_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in fourth_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (carNames == "")
                {
                    result = string.Format("{0}砖机分配的储砖轨道里没有符合状态的运输车，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                PubMaster.Device.GetDeviceName(trans.tilelifter_id), PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
                }
                else
                {
                    result = string.Format("{0}运输车不符合状态，不能分配，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                carNames, PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
                }
            }

            #endregion

            #region[直接找车]
            //else
            //{
            //    if (trans.area_id != 0)
            //    {
            //        List<AreaDevice> areatras = PubMaster.Area.GetAreaDevList(trans.area_id, DeviceTypeE.运输车);

            //        //能去这个倒库轨道所有配置的摆渡车轨道信息
            //        List<AreaDeviceTrack> traone = PubMaster.Area.GetFerryTrackId(trans.give_track_id);
            //        foreach (AreaDevice areatra in areatras)
            //        {
            //            CarrierTask task = DevList.Find(c => c.ID == areatra.device_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
            //            if (task != null && CheckCarrierIsFree(task)
            //                && PubMaster.Track.IsTrackType(task.CurrentTrackId, TrackTypeE.储砖_出)
            //                && PubMaster.Area.ExistFerryToBothTrack(traone, task.CurrentTrackId))
            //            {
            //                carrierid = task.ID;
            //                return true;
            //            }
            //        }

            //        result = string.Format("在出库轨道里没有符合状态的运输车，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{0}的砖】【没有被分配到其他任务】",
            //                    PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
            //    }
            //}
            #endregion

            return false;
        }

        /// <summary>
        /// 检测运输车通信状态是否正常并且空闲
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        private bool CheckIsConnFree(CarrierTask carrier)
        {
            if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                       && carrier.OperateMode == DevOperateModeE.自动)
            {
                if (carrier.Status == DevCarrierStatusE.停止
                    && carrier.IsNotDoingTask)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测运输车通信状态是否正常并且正在做指定的任务
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        private bool CheckIsConnInTask(CarrierTask carrier, params DevCarrierOrderE[] tasks)
        {
            if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                       && carrier.OperateMode == DevOperateModeE.自动)
            {
                if (carrier.InTask(tasks))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断运输车是否空闲<br/>
        /// 1.运输车
        /// </summary>
        /// <param name="trans">任务</param>
        /// <param name="carrier">检测的运输车</param>
        /// <param name="useifhave">如果有车则需要返回分配失败</param>
        /// <param name="carrierid">空闲的运输车ID</param>
        /// <param name="result">判断结果</param>
        /// <param name="returnfalse">是否反馈分配结果</param>
        /// <returns></returns>
        private bool CheckCarrierIsFree(StockTrans trans, CarrierTask carrier, bool useifhave, out uint carrierid, out string result, out bool returnfalse)
        {
            result = string.Empty;
            carrierid = 0;
            returnfalse = false;
            if (carrier == null)
            {
                return false;
            }

            if (!carrier.IsWorking)
            {
                if (useifhave)
                {
                    result = "运输车已停用！";
                    mlog.Status(true, string.Format("小车已停用-跳过分配\n" +
                        "小车：{0}\n" +
                        "任务：{1}", carrier.DevStatus.ToString(), trans.ToString()));
                    returnfalse = true;
                    return false;
                }
                return false;
            }
            else
            {
                if (CheckIsConnFree(carrier))
                {
                    carrierid = carrier.ID;
                    return true;
                }

                if (CheckCarrierFreeNoTask(carrier))
                {
                    carrierid = carrier.ID;
                    return true;
                }
            }

            if (carrier != null)
            {
                result = string.Format("取/卸货轨道上有运输车{0}，但运输车不符合状态，不能分配，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                       carrier.Device.name, PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
                if (useifhave)
                {
                    returnfalse = true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取靠近砖机的摆渡车所对轨道ID<br/>
        /// 1.如果只有一个摆渡车则直接获取摆渡车轨道
        /// 2.如果多个，则取靠近砖机的摆渡车轨道
        /// </summary>
        /// <param name="tracids"></param>
        /// <param name="tiletrackid"></param>
        /// <param name="defaulttrackid"></param>
        /// <returns></returns>
        private uint GetNearTileFerryOnTrack(List<uint> tracids, uint tiletrackid, uint defaulttrackid)
        {
            if (tracids.Count == 1) return tracids[0];
            if (tracids.Count > 1)
            {
                Track tiletrack = PubMaster.Track.GetTrack(tiletrackid);
                if (tiletrack != null)
                {
                    List<uint> ids = PubMaster.Track.SortTrackIdsWithOrder(tracids, tiletrackid, tiletrack.order);
                    if (ids.Count > 0) return ids[0];
                }
            }
            return defaulttrackid;
        }


        /// <summary>
        /// 上砖任务特殊分车逻辑
        /// 1.上砖工位
        /// 2.其他砖机工位考摆渡车进的运输车
        /// 3.其他储砖轨道考摆渡车进的
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="goodssizeID"></param>
        /// <param name="carrierid"></param>
        /// <param name="result"></param>
        /// <param name="fids"></param>
        /// <returns></returns>
        private bool GetTransOutCarrier(StockTrans trans, DeviceTypeE ferrytype, 
            uint goodssizeID, out uint carrierid, out string result, out bool returnfalse,
            List<uint> fids = null)
        {
            CarrierTask carrier = null;
            result = string.Empty;

            #region [1.上砖机轨道是否有车]

            //入库任务 -> 储砖入，出入
            //出库任务 -> 砖机轨道
            carrier = DevList.Find(c => c.CurrentTrackId == trans.give_track_id);
            if (CheckCarrierIsFree(trans, carrier, true, out carrierid, out result, out returnfalse))
            {
                return true;
            }

            if (returnfalse)
            {
                return false;
            }

            #endregion

            #region [2.取货轨道是否有车]
            if (carrier == null)
            {
                //入库任务 -> 砖机轨道
                //出库任务 -> 储砖出，出入
                List<CarrierTask> taketrackcarriers = DevList.FindAll(c => c.CurrentTrackId == trans.take_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
                if (taketrackcarriers.Count > 0)
                {
                    //脉冲大的排在前面
                    taketrackcarriers.Sort((x, y) => y.CurrentPoint.CompareTo(x.CurrentPoint));
                    carrier = taketrackcarriers[0];
                    if (trans.TransType == TransTypeE.上砖任务)
                    {
                        if (!carrier.IsNotDoingTask
                            && carrier.InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
                        {
                            carrier = null;
                        }

                        if (carrier != null
                            && carrier.IsNotDoingTask
                            && PubTask.Trans.IsCarrierInTrans(carrier.ID, trans.take_track_id, TransTypeE.上砖侧倒库, TransTypeE.倒库任务))
                        {
                            carrier = null;
                        }

                        //无任务，不做放砖任务
                        if (carrier != null
                            && !PubTask.Trans.HaveInCarrier(carrier.ID)
                            && carrier.NotInTask(DevCarrierOrderE.放砖指令))
                        {
                            //取砖任务
                            if(CheckIsConnInTask(carrier, DevCarrierOrderE.取砖指令))
                            {
                                carrierid = carrier.ID;
                                return true;
                            }

                            //在出轨道头空闲
                            Track track = PubMaster.Track.GetTrack(trans.take_track_id);
                            if(CheckCarrierIsFree(trans, carrier, false, out carrierid, out result, out returnfalse)
                                && carrier.CurrentSite >= track.rfid_2)
                            {
                                carrierid = carrier.ID;
                                return true;
                            }
                        }

                        carrier = null;
                    }
                }
            }
            #endregion

            #region[3.先找砖机轨道的空闲运输车]

            if (carrier == null)
            {
                //获取当前空闲摆渡车对上的轨道
                List<uint> ferryintrack = PubTask.Ferry.GetInTracks(fids);
                uint neartileferrytrackid = GetNearTileFerryOnTrack(ferryintrack, trans.give_track_id, trans.take_track_id);

                //所有上砖轨道
                List<uint> uptiletraids = PubMaster.Track.GetUpTileTracks(trans.area_id);

                // 按靠近砖机的摆渡车所对轨道进行排序
                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(uptiletraids, 0, PubMaster.Track.GetTrackOrder(neartileferrytrackid));

                //能去这个取货/卸货轨道的所有配置的摆渡车信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                string carNames = "";
                // 优先车：同侧无砖
                List<CarrierTask> first_allocate_cars = new List<CarrierTask>();
                // 次级车：同侧载砖
                List<CarrierTask> second_allocate_cars = new List<CarrierTask>();
                foreach (uint traid in tids)
                {
                    List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == traid);
                    if (tasks.Count > 0)
                    {
                        // 每条轨道只拿一车出来加以选择
                        CarrierTask tracar = null;
                        ushort dis = 0;
                        bool isUp = false;
                        foreach (CarrierTask item in tasks)
                        {
                            if (item == null) continue;
                            if (!item.IsWorking) continue;
                            if (item.ConnStatus == SocketConnectStatusE.通信正常
                                && item.Status == DevCarrierStatusE.停止
                                && item.OperateMode == DevOperateModeE.自动
                                && item.IsNotDoingTask
                                && item.DevConfig.IsUseGoodsSize(goodssizeID))
                            {
                                //是否有能够到达该轨道的摆渡车
                                if (!PubMaster.Area.ExistFerryWithTrack(ferryids, item.CurrentTrackId))
                                {
                                    continue;
                                }

                                isUp = false;
                                // 需要小车后退作业的，以最小脉冲为准
                                if (dis == 0 || dis > item.CurrentPoint)
                                {
                                    tracar = item;
                                    dis = item.CurrentPoint;
                                }
                            }
                        }

                        if (tracar == null || dis == 0) continue;
                        carNames += string.Format("[{0}]", tracar.Device.name);
                        if (tracar.IsNotLoad())
                        {
                            first_allocate_cars.Add(tracar);
                        }
                        else
                        {
                            second_allocate_cars.Add(tracar);
                        }
                    }
                }

                if (first_allocate_cars != null && first_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in first_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (second_allocate_cars != null && second_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in second_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }
            }

            #endregion

            #region [4.前面找到车了，如果空闲则分配，否则等待]

            if (carrier != null)
            {
                if (!carrier.IsWorking)
                {
                    result = "运输车已停用！";
                    mlog.Status(true, string.Format("小车已停用-跳过分配\n" +
                        "小车：{0}\n" +
                        "任务：{1}", carrier.DevStatus.ToString(), trans.ToString()));
                    carrier = null; // 继续往下找
                                    //return false;
                }
                else
                {
                    if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                        && carrier.OperateMode == DevOperateModeE.自动)
                    {
                        if (carrier.Status == DevCarrierStatusE.停止
                            && carrier.IsNotDoingTask)
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
                }

                if (carrier != null)
                {
                    result = string.Format("取/卸货轨道上有运输车{0}，但运输车不符合状态，不能分配，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                           carrier.Device.name, PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
                }
            }

            #endregion

            carrierid = 0;
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
        private bool GetTransInOutCarrier(StockTrans trans, DeviceTypeE ferrytype, out uint carrierid, out string result, List<uint> fids = null)
        {
            result = "";
            carrierid = 0;
            if (trans.goods_id == 0)
            {
                result = "任务没有品种id";
                return false;
            }

            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);

            CarrierTask carrier = null;

            if (GlobalWcsDataConfig.BigConifg.IsUpTaskNewAllocate(trans.area_id, trans.line)
                && trans.InType(TransTypeE.上砖任务, TransTypeE.手动上砖))
            {
                bool isallocate=  GetTransOutCarrier(trans, ferrytype, goodssizeID, out carrierid, out result, out bool returnfalse, fids);
                if (isallocate)
                {
                    return true;
                }

                if (returnfalse)
                {
                    return false;
                }
            }
            else
            {

                #region [1.取货轨道是否有车]
                //入库任务 -> 砖机轨道
                //出库任务 -> 储砖出，出入
                List<CarrierTask> taketrackcarriers = DevList.FindAll(c => c.CurrentTrackId == trans.take_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
                if (taketrackcarriers.Count > 0)
                {
                    //脉冲大的排在前面
                    taketrackcarriers.Sort((x, y) => y.CurrentPoint.CompareTo(x.CurrentPoint));
                    carrier = taketrackcarriers[0];
                    if (trans.TransType == TransTypeE.上砖任务)
                    {
                        if (!carrier.IsNotDoingTask
                            && carrier.InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
                        {
                            carrier = null;
                        }

                        if (carrier != null
                            && carrier.IsNotDoingTask
                            && PubTask.Trans.IsCarrierInTrans(carrier.ID, trans.take_track_id, TransTypeE.上砖侧倒库, TransTypeE.倒库任务))
                        {
                            carrier = null;
                        }
                    }
                }
                #endregion

                #region [2.卸货轨道是否有车]
                if (carrier == null)
                {
                    //入库任务 -> 储砖入，出入
                    //出库任务 -> 砖机轨道
                    carrier = DevList.Find(c => c.CurrentTrackId == trans.give_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
                }

                #endregion
            }

            #region [3.摆渡车上是否有车]
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

                //3.2获取在摆渡车上的车
                List<CarrierTask> carriers = DevList.FindAll(c => loadcarferryids.Contains(c.CurrentTrackId) && c.DevConfig.IsUseGoodsSize(goodssizeID));
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
                                    //空闲
                                    if (CheckCarrierIsFree(car))
                                    {
                                        if (car.IsLoad())
                                        {
                                            //摆渡车上的车载库存和任务对应的库存品种不符
                                            uint sgid = PubMaster.Goods.GetStockGoodId(car.DevConfig.stock_id);
                                            if (sgid != 0 && sgid != trans.goods_id)
                                            {
                                                break;
                                            }
                                        }
                                        carrierid = car.ID;
                                        return true;
                                    }
                                    break;
                                case TransTypeE.上砖任务:
                                case TransTypeE.同向上砖:
                                    //空闲
                                    if (CheckCarrierFreeNoTask(car))
                                    {
                                        if (car.IsLoad())
                                        {
                                            //摆渡车上的车载库存和任务对应的库存品种不符
                                            uint sgid = PubMaster.Goods.GetStockGoodId(car.DevConfig.stock_id);
                                            if (sgid != 0 && sgid != trans.goods_id)
                                            {
                                                break;
                                            }
                                        }
                                        carrierid = car.ID;
                                        return true;
                                    }
                                    break;
                                case TransTypeE.反抛任务:
                                    //空闲
                                    if (CheckCarrierFreeNoTask(car))
                                    {
                                        if (car.IsLoad())
                                        {
                                            break;
                                        }
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
                    //result = "摆渡车上有运输车，但运输车不符合状态，不能分配";
                }
            }

            #endregion

            #region [4.前面找到车了，如果空闲则分配，否则等待]
            if (carrier != null)
            {
                switch (trans.TransType)
                {
                    case TransTypeE.下砖任务:
                    case TransTypeE.手动下砖:
                        if (CheckCarrierIsFree(carrier))
                        {
                            carrierid = carrier.ID;
                            return true;
                        }
                        break;
                    case TransTypeE.上砖任务:
                    case TransTypeE.手动上砖:
                    case TransTypeE.同向上砖:
                    case TransTypeE.反抛任务:
                        if (!carrier.IsWorking)
                        {
                            result = "运输车已停用！";
                            mlog.Status(true, string.Format("小车已停用-跳过分配\n" +
                                "小车：{0}\n" +
                                "任务：{1}", carrier.DevStatus.ToString(), trans.ToString()));
                            carrier = null; // 继续往下找
                            //return false;
                        }
                        else
                        {
                            if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                                && carrier.OperateMode == DevOperateModeE.自动)
                            {
                                if (carrier.Status == DevCarrierStatusE.停止
                                    && carrier.IsNotDoingTask)
                                {
                                    carrierid = carrier.ID;
                                    return true;
                                }

                                if (carrier.CurrentOrder == DevCarrierOrderE.取砖指令 && carrier.FinishOrder == DevCarrierOrderE.无)
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
                        }
                        break;
                    case TransTypeE.倒库任务:
                        break;
                    case TransTypeE.其他:
                        break;
                    default:
                        break;
                }

                if (carrier != null)
                {
                    result = string.Format("取/卸货轨道上有运输车{0}，但运输车不符合状态，不能分配，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                           carrier.Device.name, PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
                }
            }

            #endregion

            #region [5.找其他轨道]
            if (carrier == null)
            {
                // 最优先车：砖机上的无砖运输车
                List<CarrierTask> zeroth_allocate_cars = new List<CarrierTask>();
                // 优先车：同侧无砖
                List<CarrierTask> first_allocate_cars = new List<CarrierTask>();
                // 次级车：同侧载砖
                List<CarrierTask> second_allocate_cars = new List<CarrierTask>();
                // 末级车：远测无砖
                List<CarrierTask> third_allocate_cars = new List<CarrierTask>();
                // 末级车：远测载砖
                List<CarrierTask> fourth_allocate_cars = new List<CarrierTask>();

                // 获取任务砖机所有可作业轨道
                List<uint> trackids; //= PubMaster.Area.GetTileTrackIds(trans);
                if (ferrytype == DeviceTypeE.上摆渡)
                {
                    trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.上砖轨道);
                }
                else
                {
                    trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, TrackTypeE.储砖_入, TrackTypeE.储砖_出入);
                }

                // 按离取货点近远排序
                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                //能去这个取货/卸货轨道的所有配置的摆渡车信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                string carNames = "";

                foreach (uint traid in tids)
                {
                    if (!PubMaster.Track.IsStoreType(traid))
                    {
                        //如果不是上砖机轨道或者不是上砖任务，就下一条轨道
                        if (!(PubMaster.Track.IsTrackType(traid, TrackTypeE.上砖轨道) && trans.TransType == TransTypeE.上砖任务))
                        {
                            continue;
                        }                        
                    }

                    List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == traid);
                    if (tasks.Count > 0)
                    {
                        // 每条轨道只拿一车出来加以选择
                        CarrierTask tracar = null;
                        ushort dis = 0;
                        bool isUp = false;
                        foreach (CarrierTask item in tasks)
                        {
                            if (item == null) continue;
                            if (!item.IsWorking) continue;
                            if (item.ConnStatus == SocketConnectStatusE.通信正常
                                && item.Status == DevCarrierStatusE.停止
                                //&& item.OperateMode == DevOperateModeE.自动
                                && item.IsNotDoingTask
                                //&& (item.CurrentOrder == item.FinishOrder || item.CurrentOrder == DevCarrierOrderE.无)
                                //&& item.CarrierType == needtype
                                && item.DevConfig.IsUseGoodsSize(goodssizeID))
                            {
                                //是否有能够到达该轨道的摆渡车
                                if (!PubMaster.Area.ExistFerryWithTrack(ferryids, item.CurrentTrackId))
                                {
                                    continue;
                                }

                                switch (trans.TransType)
                                {
                                    case TransTypeE.下砖任务:
                                    case TransTypeE.手动下砖:
                                    case TransTypeE.同向上砖:
                                        isUp = false;
                                        // 需要小车后退作业的，以最小脉冲为准
                                        if (dis == 0 || dis > item.CurrentPoint)
                                        {
                                            tracar = item;
                                            dis = item.CurrentPoint;
                                        }
                                        break;
                                    case TransTypeE.上砖任务:
                                    case TransTypeE.手动上砖:
                                    case TransTypeE.同向下砖:
                                    case TransTypeE.反抛任务:
                                        isUp = true;
                                        // 需要小车前进作业的，以最大脉冲为准
                                        if (dis == 0 || dis < item.CurrentPoint)
                                        {
                                            tracar = item;
                                            dis = item.CurrentPoint;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                        }

                        if (tracar == null || dis == 0) continue;
                        carNames += string.Format("[{0}]", tracar.Device.name);
                        if (isUp)
                        {
                            if (PubMaster.Track.IsTrackType(tracar.CurrentTrackId, TrackTypeE.上砖轨道))
                            {
                                zeroth_allocate_cars.Add(tracar);
                            }
                            // 上砖侧的RFID位数 [3XX99,3XX98,3XX96,3XX94]
                            else if (tracar.CurrentSite % 100 > 90)
                            {
                                if (tracar.IsNotLoad())
                                {
                                    first_allocate_cars.Add(tracar);
                                }
                                else
                                {
                                    second_allocate_cars.Add(tracar);
                                }
                            }
                            else
                            {
                                if (tracar.IsNotLoad())
                                {
                                    third_allocate_cars.Add(tracar);
                                }
                                else
                                {
                                    fourth_allocate_cars.Add(tracar);
                                }
                            }
                        }
                        else
                        {
                            // 下砖侧的RFID位数 [3XX06,3XX04,3XX02,3XX00]
                            if (tracar.CurrentSite % 100 < 10)
                            {
                                if (tracar.IsNotLoad())
                                {
                                    first_allocate_cars.Add(tracar);
                                }
                                else
                                {
                                    second_allocate_cars.Add(tracar);
                                }
                            }
                            else
                            {
                                if (tracar.IsNotLoad())
                                {
                                    third_allocate_cars.Add(tracar);
                                }
                                else
                                {
                                    fourth_allocate_cars.Add(tracar);
                                }
                            }
                        }
                    }

                }

                if (zeroth_allocate_cars != null && zeroth_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in zeroth_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (first_allocate_cars != null && first_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in first_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (second_allocate_cars != null && second_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in second_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (third_allocate_cars != null && third_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in third_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (fourth_allocate_cars != null && fourth_allocate_cars.Count > 0)
                {
                    foreach (CarrierTask car in fourth_allocate_cars)
                    {
                        if (CheckCarrierIsFree(car)
                            && car.DevConfig.IsUseGoodsSize(goodssizeID)
                            && !PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            carrierid = car.ID;
                            return true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(carNames))
                {
                    result = string.Format("任务ID[{0}]分配的储砖轨道里没有符合状态的运输车，分配条件：[启用] [通讯正常] [停止] [指令完成] [能取{1}的砖] [没有被分配到其他任务]",
                                trans.id, PubMaster.Goods.GetGoodsSizeName(trans.goods_id));
                }
                else
                {
                    result = string.Format("任务ID[{2}]分配的运输车[{0}]不符合状态，不能分配，分配条件：[启用] [通讯正常] [停止] [指令完成] [能取{1}的砖] [没有被分配到其他任务]",
                                carNames, PubMaster.Goods.GetGoodsSizeName(trans.goods_id), trans.id);
                }
            }

            #endregion

            return false;
        }


        /// <summary>
        /// 判断是否存在运输车绑定了该库存
        /// </summary>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool ExistCarrierBindStock(uint carrier_id, uint stockid)
        {
            return DevList.Exists(c => c.ID != carrier_id && c.DevConfig.stock_id == stockid && c.IsLoad());
        }

        /// <summary>
        /// 小车当前是否状态符合
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        private bool CheckCarrierIsFree(CarrierTask carrier)
        {
            if (carrier == null) return false;
            if (!carrier.IsWorking) return false;
            if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                    && carrier.Status == DevCarrierStatusE.停止
                    //&& carrier.OperateMode == DevOperateModeE.自动  // 非自动状态应该也要分配小车
                    && carrier.IsNotDoingTask
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

        internal CarrierTask GetDevCarrier(uint id)
        {
            return DevList.Find(c => c.ID == id);
        }

        internal List<CarrierTask> GetDevCarriers(List<uint> areaids)
        {
            return DevList.FindAll(c => areaids.Contains(c.AreaId));
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
                    //&& carrier.OperateMode == DevOperateModeE.自动  // 非自动状态应该也要分配小车
                    && carrier.IsNotDoingTask
                    //&& (carrier.CurrentOrder == carrier.FinishOrder || carrier.CurrentOrder == DevCarrierOrderE.无)
                    )
            {
                return true;
            }
            return false;
        }

        public bool IsCarrierFree(uint carrierid)
        {
            CarrierTask carrier = DevList.Find(c => c.ID == carrierid);
            return CheckCarrierIsFree(carrier);
        }

        /// <summary>
        /// 获取不能直接通过一个摆渡车到达的运输车ID们
        /// </summary>
        /// <param name="trans">交易信息</param>
        /// <param name="ferrytype">摆渡车类型</param>
        /// <param name="checktakegivetrack">检查取货放砖轨道</param>
        /// <param name="tids">区域线路和砖机分配的轨道</param>
        /// <param name="ferryids">任务允许的摆渡车</param>
        /// <returns></returns>
        internal List<CarrierTask> GetFreeCarrierWithNoDirectFerry(StockTrans trans, DeviceTypeE ferrytype, bool checktakegivetrack, 
                                                                                            out List<uint> tids, out List<uint> ferryids)
        {
            List<CarrierTask> freecarrierid = new List<CarrierTask>();
            
            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);

            // 优先车：同侧无砖
            List<CarrierTask> first_allocate_cars = new List<CarrierTask>();
            // 次级车：同侧载砖
            List<CarrierTask> second_allocate_cars = new List<CarrierTask>();
            // 末级车：远测无砖
            List<CarrierTask> third_allocate_cars = new List<CarrierTask>();
            // 末级车：远测载砖
            List<CarrierTask> fourth_allocate_cars = new List<CarrierTask>();
            List<uint> trackids;
            // 获取任务区域所有可作业轨道
            if (ferrytype == DeviceTypeE.上摆渡)
            {
                trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
            }
            else
            {
                trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, TrackTypeE.储砖_入, TrackTypeE.储砖_出入);
            }
            // 按离取货点近远排序    tids
            //能去这个取货/卸货轨道的所有配置的摆渡车信息    ferryids
            if (checktakegivetrack)
            {
                tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));
                ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);
            }
            else
            {
                tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.give_track_id, PubMaster.Track.GetTrackOrder(trans.give_track_id));
                ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);
            }

            string carNames = "";

            foreach (uint traid in tids)
            {
                if (!PubMaster.Track.IsStoreType(traid)) continue;
                List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == traid);
                if (tasks.Count > 0)
                {
                    // 每条轨道只拿一车出来加以选择
                    CarrierTask tracar = null;
                    ushort dis = 0;
                    bool isUp = false;
                    foreach (CarrierTask item in tasks)
                    {
                        if (item == null) continue;
                        if (!item.IsWorking) continue;
                        if (!item.DevConfig.IsUseGoodsSize(goodssizeID)) continue;

                        if (item.ConnStatus == SocketConnectStatusE.通信正常
                            && item.Status == DevCarrierStatusE.停止
                            && item.OperateMode == DevOperateModeE.自动
                            && item.IsNotDoingTask)
                        {
                            //不能直接直接到达作业轨道
                            if (!PubMaster.Area.ExistFerryWithTrack(ferryids, item.CurrentTrackId))
                            {
                                switch (trans.TransType)
                                {
                                    case TransTypeE.下砖任务:
                                    case TransTypeE.手动下砖:
                                    case TransTypeE.同向上砖:
                                        isUp = false;
                                        // 需要小车后退作业的，以最小脉冲为准
                                        if (dis == 0 || dis > item.CurrentPoint)
                                        {
                                            tracar = item;
                                            dis = item.CurrentPoint;
                                        }
                                        break;
                                    case TransTypeE.上砖任务:
                                    case TransTypeE.手动上砖:
                                    case TransTypeE.同向下砖:
                                        isUp = true;
                                        // 需要小车前进作业的，以最大脉冲为准
                                        if (dis == 0 || dis < item.CurrentPoint)
                                        {
                                            tracar = item;
                                            dis = item.CurrentPoint;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if (tracar == null || dis == 0) continue;
                    carNames += string.Format("[{0}]", tracar.Device.name);
                    if (isUp)
                    {
                        // 上砖侧的RFID位数 [3XX99,3XX98,3XX96,3XX94]
                        if (tracar.CurrentSite % 100 > 90)
                        {
                            if (tracar.IsNotLoad())
                            {
                                first_allocate_cars.Add(tracar);
                            }
                            else
                            {
                                second_allocate_cars.Add(tracar);
                            }
                        }
                        else
                        {
                            if (tracar.IsNotLoad())
                            {
                                third_allocate_cars.Add(tracar);
                            }
                            else
                            {
                                fourth_allocate_cars.Add(tracar);
                            }
                        }
                    }
                    else
                    {
                        // 下砖侧的RFID位数 [3XX06,3XX04,3XX02,3XX00]
                        if (tracar.CurrentSite % 100 < 10)
                        {
                            if (tracar.IsNotLoad())
                            {
                                first_allocate_cars.Add(tracar);
                            }
                            else
                            {
                                second_allocate_cars.Add(tracar);
                            }
                        }
                        else
                        {
                            if (tracar.IsNotLoad())
                            {
                                third_allocate_cars.Add(tracar);
                            }
                            else
                            {
                                fourth_allocate_cars.Add(tracar);
                            }
                        }
                    }
                }
            }

            #region[判断小车空闲并添加到空闲列表]
            if (first_allocate_cars != null && first_allocate_cars.Count > 0)
            {
                foreach (CarrierTask car in first_allocate_cars)
                {
                    if (CheckCarrierIsFree(car)
                        && car.DevConfig.IsUseGoodsSize(goodssizeID)
                        && !PubTask.Trans.HaveInCarrier(car.ID))
                    {
                        freecarrierid.Add(car);
                    }
                }
            }

            if (second_allocate_cars != null && second_allocate_cars.Count > 0)
            {
                foreach (CarrierTask car in second_allocate_cars)
                {
                    if (CheckCarrierIsFree(car)
                        && car.DevConfig.IsUseGoodsSize(goodssizeID)
                        && !PubTask.Trans.HaveInCarrier(car.ID))
                    {
                        freecarrierid.Add(car);
                    }
                }
            }

            if (third_allocate_cars != null && third_allocate_cars.Count > 0)
            {
                foreach (CarrierTask car in third_allocate_cars)
                {
                    if (CheckCarrierIsFree(car)
                        && car.DevConfig.IsUseGoodsSize(goodssizeID)
                        && !PubTask.Trans.HaveInCarrier(car.ID))
                    {
                        freecarrierid.Add(car);
                    }
                }
            }

            if (fourth_allocate_cars != null && fourth_allocate_cars.Count > 0)
            {
                foreach (CarrierTask car in fourth_allocate_cars)
                {
                    if (CheckCarrierIsFree(car)
                        && car.DevConfig.IsUseGoodsSize(goodssizeID)
                        && !PubTask.Trans.HaveInCarrier(car.ID))
                    {
                        freecarrierid.Add(car);
                    }
                }
            }
            #endregion

            return freecarrierid;
        }

        /// <summary>
        /// 获取运输车卸货脉冲或者当前的位置脉冲
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal ushort GetCarrierUnloadPointOrNowPoint(uint carrier_id)
        {
            CarrierTask carrier = DevList.Find(c => c.ID == carrier_id);
            if (carrier == null) return 0;

            if (carrier.GivePoint != 0) return carrier.GivePoint;
            return carrier.CurrentPoint;
        }

        /// <summary>
        /// 获取运输车卸货脉冲
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal ushort GetCarrierUnloadPoint(uint carrier_id)
        {
            CarrierTask carrier = DevList.Find(c => c.ID == carrier_id);
            if (carrier == null) return 0;

            if (carrier.GivePoint != 0) return carrier.GivePoint;
            return 0;
        }

        /// <summary>
        /// 获取运输车当前的位置脉冲
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal ushort GetCarrierNowPoint(uint carrier_id)
        {
            CarrierTask carrier = DevList.Find(c => c.ID == carrier_id);
            if (carrier == null) return 0;
            
            return carrier.CurrentPoint;
        }        
        
        /// <summary>
        /// 获取运输车当前脉冲和卸货脉冲
        /// </summary>
        /// <param name="carrier_id">运输车ID</param>
        /// <param name="current">当前脉冲</param>
        /// <param name="give">卸货脉冲</param>
        internal void GetCarrierNowUnloadPoint(uint carrier_id, out ushort current, out ushort give)
        {
            CarrierTask carrier = DevList.Find(c => c.ID == carrier_id);
            if (carrier == null)
            {
                current = 0;
                give = 0;
                return;
            }
            current = carrier.CurrentPoint;
            give = carrier.GivePoint;
        }

        /// <summary>
        /// 获取指定类型的轨道上的空闲运输车
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public CarrierTask GetCarrierFree(uint area_id, out uint brotrackid, params TrackTypeE[] types)
        {
            List<uint> trackids = PubMaster.Track.GetAreaTrackIdList(area_id, types);
            foreach (uint traid in trackids)
            {
                List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == traid);
                if (tasks.Count == 1)
                {
                    if (tasks[0] == null) continue;
                    if (!tasks[0].IsWorking) continue;
                    if (CheckCarrierIsFree(tasks[0]) && tasks[0].OperateMode == DevOperateModeE.自动)
                    {
                        if (!PubTask.Trans.HaveCarrierInTrans(tasks[0].ID))
                        {
                            brotrackid = PubMaster.Track.GetBrotherTrackId(tasks[0].CurrentTrackId);
                            if (!PubTask.Carrier.HaveInTrack(brotrackid))
                            {
                                return tasks[0];
                            }
                        }
                    }
                }
            }
            brotrackid = 0;
            return null;
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
            CarrierTask carrier = DevList.Find(c => c.CurrentTrackId == trackid);
            if (carrier != null) return false;

            uint brotherid = PubMaster.Track.GetBrotherTrackId(trackid);
            if (brotherid == 0)
            {
                //没有兄弟轨道，不会碰撞？？(可能也是配置漏了)
                return true;
            }
            carrier = DevList.Find(c => c.CurrentTrackId == brotherid);
            if (carrier != null)
            {
                if ((PubMaster.Goods.GetTrackStockCount(trackid)
                    + PubMaster.Goods.GetTrackStockCount(brotherid)) > 3)
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取当前RFID
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        internal ushort GetCurrentSite(uint devId)
        {
            return DevList.Find(c => c.ID == devId)?.DevStatus?.CurrentSite ?? 0;
        }
        
        /// <summary>
        /// 获取当前坐标值
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        internal ushort GetCurrentPoint(uint devId)
        {
            return DevList.Find(c => c.ID == devId)?.DevStatus?.CurrentPoint ?? 0;
        }

        /// <summary>
        /// 判断小车是否完成了取砖
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsCarrierFinishLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.IsLoad()
                                    && (c.CurrentOrder == c.FinishOrder || c.CurrentOrder == DevCarrierOrderE.无)
                                    );
        }

        /// <summary>
        /// 判断小车是否完成了卸砖
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsCarrierFinishUnLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.IsNotLoad()
                                    && (c.CurrentOrder == c.FinishOrder || c.CurrentOrder == DevCarrierOrderE.无)
                                    );
        }

        /// <summary>
        /// 判断小车是否正在执行该指令
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        internal bool IsCarrierInTask(uint carrier_id, params DevCarrierOrderE[] Orders)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && !c.IsNotDoingTask
                                    && Orders.Contains(c.CurrentOrder)
                                    && c.CurrentOrder != c.FinishOrder);
        }

        /// <summary>
        /// 判断小车是否已完成该指令
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        internal bool IsCarrierFinishTask(uint carrier_id,params DevCarrierOrderE[] Order)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.IsNotDoingTask
                                    && Order.Contains(c.CurrentOrder)
                                    && c.CurrentOrder == c.FinishOrder);
        }

        /// <summary>
        /// 判断是否有小车在同一个轨道并且位置在指定小车的前面(脉冲值更大)
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool ExistCarInFront(uint carrier_id, uint trackid)
        {
            ushort carpoint = GetCurrentPoint(carrier_id);
            if(carpoint > 0)
            {
                return DevList.Exists(c => c.CurrentTrackId == trackid && c.ID != carrier_id && c.CurrentPoint > carpoint);
            }
            return false;
        }

        /// <summary>
        /// 判断是否有小车在同一个轨道并且位置在指定小车的前面(脉冲值更大)
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool ExistCarInFront(uint carrier_id, uint trackid, out uint otherid)
        {
            ushort carpoint = GetCurrentPoint(carrier_id);
            if (carpoint > 0)
            {
                otherid = DevList.Find(c => c.CurrentTrackId == trackid && c.ID != carrier_id && c.CurrentPoint > carpoint)?.ID ?? 0;
                return otherid != 0;
            }
            otherid = 0;
            return false;
        }

        /// <summary>
        /// 判断是否有小车在同一个轨道并且位置在指定小车的后面(脉冲值更小)
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool ExistCarBehind(uint carrier_id, uint trackid)
        {
            ushort carpoint = GetCurrentPoint(carrier_id);
            if (carpoint > 0)
            {
                return DevList.Exists(c => c.CurrentTrackId == trackid && c.ID != carrier_id && c.CurrentPoint < carpoint);
            }
            return false;
        }

        internal bool ExistCarBehind(uint carrier_id, uint trackid, out uint otherid)
        {
            ushort carpoint = GetCurrentPoint(carrier_id);
            if (carpoint > 0)
            {
                otherid = DevList.Find(c => c.CurrentTrackId == trackid && c.ID != carrier_id && c.CurrentPoint < carpoint)?.ID ?? 0 ;
                return otherid != 0;
            }
            otherid = 0;
            return false;
        }

        /// <summary>
        /// 判断是否有定位到轨道的小车
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="give_track_id"></param>
        /// <returns></returns>
        internal bool ExistLocateTrack(uint carrier_id, uint track_id)
        {
            return DevList.Exists(c =>c.ID != carrier_id && (c.TargetTrackId == track_id || c.OnGoingTrackId == track_id));
        }

        /// <summary>
        /// 判断小车是否符合目的位置
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="site"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal bool IsCarrierTargetMatches(uint carrier_id, ushort site = 0, ushort point = 0, bool checkswitch = true)
        {
            if (checkswitch && !PubMaster.Dic.IsSwitchOnOff(DicTag.SeamlessMoveToFerry))
            {
                return false;
            }

            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && ((site > 0 && c.TargetSite > 0 && c.TargetSite == site) || (point > 0 && c.TargetPoint > 0 && c.TargetPoint == point))
                                    );
        }

        /// <summary>
        /// 是否有车移动顶部的库存前往轨道头部
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="take_track_id"></param>
        /// <returns></returns>
        public bool HaveCarrierMoveTopInTrackUpTop(uint carrier_id, uint track_id)
        {
            List<CarrierTask> carriers = DevList.FindAll(c => c.ID != carrier_id && c.CurrentTrackId == track_id && c.IsLoad());
            foreach (var item in carriers)
            {
                uint stockid = item.DevConfig.stock_id;
                if(stockid != 0 && PubMaster.Goods.IsTopStock(stockid))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断接力车和上砖车是否会相撞
        /// 如果接力车在前进，上砖车停止或者在后退，则判断会相撞
        /// </summary>
        /// <param name="one_car_id">接力车</param>
        /// <param name="two_car_id">上砖车</param>
        /// <returns></returns>
        public bool IsCollision(uint one_car_id, uint two_car_id)
        {
            CarrierTask onecar = DevList.Find(c => c.ID == one_car_id && c.DevStatus.DeviceStatus == DevCarrierStatusE.前进);
            return onecar != null ? DevList.Exists(c => c.ID == two_car_id && (c.DevStatus.DeviceStatus == DevCarrierStatusE.后退 || c.DevStatus.DeviceStatus == DevCarrierStatusE.停止 )) : false;
        }

        /// <summary>
        /// 根据运输车所在的位置，判断需要使用上摆渡车还是下摆渡车
        /// </summary>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        internal DeviceTypeE GetCarrierNeedFerryType(uint carrierid)
        {
            CarrierTask task = DevList.Find(c => c.ID == carrierid);
            if (task != null)
            {
                Track currenttrack = PubMaster.Track.GetTrack(task.CurrentTrackId);
                if (currenttrack != null)
                {
                    switch (currenttrack.Type)
                    {
                        case TrackTypeE.下砖轨道:
                        case TrackTypeE.摆渡车_入:
                        case TrackTypeE.储砖_入:
                            return DeviceTypeE.下摆渡;
                        case TrackTypeE.上砖轨道:
                        case TrackTypeE.摆渡车_出:
                        case TrackTypeE.储砖_出:
                            return DeviceTypeE.上摆渡;
                        case TrackTypeE.储砖_出入:
                            int downdis = Math.Abs(task.CurrentPoint - currenttrack.limit_point);
                            int updis = Math.Abs(task.CurrentPoint - currenttrack.limit_point_up);
                            if (downdis < updis)
                            {
                                return DeviceTypeE.下摆渡;
                            }
                            return DeviceTypeE.上摆渡;
                    }
                }
            }

            return DeviceTypeE.其他;
        }
        #endregion

        #region[小车逻辑警告]

        public void SetCarrierAlert(uint carrierid, uint trackid, CarrierAlertE type, bool isalert)
        {
            CarrierTask task = DevList.Find(c => c.ID == carrierid);
            if (task != null)
            {
                task.SetAlert(type, trackid, isalert);
            }
        }
        #endregion

        #region[启动/停止]

        public void UpdateWorking(uint devId, bool working)
        {
            CarrierTask task = DevList.Find(c => c.ID == devId);
            if (task != null)
            {
                task.IsWorking = working;
                MsgSend(task, task.DevStatus);
            }
        }

        #endregion

        #region[极限混砖]
        /// <summary>
        /// 判断有没有小车在对应的轨道
        /// </summary>
        /// <param name="type">任务类型</param>
        /// <param name="trackid">检测的轨道</param>
        /// <param name="carrierid">除了这个小车外</param>
        /// <param name="result">判断结果</param>
        /// <returns></returns>
        internal bool CheckHaveCarInTrack(TransTypeE type, uint trackid, uint carrierid, out string result)
        {
            CarrierTask carrier = null;
            Track track = PubMaster.Track.GetTrack(trackid);
            switch (track.Type)
            {
                case TrackTypeE.储砖_出入:
                    switch (type)
                    {
                        case TransTypeE.下砖任务:
                            carrier = DevList.Find(c => c.ID != carrierid
                                                && c.CurrentTrackId == trackid
                                                && (c.CurrentSite == track.rfid_1
                                                    || (c.CurrentSite == track.rfid_2 && c.InTask(DevCarrierOrderE.放砖指令))));
                            if(carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
                            break;
                        case TransTypeE.上砖任务:
                            carrier = DevList.Find(c => c.ID != carrierid
                                                && c.CurrentTrackId == trackid
                                                && (c.CurrentSite == track.rfid_2
                                                    || (c.CurrentSite == track.rfid_1 && c.InTask(DevCarrierOrderE.取砖指令))));
                            if (carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
                            break;
                        case TransTypeE.倒库任务:
                            break;
                        case TransTypeE.移车任务:
                            break;                    
                    }
                    break;
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                    switch (type)
                    {
                        case TransTypeE.倒库任务:
                        case TransTypeE.上砖侧倒库:
                            carrier = DevList.Find(c => c.ID != carrierid
                                                && (c.CurrentTrackId == track.id || c.CurrentTrackId == track.brother_track_id)
                                                && (c.InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库) 
                                                         || PubTask.Trans.IsCarrierInTrans(c.ID, track.id, TransTypeE.上砖侧倒库, TransTypeE.倒库任务)));
                            if (carrier != null )
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
                            break;
                        case TransTypeE.上砖任务://除了倒库任务的运输车
                            carrier = DevList.Find(c => c.ID != carrierid
                               && c.CurrentTrackId == track.id 
                               && c.NotInTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库) 
                               && !PubTask.Trans.IsCarrierInTrans(c.ID, trackid,  TransTypeE.上砖侧倒库, TransTypeE.倒库任务));
                            if (carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
                            break;
                    }

                    break;
            }

            result = "";
            return false;
        }

        /// <summary>
        /// 查找在摆渡车上的运输车
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="ids">运输车ID</param>
        /// <returns></returns>
        internal bool GetInFerryAndLoad(uint areaid, out List<uint> ids, TrackTypeE type)
        {
            List<CarrierTask> list = DevList.FindAll(c => c.AreaId == areaid
                         && c.IsConnect
                         && c.Load == DevCarrierLoadE.有货
                         && PubMaster.Track.IsTrackType(c.CurrentTrackId, type));
            if (list.Count > 0)
            {
                ids = list.Select(c => c.ID).ToList();
                return true;
            }
            ids = null;
            return false;
        }

        /// <summary>
        /// 小车倒库中并且把货放下后后退中
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsCarrierUnLoadAndBackWard(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id 
                                        && c.InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库)
                                        && c.Status == DevCarrierStatusE.后退 
                                        && c.IsNotLoad());
        }

        /// <summary>
        /// 判断小车是否处于轨道地标位置
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsCarrierInTrackBiggerRfID1(uint carrier_id, uint track_id)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            return IsCarrierInTrackBiggerSite(carrier_id, track_id, track.rfid_1);
        }

        /// <summary>
        /// 判断小车是否处于轨道地标位置
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsCarrierInTrackBiggerRfID2(uint carrier_id, uint track_id)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            return IsCarrierInTrackBiggerSite(carrier_id, track_id, track.rfid_2);
        }

        internal bool IsCarrierInTrackBiggerSite(uint carrier_id, uint track_id, ushort rfid)
        {
            return DevList.Exists(c => c.ID == carrier_id && c.CurrentTrackId == track_id && c.IsNotDoingTask && c.CurrentSite >= rfid);
        }

        /// <summary>
        /// 判断小车是否处于轨道地标位置
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsCarrierInTrackSmallerRfID1(uint carrier_id, uint track_id)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            return IsCarrierInTrackSmallerSite(carrier_id, track_id, track.rfid_1);
        }

        internal bool IsCarrierInTrackSmallerSite(uint carrier_id, uint track_id, ushort rfid)
        {
            return DevList.Exists(c => c.ID == carrier_id && c.CurrentTrackId == track_id && c.IsNotDoingTask && c.CurrentSite <= rfid);
        }

        /// <summary>
		/// 运输车当前地标是否小于判断的地标
        /// 不判断是否任务中
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <param name="rfid"></param>
        /// <returns></returns>
        internal bool IsCarrierSmallerSite(uint carrier_id, uint track_id, ushort rfid)
        {
            return DevList.Exists(c => c.ID == carrier_id && c.CurrentTrackId == track_id && c.CurrentSite <= rfid);
        }


        /// <summary>
        /// 判断小车是否处于轨道地标位置
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsFreeCarrierInTrack(uint carrier_id, uint track_id, out uint carid)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            carid = DevList.Find(c => c.ID != carrier_id && c.CurrentTrackId == track_id && c.IsNotDoingTask && c.CurrentSite >= track.rfid_1)?.ID ?? 0;
            return carid > 0;
        }
        #endregion
    }

    /// <summary>
    /// 运输车动作指令
    /// </summary>
    public class CarrierActionOrder
    {
        /// <summary>
        /// 指令类型
        /// </summary>
        public DevCarrierOrderE Order { set; get; }
        /// <summary>
        /// 校验轨道
        /// </summary>
        public ushort CheckTra { set; get; } = 0;
        /// <summary>
        /// 定位RFID
        /// </summary>
        public ushort ToRFID { set; get; } = 0;
        /// <summary>
        /// 定位坐标
        /// </summary>
        public ushort ToPoint { set; get; } = 0;
        /// <summary>
        /// 结束RFID
        /// </summary>
        public ushort OverRFID { set; get; } = 0;
        /// <summary>
        /// 结束坐标
        /// </summary>
        public ushort OverPoint { set; get; } = 0;
        /// <summary>
        /// 倒库数量
        /// </summary>
        public byte MoveCount { set; get; } = 0;
        /// <summary>
        /// 前往作业轨道
        /// </summary>
        public uint ToTrackId { set; get; } = 0;
    }
}
