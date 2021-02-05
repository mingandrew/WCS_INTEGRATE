using enums;
using module.device;
using System;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class TileLifterNeedModSql : BaseModSql
    {
        public TileLifterNeedModSql(MySql mysql) : base(mysql)
        {

        }

        #region[查询]

        public List<TileLifterNeed> QueryTileLifterNeedList()
        {
            List<TileLifterNeed> list = new List<TileLifterNeed>();
            string sql = string.Format("SELECT t.device_id, t.track_id, t.`left`, t.trans_id, t.create_time, t.trans_create_time, t.finish, t.type, t.area_id " +
                " FROM tilelifterneed AS t  WHERE t.finish IS NULL ORDER BY t.create_time ASC");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<TileLifterNeed>();
            }
            return list;
        }
        #endregion

        #region[添加]


        public bool AddTileLifterNeed(TileLifterNeed tileLifterNeed)
        {
            string str = "INSERT INTO `tilelifterneed`(`device_id`, `track_id`, `left`, `create_time`, `type`, `area_id`) VALUES({0}, {1}, {2}, {3}, {4}, {5})";
            string sql = string.Format(@str, tileLifterNeed.device_id, tileLifterNeed.track_id, tileLifterNeed.left, GetTimeOrNull(tileLifterNeed.create_time),
                 tileLifterNeed.type, tileLifterNeed.area_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
       
        #endregion

        #region[修改]

        public bool EditTileLifterNeed(TileLifterNeed tileLifterNeed, TileNeedStatusE type, bool isTrans = false)
        {
            string sql = "UPDATE `tilelifterneed` SET ";

            switch (type)
            {
                case TileNeedStatusE.Trans:
                    sql += string.Format("`trans_id` = {0}, `trans_create_time` = {1}",
                        tileLifterNeed.trans_id, GetTimeOrNull(tileLifterNeed.trans_create_time));
                    break;
                case TileNeedStatusE.Finish:
                    sql += string.Format("`finish` = {0}",
                        tileLifterNeed.finish);
                    break;
                case TileNeedStatusE.UpdateCreateTime:
                    sql += string.Format("`create_time` = {0}",
                        GetTimeOrNull(tileLifterNeed.create_time));
                    break;
                default:
                    break;
            }

            sql += string.Format(" WHERE `device_id` = {0} AND `track_id` = {1} AND `finish` IS NULL {2}", tileLifterNeed.device_id, tileLifterNeed.track_id,
                     isTrans ? " AND `trans_id` = 0 " : " ");
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        #endregion

        #region[删除]

        public bool DeleteTileLifterNeed(TileLifterNeed tileLifterNeed)
        {
            string sql = string.Format("DELETE FROM `tilelifterneed` WHERE device_id = '{0}' AND track_id = {1} AND finish IS NULL", tileLifterNeed.device_id, tileLifterNeed.track_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool DeleteTileLifterNeed(uint dev_id, uint track_id)
        {
            string sql = string.Format("DELETE FROM `tilelifterneed` WHERE device_id = '{0}' AND `trans_id` IS NULL  AND track_id = {1} AND finish IS NULL", dev_id, track_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        #endregion
    }
}
