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
    /// <summary>
    /// 出库任务
    /// 上砖-出库-流程（code- XX01）
    /// </summary>
    public class OutTaskTransV2 : BaseTaskTrans
    {
        public OutTaskTransV2(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 调度设备
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
                _M.SetStepLog(trans, false, 1101, string.Format("等待接力库存条件；"));
                #endregion
                return;
            }

            if (!_M.IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1201, string.Format("当前区域线内分配运输车数已达上限，等待空闲；"));
                #endregion
                return;
            }

            //是否有空闲的摆渡车
            if (!PubTask.Ferry.HaveFreeFerryInTrans(trans, DeviceTypeE.前摆渡, out List<uint> ferryids))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1301, string.Format("当前没有空闲的摆渡车"));
                #endregion
                return;
            }

            //分配运输车
            if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result, ferryids)
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
            bool isTileEmptyNeed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

            switch (track.Type)
            {
                #region[小车在轨道]
                case TrackTypeE.上砖轨道:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:
                    if (!isTileEmptyNeed
                        && isStopNoOrder
                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                    {
                        _M.SetStatus(trans, TransStatusE.完成, "工位非无货需求，直接完成任务");
                        return;
                    }

                    if (isLoad)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                            return;
                        }
                        else
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
                            _M.LogForCarrierGiving(trans);
                            #endregion
                            return;
                        }
                    }

                    if (isNotLoad)
                    {
                        if (trans.take_track_id == track.id)
                        {
                            #region [取砖判断]
                            //判断是否需要在库存在上砖分割点后，是否需要接力
                            if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.take_track_id, trans.goods_id, trans.level))
                            {
                                _M.SetStatus(trans, TransStatusE.调度设备, "需要先接力，解锁所有设备");
                                return;
                            }

                            // 不满足取砖状态
                            if (isStopNoOrder
                                && (PubMaster.Track.IsEmtpy(trans.take_track_id) 
                                    || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType)))
                            {
                                // 解锁摆渡车
                                RealseTakeFerry(trans);
                                _M.SetStatus(trans, TransStatusE.完成, string.Format("轨道不满足状态[ {0} ]", PubMaster.Track.GetTrackStatusLogInfo(trans.take_track_id)));
                                return;
                            }

                            // 重发指令
                            if (isStopNoOrder)
                            {
                                // 获取头部库存判断
                                Stock takeStock = PubMaster.Goods.GetStockForOut(trans.take_track_id);
                                if (takeStock == null || takeStock.goods_id != trans.goods_id)
                                {
                                    _M.SetStatus(trans, TransStatusE.取消, string.Format("[{0}]内的头部库存与任务所需不符", PubMaster.Track.GetTrackName(trans.take_track_id)));
                                    return;
                                }
                                _M.SetStock(trans, takeStock.id);

                                // 轨道内直接取砖
                                TakeInTarck(trans.stock_id, trans.take_track_id, trans.carrier_id, trans.id, out string res);

                                #region 【任务步骤记录】
                                _M.LogForCarrierTake(trans, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            #endregion
                            return;
                        }
                        else
                        {
                            //至摆渡车
                            MoveToFerrySeamless(trans, true);
                            return;
                        }
                    }

                    break;
                #endregion

                #region[小车在摆渡]
                case TrackTypeE.前置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, trans.AllocateFerryType, track)) return;

                    // 取消任务
                    if (!isTileEmptyNeed
                        && isStopNoOrder
                        && mTimer.IsTimeOutAndReset(TimerTag.TileNeedCancel, trans.id, 20))
                    {
                        // 优先回原轨道
                        if (isLoad)
                        {
                            if (!_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id, out result)
                            && !PubMaster.Goods.IsTrackHaveStockInTopPosition(trans.take_track_id)
                            && !PubTask.Carrier.HaveCarrierMoveTopInTrackUpTop(trans.carrier_id, trans.take_track_id))
                            {
                                _M.SetStatus(trans, TransStatusE.取消, "工位非无货需求，取消任务，返回原轨道");
                                return;
                            }

                        }

                        // 再者到空轨道
                        List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                        List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id);
                        foreach (uint t in tids)
                        {
                            if (!_M.IsTraInTrans(t)
                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                            {
                                // 有货的话就只能找空轨道
                                if (isLoad && !PubMaster.Track.IsEmtyp4Up(t))
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
                        _M.SetStepLog(trans, false, 1401, string.Format("砖机工位非无货需求，且运输车[ {0} ]无可回轨轨道，等待砖机工位重新需求或有合适轨道可回轨；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        #endregion
                        return;
                    }

                    if (isTileEmptyNeed)
                    {
                        if (isLoad)
                        {
                            if (PubTask.TileLifter.IsTrackEmtpy(trans.tilelifter_id, trans.give_track_id)
                                || mTimer.IsOver(TimerTag.CarrierLoadNotInTileTrack, trans.give_track_id, 5, 5))
                            {
                                _M.SetLoadTime(trans);
                                _M.SetStatus(trans, TransStatusE.放砖流程);
                                return;
                            }
                        }

                        if (isNotLoad && isStopNoOrder)
                        {
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                            {
                                Track takeTrack = PubMaster.Track.GetTrack(trans.take_track_id);

                                #region[取砖轨道有车则找空轨道放]

                                //1.不允许，则不可以有车
                                //2.允许，则不可以有非倒库车
                                if (_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id, out result))
                                {
                                    // 优先移动到空轨道
                                    List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                                    List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id);
                                    foreach (uint t in tids)
                                    {
                                        if (!_M.IsTraInTrans(t)
                                            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                            && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                        {
                                            if (_M.SetTakeSite(trans, t))
                                            {
                                                _M.SetStatus(trans, TransStatusE.取消, "轨道内有其他运输车"+ result);
                                            }

                                            return;
                                        }
                                    }

                                    #region 【任务步骤记录】
                                    _M.SetStepLog(trans, false, 1501, string.Format("取砖轨道[ {0} ]内有其他运输车，且运输车[ {1} ]无合适轨道可以回轨；",
                                        PubMaster.Track.GetTrackName(trans.take_track_id),
                                        PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                    #endregion

                                    return;
                                }

                                #endregion

                                //摆渡车 定位去 取货点
                                //小车到达摆渡车后短暂等待再开始定位
                                if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out uint ferryTraid, out string res))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                    #endregion
                                    return;
                                }

                                // 不满足取砖条件 - 至点停
                                if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                    || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                {
                                    MoveToPos(trans.take_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.take_track_id);
                                    #endregion
                                    return;
                                }

                                #region [取砖判断]
                                // 判断是否需要在库存在上砖分割点后,，待车入轨道再生成倒库任务
                                if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.take_track_id, trans.goods_id, trans.level, false))
                                {
                                    MoveToLoc(trans.take_track_id, trans.carrier_id, trans.id, takeTrack.up_split_point);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.take_track_id, "移至接力点，准备进行接力倒库");
                                    #endregion
                                    return;
                                }

                                // 判断是否能执行取砖指令
                                if (!_M.CheckStockIsableToTake(trans, trans.carrier_id, trans.take_track_id, trans.stock_id))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierNoTake(trans, trans.take_track_id);
                                    #endregion
                                    return;
                                }

                                // 获取头部库存判断
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

                                #endregion
                            }

                        }
                    }
                    break;
                #endregion

            }
        }

        /// <summary>
        /// 卸货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToGiveTrackGiveStock(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);
            bool isTileEmptyNeed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:

                    #region [判断是否还车回轨]
                    // 更新剩余上砖数量
                    if (isNotLoad && track.id == trans.give_track_id && trans.unload_time == null)
                    {
                        _M.SetUnLoadTime(trans);
                        PubMaster.DevConfig.SubTileNowGoodQty(trans.tilelifter_id, trans.goods_id);
                    }

                    bool isback = false;
                    // 1.剩余上砖数量 = 0 则回轨
                    if (!isback && !PubMaster.DevConfig.IsTileNowGoodQtyOk(trans.tilelifter_id, trans.goods_id))
                    {
                        isback = true;
                    }

                    // 2.当前所有轨道出库头无可用库存
                    if (!isback)
                    {
                        bool HaveStk = PubMaster.Goods.GetStock(trans.area_id, trans.line, trans.tilelifter_id, trans.goods_id, out List<Stock> allocatestocks);
                        if (HaveStk)
                        {
                            // 有但无法使用
                            foreach (Stock stock in allocatestocks)
                            {
                                //判断是否轨道、库存是否已经有任务占用[忽略倒库任务]
                                if (PubTask.Trans.IsStockInTransButSortTask(stock.id, stock.track_id, TransTypeE.库存整理))
                                {
                                    isback = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 无库存
                            isback = true;
                        }
                    }

                    // 3.配置还车回轨
                    if (!isback && GlobalWcsDataConfig.BigConifg.IsReturnDevBackToTrack(trans.area_id, trans.line))
                    {
                        isback = true;
                    }
                    #endregion

                    //判断小车是否已上轨道，停在砖机工位地标，是则解锁摆渡车
                    if (!isback && GlobalWcsDataConfig.BigConifg.IsFreeUpFerry(trans.area_id, trans.line))
                    {
                        if (track.id == trans.give_track_id)
                        {
                            // 解锁摆渡
                            RealseGiveFerry(trans);
                        }
                    }

                    if (isLoad)
                    {
                        if (track.id == trans.give_track_id)
                        {
                            // 重发指令
                            if (isStopNoOrder)
                            {
                                //获取砖机配置的取货点
                                ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                if (torfid == 0)
                                {
                                    torfid = PubMaster.Track.GetTrackLimitPointIn(trans.give_track_id);
                                }

                                MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, torfid);

                                #region 【任务步骤记录】
                                _M.LogForCarrierGive(trans, trans.give_track_id);
                                #endregion
                                return;
                            }
                        }
                        else
                        {
                            // 至摆渡
                            MoveToFerrySeamless(trans, false);
                            return;
                        }
                    }

                    if (isNotLoad)
                    {
                        //发送离开给上砖机
                        if (!trans.IsLeaveTileLifter
                            && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                        {
                            trans.IsLeaveTileLifter = true;
                        }

                        // 判断是否执行回轨流程
                        if (isback)
                        {
                            _M.SetStatus(trans, TransStatusE.还车回轨);
                            return;
                        }

                        // 解锁摆渡
                        RealseGiveFerry(trans);

                        // 上砖任务，在上砖轨道放砖后直接完成任务
                        _M.SetStatus(trans, TransStatusE.完成);
                        return;
                    }

                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateGiveFerry(trans, trans.AllocateFerryType, track)) return;

                    if (isLoad && isStopNoOrder)
                    {
                        if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                        {
                            //摆渡车 定位去 卸货点
                            //小车到达摆渡车后短暂等待再开始定位
                            if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out uint ferryTraid, out string res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                                #endregion
                                return;
                            }

                            #region [放砖判断]
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
                                // 不满足放砖条件
                                if (!PubTask.TileLifter.IsGiveReady(trans.tilelifter_id, trans.give_track_id, out res))
                                {
                                    #region 【任务步骤记录】
                                    _M.SetStepLog(trans, false, 1601, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足放砖条件；{2}；",
                                        PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                        PubMaster.Track.GetTrackName(trans.give_track_id), res), true);
                                    #endregion
                                    return;
                                }

                                //获取砖机配置点
                                ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                if (torfid == 0)
                                {
                                    torfid = PubMaster.Track.GetTrackLimitPointIn(trans.give_track_id);
                                }

                                MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, torfid);

                                #region 【任务步骤记录】
                                _M.LogForCarrierGive(trans, trans.give_track_id);
                                #endregion
                                return;
                            }

                            #endregion
                        }
                    }

                    break;
                #endregion

                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:
                    if (!isTileEmptyNeed
                        && isStopNoOrder
                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                    {
                        _M.SetStatus(trans, TransStatusE.完成, "工位非无货需求，直接完成任务");
                        return;
                    }

                    if (isLoad && trans.take_track_id == track.id)
                    {
                        //至摆渡车
                        MoveToFerrySeamless(trans, false);
                        return;
                    }
                    else
                    {
                        _M.SetStatus(trans, TransStatusE.取砖流程, "小车不在取砖轨道及有货状态，重新取砖");
                        return;
                    }

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
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isLoad)
                    {
                        if (isStopNoOrder)
                        {
                            // 原地放砖
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

                    if (isNotLoad)
                    {
                        //发送离开给上砖机
                        if (!trans.IsLeaveTileLifter
                            && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                        {
                            trans.IsLeaveTileLifter = true;
                        }

                        MoveToFerrySeamless(trans, false);
                        return;
                    }

                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateGiveFerry(trans, trans.AllocateFerryType, track)) return;

                    if (isNotLoad)
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

                            #region 分配结束轨道
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
                                                && !_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trackid, out result)
                                                && _M.CheckTrackStockStillCanUse(trans, trans.carrier_id, trackid)
                                                )
                                            {
                                                _M.SetFinishSite(trans, trackid, "还车轨道分配轨道[2]");
                                                isallocate = true;
                                            }

                                            // 2.分配库存
                                            if (!isallocate && PubMaster.Goods.GetStock(trans.area_id, trans.line, trans.tilelifter_id,
                                                trans.goods_id, out List<Stock> allocatestocks))
                                            {
                                                foreach (Stock stock in allocatestocks)
                                                {
                                                    if (!_M.HaveInTrackButSortTask(stock.track_id)
                                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, stock.track_id)
                                                        && !_M.CheckHaveCarrierInOutTrack(trans.carrier_id, stock.track_id, out string _)
                                                        && _M.CheckTrackStockStillCanUse(trans, trans.carrier_id, stock.track_id))
                                                    {
                                                        _M.SetFinishSite(trans, stock.track_id, "还车轨道分配轨道[3]");
                                                        isallocate = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!isallocate)
                                            {
                                                // 优先移动到空轨道
                                                List<uint> emptytras = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                                                List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(emptytras, trans.take_track_id);

                                                foreach (uint t in tids)
                                                {
                                                    if (!_M.IsTraInTrans(t)
                                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, t)
                                                        && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                                    {
                                                        _M.SetFinishSite(trans, t, "还车轨道分配轨道[4]");
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

                                                _M.SetFinishSite(trans, w_track.id, "还车轨道分配轨道[5]");
                                                isallocate = true;
                                                break;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (!isallocate)
                                    {
                                        _M.SetFinishSite(trans, trans.take_track_id, "还车轨道分配轨道[6]");
                                    }
                                }
                            }

                            #endregion

                            #region 定位结束轨道
                            if (trans.finish_track_id > 0 && isStopNoOrder)
                            {
                                //摆渡车 定位去 取货点继续取砖
                                //小车到达摆渡车后短暂等待再开始定位
                                if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out uint ferryTraid, out string res))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForFerryMove(trans, trans.give_ferry_id, trans.finish_track_id, res);
                                    #endregion
                                    return;
                                }

                                // 轨道不满足
                                if ((!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                    && !_M.CheckTrackStockStillCanUse(trans, trans.carrier_id, trans.finish_track_id))
                                    || _M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.finish_track_id, out result))
                                {
                                    _M.SetFinishSite(trans, 0, "轨道不满足状态，重新分配; " + result);
                                    return;
                                }

                                #region [取砖判断]
                                // 轨道状态不满足 - 至点停
                                if (PubMaster.Track.IsEmtpy(trans.finish_track_id) || PubMaster.Track.IsStopUsing(trans.finish_track_id, trans.TransType))
                                {
                                    MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.finish_track_id);
                                    #endregion
                                    return;
                                }

                                // 更新轨道信息
                                PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                // 判断是否需要在库存在上砖分割点后,，待车入轨道再生成倒库任务
                                if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.finish_track_id, trans.goods_id, trans.level, false))
                                {
                                    ushort toloc = PubMaster.Track.GetTrackUpSplitPoint(trans.finish_track_id);
                                    MoveToLoc(trans.finish_track_id, trans.carrier_id, trans.id, toloc);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.take_track_id, "移至接力点，准备进行接力倒库");
                                    #endregion
                                    return;
                                }

                                // 判断是否取砖
                                if (!_M.CheckStockIsableToTake(trans, trans.carrier_id, trans.finish_track_id))
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierNoTake(trans, trans.finish_track_id);
                                    #endregion
                                    return;
                                }

                                // 判断是否至点停
                                if ((PubMaster.Track.GetAndRefreshUpCount(trans.finish_track_id) == 0
                                        && !GlobalWcsDataConfig.BigConifg.IsNotNeedSortToSplitUpPlace(trans.area_id, trans.line))
                                        || !PubMaster.Goods.IsTopStockIsGood(trans.finish_track_id, trans.goods_id))
                                {
                                    MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, trans.finish_track_id);
                                    #endregion
                                    return;
                                }

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

                                #endregion
                            }

                            #endregion
                        }
                    }

                    break;
                #endregion

                #region[小车在取砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                    // 解锁摆渡车
                    RealseGiveFerry(trans);

                    if (trans.finish_track_id == track.id)
                    {
                        //判断是否需要在库存在上砖分割点后，是否需要接力
                        if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.finish_track_id, trans.goods_id, trans.level))
                        {
                            _M.SetStatus(trans, TransStatusE.完成,
                                string.Format("小车【{0}】执行接力倒库，完成上砖任务", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                            return;
                        }
                    }

                    _M.SetStatus(trans, TransStatusE.完成);
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
            _M.SetFinish(trans);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            if (trans.carrier_id == 0 && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 10))
            {
                _M.SetStatus(trans, TransStatusE.完成, "取消任务-结束");
                return;
            }

            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);
            bool isTileEmptyNeed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

            //有需求，取货了，回去取砖流程
            if (!PubTask.TileLifter.IsTileCutover(trans.tilelifter_id)
                && isLoad
                && isStopNoOrder
                && isTileEmptyNeed
                && mTimer.IsOver(TimerTag.UpTileReStoreEmtpyNeed, trans.give_track_id, 5, 5))
            {
                _M.SetStatus(trans, TransStatusE.放砖流程, "砖机工位显示无货需求，恢复流程");
                return;
            }

            switch (track.Type)
            {
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                    if (isStopNoOrder)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, trans.AllocateFerryType, track)) return;

                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                    {
                        if (isStopNoOrder)
                        {
                            //小车回到原轨道
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out uint ferryTraid, out string res))
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

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isLoad)
                    {
                        if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程, "小车载货，恢复流程");
                            return;
                        }
                    }

                    if (isNotLoad && isStopNoOrder)
                    {
                        // 判断是否执行回轨流程
                        if (GlobalWcsDataConfig.BigConifg.IsReturnDevBackToTrack(trans.area_id, trans.line))
                        {
                            //至摆渡车
                            MoveToFerrySeamless(trans, true);
                            return;
                        }

                        _M.SetStatus(trans, TransStatusE.完成);
                        return;
                    }

                    break;
                    #endregion
            }
        }

        #region[其他流程]
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
        public override void Organizing(StockTrans trans)
        {
        }
        #endregion
    }
}