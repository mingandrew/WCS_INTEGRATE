using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class StockSumPack
    {
        public List<StockSum> StockSumList { set; get; }

        public void AddSumList(List<StockSum> list)
        {
            if(StockSumList == null)
            {
                StockSumList = new List<StockSum>();
            }
            StockSumList.AddRange(list);
        }

        public bool HaveData()
        {
            return StockSumList != null && StockSumList.Count > 0;
        }
    }
}
