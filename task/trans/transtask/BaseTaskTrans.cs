using enums;
using enums.track;
using module.goods;
using module.track;
using System;
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
        internal Track track;
        internal bool isload, isnotload, tileemptyneed;
        internal uint ferryTraid;
        internal string res = "", result = "";
        internal uint carrierid;
        public BaseTaskTrans(TransMaster trans)
        {
            _M = trans;
            mTimer = new MTimer();
        }

        internal void Clearn()
        {
            track = null;
            isload = false;
            isnotload = false;
            tileemptyneed = false;
            ferryTraid = 0;
            res = "";
            carrierid = 0;
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
    }
}
