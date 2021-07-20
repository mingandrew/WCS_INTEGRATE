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
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id);

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
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                //至摆渡车
                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.上砖摆渡复位点);
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
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                // 至摆渡车
                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.上砖摆渡复位点);
                            }
                            return;
                        }
                        else
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierGiving(trans);
                            #endregion

                            // 下降放货
                            PubTask.Carrier.DoOrder(carrierid, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            }, "库存转移下降放货");
                        }
                    }

                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_出:
                case TrackTypeE.摆渡车_入:

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
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id);

            switch (track.Type)
            {
                #region[小车在摆渡车上]
                case TrackTypeE.摆渡车_出:
                case TrackTypeE.摆渡车_入:
                    if (isload)
                    {
                        //1.计算轨道下一车坐标
                        //2.卸货轨道状态是否运行放货                                    
                        //3.是否有其他车在同轨道上
                        if ((!PubMaster.Goods.CalculateNextLocation(trans.TransType, trans.carrier_id, trans.give_track_id, out ushort count, out ushort loc)
                            || !PubMaster.Track.IsStatusOkToGive(trans.give_track_id)
                            || PubTask.Carrier.HaveInTrack(trans.give_track_id, trans.carrier_id))
                            && isftask)
                        {
                            if (loc == 0)
                            {
                                // 设满砖
                                PubMaster.Track.UpdateStockStatus(trans.give_track_id, TrackStockStatusE.满砖, "计算坐标值无法存入下一车");
                                PubMaster.Track.AddTrackLog(count, trans.carrier_id, trans.give_track_id, TrackLogE.满轨道, "计算坐标值无法存入下一车");

                                #region 【任务步骤记录】
                                _M.LogForTrackFull(trans, trans.give_track_id);
                                #endregion
                            }

                            bool isWarn = false;
                            // 换轨道？？？？？？？？？

                            if (isWarn)
                            {
                                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.TransHaveNotTheGiveTrack, trans.id);
                            }
                            else
                            {
                                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.TransHaveNotTheGiveTrack, (ushort)trans.carrier_id, trans.id);

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1600, string.Format("没有找到合适的轨道卸砖，继续尝试寻找其他轨道；"));
                                #endregion
                            }

                            return;
                        }

                        //小车在摆渡车上
                        if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                        {
                            if (isftask)
                            {
                                if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                                    #endregion
                                    return;
                                }

                                #region 【任务步骤记录】
                                _M.LogForCarrierGive(trans, trans.give_track_id);
                                #endregion

                                Track givetrack = PubMaster.Track.GetTrack(trans.give_track_id);
                                //后退放砖
                                //CarrierActionOrder cao = new CarrierActionOrder
                                //{
                                //    Order = DevCarrierOrderE.放砖指令,
                                //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id)
                                //};

                                if (loc == 0)
                                {
                                    if (givetrack.Type == TrackTypeE.储砖_出入)
                                    {
                                        //cao.ToPoint = givetrack.limit_point;
                                        loc = givetrack.limit_point;
                                    }

                                    if (givetrack.Type == TrackTypeE.储砖_入)
                                    {
                                        //cao.ToPoint = givetrack.split_point;
                                        loc = givetrack.split_point;
                                    }
                                }
                                else
                                {
                                    //cao.OverRFID = givetrack.rfid_1;
                                    PubMaster.Goods.UpdateStockLocationCal(trans.stock_id, loc);
                                }
                                //cao.ToTrackId = trans.give_track_id;
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, cao);
                                MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, loc);
                                return;
                            }


                            //摆渡车 定位去 放货点
                            //小车到达摆渡车后短暂等待再开始定位
                            if (_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                            {
                                //后退放砖
                                //TODO  后退放砖，但是使用脉冲间隔放砖

                                Stock topstock = PubMaster.Goods.GetTrackTopStock(trans.give_track_id);
                                if (topstock != null)
                                {

                                }

                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在放砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                    #region[放货轨道]
                    if (!trans.IsReleaseGiveFerry
                            && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                            && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                    {
                        trans.IsReleaseGiveFerry = true;
                        _M.FreeGiveFerry(trans);
                    }

                    if (track.id == trans.give_track_id)
                    {
                        PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                    }

                    if (isnotload && isftask)
                    {
                        _M.SetUnLoadTime(trans);
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
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id);

            if (isload)
            {
                if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                {
                    _M.SetLoadTime(trans);
                    _M.SetStatus(trans, TransStatusE.放砖流程);
                    return;
                }
            }
            switch (track.Type)
            {
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_入:
                    if (isnotload)
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        {
                            _M.SetStatus(trans, TransStatusE.完成);
                        }
                    }

                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_入:
                case TrackTypeE.摆渡车_出:
                    if (isnotload)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        {
                            //小车回到原轨道
                            if (_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                            {
                                //前进至点
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                //    ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
                                //    ToTrackId = trans.finish_track_id
                                //});
                                MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, track.Type == TrackTypeE.摆渡车_入 ? CarrierPosE.轨道后侧定位点 : CarrierPosE.轨道前侧定位点);
                            }
                        }
                    }

                    break;
                #endregion

                #region[小车在下砖轨道]
                case TrackTypeE.下砖轨道:

                    if (isload)
                    {
                        if (track.id == trans.take_track_id
                            && PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id)
                            && mTimer.IsOver(TimerTag.CarrierGotLoad, trans.carrier_id, 1, 5))
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                        }
                    }

                    if (isnotload)
                    {
                        if (track.id == trans.take_track_id)
                        {
                            //小车回到原轨道
                            //没有任务并且停止
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                && _M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _, true))
                            {
                                //前进至摆渡车
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                //    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                //    ToTrackId = ferryTraid
                                //});
                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.下砖摆渡复位点);
                            }
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
