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
        #region[构造函数]
        public TransMaster() : base()
        {
            InitDiagnore(this);
        }

        #endregion

        #region[调度任务]

        #region[入库任务]
        /// <summary>
        /// 下砖-入库-流程（code- XX00）
        /// </summary>
        /// <param name="trans"></param>
        public override void DoInTrans(StockTrans trans)
        {
            Track track;
            bool isload, isnotload;
            uint ferryTraid;
            string res = "";

            switch (trans.TransStaus)
            {
                #region[检查轨道]
                case TransStatusE.检查轨道:
                    // 获取任务品种规格ID
                    uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
                    // 是否有不符规格的车在轨道
                    if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trans.give_track_id, goodssizeID, out uint carrierid))
                    {
                        if (HaveCarrierInTrans(carrierid))
                        {
                            #region 【任务步骤记录】
                            SetStepLog(trans, false, 1000, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，[ {0} ]绑定有任务，等待其任务完成；",
                                PubMaster.Device.GetDeviceName(carrierid),
                                PubMaster.Track.GetTrackName(trans.give_track_id)));
                            #endregion
                            return;
                        }

                        if (!PubTask.Carrier.IsCarrierFree(carrierid))
                        {
                            #region 【任务步骤记录】
                            SetStepLog(trans, false, 1100, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                PubMaster.Device.GetDeviceName(carrierid),
                                PubMaster.Track.GetTrackName(trans.give_track_id)));
                            #endregion
                            return;
                        }

                        #region 【任务步骤记录】
                        SetStepLog(trans, false, 1200, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                            PubMaster.Device.GetDeviceName(carrierid),
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion

                        //转移到同类型轨道
                        TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                        track = PubTask.Carrier.GetCarrierTrack(carrierid);
                        AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道);
                    }
                    else
                    {
                        SetStatus(trans, TransStatusE.调度设备);
                    }
                    break;
                #endregion

                #region[分配运输车]
                case TransStatusE.调度设备:
                    //取消任务
                    if (!PubTask.TileLifter.IsHaveLoadNeed(trans.tilelifter_id, trans.take_track_id)
                        && mTimer.IsOver(TimerTag.DownTileHaveLoadNoNeed, trans.tilelifter_id, 10, 5))
                    {
                        SetStatus(trans, TransStatusE.取消, "砖机非有货需求");
                        return;
                    }

                    //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
                    if (HaveGiveInTrackId(trans))
                    {
                        #region 【任务步骤记录】
                        SetStepLog(trans, false, 1300, string.Format("存在相同卸货轨道[ {0} ]的任务，等待任务完成；",
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion
                        return;
                    }

                    if (!IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType))
                    {
                        #region 【任务步骤记录】
                        SetStepLog(trans, false, 1400, string.Format("当前区域线内分配运输车数已达上限，等待空闲；"));
                        #endregion
                        return;
                    }

                    //分配运输车
                    if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                        && !HaveInCarrier(carrierid)
                        && mTimer.IsOver(TimerTag.CarrierAllocate, trans.take_track_id, 2, 5))
                    {
                        SetCarrier(trans, carrierid);
                        SetStatus(trans, TransStatusE.取砖流程);
                        return;
                    }

                    #region 【任务步骤记录】
                    if (LogForCarrier(trans, result)) return;
                    #endregion

                    break;
                #endregion

                #region[取砖流程]
                case TransStatusE.取砖流程:
                    // 运行前提
                    if (!RunPremise(trans, out track))
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
                        string msg = AllocateFerry(trans, DeviceTypeE.下摆渡, track, false);

                        #region 【任务步骤记录】
                        if (LogForTakeFerry(trans, msg)) return;
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
                                LogForCarrierGiving(trans);
                                #endregion

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
                                if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                {
                                    #region 【任务步骤记录】
                                    LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                    #endregion
                                    return;
                                }

                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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
                            break;

                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_入:
                            if (isnotload)
                            {
                                if (!PubTask.TileLifter.IsHaveLoadNeed(trans.tilelifter_id, trans.take_track_id)
                                    && mTimer.IsOver(TimerTag.DownTileHaveLoadNoNeed, trans.tilelifter_id, 10, 5))
                                {
                                    SetStatus(trans, TransStatusE.取消, "砖机非有货需求");
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
                                            SetStatus(trans, TransStatusE.取消, "外侧兄弟砖机有货");
                                        }
                                        return;
                                    }
                                }

                                if (PubTask.Carrier.HaveInTrack(trans.take_track_id, trans.carrier_id))
                                {
                                    SetStatus(trans, TransStatusE.取消, "有其他运输车在砖机轨道");
                                    return;
                                }

                                if (!PubTask.TileLifter.IsTakeReady(trans.tilelifter_id, trans.take_track_id, out res))
                                {
                                    #region 【任务步骤记录】
                                    SetStepLog(trans, false, 1500, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足取砖条件；{2}；",
                                        PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                        PubMaster.Track.GetTrackName(trans.take_track_id), res), true);
                                    #endregion
                                    return;
                                }

                                if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                                {
                                    //摆渡车 定位去 取货点
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierTake(trans, trans.take_track_id);
                                        #endregion

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
                                    //没有任务并且停止
                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierTake(trans, trans.take_track_id);
                                        #endregion

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
                                        return;
                                    }
                                }
                            }

                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[放砖流程]
                case TransStatusE.放砖流程:
                    // 运行前提
                    if (!RunPremise(trans, out track))
                    {
                        return;
                    }

                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_入 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            string msg = AllocateFerry(trans, DeviceTypeE.下摆渡, track, true);

                            #region 【任务步骤记录】
                            if (LogForGiveFerry(trans, msg)) return;
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
                        #region[小车在下砖轨道]
                        case TrackTypeE.下砖轨道:
                            if (isload)
                            {
                                if (!LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                {
                                    #region 【任务步骤记录】
                                    LogForFerryMove(trans, trans.give_ferry_id, track.id, res);
                                    #endregion
                                    return;
                                }

                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
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
                            break;
                        #endregion

                        #region[小车在摆渡车上]
                        case TrackTypeE.摆渡车_入:
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
                                    if (!PubMaster.Goods.CalculateNextLocation(trans.TransType, trans.carrier_id, trans.give_track_id, out ushort count, out ushort loc)
                                        || !PubMaster.Track.IsStatusOkToGive(trans.give_track_id)
                                        || PubTask.Carrier.HaveInTrack(trans.give_track_id, trans.carrier_id))
                                    {
                                        if (loc == 0)
                                        {
                                            // 设满砖
                                            PubMaster.Track.UpdateStockStatus(trans.give_track_id, TrackStockStatusE.满砖, "计算坐标值无法存入下一车");
                                            PubMaster.Track.AddTrackLog(count, trans.carrier_id, trans.give_track_id, TrackLogE.满轨道, "计算坐标值无法存入下一车");

                                            #region 【任务步骤记录】
                                            LogForTrackFull(trans, trans.give_track_id);
                                            #endregion
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

                                            #region 【任务步骤记录】
                                            SetStepLog(trans, false, 1600, string.Format("没有找到合适的轨道卸砖，继续尝试寻找其他轨道；"));
                                            #endregion
                                        }

                                        return;
                                    }

                                    //摆渡车 定位去 放货点
                                    //小车到达摆渡车后短暂等待再开始定位
                                    if (PubTask.Ferry.IsLoad(trans.give_ferry_id))
                                    {
                                        if (!LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                                        {
                                            #region 【任务步骤记录】
                                            LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                                            #endregion
                                            return;
                                        }

                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                        {
                                            #region 【任务步骤记录】
                                            LogForCarrierGive(trans, trans.give_track_id);
                                            #endregion

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

                                                PubMaster.Goods.UpdateStockLocationCal(trans.stock_id, loc);
                                            }
                                            PubTask.Carrier.DoOrder(trans.carrier_id, cao);
                                            return;
                                        }
                                    }

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

                                        #region 【任务步骤记录】
                                        LogForTrackFull(trans, trans.give_track_id);
                                        #endregion
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

                                        #region 【任务步骤记录】
                                        LogForTrackFull(trans, trans.give_track_id);
                                        #endregion
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
                    // 运行前提
                    if (!RunPremise(trans, out track))
                    {
                        return;
                    }

                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_入 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            string msg = AllocateFerry(trans, DeviceTypeE.下摆渡, track, true);

                            #region 【任务步骤记录】
                            if (LogForGiveFerry(trans, msg)) return;
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
                        #region[小车在下砖轨道]
                        case TrackTypeE.下砖轨道:
                            SetStatus(trans, TransStatusE.取消, "送运输车回轨");
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
                                    SetStatus(trans, TransStatusE.取消, "送运输车回轨");
                                }
                                else
                                {
                                    if (!LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out res))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.give_ferry_id, trans.finish_track_id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToTrack(trans, trans.finish_track_id);
                                        #endregion

                                        //前进至点
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.定位指令,
                                            CheckTra = PubMaster.Track.GetTrackUpCode(trans.finish_track_id),
                                            ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id),
                                        });
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
                                if (!LockFerryAndAction(trans, trans.give_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                                {
                                    #region 【任务步骤记录】
                                    LogForFerryMove(trans, trans.give_ferry_id, trans.give_track_id, res);
                                    #endregion
                                    return;
                                }

                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
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
                            break;
                            #endregion
                    }

                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
                    SetFinish(trans);
                    break;
                #endregion

                #region[取消任务]
                case TransStatusE.取消:
                    if (trans.carrier_id == 0 && mTimer.IsOver(TimerTag.TransCancelNoCar, trans.id, 5, 5))
                    {
                        SetStatus(trans, TransStatusE.完成, "取消任务-结束");
                        return;
                    }

                    // 运行前提
                    if (!RunPremise(trans, out track))
                    {
                        return;
                    }

                    isload = PubTask.Carrier.IsLoad(trans.carrier_id);
                    isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

                    if (isload)
                    {
                        if (PubTask.Carrier.IsCarrierFinishLoad(trans.carrier_id))
                        {
                            SetLoadTime(trans);
                            SetStatus(trans, TransStatusE.放砖流程, "继续放砖流程");
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
                                if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                                {
                                    //小车回到原轨道
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToTrack(trans, trans.give_track_id);
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
                                    SetStatus(trans, TransStatusE.放砖流程, "继续放砖流程");
                                }
                            }

                            if (isnotload)
                            {
                                if (track.id == trans.take_track_id)
                                {
                                    //小车回到原轨道
                                    //没有任务并且停止
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res, true))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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
                            break;
                            #endregion
                    }
                    break;
                    #endregion
            }
        }


        #endregion

        #region[出库任务]
        /// <summary>
        /// 上砖-出库-流程（code- XX01）
        /// </summary>
        /// <param name="trans"></param>
        public override void DoOutTrans(StockTrans trans)
        {
            Track track;
            bool isload, isnotload, tileemptyneed;
            uint ferryTraid;
            string res = "";

            switch (trans.TransStaus)
            {
                #region[分配运输车]
                case TransStatusE.调度设备:
                    //取消任务
                    if (trans.carrier_id == 0
                        && !PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id)
                        && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                    {
                        SetStatus(trans, TransStatusE.完成, "砖机非无货需求");
                        return;
                    }

                    //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
                    if (HaveTaskInTrackButSort(trans))
                    {
                        #region 【任务步骤记录】
                        SetStepLog(trans, false, 1001, string.Format("存在相同作业轨道[ {0} ]的任务，等待任务完成；",
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion
                        return;
                    }

                    if (!IsAllowToHaveCarTask(trans.area_id, trans.line, trans.TransType))
                    {
                        #region 【任务步骤记录】
                        SetStepLog(trans, false, 1101, string.Format("当前区域线内分配运输车数已达上限，等待空闲；"));
                        #endregion
                        return;
                    }

                    //分配运输车
                    if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
                        && !HaveInCarrier(carrierid))
                    {
                        SetCarrier(trans, carrierid);
                        SetStatus(trans, TransStatusE.取砖流程);
                        return;
                    }

                    #region 【任务步骤记录】
                    if (LogForCarrier(trans, result)) return;
                    #endregion

                    break;
                #endregion

                #region[取砖放砖流程]
                case TransStatusE.取砖流程:
                    // 运行前提
                    if (!RunPremise(trans, out track))
                    {
                        return;
                    }

                    if (trans.take_ferry_id != 0 && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
                    {
                        return;
                    }

                    #region[分配摆渡车]
                    //还没有分配取货过程中的摆渡车
                    if (trans.take_ferry_id == 0
                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                    {
                        string msg = AllocateFerry(trans, DeviceTypeE.上摆渡, track, false);

                        #region 【任务步骤记录】
                        if (LogForTakeFerry(trans, msg)) return;
                        #endregion
                    }
                    #endregion

                    isload = PubTask.Carrier.IsLoad(trans.carrier_id);
                    isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
                    tileemptyneed = PubTask.TileLifter.IsHaveEmptyNeed(trans.tilelifter_id, trans.give_track_id);

                    switch (track.Type)
                    {
                        #region[小车在储砖轨道]

                        case TrackTypeE.储砖_出:

                            //判断小车是否做了倒库接力任务，并生成任务且完成上砖任务
                            if (CheckCarrierInSortTaskAndAddTask(trans, trans.carrier_id, trans.take_track_id))
                            {
                                SetStatus(trans, TransStatusE.完成,
                                    string.Format("小车【{0}】执行接力倒库，完成上砖任务", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                return;
                            }

                            if (!tileemptyneed
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                            {
                                SetStatus(trans, TransStatusE.完成, "工位非无货需求，直接完成任务");
                                return;
                            }

                            if (trans.take_track_id == track.id)
                            {
                                if (isload)
                                {
                                    SetLoadTime(trans);
                                    //摆渡车接车，取到砖后不等完成指令-无缝上摆渡
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                        #endregion

                                        // 摆渡车不到位则到出库轨道头等待
                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                            && PubTask.Carrier.GetCurrentSite(trans.carrier_id) < track.rfid_2)
                                        {
                                            #region 【任务步骤记录】
                                            LogForCarrierToTrack(trans, track.id);
                                            #endregion

                                            //前进至点
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = track.ferry_down_code,
                                                ToRFID = track.rfid_2,
                                            });
                                        }

                                        return;
                                    }

                                    if ((PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        || PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, track.rfid_2))
                                    && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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

                                if (isnotload)
                                {
                                    if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                        || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                    {
                                        SetStatus(trans, TransStatusE.完成, string.Format("轨道不满足状态[ {0} ]", PubMaster.Track.GetTrackStatusLogInfo(trans.take_track_id)));
                                        return;
                                    }

                                    if (!PubMaster.Track.IsTrackEmtpy(trans.take_track_id))
                                    {
                                        //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                                        if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                        {
                                            #region 【任务步骤记录】
                                            LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                            #endregion
                                            return;
                                        }

                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                                        {
                                            #region 【任务步骤记录】
                                            LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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
                            }
                            else //在非取货轨道
                            {
                                if (isload)
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierGiving(trans);
                                    #endregion

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

                                if (isnotload)
                                {
                                    //摆渡车接车
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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
                            break;

                        case TrackTypeE.储砖_出入:
                            if (!tileemptyneed
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 10, 5))
                            {
                                SetStatus(trans, TransStatusE.完成, "工位非无货需求，直接完成任务");
                                return;
                            }

                            if (trans.take_track_id == track.id)
                            {
                                if (isload)
                                {
                                    SetLoadTime(trans);
                                    //摆渡车接车，取到砖后不等完成指令-无缝上摆渡
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                        #endregion

                                        // 摆渡车不到位则到出库轨道头等待
                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                            && PubTask.Carrier.GetCurrentSite(trans.carrier_id) < track.rfid_2)
                                        {
                                            #region 【任务步骤记录】
                                            LogForCarrierToTrack(trans, track.id);
                                            #endregion

                                            //前进至点
                                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                            {
                                                Order = DevCarrierOrderE.定位指令,
                                                CheckTra = track.ferry_down_code,
                                                ToRFID = track.rfid_2,
                                            });
                                        }

                                        return;
                                    }

                                    if ((PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        || PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, track.rfid_2))
                                    && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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

                                if (isnotload)
                                {
                                    // 取砖轨道改为优先清空轨道
                                    uint take = PubTask.TileLifter.GetTileCurrentTake(trans.tilelifter_id);
                                    if (take != 0 && take != trans.take_track_id)
                                    {
                                        //直接完成
                                        SetStatus(trans, TransStatusE.完成, string.Format("存在设置好的优先清空轨道[ {0} ]", PubMaster.Track.GetTrackName(take)));
                                        return;
                                    }

                                    if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                        || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                    {
                                        SetStatus(trans, TransStatusE.完成, string.Format("轨道不满足状态[ {0} ]", PubMaster.Track.GetTrackStatusLogInfo(trans.take_track_id)));
                                        return;
                                    }
                                    else
                                    {
                                        //小车在轨道上没有任务，需要在摆渡车上才能作业后退取货
                                        if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                        {
                                            #region 【任务步骤记录】
                                            LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                            #endregion
                                            return;
                                        }

                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                                        {
                                            #region 【任务步骤记录】
                                            LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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
                            }
                            else //在非取货轨道
                            {
                                if (isload)
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierGiving(trans);
                                    #endregion

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.放砖指令
                                        });
                                    }
                                    return;
                                }

                                if (isnotload)
                                {
                                    //摆渡车接车
                                    if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id)
                                        && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
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
                            break;

                        #endregion

                        #region[小车在摆渡车]
                        case TrackTypeE.摆渡车_出:
                            //取消任务
                            if (!tileemptyneed
                                && PubTask.Carrier.IsCarrierFree(trans.carrier_id))
                            {
                                if (CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id)
                                    || PubMaster.Goods.IsTrackHaveStockInTopPosition(trans.take_track_id)
                                    || PubTask.Carrier.HaveCarrierMoveTopInTrackUpTop(trans.carrier_id, trans.take_track_id)
                                    || mTimer.IsTimeOutAndReset(TimerTag.TileNeedCancel, trans.id, 20))
                                {
                                    // 优先移动到空轨道
                                    List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);

                                    List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                    foreach (uint t in tids)
                                    {
                                        if (!IsTraInTrans(t)
                                            && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                            && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                        {
                                            // 有货的话就只能找空轨道
                                            if (isload && !PubMaster.Track.IsEmtpy(t))
                                            {
                                                continue;
                                            }

                                            if (SetTakeSite(trans, t))
                                            {
                                                SetStatus(trans, TransStatusE.取消, "工位非无货需求，取消任务");
                                                PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileEmptyNeedAndNoBack, (ushort)trans.carrier_id);
                                                return;
                                            }
                                        }
                                    }

                                    PubMaster.Warn.AddDevWarn(WarningTypeE.UpTileEmptyNeedAndNoBack, (ushort)trans.carrier_id, trans.id);

                                    #region 【任务步骤记录】
                                    SetStepLog(trans, false, 1201, string.Format("砖机工位非无货需求，且运输车[ {0} ]无合适轨道可以回轨；",
                                        PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                    #endregion
                                }
                                //else
                                //{
                                //    SetStatus(trans, TransStatusE.取消);
                                //    PubMaster.Warn.RemoveDevWarn(WarningTypeE.UpTileEmptyNeedAndNoBack, (ushort)trans.carrier_id);
                                //    return;
                                //}

                                #region[旧-逻辑直接返回原轨道]
                                //if (isnotload)
                                //{
                                //    //摆渡车接车
                                //    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _))
                                //    {
                                //        //PubTask.Carrier.DoTask(trans.carrier_id, DevCarrierTaskE.后退取砖);
                                //        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                //        {
                                //            Order = DevCarrierOrderE.取砖指令,
                                //            CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                //            ToRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id),
                                //        });

                                //        return;
                                //    }
                                //}

                                //if (PubTask.Ferry.IsStop(trans.take_ferry_id)
                                //    && mTimer.IsOver(TimerTag.UpTileDonotHaveEmtpyAndNeed, trans.tilelifter_id, 200, 50)
                                //    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                //{
                                //    SetStatus(trans, TransStatusE.取消);
                                //    return;
                                //}
                                #endregion
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

                                        if (!PubMaster.Track.IsEmtpy(trans.take_track_id)
                                            && PubMaster.Goods.IsTrackStockEmpty(trans.take_track_id))
                                        {
                                            PubMaster.Track.UpdateStockStatus(trans.take_track_id, TrackStockStatusE.空砖, "系统已无库存,自动调整轨道为空");
                                            PubMaster.Goods.ClearTrackEmtpy(trans.take_track_id);
                                            PubTask.TileLifter.ReseTileCurrentTake(trans.take_track_id);
                                            PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trans.take_track_id, TrackLogE.空轨道, "无库存数据");

                                            #region 【任务步骤记录】
                                            LogForTrackNull(trans, trans.take_track_id);
                                            #endregion
                                        }

                                        #endregion

                                        //摆渡车 定位去 卸货点
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (!LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                                        {
                                            #region 【任务步骤记录】
                                            LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                                            #endregion
                                            return;
                                        }

                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                        {
                                            /**
                                             * 1.判断砖机是否是单个砖机
                                             * 2.如果兄弟里面的砖机有需求放砖，则优先放入里面的工位
                                             */
                                            if (PubMaster.DevConfig.IsBrother(trans.tilelifter_id)
                                                && PubTask.TileLifter.IsInSideTileNeed(trans.tilelifter_id, trans.give_track_id))
                                            {
                                                uint bro = PubMaster.DevConfig.GetBrotherIdInside(trans.tilelifter_id);
                                                SetTile(trans, bro, string.Format("砖机[ {0} & {1} ]有需求,优先放里面", bro, PubMaster.Device.GetDeviceName(bro)));
                                                return;
                                            }
                                            else
                                            {
                                                if (!PubTask.TileLifter.IsGiveReady(trans.tilelifter_id, trans.give_track_id, out res))
                                                {
                                                    #region 【任务步骤记录】
                                                    SetStepLog(trans, false, 1301, string.Format("砖机[ {0} ]的工位轨道[ {1} ]不满足放砖条件；{2}；",
                                                        PubMaster.Device.GetDeviceName(trans.tilelifter_id),
                                                        PubMaster.Track.GetTrackName(trans.give_track_id), res), true);
                                                    #endregion
                                                    return;
                                                }

                                                #region 【任务步骤记录】
                                                LogForCarrierGive(trans, trans.give_track_id);
                                                #endregion

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
                                                return;
                                            }
                                        }
                                    }
                                }

                                if (isnotload)
                                {
                                    if (PubTask.Ferry.IsLoad(trans.take_ferry_id)
                                           && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        //1.不允许，则不可以有车
                                        //2.允许，则不可以有非倒库车
                                        if (CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id))
                                        {
                                            // 优先移动到空轨道
                                            List<uint> trackids = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);

                                            List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                            foreach (uint t in tids)
                                            {
                                                if (!IsTraInTrans(t)
                                                    && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.take_ferry_id, t)
                                                    && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                                {
                                                    if (SetTakeSite(trans, t))
                                                    {
                                                        SetStatus(trans, TransStatusE.取消, "轨道内有其他运输车");
                                                    }

                                                    return;
                                                }
                                            }

                                            #region 【任务步骤记录】
                                            SetStepLog(trans, false, 1401, string.Format("取砖轨道[ {0} ]内有其他运输车，且运输车[ {1} ]无合适轨道可以回轨；",
                                                PubMaster.Track.GetTrackName(trans.take_track_id),
                                                PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                            #endregion

                                            return;
                                        }

                                        //摆渡车 定位去 取货点
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (!LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                                        {
                                            #region 【任务步骤记录】
                                            LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                            #endregion
                                            return;
                                        }

                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                        {
                                            if (PubMaster.Track.IsEmtpy(trans.take_track_id)
                                                || PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                            {
                                                #region 【任务步骤记录】
                                                LogForCarrierToTrack(trans, trans.take_track_id);
                                                #endregion

                                                //后退至点
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.定位指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id),
                                                });
                                                return;
                                            }
                                            else
                                            {
                                                //判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
                                                if (CheckTopStockAndSendSortTask(trans.carrier_id, trans.take_track_id))
                                                {
                                                    #region 【任务步骤记录】
                                                    LogForCarrierSortRelay(trans, trans.take_track_id);
                                                    #endregion
                                                    return;
                                                }

                                                if (!CheckStockIsableToTake(trans.carrier_id, trans.take_track_id, trans.stock_id))
                                                {
                                                    #region 【任务步骤记录】
                                                    LogForCarrierNoTake(trans, trans.take_track_id);
                                                    #endregion
                                                    return;
                                                }

                                                #region 【任务步骤记录】
                                                LogForCarrierTake(trans, trans.take_track_id);
                                                #endregion

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
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierGive(trans, trans.give_track_id);
                                        #endregion

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
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[取车回轨取砖流程]
                case TransStatusE.还车回轨:
                    // 运行前提
                    if (!RunPremise(trans, out track))
                    {
                        return;
                    }
                    #region[分配摆渡车/锁定摆渡车]

                    if (track.Type != TrackTypeE.储砖_出 && track.Type != TrackTypeE.储砖_出入)
                    {
                        if (trans.give_ferry_id == 0)
                        {
                            string msg = AllocateFerry(trans, DeviceTypeE.上摆渡, track, true);

                            #region 【任务步骤记录】
                            if (LogForGiveFerry(trans, msg)) return;
                            #endregion
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
                                    if (!LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                    {
                                        #region 【任务步骤记录】
                                        LogForFerryMove(trans, trans.give_ferry_id, track.id, res);
                                        #endregion
                                        return;
                                    }

                                    if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        #region 【任务步骤记录】
                                        LogForCarrierToFerry(trans, track.id, trans.give_ferry_id);
                                        #endregion

                                        // 后退至摆渡车
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
                                            && !CheckHaveCarrierInOutTrack(trans.carrier_id, trans.take_track_id)
                                            && CheckTrackStockStillCanUse(trans.carrier_id, trans.take_track_id))
                                        {
                                            SetFinishSite(trans, trans.take_track_id, "还车轨道分配轨道[1]");
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
                                                        && !HaveInTrackButSortTask(trackid)
                                                        && !CheckHaveCarrierInOutTrack(trans.carrier_id, trackid)
                                                        && CheckTrackStockStillCanUse(trans.carrier_id, trackid)
                                                        )
                                                    {
                                                        SetFinishSite(trans, trackid, "还车轨道分配轨道[2]");
                                                        isallocate = true;
                                                    }
                                                    // 2.查看是否存在未空砖但无库存的轨道
                                                    else if (PubMaster.Track.HaveTrackInGoodButNotStock(trans.area_id, trans.tilelifter_id,
                                                        trans.goods_id, out List<uint> trackids))
                                                    {
                                                        foreach (var tid in trackids)
                                                        {
                                                            if (!HaveInTrackButSortTask(tid)
                                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, trackid)
                                                                && !CheckHaveCarrierInOutTrack(trans.carrier_id, trackid)
                                                                && CheckTrackStockStillCanUse(trans.carrier_id, trackid))
                                                            {
                                                                SetFinishSite(trans, tid, "还车轨道分配轨道[3]");
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
                                                            if (!HaveInTrackButSortTask(stock.track_id)
                                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, stock.track_id)
                                                                && !CheckHaveCarrierInOutTrack(trans.carrier_id, stock.track_id)
                                                                && CheckTrackStockStillCanUse(trans.carrier_id, stock.track_id))
                                                            {
                                                                SetFinishSite(trans, stock.track_id, "还车轨道分配轨道[4]");
                                                                isallocate = true;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (!isallocate)
                                                    {
                                                        // 优先移动到空轨道
                                                        List<uint> emptytras = PubMaster.Area.GetAreaTrackIds(trans.area_id, TrackTypeE.储砖_出);

                                                        List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(emptytras, trans.take_track_id, PubMaster.Track.GetTrackOrder(trans.take_track_id));

                                                        foreach (uint t in tids)
                                                        {
                                                            if (!IsTraInTrans(t)
                                                                && PubMaster.Area.IsFerryWithTrack(trans.area_id, trans.give_ferry_id, t)
                                                                && !PubTask.Carrier.HaveInTrack(t, trans.carrier_id))
                                                            {
                                                                SetFinishSite(trans, t, "还车轨道分配轨道[5]");
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

                                                        SetFinishSite(trans, w_track.id, "还车轨道分配轨道[6]");
                                                        isallocate = true;
                                                        break;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if (!isallocate)
                                            {
                                                SetFinishSite(trans, trans.take_track_id, "还车轨道分配轨道[7]");
                                            }
                                        }
                                    }

                                    if (trans.finish_track_id != 0)
                                    {
                                        ////判断小车是否已上轨道，是则解锁摆渡车
                                        //if (PubTask.Carrier.IsCarrierInTrack(trans))
                                        //{
                                        //    PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
                                        //}

                                        //摆渡车 定位去 取货点继续取砖
                                        //小车到达摆渡车后短暂等待再开始定位
                                        if (!LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out res))
                                        {
                                            #region 【任务步骤记录】
                                            LogForFerryMove(trans, trans.give_ferry_id, trans.finish_track_id, res);
                                            #endregion
                                            return;
                                        }

                                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                        {
                                            if (!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                                && !PubMaster.Track.IsStopUsing(trans.finish_track_id, trans.TransType))
                                            {
                                                if (!CheckTrackStockStillCanUse(trans.carrier_id, trans.finish_track_id))
                                                {
                                                    SetFinishSite(trans, 0, "轨道不满足状态重新分配");
                                                    return;
                                                }

                                                PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                                PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                                //判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
                                                if (CheckTopStockAndSendSortTask(trans.carrier_id, trans.finish_track_id))
                                                {
                                                    #region 【任务步骤记录】
                                                    LogForCarrierSortRelay(trans, trans.finish_track_id);
                                                    #endregion
                                                    return;
                                                }

                                                if (!CheckStockIsableToTake(trans.carrier_id, trans.finish_track_id))
                                                {
                                                    #region 【任务步骤记录】
                                                    LogForCarrierNoTake(trans, trans.finish_track_id);
                                                    #endregion
                                                    return;
                                                }

                                                CarrierActionOrder cao = new CarrierActionOrder();
                                                if (PubMaster.Track.GetAndRefreshUpCount(trans.finish_track_id) == 0
                                                    || !PubMaster.Goods.IsTopStockIsGood(trans.finish_track_id, trans.goods_id))
                                                {
                                                    #region 【任务步骤记录】
                                                    LogForCarrierToTrack(trans, trans.finish_track_id);
                                                    #endregion

                                                    //后退至点
                                                    cao.Order = DevCarrierOrderE.定位指令;
                                                    cao.CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id);
                                                    cao.ToRFID = PubMaster.Track.GetTrackRFID1(trans.finish_track_id);
                                                }
                                                else
                                                {
                                                    #region 【任务步骤记录】
                                                    LogForCarrierTake(trans, trans.finish_track_id);
                                                    #endregion

                                                    //后退取砖
                                                    cao.Order = DevCarrierOrderE.取砖指令;
                                                    cao.CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id);
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
                                                }

                                                PubTask.Carrier.DoOrder(trans.carrier_id, cao);
                                            }
                                            else
                                            {
                                                #region 【任务步骤记录】
                                                LogForCarrierToTrack(trans, trans.finish_track_id);
                                                #endregion

                                                // 后退至点
                                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                                {
                                                    Order = DevCarrierOrderE.定位指令,
                                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.finish_track_id),
                                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.finish_track_id),
                                                });
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
                            }

                            //判断小车是否做了倒库接力任务，并生成任务且完成上砖任务
                            if (CheckCarrierInSortTaskAndAddTask(trans, trans.carrier_id, trans.take_track_id))
                            {
                                SetStatus(trans, TransStatusE.完成,
                                    string.Format("小车【{0}】执行接力倒库，完成上砖任务", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                return;
                            }

                            SetStatus(trans, TransStatusE.完成);
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
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

                    // 运行前提
                    if (!RunPremise(trans, out track))
                    {
                        return;
                    }

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
                        SetStatus(trans, TransStatusE.取砖流程, "砖机工位显示无货需求，恢复流程");
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
                                if (!LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out res))
                                {
                                    #region 【任务步骤记录】
                                    LogForFerryMove(trans, trans.take_ferry_id, trans.take_track_id, res);
                                    #endregion
                                    return;
                                }

                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierToTrack(trans, trans.take_track_id);
                                    #endregion

                                    // 后退至点
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(trans.take_track_id),
                                        ToRFID = PubMaster.Track.GetTrackRFID2(trans.take_track_id),
                                    });
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
                                    SetLoadTime(trans);
                                    SetStatus(trans, TransStatusE.取砖流程, "小车载货，恢复流程");
                                }
                            }

                            if (isnotload)
                            {
                                //小车回到原轨道
                                if (!LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out res, true))
                                {
                                    #region 【任务步骤记录】
                                    LogForFerryMove(trans, trans.take_ferry_id, track.id, res);
                                    #endregion
                                    return;
                                }

                                if (PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                {
                                    #region 【任务步骤记录】
                                    LogForCarrierToFerry(trans, track.id, trans.take_ferry_id);
                                    #endregion

                                    // 后退至摆渡车
                                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                    {
                                        Order = DevCarrierOrderE.定位指令,
                                        CheckTra = PubMaster.Track.GetTrackDownCode(ferryTraid),
                                        ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid),
                                    });
                                    return;
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
                        else if (PubTask.Carrier.IsCarrierInTask(fullcarrierid, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
                        {
                            havecarintake = false;
                        }
                    }
                    else
                    {
                        havecarintake = false;
                    }

                    if (!havecarintake && !havedifcaringive)
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
                                if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
                                {
                                    SetStatus(trans, TransStatusE.倒库中);
                                }
                            }
                            break;
                        case TrackTypeE.储砖_出:

                            if (trans.give_track_id == track.id)
                            {
                                if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
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
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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

                    if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
                    {
                        if (!PubMaster.Goods.ExistStockInTrack(trans.take_track_id))
                        {
                            SetStatus(trans, TransStatusE.小车回轨);
                            //if (PubMaster.Goods.ShiftStock(trans.take_track_id, trans.give_track_id))
                            //{

                            //}
                        }
                        else if (PubTask.Carrier.IsCarrierFree(trans.carrier_id)
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

                    if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask))
                    {
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
                    }

                    if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.终止指令))
                    {
                        PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id, trans.id, trans.take_track_id);
                    }
                    else
                    {
                        PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id);
                    }

                    //track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    //if (track != null && track.Type == TrackTypeE.储砖_入
                    //    && PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库))
                    //{
                    //    CarrierTask carrier = PubTask.Carrier.GetDevCarrier(trans.carrier_id);
                    //    if (carrier != null)
                    //    {
                    //        if (carrier.DevStatus.DeviceStatus == DevCarrierStatusE.后退
                    //            && (carrier.DevStatus.CurrentSite % 100 == 0
                    //            || carrier.Position == DevCarrierPositionE.上下摆渡中))
                    //        {
                    //            carrier.DoStop(string.Format("【自动终止小车】, 触发[ {0} ], 指令[ {1} ]", "倒库接近极限"));
                    //            carrier.DoStop(string.Format("【自动终止小车】, 触发[ {0} ], 指令[ {1} ]", "倒库接近极限"));
                    //            PubMaster.Device.SetDevWorking(carrier.ID, false, out DeviceTypeE _, "倒库接近极限");
                    //            PubMaster.Warn.AddDevWarn(WarningTypeE.DeviceSortRunOutTrack, (ushort)carrier.ID, trans.id, track.id);
                    //        }
                    //    }
                    //}

                    break;
                #endregion

                #region[调度小车回到满砖轨道]
                case TransStatusE.小车回轨:
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track != null)
                    {
                        if (!PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id)
                            && !PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
                        {
                            if ((trans.take_track_id == track.id
                                    || (trans.give_track_id == track.id
                                        && PubTask.Carrier.IsCarrierInTrackSmallerSite(trans.carrier_id, trans.give_track_id)))
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
                                && PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, trans.give_track_id)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                            }
                        }
                        else
                        {
                            if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
                            {
                                if (!HaveCarrierInTrans(othercarrier) && PubTask.Carrier.IsCarrierFree(othercarrier))
                                {
                                    //转移到同类型轨道
                                    TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                                    track = PubTask.Carrier.GetCarrierTrack(othercarrier);
                                    AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
                                }
                            }
                        }

                        //if (trans.take_track_id == track.id
                        //    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        //{
                        //    //前进至点
                        //    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        //    {
                        //        Order = DevCarrierOrderE.定位指令,
                        //        CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                        //        ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                        //    });

                        //}

                        //if (trans.give_track_id == track.id
                        //    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                        //{
                        //    //PubMaster.Track.SetSortTrackStatus(trans.take_track_id, trans.give_track_id, TrackStatusE.倒库中, TrackStatusE.启用);
                        //    SetStatus(trans, TransStatusE.完成);
                        //}

                    }
                    break;
                #endregion

                #region[任务完成]
                case TransStatusE.完成:
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id);
                    //PubMaster.Track.GetAndRefreshUpCount(trans.give_track_id);
                    SetFinish(trans);
                    break;
                #endregion

                #region[取消任务]
                case TransStatusE.取消:

                    break;
                #endregion

                #region[任务暂停]
                case TransStatusE.倒库暂停:
                    if (trans.carrier_id != 0)
                    {
                        //倒库中的小车卸完货并且后退中
                        //出库轨道中没有车辆在前面
                        if (PubTask.Carrier.IsCarrierUnLoadAndBackWard(trans.carrier_id)
                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id))
                        {
                            //前进至点
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.定位指令,
                                CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
                            });
                        }

                        if (PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, trans.give_track_id))
                        {
                            SetCarrier(trans, 0, string.Format("倒库任务暂停，释放小车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        }
                    }
                    break;

                    #endregion
            }
        }

        #endregion

        #region[上砖侧倒库任务]
        public override void DoUpSortTrans(StockTrans trans)
        {
            Track track;
            uint ferryTraid;

            switch (trans.TransStaus)
            {
                #region[检查轨道]
                case TransStatusE.检查轨道:

                    //是否有小车在满砖轨道
                    if (PubTask.Carrier.HaveInTrack(trans.take_track_id, out uint carrierid))
                    {
                        if (PubTask.Carrier.IsCarrierFree(carrierid))
                        {
                            SetStatus(trans, TransStatusE.调度设备);
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
                    if (!HaveTaskSortTrackId(trans))
                    {
                        //分配运输车
                        if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                            && !HaveInCarrier(carrierid)
                            //&& mTimer.IsOver(TimerTag.CarrierAllocate, trans.give_track_id, 2)
                            )
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

                    //if (track.id != trans.give_track_id 
                    //    && trans.take_ferry_id != 0 
                    //    && !PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id))
                    //{
                    //    return;
                    //}

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
                                if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
                                {
                                    SetStatus(trans, TransStatusE.倒库中);
                                }
                            }
                            break;
                        case TrackTypeE.储砖_出:

                            if (trans.give_track_id == track.id)
                            {
                                if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库)
                                    || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
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
                                        Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);

                                        //后退至轨道倒库
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.前进倒库,
                                            CheckTra = gtrack.ferry_down_code,
                                            ToSite = (ushort)(gtrack.split_point + 50),
                                            MoveCount = (byte)PubMaster.Goods.GetBehindUpSplitStockCount(gtrack.id, gtrack.up_split_point)
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
                                        Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);

                                        //后退至轨道倒库
                                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.前进倒库,
                                            CheckTra = gtrack.ferry_down_code,
                                            ToSite = (ushort)(gtrack.split_point + 50),
                                            MoveCount = (byte)PubMaster.Goods.GetBehindUpSplitStockCount(gtrack.id, gtrack.up_split_point)
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
                    if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.前进倒库, DevCarrierOrderE.后退倒库))
                    {
                        SetStatus(trans, TransStatusE.小车回轨);
                    }

                    if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask))
                    {
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
                    }

                    if (PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.终止指令))
                    {
                        PubMaster.Warn.AddDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id, trans.id, trans.take_track_id);
                    }
                    else
                    {
                        PubMaster.Warn.RemoveDevWarn(WarningTypeE.CarrierSortButStop, (ushort)trans.carrier_id);
                    }

                    break;
                #endregion

                #region[调度小车回到满砖轨道]
                case TransStatusE.小车回轨:
                    track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                    if (track != null)
                    {
                        if (!PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id)
                            && !PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
                        {
                            if ((trans.take_track_id == track.id
                                    || (trans.give_track_id == track.id
                                        && PubTask.Carrier.IsCarrierInTrackSmallerSite(trans.carrier_id, trans.give_track_id)))
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
                                && PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, trans.give_track_id)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                            {
                                SetStatus(trans, TransStatusE.完成);
                            }
                        }
                        else
                        {
                            if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
                            {
                                if (!HaveCarrierInTrans(othercarrier) && PubTask.Carrier.IsCarrierFree(othercarrier))
                                {
                                    //转移到同类型轨道
                                    TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                                    track = PubTask.Carrier.GetCarrierTrack(othercarrier);
                                    AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
                                }
                            }
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

                #region[任务暂停]
                case TransStatusE.倒库暂停:
                    if (trans.carrier_id != 0)
                    {
                        //倒库中的小车卸完货并且后退中
                        //出库轨道中没有车辆在前面
                        if (PubTask.Carrier.IsCarrierUnLoadAndBackWard(trans.carrier_id)
                            && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id))
                        {
                            //前进至点
                            PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.定位指令,
                                CheckTra = PubMaster.Track.GetTrackUpCode(trans.give_track_id),
                                ToRFID = PubMaster.Track.GetTrackRFID1(trans.give_track_id),
                            });
                        }

                        if (PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, trans.give_track_id))
                        {
                            SetCarrier(trans, 0, string.Format("倒库任务暂停，释放小车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        }
                    }
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
                                        //前进至点
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

                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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

                                    if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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

                            if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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

                            if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _)
                                && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                    if (!HaveTaskInTrackButSort(trans))
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
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                    if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out string _)
                                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                        {
                                            /**
                                             * 1.判断砖机是否是单个砖机
                                             * 2.如果里面有砖机同时有需求，则给里面的砖机送砖
                                             */
                                            if (PubMaster.DevConfig.IsBrother(trans.tilelifter_id)
                                                && PubTask.TileLifter.IsInSideTileNeed(trans.tilelifter_id, trans.give_track_id))
                                            {
                                                uint bro = PubMaster.DevConfig.GetBrotherIdInside(trans.tilelifter_id);
                                                SetTile(trans, bro, string.Format("砖机[{0} & {1}有需求,优先放里面", bro, PubMaster.Device.GetDeviceName(bro)));
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
                                        if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _)
                                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                    if (LockFerryAndAction(trans, trans.give_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                        && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                    {
                                        // 前进至摆渡车
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
                                        if (LockFerryAndAction(trans, trans.give_ferry_id, trans.finish_track_id, track.id, out ferryTraid, out string _)
                                            && PubTask.Carrier.IsStopFTask(trans.carrier_id))
                                        {
                                            if (!PubMaster.Track.IsEmtpy(trans.finish_track_id)
                                                && !PubMaster.Track.IsStopUsing(trans.take_track_id, trans.TransType))
                                            {
                                                PubMaster.Track.UpdateRecentGood(trans.finish_track_id, trans.goods_id);
                                                PubMaster.Track.UpdateRecentTile(trans.finish_track_id, trans.tilelifter_id);

                                                // 前进取砖
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
                                if (LockFerryAndAction(trans, trans.take_ferry_id, trans.take_track_id, track.id, out ferryTraid, out string _)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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
                                if (LockFerryAndAction(trans, trans.take_ferry_id, track.id, track.id, out ferryTraid, out string _, true)
                                    && PubTask.Carrier.IsStopFTask(trans.carrier_id))
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

                int count = GetAreaSortTaskCount(track.area, track.line);
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

        /// <summary>
        /// 获取倒库数量
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetAreaSortTaskCount(uint area, ushort line)
        {
            return TransList.Count(c => !c.finish
                                        && c.area_id == area
                                        && c.line == line
                                        && c.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库));
        }

        /// <summary>
        /// 获取非等待倒库任务数量
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetSortTaskNotWaitCount(uint area, ushort line)
        {
            return TransList.Count(c => !c.finish
                                        && c.area_id == area
                                        && c.line == line
                                        && c.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库)
                                        && c.NotInStatus(TransStatusE.倒库暂停));
        }

        #endregion

        #region[检查轨道/添加上砖侧倒库任务]
        /// <summary>
        /// 检查上砖轨道进行倒库
        /// 1.检查入库满砖轨道
        /// 2.生成倒库任务
        /// </summary>
        public override void CheckUpTrackSort()
        {
            if (!PubMaster.Track.IsUpSplit()) return;

            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)) return;

            List<Track> tracks = PubMaster.Track.GetUpSortTrack();
            foreach (Track track in tracks)
            {
                if (!PubMaster.Dic.IsAreaTaskOnoff(track.area, DicAreaTaskE.上砖)) continue;

                int count = GetAreaSortTaskCount(track.area, track.line);
                if (PubMaster.Area.IsSortTaskLimit(track.area, track.line, count)) continue;

                if (TransList.Exists(c => !c.finish && (c.take_track_id == track.id
                                        || c.give_track_id == track.id
                                        || c.finish_track_id == track.id)))
                {
                    continue;
                }

                uint goodsid = PubMaster.Goods.GetGoodsId(track.id);

                if (goodsid != 0)
                {
                    uint stockid = PubMaster.Goods.GetTrackStockId(track.id);
                    if (stockid == 0) continue;
                    uint tileid = PubMaster.Goods.GetStockTileId(stockid);

                    uint tileareaid = PubMaster.Area.GetAreaDevAreaId(tileid);

                    if (!PubMaster.Track.IsEarlyFullTimeOver(track.id))
                    {
                        continue;
                    }

                    AddTransWithoutLock(tileareaid > 0 ? tileareaid : track.area, 0, TransTypeE.上砖侧倒库, goodsid, stockid, track.id, track.id
                        , TransStatusE.检查轨道, 0, track.line);
                }
            }
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
        private string AllocateFerry(StockTrans trans, DeviceTypeE ferrytype, Track track, bool allotogiveferry)
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

            return result;
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
            return TransList.Exists(c => !c.finish && c.InTrack(traid));
        }

        /// <summary>
        /// 判断库存，作业轨道是否已经被任务占用
        /// </summary>
        /// <param name="stockid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool IsStockInTrans(uint stockid, uint trackid)
        {
            try
            {
                return TransList.Exists(c => !c.finish
                        && (c.stock_id == stockid
                        || c.take_track_id == trackid
                        || c.give_track_id == trackid));
            }
            catch (Exception)
            {

            }
            return true;
        }

        /// <summary>
        /// 判断库存，作业轨道是否已经被任务占用[但是忽略倒库任务]
        /// </summary>
        /// <param name="stockid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool IsStockInTransButSortTask(uint stockid, uint trackid)
        {
            try
            {
                //是否忽略倒库任务绑定的轨道
                bool ignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);
                return TransList.Exists(c => !c.finish
                            && (!ignoresort || c.NotInType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库))
                            && (c.stock_id == stockid || c.InTrack(trackid)));
            }
            catch (Exception)
            {

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

        /// <summary>
        /// 判断任务占用轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        private bool HaveTaskInTrackButSort(StockTrans trans)
        {
            //是否开启【倒库轨道可以同时上砖】
            bool ignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (!ignoresort
                                            || !(c.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库) && c.InStatus(TransStatusE.倒库中))
                                            || c.NotInType(TransTypeE.上砖侧倒库, TransTypeE.倒库任务))
                                    && c.InTrack(trans.take_track_id, trans.give_track_id));
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
            int count = TransList.Count(c => !c.finish && c.area_id == area && c.line == line
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

        /// <summary>
        /// 流程运行前提判断（code-50~79）
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        private bool RunPremise(StockTrans trans, out Track track)
        {
            //小车当前所在的轨道有数据
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null)
            {
                #region 【任务步骤记录】
                SetStepLog(trans, false, 50, string.Format("运输车[ {0} ]当前位置信息异常，等待[ {0} ]恢复；",
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion
                return false;
            }

            //小车没有被其他任务占用
            if (HaveCarrierInTrans(trans))
            {
                #region 【任务步骤记录】
                SetStepLog(trans, false, 51, string.Format("有其他任务锁定了运输车[ {0} ]，等待[ {0} ]空闲；",
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion
                return false;
            }

            return true;
        }

        #endregion

        #region[更新界面数据]

        /// <summary>
        /// 通过任务ID更新任务界面
        /// </summary>
        /// <param name="transid"></param>
        public void ClueViewByTransID(uint transid)
        {
            StockTrans trans = GetTrans(transid);
            if (trans != null)
            {
                SendMsg(trans);
            }
        }

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
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                            return true;
                                        }
                                        break;
                                    case TransStatusE.取砖流程:
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                                        if (PubTask.Carrier.IsNotLoad(trans.carrier_id)
                                            && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.取砖指令)
                                            && nowtrack.Type != TrackTypeE.下砖轨道)
                                        {
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
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
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                            return true;
                                        }
                                        break;
                                    case TransStatusE.取砖流程:
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                                        if (PubTask.Carrier.IsNotLoad(trans.carrier_id)
                                            && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.放砖指令)
                                            && nowtrack.Type != TrackTypeE.上砖轨道)
                                        {
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
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
                                    SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                    return true;
                                }

                                break;
                            case TransTypeE.移车任务:
                                SetStatus(trans, TransStatusE.取消, "手动取消任务");
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
                                        SetStatus(t, TransStatusE.取消, "切换砖机模式取消任务");
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

                                    SetStatus(t, TransStatusE.取消, "切换砖机模式取消任务");
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
        public bool ForseFinish(uint id, out string result, string memo = "")
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    StockTrans trans = TransList.Find(c => c.id == id);
                    if (trans != null)
                    {
                        SetStatus(trans, TransStatusE.完成, memo);
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
                result = "摆渡车载货状态未满足";
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
                    result = "存在运输车上下摆渡中";
                    return false;
                }
            }

            return ferryid != 0
                && PubTask.Ferry.TryLock(trans, ferryid, carriertrackid)
                && PubTask.Ferry.DoLocateFerry(ferryid, locatetrackid, out result);
            //&& PubTask.Carrier.IsStopFTask(trans.carrier_id); 移出单独判断，考虑无缝上摆渡，不卡运输车
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
                                SetStatus(item, TransStatusE.完成, "平板任务开关-清除任务");
                                if (item.carrier_id > 0)
                                {
                                    try
                                    {
                                        PubTask.Carrier.DoOrder(item.carrier_id, new CarrierActionOrder()
                                        {
                                            Order = DevCarrierOrderE.终止指令
                                        }, "【平板任务开关】- 终止小车");

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
                                        PubTask.Ferry.StopFerry(item.take_ferry_id, "【平板任务开关】- 终止T摆渡车", out string result);
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
                                        PubTask.Ferry.StopFerry(item.give_ferry_id, "【平板任务开关】- 终止G摆渡车", out string result);
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

        #region[极限混砖]

        /// <summary>
        /// 判断是否存在同任务类型的并使用相同轨道的任务
        /// </summary>
        /// <param name="traid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsTrasInTransWithType(uint traid, TransTypeE type)
        {
            return TransList.Exists(c => !c.finish && c.TransType == type
                && (c.give_track_id == traid || c.take_track_id == traid || c.finish_track_id == traid));
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
            StockTrans trans = TransList.Find(c => (c.TransType == TransTypeE.上砖侧倒库 || c.TransType == TransTypeE.倒库任务) && c.take_track_id == taketrackid
                                && c.give_track_id == givetrackid);
            if (trans != null)
            {
                if (trans.carrier_id > 0 && trans.carrier_id != devid)
                {
                    result = "非当前轨道任务分配的小车【" + PubMaster.Device.GetDeviceName(trans.carrier_id) + "】";
                    return false;
                }
                result = "";
                return true;
            }

            List<StockTrans> othertrans = TransList.FindAll(c => c.NotInType(TransTypeE.上砖侧倒库, TransTypeE.倒库任务)
                                                                && c.InTrack(givetrackid, taketrackid));

            if (othertrans.Count > 0)
            {
                foreach (var item in othertrans)
                {
                    result = string.Format("有任务[ {0} ], 不能执行倒库指令", item.TransType);
                    return false;
                }
            }

            //允许倒库的时候上砖
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask))
            {
                if (PubTask.Carrier.ExistCarBehind(devid, givetrackid, out uint otherid))
                {
                    result = string.Format("倒库轨道有其他车[ {0} ]，不能执行倒库指令", PubMaster.Device.GetDeviceName(otherid));
                    return false;
                }
            }
            else
            {
                if (PubTask.Carrier.HaveInTrackButCarrier(taketrackid, givetrackid, devid, out uint othercarid))
                {
                    result = string.Format("倒库轨道有其他车[ {0} ]，不能执行倒库指令", PubMaster.Device.GetDeviceName(othercarid));
                    return false;
                }
            }

            result = "";
            return true;
        }

        /// <summary>
        /// 判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        private bool CheckTopStockAndSendSortTask(uint carrier_id, uint track_id)
        {
            //1.打开使用-开关(使用上砖侧分割点坐标)
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)) return false;

            //2.判断是否是出轨道
            Track track = PubMaster.Track.GetTrack(track_id);
            if (track == null || track.Type != TrackTypeE.储砖_出) return false;

            //3.如果已经有倒库任务则不发了
            if (TransList.Exists(c => !c.finish && c.take_track_id == track_id && (c.TransType == TransTypeE.倒库任务 || c.TransType == TransTypeE.上砖侧倒库)))
            {
                return false;
            }

            //4.判断轨道中是否已经有其他车在倒库了
            if (PubTask.Carrier.CheckHaveCarInTrack(TransTypeE.上砖侧倒库, track_id, carrier_id)) return false;

            //5.轨道的头部库存位置处于则给小车发送倒库任务
            if (PubMaster.Goods.IsTopStockBehindUpSplitPoint(track_id))
            {
                //后退至轨道倒库
                PubTask.Carrier.DoOrder(carrier_id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.前进倒库,
                    CheckTra = track.ferry_down_code,
                    ToSite = (ushort)(track.split_point + 50),//倒库时，不能超过脉冲(出库轨道附件脉冲位置)
                    MoveCount = (byte)PubMaster.Goods.GetBehindUpSplitStockCount(track.id, track.up_split_point)
                });
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断小车是否在执行倒库指令
        /// 1.添加上砖倒库任务给该小车
        /// 2.返回true,完成上砖任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        private bool CheckCarrierInSortTaskAndAddTask(StockTrans trans, uint carrier_id, uint track_id)
        {
            if (PubTask.Carrier.IsCarrierInTask(carrier_id, DevCarrierOrderE.前进倒库))
            {
                Track track = PubMaster.Track.GetTrack(track_id);
                if (PubMaster.Goods.ExistStockInTrack(track_id))
                {
                    AddTransWithoutLock(trans.area_id, 0, TransTypeE.上砖侧倒库, trans.goods_id, 0, track_id, track_id, TransStatusE.倒库中, trans.carrier_id, track.line > 0 ? track.line : trans.line);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查轨道是否有其他车辆
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        private bool CheckHaveCarrierInOutTrack(uint carrierid, uint trackid)
        {
            bool isignoresorttask = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

            //1.不允许，则不可以有车
            //2.允许，则不可以有非倒库车
            return (!isignoresorttask && PubTask.Carrier.HaveInTrack(trackid, carrierid))
                    || (isignoresorttask && PubTask.Carrier.CheckHaveCarInTrack(TransTypeE.上砖任务, trackid, carrierid)
                    || (isignoresorttask && ExistSortBackTask(trackid)));
        }

        /// <summary>
        /// 是否存在倒库任务状态为还车回轨的任务
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        private bool ExistSortBackTask(uint trackid)
        {
            return TransList.Exists(c => c.give_track_id == trackid
                                && c.TransStaus == TransStatusE.小车回轨
                                && (c.TransType == TransTypeE.上砖侧倒库 || c.TransType == TransTypeE.倒库任务));
        }


        /// <summary>
        /// 是否存在倒库任务状态为还车回轨的任务
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        private bool ExistSortTask(uint trackid)
        {
            return TransList.Exists(c => c.give_track_id == trackid
                                && (c.TransType == TransTypeE.上砖侧倒库 || c.TransType == TransTypeE.倒库任务));
        }



        /// <summary>
        /// 判断轨道的库存是否能够发送小车执行取砖指令
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        private bool CheckStockIsableToTake(uint carrierid, uint trackid, uint stockid = 0)
        {
            if (stockid == 0)
            {
                stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
            }
            if (stockid == 0) return false;

            //判断运输车是否能进入轨道
            //1.允许倒库的过程中使用同轨道上砖
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask))
            {
                //【使用分割点、不限制使用分割点后的库存】
                //2.判断库存所在位置是否轨道分割点后面
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)
                    && !PubMaster.Dic.IsSwitchOnOff(DicTag.CannotUseUpSplitStock)
                    && PubMaster.Goods.IsStockBehindUpSplitPoint(trackid, stockid))
                {
                    //在分割点后的库存需要
                    return false;
                }

                //3.判断是否存在运输车绑定了该库存
                if (PubTask.Carrier.ExistCarrierBindStock(carrierid, stockid))
                {
                    return false;
                }

                //4.
            }

            return true;
        }

        /// <summary>
        /// 判断轨道是否能继续作业
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        private bool CheckTrackStockStillCanUse(uint carrierid, uint trackid, uint stockid = 0)
        {
            if (stockid == 0)
            {
                stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
            }
            if (stockid == 0) return false;

            //判断运输车是否能进入轨道
            //1.允许倒库的过程中使用同轨道上砖
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask))
            {
                //【使用分割点、限制使用分割点后的库存】
                //2.判断库存所在位置是否轨道分割点后面
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)
                    && PubMaster.Dic.IsSwitchOnOff(DicTag.CannotUseUpSplitStock)
                    && PubMaster.Goods.IsStockBehindUpSplitPoint(trackid, stockid))
                {
                    //在分割点后的库存,则不能进行取货操作
                    return false;
                }

                //3.存在倒库完成的任务
                if (ExistSortBackTask(trackid))
                {
                    return false;
                }

                //4.库存只剩1个，并且有任务在当前轨道
                if (PubMaster.Goods.GetTrackStockCount(trackid) <= 1
                    && ExistSortTask(trackid))
                {
                    return false;
                }

                //5.存在空闲小车停在轨道头
                if (PubTask.Carrier.IsCarrierInTrackBiggerSite(carrierid, trackid))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
