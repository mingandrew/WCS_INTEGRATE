using enums.track;

namespace module.track
{
    public class CarrierPos
    {
        public uint id { set; get; }//标识
        public uint area_id { set; get; }//区域ID
        public ushort track_point { set; get; }//轨道号
        public ushort track_pos { set; get; }//轨道脉冲值

        public CarrierPosE CarrierPosType => (CarrierPosE)track_point;
    }
}
