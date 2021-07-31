using enums.track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace module.goods
{
    public class StockSumStocks : StockSum
    {
        public StockSumStocks(Stock stock, byte tt2)
        {
            goods_id = stock.goods_id;
            count = 1;
            stack = stock.stack;
            pieces = stock.pieces;
            produce_time = stock.produce_time;
            track_id = stock.track_id;
            area = stock.area;
            track_type = stock.track_type;
            track_type2 = tt2;
        }

        public List<uint> StockIds { set; get; }
        public void AddStockId(uint id)
        {
            if (StockIds == null)
            {
                StockIds = new List<uint>();
            }

            StockIds.Add(id);
        }

        public bool AddToSum(Stock stock)
        {
            if(stock == null || stock.goods_id != goods_id)
            {
                return false;
            }
            stack += stock.stack;
            pieces += stock.pieces;
            count++;
            CompareSetTime(stock.produce_time);
            AddStockId(stock.id);
            return true;
        }

        public void CompareSetTime(DateTime? time)
        {
            if (produce_time is DateTime dtime && time is DateTime ctime)
            {
                if (dtime.CompareTo(ctime) < 0)
                {
                    produce_time = time;
                }
            }
        }
    }
}
