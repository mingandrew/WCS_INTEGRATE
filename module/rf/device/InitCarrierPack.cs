using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace module.rf.device
{
    public class InitCarrierPack
    {
        public uint deviceid { get; set; }
        public uint trackid { get; set; }
        public uint movedirection { get; set; }
        public uint initpoint { get; set; }
    }
}
