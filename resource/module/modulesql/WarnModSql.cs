using module;
using System;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class WarnModSql : BaseModSql
    {
        public WarnModSql(MySql sql) : base(sql)
        {

        }


        #region[查询]

        public List<Warning> QueryWarningList()
        {
            List<Warning> list = new List<Warning>();
            string sql = string.Format("SELECT t.id, t.area_id, t.type, t.resolve, t.dev_id, t.trans_id, t.track_id, t.content," +
                " t.createtime, t.resolvetime FROM warning AS t WHERE t.resolve = false ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Warning>();
            }
            return list;
        }

        /// <summary>
        /// 报警日志
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        public List<Warning> QueryWarningLog(byte wtype, DateTime start, DateTime stop, uint devid = 0)
        {
            List<Warning> list = new List<Warning>();
            string sql = "SELECT t.id, t.area_id, t.type, t.resolve, t.dev_id, t.trans_id, t.track_id, t.content," +
                " t.createtime, t.resolvetime FROM warning AS t WHERE ";

            if (wtype != 255)
            {
                sql += string.Format("t.type= {0} and ", wtype);
            }

            if(devid != 0)
            {
                sql += string.Format("t.dev_id= {0} and ", devid);
            }

            sql += string.Format("t.createtime >= '{0}' and t.createtime <= '{1}' limit 300", start.ToString(timeformat), stop.ToString(timeformat));


            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Warning>();
            }
            return list;
        }

        #endregion

        #region[添加]


        internal bool AddWarning(Warning warn)
        {
            string str = "INSERT INTO `warning`(`id`, `area_id`, `type`, `resolve`, `dev_id`, `trans_id`, `track_id`, `content`, `createtime`)" +
                " VALUES('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', {8})";
            string sql = string.Format(@str, warn.id, warn.area_id, warn.type, warn.resolve, warn.dev_id, warn.trans_id, warn.track_id, warn.content, GetTimeOrNull(warn.createtime));
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[修改]

        internal bool EditWarning(Warning warn)
        {
            string sql = "UPDATE `warning` set resolve = {0}, `resolvetime` = {1}  where id = '{2}'";
            sql = string.Format(sql, warn.resolve, GetTimeOrNull(warn.resolvetime), warn.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[删除]

        internal bool DeleteWarning(Warning warn)
        {
            string sql = string.Format("DELETE FROM warning where id = '{0}'", warn.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
