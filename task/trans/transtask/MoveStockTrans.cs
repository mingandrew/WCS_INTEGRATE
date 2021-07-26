using enums;
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
    /// 库存整理
    /// 1.单品种库存，转移轨道
    /// 2.多品种库存，分开轨道
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
            if (CheckTrackAndAddMoveTask(trans, trans.take_track_id))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1011, string.Format("有不符合规格作业要求的运输车停在[ {0} ]，尝试对其生成移车任务；",
                    PubMaster.Track.GetTrackName(trans.take_track_id)));
                #endregion
                return;
            }

            //转移卸货轨道不符合的运输车
            if (CheckTrackAndAddMoveTask(trans, trans.give_track_id))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1111, string.Format("有不符合规格作业要求的运输车停在[ {0} ]，尝试对其生成移车任务；",
                    PubMaster.Track.GetTrackName(trans.give_track_id)));
                #endregion
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
            if (_M.HaveGiveInTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1211, string.Format("存在相同作业轨道的任务，等待任务完成；"));
                #endregion
                return;
            }

            //分配运输车
            if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                && !_M.HaveInCarrier(carrierid)
                && mTimer.IsOver(TimerTag.CarrierAllocate, trans.take_track_id, 2, 5))
            {
                _M.SetCarrier(trans, carrierid);
                _M.SetStatus(trans, TransStatusE.取砖流程);
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
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            if (trans.take_ferry_id != 0
                && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
            {
                return;
            }

            #region[分配摆渡车]
            //还没有分配取货过程中的摆渡车
            if (trans.take_ferry_id == 0)
            {
                string msg = _M.AllocateFerry(trans, trans.AllocateFerryType, track, false);

                #region 【任务步骤记录】
                if (_M.LogForTakeFerry(trans, msg)) return;
                #endregion
            }
            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            switch (track.Type)
            {
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                    if (isnotload && isftask)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            // 轨道内直接取砖
                            TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out res);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, trans.take_track_id, res);
                            #endregion
                            return;
                        }
                        else
                        {
                            //摆渡车接车
                            if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                            {
                                //至摆渡车
                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);

                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion
                                return;

                            }
                        }
                    }

                    if (isload && isftask)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            _M.SetLoadTime(trans);
                            // 摆渡车接车
                            if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                            {
                                // 至摆渡车
                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);

                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion
                                return;
                            }
                        }
                        else
                        {
                            // 下降放货
                            PubTask.Carrier.DoOrder(carrierid, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            }, "库存转移下降放货");

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

                    if (isnotload && isftask)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            //摆渡车 定位去 取货点
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            // 直接取砖
                            TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out res);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, trans.take_track_id, res);
                            #endregion
                            return;
                        }
                    }

                    if (isload && isftask)
                    {
                        PubMaster.Goods.MoveStock(trans.stock_id, track.id);
                        _M.SetLoadTime(trans);
                        _M.SetStatus(trans, TransStatusE.放砖流程);
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
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            #region[分配摆渡车/锁定摆渡车]

            if (trans.give_ferry_id == 0)
            {
                string msg = _M.AllocateFerry(trans, trans.AllocateFerryType, track, true);

                #region 【任务步骤记录】
                if (_M.LogForGiveFerry(trans, msg)) return;
                #endregion
            }
            else if (!PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id))
            {
                return;
            }

            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            switch (track.Type)
            {
                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                case TrackTypeE.后置摆渡轨道:
                    if (isload && isftask)
                    {
                        //1.计算轨道下一车坐标
                        //2.卸货轨道状态是否运行放货                                    
                        //3.是否有其他车在同轨道上
                        if (CheckTrackFull(trans, trans.give_track_id, out ushort loc)
                            || !PubMaster.Track.IsStatusOkToGive(trans.give_track_id)
                            || PubTask.Carrier.HaveInTrack(trans.give_track_id, trans.carrier_id))
                        {
                            bool isWarn = false;

                            // 换轨道?
                            List<uint> newTraIDs = _M.GetOutTrackIDByInTrack(PubMaster.Track.GetTrack(trans.take_track_id), trans.goods_id);
                            if (newTraIDs != null && newTraIDs.Count > 0)
                            {
                                foreach (uint traid in newTraIDs)
                                {
                                    if (!PubTask.Carrier.HaveInTrack(traid, trans.carrier_id)
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
                                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.TransHaveNotTheGiveTrack, (ushort)trans.carrier_id, trans.id);

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1311, string.Format("没有找到合适的轨道卸砖，继续尝试寻找其他轨道；"));
                                #endregion
                            }

                            return;
                        }

                        //小车在摆渡车上
                        if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                        {
                            if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                                #endregion
                                return;
                            }

                            // 直接放砖
                            GiveInTarck(loc, trans.take_track_id, trans.carrier_id, trans.id, out res);

                            #region 【任务步骤记录】
                            _M.LogForCarrierGive(trans, trans.give_track_id, res);
                            #endregion
                            return;
                        }

                    }
                    break;
                #endregion

                #region[小车在放砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                    #region[放货轨道]
                    if (!trans.IsReleaseGiveFerry
                            && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                            && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                    {
                        trans.IsReleaseGiveFerry = true;
                        _M.FreeGiveFerry(trans);
                    }

                    if (PubTask.Carrier.IsCarrierFinishUnLoad(trans.carrier_id))
                    {
                        _M.SetUnLoadTime(trans);

                        CheckTrackFull(trans, trans.give_track_id, out ushort loc);

                        _M.SetStatus(trans, TransStatusE.完成);
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
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            if (isload)
            {
                if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                {
                    _M.SetLoadTime(trans);
                    _M.SetStatus(trans, TransStatusE.放砖流程, "已取砖，继续放砖流程");
                    return;
                }
            }

            #region[分配摆渡车/锁定摆渡车]

            if (track.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
            {
                if (trans.take_ferry_id == 0)
                {
                    string msg = _M.AllocateFerry(trans, trans.AllocateFerryType, track, false);

                    #region 【任务步骤记录】
                    if (_M.LogForTakeFerry(trans, msg)) return;
                    #endregion
                }
                else if (!PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
                {
                    return;
                }
            }

            #endregion

            switch (track.Type)
            {
                #region[小车轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_入:
                case TrackTypeE.上砖轨道:
                case TrackTypeE.下砖轨道:
                    if (isnotload && isftask)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }

                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.后置摆渡轨道:
                case TrackTypeE.前置摆渡轨道:
                    if (isnotload && isftask)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
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
