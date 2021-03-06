using enums;
using module.device;
using socket.process;

namespace simserver.simsocket.process
{

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
