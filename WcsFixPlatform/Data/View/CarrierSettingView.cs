using enums;
using GalaSoft.MvvmLight;
using module.device;
using module.deviceconfig;
using resource;
using System.Collections.Generic;

namespace wcs.Data.View
{
    public class CarrierSettingView : ViewModelBase
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
        /// 运输顶板长度（脉冲）
        /// </summary>
        public ushort length { set; get; }

        /// <summary>
        /// 运输车负责规格ID集
        /// </summary>
        public string goods_size { set; get; }

        /// <summary>
        /// 运输车负责规格Name集
        /// </summary>
        public string goods_size_name { set; get; }

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
        public CarrierSettingView(Device dev, ConfigCarrier config)
        {
            id = dev.id;
            name = dev.name;
            ip = dev.ip;
            port = dev.port;
            type = dev.type;
            type2 = dev.type2;
            area = dev.area;
            line = dev.line;

            length = config.length;
            goods_size = config.goods_size;
            GetSizeName();
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
        /// 转换成ConfigCarrier
        /// </summary>
        /// <returns></returns>
        public ConfigCarrier TransformIntoConfigCarrier()
        {
            ConfigCarrier config = new ConfigCarrier()
            {
                id = id,
                length = length,
                goods_size = goods_size,
            };
            return config;
        }

        #endregion

        #region [方法]
        /// <summary>
        /// 获取size名称的集合
        /// </summary>
        public void GetSizeName()
        {
            if (string.IsNullOrEmpty(goods_size))
            {
                goods_size_name = "";
                return;
            }
            string[] sizeidList = goods_size.Split('#');
            List<string> size_name = new List<string>();
            foreach (string sid in sizeidList)
            {
                if (uint.TryParse(sid, out uint num))
                {
                    size_name.Add(PubMaster.Goods.GetSizeName(num));
                }
            }
            goods_size_name = string.Join(" # ", size_name);
        }
        #endregion
    }
}
