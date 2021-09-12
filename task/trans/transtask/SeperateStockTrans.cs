using enums;
using module.goods;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task.trans.transtask
{
    /// <summary>
    /// 库存整理（code- XX10）
    /// 1.单品种库存，转移轨道
    /// 2.多品种库存，分开轨道
    /// </summary>
    public class SeperateStockTrans : BaseTaskTrans
    {
        /**
         * 1.检测轨道库存
         * 2.根据头部库存生成
         * 3.根据库存是否需要转移
         * 4.全部转移完毕后完成任务
         */
        public SeperateStockTrans(TransMaster trans) : base(trans)
        {

        }

        /// <summary>
        /// 整理中
        /// 1.获取轨道头部库存
        /// 2.判断头部库存品种是否需要生成转移库存任务
        /// </summary>
        /// <param name="trans"></param>
        public override void Organizing(StockTrans trans)
        {
            Stock top = PubMaster.Goods.GetStockForOut(trans.take_track_id);
            List<StockTransDtl> dtl = _M.GetTransDtls(trans.id);
            foreach (var item in dtl)
            {
                CheckTransDtlAndAddMoveStockTrans(trans, item, top);
            }

            //判断所有细单都完成了
            CheckAllDtlFinish(trans, dtl);
        }


        /// <summary>
        /// 检测细单
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dtl"></param>
        private void CheckTransDtlAndAddMoveStockTrans(StockTrans trans, StockTransDtl dtl, Stock top)
        {
            switch (dtl.DtlType)
            {
                case StockTransDtlTypeE.上砖品种:
                    DoUpGood(trans, dtl, top);
                    break;
                case StockTransDtlTypeE.转移品种:
                    DoMoveGood(trans, dtl, top);
                    break;
                case StockTransDtlTypeE.保留品种:
                    DoStayGood(trans, dtl, top);
                    break;
            }
        }

        /// <summary>
        /// 上砖品种
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dtl"></param>
        /// <param name="top"></param>
        private void DoUpGood(StockTrans trans, StockTransDtl dtl, Stock top)
        {

        }

        /// <summary>
        /// 库存转移品种
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dtl"></param>
        /// <param name="top"></param>
        private void DoMoveGood(StockTrans trans, StockTransDtl dtl, Stock top)
        {
            if (dtl.DtlStatus == StockTransDtlStatusE.完成) return;

            if (dtl.dtl_trans_id != 0 && dtl.DtlStatus != StockTransDtlStatusE.完成 && !_M.IsTransFinish(dtl.dtl_trans_id))
            {
                return;
            }

            if (top == null || dtl.dtl_left_qty == 0 || !PubMaster.Goods.HaveGoodInTrack(dtl.dtl_take_track_id, dtl.dtl_good_id, top.level))
            {
                if (dtl.dtl_trans_id == 0
                    && !PubMaster.Goods.HaveGoodInTrack(dtl.dtl_take_track_id, dtl.dtl_good_id, top.level))
                {
                    _M.SetDtlStatus(dtl, StockTransDtlStatusE.完成);
                }
            }

            if (top != null)
            {
                if (dtl.EqualGoodAndLevel(top.goods_id, top.level)
                    && dtl.dtl_trans_id == 0
                    && !_M.ExistTransWithTrackAndType(trans.take_track_id, TransTypeE.库存转移)
                    && !_M.ExistTransWithTrackButType(trans.take_track_id, TransTypeE.库存整理))
                {
                    uint transid = _M.AddTransWithoutLock(trans.area_id, 0, TransTypeE.库存转移, top.goods_id, top.level, top.id, trans.take_track_id, dtl.dtl_give_track_id, TransStatusE.检查轨道, 0, trans.line, DeviceTypeE.前摆渡);

                    _M.SetDtlTransId(dtl, transid);
                }
            }
        }

        /// <summary>
        /// 保留品种
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dtl"></param>
        /// <param name="top"></param>
        private void DoStayGood(StockTrans trans, StockTransDtl dtl, Stock top)
        {

        }


        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public override void FinishStockTrans(StockTrans trans)
        {
            if (_M.SetAllTransDtlFinish(trans.id))
            {
                _M.SetFinish(trans);
            }
        }

        /// <summary>
        /// 检测所有细单任务是否完成<br/>
        /// 如果全部完成则完成整理任务
        /// </summary>
        private void CheckAllDtlFinish(StockTrans trans, List<StockTransDtl> dtl)
        {
            //全部都已经完成
            if (!dtl.Exists(c => c.DtlStatus != StockTransDtlStatusE.完成))
            {
                _M.SetStatus(trans, TransStatusE.完成);
            }
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {

        }

        #region[其他流程]

        /// <summary>
        /// 检查轨道
        /// </summary>
        /// <param name="trans"></param>
        public override void CheckingTrack(StockTrans trans)
        {

        }


        /// <summary>
        /// 调度设备
        /// </summary>
        /// <param name="trans"></param>
        public override void AllocateDevice(StockTrans trans)
        {

        }

        /// <summary>
        /// 移车中
        /// </summary>
        /// <param name="trans"></param>
        public override void MovingCarrier(StockTrans trans)
        {

        }

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

        /// <summary>
        /// 倒库中
        /// </summary>
        /// <param name="trans"></param>
        public override void SortingStock(StockTrans trans)
        {

        }

        /// <summary>
        /// 运输车回轨
        /// </summary>
        /// <param name="trans"></param>
        public override void ReturnCarrrier(StockTrans trans)
        {

        }
        /// <summary>
        /// 倒库暂停
        /// </summary>
        /// <param name="trans"></param>
        public override void SortTaskWait(StockTrans trans)
        {

        }

        /// <summary>
        /// 接力等待
        /// </summary>
        /// <param name="trans"></param>
        public override void Out2OutRelayWait(StockTrans trans)
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
