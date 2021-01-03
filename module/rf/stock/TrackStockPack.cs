using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class TrackStockPack
    {
        public uint TrackId { set; get; }
        public List<Stock> Stocks { set; get; }

        public void AddStocks(List<Stock> list)
        {
            if(Stocks == null)
            {
                Stocks = new List<Stock>();
            }

            Stocks.AddRange(list);
        }
    }
}
