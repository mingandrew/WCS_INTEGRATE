using enums;
using System;

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
        private ushort currentsite;  //当前RFID
        private ushort currentpoint;  //当前坐标
        private ushort campare_currentpoint = 0;//用于计算的当前坐标 避免频繁刷新
        private ushort targetsite;  //目的RFID
        private ushort targetpoint;  //目的坐标
        private byte currentorder;    //当前指令
        private byte finishorder;     //完成指令
        private byte loadstatus;     //载货状态
        private byte position;       //所在位置
        private byte operatemode;    //操作模式
        private ushort takesite;  //取货RFID
        private ushort takepoint;  //取货坐标
        private ushort givesite;  //卸货RFID
        private ushort givepoint;  //卸货坐标
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
        private uint currenttrackid;
        private uint targettrackid;
        #endregion

        #region[更新属性]
        public bool IsCurrentSiteUpdate { set; get; }
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
        public ushort CurrentSite
        {
            set => Set(ref currentsite, value);
            get => currentsite;
        }

        /// <summary>
        /// 当前坐标
        /// </summary>
        public ushort CurrentPoint
        {
            set
            {
                if(Math.Abs(campare_currentpoint - value) >= 50)
                {
                    campare_currentpoint = value;
                    IsCurrentSiteUpdate = true;
                }
                currentpoint = value;
            }
            get => currentpoint;
        }

        /// <summary>
        /// 目的RFID
        /// </summary>
        public ushort TargetSite
        {
            set => Set(ref targetsite, value);
            get => targetsite;
        }

        /// <summary>
        /// 目的坐标
        /// </summary>
        public ushort TargetPoint
        {
            set => Set(ref targetpoint, value);
            get => targetpoint;
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
        public ushort TakeSite
        {
            set => Set(ref takesite, value);
            get => takesite;
        }

        /// <summary>
        /// 取货坐标
        /// </summary>
        public ushort TakePoint
        {
            set => Set(ref takepoint, value);
            get => takepoint;
        }

        /// <summary>
        /// 卸货RFID
        /// </summary>
        public ushort GiveSite
        {
            set => Set(ref givesite, value);
            get => givesite;
        }

        /// <summary>
        /// 卸货坐标
        /// </summary>
        public ushort GivePoint
        {
            set => Set(ref givepoint, value);
            get => givepoint;
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

        public uint CurrentTrackId 
        {
            set => Set(ref currenttrackid, value);
            get => currenttrackid;
        }

        public uint TargetTrackId
        {
            set => Set(ref targettrackid, value);
            get => targettrackid;
        }

        #endregion

        public override void ReSetUpdate()
        {
            base.ReSetUpdate();
            IsCurrentSiteUpdate = false;
        }

        #region[日志]

        public override string ToString()
        {
            return string.Format("状态[ {0} ], 当前[ {1}^{2} ], 目的[ {3}^{4} ], 指令[ {5} ], 完成[ {6} ], " +
                "载货[ {7} ], 位置[ {8} ], 操作[ {9} ], 取货[ {10}^{11} ], 卸货[ {12}^{13} ], 倒库[ {14} ]",
                DeviceStatus, CurrentSite, CurrentPoint, TargetSite, TargetPoint, CurrentOrder, FinishOrder,
                LoadStatus, Position, OperateMode, TakeSite, TakePoint, GiveSite, GivePoint, MoveCount);
        }

        /// <summary>
        /// 获取取货日志信息
        /// </summary>
        /// <returns></returns>
        public string GetTakeString()
        {
            return string.Format("当前[ {0}^{1} ], 指令[ {2} ], 位置[ {3} ], 操作[ {4} ], 取货[ {5}^{6} ]",
                  CurrentSite, CurrentPoint, CurrentOrder,Position, OperateMode, TakeSite, TakePoint);
        }

        /// <summary>
        /// 获取卸货日志信息
        /// </summary>
        /// <returns></returns>
        public string GetGiveString()
        {
            return string.Format("当前[ {0}^{1} ], 指令[ {2} ], 位置[ {3} ], 操作[ {4} ], 卸货[ {5}^{6} ], 倒库[ {7} ]",
                  CurrentSite, CurrentPoint, CurrentOrder,Position, OperateMode, GiveSite, GivePoint, MoveCount);
        }

        public string AlertToString()
        {
            return string.Format("一[ {0} ], 二[ {1} ], 三[ {2} ], 四[ {3} ], 五[ {4} ], 六[ {5} ], 七[ {6} ], 八[ {7} ], 九[ {8} ], 十[ {9} ]," +
                "预1[ {10} ],预2[ {11} ],预3[ {12} ],预4[ {13}",
                Aler1, Aler2, Aler3, Aler4, Aler5, Aler6, Aler7, Aler8, Aler9, Aler10, Reserve1, Reserve2, Reserve3, Reserve4);
        }
        #endregion
    }
}
