using enums;
using module.goods;
using module.line;
using module.track;
using resource;
using System.Collections.Generic;
using task.trans;
using tool.mlog;

namespace task.diagnose.trans
{
    /// <summary>
    /// 分析倒库任务是否需要暂停，并且是否释放小车，优先上砖
    /// </summary>
    public class SortTaskDiagnose : TransBaseDiagnose
    {
        public SortTaskDiagnose(TransMaster m) : base(m)
        {
            _mLog = (Log)new LogFactory().GetLog("倒库分析", false);
        }

        /// <summary>
        /// 【分析倒库任务】<br/>
        /// 1 _ 上砖任务分配不了车，需要暂停倒库任务，释放小车给上砖<br/>
        /// 2 _ 上砖任务分配设备状态持续(30秒)<br/>
        /// 3 _ 倒库任务、上砖侧倒库任务数量超过限制<br/>
        /// 4 _ 暂停倒库任务，释放小车<br/>
        /// </summary>
        public override void Diagnose()
        {
            #region[检查上砖任务，分配车超时，暂停倒库任务]
            List<StockTrans> list = _M.GetTransList()?.FindAll(c => c.InType(TransTypeE.上砖任务, TransTypeE.手动上砖)
                                        && c.IsInStatusOverTime(TransStatusE.调度设备, 20)) ?? null;
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    Line line = PubMaster.Area.GetLine(item.area_id, item.line);
                    if(line != null)
                    {
                        int sortqty = _M.GetSortTaskNotWaitCount(item.area_id, item.line);
                        if (line.sort_task_qty < sortqty)
                        {
                            StopSortTask(item, item.area_id, item.line);
                        }
                    }
                }
            }

            #endregion

            #region[检查倒库任务, 暂停任务超时，回复倒库任务]

            List<StockTrans> stopsort = _M.GetTransList()?.FindAll(c => c.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库)
                                       && c.IsInStatusOverTime(TransStatusE.倒库暂停, 60)) ?? null;
            if (stopsort != null && stopsort.Count > 0)
            {
                foreach (var item in stopsort)
                {
                    Line line = PubMaster.Area.GetLine(item.area_id, item.line);
                    if (line != null)
                    {
                        int sortqty = _M.GetSortTaskNotWaitCount(item.area_id, item.line);
                        if (line.sort_task_qty > sortqty)
                        {
                            ResumeSortTask(item);
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 暂停倒库任务<br/>
        /// 1 _ 暂停出入库倒库任务<br/>
        /// 2 _ 暂停非最近上砖轨道的上砖侧倒库任务<br/>
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="line"></param>
        private void StopSortTask(StockTrans trans, uint areaid, ushort line)
        {
            List<StockTrans> inoutsort = _M.GetTransList()?.FindAll(c => c.area_id == areaid && c.line == line && c.InType(TransTypeE.倒库任务)) ?? null;
            List<StockTrans> outoutsort = _M.GetTransList()?.FindAll(c => c.area_id == areaid && c.line == line && c.InType(TransTypeE.上砖侧倒库)) ?? null;

            if(inoutsort != null && inoutsort.Count > 0)
            {
                foreach (var item in inoutsort)
                {
                    if(item.carrier_id != 0)
                    {
                        Track track = PubTask.Carrier.GetCarrierTrack(item.carrier_id);
                        List<uint> ferrys = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.前摆渡, areaid, track.id, item.give_track_id, 0, false);
                        if (ferrys.Count > 0)
                        {
                            _M.SetStatus(item, TransStatusE.倒库暂停, string.Format("上砖任务[ {0} ]需要车, 暂停倒库运输车[ {1} ]的任务[ {2} ]",
                                trans.ToString(), PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));

                            try
                            {
                                _mLog.Status(true, string.Format("上砖任务[ {0} ]需要车, 暂停倒库运输车[ {1} ]的任务[ {2} ]", trans.ToString(),
                                    PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));
                            }
                            catch { }
                            return;
                        }
                    }
                }
            }

            if (outoutsort != null && outoutsort.Count > 0)
            {
                foreach (var item in outoutsort)
                {
                    if (item.carrier_id != 0 && !PubMaster.DevConfig.IsHaveSameTileNowGood(areaid, item.goods_id, item.level, TileWorkModeE.上砖))
                    {
                        Track track = PubTask.Carrier.GetCarrierTrack(item.carrier_id);
                        List<uint> ferrys = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.前摆渡, areaid, track.id, item.give_track_id, 0, false);
                        if (ferrys.Count > 0)
                        {
                            _M.SetStatus(item, TransStatusE.倒库暂停, string.Format("上砖任务[ {0} ]需要车, 暂停倒库运输车[ {1} ]的任务[ {2} ]",
                                trans.ToString(), PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));
                            try
                            {
                                _mLog.Status(true, string.Format("上砖任务[ {0} ]需要车, 暂停倒库运输车[ {1} ]的任务[ {2} ]", trans.ToString(),
                                    PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));
                            }
                            catch { }
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 回复倒库任务<br/>
        /// 1 _ 
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="areaid"></param>
        /// <param name="line"></param>
        private void ResumeSortTask(StockTrans trans)
        {
            _M.SetTakeFerry(trans, 0, "清空分配的信息，重新恢复倒库任务");
            _M.SetGiveFerry(trans, 0, "清空分配的信息，重新恢复倒库任务");
            _M.SetCarrier(trans, 0, "清空分配的信息，重新恢复倒库任务");
            _M.SetStatus(trans, TransStatusE.调度设备, string.Format("重新恢复倒库任务"));
        }
    }
}
