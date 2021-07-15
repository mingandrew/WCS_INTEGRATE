using GalaSoft.MvvmLight;
using module.goods;
using resource;
using System;

namespace wcs.Data.View
{
    public class StockSumView : ViewModelBase
    {
        public uint track_id { set; get; }
        public uint goods_id;
        public DateTime? produce_time;
        private int count;
        private int stack;
        private int pieces;
        public uint area { set; get; }
        public ushort line { set; get; }
        public byte track_type { set; get; }
        public byte sum_level { set; get; }

        public uint GoodId
        {
            get => goods_id;
            set => Set(ref goods_id, value);
        }

        public DateTime? ProduceTime
        {
            get => produce_time;
            set => Set(ref produce_time, value);
        }

        public int Count
        {
            get => count;
            set => Set(ref count, value);
        }
        public int Stack
        {
            get => stack;
            set => Set(ref stack, value);
        }
        public int Pieces
        {
            get => pieces;
            set => Set(ref pieces, value);
        }

        public StockSumView(StockSum sum)
        {
            track_id = sum.track_id;
            area = sum.area;
            line = PubMaster.Track.GetTrackLine(track_id);
            track_type = sum.track_type;
            sum_level = sum.sum_level;
            Update(sum);
        }

        public void Update(StockSum sum)
        {
            Count = sum.count;
            Stack = sum.stack;
            Pieces = sum.pieces;
            ProduceTime = sum.produce_time;
            GoodId = sum.goods_id;
            sum_level = sum.sum_level;
        }
    }
}
