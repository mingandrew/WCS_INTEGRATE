using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using task;
using task.task;
using wcs.Data.Model;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class AddManualTransViewModel : MViewModel
    {
        public AddManualTransViewModel() : base("AddManualTrans")
        {
            M_Stock_Take_Comb = new ObservableCollection<WcsComboBoxItem>();
            M_Stock_Give_Comb = new ObservableCollection<WcsComboBoxItem>();
        }

        #region[字段]      

        private Device m_car_car, m_stock_car;

        private Track m_car_give_track, m_stock_take_track, m_stock_give_track;

        private string m_car_car_name, m_stock_car_name, m_car_give_track_name, m_stock_take_track_name, m_stock_give_track_name, m_stock_tile_name;

        private ObservableCollection<WcsComboBoxItem> m_stock_take_comb, m_stock_give_comb;
        private WcsComboBoxItem m_stock_take_tile, m_stock_give_tile;
        private bool m_stock_take_tile_show, m_stock_give_tile_show;

        #endregion

        #region[属性]
        public string M_Car_Car_Name
        {
            get => m_car_car_name;
            set => Set(ref m_car_car_name, value);
        }

        public string M_Stock_Car_Name
        {
            get => m_stock_car_name;
            set => Set(ref m_stock_car_name, value);
        }

        public string M_Car_Give_Track_Name
        {
            get => m_car_give_track_name;
            set => Set(ref m_car_give_track_name, value);
        }

        public string M_Stock_Take_Track_Name
        {
            get => m_stock_take_track_name;
            set => Set(ref m_stock_take_track_name, value);
        }

        public string M_Stock_Give_Track_Name
        {
            get => m_stock_give_track_name;
            set => Set(ref m_stock_give_track_name, value);
        }

        public string M_Stock_Tile_Name
        {
            get => m_stock_tile_name;
            set => Set(ref m_stock_tile_name, value);
        }

        public ObservableCollection<WcsComboBoxItem> M_Stock_Take_Comb
        {
            get => m_stock_take_comb;
            set => Set(ref m_stock_take_comb, value);
        }

        public ObservableCollection<WcsComboBoxItem> M_Stock_Give_Comb
        {
            get => m_stock_give_comb;
            set => Set(ref m_stock_give_comb, value);
        }

        public WcsComboBoxItem M_Stock_Take_Tile
        {
            get => m_stock_take_tile;
            set => Set(ref m_stock_take_tile, value);
        }

        public WcsComboBoxItem M_Stock_Give_Tile
        {
            get => m_stock_give_tile;
            set => Set(ref m_stock_give_tile, value);
        }

        public bool M_Stock_Take_Tile_Show
        {
            get => m_stock_take_tile_show;
            set => Set(ref m_stock_take_tile_show, value);
        }

        public bool M_Stock_Give_Tile_Show
        {
            get => m_stock_give_tile_show;
            set => Set(ref m_stock_give_tile_show, value);
        }
        #endregion

        #region[命令]    
        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;
        public RelayCommand<string> TaskActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TaskAction)).Value;

        #endregion

        #region[方法]

        private async void TaskAction(string tag)
        {
            switch (tag)
            {
                #region[移车任务]
                case "m_car_select"://移车运输车选择
                    SelectCar(true, false, 0, 0);
                    break;
                case "m_car_give_track_select"://移车放车轨道选择
                    SelectMoveCarGiveTrack();
                    break;
                case "m_car_clean"://清空信息
                    CleanMoveCarInput();
                    break;
                case "m_car_add_task"://确认添加任务
                    MoveCarAdd();
                    break;

                #endregion


                #region[移砖任务]
                case "m_stock_take_track_select":
                    SelectMoveStockTakeTrack(true);
                    break;
                case "m_stock_give_track_select":
                    SelectMoveStockTakeTrack(false);
                    break;
                case "m_stock_car_select":
                    if (!CheckMoveStockGiveTrack()) return;
                    SelectCar(false, true, m_stock_give_track.area, m_stock_give_track.line);
                    break;
                case "m_stock_clean"://清空信息
                    CleanMoveStockInput();
                    break;
                case "m_stock_add_task"://确认添加任务
                    MoveStockAdd();
                    break;
                default:
                    break;
                    #endregion
            }
        }


        private async void SelectCar(bool selectmovecar, bool filterarea, uint areaid, ushort lineid)
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                           .Initialize<DeviceSelectViewModel>((vm) =>
                           {
                               vm.FilterArea = filterarea;
                               vm.AreaId = areaid;
                               vm.LineId = lineid;
                               vm.SetSelectType(DeviceTypeE.运输车);
                           }).GetResultAsync<DialogResult>();
            if (result.p2 is Device dev)
            {
                if (selectmovecar)
                {
                    SelectMoveCarCarrier(dev);
                }
                else
                {
                    SelectMoveStockCar(dev);
                }
            }
        }

        #region[移车任务]

        private void CleanMoveCarInput()
        {
            M_Car_Car_Name = "";
            M_Car_Give_Track_Name = "";
            m_car_car = null;
            m_car_give_track = null;
        }

        private bool CheckSelectMoveCar()
        {
            if (m_car_car == null)
            {
                Growl.Warning("请先选择运输车！");
                return false;
            }

            Track cartrack = PubTask.Carrier.GetCarrierTrack(m_car_car.id);
            if (cartrack == null)
            {
                Growl.Warning("运输车当前没有轨道位置信息！");
                return false;
            }

            return true;
        }

        private void SelectMoveCarCarrier(Device dev)
        {
            m_car_car = dev;
            M_Car_Car_Name = dev.name;

            m_car_give_track = null;
            M_Car_Give_Track_Name = "";

        }

        private bool CheckSelectMoveGiveTrack()
        {
            if (m_car_give_track == null)
            {
                Growl.Warning("请先选择卸货轨道点！");
                return false;
            }

            return true;
        }

        private async void SelectMoveCarGiveTrack()
        {
            if (!CheckSelectMoveCar()) return;
            Track cartrack = PubTask.Carrier.GetCarrierTrack(m_car_car.id);
            if (cartrack == null)
            {
                Growl.Warning("运输车当前没有轨道位置信息！");
                return;
            }

            TrackTypeE[] types = new TrackTypeE[] { TrackTypeE.上砖轨道, TrackTypeE.下砖轨道, TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入 };
            //if (cartrack.InType(TrackTypeE.下砖轨道, TrackTypeE.后置摆渡轨道, TrackTypeE.储砖_入))
            //{
            //    types = new TrackTypeE[] { TrackTypeE.下砖轨道, TrackTypeE.储砖_入, TrackTypeE.储砖_出入 };
            //}
            //else
            //{
            //    types = new TrackTypeE[] { TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入 };
            //}


            DialogResult ingivetrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                        .Initialize<TrackSelectViewModel>((vm) =>
                        {
                            vm.SetAreaFilter(0, false);
                            vm.QueryAreaTrackType(cartrack.area, cartrack.line, types);
                        }).GetResultAsync<DialogResult>();
            if (ingivetrars.p1 is Track tra)
            {
                m_car_give_track = tra;
                M_Car_Give_Track_Name = tra.name;
            }
        }

        /// <summary>
        /// 添加移车任务
        /// </summary>
        private void MoveCarAdd()
        {
            if (!CheckSelectMoveCar()) return;

            if (!CheckSelectMoveGiveTrack()) return;

            Track cartrack = PubTask.Carrier.GetCarrierTrack(m_car_car.id);
            if (!PubTask.Trans.AddMoveCarrierTask(cartrack.id, m_car_give_track.id, m_car_car.id, out string result))
            {
                Growl.Warning(result);
                return;
            }

            Growl.Success("添加成功！");
            CleanMoveCarInput();
        }
        #endregion

        #region[移砖任务]

        /// <summary>
        /// 清空所以移砖的信息
        /// </summary>
        private void CleanMoveStockInput()
        {
            M_Stock_Car_Name = "";
            m_stock_car = null;

            CleanMoveStockTakeInput();

            CleanMoveStockGiveInput();
        }

        /// <summary>
        /// 清空取货轨道信息
        /// </summary>
        private void CleanMoveStockTakeInput()
        {
            m_stock_take_track = null;
            M_Stock_Take_Track_Name = "";
            M_Stock_Take_Comb.Clear();
            M_Stock_Take_Tile_Show = false;
            M_Stock_Take_Tile = null;
        }

        /// <summary>
        /// 清空放货轨道信息
        /// </summary>
        private void CleanMoveStockGiveInput()
        {
            m_stock_give_track = null;
            M_Stock_Give_Track_Name = "";
            M_Stock_Give_Comb.Clear();
            M_Stock_Give_Tile_Show = false;
            M_Stock_Give_Tile = null;
        }

        /// <summary>
        /// 检测取货轨道信息
        /// </summary>
        /// <returns></returns>
        private bool CheckMoveStockTakeTrack()
        {
            if (m_stock_take_track == null)
            {
                Growl.Warning("请先选择取货轨道！");
                return false;
            }

            Track track = PubMaster.Track.GetTrack(m_stock_take_track.id);
            if (track.NotInType(TrackTypeE.下砖轨道, TrackTypeE.上砖轨道))
            {
                if (track.StockStatus == TrackStockStatusE.空砖)
                {

                    Growl.Warning("取货轨道无砖");
                    return false;
                }
            }
            else
            {
                if (M_Stock_Take_Tile == null)
                {
                    Growl.Warning("请选择取货轨道砖机！");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检测卸货轨道信息
        /// </summary>
        /// <returns></returns>
        private bool CheckMoveStockGiveTrack()
        {
            if (m_stock_give_track == null)
            {
                Growl.Warning("请选择卸货轨道！");
                return false;
            }

            if (m_stock_take_track.area != m_stock_give_track.area || m_stock_take_track.line != m_stock_give_track.line)
            {
                Growl.Warning("请选择同区域线路的卸货轨道！");
                return false;
            }

            Track track = PubMaster.Track.GetTrack(m_stock_give_track.id);
            if ((track.Type == TrackTypeE.储砖_入
                || (track.Type == TrackTypeE.储砖_出入 && m_stock_take_track.Type == TrackTypeE.下砖轨道))
                && track.StockStatus == TrackStockStatusE.满砖)
            {
                Growl.Warning(string.Format("[ {0} ]入轨道已经满砖，不能放砖！", track.name));
                return false;
            }

            if (track.InType(TrackTypeE.下砖轨道, TrackTypeE.上砖轨道))
            {
                if (M_Stock_Give_Tile == null)
                {
                    Growl.Warning("请选择卸货轨道砖机！");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 选择轨道
        /// </summary>
        /// <param name="selecttake"></param>
        private async void SelectMoveStockTakeTrack(bool selecttake)
        {
            if (!selecttake && !CheckMoveStockTakeTrack()) return;

            TrackTypeE[] types = null;

            //选择卸货轨道
            if (!selecttake)
            {
                if (m_stock_take_track.InType(TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入))
                {
                    types = new TrackTypeE[] { TrackTypeE.储砖_出, TrackTypeE.上砖轨道, TrackTypeE.储砖_出入 };
                }
                else if (m_stock_take_track.InType(TrackTypeE.下砖轨道))
                {
                    types = new TrackTypeE[] { TrackTypeE.储砖_入, TrackTypeE.储砖_出入 };
                }
            }

            DialogResult ingivetrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                       .Initialize<TrackSelectViewModel>((vm) =>
                       {
                           vm.SetAreaFilter(0, false);
                           vm.QueryTrack(types ?? new TrackTypeE[] { TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.上砖轨道, TrackTypeE.下砖轨道 });
                       }).GetResultAsync<DialogResult>();
            if (ingivetrars.p1 is Track tra)
            {
                if (selecttake)
                {
                    SetMoveStockTakeTrack(tra);
                }
                else
                {
                    SetMoveStockGiveTrack(tra);
                }
            }
        }

        /// <summary>
        /// 设置取货轨道，如果是砖机则设置砖机下拉框信息
        /// </summary>
        /// <param name="track"></param>
        private void SetMoveStockTakeTrack(Track track)
        {
            m_stock_take_track = track;
            M_Stock_Take_Track_Name = track.name;

            M_Stock_Take_Comb.Clear();
            if (track.InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道))
            {
                List<uint> tileids = PubMaster.DevConfig.GetTileInTrack(track.id);
                foreach (var item in tileids)
                {
                    M_Stock_Take_Comb.Add(new WcsComboBoxItem(item, PubMaster.Device.GetDeviceName(item)));
                }

                if (M_Stock_Take_Comb.Count == 1)
                {
                    M_Stock_Take_Tile = M_Stock_Take_Comb[0];
                }
            }
            M_Stock_Take_Tile_Show = M_Stock_Take_Comb.Count > 0;

            if (m_stock_give_track != null && m_stock_give_track.id == m_stock_take_track.id)
            {
                CleanMoveStockGiveInput();
            }
        }

        /// <summary>
        /// 设置卸货轨道，同时设置卸货砖机下拉框选择
        /// </summary>
        /// <param name="track"></param>
        private void SetMoveStockGiveTrack(Track track)
        {
            if (m_stock_take_track.area != track.area || m_stock_take_track.line != track.line)
            {
                Growl.Warning("请选择同区域线路的卸货轨道！");
                return;
            }

            m_stock_give_track = track;
            M_Stock_Give_Track_Name = track.name;

            M_Stock_Give_Comb.Clear();
            if (track.InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道))
            {
                List<uint> tileids = PubMaster.DevConfig.GetTileInTrack(track.id);
                foreach (var item in tileids)
                {
                    M_Stock_Give_Comb.Add(new WcsComboBoxItem(item, PubMaster.Device.GetDeviceName(item)));
                }

                if (M_Stock_Give_Comb.Count == 1)
                {
                    M_Stock_Give_Tile = M_Stock_Give_Comb[0];
                }
            }
            M_Stock_Give_Tile_Show = M_Stock_Give_Comb.Count > 0;

            if (m_stock_give_track.id == m_stock_take_track.id)
            {
                Growl.Warning("请选择不同于取砖轨道！");
                CleanMoveStockGiveInput();
            }
        }

        /// <summary>
        /// 选择移转运输车
        /// </summary>
        /// <param name="dev"></param>
        private void SelectMoveStockCar(Device dev)
        {
            m_stock_car = null;
            M_Stock_Car_Name = "";

            if (PubTask.Trans.HaveCarrierInTrans(dev.id))
            {
                Growl.Warning(string.Format("[ {0} ]运输车有任务", dev.name));
                return;
            }
            if (!PubTask.Carrier.IsCarrierFree(dev.id))
            {
                Growl.Warning(string.Format("[ {0} ]运输车当前状态不正确！", dev.name));
                return;
            }

            m_stock_car = dev;
            M_Stock_Car_Name = dev.name;
        }

        /// <summary>
        /// 添加移砖任务
        /// </summary>
        private void MoveStockAdd()
        {
            if (!CheckMoveStockTakeTrack()) return;

            if (!CheckMoveStockGiveTrack()) return;

            uint tileid = 0;
            if (m_stock_take_track.InType(TrackTypeE.下砖轨道, TrackTypeE.上砖轨道))
            {
                tileid = m_stock_take_tile.Id;
            }

            if (m_stock_give_track.InType(TrackTypeE.下砖轨道, TrackTypeE.上砖轨道))
            {
                tileid = m_stock_give_tile.Id;
            }

            if (!PubTask.Trans.AddMoveStockTask(m_stock_take_track.id, m_stock_give_track.id, tileid, m_stock_car?.id ?? 0, out string result))
            {
                Growl.Warning(result);
                return;
            }

            Growl.Success("添加成功！");
            CleanMoveStockInput();

        }
        #endregion

        private void TabSelected(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is System.Windows.Controls.TabControl pro && pro.SelectedItem is System.Windows.Controls.TabItem tab)
            {
                switch (tab.Tag.ToString())
                {
                    case "MOVECAR":
                        break;
                    case "MOVESTOCK":
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
