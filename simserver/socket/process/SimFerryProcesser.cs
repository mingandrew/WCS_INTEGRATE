using enums;
using module.device;
using socket.process;

namespace simserver.simsocket.process
{

    public class SimFerryProcesser : ProcesserBase
    {
        #region[字段]

        #endregion

        public SimFerryProcesser()
        {
        }

        internal byte[] GetStatus(DevFerry dev)
        {
            FerryStatusStruct st = new FerryStatusStruct
            {
                Head = ShiftBytes(SimSocketConst.FEERY_STATUS_HEAD_KEY),
                DeviceID = dev.DeviceID,
                DeviceStatus = (byte)dev.DeviceStatus,
                TargetSite = ShiftBytes(dev.TargetSite),
                CurrentTask = (byte)dev.CurrentTask,
                DownSite = ShiftBytes(dev.DownSite),
                UpSite = ShiftBytes(dev.UpSite),
                FinishTask = (byte)dev.FinishTask,
                LoadStatus = (byte)dev.LoadStatus,
                WorkMode = (byte)dev.WorkMode,
                DownLight = (byte)(dev.DownLight ? 1 : 0),
                UpLight = (byte)(dev.UpLight ? 1 : 0),
                Reserve = dev.Reserve,
                MarkCode = dev.MarkCode,
                Tail = ShiftBytes(SimSocketConst.TAIL_KEY)
            };

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
            FerryCmd cmd = new FerryCmd
            {
                DeviceID = st.DeviceID,
                Commond = (DevFerryCmdE)st.Commond,
                Value1 = st.Value1,
                Value2 = st.Value2,
                Value3_6 = ShiftBytes(st.Value3),
                Value7 = st.Value7
            };
            return cmd;
        }
    }
}
