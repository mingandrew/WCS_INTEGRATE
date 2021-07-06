using CommonServiceLocator;
using enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using module.goods;
using module.msg;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using wcs.Data.View;
using wcs.window;

namespace wcs.ViewModel
{
    public class StockSumViewModel : MViewModel
    {

        public StockSumViewModel() : base("StockSum")
        {
            GoodSumList = new ObservableCollection<StockSumView>();
            TrackSumList = new ObservableCollection<StockSumView>();

            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.StockSumeUpdate, StockSumeUpdate);
            Messenger.Default.Register<List<StockSum>>(this, MsgToken.GoodSumUpdate, GoodSumUpdate);

            InitList();

            GoodSumListView = System.Windows.Data.CollectionViewSource.GetDefaultView(GoodSumList);
            //GoodSumListView.Filter = new Predicate<object>(OnFilterMovie);
            TrackSumListView = System.Windows.Data.CollectionViewSource.GetDefaultView(TrackSumList);
            TrackSumListView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();
        }

        #region[字段]
        private bool showareafilter = true;
        private ObservableCollection<StockSumView> GoodSumList;
        private ObservableCollection<StockSumView> TrackSumList;

        private IList<MyRadioBtn> _arearadio;
        private bool showtrack = false;
        private uint filterareaid = 0, filtertracktype = 0;
        private ushort filterlineid = 0;
        private StockSumView trackselected;

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

        public bool ShowTrack
        {
            get => showtrack;
            set => Set(ref showtrack, value);
        }

        public StockSumView TrackSelected
        {
            get => trackselected;
            set => Set(ref trackselected, value);
        }

        public ICollectionView GoodSumListView { set; get; }
        public ICollectionView TrackSumListView { set; get; }

        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;
        public RelayCommand<string> StockSumActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(StockSumAction)).Value;
        public RelayCommand<RoutedEventArgs> ShowGoodsOrTrackCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(ShowGoodsOrTrack)).Value;
        public RelayCommand<StockSumView> ChangeGoodCmd => new Lazy<RelayCommand<StockSumView>>(() => new RelayCommand<StockSumView>(ChangeGood)).Value;

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
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                ShowAreaFileter = false;
                filterareaid = areaid;

            }
        }
        private void CheckTypeRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint type))
                {
                    filtertracktype = type;
                    TrackSumListView.Refresh();
                    PubMaster.Goods.GetGoodCountList(filterareaid, filtertracktype);
                }
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn && btn.DataContext is MyRadioBtn radio)
            {
                filterareaid = radio.AreaID;
                filterlineid = radio.Line;
                TrackSumListView.Refresh();
                GoodSumListView.Refresh();
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

                if (filtertracktype == 0)
                {
                    return sum.area == filterareaid && sum.line == filterlineid;
                }


                return sum.area == filterareaid && filterlineid == sum.line && sum.track_type == filtertracktype;
            }
            return true;
        }


        private void StockSumeUpdate(MsgAction msg)
        {
            if (msg.o1 is StockSum sum && msg.o2 is ActionTypeE type)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StockSumView view = TrackSumList.FirstOrDefault(c => c.track_id == sum.track_id && c.goods_id == sum.goods_id);
                    if (view == null)
                    {
                        view = new StockSumView(sum);
                        TrackSumList.Add(view);
                    }
                    view.Update(sum);
                    switch (type)
                    {
                        case ActionTypeE.Add:
                            break;
                        case ActionTypeE.Update:

                            break;
                        case ActionTypeE.Delete:
                            TrackSumList.Remove(view);
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

        private void GoodSumUpdate(List<StockSum> list)
        {
            if (list == null) return;
            if (list is List<StockSum> goodcountlist)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    GoodSumList.Clear();
                    foreach (StockSum sum in goodcountlist)
                    {
                        GoodSumList.Add(new StockSumView(sum));
                    }
                });
            }

        }

        private void ChangeGood(StockSumView sum)
        {
            if (sum != null)
            {
                Messenger.Default.Send("Stock", MsgToken.ActiveTab);
                Messenger.Default.Send(sum.track_id, MsgToken.SetStockSelectTrack);
            }
        }

        private void InitList()
        {
            TrackSumList.Clear();
            List<StockSum> sums = PubMaster.Goods.GetStockSums();
            foreach (StockSum sum in sums)
            {
                TrackSumList.Add(new StockSumView(sum));
            }
            GoodSumList.Clear();
            PubMaster.Goods.GetGoodCountList(filterareaid, filtertracktype);

        }

        public void ShowGoodsOrTrack(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                switch (btn.Tag)
                {
                    case "goods":
                        ShowTrack = false;
                        break;
                    case "track":
                        ShowTrack = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);
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
