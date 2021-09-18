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
    /// <summary>
    /// 倒库任务
    /// 轨道内库存转移-流程（code- XX02）
    /// </summary>
    public class In2OutSortTrans_V2 : BaseTaskTrans
    {
        public In2OutSortTrans_V2(TransMaster trans) : base(trans)
        {

        }


        /// <summary>
        /// 检查轨道（code- 1X02）
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {
            //转移卸货轨道不符合的运输车
            if (CheckGoodsAndAddMoveTask(trans, trans.give_track_id))
            {
                return;
            }

            //转移在轨道入库侧的运输车
            if (CheckCarAndAddMoveTask(trans, trans.take_track_id, true))
            {
                return;
            }

            _M.SetStatus(trans, TransStatusE.调度设备);
        }

        /// <summary>
        /// 调度设备（code- 2X02）
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {
            //是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
            if (_M.HaveTaskSortTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 2002, string.Format("存在相同作业轨道的任务，等待任务完成；"));
                #endregion
                return;
            }

            //分配运输车
            if (PubTask.Carrier.AllocateCarrier(trans, out uint carrierid, out string result)
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
        /// 移车中（code- 3X02）
        /// </summary>
        /// <param name="trans"></param>
        public override void MovingCarrier(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

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
                        if (isStopNoOrder)
                        {
                            // 解锁摆渡
                            RealseTakeFerry(trans);

                            // 倒库前确定作业库存
                            SetStockForSort(trans);
                            _M.SetStatus(trans, TransStatusE.倒库中);
                        }
                    }
                    else
                    {
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
                            //摆渡车接车
                            MoveToFerrySeamless(trans, true);
                            return;
                        }
                    }
                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.前置摆渡轨道:
                case TrackTypeE.后置摆渡轨道:
                    if (trans.AllocateFerryType == DeviceTypeE.其他)
                    {
                        _M.SetAllocateFerryType(trans, (track.Type == TrackTypeE.后置摆渡轨道 ? DeviceTypeE.后摆渡 : DeviceTypeE.前摆渡));
                    }
                    // 锁定摆渡车
                    if (!AllocateTakeFerry(trans, trans.AllocateFerryType, track)) return;

                    if (isLoad)
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 3002, string.Format("运输车[ {0} ]载着砖无法执行倒库任务流程；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        #endregion

                        PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.CarrierLoadSortTask, (ushort)trans.carrier_id, trans.id);
                        return;
                    }
                    PubMaster.Warn.RemoveTaskWarn(WarningTypeE.CarrierLoadSortTask, trans.id);

                    if (isNotLoad && isStopNoOrder)
                    {
                        if (PubTask.Ferry.IsLoad(trans.take_ferry_id))
                        {
                            if (!_M.LockFerryAndAction(trans, trans.take_ferry_id, trans.give_track_id, track.id, out uint ferryTraid, out string res))
                            {
                                #region 【任务步骤记录】
                                _M.LogForFerryMove(trans, trans.take_ferry_id, trans.give_track_id, res);
                                #endregion
                                return;
                            }

                            // 移至轨道定位点
                            MoveToPos(trans.give_track_id, trans.carrier_id, trans.id, track.Type == TrackTypeE.后置摆渡轨道 ? CarrierPosE.轨道后侧定位点 : CarrierPosE.轨道前侧定位点);

                            #region 【任务步骤记录】
                            _M.LogForCarrierToTrack(trans, trans.give_track_id);
                            #endregion
                            return;
                        }
                    }

                    break;
                #endregion

            }
        }

        /// <summary>
        /// 倒库中（code- 4X02）
        /// </summary>
        /// <param name="trans"></param>
        public override void SortingStock(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            // 小车不在本轨道
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.调度设备, "倒库运输车在其他轨道，尝试重新分配");
                return;
            }

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            // 【不允许接力 && 轨道有其他小车】
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask)
                && PubTask.Carrier.HaveInTrackButCarrier(trans.take_track_id, trans.give_track_id, trans.carrier_id, out uint carrierid))
            {
                // 不接力 - 终止且报警
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 4002, string.Format("检测到倒库轨道中有其他运输车[ {0} ], 强制终止倒库运输车[ {1} ]",
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
            if (track.Type == TrackTypeE.储砖_出入 && isStopNoOrder)
            {
                #region 基础脉冲
                // 安全距离
                ushort safe = PubMaster.Goods.GetStackSafe(trans.goods_id, trans.carrier_id);
                // 分界位置
                ushort splitP = track.split_point;
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

                #endregion

                #region 载砖
                if (isLoad)
                {
                    // 更新库存为小车绑定库存ID
                    _M.SetStock(trans, PubMaster.DevConfig.GetCarrierStockId(trans.carrier_id));

                    // 获取放砖位置
                    if (!GetTransferGivePoint(track.id, trans.carrier_id, limitP, carrier.CurrentPoint, out ushort givePoint))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 4102, string.Format("无合适放砖位置"));
                        #endregion
                        return;
                    }

                    // 直接放砖
                    GiveInTarck(givePoint, trans.give_track_id, trans.carrier_id, trans.id, out string res, trans.AllocateFerryType, true);

                    #region 【任务步骤记录】
                    _M.LogForCarrierGive(trans, trans.give_track_id, res);
                    #endregion
                    return;
                }
                #endregion

                #region 无砖
                if (isNotLoad)
                {
                    // 判断作业库存
                    if (trans.stock_id == 0)
                    {
                        // 检测轨道存砖状态
                        PubMaster.Track.CheckTrackStockStatus(track.id);

                        // 无作业库存  结束
                        _M.SetStatus(trans, TransStatusE.小车回轨);
                        return;
                    }
                    else
                    {
                        // 作业库存 
                        Stock workStock = PubMaster.Goods.GetStock(trans.stock_id);
                        // 库存轨道已经改变
                        if (workStock.track_id != track.id)
                        {
                            // 倒库前确定作业库存
                            SetStockForSort(trans);
                            return;
                        }
                        // 库存已被其他运输车绑定
                        if (PubMaster.DevConfig.GetCarrierByStockid(workStock.id, out uint carid) && carid != trans.carrier_id)
                        {
                            // 倒库前确定作业库存
                            SetStockForSort(trans);
                            return;
                        }

                        // 取砖脉冲
                        ushort takePoint = workStock?.location ?? 0;

                        // 判断取砖位置是否超接力结束点
                        if (track.is_take_forward ? (takePoint > overP) : (takePoint < overP))
                        {
                            // 停止当前倒库
                            _M.SetStock(trans, 0);
                            return;
                        }

                        // 作业库存 放砖脉冲
                        if (!GetTransferGivePoint(track.id, trans.carrier_id, limitP, takePoint, out ushort givePoint))
                        {
                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 4202, string.Format("无合适放砖位置"));
                            #endregion
                            return;
                        }

                        // 判断放砖位置是否超分界点 - 暂无分段不使用
                        //if (track.is_take_forward ? (givePoint > splitP) : (givePoint < splitP))
                        //{
                        //    // 停止当前接力
                        //    _M.SetStock(trans, 0);
                        //    return;
                        //}

                        // 取放位置间距  ≈173CM
                        ushort dis = 100;
                        // 取放位置间距过小则重新设定作业库存
                        if (Math.Abs(takePoint - givePoint) <= dis)
                        {
                            // 倒库前确定作业库存
                            SetStockForSort(trans);
                            return;
                        }

                        // 取砖最小判断脉冲 ±10
                        ushort mindicT = 10;
                        // 判断当前位置是否是取砖移动范围
                        if (track.is_take_forward ? (carrier.CurrentPoint > (takePoint - mindicT)) : (carrier.CurrentPoint < (takePoint + mindicT)))
                        {
                            // 小车需先到合适位置 (前进取 -小车要在库存位后至少 10 脉冲；后退取 -小车要在库存位前至少 10 脉冲)
                            ushort toloc = (ushort)(track.is_take_forward ? (takePoint - mindicT) : (takePoint + mindicT));
                            MoveToLoc(track.id, trans.carrier_id, trans.id, toloc);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, track.id, "取砖前，让小车先到合适位置");
                            #endregion
                            return;
                        }
                        else
                        {
                            // 暂时用取砖指令
                            TakeInTarck(trans.stock_id, track.id, trans.carrier_id, trans.id, out string res, trans.AllocateFerryType, true);

                            #region 【任务步骤记录】
                            _M.LogForCarrierTake(trans, track.id, res);
                            #endregion
                            return;

                            // 倒库
                            //MoveToSort(track.id, trans.carrier_id, trans.id, takePoint, givePoint,
                            //    string.Format("分界点[ {0} ], 取砖点[ {1} ], 放砖点[ {2} ]", splitP, takePoint, givePoint));

                            //#region 【任务步骤记录】
                            //_M.LogForCarrierSortRelay(trans, track.id);
                            //#endregion
                            //return;
                        }

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
        /// 运输车回轨（code- 5X02）
        /// </summary>
        /// <param name="trans"></param>
        public override void ReturnCarrrier(StockTrans trans)
        {
            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            // 小车不在本轨道
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "倒库运输车在其他轨道，结束任务");
                return;
            }

            bool isLoad = carrier.IsLoad();
            bool isNotLoad = carrier.IsNotLoad();
            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            #region 通用轨道
            if (track.Type == TrackTypeE.储砖_出入)
            {
                #region 基础脉冲
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

                #endregion

                #region 载砖
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
                    //else
                    //{
                    //    //终止
                    //    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                    //    {
                    //        Order = DevCarrierOrderE.终止指令
                    //    }, "倒库回轨流程保证小车无砖，终止载砖小车所有动作");
                    //}

                    #region 【任务步骤记录】
                    _M.LogForCarrierGiving(trans);
                    #endregion

                    return;
                }

                #endregion

                #region 无砖
                if (isNotLoad)
                {
                    // 运输车等待的时候需要后退几个车身
                    ushort carspace = GlobalWcsDataConfig.BigConifg.GetSortWaitNumberCarSpace(trans.area_id, trans.line);
                    carspace = (ushort)(carspace * safe);

                    // 有车阻碍
                    if (PubTask.Carrier.ExistLocateObstruct(trans.carrier_id, trans.give_track_id, carrier.CurrentPoint, track.is_take_forward, out CarrierTask otherCar))
                    {
                        // 终止小车回轨道头
                        if (!isStopNoOrder)
                        {
                            if (carrier.TargetPoint == limitP)
                            {
                                //终止
                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.终止指令
                                }, string.Format("前方存在其他运输车[ {0} ]", otherCar.Device.name));

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 5002, string.Format("检测到前方有运输车[ {0} ]，[ {1} ]停止动作；",
                                    otherCar.Device.name, PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion
                            }

                            return;
                        }

                        // 无移动
                        if (otherCar.TargetPoint == 0)
                        {
                            // 是否绑定任务
                            if (_M.HaveCarrierInTrans(otherCar.ID))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 5102, string.Format("检测到前方有运输车[ {0} ]绑定有任务，等待其任务完成；",
                                    otherCar.Device.name));
                                #endregion
                                return;
                            }

                            if (!PubTask.Carrier.IsCarrierFree(otherCar.ID))
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 5202, string.Format("检测到前方有运输车[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                    otherCar.Device.name));
                                #endregion
                                return;
                            }

                            //转移到同类型轨道
                            _M.AddMoveCarrierTask(track.id, otherCar.ID, track.Type, MoveTypeE.转移占用轨道);

                            #region 【任务步骤记录】
                            _M.SetStepLog(trans, false, 5302, string.Format("检测到前方有运输车[ {0} ]，尝试对其生成移车任务；",
                                    otherCar.Device.name));
                            #endregion
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
                            if (Math.Abs(carrier.CurrentPoint - overpoint) >= 100)
                            {
                                MoveToLoc(track.id, trans.carrier_id, trans.id, overpoint);

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 5402, string.Format("检测到前方有运输车[ {0} ]正在移动，控制[ {1} ]进行避让；",
                                    otherCar.Device.name, PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion
                            }

                            return;
                        }

                    }

                    // 回轨道头
                    if (isStopNoOrder)
                    {
                        if (Math.Abs(carrier.CurrentPoint - limitP) > 10)
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

                #endregion

            }
            #endregion

            #region 出&入 轨道 - V2.1 停用
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
        /// 倒库暂停（code- 6X02）
        /// </summary>
        /// <param name="trans"></param>
        public override void SortTaskWait(StockTrans trans)
        {
            if (trans.carrier_id == 0) return;

            // 运行前提
            if (!_M.RunPremise(trans, out Track track, out CarrierTask carrier)) return;

            bool isStopNoOrder = carrier.IsStopNoOrder(out string result);

            #region 通用轨道
            if (track.Type == TrackTypeE.储砖_出入)
            {
                if (!isStopNoOrder)
                {
                    //终止
                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                    {
                        Order = DevCarrierOrderE.终止指令
                    }, "倒库暂停，终止并解绑小车");
                    return;
                }
                else
                {
                    // 极限位置
                    ushort limit = track.is_take_forward ? track.limit_point : track.limit_point_up;

                    if (Math.Abs(carrier.CurrentPoint - limit) > 10)
                    {
                        if (!PubTask.Carrier.ExistLocateObstruct(trans.carrier_id, trans.give_track_id, carrier.CurrentPoint, track.is_take_forward, out CarrierTask otherCar))
                        {
                            //至点
                            MoveToLoc(trans.give_track_id, trans.carrier_id, trans.id, limit);

                            #region 【任务步骤记录】
                            _M.LogForCarrierToTrack(trans, trans.give_track_id);
                            #endregion
                            return;
                        }
                    }
                    else
                    {
                        _M.SetCarrier(trans, 0, string.Format("倒库暂停，释放小车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
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
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public override void FinishStockTrans(StockTrans trans)
        {
            // 检测轨道存砖状态
            PubMaster.Track.CheckTrackStockStatus(trans.take_track_id);

            PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);
            //PubMaster.Track.GetAndRefreshUpCount(trans.give_track_id);
            _M.SetFinish(trans);
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
