using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using module.device;
using module.goods;
using module.msg;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using tool.appconfig;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class TrackViewModel : MViewModel
    {
        public TrackViewModel() : base("Track")
        {
            _tracklist = new ObservableCollection<TrackView>();
            _out_tracklist = new ObservableCollection<TrackView>();
            InitAreaRadio();

            CheckIsSingle();

            UseInOutLeftViewMaxId = PubMaster.Track.GetMidId(filterareaid, filterlineid);

            Messenger.Default.Register<MsgAction>(this, MsgToken.TrackStatusUpdate, TrackStatusUpdate);
            Messenger.Default.Register<uint>(this, MsgToken.TrackStockQtyUpdate, TrackStockQtyUpdate);

            InitTrask();

            TrackView = System.Windows.Data.CollectionViewSource.GetDefaultView(TrackList);
            TrackView.Filter = new Predicate<object>(OnFilterMovie);

            OutTrackView = System.Windows.Data.CollectionViewSource.GetDefaultView(OutTrackList);
            OutTrackView.Filter = new Predicate<object>(OnFilterMovie);

        }

        /// <summary>
        /// 是否显示（区分出库/入库）
        /// </summary>
        /// <param name="sum"></param>
        /// <returns></returns>
        private bool IsShow(TrackView view)
        {
            bool IsShow = true;
            TrackTypeE tt = (TrackTypeE)filtertracktype;
            if (view.Type == TrackTypeE.储砖_出入)
            {
                switch (tt)
                {
                    case TrackTypeE.储砖_入:
                        IsShow = view.IsWorkIn();
                        break;
                    case TrackTypeE.储砖_出:
                        IsShow = view.IsWorkOut();
                        break;
                }
            }
            else
            {
                IsShow = view.Type == tt;
            }

            return IsShow;
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filtertracktype == 0) return true;
            if (item is TrackView view)
            {
                if (filterareaid == 0)
                {
                    return IsShow(view);
                }

                if (filtertracktype == 0)
                {
                    return view.Area == filterareaid && view.LineId == filterlineid;
                }

                return filterareaid == view.Area && filterlineid == view.LineId && IsShow(view);
            }
            return true;
        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);
            //filterareaid = AreaRadio[0]?.AreaID ?? 0;
            //filterlineid = AreaRadio[0]?.Line ?? 0;
        }

        #region[字段]
        private uint UseInOutLeftViewMaxId = 0;
        private bool showareafilter = true;
        private ObservableCollection<TrackView> _tracklist;
        private ObservableCollection<TrackView> _out_tracklist;
        private TrackView _trackselected, _outtrackselected;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0, filtertracktype = 0;
        private ushort filterlineid = 0;
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

        public ICollectionView TrackView { set; get; }
        public ICollectionView OutTrackView { set; get; }

        public ObservableCollection<TrackView> TrackList
        {
            get => _tracklist;
            set => Set(ref _tracklist, value);
        }

        public ObservableCollection<TrackView> OutTrackList
        {
            get => _out_tracklist;
            set => Set(ref _out_tracklist, value);
        }

        public TrackView TrackSelected
        {
            get => _trackselected;
            set => Set(ref _trackselected, value);
        }

        public TrackView OutTrackSelected
        {
            get => _outtrackselected;
            set => Set(ref _outtrackselected, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;
        public RelayCommand<string> TrackUpdateCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TrackUpdate)).Value;

        private void TrackUpdate(string tag)
        {
            string[] tags = tag.Split(':');
            if(tags != null && tags.Length > 1 && int.TryParse(tags[0], out int type) && int.TryParse(tags[1], out int value))
            {
                if (!GetSelectTrackId(value, out uint trackid)) return;

                switch (type)
                {
                    case 1://启用
                        if(!PubMaster.Track.SetTrackStatus(trackid, TrackStatusE.启用, out string result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");

                        break;
                    case 2://停用
                        if (!PubMaster.Track.SetTrackStatus(trackid, TrackStatusE.停用, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");
                        break;
                    case 3://空砖
                        if (!PubMaster.Track.SetStockStatus(trackid, TrackStockStatusE.空砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        break;
                    case 4://有砖
                        if (!PubMaster.Track.SetStockStatus(trackid, TrackStockStatusE.有砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        break;
                    case 5://满砖
                        if (!PubMaster.Track.SetStockStatus(trackid, TrackStockStatusE.满砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        break;
                    case 6://仅上砖
                        if (!PubMaster.Track.SetTrackStatus(trackid, TrackStatusE.仅上砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");
                        break;
                    case 7://仅下砖
                        if (!PubMaster.Track.SetTrackStatus(trackid, TrackStatusE.仅下砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");
                        break;
                    //case 8://清空优先
                    //    if (TrackSelected.Status == TrackStockStatusE.空砖)
                    //    {
                    //        Growl.Warning("空砖无法优先！");
                    //        return;
                    //    }
                    //    foreach (Device d in PubMaster.Device.GetTileLifters(TrackSelected.Area).FindAll(c=>c.Type == DeviceTypeE.上砖机))
                    //    {
                    //        PubMaster.Device.SetCurrentTake(d.id, TrackSelected.Id);
                    //    }
                    //    Growl.Success("修改成功！");
                    //    break;
                }
            }
        }

        public bool GetSelectTrackId(int type, out uint trackid)
        {
            if(type == 1)
            {
                if(TrackSelected != null)
                {
                    trackid = TrackSelected.Id;
                    return true;
                }

                Growl.Warning("请在左列表选择轨道！");
            }

            if(type == 2)
            {
                if (OutTrackSelected != null)
                {
                    trackid = OutTrackSelected.Id;
                    return true;
                }

                Growl.Warning("请在右列表选择轨道！");
            }
            trackid = 0;
            return false;
        }

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                ShowAreaFileter = false;
                filterareaid = areaid;
                filterlineid = lineid;
            }
        }

        private void InitTrask()
        {
            TrackList.Clear();
            List<Track> trasks = PubMaster.Track.GetTrackList();
            foreach (Track track in trasks)
            {
                TrackView view = new TrackView(track);
                view.UpdateStockQty(PubMaster.Goods.GetTrackStockCount(track.id));
                if (track.Type == TrackTypeE.储砖_入
                    || (track.Type == TrackTypeE.储砖_出入 && track.Type2 == TrackType2E.入库)
                    || (track.Type == TrackTypeE.储砖_出入 && track.Type2 == TrackType2E.通用 && IsInAreaLine(track)))
                {
                    TrackList.Add(view);
                }
                else if (track.Type == TrackTypeE.储砖_出 || (track.Type == TrackTypeE.储砖_出入))
                {
                    OutTrackList.Add(view);
                }
            }
        }

        private Dictionary<int, int> AreaLineMIdId = new Dictionary<int, int>();
        private bool IsInAreaLine(Track track)
        {
            KeyValuePair<int, int> arealine = AreaLineMIdId.FirstOrDefault(c => c.Key == (track.area + track.line));
            if(arealine.Key == 0)
            {
                uint midmaxid = PubMaster.Track.GetMidId(track.area, track.line);
                AreaLineMIdId.Add((track.area + track.line), (int)midmaxid);

                return track.id <= midmaxid;
            }

            return track.id <= arealine.Value;
        }

        private void TrackStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is Track track)
            {
                if (track.Type == TrackTypeE.储砖_入
                    || (track.Type == TrackTypeE.储砖_出入 && track.Type2 == TrackType2E.入库)
                    || (track.Type == TrackTypeE.储砖_出入 && track.Type2 == TrackType2E.通用 && IsInAreaLine(track)))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TrackView view = TrackList.FirstOrDefault(c => c.Id == track.id);
                        if (view == null)
                        {
                            view = new TrackView(track);
                            TrackList.Add(view);
                        }
                        view.Update(track);
                        view.UpdateStockQty(PubMaster.Goods.GetTrackStockCount(track.id));
                    });
                }
                else if (track.Type == TrackTypeE.储砖_出 || (track.Type == TrackTypeE.储砖_出入))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TrackView view = OutTrackList.FirstOrDefault(c => c.Id == track.id);
                        if (view == null)
                        {
                            view = new TrackView(track);
                            OutTrackList.Add(view);
                        }
                        view.Update(track);
                        view.UpdateStockQty(PubMaster.Goods.GetTrackStockCount(track.id));
                    });
                }
            }
        }


        private void TrackStockQtyUpdate(uint trackid)
        {
            if (trackid >0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TrackView view = TrackList.FirstOrDefault(c => c.Id == trackid);
                    if (view != null)
                    {
                        view.UpdateStockQty(PubMaster.Goods.GetTrackStockCount(trackid));
                    }
                    else
                    {
                        TrackView outview = OutTrackList.FirstOrDefault(c => c.Id == trackid);
                        if (outview != null)
                        {
                            outview.UpdateStockQty(PubMaster.Goods.GetTrackStockCount(trackid));
                        }
                    }
                });
            }
        }

        private void CheckTypeRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint type))
                {
                    filtertracktype = type;
                    TrackView.Refresh();
                }
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn && btn.DataContext is MyRadioBtn radio)
            {
                TrackSelected = null;
                filterareaid = radio.AreaID;
                filterlineid = radio.Line;

                UseInOutLeftViewMaxId = PubMaster.Track.GetMidId(filterareaid, filterlineid);

                TrackView.Refresh();
                OutTrackView.Refresh();
            }
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
