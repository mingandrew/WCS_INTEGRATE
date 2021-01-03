using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class GoodsPack
    {
        public List<Goods> GoodsList { set; get; }

        public void AddGoodList(List<Goods> list)
        {
            if(GoodsList == null)
            {
                GoodsList = new List<Goods>();
            }
            
            GoodsList.AddRange(list);
        }
    }
}
