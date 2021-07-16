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
    /// 同向下砖（code- XX07）
    /// 上砖机与下砖机在同一侧
    /// </summary>
    public class SameSideInTrans : BaseTaskTrans
    {
        public SameSideInTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {

            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
            // 是否有不符规格的车在轨道
            if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trans.give_track_id, goodssizeID, out uint carrierid))
            {
                if (_M.HaveCarrierInTrans(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1007, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        PubMaster.Device.GetDeviceName(carrierid),
                        PubMaster.Track.GetTrackName(trans.give_track_id)));
                    #endregion
                    return;
                }

                if (!PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1107, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        PubMaster.Device.GetDeviceName(carrierid),
                        PubMaster.Track.GetTrackName(trans.give_track_id)));
                    #endregion
                    return;
                }

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1207, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                    PubMaster.Device.GetDeviceName(carrierid),
                    PubMaster.Track.GetTrackName(trans.give_track_id)));
                #endregion

                //转移到同类型轨道
                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                track = PubTask.Carrier.GetCarrierTrack(carrierid);
                _M.AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道);
            }
            else
            {
                _M.SetStatus(trans, TransStatusE.调度设备);
            }
        }

        /// <summary>
        /// 调度设备
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            //取消任务
            if (!PubTask.TileLifter.IsHaveLoadNeed(trans.tilelifter_id, trans.take_track_id)
                && mTimer.IsOver(TimerTag.DownTileHaveLoadNoNeed, trans.tilelifter_id, 10, 5))
            {
                _M.SetStatus(trans, TransStatusE.取消, "砖机非有货需求");
                return;
            }

            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (_M.HaveGiveInTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1307, string.Format("存在相同作业轨道的任务，等待任务完成；"));
                #endregion
                return;
            }

            if (!_M.IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1407, string.Format("当前区域线内分配运输车数已达上限，等待空闲；"));
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
                string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);

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
                case TrackTypeE.储砖_入:
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

                            return;
                        }
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

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                            #endregion

                            //至摆渡车
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.定位指令,
                            //    CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                            //    OverPoint = PubMaster.Track.GetTrackLimitPointOut(ferryTraid),
                            //    ToTrackId = ferryTraid
                            //});
                            MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.上砖摆渡复位点);
                            return;
                        }
                    }
                    break;

                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_出:
                    if (isnotload)
                    {
                        if (!PubTask.TileLifter.IsHaveLoadNeed(trans.tilelifter_id, trans.take_track_id)
                            && mTimer.IsOver(TimerTag.DownTileHaveLoadNoNeed, trans.tilelifter_id, 10, 5))
                        {
                            _M.SetStatus(trans, TransStatusE.取消, "砖机非有货需求");
                            return;
                        }

                        // 串联砖机 判断
                        if (PubMaster.DevConfig.HaveBrother(trans.tilelifter_id))
                        {
                            uint bro = PubMaster.DevConfig.GetBrotherIdOutside(trans.tilelifter_id);
                            if (PubTask.TileLifter.IsTileLoad(bro, trans.take_track_id))
                            {
                                if (PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                                {
                                    _M.SetStatus(trans, TransStatusE.取消, "外侧兄弟砖机有货");
                                }
                                return;
                            }
                        }

                        if (PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                        {
                            _M.SetStatus(trans, TransStatusE.取消, "有其他运输车在砖机轨道");
                            return;
                        }

                        if (!PubTask.TileLifter.IsTakeReady(trans.tilelifter_id, trans.take_track_id, out res))
                        {
                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 1507, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足取砖条件；{2}；",
                                PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                PubMaster.Track.GetTrackName(trans.take_track_id), res), true);
                            #endregion
                            return;
                        }

                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
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

                                //获取砖机配置的取货点
                                ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.take_track_id);
                                if (torfid == 0)
                                {
                                    //如果配置为零则获取取货轨道的rfid1
                                    //torfid = PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                    torfid = PubMaster.Track.GetTrackLimitPointIn(trans.take_track_id);
                                }
                                //取砖
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.取砖指令,
                                //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                //    ToPoint = torfid,
                                //    OverPoint = PubMaster.Track.GetTrackLimitPointOut(trans.take_track_id),
                                //    ToTrackId = trans.take_track_id
                                //});
                                MoveToTake(trans.take_track_id, trans.carrier_id, trans.id, torfid);
                                return;
                            }
                        }
                    }

                    if (isload)
                    {
                        if (PubTask.TileLifter.IsTrackEmtpy(trans.tilelifter_id, trans.take_track_id)
                            || mTimer.IsOver(TimerTag.CarrierLoadNotInTileTrack, trans.take_track_id, 5, 5))
                        {
                            PubMaster.Goods.MoveStock(trans.stock_id, track.id);
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                        }
                    }

                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isload)
                    {
                        if (track.id == trans.take_track_id
                            //&& PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id)
                            )
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程);
                        }
                    }

                    if (isnotload)
                    {
                        if (track.id == trans.take_track_id)
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

                            //没有任务并且停止
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierTake(trans, trans.take_track_id);
                                #endregion

                                //获取砖机配置的取货点
                                ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.take_track_id);
                                if (torfid == 0)
                                {
                                    //如果配置为零则获取取货轨道的rfid1
                                    //torfid = PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                    torfid = PubMaster.Track.GetTrackLimitPointIn(trans.take_track_id);
                                }
                                //取砖
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.取砖指令,
                                //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                //    ToPoint = torfid,
                                //    OverPoint = PubMaster.Track.GetTrackLimitPointOut(trans.take_track_id),
                                //    ToTrackId = trans.take_track_id
                                //});
                                MoveToTake(trans.take_track_id, trans.carrier_id, trans.id, torfid);
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

            #region[分配摆渡车/锁定摆渡车]

            if (track.Type != TrackTypeE.储砖_入 && track.Type != TrackTypeE.储砖_出入)
            {
                if (trans.give_ferry_id == 0)
                {
                    string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, true);

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

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isload)
                    {
                        //摆渡车接车，取到砖后不等完成指令-无缝上摆渡
                        if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.give_ferry_id, track.id, res);
                            #endregion

                            // 摆渡车不到位则到轨道头等待
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                && PubTask.Carrier.GetCurrentPoint(trans.carrier_id) < (track.limit_point_up - 10))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, track.id);
                                #endregion

                                //至点
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = track.ferry_down_code,
                                //    OverPoint = track.limit_point_up,
                                //    ToTrackId = track.id
                                //});
                                MoveToPos(track.id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                            }

                            return;
                        }

                        if ((PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                            || PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, 0, track.limit_point_up)))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
                            #endregion

                            //至摆渡车
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.定位指令,
                            //    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                            //    OverPoint = PubMaster.Track.GetTrackLimitPointIn(ferryTraid),
                            //    ToTrackId = ferryTraid
                            //});
                            MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.上砖摆渡复位点);
                            return;
                        }
                    }
                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.摆渡车_出:
                    if (isload)
                    {
                        ////判断小车是否已上轨道，是则解锁摆渡车
                        //if (PubTask.Carrier.IsCarrierInTrack(trans))
                        //{
                        //    PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
                        //}

                        //小车在摆渡车上
                        bool ferryload = PubTask.Ferry.IsLoad(trans.give_ferry_id);
                        if (ferryload)
                        {
                            PubMaster.Goods.MoveStock(trans.stock_id, track.id);

                            //发送离开给下砖机
                            if (!trans.IsLeaveTileLifter
                                && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.take_track_id))
                            {
                                trans.IsLeaveTileLifter = true;
                            }

                            //1.计算轨道下一车坐标
                            //2.卸货轨道状态是否运行放货                                    
                            //3.是否有其他车在同轨道上
                            if ((!PubMaster.Goods.CalculateNextLocation(trans.TransType, trans.carrier_id, trans.give_track_id, out ushort count, out ushort loc)
                                || !PubMaster.Track.IsStatusOkToGive(trans.give_track_id)
                                || PubTask.Carrier.HaveInTrack(trans.give_track_id, trans.carrier_id))
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
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
                                if (PubMaster.Goods.AllocateGiveTrack(trans.area_id, trans.line, trans.tilelifter_id, trans.goods_id, out List<uint> traids))
                                {
                                    foreach (uint traid in traids)
                                    {
                                        if (!_M.IsTraInTrans(traid)
                                            && !PubTask.Carrier.HaveInTrack(traid, trans.carrier_id)
                                            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, traid)
                                            && _M.SetGiveSite(trans, traid))
                                        {
                                            PubMaster.Track.UpdateRecentGood(trans.give_track_id, trans.goods_id);
                                            PubMaster.Track.UpdateRecentTile(trans.give_track_id, trans.tilelifter_id);
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
                                    _M.SetStepLog(trans, false, 1600, string.Format("没有找到合适的轨道卸砖，继续尝试寻找其他轨道；"));
                                    #endregion
                                }

                                return;
                            }

                            //摆渡车 定位去 放货点
                            //小车到达摆渡车后短暂等待再开始定位
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

                                    Track givetrack = PubMaster.Track.GetTrack(trans.give_track_id);
                                    //放砖
                                    //CarrierActionOrder cao = new CarrierActionOrder
                                    //{
                                    //    Order = DevCarrierOrderE.放砖指令,
                                    //    CheckTra = PubMaster.Track.GetTrackDownCode(givetrack.id),
                                    //    OverPoint = givetrack.limit_point_up,
                                    //    ToTrackId = givetrack.id
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
                                        //cao.ToPoint = loc; // 定位脉冲放砖
                                        PubMaster.Goods.UpdateStockLocationCal(trans.stock_id, loc);
                                    }

                                    //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, cao);
                                    MoveToGive(givetrack.id, trans.carrier_id, trans.id, loc);
                                    return;
                                }
                            }

                        }

                    }
                    break;
                #endregion

                #region[小车在放砖轨道]

                case TrackTypeE.储砖_入:
                    #region[放货轨道]
                    if (isload)
                    {
                        if (!trans.IsReleaseGiveFerry
                                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                        {
                            trans.IsReleaseGiveFerry = true;

                            _M.FreeGiveFerry(trans);
                        }
                    }

                    if (track.id == trans.give_track_id)
                    {
                        PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                    }

                    if (PubTask.Carrier.IsCarrierFinishUnLoad(trans.carrier_id))
                    {
                        //放砖完成后，计算能不能放下一车砖，不能则将轨道满砖 20210206
                        if (!PubMaster.Goods.CalculateNextLocation(trans.TransType, trans.carrier_id, trans.give_track_id, out ushort count, out ushort loc))
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
                        }
                        _M.SetUnLoadTime(trans);
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    #endregion
                    break;

                case TrackTypeE.储砖_出入:
                    #region[放货轨道]
                    if (isload)
                    {
                        if (!trans.IsReleaseGiveFerry
                                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                        {
                            trans.IsReleaseGiveFerry = true;
                        }
                    }

                    if (track.id == trans.give_track_id)
                    {
                        PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                    }

                    if (PubTask.Carrier.IsCarrierFinishUnLoad(trans.carrier_id))
                    {
                        //放砖完成后，计算能不能放下一车砖，不能则将轨道满砖 20210206
                        if (!PubMaster.Goods.CalculateNextLocation(trans.TransType, trans.carrier_id, trans.give_track_id, out ushort count, out ushort loc))
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
                        }

                        _M.SetUnLoadTime(trans);

                        #region 按最小库存数 设满砖 -停用

                        //if (!PubMaster.Track.IsTrackFull(trans.give_track_id))
                        //{
                        //    ushort fullqty = PubMaster.Area.GetAreaFullQty(trans.area_id);

                        //    //当轨道满砖数量库存时就将轨道设为满砖轨道
                        //    if (PubMaster.Goods.GetTrackStockCount(trans.give_track_id) == fullqty)
                        //    {
                        //        PubMaster.Track.UpdateStockStatus(trans.give_track_id, TrackStockStatusE.满砖, "设定最大库存数,自动满砖");
                        //        PubMaster.Track.AddTrackLog(fullqty, trans.carrier_id, trans.give_track_id, TrackLogE.满轨道, "满足最大库存数");
                        //        return;
                        //    }
                        //}

                        #endregion

                        #region 移车 - 出入库轨道满砖则移车到空轨道

                        if (PubMaster.Track.IsTrackFull(trans.give_track_id)
                            && GlobalWcsDataConfig.BigConifg.IsMoveWhenFull(trans.area_id, trans.line)
                            && PubMaster.Track.IsTrackType(trans.give_track_id, TrackTypeE.储砖_出入))
                        {
                            // 优先移动到空轨道
                            //List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出入);
                            List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出入);

                            List<uint> tids = PubMaster.Track.SortTrackIdsWithLineOrder(trackids, track.id, PubMaster.Track.GetTrack(track.id).order);

                            foreach (uint t in tids)
                            {
                                if (!_M.IsTraInTrans(t)
                                    && PubMaster.Track.IsTrackEmtpy(t)
                                    && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, t)
                                    && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                {
                                    trans.finish_track_id = t;
                                    return;
                                }
                            }
                        }

                        #endregion

                        if (trans.finish_track_id == 0)
                        {
                            _M.SetStatus(trans, TransStatusE.完成);
                        }
                        else
                        {
                            _M.SetStatus(trans, TransStatusE.移车中);
                        }
                    }
                    #endregion
                    break;

                    #endregion
            }
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            if (trans.carrier_id == 0 && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 5))
            {
                _M.SetStatus(trans, TransStatusE.完成, "取消任务-结束");
                return;
            }

            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            if (isload)
            {
                if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                {
                    _M.SetLoadTime(trans);
                    _M.SetStatus(trans, TransStatusE.放砖流程, "继续放砖流程");
                    return;
                }
            }

            #region[分配摆渡车/锁定摆渡车]

            if (track.Type != TrackTypeE.储砖_入 && track.Type != TrackTypeE.储砖_出入)
            {
                if (trans.take_ferry_id == 0)
                {
                    string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);

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
                case TrackTypeE.储砖_入:
                    if (isnotload)
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            _M.SetStatus(trans, TransStatusE.完成);
                        }
                    }

                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_出:
                    if (isnotload)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
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

                                //至点
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                //    OverPoint = PubMaster.Track.GetTrackLimitPointOut(trans.give_track_id),
                                //    ToTrackId = trans.give_track_id
                                //});
                                MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                return;
                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    if (isload)
                    {
                        if (track.id == trans.take_track_id
                            && PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id)
                            && mTimer.IsOver(TimerTag.CarrierGotLoad, trans.carrier_id, 1, 5))
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.放砖流程, "继续放砖流程");
                        }
                    }

                    if (isnotload)
                    {
                        if (track.id == trans.take_track_id)
                        {
                            //小车回到原轨道
                            //没有任务并且停止
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res, true))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                #endregion
                                return;
                            }

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                #endregion

                                //至摆渡车
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                //    OverPoint = PubMaster.Track.GetTrackLimitPointIn(ferryTraid),
                                //    ToTrackId = ferryTraid
                                //});
                                MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.上砖摆渡复位点);
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
        /// 【移车中】<br/>
        /// 1.出入库满砖<br/>
        /// 2.将运输车移到其他轨道<br/>
        /// </summary>
        /// <param name="trans"></param>
        public override void MovingCarrier(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            #region[分配摆渡车/锁定摆渡车]

            if (trans.give_ferry_id == 0)
            {
                string msg = _M.AllocateFerry(trans, DeviceTypeE.上摆渡, track, true);

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

            switch (track.Type)
            {
                #region[小车在上砖轨道]
                case TrackTypeE.上砖轨道:
                    _M.SetStatus(trans, TransStatusE.取消, "送运输车回轨");
                    break;
                #endregion

                #region[小车在摆渡车上]
                case TrackTypeE.摆渡车_出:
                    //小车在摆渡车上
                    if (PubTask.Ferry.IsLoad(trans.give_ferry_id)
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        if (trans.finish_track_id == 0)
                        {
                            _M.SetStatus(trans, TransStatusE.取消, "送运输车回轨");
                        }
                        else
                        {
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
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

                                //至点
                                //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //{
                                //    Order = DevCarrierOrderE.定位指令,
                                //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                //    OverPoint = PubMaster.Track.GetTrackLimitPointOut(trans.finish_track_id),
                                //    ToTrackId = trans.finish_track_id
                                //});
                                MoveToPos(trans.finish_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                                return;
                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在放砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_入:
                    if (track.id == trans.finish_track_id)
                    {
                        if (!trans.IsReleaseGiveFerry
                                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
                        {
                            trans.IsReleaseGiveFerry = true;
                        }

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            _M.SetStatus(trans, TransStatusE.完成);
                        }

                        return;
                    }

                    if (track.id != trans.give_track_id)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                        return;
                    }

                    if (trans.finish_track_id == 0)
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                        return;
                    }
                    else
                    {
                        if (!_M.LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                        {
                            #region 【任务步骤记录】
                            _M.LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                            #endregion
                            return;
                        }

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
                            #endregion

                            //至摆渡车
                            //PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            //{
                            //    Order = DevCarrierOrderE.定位指令,
                            //    CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                            //    OverPoint = PubMaster.Track.GetTrackLimitPointOut(ferryTraid),
                            //    ToTrackId = ferryTraid
                            //});
                            MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.上砖摆渡复位点);
                            return;
                        }
                    }
                    break;
                    #endregion
            }
        }


        #region[其他流程]

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
