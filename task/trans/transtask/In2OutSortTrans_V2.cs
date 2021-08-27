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
    /// 启用第二种倒库类型<br/>
    /// 条件1：运输车需支持，给目标脉冲找空位<br/>
    /// 条件2：运输车需支持，给结束脉冲方第一车位置<br/>
    /// </summary>
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
            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);

            //出入轨道
            if (PubMaster.Track.IsTrackType(trans.give_track_id, TrackTypeE.储砖_出入))
            {
                // 是否有不符规格的车在轨道
                if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trans.give_track_id, goodssizeID, out uint carrierid))
                {
                    if (_M.HaveCarrierInTrans(carrierid))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1002, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                            PubMaster.Device.GetDeviceName(carrierid),
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion
                        return;
                    }

                    if (!PubTask.Carrier.IsCarrierFree(carrierid))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1102, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                            PubMaster.Device.GetDeviceName(carrierid),
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion
                        return;
                    }

                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1202, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                        PubMaster.Device.GetDeviceName(carrierid),
                        PubMaster.Track.GetTrackName(trans.give_track_id)));
                    #endregion

                    //转移到同类型轨道
                    TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                    track = PubTask.Carrier.GetCarrierTrack(carrierid);

                    DeviceTypeE ferrytype = PubTask.Carrier.GetCarrierNeedFerryType(carrierid);

                    _M.AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道, ferrytype);
                }
                else
                {
                    havedifcaringive = false;
                    havecarintake = false;
                }
            }
            else//出轨道、入轨道
            {
                // 是否有不符规格的车在轨道
                if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trans.give_track_id, goodssizeID, out uint carrierid))
                {
                    if (_M.HaveCarrierInTrans(carrierid))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1002, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                            PubMaster.Device.GetDeviceName(carrierid),
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion
                        return;
                    }

                    if (!PubTask.Carrier.IsCarrierFree(carrierid))
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1102, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                            PubMaster.Device.GetDeviceName(carrierid),
                            PubMaster.Track.GetTrackName(trans.give_track_id)));
                        #endregion
                        return;
                    }

                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1202, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
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
                    havedifcaringive = false;
                }
                
                //是否有小车在满砖轨道(入库轨道)
                if (trans.take_track_id != trans.give_track_id
                    && PubTask.Carrier.HaveInTrack(trans.take_track_id, out uint fullcarrierid))
                {
                    if (PubTask.Carrier.IsCarrierFree(fullcarrierid))
                    {
                        _M.AddMoveCarrierTask(trans.take_track_id, fullcarrierid, TrackTypeE.储砖_入, MoveTypeE.转移占用轨道);
                    }
                    else if (PubTask.Carrier.IsCarrierInTask(fullcarrierid, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
                    {
                        havecarintake = false;
                    }
                }
                else
                {
                    havecarintake = false;
                }
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
                case TrackTypeE.储砖_出:
                case TrackTypeE.储砖_出入:
                    if (trans.give_track_id == track.id || trans.take_track_id == track.id)
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
                                _M.FreeTakeFerry(trans);
                            }

                            _M.SetStatus(trans, TransStatusE.倒库中);
                        }
                        else if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
                            if (isload)
                            {
                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 1402, string.Format("运输车[ {0} ]载着砖无法执行倒库任务流程；",
                                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                                #endregion

                                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.CarrierLoadNotSortTask, (ushort)trans.carrier_id, trans.id);

                                return;
                            }

                            if (isnotload)
                            {
                                CheckAndSendSortTask(trans);
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

                            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
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

                #region[小车在摆渡车]
                case TrackTypeE.摆渡车_出:
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

                                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                {
                                    Order = DevCarrierOrderE.定位指令,
                                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                    ToTrackId = trans.give_track_id
                                });

                                #region[先后退至点，在轨道后再执行倒库任务]

                                //Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);
                                ////出库轨道
                                ////      有砖：则先定位至无砖的地方再执行倒库任务(头部没有空余位置)
                                ////      无砖：则定位到定位点执行倒库
                                //if (PubMaster.Goods.ExistStockInTrack(trans.give_track_id)
                                //    && gtrack.Type == TrackTypeE.储砖_出
                                //    && !PubMaster.Track.IsTrackHeadEmpty(gtrack))
                                //{
                                //    if (gtrack != null)
                                //    {
                                //        ushort toempypoint =0 ;

                                //        Stock btmstock = PubMaster.Goods.GetTrackButtomStock(trans.give_track_id);
                                //        if (btmstock != null)
                                //        {
                                //            ushort safe = (ushort)PubMaster.Dic.GetDtlDouble(DicTag.StackPluse, 217);//统计出来的(实际库存位置差平均值)
                                //            toempypoint = (ushort)(btmstock.location - (3 * safe));
                                //        }

                                //        if (toempypoint < gtrack.split_point)
                                //        {
                                //            toempypoint = gtrack.split_point;
                                //        }

                                //        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //        {
                                //            Order = DevCarrierOrderE.定位指令,
                                //            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                //            ToPoint = toempypoint,
                                //            ToTrackId = trans.give_track_id
                                //        });
                                //    }
                                //}
                                //else
                                //{
                                //    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                                //    {
                                //        Order = DevCarrierOrderE.定位指令,
                                //        CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                //        ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                                //        ToTrackId = trans.give_track_id
                                //    });
                                //}
                                #endregion
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
        /// 运输车停止在倒库轨道
        /// </summary>
        /// <param name="trans"></param>
        private void CheckAndSendSortTask(StockTrans trans)
        {
            Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);
            takeTrack = PubMaster.Track.GetTrack(trans.take_track_id);

            ushort safe = PubMaster.Goods.GetStackSafe(trans.goods_id, trans.carrier_id);//统计出来的(实际库存位置差平均值)

            bool isheadempty = PubMaster.Track.IsTrackHeadEmpty(gtrack);
            bool ismidempty = PubMaster.Track.IsTrackMidEmpty(gtrack, out uint stockid, out string midres);
            bool isbackempty = PubMaster.Track.IsTrackBackEmpty(gtrack, out int emptycount);

            int count = 0;//倒库数量

            //      空位脉冲        结束脉冲
            ushort toempypoint = 0, togivepoint = 0;
            ushort nowpoint = PubTask.Carrier.GetCarrierNowPoint(trans.carrier_id);
            string sortmemo = string.Empty;//倒库备注

            if (gtrack.Type == TrackTypeE.储砖_出入)
            {
                if (isheadempty)
                {
                    Stock topstock = PubMaster.Goods.CheckGetStockTop(trans.take_track_id);
                    if (topstock != null)
                    {
                        toempypoint = (ushort)(topstock.location + (3 * safe));
                    }

                    count = PubMaster.Goods.GetTrackStockCount(trans.take_track_id);

                    sortmemo = string.Format("倒库[ 前面空 ]");
                }
                else if (ismidempty)
                {
                    Stock midstock = PubMaster.Goods.GetStock(stockid);
                    if (midstock != null)
                    {
                        count = PubMaster.Goods.GetBehindPointStockCount(gtrack.id, midstock.location + 50);
                        toempypoint = (ushort)(midstock.location + (3 * safe));
                    }

                    sortmemo = string.Format("倒库[ 中间空 ]，库存[ {0} ]", midres);
                }

                if (toempypoint > gtrack.limit_point_up)
                {
                    toempypoint = (ushort)(gtrack.limit_point_up - 50);
                }

                sortmemo += string.Format(", 空砖位置[ {0} ], 倒库数量[ {1} ]", toempypoint, count);
            }
            else //出轨道，入轨道
            {
                //出轨道 头部空 或者 出轨道 空
                if (isheadempty)
                {
                    Stock topstock;
                    if (gtrack.StockStatus == TrackStockStatusE.空砖)//出轨道空砖
                    {
                        topstock = PubMaster.Goods.GetTrackTopStock(trans.take_track_id);
                        count = PubMaster.Goods.GetTrackStockCount(trans.take_track_id);
                    }
                    else
                    {
                        topstock = PubMaster.Goods.GetTrackTopStock(trans.give_track_id);
                        count = PubMaster.Goods.GetTrackStockCount(trans.give_track_id);
                    }

                    if (topstock != null)
                    {
                        toempypoint = (ushort)(topstock.location + (2 * safe));
                    }

                    if (toempypoint > gtrack.limit_point_up)
                    {
                        toempypoint = (ushort)(gtrack.limit_point_up - 50);
                    }

                    sortmemo = string.Format("倒库[ 前面空 ]");
                }
                else if (ismidempty)
                {
                    Stock midstock = PubMaster.Goods.GetStock(stockid);
                    if (midstock != null)
                    {
                        count = PubMaster.Goods.GetBehindPointStockCount(gtrack.id, midstock.location + 50);
                        toempypoint = (ushort)(midstock.location + (3 * safe));
                    }

                    sortmemo = string.Format("倒库[ 中间空 ]，库存[ {0} ]", midres);

                }
                else if (isbackempty)
                {
                    //出轨道尾部有空位，但是小于检测的数量
                    if(takeTrack.Type == TrackTypeE.储砖_出 
                        && PubMaster.Track.IsSortAble(gtrack.brother_track_id))
                    {
                        Track brotrack = PubMaster.Track.GetTrack(gtrack.brother_track_id);
                        if(brotrack != null
                            && brotrack.Type == TrackTypeE.储砖_入
                            && brotrack.sort_able
                            && brotrack.TrackStatus == TrackStatusE.启用
                            && brotrack.StockStatus != TrackStockStatusE.空砖)
                        {
                            // 任务运输车前面有车
                            if (PubTask.Carrier.ExistCarBehind(trans.carrier_id, brotrack.id, out uint othercarrier))
                            {
                                string othername = PubMaster.Device.GetDeviceName(othercarrier);
                               
                                if (_M.HaveCarrierInTrans(othercarrier))
                                {
                                    #region 【任务步骤记录】
                                    _M.SetStepLog(trans, false, 1802, string.Format("入库轨道检测到有运输车[ {0} ]绑定有任务，等待其任务完成；",
                                        othername));
                                    #endregion
                                    return;
                                }

                                if (!PubTask.Carrier.IsCarrierFree(othercarrier))
                                {
                                    #region 【任务步骤记录】
                                    _M.SetStepLog(trans, false, 1902, string.Format("入库轨道检测到有运输车[ {0} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                                        othername));
                                    #endregion
                                    return;
                                }

                                #region 【任务步骤记录】
                                _M.SetStepLog(trans, false, 2002, string.Format("入库轨道检测到有运输车[ {0} ]，尝试对其生成移车任务；",
                                        othername));
                                #endregion

                                //转移到同类型轨道
                                track = PubTask.Carrier.GetCarrierTrack(othercarrier);
                                _M.AddMoveCarrierTask(track.id, othercarrier, TrackTypeE.储砖_入, MoveTypeE.转移占用轨道);
                                return;
                            }

                            _M.SetTakeSite(trans, brotrack.id, "出轨道尾部空，入库轨道可倒库");
                            return;
                        }
                    }
                }

                //出库轨道满，入库轨道头部空
                if(count == 0
                    && takeTrack.Type == TrackTypeE.储砖_入 
                    && takeTrack.sort_able 
                    && takeTrack.TrackStatus == TrackStatusE.启用
                    && takeTrack.StockStatus != TrackStockStatusE.空砖)
                {
                    //入轨道库存数量
                    int intrackcount = PubMaster.Goods.GetTrackStockCount(trans.take_track_id);

                    Stock givetrackbtmstock = PubMaster.Goods.GetTrackButtomStock(trans.give_track_id);
                    if (givetrackbtmstock != null)
                    {
                        double rate = GlobalWcsDataConfig.BigConifg.GetIn2OutCheckButtomSafeRate(trans.area_id, trans.line);

                        //出轨道尾部距离出轨道尾部还能继续放砖
                        if (givetrackbtmstock.location > gtrack.split_point
                            && Math.Abs(givetrackbtmstock.location - gtrack.split_point) >= safe * rate)
                        {
                            toempypoint = gtrack.split_point;
                            count = 1;

                            sortmemo = string.Format("倒库[ 出尾部空补入到出 ], 入轨库存[ {0} ], 出轨有空位[ {1} ]", intrackcount, emptycount);
                        }
                        else
                        {
                            //入轨道 头部空（至少2两个位置）
                            bool istakeheadempty = PubMaster.Track.IsTrackHeadEmpty(takeTrack, 2);
                            //入轨道头部空，才需要继续补出轨道尾部
                            if (istakeheadempty)
                            {
                                count = intrackcount;
                                toempypoint = (ushort)(takeTrack.split_point - 1);
                                togivepoint = takeTrack.split_point;

                                sortmemo = string.Format("倒库[ 入往前挪倒第一个位置 ], 结束脉冲[ {0} ]", togivepoint);
                            }
                            
                        }
                    }
                }

                sortmemo += string.Format(", 空砖位置[ {0} ], 倒库数量[ {1} ]", toempypoint, count);
            }

            if(count <= 0)
            {
                _M.SetStatus(trans, TransStatusE.小车回轨, string.Format("倒库数量为0,备注[ {0} ]", sortmemo));
                return;
            }

            #region 【任务步骤记录】
            _M.LogForCarrierSort(trans, trans.give_track_id, count.ToString());
            #endregion

            //后退至轨道倒库
            PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.往前倒库,
                CheckTra = gtrack.ferry_down_code,// PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                                                  //OverRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                ToPoint = toempypoint,
                OverPoint = togivepoint,
                MoveCount = (byte)count,
                ToTrackId = trans.give_track_id
            }, sortmemo);
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

            // 【不允许接力 && 轨道有其他小车】
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask)
                && PubTask.Carrier.HaveInTrackButCarrier(trans.take_track_id, trans.give_track_id, trans.carrier_id, out carrierid))
            {
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

                PubMaster.Warn.AddDevWarn(trans.area_id, trans.line, WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id, trans.id, trans.take_track_id, carrierid);

                return;
            }

            PubMaster.Warn.RemoveDevWarn(WarningTypeE.HaveOtherCarrierInSortTrack, (ushort)trans.carrier_id);

            // 小车空闲状态
            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
            {
                // 运输车有货 - 前进放砖
                if (isload)
                {
                    #region 【任务步骤记录】
                    _M.LogForCarrierGive(trans, trans.give_track_id);
                    #endregion

                    //前进放砖
                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                    {
                        Order = DevCarrierOrderE.放砖指令,
                        CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                        ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                        ToTrackId = trans.give_track_id
                    });

                    return;
                }

                // 运输车无货 - 回出库轨道头
                if (isnotload)
                {
                    //1.出轨道空砖 或 头部是否满足空位

                    //2.出轨道尾部是否满足空位

                    //3.入轨道是否需要往入轨道第一车位置挪砖

                    //4.小车回轨
                    CheckAndSendSortTask(trans);
                   
                    //_M.SetStatus(trans, TransStatusE.小车回轨);
                    return;
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
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "倒库中的运输车在其他轨道，结束任务");
                return;
            }

            if (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.往前倒库, DevCarrierOrderE.往后倒库))
            {
                _M.SetStatus(trans, TransStatusE.倒库中);
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.SortFinishButDownExistStock, trans.id);
            }

            // 任务运输车前面即将有车
            if (PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
            {
                CheckNeedLocateBack(trans, out bool isfalsereutn);
                if (isfalsereutn)
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 2202, string.Format("运输车前面即将有车，库存小于4，运输车后退3个位置"));
                    #endregion
                    return;
                }

                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, "前方有其他运输车将至");

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1702, string.Format("终止运输车[ {0} ]，检测到前方可能有其他运输车进入轨道",
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion

                return;
            }

            // 任务运输车前面有车
            if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
            {
                string othername = PubMaster.Device.GetDeviceName(othercarrier);
                string carname = PubMaster.Device.GetDeviceName(trans.carrier_id);
                bool oftask = PubTask.Carrier.IsCarrierFree(othercarrier);

                //非空闲其他运输车，取砖，定位，有需要则先后退空位
                if (!oftask)
                {
                    CheckNeedLocateBack(trans, out bool isfalsereutn);
                    if (isfalsereutn)
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 2202, string.Format("运输车前面有车，库存小于4，运输车后退3个位置"));
                        #endregion
                        return;
                    }
                }

                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, string.Format("前方存在其他运输车[ {0} ] ", othername));

                if (_M.HaveCarrierInTrans(othercarrier))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1802, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]绑定有任务，等待其任务完成；",
                        carname,
                        othername));
                    #endregion
                    return;
                }

                if (!oftask)
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1902, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carname,
                        othername));
                    #endregion
                    return;
                }

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 2002, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]，尝试对其生成移车任务；",
                        carname,
                        othername));
                #endregion

                //转移到同类型轨道
                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                track = PubTask.Carrier.GetCarrierTrack(othercarrier);
                _M.AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
                return;
            }

            ftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);
            // 任务运输车回到出库轨道头
            if (ftask 
                && (trans.take_track_id == track.id || trans.give_track_id == track.id)
                && !PubTask.Carrier.IsCarrierInTrackBiggerRfID2(trans.carrier_id, trans.give_track_id))
            {
                #region 【任务步骤记录】
                _M.LogForCarrierToTrack(trans, trans.give_track_id);
                #endregion

                // 前进至点
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.定位指令,
                    CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                    ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                    ToTrackId = trans.give_track_id
                });
                return;
            }

            // 完成？
            if (ftask
                && track.id == trans.give_track_id
                && PubTask.Carrier.IsCarrierInTrackBiggerRfID2(trans.carrier_id, trans.give_track_id))
            {
                // 入库侧仍还有库存
                //if (PubMaster.Track.IsTrackType(trans.take_track_id,TrackTypeE.储砖_入)
                //    && PubMaster.Goods.ExistStockInTrack(trans.take_track_id))
                //{
                //    //_M.SetStatus(trans, TransStatusE.移车中, "入库侧还有库存没倒完，重新发起倒库指令");
                //    //报警运输车倒库后入库轨道还有库存，请在核实并修改入库轨道的库存后，1.如果需要继续倒库，请手动给运输车发倒库任务，2.如果不需要继续倒库，请取消当前轨道的倒库任务并修改轨道状态为空砖/有砖
                //    PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.SortFinishButDownExistStock, (ushort)trans.carrier_id, trans.id);
                //    return;
                //}

                _M.SetStatus(trans, TransStatusE.完成);
            }
        }

        /// <summary>
        /// 检测是否需要定位运输车后退3个位置，给前面的车进行取砖
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="isfalsereturn"></param>
        private void CheckNeedLocateBack(StockTrans trans, out bool isfalsereturn)
        {
            //如果前面有车准备进来，或者有车在里面库存小于4的时候后退等待运输车空闲
            int stockcount = PubMaster.Goods.GetTrackStockCount(trans.give_track_id);
            if (stockcount <= 4)
            {
                CarrierPointRFID carpoint = PubTask.Carrier.GetCarrierToRfIdOrPoint(trans.carrier_id);
                Track gtrack = PubMaster.Track.GetTrack(trans.give_track_id);
                ftask = PubTask.Carrier.IsStopFTask(trans.carrier_id, track);

                //定位脉冲说明正在后退
                if (!ftask && carpoint.TargetPoint > 0 && carpoint.TargetSite == 0)
                {
                    isfalsereturn = true;
                    return;
                }

                //小于库存并且运输车空闲
                if (ftask)
                {
                    Stock btmstock = PubMaster.Goods.GetTrackButtomStock(trans.give_track_id);
                    ushort safe = PubMaster.Goods.GetStackSafe(0, 0);
                    if (btmstock != null && carpoint.CurrentPoint >= (btmstock.location - (ushort)(safe * 3)))
                    {
                        ushort tpoint = (ushort)(btmstock.location - safe * 3);
                        if (tpoint <= gtrack.split_point)
                        {
                            tpoint = gtrack.split_point;
                        }

                        //定位到当前卸货位置或当前位置往后两个车位
                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = gtrack.ferry_down_code,
                            ToPoint = tpoint,
                        }, string.Format("还车回轨但前面有车进来取砖，库存小于4，运输车后退3个位置[ {0} ]", tpoint));
                        isfalsereturn = true;
                        return;
                    }
                }
            }

            isfalsereturn = false;
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
            // 没分配到小车
            if (trans.carrier_id == 0)
            {
                _M.SetStatus(trans, TransStatusE.完成);
                return;
            }

            // 运行前提
            if (!_M.RunPremise(trans, out track))
            {
                _M.SetStatus(trans, TransStatusE.完成, "取消流程中出现异常，结束任务");
                return;
            }

            // 小车不在本轨道
            if (track.id != trans.take_track_id && track.id != trans.give_track_id)
            {
                _M.SetStatus(trans, TransStatusE.完成, "任务运输车不在作业轨道，结束任务");
                return;
            }

            isload = PubTask.Carrier.IsLoad(trans.carrier_id);
            isnotload = PubTask.Carrier.IsNotLoad(trans.carrier_id);
            ushort torfid = PubMaster.Track.GetTrackRFID2(trans.give_track_id);


            // 任务运输车前面即将有车
            if (PubTask.Carrier.ExistLocateTrack(trans.carrier_id, trans.give_track_id))
            {
                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, "前方有其他运输车将至");

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1702, string.Format("终止运输车[ {0} ]，检测到前方可能有其他运输车进入轨道",
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion

                return;
            }

            // 任务运输车前面有车
            if (PubTask.Carrier.ExistCarInFront(trans.carrier_id, trans.give_track_id, out uint othercarrier))
            {
                string othername = PubMaster.Device.GetDeviceName(othercarrier);
                string carname = PubMaster.Device.GetDeviceName(trans.carrier_id);
                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, string.Format("前方存在其他运输车[ {0} ]", othername));

                if (_M.HaveCarrierInTrans(othercarrier))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1802, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]绑定有任务，等待其任务完成；",
                        carname,
                        othername));
                    #endregion
                    return;
                }

                if (!PubTask.Carrier.IsCarrierFree(othercarrier))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1902, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carname,
                        othername));
                    #endregion
                    return;
                }

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 2002, string.Format("终止运输车[ {0} ]，检测到前方有运输车[ {1} ]，尝试对其生成移车任务；",
                        carname,
                        othername));
                #endregion

                //转移到同类型轨道
                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trans.give_track_id);
                track = PubTask.Carrier.GetCarrierTrack(othercarrier);
                _M.AddMoveCarrierTask(track.id, othercarrier, tracktype, MoveTypeE.转移占用轨道);
                return;
            }

            // 到出库轨道就算完成
            if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
            {
                if (track.id == trans.give_track_id)
                {
                    _M.SetStatus(trans, TransStatusE.完成);
                    return;
                }
                else
                {
                    if (isload)
                    {
                        #region 【任务步骤记录】
                        _M.LogForCarrierGive(trans, trans.give_track_id);
                        #endregion

                        if (PubTask.Carrier.IsStopFTask(trans.carrier_id, track))
                        {
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
                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, trans.give_track_id);
                        #endregion

                        // 前进至点
                        PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.定位指令,
                            CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                            ToRFID = torfid,
                            ToTrackId = trans.give_track_id
                        });
                        return;
                    }
                }
            }

            // 除了定位和放砖，其他全停
            if (!PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.定位指令, DevCarrierOrderE.放砖指令)
                || (PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.定位指令)
                        && !PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, torfid, 0, false)))
            {
                //终止
                PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.终止指令
                }, "倒库任务取消流程中");

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 2102, string.Format("终止运输车[ {0} ]，准备回到出库侧轨道取消倒库任务；",
                        PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion
            }
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
                    PubTask.Carrier.DoOrder(trans.carrier_id, trans.id, new CarrierActionOrder()
                    {
                        Order = DevCarrierOrderE.定位指令,
                        CheckTra = PubMaster.Track.GetTrackDownCode(trans.give_track_id),
                        ToRFID = PubMaster.Track.GetTrackRFID2(trans.give_track_id),
                        ToTrackId = trans.give_track_id
                    });
                }

                if (PubTask.Carrier.IsCarrierInTrackBiggerRfID1(trans.carrier_id, trans.give_track_id))
                {
                    _M.SetCarrier(trans, 0, string.Format("倒库任务暂停，释放运输车[ {0} ]", PubMaster.Device.GetDeviceName(trans.carrier_id)));
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
