using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.deviceconfig;
using module.msg;
using module.track;
using resource;
using simserver.simsocket;
using simserver.simsocket.rf;
using simtask.task;
using System;
using System.Collections.Generic;
using System.Threading;

namespace simtask.master
{
    /// <summary>
    /// 摆渡车
    /// </summary>
    public class SimFerryMaster
    {
        #region[字段]
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
            _obj = new object();
            DevList = new List<SimFerryTask>();

            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.SimFerryMsgUpdate, SimFerryMsgUpdate);
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


        internal uint GetFerryTrackId(ushort rfid)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return 0;
            }
            try
            {
                return DevList.Find(c => !c.IsLocating
                && ((c.DevStatus.UpSite == rfid && c.DevStatus.UpLight)
                    || (c.DevStatus.DownSite == rfid && c.DevStatus.DownLight)))?.FerryTrackId ?? 0;
            }
            finally { Monitor.Exit(_obj); }
        }

        public void SetLoadStatus(byte deviceID, DevFerryLoadE stauts)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
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
        internal ushort GetFerryOnTrackPosCode(ushort area, ushort rfid, bool backward)
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return 0;
            }
            try
            {
                Track track = PubMaster.Track.GetTrackByPoint(area, rfid);
                SimFerryTask task = DevList.Find(c => !c.IsLocating && c.FerryTrackId == track.id);
                if (task != null)
                {
                    if (backward)
                    {
                        if (task.DevStatus.TargetSite == task.DevStatus.DownSite && task.DevStatus.DownLight)
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

        public void SetCurrentSite(uint devid, Track track, ushort ferrypose)
        {
            SimFerryTask task = DevList.Find(c => c.ID == devid);
            if (task != null)
            {
                bool isup = false, isdown = false;
                if(task.Device.Type == DeviceTypeE.上摆渡)
                {
                    if (ferrypose < 500)
                    {
                        task.DevStatus.DownSite = ferrypose;
                        isdown = true;
                    }
                    else
                    {
                        task.DevStatus.UpSite = ferrypose;
                        isup = true;
                    }
                }
                else
                {
                    if (ferrypose < 300)
                    {
                        task.DevStatus.DownSite = ferrypose;
                        isdown = true;
                    }
                    else
                    {
                        task.DevStatus.UpSite = ferrypose;
                        isup = true;
                    }
                }

                task.SetInitSiteAndPos(isdown, isup) ;
                ServerSend(task.DevId, task.DevStatus);
            }
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

        private void SimFerryMsgUpdate(SocketMsgMod mod)
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

                    if (task == null)
                    {
                        task = new SimFerryTask();

                        Device dev = PubMaster.Device.GetDeviceByMemo(mod.Devid + "");
                        ConfigFerry devconfig = PubMaster.DevConfig.GetFerry(dev.id);

                        task.Device = dev;
                        task.DevConfig = devconfig;
                        task.DevId = mod.Devid;
                        task.DevStatus.ID = mod.Devid;
                        task.DevStatus.UpSite = devconfig.sim_left_site;
                        task.DevStatus.DownSite = devconfig.sim_right_site;
                        task.SetUpFerry();
                        bool isup = task.Device.Type == DeviceTypeE.上摆渡;
                        task.SetInitSiteAndPos(!isup, isup);
                        DevList.Add(task);
                        SendDevMsg(task);
                    }

                    if (task != null)
                    {
                        task.ConnStatus = mod.ConnStatus;
                        if (mod.Device is FerryCmd cmd)
                        {
                            DoCmd(task, cmd);
                        }
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }
        #endregion

        #region[执行任务]

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
                case DevFerryCmdE.自动对位:
                    break;
                case DevFerryCmdE.终止任务:
                    task.DevStatus.CurrentTask = DevFerryTaskE.终止;
                    task.DevStatus.FinishTask = DevFerryTaskE.终止;
                    task.DevStatus.TargetSite = 0;
                    break;
                    #endregion
            }
            ServerSend(task.DevId, task.DevStatus);
        }
        #endregion

        #region[条件判断]

        #endregion

        #region[发送信息]

        private void ServerSend(byte devid, DevFerry dev)
        {
            mServer?.SendMessage(devid, dev);
        }

        private void SendDevMsg(SimTaskBase task)
        {
            Messenger.Default.Send(task, MsgToken.SimDeviceStatusUpdate);
        }

        #endregion
    }
}
