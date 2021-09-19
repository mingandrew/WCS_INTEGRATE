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
                                    if (track.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                                    {
                                        PubTask.Ferry.StopFerryByFerryTrackId(track.id, string.Format("运输车[ {0} ], 在摆渡车上[ {1} ], 执行[ {2} ]中", task.Device.name, track.name, task.OnGoingOrder), "锁定");
                                    }
                                }

                                // 目的前往摆渡车
                                Track targettrack = PubMaster.Track.GetTrack(task.TargetTrackId);
                                if (targettrack != null && targettrack.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                                {
                                    // 判断是否有摆渡车
                                    if (!PubTask.Ferry.IsTargetFerryInPlace((ushort)task.AreaId, task.CurrentSite, task.TargetSite, out string result, true))
                                    {
                                        task.DoStop(0, string.Format("【自动终止小车】, 触发[ {0} ], 位置[ {1} ], 其他[ {2} ]", "摆渡车状态不对", track.Type, result));
                                        Thread.Sleep(500);
                                    }

                                    if (task.OnGoingOrder == DevCarrierOrderE.定位指令)
                                    {
                                        PubTask.Ferry.StopFerryByFerryTrackId(targettrack.id, string.Format("运输车[ {0} ], 定位[ {1} -> {2} ]", task.Device.name, track.name, targettrack.name), "锁定");
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
                                //离线超过10秒并且没有在主动断开
                                if (!task.IsDevOfflineInBreak
                                    && task.IsOfflineTimeOver())
                                {
                                    task.SetDevConnOnBreak(true);
                                    task.Stop("休息3秒断开连接");
                                }

                                //主动断开时间超过后，开始重连
                                if (task.IsDevOfflineInBreak && task.IsInBreakOver())
                                {
                                    task.SetDevConnOnBreak(false);
                                    task.Start("休息3秒后开始连接");
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
                                // 超过 60s 没有更新过设备状态就查询一次
                                if (task.IsRefreshTimeOver(60))
                                {
                                    task.DoQuery();
                                    task.ReSetRefreshTime();
                                }
                                else
                                {
                                    if (task.ConnStatus != SocketConnectStatusE.通信正常)
                                    {
                                        task.DoQuery();
                                        task.ReSetRefreshTime();
                                    }
                                }
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
        /// 是否存在运输车在指定的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrack(uint trackid, uint carrierid)
        {
            return DevList.Exists(c => c.ID != carrierid && c.CurrentTrackId == trackid);
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道（获取单个）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrackAndGetSingle(uint trackid, out CarrierTask car, uint carrierid = 0)
        {
            car = DevList.Find(c => c.ID != carrierid && c.CurrentTrackId == trackid);
            return car != null;
        }

        /// <summary>
        /// 查找是否存在运输车在指定的轨道（获取全部）
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="carrierid"></param>
        /// <param name="cars"></param>
        /// <returns></returns>
        internal bool HaveInTrackAndGetAll(uint trackid, out List<CarrierTask> cars, uint carrierid = 0)
        {
            cars = DevList.FindAll(c => c.ID != carrierid && c.CurrentTrackId == trackid);
            return cars != null && cars.Count > 0;
        }

        /// <summary>
        /// 小车完成任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsStopFTask(uint carrier_id, Track track = null)
        {
            if (track != null)
            {
                if (track.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                {
                    return DevList.Exists(c => c.ID == carrier_id
                           && c.ConnStatus == SocketConnectStatusE.通信正常
                           && c.OperateMode == DevOperateModeE.自动
                           && c.Status == DevCarrierStatusE.停止
                           && c.IsNotDoingTask
                           && c.Position == DevCarrierPositionE.在摆渡上
                           //&& c.CurrentSite == track.rfid_1
                           );
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

        /// <summary>
        /// 获取小车当前所在轨道
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        public Track GetCarrierTrack(uint carrier_id)
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

        /// <summary>
        /// 获取当前RFID（轨道编号）
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        internal ushort GetCurrentSite(uint devId)
        {
            return DevList.Find(c => c.ID == devId)?.CurrentSite ?? 0;
        }

        /// <summary>
        /// 获取当前坐标值
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        internal ushort GetCurrentPoint(uint devId)
        {
            return DevList.Find(c => c.ID == devId)?.CurrentPoint ?? 0;
        }

        /// <summary>
        /// 获取目标坐标值
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        internal ushort GetTargetPoint(uint devId)
        {
            return DevList.Find(c => c.ID == devId)?.TargetPoint ?? 0;
        }

        /// <summary>
        /// 小车是否载货
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.IsLoad());
        }

        /// <summary>
        /// 小车是否载砖位于摆渡车上
        /// </summary>
        /// <param name="ltrack"></param>
        /// <returns></returns>
        internal bool IsLoadInFerry(uint ltrack)
        {
            return DevList.Exists(c => c.CurrentTrackId == ltrack
                         && c.IsLoad()
                         && c.Position == DevCarrierPositionE.在摆渡上);
        }

        /// <summary>
        /// 小车是否无货
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsNotLoad(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.IsNotLoad());
        }

        /// <summary>
        /// 小车是否上升/下降 中
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsLoadLifting(uint carrier_id)
        {
            return DevList.Exists(c => c.ID == carrier_id
                        && c.ConnStatus == SocketConnectStatusE.通信正常
                        && (c.Load == DevCarrierLoadE.上升中 || c.Load == DevCarrierLoadE.下降中));
        }

        /// <summary>
        /// 获取小车当前位置状态
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal DevCarrierPositionE GetPosition(uint carrier_id)
        {
            return DevList.Find(c => c.ID == carrier_id)?.Position ?? DevCarrierPositionE.异常;
        }

        /// <summary>
        /// 小车是否初始化-写入复位脉冲中
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <returns></returns>
        internal bool IsResetWriting(uint carrier_id)
        {
            return DevList.Find(c => c.ID == carrier_id)?.IsResetWork() ?? false;
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
                                task.DoReply(); // 接收后回复PLC
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
                    task.DoStop(0, string.Format("【手动时终止小车】, 触发[ {0} ], 位置[ {1} ], 指令[ {2} ]", "手动操作小车", track?.name, task.CurrentOrder));
                }
            }
            #endregion

            #region[更新摆渡车载货状态]
            if (track != null)
            {
                switch (track.Type)
                {
                    case TrackTypeE.后置摆渡轨道:
                    case TrackTypeE.前置摆渡轨道:
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

            #region[检查任务]

            #region [取卸货]

            //放货动作
            if (task.DevConfig.stock_id != 0 && task.IsNotLoad() && track != null)
            {
                if (task.DevStatus.GivePoint != 0)
                {
                    PubMaster.Goods.UpdateStockLocation(task.DevConfig.stock_id, task.DevStatus.GivePoint);
                }
                else
                {
                    PubMaster.Goods.UpdateStockLocation(task.DevConfig.stock_id, task.DevStatus.CurrentPoint);
                }

                //判断放下砖的时候轨道是否是能否放砖的轨道
                if (track.NotInType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                {
                    //将库存转移到轨道的位置
                    PubMaster.Goods.MoveStock(task.DevConfig.stock_id, track.id, "", task.ID);

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
                            task.DevConfig.stock_id = PubMaster.Goods.GetStockInTileTrack(track.id, tileid, gid);
                            if (task.DevConfig.stock_id == 0)
                            {
                                if (PubTask.TileLifter.AddTileStockInTrack(track.id, task.CurrentSite, out uint stockid))
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
                        case TrackTypeE.后置摆渡轨道:
                        case TrackTypeE.前置摆渡轨道:
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
                    mErrorLog.Error(true, string.Format("【取砖】小车[ {0} ]尝试在轨道[ {1} ]上取砖; 状态[ {2} ]",
                        task.Device.name, track?.GetLog() ?? task.DevStatus.CurrentSite + "", task.DevStatus.GetTakeString()));
                }
            }

            #endregion

            #region[运输车切换轨道]

            if (task.DevConfig.stock_id != 0)
            {
                //根据小车当前的位置更新库存对应所在的轨道
                PubMaster.Goods.MoveStock(task.DevConfig.stock_id, task.CurrentTrackId, task.CurrentOrder + "", task.ID);

                if (!task.IsNotDoingTask && task.IsLoad())
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
            if (track != null && track.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
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
                                                        DevCarrierOrderE.放砖指令, DevCarrierOrderE.倒库指令))
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
                #region [调试指令判断]
                List<DevCarrierTaskE> debugOrders = new List<DevCarrierTaskE>
                {
                    DevCarrierTaskE.前进寻复位标志点,
                    DevCarrierTaskE.后退寻复位标志点,
                    DevCarrierTaskE.测试上升,
                    DevCarrierTaskE.测试下降
                };
                bool isdebug = debugOrders.Contains(carriertask);
                if (isdebug && !GlobalWcsDataConfig.DebugConfig.IsDebug)
                {
                    result = "未启用调试模式！不可执行该指令";
                    return false;
                }
                #endregion

                Track track = GetCarrierTrack(devid);
                if (carriertask != DevCarrierTaskE.终止 && !isdebug && track == null) // 不用管当前位置的指令
                {
                    result = "未能获取到小车位置相关信息！";
                    return false;
                }

                // 初始化中不执行动作指令
                if (carriertask != DevCarrierTaskE.终止 && IsResetWriting(devid))
                {
                    result = "小车初始化/复位寻点中，请先终止";
                    return false;
                }

                // 上升/下降 不能发指令
                if (IsLoadLifting(devid)
                    && carriertask != DevCarrierTaskE.原地上升取砖
                    && carriertask != DevCarrierTaskE.原地下降放砖
                    && carriertask != DevCarrierTaskE.测试上升
                    && carriertask != DevCarrierTaskE.测试下降)
                {
                    result = "小车上升中/下降中";
                    return false;
                }

                // 小车当前所在RF点
                //ushort site = GetCurrentSite(devid);
                ushort carPoint = GetCurrentPoint(devid); // 改用脉冲

                DevCarrierOrderE order = DevCarrierOrderE.终止指令;
                ushort checkTra = 0;//校验轨道号
                ushort toRFID = 0;//目标点
                ushort toPoint = 0;//目标脉冲
                ushort overRFID = 0;//结束点
                ushort overPoint = 0;//结束脉冲
                byte moveCount = 0;//倒库数量
                uint toTrackid = 0;//目标轨道ID

                Track toTrack; // 作业轨道

                uint carstkid = 0; //车上库存ID
                ushort stkloc = 0; // 库存脉冲

                switch (carriertask)
                {
                    case DevCarrierTaskE.后退取砖:
                        #region 后退取砖
                        if (IsLoad(devid))
                        {
                            result = "运输车有货不能后退取砖！";
                            return false;
                        }

                        if (track.IsFerryTrack())
                        {
                            // 获取摆渡车后侧对应的轨道
                            if (!PubTask.Ferry.IsInPlaceByFerryTraid(false, track.id, out toTrackid, out result))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            toTrackid = track.id;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }
                        
                        if (track.IsFerryTrack() && toTrack.IsStoreTrack())
                        {
                            // 在摆渡车上，取储砖轨道  则获取库存脉冲
                            toPoint = PubMaster.Goods.GetStockInLocMax(toTrackid)?.location ?? 0;
                            if (toPoint == 0)
                            {
                                result = "无合适取砖库存坐标，请尝试定位到轨道内再发取砖指令";
                                return false;
                            }
                        }
                        else if (toTrack.IsTileTrack())
                        {
                            // 砖机轨道靠配置脉冲
                            toPoint = (srfid == 0 ? toTrack.limit_point : srfid);
                        }
                        else
                        {
                            // 轨道类型是否允许后退取砖
                            if (toTrack.is_take_forward)
                            {
                                result = "轨道无法执行后退取砖！";
                                return false;
                            }

                            // 轨道内改用靠光电取砖（ 1 -后退，65535 -前进 ）
                            toPoint = 1;
                        }

                        if (carPoint <= toPoint)
                        {
                            result = "不能再后退了！";
                            return false;
                        }

                        checkTra = toTrack.ferry_up_code;
                        overPoint = toTrack.limit_point_up;
                        order = DevCarrierOrderE.取砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.后退放砖:
                        #region 后退放砖
                        if (IsNotLoad(devid))
                        {
                            result = "运输车无砖不能后退放砖！";
                            return false;
                        }

                        if (track.IsFerryTrack())
                        {
                            // 获取摆渡车后侧对应的轨道
                            if (!PubTask.Ferry.IsInPlaceByFerryTraid(false, track.id, out toTrackid, out result))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            toTrackid = track.id;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        // 轨道类型是否允许后退放砖
                        if (!toTrack.is_give_back)
                        {
                            result = "目的轨道无法执行后退放砖！";
                            return false;
                        }

                        toPoint = (srfid != 0 ? srfid : toTrack.limit_point);
                        if (toTrack.IsStoreTrack())
                        {
                            carstkid = PubMaster.DevConfig.GetCarrierStockId(devid);
                            if (!PubMaster.Goods.CalculateNextLocByDir(DevMoveDirectionE.后, devid, toTrackid, carstkid, out stkloc))
                            {
                                result = "无合适存砖坐标！";
                                return false;
                            }

                            if (stkloc > 0) toPoint = stkloc;
                        }

                        if (carPoint <= toPoint)
                        {
                            result = "不能再后退了！";
                            return false;
                        }

                        // 砖机工位需要需求才可用
                        if (toTrack.IsTileTrack() && !PubTask.TileLifter.IsTileTrackEmptyNeed(toTrack.id, toPoint, out result))
                        {
                            return false;
                        }

                        checkTra = toTrack.ferry_up_code;
                        overPoint = toTrack.limit_point_up;
                        order = DevCarrierOrderE.放砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.前进取砖:
                        #region 前进取砖
                        if (IsLoad(devid))
                        {
                            result = "运输车有砖不能前进取砖！";
                            return false;
                        }

                        if (track.IsFerryTrack())
                        {
                            // 获取摆渡车前侧对应的轨道
                            if (!PubTask.Ferry.IsInPlaceByFerryTraid(true, track.id, out toTrackid, out result))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            toTrackid = track.id;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        if (track.IsFerryTrack() && toTrack.IsStoreTrack())
                        {
                            // 在摆渡车上，取储砖轨道  则获取库存脉冲
                            toPoint = PubMaster.Goods.GetStockInLocMin(toTrackid)?.location ?? 0;
                            if (toPoint == 0)
                            {
                                result = "无合适取砖库存坐标，请尝试定位到轨道内再发取砖指令";
                                return false;
                            }
                        }
                        else if (toTrack.IsTileTrack())
                        {
                            // 砖机轨道靠配置脉冲
                            toPoint = (srfid != 0 ? srfid : toTrack.limit_point_up);
                        }
                        else
                        {
                            // 轨道类型是否允许前进取砖
                            if (!toTrack.is_take_forward)
                            {
                                result = "轨道无法执行前进取砖！";
                                return false;
                            }

                            // 改用靠光电取砖（ 1 -后退，65535 -前进 ）
                            toPoint = 65535;
                        }

                        if (carPoint >= toPoint)
                        {
                            result = "不能再前进了！";
                            return false;
                        }

                        checkTra = toTrack.ferry_down_code;
                        overPoint = toTrack.limit_point;
                        order = DevCarrierOrderE.取砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.前进放砖:
                        #region 前进放砖
                        if (IsNotLoad(devid))
                        {
                            result = "运输车无砖不能前进放砖！";
                            return false;
                        }

                        if (track.IsFerryTrack())
                        {
                            // 获取摆渡车前侧对应的轨道
                            if (!PubTask.Ferry.IsInPlaceByFerryTraid(true, track.id, out toTrackid, out result))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            toTrackid = track.id;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        // 轨道类型是否允许前进放砖
                        if (toTrack.is_give_back)
                        {
                            result = "目的轨道无法执行前进放砖！";
                            return false;
                        }

                        toPoint = (srfid != 0 ? srfid : toTrack.limit_point_up);
                        if (toTrack.IsStoreTrack())
                        {
                            carstkid = PubMaster.DevConfig.GetCarrierStockId(devid);
                            if (!PubMaster.Goods.CalculateNextLocByDir(DevMoveDirectionE.前, devid, toTrackid, carstkid, out stkloc))
                            {
                                result = "无合适存砖坐标！";
                                return false;
                            }

                            if (stkloc > 0) toPoint = stkloc;
                        }

                        if (carPoint >= toPoint)
                        {
                            result = "不能再前进了！";
                            return false;
                        }

                        // 砖机工位需要需求才可用
                        if (toTrack.IsTileTrack() && !PubTask.TileLifter.IsTileTrackEmptyNeed(toTrack.id, toPoint, out result))
                        {
                            return false;
                        }

                        checkTra = toTrack.ferry_down_code;
                        overPoint = toTrack.limit_point;
                        order = DevCarrierOrderE.放砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.后退至摆渡车:
                        #region 后退至摆渡车
                        if (track.IsFerryTrack())
                        {
                            // 显示摆渡但实际脉冲未到
                            if (carPoint > track.limit_point && (carPoint - track.limit_point >= 100))
                            {
                                checkTra = track.ferry_down_code;
                                overPoint = track.limit_point;
                                order = DevCarrierOrderE.定位指令;
                                break;
                            }

                            result = "小车已经在摆渡车上了";
                            return false;
                        }
                        if (track.ferry_up_code < 200)
                        {
                            result = "不能再后退了";
                            return false;
                        }

                        // 超过复位点脉冲才能上摆渡 - 暂不需要
                        //if (PubMaster.Track.CanMoveToFerryAboutPos(track.id, DevMoveDirectionE.后退, site, out result))
                        //{
                        //    return false;
                        //}

                        // 是否存在前侧到位的摆渡车
                        if (!PubTask.Ferry.HaveFerryInPlace(true, track.id, out toTrackid, out result))
                        {
                            return false;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        checkTra = toTrack.ferry_down_code;
                        overPoint = toTrack.limit_point;
                        order = DevCarrierOrderE.定位指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.前进至摆渡车:
                        #region 前进至摆渡车
                        if (track.IsFerryTrack())
                        {
                            // 显示摆渡但实际脉冲未到
                            if (track.limit_point_up > carPoint && (track.limit_point_up - carPoint >= 100))
                            {
                                checkTra = track.ferry_up_code;
                                overPoint = track.limit_point_up;
                                order = DevCarrierOrderE.定位指令;
                                break;
                            }

                            result = "小车已经在摆渡车上了";
                            return false;
                        }
                        if (track.ferry_down_code > 500)
                        {
                            result = "不能再前进了";
                            return false;
                        }

                        // 超过复位点脉冲才能上摆渡 - 暂不需要
                        //if (PubMaster.Track.CanMoveToFerryAboutPos(track.id, DevMoveDirectionE.前进, site, out result))
                        //{
                        //    return false;
                        //}

                        // 是否存在后侧到位的摆渡车
                        if (!PubTask.Ferry.HaveFerryInPlace(false, track.id, out toTrackid, out result))
                        {
                            return false;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        checkTra = toTrack.ferry_up_code;
                        overPoint = toTrack.limit_point_up;
                        order = DevCarrierOrderE.定位指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.前进至定位点:
                        #region 前进至点
                        switch (track.Type)
                        {
                            case TrackTypeE.上砖轨道:
                            case TrackTypeE.下砖轨道:
                            case TrackTypeE.储砖_出:
                            case TrackTypeE.储砖_出入:
                                if (Math.Abs(carPoint - track.limit_point_up) <= 20) // 暂定（+-20）脉冲
                                {
                                    result = "小车已经不能再前进了";
                                    return false;
                                }
                                toTrackid = track.id;
                                break;

                            case TrackTypeE.储砖_入:
                                toTrackid = track.brother_track_id;
                                break;

                            case TrackTypeE.后置摆渡轨道:
                            case TrackTypeE.前置摆渡轨道:
                                // 获取摆渡车前侧对应的轨道
                                if (!PubTask.Ferry.IsInPlaceByFerryTraid(true, track.id, out toTrackid, out result))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        overPoint = track.IsFerryTrack() ? toTrack.limit_point : toTrack.limit_point_up;
                        // 车身前 100 >> 点位前 200 内不可有库存
                        if (toTrack.IsStoreTrack()
                            && IsLoad(devid)
                            && PubMaster.Goods.IsStockWithinRange(toTrackid, (ushort)(carPoint + 100), (ushort)(overPoint + 200)))
                        {
                            result = "载货前进的方向上检测到有砖挡着！";
                            return false;
                        }

                        checkTra = track.IsFerryTrack() ? toTrack.ferry_up_code : toTrack.ferry_down_code;
                        order = DevCarrierOrderE.定位指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.后退至定位点:
                        #region 后退至点
                        switch (track.Type)
                        {
                            case TrackTypeE.上砖轨道:
                            case TrackTypeE.下砖轨道:
                            case TrackTypeE.储砖_入:
                            case TrackTypeE.储砖_出入:
                                if (Math.Abs(carPoint - track.limit_point) <= 20) // 暂定（+-20）脉冲
                                {
                                    result = "小车已经不能再后退了";
                                    return false;
                                }
                                toTrackid = track.id;
                                break;

                            case TrackTypeE.储砖_出:
                                toTrackid = track.brother_track_id;
                                break;

                            case TrackTypeE.后置摆渡轨道:
                            case TrackTypeE.前置摆渡轨道:
                                // 获取摆渡车后侧对应的轨道
                                if (!PubTask.Ferry.IsInPlaceByFerryTraid(false, track.id, out toTrackid, out result))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }

                        toTrack = PubMaster.Track.GetTrack(toTrackid);
                        if (toTrack == null)
                        {
                            result = "无目的轨道数据！";
                            return false;
                        }

                        overPoint = track.IsFerryTrack() ? toTrack.limit_point_up : toTrack.limit_point;
                        // 车身后100 >> 点位后 200 内不可有库存
                        if (toTrack.IsStoreTrack() 
                            && IsLoad(devid) 
                            && PubMaster.Goods.IsStockWithinRange(toTrackid, (ushort)(overPoint - 200), (ushort)(carPoint - 100)))
                        {
                            result = "载货后退的方向上检测到有砖挡着！";
                            return false;
                        }

                        checkTra = track.IsFerryTrack() ? toTrack.ferry_down_code : toTrack.ferry_up_code;
                        order = DevCarrierOrderE.定位指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.出库方向倒库:
                        #region 倒库
                        if (!track.IsStoreTrack())
                        {
                            result = "只能在储砖轨道内执行！";
                            return false;
                        }

                        if (carPoint == 0)
                        {
                            result = "运输车脉冲反馈异常！";
                            return false;
                        }

                        #region 载砖
                        if (IsLoad(devid))
                        {
                            // 获取出库方向上临近的库存
                            Stock stk = PubMaster.Goods.GetStockInfrontStockPoint(track.id, carPoint);
                            // 计算下一车位置
                            if (!PubMaster.Goods.CalculateNextLocByStock(track.is_take_forward ? DevMoveDirectionE.前 : DevMoveDirectionE.后, stk, out toPoint, devid))
                            {
                                toPoint = track.is_take_forward ? track.limit_point : track.limit_point_up;
                            }

                            toTrackid = track.id;
                            checkTra = track.ferry_up_code;
                            order = DevCarrierOrderE.放砖指令;
                            if ((track.is_take_forward && carPoint <= toPoint) ||
                                (!track.is_take_forward && carPoint >= toPoint))
                            {
                                // 原地放砖
                                memo = string.Format("{0}；倒库脉冲判断[ {1} > {2} ]-原地放砖", memo, carPoint, toPoint);
                                toPoint = 0;
                                overPoint = 0;
                                break;
                            }
                            else
                            {
                                // 定位放砖
                                memo = string.Format("{0}；倒库脉冲判断[ {1} > {2} ]-定位放砖", memo, carPoint, toPoint);
                                overPoint = toPoint;
                                break;
                            }

                        }
                        #endregion

                        #region 空砖
                        if (IsNotLoad(devid))
                        {
                            // 按反方向加点距离，防止获取到当前顶上库存
                            carPoint = (ushort)(track.is_take_forward ? (carPoint + 10) : (carPoint - 10));

                            // 取砖库存
                            Stock stkT = PubMaster.Goods.GetStockBehindStockPoint(track.id, carPoint);
                            if (stkT == null || stkT.location == 0)
                            {
                                result = "小车当前位置范围内无可倒库的库存！";
                                return false;
                            }
                            toPoint = stkT.location;

                            // 获取出库方向上临近的库存
                            Stock stkG = PubMaster.Goods.GetStockInfrontStockPoint(track.id, toPoint);
                            // 计算下一车位置
                            if (!PubMaster.Goods.CalculateNextLocByStock(track.is_take_forward ? DevMoveDirectionE.前 : DevMoveDirectionE.后, stkG, out overPoint, devid))
                            {
                                overPoint = track.is_take_forward ? track.limit_point : track.limit_point_up;
                            }

                            // 取放位置间距  ≈173CM
                            ushort dis = 100;
                            if (Math.Abs(toPoint - overPoint) <= dis)
                            {
                                result = "倒库间距小于 一米七！";
                                return false;
                            }

                            toTrackid = track.id;
                            checkTra = track.ferry_up_code;
                            order = DevCarrierOrderE.倒库指令;
                            memo = string.Format("{0}；倒库脉冲判断[ {1} > {2} ]-倒库指令", memo, carPoint, toPoint);
                            break;
                        }
                        #endregion

                        result = "运输车载货状态异常！";
                        return false;
                        #endregion

                    case DevCarrierTaskE.原地上升取砖:
                        #region 顶升取货
                        if (IsLoad(devid))
                        {
                            result = "运输车有货不能取砖！";
                            return false;
                        }
                        if (track.IsFerryTrack())
                        {
                            result = "不能在摆渡车上执行！";
                            return false;
                        }
                        checkTra = track.ferry_up_code;
                        order = DevCarrierOrderE.取砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.原地下降放砖:
                        #region 下降放货
                        if (IsNotLoad(devid))
                        {
                            result = "运输车无货不能放砖！";
                            return false;
                        }
                        if (track.IsFerryTrack())
                        {
                            result = "不能在摆渡车上执行！";
                            return false;
                        }
                        checkTra = track.ferry_up_code;
                        order = DevCarrierOrderE.放砖指令;
                        #endregion
                        break;

                    case DevCarrierTaskE.终止:
                        order = DevCarrierOrderE.终止指令;
                        break;

                    case DevCarrierTaskE.前进寻复位标志点:
                        order = DevCarrierOrderE.寻点;
                        overPoint = 65535;  //最大脉冲-表示 前进
                        break;

                    case DevCarrierTaskE.后退寻复位标志点:
                        order = DevCarrierOrderE.寻点;
                        overPoint = 1;          //最小脉冲-表示 后退
                        break;

                    case DevCarrierTaskE.测试上升:
                        if (PubMaster.DevConfig.GetCarrierStockId(devid) > 0)
                        {
                            result = "运输车已绑定库存！";
                            return false;
                        }
                        order = DevCarrierOrderE.测试上升;
                        break;

                    case DevCarrierTaskE.测试下降:
                        if (PubMaster.DevConfig.GetCarrierStockId(devid) > 0)
                        {
                            result = "运输车已绑定库存！";
                            return false;
                        }
                        order = DevCarrierOrderE.测试下降;
                        break;

                }

                if (toTrackid > 0 && HaveInTrack(toTrackid, devid))
                {
                    result = "目的轨道有其他运输车！";
                    return false;
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
                }, string.Format("【手动指令】[ {0} ], 备注[ {1} ]", order, memo), true);

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
        public void DoOrder(uint devid, uint transid, CarrierActionOrder cao, string memo = "", bool autostop = false)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    CarrierTask task = DevList.Find(c => c.ID == devid);
                    if (task != null)
                    {
                        // 初始化中不执行动作指令
                        if (task.IsResetWriting)
                        {
                            task.DoStop(transid, string.Format("【自动终止小车】, 触发[ {0} ], 指令[ {1} ], 备注[ {2} ]", "初始化-写入PLC复位脉冲中", cao.Order, memo));
                            return;
                        }

                        // 寻点 - 一直慢速，直到扫到复位接近开关
                        if (cao.Order == DevCarrierOrderE.寻点)
                        {
                            task.DoMoveToResetPoint(cao.OverPoint == 1 ? CarrierResetE.后退寻点 : CarrierResetE.前进寻点);
                            return;
                        }

                        // 手动中的直接终止
                        if (task.OperateMode == DevOperateModeE.手动 || cao.Order == DevCarrierOrderE.终止指令)
                        {
                            task.DoStop(transid, string.Format("【自动终止小车】, 触发[ {0} ], 模式[ {1} ], 指令[ {2} ], 备注[ {3} ]", "手动中/终止中", task.OperateMode, cao.Order, memo));
                            return;
                        }

                        // 手动中连续不同类型指令 需要先终止
                        if (transid == 0 && !task.IsNotDoingTask && task.NotInTask(cao.Order))
                        {
                            // 当前 取砖&放砖，不可终止
                            if (task.DevStatus.CurrentOrder == DevCarrierOrderE.取砖指令 || 
                                task.DevStatus.CurrentOrder == DevCarrierOrderE.放砖指令)
                            {
                                return;
                            }

                            task.DoStop(transid, string.Format("【自动终止小车】, 触发[ {0} ], 指令[ {1} ], 备注[ {2} ]", "发送同指令", cao.Order, memo));

                            if (!autostop) return;
                            Thread.Sleep(500); // 手动操作的终止后继续下发其他指令
                        }

                        // 连续同类型指令  无需发送指令类型  只改变其中位置信息即可
                        if (!task.IsNotDoingTask && task.InTask(cao.Order))
                        {
                            cao.Order = DevCarrierOrderE.无;
                        }

                        // 取放砖指令 定位与结束脉冲相近时 无需再移动
                        if (cao.Order == DevCarrierOrderE.取砖指令 || cao.Order == DevCarrierOrderE.放砖指令)
                        {
                            if (Math.Abs(cao.ToPoint - cao.OverPoint) <= 15) // ≈26CM
                            {
                                cao.OverPoint = cao.ToPoint;
                            }
                        }

                        // 无轨道编号就以当前为准
                        if (cao.CheckTra == 0) cao.CheckTra = task.CurrentSite;

                        #region 交管摆渡车
                        if (!IsAllowToSend(task, cao.CheckTra, out string result))
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
        private bool IsAllowToSend(CarrierTask task, ushort tracode, out string result)
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
                if (!string.IsNullOrEmpty(msg)) result = result + "\n\t" + msg;
            }

            if (tracode > 0)
            {
                track = PubMaster.Track.GetTrackBySite((ushort)task.AreaId, tracode);
                if (track == null)
                {
                    result = string.Format("找不到指令中定位轨道编号[ {0} ]相关轨道数据", tracode);
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

            if (track.NotInType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
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
                        task.DoResetSiteByPoint(rfid, site);
                    }
                }
                catch { }
            })
            {
                IsBackground = true
            }.Start();

        }

        /// <summary>
        /// 初始化运输车位置
        /// </summary>
        public bool DoReNew(uint devid, ushort point, ushort code, DevMoveDirectionE md, out string res)
        {
            res = "";
            CarrierTask task = DevList.Find(c => c.ID == devid);
            if (task == null)
            {
                res = "无小车数据";
                return false;
            }

            if (!CheckCarrierIsFree(task))
            {
                res = "小车非空闲状态，请先终止并查看是否停用";
                return false;
            }

            if (code == 0)
            {
                res = "请选择初始化轨道";
                return false;
            }

            if (point == 0)
            {
                res = "请选择初始化复位点位";
                return false;
            }

            if (md != DevMoveDirectionE.前 && md != DevMoveDirectionE.后)
            {
                res = "请选择指令方向";
                return false;
            }

            if (task.IsResetWriting)
            {
                res = "初始化中，请5秒后再操作";
                return false;
            }

            List<CarrierPos> posList = PubMaster.Track.GetCarrierPosList(task.AreaId);
            if (posList == null || posList.Count == 0 || posList.Exists(c => c.track_point == point && c.track_pos == 0))
            {
                res = "没有复位点脉冲数据";
                return false;
            }

            Track track = PubMaster.Track.GetTrackBySite((ushort)task.AreaId, code);
            if (track != null && !IsAddTrafficControl(task, track, out string msg))
            {
                res = msg;
                return false;
            }

            try
            {
                task.IsResetWriting = true;
                foreach (CarrierPos item in posList)
                {
                    try
                    {
                        if (item.track_pos != 0)
                        {
                            task.DoResetSiteByPoint(item.track_point, item.track_pos);
                            Thread.Sleep(500);
                        }
                    }
                    catch (Exception ex)
                    {
                        res = "初始化异常：" + ex.ToString();
                        return false;
                    }
                }

                Thread.Sleep(500);
                task.DoRenew(point, code, md == DevMoveDirectionE.前 ? CarrierResetE.前进初始化 : CarrierResetE.后退初始化);
            }
            finally
            {
                task.IsResetWriting = false;
            }

            return true;
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
                        case TransTypeE.同向上砖:
                            IsGetCarrier = GetTransInOutCarrier(trans, DeviceTypeE.后摆渡, out carrierid, out result, ferryids);
                            break;
                        case TransTypeE.上砖任务:
                        case TransTypeE.手动上砖:
                        case TransTypeE.同向下砖:
                        case TransTypeE.反抛任务:
                            IsGetCarrier = GetTransInOutCarrier(trans, DeviceTypeE.前摆渡, out carrierid, out result, ferryids);
                            break;
                        case TransTypeE.上砖接力:
                        case TransTypeE.倒库任务:
                            IsGetCarrier = GetTransSortCarrier(trans, out carrierid, out result);
                            break;
                        case TransTypeE.库存转移:
                            IsGetCarrier = GetTransCarrier(trans, out carrierid, out result);
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
        /// 分配任务运输车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="carrierid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool GetTransCarrier(StockTrans trans, out uint carrierid, out string result)
        {
            result = "";
            carrierid = 0;
            if (trans.goods_id == 0) return false;

            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);

            // 是否选轨道内脉冲大的小车
            bool isbig = trans.AllocateFerryType == DeviceTypeE.前摆渡;

            CarrierTask carrier = null;

            #region 1. 直接取摆渡上的小车
            if (carrier == null)
            {
                //1. 获取能到达[取货/卸货]轨道的摆渡车的ID
                List<uint> ferrytrackids = PubMaster.Area.GetFerryWithTrackInOut(trans.AllocateFerryType, trans.area_id, trans.take_track_id, trans.give_track_id, 0, true);
                bool isOnlyOneFerry = ferrytrackids.Count == 1; // 是否只有唯一一台摆渡车可使用

                List<uint> loadcarferryids = new List<uint>();
                foreach (uint fetraid in ferrytrackids)
                {
                    uint fid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(fetraid);
                    if (PubTask.Ferry.IsLoad(fid))
                    {
                        loadcarferryids.Add(fetraid);
                    }
                }

                //2. 获取在摆渡车上的车
                List<CarrierTask> carriers = DevList.FindAll(c => loadcarferryids.Contains(c.CurrentTrackId));
                if (carriers.Count > 0)
                {
                    string err = "";
                    //如何判断哪个摆渡车最右
                    foreach (CarrierTask car in carriers)
                    {
                        //小车:没有任务绑定
                        if (!PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            //空闲
                            if (CheckCarrierIsFree(car))
                            {
                                if (!car.DevConfig.IsUseGoodsSize(goodssizeID))
                                {
                                    err = string.Format("{0}[{1}]不负责{2}的砖; ", err, car.Device.name, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                                    continue;
                                }

                                if (car.IsLoad())
                                {
                                    //摆渡车上的车载库存和任务对应的库存品种不符
                                    uint sgid = PubMaster.Goods.GetStockGoodId(car.DevConfig.stock_id);
                                    if (sgid != 0 && sgid != trans.goods_id)
                                    {
                                        err = string.Format("{0}[{1}]载着与任务品种不符的砖; ", err, car.Device.name);
                                        continue;
                                    }
                                }

                                carrierid = car.ID;
                                return true;
                            }

                            if (isOnlyOneFerry)
                            {
                                err = string.Format("[{1}]设备状态不满足; {0}", err, car.Device.name);
                            }

                        }
                    }

                    if (isOnlyOneFerry && !string.IsNullOrEmpty(err))
                    {
                        result = string.Format("任务ID[{0}]: {1}-运输车需满足条件：[启用] [通讯正常] [停止] [无指令] [能取{2}的砖] [没有被分配到其他任务]",
                                    trans.id, err, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                        return false;
                    }
                }
            }

            #endregion

            #region 2. 取货轨道以中线为分割点靠近摆渡坑侧的小车
            if (carrier == null)
            {
                carrier = GetOnlyCarrierInTrack(trans.take_track_id, isbig);
                if (carrier != null)
                {
                    result = string.Format("取货轨道上有运输车{0}：", carrier.Device.name);
                    if (!carrier.CheckCarrierIsUsable(goodssizeID, out string mes))
                    {
                        result += mes;
                        return false;
                    }

                    carrierid = carrier.ID;
                    return true;
                }

            }
            #endregion

            #region 3. 卸货轨道以中线为分割点靠近摆渡坑侧的小车
            if (carrier == null)
            {
                carrier = GetOnlyCarrierInTrack(trans.give_track_id, isbig);
                if (carrier != null)
                {
                    result = string.Format("卸货轨道上有运输车{0}：", carrier.Device.name);
                    if (!carrier.CheckCarrierIsUsable(goodssizeID, out string mes))
                    {
                        result += mes;
                        return false;
                    }

                    carrierid = carrier.ID;
                    return true;
                }

            }
            #endregion

            #region 4. 其他轨道
            if (carrier == null)
            {
                List<uint> trackids;

                // 1. 其他轨道靠近摆渡坑的无货车
                List<CarrierTask> List_1 = new List<CarrierTask>();
                // 2. 其他轨道靠近摆渡坑的有货车
                List<CarrierTask> List_2 = new List<CarrierTask>();

                // 3. 卸货轨道的车
                CarrierTask car3 = GetOnlyCarrierInTrack(trans.give_track_id, isbig, false);

                // 4. 靠近卸货轨道的无货车
                List<CarrierTask> List_4 = new List<CarrierTask>();
                // 5. 靠近卸货轨道的有货车
                List<CarrierTask> List_5 = new List<CarrierTask>();

                // 总计
                List<CarrierTask> totalList = new List<CarrierTask>();

                if (trans.tilelifter_id > 0)
                {
                    bool isup = trans.InType(TransTypeE.上砖任务, TransTypeE.手动上砖, TransTypeE.同向上砖);
                    // 存在任务砖机则获取相关轨道
                    if (trans.AllocateFerryType == DeviceTypeE.前摆渡)
                    {
                        trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, isup, TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.上砖轨道);
                    }
                    else
                    {
                        trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, isup, TrackTypeE.储砖_入, TrackTypeE.储砖_出入);
                    }
                }
                else
                {
                    // 获取同区域线内轨道
                    trackids = PubMaster.Track.GetAreaAllTrackIdList(trans.area_id, trans.line);
                }

                // 按离卸货点排序
                List<Track> tids = PubMaster.Track.GetSortTracks(trackids, trans.give_track_id);

                //能去这个取货/卸货轨道的所有配置的摆渡车信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(trans.AllocateFerryType, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                foreach (Track tra in tids)
                {
                    //是否有能够到达该轨道的摆渡车
                    if (!PubMaster.Area.ExistFerryWithTrack(ferryids, tra.id))
                    {
                        continue;
                    }

                    CarrierTask car = GetOnlyCarrierInTrack(tra.id, isbig, false);
                    if (car == null) continue;

                    // 是否靠近摆渡坑
                    bool isNearFerry = isbig ? car.CurrentPoint >= tra.split_point : car.CurrentPoint <= tra.split_point;

                    if (isNearFerry)
                    {
                        // 1. 其他轨道靠近摆渡坑的无货车
                        if (car.IsNotLoad())
                        {
                            List_1.Add(car);
                            continue;
                        }

                        // 2. 其他轨道靠近摆渡坑的有货车
                        if (car.IsLoad())
                        {
                            List_2.Add(car);
                            continue;
                        }
                    }
                    else
                    {
                        // 4. 靠近卸货轨道的无货车
                        if (car.IsNotLoad())
                        {
                            List_4.Add(car);
                            continue;
                        }

                        // 5. 靠近卸货轨道的有货车
                        if (car.IsLoad())
                        {
                            List_5.Add(car);
                            continue;
                        }
                    }

                }

                if (List_1 != null && List_1.Count > 0) totalList.AddRange(List_1);
                if (List_2 != null && List_2.Count > 0) totalList.AddRange(List_2);

                if (car3 != null) totalList.Add(car3);

                if (List_4 != null && List_4.Count > 0) totalList.AddRange(List_4);
                if (List_5 != null && List_5.Count > 0) totalList.AddRange(List_5);

                if (totalList == null || totalList.Count == 0)
                {
                    result = string.Format("任务ID[{0}]中配置的相关轨道内无任何运输车", trans.id);
                    return false;
                }

                result = string.Format("任务ID[{0}]分配小车失败：", trans.id);
                foreach (CarrierTask car in totalList)
                {
                    if (!car.CheckCarrierIsUsable(goodssizeID, out string mes))
                    {
                        result += mes;
                        continue;
                    }

                    carrierid = car.ID;
                    return true;
                }

                return false;
            }

            #endregion

            return false;
        }

        /// <summary>
        /// 同轨道内获取唯一运输车
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="isbig"></param>
        /// <returns></returns>
        private CarrierTask GetOnlyCarrierInTrack(uint trackid, bool isbig, bool isSplit = true)
        {
            CarrierTask carrier = null;
            // 获取轨道内运输车
            List<CarrierTask> cars = DevList.FindAll(c => c.CurrentTrackId == trackid);
            if (cars.Count > 0)
            {
                // 按脉冲从小到大排序
                cars.Sort((x, y) => x.CurrentPoint.CompareTo(y.CurrentPoint));
                CarrierTask CarMin = cars[0];
                CarrierTask CarMax = cars[(cars.Count - 1)];

                // 获取轨道
                Track track = PubMaster.Track.GetTrack(trackid);
                if (track.InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道))
                {
                    bool isback = track.ferry_up_code > 500; // 轨道编号 > 500 只能后退了
                    // 后退-脉冲最小的；前进-脉冲最大的
                    carrier = isback ? CarMin : CarMax;
                }
                else
                {
                    if (isSplit)    // 是否按轨道分段分配
                    {
                        if (isbig)
                        {
                            // 超过中间点  前进-脉冲最大的
                            if (CarMax.CurrentPoint > track.split_point || CarMax.TargetPoint > track.split_point) carrier = CarMax;
                        }
                        else
                        {
                            // 超过中间点  后退-脉冲最小的
                            if (CarMin.CurrentPoint < track.split_point || CarMin.TargetPoint < track.split_point) carrier = CarMin;
                        }
                    }
                    else
                    {
                        carrier = isbig ? CarMax : CarMin;
                    }

                }
            }

            return carrier;
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

            #region 1.倒库空轨道是否有车[空闲，无货
            CarrierTask carrier = DevList.Find(c => c.CurrentTrackId == trans.give_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
            #endregion

            #region 2.满砖轨道是否有车[执行过倒库]
            if (carrier == null)
            {
                CarrierTask inCar = DevList.Find(c => c.CurrentTrackId == trans.take_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));

                if (inCar != null 
                    && inCar.ConnStatus == SocketConnectStatusE.通信正常
                    && inCar.OperateMode == DevOperateModeE.自动
                    && inCar.InTask(DevCarrierOrderE.倒库指令))
                {
                    carrierid = inCar.ID;
                    return true;
                }
            }
            #endregion

            #region 3.摆渡车上是否有车[空闲，无货]
            if (carrier == null)
            {
                //3.1获取能到达[空轨道]轨道的上砖摆渡车的轨道ID
                List<uint> ferrytrackids = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.前摆渡, trans.area_id, 0, trans.give_track_id, 0, true);
                bool isOnlyOneFerry = ferrytrackids.Count == 1; // 是否只有唯一一台摆渡车可使用

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

                            if (isOnlyOneFerry)
                            {
                                result = string.Format("任务ID[{0}]: [{1}]设备状态不满足-运输车需满足条件：[启用] [通讯正常] [停止] [无指令] [能取{2}的砖] [没有被分配到其他任务]",
                                            trans.id, car.Device.name, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                                return false;
                            }
                        }
                    }
                }
            }

            #endregion

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
                //优先级：
                // 1：同侧无砖
                List<CarrierTask> allocate_cars_1 = new List<CarrierTask>();
                // 2：同侧载砖
                List<CarrierTask> allocate_cars_2 = new List<CarrierTask>();
                // 3：远测无砖
                List<CarrierTask> allocate_cars_3 = new List<CarrierTask>();
                // 4：远测载砖
                List<CarrierTask> allocate_cars_4 = new List<CarrierTask>();

                // 获取任务所有可作业轨道
                List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                // 按离取货点近远排序
                List<Track> tids = PubMaster.Track.GetSortTracks(trackids, trans.give_track_id);

                //能去这个倒库轨道所有配置的摆渡车轨道信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(DeviceTypeE.前摆渡, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                string carNames = "";

                foreach (Track tra in tids)
                {
                    if (!tra.IsStoreTrack()) continue;
                    List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == tra.id);
                    if (tasks.Count > 0)
                    {
                        // 每条轨道只拿一车出来加以选择
                        CarrierTask tracar = null;
                        ushort dis = 0;
                        foreach (CarrierTask item in tasks)
                        {
                            if (item == null) continue;

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

                        if (tracar == null || dis == 0) continue;
                        carNames += string.Format("[{0}]", tracar.Device.name);
                        // 上砖侧的RFID位数 [3XX99,3XX98,3XX96,3XX94]
                        //if (tracar.CurrentSite % 100 > 90)
                        // 以轨道中间点判断？
                        if (tracar.CurrentPoint >= tra.split_point)
                        {
                            if (tracar.IsNotLoad())
                            {
                                allocate_cars_1.Add(tracar);
                            }
                            else
                            {
                                allocate_cars_2.Add(tracar);
                            }
                        }
                        else
                        {
                            if (tracar.IsNotLoad())
                            {
                                allocate_cars_3.Add(tracar);
                            }
                            else
                            {
                                allocate_cars_4.Add(tracar);
                            }
                        }
                    }
                }

                // 总计
                List<CarrierTask> totalList = new List<CarrierTask>();
                if (allocate_cars_1 != null && allocate_cars_1.Count > 0) totalList.AddRange(allocate_cars_1);
                if (allocate_cars_2 != null && allocate_cars_2.Count > 0) totalList.AddRange(allocate_cars_2);
                if (allocate_cars_3 != null && allocate_cars_3.Count > 0) totalList.AddRange(allocate_cars_3);
                if (allocate_cars_4 != null && allocate_cars_4.Count > 0) totalList.AddRange(allocate_cars_4);

                if (totalList != null && totalList.Count > 0)
                {
                    foreach (CarrierTask car in totalList)
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
                    result = string.Format("任务ID[{0}]: {1}线路分配的储砖轨道里没有符合状态的运输车，运输车需满足条件：【启用】【通讯正常】【停止】【无指令】【能取{2}的砖】【没有被分配到其他任务】",
                                trans.id, PubMaster.Area.GetLineName(trans.area_id, trans.line), PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                }
                else
                {
                    result = string.Format("任务ID[{0}]: {1}运输车不符合状态，不能分配，运输车需满足条件：【启用】【通讯正常】【停止】【无指令】【能取{2}的砖】【没有被分配到其他任务】",
                                trans.id, carNames, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                }
            }

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
                    result = "[" + carrier.Device.name + "]运输车已停用！";
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

                if (CheckCarrierIsFree(carrier))
                {
                    carrierid = carrier.ID;
                    return true;
                }
            }

            if (carrier != null)
            {
                result = string.Format("取/卸货轨道上有运输车{0}，但运输车不符合状态，不能分配，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                       carrier.Device.name, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
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
                    List<uint> ids = PubMaster.Track.SortTrackIdsWithOrder(tracids, tiletrackid, true, false);
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

            #region[2.先找砖机轨道的空闲运输车]

            if (carrier == null)
            {
                //获取当前空闲摆渡车对上的轨道
                List<uint> ferryintrack = PubTask.Ferry.GetInTracks(fids);
                uint neartileferrytrackid = GetNearTileFerryOnTrack(ferryintrack, trans.give_track_id, trans.take_track_id);

                //所有上砖轨道
                List<uint> uptiletraids = PubMaster.Track.GetUpTileTracks(trans.area_id, (ferrytype == DeviceTypeE.后摆渡 ? TrackTypeE.下砖轨道 : TrackTypeE.上砖轨道));

                // 按靠近砖机的摆渡车所对轨道进行排序
                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(uptiletraids, neartileferrytrackid, (uptiletraids.Contains(neartileferrytrackid)), false);

                //能去这个取货/卸货轨道的所有配置的摆渡车信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                string carNames = "";
                // 1：同侧无砖
                List<CarrierTask> allocate_cars_1 = new List<CarrierTask>();
                // 2：同侧载砖
                List<CarrierTask> allocate_cars_2 = new List<CarrierTask>();

                List<uint> ntids = new List<uint>();
                List<uint> tiletrasid = PubMaster.DevConfig.GetTileTracks(trans.tilelifter_id);

                foreach (var item in tiletrasid)
                {
                    if (tids.Exists(c => c.Equals(item)))
                    {
                        ntids.Add(item);
                    }
                }
                ntids.AddRange(tids);

                foreach (uint traid in ntids)
                {
                    List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == traid);
                    if (tasks.Count > 0)
                    {
                        // 每条轨道只拿一车出来加以选择
                        CarrierTask tracar = null;
                        ushort dis = 0;
                        foreach (CarrierTask item in tasks)
                        {
                            if (item == null) continue;

                            //是否有能够到达该轨道的摆渡车
                            if (!PubMaster.Area.ExistFerryWithTrack(ferryids, item.CurrentTrackId))
                            {
                                continue;
                            }

                            if (trans.TransType == TransTypeE.同向上砖)
                            {
                                // 需要小车前进作业的，以最大脉冲为准
                                if (dis == 0 || dis < item.CurrentPoint)
                                {
                                    tracar = item;
                                    dis = item.CurrentPoint;
                                }
                            }
                            else
                            {
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
                            allocate_cars_1.Add(tracar);
                        }
                        else
                        {
                            allocate_cars_2.Add(tracar);
                        }
                    }
                }

                // 总计
                List<CarrierTask> totalList = new List<CarrierTask>();
                if (allocate_cars_1 != null && allocate_cars_1.Count > 0) totalList.AddRange(allocate_cars_1);
                if (allocate_cars_2 != null && allocate_cars_2.Count > 0) totalList.AddRange(allocate_cars_2);

                if (totalList != null && totalList.Count > 0)
                {
                    foreach (CarrierTask car in totalList)
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

            #region [3.取货轨道是否有车]
            if (carrier == null)
            {
                //入库任务 -> 砖机轨道
                //出库任务 -> 储砖出，出入
                List<CarrierTask> taketrackcarriers = DevList.FindAll(c => c.CurrentTrackId == trans.take_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
                if (taketrackcarriers.Count > 0)
                {
                    switch (ferrytype)
                    {
                        case DeviceTypeE.前摆渡:
                            //脉冲大的排在前面
                            taketrackcarriers.Sort((x, y) => y.CurrentPoint.CompareTo(x.CurrentPoint));
                            break;
                        case DeviceTypeE.后摆渡:
                            //脉冲小的排在前面
                            taketrackcarriers.Sort((x, y) => x.CurrentPoint.CompareTo(y.CurrentPoint));
                            break;
                    }
                    carrier = taketrackcarriers[0];

                    if (carrier != null
                        && !carrier.IsNotDoingTask
                        && carrier.InTask(DevCarrierOrderE.倒库指令))
                    {
                        carrier = null;
                    }

                    if (carrier != null
                        && carrier.IsNotDoingTask
                        && PubTask.Trans.IsCarrierInTrans(carrier.ID, trans.take_track_id, TransTypeE.上砖接力, TransTypeE.倒库任务))
                    {
                        carrier = null;
                    }

                    //无任务，不做放砖任务
                    if (carrier != null
                        && !PubTask.Trans.HaveInCarrier(carrier.ID)
                        && carrier.NotInTask(DevCarrierOrderE.放砖指令))
                    {
                        //取砖任务
                        if (CheckIsConnInTask(carrier, DevCarrierOrderE.取砖指令))
                        {
                            carrierid = carrier.ID;
                            return true;
                        }

                        //在出轨道头空闲
                        if (CheckCarrierIsFree(trans, carrier, false, out carrierid, out result, out returnfalse)
                           // && ((trans.InType(TransTypeE.同向上砖) && carrier.CurrentPoint <= PubMaster.Track.GetCarrierPos(carrier.AreaId, CarrierPosE.轨道后侧复位点))
                            //    ||  carrier.CurrentPoint >= PubMaster.Track.GetCarrierPos(carrier.AreaId, CarrierPosE.轨道前侧复位点))
                           )
                        {
                            carrierid = carrier.ID;
                            return true;
                        }
                    }

                    carrier = null;
                }
            }
            #endregion

            #region [4.前面找到车了，如果空闲则分配，否则等待]

            if (carrier != null)
            {
                if (!carrier.IsWorking)
                {
                    result = "[" + carrier.Device.name + "]运输车已停用！";
                    mlog.Status(true, string.Format("小车已停用-跳过分配\n" +
                        "小车：{0}\n" +
                        "任务：{1}", carrier.DevStatus.ToString(), trans.ToString()));
                    carrier = null; // 继续往下找
                                    //return false;
                }
                else
                {
                    if (CheckIsConnFree(carrier))
                    {
                        carrierid = carrier.ID;
                        return true;
                    }

                    if (CheckCarrierIsFree(carrier))
                    {
                        carrierid = carrier.ID;
                        return true;
                    }
                }

                if (carrier != null)
                {
                    result = string.Format("取/卸货轨道上有运输车{0}，但运输车不符合状态，不能分配，分配条件：【启用】【通讯正常】【停止】【任务完成】【能取{1}的砖】【没有被分配到其他任务】",
                                           carrier.Device.name, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
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

            bool isup = trans.InType(TransTypeE.上砖任务, TransTypeE.手动上砖, TransTypeE.同向上砖);

            CarrierTask carrier = null;

            #region 绝对优先 - 摆渡车上

            if (carrier == null)
            {
                //1. 获取能到达[取货/卸货]轨道的摆渡车的ID
                List<uint> ferrytrackids = PubMaster.Area.GetFerryWithTrackInOut(ferrytype, trans.area_id, trans.take_track_id, trans.give_track_id, 0, true);
                bool isOnlyOneFerry = ferrytrackids.Count == 1; // 是否只有唯一一台摆渡车可使用

                List<uint> loadcarferryids = new List<uint>();
                foreach (uint fetraid in ferrytrackids)
                {
                    uint fid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(fetraid);
                    if (PubTask.Ferry.IsLoad(fid))
                    {
                        loadcarferryids.Add(fetraid);
                    }
                }

                //2. 获取在摆渡车上的车
                List<CarrierTask> carriers = DevList.FindAll(c => loadcarferryids.Contains(c.CurrentTrackId) && c.DevConfig.IsUseGoodsSize(goodssizeID));
                if (carriers.Count > 0)
                {
                    string err = "";
                    //如何判断哪个摆渡车最右
                    foreach (CarrierTask car in carriers)
                    {
                        //小车:没有任务绑定
                        if (!PubTask.Trans.HaveInCarrier(car.ID))
                        {
                            //空闲
                            if (CheckCarrierIsFree(car))
                            {
                                if (car.IsLoad())
                                {
                                    //摆渡车上的车载库存和任务对应的库存品种不符
                                    uint sgid = PubMaster.Goods.GetStockGoodId(car.DevConfig.stock_id);
                                    if (sgid != 0 && sgid != trans.goods_id)
                                    {
                                        err = string.Format("{0}[{1}]载着与任务品种不符的砖; ", err, car.Device.name);
                                        break;
                                    }
                                }
                                carrierid = car.ID;
                                return true;
                            }

                            if (isOnlyOneFerry)
                            {
                                err = string.Format("{0}[{1}]设备状态不满足; ", err, car.Device.name);
                            }

                        }
                    }

                    if (isOnlyOneFerry && !string.IsNullOrEmpty(err))
                    {
                        result = string.Format("任务ID[{0}]: {1}-运输车需满足条件：[启用] [通讯正常] [停止] [无指令] [能取{2}的砖] [没有被分配到其他任务]",
                                    trans.id, err, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                        return false;
                    }
                }
            }

            #endregion

            if (GlobalWcsDataConfig.BigConifg.IsUpTaskNewAllocate(trans.area_id, trans.line) && isup)
            {
                bool isallocate = GetTransOutCarrier(trans, ferrytype, goodssizeID, out carrierid, out result, out bool returnfalse, fids);
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
                if (carrier == null)
                {
                    //入库任务 -> 砖机轨道
                    //出库任务 -> 储砖出，出入
                    List<CarrierTask> taketrackcarriers = DevList.FindAll(c => c.CurrentTrackId == trans.take_track_id && c.DevConfig.IsUseGoodsSize(goodssizeID));
                    if (taketrackcarriers.Count > 0)
                    {
                        switch (ferrytype)
                        {
                            case DeviceTypeE.前摆渡:
                                //脉冲大的排在前面
                                taketrackcarriers.Sort((x, y) => y.CurrentPoint.CompareTo(x.CurrentPoint));
                                break;
                            case DeviceTypeE.后摆渡:
                                //脉冲小的排在前面
                                taketrackcarriers.Sort((x, y) => x.CurrentPoint.CompareTo(y.CurrentPoint));
                                break;
                        }
                        carrier = taketrackcarriers[0];
                        if (trans.InType(TransTypeE.上砖任务, TransTypeE.同向上砖))
                        {
                            if (!carrier.IsNotDoingTask
                                && carrier.InTask(DevCarrierOrderE.倒库指令))
                            {
                                carrier = null;
                            }

                            if (carrier != null
                                && carrier.IsNotDoingTask
                                && PubTask.Trans.IsCarrierInTrans(carrier.ID, trans.take_track_id, TransTypeE.上砖接力, TransTypeE.倒库任务))
                            {
                                carrier = null;
                            }
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

            #region [4.前面找到车了，如果空闲则分配，否则等待]
            if (carrier != null)
            {
                if (!carrier.IsWorking)
                {
                    result = "[" + carrier.Device.name + "]运输车已停用！";
                    mlog.Status(true, string.Format("小车已停用-跳过分配\n" +
                        "小车：{0}\n" +
                        "任务：{1}", carrier.DevStatus.ToString(), trans.ToString()));
                    carrier = null; // 继续往下找
                }
                else
                {
                    if (carrier.ConnStatus == SocketConnectStatusE.通信正常
                        && carrier.OperateMode == DevOperateModeE.自动
                        && !PubTask.Trans.HaveInCarrier(carrier.ID))
                    {
                        carrierid = carrier.ID;
                        return true;
                    }
                }

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
                            result = "[" + carrier.Device.name + "]运输车已停用！";
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

                            if (CheckCarrierIsFree(carrier))
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
                                           carrier.Device.name, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                }
            }

            #endregion

            #region [5.找其他轨道]
            if (carrier == null)
            {
                // 1：砖机上的无砖运输车
                List<CarrierTask> allocate_cars_1 = new List<CarrierTask>();
                // 2：同侧无砖
                List<CarrierTask> allocate_cars_2 = new List<CarrierTask>();
                // 3：同侧载砖
                List<CarrierTask> allocate_cars_3 = new List<CarrierTask>();
                // 4：远测无砖
                List<CarrierTask> allocate_cars_4 = new List<CarrierTask>();
                // 5：远测载砖
                List<CarrierTask> allocate_cars_5 = new List<CarrierTask>();

                // 获取任务砖机所有可作业轨道
                List<uint> trackids; //= PubMaster.Area.GetTileTrackIds(trans);
                if (ferrytype == DeviceTypeE.前摆渡)
                {
                    trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, isup, TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.上砖轨道);
                }
                else
                {
                    trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, isup, TrackTypeE.储砖_入, TrackTypeE.储砖_出入);
                }

                // 按离取货点近远排序
                List<Track> tids = PubMaster.Track.GetSortTracks(trackids, trans.take_track_id);

                //能去这个取货/卸货轨道的所有配置的摆渡车信息
                List<uint> ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                string carNames = "";

                foreach (Track tra in tids)
                {
                    if (!tra.IsStoreTrack())
                    {
                        //如果不是上砖机轨道或者不是上砖任务，就下一条轨道
                        if (!(tra.Type == TrackTypeE.上砖轨道 && trans.InType(TransTypeE.上砖任务, TransTypeE.手动上砖, TransTypeE.同向上砖)))
                        {
                            continue;
                        }
                    }

                    List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == tra.id);
                    if (tasks.Count > 0)
                    {
                        // 每条轨道只拿一车出来加以选择
                        CarrierTask tracar = null;
                        ushort dis = 0;
                        bool isUp = false;
                        foreach (CarrierTask item in tasks)
                        {
                            if (item == null) continue;

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

                        if (tracar == null || dis == 0) continue;
                        carNames += string.Format("[{0}]", tracar.Device.name);
                        if (isUp)
                        {
                            if (PubMaster.Track.IsTrackType(tracar.CurrentTrackId, TrackTypeE.上砖轨道))
                            {
                                allocate_cars_1.Add(tracar);
                            }
                            // 上砖侧的RFID位数 [3XX99,3XX98,3XX96,3XX94]
                            //else if (tracar.CurrentSite % 100 > 90)
                            // 以轨道中间点判断？
                            else if (tracar.CurrentPoint >= tra.split_point)
                            {
                                if (tracar.IsNotLoad())
                                {
                                    allocate_cars_2.Add(tracar);
                                }
                                else
                                {
                                    allocate_cars_3.Add(tracar);
                                }
                            }
                            else
                            {
                                if (tracar.IsNotLoad())
                                {
                                    allocate_cars_4.Add(tracar);
                                }
                                else
                                {
                                    allocate_cars_5.Add(tracar);
                                }
                            }
                        }
                        else
                        {
                            // 下砖侧的RFID位数 [3XX06,3XX04,3XX02,3XX00]
                            //if (tracar.CurrentSite % 100 < 10)
                            // 以轨道中间点判断？
                            if (tracar.CurrentPoint <= tra.split_point)
                            {
                                if (tracar.IsNotLoad())
                                {
                                    allocate_cars_2.Add(tracar);
                                }
                                else
                                {
                                    allocate_cars_3.Add(tracar);
                                }
                            }
                            else
                            {
                                if (tracar.IsNotLoad())
                                {
                                    allocate_cars_4.Add(tracar);
                                }
                                else
                                {
                                    allocate_cars_5.Add(tracar);
                                }
                            }
                        }
                    }

                }

                // 总计
                List<CarrierTask> totalList = new List<CarrierTask>();
                if (allocate_cars_1 != null && allocate_cars_1.Count > 0) totalList.AddRange(allocate_cars_1);
                if (allocate_cars_2 != null && allocate_cars_2.Count > 0) totalList.AddRange(allocate_cars_2);
                if (allocate_cars_3 != null && allocate_cars_3.Count > 0) totalList.AddRange(allocate_cars_3);
                if (allocate_cars_4 != null && allocate_cars_4.Count > 0) totalList.AddRange(allocate_cars_4);
                if (allocate_cars_5 != null && allocate_cars_5.Count > 0) totalList.AddRange(allocate_cars_5);

                if (totalList != null && totalList.Count > 0)
                {
                    foreach (CarrierTask car in totalList)
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
                                trans.id, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id));
                }
                else
                {
                    result = string.Format("任务ID[{2}]分配的运输车[{0}]不符合状态，不能分配，分配条件：[启用] [通讯正常] [停止] [指令完成] [能取{1}的砖] [没有被分配到其他任务]",
                                carNames, PubMaster.Goods.GetGoodsSizeNameByGid(trans.goods_id), trans.id);
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
        /// 小车当前是否空闲（停止无指令）
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

            bool isup = trans.InType(TransTypeE.上砖任务, TransTypeE.手动上砖, TransTypeE.同向上砖);

            // 优先车：同侧无砖
            List<CarrierTask> first_allocate_cars = new List<CarrierTask>();
            // 次级车：同侧载砖
            List<CarrierTask> second_allocate_cars = new List<CarrierTask>();
            // 末级车：远测无砖
            List<CarrierTask> third_allocate_cars = new List<CarrierTask>();
            // 末级车：远测载砖
            List<CarrierTask> fourth_allocate_cars = new List<CarrierTask>();
            List<uint> trackids;
            List<Track> tracks;
            // 获取任务区域所有可作业轨道
            if (ferrytype == DeviceTypeE.前摆渡)
            {
                trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, isup, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
            }
            else
            {
                trackids = PubMaster.Track.GetAreaLineAndTileTrack(trans.area_id, trans.line, trans.tilelifter_id, isup, TrackTypeE.储砖_入, TrackTypeE.储砖_出入);
            }
            // 按离取货点近远排序    tids
            //能去这个取货/卸货轨道的所有配置的摆渡车信息    ferryids
            if (checktakegivetrack)
            {
                tracks = PubMaster.Track.GetSortTracks(trackids, trans.take_track_id);
                ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.take_track_id, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);
            }
            else
            {
                tracks = PubMaster.Track.GetSortTracks(trackids, trans.give_track_id);
                ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trans.give_track_id);
                ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);
            }
            tids = tracks?.Select(c => c.id).ToList() ?? new List<uint>();
            string carNames = "";

            foreach (Track tra in tracks)
            {
                if (!tra.IsStoreTrack()) continue;
                List<CarrierTask> tasks = DevList.FindAll(c => c.CurrentTrackId == tra.id);
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
                        //if (tracar.CurrentSite % 100 > 90)
                        if (tracar.CurrentPoint >= tra.split_point)
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
                        //if (tracar.CurrentSite % 100 < 10)
                        if (tracar.CurrentPoint <= tra.split_point)
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
                                    && c.IsNotDoingTask
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
                                    && c.IsNotDoingTask
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
                                    && (Orders.Contains(c.CurrentOrder) || Orders.Contains(c.OnGoingOrder))
                                    );
        }

        /// <summary>
        /// 判断小车是否已完成该指令
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        internal bool IsCarrierFinishTask(uint carrier_id, params DevCarrierOrderE[] Order)
        {
            return DevList.Exists(c => c.ID == carrier_id
                                    && c.ConnStatus == SocketConnectStatusE.通信正常
                                    && c.OperateMode == DevOperateModeE.自动
                                    && c.Status == DevCarrierStatusE.停止
                                    && c.IsNotDoingTask
                                    && Order.Contains(c.FinishOrder)
                                    );
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
            if (carpoint > 0)
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

        /// <summary>
        /// 判断是否有小车在同一个轨道并且位置在指定小车的后面(脉冲值更小)
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool ExistCarBehind(uint carrier_id, uint trackid, out uint otherid)
        {
            ushort carpoint = GetCurrentPoint(carrier_id);
            if (carpoint > 0)
            {
                otherid = DevList.Find(c => c.CurrentTrackId == trackid && c.ID != carrier_id && c.CurrentPoint < carpoint)?.ID ?? 0;
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
            return DevList.Exists(c => c.ID != carrier_id && (c.TargetTrackId == track_id || c.OnGoingTrackId == track_id));
        }


        /// <summary>
        /// 是否存在阻碍移动的小车
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <param name="point"></param>
        /// <param name="isforwardTake"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool ExistLocateObstruct(uint carrier_id, uint track_id, ushort point, bool isforwardTake, out CarrierTask otherCar)
        {
            otherCar = DevList.Find(c => c.ID != carrier_id
               && (c.CurrentTrackId == track_id || c.TargetTrackId == track_id || c.OnGoingTrackId == track_id)
               && (c.CurrentPoint > 0 && (isforwardTake ? (c.CurrentPoint < point) : (c.CurrentPoint > point)))
                );
            return otherCar != null;
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
                // 获取轨道内靠近出库口库存
                Stock topSTK = PubMaster.Goods.GetStockForOut(track_id);
                if (stockid != 0 && topSTK.goods_id == stockid)
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
            return onecar != null ? DevList.Exists(c => c.ID == two_car_id && (c.DevStatus.DeviceStatus == DevCarrierStatusE.后退 || c.DevStatus.DeviceStatus == DevCarrierStatusE.停止)) : false;
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
                        case TrackTypeE.上砖轨道:
                            // 后摆渡复位脉冲默认是 1000，暂定义 0~2000 内属 后摆渡
                            if (currenttrack.limit_point_up <= 2000)
                            {
                                return DeviceTypeE.后摆渡;
                            }
                            return DeviceTypeE.前摆渡;

                        case TrackTypeE.后置摆渡轨道:
                        case TrackTypeE.储砖_入:
                            return DeviceTypeE.后摆渡;

                        case TrackTypeE.前置摆渡轨道:
                        case TrackTypeE.储砖_出:
                            return DeviceTypeE.前摆渡;

                        case TrackTypeE.储砖_出入:
                            int downdis = Math.Abs(task.CurrentPoint - currenttrack.limit_point);
                            int updis = Math.Abs(task.CurrentPoint - currenttrack.limit_point_up);
                            if (downdis < updis)
                            {
                                return DeviceTypeE.后摆渡;
                            }
                            return DeviceTypeE.前摆渡;
                    }
                }
            }

            return DeviceTypeE.其他;
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
                        case TransTypeE.同向下砖:
                            carrier = DevList.Find(c => c.ID != carrierid
                                                && c.CurrentTrackId == trackid
                                                    //&& (c.CurrentSite == track.rfid_1
                                                    //    || (c.CurrentSite == track.rfid_2 && c.InTask(DevCarrierOrderE.放砖指令)))
                                                    );
                            if (carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
                            break;
                        case TransTypeE.上砖任务:
                        case TransTypeE.同向上砖:
                            //除了倒库任务的运输车
                            carrier = DevList.Find(c => c.ID != carrierid
                               && c.CurrentTrackId == track.id
                               && c.NotInTask(DevCarrierOrderE.倒库指令)
                               && !PubTask.Trans.IsCarrierInTrans(c.ID, trackid, TransTypeE.上砖接力, TransTypeE.倒库任务, TransTypeE.中转倒库));
                            if (carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }

                            //carrier = DevList.Find(c => c.ID != carrierid
                            //                    && c.CurrentTrackId == trackid
                            //                        //&& (c.CurrentSite == track.rfid_2
                            //                        //    || (c.CurrentSite == track.rfid_1 && c.InTask(DevCarrierOrderE.取砖指令)))
                            //                        );
                            //if (carrier != null)
                            //{
                            //    result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                            //    return true;
                            //}
                            break;
                        case TransTypeE.倒库任务:
                        case TransTypeE.上砖接力:
                        case TransTypeE.中转倒库:
                            carrier = DevList.Find(c => c.ID != carrierid
                                                && (c.CurrentTrackId == track.id || c.OnGoingTrackId == track.id)
                                                && (c.InTask(DevCarrierOrderE.倒库指令)
                                                         || PubTask.Trans.IsCarrierInTrans(c.ID, track.id, TransTypeE.上砖接力, TransTypeE.倒库任务, TransTypeE.中转倒库)));
                            if (carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
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
                        case TransTypeE.上砖接力:
                            carrier = DevList.Find(c => c.ID != carrierid
                                                && (c.CurrentTrackId == track.id || c.CurrentTrackId == track.brother_track_id)
                                                && (c.InTask(DevCarrierOrderE.倒库指令)
                                                         || PubTask.Trans.IsCarrierInTrans(c.ID, track.id, TransTypeE.上砖接力, TransTypeE.倒库任务)));
                            if (carrier != null)
                            {
                                result = string.Format("存在运输车[ {0} ]", carrier.Device.name);
                                return true;
                            }
                            break;
                        case TransTypeE.上砖任务://除了倒库任务的运输车
                            carrier = DevList.Find(c => c.ID != carrierid
                               && c.CurrentTrackId == track.id
                               && c.NotInTask(DevCarrierOrderE.倒库指令)
                               && !PubTask.Trans.IsCarrierInTrans(c.ID, trackid, TransTypeE.上砖接力, TransTypeE.倒库任务));
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
        /// 查找在摆渡车上的载货运输车
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="ids">运输车ID</param>
        /// <returns></returns>
        internal bool GetInFerryAndLoad(uint areaid, out List<uint> ids, TrackTypeE type)
        {
            List<CarrierTask> list = DevList.FindAll(c => c.AreaId == areaid
                         && c.IsConnect
                         && c.IsLoad()
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
                                        && c.InTask(DevCarrierOrderE.倒库指令)
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
        /// 判断小车是否处于入库侧轨道
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsFreeCarrierInTrackIn(uint carrier_id, uint track_id, out uint carid)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            carid = DevList.Find(c => c.ID != carrier_id && c.CurrentTrackId == track_id
                    && c.IsNotDoingTask
                    && (track.is_take_forward ? (c.CurrentPoint > track.split_point) : (c.CurrentPoint < track.split_point))
                    )?.ID ?? 0;
            return carid > 0;
        }

        /// <summary>
        /// 判断小车是否处于出库侧轨道
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsFreeCarrierInTrackOut(uint carrier_id, uint track_id, out uint carid)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            carid = DevList.Find(c => c.ID != carrier_id && c.CurrentTrackId == track_id
                    && c.IsNotDoingTask
                    && (track.is_take_forward ? (c.CurrentPoint < track.split_point) : (c.CurrentPoint > track.split_point))
                    )?.ID ?? 0;
            return carid > 0;
        }

        /// <summary>
        /// 下砖工位中取砖时无法继续作业
        /// </summary>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        internal bool IsCarrierNotLoadInDownTileAlert(uint carrierid)
        {
            CarrierTask task = DevList.Find(c => c.ID == carrierid);
            if (task != null)
            {
                return task.DevAlert.CanNotActionForTaking();
            }
            return false;
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
        /// 校验轨道（轨道编号）
        /// </summary>
        public ushort CheckTra { set; get; } = 0;
        /// <summary>
        /// 定位RFID（取卸位置）
        /// </summary>
        public ushort ToRFID { set; get; } = 0;
        /// <summary>
        /// 定位坐标（取卸位置）
        /// </summary>
        public ushort ToPoint { set; get; } = 0;
        /// <summary>
        /// 结束RFID（最后定位位置）
        /// </summary>
        public ushort OverRFID { set; get; } = 0;
        /// <summary>
        /// 结束坐标（最后定位位置）
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
