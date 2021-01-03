using System.Collections.Generic;

namespace module.rf
{
    public class StockGoodIdsPack
    {
        public List<StockGoodPack> GoodIdsList { set; get; }
        public void AddIds(List<StockGoodPack> ids)
        {
            if(GoodIdsList == null)
            {
                GoodIdsList = new List<StockGoodPack>();
            }

            //foreach (StockGoodPack item in ids)
            //{
            //    GoodIdsList.Add(item);
            //    if (item.Area == 1)
            //    {
            //        GoodIdsList.Add(new StockGoodPack()
            //        {
            //            Area = 2,
            //            GoodsId = item.GoodsId,
            //        });
            //    }

            //    if(item.Area == 2)
            //    {
            //        GoodIdsList.Add(new StockGoodPack()
            //        {
            //            Area = 1,
            //            GoodsId = item.GoodsId
            //        });
            //    }
            //}
            GoodIdsList.AddRange(ids);
        }
    }
}
