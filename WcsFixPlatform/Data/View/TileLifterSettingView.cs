using enums;
using GalaSoft.MvvmLight;
using module.device;
using module.deviceconfig;
using resource;
using System.Windows.Media;

namespace wcs.Data.View
{
    public class TileLifterSettingView : ViewModelBase
    {
        #region[字段]

        #region[device]
        public uint id{set;get;}
        public string name{set;get;}
        public string ip{set;get;}
        public ushort port{set;get;}
        public uint type{set;get;}
        public uint type2{set;get;}
        public ushort area{set;get;}
        public ushort line{set;get;}
        #endregion

        #region[config]

        public uint brother_dev_id{set;get;}
        public uint left_track_id { set;get;}
        public ushort left_track_point { set;get;}
        public uint right_track_id { set;get;}
        public ushort right_track_point { set;get;}
        public bool can_cutover { set;get;}
        public bool can_alter { set;get;}
        public string alter_ids { set; get; }
        public string AlterNames { set; get; }

        // 作业模式
        public byte work_mode { set; get; }

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

        /// <summary>
        /// 作业模式
        /// </summary>
        public TileWorkModeE WorkMode
        {
            get => (TileWorkModeE)work_mode;
            set => work_mode = (byte)value;
        }
        #endregion

        #region[构造]

        public TileLifterSettingView(Device dev, ConfigTileLifter config)
        {
            id = dev.id;
            name = dev.name;
            ip = dev.ip;
            port = dev.port;
            type = dev.type;
            type2 = dev.type2;
            area = dev.area;
            line = dev.line;
            brother_dev_id = config.brother_dev_id;
            left_track_id = config.left_track_id;
            left_track_point = config.left_track_point;
            right_track_id = config.right_track_id;
            right_track_point = config.right_track_point;
            can_cutover = config.can_cutover;
            can_alter = config.can_alter;
            alter_ids = config.alter_ids;

            GetAlterDevices();
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
        /// 转换成ConfigTileLifter
        /// </summary>
        /// <returns></returns>
        public ConfigTileLifter TransformIntoConfigTileLifter()
        {
            ConfigTileLifter config = new ConfigTileLifter()
            {
                id = id,
                brother_dev_id = brother_dev_id,
                left_track_id = left_track_id,
                left_track_point = left_track_point,
                right_track_id = right_track_id,
                right_track_point = right_track_point,
                can_alter = can_alter,
                can_cutover = can_cutover,
                alter_ids = alter_ids,
                WorkMode = WorkMode,
            };
            if (config.alter_ids != null)
            {
                config.GetAlertDevList();
            }

            return config;
        }

        #endregion

        #region [方法]
        public void GetAlterDevices()
        {
            if (alter_ids == null)
            {
                AlterNames = "";
                return;
            }
            string[] alteridList = alter_ids.Split(',');
            string dev_name = "";
            foreach (string id in alteridList)
            {
                if (uint.TryParse(id, out uint num))
                {
                    dev_name += PubMaster.Device.GetDeviceName(num);
                }
            }
            AlterNames = dev_name;
        }
        #endregion
    }
}
