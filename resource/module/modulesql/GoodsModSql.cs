using enums;
using module.goods;
using System;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class GoodsModSql : BaseModSql
    {
        public GoodsModSql(MySql mysql) : base(mysql)
        {

        }

        #region[查询]

        public List<Goods> QueryGoodsList()
        {
            List<Goods> list = new List<Goods>();
            string sql = string.Format("SELECT t.* FROM goods AS t ORDER BY t.top desc, t.createtime DESC, t.updatetime DESC ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Goods>();
            }
            return list;
        }

        public List<Stock> QueryStockList()
        {
            List<Stock> list = new List<Stock>();
            string sql = string.Format("SELECT t.* FROM stock AS t  ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Stock>();
            }
            return list;
        }

        public List<StockTrans> QueryStockTransList()
        {
            List<StockTrans> list = new List<StockTrans>();
            string sql = string.Format("SELECT t.* FROM stock_trans AS t" +
                " where t.finish is NULL AND t.trans_status <> {0}", (byte)TransStatusE.完成);
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<StockTrans>();
            }
            return list;
        }

        public StockTrans QueryStockTransById(uint transid)
        {
            List<StockTrans> list = new List<StockTrans>();
            string sql = string.Format("SELECT t.* FROM stock_trans AS t where t.id ={0} ", transid);
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<StockTrans>();
            }

            if(list.Count == 1)
            {
                return list[0];
            }

            return null;
        }

        public List<StockSum> QueryStockSumList()
        {
            List<StockSum> list = new List<StockSum>();
            string sql = string.Format("SELECT t.goods_id, t.track_id, t.produce_time, t.count," +
                " t.stack, t.pieces, t.area, t.track_type FROM stock_sum AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<StockSum>();
            }
            return list;
        }

        public List<GoodSize> QueryGoodSize()
        {
            List<GoodSize> list = new List<GoodSize>();
            string sql = string.Format("SELECT t.* FROM good_size AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<GoodSize>();
            }
            return list;
        }

        /// <summary>
        /// 查询任务细单信息列表
        /// </summary>
        /// <returns></returns>
        public List<StockTransDtl> QueryTransDtlList()
        {
            List<StockTransDtl> list = new List<StockTransDtl>();
            string sql = string.Format("select * from stock_trans_dtl where dtl_finish is null or dtl_finish = 0");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<StockTransDtl>();
            }
            return list;
        }
        #endregion

        #region[添加]


        internal bool AddGoods(Goods goods)
        {
            string str = "INSERT INTO `goods`(`id`, `area_id`, `name`, `color`, `pieces`, `carriertype`, `memo`, `updatetime`, `info`, `size_id`, `level`, `createtime`) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, '{8}', {9}, {10}, {11})";
            string sql = string.Format(@str, goods.id, goods.area_id, goods.name, goods.color, goods.pieces,
                goods.carriertype, goods.memo, GetTimeOrNull(goods.updatetime), goods.info, goods.size_id, goods.level, GetTimeOrNull(goods.createtime));
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        internal bool AddStock(Stock stock)
        {
            string str = "INSERT INTO `stock`(`id`, `goods_id`, `stack`, `pieces`, `track_id`" +
                ", `produce_time`, `pos`, `pos_type`, `tilelifter_id`, `area`, `track_type`, `location`, `location_cal`) " +
                "VALUES('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})";
            string sql = string.Format(@str, stock.id, stock.goods_id, stock.stack, stock.pieces, stock.track_id,
                GetTimeOrNull(stock.produce_time), stock.pos, stock.pos_type, stock.tilelifter_id, stock.area, 
                stock.track_type, stock.location, stock.location_cal);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        public bool AddStockTrans(StockTrans stotran)
        {
            string str = "INSERT INTO `stock_trans`(`id`, `trans_type`, `trans_status`, `area_id`, `goods_id`, `stock_id`," +
                " `tilelifter_id`, `take_track_id`, `give_track_id`, `create_time`,`carrier_id`)" +
                " VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10})";
            string sql = string.Format(@str, stotran.id, stotran.trans_type, stotran.trans_status,  
                stotran.area_id, stotran.goods_id,  stotran.stock_id,
                stotran.tilelifter_id, stotran.take_track_id, stotran.give_track_id, GetTimeOrNull(stotran.create_time),
                stotran.carrier_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool AddStockInLog(Stock stock)
        {
            string str = "INSERT INTO `stock_log`(`goods_id`, `stack`, `pieces`, `track_id`, `tilelifter_id`, `create_time`,`area_id`) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6})";
            string sql = string.Format(@str, stock.goods_id, stock.stack, stock.pieces, stock.track_id, stock.tilelifter_id, GetTimeOrNull(stock.produce_time), stock.area);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool AddStockOutLog(Stock stock, uint trackid, uint tileid)
        {
            string str = "INSERT INTO `stock_log`(`goods_id`, `stack`, `pieces`, `track_id`, `tilelifter_id`, `create_time`) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', {5})";
            string sql = string.Format(@str, stock.goods_id, stock.stack, stock.pieces, trackid, tileid, GetTimeOrNull(DateTime.Now));
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        /// <summary>
        /// 记录上砖机消耗记录
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tileid"></param>
        internal void AddConsumeLog(Stock stock, uint tileid)
        {
            string str = "INSERT INTO `consume_log`(`goods_id`, `area`, `stack`, `pieces`, `track_id`, `produce_tile_id`," +
                " `produce_time`, `consume_tile_id`, `consume_time`)" +
                " VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', {8})";
            string sql = string.Format(@str, stock.goods_id, stock.area, stock.stack, stock.pieces, stock.track_id, stock.tilelifter_id,
                GetTimeOrNull(stock.produce_time), tileid, GetTimeOrNull(DateTime.Now));
            mSql.ExcuteSql(sql);
        }


        /// <summary>
        /// 添加交易细单信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tileid"></param>
        public bool AddStockTransDtl(StockTransDtl dtl)
        {
            string str = "INSERT INTO `stock_trans_dtl`(`dtl_id`, `dtl_p_id`, `dtl_trans_id`, `dtl_area_id`, `dtl_line_id`, `dtl_type`, `dtl_good_id`, " +
                "`dtl_take_track_id`, `dtl_give_track_id`, `dtl_status`, `dtl_all_qty`, `dtl_left_qty`, `dtl_finish`) " +
                " VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})";
            string sql = string.Format(@str, dtl.dtl_id, dtl.dtl_p_id, dtl.dtl_trans_id, dtl.dtl_area_id, dtl.dtl_line_id, dtl.dtl_type, dtl.dtl_good_id,
                dtl.dtl_take_track_id, dtl.dtl_give_track_id, dtl.dtl_status, dtl.dtl_all_qty, dtl.dtl_left_qty, dtl.dtl_finish);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region[修改]

        internal bool EditGoods(Goods goods)
        {
            string sql = "UPDATE `goods` SET `name` = '{0}', `color` = '{1}', `size_id` = {2}," +
                " `info` = '{3}', `level` = {4}, `memo` = '{5}', `pieces` = '{6}'" +
                ", `carriertype` = '{7}', `updatetime` = {8} WHERE `id` = '{9}'";
            sql = string.Format(sql, goods.name, goods.color, goods.size_id
                , goods.info, goods.level, goods.memo, goods.pieces
                , goods.carriertype, GetTimeOrNull(goods.updatetime), goods.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditStock(Stock stock, StockUpE type)
        {
            string sql = "UPDATE `stock` SET ";
            switch (type)
            {
                case StockUpE.Goods:
                    sql += string.Format("`goods_id` = {0}, `stack` = {1}, `pieces` = {2}, `produce_time` = {3}",
                        stock.goods_id, stock.stack, stock.pieces, GetTimeOrNull(stock.produce_time));
                    break;
                case StockUpE.Track:
                    sql += string.Format("`track_id` = {0}, `area` = {1}, `track_type`={2}, `last_track_id`={3}", stock.track_id, stock.area, stock.track_type, stock.last_track_id);
                    break;
                case StockUpE.Pos:
                    sql += string.Format("`pos` = {0}, `pos_type` = {1}, `track_id` = {2}", stock.pos, stock.pos_type, stock.track_id);
                    break;
                case StockUpE.PosType:
                    sql += string.Format("`pos_type` = {0}", stock.pos_type);
                    break;
                case StockUpE.ProduceTime:
                    sql += string.Format("`produce_time` = {0}", GetTimeOrNull(stock.produce_time));
                    break;
                case StockUpE.Location:
                    sql += string.Format("`location` = {0}, `location_cal` = {1}",  stock.location, stock.location_cal);
                    break;
                default:
                    sql += "1=1";
                    break;
            }

            sql += string.Format(" WHERE `id` = {0}", stock.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        /// <summary>
        /// 更新库存的具体位置和位置信息
        /// </summary>
        /// <param name="s"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        internal bool EditStock(Stock s, byte qty)
        {
            string sql = string.Format("UPDATE `stock` SET `pos` = (`pos` + {0}), `pos_type` = {1} WHERE `id`  = {2}",
                                            qty, s.pos_type, s.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool EditStockTrans(StockTrans trans, TransUpdateE updateE)
        {
            string sql = "UPDATE `stock_trans` SET ";
            switch (updateE)
            {
                case TransUpdateE.Status:
                    sql += string.Format("`trans_status` = {0}", trans.trans_status);
                    break;
                case TransUpdateE.CarrierId:
                    sql += string.Format("`carrier_id` = {0}", trans.carrier_id);
                    break;
                case TransUpdateE.TakeFerryId:
                    sql += string.Format("`take_ferry_id` = {0}", trans.take_ferry_id);
                    break;
                case TransUpdateE.GiveFerryId:
                    sql += string.Format("`give_ferry_id` = {0}", trans.give_ferry_id);
                    break;
                case TransUpdateE.LoadTime:
                    sql += string.Format("`load_time` = {0}", GetTimeOrNull(trans.load_time));
                    break;
                case TransUpdateE.UnLoadTime:
                    sql += string.Format("`unload_time` = {0}", GetTimeOrNull(trans.unload_time));
                    break;
                case TransUpdateE.Finish:
                    sql += string.Format("`finish` = {0}, `finish_time` = {1}", trans.finish, GetTimeOrNull(trans.finish_time));
                    break;
                case TransUpdateE.TakeSite:
                    sql += string.Format("`take_track_id` = {0}", trans.take_track_id);
                    break;
                case TransUpdateE.GiveSite:
                    sql += string.Format("`give_track_id` = {0}", trans.give_track_id);
                    break;
                case TransUpdateE.Stock:
                    sql += string.Format("`stock_id` = {0}", trans.stock_id);
                    break;
                case TransUpdateE.Cancel:
                    sql += string.Format("`cancel` = {0}", trans.cancel);
                    break;
                case TransUpdateE.ReTake:
                    sql += string.Format("`take_track_id` = {0}, `stock_id` = {1}, " +
                        "`carrier_id` = {2}, `trans_status` = {3}", trans.take_track_id, trans.stock_id, trans.carrier_id, trans.trans_status);
                    break;
                case TransUpdateE.TileId:
                    sql += string.Format("`tilelifter_id` = {0}", trans.tilelifter_id);
                    break;
                case TransUpdateE.Line:
                    sql += string.Format("`line` = {0}", trans.line);
                    break;
            }
            sql += string.Format(" WHERE `id` = {0}", trans.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        /// <summary>
        /// 更新交易细单状态
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="updateE"></param>
        /// <returns></returns>
        public bool EditTransDtl(StockTransDtl trans, TransDtlUpdateE updateE)
        {
            string sql = "UPDATE `stock_trans_dtl` SET ";
            switch (updateE)
            {
                case TransDtlUpdateE.Status:
                    sql += string.Format("`dtl_status` = {0}", trans.dtl_status);
                    break;
                case TransDtlUpdateE.TakeTrack:
                    sql += string.Format("`dtl_take_track_id` = {0}", trans.dtl_take_track_id);
                    break;
                case TransDtlUpdateE.GiveTrack:
                    sql += string.Format("`dtl_give_track_id` = {0}", trans.dtl_give_track_id);
                    break;
                case TransDtlUpdateE.Qty:
                    sql += string.Format("`dtl_left_qty` = {0}", trans.dtl_left_qty);
                    break;
                case TransDtlUpdateE.Finish:
                    sql += string.Format("`dtl_finish` = {0}", trans.dtl_finish);
                    break;
                case TransDtlUpdateE.TransId:
                    sql += string.Format("`dtl_trans_id` = {0}", trans.dtl_trans_id);
                    break;
            }

            sql += string.Format(" WHERE `dtl_id` = {0}", trans.dtl_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        #endregion

        #region[删除]

        internal bool DeleteGoods(Goods goods)
        {
            string sql = string.Format("DELETE FROM `goods` where id = '{0}'", goods.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool DeleteStock(Stock stock)
        {
            string sql = string.Format("DELETE FROM `stock` where id = '{0}'", stock.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool DeleteStock(uint trackid)
        {
            string sql = string.Format("DELETE FROM `stock` where track_id = '{0}'", trackid);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool DeleteStockTrans(StockTrans trans)
        {
            string sql = string.Format("DELETE FROM `stock_trans` where id = '{0}'", trans.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
