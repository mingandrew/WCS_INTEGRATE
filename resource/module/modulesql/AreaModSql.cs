using module.area;
using module.line;
using System;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class AreaModSql : BaseModSql
    {
        public AreaModSql(MySql sql) : base(sql)
        {
        }

        #region[查询]

        public List<Area> QueryAreaList()
        {
            List<Area> list = new List<Area>();
            string sql = string.Format("SELECT t.* FROM area AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Area>();
            }
            return list;
        }

        public List<AreaDevice> QueryAreaDeviceList()
        {
            List<AreaDevice> list = new List<AreaDevice>();
            string sql = string.Format("SELECT t.* FROM area_device AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<AreaDevice>();
            }
            return list;
        }

        public List<AreaTrack> QueryAreaTrackList()
        {
            List<AreaTrack> list = new List<AreaTrack>();
            string sql = string.Format("SELECT t.* FROM area_track AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<AreaTrack>();
            }
            return list;
        }
        
        public List<AreaDeviceTrack> QueryAreaDeviceTrackList()
        {
            List<AreaDeviceTrack> list = new List<AreaDeviceTrack>();
            string sql = string.Format("SELECT t.* FROM area_device_track AS t  ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<AreaDeviceTrack>();
            }
            return list;
        }

        public List<Line> QueryLineList()
        {
            List<Line> list = new List<Line>();
            string sql = string.Format("SELECT t.* FROM line AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<Line>();
            }
            return list;
        }

        public uint GetAreaDevTraMaxId()
        {
            return QueryTableMaxId("area_device_track");
        }
        #endregion

        #region[添加]


        public bool AddArea(Area area)
        {
            string str = "INSERT INTO `area`(`name`, `enable`, `devautorun`, `memo`, `up_car_count`, `down_car_count`) VALUES('{0}', {1}, {2}, '{3}', {4}, {5})";
            string sql = string.Format(@str, area.name, area.enable, area.devautorun, area.memo, area.up_car_count, area.down_car_count);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        internal bool AddAreaDevice(AreaDevice areadev)
        {
            string str = "INSERT INTO `area_device`(`id`, `area_id`, `device_id`, `dev_type`) VALUES('{0}', '{1}', '{2}', '{3}')";
            string sql = string.Format(@str, areadev.id, areadev.area_id, areadev.device_id, areadev.dev_type);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool AddAreaTrack(AreaTrack areatra)
        {
            string str = "INSERT INTO `area_track`(`area_id`, `track_id`, `track_type`) VALUES('{0}', '{1}', '{2}')";
            string sql = string.Format(@str, areatra.area_id, areatra.track_id, areatra.track_type);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        internal bool AddAreaDeviceTrack(AreaDeviceTrack areadevtra)
        {
            string str = "INSERT INTO `area_device_track`(`id`, `area_id`, `device_id`, `track_id`, `prior`) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')";
            string sql = string.Format(@str,areadevtra.id,  areadevtra.area_id, areadevtra.device_id, areadevtra.track_id, areadevtra.prior);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool AddLine(Line line)
        {
            string str = "INSERT INTO `line`(`area_id`, `line`, `name`, `sort_task_qty`, `up_task_qty`, `down_task_qty`, `line_type`, `full_qty`)" +
                " VALUES ({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7})";
            string sql = string.Format(@str, line.area_id, line.line, line.name, line.sort_task_qty, line.up_task_qty, line.down_task_qty, line.line_type, line.full_qty);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        /// <summary>
        /// 复制其他砖机的储砖轨道信息，给备用砖机
        /// </summary>
        /// <param name="from_id">其他砖机id</param>
        /// <param name="to_id">备用砖机id</param>
        /// <returns></returns>
        internal bool CopyOtherDeviceTrackByDevId(uint from_id, uint to_id)
        {
            string str = "INSERT INTO area_device_track ( area_id, device_id, track_id, prior ) SELECT area_id, {0}, track_id, prior FROM area_device_track WHERE device_id = {1}";
            string sql = string.Format(@str, to_id, from_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        /// <summary>
        /// 根据其他砖机轨道id，复制对应摆渡车能去 的备用砖机轨道
        /// </summary>
        /// <param name="from_id">其他砖机的轨道id</param>
        /// <param name="to_id">备用砖机的轨道id</param>
        /// <returns></returns>
        internal bool CopyOtherDeviceTrackByTrackId(uint from_track_id, uint to_track_id)
        {
            string str = "INSERT INTO area_device_track ( area_id, device_id, track_id, prior ) SELECT area_id, device_id, {0}, prior FROM area_device_track WHERE track_id = {1}";
            string sql = string.Format(@str, to_track_id, from_track_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        #endregion

        #region[修改]

        public bool EditArea(Area area)
        {
            string sql = "UPDATE `area` SET `name` = '{0}', `memo` = '{1}', `up_car_count` = {2}, `down_car_count` = {3}" +
                " WHERE `id` = {4}";
            sql = string.Format(sql, area.name, area.memo, area.up_car_count, area.down_car_count, area.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        public bool EditLine(Line line)
        {
            string sql = "UPDATE `line` SET `area_id` = {0}, `line` = {1}, `name` = '{2}', `sort_task_qty` = {3}, `up_task_qty` = {4}, " +
                "`down_task_qty` = {5},  `line_type` = {6}  WHERE `id` = {7};";
            sql = string.Format(sql, line.area_id, line.line, line.name, line.sort_task_qty, line.up_task_qty, line.down_task_qty, line.line_type, line.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditAreaDevice(AreaDevice areadev)
        {
            string sql = "UPDATE `area_device` set area_id = '{0}', `device_id` = {1} where id = '{2}'";
            sql = string.Format(sql, areadev.area_id, areadev.device_id, areadev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditAreaTrack(AreaTrack areatra)
        {
            string sql = "UPDATE `area_track` set area_id = '{0}', `track_id` = {1} where id = '{2}'";
            sql = string.Format(sql, areatra.area_id, areatra.track_id, areatra.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        
        internal bool EditAreaDeviceTrack(AreaDeviceTrack areadevtra)
        {
            string sql = "UPDATE `area_device_track` set area_id = '{0}', `device_id` = {1}, `track_id` = {2} , `prior` = {3} where id = '{4}'";
            sql = string.Format(sql, areadevtra.area_id, areadevtra.device_id, areadevtra.track_id, areadevtra.prior, areadevtra.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        public bool EditAreaLine(Line line)
        {
            string str = "UPDATE `line` SET `area_id` = {0}, `line` = {1}, `name` = '{2}', `sort_task_qty` = {3}, `up_task_qty` = {4}, `down_task_qty` = {5}, " +
                "`max_upsort_num` = {6}, `line_type` = {7}, `full_qty` = {8} WHERE `id` = {9}";
            string sql = string.Format(@str, line.area_id, line.line, line.name, line.sort_task_qty, line.up_task_qty, line.down_task_qty, 
                line.max_upsort_num, line.LineType, line.full_qty, line.id);
            //string sql = "UPDATE `line` SET `max_upsort_num` = {0} WHERE `id` =  '{1}'";
            //sql = string.Format(sql, line.max_upsort_num, line.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditAreaLineOnoff(Line line)
        {
            string sql = "UPDATE `line` SET `onoff_up` = {0}, `onoff_down` = {1}, `onoff_sort` = {2} WHERE `id` =  '{3}'";
            sql = string.Format(sql, line.onoff_up, line.onoff_down, line.onoff_sort, line.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }


        #endregion

        #region[删除]

        internal bool DeleteArea(Area area)
        {
            string sql = string.Format("DELETE FROM `area` where id = '{0}'", area.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool BatchDeleteArea(string areas)
        {
            string sql = string.Format("DELETE FROM `area` where id in ({0})", areas);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        internal bool DeleteLine(Line line)
        {
            string sql = string.Format("DELETE FROM `line` where id = '{0}'", line.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool BatchDeleteLine(string lines)
        {
            string sql = string.Format("DELETE FROM `line` where id in ({0})", lines);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        internal bool DeleteAreaDevice(AreaDevice areadev)
        {
            string sql = string.Format("DELETE FROM `area_device` where id = '{0}'", areadev.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        internal bool DeleteAreaTrack(AreaTrack areatra)
        {
            string sql = string.Format("DELETE FROM `area_track` where id = '{0}'", areatra.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool DeleteAreaDeviceTrack(AreaDeviceTrack areadevtra)
        {
            string sql = string.Format("DELETE FROM `area_device_track` where id = '{0}'", areadevtra.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        /// <summary>
        /// 根据设备id删除设备对应的轨道
        /// </summary>
        /// <param name="dev_id">设备id</param>
        /// <returns></returns>
        internal bool DeleteAreaDeviceTrackByDevId(uint dev_id)
        {
            string sql = string.Format("DELETE FROM `area_device_track` where device_id = '{0}'", dev_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        /// <summary>
        /// 根据轨道id，删除指定的数据
        /// </summary>
        /// <param name="dev_id">设备id</param>
        /// <param name="track_id">轨道id</param>
        /// <returns></returns>
        internal bool DeleteAreaDeviceTrackByTrackId(uint track_id)
        {
            string sql = string.Format("DELETE FROM `area_device_track` where track_id = {0}", track_id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
