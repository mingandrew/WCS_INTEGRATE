using module.goods;

namespace module.rf
{
    public class GoodUpdatePack
    {
        public uint Tile_Id { set; get; }
        public bool AddGood { set; get; }
        public Goods EditGood { set; get; }
    }
}
