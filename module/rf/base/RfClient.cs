using enums;
using System;

namespace module.rf
{
    public class RfClient
    {
        public string rfid { set; get; }
        public string name { set; get; }
        public string ip { set; get; }
        public DateTime? conn_time { set; get; }
        public DateTime? disconn_time { set; get; }

        public RfConnectE Status { set; get; }

    }
}
