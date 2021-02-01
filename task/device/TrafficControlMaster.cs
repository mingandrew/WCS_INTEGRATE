using enums;
using module.device;
using module.goods;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using task.task;
using tool.mlog;
using tool.timer;

namespace task.device
{
    public class TrafficControlMaster
    {
        #region[字段]
        private readonly object _in, _out;
        private Thread _mRefresh;
        private bool Refreshing = true;
        private MTimer mTimer;
        private List<TrafficControl> TrafficCtlList { set; get; }
        private Log mLog;
        #endregion

        #region[构造/初始化]

        public TrafficControlMaster()
        {
            mLog = (Log)new LogFactory().GetLog("交通管制", false);
            _in = new object();
            _out = new object();
            mTimer = new MTimer();
            TrafficCtlList = new List<TrafficControl>();
            Init();
        }

        private void Init()
        {
            TrafficCtlList.Clear();
            TrafficCtlList.AddRange(PubMaster.Mod.TrafficCtlSql.QueryTrafficCtlList());
        }

        public void Start()
        {
            if (_mRefresh == null || !_mRefresh.IsAlive || _mRefresh.ThreadState == ThreadState.Aborted)
            {
                _mRefresh = new Thread(Handle)
                {
                    IsBackground = true
                };
            }

            _mRefresh.Start();
        }

        public void Handle()
        {
            while (Refreshing)
            {
                if (Monitor.TryEnter(_in, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        TrafficCtlList.RemoveAll(c => c.TrafficControlStatus == TrafficControlStatusE.已完成);
                        if (TrafficCtlList != null || TrafficCtlList.Count != 0)
                        {
                            foreach (TrafficControl ctl in TrafficCtlList)
                            {
                                try
                                {
                                    switch (ctl.TrafficControlType)
                                    {
                                        case TrafficControlTypeE.运输车交管运输车:
                                            break;
                                        case TrafficControlTypeE.摆渡车交管摆渡车:
                                            // 是否允许摆渡车移动
                                            if (IsAllowToMoveForFerry(ctl.control_id, out string result))
                                            {
                                                // 让交管车定位到结束点
                                                if (PubTask.Ferry.DoLocateFerry(ctl.control_id, ctl.to_track_id, out result))
                                                {
                                                    SetStatus(ctl, TrafficControlStatusE.已完成);
                                                }
                                            }
                                            break;
                                        case TrafficControlTypeE.运输车交管摆渡车:
                                            break;
                                        case TrafficControlTypeE.摆渡车交管运输车:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    mLog.Error(true, "[ID:" + ctl?.id + "]", e);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        mLog.Error(true, e.Message, e);
                    }
                    finally
                    {
                        Monitor.Exit(_in);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            Refreshing = false;
            _mRefresh?.Abort();
        }
        #endregion

        #region[获取对象]

        public List<TrafficControl> GetTrafficCtlList()
        {
            return TrafficCtlList;
        }

        public List<TrafficControl> GetTrafficCtlList(TrafficControlTypeE type)
        {
            return TrafficCtlList.FindAll(c => c.TrafficControlType == type);
        }

        public List<TrafficControl> GetTrafficCtlList(List<TrafficControlTypeE> types)
        {
            return TrafficCtlList.FindAll(c => types.Contains(c.TrafficControlType));
        }

        public List<TrafficControl> GetTrafficCtlList(List<TrafficControlTypeE> types, uint areaid)
        {
            return TrafficCtlList.FindAll(c => c.area == areaid && types.Contains(c.TrafficControlType));
        }

        public List<TrafficControl> GetTrafficCtlList(List<TrafficControlTypeE> types, List<uint> areaids)
        {
            return TrafficCtlList.FindAll(c => types.Contains(c.TrafficControlType) && areaids.Contains(c.area));
        }

        #endregion

        #region[方法/判断]

        /// <summary>
        /// 新增交管
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool AddTrafficControl(TrafficControl tc, out string result)
        {
            if (TrafficCtlList.Exists(c => c.TrafficControlType == tc.TrafficControlType &&
                                                         c.restricted_id == tc.restricted_id &&
                                                         c.control_id == tc.control_id &&
                                                         c.TrafficControlStatus != TrafficControlStatusE.已完成))
            {
                result = "已经存在相同交管类型的对应设备！";
                return false;
            }

            if (Monitor.TryEnter(_out, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    tc.id = PubMaster.Dic.GenerateID(DicTag.NewTranId);
                    tc.TrafficControlStatus = TrafficControlStatusE.交管中;
                    tc.create_time = DateTime.Now;
                    PubMaster.Mod.TrafficCtlSql.AddTrafficCtl(tc);
                    TrafficCtlList.Add(tc);
                    result = "";
                    return true;
                }
                finally
                {
                    Monitor.Exit(_out);
                }
            }

            result = "稍后再试！";
            return false;
        }


        /// <summary>
        /// 是否存在设备已被交管
        /// </summary>
        /// <param name="restricted_id"></param>
        /// <returns></returns>
        public bool ExistsRestricted(uint restricted_id)
        {
            return TrafficCtlList.Exists(c => c.TrafficControlStatus == TrafficControlStatusE.交管中 && c.restricted_id == restricted_id);
        }

        /// <summary>
        /// 是否存在设备已被同类型交管
        /// </summary>
        /// <param name="tct"></param>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool ExistsTrafficControl(TrafficControlTypeE tct, uint devid)
        {
            return TrafficCtlList.Exists(c => c.TrafficControlStatus == TrafficControlStatusE.交管中 &&
                c.TrafficControlType == tct && (c.restricted_id == devid || c.control_id == devid));
        }


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="status"></param>
        /// <param name="memo"></param>
        internal void SetStatus(TrafficControl ctl, TrafficControlStatusE status, string memo = "")
        {
            if (ctl.TrafficControlStatus != status)
            {
                mLog.Status(true, string.Format("交管：{0}，原状态：{1}, 新状态：{2}, 备注：{3}", ctl.id, ctl.TrafficControlStatus, status, memo));
                ctl.TrafficControlStatus = status;
                PubMaster.Mod.TrafficCtlSql.EditTrafficCtl(ctl, TrafficControlUpdateE.Status);
            }
        }

        #endregion

        #region [ 交管摆渡车是否允许移动 ]

        /// <summary>
        /// 是否允许交管摆渡车移动
        /// </summary>
        /// <returns></returns>
        private bool IsAllowToMoveForFerry(uint ferryid, out string result)
        {
            // 是否存在被运输车交管
            if (ExistsTrafficControl(TrafficControlTypeE.运输车交管摆渡车, ferryid))
            {
                result = "被运输车交管中！";
                return false;
            }

            FerryTask ferry = PubTask.Ferry.GetFerry(ferryid);
            if (!PubTask.Ferry.IsAllowToMove(ferry, out result))
            {
                return false;
            }
            uint Ftraid = ferry.GetFerryCurrentTrackId();
            // 是否锁定任务 判断任务节点是否允许移动
            if (ferry.IsLock && ferry.TransId != 0)
            {
                StockTrans trans = PubTask.Trans.GetTrans(ferry.TransId);
                if (trans != null)
                {
                    // 空车 - 在运输车对应位置 则不能移动
                    uint Ctraid = PubTask.Carrier.GetCarrierTrackID(trans.carrier_id);
                    if (Ftraid == Ctraid)
                    {
                        result = "对应运输车任务待定！";
                        return false;
                    }

                    // 载车 - 在任务的对应位置 则不能移动
                    if (ferry.Load == DevFerryLoadE.载车)
                    {
                        switch (trans.TransType)
                        {
                            case TransTypeE.下砖任务:
                            case TransTypeE.手动下砖:
                            case TransTypeE.同向下砖:
                                if (trans.TransStaus == TransStatusE.取砖流程 && Ftraid == trans.take_track_id)
                                {
                                    result = "准备取货！";
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.放砖流程 && Ftraid == trans.give_track_id)
                                {
                                    result = "准备卸货！";
                                    return false;
                                }
                                break;
                            case TransTypeE.上砖任务:
                            case TransTypeE.手动上砖:
                            case TransTypeE.同向上砖:
                                if (trans.TransStaus == TransStatusE.取砖流程)
                                {
                                    // 运输车无货 需要取砖
                                    if (PubTask.Carrier.IsNotLoad(trans.carrier_id) && Ftraid == trans.take_track_id)
                                    {
                                        result = "准备取货！";
                                        return false;
                                    }
                                    // 运输车载货 需要放砖
                                    if (PubTask.Carrier.IsLoad(trans.carrier_id) && Ftraid == trans.give_track_id)
                                    {
                                        result = "准备卸货！";
                                        return false;
                                    }
                                }
                                if (trans.TransStaus == TransStatusE.还车回轨 && Ftraid == trans.finish_track_id)
                                {
                                    result = "准备还车回轨！";
                                    return false;
                                }
                                break;
                            case TransTypeE.倒库任务:
                            case TransTypeE.移车任务:
                                if (trans.TransStaus == TransStatusE.移车中 && Ftraid == trans.give_track_id)
                                {
                                    result = "准备移车！";
                                    return false;
                                }
                                break;
                        }
                    }

                }
            }

            return true;
        }

        #endregion

    }
}
