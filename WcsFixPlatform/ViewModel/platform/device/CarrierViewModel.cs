using enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using module.device;
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
using task;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class CarrierViewModel : MViewModel
    {
        public CarrierViewModel() : base("Carrier")
        {
            _deviceList = new ObservableCollection<CarrierView>();
            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.CarrierStatusUpdate, CarrierStatusUpdate);

            PubTask.Carrier.GetAllCarrier();

            DeviceView = System.Windows.Data.CollectionViewSource.GetDefaultView(DeviceList);
            DeviceView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();
        }
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea())
            {
                ShowAreaFileter = false;
            }
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0) return true;
            if (item is CarrierView view)
            {
                return PubMaster.Area.IsFerryInArea(filterareaid, view.ID);
            }
            return true;
        }
        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);
        }
        #region[字段]

        private ObservableCollection<CarrierView> _deviceList;
        private CarrierView _devicselected;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private bool showareafilter = true;
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

        public ObservableCollection<CarrierView> DeviceList
        {
            get => _deviceList;
            set => Set(ref _deviceList, value);
        }

        public CarrierView DeviceSelected
        {
            get => _devicselected;
            set => Set(ref _devicselected, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<string> SendCarrierTaskCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SendCarrierTask)).Value;

        #endregion

        #region[方法]
        private void SendCarrierTask(string tag)
        {
            if (DeviceSelected == null)
            {
                Growl.Warning("请先选择设备");
                return;
            }

            if (byte.TryParse(tag, out byte stype))
            {
                switch (stype)
                {
                    case 10:
                        PubTask.Carrier.DoReset(DeviceSelected.ID);
                        break;
                    case 11:
                        PubTask.Carrier.DoSetMode(DeviceSelected.ID, DevCarrierWorkModeE.调试);
                        break;
                    case 12:
                        PubTask.Carrier.DoSetMode(DeviceSelected.ID, DevCarrierWorkModeE.生产);
                        break;
                    case 13://启动
                        PubTask.Carrier.StartStopCarrier(DeviceSelected.ID, true);
                        break;
                    case 14://停止
                        PubTask.Carrier.StartStopCarrier(DeviceSelected.ID, false);
                        break;
                    case 15://清空设备信息
                        PubTask.Carrier.ClearTaskStatus(DeviceSelected.ID);
                        break;
                    default:
                        DevCarrierTaskE type = (DevCarrierTaskE)stype;
                        if (!PubTask.Carrier.DoManualTask(DeviceSelected.ID, type, out string result, false))
                        {
                            Growl.Warning(result);
                        }
                        break;
                }
            }
        }

        private void CarrierStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is DevCarrier dev && msg.o2 is SocketConnectStatusE conn)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CarrierView view = DeviceList.FirstOrDefault(c => c.ID == msg.ID);
                    if (view == null)
                    {
                        view = new CarrierView()
                        {
                            ID = msg.ID,
                            Name = msg.Name
                        };
                        DeviceList.Add(view);
                    }
                    view.Update(dev, conn);
                });
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint areaid))
                {
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
