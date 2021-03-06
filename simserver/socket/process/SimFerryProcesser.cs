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
