using enums;
using GalaSoft.MvvmLight;
using module.device;
using System;

namespace wcs.Data.View
{
    public class CarrierView : ViewModelBase
    {
        public uint ID { set; get; }
        public string Name { set; get; }
        #region[逻辑字段]
        private SocketConnectStatusE connstatus;
        private bool isconnect;
        #endregion
        public bool IsConnect
        {
            get => isconnect;
            set => Set(ref isconnect, value);
        }
        public SocketConnectStatusE ConnStatus
        {
            get => connstatus;
            set => Set(ref connstatus, value);
        }

        #region[字段]
        private byte deviceid;       //设备号
        private DevCarrierStatusE devicestatus;   //设备状态
        private ushort currentsite;  //当前值
        private DevCarrierTaskE currenttask;    //当前任务
        private DevCarrierTaskE finishtask;     //完成任务
        private DevCarrierSizeE currentoversize;//超限
        //private byte finishoversize; //超限
        private DevCarrierLoadE loadstatus;     //载货状态
        private DevCarrierWorkModeE workmode;       //系统模式
        private DevOperateModeE operatemode;    //操作模式
        private ushort actiontime;     //取放时间
        private ushort taketrackcode;//取货轨道号
        private ushort givetrackcode;//取货轨道号
        private DevCarrierSignalE actiontype;     // 空满砖信息
        private byte aler1;          //报警1
        private byte aler2;          //报警2
        private byte aler3;          //报警3
        private byte aler4;          //报警4
        private byte aler5;          //报警5
        private byte aler6;          //报警6
        private byte aler7;          //报警7
        private byte aler8;          //报警8
        private byte aler9;          //报警9
        private byte aler10;          //报警10
        private byte reserve1;        //预留1
        private byte reserve2;        //预留2
        #endregion


        #region[更新属性]

        public byte DeviceID
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        public DevCarrierStatusE DeviceStatus
        {
            set => Set(ref devicestatus, value);
            get => devicestatus;
        }

        public ushort CurrentSite//当前站点
        {
            set => Set(ref currentsite, value);
            get => currentsite;
        }

        public DevCarrierTaskE CurrentTask//当前任务   
        {
            set => Set(ref currenttask, value);
            get => currenttask;
        }

        public DevCarrierSizeE CurrentOverSize//超限
        {
            set => Set(ref currentoversize, value);
            get => currentoversize;
        }

        public DevCarrierTaskE FinishTask//完成任务
        {
            set => Set(ref finishtask, value);
            get => finishtask;
        }

        //public DevCarrierSizeE FinishOverSize//超限
        //{
        //    set => Set(ref finishoversize, (byte)value);
        //    get => (DevCarrierSizeE)finishoversize;
        //}

        public DevCarrierLoadE LoadStatus//载货状态
        {
            set => Set(ref loadstatus, value);
            get => loadstatus;
        }

        public DevCarrierWorkModeE WorkMode//系统模式
        {
            set => Set(ref workmode, value);
            get => workmode;
        }

        public DevOperateModeE OperateMode//操作模式
        {
            set => Set(ref operatemode, value);
            get => operatemode;
        }
        public ushort ActionTime//取放时间
        {
            set => Set(ref actiontime, value);
            get => actiontime;
        }
        public ushort TakeTrackCode//取货轨道号
        {
            set => Set(ref taketrackcode, value);
            get => taketrackcode;
        }
        public ushort GiveTrackCode//卸货轨道号
        {
            set => Set(ref givetrackcode, value);
            get => givetrackcode;
        }
        public DevCarrierSignalE ActionType// 空满砖信息
        {
            set => Set(ref actiontype, value);
            get => actiontype;
        }
        public byte Aler1//报警1
        {
            set => Set(ref aler1, value);
            get => aler1;
        }
        public byte Aler2//报警2
        {
            set => Set(ref aler2, value);
            get => aler2;
        }
        public byte Aler3//报警3
        {
            set => Set(ref aler3, value);
            get => aler3;
        }
        public byte Aler4//报警4
        {
            set => Set(ref aler4, value);
            get => aler4;
        }
        public byte Aler5//报警5
        {
            set => Set(ref aler5, value);
            get => aler5;
        }
        public byte Aler6//报警6
        {
            set => Set(ref aler6, value);
            get => aler6;
        }
        public byte Aler7//报警7
        {
            set => Set(ref aler7, value);
            get => aler7;
        }
        public byte Aler8//报警8
        {
            set => Set(ref aler8, value);
            get => aler8;
        }
        public byte Aler9//报警9
        {
            set => Set(ref aler9, value);
            get => aler9;
        }
        public byte Aler10//报警10
        {
            set => Set(ref aler10, value);
            get => aler10;
        }
        public byte Reserve1//预留1
        {
            set => Set(ref reserve1, value);
            get => reserve1;
        }
        public byte Reserve2//预留2
        {
            set => Set(ref reserve2, value);
            get => reserve2;
        }

        #endregion

        internal void Update(DevCarrier st, SocketConnectStatusE conn)
        {
            DeviceID = st.DeviceID;
            DeviceStatus = st.DeviceStatus;
            CurrentSite = st.CurrentSite;
            CurrentTask =st.CurrentTask;
            CurrentOverSize = st.CurrentOverSize;
            FinishTask = st.FinishTask;
            //FinishOverSize = (DevCarrierSizeE)st.FinishOverSize;
            LoadStatus = st.LoadStatus;
            WorkMode = st.WorkMode;
            OperateMode = st.OperateMode;
            ActionTime = st.ActionTime;
            TakeTrackCode = st.TakeTrackCode;
            GiveTrackCode = st.GiveTrackCode;
            ActionType = st.ActionType;
            Aler1 = st.Aler1;
            Aler2 = st.Aler2;
            Aler3 = st.Aler3;
            Aler4 = st.Aler4;
            Aler5 = st.Aler5;
            Aler6 = st.Aler6;
            Aler7 = st.Aler7;
            Aler8 = st.Aler8;
            Aler9 = st.Aler9;
            Aler10 = st.Aler10;
            Reserve1 = st.Reserve1;
            Reserve2 = st.Reserve2;
            ConnStatus = conn;
            IsConnect = ConnStatus == SocketConnectStatusE.通信正常;
        }
    }
}
