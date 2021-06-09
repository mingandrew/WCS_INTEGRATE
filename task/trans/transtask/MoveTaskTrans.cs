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
            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            // 运行前提
            if (!_M.RunPremise(trans, out Track track))
            {
                return;
            }

            switch (track.Type)
            {
                #region[上砖机轨道]
                case TrackTypeE.上砖轨道:
                    break;
                #endregion

                #region[下砖机轨道]
                case TrackTypeE.下砖轨道:
                    break;
                #endregion

                #region[储砖入轨道]
                case TrackTypeE.储砖_入:
                    // 小车不在作业轨道
                    if (track.id != trans.take_track_id && track.id != trans.give_track_id)
                    {
                        _M.SetStatus(trans, TransStatusE.完成, "任务运输车不在指定作业轨道，结束任务");
                        return;
                    }

                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, trans.give_track_id);
                        #endregion

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }

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

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                                #endregion

                                //前进至点
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                });
                                return;
                            }
                        }
                        else//不同轨道
                        {
                            #region[分配摆渡车]
                            //还没有分配取货过程中的摆渡车
                            if (trans.take_ferry_id == 0)
                            {
                                string msg = _M.AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);

                                #region 【任务步骤记录】
                                if (_M.LogForTakeFerry(trans, msg)) return;
                                #endregion
                            }
                            #endregion

                            //摆渡车接车
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                #endregion
                                return;
                            }

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                //后退至摆渡车
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                });
                                return;
                            }

                        }
                    }

                    if (track.id == trans.give_track_id
                        && PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[储砖出轨道]
                case TrackTypeE.储砖_出:
                    // 小车不在作业轨道
                    if (track.id != trans.take_track_id && track.id != trans.give_track_id)
                    {
                        _M.SetStatus(trans, TransStatusE.完成, "任务运输车不在指定作业轨道，结束任务");
                        return;
                    }

                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, trans.give_track_id);
                        #endregion

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }

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

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                                #endregion

                                //后退至点
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
                                });
                                return;
                            }
                        }
                        else//不同轨道
                        {
                            #region[分配摆渡车]
                            //还没有分配取货过程中的摆渡车
                            if (trans.take_ferry_id == 0)
                            {
                                string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);

                                #region 【任务步骤记录】
                                if (_M.LogForTakeFerry(trans, msg)) return;
                                #endregion
                            }
                            #endregion

                            //摆渡车接车
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                #endregion
                                return;
                            }

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                //前进至摆渡车
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                });
                                return;
                            }

                        }
                    }

                    if (track.id == trans.give_track_id
                        && PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[储砖出入轨道]
                case TrackTypeE.储砖_出入:

                    // 小车不在作业轨道
                    if (track.id != trans.take_track_id && track.id != trans.give_track_id)
                    {
                        _M.SetStatus(trans, TransStatusE.完成, "任务运输车不在指定作业轨道，结束任务");
                        return;
                    }

                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, track.id);
                        #endregion

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }

                        return;
                    }

                    if (track.id == trans.take_track_id)
                    {
                        #region[分配摆渡车]
                        //还没有分配取货过程中的摆渡车
                        if (trans.take_ferry_id == 0)
                        {
                            string msg = "";
                            //分配下摆渡车
                            if (PubTask.Carrier.IsCarrierInTrackSmallerSite(trans.carrier_id, track.id)
                                && !PubTask.Carrier.ExistCarBehind(trans.carrier_id, track.id))
                            {
                                msg = _M.AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);
                            }
                            //分配上摆渡车
                            else if (PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, track.id)
                                && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                            {
                                msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);
                            }

                            #region 【任务步骤记录】
                            if (_M.LogForTakeFerry(trans, msg)) return;
                            #endregion
                        }
                        #endregion

                        //摆渡车接车
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                            #endregion
                            return;
                        }

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                            #endregion

                            //后退至摆渡车
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.定位指令,
                                CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                            });
                            return;
                        }
                    }

                    if (track.id == trans.give_track_id
                        && PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }

                    break;
                #endregion

                #region[摆渡车入]
                case TrackTypeE.摆渡车_入:

                    #region[分配摆渡车]
                    //还没有分配取货过程中的摆渡车
                    if (trans.take_ferry_id == 0)
                    {
                        string msg = _M.AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);

                        #region 【任务步骤记录】
                        if (_M.LogForTakeFerry(trans, msg)) return;
                        #endregion
                    }
                    #endregion

                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                            #endregion
                            return;
                        }

                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, trans.give_track_id);
                        #endregion

                        //前进至点
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
                        });
                        return;
                    }

                    break;
                #endregion

                #region[摆渡车出]
                case TrackTypeE.摆渡车_出:

                    #region[分配摆渡车]
                    //还没有分配取货过程中的摆渡车
                    if (trans.take_ferry_id == 0)
                    {
                        string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);

                        #region 【任务步骤记录】
                        if (_M.LogForTakeFerry(trans, msg)) return;
                        #endregion
                    }
                    #endregion

                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                            #endregion
                            return;
                        }

                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, trans.give_track_id);
                        #endregion

                        //后退至点
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                            ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                        });
                        return;
                    }

                    break;
                    #endregion
            }
        }

        public override void FinishStockTrans(StockTrans trans)
        {
            _M.SetFinish(trans);
        }

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

        #endregion
    }
}
