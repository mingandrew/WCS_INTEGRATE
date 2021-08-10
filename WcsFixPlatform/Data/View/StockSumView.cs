using enums.track;
using GalaSoft.MvvmLight;
using module.goods;
using resource;
using System;

namespace wcs.Data.View
{
    public class StockSumView : ViewModelBase
    {
        public uint track_id { set; get; }
        private uint goods_id;
        private DateTime? produce_time;
        private DateTime? last_produce_time;
        private int count;
        private int stack;
        private int pieces;
        public uint area { set; get; }
        public ushort line { set; get; }
        public byte track_type { set; get; }
        public byte track_type2 { set; get; }
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

        public DateTime? LastProduceTime
        {
            get => last_produce_time;
            set => Set(ref last_produce_time, value);
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

        public bool IsWorkIn()
        {
            TrackType2E tt2 = (TrackType2E)track_type2;
            return (tt2 == TrackType2E.通用 || tt2 == TrackType2E.入库);
        }

        public bool IsWorkOut()
        {
            TrackType2E tt2 = (TrackType2E)track_type2;
            return (tt2 == TrackType2E.通用 || tt2 == TrackType2E.出库);
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
            GoodId = sum.goods_id;
            Count = sum.count;
            Stack = sum.stack;
            Pieces = sum.pieces;
            ProduceTime = sum.produce_time;
            LastProduceTime = sum.last_produce_time;
            track_type2 = sum.track_type2;
            sum_level = sum.sum_level;
        }

        public StockSum GetStockSum()
        {
            return new StockSum()
            {
                area = area,
                line = line,
                track_id = track_id,
                track_type = track_type,
                track_type2 = track_type2,
                goods_id = goods_id,
                count = count,
                pieces = pieces,
                stack = stack,
                produce_time = produce_time,
                last_produce_time = last_produce_time,
                sum_level = sum_level,
            };
        }

    }
}
