using enums;
using enums.track;
using enums.warning;
using module.goods;
using module.track;
using resource;
using System;
using task.device;
using tool.appconfig;

namespace task.trans.transtask
{
    /// <summary>
    /// 上砖接力任务
    /// </summary>
    public class Out2OutSortTrans : BaseTaskTrans
    {
        public Out2OutSortTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {
            //是否有小车在满砖轨道
            if (PubTask.Carrier.HaveInTrackAndGet(trans.take_track_id, out uint carrierid))
            {
                if (PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    _M.SetStatus(trans, TransStatusE.调度设备, string.Format("轨道内有运输车[ {0} ]可直接使用",
                        PubMaster.Device.GetDeviceName(carrierid)));
                }
                else
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1008, string.Format("运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        PubMaster.Device.GetDeviceName(carrierid),
                        PubMaster.Track.GetTrackName(trans.give_track_id)));
                    #endregion
                    return;
                }
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
            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (_M.HaveTaskSortTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1208, string.Format("存在相同作业轨道的任务，等待任务完成；"));
                #endregion
                return;
            }

            //分配运输车
            if (PubTask.Carrier.AllocateCarrier(trans, out carrierid, out string result)
                && !_M.HaveInCarrier(carrierid))
            {
                _M.SetCarrier(trans, carrierid);
                _M.SetStatus(trans, TransStatusE.移车中);
                return;
            }

            #region 【任务步骤记录】
            if (_M.LogForCarrier(trans, result)) return;
            #endregion
        }

        /// <summary>
        /// 移车中
        /// </summary>
        /// <param name="trans"></param>
        public override void MovingCarrier(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            #region[分配摆渡车]
            //还没有分配取货过程中的摆渡车
            if (track.id != trans.give_track_id
                && trans.take_ferry_id == 0)
            {
                string msg = _M.AllocateFerryToCarrierSort(trans, trans.AllocateFerryType);

                #region 【任务步骤记录】
                if (_M.LogForTakeFerry(trans, msg)) return;
                #endregion
            }
            #endregion

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            switch (track.Type)
            {
                #region[小车在轨道]
                case TrackTypeE.储砖_入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:
                case TrackTypeE.上砖轨道:
                case TrackTypeE.下砖轨道:
                    if (trans.give_track_id == track.id)
                    {
                        if (isftask)
                        {
                            RealseTakeFerry(trans);
                            _M.SetStatus(trans, TransStatusE.倒库中);
                        }
                    }
                    else
                    {
                        if (isload && isftask)
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierGiving(trans);
                            #endregion
                            // 原地放砖
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

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:
                case TrackTypeE.后置摆渡轨道:
                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1408, string.Format("运输车[ {0} ]载着砖无法执行接力任务流程；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        #endregion

                        PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.CarrierLoadSortTask, (ushort)trans.carrier_id, trans.id);
                        return;
                    }
                    PubMaster.Warn.RemoveTaskWarn(WarningTypeE.CarrierLoadSortTask, trans.id);

                    if (isnotload && isftask)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out ferryTraid, out res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                                #endregion
                                return;
                            }

                            Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);
                            ushort limitP = gtrack.is_take_forward ? gtrack.limit_point : gtrack.limit_point_up;
                            if (GetTransferTGpoint(trans.id, gtrack.id, trans.carrier_id, gtrack.rfid_6, gtrack.up_split_point, limitP, 
                                out res, out ushort locTake, out ushort locGive))
                            {
                                // 取砖最小判断脉冲 ±10
                                ushort mindicT = 10;
                                // 小车当前脉冲
                                ushort carSite = PubTask.Carrier.GetCurrentPoint(trans.carrier_id);
                                // 判断当前位置是否是取砖移动范围
                                if (gtrack.is_take_forward ? (carSite > (locTake - mindicT)) : (carSite < (locTake + mindicT)))
                                {
                                    // 小车需先到合适位置 (前进取 -小车要在库存位后至少 10 脉冲；后退取 -小车要在库存位前至少 10 脉冲)
                                    ushort toloc = (ushort)(gtrack.is_take_forward ? (locTake - mindicT) : (locTake + mindicT));
                                    MoveToLoc(gtrack.id, trans.carrier_id, trans.id, toloc);

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierTake(trans, gtrack.id, "取砖前，让小车先到合适位置");
                                    #endregion
                                    return;
                                }
                                else
                                {
                                    // 倒库
                                    MoveToSort(gtrack.id, carrierid, trans.id, locTake, locGive,
                                        string.Format("接力点[ {0} ], 取砖点[ {1} ], 放砖点[ {2} ]", gtrack.up_split_point, locTake, locGive));

                                    #region 【任务步骤记录】
                                    _M.LogForCarrierSortRelay(trans, gtrack.id);
                                    #endregion
                                    return;
                                }
                            }
                            else
                            {
                                // 移到轨道头
                                MoveToLoc(gtrack.id, trans.carrier_id, trans.id, limitP);

                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, gtrack.id, res);
                                #endregion
                            }
                            return;

                        }
                    }

                    break;
                #endregion
            }
        }

        /// <summary>
        /// 倒库中
        /// </summary>
        /// <param name="trans"></param>
        public override void SortingStock(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            // 小车不在本轨道 或 兄弟轨道
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "倒库中的运输车在其他轨道，结束任务");
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            // 【不允许接力-轨道有其他小车】
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask)
                && PubTask.Carrier.HaveInTrackButCarrier(trans.take_track_id, trans.give_track_id, trans.carrier_id, out carrierid))
            {
                // 不接力 - 终止且报警
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1608, string.Format("检测到接力轨道中有其他运输车[ {0} ], 强制终止接力运输车[ {1} ]",
                    PubMaster.Device.GetDeviceName(carrierid),
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion

                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, "接力中相关任务轨道出现其他运输车");

                PubMaster.Warn.AddDevWarn(trans.area_id, trans.line, WarningTypeE.HaveOtherCarrierInSortTrack,
                    (ushort)trans.carrier_id, trans.id, trans.take_track_id, carrierid);

                return;
            }
            PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);

            #region 通用轨道
            if (track.Type == TrackTypeE.储砖_出入 && isftask)
            {
                #region 基础判断
                // 安全距离
                ushort safe = PubMaster.Goods.GetStackSafe(trans.goods_id, trans.carrier_id);
                // 极限位置
                ushort limit = track.is_take_forward ? track.limit_point : track.limit_point_up;
                // 倒库位置
                ushort takeSite = 0, giveSite = 0;
                // 小车当前脉冲
                ushort carSite = PubTask.Carrier.GetCurrentPoint(trans.carrier_id);
                // 目标库存
                Stock targetSTK = null;
                if (isload)
                {
                    // 小车当前位置前
                    targetSTK = PubMaster.Goods.GetStockInfrontStockPoint(track.id, carSite);
                }
                else
                {
                    // 接力点前
                    targetSTK = PubMaster.Goods.GetStockInfrontStockPoint(track.id, track.up_split_point);
                }

                if (targetSTK != null)
                {
                    giveSite = (ushort)(track.is_take_forward ? (targetSTK.location + safe) : (targetSTK.location - safe));
                }
                else
                {
                    giveSite = limit;
                }

                if (giveSite == 0)
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 4008, string.Format("无放砖位置"));
                    #endregion
                    return;
                }
                #endregion

                #region 载砖-找地方放
                if (isload)
                {
                    // 放砖最小判断脉冲 ±20
                    ushort mindicG = 20;
                    // 判断位置
                    if (track.is_give_back ? (giveSite > (carSite - mindicG)) : (giveSite < (carSite + mindicG)))
                    {
                        // 原地放砖
                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.放砖指令
                        });
                    }
                    else
                    {
                        MoveToGive(track.id, trans.carrier_id, trans.id, giveSite);
                    }

                    #region 【任务步骤记录】
                    _M.LogForCarrierGive(trans, track.id);
                    #endregion
                    return;
                }
                #endregion

                #region 无砖-倒库判断
                if (isnotload)
                {
                    // 判断位置是否超接力点
                    if (track.is_take_forward ? (giveSite > track.up_split_point) : (giveSite < track.up_split_point))
                    {
                        _M.SetStatus(trans, TransStatusE.接力等待, "已接力放满");
                        return;
                    }

                    // 无取砖位置 判断是否结束
                    if (!GetTransferTakePoint(track.id, track.rfid_6, track.up_split_point, out takeSite))
                    {
                        // 砖机使用该品种
                        if (PubMaster.DevConfig.IsHaveSameTileNowGood(track.area, trans.goods_id, trans.level, TileWorkModeE.上砖))
                        {
                            _M.SetStatus(trans, TransStatusE.接力等待, "无库存接力");
                        }
                        else
                        {
                            _M.SetStatus(trans, TransStatusE.小车回轨, "当前没有砖机再上该品种");
                        }

                        return;
                    }

                    // 取砖最小判断脉冲 ±10
                    ushort mindicT = 10;
                    // 判断当前位置是否是取砖移动范围
                    if (track.is_take_forward ? (carSite > (takeSite - mindicT)) : (carSite < (takeSite + mindicT)))
                    {
                        // 小车需先到合适位置 (前进取 -小车要在库存位后至少 10 脉冲；后退取 -小车要在库存位前至少 10 脉冲)
                        ushort toloc = (ushort)(track.is_take_forward ? (takeSite - mindicT) : (takeSite + mindicT));
                        MoveToLoc(track.id, trans.carrier_id, trans.id, toloc);

                        #region 【任务步骤记录】
                        _M.LogForCarrierTake(trans, track.id, "取砖前，让小车先到合适位置");
                        #endregion
                        return;
                    }
                    else
                    {
                        // 倒库
                        MoveToSort(track.id, carrierid, trans.id, takeSite, giveSite,
                            string.Format("接力点[ {0} ], 取砖点[ {1} ], 放砖点[ {2} ]", track.up_split_point, takeSite, giveSite));

                        #region 【任务步骤记录】
                        _M.LogForCarrierSortRelay(trans, track.id);
                        #endregion
                        return;
                    }

                }
                #endregion

                return;
            }
            #endregion

            #region 出&入轨道 - V2.1 停用
            //if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出))
            //{
            //    //在入轨道
            //    if (track.brother_track_id == trans.take_track_id)
            //    {
            //        if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
            //        {
            //            #region 【任务步骤记录】
            //            _M.SetStepLog(trans, false, 1308, string.Format("运输车[ {0} ], 接力不允许取入库轨道砖,系统自动终止；",
            //                PubMaster.Device.GetDeviceName(trans.carrier_id)));
            //            #endregion

            //            PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.CarrierLoadNotSortTask, (ushort)trans.carrier_id, trans.id);

            //            //终止
            //            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //            {
            //                Order = DevCarrierOrderE.终止指令
            //            }, "接力不允许取入库轨道砖");
            //            return;
            //        }

            //        bool iscarfree = PubTask.Carrier.IsCarrierFree(trans.carrier_id);
            //        if (iscarfree)
            //        {
            //            Track tra = PubMaster.Track.GetTrack(trans.give_track_id);
            //            MoveToLoc(tra.id, trans.carrier_id, trans.id, (ushort)(tra.split_point + 100), "接力不允许取入库轨道砖, 定位回去一点位置");
            //        }

            //        if (!iscarfree && PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.定位指令))
            //        {
            //            _M.SetStatus(trans, TransStatusE.接力等待, "接力不允许取入库轨道砖, 定位回去一点位置");
            //        }
            //        return;
            //    }

            //    // 小车空闲状态
            //    if (isftask)
            //    {
            //        // 有砖-前进放砖
            //        if (isload)
            //        {
            //            #region 【任务步骤记录】
            //            _M.LogForCarrierGive(trans, trans.give_track_id);
            //            #endregion

            //            //前进放砖
            //            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //            {
            //                Order = DevCarrierOrderE.放砖指令,
            //                CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
            //                ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
            //                ToTrackId = trans.give_track_id
            //            });

            //            return;
            //        }

            //        // 无砖 - 回出库轨道头
            //        if (isnotload)
            //        {
            //            //后退等待 -> 进入接力等待
            //            bool dowait = false;

            //            //倒库完成最后一车，同时轨道只剩一车库存
            //            ushort trackstockcount = PubMaster.Goods.GetTrackStockCount(trans.give_track_id);

            //            PubTask.Carrier.GetCarrierNowUnloadPoint(trans.carrier_id, out ushort nowpoint, out ushort givepoint);

            //            //接力脉冲后的库存数量
            //            int behindstockcount = PubMaster.Goods.GetBehindPointStockCount(trans.give_track_id, nowpoint);

            //            //分段接力
            //            bool separesort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpSortUseMaxNumber) && (0 != PubMaster.Area.GetLineUpSortMaxNumber(track.area, track.line));

            //            //分段接力接力完成有库存，则后退等待全部库存出完
            //            if (separesort && trackstockcount >= 1)
            //            {
            //                dowait = true;
            //            }

            //            //砖机不再使用该品种
            //            if (behindstockcount <= 0 && !PubMaster.DevConfig.IsHaveSameTileNowGood(trans.goods_id, TileWorkModeE.上砖))
            //            {
            //                _M.LogForCarrierSort(trans, trans.give_track_id, "当前没有砖机再上该品种");
            //                dowait = false;
            //            }

            //            if (dowait)
            //            {
            //                //运输车等待的时候需要后退几个车身
            //                ushort carspace = GlobalWcsDataConfig.BigConifg.GetSortWaitNumberCarSpace(trans.area_id, trans.line);

            //                ushort topoint;
            //                if (nowpoint == 0 || givepoint == 0)
            //                {
            //                    topoint = track.split_point;
            //                    //return;
            //                }
            //                else
            //                {
            //                    Stock bestock_one = null;

            //                    if (bestock_one != null)
            //                    {
            //                        topoint = bestock_one.location;
            //                        topoint += (ushort)(carspace * PubMaster.Goods.GetStackSafe(0, 0));
            //                    }
            //                    else
            //                    {
            //                        topoint = givepoint;
            //                        topoint -= (ushort)(carspace * PubMaster.Goods.GetStackSafe(0, 0));
            //                    }
            //                }

            //                //需要定位的位置比出轨道最后取货点都小则用
            //                if (track.Type == TrackTypeE.储砖_出入)
            //                {
            //                    if (topoint <= track.limit_point)
            //                    {
            //                        topoint = track.limit_point;
            //                    }
            //                }
            //                else
            //                {
            //                    if (topoint <= track.split_point)
            //                    {
            //                        topoint = track.split_point;
            //                    }
            //                }

            //                if (Math.Abs(nowpoint - topoint) <= 100)
            //                {
            //                    _M.SetStatus(trans, TransStatusE.接力等待,
            //                        string.Format("轨道有库存[ {0} ], 需接力库存[ {1} ]", trackstockcount, behindstockcount));
            //                    return;
            //                }

            //                //定位到当前卸货位置或当前位置往后两个车位
            //                MoveToLoc(track.id, trans.carrier_id, trans.id, topoint,
            //                    string.Format("接力等待后退[ {0} ]车身的位置，轨道有库存[ {1} ], 需接力库存[ {2} ]", carspace, trackstockcount, behindstockcount));
            //            }
            //            else
            //            {
            //                _M.SetStatus(trans, TransStatusE.小车回轨,
            //                    string.Format("轨道有库存[ {0} ], 需接力库存[ {1} ]", trackstockcount, behindstockcount));
            //            }
            //        }
            //    }

            //    return;
            //}

            #endregion

        }

        /// <summary>
        /// 运输车回轨
        /// </summary>
        /// <param name="trans"></param>
        public override void ReturnCarrrier(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            // 小车不在本轨道
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "倒库中的运输车在其他轨道，结束任务");
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            #region 通用轨道
            if (track.Type == TrackTypeE.储砖_出入)
            {
                // 安全距离
                ushort safe = PubMaster.Goods.GetStackSafe(trans.goods_id, trans.carrier_id);
                // 极限位置
                ushort limit = track.is_take_forward ? track.limit_point : track.limit_point_up;
                // 小车当前脉冲
                ushort carSite = PubTask.Carrier.GetCurrentPoint(trans.carrier_id);

                if (isload)
                {
                    if (isftask)
                    {
                        // 原地放砖
                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.放砖指令
                        });
                    }
                    else
                    {
                        //终止
                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.终止指令
                        }, "倒库回轨流程保证小车无砖，终止载砖小车所有动作");
                    }

                    #region 【任务步骤记录】
                    _M.LogForCarrierGiving(trans);
                    #endregion

                    return;
                }

                if (isnotload)
                {
                    // 运输车等待的时候需要后退几个车身
                    ushort carspace = GlobalWcsDataConfig.BigConifg.GetSortWaitNumberCarSpace(trans.area_id, trans.line);
                    carspace = (ushort)(carspace * safe);

                    // 有车阻碍
                    if (PubTask.Carrier.ExistLocateObstruct(trans.carrier_id, trans.give_track_id, carSite, track.is_take_forward, out CarrierTask otherCar))
                    {
                        // 终止小车
                        if (!isftask)
                        {
                            //终止
                            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                            {
                                Order = DevCarrierOrderE.终止指令
                            }, string.Format("前方存在其他运输车[ {0} ]", otherCar.Device.name));

                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 1708, string.Format("检测到前方有运输车[ {0} ]，[ {1} ]停止动作；",
                                otherCar.Device.name, PubMaster.Device.GetDeviceName(trans.carrier_id)));
                            #endregion

                            return;
                        }

                        // 无移动
                        if (otherCar.TargetPoint == 0)
                        {
                            // 是否绑定任务
                            if (_M.HaveCarrierInTrans(otherCar.ID))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1808, string.Format("检测到前方有运输车[ {0} ]绑定有任务，等待其任务完成；",
                                    otherCar.Device.name));
                                #endregion
                                return;
                            }

                            if (!PubTask.Carrier.IsCarrierFree(otherCar.ID))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1908, string.Format("检测到前方有运输车[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                    otherCar.Device.name));
                                #endregion
                                return;
                            }

                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 2008, string.Format("检测到前方有运输车[ {0} ]，尝试对其生成移车任务；",
                                    otherCar.Device.name));
                            #endregion

                            //转移到同类型轨道
                            _M.AddMoveCarrierTask(track.id, otherCar.ID, track.Type, MoveTypeE.转移占用轨道);
                            return;
                        }

                        // 移动阻碍
                        if (otherCar.TargetPoint > 0)
                        {
                            // 判断 待命位
                            ushort overpoint = (ushort)(track.is_take_forward ? (otherCar.TargetPoint + carspace) : (otherCar.TargetPoint - carspace));
                            if (track.is_take_forward ? (overpoint > track.rfid_6) : (overpoint < track.rfid_6))
                            {
                                overpoint = track.rfid_6;
                            }
                            if (Math.Abs(carSite - overpoint) >= 100)
                            {
                                MoveToLoc(track.id, trans.carrier_id, trans.id, overpoint);

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 2108, string.Format("检测到前方有运输车[ {0} ]正在移动，控制[ {1} ]进行避让；",
                                    otherCar.Device.name, PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion
                            }

                            return;
                        }

                    }

                    // 回轨道头
                    if (isftask)
                    {
                        if (Math.Abs(carSite - limit) >= 20)
                        {
                            MoveToLoc(track.id, trans.carrier_id, trans.id, limit);

                            #region 【任务步骤记录】
                            _M.LogForCarrierToTrack(trans, track.id, "回到出库侧轨道头");
                            #endregion
                            return;
                        }
                        else
                        {
                            _M.SetStatus(trans, TransStatusE.完成);
                            return;
                        }
                    }

                }

            }
            #endregion

            #region 出&入 轨道 - V2.1 停用
            //if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出))
            //{
            //    #region【接力倒库完成后，如果前面有车有任务，则先定位到后一个位置】
            //    ushort _topoint = 0;
            //    PubTask.Carrier.GetCarrierNowUnloadPoint(trans.carrier_id, out ushort _nowpoint, out ushort _givepoint);
            //    bool localneed = true; //需要定位到下一个位置
            //    bool dolocate = false;//是否需要定位
            //    bool iscarrierferr = PubTask.Carrier.IsCarrierFree(trans.carrier_id);
            //    if (_nowpoint == 0 || _givepoint == 0)
            //    {
            //        _topoint = track.split_point;
            //    }
            //    else
            //    {
            //        _topoint = _givepoint;
            //        _topoint -= (ushort)(2 * PubMaster.Goods.GetStackSafe(0, 0));
            //    }

            //    //需要定位的位置比出轨道最后取货点都小则用
            //    if (_topoint <= track.split_point)
            //    {
            //        _topoint = track.split_point;
            //    }

            //    if (Math.Abs(_nowpoint - _topoint) <= 100)
            //    {
            //        localneed = false;
            //    }
            //    #endregion

            //    // 任务运输车前面即将有车
            //    if (PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
            //    {
            //        //需要定位，并且分割点前有库存 => 才需要定位到下一个位置
            //        if (localneed && PubMaster.Goods.ExistInfrontUpSplitPoint(track.id, track.up_split_point, out int stkcount))
            //        {
            //            dolocate = true;
            //        }
            //        else
            //        {
            //            if (!iscarrierferr)
            //            {
            //                //终止
            //                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //                {
            //                    Order = DevCarrierOrderE.终止指令
            //                }, "前方有其他运输车将至");

            //                #region 【任务步骤记录】
            //                _M.SetStepLog(trans, false, 1708, string.Format("终止运输车[ {0} ]，检测到前方可能有其他运输车进入轨道",
            //                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
            //                #endregion
            //            }

            //            return;
            //        }
            //    }

            //    // 任务运输车前面有车
            //    if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
            //    {
            //        bool intrans = _M.HaveCarrierInTrans(othercarrier);

            //        //上砖车有任务，接力需要定位，并且分割点前有库存 => 才需要定位到下一个位置
            //        if ((intrans || localneed) && PubMaster.Goods.ExistInfrontUpSplitPoint(track.id, track.up_split_point, out int stkcount))
            //        {
            //            dolocate = true;
            //        }
            //        else
            //        {
            //            if (!iscarrierferr)
            //            {
            //                //判断othercar的方向跟接力车的方向是否一致，一致则不用发终止，否则发终止
            //                if (PubTask.Carrier.IsCollision(trans.carrier_id, othercarrier))
            //                {
            //                    //终止
            //                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //                    {
            //                        Order = DevCarrierOrderE.终止指令
            //                    }, string.Format("前方存在其他运输车[ {0} ]", PubMaster.Device.GetDeviceName(othercarrier)));
            //                }
            //            }

            //            if (intrans)
            //            {
            //                #region 【任务步骤记录】
            //                _M.SetStepLog(trans, false, 1808, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]绑定有任务，等待其任务完成；",
            //                    PubMaster.Device.GetDeviceName(trans.carrier_id),
            //                    PubMaster.Device.GetDeviceName(othercarrier)));
            //                #endregion
            //                return;
            //            }

            //            if (!PubTask.Carrier.IsCarrierFree(othercarrier))
            //            {
            //                #region 【任务步骤记录】
            //                _M.SetStepLog(trans, false, 1908, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
            //                    PubMaster.Device.GetDeviceName(trans.carrier_id),
            //                    PubMaster.Device.GetDeviceName(othercarrier)));
            //                #endregion
            //                return;
            //            }

            //            #region 【任务步骤记录】
            //            _M.SetStepLog(trans, false, 2008, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]，尝试对其生成移车任务；",
            //                    PubMaster.Device.GetDeviceName(trans.carrier_id),
            //                    PubMaster.Device.GetDeviceName(othercarrier)));
            //            #endregion

            //            //转移到同类型轨道
            //            TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
            //            track = PubTask.Carrier.GetCarrierTrack(othercarrier);
            //            _M.AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
            //            return;
            //        }
            //    }

            //    if (dolocate && _topoint != 0)
            //    {
            //        if (!iscarrierferr)
            //        {
            //            return;
            //        }

            //        if (Math.Abs(_nowpoint - _topoint) >= 100)
            //        {
            //            //定位到当前卸货位置或当前位置往后两个车位
            //            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //            {
            //                Order = DevCarrierOrderE.定位指令,
            //                CheckTra = track.ferry_down_code,
            //                ToPoint = _topoint,
            //            }, "接力后退两个位置避让取砖");
            //        }
            //        return;
            //    }

            //    // 任务运输车回到出库轨道头
            //    if (isftask
            //        && (trans.give_track_id == track.brother_track_id || trans.give_track_id == track.id)
            //        && !PubTask.Carrier.IsCarrierInTrackBiggerRfID2(trans.carrier_id, trans.give_track_id))
            //    {
            //        #region 【任务步骤记录】
            //        _M.LogForCarrierToTrack(trans, trans.give_track_id);
            //        #endregion

            //        // 前进至点
            //        MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点, "接力完成返回出轨道");
            //        return;
            //    }

            //    // 完成？
            //    if (isftask
            //        && track.id == trans.give_track_id
            //        && PubTask.Carrier.IsCarrierInTrackBiggerRfID2(trans.carrier_id, trans.give_track_id))
            //    {
            //        _M.SetStatus(trans, TransStatusE.完成);
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public override void FinishStockTrans(StockTrans trans)
        {
            PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);
            _M.SetFinish(trans);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            if (trans.carrier_id == 0)
            {
                _M.SetStatus(trans, TransStatusE.完成, "取消流程，当前没有运输车，结束任务");
                return;
            }
            _M.SetStatus(trans, TransStatusE.小车回轨, "取消流程，让运输车回轨道头！");
        }

        /// <summary>
        /// 倒库暂停
        /// </summary>
        /// <param name="trans"></param>
        public override void SortTaskWait(StockTrans trans)
        {
            if (trans.carrier_id != 0)
            {
                // 运行前提
                if (!_M.RunPremise(trans, out track))
                {
                    return;
                }

                #region 通用轨道
                if (track.Type == TrackTypeE.储砖_出入)
                {
                    if (!PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                    {
                        //终止
                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.终止指令
                        }, "接力暂停，终止并解绑小车");
                        return;
                    }
                    else
                    {
                        // 极限位置
                        ushort limit = track.is_take_forward ? track.limit_point : track.limit_point_up;
                        // 小车当前脉冲
                        ushort carSite = PubTask.Carrier.GetCurrentPoint(trans.carrier_id);

                        if (Math.Abs(carSite - limit) >= 20)
                        {
                            if (!PubTask.Carrier.ExistLocateObstruct(trans.carrier_id, trans.give_track_id, carSite, track.is_take_forward, out CarrierTask otherCar))
                            {
                                #region 【任务步骤记录】
                                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                                #endregion

                                //前进至点
                                MoveToLoc(trans.give_track_id, trans.carrier_id, trans.id, limit);
                                return;
                            }
                        }
                        else
                        {
                            _M.SetCarrier(trans, 0, string.Format("接力暂停，释放小车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        }
                        return;
                    }
                }
                #endregion

                #region 出&入 轨道 - V2.1 停用
                //if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出))
                //{
                //    //倒库中的小车卸完货并且后退中
                //    //出库轨道中没有车辆在前面
                //    if (PubTask.Carrier.IsCarrierUnLoadAndBackWard(trans.carrier_id)
                //        && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id))
                //    {
                //        #region 【任务步骤记录】
                //        _M.LogForCarrierToTrack(trans, trans.give_track_id);
                //        #endregion

                //        //前进至点
                //        MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                //        return;
                //    }

                //    if (PubTask.Carrier.IsCarrierInTrackBiggerRfID1(trans.carrier_id, trans.give_track_id))
                //    {
                //        _M.SetCarrier(trans, 0, string.Format("倒库任务暂停，释放小车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                //        return;
                //    }
                //}
                #endregion

            }
        }

        /// <summary>
        /// 接力等待
        /// </summary>
        /// <param name="trans"></param>
        public override void Out2OutRelayWait(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            if (isload || PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.倒库指令))
            {
                _M.SetStatus(trans, TransStatusE.倒库中);
                return;
            }

            #region 通用轨道
            if (track.Type == TrackTypeE.储砖_出入 && isftask)
            {
                // 判断是否开始接力
                bool doangin = false;
                // 小车当前脉冲
                ushort carSite = PubTask.Carrier.GetCurrentPoint(trans.carrier_id);
                // 接力点前库存
                bool haveStkTo = PubMaster.Goods.ExistInfrontUpSplitPoint(track.id, track.up_split_point, out int stkCountTo);

                //轨道是否有干扰车
                if (PubTask.Carrier.ExistLocateObstruct(trans.carrier_id, trans.give_track_id, carSite, track.is_take_forward, out CarrierTask otherCar))
                {
                    #region 移至待命点
                    ushort topoint = GetTransferWaitPoint(track.id, trans.carrier_id, track.rfid_6, track.up_split_point);
                    // 是否移动
                    if (Math.Abs(carSite - topoint) >= 100) // 100 脉冲范围
                    {
                        MoveToLoc(track.id, trans.carrier_id, trans.id, topoint);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, track.id, "接力待命");
                        #endregion
                        return;
                    }
                    #endregion

                    if (stkCountTo == 1)
                    {
                        Stock topstock = PubMaster.Goods.GetStockForOut(track.id);
                        if (topstock != null && PubMaster.DevConfig.IsCarrierBindStock(otherCar.ID, topstock.id))
                        {
                            res = "接力点前只有一车砖，同时有车取砖取了它，继续接力";
                            doangin = true;
                        }
                    }

                }
                else
                {
                    #region 小车回轨
                    if (IsTransferOver(track.id, track.rfid_6, track.up_split_point))
                    {
                        _M.SetStatus(trans, TransStatusE.小车回轨, "轨道无库存");
                        return;
                    }

                    if (!PubMaster.DevConfig.IsHaveSameTileNowGood(track.area, trans.goods_id, trans.level, TileWorkModeE.上砖))
                    {
                        _M.SetStatus(trans, TransStatusE.小车回轨, "当前没有砖机再上该品种");
                        return;
                    }
                    #endregion

                    if (!haveStkTo)
                    {
                        res = "接力点前无库存，继续接力";
                        doangin = true;
                    }

                }

                // 继续接力
                if (doangin)
                {
                    _M.SetStatus(trans, TransStatusE.倒库中, res);
                    return;
                }

            }
            #endregion

            #region 出&入轨道 - V2.1 停用
            //if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出))
            //{
            //    if (track == null || track.id != trans.give_track_id)
            //    {
            //        track = PubMaster.Track.GetTrack(trans.give_track_id);
            //    }
            //    ushort stockqty = PubMaster.Goods.GetTrackStockCount(track.id);

            //    //当前车是否空闲
            //    bool carrierfree = PubTask.Carrier.IsCarrierFree(trans.carrier_id);
            //    if (!carrierfree) return;

            //    //轨道存在其他车
            //    bool havecarinfront = PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id, out uint othercarid);

            //    //使用运输车当前所在脉冲进行计算后面需要接力的库存
            //    PubTask.Carrier.GetCarrierNowUnloadPoint(trans.carrier_id, out ushort nowpoint, out ushort givepoint);

            //    //接力点前的库存数
            //    int infrontstockcount = PubMaster.Goods.GetInfrontPointStockCount(track.id, nowpoint);

            //    #region[继续接力]

            //    //接力点前没有库存，同时没车，则开始接力
            //    bool upnonestock = false;
            //    if (infrontstockcount == 0
            //        && !havecarinfront
            //        && stockqty > 0)
            //    {
            //        upnonestock = true;
            //    }

            //    //接力点前只有一车砖，同时有车取砖取了它
            //    bool onestockcarloadit = false;
            //    Stock topstock = PubMaster.Goods.GetStockForOut(track.id);
            //    if (infrontstockcount == 1
            //        && havecarinfront
            //        && stockqty > 1)
            //    {
            //        if (topstock != null && topstock.location > nowpoint
            //            && PubMaster.DevConfig.IsCarrierBindStock(othercarid, topstock.id))
            //        {
            //            onestockcarloadit = true;
            //        }
            //    }


            //    //砖机不再使用该品种
            //    bool nonetileusegood = false;
            //    uint gid = topstock.goods_id;
            //    if (gid == 0)
            //    {
            //        gid = trans.goods_id;
            //    }
            //    if (!PubMaster.DevConfig.IsHaveSameTileNowGood(gid, TileWorkModeE.上砖))
            //    {
            //        nonetileusegood = true;
            //        _M.LogForCarrierSort(trans, trans.give_track_id, "当前没有砖机再上该品种");
            //    }

            //    bool need = PubMaster.Goods.ExistBehindUpSplitPoint(track.id, nowpoint, out int stkcount);

            //    //前面没有库存，继续倒库
            //    if (upnonestock || onestockcarloadit || nonetileusegood)
            //    {
            //        if (need)
            //        {
            //            byte movecount = (byte)PubMaster.Goods.GetBehindPointStockCount(track.id, nowpoint);
            //            byte line_max_move = PubMaster.Area.GetLineUpSortMaxNumber(track.area, track.line);

            //            if (line_max_move > 0 && movecount > line_max_move)
            //            {
            //                movecount = line_max_move;
            //            }

            //            ushort tpoint = track.split_point;
            //            if (track.Type == TrackTypeE.储砖_出)
            //            {
            //                tpoint -= 50;
            //            }

            //            //后退至轨道倒库
            //            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //            {
            //                Order = DevCarrierOrderE.往前倒库,
            //                CheckTra = track.ferry_down_code,
            //                ToPoint = tpoint,
            //                MoveCount = movecount,
            //                ToTrackId = track.id
            //            }, string.Format("轨道有库存[ {0} ], 接力数量[ {1} ], 接力脉冲[ {2} ]", stockqty, movecount, nowpoint));
            //            return;
            //        }
            //    }

            //    #endregion

            //    #region[前进至点  / 替换任务执行 入库到出库的倒库]

            //    if ((stockqty == 0 || nonetileusegood) && carrierfree && !havecarinfront)
            //    {
            //        _M.SetStatus(trans, TransStatusE.小车回轨, string.Format("轨道无库存，接力运输车回轨"));
            //    }
            //    #endregion
            //}
            #endregion

        }

        #region[其他流程]

        /// <summary>
        /// 取货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToTakeTrackTakeStock(StockTrans trans)
        {
        }

        /// <summary>
        /// 放货流程
        /// </summary>
        /// <param name="trans"></param>
        public override void ToGiveTrackGiveStock(StockTrans trans)
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
