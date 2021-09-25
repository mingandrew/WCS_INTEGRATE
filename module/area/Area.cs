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

        public uint up_car_count { set; get; }//上砖侧设定的运输车数量
        public uint down_car_count { set; get; }//下砖侧设定的运输车数量

        public CarrierTypeE CarrierType
        {
            get => (CarrierTypeE)carriertype;
            set => carriertype = (byte)value;
        }

        public void Update(Area a)
        {
            name = a.name;
            up_car_count = a.up_car_count;
            down_car_count = a.down_car_count;
        }
    }
}
