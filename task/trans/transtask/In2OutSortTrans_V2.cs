using enums;
using enums.track;
using enums.warning;
using module.goods;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using task.device;
using tool.appconfig;

namespace task.trans.transtask
{
    public class In2OutSortTrans_V2 : BaseTaskTrans
    {
        public In2OutSortTrans_V2(TransMaster trans) : base(trans)
        {

        }


        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {
            bool havedifcaringive = true, havecarintake = true;

            //转移卸货轨道不符合的运输车
            if (!CheckTrackAndAddMoveTask(trans, trans.give_track_id))
            {
                havedifcaringive = false;
            }
            
            //是否有小车在入库侧轨道
            if (CheckCarAndAddMoveTask(trans, trans.take_track_id, true))
            {
                havecarintake = false;
            }

            if (!havecarintake && !havedifcaringive)
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
                _M.SetStepLog(trans, false, 1302, string.Format("存在相同作业轨道的任务，等待任务完成；"));
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
                string msg = _M.AllocateFerryToCarrierSort(trans, DeviceTypeE.前摆渡);

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
                #region[小车在储砖轨道]
                case TrackTypeE.储砖_入:
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:
                case TrackTypeE.上砖轨道:
                case TrackTypeE.下砖轨道:
                    if (trans.give_track_id == track.id || trans.take_track_id == track.id)
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
                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1402, string.Format("运输车[ {0} ]载着砖无法执行倒库任务流程；",
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
                            ushort limitP, overP;
                            if (gtrack.is_take_forward)
                            {
                                limitP = gtrack.limit_point;
                                overP = gtrack.limit_point_up;
                            }
                            else
                            {
                                limitP = gtrack.limit_point_up;
                                overP = gtrack.limit_point;
                            }

                            if (GetTransferTGpoint(trans.id, gtrack.id, trans.carrier_id, overP, gtrack.split_point, limitP,
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
                                        string.Format("分界点[ {0} ], 取砖点[ {1} ], 放砖点[ {2} ]", gtrack.split_point, locTake, locGive));

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

            // 小车不在本轨道
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.调度设备, "倒库中的运输车在其他轨道，尝试重新分配");
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            isftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

            // 【不允许接力 && 轨道有其他小车】
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask)
                && PubTask.Carrier.HaveInTrackButCarrier(trans.take_track_id, trans.give_track_id, trans.carrier_id, out carrierid))
            {
                // 不接力 - 终止且报警
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1602, string.Format("检测到倒库轨道中有其他运输车[ {0} ], 强制终止倒库运输车[ {1} ]",
                    PubMaster.Device.GetDeviceName(carrierid),
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion

                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, "倒库中相关任务轨道出现其他运输车");

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
                // 极限/结束 位置
                ushort limitP, overP;
                if (track.is_take_forward)
                {
                    limitP = track.limit_point;
                    overP = track.limit_point_up;
                }
                else
                {
                    limitP = track.limit_point_up;
                    overP = track.limit_point;
                }

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
                    // 分界点前
                    targetSTK = PubMaster.Goods.GetStockInfrontStockPoint(track.id, track.split_point);
                }

                if (targetSTK != null)
                {
                    giveSite = (ushort)(track.is_take_forward ? (targetSTK.location + safe) : (targetSTK.location - safe));
                }
                else
                {
                    giveSite = limitP;
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
                    // 无取砖位置 判断是否结束
                    if (!GetTransferTakePoint(track.id, overP, track.up_split_point, out takeSite))
                    {
                        _M.SetStatus(trans, TransStatusE.小车回轨);
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
                            string.Format("分界点[ {0} ], 取砖点[ {1} ], 放砖点[ {2} ]", track.split_point, takeSite, giveSite));

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
            //if (isftask)
            //{
            //    // 有砖-前进放砖
            //    if (isload)
            //    {
            //        #region 【任务步骤记录】
            //        _M.LogForCarrierGive(trans, trans.give_track_id);
            //        #endregion

            //        MoveToGive(trans.give_track_id, trans.carrier_id, trans.id, PubMaster.Track.GetTrackLimitPointOut(trans.give_track_id));
            //        return;
            //    }

            //    // 无砖 - 回出库轨道头
            //    if (isnotload)
            //    {
            //        _M.SetStatus(trans, TransStatusE.小车回轨);
            //        return;
            //    }
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
                // 极限/结束 位置
                ushort limitP, overP;
                if (track.is_take_forward)
                {
                    limitP = track.limit_point;
                    overP = track.limit_point_up;
                }
                else
                {
                    limitP = track.limit_point_up;
                    overP = track.limit_point;
                }

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
                                _M.SetStepLog(trans, false, 1802, string.Format("检测到前方有运输车[ {0} ]绑定有任务，等待其任务完成；",
                                    otherCar.Device.name));
                                #endregion
                                return;
                            }

                            if (!PubTask.Carrier.IsCarrierFree(otherCar.ID))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1902, string.Format("检测到前方有运输车[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                    otherCar.Device.name));
                                #endregion
                                return;
                            }

                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 2002, string.Format("检测到前方有运输车[ {0} ]，尝试对其生成移车任务；",
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
                            if (track.is_take_forward ? (overpoint > overP) : (overpoint < overP))
                            {
                                overpoint = overP;
                            }
                            if (Math.Abs(carSite - overpoint) >= 100)
                            {
                                MoveToLoc(track.id, trans.carrier_id, trans.id, overpoint);

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 2102, string.Format("检测到前方有运输车[ {0} ]正在移动，控制[ {1} ]进行避让；",
                                    otherCar.Device.name, PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion
                            }

                            return;
                        }

                    }

                    // 回轨道头
                    if (isftask)
                    {
                        if (Math.Abs(carSite - limitP) >= 20)
                        {
                            MoveToLoc(track.id, trans.carrier_id, trans.id, limitP);

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

            #region region 出&入 轨道 - V2.1 停用
            //if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
            //{
            //    _M.SetStatus(trans, TransStatusE.倒库中);
            //    PubMaster.Warn.RemoveTaskWarn(WarningTypeE.SortFinishButDownExistStock, trans.id);
            //}

            //// 任务运输车前面即将有车
            //if (PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
            //{
            //    //终止
            //    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //    {
            //        Order = DevCarrierOrderE.终止指令
            //    }, "前方有其他运输车将至");

            //    #region 【任务步骤记录】
            //    _M.SetStepLog(trans, false, 1702, string.Format("终止运输车[ {0} ]，检测到前方可能有其他运输车进入轨道",
            //        PubMaster.Device.GetDeviceName(trans.carrier_id)));
            //    #endregion

            //    return;
            //}

            //// 任务运输车前面有车
            //if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
            //{
            //    string othername = PubMaster.Device.GetDeviceName(othercarrier);
            //    string carname = PubMaster.Device.GetDeviceName(trans.carrier_id);

            //    //终止
            //    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            //    {
            //        Order = DevCarrierOrderE.终止指令
            //    }, string.Format("前方存在其他运输车[ {0} ] ", othername));

            //    if (_M.HaveCarrierInTrans(othercarrier))
            //    {
            //        #region 【任务步骤记录】
            //        _M.SetStepLog(trans, false, 1802, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]绑定有任务，等待其任务完成；",
            //            carname,
            //            othername));
            //        #endregion
            //        return;
            //    }

            //    if (!PubTask.Carrier.IsCarrierFree(othercarrier))
            //    {
            //        #region 【任务步骤记录】
            //        _M.SetStepLog(trans, false, 1902, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
            //            carname,
            //            othername));
            //        #endregion
            //        return;
            //    }

            //    #region 【任务步骤记录】
            //    _M.SetStepLog(trans, false, 2002, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]，尝试对其生成移车任务；",
            //            carname,
            //            othername));
            //    #endregion

            //    //转移到同类型轨道
            //    TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
            //    track = PubTask.Carrier.GetCarrierTrack(othercarrier);
            //    _M.AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
            //    return;
            //}

            //// 任务运输车回到出库轨道头
            //if (isftask
            //    && (trans.take_track_id == track.id || trans.give_track_id == track.id)
            //    && !PubTask.Carrier.IsCarrierInTrackBiggerRfID2(trans.carrier_id, trans.give_track_id))
            //{
            //    #region 【任务步骤记录】
            //    _M.LogForCarrierToTrack(trans, trans.give_track_id);
            //    #endregion

            //    // 前进至点
            //    MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
            //    return;
            //}

            //// 完成？
            //if (isftask
            //    && track.id == trans.give_track_id
            //    && PubTask.Carrier.IsCarrierInTrackBiggerRfID2(trans.carrier_id, trans.give_track_id))
            //{
            //    // 入库侧仍还有库存
            //    if (PubMaster.Track.IsTrackType(trans.take_track_id, TrackTypeE.储砖_入)
            //        && PubMaster.Goods.ExistStockInTrack(trans.take_track_id))
            //    {
            //        //_M.SetStatus(trans, TransStatusE.移车中, "入库侧还有库存没倒完，重新发起倒库指令");
            //        //报警运输车倒库后入库轨道还有库存，请在核实并修改入库轨道的库存后，1.如果需要继续倒库，请手动给运输车发倒库任务，2.如果不需要继续倒库，请取消当前轨道的倒库任务并修改轨道状态为空砖/有砖
            //        PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.SortFinishButDownExistStock, (ushort)trans.carrier_id, trans.id);
            //        return;
            //    }

            //    _M.SetStatus(trans, TransStatusE.完成);
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
            //PubMaster.Track.GetAndRefreshUpCount(trans.give_track_id);
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
                //倒库中的小车卸完货并且后退中
                //出库轨道中没有车辆在前面
                //if (PubTask.Carrier.IsCarrierUnLoadAndBackWard(trans.carrier_id)
                //    && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id))
                //{
                //    #region 【任务步骤记录】
                //    _M.LogForCarrierToTrack(trans, trans.give_track_id);
                //    #endregion

                //    //前进至点
                //    MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, CarrierPosE.轨道前侧定位点);
                //}

                //if (PubTask.Carrier.IsCarrierInTrackBiggerRfID1(trans.carrier_id, trans.give_track_id))
                //{
                //    _M.SetCarrier(trans, 0, string.Format("倒库任务暂停，释放运输车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                //}
                #endregion
            }
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

        public override void Out2OutRelayWait(StockTrans trans)
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
