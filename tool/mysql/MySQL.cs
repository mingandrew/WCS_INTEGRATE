using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using tool.appconfig;
using tool.mlog;

namespace tool
{
    public class MySql
    {
        private string conn;
        private Log mLog;
        public MySql(Log log)
        {
            mLog = log;
            if (!string.IsNullOrEmpty(GlobalWcsDataConfig.MysqlConfig.MySqlConn()))
            {
                conn = GlobalWcsDataConfig.MysqlConfig.MySqlConn();
            }
        }

        /// <summary>
        /// 判断是否查询到值
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool IsNoData(DataTable dt)
        {
            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取所有字段数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string sql)
        {
            try
            {
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, conn))
                {
                    DataSet dataSet = new DataSet();
                    mySqlDataAdapter.Fill(dataSet, "Table");
                    return dataSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                mLog.Error(true, ex.Message, ex);
                //throw;
            }
            return null;
        }


        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExcuteSql(string sql)
        {
            try
            {
                using (MySqlConnection sqlcon = new MySqlConnection(conn))
                {
                    sqlcon.Open();
                    using (MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlcon))
                    {
                        return mySqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.Error(true, ex.Message + sql, ex);
                //throw;
            }
            return 0;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExcuteSqlWithException(string sql)
        {
            try
            {
                using (MySqlConnection sqlcon = new MySqlConnection(conn))
                {
                    sqlcon.Open();
                    using (MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlcon))
                    {
                        return mySqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.Error(true, ex.Message + sql, ex);
                throw;
            }
        }



        /// <summary>
        /// 获取对应table数据数目
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public int GetCount(string table, string conditions)
        {
            try
            {
                string sql = string.Format(@"select count(*) COUNT from {0} where 1 = 1 {1}", table, string.IsNullOrEmpty(conditions.Trim()) ? "" : "and " + conditions);
                DataTable dt = ExecuteQuery(sql);
                int count = Convert.ToInt32(dt.Rows[0]["COUNT"].ToString());
                return count;
            }
            catch (Exception ex)
            {
                mLog.Error(true, ex.Message, ex);
                //throw;
            }
            return -1;
        }
    }
}
