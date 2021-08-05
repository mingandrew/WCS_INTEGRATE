using module.goods;
using System.Collections.Generic;
namespace module.rf
{
    public class StockSumStocksPack
    {
        public List<StockSumStocks> StockSumStocksList { get; set; }
        public void AddStockSumStocksList(List<StockSumStocks> list)
        {
            if (StockSumStocksList == null)
            {
                StockSumStocksList = new List<StockSumStocks>();
            }
            StockSumStocksList.AddRange(list);
        }

    }
}