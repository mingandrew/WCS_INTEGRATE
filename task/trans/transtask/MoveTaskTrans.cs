using enums;
using enums.track;
using module.goods;
using module.track;
using resource;
using task.device;

namespace task.trans.transtask
{
    /// <summary>
    /// 移车任务
    /// 转移轨道-流程（code- XX03）
    /// </summary>
    public class MoveTaskTrans : BaseTaskTrans
    {
        public MoveTaskTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 移车中
        /// </summary>
        /// <param name="trans"></param>
        public override void MovingCarrier(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            // 不在摆渡 & 取放轨道
            if (!track.IsFerryTrack() && track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "任务运输车不在指定作业轨道，结束任务");
                return;
            }

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            switch (track.Type)
            {
                #region[储砖入轨道]
                case TrackTypeE.储砖_入:
                    if (isLoad)
                    {
                        if (isStopNoOrder)
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }

                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, track.id);
                        #endregion
                        return;
                    }

                    if (track.id == trans.take_track_id)
                    {
                        //切换出入侧 [同轨道-不同侧]
                        if (track.brother_track_id == trans.give_track_id)
                        {
                            if (PubTask.Carrier.HaveInTrack(trans.give_track_id))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1003, string.Format("存在其他运输车在任务目的轨道[ {0} ]，无法继续执行移车任务流程；",
                                    PubMaster.Track.GetTrackName(trans.give_track_id)));
                                #endregion
                                return;
                            }

                            if (!PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1103, string.Format("任务运输车[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion
                                return;
                            }

                            if (isStopNoOrder)
                            {
                                MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);

                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                                #endregion
                                return;
                            }
                        }
                        else//不同轨道
                        {
                            if (trans.AllocateFerryType == DeviceTypeE.其他) _M.SetAllocateFerryType(trans, DeviceTypeE.后摆渡);
                            MoveToFerrySeamless(trans, true);
                            return;
                        }
                    }

                    if (track.id == trans.give_track_id && isStopNoOrder)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[储砖出轨道]
                case TrackTypeE.储砖_出:
                    if (isLoad)
                    {
                        if (isStopNoOrder)
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }

                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, track.id);
                        #endregion
                        return;
                    }

                    if (track.id == trans.take_track_id)
                    {
                        //切换出入侧 [同轨道-不同侧]
                        if (track.brother_track_id == trans.give_track_id)
                        {
                            if (PubTask.Carrier.HaveInTrack(trans.give_track_id))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1203, string.Format("存在其他运输车在任务目的轨道[ {0} ]，无法继续执行移车任务流程；",
                                    PubMaster.Track.GetTrackName(trans.give_track_id)));
                                #endregion
                                return;
                            }

                            if (!PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1303, string.Format("任务运输车[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion
                                return;
                            }

                            if (isStopNoOrder)
                            {
                                MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道后侧定位点);

                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                                #endregion
                                return;
                            }
                        }
                        else//不同轨道
                        {
                            if (trans.AllocateFerryType == DeviceTypeE.其他) _M.SetAllocateFerryType(trans, DeviceTypeE.前摆渡);
                            MoveToFerrySeamless(trans, true);
                            return;
                        }
                    }

                    if (track.id == trans.give_track_id && isStopNoOrder)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[轨道]
                case TrackTypeE.下砖轨道:
                case TrackTypeE.上砖轨道:
                case TrackTypeE.储砖_出入:
                    if (isLoad)
                    {
                        if (isStopNoOrder)
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }

                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, track.id);
                        #endregion
                        return;
                    }

                    if (track.id == trans.take_track_id)
                    {
                        //还没有分配摆渡
                        if (trans.AllocateFerryType == DeviceTypeE.其他)
                        {
                            if (carrier.CurrentPoint >= track.split_point || carrier.TargetPoint >= track.split_point)
                            {
                                // 超过中间点  前进-脉冲最大的
                                _M.SetAllocateFerryType(trans, DeviceTypeE.前摆渡);
                            }
                            else
                            {
                                // 超过中间点  后退-脉冲最小的
                                _M.SetAllocateFerryType(trans, DeviceTypeE.后摆渡);
                            }
                        }

                        //摆渡车接车
                        MoveToFerrySeamless(trans, true);
                        return;
                    }

                    if (track.id == trans.give_track_id && isStopNoOrder)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }

                    break;
                #endregion

                #region[摆渡车入]
                case TrackTypeE.后置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, DeviceTypeE.后摆渡, track)) return;

                    if (isStopNoOrder)
                    {
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out uint ferryTraid, out string res))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                            #endregion
                            return;
                        }

                        MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道后侧定位点);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, trans.give_track_id);
                        #endregion
                        return;
                    }

                    break;
                #endregion

                #region[摆渡车出]
                case TrackTypeE.前置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, DeviceTypeE.前摆渡, track)) return;

                    if (isStopNoOrder)
                    {
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out uint ferryTraid, out string res))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                            #endregion
                            return;
                        }

                        MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, trans.give_track_id);
                        #endregion
                        return;
                    }

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
            _M.SetFinish(trans);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            _M.SetStatus(trans, TransStatusE.完成);
        }


        #region[其他流程]

        public override void AllocateDevice(StockTrans trans)
        {

        }

        public override void ToTakeTrackTakeStock(StockTrans trans)
        {

        }

        public override void ToGiveTrackGiveStock(StockTrans trans)
        {

        }
        public override void CheckingTrack(StockTrans trans)
        {

        }

        public override void OtherAction(StockTrans trans)
        {

        }

        public override void Out2OutRelayWait(StockTrans trans)
        {

        }

        public override void ReturnCarrrier(StockTrans trans)
        {

        }

        public override void ReturnDevBackToTrack(StockTrans trans)
        {

        }

        public override void SortingStock(StockTrans trans)
        {

        }

        public override void SortTaskWait(StockTrans trans)
        {

        }

        public override void Organizing(StockTrans trans)
        {
        }
        #endregion
    }
}
