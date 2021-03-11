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
        private byte reserve;       //预留
        #endregion

        #region[属性]

        public byte DeviceID      //设备号   
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }
        public DevFerryStatusE DeviceStatus  //设备状态   
        {
            set => Set(ref devicestatus, (byte)value);
            get => (DevFerryStatusE)devicestatus;
        }
        public ushort TargetSite  //目标值   
        {
            set => Set(ref targetsite, value);
            get => targetsite;
        }
        public DevFerryTaskE CurrentTask   //当前任务   
        {
            set => Set(ref currenttask, (byte)value);
            get => (DevFerryTaskE)currenttask;
        }

        public ushort UpSite //当前值   
        {
            set 
            {
                IsUpSiteChange = false;
                if(Set(ref upsite, value))
                {
                    IsUpSiteChange = true;
                }
            }

            get => upsite;
        }
        
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

        public DevFerryTaskE FinishTask    //完成任务   
        {
            set => Set(ref finishtask, (byte)value);
            get => (DevFerryTaskE)finishtask;
        }
        public DevFerryLoadE LoadStatus    //载货状态   
        {
            set => Set(ref loadstatus, (byte)value);
            get => (DevFerryLoadE)loadstatus;
        }

        public DevOperateModeE WorkMode      //作业模式   
        {
            set => Set(ref workmode, (byte)value);
            get => (DevOperateModeE)workmode;
        }

        public bool DownLight       //下砖侧光电   
        {
            set => Set(ref downlight, value);
            get => downlight;
        }

        public bool UpLight     //上砖测光电   
        {
            set => Set(ref uplight, value);
            get => uplight;
        }

        public byte Reserve       //预留   
        {
            set => Set(ref reserve, value);
            get => reserve;
        }


        #endregion

        #region[日志]

        public override string ToString()
        {
            return string.Format("状态[ {0} ], 目标[ {1} ], 任务[ {2} ], 完成[ {3} ], 上标[ {4} ], 下标[ {5} ], 货物[ {6} ], 模式[ {7} ], 上光[ {8} ], 下光[ {9} ], 预留[ {10} ]",
                DeviceStatus, TargetSite, CurrentTask, FinishTask, UpSite, DownSite, LoadStatus, WorkMode, S(UpLight), S(DownLight), Reserve);
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
