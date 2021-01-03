using enums;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using tool.mlog;

namespace task
{
    public class WarningMaster
    {

        #region[参数/构造]
        private readonly List<Warning> List;
        private DateTime inittime;
        private bool stopwarnadding;
        private object _obj;
        private Log mlog;
        public WarningMaster()
        {
            _obj = new object();
            List = new List<Warning>();
            mlog = (Log)new LogFactory().GetLog("Warn", false);
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true)
        {
            if (refr_1)
            {
                List.Clear();
                List.AddRange(PubMaster.Mod.WarnSql.QueryWarningList());
            }
            inittime = DateTime.Now;
        }

        public void GetWarns()
        {
            foreach (Warning warning in List)
            {
                SendMsg(warning);
            }
        }

        public void Stop()
        {
            stopwarnadding = true;
        }

        public List<Warning> GetWarnings()
        {
            return List;
        }
        #endregion

        #region[发送信息]

        private void SendMsg(Warning md)
        {
            Messenger.Default.Send(md, MsgToken.WarningUpdate);
        }

        #endregion

        #region[列表操作]
        private void AddWaring(Warning md)
        {
            if (stopwarnadding) return;
            if (md != null && Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    md.id = PubMaster.Dic.GenerateID(DicTag.NewWarnId);
                    md.createtime = DateTime.Now;
                    List.Add(md);
                    SendMsg(md);
                    PubMaster.Mod.WarnSql.AddWarning(md);
                }
                catch (Exception e)
                {
                    mlog.Error(true, e.Message, e);
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
        }

        private void RemoveWarning(Warning md)
        {
            if (md != null && Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    md.resolve = true;
                    md.resolvetime = DateTime.Now;
                    SendMsg(md);
                    List.Remove(md);
                    PubMaster.Mod.WarnSql.EditWarning(md);
                }
                catch (Exception e)
                {
                    mlog.Error(true, e.Message, e);
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
        }

        /// <summary>
        /// 获取致命警告
        /// </summary>
        /// <returns></returns>
        public List<Warning> GetFatalError()
        {
            return List;
        }

        public void RemoveWarning(uint warnid)
        {
            Warning warn = List.Find(c => c.id == warnid);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

        #region[设备警告]

        public void AddDevWarn(WarningTypeE warntype, ushort devid, uint transid = 0, uint trackid = 0, uint otherdevid = 0)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn == null)
            {
                if (stopwarnadding) return;
                if ((DateTime.Now - inittime).TotalSeconds < 20) return;
                warn = new Warning()
                {
                    dev_id = devid,
                    type = (byte)warntype,
                    trans_id = transid,
                    track_id = (ushort)trackid
                };
                string devname = PubMaster.Device.GetDeviceName(devid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString());
                if (trackid > 0)
                {
                    string trackname = PubMaster.Track.GetTrackName(trackid);
                    warn.content = devname + ": (" + trackname + ") " + warnmsg;
                }
                else
                {
                    warn.content = devname + ": " + warnmsg;
                }

                if (otherdevid > 0)
                {
                    warn.content += ": (" + PubMaster.Device.GetDeviceName(otherdevid) + ") ";
                }
                AddWaring(warn);
            }
        }
        public void RemoveDevWarn(WarningTypeE warntype, ushort devid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void RemoveDevWarn(ushort devid)
        {
            Warning warn = List.Find(c => c.dev_id == devid);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void AddCarrierWarn(CarrierWarnE warntype, ushort devid, ushort alertidx)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn == null)
            {
                warn = new Warning()
                {
                    dev_id = devid,
                    type = (byte)warntype,
                    track_id = alertidx
                };
                string devname = PubMaster.Device.GetDeviceName(devid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString());
                warn.content = devname + ": " + warnmsg;
                AddWaring(warn);
            }
        }
        public void RemoveCarrierWarn(CarrierWarnE warntype, ushort devid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void RemoveCarrierWarn(ushort devid, ushort alertidx)
        {
            Warning warn = List.Find(c => c.dev_id == devid && !c.resolve && c.track_id == alertidx);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

        #region[任务警告]

        public void AddTaskWarn(WarningTypeE warntype, ushort devid, uint transid = 0)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn == null)
            {
                if (stopwarnadding) return;
                if ((DateTime.Now - inittime).TotalSeconds < 20) return;
                warn = new Warning()
                {
                    dev_id = devid,
                    type = (byte)warntype,
                    trans_id = transid
                };
                string devname = PubMaster.Device.GetDeviceName(devid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString());
                warn.content = devname + ": " + warnmsg;
                AddWaring(warn);
            }
        }


        #endregion

        #region[轨道警告]

        public void AddTraWarn(WarningTypeE warntype, ushort trackid, string trackname = null)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.track_id == trackid && !c.resolve);
            if (warn == null)
            {
                warn = new Warning()
                {
                    track_id = trackid,
                    type = (byte)warntype,
                };
                string traname = trackname ?? PubMaster.Track.GetTrackName(trackid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString());
                warn.content = traname + ": " + warnmsg;
                AddWaring(warn);
            }
        }

        public void RemoveTraWarn(WarningTypeE warntype, ushort trackid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.track_id == trackid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }

        internal void RemoveTraWarn(ushort trackid)
        {
            Warning warn = List.Find(c => c.track_id == trackid);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }

        #endregion
    }
}
