using enums;

namespace module.device
{
    public class DevTileLifter : IDevice
    {
        #region[字段]
        private byte deviceid;      //设备号
        private bool loadstatus1;   //货物状态1 左
        private bool isload2;   //货物状态2 右
        private bool isneed1;   //需求信号1 左
        private bool isneed2;   //需求信号2 右
        private byte fullqty;       //满砖数量
        private byte recentqty;     //当前数量
        private bool isinvolve1;      //介入状态1 左
        private bool isinvolve2;      //介入状态2 右
        private byte operatemode;   //操作模式
        private uint goods1;   //工位1品种
        private uint goods2;   //工位2品种
        private byte shiftstatus;   //转产状态
        private bool shiftaccept;   //转产接收状态
        private byte workmode;   //作业模式
        private uint setgoods;   //设定品种
        private byte setlevel;   //设定等级
        #endregion

        #region[属性]

        /// <summary>
        /// 设备号
        /// </summary>
        public byte DeviceID
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        /// <summary>
        /// 货物状态1 左
        /// </summary>
        public bool Load1
        {
            set => Set(ref loadstatus1, value);
            get => loadstatus1;
        }

        /// <summary>
        /// 货物状态2 右
        /// </summary>
        public bool Load2
        {
            set => Set(ref isload2,value);
            get => isload2;
        }

        /// <summary>
        /// 需求信号1 左
        /// </summary>
        public bool Need1
        {
            set => Set(ref isneed1,value);
            get => isneed1;
        }

        /// <summary>
        /// 需求信号2 右
        /// </summary>
        public bool Need2
        {
            set => Set(ref isneed2, value);
            get => isneed2;
        }

        /// <summary>
        /// 满砖层数
        /// </summary>
        public byte FullQty
        {
            set => Set(ref fullqty, value);
            get => fullqty;
        }

        /// <summary>
        /// 当前层数
        /// </summary>
        public byte RecentQty
        {
            set => Set(ref recentqty, value);
            get => recentqty;
        }

        /// <summary>
        /// 介入状态1 左
        /// </summary>
        public bool Involve1
        {
            set => Set(ref isinvolve1, value);
            get => isinvolve1;
        }

        /// <summary>
        /// 介入状态2 右
        /// </summary>
        public bool Involve2
        {
            set => Set(ref isinvolve2, value);
            get => isinvolve2;
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
        /// 工位1品种 左
        /// </summary>
        public uint Goods1
        {
            set => Set(ref goods1, value);
            get => goods1;
        }

        /// <summary>
        /// 工位2品种 右
        /// </summary>
        public uint Goods2
        {
            set => Set(ref goods2, value);
            get => goods2;
        }

        /// <summary>
        /// 转产状态
        /// </summary>
        public TileShiftStatusE ShiftStatus
        {
            set => Set(ref shiftstatus, (byte)value);
            get => (TileShiftStatusE)shiftstatus;
        }

        /// <summary>
        /// 转产接收
        /// </summary>
        public bool ShiftAccept
        {
            set => Set(ref shiftaccept, value);
            get => shiftaccept;
        }

        /// <summary>
        /// 作业模式
        /// </summary>
        public TileWorkModeE WorkMode
        {
            set => Set(ref workmode, (byte)value);
            get => (TileWorkModeE)workmode;
        }

        /// <summary>
        /// 设定品种
        /// </summary>
        public uint SetGoods
        {
            set => Set(ref setgoods, value);
            get => setgoods;
        }

        /// <summary>
        /// 设定等级
        /// </summary>
        public byte SetLevel
        {
            set => Set(ref setlevel, value);
            get => setlevel;
        }

        #endregion

        #region[日志]

        public override string ToString()
        {
            return string.Format("物1[ {0} ], 物2[ {1} ], 需1[ {2} ], 需2[ {3} ], 满[ {4} ], 有[ {5} ], 介1[ {6} ], 介2[ {7} ], 操作[ {8} ], " +
                "位1[ {9} ], 位2[ {10} ], 转产[ {11} ], 接收[ {12} ], 模式[ {13} ], 设定品种[ {14} ], 设定等级[ {15} ]", 
                S(Load1), S(Load2), S(Need1), S(Need2), FullQty, RecentQty, S(Involve1), S(Involve2), OperateMode,  
                Goods1, Goods2, ShiftStatus, S(ShiftAccept), WorkMode, SetGoods, SetLevel);
        }

        private string S(bool v)
        {
            return v ? "✔" : "❌";
        }

        #endregion
    }
}
