using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.diction;
using module.goods;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;

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
        private bool isadd, qtyenable, isinsert;
        private short pos; //插入的位置
        private bool isaddbottom = false;

        private byte level;
        private List<DictionDtl> levels;
        private DictionDtl selectlevel;
        #endregion

        #region[属性]

        public List<DictionDtl> LevelList
        {
            get => levels;
            set => Set(ref levels, value);
        }

        public DictionDtl SelectLevel
        {
            get => selectlevel;
            set
            {
                Set(ref selectlevel, value);
                if (selectlevel != null)
                {
                    Level = (byte)selectlevel.int_value;
                }
            }
        }

        public uint GoodsId
        {
            get => goodsid;
            set => Set(ref goodsid, value);
        }

        public byte Level
        {
            get => level;
            set => Set(ref level, value);
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

        public byte Pieces
        {
            get => pieces;
            set => Set(ref pieces, value);
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

        public bool IsAddBottom
        {
            get => isaddbottom;
            set => Set(ref isaddbottom, value);
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
            if (isadd || isinsert)
            {
                if (goodsid == 0)
                {
                    Growl.Warning("品种信息为空");
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

                // 时间判断
                if (!PubMaster.Goods.IsAllowToOperateStock(trackid, goodsid, (DateTime)producetime, Level, out string res))
                {
                    Growl.Warning(res);
                    return;
                }

                if (StockQty <= 0 || StockQty >= 50)
                {
                    Growl.Warning("请输入正确范围的车数!");
                    return;
                }

                if (Pieces<=0 || Pieces >70)
                {
                    Growl.Warning("请输入正确范围的片数!");
                    return;
                }

                //检查轨道是否能够添加对应数量的库存
                if (!PubMaster.Goods.CheckCanAddStockQty(TrackId, GoodsId, StockQty, out int ableqty, out string result))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        Growl.Warning(string.Format("轨道剩余最多能添加：{0} 车", ableqty));
                    }
                    else
                    {
                        Growl.Warning(result);
                    }
                    return;
                }

                if (isinsert)
                {
                    PubMaster.Goods.UpdateStockTrackPos(TrackId, pos, StockQty);
                    PubMaster.Goods.InsertStock(TrackId, GoodsId, pieces, ProduceTime, StockQty, "PC插入库存", out string rrs, pos);
                    Result.p1 = true;
                }


                if (isadd && PubMaster.Goods.AddTrackStocks(0, TrackId, GoodsId, Pieces, ProduceTime, StockQty, Level, "PC添加库存", out string rs, IsAddBottom))
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

        /// <summary>
        /// 添加的尾部
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="tid"></param>
        /// <param name="pis"></param>
        public void SetAddInput(uint gid, uint tid)
        {
            isadd = true;
            isinsert = false;
            ActionTile = "添加";
            GoodsId = gid;
            TrackId = tid;
            QtyEnable = true;
            StockQty = 1;
            ProduceTime = DateTime.Now;

            LevelList = PubMaster.Dic.GetDicDtls(DicTag.GoodLevel);
            LevelList.AddRange(PubMaster.Dic.GetDicDtls(DicTag.GoodSite));
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

            LevelList = PubMaster.Dic.GetDicDtls(DicTag.GoodLevel);
            LevelList.AddRange(PubMaster.Dic.GetDicDtls(DicTag.GoodSite));
            SelectLevel = LevelList.Find(c => c.int_value == stock.level);
        }

        /// <summary>
        /// 设置插入的数据
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="tid"></param>
        /// <param name="pis"></param>
        /// <param name="oldpos"></param>
        public void SetInsertInput(uint gid, uint tid, ushort pis, short oldpos)
        {
            isinsert = true;
            isadd = false;
            ActionTile = "插入";
            GoodsId = gid;
            TrackId = tid;
            pieces = (byte)pis;
            QtyEnable = true;
            StockQty = 1;
            ProduceTime = DateTime.Now;
            pos = oldpos;
        }
        #endregion
    }
}
