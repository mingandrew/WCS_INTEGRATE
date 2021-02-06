using enums.track;
using GalaSoft.MvvmLight;
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
            Type = track.Type;
            Update(track);
        }

        #region[字段]
        private uint id;
        private string name;
        private ushort area;
        private TrackTypeE type;
        private TrackStockStatusE status;
        private TrackStatusE trackstatus;
        private ushort width;
        private ushort left_distance;
        private ushort right_distance;
        private int max_store;
        private string memo;
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
        public ushort Area
        {
            get => area;
            set => Set(ref area, value);
        }
        public TrackTypeE Type
        {
            get => type;
            set => Set(ref type, value);
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
    }
}
