using enums;
using enums.track;
using module.goods;
using module.line;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using task.device;
using task.trans;
using tool.appconfig;
using tool.mlog;

namespace task.diagnose.trans
{
    /// <summary>
    /// 分析设备分配（分配设备）
    /// </summary>
    public class MoveCarDiagnose : TransBaseDiagnose
    {
        private int MoveCarWaitOverTime { set; get; } = 20;

        public MoveCarDiagnose(TransMaster master) : base(master)
        {
            _mLog = (Log)new LogFactory().GetLog("移动车分析", false);

            MoveCarWaitOverTime = GlobalWcsDataConfig.BigConifg.MoveCarWaitOverTime;
        }

        /// <summary>
        /// 【分析设备分配】<br/>
        /// 1 _ 上砖任务(手动/同侧)/下砖任务(手动/同侧)/倒库任务<br/>
        /// 2 _ 分配设备超时<br/>
        /// 3 _ 有空闲车不能直接到达作业轨道<br/>
        /// 4 _ 生成移车任务<br/>
        /// </summary>
        public override void Diagnose()
        {
            #region[同侧移车任务]
            //1 _ 上砖任务(手动/同侧)/下砖任务(手动/同侧)/倒库任务
            //2 _ 分配设备超时
            List<StockTrans> list = _M.GetTransList()?.FindAll(c => c.NotInType(TransTypeE.移车任务, TransTypeE.其他)
                                                                                                && c.IsInStatusOverTime(TransStatusE.调度设备, MoveCarWaitOverTime)) ?? null;
            if (list != null && list.Count > 0)
            {
                foreach (var trans in list)
                {
                    CheckTransAndAddMoveTask(trans);
                }
            }
            #endregion
        }

        /// <summary>
        /// 检查任务-生成移车
        /// </summary>
        /// <param name="trans"></param>
        private void CheckTransAndAddMoveTask(StockTrans trans)
        {
            //是否已经超过设定的任务车数
            if (!_M.IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType)) return;

            //如果已经有移车任务则不生成
            if (_M.ExistAreaLineType(trans.area_id, trans.line, TransTypeE.移车任务)) return;

            #region[根据任务区分]

            bool checktakegivetrack = false;
            DeviceTypeE ferrytype = DeviceTypeE.其他;
            switch (trans.TransType)
            {
                case TransTypeE.上砖任务:
                case TransTypeE.手动上砖:
                case TransTypeE.同向下砖:
                    checktakegivetrack = true;
                    ferrytype = DeviceTypeE.上摆渡;
                    break;
                case TransTypeE.下砖任务:
                case TransTypeE.手动下砖:
                case TransTypeE.同向上砖:
                    checktakegivetrack = true;
                    ferrytype = DeviceTypeE.下摆渡;
                    break;
                case TransTypeE.倒库任务:
                case TransTypeE.上砖侧倒库:
                    checktakegivetrack = false;
                    ferrytype = DeviceTypeE.上摆渡;
                    break;
            }

            #endregion

            #region[同侧移车任务]

            //3 _ 有空闲车不能直接到达作业轨道
            List<CarrierTask> carriers = PubTask.Carrier.GetFreeCarrierWithNoDirectFerry(trans, ferrytype, checktakegivetrack,
                                                                                                     out List<uint> trackid, out List<uint> ferryid);
            foreach (CarrierTask car in carriers)
            {
                uint cartrackid = car.CurrentTrackId;
                //判断运输车所在轨道是否是下砖机上一次放砖的轨道
                if (PubTask.TileLifter.IsInTileLastTrack(cartrackid))
                {
                    continue;
                }

                //判断运输车所在轨道和任务
                List<uint> ctrackid = PubMaster.Track.SortTrackIdsWithOrder(trackid, cartrackid, PubMaster.Track.GetTrackOrder(car.CurrentTrackId));
                foreach (var traid in ctrackid)
                {
                    //轨道被任务或者有车在
                    if(_M.IsTraInTrans(traid) ||  PubTask.Carrier.HaveInTrack(traid, car.ID))
                    {
                        continue;
                    }

                    if (PubMaster.Area.ExistFerryWithTrack(ferryid, traid)
                        && PubMaster.Area.GetWithTracksFerryIds(ferrytype, cartrackid, traid).Count > 0)
                    {
                        uint sortaskid =_M.AddTransWithoutLock(trans.area_id, 0, TransTypeE.移车任务, 0, 0, cartrackid, traid, TransStatusE.移车中, car.ID, trans.line);
                        if(sortaskid != 0)
                        {
                            _mLog.Status(true, string.Format("标识[ {0} ], [ {1} ]超时, 取[ {2} ] -> 卸[ {3} ], 找不到空闲的运输车,[同侧移车]", trans.id, trans.TransType,
                                                            PubMaster.Track.GetTrackName(trans.take_track_id),
                                                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                            _mLog.Status(true, string.Format("标识[ {0} ], 移车任务[ {1}], 运输车[ {2} ], 从[ {3} ] -> 到[ {4} ],[同侧移车]", trans.id, sortaskid,
                                                            PubMaster.Device.GetDeviceName(car.ID),
                                                            PubMaster.Track.GetTrackName(cartrackid),
                                                            PubMaster.Track.GetTrackName(traid)));
                        }
                        return;
                    }
                }
            }

            #endregion

            #region 同一条轨道移车任务 - [ 出库轨道 <-> 入库轨道 ]

            //超过1分钟没有分配到运输车
            if (trans.IsInStatusOverTime(TransStatusE.调度设备, 60))
            {
                // 当前-上下砖侧运输车的数量
                uint currentUpCarCount = PubTask.Carrier.GetCurrentCarCount(trans.area_id, true, TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.摆渡车_出);
                uint currentDownCarCount = PubTask.Carrier.GetCurrentCarCount(trans.area_id, false, TrackTypeE.下砖轨道, TrackTypeE.储砖_入, TrackTypeE.摆渡车_入);
                // 设定-上下砖侧运输车的数量
                uint settingUpCarCount = PubMaster.Area.GetAreaUpCarCount(trans.area_id);
                uint settingDownCarCount = PubMaster.Area.GetAreaDownCarCount(trans.area_id);

                // 没有设定两侧运输车的数量的话则不执行这个流程
                if (settingDownCarCount == 0 || settingUpCarCount == 0)
                {
                    return;
                }

                CarrierTask freeCarrier = null;
                uint brotrackid = 0;
                if (ferrytype == DeviceTypeE.上摆渡)
                {
                    //如果当前下砖多于设定下砖，且当前上砖少于设定上砖
                    if (currentDownCarCount > settingDownCarCount && currentUpCarCount < settingUpCarCount)
                    {
                        // 找一台空闲的在下砖侧的运输车
                        freeCarrier = PubTask.Carrier.GetCarrierFree(trans.area_id, out brotrackid, TrackTypeE.储砖_入);
                    }
                }
                else
                {
                    //如果当前上砖多于设定上砖，且当前下砖少于设定下砖
                    if (currentUpCarCount > settingUpCarCount && currentDownCarCount < settingDownCarCount)
                    {
                        // 找一台空闲的在上砖侧的运输车
                        freeCarrier = PubTask.Carrier.GetCarrierFree(trans.area_id, out brotrackid, TrackTypeE.储砖_出);
                    }
                }

                if (freeCarrier == null || brotrackid == 0)
                {
                    return;
                }

                uint sortaskid = _M.AddTransWithoutLock(trans.area_id, 0, TransTypeE.移车任务, 0, 0, freeCarrier.CurrentTrackId, brotrackid, TransStatusE.移车中, freeCarrier.ID, trans.line);
                if (sortaskid != 0)
                {
                    _mLog.Status(true, string.Format("标识[ {0} ], [ {1} ]超时, 取[ {2} ] -> 卸[ {3} ], 找不到空闲的运输车,[出<->入]", trans.id, trans.TransType,
                                                    PubMaster.Track.GetTrackName(trans.take_track_id),
                                                    PubMaster.Track.GetTrackName(trans.give_track_id)));
                    _mLog.Status(true, string.Format("标识[ {0} ], 移车任务[ {1}], 运输车[ {2} ], 从[ {3} ] -> 到[ {4} ],[出<->入]", trans.id, sortaskid,
                                                    PubMaster.Device.GetDeviceName(freeCarrier.ID),
                                                    PubMaster.Track.GetTrackName(freeCarrier.CurrentTrackId),
                                                    PubMaster.Track.GetTrackName(brotrackid)));
                }
                return;

            }

            #endregion
        }

    }
}
