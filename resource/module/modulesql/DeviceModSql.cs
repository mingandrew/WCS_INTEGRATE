using enums;
using module.device;
using module.rf;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class DeviceModSql : BaseModSql
    {
        public DeviceModSql(MySql mysql) : base(mysql)
        {

        }

        #region[查询]

        public List<Device> QueryDeviceList()
        {
            List<Device> list = new List<Device>();
            string sql = string.Format("SELECT t.id, t.`name`, t.ip, t.`port`," +
                " t.type, t.type2, t.`enable`, t.att1, t.att2," +
                " t.goods_id, t.left_track_id, t.right_track_id, " +
                "t.brother_dev_id, t.strategy_in, t.strategy_out, " +
                "t.memo, t.area, t.offlinetime, t.a_givemisstrack, " +
                "t.a_takemisstrack, t.a_poweroff, t.a_alert_track, t.do_work, t.work_type, t.ignorearea, t.last_track_id " +
                "t.old_goodid, t.pre_goodid, t.do_shift, t.left_goods, t.right_goods " +
                "FROM device AS t ORDER BY t.`order` ASC");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Device>();
            }
            return list;
        }

        public List<RfClient> QueryRfClientList()
        {
            List<RfClient> list = new List<RfClient>();
            string sql = string.Format("SELECT t.rfid, t.`name`, t.ip, t.conn_time, t.disconn_time FROM rf_client AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<RfClient>();
            }
            return list;
        }
        #endregion

        #region[添加]


        internal bool AddDevice(Device dev)
        {
            string str = "INSERT INTO `device`( `name`, `ip`, `port`, `type`, `type2`, `enable`," +
                " `att1`, `att2`, `goods_id`, `left_track_id`, `right_track_id`, `brother_dev_id`, `memo`, `area`)" +
                " VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, {9}, {10}, '{11}', '{12}', '{13}')";
            string sql = string.Format(@str, dev.name, dev.ip, dev.port, dev.type,
                dev.type2, dev.enable, dev.att1, dev.att2,
                GetIntOrNull(dev.goods_id), GetIntOrNull(dev.left_track_id), 
                GetIntOrNull(dev.right_track_id), dev.brother_dev_id, dev.memo, dev.area);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        public bool AddRfClient(RfClient rf)
        {
            string str = "INSERT INTO `rf_client`(`rfid`, `name`, `ip`, `conn_time`, `disconn_time`)" +
                " VALUES('{0}', '{1}', '{2}', {3}, {4})";
            string sql = string.Format(@str, rf.rfid, rf.name, rf.ip, GetTimeOrNull(rf.conn_time), GetTimeOrNull(rf.disconn_time));
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[修改]

        internal bool EditDevice(Device dev)
        {
            string sql = "UPDATE `device` set name = '{0}', `ip` = '{1}'," +
                " `port` = '{2}', `type` = '{3}', `enable` = {4}," +
                " `att1` = '{5}',  `att2` = '{6}',  `goods_id` = {7}," +
                " `left_track_id` = {8}, `right_track_id` = {9}," +
                " memo = '{10}', `brother_dev_id` = '{11}'," +
                " strategy_in = '{12}', strategy_out= '{13}', " +
                "area= '{14}', do_work= {15}, work_type= {16}  where id = '{17}'";
            sql = string.Format(sql, dev.name, dev.ip, dev.port, dev.type, dev.enable,
                dev.att1, dev.att2, GetIntOrNull(dev.goods_id),
                GetIntOrNull(dev.left_track_id), GetIntOrNull(dev.right_track_id),
                dev.memo, dev.brother_dev_id, dev.strategy_in, dev.strategy_out, dev.area, dev.do_work, dev.work_type, dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditeTileGood(Device dev)
        {
            string sql = "UPDATE `device` set `goods_id` = '{0}', `old_goodid` = '{1}'," +
                   " `pre_goodid` = '{2}', do_shift = {3}, left_goods = {4}, right_goods = {5} where id = '{6}'";
            sql = string.Format(sql, GetIntOrNull(dev.goods_id), dev.old_goodid, dev.pre_goodid, dev.do_shift, dev.left_goods, dev.right_goods, dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal void EditDeviceLastTrack(Device dev)
        {
            string sql = string.Format("UPDATE `device` SET last_track_id = {0}  WHERE `id` = {1}", dev.last_track_id, dev.id);
            mSql.ExcuteSql(sql);
        }

        public bool EditDeviceAlert(Device dev, CarrierAlertE type)
        {
            string sql = string.Format("UPDATE `device` SET a_alert_track = {0} ,", dev.a_alert_track);
            switch (type)
            {
                case CarrierAlertE.PowerOff:
                    sql += string.Format("`a_poweroff` = {0} ", dev.a_poweroff);
                    break;
                case CarrierAlertE.GiveMissTrack:
                    sql += string.Format("`a_givemisstrack` = {0}", dev.a_givemisstrack);
                    break;
                case CarrierAlertE.TakeMissTrack:
                    sql += string.Format("`a_takemisstrack` = {0}", dev.a_takemisstrack);
                    break;
                default:
                    sql += "1=1";
                    break;
            }

            sql += string.Format(" WHERE `id` = {0}", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool EditRfDevice(RfClient rf)
        {
            string sql = "UPDATE `rf_client` SET `name` = '{0}', `ip` = '{1}', `conn_time` = {2}, `disconn_time` = {3} " +
                "WHERE `rfid` = '{4}';";
            sql = string.Format(sql, rf.name, rf.ip, GetTimeOrNull(rf.conn_time), GetTimeOrNull(rf.disconn_time), rf.rfid);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region[删除]

        internal bool DeleteDevice(Device dev)
        {
            string sql = string.Format("DELETE FROM device where id = '{0}'", dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        internal bool DeleteRfDevice(RfClient dev)
        {
            string sql = string.Format("DELETE FROM rf_client where rfid = '{0}'", dev.rfid);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
