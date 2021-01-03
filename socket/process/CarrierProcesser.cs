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
        public ushort CurrentSite;  //当前值
        public byte CurrentTask;    //当前任务
        public byte CurrentOverSize;//超限
        public byte FinishTask;     //完成任务
        //public byte FinishOverSize; //超限
        public byte LoadStatus;     //载货状态
        public byte WorkMode;       //系统模式
        public byte OperateMode;    //操作模式
        public ushort ActionTime;     //取放时间
        public ushort TakeTrackCode;//取货轨道号
        public ushort GiveTrackCode;//取货轨道号
        public byte ActionType;     // 空满砖信息
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
        public byte Reserve1;        //预留1
        public byte Reserve2;        //预留2
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
            mDev.CurrentTask = (DevCarrierTaskE)st.CurrentTask;
            mDev.CurrentOverSize = (DevCarrierSizeE)st.CurrentOverSize;
            mDev.FinishTask = (DevCarrierTaskE)st.FinishTask;
            //mDev.FinishOverSize = (DevCarrierSizeE)st.FinishOverSize;
            mDev.LoadStatus = (DevCarrierLoadE)st.LoadStatus;
            mDev.WorkMode = (DevCarrierWorkModeE)st.WorkMode;
            mDev.OperateMode = (DevOperateModeE)st.OperateMode;
            mDev.ActionTime = ShiftBytes(st.ActionTime);
            mDev.TakeTrackCode = ShiftBytes(st.TakeTrackCode);
            mDev.GiveTrackCode = ShiftBytes(st.GiveTrackCode);
            mDev.ActionType = (DevCarrierSignalE)st.ActionType;
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
            mDev.Reserve1 = st.Reserve1;
            mDev.Reserve2 = st.Reserve2;
            return mDev; 
        }


        internal byte[] GetCmd(string devid, DevCarrierCmdE type, byte v1, byte v2, DevCarrierResetE reset)
        {
            CarrierCmdStruct cmd = new CarrierCmdStruct();

            cmd.Head = ShiftBytes(SocketConst.CARRIER_CMD_HEAD_KEY);
            cmd.DeviceID = byte.Parse(devid);
            cmd.Command = (byte)type;
            cmd.Value1 = v1;
            cmd.Value2 = v2;
            cmd.Value3 = (byte)reset;
            cmd.Tail = ShiftBytes(SocketConst.TAIL_KEY);

            return StructToBuffer(cmd);
        }
    }
}
