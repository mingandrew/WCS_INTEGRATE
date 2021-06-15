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
            if (warn != null && warn.type != (byte)WarningTypeE.DownTaskSwitchClosed 
                             && warn.type != (byte)WarningTypeE.UpTaskSwitchClosed
                             && warn.type != (byte)WarningTypeE.SortTaskSwitchClosed)
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
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                if (trackid > 0)
                {
                    string trackname = PubMaster.Track.GetTrackName(trackid);
                    warn.content = devname + ": (" + trackname + ") " + warnmsg;
                }
                else
                {
                    warn.content = devname + ": " + warnmsg;
                }
                warn.level = level;
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

        #region 运输车

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
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = devname + ": " + warnmsg;
                warn.level = level;
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

        #region 摆渡车

        public void AddFerryWarn(FerryWarnE warntype, ushort devid, ushort alertidx)
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
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = devname + ": " + warnmsg;
                warn.level = level;
                AddWaring(warn);
            }
        }
        public void RemoveFerryWarn(FerryWarnE warntype, ushort devid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        public void RemoveFerryWarn(ushort devid, ushort alertidx)
        {
            Warning warn = List.Find(c => c.dev_id == devid && !c.resolve && c.track_id == alertidx);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

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
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = devname + ": " + warnmsg;
                warn.level = level;
                AddWaring(warn);
            }
        }

        public void AddTaskWarn(WarningTypeE warntype, ushort devid, uint transid, string result = "")
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.dev_id == devid && c.trans_id == transid && !c.resolve);
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
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = devname + ": " + warnmsg + " > " + result;
                warn.level = level;
                AddWaring(warn);
            }
        }

        /// <summary>
        /// 清除任务报警
        /// </summary>
        /// <param name="transid"></param>
        public void RemoveTaskWarn(WarningTypeE warntype, uint transid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.trans_id == transid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }

        /// <summary>
        /// 清除任务所有报警
        /// </summary>
        /// <param name="transid"></param>
        public void RemoveTaskAllWarn(uint transid)
        {
            List<Warning> warns = List.FindAll(c => c.trans_id == transid && !c.resolve);
            if (warns != null && warns.Count > 0)
            {
                foreach (Warning item in warns)
                {
                    RemoveWarning(item);
                }
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
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = traname + ": " + warnmsg;
                warn.level = level;
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

        #region[区域警告]

        /// <summary>
        /// 添加区域报警
        /// </summary>
        /// <param name="warntype"></param>
        /// <param name="areaid"></param>
        /// <param name="result"></param>
        public void AddAreaWarn(WarningTypeE warntype, ushort areaid, string result = "")
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.area_id == areaid && !c.resolve);
            if (warn == null)
            {
                if (stopwarnadding) return;
                if ((DateTime.Now - inittime).TotalSeconds < 20) return;
                warn = new Warning()
                {
                    area_id = areaid,
                    type = (byte)warntype,
                };
                string areaName = PubMaster.Area.GetName(areaid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = areaName + ": " + warnmsg + " > " + result;
                warn.level = level;
                AddWaring(warn);
            }
        }

        /// <summary>
        /// 清除区域报警
        /// </summary>
        /// <param name="transid"></param>
        public void RemoveAreaWarn(WarningTypeE warntype, ushort areaid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.area_id == areaid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion

        #region[线路报警]

        /// <summary>
        /// 添加[区域-线路]报警
        /// </summary>
        /// <param name="warntype"></param>
        /// <param name="areaid"></param>
        /// <param name="result"></param>
        public void AddLineWarn(WarningTypeE warntype, ushort areaid, ushort lineid, string result = "")
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.area_id == areaid && c.line_id == lineid && !c.resolve);
            if (warn == null)
            {
                if (stopwarnadding) return;
                warn = new Warning()
                {
                    area_id = areaid,
                    line_id = lineid,
                    type = (byte)warntype,
                };
                string lineName = PubMaster.Area.GetLineName(areaid, lineid);
                string warnmsg = PubMaster.Dic.GetDtlStrCode(warntype.ToString(), out byte level);
                warn.content = lineName + ": " + warnmsg + " > " + result;
                warn.level = level;
                AddWaring(warn);
            }
        }

        /// <summary>
        /// 清除[区域-线路]报警
        /// </summary>
        /// <param name="transid"></param>
        public void RemoveLineWarn(WarningTypeE warntype, ushort areaid, ushort lineid)
        {
            Warning warn = List.Find(c => c.type == (byte)warntype && c.area_id == areaid && c.line_id == lineid && !c.resolve);
            if (warn != null)
            {
                RemoveWarning(warn);
            }
        }
        #endregion
    }
}
