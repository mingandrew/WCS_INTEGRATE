using enums;
using module.device;
using socket.process;
using System.Runtime.InteropServices;

namespace simserver.simsocket.process
{
    #region[状态信息]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarrierStatusStruct
    {
        public ushort Head;  //命令字头【0x97,0x01】
        public byte DeviceID;       //设备号
        public byte DeviceStatus;   //设备状态
        public ushort CurrentPoint;  //当前RFID
        public ushort CurrentSite;  //当前坐标
        public ushort TargetPoint;  //目的RFID
        public ushort TargetSite;  //目的坐标
        public byte CurrentOrder;    //当前指令
        public byte FinishOrder;     //完成指令
        public byte LoadStatus;     //载货状态
        public byte Position;       //所在位置
        public byte OperateMode;    //操作模式
        public ushort TakePoint;  //取货RFID
        public ushort TakeSite;  //取货坐标
        public ushort GivePoint;  //卸货RFID
        public ushort GiveSite;  //卸货坐标
        public byte MoveCount;  //倒库数量
        public byte Reserve1;        //预留1
        public byte Reserve2;        //预留2
        public byte Aler1;          //报警1
        public byte Aler2;          //报警2
        public byte Aler3;          //报警3
        public byte Aler4;          //报警4
        public byte Aler5;          //报警5
        public byte Aler6;          //报警6
        public byte Aler7;          //报警7
        public byte Aler8;          //报警8
        public byte Aler9;          //报警9
        public byte Aler10;          //报警10
        public byte Reserve3;        //预留3
        public byte Reserve4;        //预留4
        public ushort Tail; //命令字尾【0xFF,0xFE】

    }
    #endregion

    #region[速度结果]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarrierSpeedStruct
    {
        public ushort Head; //命令字头【0x97,0x02】
        public byte DeviceID;      //设备号
        public byte ManualFast;    //手动快速
        public byte ManualSlow;    //手动慢速
        public byte AutoFast;      //自动快速
        public byte AutoSlow;      //自动慢速
        public byte Reserve;       //预留
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }
    #endregion

    #region[指令发送]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarrierCmdStruct
    {
        public ushort Head; //命令字头【0x96,0x01】
        public byte DeviceID;      //设备号
        public byte Command;       //控制码
        public byte Value1;        //值1
        public byte Value2;        //值2
        public byte Value3;        //值3
        public byte Value4;        //值4
        public byte Value5;        //值5
        public byte Value6;        //值6
        public byte Value7;        //值7
        public byte Value8;        //值8
        public byte Value9;        //值9
        public byte Value10;        //值10
        public byte Value11;        //值11
        public byte Value12;        //值12
        public ushort Tail; //命令字尾【0xFF,0xFE】

    }

    /// <summary>
    /// 动作指令
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarrierActionCmdStruct
    {
        public ushort Head; //命令字头【0x96,0x01】
        public byte DeviceID;      //设备号
        public byte Command;       //控制码
        public ushort Value1_2;        //值1-2
        public ushort Value3_4;        //值3-4
        public ushort Value5_6;        //值5-6
        public byte Value7;        //值7
        public ushort Value8_9;        //值8-9
        public ushort Value10_11;        //值10-11
        public byte Value12;        //值12
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }

    #endregion

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
            st.CurrentPoint = ShiftBytes(dev.CurrentPoint);
            st.CurrentSite = ShiftBytes(dev.CurrentSite);
            st.TargetPoint = ShiftBytes(dev.TargetPoint);
            st.TargetSite = ShiftBytes(dev.TargetSite);
            st.CurrentOrder = (byte)dev.CurrentOrder;
            st.FinishOrder = (byte)dev.FinishOrder;
            st.LoadStatus = (byte)dev.LoadStatus;
            st.Position = (byte)dev.Position;
            st.OperateMode = (byte)dev.OperateMode;
            st.TakePoint = ShiftBytes(dev.TakePoint);
            st.TakeSite = ShiftBytes(dev.TakeSite);
            st.GivePoint = ShiftBytes(dev.GivePoint);
            st.GiveSite = ShiftBytes(dev.GiveSite);
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
                Value7 = (DevCarrierOrderE)st.Value7,
                Value8_9 = ShiftBytes(st.Value8_9),
                Value10_11 = ShiftBytes(st.Value10_11),
                Value12 = st.Value12
            };

            if (st.Command != 0)
            {
                return cmd;
            }

            return cmd;
        }
    }
}
