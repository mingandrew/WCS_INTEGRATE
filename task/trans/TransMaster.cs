using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.area;
using module.goods;
using module.tiletrack;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task.device;

namespace task.trans
{

    /// <summary>
    /// 根据交易信息来调度整个系统
    /// </summary>
    public class TransMaster : TransBase
    {
        #region[调度任务]

        #region[入库任务]
        public override void DoInTrans(StockTrans trans)
        {
            Track track;
            bool isload, isnotload;
            uint ferryTraid;

            switch (trans.TransStaus)
            {
                #region[检查轨道]
                case TransStatusE.检查轨道:

                    // 获取任务品种规格ID
                    uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
                    // 是否有不符规格的车在轨道
                    if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trans.give_track_id, goodssizeID, out uint carrierid))
                    {
                        if (!HaveCarrierInTrans(carrierid)
                            && PubTask.Carrier.IsCarrierFree(carrierid))
                        {
                            if (PubTask.Carrier.IsLoad(carrierid))
                            {
                                PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierLoadNeedTakeCare, (ushort)carrierid, trans.id);
                            }
                            else
                            {
                                PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierLoadNeedTakeCare, (ushort)carrierid);

                                //转移到同类型轨道
                                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                                track = PubTask.Carrier.GetCarrierTrack(carrierid);
                                AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道);
                            }
                        }
                    }
                    else
                    {
                        SetStatus(trans, TransStatusE.调度设备);
                    }
                    break;
                #endregion

                #region[分配运输车]
                case TransStatusE.调度设备:
                    //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
                    if (!HaveGiveInTrackId(trans))
                    {
                        if (!IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType)) return;

                        //分配运输车
                        if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                            && !HaveInCarrier(carrierid)
                            && mTimer.IsOver(TimerTag.CarrierAllocate, trans.take_track_id, 2, 5))
                        {
                            SetCarrier(trans, carrierid);
                            SetStatus(trans, TransStatusE.取砖流程);
                        }
                    }
                    break;
                #endregion

                #region[取砖流程]
                case TransStatusE.取砖流程:
                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

                    //小车当前所在的轨道
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track == null) return;

                    if (trans.take_ferry_id != 0
                        && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
                    {
                        return;
                    }

                    #region[分配摆渡车]
                    //还没有分配取货过程中的摆渡车
                    if (trans.take_ferry_id == 0)
                    {
                        AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);
                    }
                    #endregion

                    isload = PubTask.Carrier.IsLoad(trans.carrier_id);
                    isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
                    switch (track.Type)
                    {
                        #region[小车在储砖轨道]
                        case TrackTypeE.储砖_入:
                            if (isnotload)
                            {
                                //摆渡车接车
                                if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                {
                                    //后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });

                                }
                            }
                            break;

                        case TrackTypeE.储砖_出入:
                            if (isload)
                            {
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    //下降放货
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.放砖指令
                                    });

                                    return;
                                }
                            }

                            if (isnotload)
                            {
                                //摆渡车接车
                                if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                {
                                    //后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });

                                    return;
                                }

                                // 从一端到另一端
                                //if (PubTask.Carrier.IsStopFTask(trans.carrier_id) &&
                                //    PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_2)
                                //{
                                //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                //    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                //    {
                                //        Order = DevCarrierOrderE.定位指令,
                                //        CheckTra = track.ferry_up_code,
                                //        ToRFID = track.rfid_1,
                                //    });

                                //}
                            }
                            break;

                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_入:

                            if (isnotload)
                            {
                                if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                                {
                                    //摆渡车 定位去 取货点
                                    //小车到达摆渡车后短暂等待再开始定位
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        if (!PubTask.TileLifter.IsHaveLoadNeed(trans.tilelifter_id, trans.take_track_id)
                                            && mTimer.IsOver(TimerTag.DownTileHaveLoadNoNeed, trans.tilelifter_id, 200, 50))
                                        {
                                            SetStatus(trans, TransStatusE.取消, "砖机非有货需求");
                                            return;
                                        }

                                        if (PubTask.TileLifter.IsHaveLoadNeed(trans.tilelifter_id, trans.take_track_id) &&
                                            !PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                                        {
                                            if (PubTask.TileLifter.IsTakeReady(trans.tilelifter_id, trans.take_track_id, out _))
                                            {
                                                //获取砖机配置的取货点
                                                ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.take_track_id);
                                                if (torfid == 0)
                                                {
                                                    //如果配置为零则获取取货轨道的rfid1
                                                    torfid = PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                                }
                                                //后退取砖
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.取砖指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                                    ToRFID = torfid,
                                                });
                                            }
                                        }
                                    }
                                }
                            }

                            if (isload)
                            {
                                if (PubTask.TileLifter.IsTrackEmtpy(trans.tilelifter_id, trans.take_track_id)
                                    || mTimer.IsOver(TimerTag.CarrierLoadNotInTileTrack, trans.take_track_id, 5, 5))
                                {
                                    PubMaster.Goods.MoveStock(trans.stock_id, track.id);
                                    SetLoadTime(trans);
                                    SetStatus(trans, TransStatusE.放砖流程);
                                }
                            }

                            break;
                        #endregion

                        #region[小车在下砖轨道]
                        case TrackTypeE.下砖轨道:

                            if (isload)
                            {
                                if (track.id == trans.take_track_id
                                    && PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                                {
                                    SetLoadTime(trans);
                                    SetStatus(trans, TransStatusE.放砖流程);
                                }
                            }

                            if (isnotload)
                            {
                                if (track.id == trans.take_track_id)
                                {
                                    //没有任务并且停止 需要回到摆渡然后才能后退取砖
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {                                                
                                        //获取砖机配置的取货点
                                        ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.take_track_id);
                                        if (torfid == 0)
                                        {
                                            //如果配置为零则获取取货轨道的rfid1
                                            torfid = PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                        }
                                        //后退取砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.取砖指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                            ToRFID = torfid,
                                        });
                                    }
                                }
                                else
                                {
                                    //分配了在别的下砖轨道无货的小车

                                }
                            }

                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[放砖流程]
                case TransStatusE.放砖流程:

                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

                    //小车当前所在的轨道
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track == null) return;

                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_入 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            //还没有分配取货过程中的摆渡车
                            AllocateFerry(trans, DeviceTypeE.下摆渡, track, true);
                            //调度摆渡车接运输车
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
                        #region[小车在下砖轨道]
                        case TrackTypeE.下砖轨道:
                            if (isload)
                            {
                                if (LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                {   //前进至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });
                                }
                            }
                            break;
                        #endregion

                        #region[小车在摆渡车上]
                        case TrackTypeE.摆渡车_入:
                            if (isload)
                            {
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

                                    ushort count = 0, loc = 0;
                                    //1.计算轨道下一车坐标
                                    //2.卸货轨道状态是否运行放货                                    
                                    //3.是否有其他车在同轨道上
                                    if (!PubMaster.Goods.CalculateNextLocation(trans.TransType, trans.carrier_id, trans.give_track_id, out count, out loc)
                                        || !PubMaster.Track.IsStatusOkToGive(trans.give_track_id)
                                        || PubTask.Carrier.HaveInTrack(trans.give_track_id, trans.carrier_id))
                                    {
                                        if (loc == 0)
                                        {
                                            // 设满砖
                                            PubMaster.Track.UpdateStockStatus(trans.give_track_id, TrackStockStatusE.满砖, "计算坐标值无法存入下一车");
                                            PubMaster.Track.AddTrackLog(count, trans.carrier_id, trans.give_track_id, TrackLogE.满轨道, "计算坐标值无法存入下一车");
                                        }

                                        bool isWarn = false;
                                        if (PubMaster.Goods.AllocateGiveTrack(trans.area_id, trans.tilelifter_id, trans.goods_id, out List<uint> traids))
                                        {
                                            foreach (uint traid in traids)
                                            {
                                                if (!IsTraInTrans(traid)
                                                    && !PubTask.Carrier.HaveInTrack(traid, trans.carrier_id)
                                                    && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, traid)
                                                    && SetGiveSite(trans, traid))
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
                                            PubMaster.Warn.AddTaskWarn(WarningTypeE.TransHaveNotTheGiveTrack, (ushort)trans.carrier_id, trans.id);
                                        }

                                        return;
                                    }

                                    //摆渡车 定位去 放货点
                                    //小车到达摆渡车后短暂等待再开始定位
                                    if (PubTask.Ferry.IsLoad(trans.give_ferry_id) &&
                                        LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        //前进放砖
                                        CarrierActionOrder cao = new CarrierActionOrder
                                        {
                                            Order = DevCarrierOrderE.放砖指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id)
                                        };

                                        if (loc == 0)
                                        {
                                            cao.ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id);
                                            cao.OverRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                        }
                                        else
                                        {
                                            Track givetrack = PubMaster.Track.GetTrack(trans.give_track_id);
                                            if (givetrack.Type == TrackTypeE.储砖_出入)
                                            {
                                                cao.ToRFID = givetrack.rfid_2;
                                            }

                                            if (givetrack.Type == TrackTypeE.储砖_入)
                                            {
                                                cao.ToSite = givetrack.split_point;
                                            }
                                            cao.OverRFID = givetrack.rfid_1;

                                            //cao.ToSite = loc;
                                            //cao.OverSite = PubMaster.Track.GetTrackLimitPoint(trans.give_track_id);

                                            PubMaster.Goods.UpdateStockLocationCal(trans.stock_id, loc);
                                        }
                                        PubTask.Carrier.DoOrder(trans.carrier_id, cao);

                                    }
                                }

                                //判断小车是否已上轨道，是则解锁摆渡车
                                if (PubTask.Carrier.IsCarrierInTrack(trans))
                                {
                                    PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
                                }

                                #region[小车没有扫到地标] - 停用

                                //小车离开了摆渡车但是没有扫到轨道地标
                                //if (!ferryload
                                //    && PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.放砖指令)
                                //    && PubTask.Carrier.IsStopHaveTask(trans.carrier_id)
                                //    && mTimer.IsOver(TimerTag.CarrierGiveMissTrack, trans.carrier_id, 10, 5))
                                //{
                                //    //1.记录小车报警
                                //    //2.添加警告
                                //    //3.释放摆渡车
                                //    //4.停用轨道
                                //    PubTask.Carrier.SetCarrierAlert(trans.carrier_id, trans.give_track_id, CarrierAlertE.GiveMissTrack, true);
                                //    PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierGiveMissTrack, (ushort)trans.carrier_id, trans.id, trans.give_track_id);
                                //    PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
                                //    PubMaster.Track.SetTrackStatus(trans.give_track_id, TrackStatusE.停用, out string result);
                                //    PubMaster.Track.SetTrackAlert(trans.give_track_id, trans.carrier_id, trans.id, TrackAlertE.小车读点故障);
                                //    PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.终止);
                                //    SetUnLoadTime(trans);
                                //    SetStatus(trans, TransStatusE.完成);
                                //    return;
                                //}

                                #endregion

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
                                    }
                                }
                                SetUnLoadTime(trans);
                                SetStatus(trans, TransStatusE.完成);
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
                                    }
                                }

                                SetUnLoadTime(trans);

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

                                #region 移车 -停用

                                //if (PubMaster.Track.IsTrackFull(trans.give_track_id))
                                //{
                                //    // 优先移动到空轨道
                                //    List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出入);

                                //    List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, track.id, PubMaster.Track.GetTrack(track.id).order);

                                //    foreach (uint t in tids)
                                //    {
                                //        if (!IsTraInTrans(t) && 
                                //            PubMaster.Track.IsTrackEmtpy(t) &&
                                //            PubMaster.Area.isFerryWithTrack(trans.area_id, trans.give_ferry_id, t) &&
                                //            !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                //        {
                                //            trans.finish_track_id = t;
                                //            return;
                                //        }
                                //    }
                                //}

                                #endregion

                                if (trans.finish_track_id == 0)
                                {
                                    SetStatus(trans, TransStatusE.完成);
                                }
                                else
                                {
                                    SetStatus(trans, TransStatusE.移车中);
                                }
                            }
                            #endregion
                            break;

                            #endregion
                    }

                    break;
                #endregion

                #region[移车流程]
                case TransStatusE.移车中:

                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

                    //小车当前所在的轨道
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track == null) return;

                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_入 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            //还没有分配取货过程中的摆渡车
                            AllocateFerry(trans, DeviceTypeE.下摆渡, track, true);
                            //调度摆渡车接运输车
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
                        #region[小车在下砖轨道]
                        case TrackTypeE.下砖轨道:
                            SetStatus(trans, TransStatusE.取消);
                            break;
                        #endregion

                        #region[小车在摆渡车上]
                        case TrackTypeE.摆渡车_入:
                            //小车在摆渡车上
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                if (trans.finish_track_id == 0)
                                {
                                    SetStatus(trans, TransStatusE.取消);
                                }
                                else
                                {
                                    if (LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        //前进至点
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
                                        });
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

                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    SetStatus(trans, TransStatusE.完成);
                                }

                                return;
                            }

                            if (track.id != trans.give_track_id)
                            {
                                SetStatus(trans, TransStatusE.完成);
                                return;
                            }

                            if (trans.finish_track_id == 0)
                            {
                                SetStatus(trans, TransStatusE.完成);
                                return;
                            }
                            else
                            {
                                if (LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                {
                                    //后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });
                                }
                            }
                            break;
                            #endregion
                    }

                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
                    //PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                    SetFinish(trans);
                    //完成需求
                    PubTask.TileLifterNeed.FinishTileLifterNeed(trans.tilelifter_id, trans.take_track_id);
                    break;
                #endregion

                #region[取消任务]
                case TransStatusE.取消:
                    if (trans.carrier_id == 0 && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 5))
                    {
                        SetStatus(trans, TransStatusE.完成);
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
                            SetLoadTime(trans);
                            SetStatus(trans, TransStatusE.放砖流程);
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
                                    SetStatus(trans, TransStatusE.完成);
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
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        //前进至点
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
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
                                    SetLoadTime(trans);
                                    SetStatus(trans, TransStatusE.放砖流程);
                                }
                            }

                            if (isnotload)
                            {
                                if (track.id == trans.take_track_id)
                                {
                                    //小车回到原轨道
                                    //没有任务并且停止
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //前进至摆渡车
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        });
                                    }
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// 判断轨道是否能发倒库任务
        /// 1.是否有下砖、上砖任务
        /// </summary>
        /// <param name="givetrackid"></param>
        /// <param name="taketrackid"></param>
        /// <param name="devid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool CheckTrackCanDoSort(uint givetrackid, uint taketrackid, uint devid, out string result)
        {
            StockTrans trans = TransList.Find(c => c.TransType == TransTypeE.倒库任务 && c.take_track_id == taketrackid
                                && c.give_track_id == givetrackid);
            if (trans != null)
            {
                if(trans.carrier_id > 0 && trans.carrier_id != devid)
                {
                    result = "非当前轨道任务分配的小车【" + PubMaster.Device.GetDeviceName(trans.carrier_id)+"】";
                    return false;
                }
                result = "";
                return true;
            }

            List<StockTrans> othertrans = TransList.FindAll(c => c.TransType != TransTypeE.倒库任务 && c.HaveTrack(givetrackid, taketrackid));

            if (othertrans.Count > 0)
            {
                foreach (var item in othertrans)
                {
                    result = string.Format("有任务[ {0} ], 不能执行倒库指令", item.TransType);
                    return false;
                }
            }

            if (PubTask.Carrier.HaveInTrackButCarrier(taketrackid, givetrackid, devid, out uint othercarid))
            {
                result = string.Format("倒库轨道有其他车[ {0} ]，不能执行倒库指令", PubMaster.Device.GetDeviceName(othercarid));
                return false;
            }

            result = "";
            return true;
        }

        #endregion

        #region[出库任务]
        public override void DoOutTrans(StockTrans trans)
        {
            Track track;
            bool isload, isnotload, tileemptyneed;
            uint ferryTraid;

            switch (trans.TransStaus)
            {
                #region[分配运输车]
                case TransStatusE.调度设备:

                    tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

                    //取消任务
                    if (trans.carrier_id == 0
                        && !tileemptyneed
                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                    {
                        SetStatus(trans, TransStatusE.完成);
                        return;
                    }

                    //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
                    if (!HaveTaskInTrackId(trans))
                    {
                        if (!IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType)) return;

                        //分配运输车
                        if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
                            && !HaveInCarrier(carrierid))
                        {
                            SetCarrier(trans, carrierid);
                            SetStatus(trans, TransStatusE.取砖流程);
                        }
                    }
                    break;
                #endregion

                #region[取砖放砖流程]
                case TransStatusE.取砖流程:
                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

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
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                    {
                        AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);
                        //调度摆渡车接运输车
                    }
                    #endregion

                    isload = PubTask.Carrier.IsLoad(trans.carrier_id);
                    isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
                    tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

                    switch (track.Type)
                    {
                        #region[小车在储砖轨道]

                        case TrackTypeE.储砖_出:
                            if (!tileemptyneed
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                                return;
                            }

                            if (trans.take_track_id == track.id)
                            {
                                if (isload)
                                {
                                    //小车没货，砖机没有需求了[可能小车在上砖轨道扫不到地标，然后手动放砖了]
                                    if (!tileemptyneed
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                                    {
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }

                                    if (tileemptyneed)
                                    {
                                        SetLoadTime(trans);
                                        //摆渡车接车
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //前进至摆渡车
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                            });
                                        }
                                    }
                                }

                                if (isnotload)
                                {
                                    if (!PubMaster.Track.IsTrackEmtpy(trans.take_track_id))
                                    {
                                        //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //前进至摆渡车
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                            });
                                        }
                                    }

                                    if (PubMaster.Track.IsEmtpy(trans.take_track_id))
                                    {
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }
                                }
                            }
                            else //在非取货轨道
                            {
                                if (isload)
                                {
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        //下降放货
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令
                                        });
                                    }
                                }

                                if (isnotload)
                                {
                                    //摆渡车接车
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //前进至摆渡车
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        });
                                    }
                                }
                            }
                            break;

                        case TrackTypeE.储砖_出入:
                            if (!tileemptyneed
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                                return;
                            }

                            if (trans.take_track_id == track.id)
                            {
                                if (isload)
                                {
                                    //小车没货，砖机没有需求了[可能小车在上砖轨道扫不到地标，然后手动放砖了]
                                    if (!tileemptyneed
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                                    {
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }

                                    if (tileemptyneed)
                                    {
                                        SetLoadTime(trans);
                                        //摆渡车接车
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至摆渡车);
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
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
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }

                                    if (PubMaster.Track.IsEmtpy(trans.take_track_id) || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                    {
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }
                                    else
                                    {
                                        //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //if (PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_2)
                                            //{
                                            //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至摆渡车);
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                            });

                                            return;
                                            //}
                                        }

                                        // 从一端到另一端
                                        //if (PubTask.Carrier.IsStopFTask(trans.carrier_id) &&
                                        //    PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_1)
                                        //{
                                        //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至点);
                                        //    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        //    {
                                        //        Order = DevCarrierOrderE.定位指令,
                                        //        CheckTra = track.ferry_down_code,
                                        //        ToRFID = track.rfid_2,
                                        //    });

                                        //    return;
                                        //}
                                    }

                                }
                            }
                            else //在非取货轨道
                            {
                                if (isload)
                                {
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令
                                        });
                                    }
                                }

                                if (isnotload)
                                {
                                    //摆渡车接车
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //前进至摆渡车
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        });

                                        return;
                                        //}
                                    }

                                    // 从一端到另一端
                                    //if (PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_1)
                                    //{
                                    //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至点);
                                    //    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    //    {
                                    //        Order = DevCarrierOrderE.定位指令,
                                    //        CheckTra = track.ferry_down_code,
                                    //        ToRFID = track.rfid_2,
                                    //    });

                                    //    return;
                                    //}
                                }
                            }
                            break;

                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_出:
                            //取消任务
                            if (!tileemptyneed)
                            {
                                if (isnotload)
                                {
                                    //摆渡车接车
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退取砖);
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.取砖指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id),
                                        });

                                        return;
                                    }
                                }

                                if (PubTask.Ferry.IsStop(trans.take_ferry_id)
                                    && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 200, 50)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    SetStatus(trans, TransStatusE.取消);
                                    return;
                                }
                            }

                            if (tileemptyneed)
                            {
                                if (isload)
                                {
                                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        PubMaster.Goods.MoveStock(trans.stock_id, track.id);

                                        #region 没库存时就将轨道设为空砖

                                        if (!PubMaster.Track.IsEmtpy(trans.take_track_id) && PubMaster.Goods.IsTrackStockEmpty(trans.take_track_id))
                                        {
                                            PubMaster.Track.UpdateStockStatus(trans.take_track_id, TrackStockStatusE.空砖, "系统已无库存,自动调整轨道为空");
                                            PubMaster.Goods.ClearTrackEmtpy(trans.take_track_id);
                                            PubTask.TileLifter.ReseTileCurrentTake(trans.take_track_id);
                                            PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trans.take_track_id, TrackLogE.空轨道, "无库存数据");
                                        }

                                        #endregion

                                        //摆渡车 定位去 卸货点
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                        {
                                            /**
                                             * 1.判断砖机是否是单个砖机
                                             * 2.
                                             */
                                            if (PubMaster.DevConfig.IsBrother(trans.tilelifter_id)
                                                && PubTask.TileLifter.IsInSideTileNeed(trans.tilelifter_id, trans.give_track_id))
                                            {
                                                uint bro = PubMaster.DevConfig.GetBrotherId(trans.tilelifter_id);
                                                SetTile(trans, bro);
                                                return;
                                            }
                                            else
                                            {
                                                if (PubTask.TileLifter.IsGiveReady(trans.tilelifter_id, trans.give_track_id, out _))
                                                {

                                                    //获取砖机配置的取货点
                                                    ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                                    if (torfid == 0)
                                                    {
                                                        //如果配置为零则获取取货轨道的rfid1
                                                        torfid = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                                    }

                                                    //前进放砖
                                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                    {
                                                        Order = DevCarrierOrderE.放砖指令,
                                                        CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                                        ToRFID = torfid,
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }

                                if (isnotload)
                                {
                                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                           && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        if (PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                                        {
                                            // 优先移动到空轨道
                                            List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);

                                            List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                            foreach (uint t in tids)
                                            {
                                                if (!IsTraInTrans(t) && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t) &&
                                                    !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                                {
                                                    if (SetTakeSite(trans, t))
                                                    {
                                                        SetStatus(trans, TransStatusE.取消);
                                                    }

                                                    return;
                                                }
                                            }
                                        }

                                        //摆渡车 定位去 取货点
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                        {
                                            if (PubMaster.Track.IsEmtpy(trans.take_track_id) || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                            {
                                                //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.定位指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id),
                                                });

                                            }
                                            else
                                            {
                                                //后退取砖
                                                CarrierActionOrder cao = new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.取砖指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                                };

                                                TrackTypeE tt = PubMaster.Track.GetTrackType(trans.take_track_id);
                                                if (tt == TrackTypeE.储砖_出入)
                                                {
                                                    // 去入库地标取，回轨道出库地标
                                                    cao.ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                                    cao.OverRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id);
                                                }
                                                else
                                                {
                                                    // 去分段点取，回轨道出库地标
                                                    cao.ToSite = PubMaster.Track.GetTrackSplitPoint(trans.take_track_id);
                                                    cao.OverRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id);
                                                }

                                                PubTask.Carrier.DoOrder(trans.carrier_id, cao);

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
                                //发送离开给上砖机
                                //if (!trans.IsLeaveTileLifter
                                //    && PubTask.TileLifter.DoInvLeave(trans.tilelifter_id, trans.give_track_id))
                                //{
                                //    trans.IsLeaveTileLifter = true;
                                //}

                                if (track.id == trans.give_track_id
                                    && PubTask.Carrier.IsCarrierFinishUnLoad(trans.carrier_id))
                                {
                                    PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                                    SetUnLoadTime(trans);
                                    SetStatus(trans, TransStatusE.还车回轨);
                                }
                            }

                            if (isload)
                            {
                                if (track.id == trans.give_track_id)
                                {
                                    //没有任务并且停止
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {                                                //获取砖机配置的取货点
                                        ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                        if (torfid == 0)
                                        {
                                            //如果配置为零则获取取货轨道的rfid1
                                            torfid = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                        }

                                        //前进放砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                            ToRFID = torfid,
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
                    break;
                #endregion

                #region[取车回轨取砖流程]
                case TransStatusE.还车回轨:

                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

                    //小车当前所在的轨道
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track == null) return;

                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            //还没有分配取货过程中的摆渡车
                            AllocateFerry(trans, DeviceTypeE.上摆渡, track, true);
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
                                    if (LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至摆渡车);
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        });

                                    }
                                }
                            }
                            break;
                        #endregion

                        #region[小车在摆渡车上]
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
                                                        && !IsTraInTrans(trackid)
                                                        && !PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id)
                                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid))
                                                    {
                                                        trans.finish_track_id = trackid;
                                                        isallocate = true;
                                                    }
                                                    // 2.查看是否存在未空砖但无库存的轨道
                                                    else if (PubMaster.Track.HaveTrackInGoodButNotStock(trans.area_id, trans.tilelifter_id,
                                                        trans.goods_id, out List<uint> trackids))
                                                    {
                                                        foreach (var tid in trackids)
                                                        {
                                                            if (!IsTraInTrans(tid)
                                                                && !PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id)
                                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid))
                                                            {
                                                                trans.finish_track_id = tid;
                                                                isallocate = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    // 3.分配库存
                                                    else if (!isallocate && PubMaster.Goods.GetStock(trans.area_id, trans.tilelifter_id,
                                                        trans.goods_id, out List<Stock> allocatestocks))
                                                    {
                                                        foreach (Stock stock in allocatestocks)
                                                        {
                                                            if (!IsTraInTrans(stock.track_id) &&
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
                                        if (LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out string _))
                                        {
                                            if (!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                                && !PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                            {
                                                PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                                PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                                //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退取砖, isoversize);
                                                //PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                //{
                                                //    Order = DevCarrierOrderE.取砖指令,
                                                //    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                                //    ToRFID = PubMaster.Track.GetTrackRFID2(trans.finish_track_id),
                                                //});

                                                CarrierActionOrder cao = new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.取砖指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                                };
                                                TrackTypeE tt = PubMaster.Track.GetTrackType(trans.finish_track_id);
                                                if (tt == TrackTypeE.储砖_出入)
                                                {
                                                    // 去入库地标取，回轨道出库地标
                                                    cao.ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id);
                                                    cao.OverRFID = PubMaster.Track.GetTrackRFID2(trans.finish_track_id);
                                                }
                                                else
                                                {
                                                    // 去分段点取，回轨道出库地标
                                                    cao.ToSite = PubMaster.Track.GetTrackSplitPoint(trans.finish_track_id);
                                                    cao.OverRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id);
                                                }

                                                PubTask.Carrier.DoOrder(trans.carrier_id, cao);


                                            }
                                            else
                                            {
                                                //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.定位指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.finish_track_id),
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
                            }

                            SetStatus(trans, TransStatusE.完成);
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
                    //PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                    SetFinish(trans);
                    //完成需求
                    PubTask.TileLifterNeed.FinishTileLifterNeed(trans.tilelifter_id, trans.give_track_id);
                    break;
                #endregion

                #region[取消任务]
                case TransStatusE.取消:

                    if (trans.carrier_id == 0
                        && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 10))
                    {
                        SetStatus(trans, TransStatusE.完成);
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
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                        && mTimer.IsOver(TimerTag.UpTileReStoreEmtpyNeed, trans.give_track_id, 5, 5))
                    {
                        SetStatus(trans, TransStatusE.取砖流程);
                        return;
                    }

                    switch (track.Type)
                    {
                        #region[小车在储砖轨道]
                        case TrackTypeE.储砖_出入:
                        case TrackTypeE.储砖_出:
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                            }
                            break;
                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_出:
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                            {
                                //小车回到原轨道
                                if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                {
                                    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                        ToRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id),
                                    });

                                    break;
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
                                    SetLoadTime(trans);
                                    SetStatus(trans, TransStatusE.取砖流程);
                                }
                            }

                            if (isnotload)
                            {
                                //小车回到原轨道
                                if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                {
                                    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至摆渡车);
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });

                                }
                            }

                            break;
                            #endregion
                    }
                    break;
                    #endregion

            }
        }

        #endregion

        #region[倒库任务]
        public override void DoSortTrans(StockTrans trans)
        {
            Track track;
            uint ferryTraid;

            switch (trans.TransStaus)
            {
                #region[检查轨道]
                case TransStatusE.检查轨道:
                    bool havedifcaringive = true, havecarintake = true;
                    // 获取任务品种规格ID
                    uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
                    // 是否有不符规格的车在轨道
                    if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trans.give_track_id, goodssizeID, out uint carrierid))
                    {
                        if (!HaveCarrierInTrans(carrierid)
                            && PubTask.Carrier.IsCarrierFree(carrierid))
                        {
                            if (PubTask.Carrier.IsLoad(carrierid))
                            {
                                PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierLoadNeedTakeCare, (ushort)carrierid, trans.id);
                            }
                            else
                            {
                                PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierLoadNeedTakeCare, (ushort)carrierid);

                                //转移到同类型轨道
                                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                                track = PubTask.Carrier.GetCarrierTrack(carrierid);
                                AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道);
                            }
                        }
                    }
                    else
                    {
                        havedifcaringive = false;
                    }

                    //是否有小车在满砖轨道
                    if (PubTask.Carrier.HaveInTrack(trans.take_track_id, out uint fullcarrierid))
                    {
                        if (PubTask.Carrier.IsCarrierFree(fullcarrierid))
                        {
                            AddMoveCarrierTask(trans.take_track_id, fullcarrierid, TrackTypeE.储砖_入, MoveTypeE.转移占用轨道);
                        }
                        else if (PubTask.Carrier.IsCarrierInTask(fullcarrierid, DevCarrierOrderE.前进倒库) ||
                                    PubTask.Carrier.IsCarrierInTask(fullcarrierid, DevCarrierOrderE.后退倒库))
                        {
                            havecarintake = false;
                        }
                    }
                    else
                    {
                        havecarintake = false;
                    }

                    if(!havecarintake && !havedifcaringive)
                    {
                        SetStatus(trans, TransStatusE.调度设备);
                    }
                    break;

                #endregion

                #region[分配运输车]

                case TransStatusE.调度设备:
                    //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
                    if (!HaveTaskSortTrackId(trans))
                    {
                        //分配运输车
                        if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                            && !HaveInCarrier(carrierid))
                        {
                            SetCarrier(trans, carrierid);
                            SetStatus(trans, TransStatusE.移车中);
                        }
                    }
                    break;
                #endregion

                #region[调度车到倒库轨道]
                case TransStatusE.移车中:
                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

                    //小车当前所在的轨道
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track == null) return;

                    #region[分配摆渡车]
                    //还没有分配取货过程中的摆渡车
                    if (track.id != trans.give_track_id
                        && trans.take_ferry_id == 0)
                    {
                        AllocateFerryToCarrierSort(trans, DeviceTypeE.上摆渡);
                        //调度摆渡车接运输车
                    }
                    #endregion

                    bool isload = PubTask.Carrier.IsLoad(trans.carrier_id);
                    bool isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
                    switch (track.Type)
                    {
                        #region[小车在储砖轨道]
                        case TrackTypeE.储砖_入:
                            if (trans.take_track_id == track.id)
                            {
                                if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库) ||
                                    PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.后退倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.后退倒库))
                                {
                                    SetStatus(trans, TransStatusE.倒库中);
                                }
                            }
                            break;
                        case TrackTypeE.储砖_出:

                            if (trans.give_track_id == track.id)
                            {
                                if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库) ||
                                    PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.后退倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.后退倒库))
                                {
                                    if (!trans.IsReleaseGiveFerry
                                         && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                                         && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
                                    {
                                        trans.IsReleaseGiveFerry = true;
                                    }

                                    SetStatus(trans, TransStatusE.倒库中);
                                }
                                else if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    if (isload)
                                    {
                                        PubMaster.Warn.AddTaskWarn(WarningTypeE.CarrierLoadNotSortTask, (ushort)trans.carrier_id, trans.id);
                                    }

                                    if (isnotload)
                                    {
                                        //后退至轨道倒库
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.前进倒库,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                            //OverRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                            MoveCount = (byte)PubMaster.Goods.GetTrackStockCount(trans.take_track_id)
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (isload)
                                {
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令
                                        });
                                    }
                                }

                                if (isnotload)
                                {
                                    //摆渡车接车
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //前进至摆渡车
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid)
                                        });
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_出:

                            if (isload)
                            {
                                PubMaster.Warn.AddTaskWarn(WarningTypeE.CarrierLoadSortTask, (ushort)trans.carrier_id, trans.id);
                            }

                            if (isnotload)
                            {
                                if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                                {
                                    //摆渡车 定位去 空轨道
                                    //小车到达摆渡车后短暂等待再开始定位
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        //后退至轨道倒库
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.前进倒库,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                            //OverRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                            MoveCount = (byte)PubMaster.Goods.GetTrackStockCount(trans.take_track_id)
                                        });
                                    }
                                }
                            }

                            break;
                        #endregion

                        #region[小车在上砖轨道]
                        case TrackTypeE.上砖轨道:

                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[小车倒库]
                case TransStatusE.倒库中:

                    if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库) ||
                        PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.后退倒库))
                    {
                        if (!PubMaster.Goods.ExistStockInTrack(trans.take_track_id))
                        {
                            SetStatus(trans, TransStatusE.小车回轨);
                            //if (PubMaster.Goods.ShiftStock(trans.take_track_id, trans.give_track_id))
                            //{
                                
                            //}
                        }
                        else if(PubTask.Carrier.IsCarrierFree(trans.carrier_id)
                            && PubTask.Carrier.IsNotLoad(trans.carrier_id))
                        {
                            //倒库任务完成，但是还有库存则进行发倒库任务
                            //继续倒库
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.前进倒库,
                                CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                //OverRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                MoveCount = (byte)PubMaster.Goods.GetTrackStockCount(trans.take_track_id)
                            });
                        }
                    }

                    //倒库中，突然倒库的轨道存在其他小车
                    if (PubTask.Carrier.HaveInTrackButCarrier(trans.take_track_id, trans.give_track_id, trans.carrier_id, out carrierid))
                    {
                        //终止
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.终止指令
                        });

                        PubMaster.Warn.AddDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack,
                            (ushort)trans.carrier_id, trans.id, trans.take_track_id, carrierid);
                    }
                    else
                    {
                        PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);
                    }

                    if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.终止指令))
                    {
                        PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id, trans.id, trans.take_track_id);
                    }
                    else
                    {
                        PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id);
                    }

                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track != null && track.Type == TrackTypeE.储砖_入)
                    {
                        CarrierTask carrier = PubTask.Carrier.GetDevCarrier(trans.carrier_id);
                        if (carrier != null)
                        {
                            if (carrier.DevStatus.DeviceStatus == DevCarrierStatusE.后退
                                && (carrier.DevStatus.CurrentSite%100 == 0 
                                || carrier.Position == DevCarrierPositionE.上下摆渡中))
                            {
                                carrier.DoStopNow();
                                carrier.DoStopNow();
                                PubMaster.Device.SetDevWorking(carrier.ID, false, out DeviceTypeE _);
                                PubMaster.Warn.AddDevWarn(WarningTypeE.DeviceSortRunOutTrack, (ushort)carrier.ID, trans.id, track.id);
                            }
                        }
                    }

                    break;
                #endregion

                #region[调度小车回到满砖轨道]
                case TransStatusE.小车回轨:
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track != null)
                    {
                        if (trans.take_track_id == track.id
                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        {
                            //前进至点
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.定位指令,
                                CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                            });

                        }

                        if (trans.give_track_id == track.id
                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        {
                            PubMaster.Track.SetSortTrackStatus(trans.take_track_id, trans.give_track_id, TrackStatusE.倒库中, TrackStatusE.启用);
                            SetStatus(trans, TransStatusE.完成);
                        }
                    }
                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id);
                    SetFinish(trans);
                    break;
                #endregion

                #region[取消任务]
                case TransStatusE.取消:

                    break;
                    #endregion
            }
        }

        #endregion

        #region[移车任务]
        public override void DoMoveCarrier(StockTrans trans)
        {
            Track track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null) return;
            bool isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            uint ferryTraid;
            switch (trans.TransStaus)
            {
                #region[移车中]
                case TransStatusE.移车中:
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
                            if (track.id == trans.take_track_id)
                            {
                                //切换区域[同轨道-不同区域]
                                if (track.brother_track_id == trans.give_track_id)
                                {
                                    if (PubTask.Carrier.IsCarrierFree(trans.carrier_id)
                                        && !PubTask.Carrier.HaveInTrack(trans.give_track_id))
                                    {
                                        //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至点);
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                        });

                                    }
                                }
                                else//不同轨道
                                {
                                    #region[分配摆渡车]
                                    //还没有分配取货过程中的摆渡车
                                    if (trans.take_ferry_id == 0)
                                    {
                                        AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);
                                        //调度摆渡车接运输车
                                    }
                                    #endregion

                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //后退至摆渡车
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        });

                                    }
                                }
                            }

                            if (track.id == trans.give_track_id
                                && PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                            }
                            break;
                        #endregion

                        #region[储砖出轨道]
                        case TrackTypeE.储砖_出:
                            if (isload)
                            {
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                //切换区域[同轨道-不同区域]
                                if (track.brother_track_id == trans.give_track_id)
                                {
                                    if (PubTask.Carrier.IsCarrierFree(trans.carrier_id)
                                        && !PubTask.Carrier.HaveInTrack(trans.give_track_id))
                                    {
                                        //后退至点
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
                                        });

                                    }
                                }
                                else//不同轨道
                                {
                                    #region[分配摆渡车]
                                    //还没有分配取货过程中的摆渡车
                                    if (trans.take_ferry_id == 0)
                                    {
                                        AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);
                                        //调度摆渡车接运输车
                                    }
                                    #endregion

                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //前进至摆渡车
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                        });

                                    }
                                }
                            }

                            if (track.id == trans.give_track_id
                                && PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                            }
                            break;
                        #endregion

                        #region[储砖出入轨道]
                        case TrackTypeE.储砖_出入:
                            break;
                        #endregion

                        #region[摆渡车入]
                        case TrackTypeE.摆渡车_入:

                            #region[分配摆渡车]
                            //还没有分配取货过程中的摆渡车
                            if (trans.take_ferry_id == 0)
                            {
                                AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);
                            }
                            #endregion

                            if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                            {
                                //前进至点
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
                                });

                            }
                            break;
                        #endregion

                        #region[摆渡车出]
                        case TrackTypeE.摆渡车_出:

                            #region[分配摆渡车]
                            //还没有分配取货过程中的摆渡车
                            if (trans.take_ferry_id == 0)
                            {
                                AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);
                            }
                            #endregion

                            if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                            {
                                //后退至点
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                });

                            }
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[完成]
                case TransStatusE.完成:
                    SetFinish(trans);
                    break;
                #endregion

                #region[取消]
                case TransStatusE.取消:
                    SetStatus(trans, TransStatusE.完成);
                    break;
                    #endregion
            }
        }
        #endregion

        #region[手动入库]

        /// <summary>
        /// 执行取货放货任务
        /// </summary>
        /// <param name="trans"></param>
        public override void DoManualInTrans(StockTrans trans)
        {

        }

        #endregion

        #region[手动出库]
        public override void DoManualOutTrans(StockTrans trans)
        {
        }
        #endregion

        #region[同向上砖]

        /// <summary>
        /// 同向上砖任务
        /// </summary>
        /// <param name="trans"></param>
        public override void DoSameSideOutTrans(StockTrans trans)
        {
            Track track;
            bool isload, isnotload, tileemptyneed;
            uint ferryTraid;

            switch (trans.TransStaus)
            {
                #region[分配运输车]
                case TransStatusE.调度设备:

                    tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

                    //取消任务
                    if (trans.carrier_id == 0
                        && !tileemptyneed
                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                    {
                        SetStatus(trans, TransStatusE.完成);
                        return;
                    }

                    //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
                    if (!HaveTaskInTrackId(trans))
                    {
                        if (!IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType)) return;

                        //分配运输车
                        if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
                            && !HaveInCarrier(carrierid))
                        {
                            SetCarrier(trans, carrierid);
                            SetStatus(trans, TransStatusE.取砖流程);
                        }
                    }
                    break;
                #endregion

                #region[取砖放砖流程]
                case TransStatusE.取砖流程:
                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

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
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                    {
                        AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);
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
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                                return;
                            }

                            if (trans.take_track_id == track.id)
                            {
                                if (isload)
                                {
                                    //小车没货，砖机没有需求了[可能小车在上砖轨道扫不到地标，然后手动放砖了]
                                    if (!tileemptyneed
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                                    {
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }

                                    if (tileemptyneed)
                                    {
                                        SetLoadTime(trans);
                                        //摆渡车接车
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //后退至摆渡车
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
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
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }

                                    if (PubMaster.Track.IsEmtpy(trans.take_track_id) || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                    {
                                        SetStatus(trans, TransStatusE.完成);
                                        return;
                                    }
                                    else
                                    {
                                        //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //后退至摆渡车
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                            });

                                            return;
                                        }

                                        // 从一端到另一端
                                        //if (PubTask.Carrier.IsStopFTask(trans.carrier_id) &&
                                        //    PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_2)
                                        //{
                                        //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                        //    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
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
                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    if (isload)
                                    {
                                        //前进放砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令,
                                            CheckTra = track.ferry_up_code,
                                            ToRFID = track.rfid_1,
                                        });
                                    }

                                    if (isnotload)
                                    {
                                        //摆渡车接车
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                        {
                                            //后退至摆渡车
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                                ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                            });

                                            return;
                                        }

                                        // 从一端到另一端
                                        //if (PubTask.Carrier.GetCurrentPoint(trans.carrier_id) == track.rfid_2)
                                        //{
                                        //    //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退至点);
                                        //    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
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
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                    {
                                        //前进取砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.取砖指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                        });

                                        return;
                                    }
                                }

                                if (PubTask.Ferry.IsStop(trans.take_ferry_id)
                                    && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 200, 50)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    SetStatus(trans, TransStatusE.取消);
                                    return;
                                }
                            }

                            if (tileemptyneed)
                            {
                                if (isload)
                                {
                                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        PubMaster.Goods.MoveStock(trans.stock_id, track.id);

                                        #region 没库存时就将轨道设为空砖

                                        if (!PubMaster.Track.IsEmtpy(trans.take_track_id) && PubMaster.Goods.IsTrackStockEmpty(trans.take_track_id))
                                        {
                                            PubMaster.Track.UpdateStockStatus(trans.take_track_id, TrackStockStatusE.空砖, "系统已无库存,自动调整轨道为空");
                                            PubMaster.Goods.ClearTrackEmtpy(trans.take_track_id);
                                            PubTask.TileLifter.ReseTileCurrentTake(trans.take_track_id);
                                            PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trans.take_track_id, TrackLogE.空轨道, "无库存数据");
                                        }

                                        #endregion

                                        //摆渡车 定位去 卸货点
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _))
                                        {
                                            /**
                                             * 1.判断砖机是否是单个砖机
                                             * 2.如果里面有砖机同时有需求，则给里面的砖机送砖
                                             */
                                            if (PubMaster.DevConfig.IsBrother(trans.tilelifter_id)
                                                && PubTask.TileLifter.IsInSideTileNeed(trans.tilelifter_id, trans.give_track_id))
                                            {
                                                uint bro = PubMaster.DevConfig.GetBrotherId(trans.tilelifter_id);
                                                SetTile(trans, bro);
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
                                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                    {
                                                        Order = DevCarrierOrderE.放砖指令,
                                                        CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                                        ToRFID = torfid,
                                                    });

                                                }
                                            }
                                        }
                                    }
                                }

                                if (isnotload)
                                {
                                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                           && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        if (PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                                        {
                                            // 优先移动到空轨道
                                            List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);

                                            List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                            foreach (uint t in tids)
                                            {
                                                if (!IsTraInTrans(t) && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t) &&
                                                    !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                                {
                                                    if (SetTakeSite(trans, t))
                                                    {
                                                        SetStatus(trans, TransStatusE.取消);
                                                    }
                                                    return;
                                                }
                                            }
                                        }

                                        //摆渡车 定位去 取货点
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                        {
                                            if (PubMaster.Track.IsEmtpy(trans.take_track_id) || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                            {
                                                //前进至点
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.定位指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                                });

                                            }
                                            else
                                            {
                                                //前进取砖
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.取砖指令,
                                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
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
                                    SetUnLoadTime(trans);
                                    SetStatus(trans, TransStatusE.还车回轨);
                                }
                            }

                            if (isload)
                            {
                                if (track.id == trans.give_track_id)
                                {
                                    //没有任务并且停止
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        ushort torfid = PubMaster.DevConfig.GetTileSite(trans.tilelifter_id, trans.give_track_id);
                                        if (torfid == 0)
                                        {
                                            //如果配置为零则获取取货轨道的rfid1
                                            torfid = PubMaster.Track.GetTrackRFID1(trans.give_track_id);
                                        }

                                        //前进放砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令,
                                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                            ToRFID = torfid,
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
                    break;
                #endregion

                #region[取车回轨取砖流程]
                case TransStatusE.还车回轨:

                    //小车没有被其他任务占用
                    if (HaveCarrierInTrans(trans)) return;

                    //小车当前所在的轨道
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track == null) return;

                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            //还没有分配取货过程中的摆渡车
                            AllocateFerry(trans, DeviceTypeE.下摆渡, track, true);
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
                                    if (LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                    {
                                        //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至摆渡车);
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
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
                                                        && !IsTraInTrans(trackid)
                                                        && !PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id)
                                                        && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid))
                                                    {
                                                        trans.finish_track_id = trackid;
                                                        isallocate = true;
                                                    }
                                                    // 2.查看是否存在未空砖但无库存的轨道
                                                    else if (PubMaster.Track.HaveTrackInGoodButNotStock(trans.area_id, trans.tilelifter_id,
                                                        trans.goods_id, out List<uint> trackids))
                                                    {
                                                        foreach (var tid in trackids)
                                                        {
                                                            if (!IsTraInTrans(tid)
                                                                && !PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id)
                                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid))
                                                            {
                                                                trans.finish_track_id = tid;
                                                                isallocate = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    // 3.分配库存
                                                    else if (!isallocate && PubMaster.Goods.GetStock(trans.area_id, trans.tilelifter_id,
                                                        trans.goods_id, out List<Stock> allocatestocks))
                                                    {
                                                        foreach (Stock stock in allocatestocks)
                                                        {
                                                            if (!IsTraInTrans(stock.track_id) &&
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
                                        if (LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out string _))
                                        {
                                            if (!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                                && !PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                            {
                                                PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                                PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                                //bool isoversize = PubMaster.Goods.IsGoodsOverSize(trans.goods_id);
                                                //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进取砖, isoversize);
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.取砖指令,
                                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
                                                });

                                            }
                                            else
                                            {
                                                //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.前进至点);
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.定位指令,
                                                    CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
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
                            }

                            SetStatus(trans, TransStatusE.完成);
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
                    //PubMaster.Goods.MoveStock(trans.stock_id, trans.give_track_id);
                    SetFinish(trans);
                    break;
                #endregion

                #region[取消任务]
                case TransStatusE.取消:

                    if (trans.carrier_id == 0
                        && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 10))
                    {
                        SetStatus(trans, TransStatusE.完成);
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
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                        && mTimer.IsOver(TimerTag.UpTileReStoreEmtpyNeed, trans.give_track_id, 5, 5))
                    {
                        SetStatus(trans, TransStatusE.取砖流程);
                        return;
                    }

                    switch (track.Type)
                    {
                        #region[小车在储砖轨道]
                        case TrackTypeE.储砖_出入:
                        case TrackTypeE.储砖_出:
                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                            }
                            break;
                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_入:
                        case TrackTypeE.摆渡车_出:
                            if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                //小车回到原轨道
                                if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                {
                                    if (isload)
                                    {
                                        PubMaster.Goods.MoveStock(trans.stock_id, trans.take_track_id);

                                        //前进放砖
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
                                        });

                                        break;
                                    }

                                    if (isnotload)
                                    {
                                        //前进至点
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.take_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.take_track_id),
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
                                    SetLoadTime(trans);
                                    SetStatus(trans, TransStatusE.取砖流程);
                                }
                            }

                            if (isnotload)
                            {
                                //小车回到原轨道
                                if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true))
                                {
                                    //前进至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });

                                }
                            }

                            break;
                            #endregion
                    }
                    break;
                    #endregion

            }
        }

        #endregion

        #endregion

        #region[检查满轨/添加倒库任务]

        /// <summary>
        /// 检查满砖轨道进行倒库
        /// 1.检查入库满砖轨道
        /// 2.生成倒库任务
        /// </summary>
        public override void CheckTrackSort()
        {
            List<Track> tracks = PubMaster.Track.GetFullInTrackList();
            foreach (Track track in tracks)
            {
                if (!PubMaster.Dic.IsAreaTaskOnoff(track.area, DicAreaTaskE.倒库)) continue;

                int count = HaveAreaSortTask(track.area, track.line);
                if (PubMaster.Area.IsSortTaskLimit(track.area, track.line, count)) continue;

                //if (PubTask.Carrier.HaveUnFinishSortCar(track.area)) continue;

                //if (!PubMaster.Goods.ExistStockInTrack(track.id))
                //{
                //    PubMaster.Warn.AddTraWarn(WarningTypeE.TrackFullButNoneStock, (ushort)track.id, track.name);
                //    continue;
                //}
                //else
                //{
                //    PubMaster.Warn.RemoveTraWarn(WarningTypeE.TrackFullButNoneStock, (ushort)track.id);
                //}

                if (TransList.Exists(c => !c.finish && (c.take_track_id == track.id
                                        || c.give_track_id == track.id
                                        || c.finish_track_id == track.id)))
                {
                    continue;
                }

                if (!PubMaster.Track.IsTrackEmtpy(track.brother_track_id)) continue;

                uint goodsid = PubMaster.Goods.GetGoodsId(track.id);

                if (goodsid != 0)
                {
                    if (!PubMaster.Goods.IsTrackOkForGoods(track.brother_track_id, goodsid))
                    {
                        continue;
                    }

                    uint stockid = PubMaster.Goods.GetTrackStockId(track.id);
                    if (stockid == 0) continue;
                    uint tileid = PubMaster.Goods.GetStockTileId(stockid);

                    uint tileareaid = PubMaster.Area.GetAreaDevAreaId(tileid);

                    if (!PubMaster.Track.IsEarlyFullTimeOver(track.id))
                    {
                        continue;
                    }

                    PubMaster.Track.SetTrackEaryFull(track.id, false, null);

                    //PubMaster.Track.SetSortTrackStatus(track.id, track.brother_track_id, TrackStatusE.启用, TrackStatusE.倒库中);
                    AddTransWithoutLock(tileareaid > 0 ? tileareaid : track.area, 0, TransTypeE.倒库任务, goodsid, stockid, track.id, track.brother_track_id
                        , TransStatusE.检查轨道, 0, track.line);
                }
            }
        }

        private int HaveAreaSortTask(ushort area, ushort line)
        {
            return TransList.Count(c => !c.finish && c.area_id == area && c.line == line && c.TransType == TransTypeE.倒库任务);
        }

        #endregion

        #region[添加移车任务]
        private void AddMoveCarrierTask(uint trackid, uint carrierid, TrackTypeE totracktype, MoveTypeE movetype)
        {
            if (HaveCarrierInTrans(carrierid)) return;

            Track track = PubMaster.Track.GetTrack(trackid);
            if (PubTask.Carrier.IsCarrierFree(carrierid))
            {
                uint givetrackid = 0;
                switch (movetype)
                {
                    case MoveTypeE.转移占用轨道://优先到空轨道

                        // 优先移动到空轨道
                        List<uint> trackids = PubMaster.Area.GetAreaTrackIds(track.area, totracktype);

                        List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trackid, PubMaster.Track.GetTrackOrder(trackid));

                        List<AreaDeviceTrack> traone = PubMaster.Area.GetFerryTrackId(trackid);
                        foreach (uint t in tids)
                        {
                            if (!IsTraInTrans(t) 
                                && !PubTask.Carrier.HaveInTrack(t, carrierid)
                                && PubMaster.Area.ExistFerryToBothTrack(traone, t))
                            {
                                givetrackid = t;
                                break;
                            }
                        }

                        //if (PubMaster.Track.IsTrackFree(track.right_track_id)
                        //    && !IsTraInTrans(track.right_track_id)
                        //    && !PubTask.Carrier.HaveInTrack(track.right_track_id))
                        //{
                        //    givetrackid = track.right_track_id;
                        //}
                        //else if (PubMaster.Track.IsTrackFree(track.left_track_id)
                        //    && !IsTraInTrans(track.left_track_id)
                        //    && !PubTask.Carrier.HaveInTrack(track.right_track_id))
                        //{
                        //    givetrackid = track.left_track_id;
                        //}

                        //if (givetrackid == 0)
                        //{
                        //    List<Track> tracklist = PubMaster.Track.GetTrackInTypeFree(track.area, totracktype);
                        //    foreach (Track tra in tracklist)
                        //    {
                        //        if (!IsTraInTrans(tra.id)
                        //            && !PubTask.Carrier.HaveInTrack(tra.id))
                        //        {
                        //            givetrackid = tra.id;
                        //            break;
                        //        }
                        //    }
                        //}
                        break;
                    case MoveTypeE.释放摆渡车:
                        break;
                    case MoveTypeE.离开砖机轨道:
                        break;
                    case MoveTypeE.切换区域:
                        break;
                }

                if (givetrackid != 0)
                {
                    AddTransWithoutLock(track.area, 0, TransTypeE.移车任务, 0, 0, trackid, givetrackid, TransStatusE.移车中, carrierid, track.line);
                }
            }
        }
        #endregion

        #region[添加手动任务]

        public bool AddManualTrans(ushort area, uint devid, TransTypeE transtype, uint goods_id, uint taketrackid, uint givetrackid, TransStatusE transtatus, out string result)
        {
            result = "";
            if (transtype == TransTypeE.手动下砖)
            {
                if (HaveInTileTrack(taketrackid))
                {
                    result = "已经有该砖机轨道的任务";
                    return false;
                }

                //[已有库存]
                if (!PubMaster.Goods.HaveStockInTrack(taketrackid, goods_id, out uint stockid))
                {
                    byte fullqty = PubTask.TileLifter.GetTileFullQty(devid, goods_id);
                    ////[生成库存]
                    stockid = PubMaster.Goods.AddStock(devid, taketrackid, goods_id, fullqty);
                    if (stockid > 0)
                    {
                        PubMaster.Track.UpdateStockStatus(taketrackid, TrackStockStatusE.有砖, "手动任务");
                        PubMaster.Goods.AddStockInLog(stockid);
                    }
                }

                if (givetrackid != 0)
                {
                    if (!PubMaster.Goods.IsTrackOkForGoods(givetrackid, goods_id))
                    {
                        result = "该轨道放置该品种会碰撞！";
                        return false;
                    }

                    if (!PubMaster.Track.IsEmtpy(givetrackid)
                        && !PubMaster.Goods.ExistStockInTrack(givetrackid, goods_id))
                    {
                        if (PubMaster.Track.HaveEmptyTrackInTile(area, devid))
                        {
                            result = "还有空轨道，请不要混放";
                            return false;
                        }
                    }
                }

                //分配放货点
                if (stockid != 0
                    && givetrackid == 0
                    && PubMaster.Goods.AllocateGiveTrack(area, devid, goods_id, out List<uint> traids))
                {
                    foreach (uint traid in traids)
                    {
                        if (!PubTask.Trans.IsTraInTransWithLock(traid))
                        {
                            givetrackid = traid;
                            break;
                        }
                    }

                    if (givetrackid != 0)
                    {
                        PubMaster.Track.UpdateRecentGood(givetrackid, goods_id);
                        PubMaster.Track.UpdateRecentTile(givetrackid, devid);
                        //生成手动入库任务
                        AddTrans(area, devid, transtype, goods_id, stockid, taketrackid, givetrackid);
                        return true;
                    }
                }
            }

            if (transtype == TransTypeE.手动上砖)
            {
                if (taketrackid != 0)
                {
                    if (PubMaster.Track.IsEmtpy(taketrackid))
                    {
                        result = "轨道已经空！";
                        return false;
                    }

                    if (!PubMaster.Track.IsRecentGoodId(taketrackid, goods_id)
                        && !PubMaster.Goods.ExistStockInTrack(taketrackid, goods_id))
                    {
                        result = "该轨道中没有砖机需要的品种！";
                        return false;
                    }

                    if (!HaveInTileTrack(taketrackid))
                    {
                        uint stockid = PubMaster.Goods.GetTrackTopStockId(taketrackid);
                        //有库存但是不是砖机需要的品种
                        if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goods_id))
                        {
                            PubMaster.Track.UpdateRecentTile(taketrackid, 0);
                            PubMaster.Track.UpdateRecentGood(taketrackid, 0);
                            result = "请再次尝试添加";
                            return false;
                        }
                        //生成出库交易
                        AddTrans(area, devid, transtype, goods_id, stockid, taketrackid, givetrackid);
                        //PubMaster.Goods.AddStockOutLog(stockid, givetrackid, devid);

                        return true;
                    }
                    else
                    {
                        result = "取货轨道已有任务";
                        return false;
                    }
                }

                //1.查看是否有需要重新派发取货的空轨道
                if (PubMaster.Track.HaveTrackInGoodButNotStock(area, devid, goods_id, out List<uint> trackids))
                {
                    foreach (var trackid in trackids)
                    {
                        if (!HaveInTileTrack(trackid))
                        {
                            uint stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
                            //有库存但是不是砖机需要的品种
                            if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goods_id))
                            {
                                PubMaster.Track.UpdateRecentTile(trackid, 0);
                                PubMaster.Track.UpdateRecentGood(trackid, 0);
                                result = "请再次尝试添加";
                                return false;
                            }
                            //生成出库交易
                            AddTrans(area, devid, transtype, goods_id, stockid, trackid, givetrackid);
                            //PubMaster.Goods.AddStockOutLog(stockid, givetrackid, devid);
                            return true;
                        }
                    }

                }
                //分配库存
                else if (PubMaster.Goods.GetStock(area, devid, goods_id, out List<Stock> allocatestocks))
                {
                    foreach (Stock stock in allocatestocks)
                    {
                        if (!IsStockInTrans(stock.id, stock.track_id))
                        {
                            PubMaster.Track.UpdateRecentGood(stock.track_id, goods_id);
                            PubMaster.Track.UpdateRecentTile(stock.track_id, devid);
                            //生成出库交易
                            AddTrans(area, devid, transtype, goods_id, stock.id, stock.track_id, givetrackid);

                            //PubMaster.Goods.AddStockOutLog(stock.id, givetrackid, devid);
                            break;
                        }
                    }
                }
                else
                {
                    result = "找不到库存";
                }
            }
            return false;
        }

        #endregion

        #region[根据小车位置分配摆渡车]

        /// <summary>
        /// 分配摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="track"></param>
        private void AllocateFerry(StockTrans trans, DeviceTypeE ferrytype, Track track, bool allotogiveferry)
        {
            uint ferryid = 0;
            string result = "";
            switch (track.Type)
            {
                #region[下砖区轨道]
                case TrackTypeE.下砖轨道://小车在下砖机轨道上(前往下砖机取砖中)
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.give_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;

                case TrackTypeE.储砖_入://小车在储砖轨道上(准备上摆渡车)
                    //调度摆渡车
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.take_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;

                case TrackTypeE.摆渡车_入://小车在摆渡车上(已经在摆渡车上)

                    uint tferryid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(track.id);
                    if (tferryid != 0)
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, tferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, tferryid);
                        }
                    }
                    break;

                #endregion

                #region[上砖区轨道]
                case TrackTypeE.上砖轨道:
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.give_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;
                case TrackTypeE.摆渡车_出:
                    uint outtferryid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(track.id);
                    if (outtferryid != 0)
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, outtferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, outtferryid);
                        }
                    }
                    break;
                case TrackTypeE.储砖_出:
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.take_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;
                #endregion

                case TrackTypeE.储砖_出入:
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.take_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (ferryid != 0)
            {
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.FailAllocateFerry, trans.id);
            }
            else if (ferryid == 0 && mTimer.IsOver(TimerTag.FailAllocateFerry, trans.take_track_id, 10, 5))
            {
                result = string.Format("{0},{1}货摆渡车", result, allotogiveferry ? "卸" : "取");
                PubMaster.Warn.AddTaskWarn(WarningTypeE.FailAllocateFerry, (ushort)trans.tilelifter_id, trans.id, result);
            }
        }

        /// <summary>
        /// 分配摆渡车给倒库
        /// </summary>
        /// <param name="trans"></param>
        private void AllocateFerryToCarrierSort(StockTrans trans, DeviceTypeE ferrytype)
        {
            if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.give_track_id, out uint ferryid, out string result))
            {
                SetTakeFerry(trans, ferryid);
            }
        }

        #endregion

        #region[其他判断方法]

        /// <summary>
        /// 库存是否被任务占用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool IsStockInTrans(uint id, out string rs)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    rs = "库存在任务中不能删除！";
                    return TransList.Exists(c => c.stock_id == id);
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            rs = "稍后再试！";
            return true;
        }

        /// <summary>
        /// 判断轨道是否有被占用
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        internal bool IsTraInTransWithLock(uint traid)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    return IsTraInTrans(traid);
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        /// <summary>
        /// 判断轨道是否有被占用
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        internal bool IsTraInTrans(uint traid)
        {
            return TransList.Exists(c => !c.finish
                && (c.give_track_id == traid || c.take_track_id == traid || c.finish_track_id == traid));
        }

        internal bool IsStockInTrans(uint stockid, uint trackid)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    return TransList.Exists(c => !c.finish
                    && (c.stock_id == stockid
                    || c.take_track_id == trackid
                    || c.give_track_id == trackid));
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        private bool HaveGiveInTrackId(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && c.give_track_id == trans.give_track_id);
        }

        private bool HaveTaskInTrackId(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (c.take_track_id == trans.take_track_id || c.take_track_id == trans.give_track_id));
        }

        private bool HaveTaskSortTrackId(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && c.IsSiteSame(trans));
        }

        private bool HaveCarrierInTrans(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && c.carrier_id == trans.carrier_id);
        }

        private bool HaveCarrierInTrans(uint carrrierid)
        {
            return TransList.Exists(c => c.TransStaus != TransStatusE.完成 && c.carrier_id == carrrierid);
        }

        private bool HaveTakeFerryInTrans(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (c.take_ferry_id == trans.take_ferry_id || c.give_ferry_id == trans.take_ferry_id));
        }

        private bool HaveGiveFerryInTrans(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (c.take_ferry_id == trans.give_ferry_id || c.give_ferry_id == trans.give_ferry_id));
        }

        /// <summary>
        /// 判断运输车是否在使用中
        /// </summary>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        internal bool HaveInCarrier(uint carrierid)
        {
            return TransList.Exists(c => c.TransStaus != TransStatusE.完成 && c.carrier_id == carrierid);
        }

        /// <summary>
        /// 是否允许继续分配运输车
        /// </summary>
        /// <returns></returns>
        private bool IsAllowToHaveCarTask(uint area, ushort line, TransTypeE tt)
        {
            int count = TransList.Count(c => !c.finish  && c.area_id == area && c.line == line 
                                && c.TransType == tt && c.carrier_id > 0);
            switch (tt)
            {
                case TransTypeE.手动下砖:
                case TransTypeE.下砖任务:
                case TransTypeE.同向下砖:
                    return !PubMaster.Area.IsDownTaskLimit(area, line, count);
                case TransTypeE.手动上砖:
                case TransTypeE.上砖任务:
                case TransTypeE.同向上砖:
                    return !PubMaster.Area.IsUpTaskLimit(area, line, count);
            }
            return !PubMaster.Area.IsSortTaskLimit(area, line, count);
        }

        /// <summary>
        /// 是否存在指定类型未分摆渡车任务
        /// </summary>
        /// <param name="area"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        internal bool IsExistsTask(uint area, TransTypeE tt)
        {
            return TransList.Exists(c => !c.finish && c.area_id == area && c.TransType == tt &&
                (c.take_ferry_id == 0 || c.give_ferry_id == 0));
        }

        #endregion

        #region[更新界面数据]

        protected override void SendMsg(StockTrans trans)
        {
            mMsg.o1 = trans;
            Messenger.Default.Send(mMsg, MsgToken.TransUpdate);
        }

        /// <summary>
        /// 获取所有交易信息
        /// </summary>
        public void GetAllTrans()
        {
            if (Monitor.TryEnter(_for, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    foreach (StockTrans trans in TransList)
                    {
                        SendMsg(trans);
                    }
                }
                finally
                {
                    Monitor.Exit(_for);
                }
            }
        }

        #endregion

        #region[取消任务]

        public bool CancelTask(uint transid, out string result)
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    StockTrans trans = TransList.Find(c => c.id == transid);
                    if (trans != null)
                    {
                        if (trans.TransStaus == TransStatusE.完成)
                        {
                            result = "任务已经完成";
                            return false;
                        }
                        if (trans.TransStaus == TransStatusE.取消)
                        {
                            result = "已经在取消中";
                            return false;
                        }
                        switch (trans.TransType)
                        {
                            case TransTypeE.下砖任务:
                            case TransTypeE.手动下砖:
                                switch (trans.TransStaus)
                                {
                                    case TransStatusE.调度设备:
                                        if (trans.carrier_id == 0)
                                        {
                                            SetStatus(trans, TransStatusE.取消);
                                            return true;
                                        }
                                        break;
                                    case TransStatusE.取砖流程:
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                                        if (PubTask.Carrier.IsNotLoad(trans.carrier_id)
                                            && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.取砖指令)
                                            && nowtrack.Type != TrackTypeE.下砖轨道)
                                        {
                                            SetStatus(trans, TransStatusE.取消);
                                            return true;
                                        }
                                        else
                                        {
                                            result = "小车正在取砖不能取消！";
                                        }
                                        break;
                                    case TransStatusE.放砖流程:
                                        result = "进入放砖流程，不能取消！";
                                        break;
                                }

                                break;
                            case TransTypeE.上砖任务:
                            case TransTypeE.手动上砖:
                            case TransTypeE.同向上砖:
                                switch (trans.TransStaus)
                                {
                                    case TransStatusE.调度设备:
                                        if (trans.carrier_id == 0)
                                        {
                                            SetStatus(trans, TransStatusE.取消);
                                            return true;
                                        }
                                        break;
                                    case TransStatusE.取砖流程:
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                                        if (PubTask.Carrier.IsNotLoad(trans.carrier_id)
                                            && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.放砖指令)
                                            && nowtrack.Type != TrackTypeE.上砖轨道)
                                        {
                                            SetStatus(trans, TransStatusE.取消);
                                            return true;
                                        }
                                        else
                                        {
                                            result = "小车正在上砖！";
                                        }
                                        break;
                                    case TransStatusE.还车回轨:
                                        result = "正在调度小车回轨道";
                                        break;
                                }
                                break;
                            case TransTypeE.倒库任务:
                                if (trans.TransStaus == TransStatusE.调度设备
                                    && trans.carrier_id == 0)
                                {
                                    SetStatus(trans, TransStatusE.取消);
                                    return true;
                                }

                                break;
                            case TransTypeE.移车任务:
                                SetStatus(trans, TransStatusE.取消);
                                break;
                            case TransTypeE.其他:

                                break;
                            default:
                                break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            result = "";
            return false;
        }

        /// <summary>
        /// 切换模式取消任务
        /// </summary>
        /// <param name="tilelifterid"></param>
        /// <param name="goodsid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CancelTaskForCutover(uint tilelifterid, uint goodsid, out string result)
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    List<StockTrans> trans = TransList.FindAll(c => !c.finish && !c.cancel && c.tilelifter_id == tilelifterid);
                    if (trans != null && trans.Count != 0)
                    {
                        foreach (StockTrans t in trans)
                        {
                            switch (t.TransType)
                            {
                                case TransTypeE.下砖任务:
                                case TransTypeE.手动下砖:
                                    if (PubTask.Carrier.IsLoad(t.carrier_id))
                                    {
                                        result = "运输车已取砖，不能取消任务！";
                                        return false;
                                    }

                                    if (t.goods_id == goodsid)
                                    {
                                        SetStatus(t, TransStatusE.取消);
                                    }
                                    break;

                                case TransTypeE.上砖任务:
                                case TransTypeE.手动上砖:
                                case TransTypeE.同向上砖:
                                    if (t.TransStaus == TransStatusE.取砖流程)
                                    {
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(t.carrier_id);
                                        if (PubTask.Carrier.IsLoad(t.carrier_id)
                                            && (PubTask.Carrier.IsCarrierInTask(t.carrier_id, DevCarrierOrderE.放砖指令))
                                            && (nowtrack.Type == TrackTypeE.摆渡车_入 ||
                                                   nowtrack.Type == TrackTypeE.摆渡车_出 ||
                                                   nowtrack.Type == TrackTypeE.上砖轨道))
                                        {
                                            result = "运输车正在上砖，不能取消任务！";
                                            return false;
                                        }
                                    }

                                    SetStatus(t, TransStatusE.取消);
                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        #endregion

        #region[强制完成任务]
        public bool ForseFinish(uint id, out string result)
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    StockTrans trans = TransList.Find(c => c.id == id);
                    if (trans != null)
                    {
                        SetStatus(trans, TransStatusE.完成);
                        return true;
                    }
                    else
                    {
                        result = "找不到任务";
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }

            result = "";
            return false;
        }
        #endregion

        #region[摆渡车锁定定位]

        /// <summary>
        /// 锁定摆渡车并且定位摆渡车
        /// </summary>
        /// <param name="trans">交易</param>
        /// <param name="ferryid">摆渡车ID</param>
        /// <param name="locatetrackid">摆渡车定位站点</param>
        /// <param name="carriertrackid">小车所在站点</param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool LockFerryAndAction(StockTrans trans, uint ferryid, uint locatetrackid, uint carriertrackid,
            out uint ferryTraid, out string result, bool isnotload = false)
        {
            result = "";
            ferryTraid = PubTask.Ferry.GetFerryTrackId(ferryid);

            if (ferryid != 0 && isnotload && PubTask.Ferry.IsLoad(ferryid))
            {
                return false;
            }

            //找摆渡车上的运输车
            //判断运输车是否在动
            //在动则返回false，不给摆渡车发任务
            if (PubTask.Ferry.IsLoad(ferryid))
            {
                //在摆渡车轨道上的运输车是否有状态不是停止的或者是手动的
                if (PubTask.Carrier.HaveTaskForFerry(ferryTraid))
                {
                    return false;
                }
            }

            return ferryid != 0
                && PubTask.Ferry.TryLock(trans, ferryid, carriertrackid)
                && PubTask.Ferry.DoLocateFerry(ferryid, locatetrackid, out result)
                && PubTask.Carrier.IsStopFTask(trans.carrier_id);
        }

        #endregion

        #region[开关联动-取消对应的任务]
        private void StopAreaTask(uint areaid, TransTypeE[] types)
        {
            if (Monitor.TryEnter(_for, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    List<StockTrans> trans = TransList.FindAll(c => !c.finish
                                                && c.area_id == areaid
                                                && types.Contains(c.TransType));
                    if (trans != null)
                    {
                        foreach (StockTrans item in trans)
                        {
                            try
                            {
                                SetStatus(item, TransStatusE.完成);
                                if (item.carrier_id > 0)
                                {
                                    try
                                    {
                                        //PubTask.Carrier.DoTask(item.carrier_id, DevCarrierTaskE.终止);
                                        PubTask.Carrier.DoOrder(item.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.终止指令
                                        });

                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.StackTrace);
                                    }
                                }
                                if (item.take_ferry_id > 0)
                                {
                                    try
                                    {
                                        PubTask.Ferry.StopFerry(item.take_ferry_id, out string result);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.StackTrace);
                                    }
                                }

                                if (item.give_ferry_id > 0)
                                {
                                    try
                                    {
                                        PubTask.Ferry.StopFerry(item.give_ferry_id, out string result);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.StackTrace);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }

                            if (item.TransType == TransTypeE.倒库任务)
                            {

                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_for);
                }
            }
        }

        internal void StopAreaUp(uint areaid)
        {
            StopAreaTask(areaid, new TransTypeE[] { TransTypeE.上砖任务, TransTypeE.手动上砖, TransTypeE.同向上砖 });
        }

        internal void StopAreaDown(uint areaid)
        {
            StopAreaTask(areaid, new TransTypeE[] { TransTypeE.下砖任务, TransTypeE.手动下砖 });
        }

        internal void StopAreaSort(uint areaid)
        {
            StopAreaTask(areaid, new TransTypeE[] { TransTypeE.倒库任务 });
        }

        #endregion
    }
}
