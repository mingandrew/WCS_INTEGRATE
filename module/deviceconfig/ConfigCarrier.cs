using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace module.deviceconfig
{
    public class ConfigCarrier
    {
        /// <summary>
        /// 运输车 设备ID
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 前进放货没扫到地标
        /// </summary>
        public bool a_givemisstrack { set; get; }

        /// <summary>
        /// 后退取砖没扫到地标
        /// </summary>
        public bool a_takemisstrack { set; get; }

        /// <summary>
        /// 故障轨道
        /// </summary>
        public uint a_alert_track { set; get; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public uint stock_id { set; get; }

    }
}
