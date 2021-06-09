using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
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
using task;
using wcs.Data.View;
using wcs.Dialog;
using wcs.ViewModel.platform.device;

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
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                ShowAreaFileter = false;
                filterareaid = areaid;
                filterlineid = lineid;
            }
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0) return true;
            if (item is CarrierView view)
            {
                if (view.LineId == 0) return view.AreaId == filterareaid;
                return view.AreaId == filterareaid && view.LineId == filterlineid;
                //return PubMaster.Area.IsDeviceInArea(filterareaid, view.ID);
            }
            return true;
        }
        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);
        }
        #region[字段]

        private ObservableCollection<CarrierView> _deviceList;
        private CarrierView _devicselected;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private ushort filterlineid = 0;
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
        private async void SendCarrierTask(string tag)
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
                    case 20://连接通讯
                        PubTask.Carrier.StartStopCarrier(DeviceSelected.ID, true);
                        break;
                    case 21://中断通讯
                        PubTask.Carrier.StartStopCarrier(DeviceSelected.ID, false);
                        break;

                    case 22://启用
                        if (PubMaster.Device.SetDevWorking(DeviceSelected.ID, true, out DeviceTypeE _, "PC手动"))
                        {
                            PubTask.Carrier.UpdateWorking(DeviceSelected.ID, true);
                        }
                        break;
                    case 23://停用
                        if (PubMaster.Device.SetDevWorking(DeviceSelected.ID, false, out DeviceTypeE _, "PC手动"))
                        {
                            PubTask.Carrier.UpdateWorking(DeviceSelected.ID, false);
                        }
                        break;

                    case 24://清空设备信息
                        Growl.Ask("清除前请确认小车位于安全且不干扰作业的位置，如：所在轨道已停用，是维修轨道等", isConfirmed =>
                        {
                            if (isConfirmed)
                            {
                                string msg = PubTask.Carrier.ClearTaskStatus(DeviceSelected.ID);
                                Growl.Info(msg);
                            }
                            return true;
                        });
                        break;

                    default:
                        DevCarrierTaskE type = (DevCarrierTaskE)stype;
                        ushort srfid = 0;
                        
                        //判断前进放砖是否有串联砖机
                        if ((DevCarrierTaskE)stype == DevCarrierTaskE.前进放砖)
                        {

                            Track track = PubMaster.Track.GetTrack(DeviceSelected.CurrentTrackId);
                            if (track.Type == TrackTypeE.摆渡车_出)
                            {
                                if (!PubTask.Ferry.IsStopAndSiteOnTrack(track.id, true, out uint intrackid, out string warning))
                                {
                                    Growl.Warning(warning);
                                    return;
                                }
                                Track tt = PubMaster.Track.GetTrack(intrackid);
                                if (tt.Type == TrackTypeE.上砖轨道 && tt.rfid_1 != tt.rfid_2 && tt.rfid_2 != 0)
                                {
                                    DialogResult result1 = await HandyControl.Controls.Dialog.Show<Carrier2TileLifterDialog>()
                                        .Initialize<Carrier2TileLifterViewModel>((vm) =>
                                        {
                                            vm.DeviceList = PubMaster.DevConfig.GetDevices(tt.id);
                                            vm.TRACK = tt;
                                        }).GetResultAsync<DialogResult>();
                                    if (result1.p1 is ushort selectrfid)
                                    {
                                        srfid = selectrfid;
                                    }
                                }
                            }
                        }


                        if (!PubTask.Carrier.DoManualNewTask(DeviceSelected.ID, type, out string result, "PC手动", srfid))
                        {
                            Growl.Warning(result);
                        }
                        break;
                }
            }
        }

        private void CarrierStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is DevCarrier dev
                && msg.o2 is SocketConnectStatusE conn
                && msg.o3 is bool working
                && msg.o4 is uint currenttrackId
                && msg.o5 is uint targettrackId
                && msg.o6 is ushort currenttrackline)
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
                        PubMaster.Device.GetDeviceAreaLine(view.ID, out uint areaid, out ushort lineid);
                        view.AreaId = areaid;
                        view.LineId = lineid;
                        DeviceList.Add(view);
                    }
                    view.Update(dev, conn, working, currenttrackId, targettrackId);
                    if (view.UpdateLine(currenttrackline))
                    {
                        DeviceView.Refresh();
                    }
                });
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn && btn.DataContext is MyRadioBtn radio)
            {
                filterareaid = radio.AreaID;
                filterlineid = radio.Line;
                DeviceView.Refresh();
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
