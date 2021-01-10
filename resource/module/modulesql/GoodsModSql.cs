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
            string sql = string.Format("SELECT * FROM goods AS t ORDER BY t.top desc, t.createtime DESC, t.updatetime DESC ");
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
            string sql = string.Format("SELECT t.id, t.goods_id, t.stack, t.pieces, t.track_id, t.produce_time, " +
                "t.pos, t.pos_type, t.tilelifter_id, t.area, t.track_type  FROM stock AS t  ");
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
            string sql = string.Format("SELECT t.id, t.trans_type, t.trans_status, t.area_id," +
                " t.goods_id, t.take_track_id, t.give_track_id," +
                " t.tilelifter_id, t.take_ferry_id, t.give_ferry_id," +
                " t.carrier_id, t.create_time, t.load_time, t.unload_time," +
                " t.finish, t.finish_time, t.cancel FROM stock_trans AS t" +
                " where t.finish is NULL AND t.trans_status <> {0}", (byte)TransStatusE.完成);
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<StockTrans>();
            }
            return list;
        }

        public StockTrans QueryStockTransById(int transid)
        {
            List<StockTrans> list = new List<StockTrans>();
            string sql = string.Format("SELECT t.id, t.trans_type, t.trans_status, t.area_id, t.goods_id, t.take_track_id, t.give_track_id," +
                " t.tilelifter_id, t.take_ferry_id, t.give_ferry_id, t.carrier_id, t.create_time, t.load_time, t.unload_time," +
                " t.finish, t.finish_time FROM stock_trans AS t " +
                "where t.id ={0} ", transid);
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

        #endregion

        #region[添加]


        internal bool AddGoods(Goods goods)
        {
            string str = "INSERT INTO `goods`(`id`, `area_id`, `name`, `color`, `pieces`, `carriertype`, `memo`, `updatetime`,`size_id`,`level`) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}, {9})";
            string sql = string.Format(@str, goods.id, goods.area_id, goods.name, goods.color, goods.pieces,
                goods.carriertype, goods.memo, GetTimeOrNull(goods.updatetime),goods.size_id, goods.level);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool AddStock(Stock stock)
        {
            string str = "INSERT INTO `stock`(`id`, `goods_id`, `stack`, `pieces`, `track_id`" +
                ", `produce_time`, `pos`, `pos_type`, `tilelifter_id`, `area`, `track_type`) " +
                "VALUES('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})";
            string sql = string.Format(@str, stock.id, stock.goods_id, stock.stack, stock.pieces, stock.track_id,
                GetTimeOrNull(stock.produce_time), stock.pos, stock.pos_type, stock.tilelifter_id, stock.area, stock.track_type);
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
            string str = "INSERT INTO `stock_log`(`goods_id`, `stack`, `pieces`, `track_id`, `tilelifter_id`, `create_time`) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', {5})";
            string sql = string.Format(@str, stock.goods_id, stock.stack, stock.pieces, stock.track_id, stock.tilelifter_id, GetTimeOrNull(stock.produce_time));
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

        #endregion

        #region[修改]

        internal bool EditGoods(Goods goods)
        {
            string sql = "UPDATE `goods` SET `name` = '{0}', `color` = '{1}', `size_id` = {2}, `memo` = '{3}', " +
                " `pieces` = '{4}', `carriertype` = '{5}', `updatetime` = {6},`level` = {7} WHERE `id` = '{8}'";
            sql = string.Format(sql, goods.name, goods.color, goods.size_id, goods.memo, goods.pieces
                , goods.carriertype, GetTimeOrNull(goods.updatetime),goods.level, goods.id);
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
                    sql += string.Format("`track_id` = {0}, `area` = {1}, `track_type`={2}", stock.track_id, stock.area, stock.track_type);
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
                default:
                    sql += "1=1";
                    break;
            }

            sql += string.Format(" WHERE `id` = {0}", stock.id);
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
            }
            sql += string.Format(" WHERE `id` = {0}", trans.id);
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
