using enums;
using module.diction;
using System;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class DictionModSql : BaseModSql
    {

        public DictionModSql(MySql _sql) : base(_sql)
        {

        }

        #region[查询]
        public List<Diction> QueryDictionList()
        {
            List<Diction> list = new List<Diction>();
            string sql = string.Format("SELECT d.id, d.type, d.valuetype, d.`name`, d.isadd, d.isedit, d.isdelete, d.authorizelevel FROM diction AS d ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Diction>();
            }
            return list;
        }

        public List<DictionDtl> QueryDictionDtlList()
        {
            List<DictionDtl> list = new List<DictionDtl>();
            string sql = string.Format("SELECT t.id, t.diction_id, t.`code`, t.`name`, t.int_value, t.bool_value, t.string_value, " +
                "t.double_value, t.uint_value, t.`order`, t.updatetime FROM diction_dtl AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<DictionDtl>();
            }
            return list;
        }
        #endregion

        #region[添加]

        internal bool AddDiction(Diction dic)
        {
            string str = "INSERT INTO `diction`(`type`, `valuetype`, `name`, `isadd`, `isedit`, `isdelete`, `authorizelevel`) " +
                             "VALUES('{0}', '{1}', '{2}', {3}, {4}, {5},' {6}')";
            string sql = string.Format(@str, dic.type, dic.valuetype, dic.name, dic.isadd, dic.isedit, dic.isdelete, dic.authorizelevel);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool AddDictionDtl(DictionDtl dtl, ValueTypeE dicType)
        {
            string sql = "INSERT INTO `diction_dtl`(`diction_id`, `code`,  `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}')";

            sql = string.Format(sql, dtl.diction_id, dtl.code, dtl.name, dtl.int_value, dtl.bool_value, dtl.string_value, dtl.double_value, dtl.uint_value, dtl.order);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region[修改]

        internal bool EditDiction(Diction dic)
        {
            string sql = "UPDATE diction set  type = '{0}', `name` = '{1}', isadd = {2}, isedit = {3}" +
                ", isdelete = {4}, authorizelevel = '{5}', valuetype = '{6}' where id = '{7}'";

            sql = string.Format(sql, dic.valuetype, dic.name, dic.isadd, dic.isedit, dic.isdelete, dic.authorizelevel, dic.valuetype, dic.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditDicDtlComm(DictionDtl dtl)
        {
            string sql = string.Format("UPDATE diction_dtl set name = '{0}', `order` = '{1}' where id = {2}'", dtl.name, dtl.order, dtl.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditDicDtlValue(DictionDtl dtl, ValueTypeE dicType)
        {
            string sql = string.Format("UPDATE diction_dtl set updatetime = {0}, ", GetTimeOrNull(DateTime.Now));
            switch (dicType)
            {
                case ValueTypeE.Integer:
                    sql += string.Format("int_value = '{0}'", dtl.int_value);
                    break;
                case ValueTypeE.Boolean:
                    sql += string.Format("bool_value = {0}", dtl.bool_value);
                    break;
                case ValueTypeE.String:
                    sql += string.Format("string_value = '{0}'", dtl.string_value);
                    break;
                case ValueTypeE.Double:
                    sql += string.Format("double_value = '{0}'", dtl.double_value);
                    break;
                case ValueTypeE.UInteger:
                    sql += string.Format("uint_value = '{0}'", dtl.uint_value);
                    break;
                default:
                    sql += "1=1";
                    break;
            }
            sql += string.Format(" where id = {0}", dtl.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        #endregion
    }
}
