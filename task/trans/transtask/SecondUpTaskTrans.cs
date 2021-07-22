using enums;
using enums.track;
using enums.warning;
using module.goods;
using module.tiletrack;
using module.track;
using resource;
using System.Collections.Generic;
using task.device;
using tool.appconfig;

namespace task.trans.transtask
{
    //反抛
    public class SecondUpTaskTrans : BaseTaskTrans
    {
        public SecondUpTaskTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 判断反抛是否可以执行
        /// 1.上砖机工位要空
        /// 2.出库轨道要没有当前砖机的库存
        /// 3.上砖机转产后，上新的品种前就要执行反抛任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {

            //上砖机轨道有砖就不能反抛
            if (PubTask.TileLifter.IsTileLoad(trans.tilelifter_id, trans.give_track_id))
            {
                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.Warning34, (ushort)trans.tilelifter_id, trans.id);
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1009, string.Format("上砖机工位有砖，不能执行反抛任务！"));
                #endregion
                return;
            }
            else
            {
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.Warning34, trans.id);
            }

            //上砖机当前品种的砖没有了可上的库存  或者   上砖机转产了，品种变了
            if ((!PubMaster.Goods.ExistStockInTrackTopCanUp(trans.goods_id)
                || !PubTask.TileLifter.EqualTileGood(trans.tilelifter_id, trans.goods_id))
                && !PubTask.Trans.HaveInGoods(trans.area_id, trans.goods_id, TransTypeE.上砖任务))
            {
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.Warning35, trans.id);
                _M.SetStatus(trans, TransStatusE.调度设备);
                return;
            }

            if (PubMaster.Goods.ExistStockInTrackTopCanUp(trans.goods_id))
            {
                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.Warning35, (ushort)trans.tilelifter_id, trans.id);
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1109, string.Format("出库轨道库存里还有【{0}】可上，不能执行反抛任务！", PubMaster.Goods.GetGoodsName(trans.goods_id)));
                #endregion
                return;
            }
        }

        /// <summary>
        /// 1.分配摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (_M.HaveTaskSortTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1209, string.Format("存在相同作业轨道的任务，等待任务完成；"));
                #endregion
                return;
            }

            //分配运输车
            if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
                && !_M.HaveInCarrier(carrierid))
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
        /// 取砖流程
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
                && !trans.IsReleaseTakeFerry
                && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
            {
                return;
            }

            #region[分配摆渡车]
            //还没有分配取货过程中的摆渡车
            if (trans.take_ferry_id == 0 && !trans.IsReleaseTakeFerry)
            {
                string msg = _M.AllocateFerry(trans, DeviceTypeE.前摆渡, track, false);

                #region 【任务步骤记录】
                if (_M.LogForTakeFerry(trans, msg)) return;
                #endregion
            }
            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            switch (track.Type)
            {
                #region[小车在储砖轨道]

                case TrackTypeE.储砖_出:

                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.LogForCarrierGiving(trans);
                        #endregion

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }
                        return;
                    }

                    if (isnotload)
                    {
                        //摆渡车接车
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                            #endregion
                            return;
                        }

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                            #endregion

                            //前进至摆渡车
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.定位指令,
                            //    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                            //    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                            //    ToTrackId = ferryTraid
                            //});
                            MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                            return;
                        }
                    }
                    break;

                case TrackTypeE.储砖_出入:
                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.LogForCarrierGiving(trans);
                        #endregion

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
                        }
                        return;
                    }

                    if (isnotload)
                    {
                        //摆渡车接车
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                            #endregion
                            return;
                        }

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                            #endregion

                            //前进至摆渡车
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.定位指令,
                            //    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                            //    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                            //    ToTrackId = ferryTraid
                            //});
                            MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                            return;
                        }
                    }
                    break;

                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:

                    if (isnotload)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                               && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //摆渡车 定位去 取货点
                            //小车到达摆渡车后短暂等待再开始定位
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            /**
                             * 介入了才能进去砖机轨道
                             */
                            if (!PubTask.TileLifter.IsGiveReadyWithBackUp(trans.tilelifter_id, trans.take_track_id, out res, true))
                            {

                                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.Warning34, (ushort)trans.tilelifter_id, trans.id);
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1309, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足放砖条件；{2}；",
                                    PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                    PubMaster.Track.GetTrackName(trans.take_track_id), res), true);
                                #endregion
                                return;
                            }

                            PubMaster.Warn.RemoveTaskWarn(WarningTypeE.Warning34, trans.id);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, trans.take_track_id);
                            #endregion

                            ushort torfid = PubMaster.Track.GetTrackRFID3(trans.take_track_id);
                            if (torfid == 0)
                            {
                                return;
                            }

                            //取砖
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.取砖指令,
                            //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                            //    ToRFID = torfid,
                            //    ToTrackId = trans.take_track_id,
                            //    OverRFID = torfid,
                            //});
                            MoveToTake(trans.take_track_id, trans.carrier_id, trans.id, torfid);
                            return;
                        }
                    }
                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:

                    if (!trans.IsReleaseTakeFerry
                        && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                        && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
                    {
                        trans.IsReleaseTakeFerry = true;
                        _M.FreeTakeFerry(trans);
                    }

                    if (isnotload)
                    {
                        /**
                         * 介入了才能进去砖机轨道
                         */
                        if (!PubTask.TileLifter.IsGiveReadyWithBackUp(trans.tilelifter_id, trans.take_track_id, out res, true))
                        {

                            PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.Warning34, (ushort)trans.tilelifter_id, trans.id);
                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 1309, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足放砖条件；{2}；",
                                PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                PubMaster.Track.GetTrackName(trans.take_track_id), res), true);
                            #endregion
                            return;
                        }

                        PubMaster.Warn.RemoveTaskWarn(WarningTypeE.Warning34, trans.id);


                        if (track.id == trans.give_track_id && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            ushort torfid = PubMaster.Track.GetTrackRFID3(trans.take_track_id);
                            if (torfid == 0)
                            {
                                return;
                            }

                            //取砖
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.取砖指令,
                            //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                            //    ToRFID = torfid,
                            //    ToTrackId = trans.take_track_id,
                            //    OverRFID = torfid,
                            //});
                            MoveToTake(trans.take_track_id, trans.carrier_id, trans.id, torfid);
                        }
                    }

                    if (isload)
                    {
                        if (track.id == trans.give_track_id)
                        {
                            //没有任务并且停止
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {

                                _M.SetLoadTime(trans);

                                _M.SetStatus(trans, TransStatusE.放砖流程);
                                return;
                            }
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
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isload)
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierGive(trans, trans.give_track_id);
                            #endregion

                            //获取砖机配置的取货点
                            ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                            if (torfid == 0)
                            {
                                //如果配置为零则获取取货轨道的rfid1
                                //torfid = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                torfid = PubMaster.Track.GetTrackLimitPointIn(trans.give_track_id);
                            }

                            //前进放砖
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.放砖指令,
                            //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                            //    ToRFID = torfid,
                            //    ToTrackId = trans.give_track_id
                            //});
                            MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, torfid);
                            return;
                        }
                    }

                    #region[要执行还车回归的操作]
                    if (isnotload)
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            _M.SetUnLoadTime(trans);

                            _M.SetStatus(trans, TransStatusE.还车回轨);
                            return;
                        }

                    }
                    #endregion

                    #region[放下砖马上完成任务-不执行]
                    //if (isnotload)
                    //{
                    //    //发送离开给上砖机
                    //    if (!trans.IsLeaveTileLifter
                    //        && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                    //    {
                    //        trans.IsLeaveTileLifter = true;
                    //        break;
                    //    }

                    //    _M.SetStatus(trans, TransStatusE.完成);
                    //}
                    #endregion

                    break;
                    #endregion
            }
        }


        /// <summary>
        /// 还车回轨
        /// </summary>
        /// <param name="trans"></param>
        public override void ReturnDevBackToTrack(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            #region[分配摆渡车/锁定摆渡车-不用分配摆渡车]

            //if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
            //{
            //    if (trans.give_ferry_id == 0)
            //    {
            //        string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, true);

            //        #region 【任务步骤记录】
            //        if (_M.LogForGiveFerry(trans, msg)) return;
            //        #endregion
            //    }
            //    else if (!PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id))
            //    {
            //        return;
            //    }
            //}

            #endregion

            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isnotload)
                    {
                        //发送离开给上砖机
                        if (!trans.IsLeaveTileLifter
                            && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                        {
                            trans.IsLeaveTileLifter = true;
                        }

                        if (track.id == trans.give_track_id && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            ushort torfid = PubMaster.Track.GetTrackRFID3(trans.take_track_id);
                            if (torfid == 0)
                            {
                                return;
                            }
                            if (PubTask.Carrier.GetCurrentSite(trans.carrier_id) == 0)
                            {
                                return;
                            }
                            if (PubTask.Carrier.GetCurrentSite(trans.carrier_id) != torfid)
                            {
                                //回到反抛取砖点
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                //    ToRFID = torfid,
                                //    ToTrackId = trans.take_track_id,
                                //});
                                MoveToLoc(trans.take_track_id, trans.carrier_id, trans.id, torfid);
                                return;
                            }

                            _M.SetStatus(trans, TransStatusE.完成);
                        }

                        #region[使用摆渡车回到储砖轨道-不用了]
                        //if (trans.give_ferry_id != 0)
                        //{
                        //    if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                        //    {
                        //        #region 【任务步骤记录】
                        //        _M.LogForFerryMove(trans, trans.give_ferry_id, track.id, res);
                        //        #endregion
                        //        return;
                        //    }

                        //    if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        //    {
                        //        #region 【任务步骤记录】
                        //        _M.LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
                        //        #endregion

                        //        // 后退至摆渡车
                        //        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        //        {
                        //            Order = DevCarrierOrderE.定位指令,
                        //            CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                        //            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                        //            ToTrackId = ferryTraid
                        //        });
                        //        return;
                        //    }
                        //}
                        #endregion

                        break;
                    }
                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                    if (isnotload)
                    {
                        //小车在摆渡车上
                        if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                        {
                            //发送离开给上砖机
                            if (!trans.IsLeaveTileLifter
                                && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                            {
                                trans.IsLeaveTileLifter = true;
                            }

                            if (trans.finish_track_id == 0)
                            {
                                //获取所有的出库轨道
                                List<uint> cantras = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出);
                                //按距离近的排序
                                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(cantras, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                foreach (uint t in tids)
                                {
                                    if (!_M.IsTraInTrans(t)
                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, t)
                                        && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                    {
                                        _M.SetFinishSite(trans, t, "反抛还车轨道分配轨道[1]");
                                        break;
                                    }
                                }
                            }

                            if (trans.finish_track_id != 0)
                            {
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                                {
                                    //摆渡车 定位去 取货点继续取砖
                                    //小车到达摆渡车后短暂等待再开始定位
                                    if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out res))
                                    {
                                        #region 【任务步骤记录】
                                        _M.LogForFerryMove(trans, trans.give_ferry_id, trans.finish_track_id, res);
                                        #endregion
                                        return;
                                    }

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.finish_track_id);
                                    #endregion

                                    // 后退至点
                                    //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                    //{
                                    //    Order = DevCarrierOrderE.定位指令,
                                    //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                    //    ToRFID = PubMaster.Track.GetTrackRFID2(trans.finish_track_id),
                                    //    ToTrackId = trans.finish_track_id
                                    //});
                                    MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在取砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:

                    if (!trans.IsReleaseGiveFerry
                        && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                        && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                    {
                        trans.IsReleaseGiveFerry = true;
                        _M.FreeGiveFerry(trans);
                    }

                    _M.SetStatus(trans, TransStatusE.完成);
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// 任务完成
        /// </summary>
        /// <param name="trans"></param>
        public override void FinishStockTrans(StockTrans trans)
        {
            _M.SetFinish(trans);
        }

        /// <summary>
        /// 任务取消
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            if (trans.carrier_id == 0
                        && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 10))
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

            #region[分配摆渡车/锁定摆渡车]

            if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
            {
                if (trans.take_ferry_id == 0)
                {
                    string msg = _M.AllocateFerry(trans, DeviceTypeE.前摆渡, track, false);

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
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:

                    if (!trans.IsReleaseTakeFerry
                        && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                        && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
                    {
                        trans.IsReleaseTakeFerry = true;
                        _M.FreeTakeFerry(trans);
                    }

                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:

                    if (isload)
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            //原地放砖
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令,
                            });
                        }
                    }

                    //释放摆渡车
                    if (!trans.IsReleaseTakeFerry
                        && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                        && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
                    {
                        trans.IsReleaseTakeFerry = true;
                        _M.FreeTakeFerry(trans);
                    }

                    if (isnotload)
                    {
                        //发送离开给上砖机
                        if (!trans.IsLeaveTileLifter
                            && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                        {
                            trans.IsLeaveTileLifter = true;
                        }

                        if (track.id == trans.give_track_id && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            ushort torfid = PubMaster.Track.GetTrackRFID3(trans.take_track_id);
                            if (torfid == 0)
                            {
                                return;
                            }
                            if (PubTask.Carrier.GetCurrentSite(trans.carrier_id) == 0)
                            {
                                return;
                            }
                            if (PubTask.Carrier.GetCurrentSite(trans.carrier_id) != torfid)
                            {
                                //回到反抛取砖点
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                //    ToRFID = torfid,
                                //    ToTrackId = trans.take_track_id,
                                //});
                                MoveToLoc(trans.take_track_id, trans.carrier_id, trans.id, torfid);
                                return;
                            }

                            _M.SetStatus(trans, TransStatusE.完成);
                        }
                    }
                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            if (trans.finish_track_id == 0)
                            {
                                //获取所有的出库轨道
                                List<uint> cantras = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出);
                                //按距离近的排序
                                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(cantras, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                foreach (uint t in tids)
                                {
                                    if (!_M.IsTraInTrans(t)
                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                        && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                    {
                                        _M.SetFinishSite(trans, t, "反抛还车轨道分配轨道[1]");
                                        break;
                                    }
                                }
                                return;
                            }

                            if (trans.finish_track_id != 0)
                            {
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                                {
                                    //摆渡车 定位去 取货点继续取砖
                                    //小车到达摆渡车后短暂等待再开始定位
                                    if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out res))
                                    {
                                        #region 【任务步骤记录】
                                        _M.LogForFerryMove(trans, trans.take_ferry_id, trans.finish_track_id, res);
                                        #endregion
                                        return;
                                    }

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.finish_track_id);
                                    #endregion

                                    // 后退至点
                                    //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                    //{
                                    //    Order = DevCarrierOrderE.定位指令,
                                    //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                    //    ToRFID = PubMaster.Track.GetTrackRFID2(trans.finish_track_id),
                                    //    ToTrackId = trans.finish_track_id
                                    //});
                                    MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                }
                            }
                        }
                    }
                    break;
                    #endregion
            }
        }

        #region[其他方法]

        public override void MovingCarrier(StockTrans trans)
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
