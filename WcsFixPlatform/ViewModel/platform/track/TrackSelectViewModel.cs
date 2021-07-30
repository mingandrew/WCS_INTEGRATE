using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
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

namespace wcs.ViewModel
{
    public class TrackSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public TrackSelectViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _tralist = new ObservableCollection<Track>();

            TrackView = System.Windows.Data.CollectionViewSource.GetDefaultView(TraList);
            TrackView.Filter = new Predicate<object>(OnFilterMovie);

            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);
        }

        #region[字段]

        private DialogResult _result;
        private ObservableCollection<Track> _tralist;
        private Track trackselected;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0, filtertracktype = 0;
        private ushort filterlineid = 0;

        private DateTime? refreshtime;
        private bool showareafilter;
        #endregion

        #region[属性]
        public IList<MyRadioBtn> AreaRadio
        {
            get => _arearadio;
            set => Set(ref _arearadio, value);
        }
        public ICollectionView TrackView { set; get; }

        public ObservableCollection<Track> TraList
        {
            get => _tralist;
            set => Set(ref _tralist, value);
        }

        public Track TrackSelected
        {
            get => trackselected;
            set => Set(ref trackselected, value);
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

        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;

        #endregion

        #region[方法]
        private List<TrackTypeE> Types;
        public bool IsTypeChange(List<TrackTypeE> types)
        {
            if (Types.Count != types.Count) return true;

            foreach (TrackTypeE typeE in types)
            {
                if (!Types.Contains(typeE))
                {
                    return true;
                }
            }
            return false;
        }


        public void QueryFerryTrack(uint ferryid)
        {
            List<Track> tracks = PubMaster.Track.GetFerryTracks(ferryid);
            Application.Current.Dispatcher.Invoke(() =>
            {
                TraList.Clear();
                foreach (Track track in tracks)
                {
                    TraList.Add(track);
                }
            });
        }

        public void QueryTrack(List<TrackTypeE> types)
        {
            if (refreshtime is null 
                || (refreshtime is DateTime time && (DateTime.Now-time).TotalSeconds > 60)
                || IsTypeChange(types))
            {
                List<Track> tracks = PubMaster.Track.GetTracksInTypes(types);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TraList.Clear();
                    foreach (Track track in tracks)
                    {
                        TraList.Add(track);
                    }
                });
            }
            Types = types;
        }

        public void QueryTrack(uint areaid, List<TrackTypeE> types)
        {
            if (refreshtime is null
                || (refreshtime is DateTime time && (DateTime.Now - time).TotalSeconds > 60)
                || IsTypeChange(types))
            {
                List<Track> tracks = PubMaster.Track.GetTracksInTypes(areaid, types);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TraList.Clear();
                    foreach (Track track in tracks)
                    {
                        TraList.Add(track);
                    }
                });
            }
            Types = types;
        }

        public void SetAreaFilter(uint areaid, bool isshow)
        {
            filterareaid = areaid;
            if (isshow)
            {
                if(PubMaster.Area.IsSingleAreaLine(out uint aid, out ushort lineid))
                {
                    ShowAreaFilter = false;
                    filterareaid = aid;
                    filterlineid = lineid;
                }
                else
                {
                    ShowAreaFilter = true;
                }
            }
            else
            {
                ShowAreaFilter = isshow;
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

        public void QueryAreaTrack(uint areaid)
        {
            List<Track> tracks = PubMaster.Track.GetAreaTracks(areaid);
            Application.Current.Dispatcher.Invoke(() =>
            {
                TraList.Clear();
                foreach (Track track in tracks)
                {
                    TraList.Add(track);
                }
            });
        }

        public void QueryAreaTrack(uint areaid, params TrackTypeE[] notContainType)
        {
            List<Track> tracks = PubMaster.Track.GetAreaTracks(areaid);
            Application.Current.Dispatcher.Invoke(() =>
            {
                TraList.Clear();
                foreach (Track track in tracks)
                {
                    if (notContainType.Contains(track.Type))
                    {
                        continue;
                    }
                    TraList.Add(track);
                }
            });
        }


        public void QueryTileTrack(uint tileid)
        {
            List<Track> tracks = PubMaster.Track.GetTileTrack(tileid);
            Application.Current.Dispatcher.Invoke(() =>
            {
                TraList.Clear();
                foreach (Track track in tracks)
                {
                    TraList.Add(track);
                }
            });
        }

        public void QueryTileTrack(uint areaid, uint tileid)
        {
            List<Track> tracks = PubMaster.Track.GetTileTrack(areaid, tileid);
            Application.Current.Dispatcher.Invoke(() =>
            {
                TraList.Clear();
                foreach (Track track in tracks)
                {
                    TraList.Add(track);
                }
            });
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0 && filtertracktype == 0) return true;
            if (item is Track view)
            {
                TrackTypeE tt = (TrackTypeE)filtertracktype;

                if (filterareaid == 0)
                {
                    if (view.Type == TrackTypeE.储砖_出入)
                    {
                        switch (tt)
                        {
                            case TrackTypeE.储砖_入:
                                return view.IsWorkIn();

                            case TrackTypeE.储砖_出:
                                return view.IsWorkOut();
                        }
                    }

                    return view.Type == tt;
                }

                if (filtertracktype == 0)
                {
                    return view.area == filterareaid && view.line == filterlineid;
                }

                if (view.Type == TrackTypeE.储砖_出入)
                {
                    switch (tt)
                    {
                        case TrackTypeE.储砖_入:
                            return filterareaid == view.area && filterlineid == view.line && view.IsWorkIn();

                        case TrackTypeE.储砖_出:
                            return filterareaid == view.area && filterlineid == view.line && view.IsWorkOut();
                    }
                }

                return filterareaid == view.area && filterlineid == view.line && tt == view.Type;
            }
            return true;
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn && btn.DataContext is MyRadioBtn radio)
            {
                TrackSelected = null;
                filterareaid = radio.AreaID;
                filterlineid = radio.Line;
                TrackView.Refresh();
            }
        }

        private void Comfirm()
        {
            if (TrackSelected == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = TrackSelected;

            ClearData();
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = null;

            ClearData();
            CloseAction?.Invoke();
        }

        private void ClearData()
        {
            filterareaid = 0;
            filtertracktype = 0;
            filterlineid = 0;
    }

        #endregion
    }
}
