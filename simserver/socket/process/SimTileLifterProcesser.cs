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
            TileStatusStruct st = new TileStatusStruct
            {
                Head = ShiftBytes(SimSocketConst.TILELIFTER_STATUS_HEAD_KEY),
                DeviceID = dev.DeviceID,
                LoadStatus1 = (byte)dev.LoadStatus1,
                LoadStatus2 = (byte)dev.LoadStatus2,
                NeedStatus1 = (byte)(dev.Need1 ? 0x01 : 0x00),
                NeedStatus2 = (byte)(dev.Need2 ? 0x01 : 0x00),
                FullQty = dev.FullQty,
                Site1Qty = dev.Site1Qty,
                Site2Qty = dev.Site2Qty,
                Involve1 = (byte)(dev.Involve1 ? 0x01 : 0x00),
                Involve2 = (byte)(dev.Involve2 ? 0x01 : 0x00),
                OperateMode = (byte)dev.OperateMode,
                Goods1 = ShiftBytes(dev.Goods1),
                Goods2 = ShiftBytes(dev.Goods2),
                ShiftStatus = (byte)dev.ShiftStatus,
                ShiftAccept = (byte)(dev.ShiftAccept ? 0x01 : 0x00),
                WorkMode = (byte)dev.WorkMode,
                SetGoods = ShiftBytes(dev.SetGoods),
                SetLevel = dev.SetLevel,
                NeedSytemShift = (byte)(dev.NeedSytemShift ? 0x01 : 0x00),
                BackupShiftDev = dev.BackupShiftDev,
                AlertLightStatus = dev.AlertLightStatus,
                Reserve2 = dev.ReceiveSetFull,
                Reserve3 = dev.Reserve3,
                Reserve4 = dev.Reserve4,
                MarkCode = dev.MarkCode,
                Tail = ShiftBytes(SimSocketConst.TAIL_KEY)
            };

            return StructToBuffer(st) ;
        }

        internal DevTileCmd GetCmd(byte[] data)
        {
            TileCmdStruct st = BufferToStruct<TileCmdStruct>(data);
            DevTileCmd cmd = new DevTileCmd
            {
                DeviceID = st.DeviceID,
                Command = (DevLifterCmdTypeE)st.Command,
                Value1 = st.Value1,
                Value2 = st.Value2,
                Value3_6 = ShiftBytes(st.Value3),
                Value7 = st.Value7
            };

            return cmd;
        }
    }
}
