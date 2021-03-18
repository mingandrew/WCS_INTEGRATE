using enums;
using System;

namespace module.deviceconfig
{
    public class ConfigTileLifter
    {
        /// <summary>
        /// 砖机 设备ID
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 品种ID
        /// </summary>
        public uint goods_id { set; get; }

        /// <summary>
        /// 左 轨道ID
        /// </summary>
        public uint left_track_id { set; get; }
        
        /// <summary>
        /// 左 轨道地标
        /// </summary>
        public uint left_track_point { set; get; }

        /// <summary>
        /// 右 轨道ID
        /// </summary>
        public uint right_track_id { set; get; }

        /// <summary>
        /// 右 轨道地标
        /// </summary>
        public uint right_track_point { set; get; }

        /// <summary>
        /// 兄弟砖机ID
        /// </summary>
        public uint brother_dev_id { set; get; }

        // 入库策略
        public byte strategy_in { set; get; }

        // 出库策略
        public byte strategy_out { set; get; }

        // 作业类型
        public byte work_type { set; get; }

        /// <summary>
        /// 最后作业轨道ID
        /// </summary>
        public uint last_track_id { set; get; }

        /// <summary>
        /// 转产 旧品种ID
        /// </summary>
        public uint old_goodid { set; get; }

        /// <summary>
        /// 转产 预设品种ID
        /// </summary>
        public uint pre_goodid { set; get; }

        /// <summary>
        /// 是否转产
        /// </summary>
        public bool do_shift { set; get; }

        /// <summary>
        /// 能否切换模式
        /// </summary>
        public bool can_cutover { set; get; }

        // 作业模式
        public byte work_mode { set; get; }

        // 下一个作业模式
        public byte work_mode_next { set; get; }

        /// <summary>
        /// 是否切换模式
        /// </summary>
        public bool do_cutover { set; get; }

        /// <summary>
        /// 是否备用砖机
        /// </summary>
        public bool can_alter { set; get; }

        /// <summary>
        /// 备用的砖机选项列表id（用'#'隔开）
        /// </summary>
        public string alter_ids { set; get; }

        /// <summary>
        /// 当前备用砖机ID
        /// </summary>
        public uint alter_dev_id { set; get; }

        /// <summary>
        /// 砖机设定等级
        /// </summary>
        public byte level { set; get; }

        //没有保存到数据库中
        public DateTime last_shift_time { set; get; }//最近一次转产时间
        /// <summary>
        /// 入库策略
        /// </summary>
        public StrategyInE InStrategey
        {
            get => (StrategyInE)strategy_in;
            set => strategy_in = (byte)value;
        }

        /// <summary>
        /// 出库策略
        /// </summary>
        public StrategyOutE OutStrategey
        {
            get => (StrategyOutE)strategy_out;
            set => strategy_out = (byte)value;
        }

        /// <summary>
        /// 作业类型
        /// </summary>
        public DevWorkTypeE WorkType
        {
            get => (DevWorkTypeE)work_type;
            set => work_type = (byte)value;
        }

        /// <summary>
        /// 作业模式
        /// </summary>
        public TileWorkModeE WorkMode
        {
            get => (TileWorkModeE)work_mode;
            set => work_mode = (byte)value;
        }

        /// <summary>
        /// 下一个作业模式
        /// </summary>
        public TileWorkModeE WorkModeNext
        {
            get => (TileWorkModeE)work_mode_next;
            set => work_mode_next = (byte)value;
        }

        /// <summary>
        /// 是否存在干预砖机
        /// </summary>
        public bool HaveBrother
        {
            get => brother_dev_id != 0;
        }

        /// <summary>
        /// 判断当前转产是否可以执行
        /// 距离最近一次转产时间在5分钟内
        /// </summary>
        /// <returns></returns>
        public bool IsLastShiftTimeOk()
        {
            return DateTime.Now.Subtract(last_shift_time).TotalMinutes >= 5;
        }

        public bool IsInBackUpList(uint devid)
        {
            if (string.IsNullOrEmpty(alter_ids)) return false;
            foreach (var item in alter_ids.Split(','))
            {
                if(uint.TryParse(item, out uint did) && did == devid)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
