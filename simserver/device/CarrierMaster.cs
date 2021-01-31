using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.msg;
using resource;
using simserver.simsocket;
using simserver.simsocket.rf;
using System;
using System.Collections.Generic;
using System.Threading;

namespace task.device
{
    public class SimCarrierMaster
    {
        #region[字段]
        private readonly MsgAction mMsg;

        private List<SimCarrierTask> DevList { set; get; }
        private readonly object _obj;
        private SimCarrierServer mServer;
        private Thread _mRefresh;
        private bool Refreshing = true;

        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public SimCarrierMaster()
        {
            mMsg = new MsgAction();
            _obj = new object();
            DevList = new List<SimCarrierTask>();
            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.CarrierMsgUpdate, CarrierMsgUpdate);
        }

        public void Start()
        {
            mServer = new CarrierServer(2003);

            if (_mRefresh == null || !_mRefresh.IsAlive || _mRefresh.ThreadState == ThreadState.Aborted)
            {
                _mRefresh = new Thread(Refresh)
                {
                    IsBackground = true
                };
            }

            _mRefresh.Start();
        }

        public void GetAllCarrier()
        {
            if (!Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                return;
            }
            try
            {
                foreach (SimCarrierTask task in DevList)
                {
                    MsgSend(task);
                }
            }
            finally { Monitor.Exit(_obj); }
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
                        foreach (SimCarrierTask task in DevList)
                        {
                            if (task.IsEnable)
                            {
                                task.CheckTask();
                            }
                        }
                    }
                    finally { Monitor.Exit(_obj); }
                }
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region[获取信息]

        #endregion

        #region[数据更新]

        private void CarrierMsgUpdate(SocketMsgMod mod)
        {
            if (mod != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        SimCarrierTask task = DevList.Find(c => c.DevId == mod.Devid);

                        if (task == null)
                        {
                            task = new SimCarrierTask();

                            Device dev = PubMaster.Device.GetDevice(mod.Devid);
                            if (dev == null)
                            {
                                dev = new Device()
                                {
                                    id = mod.Devid,
                                    name = "运_" + mod.Devid.ToString("X2"),
                                    enable = true,
                                    Type = DeviceTypeE.运输车,
                                    memo = ""
                                };
                                PubMaster.Device.AddDevice(dev);
                            }
                            task.Device = dev;
                            task.DevId = mod.Devid;
                            task.DevStatus.ID = mod.Devid;
                            task.UpdateCurrentSite(dev.lastsite);

                            DevList.Add(task);
                        }

                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is CarrierCmd cmd)
                            {
                                CheckDev(task, cmd);
                                MsgSend(task);
                            }
                        }
                    }
                    finally { Monitor.Exit(_obj); }
                }
            }
        }

        internal bool ExistOnFerry(uint ferryTrackId)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    return DevList.Exists(c => c.NowTrack?.id == ferryTrackId);
                }
                finally { Monitor.Exit(_obj); }
            }
            return false;
        }

        private void CheckDev(SimCarrierTask task, CarrierCmd cmd)
        {
            
            ServerSend(task.DevId, task.DevStatus);
        }

        #endregion

        #region[检查设备状态]

        #endregion

        #region[发送信息]
        private void MsgSend(SimCarrierTask task)
        {
            mMsg.ID = task.DevId;
            mMsg.Name = task.Device.name;
            mMsg.o1 = task.DevStatus;
            Messenger.Default.Send(mMsg, MsgToken.CarrierStatusUpdate);
        }
        #endregion


        private void ServerSend(byte devid, DevCarrier dev)
        {
            mServer?.SendMessage(devid, dev);
        }

        public void SetCurrentSite(byte deviceID, ushort poscode)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimCarrierTask task = DevList.Find(c => c.DevId == deviceID);

                    if (task != null)
                    {
                        task.UpdateCurrentSite(poscode);
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void SetOperation(byte deviceID, DevOperateModeE mode)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimCarrierTask task = DevList.Find(c => c.DevId == deviceID);

                    if (task != null)
                    {
                        task.DevStatus.OperateMode = mode;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        public void SetMoveStatus(byte deviceID, DevCarrierStatusE mode)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    SimCarrierTask task = DevList.Find(c => c.DevId == deviceID);

                    if (task != null)
                    {
                        task.DevStatus.DeviceStatus = mode;
                    }
                }
                finally { Monitor.Exit(_obj); }
            }
        }
    }
}
