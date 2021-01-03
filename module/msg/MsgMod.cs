using enums;
using module.device;

namespace module.msg
{
    public class SocketMsgMod
    {
        public uint ID { set; get; }
        public SocketMsgTypeE MsgType { set; get; } 
        public SocketConnectStatusE ConnStatus { set; get; }
        public IDevice Device { set; get; }
        public SocketMsgMod()
        {

        }
    }
}
