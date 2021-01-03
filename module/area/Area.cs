using enums;

namespace module.area
{
    public class Area
    {
        public uint id { set; get; }
        public string name { set; get; }
        public bool enable { set; get; }
        public bool devautorun { set; get; }
        public string memo { set; get; }
        public byte carriertype { set; get; }//运输车类型
        public int c_sorttask { set; get; }//分配限制
        public byte full_qty { set; get; }//满砖数量

        public CarrierTypeE CarrierType
        {
            get => (CarrierTypeE)carriertype;
            set => carriertype = (byte)value;
        }
    }
}
