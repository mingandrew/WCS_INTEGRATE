using enums;

namespace module.device
{
    public class DevFerry : IDevice
    {
        public bool IsUpSiteChange { set; get; }
        public bool IsDownSiteChange { set; get; }
        #region[字段]
        private byte deviceid;      //设备号
        private byte devicestatus;  //设备状态
        private ushort targetsite;  //目标值
        private byte currenttask;   //当前任务
        private ushort upsite; //当前左值
        private ushort downsite; //当前右值
        private byte finishtask;    //完成任务
        private byte loadstatus;    //载货状态
        private byte workmode;      //作业模式
        private bool downlight;       //上砖侧光电
        private bool uplight;     //下砖侧光电
        private byte reserve;       //报警码
        private byte markcode;       //标识码（PLC发送的码，需要PC进行控制码0x88回复）
        #endregion

        #region[属性]

        public byte DeviceID      //设备号   
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        /// <summary>
        /// 设备状态
        /// </summary>
        public DevFerryStatusE DeviceStatus
        {
            set => Set(ref devicestatus, (byte)value);
            get => (DevFerryStatusE)devicestatus;
        }

        /// <summary>
        /// 目标轨道编号
        /// </summary>
        public ushort TargetSite  //目标值   
        {
            set => Set(ref targetsite, value);
            get => targetsite;
        }

        /// <summary>
        /// 当前指令
        /// </summary>
        public DevFerryTaskE CurrentTask
        {
            set => Set(ref currenttask, (byte)value);
            get => (DevFerryTaskE)currenttask;
        }

        /// <summary>
        /// 当前轨道编号（前侧）
        /// </summary>
        public ushort UpSite //当前值   
        {
            set
            {
                IsUpSiteChange = false;
                if (Set(ref upsite, value))
                {
                    IsUpSiteChange = true;
                }
            }

            get => upsite;
        }

        /// <summary>
        /// 当前轨道编号（后侧）
        /// </summary>
        public ushort DownSite //当前值   
        {
            set
            {
                IsDownSiteChange = false;
                if (Set(ref downsite, value))
                {
                    IsDownSiteChange = true;
                }
            }
            get => downsite;
        }

        /// <summary>
        /// 完成指令
        /// </summary>
        public DevFerryTaskE FinishTask
        {
            set => Set(ref finishtask, (byte)value);
            get => (DevFerryTaskE)finishtask;
        }

        /// <summary>
        /// 载货状态   
        /// </summary>
        public DevFerryLoadE LoadStatus
        {
            set => Set(ref loadstatus, (byte)value);
            get => (DevFerryLoadE)loadstatus;
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public DevOperateModeE WorkMode
        {
            set => Set(ref workmode, (byte)value);
            get => (DevOperateModeE)workmode;
        }

        /// <summary>
        /// 后侧到位信号
        /// </summary>
        public bool DownLight
        {
            set => Set(ref downlight, value);
            get => downlight;
        }

        /// <summary>
        /// 前侧到位信号
        /// </summary>
        public bool UpLight
        {
            set => Set(ref uplight, value);
            get => uplight;
        }

        /// <summary>
        /// 报警码
        /// </summary>
        public byte Reserve
        {
            set => Set(ref reserve, value);
            get => reserve;
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

        #region[日志]

        public override string ToString()
        {
            return string.Format("状态[ {0} ], 目标[ {1} ], 任务[ {2} ], 完成[ {3} ], 前标[ {4} ], 后标[ {5} ], 货物[ {6} ], 模式[ {7} ], 前到位[ {8} ], 后到位[ {9} ], 报警[ {10} ], 标识码[ {11} ]",
                DeviceStatus, TargetSite, CurrentTask, FinishTask, UpSite, DownSite, LoadStatus, WorkMode, S(UpLight), S(DownLight), Reserve, MarkCode);
        }
        private string S(bool v)
        {
            return v ? "✔" : "❌";
        }

        #endregion
    }

    public class DevFerrySite : IDevice
    {
        #region[字段]
        private byte deviceid;//设备号
        private ushort trackcode;//轨道号
        private int tracksite;//已设坐标值
        private int nowtrackpos;//当前坐标值
        private byte reserve;//预留
        #endregion

        #region[属性]
        public byte DeviceID//设备号        
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        public ushort TrackCode//轨道号
        {
            set => Set(ref trackcode, value);
            get => trackcode;
        }

        public int TrackPos//已设坐标值
        {
            set => Set(ref tracksite, value);
            get => tracksite;
        }

        public int NowTrackPos//当前坐标值
        {
            set => Set(ref nowtrackpos, value);
            get => nowtrackpos;
        }

        public byte Reserve//预留
        {
            set => Set(ref reserve, value);
            get => reserve;
        }
        #endregion

        #region[日志]

        public override string ToString()
        {
            return string.Format("轨道号[ {0} ], 轨道设值[ {1} ], 当前坐标[ {2} ], 预留[ {3} ]", TrackCode, TrackPos, NowTrackPos, Reserve);
        }
        #endregion
    }


}
