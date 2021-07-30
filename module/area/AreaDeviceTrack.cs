namespace module.area
{
    public class AreaDeviceTrack
    {
        public uint id { set; get; }
        public uint area_id { set; get; }
        public uint device_id { set; get; }
        public uint track_id { set; get; }
        public ushort prior { set; get; }

        public bool can_up { set; get; }
        public bool can_down { set; get; }



        /// <summary>
        /// 虚拟字段：是否可更改上下砖作业选项
        /// </summary>
        public bool IsEnabledToUpDown { set; get; }
    }
}