namespace module.rf
{
    public class TrackUpdatePack
    {
        public uint TrackId { set; get; }
        public int StockStatus { set; get; }//库存状态
        public int TrackStatus { set; get; }//轨道使用状态

        public int OldStockStatus { set; get; }
        public int OldTrackStatus { set; get; }

        public bool IsStockStatusChange()
        {
            return StockStatus != OldStockStatus;
        }
        
        public bool IsStatusChange()
        {
            return TrackStatus != OldTrackStatus;
        }


    }
}
