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
            if (PubTask.Carrier.HaveInTrack(trans.take_track_id, out uint carrierid))
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
                string msg = _M.AllocateFerryToCarrierSort(trans, DeviceTypeE.上摆渡);

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
                    if (trans.take_track_id == track.id)
                    {
                        if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库)
                            || PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
                        {
                            _M.SetStatus(trans, TransStatusE.倒库中);
                        }
                    }
                    break;
                case TrackTypeE.储砖_出:

                    if (trans.give_track_id == track.id)
                    {
                        if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库)
                            //|| PubTask.Carrier.IsCarrierFinishTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库)
                            )
                        {
                            if (!trans.IsReleaseTakeFerry
                                 && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                                 && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
                            {
                                trans.IsReleaseTakeFerry = true;
                            }

                            _M.SetStatus(trans, TransStatusE.倒库中);
                        }
                        else if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            if (isload)
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1308, string.Format("运输车[ {0} ]载着砖无法执行倒库任务流程；",
                                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion

                                PubMaster.Warn.AddTaskWarn(WarningTypeE.CarrierLoadNotSortTask, (ushort)trans.carrier_id, trans.id);

                                return;
                            }

                            if (isnotload)
                            {
                                byte movecount = (byte)PubMaster.Goods.GetBehindPointStockCount(track.id, track.up_split_point);

                                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpSortUseMaxNumber))
                                {
                                    byte line_max_move = PubMaster.Area.GetLineUpSortMaxNumber(track.area, track.line);

                                    if (line_max_move > 0 && movecount > line_max_move)
                                    {
                                        movecount = line_max_move;
                                    }
                                }

                                #region 【任务步骤记录】
                                _M.LogForCarrierSort(trans, trans.give_track_id, movecount.ToString());
                                #endregion

                                //后退至轨道倒库
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.往前倒库,
                                    CheckTra = track.ferry_down_code,
                                    ToPoint = (ushort)(track.split_point + 50),
                                    MoveCount = movecount
                                });
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (isload)
                        {
                            #region 【任务步骤记录】
                            _M.LogForCarrierGiving(trans);
                            #endregion

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
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

                                //前进至摆渡车
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackUpCode(ferryTraid),
                                    ToRFID = PubMaster.Track.GetTrackRFID1(ferryTraid)
                                });
                                return;
                            }
                        }
                    }
                    break;
                #endregion

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_出:
                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1408, string.Format("运输车[ {0} ]载着砖无法执行倒库任务流程；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        #endregion

                        PubMaster.Warn.AddTaskWarn(WarningTypeE.CarrierLoadSortTask, (ushort)trans.carrier_id, trans.id);

                        return;
                    }

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

                                Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);

                                byte movecount = (byte)PubMaster.Goods.GetBehindPointStockCount(gtrack.id, gtrack.up_split_point);

                                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpSortUseMaxNumber))
                                {
                                    byte line_max_move = PubMaster.Area.GetLineUpSortMaxNumber(gtrack.area, gtrack.line);

                                    if (line_max_move > 0 && movecount > line_max_move)
                                    {
                                        movecount = line_max_move;
                                    }
                                }

                                #region 【任务步骤记录】
                                _M.LogForCarrierSort(trans, trans.give_track_id, movecount.ToString());
                                #endregion

                                //后退至轨道倒库
                                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.往前倒库,
                                    CheckTra = gtrack.ferry_down_code,
                                    ToPoint = (ushort)(gtrack.split_point + 50),
                                    MoveCount = movecount
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
            if (track.id != trans.take_track_id 
                && track.id != trans.give_track_id 
                && track.brother_track_id != trans.take_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "倒库中的运输车在其他轨道，结束任务");
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);

            // 【不允许接力-轨道有其他小车】
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask)
                && PubTask.Carrier.HaveInTrackButCarrier(trans.take_track_id, trans.give_track_id, trans.carrier_id, out carrierid))
            {
                // 不接力 - 终止且报警
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1608, string.Format("检测到倒库轨道中有其他运输车[ {0} ], 强制终止倒库运输车[ {1} ]",
                    PubMaster.Device.GetDeviceName(carrierid),
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion

                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, "倒库中相关任务轨道出现其他运输车");

                PubMaster.Warn.AddDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack,
                    (ushort)trans.carrier_id, trans.id, trans.take_track_id, carrierid);

                return;
            }

            PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);

            // 小车空闲状态
            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
            {
                // 有砖-前进放砖
                if (isload)
                {
                    #region 【任务步骤记录】
                    _M.LogForCarrierGive(trans, trans.give_track_id);
                    #endregion

                    //前进放砖
                    CarrierActionOrder cao = new CarrierActionOrder
                    {
                        Order = DevCarrierOrderE.放砖指令,
                        CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                        ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id)
                    };

                    return;
                }

                // 无砖 - 回出库轨道头
                if (isnotload)
                {
                    ushort carspace = GlobalWcsDataConfig.BigConifg.GetSortWaitNumberCarSpace(trans.area_id, trans.line);
                    //小车当前所在的轨道
                    if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpSortUseMaxNumber)
                        && 0 != PubMaster.Area.GetLineUpSortMaxNumber(track.area, track.line)
                        && 1 < PubMaster.Goods.GetBehindPointStockCount(trans.give_track_id, track.up_split_point))
                    {
                        ushort topoint;
                        PubTask.Carrier.GetCarrierNowUnloadPoint(trans.carrier_id, out ushort nowpoint, out ushort givepoint);
                        if (nowpoint == 0 || givepoint == 0)
                        {
                            topoint = track.split_point;
                            //return;
                        }
                        else
                        {
                            Stock bestock_one = null;

                            //PubMaster.Goods.GetStockBehindStockPoint(track.id, givepoint);
                            //if (bestock_one != null)
                            //{
                            //    if (Math.Abs(bestock_one.location - givepoint) <= 200)
                            //    {
                            //        bestock_one = PubMaster.Goods.GetStockBehindStockPoint(track.id, bestock_one.location);
                            //    }
                            //}
                            if (bestock_one != null)
                            {
                                topoint = bestock_one.location;
                                topoint += (ushort)(carspace * PubMaster.Goods.GetStackSafe(0, 0));
                            }
                            else
                            {
                                topoint = givepoint;
                                topoint -= (ushort)(carspace * PubMaster.Goods.GetStackSafe(0, 0));
                            }
                        }

                        //需要定位的位置比出轨道最后取货点都小则用
                        if (topoint <= track.split_point)
                        {
                            topoint = track.split_point;
                        }

                        if (Math.Abs(nowpoint - topoint) <= 100)
                        {
                            _M.SetStatus(trans, TransStatusE.接力等待);
                            return;
                        }

                        //定位到当前卸货位置或当前位置往后两个车位
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = track.ferry_down_code,
                            ToPoint = topoint,
                        }, string.Format("接力等待后退[ {0} ]车身的位置", carspace));
                    }
                    else
                    {
                        _M.SetStatus(trans, TransStatusE.小车回轨);
                    }
                }
            }
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
            if (track.id != trans.take_track_id && track.id != trans.give_track_id && track.brother_track_id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "倒库中的运输车在其他轨道，结束任务");
                return;
            }

            #region【接力倒库完成后，如果前面有车有任务，则先定位到后一个位置】
            ushort _topoint = 0;
            PubTask.Carrier.GetCarrierNowUnloadPoint(trans.carrier_id, out ushort _nowpoint, out ushort _givepoint);
            bool localneed = true; //需要定位到下一个位置
            bool dolocate = false;//是否需要定位
            bool iscarrierferr = PubTask.Carrier.IsCarrierFree(trans.carrier_id);
            if (_nowpoint == 0 || _givepoint == 0)
            {
                _topoint = track.split_point;
            }
            else
            {
                _topoint = _givepoint;
                _topoint -= (ushort)(2 * PubMaster.Goods.GetStackSafe(0, 0));
            }

            //需要定位的位置比出轨道最后取货点都小则用
            if (_topoint <= track.split_point)
            {
                _topoint = track.split_point;
            }

            if (Math.Abs(_nowpoint - _topoint) <= 100)
            {
                localneed = false;
            }
            #endregion

            // 任务运输车前面即将有车
            if (PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
            {
                //需要定位，并且分割点前有库存 => 才需要定位到下一个位置
                if (localneed && PubMaster.Goods.ExistBehindUpSplitPoint(track.id, track.up_split_point))
                {
                    dolocate = true;
                }
                else
                {
                    if (!iscarrierferr)
                    {
                        //终止
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.终止指令
                        }, "前方有其他运输车将至");

                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1708, string.Format("终止运输车[ {0} ]，检测到前方可能有其他运输车进入轨道",
                            PubMaster.Device.GetDeviceName(trans.carrier_id)));
                        #endregion
                    }

                    return;
                }
            }

            // 任务运输车前面有车
            if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
            {
                bool intrans = _M.HaveCarrierInTrans(othercarrier);

                //上砖车有任务，接力需要定位，并且分割点前有库存 => 才需要定位到下一个位置
                if (intrans && localneed && PubMaster.Goods.ExistBehindUpSplitPoint(track.id, track.up_split_point))
                {
                    dolocate = true;
                }
                else
                {
                    if (!iscarrierferr)
                    {
                        //终止
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.终止指令
                        }, string.Format("前方存在其他运输车[ {0} ]", PubMaster.Device.GetDeviceName(othercarrier)));
                    }

                    if (intrans)
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1808, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]绑定有任务，等待其任务完成；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id),
                            PubMaster.Device.GetDeviceName(othercarrier)));
                        #endregion
                        return;
                    }

                    if (!PubTask.Carrier.IsCarrierFree(othercarrier))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1908, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id),
                            PubMaster.Device.GetDeviceName(othercarrier)));
                        #endregion
                        return;
                    }

                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 2008, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]，尝试对其生成移车任务；",
                            PubMaster.Device.GetDeviceName(trans.carrier_id),
                            PubMaster.Device.GetDeviceName(othercarrier)));
                    #endregion

                    //转移到同类型轨道
                    TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                    track = PubTask.Carrier.GetCarrierTrack(othercarrier);
                    _M.AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
                    return;
                }
            }

            if (dolocate && _topoint != 0)
            {
                if (!iscarrierferr)
                {
                    return;
                }

                //定位到当前卸货位置或当前位置往后两个车位
                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.定位指令,
                    CheckTra = track.ferry_down_code,
                    ToPoint = _topoint,
                }, "接力后退两个位置避让取砖");
                return;
            }

            // 任务运输车回到出库轨道头
            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                && (track.id == track.brother_track_id
                        || (track.id == trans.give_track_id
                            && !PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, trans.give_track_id))))
            {
                #region 【任务步骤记录】
                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                #endregion

                // 前进至点
                PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.定位指令,
                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                }, "接力完成返回出轨道");
                return;
            }

            // 完成？
            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track)
                && track.id == trans.give_track_id
                && PubTask.Carrier.IsCarrierInTrackBiggerSite(trans.carrier_id, trans.give_track_id))
            {
                _M.SetStatus(trans, TransStatusE.完成);
            }
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

        }

        /// <summary>
        /// 倒库暂停
        /// </summary>
        /// <param name="trans"></param>
        public override void SortTaskWait(StockTrans trans)
        {
            if (trans.carrier_id != 0)
            {
                //倒库中的小车卸完货并且后退中
                //出库轨道中没有车辆在前面
                if (PubTask.Carrier.IsCarrierUnLoadAndBackWard(trans.carrier_id)
                    && !PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id))
                {
                    #region 【任务步骤记录】
                    _M.LogForCarrierToTrack(trans, trans.give_track_id);
                    #endregion

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
                    _M.SetCarrier(trans, 0, string.Format("倒库任务暂停，释放小车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
                }
            }
        }

        /// <summary>
        /// 接力等待
        /// </summary>
        /// <param name="trans"></param>
        public override void Out2OutRelayWait(StockTrans trans)
        {
            if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
            {
                _M.SetStatus(trans, TransStatusE.倒库中);
                return;
            }

            if (track == null || track.id != trans.give_track_id)
            {
                track = PubMaster.Track.GetTrack(trans.give_track_id);
            }
            //当前车是否空闲
            bool carrierfree = PubTask.Carrier.IsCarrierFree(trans.carrier_id);
            if (!carrierfree) return;

            //轨道存在其他车
            bool havecarinfront = PubTask.Carrier.ExistCarInFront(trans.carrier_id, track.id, out uint othercarid);

            //接力点前的库存数
            int infrontstockcount = PubMaster.Goods.GetInfrontPointStockCount(track.id, track.up_split_point);

            //接力点前没有库存，同时没车，则开始接力
            bool upnonestock = false;
            if (infrontstockcount  == 0
                && !havecarinfront)
            {
                upnonestock = true;
            }

            //接力点前只有一车砖，同时有车取砖取了它
            bool onestockcarloadit = false;
            if(infrontstockcount ==1
                && havecarinfront)
            {
                Stock topstock = PubMaster.Goods.GetTrackTopStock(track.id);
                if(topstock != null && topstock.location > track.up_split_point 
                    && PubMaster.DevConfig.IsCarrierBindStock(othercarid, topstock.id))
                {
                    onestockcarloadit = true;
                }
            }

            //前面没有库存，继续倒库
            if (upnonestock || onestockcarloadit)
            {
                //运输车当前脉冲
                ushort carrierpoint = PubTask.Carrier.GetCurrentPoint(trans.carrier_id);
                //大于运输车当前脉冲的库存位置【如果有：运输车需要前进至点然后再执行倒库，无则直接倒库】
                ushort infrontcarstockloc = PubMaster.Goods.GetInfrontPointStockMaxLocation(track.id, carrierpoint);
                if (upnonestock && infrontcarstockloc > 0)
                {
                    infrontcarstockloc += (ushort)(2 * PubMaster.Goods.GetStackSafe(0, 0));

                    //需要定位的位置比出轨道最后取货点都小则用
                    if (infrontcarstockloc >= track.limit_point_up)
                    {
                        //前进至点
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = track.ferry_down_code,
                            ToRFID = track.rfid_1,
                        });
                    }
                    else
                    {
                        //定位到当前卸货位置或当前位置往后两个车位
                        PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = track.ferry_down_code,
                            ToPoint = infrontcarstockloc,
                        });
                    }
                }
                else
                {
                    byte movecount = (byte)PubMaster.Goods.GetBehindPointStockCount(track.id, track.up_split_point);
                    byte line_max_move = PubMaster.Area.GetLineUpSortMaxNumber(track.area, track.line);

                    if (line_max_move > 0 && movecount > line_max_move)
                    {
                        movecount = line_max_move;
                    }

                    //后退至轨道倒库
                    PubTask.Carrier.DoOrder(trans.carrier_id, new CarrierActionOrder()
                    {
                        Order = DevCarrierOrderE.往前倒库,
                        CheckTra = track.ferry_down_code,
                        ToPoint = (ushort)(track.split_point + 50),
                        MoveCount = movecount
                    });
                }
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

        public override void ReturnDevBackToTrack(StockTrans trans)
        {

        }

        #endregion
    }
}
