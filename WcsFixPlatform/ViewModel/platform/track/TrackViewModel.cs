using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using module.device;
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
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class TrackViewModel : MViewModel
    {
        public TrackViewModel() : base("Track")
        {
            _tracklist = new ObservableCollection<TrackView>();
            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.TrackStatusUpdate, TrackStatusUpdate);
            InitTrask();

            DeviceView = System.Windows.Data.CollectionViewSource.GetDefaultView(TrackList);
            DeviceView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filtertracktype == 0) return true;
            if (item is TrackView view)
            {
                if (filterareaid == 0)
                {
                    return view.Type == (TrackTypeE)filtertracktype;
                }

                if (filtertracktype == 0)
                {
                    return view.Area == filterareaid;
                }

                return filterareaid == view.Area && (TrackTypeE)filtertracktype == view.Type;
            }
            return true;
        }
        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);
        }
        #region[字段]
        private bool showareafilter = true;
        private ObservableCollection<TrackView> _tracklist;
        private TrackView _trackselected;

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

        public ICollectionView DeviceView { set; get; }

        public ObservableCollection<TrackView> TrackList
        {
            get => _tracklist;
            set => Set(ref _tracklist, value);
        }

        public TrackView TrackSelected
        {
            get => _trackselected;
            set => Set(ref _trackselected, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;
        public RelayCommand<string> TrackUpdateCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TrackUpdate)).Value;

        private void TrackUpdate(string tag)
        {
            if(TrackSelected == null)
            {
                Growl.Warning("请先选择轨道!");
                return;
            }

            if(int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1://启用
                        if(!PubMaster.Track.SetTrackStatus(TrackSelected.Id, TrackStatusE.启用, out string result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");

                        break;
                    case 2://停用
                        if (!PubMaster.Track.SetTrackStatus(TrackSelected.Id, TrackStatusE.停用, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");
                        break;
                    case 3://空砖
                        if (!PubMaster.Track.SetStockStatus(TrackSelected.Id, TrackStockStatusE.空砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        break;
                    case 4://有砖
                        if (!PubMaster.Track.SetStockStatus(TrackSelected.Id, TrackStockStatusE.有砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        break;
                    case 5://满砖
                        if (!PubMaster.Track.SetStockStatus(TrackSelected.Id, TrackStockStatusE.满砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        break;
                    case 6://仅上砖
                        if (!PubMaster.Track.SetTrackStatus(TrackSelected.Id, TrackStatusE.仅上砖, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        Growl.Success("修改成功！");
                        break;
                    case 7://仅下砖
                        if (!PubMaster.Track.SetTrackStatus(TrackSelected.Id, TrackStatusE.仅下砖, out result, "PC手动"))
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

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint aid))
            {
                ShowAreaFileter = false;
            }
        }

        private void InitTrask()
        {
            TrackList.Clear();
            List<Track> trasks = PubMaster.Track.GetTrackList();
            foreach (Track track in trasks)
            {
                if (track.Type == TrackTypeE.储砖_入
                    || track.Type == TrackTypeE.储砖_出
                    || track.Type == TrackTypeE.储砖_出入)
                {
                    TrackList.Add(new TrackView(track));
                }
            }
        }

        private void TrackStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is Track track)
            {
                if (track.Type == TrackTypeE.储砖_入
                    || track.Type == TrackTypeE.储砖_出
                    || track.Type == TrackTypeE.储砖_出入)
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
                    });
                }
            }
        }

        private void CheckTypeRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint type))
                {
                    filtertracktype = type;
                    DeviceView.Refresh();
                }
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint areaid))
                {
                    TrackSelected = null;
                    filterareaid = areaid;
                    DeviceView.Refresh();
                }
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
