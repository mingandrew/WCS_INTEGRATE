using GalaSoft.MvvmLight;
using module.goods;
using System;

namespace wcs.Data.View
{
    public class StockSumView : ViewModelBase
    {
        public uint track_id { set; get; }
        public uint goods_id;
        public DateTime? produce_time;
        private uint count;
        private uint stack;
        private uint pieces;
        public uint area { set; get; }
        public byte track_type { set; get; }

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

        public uint Count
        {
            get => count;
            set => Set(ref count, value);
        }
        public uint Stack
        {
            get => stack;
            set => Set(ref stack, value);
        }
        public uint Pieces
        {
            get => pieces;
            set => Set(ref pieces, value);
        }

        public StockSumView(StockSum sum)
        {
            track_id = sum.track_id;
            area = sum.area;
            track_type = sum.track_type;
            Update(sum);
        }

        public void Update(StockSum sum)
        {
            Count = sum.count;
            Stack = sum.stack;
            Pieces = sum.pieces;
            ProduceTime = sum.produce_time;
            GoodId = sum.goods_id;
        }
    }
}
