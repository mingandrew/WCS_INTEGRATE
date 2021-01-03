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
        private byte operatemode;   //作业模式
        private byte goods1;   //工位1品种
        private byte goods2;   //工位2品种
        private byte shiftstatus;   //转产状态
        private bool shiftaccept;   //转产接收状态
        #endregion

        #region[属性]
        public byte DeviceID//设备号
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        public bool Load1//货物状态1 左
        {
            set => Set(ref loadstatus1, value);
            get => loadstatus1;
        }

        public bool Load2//货物状态2 右
        {
            set => Set(ref isload2,value);
            get => isload2;
        }

        public bool Need1//需求信号1 左
        {
            set => Set(ref isneed1,value);
            get => isneed1;
        }

        public bool Need2//需求信号2 右
        {
            set => Set(ref isneed2, value);
            get => isneed2;
        }

        public byte FullQty//满砖数量
        {
            set => Set(ref fullqty, value);
            get => fullqty;
        }
        public byte RecentQty//当前数量
        {
            set => Set(ref recentqty, value);
            get => recentqty;
        }

        public bool Involve1//介入状态1 左
        {
            set => Set(ref isinvolve1, value);
            get => isinvolve1;
        }

        public bool Involve2//介入状态2 右
        {
            set => Set(ref isinvolve2, value);
            get => isinvolve2;
        }

        public DevOperateModeE OperateMode//作业模式
        {
            set => Set(ref operatemode, (byte)value);
            get => (DevOperateModeE)operatemode;
        }

        public DevLifterGoodsE Goods1//工位1品种 左
        {
            set => Set(ref goods1, (byte)value);
            get => (DevLifterGoodsE)goods1;
        }

        public DevLifterGoodsE Goods2//工位2品种 右
        {
            set => Set(ref goods2, (byte)value);
            get => (DevLifterGoodsE)goods2;
        }

        public TileShiftStatusE ShiftStatus//转产状态
        {
            set => Set(ref shiftstatus, (byte)value);
            get => (TileShiftStatusE)shiftstatus;
        }

        public bool ShiftAccept//转产接收状态
        {
            set => Set(ref shiftaccept, value);
            get => shiftaccept;
        }

        #endregion

        #region[日志]

        public override string ToString()
        {
            return string.Format("货物1：{0}, 货物2：{1}, 需求1：{2}, 需求2：{3}, 满砖：{4}, 现有：{5}, 介入1：{6}, 介入2：{7}, 模式：{8}, 工位1：{9}, 工位2：{10}, 转产：{11}, 转产接收：{12}"
                   , S(Load1), S(Load2), S(Need1), S(Need2), FullQty, RecentQty, S(Involve1), S(Involve2), OperateMode, Goods1, Goods2, ShiftStatus, S(ShiftAccept));
        }

        private string S(bool v)
        {
            return v ? "✔" : "❌";
        }

        #endregion
    }
}
