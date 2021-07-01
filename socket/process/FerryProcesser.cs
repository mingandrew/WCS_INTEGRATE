using enums;
using module.device;
using System.Runtime.InteropServices;

namespace socket.process
{
    #region[接收状态]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FerryStatusStruct
    {
        public ushort Head; //命令字头【0x95,0x01】
        public byte DeviceID;      //设备号
        public byte DeviceStatus;  //设备状态
        public ushort TargetSite;  //目标值
        public byte CurrentTask;   //当前任务
        public ushort DownSite; //下砖测轨道号
        public ushort UpSite; //上砖测轨道号
        public byte FinishTask;    //完成任务
        public byte LoadStatus;    //载货状态
        public byte WorkMode;      //作业模式
        public byte DownLight;       //下砖侧光电
        public byte UpLight;     //上砖侧光电
        public byte Reserve;       //预留
        public ushort Tail; //命令字尾【0xFF,0xFE】

    }

    #endregion

    #region[设置速度]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FerrySpeedStruct
    {
        public ushort Head; //命令字头【0x95,0x02】
        public byte DeviceID;      //设备号
        public byte ManualFast;    //手动快速
        public byte ManualSlow;    //手动慢速
        public byte AutoFast;      //自动快速
        public byte AutoSlow;      //自动慢速
        public byte OverloadStop;  //重载提前停止
        public byte NoloadStop;    //空载提前停止
        public byte Reserve1;       //预留1
        public byte Reserve2;       //预留2
        public byte Reserve3;       //预留3
        public byte Reserve4;       //预留4
        public byte Reserve5;       //预留5
        public byte Reserve6;       //预留6
        public byte Reserve7;       //预留7
        public byte Reserve8;       //预留8
        public ushort Tail; //命令字尾【0xFF,0xFE】

    }
    #endregion

    #region[轨道坐标结果]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FerrySiteStruct
    {
        public ushort Head; //命令字头【0x95,0x03】
        public byte DeviceID;      //设备号
        public ushort TrackCode;       //轨道号
        public int TrackPos;   //已设坐标值
        public int NowTrackPos; //当前坐标值
        public byte Reserve1;       //预留1
        public byte Reserve2;       //预留2
        public byte Reserve3;       //预留3
        public byte Reserve4;       //预留4
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }
    #endregion

    #region[指令发送]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FerryCmdStruct
    {
        public ushort Head; //命令字头【0x94,0x01】
        public byte DeviceID;      //设备号
        public byte Commond; //控制码
        public byte Value1;  //值1
        public byte Value2;  //值2
        public int Value3;//值3
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FerryAutoPosCmdStruct
    {
        public ushort Head; //命令字头【0x94,0x01】
        public byte DeviceID;      //设备号
        public byte Commond; //控制码
        public byte Value1;  //值1
        public byte Value2;  //值2
        public byte Value3;//值3
        public byte Value4;
        public byte Value5;
        public byte Value6;
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }


    #endregion

    public class FerryProcesser : ProcesserBase
    {
        #region[字段]

        DevFerry mDev;
        DevFerrySite mDevSite;

        #endregion

        public FerryProcesser()
        {
            mDev = new DevFerry();
            mDevSite = new DevFerrySite();
        }

        internal DevFerry GetStatus(byte[] data)
        {
            FerryStatusStruct st = BufferToStruct<FerryStatusStruct>(data);

            mDev.ReSetUpdate();
            mDev.DeviceID = st.DeviceID;
            mDev.DeviceStatus = (DevFerryStatusE)st.DeviceStatus;
            mDev.TargetSite = ShiftBytes(st.TargetSite);
            mDev.CurrentTask = (DevFerryTaskE)st.CurrentTask;
            mDev.DownSite = ShiftBytes(st.DownSite);
            mDev.UpSite = ShiftBytes(st.UpSite);
            mDev.FinishTask = (DevFerryTaskE)st.FinishTask;
            //mDev.LoadStatus = (DevFerryLoadE)st.LoadStatus;
            mDev.WorkMode = (DevOperateModeE)st.WorkMode;
            mDev.DownLight = st.DownLight == 1;
            mDev.UpLight = st.UpLight == 1;
            mDev.Reserve = st.Reserve;

            return mDev;
        }

        internal DevFerrySite GetSite(byte[] data)
        {
            FerrySiteStruct st = BufferToStruct<FerrySiteStruct>(data);

            mDevSite.ReSetUpdate();
            mDevSite.DeviceID = st.DeviceID;
            mDevSite.TrackCode = ShiftBytes(st.TrackCode);
            mDevSite.TrackPos = ShiftBytes(st.TrackPos);
            mDevSite.NowTrackPos= ShiftBytes(st.NowTrackPos);
            mDevSite.Reserve = st.Reserve1;

            return mDevSite;
        }

        internal IDevice GetSpeed(byte[] pdata)
        {
            return new IDevice();
        }

        internal byte[] GetCmd(string devid, DevFerryCmdE type, byte b1, byte b2, int int3)
        {
            FerryCmdStruct cmd = new FerryCmdStruct();
            cmd.Head = ShiftBytes(SocketConst.FERRY_CMD_HEAD_KEY);
            cmd.DeviceID = byte.Parse(devid);
            cmd.Commond = (byte)type;
            cmd.Value1 = b1;
            cmd.Value2 = b2;
            cmd.Value3 =  ShiftBytes(int3);
            cmd.Tail = ShiftBytes(SocketConst.TAIL_KEY);
            return StructToBuffer(cmd);
        }

        internal byte[] GetAutoPosCmd(string devid, DevFerryCmdE type, byte b1, byte b2, byte b3, byte b4)
        {
            FerryAutoPosCmdStruct cmd = new FerryAutoPosCmdStruct();
            cmd.Head = ShiftBytes(SocketConst.FERRY_CMD_HEAD_KEY);
            cmd.DeviceID = byte.Parse(devid);
            cmd.Commond = (byte)type;
            cmd.Value1 = b1;
            cmd.Value2 = b2;
            cmd.Value3 = b3;
            cmd.Value4 = b4;
            cmd.Tail = ShiftBytes(SocketConst.TAIL_KEY);
            return StructToBuffer(cmd);
        }

    }
}
