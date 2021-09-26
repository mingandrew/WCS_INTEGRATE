﻿using enums;
using enums.track;
using enums.warning;
using module.goods;
using module.track;
using resource;
using System.Collections.Generic;
using task.device;
using tool.appconfig;

namespace task.trans.transtask
{
    /// <summary>
    /// 库存转移任务
    /// 转移库存轨道-流程（code- XX11）
    /// </summary>
    public class MoveStockTrans : BaseTaskTrans
    {
        public MoveStockTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {
            //转移取货轨道不符合的运输车
            if (CheckGoodsAndAddMoveTask(trans, trans.take_track_id))
            {
                return;
            }

            //转移卸货轨道不符合的运输车
            if (CheckGoodsAndAddMoveTask(trans, trans.give_track_id))
            {
                return;
            }

            //是否有小车在取货轨道入库侧
            if (CheckCarAndAddMoveTask(trans, trans.take_track_id, true))
            {
                return;
            }

            _M.SetStatus(trans, TransStatusE.调度设备);
        }

        /// <summary>
        /// 调度设备
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            //if (_M.HaveGiveInTrackId(trans))
            //{
            //    #region 【任务步骤记录】
            //    _M.SetStepLog(trans, false, 1111, string.Format("存在相同作业轨道的任务，等待任务完成；"));
            //    #endregion
            //    return;
            //}

            //是否开启【出入倒库轨道可以同时上砖】
            bool isCancel = false;
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask))
            {
                if (_M.ExistTransWithTrackButType(trans.id, trans.give_track_id, TransTypeE.移车任务, TransTypeE.上砖任务, TransTypeE.同向上砖))
                {
                    isCancel = true;
                }
            }
            else
            {
                if (_M.ExistTransWithTrackButType(trans.id, trans.give_track_id, TransTypeE.移车任务))
                {
                    isCancel = true;
                }
            }
            if (isCancel)
            {
                _M.SetStatus(trans, TransStatusE.取消, "卸货轨道的存在任务");
                return;
            }

            //分配运输车
            if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
                && !_M.HaveInCarrier(carrierid))
            {
                _M.SetCarrier(trans, carrierid);
                _M.SetStatus(trans, TransStatusE.取砖流程);

                // 直接先跑一次
                if (trans.TransStaus == TransStatusE.取砖流程) ToTakeTrackTakeStock(trans);
                return;
            }

            #region 【任务步骤记录】
            if (_M.LogForCarrier(trans, result)) return;
            #endregion

        }

        /// <summary>
        /// 取货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToTakeTrackTakeStock(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            switch (track.Type)
            {
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                    if (isNotLoad)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            // 判断提前解锁摆渡车
                            UnlockTakeFerryFrist(trans, track, carrier);

                            if (isStopNoOrder)
                            {
                                // 轨道内直接取砖
                                TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out string res);

                                #region 【任务步骤记录】
                                _M.LogForCarrierTake(trans, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            // 长时间无法取到砖
                            if (!isStopNoOrder && carrier.InTask(DevCarrierOrderE.取砖指令))
                            {
                                // 取砖相关报警超20s
                                if (carrier.DevAlert.CanNotActionForTaking() && mTimer.IsOver(trans.carrier_id + "LoadError", 20, 10))
                                {
                                    // 先解锁摆渡
                                    if (trans.HaveTakeFerry)
                                    {
                                        RealseTakeFerry(trans, "运输车取砖出现异常，长时间无法完成取砖指令");
                                        return;
                                    }

                                    // 再尝试终止运输车
                                    if (carrier.DevStatus.DeviceStatus == DevCarrierStatusE.停止)
                                    {
                                        carrier.DoStop(trans.id, "取砖指令超时");

                                        #region 【任务步骤记录】
                                        _M.SetStepLog(trans, false, 1711, string.Format("[ {0} ]取砖指令超时, 尝试终止", carrier.Device.name));
                                        #endregion
                                        return;
                                    }
                                }
                            }

                        }
                        else
                        {
                            //至摆渡车
                            MoveToFerrySeamless(trans, true);
                            return;
                        }
                    }

                    if (isLoad)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                        }
                        else
                        {
                            if (isStopNoOrder)
                            {
                                // 下降放货
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.放砖指令
                                }, "库存转移下降放货");
                            }
                            #region 【任务步骤记录】
                            _M.LogForCarrierGiving(trans);
                            #endregion
                            return;
                        }

                    }

                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:
                case TrackTypeE.后置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, trans.AllocateFerryType, track)) return;

                    // 获取库存脉冲
                    ushort stkLoc = PubMaster.Goods.GetStockLocation(trans.stock_id);

                    //是否有小车在取货轨道 - 停用
                    //bool CheckOtherCar = CheckTrackAndAddMoveTask(trans, trans.take_track_id, track.Type == TrackTypeE.前置摆渡轨道 ? DeviceTypeE.后摆渡 : DeviceTypeE.前摆渡);

                    // 判断是否有阻碍
                    bool CheckOtherCar = !PubTask.Carrier.CanDoOrderSafe(trans.carrier_id, trans.take_track_id, stkLoc, out result);

                    if (isNotLoad && isStopNoOrder)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            //摆渡车 定位去 取货点
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out uint ferryTraid, out string res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            //是否有小车在取货轨道
                            if (CheckOtherCar)
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierNoTake(trans, trans.take_track_id, result);
                                #endregion
                                return;
                            }

                            // 直接取砖 - 停用
                            //TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out res);

                            // 取砖
                            MoveToTake(trans.take_track_id, trans.carrier_id, trans.id, stkLoc, "", trans.AllocateFerryType);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, trans.take_track_id, res);
                            #endregion
                            return;
                        }
                    }

                    break;
                    #endregion
            }
        }

        /// <summary>
        /// 放货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToGiveTrackGiveStock(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            switch (track.Type)
            {
                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                case TrackTypeE.后置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateGiveFerry(trans, trans.AllocateFerryType, track)) return;

                    if (isLoad && isStopNoOrder)
                    {
                        #region [根据摆渡车code比对卸货轨道code - 获取存砖脉冲]
                        ushort targetCode = PubMaster.Track.GetTrackDownCode(trans.give_track_id);
                        // 是否后退存砖
                        bool isback = track.ferry_down_code > targetCode;

                        // 下一车库存脉冲
                        bool canMove = PubMaster.Goods.CalculateNextLocByDir(isback ? DevMoveDirectionE.后 : DevMoveDirectionE.前, trans.carrier_id, trans.give_track_id, trans.stock_id, out ushort stkLoc);

                        // 是否被任务占用
                        if (canMove)
                        {
                            //是否开启【出入倒库轨道可以同时上砖】
                            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask))
                            {
                                if (_M.ExistTransWithTrackButType(trans.id, trans.give_track_id, TransTypeE.移车任务, TransTypeE.上砖任务, TransTypeE.同向上砖))
                                {
                                    canMove = false;
                                }
                            }
                            else
                            {
                                if (_M.ExistTransWithTrackButType(trans.id, trans.give_track_id, TransTypeE.移车任务))
                                {
                                    canMove = false;
                                }
                            }
                        }

                        #endregion

                        #region [重新分配轨道]
                        if (!canMove)
                        {
                            bool isWarn = false;

                            // 换轨道?
                            List<uint> newTraIDs = PubMaster.Track.GetOutTrackIDByInTrack(trans.take_track_id, trans.goods_id, trans.level);
                            if (newTraIDs != null && newTraIDs.Count > 0)
                            {
                                foreach (uint traid in newTraIDs)
                                {
                                    if (!_M.ExistTransWithTracks(traid)
                                        //&& !PubTask.Carrier.HaveInTrack(traid, trans.carrier_id)
                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, traid)
                                        && _M.SetGiveSite(trans, traid))
                                    {
                                        isWarn = true;
                                        break;
                                    }
                                }
                            }

                            if (isWarn)
                            {
                                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.TransHaveNotTheGiveTrack, trans.id);
                            }
                            else
                            {
                                // 超60s - 取消
                                if (mTimer.IsOver(trans.carrier_id + "NotGiveSite", 60, 10))
                                {
                                    _M.SetStatus(trans, TransStatusE.取消, "超时 - 没有找到合适的轨道卸砖");
                                    return;
                                }

                                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.TransHaveNotTheGiveTrack, (ushort)trans.carrier_id, trans.id);

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1311, string.Format("没有找到合适的轨道卸砖，继续尝试寻找其他轨道；"));
                                #endregion
                            }

                            return;
                        }
                        #endregion

                        #region 卸砖
                        if (canMove)
                        {
                            //是否有小车在放货轨道
                            //bool CheckOtherCar = CheckTrackAndAddMoveTask(trans, trans.give_track_id, track.Type == TrackTypeE.前置摆渡轨道 ? DeviceTypeE.后摆渡 : DeviceTypeE.前摆渡);

                            // 判断是否有阻碍
                            bool CheckOtherCar = !PubTask.Carrier.CanDoOrderSafe(trans.carrier_id, trans.give_track_id, stkLoc, out result);

                            //小车在摆渡车上
                            if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                            {
                                if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out uint ferryTraid, out string res))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                                    #endregion
                                    return;
                                }

                                //是否有小车在放货轨道
                                if (CheckOtherCar)
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierNoTake(trans, trans.give_track_id, result);
                                    #endregion
                                    return;
                                }

                                MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, stkLoc, "", trans.AllocateFerryType);

                                #region 【任务步骤记录】
                                _M.LogForCarrierGive(trans, trans.give_track_id);
                                #endregion
                                return;
                            }

                        }

                        #endregion

                    }
                    break;
                #endregion

                #region[小车在放砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                    #region[放货轨道]
                    if (track.id == trans.give_track_id)
                    {
                        // 解锁摆渡车
                        RealseGiveFerry(trans);

                        #region 原地放砖 - 暂用
                        if (isLoad)
                        {
                            if (isStopNoOrder)
                            {
                                // 下降放货
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.放砖指令
                                });
                            }
                            #region 【任务步骤记录】
                            _M.LogForCarrierGiving(trans);
                            #endregion
                            return;
                        }

                        #endregion

                        #region 往出库侧存砖 - 停用
                        //if (isLoad && isStopNoOrder)
                        //{
                        //    // 更新库存为小车绑定库存ID
                        //    _M.SetStock(trans, PubMaster.DevConfig.GetCarrierStockId(trans.carrier_id));

                        //    // 获取放砖位置
                        //    ushort limitP = track.is_give_back ? track.limit_point : track.limit_point_up;
                        //    if (GetTransferGivePoint(track.id, carrier.ID, limitP, carrier.CurrentPoint, out ushort givePoint))
                        //    {
                        //        // 直接放砖
                        //        GiveInTarck(givePoint, track.id, trans.carrier_id, trans.id, out string res);

                        //        #region 【任务步骤记录】
                        //        _M.LogForCarrierGive(trans, track.id, res);
                        //        #endregion
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        // 原地放砖
                        //        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        //        {
                        //            Order = DevCarrierOrderE.放砖指令
                        //        });

                        //        #region 【任务步骤记录】
                        //        _M.LogForCarrierGiving(trans);
                        //        #endregion
                        //        return;
                        //    }

                        //}
                        #endregion

                        if (isNotLoad && isStopNoOrder)
                        {
                            _M.SetUnLoadTime(trans);
                            // 检测轨道存砖状态
                            PubMaster.Track.CheckTrackStockStatus(trans.give_track_id);

                            _M.SetStatus(trans, TransStatusE.完成);
                            return;
                        }
                    }
                    else
                    {
                        //至摆渡车
                        MoveToFerrySeamless(trans, false);
                        return;
                    }
                    #endregion
                    break;
                    #endregion
            }
        }


        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public override void FinishStockTrans(StockTrans trans)
        {
            PubMaster.Warn.RemoveTaskAllWarn(trans.id);
            _M.SetTransDtlTransFinish(trans.id);
            _M.SetFinish(trans);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            if (trans.carrier_id == 0 && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 5))
            {
                _M.SetStatus(trans, TransStatusE.完成);
                return;
            }

            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            //if (isLoad && isStopNoOrder)
            //{
            //    _M.SetLoadTime(trans);
            //    _M.SetStatus(trans, TransStatusE.放砖流程, "已取砖，继续放砖流程");
            //    return;
            //}

            switch (track.Type)
            {
                #region[小车轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                case TrackTypeE.上砖轨道:
                case TrackTypeE.下砖轨道:
                    if (isStopNoOrder)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }

                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.后置摆渡轨道:
                case TrackTypeE.前置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, trans.AllocateFerryType, track)) return;

                    if (isStopNoOrder)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out uint ferryTraid, out string res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            // 移至轨道定位点
                            MoveToPos(trans.take_track_id, trans.carrier_id, trans.id, track.Type == TrackTypeE.后置摆渡轨道 ? CarrierPosE.轨道后侧定位点 : CarrierPosE.轨道前侧定位点);

                            #region 【任务步骤记录】
                            _M.LogForCarrierToTrack(trans, trans.take_track_id);
                            #endregion
                            return;

                        }
                    }

                    break;
                    #endregion

            }
        }



        #region[其他流程]


        /// <summary>
        /// 移车中
        /// </summary>
        /// <param name="trans"></param>
        public override void MovingCarrier(StockTrans trans)
        {

        }


        /// <summary>
        /// 倒库中
        /// </summary>
        /// <param name="trans"></param>
        public override void SortingStock(StockTrans trans)
        {

        }

        /// <summary>
        /// 运输车回轨
        /// </summary>
        /// <param name="trans"></param>
        public override void ReturnCarrrier(StockTrans trans)
        {

        }
        /// <summary>
        /// 倒库暂停
        /// </summary>
        /// <param name="trans"></param>
        public override void SortTaskWait(StockTrans trans)
        {

        }

        /// <summary>
        /// 接力等待
        /// </summary>
        /// <param name="trans"></param>
        public override void Out2OutRelayWait(StockTrans trans)
        {

        }

        public override void OtherAction(StockTrans trans)
        {

        }

        public override void ReturnDevBackToTrack(StockTrans trans)
        {

        }

        public override void Organizing(StockTrans trans)
        {

        }

        #endregion
    }
}
