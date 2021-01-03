using enums;
using module.device;

namespace module.rf
{
    public class RfDevCarrier
    {
        public uint Id { set; get; }
        public uint Area { set; get; }
        public SocketConnectStatusE Conn { set; get; }
        public DevCarrier Carrier { set; get; }
    }
}
