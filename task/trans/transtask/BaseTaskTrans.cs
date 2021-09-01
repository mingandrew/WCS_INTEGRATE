using enums;
using enums.track;
using module.goods;
using module.track;
using resource;
using System;
using task.device;
using tool.appconfig;
using tool.timer;

namespace task.trans.transtask
{
    /// <summary>
    /// 任务基础逻辑类
    /// </summary>
    public abstract class BaseTaskTrans
    {
        internal TransMaster _M { private set; get; }
        internal MTimer mTimer;
        internal Track track, takeTrack;
        internal bool isload, isnotload, tileemptyneed, isftask;
        internal uint ferryTraid;
        internal string res = "", result = "";
        internal uint carrierid;
        internal bool allocatakeferry, allocagiveferry;

        public BaseTaskTrans(TransMaster trans)
        {
            _M = trans;
            mTimer = new MTimer();
        }

        internal void Clearn()
        {
            track = null;
            takeTrack = null;
            isload = false;
            isnotload = false;
            tileemptyneed = false;
            isftask = false;
            ferryTraid = 0;
            carrierid = 0;
            allocatakeferry = false;
            allocagiveferry = false;
            res = "";
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="trans"></param>
        public void DoTrans(StockTrans trans)
        {
            try
            {
                Clearn();

                #region[流程超时报警 - 默认超时10分钟则报警，倒库中流程则要2小时才报警]

                PubTask.Trans.CheckAndAddTransStatusOverTimeWarn(trans);

                #endregion

                switch (trans.TransStaus)
                {
                    case TransStatusE.调度设备:
                        AllocateDevice(trans);
                        break;
                    case TransStatusE.取砖流程:
                        ToTakeTrackTakeStock(trans);
                        break;
                    case TransStatusE.放砖流程:
                        ToGiveTrackGiveStock(trans);
                        break;
                    case TransStatusE.还车回轨:
                        ReturnDevBackToTrack(trans);
                        break;
                    case TransStatusE.倒库中:
                        SortingStock(trans);
                        break;
                    case TransStatusE.移车中:
                        MovingCarrier(trans);
                        break;
                    case TransStatusE.小车回轨:
                        ReturnCarrrier(trans);
                        break;
                    case TransStatusE.完成:
                        FinishStockTrans(trans);
                        FinishAndReleaseFerry(trans);
                        break;
                    case TransStatusE.取消:
                        CancelStockTrans(trans);
                        break;
                    case TransStatusE.检查轨道:
                        CheckingTrack(trans);
                        break;
                    case TransStatusE.倒库暂停:
                        SortTaskWait(trans);
                        break;
                    case TransStatusE.接力等待:
                        Out2OutRelayWait(trans);
                        break;
                    case TransStatusE.整理中:
                        Organizing(trans);
                        break;
                    case TransStatusE.其他:
                        OtherAction(trans);
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        #region[任务调度逻辑]

        /// <summary>
        /// 调度设备
        /// </summary>
        /// <param name="trans"></param>
        public abstract void AllocateDevice(StockTrans trans);

        /// <summary>
        /// 取砖流程
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ToTakeTrackTakeStock(StockTrans trans);

        /// <summary>
        /// 放砖流程
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ToGiveTrackGiveStock(StockTrans trans);

        /// <summary>
        /// 还车回轨
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ReturnDevBackToTrack(StockTrans trans);

        /// <summary>
        /// 倒库中
        /// </summary>
        /// <param name="trans"></param>
        public abstract void SortingStock(StockTrans trans);

        /// <summary>
        /// 移车中
        /// </summary>
        /// <param name="trans"></param>
        public abstract void MovingCarrier(StockTrans trans);

        /// <summary>
        /// 小车回轨
        /// </summary>
        /// <param name="trans"></param>
        public abstract void ReturnCarrrier(StockTrans trans);

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public abstract void FinishStockTrans(StockTrans trans);

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public abstract void CancelStockTrans(StockTrans trans);

        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public abstract void CheckingTrack(StockTrans trans);

        /// <summary>
        /// 倒库暂停
        /// </summary>
        /// <param name="trans"></param>
        public abstract void SortTaskWait(StockTrans trans);

        /// <summary>
        /// 接力等待
        /// </summary>
        /// <param name="trans"></param>
        public abstract void Out2OutRelayWait(StockTrans trans);

        public abstract void OtherAction(StockTrans trans);

        public abstract void Organizing(StockTrans trans);

        #endregion

        #region[其他判断]

        /// <summary>
        /// 完成任务的同时判断是否需要释放运输车
        /// </summary>
        /// <param name="trans"></param>
        private void FinishAndReleaseFerry(StockTrans trans)
        {
            if (trans.carrier_id == 0) return;
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null || track.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道)) return;
            if (trans.take_ferry_id != 0)
            {
                PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id);
            }

            if (trans.give_ferry_id != 0)
            {
                PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
            }
        }

        /// <summary>
        /// 判断是否满砖(返回下一车库存脉冲)
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackID"></param>
        /// <param name="stkLoc"></param>
        /// <returns></returns>
        public bool CheckTrackFull(StockTrans trans, uint trackID, out ushort stkLoc)
        {
            // 是否已满砖
            if (PubMaster.Track.IsTrackFull(trackID))
            {
                stkLoc = 0;
                return true;
            }

            bool isback = PubMaster.Track.IsGiveBackTrack(trackID);

            // 判断下一车库存脉冲
            if (!PubMaster.Goods.CalculateNextLocByDir(isback ? DevMoveDirectionE.后退 : DevMoveDirectionE.前进, trans.carrier_id, trackID, trans.stock_id, out stkLoc)
                || PubMaster.Goods.IsMoreThanFullQty(trans.area_id, trans.line, trackID))
            {
                // 设满砖
                PubMaster.Track.UpdateStockStatus(trackID, TrackStockStatusE.满砖, "计算坐标值无法存入下一车");
                PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trackID, TrackLogE.满轨道, "计算坐标值无法存入下一车");

                #region 【任务步骤记录】
                _M.LogForTrackFull(trans, trackID);
                #endregion

                return true;
            }

            // 判断是否库存数已到设定的上限
            if (PubMaster.Goods.IsMoreThanFullQty(trans.area_id, trans.line, trackID))
            {
                // 设满砖
                PubMaster.Track.UpdateStockStatus(trackID, TrackStockStatusE.满砖, "当前库存数已达上限，无法存入下一车");
                PubMaster.Track.AddTrackLog((ushort)trans.area_id, trans.carrier_id, trackID, TrackLogE.满轨道, "当前库存数已达上限，无法存入下一车");

                #region 【任务步骤记录】
                _M.LogForTrackFull(trans, trackID);
                #endregion

                return true;
            }

            return false;
        }

        #endregion

        #region[检测轨道并添加移车任务]

        /// <summary>
        /// 判断是否有不符规格的车在作业的轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool CheckGoodsAndAddMoveTask(StockTrans trans, uint trackid)
        {
            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
            // 是否有不符规格的车在轨道
            if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trackid, goodssizeID, out uint carrierid))
            {
                string carName = PubMaster.Device.GetDeviceName(carrierid);
                string traName = PubMaster.Track.GetTrackName(trackid);

                if (_M.HaveCarrierInTrans(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 100, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        carName, traName));
                    #endregion
                    return true;
                }

                if (!PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 101, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carName, traName));
                    #endregion
                    return true;
                }

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 102, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                        carName, traName));
                #endregion

                //转移到同类型轨道
                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trackid);
                track = PubTask.Carrier.GetCarrierTrack(carrierid);
                DeviceTypeE ferrytype = PubTask.Carrier.GetCarrierNeedFerryType(carrierid);
                _M.AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道, ferrytype);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查轨道一侧内运输车是否需要移走
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="isdown">是否移走下砖侧</param>
        /// <param name="res"></param>
        /// <returns></returns>
        internal bool CheckCarAndAddMoveTask(StockTrans trans, uint trackid, bool isdown, DeviceTypeE ferrytype = DeviceTypeE.其他)
        {
            res = "";
            if (PubTask.Carrier.HaveInTrackAndGet(trackid, out uint carrierid))
            {
                Track track = PubMaster.Track.GetTrack(trackid);
                CarrierTask carrier = PubTask.Carrier.GetDevCarrier(carrierid);

                if (!PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 103, string.Format("有运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

                if (_M.HaveCarrierInTrans(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 104, string.Format("有运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

                bool ismove = false;
                if (isdown)
                {
                    // 入库侧的移走
                    if (track.Type == TrackTypeE.储砖_入
                        || (track.Type == TrackTypeE.储砖_出入
                            && (track.is_give_back ? (carrier.CurrentPoint >= track.split_point) : (carrier.CurrentPoint <= track.split_point)))
                        )
                    {
                        ismove = true;
                    }
                }
                else
                {
                    // 出库侧的移走
                    if (track.Type == TrackTypeE.储砖_出
                        || (track.Type == TrackTypeE.储砖_出入
                            && (track.is_take_forward ? (carrier.CurrentPoint <= track.split_point) : (carrier.CurrentPoint >= track.split_point)))
                        )
                    {
                        ismove = true;
                    }
                }

                if (ismove)
                {
                    if (ferrytype == DeviceTypeE.其他) ferrytype = PubTask.Carrier.GetCarrierNeedFerryType(carrierid);

                    _M.AddMoveCarrierTask(track.id, carrierid, track.Type, MoveTypeE.转移占用轨道, ferrytype);

                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 105, string.Format("有运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// 检查轨道内是否存在其他运输车需要移走
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool CheckTrackAndAddMoveTask(StockTrans trans, uint trackid, DeviceTypeE ferrytype = DeviceTypeE.其他)
        {
            res = "";
            if (PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id, out uint othercarrierid))
            {
                Track track = PubMaster.Track.GetTrack(trackid);
                CarrierTask carrier = PubTask.Carrier.GetDevCarrier(othercarrierid);

                if (!PubTask.Carrier.IsCarrierFree(othercarrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 106, string.Format("有运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

                if (_M.HaveCarrierInTrans(othercarrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 107, string.Format("有运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        carrier.Device.name, track.name));
                    #endregion
                    return true;
                }

                if (ferrytype == DeviceTypeE.其他) ferrytype = PubTask.Carrier.GetCarrierNeedFerryType(othercarrierid);

                _M.AddMoveCarrierTask(track.id, othercarrierid, track.Type, MoveTypeE.转移占用轨道, ferrytype);

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 108, string.Format("有运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                    carrier.Device.name, track.name));
                #endregion
                return true;

            }

            return false;
        }

        #endregion

        #region[分配摆渡车]

        /// <summary>
        /// 是否成功分配取货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="track"></param>
        /// <param name="isfalsereturn"></param>
        public bool AllocateTakeFerry(StockTrans trans, DeviceTypeE ferrytype, Track track)
        {
            if (trans.HaveTakeFerry)
            {
                return PubTask.Ferry.TryLock(trans, trans.take_ferry_id, track.id);
            }
            else
            {
                string msg = _M.AllocateFerry(trans, ferrytype, track, false);
                // 失败
                if (_M.LogForTakeFerry(trans, msg)) return false;
                // 再锁
                trans.IsReleaseTakeFerry = false;

                return true;
            }
        }

        /// <summary>
        /// 是否成功分配放货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="track"></param>
        /// <param name="isfalsereturn"></param>
        public bool AllocateGiveFerry(StockTrans trans, DeviceTypeE ferrytype, Track track)
        {
            if (trans.HaveGiveFerry)
            {
                return PubTask.Ferry.TryLock(trans, trans.give_ferry_id, track.id);
            }
            else
            {
                string msg = _M.AllocateFerry(trans, ferrytype, track, true);
                // 失败
                if (_M.LogForTakeFerry(trans, msg)) return false;
                // 再锁
                trans.IsReleaseTakeFerry = false;

                return true;
            }
        }

        #endregion

        #region[完全释放摆渡车]

        /// <summary>
        /// 释放取货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseTakeFerry(StockTrans trans, string memo = "")
        {
            if (trans.HaveTakeFerry
                && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
            {
                trans.take_ferry_id = 0;
                trans.IsReleaseTakeFerry = true;

                _M.FreeTakeFerry(trans, memo);
            }
        }

        /// <summary>
        /// 释放送货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseGiveFerry(StockTrans trans, string memo = "")
        {
            if (trans.HaveGiveFerry
                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
            {
                trans.give_ferry_id = 0;
                trans.IsReleaseGiveFerry = true;

                _M.FreeGiveFerry(trans, memo);
            }
        }
        #endregion

        #region [运输车指令]

        #region 基础指令
        /// <summary>
        /// 移至轨道定位点
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        public void MoveToPos(uint trackID, uint carrierID, uint transID, CarrierPosE cp, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            CarrierActionOrder cao = new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.定位指令,
                CheckTra = toTrack.ferry_up_code,  // 无所谓了 反正都是同一个轨道编号
                ToTrackId = toTrack.id,
            };

            switch (cp)
            {
                case CarrierPosE.下砖机复位点:
                    cao.OverPoint = toTrack.limit_point_up;
                    break;
                case CarrierPosE.上砖机复位点:
                    cao.OverPoint = toTrack.limit_point;
                    break;
                case CarrierPosE.后置摆渡复位点:
                case CarrierPosE.前置摆渡复位点:
                    cao.OverPoint = toTrack.limit_point;  // 无所谓了 反正都是只有一个定位点
                    break;
                case CarrierPosE.轨道后侧定位点:
                case CarrierPosE.轨道后侧复位点:
                    cao.OverPoint = toTrack.limit_point;
                    break;
                case CarrierPosE.轨道中间复位点:
                    cao.OverPoint = toTrack.split_point;
                    break;
                case CarrierPosE.轨道前侧定位点:
                case CarrierPosE.轨道前侧复位点:
                    cao.OverPoint = toTrack.limit_point_up;
                    break;
                default:
                    break;
            }
            PubTask.Carrier.DoOrder(carrierID, trackID, cao, mes);
        }

        /// <summary>
        /// 移至指定脉冲
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        public void MoveToLoc(uint trackID, uint carrierID, uint transID, ushort loc, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 至指定脉冲
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.定位指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                OverPoint = loc,
                ToTrackId = toTrack.id
            }, mes);
        }

        /// <summary>
        /// 移至指定脉冲取砖
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        public void MoveToTake(uint trackID, uint carrierID, uint transID, ushort loc, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 至指定脉冲取砖
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.取砖指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                ToPoint = loc,
                OverPoint = toTrack.is_take_forward ? toTrack.limit_point : toTrack.limit_point_up, // 前进取则后侧停，后退取则前侧停
                ToTrackId = toTrack.id
            }, mes);
        }

        /// <summary>
        /// 移至指定脉冲存砖
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        public void MoveToGive(uint trackID, uint carrierID, uint transID, ushort loc, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 至指定脉冲放砖
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.放砖指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                ToPoint = loc,
                OverPoint = toTrack.is_give_back ? toTrack.limit_point_up : toTrack.limit_point, // 后退存则前侧停，前进存则后侧停
                ToTrackId = toTrack.id
            }, mes);
        }

        /// <summary>
        /// 指定脉冲倒库
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="loc"></param>
        public void MoveToSort(uint trackID, uint carrierID, uint transID, ushort locTake, ushort locGive, string mes = "")
        {
            // 目的轨道
            Track toTrack = PubMaster.Track.GetTrack(trackID);

            // 至指定脉冲倒库
            PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
            {
                Order = DevCarrierOrderE.倒库指令,
                CheckTra = toTrack.ferry_up_code, // 无所谓了 反正都是同一个轨道编号
                ToPoint = locTake,
                OverPoint = locGive,
                ToTrackId = toTrack.id
            }, mes);
        }

        #endregion


        /// <summary>
        /// 运输车直接取砖 By 指定库存
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="mes"></param>
        public void TakeInTarck(uint stockID, uint trackID, uint carrierID, uint transID, out string mes)
        {
            // 获取库存
            Stock stk = PubMaster.Goods.GetStock(stockID);
            if (stk == null)
            {
                mes = string.Format("无库存[ id = {0} ]数据，无法取砖", stockID);
                return;
            }

            // 获取目的轨道
            Track tra = PubMaster.Track.GetTrack(trackID);
            if (tra == null)
            {
                mes = string.Format("无轨道[ id = {0} ]数据，无法取砖", trackID);
                return;
            }

            if (stk.track_id != tra.id)
            {
                mes = string.Format("库存[ {0} ]不在目的轨道[ {1} ]，无法取砖",
                    PubMaster.Track.GetTrackName(stk.track_id), tra.name);
                return;
            }

            // 获取小车当前脉冲
            ushort carloc = PubTask.Carrier.GetCurrentPoint(carrierID);

            // 取砖最小前提 ±10脉冲
            ushort safeloc = 10;

            if (stk.location == 0)
            {
                // 先回轨道头再靠光电取
                if (tra.is_take_forward && carloc > (tra.limit_point + safeloc))
                {
                    // 前进取 - 回后侧点
                    mes = "无库存脉冲，尝试前进取砖先回轨道后侧点";
                    MoveToPos(tra.id, carrierID, transID, CarrierPosE.轨道后侧定位点);
                    return;
                }

                if (!tra.is_take_forward && carloc < (tra.limit_point_up - safeloc))
                {
                    // 后退取 - 回前侧点
                    mes = "无库存脉冲，尝试后退取砖先回轨道前侧点";
                    MoveToPos(tra.id, carrierID, transID, CarrierPosE.轨道前侧定位点);
                    return;
                }

                // 直接靠光电取砖 前-65535；后-1
                mes = "无库存脉冲，尝试直接靠光电取砖";
                MoveToTake(tra.id, carrierID, transID, (ushort)(tra.is_take_forward ? 65535 : 1));
                return;
            }
            else
            {
                ushort toloc = (ushort)(tra.is_take_forward ? (stk.location - safeloc) : (stk.location + safeloc));
                if (tra.is_take_forward && carloc > toloc)
                {
                    // 前进取 -小车要在库存位后至少 10 脉冲；
                    mes = "前进取砖，小车需先到合适位置";
                    MoveToLoc(tra.id, carrierID, transID, toloc);
                    return;
                }

                if (!tra.is_take_forward && carloc < toloc)
                {
                    // 后退取 -小车要在库存位前至少 10 脉冲；
                    mes = "后退取砖，小车需先到合适位置";
                    MoveToLoc(tra.id, carrierID, transID, toloc);
                    return;
                }

                // 取砖
                mes = "执行取砖";
                MoveToTake(tra.id, carrierID, transID, stk.location);
                return;
            }

        }

        /// <summary>
        /// 运输车直接放砖 By 指定脉冲
        /// </summary>
        /// <param name="stkloc"></param>
        /// <param name="trackID"></param>
        /// <param name="carrierID"></param>
        /// <param name="transID"></param>
        /// <param name="mes"></param>
        public void GiveInTarck(ushort stkloc, uint trackID, uint carrierID, uint transID, out string mes)
        {
            if (stkloc == 0)
            {
                mes = "无效库存存放位置0！";
                return;
            }

            // 获取目的轨道
            Track tra = PubMaster.Track.GetTrack(trackID);
            if (tra == null)
            {
                mes = string.Format("无轨道[ id = {0} ]数据，无法取砖", trackID);
                return;
            }

            // 获取小车当前脉冲
            ushort carloc = PubTask.Carrier.GetCurrentPoint(carrierID);

            // 放砖最小前提 ±20脉冲
            ushort safeloc = 20;

            if (tra.is_give_back && carloc < (stkloc - safeloc))
            {
                mes = string.Format("后退放砖-小车位置[ {0} ]无法后退到存放位置[ {1} ], 则原地放砖", carloc, stkloc);
                PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.放砖指令
                });
                return;
            }

            if (!tra.is_give_back && carloc > (stkloc + safeloc))
            {
                mes = string.Format("前进放砖-小车位置[ {0} ]无法前进到存放位置[ {1} ], 则原地放砖", carloc, stkloc);
                PubTask.Carrier.DoOrder(carrierID, transID, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.放砖指令
                });
                return;
            }

            // 放砖
            mes = "执行放砖";
            MoveToGive(trackID, carrierID, transID, stkloc);
            return;
        }

        #endregion

        #region [库存转移 - 轨道内倒库]

        /// <summary>
        /// 获取倒库取砖位置
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="overPoint"></param>
        /// <param name="splitPoint"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool GetTransferTakePoint(uint trackid, int overPoint, int splitPoint, out ushort loc)
        {
            loc = 0;
            // 获取分界点后 最前的库存
            Stock stk = PubMaster.Goods.GetStockBehindStockPoint(trackid, splitPoint);
            if (stk == null) return false;

            loc = stk.location;
            ushort limit = 50; // 误差范围
            bool isforward = PubMaster.Track.IsTakeForwardTrack(trackid);
            // 判断是否超过结束点
            if (isforward ? (loc > (overPoint + limit)) : (loc < (overPoint - limit)))
            {
                return false;
            }

            return loc > 0;
        }

        /// <summary>
        /// 获取倒库放砖位置
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="limitPoint"></param>
        /// <param name="splitPoint"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool GetTransferGivePoint(uint trackid, uint carrierid, int limitPoint, int splitPoint, out ushort loc)
        {
            // 获取分界点前 最后的库存
            bool isforward = PubMaster.Track.IsTakeForwardTrack(trackid);
            Stock stk = PubMaster.Goods.GetStockInfrontStockPoint(trackid, splitPoint);
            // 计算下一车位置
            if (PubMaster.Goods.CalculateNextLocByStock(isforward ? DevMoveDirectionE.前进 : DevMoveDirectionE.后退, stk, out loc, carrierid))
            {
                // 判断是否超过分界点
                if (isforward ? (loc > splitPoint) : (loc < splitPoint))
                {
                    return false;
                }
            }
            else
            {
                loc = (ushort)limitPoint;
            }

            return loc > 0;
        }

        /// <summary>
        /// 获取倒库待命位置
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="overPoint"></param>
        /// <param name="splitPoint"></param>
        /// <returns></returns>
        public ushort GetTransferWaitPoint(uint trackid, uint carrierid, int overPoint, int splitPoint)
        {
            // 获取轨道数据
            Track track = PubMaster.Track.GetTrack(trackid);
            // 待命点
            int loc = 0;

            // 获取分界点前 最后的库存
            Stock stk = PubMaster.Goods.GetStockInfrontStockPoint(trackid, splitPoint);
            if (stk != null)
            {
                // 安全距离
                ushort safe = PubMaster.Goods.GetStackSafe(stk.goods_id, carrierid);
                // 运输车等待的时候需要后退几个车身
                ushort carspace = GlobalWcsDataConfig.BigConifg.GetSortWaitNumberCarSpace(track.area, track.line);
                carspace = (ushort)(carspace * safe);

                loc = track.is_take_forward ? (stk.location + carspace) : (stk.location - carspace);
            }

            // 获取分界点后 最前的库存
            Stock nextstk = PubMaster.Goods.GetStockBehindStockPoint(trackid, splitPoint);
            if (nextstk != null)
            {
                // 定最远的
                if (loc == 0 || track.is_take_forward ? (nextstk.location > loc) : (nextstk.location < loc))
                {
                    loc = nextstk.location;
                }
            }

            // 判断是否超过结束点
            if (loc == 0 || track.is_take_forward ? (loc > overPoint) : (loc < overPoint))
            {
                return (ushort)overPoint;
            }

            return (ushort)loc;
        }



        /// <summary>
        /// 是否倒库转移结束
        /// </summary>
        /// <param name="trackid">轨道ID</param>
        /// <param name="overPoint">结束点</param>
        /// <param name="splitPoint">分界点</param>
        /// <returns></returns>
        public bool IsTransferOver(uint trackid, int overPoint, int splitPoint)
        {
            // 结束点前是否有库存
            if (!PubMaster.Goods.ExistInfrontUpSplitPoint(trackid, overPoint, out int stkcount))
            {
                return true;
            }

            // 分界点后是否有库存
            if (!PubMaster.Goods.ExistBehindUpSplitPoint(trackid, splitPoint, out stkcount))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取倒库取放位置
        /// </summary>
        /// <param name="transid"></param>
        /// <param name="trackid"></param>
        /// <param name="carrierid"></param>
        /// <param name="overPoint"></param>
        /// <param name="splitPoint"></param>
        /// <param name="limitPoint"></param>
        /// <param name="mes"></param>
        /// <param name="locTake"></param>
        /// <param name="locGive"></param>
        /// <returns></returns>
        public bool GetTransferTGpoint(uint transid, uint trackid, uint carrierid, int overPoint, int splitPoint, int limitPoint,
            out string mes, out ushort locTake, out ushort locGive)
        {
            locTake = 0;
            locGive = 0;

            if (!GetTransferTakePoint(trackid, overPoint, splitPoint, out locTake))
            {
                mes = "无合适取货位置";
                return false;
            }

            if (!GetTransferGivePoint(trackid, carrierid, limitPoint, splitPoint, out locGive))
            {
                mes = "无合适卸货位置";
                return false;
            }

            // 取放位置间距
            ushort dis = 100;
            if (Math.Abs(locTake - locGive) <= dis)
            {
                mes = "取放位置相隔过小，无倒库必要";
                return false;
            }

            mes = "";
            return true;
        }

        #endregion

        #region [无缝上摆渡]

        /// <summary>
        /// 无缝上摆渡
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="istake"></param>
        public void MoveToFerrySeamless(StockTrans trans, bool istake)
        {
            CarrierTask car = PubTask.Carrier.GetDevCarrier(trans.carrier_id);
            if (car == null) return;

            Track carTrack = PubMaster.Track.GetTrack(car.CurrentTrackId);
            if (carTrack == null) return;
            if (carTrack.InType(TrackTypeE.前置摆渡轨道, TrackTypeE.后置摆渡轨道)) return;

            // 小车根据摆渡类型需要先定位到轨道头
            ushort tosite = 0;
            switch (trans.AllocateFerryType)
            {
                case DeviceTypeE.前摆渡:
                    tosite = carTrack.limit_point_up;
                    break;
                case DeviceTypeE.后摆渡:
                    tosite = carTrack.limit_point;
                    break;
            }
            if (tosite == 0) return;

            // 离轨道头范围 ≈20M
            if (Math.Abs(tosite - car.CurrentPoint) <= 1153)
            {
                // 范围内
                // 锁定摆渡  
                bool isFerryOK = false;  // 是否摆渡到位
                uint ferryID = istake ? trans.take_ferry_id : trans.give_ferry_id;
                if (istake ? AllocateTakeFerry(trans, trans.AllocateFerryType, carTrack) : AllocateGiveFerry(trans, trans.AllocateFerryType, carTrack))
                {
                    // 定位摆渡  
                    if (_M.LockFerryAndAction(trans, ferryID, carTrack.id, carTrack.id, out ferryTraid, out result, true))
                    {
                        isFerryOK = true;
                    }
                    else
                    {
                        #region 【任务步骤记录】
                        _M.LogForFerryMove(trans, ferryID, carTrack.id, result);
                        #endregion
                    }
                }

                // 无缝上摆渡
                if (car.IsStopNoOrder(out result))
                {
                    // 停止：直接定位摆渡
                    if (isFerryOK)
                    {
                        //至摆渡车
                        MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToFerry(trans, carTrack.id, ferryID);
                        #endregion
                        return;
                    }

                    // 停止：摆渡到位前先到轨道头
                    if (!isFerryOK && Math.Abs(tosite - car.CurrentPoint) > 10)
                    {
                        // 移至轨道定位点
                        MoveToLoc(carTrack.id, trans.carrier_id, trans.id, tosite);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToTrack(trans, carTrack.id);
                        #endregion
                        return;
                    }
                }
                else
                {
                    // 运动：目的点轨道头则改为摆渡
                    if (isFerryOK && PubTask.Carrier.IsCarrierTargetMatches(trans.carrier_id, 0, tosite))
                    {
                        //至摆渡车
                        MoveToPos(ferryTraid, trans.carrier_id, trans.id, CarrierPosE.前置摆渡复位点);

                        #region 【任务步骤记录】
                        _M.LogForCarrierToFerry(trans, carTrack.id, ferryID);
                        #endregion
                        return;
                    }
                }

            }
            else
            {
                // 范围外
                if (car.IsStopNoOrder(out result))
                {
                    // 停止：定位到轨道头
                    MoveToLoc(carTrack.id, trans.carrier_id, trans.id, tosite);

                    #region 【任务步骤记录】
                    _M.LogForCarrierToTrack(trans, carTrack.id);
                    #endregion
                    return;
                }
                else
                {
                    // 运动：查询最新状态
                    car.DoQuery();
                    return;
                }
            }

        }

        #endregion

    }
}
