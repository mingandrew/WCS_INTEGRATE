using enums;
using module.device;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class TrafficCtlModSql : BaseModSql
    {
        public TrafficCtlModSql(MySql sql) : base(sql)
        {

        }

        #region[查询]

        public List<TrafficControl> QueryTrafficCtlList()
        {
            List<TrafficControl> list = new List<TrafficControl>();
            string sql = string.Format("SELECT t.* FROM traffic_control AS t WHERE t.traffic_control_status = 0 ORDER BY t.create_time");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<TrafficControl>();
            }
            return list;
        }

        #endregion

        #region[添加]


        public bool AddTrafficCtl(TrafficControl ctl)
        {
            string sql = string.Format(@"INSERT INTO `traffic_control`(`id`, `area`, `traffic_control_type`, `restricted_id`, `control_id`, `traffic_control_status`, 
`from_track_id`, `from_point`, `from_site`, `to_track_id`, `to_point`, `to_site`, `create_time`) 
VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})", ctl.id, ctl.area, ctl.traffic_control_type, ctl.restricted_id, ctl.control_id,
ctl.traffic_control_status, ctl.from_track_id, ctl.from_point, ctl.from_site, ctl.to_track_id, ctl.to_point, ctl.to_site, GetTimeOrNull(ctl.create_time));
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[修改]

        public bool EditTrafficCtl(TrafficControl ctl, TrafficControlUpdateE type)
        {
            string sql = "UPDATE `traffic_control` SET ";
            switch (type)
            {
                case TrafficControlUpdateE.Status:
                    sql += string.Format("`traffic_control_status` = {0},  `update_time` = {1}",
                        ctl.traffic_control_status, GetTimeOrNull(ctl.update_time));
                    break;
                case TrafficControlUpdateE.from:
                    sql += string.Format("`from_track_id` = {0}, `from_point` = {1}, `from_site` = {2}",
                        ctl.from_track_id, ctl.from_point, ctl.from_site);
                    break;
                case TrafficControlUpdateE.to:
                    sql += string.Format("`to_track_id` = {0}, `to_point` = {1}, `to_site` = {2}",
                        ctl.to_track_id, ctl.to_point, ctl.to_site);
                    break;
                default:
                    sql += "1=1";
                    break;
            }

            sql += string.Format(" WHERE `id` = {0}", ctl.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[删除]

        public bool DeleteTrafficCtl(TrafficControl ctl)
        {
            string sql = string.Format("DELETE FROM traffic_control where id = {0}", ctl.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
