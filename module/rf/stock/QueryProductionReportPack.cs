using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace module.rf
{
    public class QueryProductionReportPack
    {
        public uint areaid { get; set; }
        public ushort lineid { get; set; }
        public long starttime { get; set; }
        public long stoptime { get; set; }
        public int querytype { get; set; }
    }
}
