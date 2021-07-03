using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class StockSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public StockSelectViewModel()
        {
            filtername = string.Empty;
            _result = new DialogResult();
            Param = new DialogResult();
            StockList = new ObservableCollection<StockGoodSumView>();
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);

            GoodListView = System.Windows.Data.CollectionViewSource.GetDefaultView(StockList);
            GoodListView.Filter = new Predicate<object>(OnFilterMovie);
        }

        #region[字段]

        private DialogResult _result;
        private StockGoodSumView selectstock, lastselectstock;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0, filterwidth = 0;
        private bool showareafilter;
        private string filtername;
        #endregion

        #region[属性]

        public IList<MyRadioBtn> AreaRadio
        {
            get => _arearadio;
            set => Set(ref _arearadio, value);
        }
        public ICollectionView GoodListView { set; get; }

        private ObservableCollection<StockGoodSumView> StockList { set; get; }

        public StockGoodSumView SelectStock
        {
            get => selectstock;
            set
            {
                selectstock?.SetSelected(false);
                Set(ref selectstock, value);
                selectstock?.SetSelected(true);
            }
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

        public bool FilterArea { set; get; }
        public uint AreaId { set; get; }
        public bool ShowAreaFilter
        {
            get => showareafilter;
            set => Set(ref showareafilter, value);
        }
        public string FilterName
        {
            get => filtername;
            set => Set(ref filtername, value);
        }
        #endregion

        #region[命令]        
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        public RelayCommand SearchCmd => new Lazy<RelayCommand>(() => new RelayCommand(Search)).Value;

        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> CheckWidthRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckWidthRadioBtn)).Value;

        public RelayCommand<StockGoodSumView> QtyAddCmd => new Lazy<RelayCommand<StockGoodSumView>>(() => new RelayCommand<StockGoodSumView>(StockQtyAdd)).Value;
        public RelayCommand<StockGoodSumView> QtySubCmd => new Lazy<RelayCommand<StockGoodSumView>>(() => new RelayCommand<StockGoodSumView>(StockQtySub)).Value;
        #endregion

        #region[方法]
        private void Clear()
        {
            lastselectstock = null;
            selectstock = null;
        }
        private void Search()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GoodListView.Refresh();
            });
        }

        public void SetAreaFilter(uint areaid, bool isshow)
        {
            filterareaid = areaid;
            if(PubMaster.Area.IsSingleArea(out uint aid))
            {
                showareafilter = false;
            }
            else
            {
                ShowAreaFilter = isshow;
            }
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filterwidth == 0) return true;

            if (item is StockGoodSumView view)
            {
                if (filterareaid == 0 || filterareaid == view.AreaId) return true;

                return (filterareaid == 0 || filterareaid == view.AreaId)
                    && (filterwidth == 0 || filterwidth == view.Width)
                    && (string.IsNullOrEmpty(FilterName) || view.GoodName.Contains(FilterName));
            }
            return true;
        }

        public void QueryStockGood(bool isonlyupgood = false)
        {
            Clear();
            FilterName = string.Empty;
            List<StockSum> sums = PubMaster.Goods.GetStockSums();
            List<StockGoodSumView> goodsums = new List<StockGoodSumView>();
            foreach (var item in sums)
            {
                StockGoodSumView sum = goodsums.Find(c => c.GoodId == item.goods_id);
                if (sum != null)
                {
                    sum.AddToSum(item);
                }
                else
                {
                    if (isonlyupgood)
                    {
                        if(item.TrackType != TrackTypeE.储砖_出
                            && item.TrackType != TrackTypeE.储砖_出入)
                        {
                            continue;
                        }
                    }
                    sum = new StockGoodSumView(item);
                    Goods goods = PubMaster.Goods.GetGoods(item.goods_id);
                    if (goods != null)
                    {
                        sum.GoodName = goods.name;
                        sum.Color = goods.color;
                        sum.Level = goods.level;
                        sum.Width = PubMaster.Goods.GetSizeWidth(goods.size_id);
                    }
                    goodsums.Add(sum);
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                StockList.Clear();
                foreach (StockGoodSumView mod in goodsums)
                {
                    mod.Count++;
                    StockList.Add(mod);
                }
            });
        }

        private void Comfirm()
        {
            if (SelectStock == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            if (SelectStock.Count == 0)
            {
                Growl.Warning(string.Format("所选品种【 {0} 】数量不能为0！", SelectStock.GoodName, SelectStock.OrgCount));
                return;
            }
            Result.p1 = true;
            Result.p2 = SelectStock;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            Result.p2 = null;
            CloseAction?.Invoke();
        }


        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint areaid))
                {
                    filterareaid = areaid;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GoodListView.Refresh();
                    });
                }
            }
        }

        private void CheckWidthRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint width))
                {
                    filterwidth = width;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GoodListView.Refresh();
                    });
                }
            }
        }

        private void StockQtyAdd(StockGoodSumView view)
        {
            view.AddSubQty(true);
        }
        private void StockQtySub(StockGoodSumView view)
        {
            view.AddSubQty(false);
        }
        #endregion
    }
}
