using enums;
using enums.track;
using module.goods;
using module.tiletrack;
using module.track;
using resource;
using System.Collections.Generic;
using task.device;

namespace task.trans.transtask
{
    /// <summary>
    /// 同向上砖
    /// 上砖机与下砖机在同一侧
    /// </summary>
    public class SameSideOutTrans : BaseTaskTrans
    {
        public SameSideOutTrans(TransMaster trans) : base(trans)
        {

        }


        /// <summary>
        /// 1.分配摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

            //取消任务
            if (trans.carrier_id == 0
                && !tileemptyneed
                && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
            {
                _M.SetStatus(trans, TransStatusE.完成);
                return;
            }

            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (!_M.HaveTaskInTrackButSort(trans))
            {
                if (!_M.IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType)) return;

                //分配运输车
                if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
                    && !_M.HaveInCarrier(carrierid))
                {
                    _M.SetCarrier(trans, carrierid);
                    _M.SetStatus(trans, TransStatusE.取砖流程);
                }
            }

        }

        /// <summary>
        /// 取砖流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToTakeTrackTakeStock(StockTrans trans)
        {//小车没有被其他任务占用
            if (_M.HaveCarrierInTrans(trans)) return;

            //小车当前所在的轨道
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;

            if (trans.take_ferry_id != 0 && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
            {
                return;
            }

            #region[分配摆渡车]
            //还没有分配取货过程中的摆渡车
            if (trans.take_ferry_id == 0
                && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
            {
                _M.AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);
                //调度摆渡车接运输车
            }
            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

            switch (track.Type)
            {
                #region[小车在储砖轨道]

                case TrackTypeE.储砖_出入:
                    if (!tileemptyneed
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                        return;
                    }

                    if (trans.take_track_id == track.id)
                    {
                        if (isload)
                        {
                            //小车没货，砖机没有需求了[可能小车在上砖轨道扫不到地标，然后手动放砖了]
                            if (!tileemptyneed
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                            {
                                _M.SetStatus(trans, TransStatusE.完成);
                                return;
                            }

                            if (tileemptyneed)
                            {
                                _M.SetLoadTime(trans);
                                //摆渡车接车
                                if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                                {
                                    //后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        ToTrackId = ferryTraid
                                    });

                                }
                            }
                        }

                        if (isnotload)
                        {
                            // 取砖轨道改为优先清空轨道
                            uint take = PubTask.TileLifter.GetTileCurrentTake(trans.tilelifter_id);
                            if (take != 0 && take != trans.take_track_id)
                            {
                                //直接完成
                                _M.SetStatus(trans, TransStatusE.完成);
                                return;
                            }

                            if (PubMaster.Track.IsEmtpy(trans.take_track_id) || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                            {
                                _M.SetStatus(trans, TransStatusE.完成);
                                return;
                            }
                            else
                            {
                                //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                                if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                                {
                                    //后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        ToTrackId = ferryTraid
                                    });

                                    return;
                                }

                                // 从一端到另一端
                                //if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track) &&
                                //    PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_2)
                                //{
                                //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                //    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //    {
                                //        Order = DevCarrierOrderE.定位指令,
                                //        CheckTra = track.ferry_up_code,
                                //        ToRFID = track.rfid_1,
                                //    });

                                //    return;
                                //}
                            }

                        }
                    }
                    else //在非取货轨道
                    {
                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            if (isload)
                            {
                                //前进放砖
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.放砖指令,
                                    CheckTra = track.ferry_up_code,
                                    ToRFID = track.rfid_1,
                                    ToTrackId = track.id
                                });
                            }

                            if (isnotload)
                            {
                                //摆渡车接车
                                if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                                {
                                    //后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        ToTrackId = ferryTraid
                                    });

                                    return;
                                }

                                // 从一端到另一端
                                //if (PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_2)
                                //{
                                //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                //    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //    {
                                //        Order = DevCarrierOrderE.定位指令,
                                //        CheckTra = track.ferry_up_code,
                                //        ToRFID = track.rfid_2,
                                //    });

                                //    return;
                                //}
                            }
                        }
                    }
                    break;

                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_入:
                    //取消任务
                    if (!tileemptyneed)
                    {
                        if (isnotload)
                        {
                            //摆渡车接车
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                && _M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                            {
                                //前进取砖
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.取砖指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                    ToTrackId = trans.take_track_id
                                });

                                return;
                            }
                        }

                        if (PubTask.Ferry.IsStop(trans.take_ferry_id)
                            && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 200, 50)
                            && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            _M.SetStatus(trans, TransStatusE.取消);
                            return;
                        }
                    }

                    if (tileemptyneed)
                    {
                        if (isload)
                        {
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                PubMaster.Goods.MoveStock(trans.stock_id, track.id);

                                #region 没库存时就将轨道设为空砖

                                if (!PubMaster.Track.IsEmtpy(trans.take_track_id) && PubMaster.Goods.IsTrackStockEmpty(trans.take_track_id))
                                {
                                    PubMaster.Track.UpdateStockStatus(trans.take_track_id, TrackStockStatusE.空砖, "系统已无库存,自动调整轨道为空");
                                    PubMaster.Goods.ClearTrackEmtpy(trans.take_track_id);
                                    PubTask.TileLifter.ReseUpTileCurrentTake(trans.take_track_id);
                                    PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trans.take_track_id, TrackLogE.空轨道, "无库存数据");
                                }

                                #endregion

                                //摆渡车 定位去 卸货点
                                //小车到达摆渡车后短暂等待再开始定位
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                    && _M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                {
                                    /**
                                     * 1.判断砖机是否是单个砖机
                                     * 2.如果里面有砖机同时有需求，则给里面的砖机送砖
                                     */
                                    if (PubMaster.DevConfig.IsBrother(trans.tilelifter_id)
                                        && PubTask.TileLifter.IsInSideTileNeed(trans.tilelifter_id, trans.give_track_id))
                                    {
                                        uint bro = PubMaster.DevConfig.GetBrotherIdInside(trans.tilelifter_id);
                                        _M.SetTile(trans, bro, string.Format("砖机[{0} & {1}有需求,优先放里面", bro, PubMaster.Device.GetDeviceName(bro)));
                                        return;
                                    }
                                    else
                                    {
                                        if (PubTask.TileLifter.IsGiveReady(trans.tilelifter_id, trans.give_track_id, out _))
                                        {
                                            ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                            if (torfid == 0)
                                            {
                                                //如果配置为零则获取取货轨道的rfid1
                                                torfid = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                            }

                                            //前进放砖
                                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.放砖指令,
                                                CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                                ToRFID = torfid,
                                                ToTrackId = trans.give_track_id
                                            });

                                        }
                                    }
                                }
                            }
                        }

                        if (isnotload)
                        {
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                   && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                if (PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                                {
                                    // 优先移动到空轨道
                                    //List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);
                                    List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(trans.area_id, trans.line, TrackTypeE.储砖_出);

                                    List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                    foreach (uint t in tids)
                                    {
                                        if (!_M.IsTraInTrans(t)
                                            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                            && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                        {
                                            if (_M.SetTakeSite(trans, t))
                                            {
                                                _M.SetStatus(trans, TransStatusE.取消);
                                            }
                                            return;
                                        }
                                    }
                                }

                                //摆渡车 定位去 取货点
                                //小车到达摆渡车后短暂等待再开始定位
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                    && _M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                {
                                    if (PubMaster.Track.IsEmtpy(trans.take_track_id) || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                    {
                                        //前进至点
                                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                            ToTrackId = trans.take_track_id
                                        });

                                    }
                                    else
                                    {
                                        //前进取砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.取砖指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                            ToTrackId = trans.take_track_id
                                        });
                                    }
                                }
                            }
                        }

                    }
                    break;
                #endregion

                #region[小车在上砖轨道]
                case TrackTypeE.下砖轨道:
                case TrackTypeE.上砖轨道:
                    if (isnotload)
                    {
                        if (track.id == trans.give_track_id
                            && PubTask.Carrier.IsCarrierFinishUnLoad(trans.carrier_id))
                        {
                            PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                            _M.SetUnLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.还车回轨);
                        }
                    }

                    if (isload)
                    {
                        if (track.id == trans.give_track_id)
                        {
                            //没有任务并且停止
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                if (torfid == 0)
                                {
                                    //如果配置为零则获取取货轨道的rfid1
                                    torfid = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                }

                                //前进放砖
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.放砖指令,
                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                    ToRFID = torfid,
                                    ToTrackId = trans.give_track_id
                                });

                            }
                        }
                        else
                        {
                            //分配了在别的上砖轨道无货的小车
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
            //小车没有被其他任务占用
            if (_M.HaveCarrierInTrans(trans)) return;

            //小车当前所在的轨道
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;

            #region[分配摆渡车/锁定摆渡车]

            if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
            {
                if (trans.give_ferry_id == 0)
                {
                    //还没有分配取货过程中的摆渡车
                    _M.AllocateFerry(trans, DeviceTypeE.下摆渡, track, true);
                    //调度摆渡车接运输车
                }
                else if (!PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id))
                {
                    return;
                }
            }

            #endregion

            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            switch (track.Type)
            {
                #region[小车在上砖/下砖 轨道]
                case TrackTypeE.上砖轨道:
                case TrackTypeE.下砖轨道:
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
                            if (_M.LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                            {
                                // 前进至摆渡车
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

                #region[小车在摆渡车上]
                case TrackTypeE.摆渡车_入:
                case TrackTypeE.摆渡车_出:
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
                                    && !PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                                {
                                    trans.finish_track_id = trans.take_track_id;
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
                                                && !_M.IsTraInTrans(trackid)
                                                && !PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id)
                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid))
                                            {
                                                trans.finish_track_id = trackid;
                                                isallocate = true;
                                            }

                                            #region 2.查看是否存在未空砖但无库存的轨道 - 停用，无库存一定空轨
                                            //else if (PubMaster.Track.HaveTrackInGoodButNotStock(trans.area_id, trans.tilelifter_id,
                                            //    trans.goods_id, out List<uint> trackids))
                                            //{
                                            //    foreach (var tid in trackids)
                                            //    {
                                            //        if (!IsTraInTrans(tid)
                                            //            && !PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id)
                                            //            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid))
                                            //        {
                                            //            trans.finish_track_id = tid;
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
                                                    if (!_M.IsTraInTrans(stock.track_id) &&
                                                        !PubTask.Carrier.HaveInTrack(stock.track_id, trans.carrier_id) &&
                                                        PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, stock.track_id))
                                                    {
                                                        trans.finish_track_id = stock.track_id;
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

                                                trans.finish_track_id = w_track.id;
                                                isallocate = true;
                                                break;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (!isallocate)
                                    {
                                        trans.finish_track_id = trans.take_track_id;
                                    }
                                }
                            }

                            if (trans.finish_track_id != 0)
                            {
                                //摆渡车 定位去 取货点继续取砖
                                //小车到达摆渡车后短暂等待再开始定位
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                                    && _M.LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out string _))
                                {
                                    if (!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                        && !PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                    {
                                        PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                        PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                        // 前进取砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.取砖指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
                                            ToTrackId = trans.finish_track_id
                                        });

                                    }
                                    else
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
                                //判断小车是否已上轨道，是则解锁摆渡车
                                if (PubTask.Carrier.IsCarrierInTrack(trans))
                                {
                                    PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
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
            //PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
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

            //小车当前所在的轨道
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

            //有需求，取货了，回去取砖流程
            if (!PubTask.TileLifter.IsTileCutover(trans.tilelifter_id)
                && isload
                && tileemptyneed
                && PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                && mTimer.IsOver(TimerTag.UpTileReStoreEmtpyNeed, trans.give_track_id, 5, 5))
            {
                _M.SetStatus(trans, TransStatusE.取砖流程);
                return;
            }

            switch (track.Type)
            {
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_出入:
                case TrackTypeE.储砖_出:
                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        _M.SetStatus(trans, TransStatusE.完成);
                    }
                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_入:
                case TrackTypeE.摆渡车_出:
                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        //小车回到原轨道
                        if (_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                        {
                            if (isload)
                            {
                                PubMaster.Goods.MoveStock(trans.stock_id, trans.take_track_id);

                                //前进放砖
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.放砖指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                    ToTrackId = trans.take_track_id
                                });

                                break;
                            }

                            if (isnotload)
                            {
                                //前进至点
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                    ToTrackId = trans.take_track_id
                                });

                                break;
                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在下砖轨道]
                case TrackTypeE.上砖轨道:
                case TrackTypeE.下砖轨道:

                    if (isload)
                    {
                        if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                        {
                            _M.SetLoadTime(trans);
                            _M.SetStatus(trans, TransStatusE.取砖流程);
                        }
                    }

                    if (isnotload)
                    {
                        //小车回到原轨道
                        if (_M.LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                            && PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
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
