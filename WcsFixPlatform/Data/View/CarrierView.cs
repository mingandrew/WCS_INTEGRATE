using enums;
using GalaSoft.MvvmLight;
using module.device;

namespace wcs.Data.View
{
    public class CarrierView : ViewModelBase
    {
        public uint ID { set; get; }
        public uint AreaId { set; get; }
        public ushort LineId { set; get; }
        public string Name { set; get; }

        private bool working;
        private uint currenttrackId;
        private uint targettrackId;

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

        public bool Working
        {
            get => working;
            set => Set(ref working, value);
        }

        /// <summary>
        /// 当前轨道ID
        /// </summary>
        public uint CurrentTrackId
        {
            get => currenttrackId;
            set => Set(ref currenttrackId, value);
        }

        /// <summary>
        /// 目标轨道ID
        /// </summary>
        public uint TargetTrackId
        {
            get => targettrackId;
            set => Set(ref targettrackId, value);
        }

        #region[字段]
        private byte deviceid;       //设备号
        private DevCarrierStatusE devicestatus;   //设备状态
        private ushort currentpoint;  //当前RFID
        private ushort currentsite;  //当前坐标
        private ushort targetpoint;  //目的RFID
        private ushort targetsite;  //目的坐标
        private DevCarrierOrderE currentorder;    //当前指令
        private DevCarrierOrderE finishorder;     //完成指令
        private DevCarrierLoadE loadstatus;     //载货状态
        private DevCarrierPositionE position;       //所在位置
        private DevOperateModeE operatemode;    //操作模式
        private ushort takepoint;  //取货RFID
        private ushort takesite;  //取货坐标
        private ushort givepoint;  //卸货RFID
        private ushort givesite;  //卸货坐标
        private byte movecount;  //倒库数量
        private byte reserve1;        //预留1
        private byte reserve2;        //预留2
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
        private byte reserve3;        //预留3
        private byte markcode;        //标识码（PLC发送的码，需要PC进行控制码0x88回复）
        #endregion

        #region[更新属性]

        /// <summary>
        /// 设备号
        /// </summary>
        public byte DeviceID
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        /// <summary>
        /// 设备状态
        /// </summary>
        public DevCarrierStatusE DeviceStatus
        {
            set => Set(ref devicestatus, value);
            get => devicestatus;
        }

        /// <summary>
        /// 当前RFID
        /// </summary>
        public ushort CurrentPoint
        {
            set => Set(ref currentpoint, value);
            get => currentpoint;
        }

        /// <summary>
        /// 当前坐标
        /// </summary>
        public ushort CurrentSite
        {
            set => Set(ref currentsite, value);
            get => currentsite;
        }

        /// <summary>
        /// 目的RFID
        /// </summary>
        public ushort TargetPoint
        {
            set => Set(ref targetpoint, value);
            get => targetpoint;
        }

        /// <summary>
        /// 目的坐标
        /// </summary>
        public ushort TargetSite
        {
            set => Set(ref targetsite, value);
            get => targetsite;
        }

        /// <summary>
        /// 当前指令
        /// </summary>
        public DevCarrierOrderE CurrentOrder
        {
            set => Set(ref currentorder, value);
            get => currentorder;
        }

        /// <summary>
        /// 目的指令
        /// </summary>
        public DevCarrierOrderE FinishOrder
        {
            set => Set(ref finishorder, value);
            get => finishorder;
        }

        /// <summary>
        /// 载货状态
        /// </summary>
        public DevCarrierLoadE LoadStatus
        {
            set => Set(ref loadstatus, value);
            get => loadstatus;
        }

        /// <summary>
        /// 所在位置
        /// </summary>
        public DevCarrierPositionE Position
        {
            set => Set(ref position, value);
            get => position;
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public DevOperateModeE OperateMode
        {
            set => Set(ref operatemode, value);
            get => operatemode;
        }

        /// <summary>
        /// 取货RFID
        /// </summary>
        public ushort TakePoint
        {
            set => Set(ref takepoint, value);
            get => takepoint;
        }

        /// <summary>
        /// 取货坐标
        /// </summary>
        public ushort TakeSite
        {
            set => Set(ref takesite, value);
            get => takesite;
        }

        /// <summary>
        /// 卸货RFID
        /// </summary>
        public ushort GivePoint
        {
            set => Set(ref givepoint, value);
            get => givepoint;
        }

        /// <summary>
        /// 卸货坐标
        /// </summary>
        public ushort GiveSite
        {
            set => Set(ref givesite, value);
            get => givesite;
        }

        /// <summary>
        /// 倒库数量
        /// </summary>
        public byte MoveCount
        {
            set => Set(ref movecount, value);
            get => movecount;
        }

        #region 报警

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

        #endregion

        /// <summary>
        /// 预留1
        /// </summary>
        public byte Reserve1
        {
            set => Set(ref reserve1, value);
            get => reserve1;
        }

        /// <summary>
        /// 预留2
        /// </summary>
        public byte Reserve2
        {
            set => Set(ref reserve2, value);
            get => reserve2;
        }

        /// <summary>
        /// 预留3
        /// </summary>
        public byte Reserve3
        {
            set => Set(ref reserve3, value);
            get => reserve3;
        }

        /// <summary>
        /// 标识码（PLC发送的码，需要PC进行控制码0x88回复）
        /// </summary>
        public byte MarkCode
        {
            set => Set(ref markcode, value);
            get => markcode;
        }

        #endregion

        internal void Update(DevCarrier st, SocketConnectStatusE conn, bool working, uint currenttrackId,uint targettrackId)
        {
            DeviceID = st.DeviceID;
            DeviceStatus = st.DeviceStatus;
            CurrentPoint = st.CurrentSite;
            CurrentSite = st.CurrentPoint;
            TargetPoint = st.TargetSite;
            TargetSite = st.TargetPoint;
            CurrentOrder = st.CurrentOrder;
            FinishOrder = st.FinishOrder;
            LoadStatus = st.LoadStatus;
            Position = st.Position;
            OperateMode = st.OperateMode;
            TakePoint = st.TakeSite;
            TakeSite = st.TakePoint;
            GivePoint = st.GiveSite;
            GiveSite = st.GivePoint;
            MoveCount = st.MoveCount;
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
            Reserve3 = st.Reserve3;
            MarkCode = st.MarkCode;
            ConnStatus = conn;
            IsConnect = ConnStatus == SocketConnectStatusE.通信正常;
            Working = working;
            CurrentTrackId = currenttrackId;
            TargetTrackId = targettrackId;
        }

        internal bool UpdateLine(ushort currenttrackline)
        {
            if (currenttrackline != 0 && LineId != currenttrackline)
            {
                LineId = currenttrackline;
                return true;
            }

            return false;
        }
    }
}
