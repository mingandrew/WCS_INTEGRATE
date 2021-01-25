using enums;

namespace module.device
{
    public class DevCarrier : IDevice
    {

        public DevCarrier()
        {

        }

        #region[字段]
        private byte deviceid;       //设备号
        private byte devicestatus;   //设备状态
        private ushort currentpoint;  //当前RFID
        private ushort currentsite;  //当前坐标
        private ushort targetpoint;  //目的RFID
        private ushort targetsite;  //目的坐标
        private byte currentorder;    //当前指令
        private byte finishorder;     //完成指令
        private byte loadstatus;     //载货状态
        private byte position;       //所在位置
        private byte operatemode;    //操作模式
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
        private byte reserve4;        //预留4
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
            set => Set(ref devicestatus, (byte)value);
            get => (DevCarrierStatusE)devicestatus;
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


        public DevCarrierTaskE CurrentTask
        {
            set => Set(ref currentorder, (byte)value);
            get => (DevCarrierTaskE)currentorder;
        }
        public DevCarrierTaskE FinishTask
        {
            set => Set(ref finishorder, (byte)value);
            get => (DevCarrierTaskE)finishorder;
        }


        /// <summary>
        /// 当前指令
        /// </summary>
        public DevCarrierOrderE CurrentOrder
        {
            set => Set(ref currentorder, (byte)value);
            get => (DevCarrierOrderE)currentorder;
        }

        /// <summary>
        /// 目的指令
        /// </summary>
        public DevCarrierOrderE FinishOrder
        {
            set => Set(ref finishorder, (byte)value);
            get => (DevCarrierOrderE)finishorder;
        }

        /// <summary>
        /// 载货状态
        /// </summary>
        public DevCarrierLoadE LoadStatus
        {
            set => Set(ref loadstatus, (byte)value);
            get => (DevCarrierLoadE)loadstatus;
        }

        /// <summary>
        /// 所在位置
        /// </summary>
        public DevCarrierPositionE Position
        {
            set => Set(ref position, (byte)value);
            get => (DevCarrierPositionE) position;
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public DevOperateModeE OperateMode
        {
            set => Set(ref operatemode, (byte)value);
            get => (DevOperateModeE)operatemode;
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
            set => SetAlert(ref aler1, value);
            get => aler1;
        }

        public byte Aler2//报警2
        {
            set => SetAlert(ref aler2, value);
            get => aler2;
        }

        public byte Aler3//报警3
        {
            set => SetAlert(ref aler3, value);
            get => aler3;
        }

        public byte Aler4//报警4
        {
            set => SetAlert(ref aler4, value);
            get => aler4;
        }

        public byte Aler5//报警5
        {
            set => SetAlert(ref aler5, value);
            get => aler5;
        }

        public byte Aler6//报警6
        {
            set => SetAlert(ref aler6, value);
            get => aler6;
        }

        public byte Aler7//报警7
        {
            set => SetAlert(ref aler7, value);
            get => aler7;
        }

        public byte Aler8//报警8
        {
            set => SetAlert(ref aler8, value);
            get => aler8;
        }

        public byte Aler9//报警9
        {
            set => SetAlert(ref aler9, value);
            get => aler9;
        }

        public byte Aler10//报警10
        {
            set => SetAlert(ref aler10, value);
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
        /// 预留4
        /// </summary>
        public byte Reserve4
        {
            set => Set(ref reserve4, value);
            get => reserve4;
        }

        #endregion

        #region[日志]

        public override string ToString()
        {
            return string.Format("设备[ {0} ]，状态[ {1} ]，当前[ {2}^{3} ]，目的[ {4}^{5} ]，指令[ {6} ]，完成[ {7} ]，" +
                "载货[ {8} ]，位置[ {9} ]，操作[ {10} ]，取货[ {11}^{12} ]，卸货[ {13}^{14} ]，倒库[ {15} ]",
                DeviceID, DeviceStatus, CurrentPoint, CurrentSite, TargetPoint, TargetSite, CurrentOrder, FinishOrder,
                LoadStatus, Position, OperateMode, TakePoint, TakeSite, GivePoint, GiveSite, MoveCount);
        }

        public string AlertToString()
        {
            return string.Format("警1：{0}, 警2：{1}, 警3：{2}, 警4：{3}, 警5：{4}, 警6：{5}, 警7：{6}, 警8：{7}, 警9：{8}, 警10：{9}, " +
                "预1：{10}, 预2：{11}, 预3：{12}, 预4：{13}",
                Aler1, Aler2, Aler3, Aler4, Aler5, Aler6, Aler7, Aler8, Aler9, Aler10, Reserve1, Reserve2, Reserve3, Reserve4);
        }
        #endregion
    }
}
