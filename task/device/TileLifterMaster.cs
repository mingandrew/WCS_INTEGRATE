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
using System.Linq;
using System.Threading;
using task.task;
using tool.appconfig;
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

        /// <summary>
        /// 轮询时间
        /// </summary>
        public int TileRefreshTime { set; get; }
        /// <summary>
        /// 离开等待时间
        /// </summary>
        private int TileLiveTime { set; get; }
        /// <summary>
        /// 介入等待时间
        /// </summary>
        private int TileInvaTime { set; get; }
        /// <summary>
        /// 其他等待时间
        /// </summary>
        private int TileOtherTime { set; get; }

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
            TileRefreshTime = GlobalWcsDataConfig.BigConifg.TileRefreshTime;
            TileLiveTime = GlobalWcsDataConfig.BigConifg.TileLiveTime;
            TileInvaTime = GlobalWcsDataConfig.BigConifg.TileInvaTime;
            TileOtherTime = GlobalWcsDataConfig.BigConifg.TileOtherTime;
        }

        public void Start()
        {
            List<Device> tilelifters = PubMaster.Device.GetTileLifters();
            foreach (Device dev in tilelifters)
            {
                TileLifterTask task = new TileLifterTask
                {
                    Device = dev,
                    DevConfig = PubMaster.DevConfig.GetTileLifter(dev.id),
                    Config_Light = GlobalWcsDataConfig.AlertLightConfig.GetDevLight(dev.id)
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

        /// <summary>
        /// 只要发送了内容，就会有反馈则不再发送查询内容
        /// </summary>
        private bool refreshsend = false;
        private void Refresh()
        {
            while (Refreshing)
            {
                try
                {
                    foreach (TileLifterTask task in DevList)
                    {
                        try
                        {
                            refreshsend = false;

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

                                continue;
                            }

                            #endregion

                            if (!task.IsEnable || !task.IsConnect ||
                                (task.ConnStatus != SocketConnectStatusE.通信正常 && task.ConnStatus != SocketConnectStatusE.连接成功))
                            {
                                continue;
                            }

                            #region 下砖-转产

                            if (task.DevConfig.WorkMode == TileWorkModeE.下砖)
                            {
                                int count = PubMaster.Dic.GetDtlIntCode(DicTag.TileLifterShiftCount);
                                switch (task.DevStatus.ShiftStatus)
                                {
                                    case TileShiftStatusE.复位:
                                        #region [复位]
                                        if (task.DevConfig.do_shift)
                                        {
                                            if (!task.DevStatus.ShiftAccept)
                                            {
                                                task.DoShift(TileShiftCmdE.执行转产, (byte)count, task.DevConfig.goods_id);
                                                refreshsend = true;
                                                break;
                                            }
                                        }

                                        if (!task.DevConfig.do_shift)
                                        {
                                            if (task.DevStatus.ShiftAccept)
                                            {
                                                task.DoShift(TileShiftCmdE.复位); 
                                                refreshsend = true;
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
                                                task.DoShift(TileShiftCmdE.执行转产, (byte)count, task.DevConfig.goods_id);
                                                refreshsend = true;
                                                break;
                                            }
                                        }

                                        if (!task.DevConfig.do_shift)
                                        {
                                            if (task.DevStatus.ShiftAccept)
                                            {
                                                task.DoShift(TileShiftCmdE.复位);
                                                refreshsend = true;
                                                break;
                                            }
                                        }
                                        #endregion
                                        break;
                                    case TileShiftStatusE.完成:
                                        #region [完成]
                                        if (task.DevConfig.do_shift)
                                        {
                                            task.DevConfig.do_shift = false;
                                            task.DevConfig.old_goodid = 0;
                                            PubMaster.DevConfig.SetTileLifterGoods(task.ID, task.DevConfig.goods_id);
                                            break;
                                        }
                                        task.DoShift(TileShiftCmdE.复位);
                                        refreshsend = true;
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                            }

                            #endregion

                            #region 上砖-转产

                            if (task.DevConfig.WorkMode == TileWorkModeE.上砖 && task.DevConfig.do_shift)
                            {
                                task.DevConfig.do_shift = false;
                                task.DevConfig.old_goodid = 0;
                                PubMaster.DevConfig.SetTileLifterGoods(task.ID, task.DevConfig.goods_id);
                            }

                            #endregion

                            #region 同步当前品种/等级

                            if (task.DevConfig.goods_id != task.DevStatus.SetGoods)
                            {
                                task.DoShift(TileShiftCmdE.变更品种, 0, task.DevConfig.goods_id);
                                refreshsend = true;
                            }

                            byte level = PubMaster.Goods.GetGoodsLevel(task.DevConfig.goods_id);
                            if (level != task.DevStatus.SetLevel)
                            {
                                task.DoUpdateLevel(level);
                                refreshsend = true;
                            }

                            #endregion

                            #region 切换模式
                            if (task.DevConfig.can_cutover)
                            {
                                if (!task.DevConfig.do_cutover &&
                                    task.DevConfig.WorkMode != task.DevStatus.WorkMode)
                                {
                                    // 以砖机为准？
                                    PubMaster.DevConfig.FinishCutover(task.ID, task.DevStatus.WorkMode);

                                    // 以调度为准？
                                    //task.DoCutover(task.DevConfig.WorkMode, TileFullE.忽略);
                                    break;
                                }

                                if (task.DevConfig.do_cutover)
                                {
                                    if (task.DevConfig.WorkModeNext == TileWorkModeE.无 ||
                                        task.DevConfig.WorkModeNext == task.DevStatus.WorkMode)
                                    {
                                        // 复位
                                        PubMaster.DevConfig.FinishCutover(task.ID, task.DevStatus.WorkMode);
                                        break;
                                    }

                                    switch (task.DevConfig.WorkModeNext)
                                    {
                                        case TileWorkModeE.过砖: // xxx => 过砖
                                            task.DoCutover(TileWorkModeE.过砖, TileFullE.忽略);
                                            refreshsend = true;
                                            break;

                                        case TileWorkModeE.下砖: // xxx => 下砖
                                            if (!PubTask.Trans.CancelTaskForCutover(task.ID, task.DevConfig.goods_id, out string res))
                                            {
                                                mlog.Info(true, res);
                                                break;
                                            }

                                            if (task.DevConfig.goods_id == task.DevConfig.pre_goodid ||
                                                (!task.DevStatus.Load1 && !task.DevStatus.Load2))
                                            {
                                                task.DoCutover(TileWorkModeE.下砖, TileFullE.忽略);
                                                refreshsend = true;
                                            }
                                            break;

                                        case TileWorkModeE.上砖: // xxx => 上砖
                                            if (task.DevConfig.goods_id != task.DevConfig.pre_goodid 
                                                && (task.DevStatus.Load1 || task.DevStatus.Load2) 
                                                && (!task.DevStatus.Need1 && !task.DevStatus.Need2))
                                            {
                                                task.DoCutover(TileWorkModeE.下砖, TileFullE.设为满砖);
                                                refreshsend = true;
                                            }

                                            if (!task.DevStatus.Load1 && !task.DevStatus.Load2)
                                            {
                                                task.DoCutover(TileWorkModeE.上砖, TileFullE.忽略);
                                                refreshsend = true;
                                                break;
                                            }

                                            if (!PubTask.Trans.CancelTaskForCutover(task.ID, task.DevConfig.goods_id, out res))
                                            {
                                                mlog.Info(true, res);
                                                break;
                                            }

                                            if (task.DevConfig.goods_id == task.DevConfig.pre_goodid)
                                            {
                                                task.DoCutover(TileWorkModeE.上砖, TileFullE.忽略);
                                                refreshsend = true;
                                            }
                                            break;

                                        case TileWorkModeE.无:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            #endregion

                            #region[报警灯逻辑]

                            if(task.Config_Light != null && task.Config_Light.HaveLight)
                            {
                                bool havewarn = false;
                                //自管砖机本身的报警信息
                                if (task.Config_Light.OnlyMyself)
                                {
                                    havewarn = PubMaster.Warn.HaveDevWarn(task.ID, task.Config_Light.WarnLevel);
                                }
                                //自管区域的报警信息
                                else if (task.Config_Light.OnlyArea)
                                {
                                    havewarn = PubMaster.Warn.HaveAreaWarn(task.AreaId, task.Config_Light.WarnLevel);
                                }
                                //自管区域线路的报警信息
                                else if (task.Config_Light.OnlyLine)
                                {
                                    havewarn = PubMaster.Warn.HaveAreaLineWarn(task.AreaId, task.Line, task.Config_Light.WarnLevel);
                                }

                                ///有报警，灯关 则 开灯
                                if (havewarn)
                                {
                                    if (task.LightOff)
                                    {
                                        task.DoLight(TileLightShiftE.灯开);
                                        refreshsend = true;
                                    }
                                }
                                //无报警，灯开 则 关灯
                                else
                                {
                                    if (task.LightOn)
                                    {
                                        task.DoLight(TileLightShiftE.灯关);
                                        refreshsend = true;
                                    }
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
                            if (task.IsEnable && task.IsConnect && !refreshsend)
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
                Thread.Sleep(TileRefreshTime);
            }
        }

        /// <summary>
        /// 停止模拟的设备连接
        /// </summary>
        internal void StockSimDevice()
        {
            List<TileLifterTask> tasks = DevList.FindAll(c => c.IsConnect && c.Device.ip.Equals("127.0.0.1"));
            foreach (TileLifterTask task in tasks)
            {
                if (task.IsEnable)
                {
                    task.Device.enable = false;
                }
                task.Stop("模拟停止");
            }
        }

        public TileShiftStatusE GetTileShiftStatus(uint devid)
        {
            return DevList.Find(c => c.ID == devid)?.TileShiftStatus ?? TileShiftStatusE.复位;
        }

        public void DoInv(uint devid, bool isone, DevLifterInvolE type)
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

        /// <summary>
        /// 忽略兄弟砖机工位介入
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="isone"></param>
        /// <returns></returns>
        public void DoIgnore(uint devid, bool isone)
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
            catch { }
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

                            if (task.Type == DeviceTypeE.上砖机)
                            {
                                if (!task.IsInvo_1)
                                {
                                    return true;
                                }
                                return false;
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
                            if (bro != null && bro.IsNeed_1 && bro.Type == DeviceTypeE.下砖机 && task.IsNeed_1)
                            {
                                task.Do1Invo(DevLifterInvolE.清除需求);
                                mlog.Status(true, string.Format("发送砖机：[ {0} ] 工位1清除需求指令", task.Device.name));
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

                            if (task.Type == DeviceTypeE.上砖机)
                            {
                                if (!task.IsInvo_2)
                                {
                                    return true;
                                }
                                return false;
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
                            if (bro != null && bro.IsNeed_2 && bro.Type == DeviceTypeE.下砖机 && task.IsNeed_2)
                            {
                                task.Do2Invo(DevLifterInvolE.清除需求);
                                mlog.Status(true, string.Format("发送砖机：[ {0} ] 工位2清除需求指令", task.Device.name));
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
            catch { }
            return false;
        }

        internal bool IsTrackEmtpy(uint tileid, uint trackid)
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
            return false;
        }

        public void StartStopTileLifter(uint tileid, bool isstart)
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
            }catch(Exception ex)
            {
                mlog.Error(true, ex.StackTrace);
            }
        }

        public void UpdateTileInStrategry(uint id, StrategyInE instrategy, DevWorkTypeE worktype)
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
            catch { }
        }

        public void UpdateTileOutStrategry(uint id, StrategyOutE outstrategy, DevWorkTypeE worktype)
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
            catch { }
        }

        #endregion

        #region[获取信息]

        public void GetAllTileLifter()
        {
            foreach (TileLifterTask task in DevList)
            {
                MsgSend(task, task.DevStatus);
            }
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

        /// <summary>
        /// 用于获取备用砖机的信息
        /// </summary>
        /// <param name="dev_id">备用砖机id</param>
        public List<List<object>> GetBackupTileLifter(uint dev_id)
        {
            TileLifterTask task = DevList.Find(c => c.ID == dev_id);
            //保存每个要备用的砖机的有用信息
            List<List<object>> altertileinfo = new List<List<object>>();
            if (task != null && task.DevConfig.can_alter)
            {
                string[] alteridList = task.DevConfig.alter_ids.Split(',');
                foreach (string item in alteridList)
                {
                    List<object> temp = new List<object>();
                    if (uint.TryParse(item, out uint alterid))
                    {
                        TileLifterTask altertile = DevList.Find(c => c.ID == alterid);
                        temp.Add(altertile.ID);
                        temp.Add(altertile.Device.name);
                        temp.Add(altertile.Type);
                        uint goodid = altertile.DevConfig.goods_id;
                        string goodidnfo = PubMaster.Goods.GetGoodsName(goodid);
                        temp.Add(goodidnfo);
                        temp.Add(altertile.DevConfig.last_track_id);

                        List<uint> tiletrackids = PubMaster.Area.GetAreaDevTrackWithTrackIds(altertile.ID);
                        string maxname = PubMaster.Track.GetTrackName(tiletrackids.Max());
                        string minname = PubMaster.Track.GetTrackName(tiletrackids.Min());
                        string content = string.Format("{0} 至 {1}", minname, maxname);
                        temp.Add(content);
                    }

                    altertileinfo.Add(temp);
                }
            }

            return altertileinfo;
        }

        #endregion

        #region[数据更新]

        private void TileLifterMsgUpdate(SocketMsgMod mod)
        {
            if (isWcsStoping) return;
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromMilliseconds(500)))
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
                                if (task.Ignore_1 || task.Ignore_2)
                                {
                                    task.Ignore_1 = false;
                                    task.Ignore_2 = false;
                                }
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
                    if (task.IsEnable) PubMaster.Warn.AddDevWarn(task.AreaId, task.Line, WarningTypeE.DeviceOffline, (ushort)task.ID);
                    PubTask.Ping.AddPing(task.Device.ip, task.Device.name);
                    break;
            }

            if (task.MConChange || mTimer.IsTimeOutAndReset("ConnRefresh", task.ID, 3))
            {
                MsgSend(task, task.DevStatus);
            }
        }

        /// <summary>
        /// 重置上砖机优先作业轨道
        /// </summary>
        /// <param name="takeid"></param>
        public void ReseUpTileCurrentTake(uint takeid)
        {
            foreach (TileLifterTask t in DevList.FindAll(c => c.DevConfig.last_track_id == takeid && c.Type == DeviceTypeE.上砖机))
            {
                PubMaster.DevConfig.SetLastTrackId(t.ID, 0);
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

            #region[检查基础信息]

            if(task.DevConfig == null)
            {
                mlog.Warn(true, task.ID + "：没有 config");
                return;
            }

            if (task.DevConfig.goods_id == 0)
            {
                mlog.Warn(true, task.ID + "：没有 config.goods_id");
                return;
            }

            //品种是空品种
            if (PubMaster.Goods.IsGoodEmpty(task.DevConfig.goods_id))
            {
                mlog.Warn(true, task.ID + "：config.goods_id 是空品种属性");
                return;
            }

            if (task.DevConfig.left_track_id == 0)
            {
                mlog.Warn(true, task.ID + "：没有 config.left_track_id");
                return;
            }

            //生成砖机的需求 20210121
            PubTask.TileLifterNeed.CheckTileLifterNeed(task);

            #endregion

            #region[砖机发起转产信号]

            if (task.DevStatus.NeedSytemShift
                && PubMaster.Dic.IsSwitchOnOff(DicTag.TileNeedSysShiftFunc, false))
            {
                if (task.DevConfig.IsLastShiftTimeOk())
                {
                    //在转产允许间隔5分钟后，依然还没完成转产，则更新转产时间
                    if (task.DevConfig.do_shift)
                    {
                        PubMaster.DevConfig.UpdateShiftTime(task.ID);
                    }

                    if (!task.DevConfig.do_shift)//执行转产
                    {
                        if (task.Type == DeviceTypeE.下砖机 && task.DevConfig.pre_goodid == 0)
                        {
                            //添加默认品种 A,B,C,D,E....
                            if (!PubMaster.Goods.AddDefaultGood(task.DevConfig.id, task.DevConfig.goods_id, out string ad_rs, out uint pgoodid))
                            {

                            }
                            else
                            {
                                if (!PubMaster.DevConfig.UpdateTilePreGood(task.ID, task.DevConfig.goods_id, pgoodid, 0, out string up_rs))
                                {

                                }
                            }
                        }

                        if (task.Device.Type == DeviceTypeE.上砖机)
                        {
                            if (task.DevConfig.pre_goodid == 0)
                            {
                                if (mTimer.IsOver("UpNotSetPreGood" + task.ID, 5 * 60, 60))
                                {
                                    task.DevConfig.last_shift_time = DateTime.Now;
                                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTilePreGoodNotSet, (ushort)task.ID);
                                }
                                else
                                {
                                    PubMaster.Warn.AddDevWarn(task.AreaId, task.Line, WarningTypeE.UpTilePreGoodNotSet, (ushort)task.ID);
                                }
                            }
                            else
                            {
                                PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTilePreGoodNotSet, (ushort)task.ID);
                            }
                        }

                        if (task.DevConfig.pre_goodid > 0)
                        {
                            if (!PubMaster.DevConfig.UpdateShiftTileGood(task.ID, task.DevConfig.goods_id, task.DevConfig.prior_empty_track, out string rs))
                            {

                            }
                        }
                    }
                }
                else//复位信号
                {
                    if (mTimer.IsOver(task.ID + "", 2, 10))
                    {
                        Thread.Sleep(TileOtherTime);
                        task.DoTileShiftSignal(TileAlertShiftE.收到转产);
                    }
                }
            }

            #endregion

            #region[备用砖机切换备用]

            //是否 开启 自动转备用机 否则只能在界面通过备用砖机启用进行设定
            if(PubMaster.Dic.IsSwitchOnOff(DicTag.AutoBackupTileFunc, false))
            {
                //第一种方式：备用机 指定 备用哪台砖机
                if(!GlobalWcsDataConfig.BigConifg.IsUserAutoBackDevVersion2(task.AreaId, task.Line))
                {
                    if (task.DevConfig.can_alter)
                    {
                        if (task.DevStatus.BackupShiftDev > 0)
                        {
                            if(task.DevConfig.alter_dev_id == 0)
                            {
                                PubMaster.DevConfig.SetBackupTileLifterCode(task.ID, task.DevStatus.BackupShiftDev);
                            }
                        }
                        else
                        {
                            bool needhavetrans = true;
                            //结束备用, 备用机有货则转产
                            bool haveload1 = task.DevStatus.Load1 ? task.DevStatus.Need1 : true;
                            bool haveload2 = task.DevStatus.Load2 ? task.DevStatus.Need2 : true;
                            if (!haveload1 || !haveload2)
                            {
                                task.DoCutover(TileWorkModeE.下砖, TileFullE.设为满砖);
                                needhavetrans = false;
                            }

                            //左工位有需求，但没任务则不结束备用
                            if(task.DevStatus.Load1
                                && task.DevStatus.Need1
                                && !PubTask.Trans.HaveInTileTrack(task.DevConfig.left_track_id))
                            {
                                needhavetrans = false;
                            }

                            //右工位有需求，但没任务则不结束备用
                            if(task.DevStatus.Load2
                                && task.DevStatus.Need2
                                && !PubTask.Trans.HaveInTileTrack(task.DevConfig.right_track_id))
                            {
                                needhavetrans = false;
                            }

                            if (haveload1 && haveload2 && needhavetrans
                                && PubMaster.DevConfig.StopBackupTileLifter(task.ID, task.DevStatus.Load1 || task.DevStatus.Load2))
                            {
                                if (task.DevStatus.IsReceiveSetFull)
                                {
                                    task.DoCutover(TileWorkModeE.下砖, TileFullE.忽略);
                                }
                            }
                        }
                    }
                }
                //第二种方式：砖机 指定 备用机 进行 备用
                else
                {
                    if (!task.DevConfig.can_alter)//普通砖机
                    {
                        //开始备用，
                        if (task.DevStatus.BackupShiftDev > 0)
                        {
                            if(task.DevConfig.alter_dev_id == 0)
                            {
                                uint backup_id = PubMaster.Device.GetDevIdByMemo(task.DevStatus.BackupShiftDev + "");
                                if (backup_id != 0
                                    && task.DevConfig.IsInBackUpList(backup_id))
                                {
                                    if (PubMaster.DevConfig.SetBackupTileLifter(task.ID, backup_id, false))
                                    {
                                        PubMaster.DevConfig.SetNormalTileBackTileId(task.ID, backup_id);
                                    }
                                }
                            }
                        }
                        else if(task.DevStatus.BackupShiftDev == 0 && task.DevConfig.alter_dev_id != 0)
                        {
                            TileLifterTask backtile = GetTileLifter(task.DevConfig.alter_dev_id);
                            
                            //备用砖机备用了当前普通砖机
                            if(backtile!=null && backtile.DevConfig.alter_dev_id == task.ID && backtile.DevConfig.WorkMode == TileWorkModeE.下砖)
                            {
                                //结束备用, 备用机有货则转产

                                bool needhavetrans = true;
                                //结束备用, 备用机有货则转产
                                bool haveload1 = backtile.DevStatus.Load1 ? backtile.DevStatus.Need1 : true;
                                bool haveload2 = backtile.DevStatus.Load2 ? backtile.DevStatus.Need2 : true;
                                if ((!haveload1 || !haveload2) && !backtile.DevStatus.IsReceiveSetFull)
                                {
                                    backtile.DoCutover(TileWorkModeE.下砖, TileFullE.设为满砖);
                                    needhavetrans = false;
                                }

                                //左工位有需求，但没任务则不结束备用
                                if (backtile.DevStatus.Load1
                                    && backtile.DevStatus.Need1
                                    && !PubTask.Trans.HaveInTileTrack(backtile.DevConfig.left_track_id))
                                {
                                    needhavetrans = false;
                                }

                                //右工位有需求，但没任务则不结束备用
                                if (backtile.DevStatus.Load2
                                    && backtile.DevStatus.Need2
                                    && !PubTask.Trans.HaveInTileTrack(backtile.DevConfig.right_track_id))
                                {
                                    needhavetrans = false;
                                }

                                if (haveload1 && haveload2 && needhavetrans
                                    && PubMaster.DevConfig.StopBackupTileLifter(backtile.ID, backtile.DevStatus.Load1 || backtile.DevStatus.Load2))
                                {
                                    PubMaster.DevConfig.SetNormalTileBackTileId(task.ID, 0);

                                    if (backtile.DevStatus.IsReceiveSetFull)
                                    {
                                        backtile.DoCutover(TileWorkModeE.下砖, TileFullE.忽略);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region[自动离开]
            AutoSendTileInvo(task, true, true);
            #endregion

            #region[设满砖信号自动复位]

            if (task.DevStatus.IsReceiveSetFull 
                && task.DevConfig.WorkMode == TileWorkModeE.下砖 
                && mTimer.IsOver("reveivesetfull"+task.ID, 60, 20))
            {
                Thread.Sleep(TileOtherTime);
                task.DoCutover(TileWorkModeE.下砖, TileFullE.忽略);
            }

            #endregion
        }

        /// <summary>
        /// 自动离开<br/>
        /// 下砖机：有介入，没货<br/>
        /// 非串联砖机：
        /// </summary>
        private void AutoSendTileInvo(TileLifterTask task, bool checkleft, bool checkright)
        {
            //工位1
            if (checkleft
                //&& !task.IsNeed_1 
                && task.IsInvo_1
                && ((task.DevConfig.WorkMode == TileWorkModeE.下砖 && task.IsEmpty_1) 
                    || (task.DevConfig.WorkMode == TileWorkModeE.上砖 && task.IsLoad_1)))
            {
                bool isOK = false;
                switch (task.DevConfig.WorkMode)
                {
                    case TileWorkModeE.上砖:
                        isOK = !PubTask.Carrier.HaveInTrackAndLoad(task.DevConfig.left_track_id);
                        break;
                    case TileWorkModeE.下砖:
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
                        Thread.Sleep(TileLiveTime);
                        task.Do1Invo(DevLifterInvolE.离开);
                    }
                    else
                    {
                        TileLifterTask bro = DevList.Find(c => c.BrotherId == task.ID);
                        if (bro == null || (bro != null && !bro.IsNeed_1))
                        {
                            Thread.Sleep(TileLiveTime);
                            task.Do1Invo(DevLifterInvolE.离开);
                        }
                    }
                }
            }

            //工位2
            if (checkright 
                //&& !task.IsNeed_2 
                && task.IsTwoTrack 
                && task.IsInvo_2
                && ((task.DevConfig.WorkMode == TileWorkModeE.下砖 && task.IsEmpty_2)
                    || (task.DevConfig.WorkMode == TileWorkModeE.上砖 && task.IsLoad_2)))
            {
                bool isOK = false;
                if (task.DevConfig.right_track_id == 0)
                {
                    isOK = true;
                }
                else
                {
                    switch (task.DevConfig.WorkMode)
                    {
                        case TileWorkModeE.上砖:
                            isOK = !PubTask.Carrier.HaveInTrackAndLoad(task.DevConfig.right_track_id);
                            break;
                        case TileWorkModeE.下砖:
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
                        Thread.Sleep(TileLiveTime);
                        task.Do2Invo(DevLifterInvolE.离开);
                    }
                    else
                    {
                        TileLifterTask bro = DevList.Find(c => c.BrotherId == task.ID);
                        if (bro == null || (bro != null && !bro.IsNeed_2))
                        {
                            Thread.Sleep(TileLiveTime);
                            task.Do2Invo(DevLifterInvolE.离开);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 根据砖机状态和需求来生成任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="need"></param>
        public void CheckAndCreateStockTrans(TileLifterTask task, TileLifterNeed need)
        {
            if (!task.IsWorking) return;

            #region[工位1有需求]

            if (task.IsNeed_1 && need.left)
            {
                #region[下砖机-满砖]

                if (task.DevConfig.WorkMode == TileWorkModeE.下砖)
                {
                    if (task.IsEmpty_1 && task.IsInvo_1 && !PubTask.Carrier.HaveInTrack(task.DevConfig.left_track_id))
                    {
                        AutoSendTileInvo(task, true, false);
                        //Thread.Sleep(TileLiveTime);
                        //task.Do1Invo(DevLifterInvolE.离开);
                        return;
                    }

                    if (!PubMaster.Area.IsLineDownOnoff(task.AreaId, task.Device.line)) return;

                    #region[介入]

                    if (!task.IsInvo_1)
                    {
                        Thread.Sleep(TileInvaTime);
                        task.Do1Invo(DevLifterInvolE.介入);
                        return;
                    }

                    #endregion

                    if (!CheckBrotherIsReady(task, false, true)) return;

                    if (PubTask.Trans.HaveInTileTrack(task.DevConfig.left_track_id)) return;

                    #region[生成入库交易]

                    uint gid = task.DevStatus.Goods1;
                    if (!IsAllowToBeTaskGoods(task.AreaId, task.Line, task.ID, gid))
                    {
                        // 砖机回馈品种有问题，直接报警 沿用当前设定品种进行作业
                        if (task.DevConfig.goods_id == 0) return;
                        gid = task.DevConfig.goods_id;
                    }

                    if (task.DevConfig.do_cutover &&
                        !IsAllowToWorkForCutover(gid, task.DevConfig.pre_goodid, 
                                task.DevConfig.WorkMode, task.DevConfig.WorkModeNext)) return;

                    bool iseffect = CheckInStrategy(task, task.DevConfig.left_track_id, gid);

                    if (!iseffect)
                    {
                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.品种作业:
                                AddAndGetStockId(task.ID, task.DevConfig.left_track_id, gid, task.Site1Qty, out uint stockid);
                                TileAddInTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, gid, stockid, task.Line);
                                break;
                            case DevWorkTypeE.轨道作业:

                                break;
                            case DevWorkTypeE.混砖作业:
                                AddAndGetStockId(task.ID, task.DevConfig.left_track_id, gid, task.Site1Qty, out stockid);
                                AddMixTrackTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, gid, stockid, task.Line);
                                break;
                        }
                    }

                    #endregion
                }

                #endregion

                #region[上砖机-空砖]

                else if (task.DevConfig.WorkMode == TileWorkModeE.上砖 && task.IsEmpty_1)
                {
                    //if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.上砖)) return;
                    if (!PubMaster.Area.IsLineUpOnoff(task.AreaId, task.Device.line)) return;

                    #region[介入]

                    if (!task.IsInvo_1)
                    {
                        Thread.Sleep(TileInvaTime);
                        task.Do1Invo(DevLifterInvolE.介入);
                        return;
                    }
                    if (task.HaveBrother)
                    {
                        TileLifterTask brotask = DevList.Find(c => c.ID == task.BrotherId);
                        if (!brotask.IsInvo_1 && brotask.IsEmpty_1 && brotask.ConnStatus == SocketConnectStatusE.通信正常) 
                        {
                            Thread.Sleep(TileInvaTime);
                            brotask.Do1Invo(DevLifterInvolE.介入);
                            return;
                        }
                    }
                    #endregion

                    //判断当前砖机轨道是否已有任务
                    if (PubTask.Trans.HaveInTileTrack(task.DevConfig.left_track_id))
                    {
                        //如果已有任务，任务不是反抛任务的话，退出
                        if (PubTask.Trans.HaveInTrackButNotSecondUpTask(task.ID))
                        {
                            return;
                        }
                    }

                    if (PubTask.Trans.HaveInTrackButSecondUpTask(task.ID, task.DevConfig.goods_id)) return;

                    if (!CheckUpBrotherIsReady(task, true, true)) return;

                    if (!CheckUpBrotherIsFullSign(task, true)) return;

                    #region[生成出库交易]

                    if (task.DevConfig.do_cutover &&
                        !IsAllowToWorkForCutover(task.DevConfig.goods_id, task.DevConfig.pre_goodid, 
                                task.DevConfig.WorkMode, task.DevConfig.WorkModeNext)) return;

                    bool iseffect = CheckOutStrategy(task, task.DevConfig.left_track_id);

                    if (!iseffect)
                    {
                        #region[清空轨道上砖轨道库存]

                        PubMaster.Goods.ClearTrackEmtpy(task.DevConfig.left_track_id, true, task.ID);

                        #endregion

                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.品种作业:
                                TileAddOutTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, task.DevConfig.goods_id, task.DevConfig.last_track_id, task.Line);
                                break;
                            case DevWorkTypeE.轨道作业:
                                TileAddTrackOutTransTask(task.AreaId, task.ID, task.DevConfig.left_track_id, task.DevConfig.goods_id, task.Line);
                                break;
                        }
                    }

                    #endregion

                }

                #endregion

            }
            else
            {
                AutoSendTileInvo(task, true, false);
            }
            #endregion

            #region[工位2有需求]

            if (task.IsNeed_2 && task.IsTwoTrack && !need.left)
            {
                if (task.DevConfig.right_track_id == 0) return;

                #region[下砖机-满砖]

                if (task.DevConfig.WorkMode == TileWorkModeE.下砖)
                {
                    if (task.IsEmpty_2 && task.IsInvo_2 && !PubTask.Carrier.HaveInTrack(task.DevConfig.right_track_id))
                    {
                        AutoSendTileInvo(task, false, true);
                        //Thread.Sleep(TileLiveTime);
                        //task.Do2Invo(DevLifterInvolE.离开);
                        return;
                    }

                    //if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.下砖)) return;
                    if (!PubMaster.Area.IsLineDownOnoff(task.AreaId, task.Device.line)) return;

                    #region[介入]

                    if (!task.IsInvo_2)
                    {
                        Thread.Sleep(TileInvaTime);
                        task.Do2Invo(DevLifterInvolE.介入);
                        return;
                    }

                    #endregion

                    if (!CheckBrotherIsReady(task, false, false)) return;

                    if (PubTask.Trans.HaveInTileTrack(task.DevConfig.right_track_id)) return;

                    #region[生成入库交易]

                    uint gid = task.DevStatus.Goods2;
                    if (!IsAllowToBeTaskGoods(task.AreaId, task.Line, task.ID, gid))
                    {
                        // 砖机回馈品种有问题，直接报警 沿用当前设定品种进行作业
                        if (task.DevConfig.goods_id == 0) return;
                        gid = task.DevConfig.goods_id;
                    }

                    if (task.DevConfig.do_cutover &&
                        !IsAllowToWorkForCutover(gid, task.DevConfig.pre_goodid,
                                task.DevConfig.WorkMode, task.DevConfig.WorkModeNext)) return;

                    bool iseffect = CheckInStrategy(task, task.DevConfig.right_track_id, gid);

                    if (!iseffect)
                    {
                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.品种作业:
                                AddAndGetStockId(task.ID, task.DevConfig.right_track_id, gid, task.Site2Qty, out uint stockid);
                                TileAddInTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, gid, stockid, task.Line);
                                break;
                            case DevWorkTypeE.轨道作业:

                                break;
                            case DevWorkTypeE.混砖作业:
                                AddAndGetStockId(task.ID, task.DevConfig.right_track_id, gid, task.Site2Qty, out stockid);
                                AddMixTrackTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, gid, stockid, task.Line);
                                break;
                        }
                    }

                    #endregion
                }

                #endregion

                #region[上砖机-空砖]
                else if (task.DevConfig.WorkMode == TileWorkModeE.上砖 && task.IsEmpty_2)
                {
                    //if (!PubMaster.Dic.IsAreaTaskOnoff(task.AreaId, DicAreaTaskE.上砖)) return;
                    if (!PubMaster.Area.IsLineUpOnoff(task.AreaId, task.Device.line)) return;

                    #region[介入]

                    if (!task.IsInvo_2)
                    {
                        Thread.Sleep(TileInvaTime);
                        task.Do2Invo(DevLifterInvolE.介入);
                        return;
                    }
                    if (task.HaveBrother)
                    {
                        TileLifterTask brotask = DevList.Find(c => c.ID == task.BrotherId);
                        if (!brotask.IsInvo_2 && brotask.IsEmpty_2 && brotask.ConnStatus == SocketConnectStatusE.通信正常)
                        {
                            Thread.Sleep(TileInvaTime);
                            brotask.Do2Invo(DevLifterInvolE.介入);
                            return;
                        }
                    }
                    #endregion

                    //判断当前砖机轨道是否已有任务
                    if (PubTask.Trans.HaveInTileTrack(task.DevConfig.right_track_id))
                    {
                        //如果已有任务，任务不是反抛任务的话，退出
                        if (PubTask.Trans.HaveInTrackButNotSecondUpTask(task.ID))
                        {
                            return;
                        }
                    }

                    if (PubTask.Trans.HaveInTrackButSecondUpTask(task.ID, task.DevConfig.goods_id)) return;

                    if (!CheckUpBrotherIsReady(task, true, false)) return;

                    if (!CheckUpBrotherIsFullSign(task, false)) return;

                    #region[生成出库交易]

                    if (task.DevConfig.do_cutover &&
                        !IsAllowToWorkForCutover(task.DevConfig.goods_id, task.DevConfig.pre_goodid,
                                task.DevConfig.WorkMode, task.DevConfig.WorkModeNext)) return;

                    bool iseffect = CheckOutStrategy(task, task.DevConfig.right_track_id);

                    if (!iseffect)
                    {
                        #region[清空轨道上砖轨道库存]

                        PubMaster.Goods.ClearTrackEmtpy(task.DevConfig.right_track_id, true, task.ID);

                        #endregion

                        switch (task.WorkType)
                        {
                            case DevWorkTypeE.品种作业:
                                TileAddOutTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, task.DevConfig.goods_id, task.DevConfig.last_track_id, task.Line);
                                break;
                            case DevWorkTypeE.轨道作业:
                                TileAddTrackOutTransTask(task.AreaId, task.ID, task.DevConfig.right_track_id, task.DevConfig.goods_id, task.Line);
                                break;
                        }
                    }

                    #endregion
                }

                #endregion
            }
            else if (task.IsInvo_2 && !need.left)
            {
                AutoSendTileInvo(task, false, true);
            }

            #endregion

        }
    
        /// <summary>
        /// 是否允许作为任务品种
        /// </summary>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        private bool IsAllowToBeTaskGoods(uint areaid, ushort lineid, uint tileid, uint goodsid)
        {
            // 无品种反馈
            if (goodsid == 0)
            {
                PubMaster.Warn.AddDevWarn(areaid, lineid, WarningTypeE.TileGoodsIsZero, (ushort)tileid);
                return false;
            }

            // 无品种数据
            if (PubMaster.Goods.GetGoods(goodsid) == null)
            {
                PubMaster.Warn.AddDevWarn(areaid, lineid, WarningTypeE.TileGoodsIsNull, (ushort)tileid);
                return false;
            }

            PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileGoodsIsZero, (ushort)tileid);
            PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileGoodsIsNull, (ushort)tileid);
            return true;
        }

        /// <summary>
        /// 是否允许切换中继续作业
        /// </summary>
        /// <param name="goodsid"></param>
        /// <param name="nextgoodsid"></param>
        /// <param name="mode"></param>
        /// <param name="nextmode"></param>
        /// <returns></returns>
        private bool IsAllowToWorkForCutover(uint goodsid, uint nextgoodsid, TileWorkModeE mode, TileWorkModeE nextmode)
        {
            if (mode == TileWorkModeE.上砖 && nextmode == TileWorkModeE.下砖)
            {
                // 上 -> 下 ，直接停止任务
                return false;
            }

            if (mode == TileWorkModeE.下砖 && nextmode == TileWorkModeE.上砖)
            {
                // 下 -> 上，品种一致不拉走，直接等着上
                if (goodsid == nextgoodsid)
                {
                    return false;
                }
            }

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
        private void AddMixTrackTransTask(uint areaid, uint tileid, uint tiletrackid, uint goodid, uint stockid, ushort line)
        {
            if (stockid == 0) return;
            uint lasttrack = PubMaster.DevConfig.GetLastTrackId(tileid);
            if (lasttrack != 0 && PubMaster.Track.IsStatusOkToGive(lasttrack))
            {
                if (PubTask.Trans.IsTraInTransWithLock(lasttrack))
                {
                    PubMaster.Warn.AddDevWarn(areaid, line, WarningTypeE.TileMixLastTrackInTrans, (ushort)tileid, 0, lasttrack);
                    return;
                }

                PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileMixLastTrackInTrans, (ushort)tileid);

                PubMaster.Track.UpdateRecentGood(lasttrack, goodid);
                PubMaster.Track.UpdateRecentTile(lasttrack, tileid);
                //生成入库交易
                PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.下砖任务, goodid, stockid, tiletrackid, lasttrack, 0, line);
            }
            else
            {
                TileAddInTransTask(areaid, tileid, tiletrackid, goodid, stockid, line);

                PubMaster.Warn.RemoveDevWarn(WarningTypeE.TileMixLastTrackInTrans, (ushort)tileid);
            }
        }

        /// <summary>
        /// 手动取砖的时候
        /// 砖机需求有问题的时候，不能生成库存
        /// 则在手动取货的时候，生成库存信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool AddTileStockInTrack(uint trackid, ushort tracksite, out uint stockid)
        {
            stockid = 0;
            TileLifterTask tile = null;
            //查找轨道同时配置了该工位的砖机
             List <TileLifterTask> tiles = DevList.FindAll(c => c.DevConfig.InTrack(trackid) && c.DevConfig.InTrackSite(tracksite));
            if(tiles.Count > 0)
            {
                tile = tiles[0];
            }else
            {
                //查找配置轨道的所有砖机
                tiles = DevList.FindAll(c => c.DevConfig.InTrack(trackid));
                foreach (var item in tiles)
                {
                    //如果工位1数量达到满砖数量则设置为该砖机
                    if(item.DevConfig.left_track_id == trackid && item.DevStatus.Site1Qty == item.DevStatus.FullQty)
                    {
                        tile = item;
                        break;
                    }

                    //如果工位2数量达到满砖数量则设置为该砖机
                    if (item.DevConfig.right_track_id == trackid && item.DevStatus.Site2Qty == item.DevStatus.FullQty)
                    {
                        tile = item;
                        break;
                    }
                }

                if(tile == null)
                {
                    tile = tiles[0];
                }
            }
            if (tile != null)
            {
                byte qty;
                uint gid;
                if (tile.DevConfig.left_track_id == trackid)
                {
                    gid = tile.DevStatus.Goods1 != 0 ? tile.DevStatus.Goods1 : tile.DevConfig.goods_id;
                    qty = tile.DevStatus.Site1Qty > 0 ? tile.DevStatus.Site1Qty : tile.DevStatus.FullQty;
                }
                else
                {
                    gid = tile.DevStatus.Goods2 != 0 ? tile.DevStatus.Goods2 : tile.DevConfig.goods_id;
                    qty = tile.DevStatus.Site2Qty > 0 ? tile.DevStatus.Site2Qty : tile.DevStatus.FullQty;
                }

                AddAndGetStockId(tile.ID, trackid, gid, qty, out stockid);
            }
            return stockid != 0;
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

                PubMaster.Goods.RemoveTileTrackOtherStock(tileid, tiletrackid, goodid);
            }
        }

        /// <summary>
        /// 添加入库任务
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="tileid">砖机ID</param>
        /// <param name="tiletrackid">砖机轨道ID</param>
        /// <param name="goodid">砖机品种</param>
        /// <param name="fullqty">砖机满砖数量</param>
        private void TileAddInTransTask(uint areaid, uint tileid, uint tiletrackid, uint goodid, uint stockid, ushort line)
        {
            //分配放货点
            if (stockid != 0)
            {
                PubTask.Allocate.AllocateInGiveTrack(areaid, line, tileid, goodid,
                    out uint givetrackid, out uint lastgoodid, out bool islimitallocate);

                if (givetrackid != 0)
                {
                    PubMaster.DevConfig.SetLastTrackId(tileid, givetrackid);
                    PubMaster.Track.UpdateRecentGood(givetrackid, goodid);
                    PubMaster.Track.UpdateRecentTile(givetrackid, tileid);
                    //生成入库交易
                    uint transid = PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.下砖任务, goodid, stockid, tiletrackid, givetrackid, 0, line);

                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.DownTileHaveNotTrackToStore, (ushort)tileid);
                    PubMaster.Warn.RemoveTaskWarn(WarningTypeE.PreventTimeConflict, tileid);

                    if (islimitallocate)
                    {
                        mlog.Status(true, string.Format("极限混砖【砖机：{0}，{1}】【轨道：{2}，{3}】【任务：{4}】",
                                            PubMaster.Device.GetDeviceName(tileid),
                                            PubMaster.Goods.GetGoodsName(goodid),
                                            PubMaster.Track.GetTrackName(givetrackid),
                                            PubMaster.Goods.GetGoodsName(lastgoodid),
                                            transid));
                    }
                }
                else if (stockid != 0)
                {
                    PubMaster.Warn.AddDevWarn(areaid, line, WarningTypeE.DownTileHaveNotTrackToStore, (ushort)tileid);
                }
            }
            
        }

        /// <summary>
        /// 添加出库任务
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="tileid">砖机ID</param>
        /// <param name="tiletrackid">砖机轨道ID</param>
        /// <param name="goodid">砖机品种</param>
        /// <param name="currentid">设置优先轨道</param>
        private void TileAddOutTransTask(uint areaid, uint tileid, uint tiletrackid, uint goodid, uint currentid, ushort line)
        {
            //判断砖机当前数量（全部、或者上砖数量是否大于0）
            if (!PubMaster.DevConfig.IsTileNowGoodQtyOk(tileid, goodid))
            {
                PubMaster.Warn.AddDevWarn(areaid, line, WarningTypeE.Warning37, (ushort)tileid);
                return;
            }
            else
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.Warning37, (ushort)tileid);
            }

            bool isallocate = false;

            // 1.查看当前设定优先作业轨道是否能作业
            if (PubMaster.Track.HaveTrackInGoodFrist(areaid, tileid, goodid, currentid, out uint trackid))
            {
                //判断是否轨道是否已经有任务占用[忽略倒库任务]
                if (!PubTask.Trans.HaveInTrackButSortTask(trackid))
                {
                    uint stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
                    //有库存但是不是砖机需要的品种
                    if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goodid))
                    {
                        PubMaster.Track.UpdateRecentTile(trackid, 0);
                        PubMaster.Track.UpdateRecentGood(trackid, 0);
                        PubTask.TileLifter.ReseUpTileCurrentTake(trackid);
                        return;
                    }
                    if (PubMaster.Track.IsTrackType(tiletrackid, TrackTypeE.下砖轨道))
                    {
                        //生成出库交易
                        PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.同向上砖, goodid, stockid, trackid, tiletrackid, 0, line);
                    }
                    else
                    {
                        //生成出库交易
                        PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stockid, trackid, tiletrackid, 0, line);
                    }
                    //PubMaster.Goods.AddStockOutLog(stockid, tiletrackid, tileid);
                    isallocate = true;
                }
            }

            #region  2.查看是否存在未空砖但无库存的轨道 - 停用，无库存一定空轨
            //else if (PubMaster.Track.HaveTrackInGoodButNotStock(areaid, tileid, goodid, out List<uint> trackids))
            //{
            //    foreach (uint tra in trackids)
            //    {
            //        //判断是否轨道是否已经有任务占用[忽略倒库任务]
            //        if (!PubTask.Trans.HaveInTrackButSortTask(tra))
            //        {
            //            uint stockid = PubMaster.Goods.GetTrackTopStockId(tra);
            //            //有库存但是不是砖机需要的品种
            //            if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goodid))
            //            {
            //                PubMaster.Track.UpdateRecentTile(tra, 0);
            //                PubMaster.Track.UpdateRecentGood(tra, 0);
            //                return;
            //            }
            //            if (PubMaster.Track.IsTrackType(tiletrackid, TrackTypeE.下砖轨道))
            //            {
            //                //生成出库交易
            //                PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.同向上砖, goodid, stockid, tra, tiletrackid, 0, line);
            //            }
            //            else
            //            {
            //                //生成出库交易
            //                PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stockid, tra, tiletrackid, 0, line);
            //            }

            //            //PubMaster.Goods.AddStockOutLog(stockid, tiletrackid, tileid);
            //            isallocate = true;
            //            break;
            //        }
            //    }
            //}
            #endregion

            // 3.分配库存
            else if (PubMaster.Goods.GetStock(areaid, line, tileid, goodid, out List<Stock> allocatestocks))
            {
                foreach (Stock stock in allocatestocks)
                {
                    //判断是否轨道、库存是否已经有任务占用[忽略倒库任务]
                    if (PubTask.Trans.IsStockInTransButSortTask(stock.id, stock.track_id, TransTypeE.库存整理))
                    {
                        break;
                    }

                    PubMaster.Track.UpdateRecentGood(stock.track_id, goodid);
                    PubMaster.Track.UpdateRecentTile(stock.track_id, tileid);

                    if (PubMaster.Track.IsTrackType(tiletrackid, TrackTypeE.下砖轨道))
                    {
                        //生成出库交易
                        PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.同向上砖, goodid, stock.id, stock.track_id, tiletrackid, 0, line);
                    }
                    else
                    {
                        //生成出库交易
                        PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stock.id, stock.track_id, tiletrackid, 0, line);
                    }

                    //PubMaster.Goods.AddStockOutLog(stock.id, tiletrackid, tileid);
                    isallocate = true;
                    break;
                    
                }
            }

            if (isallocate)
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileHaveNotStockToOut, (ushort)tileid);
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.TheEarliestStockInDown, tileid);
            }
            else
            {
                PubMaster.Warn.AddDevWarn(areaid, line, WarningTypeE.UpTileHaveNotStockToOut, (ushort)tileid);
            }

        }

        /// <summary>
        /// 添加上砖机按轨道的出库任务
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="tileid"></param>
        /// <param name="tiletrackid"></param>
        private void TileAddTrackOutTransTask(uint areaid, uint tileid, uint tiletrackid, uint tilegoodid, ushort line)
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
                if (PubMaster.Track.IsTrackType(tiletrackid, TrackTypeE.下砖轨道))
                {
                    //生成出库交易
                    PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.同向上砖, goodid, stockid, tt.track_id, tiletrackid, 0, line);
                }
                else
                {
                    //生成出库交易
                    PubTask.Trans.AddTrans(areaid, tileid, TransTypeE.上砖任务, goodid, stockid, tt.track_id, tiletrackid, 0, line);
                }
                //PubMaster.Goods.AddStockOutLog(stockid, tiletrackid, tileid);
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileHaveNoTrackToOut, (ushort)tileid);
                isallocate = true;
                break;
            }

            if (!isallocate)
            {
                PubMaster.Warn.AddDevWarn(areaid, line, WarningTypeE.UpTileHaveNoTrackToOut, (ushort)tileid);
            }
        }

        internal List<TileLifterTask> GetDevTileLifters()
        {
            return DevList;
        }

        internal List<TileLifterTask> GetDevTileLifters(List<uint> areaids)
        {
            return DevList.FindAll(c=>areaids.Contains(c.AreaId));
        }

        internal List<TileLifterTask> GetDevTileLifters(List<DeviceTypeE> types)
        {
            return DevList.FindAll(c => types.Contains(c.Type));
        }

        internal List<TileLifterTask> GetDevTileLifters(List<uint> areaids, List<DeviceTypeE> types)
        {
            return DevList.FindAll(c => types.Contains(c.Type) && areaids.Contains(c.AreaId));
        }

        internal List<TileLifterTask> GetCanCutoverTiles()
        {
            return DevList.FindAll(c => c.IsCanCutover);
        }

        internal List<TileLifterTask> GetCanCutoverTiles(List<uint> areaids)
        {
            return DevList.FindAll(c => c.IsCanCutover && areaids.Contains(c.AreaId));
        }


        /// <summary>
        /// 砖机是否切换模式中
        /// </summary>
        /// <param name="tileid"></param>
        /// <returns></returns>
        public bool IsTileCutover(uint tileid)
        {
            return DevList.Find(c => c.ID == tileid)?.DevConfig.do_cutover ?? false;
        }

        /// <summary>
        /// 砖机轨道是否有货
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool IsTileLoad(uint tileid, uint track)
        {
            return DevList.Exists(c => c.ID == tileid && (
                (c.DevConfig.left_track_id == track && c.IsLoad_1) || (c.DevConfig.right_track_id == track && c.IsLoad_2)
                ));
        }

        /// <summary>
        /// 砖机有货需求与否判断
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="track"></param>
        /// <param name="isload"></param>
        /// <param name="isneed"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 判断砖机是否在线
        /// </summary>
        /// <param name="tile_id"></param>
        /// <returns></returns>
        public bool IsOnline(uint tile_id)
        {
            return DevList.Exists(c => c.ID == tile_id && c.IsConnect);
        }

        /// <summary>
        /// 砖机是否 有货 无需求
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool IsHaveLoadNoNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, true, false);
        }

        /// <summary>
        /// 砖机是否 无货 无需求
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool IsHaveEmptyNoNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, false, false);
        }

        /// <summary>
        /// 砖机是否 有货 有需求
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool IsHaveLoadNeed(uint tileid, uint track)
        {
            return IsTileLoadAndNeed(tileid, track, true, true);
        }

        /// <summary>
        /// 砖机是否 无货 有需求
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="track"></param>
        /// <returns></returns>
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
                    PubMaster.Warn.AddDevWarn(task.AreaId, task.Line, WarningTypeE.TileNoneStrategy, (ushort)task.ID);
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
                    // 获取所有同策略砖机
                    List<uint> tileids = new List<uint>();
                    foreach (TileLifterTask item in DevList.FindAll(c => c.InStrategy == task.InStrategy))
                    {
                        tileids.Add(item.ID);
                    }
                    iseffect = PubTask.Trans.HaveInGoods(task.AreaId, goodsId, TransTypeE.下砖任务, tileids);
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
                    PubMaster.Warn.AddDevWarn(task.AreaId, task.Line, WarningTypeE.TileNoneStrategy, (ushort)task.ID);
                    break;
                case StrategyOutE.同机同轨:
                    iseffect = PubTask.Trans.HaveInLifter(task.ID);
                    break;
                case StrategyOutE.同规同轨:
                    // 获取所有同策略砖机
                    List<uint> tileids = new List<uint>();
                    foreach (TileLifterTask item in DevList.FindAll(c => c.OutStrategy == task.OutStrategy))
                    {
                        tileids.Add(item.ID);
                    }
                    iseffect = PubTask.Trans.HaveInGoods(task.AreaId, task.DevConfig.goods_id, TransTypeE.上砖任务, tileids);
                    break;
                case StrategyOutE.优先上砖:
                    iseffect = PubTask.Trans.ExistInTileTrack(task.ID, trackid);
                    break;
                case StrategyOutE.同轨同轨://双下砖机，同时只作业一台砖机作业【间接限制了会下不同轨道】
                    iseffect = PubTask.Trans.HaveInTileTrack(task.DevConfig.left_track_id, task.DevConfig.right_track_id);
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
                    if (brotask.DevConfig.WorkMode == TileWorkModeE.下砖 
                        && brotask.IsNeed_1  
                        && brotask.IsEmpty_1)
                    {
                        brotask.Do1Invo(DevLifterInvolE.清除需求);
                        mlog.Status(true, string.Format("发送砖机：[ {0} ] 工位1清除需求指令", brotask.Device.name));
                        return true;
                    }

                    if (brotask.IsNeed_1 && (checkfull ? brotask.IsEmpty_1 : brotask.IsLoad_1)) return false;

                    if (!brotask.IsInvo_1 && (checkfull ? brotask.IsLoad_1 : brotask.IsEmpty_1))
                    {
                        Thread.Sleep(TileInvaTime);
                        brotask.Do1Invo(DevLifterInvolE.介入);
                    }
                    return brotask.IsInvo_1 && (checkfull ? brotask.IsLoad_1 : brotask.IsEmpty_1);
                }

                if (brotask.DevConfig.WorkMode == TileWorkModeE.下砖
                        && brotask.IsNeed_2
                        && brotask.IsEmpty_2)
                {
                    brotask.Do2Invo(DevLifterInvolE.清除需求);
                    mlog.Status(true, string.Format("发送砖机：[ {0} ] 工位1清除需求指令", brotask.Device.name));
                    return true;
                }

                if (brotask.IsNeed_2 && (checkfull ? brotask.IsEmpty_2 : brotask.IsLoad_2)) return false;

                if (!brotask.IsInvo_2 && (checkfull ? brotask.IsLoad_2 : brotask.IsEmpty_2))
                {
                    if (brotask.IsNeed_2) return false;
                    Thread.Sleep(TileInvaTime);
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


        /// <summary>
        /// 检查上砖机兄弟砖机是否满砖状态
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckUpBrotherIsFullSign(TileLifterTask task, bool checkleft)
        {
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign))
            {
                return true;
            }
            //外侧上砖机          
            if (!task.HaveBrother)
            {
                TileLifterTask brotaskin = DevList.Find(c => c.BrotherId == task.ID);//查找有BrotherId是该砖机ID的砖机(及并联内侧上砖机)
                if (brotaskin == null) return true;
                if (task.IsWorking && brotaskin.ConnStatus == SocketConnectStatusE.通信正常)
                {
                    #region[检查远离摆渡车的砖机是否满砖状态]

                    if (checkleft)
                    {
                        if (brotaskin.LoadStatus1 != DevLifterLoadE.满砖) return false;//左轨道检查满砖状态1
                    }
                    else
                    {
                        if (brotaskin.LoadStatus2 != DevLifterLoadE.满砖) return false;//右轨道检查满砖状态2
                    }

                    #endregion
                }
                return true;
            }

            TileLifterTask brotask = DevList.Find(c => c.ID == task.BrotherId);//DevList.Find(c => c.BrotherId == task.ID)
            if (brotask == null) return false;
            if (brotask.ConnStatus == SocketConnectStatusE.通信正常)
            {
                #region[开关-启用砖机的-满砖信号]

                if (checkleft)
                {
                    if (task.LoadStatus2 == DevLifterLoadE.满砖 && brotask.IsEmpty_2)
                    {
                        return false;
                    }
                }
                else
                {
                    if (task.LoadStatus1 == DevLifterLoadE.满砖 && brotask.IsEmpty_1)
                    {
                        return false;
                    }
                }

                #endregion
            }
            return true;
        }

        #endregion

        #region[发送信息]
        private void MsgSend(TileLifterTask task, DevTileLifter tilelifter)
        {
            if (Monitor.TryEnter(_objmsg, TimeSpan.FromSeconds(1)))
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
                    if (task.Type == DeviceTypeE.上砖机)
                    {
                        mMsg.o9 = task.DevConfig.now_good_all ? "不限" : (task.DevConfig.now_good_qty + "");
                    }
                    if (task.Type == DeviceTypeE.下砖机)
                    {
                        mMsg.o9 = "不限";
                    }
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
        /// <summary>
        /// 判断左右工位品种是否一致，满足转产状态
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool IsSiteGoodSame(uint devid)
        {
            return DevList.Exists(c => c.ID == devid && c.IsSiteGoodSame());
        }

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
                if (!task.IsInvo_1 && task.IsNeed_1 && task.IsLoad_1)
                {
                    task.Do1Invo(DevLifterInvolE.介入);
                }
                return task.IsNeed_1 && task.IsLoad_1 && task.IsInvo_1;
            }

            if (task.DevConfig.right_track_id == taketrackid)
            {
                if (!task.IsInvo_2 && task.IsNeed_2 && task.IsLoad_2)
                {
                    task.Do2Invo(DevLifterInvolE.介入);
                }
                return task.IsNeed_2 && task.IsLoad_2 && task.IsInvo_2;
            }
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
            return false;
        }

        /// <summary>
        /// 判断上砖机是否可以放砖
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <param name="givetrackid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool IsGiveReadyWithBackUp(uint tilelifter_id, uint givetrackid, out string result, bool isignoreneed)
        {
            result = "";
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
                if (!task.IsInvo_1 && (task.IsNeed_1 || isignoreneed) && task.IsEmpty_1)
                {
                    task.Do1Invo(DevLifterInvolE.介入);
                }
                return (task.IsNeed_1 || isignoreneed) && task.IsEmpty_1 && task.IsInvo_1;
            }

            if (task.DevConfig.right_track_id == givetrackid)
            {
                if (!task.IsInvo_2 && (task.IsNeed_2 || isignoreneed) && task.IsEmpty_2)
                {
                    task.Do2Invo(DevLifterInvolE.介入);
                }
                return (task.IsNeed_2 || isignoreneed) && task.IsEmpty_2 && task.IsInvo_2;
            }
            return false;
        }

        internal bool IsAnyoneNeeds(uint area, DeviceTypeE dt)
        {
            return DevList.Exists(c => c.AreaId == area && c.Type == dt && (c.IsNeed_1 || c.IsNeed_2));
        }

        /// <summary>
        /// 判断砖机类型
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <param name="tileLifterType"></param>
        /// <returns></returns>
        public bool IsTileLifterType(uint tilelifter_id, TileLifterTypeE tileLifterType)
        {
            return DevList.Exists(c => c.ID == tilelifter_id && c.TileLifterType == tileLifterType);
        }

        /// <summary>
        /// 判断砖机是否离线/对应工位没有了需求 20210121
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isleft"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CheckTileLifterStatusWithNeed(TileLifterTask t, bool isleft, out string result)
        {
            result = "";
            bool fla = CheckTileLifterStatus(t, out result);
            return (fla && (isleft ? t.IsNeed_1 : t.IsNeed_2));
        }

        /// <summary>
        /// 判断砖机是否备用砖机
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <returns></returns>
        public bool IsBackupTileLifter(uint tilelifter_id)
        {
            return DevList.Exists(c => c.ID == tilelifter_id && c.DevConfig.can_alter);
        }

        /// <summary>
        /// 判断品种跟指定的砖机的品种是否一致
        /// </summary>
        /// <param name="tile_id"></param>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public bool EqualTileGood(uint tile_id, uint goodid)
        {
            return DevList.Exists(c => c.ID == tile_id && c.DevConfig.goods_id == goodid);
        }

        /// <summary>
        /// 判断轨道是否下砖机上一次放砖的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsInTileLastTrack(uint trackid)
        {
            if (trackid == 0)
            {
                return false;
            }
            return DevList.Exists(c => c.DevConfig.last_track_id == trackid && c.ConnStatus == SocketConnectStatusE.通信正常);
        }
        #endregion

        #region[更新品种信息]
        public void UpdateTileLifterGoods(uint devid, uint goodid)
        {
            try
            {
                TileLifterTask task = DevList.Find(c => c.ID == devid);
                if (task != null)
                {
                    task.DevConfig.goods_id = goodid;

                    //刷新界面
                    MsgSend(task, task.DevStatus);

                    if (task.IsEnable && task.IsConnect)
                    {
                        #region 同步当前品种/等级
                        if (task.DevConfig.goods_id != task.DevStatus.SetGoods)
                        {
                            task.DoShift(TileShiftCmdE.变更品种, 0, task.DevConfig.goods_id);
                            Thread.Sleep(TileOtherTime);
                        }

                        byte level = PubMaster.Goods.GetGoodsLevel(task.DevConfig.goods_id);
                        if (level != task.DevStatus.SetLevel)
                        {
                            task.DoUpdateLevel(level);
                        }
                        #endregion
                    }
                }
            }
            catch
            {

            }
        }

        #endregion

        #region[启动/停止]

        public void UpdateWorking(uint devId, bool working, byte worktype)
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

        #endregion

        #region[获取属性]


        /// <summary>
        /// 获取砖机工位对应的品种
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal uint GetTileTrackGid(uint tileid, uint trackid)
        {
            uint gid = 0;
            TileLifterTask task = GetTileLifter(tileid);
            if(task != null)
            {
                if(task.DevConfig.left_track_id == trackid)
                {
                    gid = task.DevStatus.Goods1;
                }else if(task.DevConfig.right_track_id == trackid)
                {
                    gid = task.DevStatus.Goods1;
                }

                if (gid == 0)
                {
                    gid = task.DevConfig.goods_id;
                }
            }
            return gid;
        }

        /// <summary>
        /// 判断区域上砖机是否有需求
        /// </summary>
        /// <param name="area_id"></param>
        /// <returns></returns>
        internal bool IsUpTileHaveNeed(uint area_id)
        {
            return DevList.Exists(c => c.AreaId == area_id && c.Type == DeviceTypeE.上砖机 && (c.IsNeed_1 || c.IsNeed_2));
        }

        #endregion
    }
}
