using module.area;
using System.Collections.Generic;

namespace module.rf
{
    public class RfFilterDataPack
    {
        public List<Area> AreaList { set; get; }
        public int FilterType { set; get; } //0 全部 1 上砖测 2 下砖测
        public int FilterArea { set; get; } //0 全部 其他：区域ID

        public void AddAreaList(List<Area> list)
        {
            if(AreaList == null)
            {
                AreaList = new List<Area>();
            }
            AreaList.AddRange(list) ;
        }
    }
}
