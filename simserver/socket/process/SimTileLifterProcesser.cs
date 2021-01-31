using enums;
using module.device;
using socket.process;
using System.Runtime.InteropServices;

namespace simserver.simsocket.process
{
    #region[接收状态]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TileStatusStruct
    {
        public ushort Head; //命令字头【0x91,0x01】
        public byte DeviceID;      //设备号
        public byte LoadStatus1;   //货物状态1 左
        public byte LoadStatus2;   //货物状态2 右
        public byte NeedStatus1;   //需求信号1 左
        public byte NeedStatus2;   //需求信号2 右
        public byte FullQty;       //满砖数量
        public byte RecentQty;     //当前数量
        public byte Involve1;      //介入状态1 左
        public byte Involve2;      //介入状态2 右
        public byte OperateMode;   //操作模式
        public uint Goods1;   //工位1品种
        public uint Goods2;   //工位2品种
        public byte ShiftStatus;   //转产状态
        public byte ShiftAccept;   //转产接收状态
        public byte WorkMode;   //作业模式
        public uint SetGoods;   //设定品种
        public byte SetLevel;   //设定等级
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }

    #endregion

    #region[命令发送]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TileCmdStruct
    {
        public ushort Head; //命令字头【0x90,0x01】
        public byte DeviceID;      //设备号
        public byte Command;        //故障位1
        public byte Value1;         //值1
        public byte Value2;        //值2
        public uint Value3;          //值3
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }

    #endregion

    public class SimTileLifterProcesser : ProcesserBase
    {
        #region[字段]
        DevTileLifter mDev;
        #endregion

        public SimTileLifterProcesser()
        {
            mDev = new DevTileLifter();
        }

        internal byte[] GetStatus(DevTileLifter dev)
        {
            TileStatusStruct st = new TileStatusStruct();

            st.Head = ShiftBytes(SimSocketConst.TILELIFTER_STATUS_HEAD_KEY);
            st.DeviceID = dev.DeviceID;
            st.LoadStatus1 = (byte)(dev.Load1 ? 0x01 : 0x00);
            st.LoadStatus2 = (byte)(dev.Load2 ? 0x01 : 0x00);
            st.NeedStatus1 = (byte)(dev.Need1 ? 0x01 : 0x00);
            st.NeedStatus2 = (byte)(dev.Need2 ? 0x01 : 0x00);
            st.FullQty = dev.FullQty;
            st.RecentQty = dev.RecentQty;
            st.Involve1 = (byte)(dev.Involve1 ? 0x01 : 0x00);
            st.Involve2 = (byte)(dev.Involve2 ? 0x01 : 0x00);
            st.OperateMode = (byte)dev.OperateMode;
            st.Goods1 = dev.Goods1;
            st.Goods2 = dev.Goods2;
            st.ShiftStatus = (byte)dev.ShiftStatus;
            st.ShiftAccept = (byte)(dev.ShiftAccept ? 0x01 : 0x00);
            st.WorkMode = (byte)dev.WorkMode;
            st.SetGoods = dev.SetGoods;
            st.SetLevel = dev.SetLevel;
            st.Tail = ShiftBytes(SimSocketConst.TAIL_KEY);

            return StructToBuffer(st) ;
        }

        internal DevTileCmd GetCmd(byte[] data)
        {
            TileCmdStruct st = BufferToStruct<TileCmdStruct>(data);
            DevTileCmd cmd = new DevTileCmd();
            cmd.DeviceID = st.DeviceID;
            cmd.Command = (DevLifterCmdTypeE)st.Command;
            cmd.Value1 = st.Value1;
            cmd.Value2 = st.Value2;
            cmd.Value3 = ShiftBytes(st.Value3);

            return cmd;
        }
    }
}
