using enums;
using enums.track;
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
            if (CheckTrackAndAddMoveTask(trans, trans.take_track_id)) return;

            //转移卸货轨道不符合的运输车
            if (CheckTrackAndAddMoveTask(trans, trans.give_track_id)) return;

            _M.SetStatus(trans, TransStatusE.调度设备);
        }

        /// <summary>
        /// 调度设备
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            //转移取货轨道不符合的运输车
            if (CheckTrackAndAddMoveTask(trans, trans.take_track_id)) return;

            //转移卸货轨道不符合的运输车
            if (CheckTrackAndAddMoveTask(trans, trans.give_track_id)) return;

            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (!_M.HaveGiveInTrackId(trans))
            {
                //分配运输车
                if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                    && !_M.HaveInCarrier(carrierid)
                    && mTimer.IsOver(TimerTag.CarrierAllocate, trans.take_track_id, 2, 5)
                    )
                {
                    _M.SetCarrier(trans, carrierid);
                    _M.SetStatus(trans, TransStatusE.取砖流程);
                }
            }
        }

        #region[取货流程 - 取货轨道 下砖机轨道，储砖出，储砖入轨道]

        /// <summary>
        /// 取货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToTakeTrackTakeStock(StockTrans trans)
        {
            //小车没有被其他任务占用
            if (_M.HaveCarrierInTrans(trans)) return;

            //小车当前所在的轨道
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;

            if (trans.take_ferry_id != 0
                && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
            {
                return;
            }

            Track taketrack = PubMaster.Track.GetTrack(trans.take_track_id);
            Track givetrack = PubMaster.Track.GetTrack(trans.give_track_id);

            bool isuparea = false;
            if(taketrack.InType(TrackTypeE.储砖_入, TrackTypeE.下砖轨道)
                || givetrack.InType(TrackTypeE.下砖轨道))
            {
                isuparea = false;
            }
            else if(taketrack.InType(TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入)
                || givetrack.InType(TrackTypeE.上砖轨道))
            {
                isuparea = true;
            }


            #region[分配摆渡车]

            //还没有分配取货过程中的摆渡车
            if (trans.take_ferry_id == 0)
            {
                _M.AllocateFerry(trans, isuparea ? DeviceTypeE.上摆渡 : DeviceTypeE.下摆渡, track, false);
                //调度摆渡车接运输车
            }
            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            ftask = PubTask.Carrier.IsStopFTask(trans.carrier_id);
            switch (track.Type)
            {
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_入:
                    if(track.id == trans.take_track_id)
                    {
                        if (isload && ftask)
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                        }
                    }
                    break;
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                    if (isnotload && ftask)
                    {
                        //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        {
                            //摆渡车接车
                            if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                            {
                                //前进至摆渡车
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    ToTrackId = ferryTraid
                                });
                            }
                        }
                    }

                    if (isload && ftask)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            _M.SetLoadTime(trans);
                            //摆渡车接车
                            if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                            {
                                //前进至摆渡车
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    ToTrackId = ferryTraid
                                });
                            }
                            return;
                        }
                        else
                        {
                            //下降放货
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

                    if (isnotload && ftask)
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


                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, trans.take_track_id);
                            #endregion

                            //后退取砖
                            CarrierActionOrder cao = new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.取砖指令,
                                CheckTra = taketrack.ferry_down_code,
                            };

                            switch (taketrack.Type)
                            {
                                case TrackTypeE.上砖轨道:
                                case TrackTypeE.下砖轨道:
                                    //获取砖机配置的取货点
                                    ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.take_track_id);
                                    if (torfid == 0)
                                    {
                                        //如果配置为零则获取取货轨道的rfid1
                                        torfid = taketrack.rfid_1;// PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                    }
                                    cao.ToRFID = torfid;
                                    break;
                                case TrackTypeE.储砖_入:

                                    break;
                                case TrackTypeE.储砖_出:
                                    // 去分段点取，回轨道出库地标
                                    cao.ToPoint = taketrack.split_point;// PubMaster.Track.GetTrackSplitPoint(trans.take_track_id);
                                    cao.OverRFID = taketrack.rfid_1;// PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                    break;
                                case TrackTypeE.储砖_出入:
                                    // 去入库地标取，回轨道出库地标
                                    cao.ToRFID = taketrack.rfid_1;// PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                    cao.OverRFID = taketrack.rfid_2;// PubMaster.Track.GetTrackRFID2(trans.take_track_id);
                                    break;
                            }
                            cao.ToTrackId = trans.take_track_id;
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, cao);
                        }
                    }

                    if (isload && ftask)
                    {
                        PubMaster.Goods.MoveStock(trans.stock_id, track.id);
                        _M.SetLoadTime(trans);
                        _M.SetStatus(trans, TransStatusE.放砖流程);
                    }

                    break;
                    #endregion

                #region[上砖轨道]



                #endregion

                #region[下砖轨道]


                #endregion
            }
        }

        /// <summary>
        /// 在上砖区域
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="taketrack"></param>
        /// <param name="givetrack"></param>
        private void InUpAreaToTakeTrackTakeStock(StockTrans trans, Track taketrack, Track givetrack)
        {
        }

        private void InDownAreaToTakeTrackTakeStock(StockTrans trans, Track taketrack, Track givetrack)
        {

        }
        #endregion

        /// <summary>
        /// 放货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToGiveTrackGiveStock(StockTrans trans)
        {
            //小车没有被其他任务占用
            if (_M.HaveCarrierInTrans(trans)) return;

            //小车当前所在的轨道
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;

            #region[分配摆渡车/锁定摆渡车]

            if (trans.give_ferry_id == 0)
            {
                //还没有分配取货过程中的摆渡车
                _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, true);
                //调度摆渡车接运输车
            }
            else if (!PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id))
            {
                return;
            }

            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            ftask = PubTask.Carrier.IsStopFTask(trans.carrier_id);
            switch (track.Type)
            {
                #region[小车在摆渡车上]
                case TrackTypeE.摆渡车_出:
                    if (isload)
                    {
                        //小车在摆渡车上
                        if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                        {
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
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

                                //需要定位的位置
                                ushort loc = 0;

                                //后退放砖
                                //TODO  后退放砖，但是使用脉冲间隔放砖

                                Stock topstock = PubMaster.Goods.GetTrackTopStock(trans.give_track_id);
                                if (topstock != null)
                                {
                                    float movecarseperate = GlobalWcsDataConfig.BigConifg.MoveStockSeperateCarCount;
                                    ushort safe = PubMaster.Goods.GetStackSafe(0, 0);
                                    ushort space = (ushort)(movecarseperate * safe);
                                    if(space < safe)
                                    {
                                        space = (ushort)(safe * 2);
                                    }
                                    loc = topstock.location;
                                    loc += space;
                                }

                                //后退放砖
                                CarrierActionOrder cao = new CarrierActionOrder
                                {
                                    Order = DevCarrierOrderE.放砖指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id)
                                };

                                Track givetrack = PubMaster.Track.GetTrack(trans.give_track_id);

                                cao.OverRFID = givetrack.rfid_2;
                                if (loc == 0 && givetrack.StockStatus == TrackStockStatusE.空砖)
                                {
                                    if (givetrack.Type == TrackTypeE.储砖_出入)
                                    {
                                        cao.ToRFID = givetrack.rfid_1;
                                    }

                                    if (givetrack.Type == TrackTypeE.储砖_入)
                                    {
                                        cao.ToPoint = givetrack.split_point;
                                    }
                                }

                                if(loc != 0)
                                {
                                    if(givetrack.Type == TrackTypeE.储砖_出
                                        && givetrack.split_point != 0
                                        && givetrack.split_point > loc)
                                    {
                                        loc = givetrack.split_point;
                                    }

                                    if(givetrack.Type == TrackTypeE.储砖_出入
                                        && givetrack.limit_point != 0
                                        && givetrack.limit_point > loc)
                                    {
                                        loc = givetrack.limit_point;
                                    }

                                    cao.ToPoint = loc;
                                    PubMaster.Goods.UpdateStockLocationCal(trans.stock_id, loc);
                                }

                                cao.ToTrackId = trans.give_track_id;
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, cao,string.Format("库存整理: 轨道[ {0} ]，放货脉冲[ {1} ], 地标[ {2} ]",givetrack.StockStatus, cao.ToPoint, cao.ToRFID));
                                return;
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

                    if (isnotload && ftask)
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
            //小车当前所在的轨道
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

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

                    if (isnotload)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        {
                            //小车回到原轨道
                            if (_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                            {
                                //前进至点
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
                                    ToTrackId = trans.finish_track_id
                                });
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
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    ToTrackId = ferryTraid
                                });
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
