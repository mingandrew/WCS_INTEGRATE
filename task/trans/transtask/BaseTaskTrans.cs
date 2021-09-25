﻿using enums;
using enums.track;
using module.goods;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using task.device;
using tool.appconfig;
using tool.timer;

namespace task.trans.transtask
{
    /// <summary>
    /// 任务基础逻辑类
    /// </summary>
    public abstract class BaseTaskTrans
    {
        internal TransMaster _M { private set; get; }
        internal MTimer mTimer;

        public BaseTaskTrans(TransMaster trans)
        {
            _M = trans;
            mTimer = new MTimer();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="trans"></param>
        public void DoTrans(StockTrans trans)
        {
            try
            {
                #region[流程超时报警 - 默认超时10分钟则报警，倒库中流程则要2小时才报警]

                PubTask.Trans.CheckAndAddTransStatusOverTimeWarn(trans);

                #endregion

                switch (trans.TransStaus)
                {
                    case TransStatusE.调度设备:
                        AllocateDevice(trans);
                        break;
                    case TransStatusE.取砖流程:
                        ToTakeTrackTakeStock(trans);
                        break;
                    case TransStatusE.放砖流程:
                        ToGiveTrackGiveStock(trans);
                        break;
                    case TransStatusE.还车回轨:
                        ReturnDevBackToTrack(trans);
                        break;
                    case TransStatusE.倒库中:
                        SortingStock(trans);
                        break;
                    case TransStatusE.移车中:
                        MovingCarrier(trans);
                        break;
                    case TransStatusE.小车回轨:
                        ReturnCarrrier(trans);
                        break;
                    case TransStatusE.完成:
                        if (PubTask.Ferry.CancelFerryTrafficControlByTrans(trans.id))
                        {
                            FinishAndReleaseFerry(trans);
                            FinishStockTrans(trans);
                        }
                        break;
                    case TransStatusE.取消:
                        CancelStockTrans(trans);
                        break;
                    case TransStatusE.检查轨道:
                        CheckingTrack(trans);
                        break;
                    case TransStatusE.倒库暂停:
                        SortTaskWait(trans);
                        break;
                    case TransStatusE.接力等待:
                        Out2OutRelayWait(trans);
                        break;
                    case TransStatusE.整理中:
                        Organizing(trans);
                        break;
                    case TransStatusE.其他:
                        OtherAction(trans);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region[任务调度逻辑]

        /// <summary>
        /// 调度设备
        /// </summary>
        /// <param name="trans"></param>
        public abstract void AllocateDevice(StockTrans trans);

        /// <summary>
        /// 取砖流程
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ToTakeTrackTakeStock(StockTrans trans);

        /// <summary>
        /// 放砖流程
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ToGiveTrackGiveStock(StockTrans trans);

        /// <summary>
        /// 还车回轨
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ReturnDevBackToTrack(StockTrans trans);

        /// <summary>
        /// 倒库中
        /// </summary>
        /// <param name="trans"></param>
        public abstract void SortingStock(StockTrans trans);

        /// <summary>
        /// 移车中
        /// </summary>
        /// <param name="trans"></param>
        public abstract void MovingCarrier(StockTrans trans);

        /// <summary>
        /// 小车回轨
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ReturnCarrrier(StockTrans trans);

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public abstract void FinishStockTrans(StockTrans trans);

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public abstract void CancelStockTrans(StockTrans trans);

        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public abstract void CheckingTrack(StockTrans trans);

        /// <summary>
        /// 倒库暂停
        /// </summary>
        /// <param name="trans"></param>
        public abstract void SortTaskWait(StockTrans trans);

        /// <summary>
        /// 接力等待
        /// </summary>
        /// <param name="trans"></param>
        public abstract void Out2OutRelayWait(StockTrans trans);

        public abstract void OtherAction(StockTrans trans);

        public abstract void Organizing(StockTrans trans);

        #endregion

        #region[其他判断]

        /// <summary>
        /// 完成任务的同时判断是否需要释放运输车
        /// </summary>
        /// <param name="trans"></param>
        private void FinishAndReleaseFerry(StockTrans trans)
        {
            if (trans.carrier_id == 0) return;
            Track track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null || track.IsFerryTrack()) return;
            if (trans.take_ferry_id != 0)
            {
                PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id);
            }

            if (trans.give_ferry_id != 0)
            {
                PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
            }
        }

        /// <summary>
        /// 判断是否满砖(返回下一车库存脉冲)
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackID"></param>
        /// <param name="stkLoc"></param>
        /// <returns></returns>
        public bool CheckTrackFull(StockTrans trans, uint trackID, out ushort stkLoc)
        {
            // 是否已满砖
            if (PubMaster.Track.IsTrackFull(trackID))
            {
                stkLoc = 0;
                return true;
            }

            bool isback = PubMaster.Track.IsGiveBackTrack(trackID);

            // 判断下一车库存脉冲
            if (!PubMaster.Goods.CalculateNextLocByDir(isback ? DevMoveDirectionE.后 : DevMoveDirectionE.前, trans.carrier_id, trackID, trans.stock_id, out stkLoc))
            {
                // 设满砖
                PubMaster.Track.SetStockStatusAuto(trackID, TrackStockStatusE.满砖, "计算坐标值无法存入下一车");
                PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trackID, TrackLogE.满轨道, "计算坐标值无法存入下一车");

                #region 【任务步骤记录】
                _M.LogForTrackFull(trans, trackID);
                #endregion

                return true;
            }

            // 判断是否库存数已到设定的上限
            if (PubMaster.Goods.IsMoreThanFullQty(trans.area_id, trans.line, trackID))
            {
                // 设满砖
                PubMaster.Track.SetStockStatusAuto(trackID, TrackStockStatusE.满砖, "当前库存数已达上限，无法存入下一车");
                PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trackID, TrackLogE.满轨道, "当前库存数已达上限，无法存入下一车");

                #region 【任务步骤记录】
                _M.LogForTrackFull(trans, trackID);
                #endregion

                return true;
            }

            return false;
        }

        #endregion

        #region[检测轨道并添加移车任务]

        /// <summary>
        /// 判断是否有不符规格的车在作业的轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool CheckGoodsAndAddMoveTask(StockTrans trans, uint trackid)
        {
            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
            // 是否有不符规格的车在轨道
            if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trackid, goodssizeID, out uint carrierid))
            {
                string carName = PubMaster.Device.GetDeviceName(carrierid);
                string traName = PubMaster.Track.GetTrackName(trackid);

                if (_M.HaveCarrierInTrans(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 100, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        carName, traName));
                    #endregion
                    return true;
                }

                if (!PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 101, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carName, traName));
                    #endregion
                    return true;
                }

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 102, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                        carName, traName));
                #endregion

                //转移到同类型轨道
                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trackid);
                Track track = PubTask.Carrier.GetCarrierTrack(carrierid);
                DeviceTypeE ferrytype = PubTask.Carrier.GetCarrierNeedFerryType(carrierid);
                _M.AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道, ferrytype);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查轨道一侧内运输车是否需要移走
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="isdown">是否移走下砖侧</param>
        /// <param name="res"></param>
        /// <returns></returns>
        internal bool CheckCarAndAddMoveTask(StockTrans trans, uint trackid, bool isdown, DeviceTypeE ferrytype = DeviceTypeE.其他)
        {
            if (PubTask.Carrier.HaveInTrackAndGetSingle(trackid, out CarrierTask carrier))
            {
                Track track = PubMaster.Track.GetTrack(trackid);

                if (_M.HaveCarrierInTrans(carrier.ID))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 103, string.Format("有运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

                if (!PubTask.Carrier.IsCarrierFree(carrier.ID))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 104, string.Format("有运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

                bool ismove = false;
                if (isdown)
                {
                    // 入库侧的移走
                    if (track.Type == TrackTypeE.储砖_入
                        || (track.Type == TrackTypeE.储砖_出入
                            && (track.is_give_back ? (carrier.CurrentPoint >= track.split_point) : (carrier.CurrentPoint <= track.split_point)))
                        )
                    {
                        ismove = true;
                    }
                }
                else
                {
                    // 出库侧的移走
                    if (track.Type == TrackTypeE.储砖_出
                        || (track.Type == TrackTypeE.储砖_出入
                            && (track.is_take_forward ? (carrier.CurrentPoint <= track.split_point) : (carrier.CurrentPoint >= track.split_point)))
                        )
                    {
                        ismove = true;
                    }
                }

                if (ismove)
                {
                    if (ferrytype == DeviceTypeE.其他) ferrytype = PubTask.Carrier.GetCarrierNeedFerryType(carrier.ID);

                    _M.AddMoveCarrierTask(track.id, carrier.ID, track.Type, MoveTypeE.转移占用轨道, ferrytype);

                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 105, string.Format("有运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// 检查轨道内是否存在其他运输车需要移走
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool CheckTrackAndAddMoveTask(StockTrans trans, uint trackid, DeviceTypeE ferrytype)
        {
            if (PubTask.Carrier.HaveInTrackAndGetAll(trackid, out List<CarrierTask> cars, trans.carrier_id))
            {
                Track track = PubMaster.Track.GetTrack(trackid);

                // 根据摆渡类型排序
                if (cars.Count > 1)
                {
                    cars.Sort((x, y) =>
                    {
                        switch (ferrytype)
                        {
                            case DeviceTypeE.前摆渡:
                                return y.CurrentPoint.CompareTo(x.CurrentPoint);

                            case DeviceTypeE.后摆渡:
                                return x.CurrentPoint.CompareTo(y.CurrentPoint);
                        }
                        return 0;
                    });
                }

                foreach (CarrierTask item in cars)
                {
                    if (_M.HaveCarrierInTrans(item.ID))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 106, string.Format("有运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                            item.Device.name, track.name));
                        #endregion
                        return true;
                    }

                    if (!PubTask.Carrier.IsCarrierFree(item.ID))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 107, string.Format("有运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                            item.Device.name, track.name));
                        #endregion
                        return true;
                    }

                    _M.AddMoveCarrierTask(track.id, item.ID, track.Type, MoveTypeE.转移占用轨道, ferrytype);

                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 108, string.Format("有运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                        item.Device.name, track.name));
                    #endregion
                    return true;
                }

            }

            return false;
        }

        #endregion

        #region[分配摆渡车]

        /// <summary>
        /// 是否成功分配取货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="track"></param>
        /// <param name="isfalsereturn"></param>
        public bool AllocateTakeFerry(StockTrans trans, DeviceTypeE ferrytype, Track track)
        {
            if (trans.HaveTakeFerry)
            {
                return PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id);
            }
            else
            {
                string msg = _M.AllocateFerry(trans, ferrytype, track, false);
                // 失败
                if (_M.LogForTakeFerry(trans, msg)) return false;

                return true;
            }
        }

        /// <summary>
        /// 是否成功分配放货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="track"></param>
        /// <param name="isfalsereturn"></param>
        public bool AllocateGiveFerry(StockTrans trans, DeviceTypeE ferrytype, Track track)
        {
            if (trans.HaveGiveFerry)
            {
                return PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id);
            }
            else
            {
                string msg = _M.AllocateFerry(trans, ferrytype, track, true);
                // 失败
                if (_M.LogForGiveFerry(trans, msg)) return false;

                return true;
            }
        }

        #endregion

        #region[完全释放摆渡车]

        /// <summary>
        /// 释放取货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseTakeFerry(StockTrans trans, string memo = "")
        {
            if (trans.HaveTakeFerry
                && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
            {
                _M.FreeTakeFerry(trans, memo);
            }
        }

        /// <summary>
        /// 释放送货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseGiveFerry(StockTrans trans, string memo = "")
        {
            if (trans.HaveGiveFerry
                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
            {
                _M.FreeGiveFerry(trans, memo);
            }
        }
        #endregion

        #region [运输车指令]

        #region 基础指令
        /// <summary>
        /// 移至轨道定位点
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        public void MoveToPos(uint trackID, uint carrierID, uint transID, CarrierPosE cp, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            CarrierActionOrder cao = new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.定位指令,
                CheckTra = toTrack.ferry_up_code,  // 无所谓了 反正都是同一个轨道编号
                ToTrackId = toTrack.id,
            };

            switch (cp)
            {
                case CarrierPosE.下砖机复位点:
                    cao.OverPoint = toTrack.limit_point_up;
                    break;
                case CarrierPosE.上砖机复位点:
                    cao.OverPoint = toTrack.limit_point;
                    break;
                case CarrierPosE.后置摆渡复位点:
                case CarrierPosE.前置摆渡复位点:
                    cao.OverPoint = toTrack.limit_point;  // 无所谓了 反正都是只有一个定位点
                    break;
                case CarrierPosE.轨道后侧定位点:
                case CarrierPosE.轨道后侧复位点:
                    cao.OverPoint = toTrack.limit_point;
                    break;
                case CarrierPosE.轨道中间复位点:
                    cao.OverPoint = toTrack.split_point;
                    break;
                case CarrierPosE.轨道前侧定位点:
                case CarrierPosE.轨道前侧复位点:
                    cao.OverPoint = toTrack.limit_point_up;
                    break;
                default:
                    break;
            }
            PubTask.Carrier.DoOrder(carrierID, transID, cao, mes);
        }

        /// <summary>
        /// 移至指定脉冲
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        public void MoveToLoc(uint trackID, uint carrierID, uint transID, ushort loc, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 至指定脉冲
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.定位指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                OverPoint = loc,
                ToTrackId = toTrack.id
            }, mes);
        }

        /// <summary>
        /// 移至指定脉冲取砖
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        /// <param name="doNotBack">是否取砖后不移动</param>
        public void MoveToTake(uint trackID, uint carrierID, uint transID, ushort loc, string mes = "", DeviceTypeE ferryType = DeviceTypeE.其他, bool doNotBack = false)
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 取后 返回脉冲
            ushort overP = loc;
            if (!doNotBack)
            {
                switch (ferryType)
                {
                    case DeviceTypeE.前摆渡:
                        if (toTrack.IsStoreTrack()) overP = toTrack.limit_point_up;
                        break;
                    case DeviceTypeE.后摆渡:
                        if (toTrack.IsStoreTrack()) overP = toTrack.limit_point;
                        break;
                    default:
                        // 前进取则后侧停，后退取则前侧停
                        overP = toTrack.is_take_forward ? toTrack.limit_point : toTrack.limit_point_up;
                        break;
                }
            }

            // 至指定脉冲取砖
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.取砖指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                ToPoint = loc,
                OverPoint = overP,
                ToTrackId = toTrack.id
            }, mes);
        }

        /// <summary>
        /// 移至指定脉冲存砖
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        /// <param name="doNotBack">是否存砖后不移动</param>
        public void MoveToGive(uint trackID, uint carrierID, uint transID, ushort loc, string mes = "", DeviceTypeE ferryType = DeviceTypeE.其他, bool doNotBack = false)
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 卸后 返回脉冲
            ushort overP = loc;
            if (!doNotBack)
            {
                switch (ferryType)
                {
                    case DeviceTypeE.前摆渡:
                        if (toTrack.IsStoreTrack()) overP = toTrack.limit_point_up;
                        break;
                    case DeviceTypeE.后摆渡:
                        if (toTrack.IsStoreTrack()) overP = toTrack.limit_point;
                        break;
                    default:
                        // 后退存则前侧停，前进存则后侧停
                        overP = toTrack.is_give_back ? toTrack.limit_point_up : toTrack.limit_point;
                        break;
                }
            }

            // 至指定脉冲放砖
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.放砖指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                ToPoint = loc,
                OverPoint = overP,
                ToTrackId = toTrack.id
            }, mes);
        }

        /// <summary>
        /// 指定脉冲倒库
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        public void MoveToSort(uint trackID, uint carrierID, uint transID, ushort locTake, ushort locGive, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 至指定脉冲倒库
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.倒库指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                ToPoint = locTake,
                OverPoint = locGive,
                ToTrackId = toTrack.id
            }, mes);
        }

        #endregion


        /// <summary>
        /// 运输车直接取砖 By 指定库存
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="mes"></param>
        /// <param name="doNotBack">是否回轨道头</param>
        public void TakeInTarck(uint stockID, uint trackID, uint carrierID, uint transID, out string mes, DeviceTypeE ferryType = DeviceTypeE.其他, bool doNotBack = false)
        {
            // 获取库存
            Stock stk = PubMaster.Goods.GetStock(stockID);
            if (stk == null)
            {
                mes = string.Format("无库存[ id = {0} ]数据，无法取砖", stockID);
                return;
            }

            // 获取目的轨道
            Track tra = PubMaster.Track.GetTrack(trackID);
            if (tra == null)
            {
                mes = string.Format("无轨道[ id = {0} ]数据，无法取砖", trackID);
                return;
            }

            if (stk.track_id != tra.id)
            {
                mes = string.Format("库存[ {0} ]不在目的轨道[ {1} ]，无法取砖",
                    PubMaster.Track.GetTrackName(stk.track_id), tra.name);
                return;
            }

            // 获取小车当前脉冲
            ushort carloc = PubTask.Carrier.GetCurrentPoint(carrierID);

            // 取砖最小前提 ±10脉冲
            ushort safeloc = 10;

            if (stk.location == 0)
            {
                // 先回轨道头再靠光电取
                if (tra.is_take_forward && carloc > (tra.limit_point + safeloc))
                {
                    // 前进取 - 回后侧点
                    mes = "无库存脉冲，尝试前进取砖先回轨道后侧点";
                    MoveToPos(tra.id, carrierID, transID, CarrierPosE.轨道后侧定位点);
                    return;
                }

                if (!tra.is_take_forward && carloc < (tra.limit_point_up - safeloc))
                {
                    // 后退取 - 回前侧点
                    mes = "无库存脉冲，尝试后退取砖先回轨道前侧点";
                    MoveToPos(tra.id, carrierID, transID, CarrierPosE.轨道前侧定位点);
                    return;
                }

                // 直接靠光电取砖 前-65535；后-1
                mes = "无库存脉冲，尝试直接靠光电取砖";
                ushort loc = (ushort)(tra.is_take_forward ? 65535 : 1);
                ushort over = carloc;
                if (tra.is_take_forward && over < tra.limit_point)
                {
                    over = tra.limit_point;
                }
                if (!tra.is_take_forward && over > tra.limit_point_up)
                {
                    over = tra.limit_point_up;
                }
                PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.取砖指令,
                    CheckTra = tra.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                    ToPoint = loc,
                    OverPoint = over,
                    ToTrackId = tra.id
                }, mes);
                return;
            }
            else
            {
                ushort toloc = (ushort)(tra.is_take_forward ? (stk.location - safeloc) : (stk.location + safeloc));
                if (tra.is_take_forward && carloc > toloc)
                {
                    // 前进取 -小车要在库存位后至少 10 脉冲；
                    mes = "前进取砖，小车需先到合适位置";
                    MoveToLoc(tra.id, carrierID, transID, toloc);
                    return;
                }

                if (!tra.is_take_forward && carloc < toloc)
                {
                    // 后退取 -小车要在库存位前至少 10 脉冲；
                    mes = "后退取砖，小车需先到合适位置";
                    MoveToLoc(tra.id, carrierID, transID, toloc);
                    return;
                }

                // 取砖
                mes = "执行取砖";
                MoveToTake(tra.id, carrierID, transID, stk.location, mes, ferryType, doNotBack);
                return;
            }

        }

        /// <summary>
        /// 运输车直接放砖 By 指定脉冲
        /// </summary>
        /// <param name="stkloc"></param>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="mes"></param>
        /// <param name="doNotBack">是否回轨道头</param>
        public void GiveInTarck(ushort stkloc, uint trackID, uint carrierID, uint transID, out string mes, DeviceTypeE ferryType = DeviceTypeE.其他, bool doNotBack = false)
        {
            if (stkloc == 0)
            {
                mes = "无效库存存放位置0！";
                return;
            }

            // 获取目的轨道
            Track tra = PubMaster.Track.GetTrack(trackID);
            if (tra == null)
            {
                mes = string.Format("无轨道[ id = {0} ]数据，无法取砖", trackID);
                return;
            }

            // 获取小车当前脉冲
            ushort carloc = PubTask.Carrier.GetCurrentPoint(carrierID);

            // 放砖最小前提 ±20脉冲
            ushort safeloc = 20;

            if (tra.is_give_back && carloc < (stkloc - safeloc))
            {
                mes = string.Format("后退放砖-小车位置[ {0} ]无法后退到存放位置[ {1} ], 则原地放砖", carloc, stkloc);
                PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.放砖指令
                });
                return;
            }

            if (!tra.is_give_back && carloc > (stkloc + safeloc))
            {
                mes = string.Format("前进放砖-小车位置[ {0} ]无法前进到存放位置[ {1} ], 则原地放砖", carloc, stkloc);
                PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.放砖指令
                });
                return;
            }

            // 放砖
            mes = "执行放砖";
            MoveToGive(trackID, carrierID, transID, stkloc, mes, ferryType, doNotBack);
            return;
        }

        #endregion

        #region [库存转移 - 轨道内倒库]

        /// <summary>
        /// 更新倒库作业库存
        /// </summary>
        /// <param name="trans"></param>
        public void SetStockForSort(StockTrans trans)
        {
            List<Stock> stocks = PubMaster.Goods.GetStocks(trans.give_track_id);
            if (stocks == null || stocks.Count == 0)
            {
                _M.SetStock(trans, 0);
                return;
            }

            if (PubMaster.Track.IsTakeForwardTrack(trans.give_track_id))
            {
                // 前进取砖，库存脉冲按从小到大
                stocks.Sort((x, y) => x.location.CompareTo(y.location));
            }
            else
            {
                // 后退取砖，库存脉冲按从大到小
                stocks.Sort((x, y) => y.location.CompareTo(x.location));
            }

            // 先看第一车
            if (trans.stock_id == 0)
            {
                _M.SetStock(trans, stocks[0].id);
                return;
            }

            // 开始计算index
            int next = (stocks.FindIndex(c => c.id == trans.stock_id) + 1);
            // 超范围 - 结束
            if (next == 0 || next == stocks.Count)
            {
                _M.SetStock(trans, 0);
                return;
            }

            // 安全距离
            ushort safe = PubMaster.Goods.GetStackSafe(trans.goods_id, trans.carrier_id);
            // 再加个 ≈173CM 判断好了
            safe += 100;
            for (int i = next; i < stocks.Count; i++)
            {
                // 取放位置间距过小则跳过
                if (Math.Abs(stocks[i].location - stocks[i - 1].location) <= safe)
                {
                    continue;
                }

                // 设定作业库存
                _M.SetStock(trans, stocks[i].id);
                return;
            }

            // 结束
            _M.SetStock(trans, 0);
            return;
        }

        /// <summary>
        /// 获取倒库放砖位置
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="limitPoint"></param>
        /// <param name="splitPoint"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool GetTransferGivePoint(uint trackid, uint carrierid, int limitPoint, int splitPoint, out ushort loc)
        {
            // 获取分界点前 最后的库存
            bool isforward = PubMaster.Track.IsTakeForwardTrack(trackid);
            Stock stk = PubMaster.Goods.GetStockInfrontStockPoint(trackid, splitPoint);
            if (stk != null)
            {
                // 参考库存当前被车载移动
                CarrierTask carrier = PubTask.Carrier.GetCarrierByStockid(stk.id);
                if (carrier != null && carrier.TargetPoint > 0 && (carrier.Status == DevCarrierStatusE.前进 || carrier.Status == DevCarrierStatusE.后退))
                {
                    ushort safe = PubMaster.Goods.GetStackSafe(stk.goods_id, carrierid); // 安全间隔
                    safe = (ushort)(safe * 2); // 感觉2个比较稳妥

                    // 运动过程中以目的脉冲为计算值
                    loc = (ushort)(isforward ? (carrier.TargetPoint + safe) : (carrier.TargetPoint - safe));
                }
                else
                {
                    // 计算下一车位置
                    if (!PubMaster.Goods.CalculateNextLocByStock(isforward ? DevMoveDirectionE.前 : DevMoveDirectionE.后, stk, out loc, carrierid))
                    {
                        return false;
                    }
                }

                // 判断是否超过极限点
                if (isforward ? (loc < limitPoint) : (loc > limitPoint))
                {
                    loc = (ushort)limitPoint;
                }

                // 判断是否超过分界点
                if (isforward ? (loc > splitPoint) : (loc < splitPoint))
                {
                    return false;
                }

                return true;
            }

            // 无库存就放极限位置
            loc = (ushort)limitPoint;
            return loc > 0;
        }

        /// <summary>
        /// 获取倒库待命位置
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="overPoint"></param>
        /// <param name="splitPoint"></param>
        /// <returns></returns>
        public ushort GetTransferWaitPoint(uint trackid, uint carrierid, int overPoint, int splitPoint)
        {
            // 获取轨道数据
            Track track = PubMaster.Track.GetTrack(trackid);
            // 待命点
            int loc = 0;

            // 获取分界点前 最后的库存
            Stock stk = PubMaster.Goods.GetStockInfrontStockPoint(trackid, splitPoint);
            if (stk != null)
            {
                // 安全距离
                ushort safe = PubMaster.Goods.GetStackSafe(stk.goods_id, carrierid);
                // 运输车等待的时候需要后退几个车身
                ushort carspace = GlobalWcsDataConfig.BigConifg.GetSortWaitNumberCarSpace(track.area, track.line);
                carspace = (ushort)(carspace * safe);

                loc = track.is_take_forward ? (stk.location + carspace) : (stk.location - carspace);
            }

            // 获取分界点后 最前的库存
            Stock nextstk = PubMaster.Goods.GetStockBehindStockPoint(trackid, splitPoint);
            if (nextstk != null)
            {
                // 定最远的
                if (loc == 0 || track.is_take_forward ? (nextstk.location > loc) : (nextstk.location < loc))
                {
                    loc = nextstk.location;
                }
            }

            // 判断是否超过结束点
            if (loc == 0 || track.is_take_forward ? (loc > overPoint) : (loc < overPoint))
            {
                return (ushort)overPoint;
            }

            return (ushort)loc;
        }

        /// <summary>
        /// 是否倒库转移结束
        /// </summary>
        /// <param name="trackid">轨道ID</param>
        /// <param name="overPoint">结束点</param>
        /// <param name="splitPoint">分界点</param>
        /// <returns></returns>
        public bool IsTransferOver(uint trackid, int overPoint, int splitPoint)
        {
            // 结束点前是否有库存
            if (!PubMaster.Goods.ExistInfrontUpSplitPoint(trackid, overPoint, out int stkcount))
            {
                return true;
            }

            // 分界点后是否有库存
            if (!PubMaster.Goods.ExistBehindUpSplitPoint(trackid, splitPoint, out stkcount))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region [无缝上摆渡]

        /// <summary>
        /// 是否提前解锁摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="track"></param>
        /// <param name="carrier"></param>
        public void UnlockTakeFerryFrist(StockTrans trans, Track track, CarrierTask carrier)
        {
            if (!trans.HaveTakeFerry) return;

            // 解锁摆渡车
            ushort limit = (ushort)PubMaster.Dic.GetDtlDouble(DicTag.UnlockFerryLimit, 576); // 10M
            switch (trans.AllocateFerryType)
            {
                case DeviceTypeE.前摆渡:
                    int disBack = (track.limit_point_up - limit);
                    if ((carrier.CurrentPoint > 0 && carrier.CurrentPoint <= disBack) ||
                        (carrier.TargetPoint > 0 && carrier.TargetPoint <= disBack))
                    {
                        RealseTakeFerry(trans);
                    }
                    break;
                case DeviceTypeE.后摆渡:
                    int disFront = (track.limit_point + limit);
                    if ((carrier.CurrentPoint > 0 && carrier.CurrentPoint >= disFront) ||
                        (carrier.TargetPoint > 0 && carrier.TargetPoint >= disFront))
                    {
                        RealseTakeFerry(trans);
                    }
                    break;
            }

        }

        /// <summary>
        /// 无缝上摆渡
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="istake"></param>
        public void MoveToFerrySeamless(StockTrans trans, bool istake)
        {
            CarrierTask car = PubTask.Carrier.GetDevCarrier(trans.carrier_id);
            if (car == null) return;

            Track carTrack = PubMaster.Track.GetTrack(car.CurrentTrackId);
            if (carTrack == null) return;
            if (carTrack.IsFerryTrack()) return;

            // 小车根据摆渡类型需要先定位到轨道头
            ushort tosite = 0;
            switch (trans.AllocateFerryType)
            {
                case DeviceTypeE.其他:
                    ushort carP = trans.TransType == TransTypeE.移车任务 ? car.CurrentPoint : (ushort)0;
                    DeviceTypeE type = _M.GetAllocateFerryType(trans.take_track_id, trans.give_track_id, carP);
                    _M.SetAllocateFerryType(trans, type);
                    return;

                case DeviceTypeE.前摆渡:
                    if (carTrack.ferry_down_code > 400) // 前摆渡 401~499
                    {
                        tosite = carTrack.limit_point;
                    }
                    else
                    {
                        tosite = carTrack.limit_point_up;
                    }
                    break;

                case DeviceTypeE.后摆渡:
                    if (carTrack.ferry_down_code < 200) // 后摆渡 201~299
                    {
                        tosite = carTrack.limit_point_up;
                    }
                    else
                    {
                        tosite = carTrack.limit_point;
                    }
                    break;
            }
            if (tosite == 0) return;

            string result = "";
            // 离轨道头范围 ≈10M
            ushort limit = (ushort)PubMaster.Dic.GetDtlDouble(DicTag.UnlockFerryLimit, 576); // 10M
            if (Math.Abs(tosite - car.CurrentPoint) <= limit)
            {
                // 范围内
                // 锁定摆渡  
                uint ferryTraid = 0;
                bool isFerryOK = false;  // 是否摆渡到位
                uint ferryID = istake ? trans.take_ferry_id : trans.give_ferry_id;
                if (istake ? AllocateTakeFerry(trans, trans.AllocateFerryType, carTrack) : AllocateGiveFerry(trans, trans.AllocateFerryType, carTrack))
                {
                    if (ferryID == 0) ferryID = istake ? trans.take_ferry_id : trans.give_ferry_id;
                    // 定位摆渡  
                    if (_M.LockFerryAndAction(trans, ferryID, carTrack.id, carTrack.id, out ferryTraid, out result, true))
                    {
                        isFerryOK = true;
                    }
                    else
                    {
                        #region 【任务步骤记录】
                        _M.LogForFerryMove(trans, ferryID, carTrack.id, result);
                        #endregion
                    }
                }

                // 无缝上摆渡
                if (car.IsStopNoOrder(out result))
                {
                    // 摆渡到位 直接定位摆渡
                    if (isFerryOK)
                    {
                        //至摆渡车
                        MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToFerry(trans, carTrack.id, ferryID);
                        #endregion
                        return;
                    }

                    // 超过轨道头则不动
                    if (tosite == carTrack.limit_point_up && car.CurrentPoint > tosite)
                    {
                        return;
                    }
                    if (tosite == carTrack.limit_point && car.CurrentPoint < tosite)
                    {
                        return;
                    }

                    // 差距小也不动
                    if (Math.Abs(tosite - car.CurrentPoint) <= 20)
                    {
                        return;
                    }

                    // 移至轨道定位点
                    MoveToLoc(carTrack.id, trans.carrier_id, trans.id, tosite);

                    #region 【任务步骤记录】
                    _M.LogForCarrierToTrack(trans, carTrack.id);
                    #endregion
                    return;
                }
                else
                {
                    // 运动：目的点轨道头则改为摆渡
                    if (isFerryOK && PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, 0, tosite))
                    {
                        //至摆渡车
                        MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToFerry(trans, carTrack.id, ferryID);
                        #endregion
                        return;
                    }
                }

            }
            else
            {
                // 范围外
                if (car.IsStopNoOrder(out result))
                {
                    // 停止：定位到轨道头
                    MoveToLoc(carTrack.id, trans.carrier_id, trans.id, tosite);

                    #region 【任务步骤记录】
                    _M.LogForCarrierToTrack(trans, carTrack.id);
                    #endregion
                    return;
                }
                else
                {
                    // 运动：查询最新状态
                    car.DoQuery();
                    return;
                }
            }

        }

        #endregion

    }
}
