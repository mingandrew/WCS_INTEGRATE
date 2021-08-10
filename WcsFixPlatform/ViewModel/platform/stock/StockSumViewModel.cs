using CommonServiceLocator;
using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using HandyControl.Controls;
using Microsoft.Win32;
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
using tool.excel;
using wcs.Data.View;
using wcs.window;

namespace wcs.ViewModel
{
    public class StockSumViewModel : MViewModel
    {

        public StockSumViewModel() : base("StockSum")
        {
            STARTDATE = DateTime.Now;
            STOPDATE = DateTime.Now;

            GoodSumList = new ObservableCollection<StockSumView>();
            TrackSumList = new ObservableCollection<StockSumView>();

            TrackStockSearchList = new ObservableCollection<StockSumView>();
            ConsumeStockSearchList = new ObservableCollection<StockSumView>();

            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.StockSumeUpdate, StockSumeUpdate);

            InitList();

            GoodSumListView = System.Windows.Data.CollectionViewSource.GetDefaultView(GoodSumList);

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
        private uint filterareaid = 0;
        private byte filtertracktype = 0;
        private ushort filterlineid = 0;
        private StockSumView trackselected;

        #region[轨道时间过滤]

        private DateTime lastquerytime;
        private DateTime? startdate;
        private DateTime? stopdate;

        #endregion
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

        /// <summary>
        /// 当前选项卡名称
        /// </summary>
        private string Now_Tag_Name { set; get; }

        public StockSumView TrackSelected
        {
            get => trackselected;
            set => Set(ref trackselected, value);
        }

        public ICollectionView GoodSumListView { set; get; }
        public ICollectionView TrackSumListView { set; get; }

        public ObservableCollection<StockSumView> TrackStockSearchList { set; get; }
        public ObservableCollection<StockSumView> ConsumeStockSearchList { set; get; }

        #region[轨道过滤]


        public DateTime? STARTDATE
        {
            get => startdate;
            set => Set(ref startdate, value);
        }

        public DateTime? STOPDATE
        {
            get => stopdate;
            set => Set(ref stopdate, value);
        }

        #endregion
        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;
        public RelayCommand<string> StockSumActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(StockSumAction)).Value;
        public RelayCommand<StockSumView> ChangeGoodCmd => new Lazy<RelayCommand<StockSumView>>(() => new RelayCommand<StockSumView>(ChangeGood)).Value;

        public RelayCommand ExportSumViewCmd => new Lazy<RelayCommand>(() => new RelayCommand(ExportSumView)).Value;

        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;

        public RelayCommand<string> SetFilterCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SetFilter)).Value;

        private void SetFilter(string tag)
        {
            if ((DateTime.Now - lastquerytime).TotalMilliseconds < 800)
            {
                Growl.Warning("请不要频繁刷新!");
                return;
            }

            lastquerytime = DateTime.Now;

            if (STARTDATE == null)
            {
                Growl.Warning("请选择开始时间");
                return;
            }

            if (STOPDATE == null)
            {
                Growl.Warning("请选择结束时间");
                return;
            }

            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1:
                        break;

                    case 2://今天8-18
                        DateTime today = DateTime.Now;
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, 6, 00, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 18, 00, 000);
                        break;

                    case 3://昨天18-今天6
                        today = DateTime.Now;
                        today = today.AddDays(-1);
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, 18, 00, 00);

                        today = DateTime.Now;
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 06, 00, 00);
                        break;
                }
            }

            if (STARTDATE is DateTime start && STOPDATE is DateTime stop)
            {
                if ((stop - start).TotalDays > 60)
                {
                    Growl.Warning("查询日期不能超过两周");
                    return;
                }
            }

            switch (Now_Tag_Name)
            {
                case "inlog":
                    QueryTrackInLog();
                    break;
                case "outlog":
                    QueryTrackOutLog();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region[方法]

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

        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                ShowAreaFileter = false;
                filterareaid = areaid;
                filterlineid = lineid;
            }
        }

        private void CheckTypeRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (byte.TryParse(btn.Tag.ToString(), out byte type))
                {
                    filtertracktype = type;
                    TrackSumListView.Refresh();
                    UpdateGoodsSum();
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
                UpdateGoodsSum();
            }
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filtertracktype == 0) return true;
            if (item is StockSumView sum)
            {
                if (filterareaid == 0)
                {
                    return IsShow(sum);
                }

                if (filtertracktype == 0)
                {
                    return sum.area == filterareaid && sum.line == filterlineid;
                }

                return sum.area == filterareaid && filterlineid == sum.line && IsShow(sum);
            }
            return true;
        }

        private void StockSumeUpdate(MsgAction msg)
        {
            if (msg.o1 is StockSum sum && msg.o2 is ActionTypeE type)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StockSumView view = TrackSumList.FirstOrDefault(c => c.track_id == sum.track_id && c.GoodId == sum.goods_id);
                    if (view == null)
                    {
                        view = new StockSumView(sum);
                        TrackSumList.Add(view);
                    }

                    bool isUpdateType2 = view.track_type2 != sum.track_type2;
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

                    UpdateGoodsSum();

                    if (isUpdateType2)
                    {
                        if (TrackSumListView != null) TrackSumListView.Refresh();
                        if (GoodSumListView != null) GoodSumListView.Refresh();
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

        /// <summary>
        /// 是否显示（区分出库/入库）
        /// </summary>
        /// <param name="sum"></param>
        /// <returns></returns>
        private bool IsShow(StockSumView sum)
        {
            bool IsShow = true;

            switch ((TrackTypeE)filtertracktype)
            {
                case TrackTypeE.储砖_入:
                    IsShow = sum.IsWorkIn();
                    break;
                case TrackTypeE.储砖_出:
                    IsShow = sum.IsWorkOut();
                    break;
            }

            return IsShow;
        }

        /// <summary>
        /// 更新品种统计
        /// </summary>
        private void UpdateGoodsSum()
        {
            GoodSumList.Clear();

            List<StockSum> sumlist = new List<StockSum>();

            if (TrackSumList != null && TrackSumList.Count > 0)
            {
                foreach (StockSumView item in TrackSumList)
                {
                    bool isadd = false;

                    if (filterareaid == 0)
                    {
                        isadd = IsShow(item);
                    }
                    else if (filtertracktype == 0)
                    {
                        isadd = item.area == filterareaid && item.line == filterlineid;
                    }
                    else
                    {
                        isadd = item.area == filterareaid && filterlineid == item.line && IsShow(item);
                    }

                    if (isadd) sumlist.Add(item.GetStockSum());
                }
            }

            if (sumlist != null && sumlist.Count > 0)
            {
                var list = sumlist.GroupBy(c => new { c.goods_id, c.sum_level }).Select(c => new StockSum
                {
                    goods_id = c.Key.goods_id,
                    count = c.Sum(b => b.count),
                    stack = c.Sum(b => b.stack),
                    pieces = c.Sum(b => b.pieces),
                    produce_time = c.Min(b => b.produce_time),
                    last_produce_time = c.Min(b => b.last_produce_time),
                    sum_level = c.Key.sum_level,
                });

                foreach (StockSum sum in list.ToList())
                {
                    GoodSumList.Add(new StockSumView(sum));
                }
            }
        }

        private void InitList()
        {
            TrackSumList.Clear();
            List<StockSum> sums = PubMaster.Sums.GetStockSums();
            foreach (StockSum sum in sums)
            {
                TrackSumList.Add(new StockSumView(sum));
            }

            UpdateGoodsSum();
        }

        public void ShowGoodsOrTrack(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                Now_Tag_Name = btn.Tag.ToString();
            }
        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);
        }

        private void ExportSumView()
        {
            string tile = string.Empty;
            switch (Now_Tag_Name)
            {
                case "goods":
                    tile = "品种分类-库存统计-";
                    break;
                case "track":
                    tile = "轨道分类-库存统计-";
                    break;
                case "inlog":
                    tile = "库存统计-";
                    break;
                case "outlog":
                    tile = "出库记录统计-";
                    break;
                default:
                    tile = "未命名统计-";
                    break;
            }
            SaveFileDialog saveFileDialog = ExcelTool.createSaveFileDialog(tile);

            bool? result = saveFileDialog.ShowDialog();
            if (result == null || result == false)
            {
                return;
            }
            string sql = string.Empty;
            switch (Now_Tag_Name)
            {
                case "goods":
                    sql = "select `g`.`info` AS `品种`,min(`t`.`produce_time`) AS `生产时间`,count(`t`.`id`) AS `车数`,sum(`t`.`stack`) AS `垛数`,sum(`t`.`pieces`) AS `片数`,`a`.`name` AS `区域`," +
                    "(select `line`.`name` from (`track` join `line`) where ((`track`.`id` = `t`.`track_id`) and (`track`.`line` = `line`.`line`))) AS `线` " +
                    "from (((`stock` `t` join `goods` `g`) join `track` `tt`) join `area` `a`) " +
                    "where ((`t`.`track_type` in (2,3,4)) and (`t`.`goods_id` = `g`.`id`) and (`t`.`track_id` = `tt`.`id`) and (`a`.`id` = `t`.`area`)) " +
                    "group by `t`.`goods_id` " +
                    "order by `t`.`area`,`t`.`goods_id`,`t`.`produce_time`";
                    break;
                case "track":
                    sql = "select `tt`.`name` AS `轨道`,`g`.`info` AS `品种`,min(`t`.`produce_time`) AS `生产时间`,count(`t`.`id`) AS `车数`,sum(`t`.`stack`) AS `垛数`,sum(`t`.`pieces`) AS `片数`," +
                    "`a`.`name` AS `区域`,(select `line`.`name` from (`track` join `line`) where ((`track`.`id` = `t`.`track_id`) and (`track`.`line` = `line`.`line`))) AS `线` " +
                    "from (((`stock` `t` join `goods` `g`) join `track` `tt`) join `area` `a`) " +
                    "where ((`t`.`track_type` in (2,3,4)) and (`t`.`goods_id` = `g`.`id`) and (`t`.`track_id` = `tt`.`id`) and (`a`.`id` = `t`.`area`)) " +
                    "group by `t`.`goods_id`,`t`.`track_id` " +
                    "order by `t`.`area`,`t`.`goods_id`,`t`.`produce_time`,`t`.`track_id`";
                    break;
                case "inlog":
                    sql = PubMaster.Mod.GoodSql.GetQueryStockLogSumSql(filterareaid, filterlineid, STARTDATE, STOPDATE);
                    break;

                case "outlog":
                    sql = PubMaster.Mod.GoodSql.GetQueryConsumeLogSumList(filterareaid, filterlineid, STARTDATE, STOPDATE);
                    break;
            }

            if (!string.IsNullOrEmpty(sql))
            {
                PubMaster.Mod.ExcelConfigSql.SaveToExcel(saveFileDialog, sql);
            }
            else
            {
                Growl.Warning("未配置导出语句！");
            }
        }

        #endregion

        #region[Tab切换]

        private void TabSelected(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is HandyControl.Controls.TabControl pro && pro.SelectedItem is HandyControl.Controls.TabItem tabitem)
            {
                Now_Tag_Name = tabitem.Tag.ToString();
            }
        }

        #endregion

        protected override void TabActivate()
        {

        }

        protected override void TabDisActivate()
        {

        }

        #region[库存查询]

        /// <summary>
        /// 当前库存查询
        /// </summary>
        private void QueryTrackInLog()
        {
            List<StockSum>  list = PubMaster.Mod.GoodSql.QueryStockLogSumList(filterareaid, filterlineid, STARTDATE, STOPDATE);
            TrackStockSearchList.Clear();

            foreach (var item in list)
            {
                TrackStockSearchList.Add(new StockSumView(item));
            }
        }

        /// <summary>
        /// 出库库存记录
        /// </summary>
        private void QueryTrackOutLog()
        {
            List<StockSum> list = PubMaster.Mod.GoodSql.QueryConsumeLogSumList(filterareaid, filterlineid, STARTDATE, STOPDATE);
            ConsumeStockSearchList.Clear();

            foreach (var item in list)
            {
                ConsumeStockSearchList.Add(new StockSumView(item));
            }
        }


        #endregion
    }
}
