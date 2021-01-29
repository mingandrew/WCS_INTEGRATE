
using enums;
using System;

namespace module.device
{
    /// <summary>
    /// 交通管制
    /// </summary>
    public class TrafficControl
    {
        public uint id { set; get; }

        /// <summary>
        /// 区域
        /// </summary>
        public ushort area { set; get; }

        public byte traffic_control_type { set; get; } // 交管类型

        /// <summary>
        /// 被交管设备ID
        /// </summary>
        public uint restricted_id { set; get; }

        /// <summary>
        /// 交管设备ID
        /// </summary>
        public uint control_id { set; get; }

        public byte traffic_control_status { set; get; } // 交管状态

        /// <summary>
        /// 交管起始轨道ID
        /// </summary>
        public uint from_track_id { set; get; }

        /// <summary>
        /// 交管起始点位
        /// </summary>
        public uint from_point { set; get; }

        /// <summary>
        /// 交管起始坐标
        /// </summary>
        public uint from_site { set; get; }

        /// <summary>
        /// 交管结束轨道ID
        /// </summary>
        public uint to_track_id { set; get; }

        /// <summary>
        /// 交管结束点位
        /// </summary>
        public uint to_point { set; get; }

        /// <summary>
        /// 交管结束坐标
        /// </summary>
        public uint to_site { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { set; get; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { set; get; }


        /// <summary>
        /// 交管类型
        /// </summary>
        public TrafficControlTypeE TrafficControlType
        {
            get => (TrafficControlTypeE)traffic_control_type;
            set => traffic_control_type = (byte)value;
        }

        /// <summary>
        /// 交管状态
        /// </summary>
        public TrafficControlStatusE TrafficControlStatus
        {
            get => (TrafficControlStatusE)traffic_control_status;
            set => traffic_control_status = (byte)value;
        }

    }
}
