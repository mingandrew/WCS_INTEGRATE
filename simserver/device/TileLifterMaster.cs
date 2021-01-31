using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.msg;
using simserver.simsocket.rf;
using System;
using System.Collections.Generic;
using System.Threading;
using task.task;

namespace task.device
{
    public class SimTileLifterMaster
    {
        #region[字段]
        private MsgAction mMsg;
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
            mMsg = new MsgAction();
            _obj = new object();
            DevList = new List<SimTileLifterTask>();
            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.TileLifterMsgUpdate, TileLifterMsgUpdate);

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

        public void GetAllTileLifter()
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                foreach (SimTileLifterTask task in DevList)
                {
                    MsgSend(task);
                }
            }
            finally { Monitor.Exit(_obj); }
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

        private void TileLifterMsgUpdate(SocketMsgMod mod)
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

                            Device dev = PubMaster.Device.GetDevice(mod.Devid);
                            if (dev == null)
                            {
                                DeviceTypeE devtype = DeviceTypeE.上砖机;
                                string name = "上_";
                                if (mod.Devid < 209)
                                {
                                    devtype = DeviceTypeE.下砖机;
                                    name = "下_";
                                }
                                dev = new Device()
                                {
                                    id = mod.Devid,
                                    name = name + mod.Devid.ToString("X2"),
                                    enable = true,
                                    Type = devtype,
                                    memo = ""
                                };
                                PubMaster.Device.AddDevice(dev);
                            }
                            task.Device = dev;
                            task.DevId = mod.Devid;
                            task.DevStatus.DeviceID = mod.Devid;
                            task.SetDicInfo();
                            DevList.Add(task);
                        }

                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is DevTileCmd cmd)
                            {
                                CheckDev(task, cmd);

                                MsgSend(task);
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

        public void SetLoadStatus(byte deviceID, bool status, bool v)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    SimTileLifterTask task = DevList.Find(c => c.DevId == deviceID);
                    if (task != null)
                    {
                        if (v)//left
                        {
                            task.IsLoad_1 = status;
                            task.IsNeed_1 = true;
                        }
                        else if(!v && task.Device.Type2 == DeviceType2E.双轨)
                        {

                            task.IsLoad_2 = status;
                            task.IsNeed_2 = true;
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
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
                        if(tasks.Count>1)
                            tasks.Sort((x, y) => x.OutDistance.CompareTo(y.OutDistance));
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
                        if(tasks.Count>1)
                            tasks.Sort((x, y) => x.OutDistance.CompareTo(y.OutDistance));
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

        public void StartWork(byte deviceID)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    DevList.Find(c => c.DevId == deviceID)?.StartWorking();
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void StopWork(byte deviceID)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    DevList.Find(c => c.DevId == deviceID)?.StopWorking();
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
                    if(task.Type == DeviceTypeE.下砖机)
                    {
                        if(cmd.InVolType == DevLifterInvolE.介入)
                        {
                            //if (task.IsLoad_1 && task.IsNeed_1)
                            //{
                            //    task.IsInvo_1 = true;
                            //}
                            task.IsInvo_1 = true;
                        }
                        else
                        {
                            task.IsInvo_1 = false;
                        }
                    }else if(task.Type == DeviceTypeE.上砖机)
                    {
                        if (cmd.InVolType == DevLifterInvolE.介入)
                        {
                            //if (!task.IsLoad_1 && task.IsNeed_1)
                            //{
                            //    task.IsInvo_1 = true;
                            //}
                            task.IsInvo_1 = true;
                        }
                        else
                        {
                            task.IsInvo_1 = false;
                        }
                        
                    }
                    break;
                case DevLifterCmdTypeE.介入2:
                    if (task.Type == DeviceTypeE.下砖机)
                    {
                        if (cmd.InVolType == DevLifterInvolE.介入)
                        {
                            //if (task.IsLoad_2 && task.IsNeed_2)
                            //{
                            //    task.IsInvo_2 = true;
                            //}
                            task.IsInvo_2 = true;
                        }
                        else
                        {
                            task.IsInvo_2 = false;
                        }
                    }
                    else if (task.Type == DeviceTypeE.上砖机)
                    {
                        if (cmd.InVolType == DevLifterInvolE.介入)
                        {
                            //if (!task.IsLoad_2 && task.IsNeed_2)
                            //{
                            //    task.IsInvo_2 = true;
                            //}
                            task.IsInvo_2 = true;
                        }
                        else
                        {
                            task.IsInvo_2 = false;
                        }

                    }
                    break;
                default:
                    break;
            }

            ServerSend(task.DevId, task.DevStatus);
        }

        #endregion

        #region[发送信息]
        private void MsgSend(SimTileLifterTask task)
        {
            mMsg.ID = task.DevId;
            mMsg.Name = task.Device?.name ?? "未设定";
            mMsg.o1 = task.DevStatus;
            Messenger.Default.Send(mMsg, MsgToken.TileLifterStatusUpdate);
        }
        #endregion

        #region[反馈状态]
        
        private void ServerSend(byte devid, DevTileLifter dev)
        {
            mServer?.SendMessage(devid, dev);
        }

        #endregion
    }
}
