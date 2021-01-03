using enums;
using module.rf;

namespace module.msg
{
    public class RfMsgMod
    {
        public string MEID { set; get; }
        public string IP { set; get; }
        public RfConnectE Conn { set; get; }
        public RfPackage Pack { set; get; }

        public bool IsPackHaveData()
        {
            return  Pack!=null && Pack.Data != null && Pack.Data.Length > 0;
        }
    }
}
