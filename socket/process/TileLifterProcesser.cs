using enums;
using module.device;
using System.Runtime.InteropServices;

namespace socket.process
{
    #region[接收状态]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TileStatusStruct
    {
        public ushort Head;         //命令字头【0x91,0x01】
        public byte DeviceID;       //设备号
        public byte LoadStatus1;    //货物状态1 左
        public byte LoadStatus2;    //货物状态2 右
        public byte NeedStatus1;    //需求信号1 左
        public byte NeedStatus2;    //需求信号2 右
        public byte FullQty;        //满砖数量
        public byte Site1Qty;       //工位1数量
        public byte Involve1;       //介入状态1 左
        public byte Involve2;       //介入状态2 右
        public byte OperateMode;    //操作模式
        public uint Goods1;         //工位1品种
        public uint Goods2;         //工位2品种
        public byte ShiftStatus;    //转产状态
        public byte ShiftAccept;    //转产接收状态
        public byte WorkMode;       //作业模式
        public uint SetGoods;       //设定品种
        public byte SetLevel;       //设定等级
        public byte Site2Qty;       //工位2数量
        public byte NeedSytemShift; //砖机需转产信号
        public byte BackupShiftDev; //切换砖机设备号
        public byte AlertLightStatus;       //报警灯状态
        public byte Reserve2;       //预留2
        public byte Reserve3;       //预留3
        public byte Reserve4;       //预留4
        public byte Reserve5;       //预留5
        public ushort Tail;         //命令字尾【0xFF,0xFE】
    }

    #endregion

    #region[命令发送]

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TileCmdStruct
    {
        public ushort Head; //命令字头【0x90,0x01】
        public byte DeviceID;      //设备号
        public byte Command;   //控制码
        public byte Value1;         //值1
        public byte Value2;        //值2
        public uint Value3;          //值3
        public ushort Tail; //命令字尾【0xFF,0xFE】
    }

    #endregion

    public class TileLifterProcesser : ProcesserBase
    {
        #region[字段]
        DevTileLifter mDev;
        #endregion

        public TileLifterProcesser()
        {
            mDev = new DevTileLifter();
        }

        internal DevTileLifter GetStatus(byte[] data)
        {
            TileStatusStruct st = BufferToStruct<TileStatusStruct>(data);

            mDev.ReSetUpdate();
            mDev.DeviceID = st.DeviceID;
            //mDev.Load1 = st.LoadStatus1 == 1;
            //mDev.Load2 = st.LoadStatus2 == 1;
            mDev.LoadStatus1 = (DevLifterLoadE)st.LoadStatus1;
            mDev.LoadStatus2 = (DevLifterLoadE)st.LoadStatus2;
            mDev.Need1 = st.NeedStatus1 == 1;
            mDev.Need2 = st.NeedStatus2 == 1;
            mDev.FullQty = st.FullQty;
            mDev.Site1Qty = st.Site1Qty;
            mDev.Site2Qty = st.Site2Qty;
            mDev.Involve1 = st.Involve1 == 1;
            mDev.Involve2 = st.Involve2 == 1;
            mDev.OperateMode = (DevOperateModeE)st.OperateMode;
            mDev.Goods1 = ShiftBytes(st.Goods1);
            mDev.Goods2 = ShiftBytes(st.Goods2);
            mDev.ShiftStatus = (TileShiftStatusE)st.ShiftStatus;
            mDev.ShiftAccept = st.ShiftAccept == 1;
            mDev.WorkMode = (TileWorkModeE)st.WorkMode;
            mDev.SetGoods = ShiftBytes(st.SetGoods);
            mDev.SetLevel = st.SetLevel;
            mDev.NeedSytemShift = st.NeedSytemShift == 1;
            mDev.BackupShiftDev = st.BackupShiftDev;
            mDev.AlertLightStatus = st.AlertLightStatus;
            mDev.receivesetfull = st.Reserve2;
            mDev.reserve3 = st.Reserve3;
            mDev.reserve4 = st.Reserve4;
            mDev.reserve5 = st.Reserve5;
            return mDev;
        }

        internal byte[] GetCmd(string devid, DevLifterCmdTypeE type, byte value1, byte value2, uint value3)
        {
            TileCmdStruct cmd = new TileCmdStruct();
            cmd.Head = ShiftBytes(SocketConst.TILELIFTER_CMD_HEAD_KEY);
            cmd.DeviceID = byte.Parse(devid);
            cmd.Command = (byte)type;
            cmd.Value1 = value1;
            cmd.Value2 = value2;
            cmd.Value3 = ShiftBytes(value3);
            cmd.Tail = ShiftBytes(SocketConst.TAIL_KEY);

            return StructToBuffer(cmd);
        }
    }
}
