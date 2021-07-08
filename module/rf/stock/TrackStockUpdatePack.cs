using System;

namespace module.rf
{
    public class TrackStockUpdatePack
    {
        public uint TrackId { set; get; }
        public uint GoodId { set; get; }
        public uint NewGoodId { set; get; }
        public byte AddQty { set; get; }
        public DateTime? ProduceTime { set; get; }
        public bool ChangeDate { set; get; }
    }
}
