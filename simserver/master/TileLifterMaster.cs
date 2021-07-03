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
using tool.appconfig;

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
            _mRefresh?.Abort();
            mServer?.Stop();

            #region[保存模拟信息]
            try
            {
                foreach (var item in DevList)
                {
                    GlobalWcsDataConfig.SimulateConfig.UpdateSim(item.SaveSimulate());
                }
            }
            catch { }
            #endregion
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
                            task.SetUpSimulate(GlobalWcsDataConfig.SimulateConfig.GetSimTileLifter(dev.id));
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

        public void SetLoadStatus(uint deviceID, bool setleft, out string shiftmsg)
        {
            shiftmsg = "";
            SimTileLifterTask task = DevList.Find(c => c.ID == deviceID);
            if (task != null)
            {
                if (setleft)//left
                {
                    if (task.DevConfig.left_track_id == 0) return;
                    switch (task.DevStatus.LoadStatus1)
                    {
                        case DevLifterLoadE.无砖:
                            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign))
                            {
                                task.DevStatus.LoadStatus1 = DevLifterLoadE.有砖;
                                shiftmsg = string.Format("[ {0} ]： 切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.无砖, DevLifterLoadE.有砖);
                            }
                            else
                            {
                                task.SetSite1Status(task.DevStatus.SetGoods, task.FULL_QTY, DevLifterLoadE.有砖);
                                shiftmsg = string.Format("[ {0} ]： 切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.无砖, DevLifterLoadE.有砖);
                            }
                            break;
                        case DevLifterLoadE.有砖:
                            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign))
                            {
                                task.SetSite1Status(task.DevStatus.SetGoods, task.FULL_QTY, DevLifterLoadE.满砖);
                                shiftmsg = string.Format("[ {0} ]：切换：[ {1} ] - > [ {2} ], 数量：[ {3} ]", task.Device.name, DevLifterLoadE.有砖, DevLifterLoadE.满砖, task.FULL_QTY);
                            }
                            else
                            {
                                task.SetSite1Status(0, 0, DevLifterLoadE.无砖);
                                shiftmsg = string.Format("[ {0} ]：切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.有砖, DevLifterLoadE.无砖);
                            }
                            break;
                        case DevLifterLoadE.满砖:
                            task.SetSite1Status(0, 0, DevLifterLoadE.无砖);
                            shiftmsg = string.Format("[ {0} ]：切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.满砖, DevLifterLoadE.无砖);
                            break;
                    }
                }
                else if (!setleft)
                {
                    if (task.DevConfig.right_track_id == 0) return;
                    switch (task.DevStatus.LoadStatus2)
                    {
                        case DevLifterLoadE.无砖:
                            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign))
                            {
                                task.DevStatus.LoadStatus2 = DevLifterLoadE.有砖;
                                shiftmsg = string.Format("[ {0} ]： 切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.无砖, DevLifterLoadE.有砖);
                            }
                            else
                            {
                                task.SetSite2Status(task.DevStatus.SetGoods, task.FULL_QTY, DevLifterLoadE.有砖);
                                shiftmsg = string.Format("[ {0} ]： 切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.无砖, DevLifterLoadE.有砖);
                            }
                            break;
                        case DevLifterLoadE.有砖:
                            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign))
                            {
                                task.SetSite2Status(task.DevStatus.SetGoods, task.FULL_QTY, DevLifterLoadE.满砖);
                                shiftmsg = string.Format("[ {0} ]：切换：[ {1} ] - > [ {2} ], 数量：[ {3} ]", task.Device.name, DevLifterLoadE.有砖, DevLifterLoadE.满砖, task.FULL_QTY);
                            }
                            else
                            {
                                task.SetSite2Status(0, 0, DevLifterLoadE.无砖);
                                shiftmsg = string.Format("[ {0} ]：切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.有砖, DevLifterLoadE.无砖);
                            }
                            break;
                        case DevLifterLoadE.满砖:
                            task.SetSite2Status(0, 0, DevLifterLoadE.无砖);
                            shiftmsg = string.Format("[ {0} ]：切换：[ {1} ] - > [ {2} ]", task.Device.name, DevLifterLoadE.满砖, DevLifterLoadE.无砖);
                            break;
                    }
                }
            }
        }

        public void SetLoadStatusNeed(uint deviceID, bool need, bool setleft)
        {
            SimTileLifterTask task = DevList.Find(c => c.ID == deviceID);
            if (task != null)
            {
                if (setleft)//left
                {
                    task.IsNeed_1 = !task.IsNeed_1;
                }
                else if (!setleft)
                {
                    task.IsNeed_2 = !task.IsNeed_2;
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

        public void SetBackUpDevice(uint deviceID, byte backdevcode)
        {
            SimTileLifterTask task = DevList.Find(c => c.ID == deviceID);
            if (task != null)
            {
                task.DevStatus.BackupShiftDev = backdevcode;
            }
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
                #region[查询]
                case DevLifterCmdTypeE.查询:
                    break;
                #endregion

                #region[介入1]
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
                                task.DevStatus.Goods1 = 0;
                                task.DevStatus.LoadStatus1 = PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign) ? DevLifterLoadE.满砖 : DevLifterLoadE.有砖;
                            }
                            task.IsInvo_1 = false;
                        }
                        else
                        {
                            task.IsInvo_1 = true;
                        }
                    }
                    break;
                #endregion

                #region[介入2]
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
                                task.DevStatus.Goods2 = 0;
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
                                task.DevStatus.LoadStatus2 = PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign) ? DevLifterLoadE.满砖 : DevLifterLoadE.有砖;
                            }
                            task.IsInvo_2 = false;
                        }
                        else
                        {
                            task.IsInvo_2 = true;
                        }
                    }
                    break;
                #endregion

                #region[转产]
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
                #endregion

                #region[模式]
                case DevLifterCmdTypeE.模式:
                    task.DevStatus.WorkMode = cmd.WorkMode;
                    if (cmd.SetFullType == TileFullE.设为满砖)
                    {
                        if (task.IsGood_1 || task.IsFull_1 || task.DevStatus.Site1Qty >0)
                        {
                            task.DevStatus.Need1 = true;
                        }

                        if (task.IsGood_2 || task.IsFull_2 || task.DevStatus.Site2Qty > 0)
                        {
                            task.DevStatus.Need2 = true;
                        }
                    }
                    break;
                #endregion

                #region[等级]
                case DevLifterCmdTypeE.等级:
                    task.DevStatus.SetLevel = cmd.Level;

                    break;
                #endregion

                #region[复位转产]
                case DevLifterCmdTypeE.复位转产:
                    task.DevStatus.NeedSytemShift = false;
                    break;
                #endregion

                #region[复位转产]
                case DevLifterCmdTypeE.开关灯:
                    task.DevStatus.AlertLightStatus = cmd.Value1;
                    break;
                #endregion

                #region[复位转产]
                default:
                    break;
                #endregion
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
