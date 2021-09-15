using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tool.appconfig
{
    public class TileBindGoodsConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\TileBindGoodsConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";


        public List<TileBindGood> TileBindingGoodsList { set; get; } = new List<TileBindGood>();
        public uint GetTileBindingGoods(uint devid)
        {
            if (devid == 0) { return 0; }

            TileBindGood TileBGoods = TileBindingGoodsList.Find(c => c.Tile_id == devid);
            
            if (TileBGoods == null)
            {
                TileBGoods = new TileBindGood()
                {
                    Tile_id = devid,
                    Goods_id = 0
                };
                TileBindingGoodsList.Add(TileBGoods);
                GlobalWcsDataConfig.SaveTileBindingGoodsConfig();
            }
            
            return TileBGoods.Goods_id;
        }
        public void SetTileBindingGoods(uint devid, uint goodsid)
        {
             TileBindGood TileBGoods = TileBindingGoodsList.Find(c => c.Tile_id == devid);
            
            if (TileBGoods == null)
            {
                TileBGoods = new TileBindGood()
                {
                    Tile_id = devid,
                    Goods_id = goodsid
                };
                TileBindingGoodsList.Add(TileBGoods);
            }
            else {
                TileBGoods.Tile_id = devid;
                TileBGoods.Goods_id = goodsid;
               
            }           
            GlobalWcsDataConfig.SaveTileBindingGoodsConfig();
        }
    }

    public class TileBindGood
    {
        public uint Tile_id { set; get; }
        public uint Goods_id { set; get; }
    }


}
