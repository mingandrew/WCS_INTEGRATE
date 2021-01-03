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
        public ushort length { set; get; }
        public ushort width { set; get; }
        public byte stack { set; get; }
        public ushort pieces { set; get; }
        public bool oversize { set; get; }
        public byte carriertype { set; get; }
        public string memo { set; get; }
        public DateTime? updatetime { set; get; }
        public ushort minstack { set; get; }//该规格最少库存数
        public CarrierTypeE GoodCarrierType
        {
            get => (CarrierTypeE)carriertype;
            set => carriertype = (byte)value;
        }
    }
}
