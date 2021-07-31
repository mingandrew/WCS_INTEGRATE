using enums.track;
using GalaSoft.MvvmLight;
using HandyControl.Data;
using module.track;
using System.Windows.Media;

namespace wcs.Data.View
{
    public class TrackView : ViewModelBase
    {
        public TrackView(Track track)
        {
            Id = track.id;
            Name = track.name;
            Area = track.area;
            LineId = track.line;
            Type = track.Type;
            Type2 = track.Type2;
            Update(track);
        }

        #region[字段]
        private uint id;
        
        private string name;
        private TrackTypeE type;
        private TrackType2E type2;
        private TrackStockStatusE status;
        private TrackStatusE trackstatus;
        private ushort width;
        private ushort left_distance;
        private ushort right_distance;
        private int max_store;
        private string memo;
        private ushort stock_qty;
        private bool sort_able;//倒库状态
        private int sort_level;//倒库等级
        private SolidColorBrush trackbrush, trackfbrush, stockbrush;

        #region[轨道状态]
        private SolidColorBrush Green = new SolidColorBrush(Color.FromRgb(34, 177, 76));
        private SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(237, 28, 36));
        private SolidColorBrush Blue = new SolidColorBrush(Color.FromRgb(0, 162, 232));
        private SolidColorBrush Yellow = new SolidColorBrush(Color.FromRgb(255, 242, 0));
        private SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        private SolidColorBrush LightGray = new SolidColorBrush(Color.FromRgb(242, 242, 242));
        private SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(195, 195, 195));
        private SolidColorBrush DarkGray = new SolidColorBrush(Color.FromRgb(140, 140, 140));
        private SolidColorBrush White = new SolidColorBrush(Colors.White);
        private double trackprogress;
        #endregion

        #endregion

        public uint Id
        {
            get => id;
            set => Set(ref id, value);
        }
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        public ushort Area { set; get; }
        public ushort LineId { set; get; }
        public TrackTypeE Type
        {
            get => type;
            set => Set(ref type, value);
        }
        public TrackType2E Type2
        {
            get => type2;
            set => Set(ref type2, value);
        }
        public TrackStockStatusE Status
        {
            get => status;
            set => Set(ref status, value);
        }

        public TrackStatusE TrackStatus
        {
            get => trackstatus;
            set => Set(ref trackstatus, value);
        }
        public ushort Width
        {
            get => width;
            set => Set(ref width, value);
        }
        public ushort Left_distance
        {
            get => left_distance;
            set => Set(ref left_distance, value);
        }
        public ushort Right_distance
        {
            get => right_distance;
            set => Set(ref right_distance, value);
        }
        public int Max_store
        {
            get => max_store;
            set => Set(ref max_store, value);
        }
        public string Memo
        {
            get => memo;
            set => Set(ref memo, value);
        }

        public ushort StockQty
        {
            get => stock_qty;
            set => Set(ref stock_qty, value);
        }

        public bool SortAble
        {
            get => sort_able;
            set => Set(ref sort_able, value);
        }

        public int SortLevel
        {
            get => sort_level;
            set => Set(ref sort_level, value);
        }

        public SolidColorBrush TrackBrush
        {
            get => trackbrush;
            set => Set(ref trackbrush, value);
        }

        public SolidColorBrush TrackFBrush
        {
            get => trackfbrush;
            set => Set(ref trackfbrush, value);
        }

        public SolidColorBrush StockBrush
        {
            get => stockbrush;
            set => Set(ref stockbrush, value);
        }
        public double TrackProgress
        {
            get => trackprogress;
            set => Set(ref trackprogress, value);
        }

        public void Update(Track track)
        {
            Status = track.StockStatus;
            TrackStatus = track.TrackStatus;
            Width = track.width;
            Left_distance = track.left_distance;
            Right_distance = track.right_distance;
            Max_store = track.max_store;
            Memo = track.memo;
            SortAble = track.sort_able;
            SortLevel = track.sort_level;
            switch (TrackStatus)
            {
                case TrackStatusE.停用:
                    TrackBrush = Red;
                    TrackFBrush = Black; 
                    break;
                case TrackStatusE.启用:
                    TrackBrush = Green;
                    TrackFBrush = Black;
                    break;
                case TrackStatusE.倒库中:
                    TrackBrush = Green;
                    TrackFBrush = Black;
                    break;
                case TrackStatusE.仅上砖:
                    TrackBrush = Blue;
                    TrackFBrush = Yellow;
                    break;
                case TrackStatusE.仅下砖:
                    TrackBrush = Yellow;
                    TrackFBrush = Black;
                    break;
            }

            switch (Status)
            {
                case TrackStockStatusE.空砖:
                    StockBrush = LightGray;
                    TrackProgress = 0;
                    break;
                case TrackStockStatusE.有砖:
                    StockBrush = Gray;
                    TrackProgress = 50;
                    break;
                case TrackStockStatusE.满砖:
                    StockBrush = DarkGray;
                    TrackProgress = 100;
                    break;
                default:
                    break;
            }
        }

        public void UpdateStockQty(ushort qty)
        {
            StockQty = qty;
        }


        /// <summary>
        /// 是否入库作业轨道
        /// </summary>
        /// <returns></returns>
        public bool IsWorkIn()
        {
            return Type == TrackTypeE.储砖_入
                || (Type == TrackTypeE.储砖_出入 && (Type2 == TrackType2E.通用 || Type2 == TrackType2E.入库));
        }

        /// <summary>
        /// 是否出库作业轨道
        /// </summary>
        /// <returns></returns>
        public bool IsWorkOut()
        {
            return Type == TrackTypeE.储砖_出
                || (Type == TrackTypeE.储砖_出入 && (Type2 == TrackType2E.通用 || Type2 == TrackType2E.出库));
        }

    }
}
