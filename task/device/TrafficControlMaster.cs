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
            //Init(); // 不管旧的
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
                                            ControlCarrierByCarrier(ctl);
                                            break;
                                        case TrafficControlTypeE.摆渡车交管摆渡车:
                                            ControlFerryByFerry(ctl);
                                            break;
                                        case TrafficControlTypeE.运输车交管摆渡车:
                                            ControlFerryByCarrier(ctl);
                                            break;
                                        case TrafficControlTypeE.摆渡车交管运输车:
                                            ControlCarrierByFerry(ctl);
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
            if (Monitor.TryEnter(_in, TimeSpan.FromSeconds(3)))
            {
                try
                {
                    if (TrafficCtlList.Exists(c => c.TrafficControlType == tc.TrafficControlType &&
                                                                 c.restricted_id == tc.restricted_id &&
                                                                 c.control_id == tc.control_id &&
                                                                 c.TrafficControlStatus != TrafficControlStatusE.已完成))
                    {
                        result = "已经存在相同交管！";
                        return false;
                    }

                    tc.id = PubMaster.Dic.GenerateID(DicTag.NewTrafficCtlId);
                    tc.TrafficControlStatus = TrafficControlStatusE.交管中;
                    tc.create_time = DateTime.Now;
                    PubMaster.Mod.TrafficCtlSql.AddTrafficCtl(tc);
                    TrafficCtlList.Add(tc);
                    result = string.Format("生成交管[ {0} ]：摆渡车ID[ {1} ]_轨道ID[ {2} >> {3} ]", tc.id, tc.control_id, tc.from_track_id, tc.to_track_id);
                    return true;
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
                ctl.TrafficControlStatus = status;
                ctl.update_time = System.DateTime.Now;
                PubMaster.Mod.TrafficCtlSql.EditTrafficCtl(ctl, TrafficControlUpdateE.Status);
            }
            mLog.Status(true, string.Format("交管[ {0} ], 状态[ {1} ], 交管车[ {2} ], 备注[ {3} ]", ctl.id, ctl.TrafficControlStatus, ctl.control_id, memo));
        }

        #endregion

        #region [交管逻辑]

        /// <summary>
        /// 运输车交管运输车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlCarrierByCarrier(TrafficControl ctl)
        {
            try
            {

            }
            catch (Exception ex)
            {
                mLog.Error(true, ctl.id + " >>" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 摆渡车交管摆渡车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlFerryByFerry(TrafficControl ctl)
        {
            try
            {
                string result, res;

                // 是否存在被运输车交管
                if (ExistsTrafficControl(TrafficControlTypeE.运输车交管摆渡车, ctl.control_id))
                {
                    result = "[ ❌ ]存在运输车交管！";
                    return;
                }

                // 交管车当前位置是否满足结束交管条件
                if (IsMeetLocationForFerry(ctl.control_id, ctl.from_track_id, ctl.to_track_id, out result))
                {
                    SetStatus(ctl, TrafficControlStatusE.已完成, result);
                    return;
                }
                else
                {
                    // 是否允许交管摆渡车移动
                    if (IsAllowToMoveForFerry(ctl.control_id, out res))
                    {
                        // 让交管车定位到结束点
                        if (PubTask.Ferry.DoLocateFerry(ctl.control_id, ctl.to_track_id, out res))
                        {
                            //SetStatus(ctl, TrafficControlStatusE.已完成, result);
                            //return;
                        }
                    }
                }

                // 记录一笔信息
                SetStatus(ctl, TrafficControlStatusE.交管中, string.Format("{0}, 摆渡车况[ {1} ]", result, res));
            }
            catch (Exception ex)
            {
                mLog.Error(true, ctl.id + " >>" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 运输车交管摆渡车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlFerryByCarrier(TrafficControl ctl)
        {
            try
            {

            }
            catch (Exception ex)
            {
                mLog.Error(true, ctl.id + " >>" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 摆渡车交管运输车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlCarrierByFerry(TrafficControl ctl)
        {
            try
            {

            }
            catch (Exception ex)
            {
                mLog.Error(true, ctl.id + " >>" + ex.Message, ex);
            }
        }

        #endregion

        #region [ 交管摆渡车判断 ]

        /// <summary>
        /// 是否满足交管摆渡车位置条件
        /// </summary>
        /// <returns></returns>
        private bool IsMeetLocationForFerry(uint ferryid, uint fromTraid, uint toTraid, out string result)
        {
            // 当前轨道ID
            uint nowTraid = PubTask.Ferry.GetFerryCurrentTrackId(ferryid);
            if (nowTraid == 0)
            {
                result = string.Format("[ ❌ ]没有当前轨道数据, 摆渡车ID[ {0} ], 轨道ID[ {1} ]",
                    ferryid, nowTraid);
                return false;
            }

            // 当前轨道顺序
            short Norder = PubMaster.Track.GetTrackOrder(nowTraid);
            // 起始轨道顺序
            short Forder = PubMaster.Track.GetTrackOrder(fromTraid);
            // 结束轨道顺序
            short Torder = PubMaster.Track.GetTrackOrder(toTraid);

            #region 0 的判断
            if (Norder == 0 || Norder.Equals(0) || Norder.CompareTo(0) == 0)
            {
                result = string.Format("[ ❌ ]未获取到轨道相对位置序号用于判断, 摆渡车ID[ {0} ], 轨道ID[ {1} ]",
                    ferryid, nowTraid);
                return false;
            }

            if (Forder == 0 || Forder.Equals(0) || Forder.CompareTo(0) == 0)
            {
                result = string.Format("[ ❌ ]未获取到轨道相对位置序号用于判断, 摆渡车ID[ {0} ], 轨道ID[ {1} ]",
                    ferryid, fromTraid);
                return false;
            }

            if (Torder == 0 || Torder.Equals(0) || Torder.CompareTo(0) == 0)
            {
                result = string.Format("[ ❌ ]未获取到轨道相对位置序号用于判断, 摆渡车ID[ {0} ], 轨道ID[ {1} ]",
                    ferryid, toTraid);
                return false;
            }
            #endregion

            // 当前位置 需要在移动方向之外
            if ((Torder > Forder && Norder >= Torder)  // 前进方向
                || (Torder < Forder && Norder <= Torder)) // 后退方向
            {
                result = string.Format("[ ✔ ]满足条件, 摆渡车ID[ {0} ], 交管移序[ {1} - {2} ], 当前位序[ {3} ]",
                    ferryid, Forder, Torder, Norder);
                return true;
            }

            result = string.Format("[ ❌ ]未满足条件, 摆渡车ID[ {0} ], 交管移序[ {1} - {2} ], 当前位序[ {3} ]",
                    ferryid, Forder, Torder, Norder);
            return false;
        }

        /// <summary>
        /// 是否允许交管摆渡车移动
        /// </summary>
        /// <returns></returns>
        private bool IsAllowToMoveForFerry(uint ferryid, out string result)
        {
            FerryTask ferry = PubTask.Ferry.GetFerry(ferryid);
            if (!PubTask.Ferry.IsAllowToMove(ferry, out result))
            {
                result = string.Format("[ ❌ ]{0}, 摆渡车ID[ {1} ]]", result, ferryid);
                return false;
            }
            else
            {
                result = string.Format("[ ✔ ]允许移动, 摆渡车ID[ {1} ]]", result, ferryid);
            }

            // 是否锁定任务 判断任务节点是否允许移动
            if (ferry.IsLock && ferry.TransId != 0)
            {
                StockTrans trans = PubTask.Trans.GetTrans(ferry.TransId);
                if (trans != null && !trans.finish)
                {
                    List<uint> Ftraids = ferry.GetFerryCurrentTrackIds();
                    if (Ftraids == null || Ftraids.Count == 0)
                    {
                        result = string.Format("[ ❌ ]没有当前位置信息, 摆渡车ID[ {0} ], 任务ID[ {1} ]", ferryid, trans.id);
                        return false;
                    }

                    // 空车 - 在运输车对应位置 则不能移动
                    uint Ctraid = PubTask.Carrier.GetCarrierTrackID(trans.carrier_id);
                    if (Ftraids.Contains(Ctraid))
                    {
                        result = string.Format("[ ❌ ]等待运输车作业, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]", ferryid, trans.id, trans.carrier_id);
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
                                if (trans.TransStaus == TransStatusE.取砖流程 && Ftraids.Contains(trans.take_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务取砖流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.放砖流程 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务放砖流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.取消 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务取消流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                    return false;
                                }
                                break;
                            case TransTypeE.上砖任务:
                            case TransTypeE.手动上砖:
                            case TransTypeE.同向上砖:
                                if (trans.TransStaus == TransStatusE.取砖流程)
                                {
                                    // 运输车无货 需要取砖
                                    if (PubTask.Carrier.IsNotLoad(trans.carrier_id) && Ftraids.Contains(trans.take_track_id))
                                    {
                                        result = string.Format("[ ❌ ]自动任务取砖流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                        return false;
                                    }
                                    // 运输车载货 需要放砖
                                    if (PubTask.Carrier.IsLoad(trans.carrier_id) && Ftraids.Contains(trans.give_track_id))
                                    {
                                        result = string.Format("[ ❌ ]自动任务放砖流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                        return false;
                                    }
                                }
                                if (trans.TransStaus == TransStatusE.还车回轨 && Ftraids.Contains(trans.finish_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务还车回轨流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.取消 && Ftraids.Contains(trans.take_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务取消流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
                                    return false;
                                }
                                break;
                            case TransTypeE.倒库任务:
                            case TransTypeE.移车任务:
                                if (trans.TransStaus == TransStatusE.移车中 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务移车流程中, 摆渡车ID[ {0} ], 任务ID[ {1} ], 运输车ID[ {2} ]",
                                        ferryid, trans.id, trans.carrier_id);
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
