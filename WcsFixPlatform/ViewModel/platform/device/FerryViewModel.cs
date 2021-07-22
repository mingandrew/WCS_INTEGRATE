﻿using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.msg;
using module.track;
using module.window;
using module.window.device;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using task;
using wcs.Dialog;
using wcs.Dialog.platform.track;
using wcs.ViewModel.platform.track;

namespace wcs.ViewModel
{
    public class FerryViewModel : MViewModel
    {
        public FerryViewModel() : base("Ferry")
        {
            _deviceList = new ObservableCollection<FerryView>();
            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.FerryStatusUpdate, FerryStatusUpdate);

            DeviceView = System.Windows.Data.CollectionViewSource.GetDefaultView(DeviceList);
            DeviceView.Filter = new Predicate<object>(OnFilterMovie);

            PubTask.Ferry.GetAllFerry();

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
            if (item is FerryView view)
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

        private ObservableCollection<FerryView> _deviceList;
        private FerryView _devicselected;

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

        public ObservableCollection<FerryView> DeviceList
        {
            get => _deviceList;
            set => Set(ref _deviceList, value);
        }

        public FerryView DeviceSelected
        {
            get => _devicselected;
            set => Set(ref _devicselected, value);
        }
        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<string> SendFerryTaskCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SendFerryTask)).Value;

        #endregion

        #region[方法]


        private void FerryStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is DevFerry ferry 
                && msg.o2 is SocketConnectStatusE conn
                && msg.o3 is bool working
                && msg.o4 is uint uptraid
                && msg.o5 is uint downtraid)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    FerryView view = DeviceList.FirstOrDefault(c => c.ID == msg.ID);
                    if (view == null)
                    {
                        view = new FerryView()
                        {
                            ID = msg.ID,
                            Name = msg.Name
                        };
                        PubMaster.Device.GetDeviceAreaLine(view.ID, out uint areaid, out ushort lineid);
                        view.AreaId = areaid;
                        view.LineId = lineid;
                        DeviceList.Add(view);
                    }
                    view.Update(ferry, conn, working, uptraid, downtraid);
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

        private async void SendFerryTask(string tag)
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
                    case 1://连接通讯
                        PubTask.Ferry.StartStopFerry(DeviceSelected.ID, true);
                        break;
                    case 2://中断通讯
                        PubTask.Ferry.StartStopFerry(DeviceSelected.ID, false);
                        break;

                    case 3://启用
                        if (PubMaster.Device.SetDevWorking(DeviceSelected.ID, true, out DeviceTypeE _, "PC"))
                        {
                            PubTask.Ferry.UpdateWorking(DeviceSelected.ID, true);
                        }
                        break;
                    case 4://停用
                        if (PubMaster.Device.SetDevWorking(DeviceSelected.ID, false, out DeviceTypeE _, "PC"))
                        {
                            PubTask.Ferry.UpdateWorking(DeviceSelected.ID, false);
                        }
                        break;

                    case 5://中止
                        if (!PubTask.Ferry.StopFerry(0, DeviceSelected.ID, "PC终止", "人为", out string rs))
                        {
                            Growl.Info(rs);
                            return;
                        }
                        Growl.Success("发送成功！");
                        break;

                    case 6://定位
                        bool isdownferry = PubMaster.Device.IsDevType(DeviceSelected.ID, DeviceTypeE.后摆渡);
                        DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                         .Initialize<TrackSelectViewModel>((vm) =>
                         {
                             vm.SetAreaFilter(0, false);
                             vm.QueryFerryTrack(DeviceSelected.ID);
                         }).GetResultAsync<DialogResult>();
                        if (result.p1 is Track tra)
                        {
                            if (!PubTask.Ferry.DoManualLocate(DeviceSelected.ID, tra.id, isdownferry, out string locateresult))
                            {
                                Growl.Warning(locateresult);
                                return;
                            }
                            Growl.Success("发送成功！");
                        }
                        break;

                    case 7://位置初始化
                        isdownferry = PubMaster.Device.IsDevType(DeviceSelected.ID, DeviceTypeE.后摆渡);
                        await HandyControl.Controls.Dialog.Show<LocationSelectDialog>()
                            .Initialize<LocationDialogViewModel>((vm) =>
                            {
                                vm.SetArea(DeviceSelected.AreaId, DeviceSelected.ID, isdownferry ? DeviceTypeE.后摆渡 : DeviceTypeE.后摆渡, DeviceSelected.Name);
                            }).GetResultAsync<MsgAction>();
                        break;

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
