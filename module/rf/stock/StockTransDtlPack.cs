using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class StockTransDtlPack
    {
        public List<StockTransDtl> sorttranslist { get; set; }

        public void AddList(List<StockTransDtl> list)
        {
            if (sorttranslist == null)
            {
                sorttranslist = new List<StockTransDtl>();
            }
            sorttranslist.AddRange(list);
        }
    }
}
