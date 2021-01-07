using enums;
using System;
using System.Net.Configuration;

namespace module.device
{
    public class Device
    {
        public uint id { set; get; }
        public string name { set; get; }
        public string ip { set; get; }
        public ushort port { set; get; }
        public byte type { set; get; }
        public byte type2 { set; get; }
        public bool enable { set; get; }
        public byte att1 { set; get; }//用于区分运输车类型  窄 宽
        public byte att2 { set; get; }//用于上砖机优先清空轨道使用
        public string memo { set; get; }
        public ushort area { set; get; }
        public bool do_work { set; get; }//是否作业


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
        /// 运输车类型
        /// </summary>
        public CarrierTypeE CarrierType
        {
            get => (CarrierTypeE)att1;
            set => att1 = (byte)value;
        }

    }
}
