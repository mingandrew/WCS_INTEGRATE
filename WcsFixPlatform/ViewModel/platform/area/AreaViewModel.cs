using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.device;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class AreaViewModel : ViewModelBase
    {
        public AreaViewModel()
        {
            _devlist = new List<AreaDevice>();
            _ferrytralist = new ObservableCollection<AreaDeviceTrack>();
            _tiletralist = new ObservableCollection<AreaDeviceTrack>();
            _areatracklist = new ObservableCollection<AreaTrack>();
            InitAreaRadio();

            DeviceView = System.Windows.Data.CollectionViewSource.GetDefaultView(DevList);
            DeviceView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();
        }


        #region[字段]
        private bool showareafilter = true;

        private IList<MyRadioBtn> _arearadio;
        private uint SelectAreaId = 0;
        private ushort SelectLineId = 0;
        private bool _refrdev, _refareatrac, _refrtra, _refrtraprior;
        private string _tabtag;


        #region[设备配置]
        private DeviceTypeE _filterdevtype = DeviceTypeE.上砖机;
        private IList<AreaDevice> _devlist;
        private AreaDevice _devselect;
        #endregion

        #region[区域轨道]
        private ObservableCollection<AreaTrack> _areatracklist;
        private AreaTrack _areatraselect;
        #endregion

        #region[摆渡车轨道配置]
        private ObservableCollection<AreaDeviceTrack> _ferrytralist;
        private Device _selectferry;
        private AreaDeviceTrack _ferrytraselect;
        private string ferryname;
        #endregion

        #region[上下砖机轨道配置]

        private ObservableCollection<AreaDeviceTrack> _tiletralist;
        private Device _selecttile;
        private AreaDeviceTrack _tiletraselect;
        private string tilename;

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

        public bool IsUpDownTile
        {
            get => _selecttile?.Type == DeviceTypeE.砖机;
        }

        #region[设备配置]


        public ICollectionView DeviceView { set; get; }
        public AreaDevice DeviceSelected
        {
            get => _devselect;
            set => Set(ref _devselect, value);
        }

        private IList<AreaDevice> DevList
        {
            get => _devlist;
            set => Set(ref _devlist, value);
        }
        #endregion

        #region[区域轨道]

        public ObservableCollection<AreaTrack> AreaTrackList
        {
            get => _areatracklist;
            set => Set(ref _areatracklist, value);
        }

        public AreaTrack AreaTraSelect
        {
            get => _areatraselect;
            set => Set(ref _areatraselect, value);
        }

        #endregion

        #region[摆渡车轨道配置]
        public string FerryName
        {
            get => ferryname;
            set => Set(ref ferryname, value);
        }

        public ObservableCollection<AreaDeviceTrack> FerryTraList
        {
            get => _ferrytralist;
            set => Set(ref _ferrytralist, value);
        }

        public AreaDeviceTrack FerryTrackSelect
        {
            get => _ferrytraselect;
            set => Set(ref _ferrytraselect, value);
        }
        #endregion

        #region[上下砖机轨道配置]
        public string TileName
        {
            get => tilename;
            set => Set(ref tilename, value);
        }
        public ObservableCollection<AreaDeviceTrack> TileTraList
        {
            get => _tiletralist;
            set => Set(ref _tiletralist, value);
        }

        public AreaDeviceTrack TileTrackSelect
        {
            get => _tiletraselect;
            set => Set(ref _tiletraselect, value);
        }
        #endregion

        #endregion

        #region[命令]
        //区域切换
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        //模块Tab切换
        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;

        //设备切换
        public RelayCommand<string> DeviceTypeCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DeviceType)).Value;

        //设备操作
        public RelayCommand<string> DeviceEditCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DeviceEdit)).Value;

        //选择设备
        public RelayCommand FerrySelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(FerrySelected)).Value;
        public RelayCommand TileSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(TileSelected)).Value;

        public RelayCommand SavePriorToDbCmd => new Lazy<RelayCommand>(() => new RelayCommand(SavePriorToDb)).Value;
        public RelayCommand<string> AddOtherAreaCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(AddOtherArea)).Value;
        public RelayCommand<string> AddOtherTileAreaCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(AddOtherTileArea)).Value;


        public RelayCommand AddOtherTrackCmd => new Lazy<RelayCommand>(() => new RelayCommand(AddOtherTrack)).Value;
        public RelayCommand AddOtherTileTrackCmd => new Lazy<RelayCommand>(() => new RelayCommand(AddOtherTileTrack)).Value;

        #endregion

        #region[方法]

        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                ShowAreaFileter = false;
                SelectAreaId = areaid;
                SelectLineId = lineid;
                RefreshDev();
            }
        }

        #region[区域按钮/Tab切换]
        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaLineRadioList(false);
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn && btn.DataContext is MyRadioBtn radio)
            {
                SelectAreaId = radio.AreaID;
                SelectLineId = radio.Line;

                _refrdev = false;
                _refrtra = false;
                _refrtraprior = false;
                _refareatrac = false;
                switch (_tabtag)
                {
                    case "device":
                        RefreshDev();
                        break;
                    case "areatrack":
                        RefreshAreaTrack();
                        break;
                }
                FerryName = "";
                _selectferry = null;
                TileName = "";
                _selecttile = null;
                FerryTrackSelect = null;
                TileTrackSelect = null;
                FerryTraList.Clear();
                TileTraList.Clear();
            }
        }

        private void TabSelected(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is System.Windows.Controls.TabControl pro
                && pro.SelectedItem is System.Windows.Controls.TabItem tab)
            {
                switch (tab.Tag.ToString())
                {
                    case "device":
                        if (!_refrdev)
                        {
                            RefreshDev();
                        }
                        break;
                    case "areatrack":
                        if (!_refareatrac)
                        {
                            RefreshAreaTrack();
                        }
                        break;
                    case "ferrytrack":
                        if (!_refrtra)
                        {
                            RefreshFerryTrackPrior();
                        }
                        break;
                    case "tiletrack":
                        if (!_refrtraprior)
                        {
                            RefreshTileLifterTrackPrior();
                        }
                        break;
                    default:
                        break;
                }
                _tabtag = tab.Tag.ToString();
            }
        }
        #endregion

        #region[设备配置]

        private void DeviceType(string tag)
        {
            switch (tag)
            {
                case "tilelifter":
                    _filterdevtype = DeviceTypeE.上砖机;
                    break;
                case "ferry":
                    _filterdevtype = DeviceTypeE.前摆渡;
                    break;
                case "carrier":
                    _filterdevtype = DeviceTypeE.运输车;
                    break;
            }
            DeviceView.Refresh();
        }

        bool OnFilterMovie(object item)
        {
            if (item is AreaDevice view)
            {
                switch (view.DevType)
                {
                    case DeviceTypeE.砖机:
                    case DeviceTypeE.上砖机:
                    case DeviceTypeE.下砖机:
                        return _filterdevtype == DeviceTypeE.上砖机;
                    case DeviceTypeE.前摆渡:
                    case DeviceTypeE.后摆渡:
                        return _filterdevtype == DeviceTypeE.前摆渡;
                    case DeviceTypeE.运输车:
                        return _filterdevtype == DeviceTypeE.运输车;
                }
            }
            return false;
        }

        /// <summary>
        /// 设备添加/修改/删除
        /// </summary>
        /// <param name="tag"></param>
        private void DeviceEdit(string tag)
        {
            switch (tag)
            {
                case "add":
                    AddDevice();
                    break;

                case "edit":
                    if (!CheckDevSelect()) return;
                    EditDevice();
                    break;

                case "delete":
                    if (!CheckDevSelect()) return;
                    DeleteDevice();
                    break;

                case "trackprior":

                    break;
                case "ferrytradelete":
                    if (FerryTrackSelect == null) return;
                    if (PubMaster.Area.DeleteAreaDevTrack(FerryTrackSelect.id))
                    {
                        RefreshFerryTrackPrior();
                    }
                    break;
                case "tiletradelete":
                    if (TileTrackSelect == null) return;
                    if (PubMaster.Area.DeleteAreaDevTrack(TileTrackSelect.id))
                    {
                        RefreshTileLifterTrackPrior();
                    }
                    break;
                case "areatrackdelete":

                    break;
            }
        }
        private void RefreshDev()
        {
            DevList.Clear();
            foreach (var item in PubMaster.Area.GetAreaDevList(SelectAreaId))
            {
                if(SelectLineId > 0)
                {
                    ushort lineid = PubMaster.Device.GetDeviceLIne(item.device_id);
                    if(lineid ==0 || ( lineid != 0 && SelectLineId == lineid))
                    {
                        DevList.Add(item);
                    }
                }
                else
                {
                    DevList.Add(item);
                }
            }

            DeviceView.Refresh();
            _refrdev = true;
        }

        /// <summary>
        /// 判断是否选定设备
        /// </summary>
        /// <returns></returns>
        private bool CheckDevSelect()
        {
            if (DeviceSelected == null)
            {
                Growl.Warning("请先选择设备！");
                return false;
            }
            return true;
        }

        #region[设备添加/修改/删除]

        private async void AddDevice()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                   .Initialize<DeviceSelectViewModel>((vm) =>
                   {
                       vm.FilterArea = false;
                       vm.AreaId = 0;
                       vm.LineId = 0;
                       vm.SetSelectType(_filterdevtype);
                   }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                if (!PubMaster.Area.AddAreaDevice(SelectAreaId, dev.id, out string _result))
                {
                    Growl.Warning(_result);
                }
                else
                {
                    Growl.Success("添加成功！");
                    RefreshDev();
                }
            }
        }

        private void EditDevice()
        {


        }

        private void DeleteDevice()
        {
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否删除设备！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (PubMaster.Area.DeleteAreaDevice(DeviceSelected.id))
                {
                    Growl.Success("删除成功！");
                    RefreshDev();
                }
            }
        }

        #endregion

        #endregion

        #region[区域轨道]

        private void RefreshAreaTrack()
        {
            AreaTrackList.Clear();
            foreach (var item in PubMaster.Area.GetAreaTracks(SelectAreaId))
            {
                if (SelectLineId > 0)
                {
                    ushort lineid = PubMaster.Track.GetTrackLine(item.track_id);
                    if (lineid == 0 || (lineid != 0 && SelectLineId == lineid))
                    {
                        AreaTrackList.Add(item);
                    }
                }
                else
                {
                    AreaTrackList.Add(item);
                }
            }
            _refareatrac = true;
        }

        #endregion

        #region[摆渡车配置]
        private void RefreshFerryTrackPrior()
        {
            if (_selectferry == null)
            {
                return;
            }

            FerryTraList.Clear();
            foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selectferry.id))
            {
                FerryTraList.Add(item);
            }

            if (FerryTraList.Count == 0)
            {
                Growl.Ask("是否添加砖机对应轨道信息！", isConfirmed =>
                {
                    if (isConfirmed)
                    {
                        PubMaster.Area.AddAreaFerryTrackList(SelectAreaId, SelectAreaId, _selectferry);

                        foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selectferry.id))
                        {
                            FerryTraList.Add(item);
                        }
                    }
                    return true;
                });
            }

            _refrtra = true;
        }

        private async void FerrySelected()
        {
            if (SelectAreaId == 0)
            {
                Growl.Warning("请先选择区域!");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                .Initialize<DeviceSelectViewModel>((vm) =>
                {
                    vm.FilterArea = true;
                    vm.AreaId = SelectAreaId;
                    vm.LineId = SelectLineId;

                    vm.SetSelectType(DeviceTypeE.前摆渡, DeviceTypeE.后摆渡);
                }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                _selectferry = dev;
                FerryName = dev.name;
                RefreshFerryTrackPrior();
            }
        }

        private void AddOtherArea(string tag)
        {
            if (_selectferry == null)
            {
                return;
            }

            if (uint.TryParse(tag, out uint trackarea))
            {
                PubMaster.Area.AddAreaFerryTrackList(trackarea, SelectAreaId, _selectferry);

                FerryTraList.Clear();
                foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selectferry.id))
                {
                    FerryTraList.Add(item);
                }
            }
        }
        
        /// <summary>
        /// 选择轨道添加
        /// </summary>
        private async void AddOtherTrack()
        {
            if (_selectferry == null)
            {
                return;
            }

            TrackTypeE tracktype1, tracktype2, tiletrtype;
            switch (_selectferry.Type)
            {
                case DeviceTypeE.前摆渡:
                    tracktype1 = TrackTypeE.储砖_出;
                    tracktype2 = TrackTypeE.储砖_出入;
                    tiletrtype = TrackTypeE.上砖轨道;
                    break;
                case DeviceTypeE.后摆渡:
                    tracktype1 = TrackTypeE.储砖_入;
                    tracktype2 = TrackTypeE.储砖_出入;
                    tiletrtype = TrackTypeE.下砖轨道;
                    break;
                default:
                    return;
            }
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                 .Initialize<TrackSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(0, true);
                     vm.QueryTrack(SelectAreaId, new List<TrackTypeE>() { tracktype1, tracktype2, tiletrtype });
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                if (PubMaster.Area.IsInDevTrack(tra.id, SelectAreaId, _selectferry.id))
                {
                    Growl.Warning("已存在该轨道！");
                    return;
                }

                PubMaster.Area.AddAreaTrack(tra.id, SelectAreaId, _selectferry);

                FerryTraList.Clear();
                foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selectferry.id))
                {
                    FerryTraList.Add(item);
                }
            }
        }

        #endregion

        #region[上下砖机配置]

        private void RefreshTileLifterTrackPrior()
        {
            if (_selecttile == null)
            {
                return;
            }

            TileTraList.Clear();
            foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selecttile.id))
            {
                TileTraList.Add(item);
            }

            if (TileTraList.Count == 0)
            {
                Growl.Ask("是否添加砖机对应轨道信息！", isConfirmed =>
                {
                    if (isConfirmed)
                    {
                        PubMaster.Area.AddAreaTileTrackList(SelectAreaId, SelectAreaId, _selecttile);

                        foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selecttile.id))
                        {
                            TileTraList.Add(item);
                        }
                    }
                    return true;
                });
            }
            _refrtraprior = true;
        }

        private async void TileSelected()
        {
            if (SelectAreaId == 0)
            {
                Growl.Warning("请先选择区域!");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                .Initialize<DeviceSelectViewModel>((vm) =>
                {
                    vm.AreaId = SelectAreaId;
                    vm.LineId = SelectLineId;
                    vm.FilterArea = true;
                    vm.SetSelectType(DeviceTypeE.上砖机, DeviceTypeE.下砖机, DeviceTypeE.砖机);
                }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                _selecttile = dev;
                TileName = dev.name;
                RefreshTileLifterTrackPrior();
            }
        }

        private void AddOtherTileArea(string tag)
        {
            if (_selecttile == null)
            {
                return;
            }

            if (uint.TryParse(tag, out uint trackarea))
            {
                PubMaster.Area.AddAreaTileTrackList(trackarea, SelectAreaId, _selecttile);
                TileTraList.Clear();
                foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selecttile.id))
                {
                    TileTraList.Add(item);
                }
            }
        }

        /// <summary>
        /// 选择轨道添加
        /// </summary>
        private async void AddOtherTileTrack()
        {
            if (_selecttile == null)
            {
                return;
            }

            TrackTypeE tracktype1, tracktype2;
            switch (_selecttile.Type)
            {
                case DeviceTypeE.砖机:
                case DeviceTypeE.上砖机:
                    tracktype1 = TrackTypeE.储砖_出;
                    tracktype2 = TrackTypeE.储砖_出入;
                    break;
                case DeviceTypeE.下砖机:
                    tracktype1 = TrackTypeE.储砖_入;
                    tracktype2 = TrackTypeE.储砖_出入;
                    break;
                default:
                    return;
            }
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                 .Initialize<TrackSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(0, true);
                     vm.QueryTrack(_selecttile.area, new List<TrackTypeE>() { tracktype1, tracktype2 });
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                if (tra.Type2 == TrackType2E.入库 && _selecttile.Type == DeviceTypeE.上砖机)
                {
                    // 确认当前下砖机是否配置了该轨道
                    if (PubMaster.Area.IsTrackInTiles(_selecttile.area, DeviceTypeE.下砖机, tra.id))
                    {
                        Growl.Warning("该轨道已被下砖机配置，无法再配置给上砖机！");
                        return;
                    }
                }

                if (tra.Type2 == TrackType2E.出库 && _selecttile.Type == DeviceTypeE.下砖机)
                {
                    // 确认当前下砖机是否配置了该轨道
                    if (PubMaster.Area.IsTrackInTiles(_selecttile.area, DeviceTypeE.上砖机, tra.id))
                    {
                        Growl.Warning("该轨道已被上砖机配置，无法再配置给下砖机！");
                        return;
                    }
                }

                if (PubMaster.Area.IsInDevTrack(tra.id, SelectAreaId, _selecttile.id))
                {
                    Growl.Warning("已存在该轨道！");
                    return;
                }

                PubMaster.Area.AddAreaTrack(tra.id, SelectAreaId, _selecttile);

                TileTraList.Clear();
                foreach (var item in PubMaster.Area.GetAreaDevTraList(SelectAreaId, _selecttile.id))
                {
                    TileTraList.Add(item);
                }
            }
        }

        #endregion

        #region[保存的数据库]


        private void SavePriorToDb()
        {
            if (_tabtag.Equals("tiletrack"))
            {
                if (_selecttile == null)
                {
                    return;
                }

                PubMaster.Area.SaveToDb(SelectAreaId, _selecttile);
            }
            else
            {
                if (_selectferry == null)
                {
                    return;
                }

                PubMaster.Area.SaveToDb(SelectAreaId, _selectferry);
            }
            Growl.Success("保存成功！");
        }

        #endregion

        #endregion

    }
}
