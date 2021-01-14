using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class GoodSizePack
    {
        public List<GoodSize> GoodSizeList { set; get; }

        public void AddGoodSizeList(List<GoodSize> list)
        {
            if (GoodSizeList == null)
            {
                GoodSizeList = new List<GoodSize>();
            }

            GoodSizeList.AddRange(list);
        }
    }
}