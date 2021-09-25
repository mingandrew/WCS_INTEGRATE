﻿namespace module.deviceconfig
{
    public class ConfigFerry
    {
        /// <summary>
        /// 摆渡车 设备ID
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 摆渡轨道ID
        /// </summary>
        public uint track_id { set; get; }

        /// <summary>
        /// 摆渡轨道地标
        /// </summary>
        public ushort track_point { set; get; }

        public void Update(ConfigFerry config)
        {
            track_id = config.track_id;
            track_point = config.track_point;
        }
    }
}
