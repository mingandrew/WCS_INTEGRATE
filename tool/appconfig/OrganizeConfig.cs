using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tool.appconfig
{
    public class OrganizeConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\OrganizeConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";

        public List<OrganizeSet> OrgList { set; get; } = new List<OrganizeSet>();

        /// <summary>
        /// 获取区域里面配置的可库存整理车辆
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public List<OrganizeSet> GetOrganizeCarIds(uint areaid, ushort lineid)
        {
            return OrgList.FindAll(c => c.Area_ID == areaid && c.Line_ID == lineid);
        }

    }

    public class OrganizeSet
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public uint Area_ID { set; get; }

        /// <summary>
        /// 线路ID
        /// </summary>
        public ushort Line_ID { set; get; }

        /// <summary>
        /// 库存间隔
        /// </summary>
        public ushort Stock_Space { set; get; } = 250;

    }
}
