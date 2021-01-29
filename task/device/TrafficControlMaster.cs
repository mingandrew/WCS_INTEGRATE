using enums;
using module.device;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
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
                        if (TrafficCtlList != null || TrafficCtlList.Count !=0)
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
                                            // 是否存在被运输车交管
                                            if (ExistsRestricted(TrafficControlTypeE.运输车交管摆渡车, ctl.control_id))
                                            {
                                                continue;
                                            }
                                            // 让交管车定位到结束点
                                            if (PubTask.Ferry.DoLocateFerry(ctl.control_id, ctl.to_track_id, out string res))
                                            {
                                                SetStatus(ctl, TrafficControlStatusE.已完成);
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
        /// <param name="restricted_id"></param>
        /// <returns></returns>
        public bool ExistsRestricted(TrafficControlTypeE tct, uint restricted_id)
        {
            return TrafficCtlList.Exists(c => c.TrafficControlStatus == TrafficControlStatusE.交管中 &&
                c.TrafficControlType == tct && c.restricted_id == restricted_id);
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

    }
}
