using enums;
using enums.track;
using module.goods;
using module.track;
using resource;
using System;
using task.device;
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
        internal bool allocatakeferry, allocagiveferry, isfalsereturn;

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
        /// 判断是否有其他车在需要作业的轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool CheckTrackAndAddMoveTask(StockTrans trans, uint trackid, DeviceTypeE ferytype = DeviceTypeE.其他)
        {
            // 获取任务品种规格ID
            uint goodssizeID = PubMaster.Goods.GetGoodsSizeID(trans.goods_id);
            // 是否有不符规格的车在轨道
            if (PubTask.Carrier.HaveDifGoodsSizeInTrack(trackid, goodssizeID, out uint carrierid))
            {
                if (_M.HaveCarrierInTrans(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 100, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，绑定有任务，等待其任务完成；",
                        PubMaster.Device.GetDeviceName(carrierid),
                        PubMaster.Track.GetTrackName(trackid)));
                    #endregion
                    return true;
                }

                if (!PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 101, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，状态不满足(需通讯正常且启用，停止且无执行指令)；",
                        PubMaster.Device.GetDeviceName(carrierid),
                        PubMaster.Track.GetTrackName(trackid)));
                    #endregion
                    return true;
                }

                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 102, string.Format("有不符合规格作业要求的运输车[ {0} ]停在[ {1} ]，尝试对其生成移车任务；",
                    PubMaster.Device.GetDeviceName(carrierid),
                    PubMaster.Track.GetTrackName(trackid)));
                #endregion

                //转移到同类型轨道
                TrackTypeE tracktype = PubMaster.Track.GetTrackType(trackid);
                track = PubTask.Carrier.GetCarrierTrack(carrierid);
                _M.AddMoveCarrierTask(track.id, carrierid, tracktype, MoveTypeE.转移占用轨道);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region[分配摆渡车]

        /// <summary>
        /// 分配取货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="track"></param>
        /// <param name="isfalsereturn"></param>
        public void AllocateTakeFerry(StockTrans trans, DeviceTypeE ferrytype, Track track, out bool isfalsereturn)
        {
            if (trans.take_ferry_id == 0
               && !trans.IsReleaseTakeFerry)
            {
                string msg = _M.AllocateFerry(trans, ferrytype, track, false);

                #region 【任务步骤记录】
                if (_M.LogForTakeFerry(trans, msg))
                {
                    isfalsereturn = true;
                    return;
                }
                #endregion
            }
            isfalsereturn = false;
        }


        /// <summary>
        /// 分配放货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="ferrytype"></param>
        /// <param name="track"></param>
        /// <param name="isfalsereturn"></param>
        public void AllocateGiveFerry(StockTrans trans, DeviceTypeE ferrytype, Track track, out bool isfalsereturn)
        {
            if (trans.give_ferry_id == 0
               && !trans.IsReleaseGiveFerry)
            {
                string msg = _M.AllocateFerry(trans, ferrytype, track, true);

                #region 【任务步骤记录】
                if (_M.LogForGiveFerry(trans, msg))
                {
                    isfalsereturn = true;
                    return;
                }
                #endregion
            }
            isfalsereturn = false;
        }

        #endregion

        #region[完全释放摆渡车]

        /// <summary>
        /// 释放取货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseTakeFerry(StockTrans trans)
        {
            if (!trans.IsReleaseTakeFerry
                && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
            {
                _M.FreeTakeFerry(trans);

                trans.take_ferry_id = 0;
            }
        }

        /// <summary>
        /// 是否送货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseGiveFerry(StockTrans trans)
        {
            if (!trans.IsReleaseGiveFerry
                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
            {
                _M.FreeGiveFerry(trans);

                trans.give_ferry_id = 0;
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
            // 获取库存脉冲
            ushort stkloc = PubMaster.Goods.GetStockLocation(stockID);
            ushort carloc = PubTask.Carrier.GetCurrentPoint(carrierID);
            ushort safeloc = 10; // ±10脉冲
            bool isforward = PubMaster.Track.IsTakeForwardTrack(trackID);
            if (stkloc == 0)
            {
                // 先回轨道头再靠光电取
                if (isforward && carloc < (track.limit_point + safeloc))
                {
                    // 前进取 - 回后侧点
                    mes = "无库存脉冲，尝试前进取砖先回轨道后侧点";
                    MoveToPos(track.id, carrierID, transID, CarrierPosE.轨道后侧定位点);
                    return;
                }
                else if (!isforward && carloc < (track.limit_point_up - safeloc))
                {
                    // 后退取 - 回前侧点
                    mes = "无库存脉冲，尝试后退取砖先回轨道前侧点";
                    MoveToPos(track.id, carrierID, transID, CarrierPosE.轨道前侧定位点);
                    return;
                }
                else
                {
                    // 直接靠光电取砖 前-65535；后-1
                    mes = "无库存脉冲，尝试直接靠光电取砖";
                    MoveToTake(track.id, carrierID, transID, (ushort)(isforward ? 65535 : 1));
                    return;
                }

            }
            else
            {
                ushort toloc = (ushort)(isforward ? (stkloc - safeloc) : (stkloc + safeloc));
                if (isforward && carloc > toloc)
                {
                    // 前进取 -小车要在库存位后至少 10 脉冲；
                    mes = "前进取砖，小车需先到合适位置";
                    MoveToLoc(track.id, carrierID, transID, toloc);
                    return;
                }
                else if (!isforward && carloc < toloc)
                {
                    // 后退取 -小车要在库存位前至少 10 脉冲；
                    mes = "后退取砖，小车需先到合适位置";
                    MoveToLoc(track.id, carrierID, transID, toloc);
                    return;
                }
                else
                {
                    // 取砖
                    mes = "执行取砖";
                    MoveToTake(track.id, carrierID, transID, stkloc);
                    return;
                }

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
                mes = "无效库存存放位置！";
                return;
            }

            // 当前位置是否异常
            ushort carloc = PubTask.Carrier.GetCurrentPoint(carrierID);
            ushort safeloc = 20; // ±20脉冲
            bool isback = PubMaster.Track.IsGiveBackTrack(trackID);

            if (isback && carloc < (stkloc - safeloc))
            {
                mes = "后退放砖-小车位置再无法后退到库存存放位置！";
                return;
            }

            if (isback && carloc > (stkloc + safeloc))
            {
                mes = "前进放砖-小车位置再无法前进到库存存放位置！";
                return;
            }

            // 放砖
            mes = "执行放砖";
            MoveToGive(track.id, carrierID, transID, stkloc);
            return;
        }

        #endregion

    }
}
