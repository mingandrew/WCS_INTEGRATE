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
        public byte Value13;        //值13

        #region[执行任务]
        /// <summary>
        /// 值1-2：校验用轨道编号
        /// </summary>
        public ushort CheckTrackCode
        {
            get => Value1_2;
        }
        /// <summary>
        /// 值3-4：定位用RFID编号
        /// </summary>
        public ushort TargetSite
        {
            get => Value3_4;
        }

        /// <summary>
        /// 值5-6：定位用坐标值
        /// </summary>
        public ushort TargetPoint
        {
            get => Value5_6;
        }

        /// <summary>
        /// 值8-9：结束用RFID编号
        /// </summary>
        public ushort FinishSite
        {
            get => Value8_9;
        }

        /// <summary>
        /// 值10-11：结束用坐标值
        /// </summary>
        public ushort FinishPoint
        {
            get => Value10_11;
        }

        /// <summary>
        /// 值12：倒库数量
        /// </summary>
        public byte SortQty
        {
            get => Value12;
        }

        /// <summary>
        /// 值13：标识码
        /// </summary>
        public byte MarkCode
        {
            get => Value13;
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
