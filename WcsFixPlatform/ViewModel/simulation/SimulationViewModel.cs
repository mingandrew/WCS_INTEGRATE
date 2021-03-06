using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using module.window;
using resource;
using simtask;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class SimulationViewModel : ViewModelBase
    {
        public SimulationViewModel()
        {
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
        #endregion

        #region[属性]

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
                            SimServer.Stop();
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
        //区域切换
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        //模块Tab切换
        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;

        #endregion

        #region[方法]
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
            AreaRadio = PubMaster.Area.GetAreaRadioList(false);
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
                    _refferrypose = false;
                    _refcarrier = false;
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

        }

        private void RefreshFerry()
        {

        }

        private void RefreshTile()
        {

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

        #endregion
    }
}
