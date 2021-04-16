using enums;
using module.goods;
using module.line;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tool.mlog;

namespace task.trans.diagnose
{
    /// <summary>
    /// 分析倒库任务是否需要暂停，并且是否释放小车，优先上砖
    /// </summary>
    public class SortTaskDiagnose : BaseDiagnose
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
            List<StockTrans> list = _M.GetTransList()?.FindAll(c => c.InType(TransTypeE.上砖任务, TransTypeE.手动上砖)
                                        && c.IsInStatusOverTime(TransStatusE.调度设备, 30)) ?? null;
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    Line line = PubMaster.Area.GetLine(item.area_id, item.line);
                    if(line != null)
                    {
                        int sortqty = _M.GetSortTaskNotWaitCount(item.area_id, item.line);
                        if (line.sort_task_qty <= sortqty)
                        {
                            StopSortTask(item, item.area_id, item.line);
                        }
                    }
                }
            }
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
                        List<uint> ferrys = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.上摆渡, areaid, track.id, item.give_track_id, 0, false);
                        if (ferrys.Count > 0)
                        {
                            _M.SetStatus(item, TransStatusE.倒库暂停, string.Format("上砖任务[ {0} ]需要车, 暂停倒库的小车[ {1} ]的任务[ {2} ]",
                                trans.ToString(), PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));

                            try
                            {
                                _mLog.Status(true, string.Format("上砖任务[ {0} ]需要车, 暂停倒库的小车[ {1} ]的任务[ {2} ]", trans.ToString(),
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
                    if (item.carrier_id != 0 && !PubMaster.DevConfig.IsHaveSameTileNowGood(item.goods_id, TileWorkModeE.上砖))
                    {
                        Track track = PubTask.Carrier.GetCarrierTrack(item.carrier_id);
                        List<uint> ferrys = PubMaster.Area.GetFerryWithTrackInOut(DeviceTypeE.上摆渡, areaid, track.id, item.give_track_id, 0, false);
                        if (ferrys.Count > 0)
                        {
                            _M.SetStatus(item, TransStatusE.倒库暂停, string.Format("上砖任务[ {0} ]需要车, 暂停倒库的小车[ {1} ]的任务[ {2} ]",
                                trans.ToString(), PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));
                            try
                            {
                                _mLog.Status(true, string.Format("上砖任务[ {0} ]需要车, 暂停倒库的小车[ {1} ]的任务[ {2} ]", trans.ToString(),
                                    PubMaster.Device.GetDeviceName(item.carrier_id), item.ToString()));
                            }
                            catch { }
                            return;
                        }
                    }
                }
            }
        }
    }
}
