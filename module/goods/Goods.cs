using enums;
using System;

namespace module.goods
{
    public class Goods
    {
        public uint id { set; get; }
        public uint area_id { set; get; }
        public string name { set; get; }
        public string color { set; get; }
        public uint size_id { set; get; }
        public ushort pieces { set; get; }
        public byte carriertype { set; get; }
        public string memo { set; get; }
        public DateTime? updatetime { set; get; }
        public ushort minstack { set; get; }//该品种最少库存数
        public CarrierTypeE GoodCarrierType
        {
            get => (CarrierTypeE)carriertype;
            set => carriertype = (byte)value;
        }
        //public byte level { set; get; }
        public string info { set; get; }
        public DateTime? createtime { set; get; }
        public bool top { set; get; }
        public bool empty { set; get; }
    }
}
