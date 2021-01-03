using enums;

namespace module.area
{
    public class AreaDevice
    {
        public uint id { set; get; }
        public uint area_id { set; get; }
        public uint device_id { set; get; }
        public byte dev_type { set; get; }

        public DeviceTypeE DevType
        {
            get => (DeviceTypeE)dev_type;
            set => dev_type = (byte)value;
        }
    }
}
