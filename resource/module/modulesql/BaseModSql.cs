using System;
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

        #endregion

        #region[添加]

        #endregion

        #region[修改]

        #endregion
    }
}
