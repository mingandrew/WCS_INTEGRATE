using enums;
using module.device;

namespace simserver.simsocket
{
    /// <summary>
    /// 运输车接收指令状态
    /// </summary>
    public class CarrierCmd : IDevice
    {
        public byte DeviceID;      //设备号
        public DevCarrierCmdE Command { set; get; }       //控制码
        public ushort Value1_2;        //值1-2
        public ushort Value3_4;        //值3-4
        public ushort Value5_6;        //值5-6
        public DevCarrierOrderE CarrierOrder;        //值7
        public ushort Value8_9;        //值8-9
        public ushort Value10_11;        //值10-11
        public byte Value12;        //值12

        #region[执行任务]
        public ushort CheckTrackCode
        {
            get => Value1_2;
        }

        public ushort LocateSite
        {
            get => Value3_4;
        }

        public ushort LocatePoint
        {
            get => Value5_6;
        }

        public ushort EndSite
        {
            get => Value8_9;
        }

        public ushort EndPoint
        {
            get => Value10_11;
        }

        /// <summary>
        /// 倒库数量
        /// </summary>
        public byte SortQty
        {
            get => Value12;
        }

        #endregion

        #region[设置轨道对应坐标值]

        public ushort SetSite
        {
            get => Value3_4;
        }
        public ushort SetPoint
        {
            get => Value5_6;
        }

        #endregion
    }
}
