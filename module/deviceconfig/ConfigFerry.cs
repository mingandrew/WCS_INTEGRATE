using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace module.deviceconfig
{
    public class ConfigFerry
    {
        /// <summary>
        /// 摆渡车 设备ID
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 摆渡轨道ID
        /// </summary>
        public uint track_id { set; get; }

    }
}
