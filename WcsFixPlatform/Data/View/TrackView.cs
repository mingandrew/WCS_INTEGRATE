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
        private SolidColorBrush trackbrush;
        private SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
        private SolidColorBrush GrayBrush = new SolidColorBrush(Colors.Gray);
        private SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        private SolidColorBrush LightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        private SolidColorBrush OrangeBrush = new SolidColorBrush(Colors.LightSkyBlue);
        //private ushort ferry_up_code;
        //private ushort ferry_down_code;
        //private uint brother_track_id;
        //private uint left_track_id;
        //private uint right_track_id;
        //private ushort rfid_1;
        //private ushort rfid_2;
        //private ushort rfid_3;
        //private ushort rfid_4;
        //private ushort rfid_5;
        //private ushort rfid_6;
        //private short order;
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

        public void Update(Track track)
        {
            Status = track.StockStatus;
            TrackStatus = track.TrackStatus;
            Width = track.width;
            Left_distance = track.left_distance;
            Right_distance = track.right_distance;
            Max_store = track.max_store;
            Memo = track.memo;
            switch (Status)
            {
                case TrackStockStatusE.空砖:
                    TrackBrush = WhiteBrush;
                    break;
                case TrackStockStatusE.有砖:
                    TrackBrush = OrangeBrush;
                    break;
                case TrackStockStatusE.满砖:
                    TrackBrush = BlackBrush;
                    break;
            }
        }
    }
}
