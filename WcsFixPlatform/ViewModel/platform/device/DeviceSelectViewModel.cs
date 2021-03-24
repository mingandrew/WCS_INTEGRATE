using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class DeviceSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DeviceSelectViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _devlist = new ObservableCollection<Device>();
        }

        #region[字段]

        private DialogResult _result;
        private ObservableCollection<Device> _devlist;
        private Device selectdic;
        private bool _liftercheck, _ferrycheck, _carriercheck;
        private bool _lifterenable, _ferryenable, _carrierenable;
        private List<DeviceTypeE> lastquerytype;

        #endregion

        #region[属性]

        public bool LifterCheck
        {
            get => _liftercheck;
            set => Set(ref _liftercheck, value);
        }

        public bool FerryCheck
        {
            get => _ferrycheck;
            set => Set(ref _ferrycheck, value);
        }

        public bool CarrierCheck
        {
            get => _carriercheck;
            set => Set(ref _carriercheck, value);
        }
        public bool LifterEnable
        {
            get => _lifterenable;
            set => Set(ref _lifterenable, value);
        }
        public bool FerryEnable
        {
            get => _ferryenable;
            set => Set(ref _ferryenable, value);
        }
        public bool CarrierEnable
        {
            get => _carrierenable;
            set => Set(ref _carrierenable, value);
        }

        public ObservableCollection<Device> DevList
        {
            get => _devlist;
            set => Set(ref _devlist, value);
        }

        public Device SelectDev
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
        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]

        public void SetSelectType(List<DeviceTypeE> types)
        {
            GetDevce(types);
        }

        private void CheckRadioBtn(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is RadioButton rbtn)
            {
                //switch (rbtn.Tag.ToString())
                //{
                //    case "tilelifter":
                //        GetDevce(DeviceTypeE.上砖机);
                //        break;
                //    case "ferry":
                //        GetDevce(DeviceTypeE.上摆渡);
                //        break;
                //    case "carrier":
                //        GetDevce(DeviceTypeE.运输车);
                //        break;
                //}
            }
        }

        private void GetDevce(List<DeviceTypeE> types)
        {
            //if (DevList.Count > 0 && lastquerytype == type)
            //{
            //    return;
            //}

            if (FilterArea && AreaId == 0)
            {
                Growl.Warning("请选择区域");
                return;
            }

            List<Device> list = new List<Device>();

            if (!FilterArea)
            {
                list.AddRange(PubMaster.Device.GetDevices(types));
            }
            else
            {
                list.AddRange(PubMaster.Device.GetDevices(types, AreaId));
            }

            lastquerytype = types;
            DevList.Clear();
            foreach (Device dev in list)
            {
                DevList.Add(dev);
            }
        }

        private void Comfirm()
        {
            if (SelectDev == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = true;
            Result.p2 = SelectDev;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            Result.p2 = null;
            CloseAction?.Invoke();
        }
        #endregion
    }
}
