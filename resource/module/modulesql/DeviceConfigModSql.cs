using enums;
using module.deviceconfig;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class DeviceConfigModSql : BaseModSql
    {
        public DeviceConfigModSql(MySql mysql) : base(mysql)
        {

        }

        #region[查询]

        #region 运输车

        public List<ConfigCarrier> QueryConfigCarrier()
        {
            List<ConfigCarrier> list = new List<ConfigCarrier>();
            string sql = string.Format(@"SELECT t.* FROM config_carrier AS t ");
            DataTable dt = mSql.ExecuteQuery(sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<ConfigCarrier>();
            }
            return list;
        }

        #endregion

        #region 摆渡车

        public List<ConfigFerry> QueryConfigFerry()
        {
            List<ConfigFerry> list = new List<ConfigFerry>();
            string sql = string.Format(@"SELECT t.* FROM config_ferry AS t ");
            DataTable dt = mSql.ExecuteQuery(sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<ConfigFerry>();
            }
            return list;
        }

        #endregion

        #region 砖机

        public List<ConfigTileLifter> QueryConfigTileLifter()
        {
            List<ConfigTileLifter> list = new List<ConfigTileLifter>();
            string sql = string.Format(@"SELECT t.* FROM config_tilelifter AS t");
            DataTable dt = mSql.ExecuteQuery(sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<ConfigTileLifter>();
            }
            return list;
        }

        #endregion

        #endregion

        #region[添加]

        #region 运输车

        internal bool AddConfigCarrier(ConfigCarrier dev)
        {
            string sql = string.Format("INSERT INTO config_carrier(id) VALUES({0})", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region 摆渡车

        internal bool AddConfigFerry(ConfigFerry dev)
        {
            string sql = string.Format("INSERT INTO config_ferry(id, track_id) VALUES({0}, {1})", dev.id, dev.track_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region 砖机

        internal bool AddConfigTileLifter(ConfigTileLifter dev)
        {
            string sql = string.Format(@"INSERT INTO config_tilelifter(id, brother_dev_id, left_track_id, right_track_id, 
strategy_in, strategy_out, work_type, last_track_id, old_goodid, goods_id, pre_goodid, do_shift, 
can_cutover, work_mode, work_mode_next, do_cutover)
VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15})",
                dev.id, dev.brother_dev_id, GetIntOrNull(dev.left_track_id), GetIntOrNull(dev.right_track_id), 
                dev.strategy_in, dev.strategy_out, dev.work_type, dev.last_track_id, 
                dev.old_goodid, GetIntOrNull(dev.goods_id), dev.pre_goodid, dev.do_shift,
                dev.can_cutover, dev.work_mode, dev.work_mode_next, dev.do_cutover);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #endregion

        #region[修改]

        #region 运输车

        public bool EditConfigCarrier(ConfigCarrier dev)
        {
            string sql = string.Format(@"UPDATE config_carrier SET a_takemisstrack = {1}, a_givemisstrack = {2}, a_alert_track = {3}, stock_id = {4} 
WHERE id = {0}", dev.id, dev.a_takemisstrack, dev.a_givemisstrack, dev.a_alert_track, GetIntOrNull(dev.stock_id));
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool EditCarrierAlert(ConfigCarrier dev, CarrierAlertE type)
        {
            string sql = string.Format("UPDATE config_carrier SET a_alert_track = {0} ,", dev.a_alert_track);
            switch (type)
            {
                case CarrierAlertE.GiveMissTrack:
                    sql += string.Format("a_givemisstrack` = {0}", dev.a_givemisstrack);
                    break;
                case CarrierAlertE.TakeMissTrack:
                    sql += string.Format("a_takemisstrack = {0}", dev.a_takemisstrack);
                    break;
                default:
                    sql += "1=1";
                    break;
            }

            sql += string.Format(" WHERE id = {0}", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region 摆渡车

        internal bool EditConfigFerry(ConfigFerry dev)
        {
            string sql = string.Format(@"UPDATE config_ferry SET track_id = {1} WHERE id = {0}", dev.id, dev.track_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region 砖机

        internal bool EditConfigTileLifter(ConfigTileLifter dev)
        {
            string sql = string.Format(@"UPDATE config_tilelifter SET brother_dev_id = {1}, left_track_id = {2}, right_track_id = {3}, strategy_in = {4}, strategy_out = {5}, 
work_type = {6}, last_track_id = {7}, old_goodid = {8}, goods_id = {9}, pre_goodid = {10}, do_shift = {11},
can_cutover = {12}, work_mode = {13}, work_mode_next = {14}, do_cutover = {15}, alter_dev_id = {16} WHERE id = {0}",
                dev.id, dev.brother_dev_id, GetIntOrNull(dev.left_track_id), GetIntOrNull(dev.right_track_id), dev.strategy_in, dev.strategy_out, 
                dev.work_type, dev.last_track_id, dev.old_goodid, GetIntOrNull(dev.goods_id), dev.pre_goodid, dev.do_shift,
                dev.can_cutover, dev.work_mode, dev.work_mode_next, dev.do_cutover, dev.alter_dev_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        /// <summary>
        /// 更新砖机的配置信息
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal bool EditConfigTileLifter(ConfigTileLifter dev, TileConfigUpdateE type)
        {
            string sql = "UPDATE config_tilelifter SET ";

            switch (type)
            {
                case TileConfigUpdateE.Goods:
                    sql += string.Format("old_goodid = {0}, goods_id = {1}, pre_goodid = {2}, do_shift = {3} ",
                        dev.old_goodid, GetIntOrNull(dev.goods_id), dev.pre_goodid, dev.do_shift);
                    break;
                case TileConfigUpdateE.LastTrack:
                    sql += string.Format("last_track_id = {0}", dev.last_track_id);
                    break;
                case TileConfigUpdateE.NoWorkTrack:
                    sql += string.Format("non_work_track_id =  {0}", dev.non_work_track_id);
                    break;
                case TileConfigUpdateE.Alert_Dev_Id:
                    sql += string.Format("alter_dev_id = {0}", dev.alter_dev_id);
                    break;
                case TileConfigUpdateE.WorkMode:
                    sql += string.Format("can_cutover = {0}, work_mode = {1}, work_mode_next = {2}, " +
                        "do_cutover = {3}, goods_id = {4}, pre_goodid = {5}", 
                        dev.can_cutover, dev.work_mode, dev.work_mode_next, 
                        dev.do_cutover, GetIntOrNull(dev.goods_id), dev.pre_goodid);
                    break;
                case TileConfigUpdateE.Strategey:
                    sql += string.Format("strategy_in = {0}, strategy_out = {1}, work_type = {2}", dev.strategy_in, dev.strategy_out, dev.work_type);
                    break;
                default:
                    sql += string.Format("brother_dev_id = {0}", dev.brother_dev_id);
                    break;
            }
            sql += string.Format(" WHERE id = {0}", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #endregion

        #region[删除]

        #region 运输车

        internal bool DeleteConfigCarrier(ConfigCarrier dev)
        {
            string sql = string.Format("DELETE FROM config_carrier WHERE id = {0}", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region 摆渡车

        internal bool DeleteConfigFerry(ConfigFerry dev)
        {
            string sql = string.Format("DELETE FROM config_ferry WHERE id = {0}", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region 砖机

        internal bool DeleteConfigTileLifter(ConfigTileLifter dev)
        {
            string sql = string.Format("DELETE FROM config_tilelifter WHERE id = {0}", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #endregion
    }
}
