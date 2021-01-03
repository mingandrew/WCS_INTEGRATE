using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.window;
using resource;
using System;

namespace wcs.ViewModel
{
    public class StockEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public StockEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
        }

        #region[字段]

        private DialogResult _result;

        private uint goodsid,trackid;
        private DateTime? producetime;
        private byte pieces;
        private byte stockqty = 0;
        private Stock oldstock;
        private string actiontitle;
        private bool isadd, qtyenable;
        #endregion

        #region[属性]
        
        public uint GoodsId
        {
            get => goodsid;
            set => Set(ref goodsid, value);
        }
        
        public uint TrackId
        {
            get => trackid;
            set => Set(ref trackid, value);
        }

        public byte StockQty
        {
            get => stockqty;
            set => Set(ref stockqty, value);
        }

        public DateTime? ProduceTime
        {
            get => producetime;
            set => Set(ref producetime, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public DialogResult Param
        {
            set; get;
        }

        public Action CloseAction { get; set; }

        public string ActionTile
        {
            get => actiontitle;
            set => Set(ref actiontitle, value);
        }

        public bool QtyEnable
        {
            get => qtyenable;
            set => Set(ref qtyenable, value);
        }
        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]

        private void Comfirm()
        {
            Result.p1 = false;
            if (isadd)
            {
                if (goodsid == 0)
                {
                    Growl.Warning("规格信息为空");
                }

                if (trackid == 0)
                {
                    Growl.Warning("轨道信息为空!");
                }

                if (producetime is null)
                {
                    Growl.Warning("选择生产时间");
                    return;
                }

                if (StockQty == 0 || StockQty > 50)
                {
                    Growl.Warning("请输入正确范围的数量!");
                    return;
                }

                if (PubMaster.Goods.AddTrackStocks(0, TrackId, GoodsId, pieces, ProduceTime, StockQty, "PC添加库存"))
                {
                    Result.p1 = true;
                }
            }
            else
            {
                if(ProduceTime is DateTime time && oldstock.produce_time is DateTime otime)
                {
                    if((time - otime).TotalSeconds == 0)
                    {
                        Growl.Warning("没有修改信息");
                        return;
                    }

                    PubMaster.Goods.UpdateStockProTime(oldstock.id, ProduceTime);
                    Result.p1 = true;
                }
            }

            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }

        public void SetAddInput(uint gid, uint tid, ushort pis)
        {
            isadd = true;
            ActionTile = "添加";
            GoodsId = gid;
            TrackId = tid;
            pieces = (byte)pis;
            QtyEnable = true;
            StockQty = 1;
            ProduceTime = DateTime.Now;
        }

        public void SetEditInput(Stock stock)
        {
            ActionTile = "修改";
            isadd = false;
            GoodsId = stock.goods_id;
            TrackId = stock.track_id;
            ProduceTime = stock.produce_time;
            QtyEnable = false;
            StockQty = 1;
            oldstock = stock;
        }
        #endregion
    }
}
