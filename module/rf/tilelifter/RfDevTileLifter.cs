using enums;
using module.device;

namespace module.rf
{
    public class RfDevTileLifter
    {
        public uint Id { set; get; }
        public uint Area { set; get; }
        public SocketConnectStatusE Conn { set; get; }
        public bool Working { set; get; }
        public DevWorkTypeE WorkType { set; get; }
        public DevTileLifter TileLifter { set; get; }
    }
}
