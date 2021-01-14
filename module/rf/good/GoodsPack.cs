using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class GoodsPack
    {
        public List<Goods> GoodsList { set; get; }
        public List<GoodSize> SizeList { set; get; }

        public void AddGoodList(List<Goods> list)
        {
            if(GoodsList == null)
            {
                GoodsList = new List<Goods>();
            }
            
            GoodsList.AddRange(list);
        }
        public void AddGoodSizeList(List<GoodSize> list)
        {
            if (SizeList == null)
            {
                SizeList = new List<GoodSize>();
            }

            SizeList.AddRange(list);
        }
    }
}
