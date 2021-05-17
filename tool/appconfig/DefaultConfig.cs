using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tool.appconfig
{
    public class DefaultConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\DefaultConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";

        #region【区域品种常用信息】
        public List<AreaDefaultData> AreaDefaultList = new List<AreaDefaultData>();
        public AreaDefaultData GetAreaDefault(uint areaid)
        {
            return AreaDefaultList.Find(c => c.AreaId == areaid);
        }

        public void UpdateAreaDefault(uint areaid, ushort goodqty, uint goodsizeid, byte goodlevel)
        {
            AreaDefaultData area = GetAreaDefault(areaid);
            if(area == null)
            {
                area = new AreaDefaultData()
                {
                    AreaId = areaid,
                    Last_Good_Qty = goodqty,
                    Last_Good_SizeId = goodsizeid,
                    Last_Good_Level = goodlevel,
                };
                AreaDefaultList.Add(area);
            }
            else
            {
                area.Last_Good_Qty = goodqty;
                area.Last_Good_SizeId = goodsizeid;
                area.Last_Good_Level = goodlevel;
            }
            GlobalWcsDataConfig.SaveDefaultConfig();
        }
        #endregion

        #region[脉冲默认信息]

        public List<AreaPointSetData> AreaPointList = new List<AreaPointSetData>();

        public AreaPointSetData GetAreaPoint(uint areaid)
        {
            AreaPointSetData data =AreaPointList.Find(c =>c.AreaId == areaid);
            if(data == null)
            {
                data = new AreaPointSetData()
                {
                    AreaId = areaid
                };

                AreaPointList.Add(data);
            }
            return data;
        }

        public void UpdateAreaPoint(uint areaid, double outmore, double middlespace)
        {
            AreaPointSetData data = GetAreaPoint(areaid);
            data.Out_More_Than_In = outmore;
            data.Middle_Space_M = middlespace;

            GlobalWcsDataConfig.SaveDefaultConfig();
        }

        public void UpdateAreaPointSortQty(uint areaid, ushort setsortqty)
        {
            AreaPointSetData data = GetAreaPoint(areaid);
            data.Set_Out_Sort_Qty = setsortqty;

            GlobalWcsDataConfig.SaveDefaultConfig();
        }


        #endregion
    }

    public class AreaDefaultData
    {
        public uint AreaId { set; get; }
        public ushort Last_Good_Qty { set; get; } = 50;
        public uint Last_Good_SizeId { set; get; }
        public byte Last_Good_Level { set; get; }
    }

    public class AreaPointSetData
    {
        public uint AreaId { set; get; }
        public double Out_More_Than_In { set; get; } = 1;
        public double Middle_Space_M { set; get; } = 4.7;
        public ushort Set_Out_Sort_Qty { set; get; }
    }
}
