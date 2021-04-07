﻿using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Tools.Extension;
using module.track;
using module.window;
using resource;
using simtask;
using simtask.task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using task;
using wcs.Data.View.sim;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class SimulationViewModel : ViewModelBase
    {
        public SimulationViewModel()
        {
            _tilelist = new ObservableCollection<SimDeviceView>();
            _ferrylist = new ObservableCollection<SimDeviceView>();
            _carrierlist = new ObservableCollection<SimDeviceView>();

            Messenger.Default.Register<SimTaskBase>(this, MsgToken.SimDeviceStatusUpdate, SimDeviceStatusUpdate);

            TileView = System.Windows.Data.CollectionViewSource.GetDefaultView(_tilelist);
            TileView.Filter = new Predicate<object>(OnFilter);

            FerryView = System.Windows.Data.CollectionViewSource.GetDefaultView(_ferrylist);
            FerryView.Filter = new Predicate<object>(OnFilter);

            CarrierView = System.Windows.Data.CollectionViewSource.GetDefaultView(_carrierlist);
            CarrierView.Filter = new Predicate<object>(OnFilter);

            InitAreaRadio();
            CheckIsSingle();
            SimServer.Init();
        }

        #region[字段]

        private bool simserverrun;

        private bool showareafilter = true;

        private IList<MyRadioBtn> _arearadio;
        private uint SelectAreaId = 0;
        private bool _reftile, _refcarrier, _refferry, _refferrypose;
        private string _tabtag;

        private ObservableCollection<SimDeviceView> _tilelist, _ferrylist, _carrierlist;

        #endregion

        #region[属性]
        public SimDeviceView SelectedTile, SelectedFerry, SelectedCarrier;
        public ICollectionView TileView { set; get; }
        public ICollectionView FerryView { set; get; }
        public ICollectionView CarrierView { set; get; }

        public bool SimServerRun
        {
            get => simserverrun;
            set
            {
                if (Set(ref simserverrun, value))
                {
                    if (SimServer.IsStartServer != value)
                    {
                        if (value)
                        {
                            SimServer.Start();
                        }
                        else
                        {
                            PubTask.StopSimDevice();
                            //SimServer.Stop();
                        }
                    }
                }
            }
        }

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

        #endregion

        #region[命令]

        #region[常用]
        //区域切换
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        //模块Tab切换
        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;
        public RelayCommand SavePriorToDbCmd => new Lazy<RelayCommand>(() => new RelayCommand(SavePriorToDb)).Value;
        #endregion

        #region[砖机]
        public RelayCommand<SimDeviceView> TileChangeWorkCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(TileChangeWork)).Value;
        public RelayCommand<SimDeviceView> TileSite1FullCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(TileSite1Full)).Value;
        public RelayCommand<SimDeviceView> TileSite2FullCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(TileSite2Full)).Value;
        public RelayCommand<SimDeviceView> TileSite1EmptyCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(TileSite1Empty)).Value;
        public RelayCommand<SimDeviceView> TileSite2EmptyCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(TileSite2Empty)).Value;
        public RelayCommand<SimDeviceView> TileRequireShiftCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(TileRequireShift)).Value;
        #endregion

        #region[运输车]
        public RelayCommand<SimDeviceView> CarrierSetInitSiteCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(CarrierSetInitSite)).Value;

        #endregion

        #region[摆渡车]
        public RelayCommand<SimDeviceView> FerrySetInitSiteCmd => new Lazy<RelayCommand<SimDeviceView>>(() => new RelayCommand<SimDeviceView>(FerrySetInitSite)).Value;

        #endregion

        #endregion

        #region[方法]

        #region[区域选择，Tab切换，列表状态更新]
        bool OnFilter(object item)
        {
            if (SelectAreaId == 0) return true;
            if (item is SimDeviceView view)
            {
                return view.area_id == SelectAreaId ;
            }
            return true;
        }

        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint areaid))
            {
                ShowAreaFileter = false;
                SelectAreaId = areaid;
                RefreshTile();
            }
        }

        #region[区域按钮/Tab切换]
        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint areaid))
                {
                    SelectAreaId = areaid;
                    _reftile = false;
                    _refferry = false;
                    _refcarrier = false;
                    _refferrypose = false;
                    switch (_tabtag)
                    {
                        case "tile":
                            RefreshTile();
                            break;
                        case "carrier":
                            RefreshCarrier();
                            break;
                        case "ferry":
                            RefreshFerry();
                            break;
                        case "ferrypose":
                            RefreshFerryPose();
                            break;
                    }
                }
            }
        }

        private void RefreshFerryPose()
        {

        }

        private void RefreshCarrier()
        {
            CarrierView.Refresh();
        }

        private void RefreshFerry()
        {
            FerryView.Refresh();
        }

        private void RefreshTile()
        {
            TileView.Refresh();
        }

        private void TabSelected(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is System.Windows.Controls.TabControl pro
                && pro.SelectedItem is System.Windows.Controls.TabItem tab)
            {
                switch (tab.Tag.ToString())
                {
                    case "tile":
                        if (!_reftile)
                        {
                            RefreshTile();
                        }
                        break;
                    case "carrier":
                        if (!_refcarrier)
                        {
                            RefreshCarrier();
                        }
                        break;
                    case "ferry":
                        if (!_refferry)
                        {
                            RefreshFerry();
                        }
                        break;
                    case "ferrypose":
                        if (!_refferrypose)
                        {
                            RefreshFerryPose();
                        }
                        break;
                    default:
                        break;
                }
                _tabtag = tab.Tag.ToString();
            }
        }
        #endregion

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="task"></param>
        private void SimDeviceStatusUpdate(SimTaskBase task)
        {
            if (task != null )
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if(task is SimCarrierTask c)
                    {
                        _carrierlist.Add(new SimDeviceView(task));
                    }else if(task is SimTileLifterTask t)
                    {
                        _tilelist.Add(new SimDeviceView(task));
                    }else if(task is SimFerryTask f)
                    {
                        _ferrylist.Add(new SimDeviceView(task));
                    }
                });
            }
        }
        
        #endregion

        #region[砖机操作]

        private void TileChangeWork(SimDeviceView dev)
        {
            if (dev != null)
            {
                dev.Working = !dev.Working;
                SimServer.TileLifter.StartOrStopWork(dev.dev_id, dev.Working);
            }
        }

        private void TileSite1Full(SimDeviceView dev)
        {
            if (dev != null)
            {
                SimServer.TileLifter.SetLoadStatus(dev.dev_id, true, true);
            }
        }

        private void TileSite2Full(SimDeviceView dev)
        {
            if (dev != null)
            {
                SimServer.TileLifter.SetLoadStatus(dev.dev_id, true, false);
            }
        }

        private void TileSite1Empty(SimDeviceView dev)
        {
            if (dev != null)
            {
                SimServer.TileLifter.SetLoadStatus(dev.dev_id, false, true);
            }
        }

        private void TileSite2Empty(SimDeviceView dev)
        {
            if (dev != null)
            {
                SimServer.TileLifter.SetLoadStatus(dev.dev_id, false, false);
            }
        }

        private void TileRequireShift(SimDeviceView dev)
        {
            if (dev != null)
            {
                SimServer.TileLifter.SetRequireShift(dev.dev_id);
            }
        }

        #endregion

        #region[小车操作]

        private async void CarrierSetInitSite(SimDeviceView dev)
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                             .Initialize<TrackSelectViewModel>((vm) =>
                             {
                                 vm.SetAreaFilter(dev.area_id, false);
                                 vm.QueryAreaTrack(dev.area_id);
                             }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                ushort setsite = 0, setpoint = 0;
                bool isontrack = true;
                switch (tra.Type)
                {
                    case TrackTypeE.上砖轨道:
                        setsite = tra.rfid_1;
                        setpoint = SimServer.Carrier.GetUpTileTrackPoint(dev.area_id);
                        break;
                    case TrackTypeE.下砖轨道:
                        setsite = tra.rfid_1;
                        setpoint = SimServer.Carrier.GetDownTileTrackPoint(dev.area_id);
                        break;
                    case TrackTypeE.储砖_入:
                        setsite = tra.rfid_1;
                        setpoint = tra.limit_point;
                        break;
                    case TrackTypeE.储砖_出:
                        setsite = tra.rfid_1;
                        setpoint = tra.limit_point_up;
                        break;
                    case TrackTypeE.储砖_出入:
                        setsite = tra.rfid_1;
                        setpoint = tra.limit_point;
                        break;
                    case TrackTypeE.摆渡车_入:
                        setsite = tra.rfid_1;
                        setpoint = SimServer.Carrier.GetFerryTrackPos(tra.rfid_1);
                        isontrack = false;
                        break;
                    case TrackTypeE.摆渡车_出:
                        setsite = tra.rfid_1;
                        setpoint = SimServer.Carrier.GetFerryTrackPos(tra.rfid_1);
                        isontrack = false;
                        break;
                }
                SimServer.Carrier.SetCurrentSite(dev.dev_id, setsite, setpoint, isontrack);
            }
        }

        #endregion

        #region[摆渡车操作]

        private async void FerrySetInitSite(SimDeviceView dev)
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                             .Initialize<TrackSelectViewModel>((vm) =>
                             {
                                 vm.SetAreaFilter(0, true);
                                 vm.QueryTileTrack(dev.area_id, dev.dev_id);
                             }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                ushort ferrycode = 0;
                switch (tra.Type)
                {
                    case TrackTypeE.上砖轨道:
                        ferrycode = tra.ferry_up_code;
                        break;
                    case TrackTypeE.下砖轨道:
                        ferrycode = tra.ferry_down_code;
                        break;
                    case TrackTypeE.储砖_入:
                        ferrycode = tra.ferry_up_code;
                        break;
                    case TrackTypeE.储砖_出:
                        ferrycode = tra.ferry_down_code;
                        break;
                    case TrackTypeE.储砖_出入:
                        ferrycode = dev.DevType == DeviceTypeE.下摆渡 ? tra.ferry_up_code : tra.ferry_down_code;
                        break;
                }
                SimServer.Ferry.SetCurrentSite(dev.dev_id, tra, ferrycode);
            }
        }


        #endregion

        private void SavePriorToDb()
        {

        }
        #endregion
    }
}
