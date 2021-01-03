using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class TransPack
    {

        public List<StockTrans> TransList { set; get; }

        public void AddTransList(List<StockTrans> list)
        {
            if (TransList == null)
            {
                TransList = new List<StockTrans>();
            }
            TransList.AddRange(list);
        }
    }
}
