using GalaSoft.MvvmLight;
using module.goods;
using System;

namespace wcs.Data.View
{
    public class StockGoodSumView : ViewModelBase
    {
        public DateTime? produce_time;
        private int count, orgcount;
        private int stack;
        private int pieces;
        private bool selected;
        private bool showcount;
        private bool showlabel = true;

        public uint AreaId { set; get; }
        public uint GoodId { set; get; }
        public string GoodName { set; get; }
        public string Color { set; get; }
        public int Level { set; get; }
        public int Width { set; get; }
        public int Order { set; get; }

        public DateTime? ProduceTime
        {
            get => produce_time;
            set => Set(ref produce_time, value);
        }

        public int Count
        {
            get => count;
            set => Set(ref count, value);
        }

        public int OrgCount
        {
            get => orgcount;
            set => Set(ref orgcount, value);
        }
        public int Stack
        {
            get => stack;
            set => Set(ref stack, value);
        }
        public int Pieces
        {
            get => pieces;
            set => Set(ref pieces, value);
        }

        public bool Selected
        {
            get => selected;
            set => Set(ref selected, value);
        }

        public bool ShowCount
        {
            get => showcount;
            set => Set(ref showcount, value);
        }

        public bool ShowLabel
        {
            get => showlabel;
            set {
                if(Set(ref showlabel, value)){
                    Console.WriteLine("ShowLabel{0}", value);
                }
            }
        }

        public StockGoodSumView(PreStockGood pg)
        {
            Count = pg.pre_good_qty;
            GoodId = pg.good_id;
            orgcount = count;
            Level = pg.level;
        }

        public StockGoodSumView(StockSum sum)
        {
            AreaId = sum.area;
            Count = sum.count;
            Stack = sum.stack;
            Pieces = sum.pieces;
            ProduceTime = sum.produce_time;
            GoodId = sum.goods_id;
            orgcount = count;
        }

        public StockGoodSumView(StockGoodSumView sv)
        {
            AreaId = sv.AreaId;
            Count = sv.count;
            Stack = sv.Stack;
            Pieces = sv.Pieces;
            ProduceTime = sv.ProduceTime;
            GoodId = sv.GoodId;
            orgcount = sv.orgcount;
            ShowCount = sv.ShowCount;
            ShowLabel = sv.ShowLabel;
            Level = sv.Level;
            Color = sv.Color;
            GoodName = sv.GoodName;
        }

        public void AddToSum(StockSum sum)
        {
            Count += sum.count;
            Stack += sum.stack;
            Pieces += sum.pieces;
            if(sum.CompareProduceTime(ProduceTime) <= 0)
            {
                ProduceTime = sum.produce_time;
            }

            orgcount = count;
        }

        public bool IsUseAll()
        {
            return count > orgcount;
        }

        /// <summary>
        /// 是否超过最大值
        /// </summary>
        /// <returns></returns>
        public bool IsOverMax()
        {
            return count > orgcount;
        }

        public void SetSelected(bool v)
        {
            Selected = v;
            if (!v)
            {
                //Count = orgcount + 1;
                //ShowLabel = true;
                //ShowCount = false;
            }
        }

        public void AddSubQty(bool v)
        {
            if(v && count <= orgcount)
            {
                Count++;
            }

            if (v && count > orgcount)
            {
                ShowCount = false;
                Count = orgcount + 1;
            }

            if(!v && count > 1)
            {
                Count--;
                ShowCount = true;
            }
            ShowLabel = !ShowCount;
        }

        public bool EqualGoodAndLevel(uint gid, byte lvl)
        {
            return GoodId == gid && Level == lvl;
        }
    }
}
