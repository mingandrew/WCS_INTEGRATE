using enums;
using enums.track;
using enums.warning;
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
using wcs.Dialog.platform.device;

namespace wcs.ViewModel
{
    public class TileLifterViewModel : MViewModel
    {
        public TileLifterViewModel() : base("TileLifter")
        {
            _deviceList = new ObservableCollection<TileLifterView>();
            InitAreaRadio();

            Messenger.Default.Register<MsgAction>(this, MsgToken.TileLifterStatusUpdate, TileLifterStatusUpdate);


            DeviceView = System.Windows.Data.CollectionViewSource.GetDefaultView(DeviceList);
            DeviceView.Filter = new Predicate<object>(OnFilterMovie);

            PubTask.TileLifter.GetAllTileLifter();

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
            if (item is TileLifterView view)
            {
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
        private bool showareafilter = true;

        private ObservableCollection<TileLifterView> _deviceList;
        private TileLifterView _devicselected;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private ushort filterlineid = 0;
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

        public ObservableCollection<TileLifterView> DeviceList
        {
            get => _deviceList;
            set => Set(ref _deviceList, value);
        }

        public TileLifterView DeviceSelected
        {
            get => _devicselected;
            set => Set(ref _devicselected, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        public RelayCommand<string> SendTileLifterTaskCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SendTileLifterTask)).Value;


        #endregion

        #region[方法]
        private async void SendTileLifterTask(string tag)
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
                        PubTask.TileLifter.StartStopTileLifter(DeviceSelected.ID, true);
                        break;
                    case 2://中断通讯
                        PubTask.TileLifter.StartStopTileLifter(DeviceSelected.ID, false);
                        break;

                    case 3://启用
                           //判断是否备用砖机
                        if (PubTask.TileLifter.IsBackupTileLifter(DeviceSelected.ID)
                            && !PubMaster.Dic.IsSwitchOnOff(DicTag.AutoBackupTileFunc, false))
                        {
                            //展示能备用的砖机的信息，并返回被选择的砖机id
                            DialogResult alterresult = await HandyControl.Controls.Dialog.Show<DeviceBackupSelectDialog>()
                               .Initialize<DeviceBackupSelectViewModel>((vm) =>
                               {
                                   vm.GetBackupTilelifterInfo(DeviceSelected.ID);
                               }).GetResultAsync<DialogResult>();

                            if (alterresult.p1 is bool ar && alterresult.p2 is uint need_dev_id)
                            {
                                string rr = string.Format("是否确定选择：\r\n 【名称 - {0}】\r\n【类型 - {1}】\r\n【品种 - {2}】\r\n【当前作业轨道 - {3}】\r\n【砖机作业轨道 - {4}】",
                                    alterresult.p3, alterresult.p4, alterresult.p5, alterresult.p6, alterresult.p7);
                                MessageBoxResult box = HandyControl.Controls.MessageBox.Show(rr, "警告",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                                if (box == MessageBoxResult.Yes)
                                {
                                    //然后调用下面方法，用于修改备用砖机的品种等信息
                                    if (PubMaster.DevConfig.SetBackupTileLifter(need_dev_id, DeviceSelected.ID))
                                    {
                                        Growl.Success("修改成功！");
                                    }
                                    else
                                    {
                                        Growl.Warning("修改失败");
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                //然后调用下面方法，用于修改备用砖机的品种等信息
                                //PubMaster.DevConfig.SetBackupTileLifter(need_dev_id, DeviceSelected.ID);
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        if (PubMaster.Device.SetDevWorking(DeviceSelected.ID, true, out DeviceTypeE _, "PC"))
                        {
                            PubTask.TileLifter.UpdateWorking(DeviceSelected.ID, true, 255);
                        }
                        break;
                    case 4://停用
                        if (PubMaster.Device.SetDevWorking(DeviceSelected.ID, false, out DeviceTypeE _, "PC"))
                        {
                            PubTask.TileLifter.UpdateWorking(DeviceSelected.ID, false, 255);
                        }
                        break;

                    case 5://变更品种
                        uint area = PubMaster.Device.GetDeviceArea(DeviceSelected.ID);
                        bool isuptilelifter = PubMaster.DevConfig.IsTileWorkMod(DeviceSelected.ID, TileWorkModeE.上砖);
                        DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                         .Initialize<GoodsSelectViewModel>((vm) =>
                         {
                             vm.SetAreaFilter(area, false);
                             if (isuptilelifter)
                             {
                                 vm.QueryStockGood();
                             }
                             else
                             {
                                 vm.QueryGood();
                             }
                         }).GetResultAsync<DialogResult>();

                        if (result.p1 is bool rs && result.p2 is GoodsView good)
                        {
                            if (PubMaster.DevConfig.SetTileLifterGoodsAllCount(DeviceSelected.ID, good.ID))
                            {
                                PubTask.TileLifter.UpdateTileLifterGoods(DeviceSelected.ID, good.ID);
                                //清除上砖数量为0的报警
                                PubMaster.Warn.RemoveDevWarn(WarningTypeE.Warning37, (ushort)DeviceSelected.ID);
                            }
                        }
                        break;

                    case 6:
                        PubTask.TileLifter.DoInv(DeviceSelected.ID, true, DevLifterInvolE.介入);
                        break;
                    case 7:
                        PubTask.TileLifter.DoInv(DeviceSelected.ID, true, DevLifterInvolE.离开);
                        break;
                    case 8:
                        PubTask.TileLifter.DoInv(DeviceSelected.ID, false, DevLifterInvolE.介入);
                        break;
                    case 9:
                        PubTask.TileLifter.DoInv(DeviceSelected.ID, false, DevLifterInvolE.离开);
                        break;

                    case 10://修改策略
                        bool isdowntile = PubMaster.DevConfig.IsTileWorkMod(DeviceSelected.ID, TileWorkModeE.下砖);
                        MsgAction strategyrs = await HandyControl.Controls.Dialog.Show<ChangeStrategyDialog>()
                            .Initialize<ChangeStrategyDialogViewModel>((vm) =>
                            {

                                vm.SetShow(isdowntile);
                                if (isdowntile)
                                {
                                    vm.SetInStrategy(DeviceSelected.InStrategy, DeviceSelected.WorkType);
                                }
                                else
                                {
                                    vm.SetOutStrategy(DeviceSelected.OutStrategy, DeviceSelected.WorkType);
                                }
                            }).GetResultAsync<MsgAction>();

                        if (strategyrs.o1 is bool ishcnage)
                        {
                            if (isdowntile && strategyrs.o2 is StrategyInE instrategy && strategyrs.o3 is DevWorkTypeE inworktype)
                            {
                                if (PubMaster.DevConfig.SetInStrategy(DeviceSelected.ID, instrategy, inworktype))
                                {
                                    PubTask.TileLifter.UpdateTileInStrategry(DeviceSelected.ID, instrategy, inworktype);
                                    //清除没有策略的报警
                                    PubMaster.Warn.RemoveDevWarn(enums.warning.WarningTypeE.TileNoneStrategy, (ushort)DeviceSelected.ID);
                                }
                            }

                            if (!isdowntile && strategyrs.o2 is StrategyOutE outstrategy && strategyrs.o3 is DevWorkTypeE worktype)
                            {
                                if ((DeviceSelected.OutStrategy != outstrategy
                                    || DeviceSelected.WorkType != worktype)
                                    && PubMaster.DevConfig.SetOutStrategy(DeviceSelected.ID, outstrategy, worktype))
                                {
                                    PubTask.TileLifter.UpdateTileOutStrategry(DeviceSelected.ID, outstrategy, worktype);
                                    //清除没有策略的报警
                                    PubMaster.Warn.RemoveDevWarn(enums.warning.WarningTypeE.TileNoneStrategy, (ushort)DeviceSelected.ID);
                                }
                            }
                        }
                        break;

                    case 11://设置优先上砖轨道
                        bool isupTL = PubMaster.DevConfig.IsTileWorkMod(DeviceSelected.ID, TileWorkModeE.上砖);
                        if (!isupTL)
                        {
                            Growl.Info("只有上砖机可用此功能！");
                            return;
                        }
                        if (DeviceSelected.WorkType == DevWorkTypeE.轨道作业)
                        {
                            Growl.Info("轨道作业中，无法设置！");
                            return;
                        }
                        DialogResult res = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                         .Initialize<TrackSelectViewModel>((vm) =>
                         {
                             vm.SetAreaFilter(0, true);
                             vm.QueryTileTrack(DeviceSelected.ID);
                         }).GetResultAsync<DialogResult>();
                        if (res.p1 is Track tra)
                        {
                            if (tra.Type != TrackTypeE.储砖_出 && tra.Type != TrackTypeE.储砖_出入)
                            {
                                Growl.Warning("请选择能上砖作业的轨道！");
                                return;
                            }

                            if (tra.TrackStatus != TrackStatusE.启用 && tra.TrackStatus != TrackStatusE.仅上砖)
                            {
                                Growl.Warning("请选择能上砖作业的轨道！");
                                return;
                            }

                            if (tra.StockStatus == TrackStockStatusE.空砖)
                            {
                                Growl.Warning("请选择不为空砖的轨道！");
                                return;
                            }

                            PubMaster.DevConfig.SetLastTrackId(DeviceSelected.ID, tra.id);
                            Growl.Success("设置成功！");
                        }
                        break;
                    case 12:  //忽略1工位
                        PubTask.TileLifter.DoIgnore(DeviceSelected.ID, true);
                        break;
                    case 13:  //忽略2工位
                        PubTask.TileLifter.DoIgnore(DeviceSelected.ID, false);
                        break;
                    case 14:  //转产操作

                        area = PubMaster.Device.GetDeviceArea(DeviceSelected.ID);
                        await HandyControl.Controls.Dialog.Show<GoodShiftDialog>()
                            .Initialize<GoodShiftDialogViewModel>((vm) =>
                            {
                                vm.SetArea(area, DeviceSelected.ID, DeviceSelected.Name, DeviceSelected.GoodsId);
                            }).GetResultAsync<MsgAction>();
                        break;
                    case 15:  //切换作业模式
                        if (!PubMaster.DevConfig.IsAllowToDo(DeviceSelected.ID, DevLifterCmdTypeE.模式, out string msg))
                        {
                            Growl.Warning(msg);
                            return;
                        }

                        area = PubMaster.Device.GetDeviceArea(DeviceSelected.ID);
                        await HandyControl.Controls.Dialog.Show<CutoverDialog>()
                            .Initialize<CutoverDialogViewModel>((vm) =>
                            {
                                vm.SetArea(area, DeviceSelected.ID, DeviceSelected.Name, DeviceSelected.GoodsId);
                                vm.SetWorkMode(PubMaster.DevConfig.GetTileWorkMode(DeviceSelected.ID));
                            }).GetResultAsync<MsgAction>();
                        break;
                    case 16: //取消切换作业模式
                        if (!PubMaster.DevConfig.CancelCutover(DeviceSelected.ID, out string r))
                        {

                            Growl.Warning(r);
                            return;
                        }
                        break;
                }
            }
        }



        private void TileLifterStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is DevTileLifter dev
                && msg.o2 is SocketConnectStatusE conn
                && msg.o3 is uint gid
                && msg.o4 is StrategyInE instrategy
                && msg.o5 is StrategyOutE outstrategy
                && msg.o6 is bool working
                && msg.o7 is string tid
                && msg.o8 is DevWorkTypeE worktype
                && msg.o9 is string goodscount)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TileLifterView view = DeviceList.FirstOrDefault(c => c.ID == msg.ID);
                    if (view == null)
                    {
                        view = new TileLifterView()
                        {
                            ID = msg.ID,
                            Name = msg.Name
                        };
                        PubMaster.Device.GetDeviceAreaLine(view.ID, out uint areaid, out ushort lineid);
                        view.AreaId = areaid;
                        view.LineId = lineid;
                        DeviceList.Add(view);
                    }
                    view.Update(dev, conn, gid, instrategy, outstrategy, working, tid, worktype, goodscount);
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
