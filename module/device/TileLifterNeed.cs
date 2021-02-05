using enums;
using System;

namespace module.device
{
    //砖机需求
    public class TileLifterNeed
    {
        public uint device_id { set; get; }
        public uint track_id { set; get; }
        public bool left { set; get; }
        public uint trans_id { set; get; }
        public DateTime? create_time { set; get; }
        public DateTime? trans_create_time { set; get; }
        public bool finish { set; get; }
        public byte type { set; get; }
        public uint area_id { set; get; }

        public DeviceTypeE need_type
        {
            get => (DeviceTypeE)type;
            set => type = (byte)value;
        }
    }
}
