using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.track;
using module.window;
using System;
using System.Collections.Generic;
using System.Windows;
using task;
using task.task;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class AddManualTransViewModel : MViewModel
    {
        public AddManualTransViewModel() : base("AddManualTrans")
        {

        }

        #region[字段]      

        private Device m_car_car, m_stock_car, m_stock_tile;

        private Track m_car_give_track, m_stock_take_track, m_stock_give_track;

        private string m_car_car_name, m_stock_car_name, m_car_give_track_name, m_stock_take_track_name, m_stock_give_track_name, m_stock_tile_name;

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
                    SelectMoveCar();
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
                    break;
                case "m_stock_give_track_select":
                    break;
                case "m_stock_car_select":
                    break;
                case "m_stock_clean"://清空信息
                    CleanMoveStockInput();
                    break;
                case "m_stock_add_task"://确认添加任务

                    break;
                #endregion
            }

            //if(int.TryParse(tag, out int type))
            //{
            //    switch (type)
            //    {
            //        #region[添加手动入库]

            //        case 1:
            //            #region[选择下砖机]
            //            DialogResult inresult = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
            //               .Initialize<DeviceSelectViewModel>((vm) =>
            //               {
            //                   vm.FilterArea = false;
            //                   vm.SetSelectType(DeviceTypeE.下砖机 , DeviceTypeE.砖机);
            //               }).GetResultAsync<DialogResult>();
            //            if (inresult.p1 is bool rs && inresult.p2 is Device indev)
            //            {
            //                ClearInTaskInput();
            //                TileLifterTask inTile = PubTask.TileLifter.GetTileLifter(indev.id);
            //                if (inTile.DevConfig.goods_id == 0)
            //                {
            //                    Growl.Warning("请先设置砖机品种！");
            //                    return;
            //                }
            //                in_dev = indev;
            //                In_Tilelifter_id = indev.id;
            //                In_Goods_id = inTile.DevConfig.goods_id;

            //                if(indev.Type2 == DeviceType2E.单轨)
            //                {
            //                    In_Take_track_id = inTile.DevConfig.left_track_id;
            //                    Is_In_Double_Track = false;
            //                }
            //                else
            //                {
            //                    In_Take_Left_TrackId = inTile.DevConfig.left_track_id;
            //                    In_Take_Right_TrackId = inTile.DevConfig.right_track_id;
            //                    Is_In_Double_Track = true;
            //                }
            //            }
            //            #endregion
            //            break;
            //        case 2://选择取砖轨道

            //            break;
            //        case 3://选择放砖轨道
            //            if (!CheckInDev()) return;
            //            DialogResult ingivetrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
            //            .Initialize<TrackSelectViewModel>((vm) =>
            //            {
            //                vm.SetAreaFilter(0, false);
            //                vm.QueryTileTrack(in_dev.area, in_dev.id);
            //            }).GetResultAsync<DialogResult>();
            //            if (ingivetrars.p1 is Track tra)
            //            {
            //                if(tra.StockStatus == TrackStockStatusE.满砖)
            //                {
            //                    Growl.Warning("轨道满砖了，请选择其他轨道！");
            //                    return;
            //                }
            //                In_Give_track_id = tra.id;
            //            }
            //            break;
            //        case 4://清空信息
            //            ClearInTaskInput();
            //            break;
            //        case 5://添加入库任务
            //            AddInTrans();
            //            break;

            //        #endregion

            //        #region[添加手动出库]

            //        case 6:
            //            #region[选择上砖机]
            //            DialogResult outresult = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
            //               .Initialize<DeviceSelectViewModel>((vm) =>
            //               {
            //                   vm.FilterArea = false;
            //                   vm.SetSelectType(DeviceTypeE.上砖机, DeviceTypeE.砖机 );
            //               }).GetResultAsync<DialogResult>();
            //            if (outresult.p1 is bool outrs && outresult.p2 is Device outdev)
            //            {
            //                ClearOutTaskInput();
            //                TileLifterTask outTile = PubTask.TileLifter.GetTileLifter(outdev.id);
            //                if(outTile.DevConfig.goods_id == 0)
            //                {
            //                    Growl.Warning("请先设置砖机品种！");
            //                    return;
            //                }
            //                out_dev = outdev;
            //                Out_Tilelifter_id = outdev.id;
            //                Out_Goods_id = outTile.DevConfig.goods_id;
            //                if(outdev.Type2 == DeviceType2E.单轨)
            //                {
            //                    Out_Give_track_id = outTile.DevConfig.left_track_id;
            //                    Is_Out_Double_Track = false;
            //                }
            //                else
            //                {
            //                    Out_Give_Left_TrackId = outTile.DevConfig.left_track_id;
            //                    Out_Give_Right_TrackId = outTile.DevConfig.right_track_id;
            //                    Is_Out_Double_Track = true;
            //                }
            //            }
            //            #endregion
            //            break;
            //        case 7://选择取砖轨道
            //            if (!CheckOutDev()) return;
            //            DialogResult outtaketrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
            //            .Initialize<TrackSelectViewModel>((vm) =>
            //            {
            //                vm.SetAreaFilter(0, false);
            //                vm.QueryTileTrack(out_dev.area, out_dev.id);
            //            }).GetResultAsync<DialogResult>();
            //            if (outtaketrars.p1 is Track outtaketra)
            //            {
            //                Out_Take_track_id = outtaketra.id;
            //            }
            //            break;
            //        case 8://选择放砖轨道

            //            break;
            //        case 9://清空信息
            //            ClearOutTaskInput();
            //            break;
            //        case 10://添加出库任务
            //            AddOutTrans();
            //            break;

            //        #endregion
            //    }
            //}
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
            if(m_car_car == null)
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

        private bool CheckSelectMoveGiveTrack()
        {
            if(m_car_give_track == null)
            {
                Growl.Warning("请先选择卸货轨道点！");
                return false;
            }

            return true;
        }

        private async void SelectMoveCar()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                           .Initialize<DeviceSelectViewModel>((vm) =>
                           {
                               vm.FilterArea = false;
                               vm.SetSelectType(DeviceTypeE.运输车);
                           }).GetResultAsync<DialogResult>();
            if (result.p2 is Device dev)
            {
                m_car_car = dev;
                M_Car_Car_Name = dev.name;

                m_car_give_track = null;
                M_Car_Give_Track_Name = "";
            }
        }

        private async void SelectMoveCarGiveTrack()
        {
            if (!CheckSelectMoveCar()) return;
            Track cartrack = PubTask.Carrier.GetCarrierTrack(m_car_car.id);
            if(cartrack == null)
            {
                Growl.Warning("运输车当前没有轨道位置信息！");
                return;
            }

            DialogResult ingivetrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                        .Initialize<TrackSelectViewModel>((vm) =>
                        {
                            vm.SetAreaFilter(0, false);

                            TrackTypeE[] types;
                            if(cartrack.InType(TrackTypeE.下砖轨道, TrackTypeE.摆渡车_入, TrackTypeE.储砖_入))
                            {
                                types = new TrackTypeE[] { TrackTypeE.下砖轨道, TrackTypeE.储砖_入, TrackTypeE.储砖_出入};
                            }
                            else
                            {
                                types = new TrackTypeE[] { TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入 };
                            }

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
            if(!PubTask.Trans.AddMoveCarrierTask(cartrack.id, m_car_give_track.id, m_car_car.id, out string result))
            {
                Growl.Warning(result);
                return;
            }

            Growl.Success("添加成功！");
            CleanMoveCarInput();
        }
        #endregion

        private void CleanMoveStockInput()
        {
            M_Stock_Car_Name = "";
            M_Stock_Give_Track_Name = "";
            M_Stock_Take_Track_Name = "";
            M_Stock_Tile_Name = "";
        }

        private void TabSelected(RoutedEventArgs orgs)
        {
            //if (orgs != null && orgs.OriginalSource is System.Windows.Controls.TabControl pro && pro.SelectedItem is System.Windows.Controls.TabItem tab)
            //{
            //    Growl.Info(tab.Header.ToString());
            //}
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
