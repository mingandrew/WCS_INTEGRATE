using enums;
using GalaSoft.MvvmLight;
using module.device;
using module.deviceconfig;

namespace module.window.device
{
    public class FerrySettingView : ViewModelBase
    {
        #region[字段]

        #region[device]
        public uint id { set; get; }
        public string name { set; get; }
        public string ip { set; get; }
        public ushort port { set; get; }
        public uint type { set; get; }
        public uint type2 { set; get; }
        public ushort area { set; get; }
        public ushort line { set; get; }
        #endregion

        #region[config]

        /// <summary>
        /// 摆渡轨道ID
        /// </summary>
        public uint track_id { set; get; }

        /// <summary>
        /// 摆渡轨道地标
        /// </summary>
        public ushort track_point { set; get; }

        #endregion

        #endregion

        #region[属性]

        public DeviceTypeE Type
        {
            get => (DeviceTypeE)type;
            set => type = (byte)value;
        }

        public DeviceType2E Type2
        {
            get => (DeviceType2E)type2;
            set => type2 = (byte)value;
        }

        #endregion

        #region[构造]

        public FerrySettingView(Device dev, ConfigFerry config)
        {
            id = dev.id;
            name = dev.name;
            ip = dev.ip;
            port = dev.port;
            type = dev.type;
            type2 = dev.type2;
            area = dev.area;
            line = dev.line;

            track_id = config.track_id;
            track_point = config.track_point;
        }

        #endregion

        #region[转换]

        /// <summary>
        /// 转换成Device
        /// </summary>
        /// <returns></returns>
        public Device TransformIntoDevice()
        {
            Device dev = new Device()
            {
                id = id,
                name = name,
                ip = ip,
                port = port,
                Type = Type,
                Type2 = Type2,
                area = area,
                line = line,
            };
            return dev;
        }

        /// <summary>
        /// 转换成ConfigFerry
        /// </summary>
        /// <returns></returns>
        public ConfigFerry TransformIntoConfigFerry()
        {
            ConfigFerry config = new ConfigFerry()
            {
                id = id,
                track_id = track_id,
                track_point = track_point,
            };
            return config;
        }

        #endregion
    }
}
