using enums;

namespace module.rf.device
{
    public class RfTileGoodPack
    {
        public uint tile_id { set; get; }
        public byte type { set; get; }
        public ushort area { set; get; }
        public uint good_id { set; get; }
        public uint oldgood_id { set; get; }
        public uint pregood_id { set; get; }

        public TileShiftStatusE shiftstatus { set; get; }

    }
}
