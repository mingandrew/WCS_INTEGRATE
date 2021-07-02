using enums.track;
using System.Linq;

namespace module.track
{
    public class CarrierPos
    {
        /// <summary>
        /// 标识
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public uint area_id { set; get; }

        /// <summary>
        /// 复位号码
        /// </summary>
        public ushort track_point { set; get; }

        /// <summary>
        /// 轨道脉冲值
        /// </summary>
        public ushort track_pos { set; get; }

        /// <summary>
        /// 复位点类型
        /// </summary>
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
