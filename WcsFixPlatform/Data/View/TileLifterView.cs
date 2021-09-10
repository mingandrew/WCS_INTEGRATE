using enums;
using GalaSoft.MvvmLight;
using module.device;
using resource;
using System.Windows.Media;

namespace wcs.Data.View
{
    public class TileLifterView : ViewModelBase
    {
        public uint ID { set; get; }
        public uint AreaId { set; get; }
        public ushort LineId { set; get; }
        public string Name { set; get; }
        private uint goodsid;
        private bool working;
        private string trackid;
        private DevWorkTypeE worktype;
        private string goodscount;
        private string goodsname;

        private byte level;
        #region[逻辑字段]

        private SocketConnectStatusE connstatus;
        private bool isconnect;
        #endregion
        public byte Level
        {
            get => level;
            set => Set(ref level, value);
        }

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

        public uint GoodsId
        {
            get => goodsid;
            set => Set(ref goodsid, value);
        }

        public string GoodsName
        {
            get => goodsname;
            set => Set(ref goodsname, value);
        }

        public string GoodsCount
        {
            get => goodscount;
            set => Set(ref goodscount, value);
        }

        public bool Working
        {
            get => working;
            set => Set(ref working, value);
        }

        public string LastTrackId
        {
            get => trackid;
            set => Set(ref trackid, value);
        }

        public DevWorkTypeE WorkType
        {
            get => worktype;
            set => Set(ref worktype, value);
        }

        #region[字段]
        private byte deviceid;      //设备号
        //private DevLifterLoadE isload1;   //货物状态1 左
        //private DevLifterLoadE isload2;   //货物状态2 右
        private bool isneed1;   //需求信号1 左
        private bool isneed2;   //需求信号2 右
        private byte fullqty;       //满砖数量
        private byte site1qty;     //工位1数量
        private byte site2qty;     //当前数量
        private bool isinvolve1;      //介入状态1 左
        private bool isinvolve2;      //介入状态2 右
        private DevOperateModeE operatemode;   //作业模式
        private StrategyInE instrategy;//入库策略
        private StrategyOutE outstrategy;//出库策略
        private uint goods1;   //工位1品种
        private uint goods2;   //工位2品种
        private TileShiftStatusE shiftstatus;   //转产状态
        private uint setgoods;   //设定品种
        private byte setlevel;   //设定等级


        private SolidColorBrush isload1brush = Gray;   //货物状态1 颜色
        private SolidColorBrush isload2brush = Gray;   //货物状态2 颜色
        #endregion

        #region[属性]
        public byte DeviceID//设备号
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }

        private DevLifterLoadE LoadStatus1 { set; get; }//货物状态1 左

        private DevLifterLoadE LoadStatus2 { set; get; }//货物状态2 右

        #region[工位状态]
        private static SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(224,224,224));
        private static SolidColorBrush Yellow = new SolidColorBrush(Color.FromRgb(238, 170, 34));
        private static SolidColorBrush Orange = new SolidColorBrush(Color.FromRgb(255, 94, 3));
        #endregion
        public SolidColorBrush IsLoad1Brush
        {
            get => isload1brush;
            set => Set(ref isload1brush, value);
        }
        public SolidColorBrush IsLoad2Brush
        {
            get => isload2brush;
            set => Set(ref isload2brush, value);
        }

        public bool IsNeed1//需求信号1 左
        {
            set => Set(ref isneed1, value);
            get => isneed1;
        }

        public bool IsNeed2//需求信号2 右
        {
            set => Set(ref isneed2, value);
            get => isneed2;
        }

        public byte FullQty//满砖数量
        {
            set => Set(ref fullqty, value);
            get => fullqty;
        }
        public byte Site1Qty//当前数量
        {
            set => Set(ref site1qty, value);
            get => site1qty;
        }
        public byte Site2Qty//当前数量
        {
            set => Set(ref site2qty, value);
            get => site2qty;
        }

        public bool IsInvolve1//介入状态1 左
        {
            set => Set(ref isinvolve1, value);
            get => isinvolve1;
        }

        public bool IsInvolve2//介入状态2 右
        {
            set => Set(ref isinvolve2, value);
            get => isinvolve2;
        }

        public DevOperateModeE OperateMode//作业模式
        {
            set => Set(ref operatemode, value);
            get => operatemode;
        }

        public StrategyInE InStrategy
        {
            get => instrategy;
            set => Set(ref instrategy, value);
        }
        public StrategyOutE OutStrategy
        {
            get => outstrategy;
            set => Set(ref outstrategy, value);
        }

        public uint Goods1 //左工位品种ID
        {
            get => goods1;
            set => Set(ref goods1, value);
        }

        public uint Goods2 //右工位品种ID
        {
            get => goods2;
            set => Set(ref goods2, value);
        }

        public uint SetGoods //设定品种ID
        {
            get => setgoods;
            set => Set(ref setgoods, value);
        }

        public byte SetLevel //设定等级
        {
            set => Set(ref setlevel, value);
            get => setlevel;
        }

        public TileShiftStatusE ShiftStatus //转产状态
        {
            get => shiftstatus;
            set => Set(ref shiftstatus, value);
        }

        #endregion

        #region[更新]

        internal void Update(DevTileLifter st, SocketConnectStatusE conn, uint gid,
            StrategyInE instrategy, StrategyOutE outstrategy, bool working, string tid, DevWorkTypeE wtype, string goodscount, byte level)
        {
            DeviceID = st.DeviceID;
            if(LoadStatus1 != st.LoadStatus1)
            {
                switch (st.LoadStatus1)
                {
                    case DevLifterLoadE.无砖:
                        IsLoad1Brush = Gray;
                        break;
                    case DevLifterLoadE.有砖:
                        IsLoad1Brush = Yellow;
                        break;
                    case DevLifterLoadE.满砖:
                        IsLoad1Brush = Orange;
                        break;
                }
                LoadStatus1 = st.LoadStatus1;
            }


            if (LoadStatus2 != st.LoadStatus2)
            {
                switch (st.LoadStatus2)
                {
                    case DevLifterLoadE.无砖:
                        IsLoad2Brush = Gray;
                        break;
                    case DevLifterLoadE.有砖:
                        IsLoad2Brush = Yellow;
                        break;
                    case DevLifterLoadE.满砖:
                        IsLoad2Brush = Orange;
                        break;
                }
                LoadStatus2 = st.LoadStatus2;
            }

            IsNeed1 = st.Need1;
            IsNeed2 = st.Need2;
            FullQty = st.FullQty;
            Site1Qty = st.Site1Qty;
            Site2Qty = st.Site2Qty;
            IsInvolve1 = st.Involve1;
            IsInvolve2 = st.Involve2;
            OperateMode = st.OperateMode;
            ConnStatus = conn;
            IsConnect = ConnStatus == SocketConnectStatusE.通信正常;
            GoodsId = gid;
            GoodsName = PubMaster.Goods.GetGoodsName(GoodsId);
            GoodsCount = goodscount;
            InStrategy = instrategy;
            OutStrategy = outstrategy;

            Goods1 = st.Goods1;
            Goods2 = st.Goods2;
            SetGoods = st.SetGoods;
            SetLevel = st.SetLevel;
            ShiftStatus = st.ShiftStatus;

            Working = working;
            LastTrackId = tid;
            WorkType = wtype;

            Level = level;
        }
        #endregion
    }
}
