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

namespace wcs.ViewModel
{
    public class FerryViewModel : MViewModel
    {
        public FerryViewModel() : base("Ferry")
        {
            _deviceList = new ObservableCollection<FerryView>();
            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.FerryStatusUpdate, FerryStatusUpdate);

            PubTask.Ferry.GetAllFerry();

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
            if (item is FerryView view)
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

        private ObservableCollection<FerryView> _deviceList;
        private FerryView _devicselected;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private bool showareafilter = true;

        #region[配置]
        private string devname;
        private bool configdrawopen;
        #endregion

        #endregion

        #region[属性]

        #region[配置]
        public string DevName
        {
            get => devname;
            set => Set(ref devname, value);
        }

        public bool ConfigDrawOpen
        {
            get => configdrawopen;
            set => Set(ref configdrawopen, value);
        }
        #endregion

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
        public RelayCommand<FerryView> DevConfigCmd => new Lazy<RelayCommand<FerryView>>(() => new RelayCommand<FerryView>(DevConfig)).Value;

        private void DevConfig(FerryView dev)
        {
            if (dev != null)
            {
                ConfigDrawOpen = true;
                DevName = dev.Name;
            }
        }
        #endregion

        #region[方法]


        private void FerryStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is DevFerry ferry 
                && msg.o2 is SocketConnectStatusE conn
                && msg.o3 is bool working)
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
                        DeviceList.Add(view);
                    }
                    view.Update(ferry, conn, working);
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

        private async void SendFerryTask(string tag)
        {
            if (DeviceSelected == null)
            {
                Growl.Warning("请先选择设备");
                return;
            }

            string resultMsg = "";
            bool isOK = true;
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
                        if (PubTask.Ferry.StopFerry(DeviceSelected.ID, out resultMsg))
                        {
                            resultMsg = "发送成功！";
                        }
                        else
                        {
                            isOK = false;
                        }
                        break;

                    case 6://定位
                        bool isdownferry = PubMaster.Device.IsDevType(DeviceSelected.ID, DeviceTypeE.下摆渡);
                        DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                         .Initialize<TrackSelectViewModel>((vm) =>
                         {
                             vm.SetAreaFilter(0, false);
                             vm.QueryFerryTrack(DeviceSelected.ID);
                         }).GetResultAsync<DialogResult>();
                        if (result.p1 is Track tra)
                        {
                            if (PubTask.Ferry.DoManualLocate(DeviceSelected.ID, tra.id, isdownferry, out resultMsg))
                            {
                                resultMsg = "发送成功！";
                            }
                            else
                            {
                                isOK = false;
                            }
                        }
                        break;
                }
            }

            // 提示信息
            ShowMsg(resultMsg, isOK);
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
