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
    /// 中转库存（倒库？从当前轨道由中转摆渡移至其他轨道）
    /// </summary>
    class TransitStockTrans : BaseTaskTrans
    {
        /**
         * 1.检测轨道库存
         * 2.根据库存生成
         * 3.获取转移的目的轨道
         * 4.全部转移完毕后完成任务
         */
        public TransitStockTrans(TransMaster trans) : base(trans)
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
            if (_M.HaveTaskUsedTakeTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1012, string.Format("存在子任务进行中"));
                #endregion
                return;
            }

            Stock stk = PubMaster.Goods.GetStockForOut(trans.take_track_id);

            if (stk == null)
            {
                _M.SetStatus(trans, TransStatusE.完成);
                return;
            }
            else
            {
                // 已存在同品种中转则跳过
                if (_M.ExistTaskSameGoods(trans.area_id, trans.line, trans.id, stk.goods_id, stk.level, TransTypeE.中转倒库))
                {
                    #region 【任务步骤记录】
                    _M.SetStepLog(trans, false, 1112, string.Format("存在相同品种的中转任务"));
                    #endregion
                    return;
                }

                _M.SetGoods(trans, stk.goods_id);
                
                DoMoveStock(trans, stk);
            }
        }

        /// <summary>
        /// 库存转移
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dtl"></param>
        /// <param name="top"></param>
        private void DoMoveStock(StockTrans trans, Stock top)
        {
            if (top != null)
            {
                if ( !_M.ExistUnFinishTrans(trans.area_id, trans.take_track_id, TransTypeE.库存转移)
                    && !_M.ExistTransWithTrackButType(trans.take_track_id, TransTypeE.中转倒库))
                {
                    List<uint> trackids = PubMaster.Track.GetOutTrackIDByInTrack(trans.take_track_id, top.goods_id);
                    uint trackid = 0;
                    foreach (uint traid in trackids)
                    {
                        if (!_M.ExistTransWithTracks(traid))
                        {
                            trackid = traid;
                            break;
                        }
                    }

                    if (trackid > 0)
                    {
                        uint transid = _M.AddTransWithoutLock(trans.area_id, 0, TransTypeE.库存转移, top.goods_id, top.id,
                            trans.take_track_id, trackid, TransStatusE.检查轨道, 0, trans.line, trans.AllocateFerryType);

                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1112, string.Format("生成库存转移任务 [ID：{0}]", transid));
                        #endregion
                    }
                    else
                    {
                        #region 【任务步骤记录】
                        _M.SetStepLog(trans, false, 1212, string.Format("找不到合适轨道中转存砖"));
                        #endregion
                    }

                }
            }
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="trans"></param>
        public override void FinishStockTrans(StockTrans trans)
        {
            PubMaster.Warn.RemoveTaskAllWarn(trans.id);
            _M.SetFinish(trans);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="trans"></param>
        public override void CancelStockTrans(StockTrans trans)
        {
            if (_M.HaveTaskUsedTakeTrackId(trans))
            {
                #region 【任务步骤记录】
                _M.SetStepLog(trans, false, 1312, string.Format("等待子任务完成"));
                #endregion
                return;
            }

            _M.SetStatus(trans, TransStatusE.完成);
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
