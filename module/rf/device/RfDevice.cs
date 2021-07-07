using enums;
using module.device;
using module.deviceconfig;

namespace module.rf.device
{
    public class RfDevice
    {
        public uint id { set; get; }
        public string name { set; get; }
        public byte type { set; get; }
        public byte type2 { set; get; }
        public bool enable { set; get; }
        public byte att1 { set; get; }//用于区分运输车类型  窄 宽
        public byte att2 { set; get; }
        public uint goods_id { set; get; }
        public string memo { set; get; }
        public uint area { set; get; }
        public bool ignorearea { set; get; }
        public uint old_goodid { set; get; }//旧品种ID
        public uint pre_goodid { set; get; }//预设品种ID

        private void CopyValue(Device dev)
        {
            id = dev.id;
            name = dev.name;
            type = dev.type;
            type2 = dev.type2;
            enable = dev.enable;
            att1 = dev.att1;
            att2 = dev.att2;
            memo = dev.memo;
            area = dev.area;
        }

        private void CopyValue(Device dev,ConfigTileLifter config)
        {
            id = dev.id;
            name = dev.name;
            if (dev.Type == DeviceTypeE.砖机)
            {
                switch (config.WorkMode)
                {
                    case TileWorkModeE.上砖:
                        type = (byte)DeviceTypeE.上砖机;
                        break;
                    case TileWorkModeE.下砖:
                    case TileWorkModeE.补砖:
                        type = (byte)DeviceTypeE.下砖机;
                        break;
                }
            }
            else
            {
            type = dev.type;
            }
            type2 = dev.type2;
            enable = dev.enable;
            att1 = dev.att1;
            att2 = dev.att2;
            memo = dev.memo;
            area = dev.area;
        }



        public RfDevice(Device dev, ConfigTileLifter config)
        {
            CopyValue(dev,config);
            goods_id = config.goods_id;
            old_goodid = config.old_goodid;
            pre_goodid = config.pre_goodid;
        }

        public RfDevice(Device dev, ConfigCarrier config)
        {
            CopyValue(dev);
        }

        public RfDevice(Device dev, ConfigFerry config)
        {
            CopyValue(dev);

        }

    }
}
