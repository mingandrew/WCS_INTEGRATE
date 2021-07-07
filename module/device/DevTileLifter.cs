using enums;

namespace module.device
{
    public class DevTileLifter : IDevice
    {
        #region[字段]
        private byte deviceid;      //设备号
        //private bool isload1;       //货物状态1 左
        //private bool isload2;       //货物状态2 右
        private byte isload1;       //货物状态1 左
        private byte isload2;       //货物状态2 右
        private bool isneed1;       //需求信号1 左
        private bool isneed2;       //需求信号2 右
        private byte fullqty;       //满砖数量
        private byte site1qty;      //工位1数量
        private byte site2qty;      //工位2数量
        private bool isinvolve1;    //介入状态1 左
        private bool isinvolve2;    //介入状态2 右
        private byte operatemode;   //操作模式
        private uint goods1;        //工位1品种
        private uint goods2;        //工位2品种
        private byte shiftstatus;   //转产状态
        private bool shiftaccept;   //转产接收状态
        private byte workmode;      //作业模式
        private uint setgoods;      //设定品种
        private byte setlevel;      //设定等级

        private bool needsystemshift; //砖机需转产信号
        private byte backupshiftdev; //切换砖机设备号
        private byte alertlightstatus;       //报警灯状态
        public byte receivesetfull;       //预留2：收到设满砖信号， 0：无    1：收到设满砖信号
        private byte reserve3;       //预留3
        private byte reserve4;       //预留4
        private byte markcode;       //标识码（PLC发送的码，需要PC进行控制码0x88回复）
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
            set => Set(ref isload1, (byte)(value ? 1 : 0));
            get => LoadStatus1 != DevLifterLoadE.无砖;
        }

        /// <summary>
        /// 货物状态1 左
        /// </summary>
        public DevLifterLoadE LoadStatus1
        {
            set => Set(ref isload1, (byte)value);
            get => (DevLifterLoadE)isload1;
        }

        /// <summary>
        /// 货物状态2 右
        /// </summary>
        public bool Load2
        {
            set => Set(ref isload2, (byte)(value ? 1 : 0));
            get => LoadStatus2 != DevLifterLoadE.无砖;
        }

        /// <summary>
        /// 货物状态2 右
        /// </summary>
        public DevLifterLoadE LoadStatus2
        {
            set => Set(ref isload2, (byte)value);
            get => (DevLifterLoadE)isload2;
        }

        /// <summary>
        /// 需求信号1 左
        /// </summary>
        public bool Need1
        {
            set => Set(ref isneed1, value);
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
        /// 工位1数量
        /// </summary>
        public byte Site1Qty
        {
            set => Set(ref site1qty, value);
            get => site1qty;
        }

        /// <summary>
        /// 工位2数量
        /// </summary>
        public byte Site2Qty
        {
            set => Set(ref site2qty, value);
            get => site2qty;
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

        /// <summary>
        /// 转产信号
        /// </summary>
        public bool NeedSytemShift
        {
            get => needsystemshift;
            set => Set(ref needsystemshift, value);
        }

        /// <summary>
        /// 备用选用设备号
        /// </summary>
        public byte BackupShiftDev
        {
            get => backupshiftdev;
            set => Set(ref backupshiftdev, value);
        }

        /// <summary>
        /// 报警灯
        /// </summary>
        public byte AlertLightStatus
        {
            get => alertlightstatus;
            set => Set(ref alertlightstatus, value);
        }

        public bool ReceiveSetFull
        {
            get => receivesetfull == 1;
            set => receivesetfull = (byte)(value ? 1 : 0);
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
            return string.Format("物1[ {0} ], 物2[ {1} ], 需1[ {2} ], 需2[ {3} ], 满[ {4} ], 工1[ {5} ], 工2[ {6} ], 介1[ {7} ], 介2[ {8} ], 操作[ {9} ], " +
                "位1[ {10} ], 位2[ {11} ], 转产[ {12} ], 接收[ {13} ], 模式[ {14} ], 设品[ {15} ], 设级[ {16} ], 转产信号[ {17} ], 备用设备[ {18} ], " +
                "灯[ {19} ], 设满砖[ {20} ], 预3[ {21} ], 预4[ {22} ], 标识码[ {23} ]",
                LoadStatus1, LoadStatus2, S(Need1), S(Need2), FullQty, Site1Qty, Site2Qty, S(Involve1), S(Involve2), OperateMode,
                Goods1, Goods2, ShiftStatus, S(ShiftAccept), WorkMode, SetGoods, SetLevel, S(NeedSytemShift), BackupShiftDev,
                AlertLightStatus, S(ReceiveSetFull), Reserve3, Reserve4, MarkCode);
        }

        private string S(bool v)
        {
            return v ? "✔" : "❌";
        }

        #endregion
    }
}
