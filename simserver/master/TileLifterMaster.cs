using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.deviceconfig;
using module.msg;
using resource;
using simserver.simsocket;
using simserver.simsocket.rf;
using simtask.task;
using System;
using System.Collections.Generic;
using System.Threading;

namespace simtask.master
{
    public class SimTileLifterMaster
    {
        #region[字段]
        private List<SimTileLifterTask> DevList { set; get; }
        private readonly object _obj;
        private SimTileLifterServer mServer;
        private Thread _mRefresh;
        private bool Refreshing = true;
        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public SimTileLifterMaster()
        {
            _obj = new object();
            DevList = new List<SimTileLifterTask>();
            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.SimTileLifterMsgUpdate, SimTileLifterMsgUpdate);

        }

        public void Start()
        {
            mServer = new SimTileLifterServer(2001);

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
            if (_mRefresh != null)
            {
                _mRefresh.Abort(TimeSpan.FromSeconds(3));
            }

            mServer?.Stop();
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
                        foreach (SimTileLifterTask task in DevList)
                        {
                            if (task.IsEnable)
                            {
                                task.CheckFullOrEmpty();
                            }
                        }
                    }
                    finally { Monitor.Exit(_obj); }
                }
                Thread.Sleep(2000);
            }
        }


        #endregion

        #region[获取信息]

        #endregion

        #region[数据更新]

        private void SimTileLifterMsgUpdate(SocketMsgMod mod)
        {
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        SimTileLifterTask task = DevList.Find(c => c.DevId == mod.Devid);
                        if(task == null && mod.Devid > 0)
                        {
                            task = new SimTileLifterTask();

                            Device dev = PubMaster.Device.GetDeviceByMemo(mod.Devid+"");
                            ConfigTileLifter devconf = PubMaster.DevConfig.GetTileLifter(dev.id);
                            task.Device = dev;
                            task.DevConfig = devconf;
                            task.DevId = mod.Devid;
                            task.DevStatus.DeviceID = mod.Devid;
                            task.SetupTile();
                            DevList.Add(task);
                            SendDevMsg(task);
                        }

                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is DevTileCmd cmd)
                            {
                                CheckDev(task, cmd);
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(_obj);
                    }
                }
            }
        }

        public void SetLoadStatus(uint deviceID, bool status, bool setleft)
        {
            SimTileLifterTask task = DevList.Find(c => c.ID == deviceID);
            if (task != null)
            {
                if (setleft)//left
                {
                    task.DevStatus.Goods1 = task.DevStatus.SetGoods;
                    task.IsLoad_1 = status;
                    task.IsNeed_1 = true;
                }
                else if (!setleft && task.Device.Type2 == DeviceType2E.双轨)
                {
                    task.DevStatus.Goods2 = task.DevStatus.SetGoods;
                    task.IsLoad_2 = status;
                    task.IsNeed_2 = true;
                }
            }
        }

        public void SetRequireShift(uint deviceID)
        {
            SimTileLifterTask task = DevList.Find(c => c.ID == deviceID);
            if (task != null)
            {
                task.DevStatus.NeedSytemShift = true;
            }
        }


        internal bool DoUnload(uint trackid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    List<SimTileLifterTask> tasks = DevList.FindAll(c => c.IsWorkingTrack(trackid));
                    if (tasks.Count > 0)
                    {
                        if(tasks.Count == 1)
                        {
                            tasks[0].DoTrackUnload(trackid);
                        }
                        foreach (SimTileLifterTask task in tasks)
                        {
                            if (task.IsTrackLoad(trackid))
                            {
                                task.DoTrackUnload(trackid);
                                break;
                            }
                        }

                        return true;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }


        internal bool DoLoad(uint trackid)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    List<SimTileLifterTask> tasks = DevList.FindAll(c => c.IsWorkingTrack(trackid));
                    if (tasks.Count > 0)
                    {
                        if(tasks.Count == 1)
                        {
                            tasks[0].DoTrackLoad(trackid);
                        }
                        foreach (SimTileLifterTask task in tasks)
                        {
                            if (task.IsTrackUnLoad(trackid))
                            {
                                task.DoTrackLoad(trackid);
                                break;
                            }
                        }

                        return true;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        public void StartOrStopWork(uint devid, bool isstart)
        {
            if (isstart)
            {
                DevList.Find(c => c.ID == devid)?.StartWorking();
            }
            else
            {
                DevList.Find(c => c.ID == devid)?.StopWorking();
            }
        }

        #endregion

        #region[检查状态/生成交易]

        /// <summary>
        /// 检查上下砖机状态
        /// 1.需求信号
        /// 2.满砖/空砖信号
        /// 3.持续5秒有信号-生成交易
        /// </summary>
        /// <param name="task"></param>
        private void CheckDev(SimTileLifterTask task, DevTileCmd cmd)
        {
            switch (cmd.Command)
            {
                case DevLifterCmdTypeE.查询:
                    break;
                case DevLifterCmdTypeE.介入1:
                    if(task.DevConfig.WorkMode == TileWorkModeE.下砖)
                    {
                        if(cmd.InVolType == DevLifterInvolE.离开)
                        {
                            if (task.DevStatus.Need1)
                            {
                                task.DevStatus.Need1 = false;
                                task.DevStatus.Load1 = false;
                                task.DevStatus.Site1Qty = 0;
                            }
                            task.IsInvo_1 = false;
                        }
                        else
                        {
                            task.IsInvo_1 = true;
                        }
                    }else if(task.DevConfig.WorkMode == TileWorkModeE.上砖)
                    {
                        if (cmd.InVolType == DevLifterInvolE.离开)
                        {
                            if (task.DevStatus.Need1)
                            {
                                task.DevStatus.Need1 = false;
                                task.DevStatus.Load1 = true;
                                task.DevStatus.Site1Qty = task.DevStatus.FullQty;
                            }
                            task.IsInvo_1 = false;
                        }
                        else
                        {
                            task.IsInvo_1 = true;
                        }
                    }
                    break;
                case DevLifterCmdTypeE.介入2:

                    if (task.DevConfig.WorkMode == TileWorkModeE.下砖)
                    {
                        if (cmd.InVolType == DevLifterInvolE.离开)
                        {
                            if (task.DevStatus.Need2)
                            {
                                task.DevStatus.Need2 = false;
                                task.DevStatus.Load2 = false;
                                task.DevStatus.Site2Qty = 0;
                            }
                            task.IsInvo_2 = false;
                        }
                        else
                        {
                            task.IsInvo_2 = true;
                        }
                    }
                    else if (task.DevConfig.WorkMode == TileWorkModeE.上砖)
                    {
                        if (cmd.InVolType == DevLifterInvolE.离开)
                        {
                            if (task.DevStatus.Need2)
                            {
                                task.DevStatus.Need2 = false;
                                task.DevStatus.Load2 = true;
                                task.DevStatus.Site2Qty = task.DevStatus.FullQty;
                            }
                            task.IsInvo_2 = false;
                        }
                        else
                        {
                            task.IsInvo_2 = true;
                        }
                    }
                    break;
                case DevLifterCmdTypeE.转产:
                    switch (cmd.ShiftType)
                    {
                        case TileShiftCmdE.复位:
                            break;
                        case TileShiftCmdE.变更品种:
                            task.DevStatus.SetGoods = cmd.GoodId;
                            break;
                        case TileShiftCmdE.执行转产:
                            #region[转产]
                            task.DevStatus.ShiftAccept = true;
                            task.DevStatus.ShiftStatus = TileShiftStatusE.转产中;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    break;
                case DevLifterCmdTypeE.模式:
                    #region[模式]
                    task.DevStatus.WorkMode = cmd.WorkMode;
                    if (cmd.SetFullType == TileFullE.设为满砖)
                    {
                        if (task.IsLoad_1)
                        {
                            task.DevStatus.Need1 = true;
                        }

                        if (task.IsLoad_2)
                        {
                            task.DevStatus.Need2 = true;
                        }
                    }
                    #endregion
                    break;
                case DevLifterCmdTypeE.等级:
                    #region[等级]

                    task.DevStatus.SetLevel = cmd.Level;

                    #endregion
                    break;
                case DevLifterCmdTypeE.复位转产:
                    task.DevStatus.NeedSytemShift = false;
                    break;
                default:
                    break;
            }

            ServerSend(task.DevId, task.DevStatus);
        }

        #endregion

        #region[反馈状态]
        
        private void ServerSend(byte devid, DevTileLifter dev)
        {
            mServer?.SendMessage(devid, dev);
        }

        #endregion


        #region[发送信息]

        private void SendDevMsg(SimTaskBase task)
        {
            Messenger.Default.Send(task, MsgToken.SimDeviceStatusUpdate);
        }

        #endregion
    }
}
