using enums;
using GalaSoft.MvvmLight;
using module.goods;
using resource;
using System;

namespace wcs.Data.View
{
    public class GoodsView : ViewModelBase
    {
        #region[字段]

        private uint id;
        private uint area_id;
        private string name;
        private string color;
        private ushort length;
        private ushort width;
        private bool oversize;
        private byte stack;
        private ushort pieces;
        private string memo;
        private CarrierTypeE carriertype;
        private DateTime? updatetime;
        private ushort minstack;
        private uint sizeid;
        public bool empty { set; get; }
        private byte level;
        #endregion

        #region[属性]
        public uint ID
        {
            get => id;
            set => Set(ref id, value);
        }
        public uint AreaId
        {
            get => area_id;
            set => Set(ref area_id, value);
        }
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        public string Color
        {
            get => color;
            set => Set(ref color, value);
        }

        public ushort Length
        {
            get => length;
            set => Set(ref length, value);
        }

        public ushort Width
        {
            get => width;
            set => Set(ref width, value);
        }

        public byte Stack
        {
            get => stack;
            set => Set(ref stack, value);
        }

        public ushort Pieces
        {
            get => pieces;
            set => Set(ref pieces, value);
        }

        public bool Isoversize
        {
            get => oversize;
            set => Set(ref oversize, value);
        }

        public CarrierTypeE CarrierType
        {
            get => carriertype;
            set => Set(ref carriertype, value);
        }

        public string Memo
        {
            get => memo;
            set => Set(ref memo, value);
        }

        public DateTime? UpdateTime
        {
            get => updatetime;
            set => Set(ref updatetime, value);
        }

        public ushort MinStack
        {
            get => minstack;
            set => Set(ref minstack, value);
        }

        public uint SizeId
        {
            get => sizeid;
            set => Set(ref sizeid, value);
        }

        public byte Level
        {
            get => level;
            set => Set(ref level, value);
        }

        #endregion
        public GoodsView(Goods goods)
        {
            Update(goods);
        }

        #region[更新]

        public void Update(Goods goods)
        {
            ID = goods.id;
            Name = goods.name;
            Color = goods.color;
            Level = goods.level;
            if(sizeid != goods.size_id)
            {
                SizeId = goods.size_id;
                GoodSize size = PubMaster.Goods.GetSize(goods.size_id);
                if (size != null)
                {
                    Length = size.length;
                    Width = size.width;
                    Isoversize = size.oversize;
                    Stack = size.stack;
                }
            }
            Pieces = goods.pieces;
            Memo = goods.memo;
            AreaId = goods.area_id;
            CarrierType = goods.GoodCarrierType;
            MinStack = goods.minstack;
            empty = goods.empty;
        }

        #endregion
    }
}
