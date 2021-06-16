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
            st.LoadStatus1 = (byte)dev.LoadStatus1;
            st.LoadStatus2 = (byte)dev.LoadStatus2;
            st.NeedStatus1 = (byte)(dev.Need1 ? 0x01 : 0x00);
            st.NeedStatus2 = (byte)(dev.Need2 ? 0x01 : 0x00);
            st.FullQty = dev.FullQty;
            st.Site1Qty = dev.Site1Qty;
            st.Site2Qty = dev.Site2Qty;
            st.Involve1 = (byte)(dev.Involve1 ? 0x01 : 0x00);
            st.Involve2 = (byte)(dev.Involve2 ? 0x01 : 0x00);
            st.OperateMode = (byte)dev.OperateMode;
            st.Goods1 = ShiftBytes(dev.Goods1);
            st.Goods2 = ShiftBytes(dev.Goods2);
            st.ShiftStatus = (byte)dev.ShiftStatus;
            st.ShiftAccept = (byte)(dev.ShiftAccept ? 0x01 : 0x00);
            st.WorkMode = (byte)dev.WorkMode;
            st.SetGoods = ShiftBytes(dev.SetGoods);
            st.SetLevel = dev.SetLevel;
            st.NeedSytemShift = (byte)(dev.NeedSytemShift ? 0x01 : 0x00);
            st.BackupShiftDev = dev.BackupShiftDev;
            st.AlertLightStatus = dev.alertlightstatus;
            st.Reserve2 = dev.reserve2;
            st.Reserve3 = dev.reserve3;
            st.Reserve4 = dev.reserve4;
            st.Reserve5 = dev.reserve5;

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
