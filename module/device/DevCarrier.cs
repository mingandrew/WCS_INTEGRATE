using enums;

namespace module.device
{
    public class DevCarrier : IDevice
    {

        public DevCarrier()
        {

        }

        public bool IsSiteChange { set; get; }
        #region[字段]
        private byte deviceid;       //设备号
        private byte devicestatus;   //设备状态
        private ushort currentsite;  //当前值
        private byte currenttask;    //当前任务
        private byte finishtask;     //完成任务
        private byte currentoversize;//超限
        //private byte finishoversize; //超限
        private byte loadstatus;     //载货状态
        private byte carrierposition;       //运输车位置
        private byte operatemode;    //操作模式
        private ushort actiontime;     //取放时间
        private ushort taketrackcode;//取货轨道号
        private ushort givetrackcode;//取货轨道号
        private byte actiontype;     // 空满砖信息
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
            set => Set(ref devicestatus, (byte)value);
            get => (DevCarrierStatusE)devicestatus;
        }

        public ushort CurrentSite//当前站点
        {
            set
            {
                IsSiteChange = false;
                if (Set(ref currentsite, value)) 
                {
                    IsSiteChange = true;
                }
            }
            get => currentsite;
        }

        public DevCarrierTaskE CurrentTask//当前任务   
        {
            set => Set(ref currenttask, (byte)value);
            get => (DevCarrierTaskE)currenttask;
        }

        public DevCarrierSizeE CurrentOverSize//超限
        {
            set => Set(ref currentoversize, (byte)value);
            get => (DevCarrierSizeE)currentoversize;
        }

        public DevCarrierTaskE FinishTask//完成任务
        {
            set => Set(ref finishtask, (byte)value);
            get => (DevCarrierTaskE)finishtask;
        }

        //public DevCarrierSizeE FinishOverSize//超限
        //{
        //    set => Set(ref finishoversize, (byte)value);
        //    get => (DevCarrierSizeE)finishoversize;
        //}

        public DevCarrierLoadE LoadStatus//载货状态
        {
            set => Set(ref loadstatus, (byte)value);
            get => (DevCarrierLoadE)loadstatus;
        }

        public DevCarrierPositionE CarrierPosition//系统模式
        {
            set => Set(ref carrierposition, (byte)value);
            get => (DevCarrierPositionE)carrierposition;
        }

        public DevOperateModeE OperateMode//操作模式
        {
            set => Set(ref operatemode, (byte)value);
            get => (DevOperateModeE)operatemode;
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
            set => Set(ref actiontype, (byte)value);
            get => (DevCarrierSignalE)actiontype;
        }
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
        public byte Reserve1//预留1
        {
            set => SetAlert(ref reserve1, value);
            get => reserve1;
        }
        public byte Reserve2//预留2
        {
            set => SetAlert(ref reserve2, value);
            get => reserve2;
        }

        #endregion

        #region[日志]

        public override string ToString()
        {
            return string.Format("设备：{0} ,状态：{1}, 站点：{2}, 任务：{3}, 完成：{4}, 超限：{5}," +
                " 载货：{6}, 作业：{7}, 操作：{8}, 时间：{9}, 取货点：{10}, 卸货点：{11}, 空满砖：{12} ",
                DeviceID, DeviceStatus, CurrentSite, CurrentTask, FinishTask, CurrentOverSize,
                LoadStatus, CarrierPosition, OperateMode, ActionTime, TakeTrackCode, GiveTrackCode, ActionType);
        }

        public string AlertToString()
        {
            return string.Format("警1：{0}, 警2：{1}, 警3：{2}, 警4：{3}, 警5：{4}, 警6：{5}, 警7：{6}, 警8：{7}, 警9：{8}, 警10：{9}, 预1：{10}, 预2：{11}",
                Aler1, Aler2, Aler3, Aler4, Aler5, Aler6, Aler7, Aler8, Aler9, Aler10, Reserve1, Reserve2);
        }
        #endregion
    }
}
