using enums;
using module.device;
using socket.process;

namespace simserver.simsocket.process
{
    /// <summary>
    /// 运输车数据处理
    /// </summary>
    public class SimCarrierProcesser : ProcesserBase
    {

        public SimCarrierProcesser()
        {
        }

        /// <summary>
        /// 获取设备的状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[]  GetStatus(DevCarrier dev)
        {
            CarrierStatusStruct st = new CarrierStatusStruct();

            st.Head = ShiftBytes(SimSocketConst.CARRIER_HEAD_KEY);
            st.DeviceID = dev.DeviceID;
            st.DeviceStatus = (byte)dev.DeviceStatus;
            st.CurrentSite = ShiftBytes(dev.CurrentSite);
            st.CurrentPoint = ShiftBytes(dev.CurrentPoint);
            st.TargetSite = ShiftBytes(dev.TargetSite);
            st.TargetPoint = ShiftBytes(dev.TargetPoint);
            st.CurrentOrder = (byte)dev.CurrentOrder;
            st.FinishOrder = (byte)dev.FinishOrder;
            st.LoadStatus = (byte)dev.LoadStatus;
            st.Position = (byte)dev.Position;
            st.OperateMode = (byte)dev.OperateMode;
            st.TakePoint = ShiftBytes(dev.TakeSite);
            st.TakeSite = ShiftBytes(dev.TakePoint);
            st.GivePoint = ShiftBytes(dev.GiveSite);
            st.GiveSite = ShiftBytes(dev.GivePoint);
            st.MoveCount = dev.MoveCount;
            st.Reserve1 = dev.Reserve1;
            st.Reserve2 = dev.Reserve2;
            st.Aler1 = dev.Aler1;
            st.Aler2 = dev.Aler2;
            st.Aler3 = dev.Aler3;
            st.Aler4 = dev.Aler4;
            st.Aler5 = dev.Aler5;
            st.Aler6 = dev.Aler6;
            st.Aler7 = dev.Aler7;
            st.Aler8 = dev.Aler8;
            st.Aler9 = dev.Aler9;
            st.Aler10 = dev.Aler10;
            st.Reserve3 = dev.Reserve3;
            st.Reserve4 = dev.Reserve4;
            st.Tail = ShiftBytes(SimSocketConst.TAIL_KEY);

            return StructToBuffer<CarrierStatusStruct>(st); 
        }


        internal CarrierCmd GetCmd(byte[] data)
        {
            CarrierActionCmdStruct st = BufferToStruct<CarrierActionCmdStruct>(data);
            CarrierCmd cmd = new CarrierCmd
            {
                DeviceID = st.DeviceID,
                Command = (DevCarrierCmdE)st.Command,
                Value1_2 = ShiftBytes(st.Value1_2),
                Value3_4 = ShiftBytes(st.Value3_4),
                Value5_6= ShiftBytes(st.Value5_6),
                CarrierOrder = (DevCarrierOrderE)st.Value7,
                Value8_9 = ShiftBytes(st.Value8_9),
                Value10_11 = ShiftBytes(st.Value10_11),
                Value12 = st.Value12
            };

            return cmd;
        }
    }
}
