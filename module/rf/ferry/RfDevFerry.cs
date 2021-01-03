using enums;
using module.device;

namespace module.rf
{
    public class RfDevFerry
    {
        public uint Id { set; get; }
        public uint Area { set; get; }
        public SocketConnectStatusE Conn { set; get; }
        public DevFerry Ferry { set; get; }
    }
}
