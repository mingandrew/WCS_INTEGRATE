﻿using enums;
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

namespace wcs.ViewModel
{
    public class GoodsSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public GoodsSelectViewModel()
        {
            filtername = string.Empty;
            _result = new DialogResult();
            Param = new DialogResult();
            GoodsList = new ObservableCollection<Goods>();
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);

            GoodListView = System.Windows.Data.CollectionViewSource.GetDefaultView(GoodsList);
            GoodListView.Filter = new Predicate<object>(OnFilterMovie);
        }

        #region[字段]

        private DialogResult _result;
        private Goods selectdic;

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

        private ObservableCollection<Goods> GoodsList { set; get; }

        public Goods SelectGood
        {
            get => selectdic;
            set => Set(ref selectdic, value);
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

        #endregion

        #region[方法]
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
            ShowAreaFilter = isshow;
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filterwidth == 0) return true;

            if (item is Goods view)
            {
                return (filterareaid == 0 || filterareaid == view.area_id)
                    && (filterwidth == 0 || filterwidth == view.width)
                    && (string.IsNullOrEmpty(FilterName) || view.name.Contains(FilterName));
            }
            return true;
        }

        public void QueryGood()
        {
            FilterName = string.Empty;
            List<Goods> list = new List<Goods>();

            list.AddRange(PubMaster.Goods.GetGoodsList(filterareaid));
            Application.Current.Dispatcher.Invoke(() =>
            {
                GoodsList.Clear();
                foreach (Goods mod in list)
                {
                    GoodsList.Add(mod);
                }
            });
        }

        public void QueryStockGood()
        {
            FilterName = string.Empty;
            List<Goods> list = new List<Goods>();

            list.AddRange(PubMaster.Goods.GetStockOutGoodsList(filterareaid));
            Application.Current.Dispatcher.Invoke(() =>
            {
                GoodsList.Clear();
                foreach (Goods mod in list)
                {
                    GoodsList.Add(mod);
                }
            });
        }

        private void Comfirm()
        {
            if (SelectGood == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = true;
            Result.p2 = SelectGood;
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
        #endregion
    }
}
