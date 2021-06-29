using enums.track;
using System.Linq;

namespace module.track
{
    public class CarrierPos
    {
        public uint id { set; get; }//标识
        public uint area_id { set; get; }//区域ID
        public ushort track_point { set; get; }//轨道号（复位号码）
        public ushort track_pos { set; get; }//轨道脉冲值

        public CarrierPosE CarrierPosType => (CarrierPosE)track_point;

        /// <summary>
        /// 检查是否符合类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InType(params CarrierPosE[] types)
        {
            return types.Contains(CarrierPosType);
        }

    }
}
