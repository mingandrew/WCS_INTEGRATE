using module.tiletrack;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class TileTrackModSql : BaseModSql
    {
        public TileTrackModSql(MySql sql) : base(sql)
        {

        }



        #region[查询]

        public List<TileTrack> QueryTileTrackList()
        {
            List<TileTrack> list = new List<TileTrack>();
            string sql = string.Format("SELECT t.`id`, t.tile_id,t.track_id, t.`order` FROM tile_track AS t");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<TileTrack>();
            }
            return list;
        }

        #endregion

        #region[添加]


        internal bool AddTileTrack(TileTrack titr)
        {
            string str = "INSERT INTO `tile_track`(`id`, `tile_id`, `track_id`, `order`) VALUES ({0}, {1}, {2}, {3})";
            string sql = string.Format(@str,titr.id, titr.tile_id, titr.track_id, titr.order);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[修改]

        internal bool EditTileTrack(TileTrack titr)
        {
            string sql = "UPDATE `tile_track` set `order` = {0} where id = {1}";
            sql = string.Format(sql, titr.order, titr.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[删除]

        internal bool DeleteTileTrack(TileTrack titr)
        {
            string sql = string.Format("DELETE FROM tile_track where id = {0}", titr.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool DeleteTileTracks(uint tileid)
        {
            string sql = string.Format("DELETE FROM tile_track where tile_id = {0}", tileid);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
