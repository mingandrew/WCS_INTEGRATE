using enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using module.goods;
using module.msg;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class StockSumViewModel : MViewModel
    {

        public StockSumViewModel() : base("StockSum")
        {
            List = new ObservableCollection<StockSumView>();
            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.StockSumeUpdate, StockSumeUpdate);

            InitList();

            ListView = System.Windows.Data.CollectionViewSource.GetDefaultView(List);
            ListView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();
        }

        #region[字段]
        private bool showareafilter = true;
        private ObservableCollection<StockSumView> List;
        private IList<MyRadioBtn> _arearadio;

        private uint filterareaid = 0, filtertracktype = 0;
        #endregion

        #region[属性]
        public bool ShowAreaFileter
        {
            get => showareafilter;
            set => Set(ref showareafilter, value);
        }
        public IList<MyRadioBtn> AreaRadio
        {
            get => _arearadio;
            set => Set(ref _arearadio, value);
        }

        public ICollectionView ListView { set; get; }
        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;
        public RelayCommand<string> StockSumActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(StockSumAction)).Value;

        private void StockSumAction(string tag)
        {
            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1://刷新数据
                        InitList();
                        break;
                }
            }
        }

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea())
            {
                ShowAreaFileter = false;
            }
        }
        private void CheckTypeRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint type))
                {
                    filtertracktype = type;
                    ListView.Refresh();
                }
            }
        }
        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint areaid))
                {
                    filterareaid = areaid;
                    ListView.Refresh();
                }
            }
        }
        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filtertracktype == 0) return true;
            if (item is StockSumView sum)
            {
                if (filterareaid == 0)
                {
                    return sum.track_type == filtertracktype;
                }

                if (filtertracktype == 0)
                {
                    return sum.area == filterareaid;
                }

                return sum.area == filterareaid && sum.track_type == filtertracktype;
            }
            return true;
        }


        private void StockSumeUpdate(MsgAction msg)
        {
            if (msg.o1 is StockSum sum && msg.o2 is ActionTypeE type)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StockSumView view = List.FirstOrDefault(c => c.track_id == sum.track_id && c.goods_id == sum.goods_id);
                    if (view == null)
                    {
                        view = new StockSumView(sum);
                        List.Add(view);
                    }
                    view.Update(sum);
                    switch (type)
                    {
                        case ActionTypeE.Add:
                            break;
                        case ActionTypeE.Update:
                            break;
                        case ActionTypeE.Delete:
                            List.Remove(view);
                            break;
                        case ActionTypeE.Finish:
                            break;
                    }
                    //if (List.Count(c=> c.track_id== sum.track_id) <= 1)
                    //{

                    //}
                    //else
                    //{
                    //    InitList();
                    //}

                });
            }
        }

        private void InitList()
        {
            List.Clear();
            List<StockSum> sums = PubMaster.Goods.GetStockSums();
            foreach (StockSum sum in sums)
            {
                List.Add(new StockSumView(sum));
            }
        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);
        }
        #endregion

        protected override void TabActivate()
        {

        }

        protected override void TabDisActivate()
        {

        }
    }
}
