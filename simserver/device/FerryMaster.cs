using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.msg;
using module.track;
using simserver.simsocket;
using simserver.simsocket.rf;
using System;
using System.Collections.Generic;
using System.Threading;
using task.task;

namespace task.device
{
    /// <summary>
    /// 摆渡车
    /// </summary>
    public class SimFerryMaster
    {
        #region[字段]
        private MsgAction mMsg;
        private List<SimFerryTask> DevList { set; get; }

        private readonly object _obj;
        private SimFerryServer mServer;
        private Thread _mRefresh;
        private bool Refreshing = true;
        #region[摆渡对位]
        #endregion
        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public SimFerryMaster()
        {
            mMsg = new MsgAction();
            _obj = new object();
            DevList = new List<SimFerryTask>();

            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.FerryMsgUpdate, FerryMsgUpdate);
        }

        public void Start()
        {
            mServer = new SimFerryServer(2002);

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
                        foreach (SimFerryTask task in DevList)
                        {
                            if (task.IsEnable)
                            {
                                task.CheckLoaction();
                                task.HaveLoad();
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    finally { Monitor.Exit(_obj); }
                }
                Thread.Sleep(500);
            }
        }

        #endregion

        #region[获取信息]

        public void GetAllFerry()
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                foreach (SimFerryTask task in DevList)
                {
                    MsgSend(task);
                }
            }
            finally { Monitor.Exit(_obj); }
        }

        internal uint GetFerryTrackId(ushort rfid)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return 0;
            }
            try
            {
                return DevList.Find(c =>!c.IsLocating 
                && ((c.DevStatus.UpSite == rfid && c.DevStatus.UpLight)
                    ||(c.DevStatus.DownSite == rfid && c.DevStatus.DownLight)))?.FerryTrackId ?? 0;
            }
            finally { Monitor.Exit(_obj); }
        }

        public void SetLoadStatus(byte deviceID, DevFerryLoadE stauts)
        {
            if(Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimFerryTask task = DevList.Find(c => c.DevId == deviceID);
                    if (task != null)
                    {
                        task.DevStatus.LoadStatus = stauts;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        /// <summary>
        /// 获取摆渡车的目标站点
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        internal ushort GetFerryOnTrackPosCode(ushort rfid, bool backward)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return 0;
            }
            try
            {
                Track track = PubMaster.Track.GetTrackByCode(rfid);
                SimFerryTask task = DevList.Find(c =>!c.IsLocating && c.FerryTrackId == track.id);
                if (task != null)
                {
                    if (backward)
                    {
                        if(task.DevStatus.TargetSite == task.DevStatus.DownSite && task.DevStatus.DownLight)
                        {
                            return task.DevStatus.DownSite;
                        }
                    }
                    else
                    {
                        if (task.DevStatus.TargetSite == task.DevStatus.UpSite && task.DevStatus.UpLight)
                        {
                            return task.DevStatus.UpSite;
                        }
                    }
                }
            }
            finally { Monitor.Exit(_obj); }
            return 0;
        }

        public void SetCurrentSite(byte deviceID, bool isleft, ushort poscode)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                SimFerryTask task = DevList.Find(c => c.DevId == deviceID);
                if (task != null)
                {
                    if (isleft)
                    {
                        task.DevStatus.UpSite = poscode;
                        task.DevStatus.UpLight = true;
                    }else
                    {
                        task.DevStatus.DownSite = poscode;
                        task.DevStatus.DownLight = true;
                    }
                }
            }
            finally { Monitor.Exit(_obj); }
        }

        public void SetOperation(byte deviceID, DevOperateModeE mode)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimFerryTask task = DevList.Find(c => c.DevId == deviceID);

                    if (task != null)
                    {
                        task.DevStatus.WorkMode = mode;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void SetMoveStatus(byte deviceID, DevFerryStatusE mode)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimFerryTask task = DevList.Find(c => c.DevId == deviceID);

                    if (task != null)
                    {
                        task.DevStatus.DeviceStatus = mode;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void SetLightOut(byte deviceID, bool isuplight)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimFerryTask task = DevList.Find(c => c.DevId == deviceID);

                    if (task != null)
                    {
                        if (isuplight)
                        {
                            task.DevStatus.UpLight = false;
                        }
                        else
                        {
                            task.DevStatus.DownLight = false;
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        #endregion

        #region[数据更新]

        private void FerryMsgUpdate(SocketMsgMod mod)
        {
            if (mod != null)
            {
                if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    return;
                }
                try
                {
                    SimFerryTask task = DevList.Find(c => c.DevId == mod.Devid);

                    if(task == null)
                    {
                        task = new SimFerryTask();

                        Device dev = PubMaster.Device.GetDevice(mod.Devid);
                        if (dev == null)
                        {
                            DeviceTypeE devtype = DeviceTypeE.上摆渡;
                            if (mod.Devid < 181)
                            {
                                devtype = DeviceTypeE.下摆渡;
                            }

                            dev = new Device()
                            {
                                id = mod.Devid,
                                name = "摆_" + mod.Devid.ToString("X2"),
                                enable = true,
                                Type = devtype,
                                memo = ""
                            };
                            PubMaster.Device.AddDevice(dev);
                        }
                        task.Device = dev;
                        task.DevId = mod.Devid;
                        task.DevStatus.ID = mod.Devid;
                        task.NowPosCode = dev.lastsite;
                        task.DevStatus.UpSite = dev.lastsite;
                        DevList.Add(task);
                    }

                    if (task != null)
                    {
                        task.ConnStatus = mod.ConnStatus;
                        if (mod.Device is FerryCmd cmd)
                        {
                            DoCmd(task, cmd);
                            MsgSend(task);
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }
        #endregion

        #region[执行任务]

        #endregion

        #region[发送信息]
        private void MsgSend(SimFerryTask task)
        {
            mMsg.ID = task.DevId;
            mMsg.Name = task.Device.name;
            mMsg.o1 = task.DevStatus;
            Messenger.Default.Send(mMsg, MsgToken.FerryStatusUpdate);
        }

        #endregion


        #region[条件判断]


        #endregion


        private void DoCmd(SimFerryTask task, FerryCmd cmd)
        {
            switch (cmd.Commond)
            {
                case DevFerryCmdE.查询:
                    break;
                case DevFerryCmdE.定位:
                    task.StartLocate(cmd.DesCode);
                    break;
                #region[无操作]
                case DevFerryCmdE.速度操作:
                    break;
                case DevFerryCmdE.查询轨道坐标:
                    break;
                case DevFerryCmdE.设置轨道坐标:
                    break;
                case DevFerryCmdE.原点复位:
                    break;
                case DevFerryCmdE.终止任务:
                    task.DevStatus.CurrentTask = DevFerryTaskE.终止;
                    task.DevStatus.FinishTask = DevFerryTaskE.终止;
                    break;
                    #endregion
            }
            ServerSend(task.DevId, task.DevStatus);
        }

        private void ServerSend(byte devid, DevFerry dev)
        {
            mServer?.SendMessage(devid, dev);
        }
    }
}
