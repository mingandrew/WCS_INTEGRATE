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
            CarrierStatusStruct st = new CarrierStatusStruct
            {
                Head = ShiftBytes(SimSocketConst.CARRIER_HEAD_KEY),
                DeviceID = dev.DeviceID,
                DeviceStatus = (byte)dev.DeviceStatus,
                CurrentSite = ShiftBytes(dev.CurrentSite),
                CurrentPoint = ShiftBytes(dev.CurrentPoint),
                TargetSite = ShiftBytes(dev.TargetSite),
                TargetPoint = ShiftBytes(dev.TargetPoint),
                CurrentOrder = (byte)dev.CurrentOrder,
                FinishOrder = (byte)dev.FinishOrder,
                LoadStatus = (byte)dev.LoadStatus,
                Position = (byte)dev.Position,
                OperateMode = (byte)dev.OperateMode,
                TakePoint = ShiftBytes(dev.TakeSite),
                TakeSite = ShiftBytes(dev.TakePoint),
                GivePoint = ShiftBytes(dev.GiveSite),
                GiveSite = ShiftBytes(dev.GivePoint),
                MoveCount = dev.MoveCount,
                ResetID = dev.ResetID,
                ResetPoint = dev.ResetPoint,
                Aler1 = dev.Aler1,
                Aler2 = dev.Aler2,
                Aler3 = dev.Aler3,
                Aler4 = dev.Aler4,
                Aler5 = dev.Aler5,
                Aler6 = dev.Aler6,
                Aler7 = dev.Aler7,
                Aler8 = dev.Aler8,
                Aler9 = dev.Aler9,
                Aler10 = dev.Aler10,
                OrderStep = dev.OrderStep,
                MarkCode = dev.MarkCode,
                Tail = ShiftBytes(SimSocketConst.TAIL_KEY)
            };

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
                Value12 = st.Value12,
                Value13 = st.Value13
            };

            return cmd;
        }
    }
}
