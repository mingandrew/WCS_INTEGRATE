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
        internal TransMaster _M {private set; get; }
        internal MTimer mTimer;
        internal Track track, takeTrack;
        internal bool isload, isnotload, tileemptyneed, ftask;
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
            ftask = false;
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
        public  void DoTrans(StockTrans trans)
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
            }catch(Exception ex)
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
            if (track == null || track.InType(TrackTypeE.摆渡车_入, TrackTypeE.摆渡车_出)) return;
            if (trans.take_ferry_id != 0)
            {
                PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id);
            }

            if (trans.give_ferry_id != 0)
            {
                PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id);
            }
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
            CarrierTypeE carrier = PubMaster.Goods.GetGoodsCarrierType(trans.goods_id);
            bool haveintrack = PubTask.Carrier.HaveDifTypeInTrack(trackid, carrier, out uint carrierid);

            if (!haveintrack && trans.carrier_id != 0)
            {
                haveintrack = PubTask.Carrier.HaveInTrack(trackid, trans.carrier_id, out carrierid);
            }

            if (haveintrack && !_M.HaveCarrierInTrans(carrierid))
            {
                if (PubTask.Carrier.IsCarrierFree(carrierid))
                {
                    if (PubTask.Carrier.IsLoad(carrierid))
                    {
                        //下降放货
                        PubTask.Carrier.DoOrder(carrierid, trans.id, new CarrierActionOrder()
                        {
                            Order = DevCarrierOrderE.放砖指令
                        }, "库存转移下降放货");
                    }
                    else
                    {
                        //转移到同类型轨道
                        TrackTypeE tracktype = PubMaster.Track.GetTrackType(trackid);
                        _M.AddMoveCarrierTask(trackid, carrierid, tracktype, MoveTypeE.转移占用轨道, ferytype);
                    }
                }
            }

            return haveintrack;
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
        public void RealseTakeFerry(StockTrans trans, string memo = "")
        {
            if (!trans.IsReleaseTakeFerry
                && PubTask.Ferry.IsUnLoad(trans.take_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.take_ferry_id))
            {
                _M.FreeTakeFerry(trans, memo);

                trans.take_ferry_id = 0;
            }
        }

        /// <summary>
        /// 是否送货摆渡车
        /// </summary>
        /// <param name="trans"></param>
        public void RealseGiveFerry(StockTrans trans, string memo = "")
        {
            if (!trans.IsReleaseGiveFerry
                && PubTask.Ferry.IsUnLoad(trans.give_ferry_id)
                && PubTask.Ferry.UnlockFerry(trans, trans.give_ferry_id))
            {
                _M.FreeGiveFerry(trans, memo);

                trans.give_ferry_id = 0;
            }
        }
        #endregion
    }
}
