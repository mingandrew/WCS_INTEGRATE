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

        public List<OrganizeCar> CarIds { set; get; } = new List<OrganizeCar>();

        /// <summary>
        /// 获取区域里面配置的可库存整理车辆
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public List<OrganizeCar> GetOrganizeCarIds(uint areaid)
        {
            return CarIds.FindAll(c => c.Area_ID == areaid);
        }

        public bool IsCarSet(uint devid)
        {
            return CarIds.Exists(c => c.Car_ID == devid);
        }
    }

    public class OrganizeCar
    {
        public uint Area_ID { set; get; }
        public uint Car_ID { set; get; }
    }
}
