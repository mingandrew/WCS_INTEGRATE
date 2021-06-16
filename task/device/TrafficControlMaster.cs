using enums;
using enums.track;
using enums.warning;
using module.device;
using module.goods;
using module.track;
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
            // 只拿运输车交管摆渡车类型的
            TrafficCtlList.AddRange(PubMaster.Mod.TrafficCtlSql.QueryTrafficCtlList().FindAll(c=>c.TrafficControlType == TrafficControlTypeE.运输车交管摆渡车));
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
                                    if (ctl.TrafficControlStatus == TrafficControlStatusE.初始化)
                                    {
                                        SetStatus(ctl, TrafficControlStatusE.交管中, "开始交管！");
                                        continue;
                                    }

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
            if (Monitor.TryEnter(_in, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    if (TrafficCtlList.Exists(c => c.TrafficControlType == tc.TrafficControlType &&
                                                                 c.restricted_id == tc.restricted_id &&
                                                                 c.control_id == tc.control_id &&
                                                                 c.TrafficControlStatus != TrafficControlStatusE.已完成))
                    {
                        result = "已经存在相同交管！";
                        return true;
                    }

                    tc.id = PubMaster.Dic.GenerateID(DicTag.NewTrafficCtlId);
                    tc.TrafficControlStatus = TrafficControlStatusE.初始化;
                    tc.create_time = DateTime.Now;
                    PubMaster.Mod.TrafficCtlSql.AddTrafficCtl(tc);
                    TrafficCtlList.Add(tc);
                    result = string.Format("生成交管[ {0} ]{1}", tc.id, tc.TrafficControlType);

                    switch (tc.TrafficControlType)
                    {
                        case TrafficControlTypeE.运输车交管运输车:
                            break;

                        case TrafficControlTypeE.摆渡车交管摆渡车:
                            result = string.Format("{0}: 摆渡车[ {1} ], 移动[ {2} >> {3} ] - 被交管摆渡车[ {4} ]", result,
                                PubMaster.Device.GetDeviceName(tc.control_id), 
                                PubMaster.Track.GetTrackName(tc.from_track_id), 
                                PubMaster.Track.GetTrackName(tc.to_track_id),
                                PubMaster.Device.GetDeviceName(tc.restricted_id));

                            ClueTransForFerry(tc); // 更新到任务界面
                            break;

                        case TrafficControlTypeE.运输车交管摆渡车:
                            result = string.Format("{0}: 运输车[ {1} ], 当前轨道[ {2} ], 指令涉及轨道[ {3} ] - 被交管摆渡车[ {4} ]", result,
                                PubMaster.Device.GetDeviceName(tc.control_id),
                                PubMaster.Track.GetTrackName(tc.from_track_id),
                                PubMaster.Track.GetTrackName(tc.to_track_id),
                                PubMaster.Device.GetDeviceName(tc.restricted_id));
                            break;

                        case TrafficControlTypeE.摆渡车交管运输车:
                            break;
                        default:
                            break;
                    }

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
            return TrafficCtlList.Exists(c => c.TrafficControlStatus != TrafficControlStatusE.已完成 && c.restricted_id == restricted_id);
        }

        /// <summary>
        /// 是否存在设备已被同类型交管
        /// </summary>
        /// <param name="tct"></param>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool ExistsTrafficControl(TrafficControlTypeE tct, uint devid, out uint otherDevid)
        {
            otherDevid = TrafficCtlList.Find(c => c.TrafficControlStatus != TrafficControlStatusE.已完成 && c.TrafficControlType == tct && c.restricted_id == devid)?.control_id ?? 0;

            if (otherDevid == 0)
            {
                otherDevid = TrafficCtlList.Find(c => c.TrafficControlStatus != TrafficControlStatusE.已完成 && c.TrafficControlType == tct && c.control_id == devid)?.restricted_id ?? 0;
            }

            return otherDevid > 0;
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

            if (!memo.Equals(ctl.Memo))
            {
                mLog.Status(true, string.Format("交管ID[ {0} ], 状态[ {1} ], 交管车[ {2} ], 被交管车[ {3} ], 备注[ {4} ]", ctl.id, ctl.TrafficControlStatus,
                    PubMaster.Device.GetDeviceName(ctl.control_id),
                    PubMaster.Device.GetDeviceName(ctl.restricted_id),
                    memo));
                ctl.Memo = memo;
            }

            if (ctl.TrafficControlType == TrafficControlTypeE.摆渡车交管摆渡车 && status == TrafficControlStatusE.已完成)
            {
                ClueTransForFerry(ctl); // 更新到任务界面
            }
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
                string result = "", res = "";

                // 交管车当前位置是否满足结束交管条件
                if (IsMeetLocationForFerry(ctl.control_id, ctl.from_track_id, ctl.to_track_id, out result))
                {
                    SetStatus(ctl, TrafficControlStatusE.已完成, result);
                    return;
                }

                // 是否允许交管摆渡车移动
                if (IsAllowToMoveForFerry(ctl.control_id, ctl.to_track_id, out res))
                {
                    // 让交管车定位到结束点
                    if (PubTask.Ferry.DoLocateFerry(ctl.control_id, ctl.to_track_id, out res))
                    {
                        //SetStatus(ctl, TrafficControlStatusE.已完成, result);
                        //return;
                    }
                }

                // 记录一笔信息
                SetStatus(ctl, TrafficControlStatusE.交管中, string.Format("{0}, 车况[ {1} ]", result, res));
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
                string res = "";

                // 关闭 直接完成
                if (!PubMaster.Dic.IsSwitchOnOff(DicTag.EnableCarrierTraffic))
                {
                    res = "关闭 - 运输车交管摆渡车";
                    goto FinallyLogYes;
                }

                CarrierTask carrier = PubTask.Carrier.GetDevCarrier(ctl.control_id);
                FerryTask ferry = PubTask.Ferry.GetFerry(ctl.restricted_id);

                #region 解锁
                // 常规解锁
                if (PubTask.Carrier.IsStopFTask(ctl.control_id)
                    && (carrier.Position == DevCarrierPositionE.在轨道上 || carrier.Position == DevCarrierPositionE.在摆渡上)
                    )
                {
                    // 在摆渡上但运输车当前轨道与交管摆渡不符-报警
                    if (carrier.Position == DevCarrierPositionE.在摆渡上 && carrier.CurrentTrackId != ferry.DevConfig.track_id)
                    {
                        PubMaster.Warn.AddTaskWarn(ferry.AreaId, ferry.Line, WarningTypeE.CarrierFreeInFerryButLocErr, (ushort)ctl.control_id, ctl.id, PubMaster.Device.GetDeviceName(ctl.restricted_id));
                        goto FinallyLogNo;
                    }

                    res = string.Format("[ ✔ ]满足条件]: 运输车[ {0} ], 位置状态[ {1} ], 当前轨道[ {2} ] - 允许解锁摆渡车[ {3} ]",
                        PubMaster.Device.GetDeviceName(ctl.control_id),
                        carrier.Position,
                        PubMaster.Track.GetTrackName(carrier.CurrentTrackId),
                        PubMaster.Device.GetDeviceName(ctl.restricted_id));
                    goto FinallyLogYes;
                }

                // 指令中解锁
                if (!PubTask.Carrier.IsStopFTask(ctl.control_id)
                    && carrier.Position == DevCarrierPositionE.在轨道上
                    && carrier.InTask(DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库, DevCarrierOrderE.取砖指令, DevCarrierOrderE.放砖指令, DevCarrierOrderE.定位指令)
                    && carrier.NotInTrack(ferry.DevConfig.track_id))
                {
                    res = string.Format("[ ✔ ]满足条件]: 运输车[ {0} ], 位置状态[ {1} ], 当前轨道[ {2} ], 执行指令[ {3} ] - 允许解锁摆渡车[ {4} ]",
                        PubMaster.Device.GetDeviceName(ctl.control_id),
                        carrier.Position,
                        PubMaster.Track.GetTrackName(carrier.CurrentTrackId),
                        carrier.CurrentOrder,
                        PubMaster.Device.GetDeviceName(ctl.restricted_id));
                    goto FinallyLogYes;
                }

                // 异常解锁
                if (!carrier.IsWorking && !carrier.IsEnable && carrier.Position != DevCarrierPositionE.上下摆渡中)
                {
                    res = string.Format("[ ✔ ]满足条件]: 运输车[ {0} ]断连停用 - 允许解锁摆渡车[ {1} ]",
                        PubMaster.Device.GetDeviceName(ctl.control_id),
                        PubMaster.Device.GetDeviceName(ctl.restricted_id));
                    goto FinallyLogYes;
                }

                #endregion

                #region 持续锁定
                // 锁定返回
                FinallyLogNo:

                // 空闲但上下摆渡中-报警
                if (PubTask.Carrier.IsStopFTask(ctl.control_id) && carrier.Position == DevCarrierPositionE.上下摆渡中)
                {
                    PubMaster.Warn.AddTaskWarn(carrier.AreaId, carrier.Line, WarningTypeE.CarrierFreeButMoveInFerry, (ushort)ctl.control_id, ctl.id, PubMaster.Device.GetDeviceName(ctl.restricted_id));
                }

                // 数据全发
                res = string.Format(@"[ ❌ ]：
        {0}
        {1}
        运输车当前状态不允许解锁摆渡车", carrier.GetInfo(), ferry.GetInfo());

                // 记录一笔信息
                SetStatus(ctl, TrafficControlStatusE.交管中, res);
                return;

                #endregion

                // 解锁返回
                FinallyLogYes:
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.CarrierFreeButMoveInFerry, ctl.id);
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.CarrierFreeInFerryButLocErr, ctl.id);
                SetStatus(ctl, TrafficControlStatusE.已完成, res);
                return;
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
            if (nowTraid == 0 || nowTraid.Equals(0) || nowTraid.CompareTo(0) == 0)
            {
                result = string.Format("[ ❌ ]摆渡车[ {0} ]没有当前轨道数据",
                    PubMaster.Device.GetDeviceName(ferryid));
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
                result = string.Format("[ ❌ ]未获取到轨道[ {0} ]的相对位置序号",
                    PubMaster.Track.GetTrackName(nowTraid));
                return false;
            }

            if (Forder == 0 || Forder.Equals(0) || Forder.CompareTo(0) == 0)
            {
                result = string.Format("[ ❌ ]未获取到轨道[ {0} ]的相对位置序号",
                    PubMaster.Track.GetTrackName(fromTraid));
                return false;
            }

            if (Torder == 0 || Torder.Equals(0) || Torder.CompareTo(0) == 0)
            {
                result = string.Format("[ ❌ ]未获取到轨道[ {0} ]的相对位置序号",
                    PubMaster.Track.GetTrackName(toTraid));
                return false;
            }
            #endregion

            // 当前位置 需要在移动方向之外
            if ((Torder > Forder && Norder >= Torder)  // 前进方向
                || (Torder < Forder && Norder <= Torder)) // 后退方向
            {
                result = string.Format("[ ✔ ]满足条件: 摆渡车[ {0} ], 交管移序[ {1} - {2} ], 当前位序[ {3} ]",
                    PubMaster.Device.GetDeviceName(ferryid), Forder, Torder, Norder);
                return true;
            }

            result = string.Format("[ ❌ ]未满足条件: 摆渡车[ {0} ], 交管移序[ {1} - {2} ], 当前位序[ {3} ]",
                    PubMaster.Device.GetDeviceName(ferryid), Forder, Torder, Norder);
            return false;
        }

        /// <summary>
        /// 是否允许交管摆渡车移动
        /// </summary>
        /// <returns></returns>
        private bool IsAllowToMoveForFerry(uint ferryid, uint trackid, out string result)
        {
            FerryTask ferry = PubTask.Ferry.GetFerry(ferryid);
            if (!PubTask.Ferry.IsAllowToMove(ferry, trackid, out result))
            {
                result = string.Format("[ ❌ ]摆渡车[ {0} - {1}]", PubMaster.Device.GetDeviceName(ferryid), result);
                return false;
            }
            else
            {
                result = string.Format("[ ✔ ]摆渡车[ {0} - 允许移动]", PubMaster.Device.GetDeviceName(ferryid));
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
                        result = string.Format("[ ❌ ]摆渡车[ {0} - 没有当前位置信息]", PubMaster.Device.GetDeviceName(ferryid));
                        return false;
                    }

                    // 空车 - 在运输车对应位置 则不能移动
                    uint Ctraid = PubTask.Carrier.GetCarrierTrackID(trans.carrier_id);
                    if (Ftraids.Contains(Ctraid))
                    {
                        result = string.Format("[ ❌ ]摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]", 
                            PubMaster.Device.GetDeviceName(ferryid), trans.id,
                            PubMaster.Device.GetDeviceName(trans.carrier_id));
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
                                    result = string.Format("[ ❌ ]自动任务取砖流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                        PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                        PubMaster.Device.GetDeviceName(trans.carrier_id));
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.放砖流程 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务放砖流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                        PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                        PubMaster.Device.GetDeviceName(trans.carrier_id));
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.取消 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务取消流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                        PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                        PubMaster.Device.GetDeviceName(trans.carrier_id));
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
                                        result = string.Format("[ ❌ ]自动任务取砖流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                            PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                            PubMaster.Device.GetDeviceName(trans.carrier_id));
                                        return false;
                                    }
                                    // 运输车载货 需要放砖
                                    if (PubTask.Carrier.IsLoad(trans.carrier_id) && Ftraids.Contains(trans.give_track_id))
                                    {
                                        result = string.Format("[ ❌ ]自动任务放砖流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                            PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                            PubMaster.Device.GetDeviceName(trans.carrier_id));
                                        return false;
                                    }
                                }
                                if (trans.TransStaus == TransStatusE.还车回轨 && Ftraids.Contains(trans.finish_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务还车回轨流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                            PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                            PubMaster.Device.GetDeviceName(trans.carrier_id));
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.取消 && Ftraids.Contains(trans.take_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务取消流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                            PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                            PubMaster.Device.GetDeviceName(trans.carrier_id));
                                    return false;
                                }
                                break;
                            case TransTypeE.倒库任务:
                            case TransTypeE.移车任务:
                                if (trans.TransStaus == TransStatusE.移车中 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = string.Format("[ ❌ ]自动任务移车流程中, 摆渡车[ {0} - 被任务[ {1} ]锁定, 等待运输车[ {2} ]作业]",
                                            PubMaster.Device.GetDeviceName(ferryid), trans.id,
                                            PubMaster.Device.GetDeviceName(trans.carrier_id));
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

        #region [获取交管信息]

        /// <summary>
        /// 提示任务更新交管信息
        /// </summary>
        /// <param name="tc"></param>
        private void ClueTransForFerry(TrafficControl tc)
        {
            uint resid = PubTask.Ferry.GetFerryTransId(tc.restricted_id);
            if (resid > 0)
            {
                PubTask.Trans.ClueViewByTransID(resid);
            }

            uint conid = PubTask.Ferry.GetFerryTransId(tc.control_id);
            if (conid > 0)
            {
                PubTask.Trans.ClueViewByTransID(conid);
            }
        }

        /// <summary>
        /// 获取摆渡车交管摆渡车信息
        /// </summary>
        /// <param name="ferryid"></param>
        /// <returns></returns>
        public string GetTrafficCtlInfoForFerry(uint ferryid)
        {
            string res = "";
            List<TrafficControl> tcs = TrafficCtlList.FindAll(c => c.TrafficControlType == TrafficControlTypeE.摆渡车交管摆渡车
                && (c.restricted_id == ferryid || c.control_id == ferryid));
            if (tcs != null && tcs.Count > 0)
            {
                foreach (TrafficControl item in tcs)
                {
                    if (item.TrafficControlStatus == TrafficControlStatusE.已完成)
                    {
                        continue;
                    }

                    res = string.Format(@"{0}[{1}]❌-等[{2}]移到[{3}]或之外；", res,
                        PubMaster.Device.GetDeviceName(item.restricted_id),
                        PubMaster.Device.GetDeviceName(item.control_id),
                        PubMaster.Track.GetTrackName(item.to_track_id));
                }
            }

            return res;
        }

        #endregion
    }
}
