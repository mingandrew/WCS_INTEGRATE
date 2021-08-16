using enums;
using enums.track;
using enums.warning;
using module.goods;
using resource;
using System.Collections.Generic;
using task.device;
using tool.appconfig;

namespace task.trans.transtask
{
    /// <summary>
    /// 新出库逻辑
    /// 分配运输车逻辑：<br/>
    /// 【a.查找砖机轨道上是否有运输车】<br/>
    /// 【b.取货轨道是否有车，出入库轨道上砖侧】<br/>
    /// 【c.查找其他砖机轨道有没有运输车】<br/>
    /// 【d.查找其他轨道】<br/>
    /// 完成任务： 在上砖轨道放砖后结束<br/>
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

            //是否有空闲的摆渡车
            if (!PubTask.Ferry.HaveFreeFerryInTrans(trans, DeviceTypeE.前摆渡, out List<uint> ferryids))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1601, string.Format("当前没有空闲的摆渡车"));
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
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            //锁定摆渡车
            if (trans.take_ferry_id != 0
                && !trans.IsReleaseTakeFerry
                && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
            {
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            #region[分配摆渡车]

            //分配摆渡车的情况
            //开始分配
            //  1.未在取砖轨道
            //  2.在取砖轨道取到砖
            //释放分配
            //  1.在取砖轨道取砖(位置小于取砖轨道定位点)【取砖流程】
            //  2.在砖机轨道停止位停止【放砖流程】

            //无取砖摆渡车
            //不在取砖轨道
            //在取砖轨道，但无货并且无取砖任务
            if (trans.take_ferry_id == 0
                && (track.id != trans.take_track_id
                        || (isnotload && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.取砖指令))))
            {
                if (!trans.HaveTakeFerry)
                {
                    AllocateTakeFerry(trans, DeviceTypeE.前摆渡, track, out isfalsereturn);
                    if (isfalsereturn) return;
                }
            }

            //在取砖轨道取到货
            if (track.id == trans.take_track_id && isload)
            {
                if (!trans.HaveGiveFerry)
                {
                    AllocateGiveFerry(trans, DeviceTypeE.前摆渡, track, out isfalsereturn);
                }
            }

            #endregion

            switch (track.Type)
            {
                #region[小车在储砖轨道]

                #region[储砖_出]
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:
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
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                            return;
                        }

                        if (isnotload && isftask)
                        {
                            if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                            {
                                _M.SetStatus(trans, TransStatusE.完成, string.Format("轨道不满足状态[ {0} ]", PubMaster.Track.GetTrackStatusLogInfo(trans.take_track_id)));
                                return;
                            }

                            //判断是否需要在库存在上砖分割点后，是否需要接力
                            if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.take_track_id, trans.goods_id))
                            {
                                _M.SetTakeFerry(trans, 0, "清空分配的信息");
                                _M.SetCarrier(trans, 0, "清空分配的信息");
                                _M.SetStatus(trans, TransStatusE.调度设备, "需要先接力，解锁所有设备");
                                return;
                            }

                            if (!PubMaster.Track.IsTrackEmtpy(trans.take_track_id))
                            {
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
                    }
                    else //在非取货轨道
                    {
                        if (isload)
                        {
                            if (isftask)
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierGiving(trans);
                                #endregion

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

                    takeTrack = PubMaster.Track.GetTrack(trans.take_track_id);

                    if (tileemptyneed)
                    {
                        if (isnotload && isftask)
                        {
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                            {
                                bool needtoemtpytrack = false;
                                if (takeTrack.Type == TrackTypeE.储砖_出入)
                                {
                                    if (PubTask.Carrier.HaveInTrackAndGet(trans.take_track_id, out uint carrierid))
                                    {
                                        //停在出入储砖轨道，上砖侧的空闲运输车
                                        if (PubTask.Carrier.IsCarrierInTrackBiggerSite(carrierid, trans.take_track_id, takeTrack.rfid_2))
                                        {
                                            needtoemtpytrack = true;
                                        }

                                        //停在出入储砖轨道，下砖侧的空闲运输车
                                        if (PubTask.Carrier.IsCarrierInTrackSmallerSite(carrierid, trans.take_track_id, takeTrack.rfid_1)
                                            && !_M.HaveInCarrier(carrierid))
                                        {
                                            CheckTrackAndAddMoveTask(trans, trans.take_track_id);
                                            return;
                                        }
                                    }
                                }

                                #region[取砖轨道有车则找空轨道放]

                                //1.不允许，则不可以有车
                                //2.允许，则不可以有非倒库车
                                if (_M.CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id, out result))
                                {
                                    // 优先移动到空轨道
                                    //List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);
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

                                #endregion

                                if (isftask)
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
                                        //if (takeTrack.Type == TrackTypeE.储砖_出入)
                                        //{
                                        //    if (CheckTrackAndAddMoveTask(trans, trans.take_track_id, DeviceTypeE.后摆渡))
                                        //    {
                                        //        #region 【任务步骤记录】
                                        //        _M.SetStepLog(trans, false, 1401, string.Format("取砖轨道[ {0} ]内有其他运输车，等待轨道中的运输车转移；", takeTrack.name));
                                        //        #endregion
                                        //        return;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    if (CheckTrackAndAddMoveTask(trans, trans.take_track_id))
                                        //    {
                                        //        #region 【任务步骤记录】
                                        //        _M.SetStepLog(trans, false, 1401, string.Format("取砖轨道[ {0} ]内有其他运输车，等待轨道中的运输车转移；", takeTrack.name));
                                        //        #endregion
                                        //        return;
                                        //    }
                                        //}

                                        //判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
                                        if (_M.CheckTopStockAndSendSortTask(trans.id, trans.carrier_id, trans.take_track_id, trans.goods_id))
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
                    }
                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isnotload)
                    {
                        if (trans.take_ferry_id != 0)
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
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            #region[分配摆渡车/锁定摆渡车]

            if (track.Type == TrackTypeE.储砖_出入 || track.Type == TrackTypeE.前置摆渡轨道)
            {
                if (trans.give_ferry_id == 0)
                {
                    AllocateGiveFerry(trans, DeviceTypeE.前摆渡, track, out isfalsereturn);
                    if (isfalsereturn) return;
                }
                else if (!PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id))
                {
                    return;
                }
            }

            #endregion

            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    //判断小车是否已上轨道，停在砖机工位地标，是则解锁摆渡车
                    if (GlobalWcsDataConfig.BigConifg.IsFreeUpFerry(trans.area_id, trans.line))
                    {
                        if (!trans.IsReleaseGiveFerry
                            && PubTask.Carrier.IsCarrierStockInTileSite(trans.carrier_id, trans.tilelifter_id, trans.give_track_id))
                        {
                            RealseGiveFerry(trans);
                            if (!trans.IsReleaseGiveFerry
                                  && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                                  && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                            {
                                trans.IsReleaseGiveFerry = true;
                                _M.FreeGiveFerry(trans);
                            }
                        }
                    }

                    if (isnotload)
                    {
                        //发送离开给上砖机
                        if (!trans.IsLeaveTileLifter
                            && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                        {
                            trans.IsLeaveTileLifter = true;
                        }

                        if (track.id == trans.give_track_id
                            && PubTask.Carrier.IsCarrierFinishUnLoad(trans.carrier_id))
                        {
                            if (!trans.IsReleaseGiveFerry
                                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                            {
                                trans.IsReleaseGiveFerry = true;
                                _M.FreeGiveFerry(trans);
                            }

                            _M.SetUnLoadTime(trans);

                            //上砖任务，在上砖轨道放砖后直接完成任务
                            _M.SetStatus(trans, TransStatusE.完成);
                            return;
                        }
                    }

                    if (isload)
                    {
                        if (track.id == trans.give_track_id)
                        {
                            //没有任务并且停止
                            if (isftask)
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
                    }
                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.前置摆渡轨道:
                    if (isload && isftask)
                    {
                        if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                        {
                            #region 没库存时就将轨道设为空砖

                            if (!PubMaster.Track.IsEmtpy(trans.take_track_id)
                                && PubMaster.Goods.IsTrackStockEmpty(trans.take_track_id))
                            {
                                PubMaster.Track.UpdateStockStatus(trans.take_track_id, TrackStockStatusE.空砖, "系统已无库存,自动调整轨道为空");
                                PubMaster.Goods.ClearTrackEmtpy(trans.take_track_id);
                                PubTask.TileLifter.ReseUpTileCurrentTake(trans.take_track_id);
                                PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trans.take_track_id, TrackLogE.空轨道, "无库存数据");

                                #region 【任务步骤记录】
                                _M.LogForTrackNull(trans, trans.take_track_id);
                                #endregion
                            }

                            #endregion

                            //摆渡车 定位去 卸货点
                            //小车到达摆渡车后短暂等待再开始定位
                            if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
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

                    break;
                #endregion

                #region[小车在储砖_出入]
                case TrackTypeE.储砖_出入:

                    //摆渡车接车，取到砖后不等完成指令-无缝上摆渡
                    if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                    {
                        #region 【任务步骤记录】
                        _M.LogForFerryMove(trans, trans.give_ferry_id, track.id, res);
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
                        _M.LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
                        #endregion

                        MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);
                        return;
                    }
                    break;
                #endregion

                #region[小车在储砖_出]

                case TrackTypeE.储砖_出:

                    //判断小车是否做了倒库接力任务，并生成任务且完成上砖任务
                    if (_M.CheckCarrierInSortTaskAndAddTask(trans, trans.carrier_id, trans.take_track_id))
                    {
                        _M.SetStatus(trans, TransStatusE.完成,
                            string.Format("小车【{0}】执行接力倒库，完成上砖任务", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        return;
                    }

                    if (!tileemptyneed
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
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
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                    && PubTask.Carrier.GetCurrentSite(trans.carrier_id) < track.rfid_2)
                                {
                                    #region 【任务步骤记录】
                                    _M.LogForCarrierToTrack(trans, track.id);
                                    #endregion

                                    //前进至点
                                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = track.ferry_down_code,
                                        ToRFID = track.rfid_2,
                                        ToTrackId = track.id
                                    });
                                }

                                return;
                            }

                            if ((PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                || PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, track.rfid_2))
                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                //前进至摆渡车
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    ToTrackId = ferryTraid
                                });
                                return;
                            }
                        }
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
            if (trans.carrier_id == 0 && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 10))
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
                _M.SetStatus(trans, TransStatusE.放砖流程, "砖机工位显示无货需求，恢复流程");
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
                            _M.SetStatus(trans, TransStatusE.放砖流程, "小车载货，恢复流程");
                        }
                    }

                    if (isnotload && isftask)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
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