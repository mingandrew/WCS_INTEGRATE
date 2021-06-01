using System;
using System.Data;
using tool;

namespace resource.module.modulesql
{
    public class BaseModSql
    {
        protected readonly MySql mSql;
        protected readonly string timeformat = "yyyy-MM-dd HH:mm:ss";

        public BaseModSql(MySql sql)
        {
            mSql = sql;
        }

        public object GetIntOrNull(uint value)
        {
            if(value > 0)
            {
                return value;
            }

            return "NULL";
        }

        public string GetTimeOrNull(DateTime? datetime)
        {
            if(datetime is DateTime time)
            {
                return "'" + time.ToString(timeformat) + "'";
            }
            return "NULL";
        }

        #region[查询]


        internal uint QueryTableMaxId(string tablename, string column = "id")
        {
            try
            {
                string sql = string.Format("select max({0}) as id from {1}", column, tablename);
                DataTable dt = mSql.ExecuteQuery(@sql);
                if (!mSql.IsNoData(dt))
                {
                    object value = dt.Rows[0][column];
                    if (value != null)
                    {
                        if (uint.TryParse(value.ToString(), out uint maxid))
                        {
                            return maxid;
                        }
                    }
                }
            }
            catch { }
            return 1;
        }

        #endregion

        #region[添加]

        #endregion

        #region[修改]

        #endregion
    }
}
