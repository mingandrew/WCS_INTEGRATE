namespace module.goods
{
    public class PreStockGood
    {
        public uint id { set; get; }
        public uint tile_id { set; get; }
        public uint good_id { set; get; }
        public byte order { set; get; }
        public int pre_good_qty { set; get; }//预约品种数量
        public bool pre_good_all { set; get; }//预约使用全部库存
        //public bool loop { set; get; }//循环预设品种列表
    }
}
