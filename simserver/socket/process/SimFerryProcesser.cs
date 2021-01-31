using enums;
using module.device;
using socket.process;
using System.Runtime.InteropServices;

namespace simserver.simsocket.process
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

    #endregion

    public class SimFerryProcesser : ProcesserBase
    {
        #region[字段]

        #endregion

        public SimFerryProcesser()
        {
        }

        internal byte[] GetStatus(DevFerry dev)
        {
            FerryStatusStruct st = new FerryStatusStruct();

            st.Head = ShiftBytes(SimSocketConst.FEERY_STATUS_HEAD_KEY);
            st.DeviceID = dev.DeviceID;
            st.DeviceStatus = (byte)dev.DeviceStatus;
            st.TargetSite = ShiftBytes(dev.TargetSite);
            st.CurrentTask = (byte)dev.CurrentTask;
            st.DownSite = ShiftBytes(dev.DownSite);
            st.UpSite = ShiftBytes(dev.UpSite);
            st.FinishTask = (byte)dev.FinishTask;
            st.LoadStatus = (byte)dev.LoadStatus;
            st.WorkMode = (byte)dev.WorkMode;
            st.DownLight = (byte)(dev.DownLight ? 1: 0);
            st.UpLight = (byte)(dev.UpLight ? 1 : 0); ;
            st.Reserve = dev.Reserve;
            st.Tail = ShiftBytes(SimSocketConst.TAIL_KEY);

            return StructToBuffer<FerryStatusStruct>(st);
        }

        //internal DevFerrySite GetSite(byte[] data)
        //{
        //    FerrySiteStruct st = BufferToStruct<FerrySiteStruct>(data);

        //    mDevSite.ReSetUpdate();
        //    mDevSite.DeviceID = st.DeviceID;
        //    mDevSite.TrackCode = ShiftBytes(st.TrackCode);
        //    mDevSite.TrackPos = ShiftBytes(st.TrackPos);
        //    mDevSite.NowTrackPos= ShiftBytes(st.NowTrackPos);
        //    mDevSite.Reserve = st.Reserve1;

        //    return mDevSite;
        //}

        internal IDevice GetSpeed(byte[] pdata)
        {
            return new IDevice();
        }

        internal FerryCmd GetCmd(byte[] data)
        {
            FerryCmdStruct st = BufferToStruct<FerryCmdStruct>(data);
            FerryCmd cmd = new FerryCmd();

            cmd.DeviceID = st.DeviceID;
            cmd.Commond = (DevFerryCmdE)st.Commond;
            cmd.Value1 = st.Value1;
            cmd.Value2 = st.Value2;
            cmd.Value3 =  ShiftBytes(st.Value3);
            return cmd;
        }
    }
}
