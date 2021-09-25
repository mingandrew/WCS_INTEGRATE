﻿using enums;
using module.device;
using module.rf;
using System;
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
            string sql = string.Format("SELECT t.* FROM device AS t");
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
            string sql = string.Format("SELECT t.* FROM rf_client AS t ");
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
            string str = "INSERT INTO `device`( `id`, `name`, `ip`, `port`, `type`, `type2`, `enable`, `att1`, `att2`,`memo`, `area`, `do_work`, `line`)" +
                " VALUES({11}, '{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', '{7}', '{8}', '{9}',{10}, {12})";
            string sql = string.Format(@str, dev.name, dev.ip, dev.port, dev.type,
                dev.type2, dev.enable, dev.att1, dev.att2, dev.memo, dev.area, dev.do_work, dev.id, dev.line);
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
                " `att1` = '{5}',  `att2` = '{6}', memo = '{7}'," +
                "area= '{8}', do_work= {9} where id = '{10}'";
            sql = string.Format(sql, dev.name, dev.ip, dev.port, dev.type, dev.enable,
                dev.att1, dev.att2, dev.memo, dev.area, dev.do_work, dev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditDeviceLine(Device dev)
        {
            string sql = "UPDATE `device` set line= {0} where id = '{1}'";
            sql = string.Format(sql, dev.line, dev.id);
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

        public void EditRfFilter(RfClient rf)
        {
            string sql = "UPDATE `rf_client` SET `filter_area` = {0}, `filter_areaids` = '{1}', " +
                "`filter_type` = {2}, `filter_typevalues` = '{3}' , `filter_dev` = {4}, `filter_devids` = '{5}' " +
                   "WHERE `rfid` = '{6}';";
            sql = string.Format(sql, rf.filter_area, rf.filter_areaids, rf.filter_type, rf.filter_typevalues,
                rf.filter_dev, rf.filter_devids, rf.rfid);
            mSql.ExcuteSql(sql);

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
