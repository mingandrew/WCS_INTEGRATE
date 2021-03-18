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
    public class SimCarrierMaster
    {
        #region[字段]
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
            _obj = new object();
            DevList = new List<SimCarrierTask>();
            Messenger.Default.Register<SocketMsgMod>(this, MsgToken.SimCarrierMsgUpdate, SimCarrierMsgUpdate);
        }

        public void Start()
        {
            mServer = new SimCarrierServer(2003);

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

        private void SimCarrierMsgUpdate(SocketMsgMod mod)
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

                            Device dev = PubMaster.Device.GetDeviceByMemo(mod.Devid + "");
                            ConfigCarrier devconf = PubMaster.DevConfig.GetCarrier(dev.id);
                            task.Device = dev;
                            task.DevConfig = devconf;
                            task.DevId = mod.Devid;
                            task.DevStatus.ID = mod.Devid;
                            task.UpdateCurrentSite(devconf.sim_init_site, devconf.sim_init_point);
                            task.SetUpInit();
                            DevList.Add(task);
                            SendDevMsg(task);
                        }

                        if (task != null)
                        {
                            task.ConnStatus = mod.ConnStatus;
                            if (mod.Device is CarrierCmd cmd)
                            {
                                CheckDev(task, cmd);
                            }
                        }
                    }
                    finally { Monitor.Exit(_obj); }
                }
            }
        }

        internal bool ExistOnFerry(uint ferryTrackId)
        {
            return DevList.Exists(c => c.NowTrack?.id == ferryTrackId);
        }

        private void CheckDev(SimCarrierTask task, CarrierCmd cmd)
        {

            switch (cmd.Command)
            {
                case DevCarrierCmdE.查询:
                    break;
                case DevCarrierCmdE.执行指令:
                    #region[执行任务]
                    switch (cmd.CarrierOrder)
                    {
                        case DevCarrierOrderE.无:
                            break;
                        case DevCarrierOrderE.定位指令:

                            break;
                        case DevCarrierOrderE.取砖指令:

                            break;
                        case DevCarrierOrderE.放砖指令:

                            break;
                        case DevCarrierOrderE.前进倒库:

                            break;
                        case DevCarrierOrderE.后退倒库:

                            break;
                        case DevCarrierOrderE.终止指令:
                            task.DevStatus.DeviceStatus = DevCarrierStatusE.停止;
                            task.DevStatus.CurrentOrder = DevCarrierOrderE.终止指令;
                            task.DevStatus.FinishOrder = DevCarrierOrderE.终止指令;
                            break;
                        case DevCarrierOrderE.异常:

                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case DevCarrierCmdE.设复位点:

                    break;
                case DevCarrierCmdE.终止指令:
                    break;
                default:
                    break;
            }

            ServerSend(task.DevId, task.DevStatus);
        }

        #endregion

        #region[检查设备状态]

        #endregion

        private void ServerSend(byte devid, DevCarrier dev)
        {
            mServer?.SendMessage(devid, dev);
        }

        public void SetCurrentSite(uint deviceID, ushort initsite, ushort initpoint)
        {
            SimCarrierTask task = DevList.Find(c => c.DevId == deviceID);

            if (task != null)
            {
                task.UpdateCurrentSite(initsite, initpoint);
            }
        }

        public void SetOperation(byte deviceID, DevOperateModeE mode)
        {
            SimCarrierTask task = DevList.Find(c => c.DevId == deviceID);

            if (task != null)
            {
                task.DevStatus.OperateMode = mode;
            }
        }

        public void SetMoveStatus(byte deviceID, DevCarrierStatusE mode)
        {
            SimCarrierTask task = DevList.Find(c => c.DevId == deviceID);

            if (task != null)
            {
                task.DevStatus.DeviceStatus = mode;
            }
        }

        #region[发送信息]

        private void SendDevMsg(SimTaskBase task)
        {
            Messenger.Default.Send(task, MsgToken.SimDeviceStatusUpdate);
        }

        #endregion
    }
}
