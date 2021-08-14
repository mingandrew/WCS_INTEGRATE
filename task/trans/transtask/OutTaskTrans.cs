using enums;
using enums.track;
using enums.warning;
using module.goods;
using module.tiletrack;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using task.device;
using tool.appconfig;

namespace task.trans.transtask
{
    public class OutTaskTrans : BaseTaskTrans
    {
        /// <summary>
        /// 原上砖流程 - 已停用
        /// </summary>
        /// <param name="trans"></param>
        public OutTaskTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 1.分配摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            //取消任务
            if (trans.carrier_id == 0
                && !PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id)
                && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
            {
                _M.SetStatus(trans, TransStatusE.完成, "砖机非无货需求");
                return;
            }

            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (_M.HaveTaskInTrackButSort(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1001, string.Format("存在相同作业轨道的任务，等待任务完成；"));
                #endregion
                return;
            }

            //存在接力倒库/倒库任务
            //1.库存剩最后一车的任务
            if (!_M.CheckTrackStockStillCanUse(trans, 0, trans.take_track_id, trans.stock_id))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1501, string.Format("等待接力库存条件；"));
                #endregion
                return;
            }

            if (!_M.IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1101, string.Format("当前区域线内分配运输车数已达上限，等待空闲；"));
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
            tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            switch (track.Type)
            {
                #region[小车在储砖轨道]

                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:

                    //判断小车是否做了倒库接力任务，并生成任务且完成上砖任务
                    if (_M.CheckCarrierInSortTaskAndAddTask(trans, trans.carrier_id, trans.take_track_id))
                    {
                        _M.SetStatus(trans, TransStatusE.完成,
                            string.Format("小车【{0}】执行接力倒库，完成上砖任务", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        return;
                    }

                    if (!tileemptyneed
                        && isftask
                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                    {
                        _M.SetStatus(trans, TransStatusE.完成, "工位非无货需求，直接完成任务");
                        return;
                    }

                    if (trans.take_track_id == track.id)
                    {
                        if (isload)
                        {
                            _M.SetLoadTime(trans);
                            //摆渡车接车，取到砖后不等完成指令-无缝上摆渡
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                #endregion

                                // 摆渡车不到位则到出库轨道头等待
                                if (isftask
                                    && PubTask.Carrier.GetCurrentPoint(trans.carrier_id) < (track.limit_point_up - 10))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, track.id);
                                    #endregion

                                    MoveToPos(track.id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                }

                                return;
                            }

                            if ((isftask
                                || PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, 0, track.limit_point_up))
                                && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                                return;
                            }
                        }

                        if (isnotload && isftask)
                        {
                            // 取砖轨道改为优先清空轨道
                            uint take = PubTask.TileLifter.GetTileCurrentTake(trans.tilelifter_id);
                            if (take != 0 && take != trans.take_track_id)
                            {
                                //直接完成
                                _M.SetStatus(trans, TransStatusE.完成, string.Format("存在设置好的优先清空轨道[ {0} ]", PubMaster.Track.GetTrackName(take)));
                                return;
                            }

                            if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                            {
                                _M.SetStatus(trans, TransStatusE.完成, string.Format("轨道不满足状态[ {0} ]", PubMaster.Track.GetTrackStatusLogInfo(trans.take_track_id)));
                                return;
                            }

                            //取砖失败，报警且不能
                            if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.取砖指令))
                            {
                                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.GetStockButNull, (ushort)trans.carrier_id, trans.id);
                                #region 【任务步骤记录】
                                _M.LogForCarrierGetStockFalse(trans);
                                #endregion
                                return;
                            }
                            PubMaster.Warn.RemoveTaskWarn(WarningTypeE.GetStockButNull, trans.id);

                            // 轨道内直接取砖
                            TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out res);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, trans.take_track_id, res);
                            #endregion
                            return;

                        }
                    }
                    else //在非取货轨道
                    {
                        if (isload && isftask)
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierGiving(trans);
                            #endregion

                            //下降放货
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.放砖指令
                            });
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

                            if (isftask
                                && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                                return;
                            }
                        }
                    }
                    break;

                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:
                    //取消任务
                    if (!tileemptyneed
                        && PubTask.Carrier.IsCarrierFree(trans.carrier_id)
                        && mTimer.IsTimeOutAndReset(TimerTag.TileNeedCancel, trans.id, 20))
                    {
                        if (isload)
                        {
                            if (!_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id, out result)
                            && !PubMaster.Goods.IsTrackHaveStockInTopPosition(trans.take_track_id)
                            && !PubTask.Carrier.HaveCarrierMoveTopInTrackUpTop(trans.carrier_id, trans.take_track_id))
                            {
                                // 优先回原轨道
                                _M.SetStatus(trans, TransStatusE.取消, "工位非无货需求，取消任务，返回原轨道");
                                return;
                            }

                        }

                        // 再者到空轨道
                        List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                        List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                        foreach (uint t in tids)
                        {
                            if (!_M.IsTraInTrans(t)
                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                            {
                                // 有货的话就只能找空轨道
                                if (isload && !PubMaster.Track.IsEmtyp4Up(t))
                                {
                                    continue;
                                }

                                if (_M.SetTakeSite(trans, t))
                                {
                                    _M.SetStatus(trans, TransStatusE.取消, "工位非无货需求，取消任务，寻找空轨道");

                                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileEmptyNeedAndNoBack, (ushort)trans.carrier_id);
                                    return;
                                }
                            }
                        }

                        PubMaster.Warn.AddDevWarn(trans.area_id, trans.line, WarningTypeE.UpTileEmptyNeedAndNoBack, (ushort)trans.carrier_id, trans.id);

                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1201, string.Format("砖机工位非无货需求，且运输车[ {0} ]无合适轨道可以回轨；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        #endregion

                    }

                    if (tileemptyneed)
                    {
                        if (isload && isftask)
                        {
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                            {
                                //摆渡车 定位去 卸货点
                                //小车到达摆渡车后短暂等待再开始定位
                                if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                                    #endregion
                                    return;
                                }

                                /**
                                 * 1.判断砖机是否是单个砖机
                                 * 2.如果兄弟里面的砖机有需求放砖，则优先放入里面的工位
                                 */
                                if (PubMaster.DevConfig.IsBrother(trans.tilelifter_id)
                                    && PubTask.TileLifter.IsInSideTileNeed(trans.tilelifter_id, trans.give_track_id))
                                {
                                    uint bro = PubMaster.DevConfig.GetBrotherIdInside(trans.tilelifter_id);
                                    _M.SetTile(trans, bro, string.Format("砖机[ {0} & {1} ]有需求,优先放里面", bro, PubMaster.Device.GetDeviceName(bro)));
                                    return;
                                }
                                else
                                {
                                    if (!PubTask.TileLifter.IsGiveReady(trans.tilelifter_id, trans.give_track_id, out res))
                                    {
                                        #region 【任务步骤记录】
                                        _M.SetStepLog(trans, false, 1301, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足放砖条件；{2}；",
                                            PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                            PubMaster.Track.GetTrackName(trans.give_track_id), res), true);
                                        #endregion
                                        return;
                                    }

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierGive(trans, trans.give_track_id);
                                    #endregion

                                    //获取砖机配置的取货点
                                    ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                    if (torfid == 0)
                                    {
                                        torfid = PubMaster.Track.GetTrackLimitPointIn(trans.give_track_id);
                                    }

                                    MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, torfid);
                                    return;
                                }
                            }
                        }

                        if (isnotload && isftask)
                        {
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                            {
                                //1.不允许，则不可以有车
                                //2.允许，则不可以有非倒库车
                                if (_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id, out result))
                                {
                                    // 优先移动到空轨道
                                    List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);

                                    List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                    foreach (uint t in tids)
                                    {
                                        if (!_M.IsTraInTrans(t)
                                            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                            && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                        {
                                            if (_M.SetTakeSite(trans, t))
                                            {
                                                _M.SetStatus(trans, TransStatusE.取消, "轨道内有其他运输车");
                                            }

                                            return;
                                        }
                                    }

                                    #region 【任务步骤记录】
                                    _M.SetStepLog(trans, false, 1401, string.Format("取砖轨道[ {0} ]内有其他运输车，且运输车[ {1} ]无合适轨道可以回轨；",
                                        PubMaster.Track.GetTrackName(trans.take_track_id),
                                        PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                    #endregion

                                    return;
                                }

                                //摆渡车 定位去 取货点
                                //小车到达摆渡车后短暂等待再开始定位
                                if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                    #endregion
                                    return;
                                }

                                if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                    || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.take_track_id);
                                    #endregion

                                    MoveToPos(trans.take_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                    return;
                                }
                                else
                                {
                                    //判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
                                    if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.take_track_id))
                                    {
                                        #region 【任务步骤记录】
                                        _M.LogForCarrierSortRelay(trans, trans.take_track_id);
                                        #endregion
                                        return;
                                    }

                                    if (!_M.CheckStockIsableToTake(trans, trans.carrier_id, trans.take_track_id, trans.stock_id))
                                    {
                                        #region 【任务步骤记录】
                                        _M.LogForCarrierNoTake(trans, trans.take_track_id);
                                        #endregion
                                        return;
                                    }

                                    // 获取头部库存
                                    Stock takeStock = PubMaster.Goods.GetStockForOut(trans.take_track_id);
                                    if (takeStock == null || takeStock.goods_id != trans.goods_id)
                                    {
                                        _M.SetStatus(trans, TransStatusE.取消, string.Format("[{0}]内的头部库存与任务所需不符", PubMaster.Track.GetTrackName(trans.take_track_id)));
                                        return;
                                    }
                                    _M.SetStock(trans, takeStock.id);

                                    // 直接取砖
                                    TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out res);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierTake(trans, trans.take_track_id, res);
                                    #endregion
                                    return;
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:

                    //判断小车是否已上轨道，是则解锁摆渡车
                    if (GlobalWcsDataConfig.BigConifg.IsFreeUpFerry(trans.area_id, trans.line))
                    {
                        if (!trans.IsReleaseTakeFerry
                            && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                            && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
                        {
                            trans.IsReleaseTakeFerry = true;
                            _M.FreeTakeFerry(trans);
                        }
                    }

                    if (isnotload)
                    {
                        if (track.id == trans.give_track_id)
                        {
                            _M.SetUnLoadTime(trans);

                            _M.SetStatus(trans, TransStatusE.还车回轨);
                            PubMaster.DevConfig.SubTileNowGoodQty(trans.tilelifter_id, trans.goods_id);
                            return;
                        }

                        //摆渡车去接运输车
                        if (track.id != trans.give_track_id && trans.take_ferry_id != 0)
                        {
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                #endregion
                                return;
                            }

                            if (isftask)
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                                return;
                            }

                        }
                    }

                    if (isload && isftask)
                    {
                        if (track.id == trans.give_track_id)
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierGive(trans, trans.give_track_id);
                            #endregion

                            //获取砖机配置的取货点
                            ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                            if (torfid == 0)
                            {
                                torfid = PubMaster.Track.GetTrackLimitPointIn(trans.give_track_id);
                            }

                            MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, torfid);
                        }
                    }
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

            #region[分配摆渡车/锁定摆渡车]

            if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
            {
                if (trans.give_ferry_id == 0)
                {
                    string msg = _M.AllocateFerry(trans, DeviceTypeE.前摆渡, track, true);

                    #region 【任务步骤记录】
                    if (_M.LogForGiveFerry(trans, msg)) return;
                    #endregion
                }
                else if (!PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id))
                {
                    return;
                }
            }

            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

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

                        if (trans.give_ferry_id != 0)
                        {
                            //摆渡车接车，取到砖后不等完成指令-无缝上摆渡
                            if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.give_ferry_id, track.id, res);
                                #endregion

                                // 摆渡车不到位则到轨道头等待
                                if (isftask
                                    && PubTask.Carrier.GetCurrentPoint(trans.carrier_id) < (track.limit_point_up - 10))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, track.id);
                                    #endregion

                                    MoveToPos(track.id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                }

                                return;
                            }

                            if ((isftask
                                || PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, 0, track.limit_point_up)))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
                                #endregion

                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                                return;
                            }

                        }
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
                                //只要轨道没有空都去轨道取，直到空轨道
                                if (!PubMaster.Track.IsEmtpy(trans.take_track_id)
                                    && !PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType)
                                    && !_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id, out result)
                                    && _M.CheckTrackStockStillCanUse(trans, trans.carrier_id, trans.take_track_id))
                                {
                                    _M.SetFinishSite(trans, trans.take_track_id, "还车轨道分配轨道[1]");
                                }
                                else
                                {
                                    bool isallocate = false;
                                    DevWorkTypeE type = PubMaster.DevConfig.GetTileWorkType(trans.tilelifter_id);
                                    switch (type)
                                    {
                                        case DevWorkTypeE.品种作业:
                                            // 1.查看当前作业轨道是否能作业
                                            if (PubMaster.Track.HaveTrackInGoodFrist(trans.area_id, trans.tilelifter_id,
                                                trans.goods_id, PubTask.TileLifter.GetTileCurrentTake(trans.tilelifter_id), out uint trackid)
                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid)
                                                && !_M.HaveInTrackButSortTask(trackid)
                                                && !_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trackid, out string _)
                                                && _M.CheckTrackStockStillCanUse(trans, trans.carrier_id, trackid)
                                                )
                                            {
                                                _M.SetFinishSite(trans, trackid, "还车轨道分配轨道[2]");
                                                isallocate = true;
                                            }

                                            #region  2.查看是否存在未空砖但无库存的轨道 - 停用，无库存一定空轨
                                            //else if (PubMaster.Track.HaveTrackInGoodButNotStock(trans.area_id, trans.tilelifter_id,
                                            //    trans.goods_id, out List<uint> trackids))
                                            //{
                                            //    foreach (var tid in trackids)
                                            //    {
                                            //        if (!HaveInTrackButSortTask(tid)
                                            //            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid)
                                            //            && !_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trackid, out string _)
                                            //            && CheckTrackStockStillCanUse(trans.carrier_id, trackid))
                                            //        {
                                            //            _M.SetFinishSite(trans, tid, "还车轨道分配轨道[3]");
                                            //            isallocate = true;
                                            //            break;
                                            //        }
                                            //    }
                                            //}
                                            #endregion

                                            // 3.分配库存
                                            else if (!isallocate && PubMaster.Goods.GetStock(trans.area_id, trans.line, trans.tilelifter_id,
                                                trans.goods_id, out List<Stock> allocatestocks))
                                            {
                                                foreach (Stock stock in allocatestocks)
                                                {
                                                    if (!_M.HaveInTrackButSortTask(stock.track_id)
                                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, stock.track_id)
                                                        && !_M.CheckHaveCarrierInOutTrack(trans.carrier_id, stock.track_id, out string _)
                                                        && _M.CheckTrackStockStillCanUse(trans, trans.carrier_id, stock.track_id))
                                                    {
                                                        _M.SetFinishSite(trans, stock.track_id, "还车轨道分配轨道[4]");
                                                        isallocate = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!isallocate)
                                            {
                                                // 优先移动到空轨道
                                                List<uint> emptytras = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                                                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(emptytras, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                                foreach (uint t in tids)
                                                {
                                                    if (!_M.IsTraInTrans(t)
                                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, t)
                                                        && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                                    {
                                                        _M.SetFinishSite(trans, t, "还车轨道分配轨道[5]");
                                                        isallocate = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            break;

                                        case DevWorkTypeE.轨道作业:
                                            List<TileTrack> tracks = PubMaster.TileTrack.GetTileTrack2Out(trans.tilelifter_id);
                                            foreach (TileTrack tt in tracks)
                                            {
                                                Track w_track = PubMaster.Track.GetTrack(tt.track_id);
                                                if (track.StockStatus == TrackStockStatusE.空砖 ||
                                                    (track.TrackStatus != TrackStatusE.启用 && track.TrackStatus != TrackStatusE.仅上砖))
                                                {
                                                    PubMaster.TileTrack.DeleteTileTrack(tt);
                                                    continue;
                                                }

                                                _M.SetFinishSite(trans, w_track.id, "还车轨道分配轨道[6]");
                                                isallocate = true;
                                                break;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (!isallocate)
                                    {
                                        _M.SetFinishSite(trans, trans.take_track_id, "还车轨道分配轨道[7]");
                                    }
                                }
                            }

                            if (trans.finish_track_id != 0)
                            {
                                if (isftask)
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

                                    if ((!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                        && !_M.CheckTrackStockStillCanUse(trans, trans.carrier_id, trans.finish_track_id))
                                        || _M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.finish_track_id, out result))
                                    {
                                        _M.SetFinishSite(trans, 0, "轨道不满足状态，重新分配");
                                        return;
                                    }

                                    if (!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                        && !PubMaster.Track.IsStopUsing(trans.finish_track_id, trans.TransType))
                                    {
                                        PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                        PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                        //判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
                                        if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.finish_track_id))
                                        {
                                            #region 【任务步骤记录】
                                            _M.LogForCarrierSortRelay(trans, trans.finish_track_id);
                                            #endregion
                                            return;
                                        }

                                        //判断是否能进去取货
                                        if (!_M.CheckStockIsableToTake(trans, trans.carrier_id, trans.finish_track_id))
                                        {
                                            #region 【任务步骤记录】
                                            _M.LogForCarrierNoTake(trans, trans.finish_track_id);
                                            #endregion
                                            return;
                                        }

                                        if ((PubMaster.Track.GetAndRefreshUpCount(trans.finish_track_id) == 0
                                                && !GlobalWcsDataConfig.BigConifg.IsNotNeedSortToSplitUpPlace(trans.area_id, trans.line))
                                                || !PubMaster.Goods.IsTopStockIsGood(trans.finish_track_id, trans.goods_id))
                                        {
                                            #region 【任务步骤记录】
                                            _M.LogForCarrierToTrack(trans, trans.finish_track_id);
                                            #endregion

                                            MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                        }
                                        else
                                        {
                                            #region 库存判断
                                            // 获取头部库存
                                            Stock takeStock = PubMaster.Goods.GetStockForOut(trans.finish_track_id);
                                            if (takeStock == null || takeStock.goods_id != trans.goods_id)
                                            {
                                                _M.SetStatus(trans, TransStatusE.取消, string.Format("[{0}]内的头部库存与任务所需不符", PubMaster.Track.GetTrackName(trans.finish_track_id)));
                                                return;
                                            }
                                            _M.SetStock(trans, takeStock.id);

                                            #endregion

                                            // 直接取砖
                                            TakeInTarck(trans.stock_id, trans.finish_track_id, trans.carrier_id, trans.id, out res);

                                            #region 【任务步骤记录】
                                            _M.LogForCarrierTake(trans, trans.finish_track_id, res);
                                            #endregion
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        #region 【任务步骤记录】
                                        _M.LogForCarrierToTrack(trans, trans.finish_track_id);
                                        #endregion

                                        MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                    }
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

                    //判断小车是否做了倒库接力任务，并生成任务且完成上砖任务
                    if (_M.CheckCarrierInSortTaskAndAddTask(trans, trans.carrier_id, trans.take_track_id))
                    {
                        _M.SetStatus(trans, TransStatusE.完成,
                            string.Format("小车【{0}】执行接力倒库，完成上砖任务", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        return;
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
            tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            //有需求，取货了，回去取砖流程
            if (!PubTask.TileLifter.IsTileCutover(trans.tilelifter_id)
                && isload
                && tileemptyneed
                && isftask
                && mTimer.IsOver(TimerTag.UpTileReStoreEmtpyNeed, trans.give_track_id, 5, 5))
            {
                _M.SetStatus(trans, TransStatusE.取砖流程, "砖机工位显示无货需求，恢复流程");
                return;
            }

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
                    if (isftask)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:
                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                    {
                        if (isftask)
                        {
                            //小车回到原轨道
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            #region 【任务步骤记录】
                            _M.LogForCarrierToTrack(trans, trans.take_track_id);
                            #endregion

                            MoveToPos(trans.take_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                            return;
                        }
                    }
                    break;
                #endregion

                #region[小车在下砖轨道]
                case TrackTypeE.上砖轨道:

                    if (isload)
                    {
                        if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.取砖流程, "小车载货，恢复流程");
                        }
                    }

                    if (isnotload)
                    {
                        //小车回到原轨道
                        if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                            #endregion
                            return;
                        }

                        if (isftask)
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                            #endregion

                            MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                            return;
                        }
                    }

                    break;
                    #endregion
            }
        }

        #region[其他方法]
        public override void CheckingTrack(StockTrans trans)
        {

        }


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

        public override void ToGiveTrackGiveStock(StockTrans trans)
        {

        }
        public override void Organizing(StockTrans trans)
        {
        }
        #endregion
    }
}
