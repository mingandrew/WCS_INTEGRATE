using enums;

namespace module.rf.device
{
    /// <summary>
    /// 砖机品种和模式信息
    /// </summary>
    public class RfTileModePack
    {
        public uint tile_id { set; get; }
        public byte type { set; get; }
        public uint area { set; get; }
        public uint goods_id { set; get; }
        public uint pregood_id { set; get; }

        public int work_mode { set; get; }
        public int work_mode_next { set; get; }
        public bool do_cutover { set; get; }

        public TileWorkModeE WorkMode
        {
            get => (TileWorkModeE)work_mode;
        }

        public TileWorkModeE WorkModeNext
        {
            get => (TileWorkModeE)work_mode_next;
        }
    }
}
