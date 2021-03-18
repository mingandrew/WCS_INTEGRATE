using enums;
using module.device;
using System.Runtime.InteropServices;

namespace socket.process
{
    #region[状态信息]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarrierStatusStruct
    {
        public ushort Head;  //命令字头【0x97,0x01】
        public byte DeviceID;       //设备号
        public byte DeviceStatus;   //设备状态
        public ushort CurrentSite;  //当前RFID
        public ushort CurrentPoint;  //当前坐标
        public ushort TargetSite;  //目的RFID
        public ushort TargetPoint;  //目的坐标
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

    #region[指令发送]

    /// <summary>
    /// 基础指令
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarrierBaseCmdStruct
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
    public class CarrierProcesser : ProcesserBase
    {
        #region[字段]
        DevCarrier mDev;
        #endregion

        public CarrierProcesser()
        {
            mDev = new DevCarrier();
        }

        /// <summary>
        /// 获取设备的状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DevCarrier GetStatus(byte[] data)
        {
            CarrierStatusStruct st = BufferToStruct<CarrierStatusStruct>(data);
            mDev.ReSetUpdate();
            mDev.DeviceID = st.DeviceID;
            mDev.DeviceStatus = (DevCarrierStatusE)st.DeviceStatus;
            mDev.CurrentSite = ShiftBytes(st.CurrentSite);
            mDev.CurrentPoint = ShiftBytes(st.CurrentPoint);
            mDev.TargetSite = ShiftBytes(st.TargetSite);
            mDev.TargetPoint = ShiftBytes(st.TargetPoint);
            mDev.CurrentOrder = (DevCarrierOrderE)st.CurrentOrder;
            mDev.FinishOrder = (DevCarrierOrderE)st.FinishOrder;
            mDev.LoadStatus = (DevCarrierLoadE)st.LoadStatus;
            mDev.Position = (DevCarrierPositionE)st.Position;
            mDev.OperateMode = (DevOperateModeE)st.OperateMode;
            mDev.TakeSite = ShiftBytes(st.TakePoint);
            mDev.TakePoint = ShiftBytes(st.TakeSite);
            mDev.GiveSite = ShiftBytes(st.GivePoint);
            mDev.GivePoint = ShiftBytes(st.GiveSite);
            mDev.MoveCount = st.MoveCount;
            mDev.Reserve1 = st.Reserve1;
            mDev.Reserve2 = st.Reserve2;
            mDev.Aler1 = st.Aler1;
            mDev.Aler2 = st.Aler2;
            mDev.Aler3 = st.Aler3;
            mDev.Aler4 = st.Aler4;
            mDev.Aler5 = st.Aler5;
            mDev.Aler6 = st.Aler6;
            mDev.Aler7 = st.Aler7;
            mDev.Aler8 = st.Aler8;
            mDev.Aler9 = st.Aler9;
            mDev.Aler10 = st.Aler10;
            mDev.Reserve3 = st.Reserve3;
            mDev.Reserve4 = st.Reserve4;
            return mDev;
        }

        /// <summary>
        /// 获取指令（值全为 0 ）
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal byte[] GetCmd(string devid, DevCarrierCmdE type)
        {
            CarrierBaseCmdStruct cmd = new CarrierBaseCmdStruct
            {
                Head = ShiftBytes(SocketConst.CARRIER_CMD_HEAD_KEY),
                DeviceID = byte.Parse(devid),
                Command = (byte)type,
                Tail = ShiftBytes(SocketConst.TAIL_KEY)
            };

            return StructToBuffer(cmd);
        }

        /// <summary>
        /// 获取指令（逐个赋值）
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="v5"></param>
        /// <param name="v6"></param>
        /// <returns></returns>
        internal byte[] GetCmd(string devid, DevCarrierCmdE type, DevCarrierOrderE order, 
            ushort v1, ushort v2, ushort v3, ushort v4, ushort v5, byte v6)
        {
            CarrierActionCmdStruct cmd = new CarrierActionCmdStruct
            {
                Head = ShiftBytes(SocketConst.CARRIER_CMD_HEAD_KEY),
                DeviceID = byte.Parse(devid),
                Command = (byte)type,
                Value1_2 = ShiftBytes(v1),
                Value3_4 = ShiftBytes(v2),
                Value5_6 = ShiftBytes(v3),
                Value7 = (byte)order,
                Value8_9 = ShiftBytes(v4),
                Value10_11 = ShiftBytes(v5),
                Value12 = v6,
                Tail = ShiftBytes(SocketConst.TAIL_KEY)
            };

            return StructToBuffer(cmd);
        }

    }
}
