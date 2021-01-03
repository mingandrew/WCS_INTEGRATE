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
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class AddManualTransViewModel : MViewModel
    {
        public AddManualTransViewModel() : base("AddManualTrans")
        {

        }

        #region[字段]      

        private Device in_dev, out_dev;
        private uint in_goods_id,out_goods_id;
        private uint in_take_track_id, out_take_track_id;
        private uint in_give_track_id, out_give_track_id;
        private uint in_tilelifter_id, out_tilelifter_id;
        private bool is_in_double_track, is_out_double_track;

        private uint in_take_left_trackid, in_take_right_trackid;
        private uint out_give_left_trackid, out_give_right_trackid;

        private bool in_left_track_check, in_right_track_check;
        private bool out_left_track_check, out_right_track_check;
        #endregion

        #region[属性]

        public uint In_Goods_id
        {
            get => in_goods_id;
            set => Set(ref in_goods_id, value);
        }

        public uint Out_Goods_id
        {
            get => out_goods_id;
            set => Set(ref out_goods_id, value);
        }

        public uint In_Take_track_id
        {
            get => in_take_track_id;
            set => Set(ref in_take_track_id, value);
        }
        public uint Out_Take_track_id
        {
            get => out_take_track_id;
            set => Set(ref out_take_track_id, value);
        }

        public uint In_Give_track_id
        {
            get => in_give_track_id;
            set => Set(ref in_give_track_id, value);
        }

        public uint Out_Give_track_id
        {
            get => out_give_track_id;
            set => Set(ref out_give_track_id, value);
        }

        public uint In_Tilelifter_id
        {
            get => in_tilelifter_id;
            set => Set(ref in_tilelifter_id, value);
        }

        public uint Out_Tilelifter_id
        {
            get => out_tilelifter_id;
            set => Set(ref out_tilelifter_id, value);
        }

        public bool Is_In_Double_Track
        {
            get => is_in_double_track;
            set => Set(ref is_in_double_track, value);
        }

        public bool Is_Out_Double_Track
        {
            get => is_out_double_track;
            set => Set(ref is_out_double_track, value);
        }

        public uint In_Take_Left_TrackId
        {
            get => in_take_left_trackid;
            set => Set(ref in_take_left_trackid, value);
        }

        public uint In_Take_Right_TrackId
        {
            get => in_take_right_trackid;
            set => Set(ref in_take_right_trackid, value);
        }


        public uint Out_Give_Left_TrackId
        {
            get => out_give_left_trackid;
            set => Set(ref out_give_left_trackid, value);
        }

        public uint Out_Give_Right_TrackId
        {
            get => out_give_right_trackid;
            set => Set(ref out_give_right_trackid, value);
        }

        public bool In_Left_Track_Check
        {
            get => in_left_track_check;
            set => Set(ref in_left_track_check, value);
        }

        public bool In_Right_Track_Check
        {
            get => in_right_track_check;
            set => Set(ref in_right_track_check, value);
        }

        public bool Out_Left_Track_Check
        {
            get => out_left_track_check;
            set => Set(ref out_left_track_check, value);
        }

        public bool Out_Right_Track_Check
        {
            get => out_right_track_check;
            set => Set(ref out_right_track_check, value);
        }
        #endregion

        #region[命令]    
        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;
        public RelayCommand<string> TaskActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TaskAction)).Value;

        #endregion

        #region[方法]

        private async void TaskAction(string tag)
        {
            if(int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    #region[添加手动入库]

                    case 1:
                        #region[选择下砖机]
                        DialogResult inresult = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                           .Initialize<DeviceSelectViewModel>((vm) =>
                           {
                               vm.FilterArea = false;
                               vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.下砖机 });
                           }).GetResultAsync<DialogResult>();
                        if (inresult.p1 is bool rs && inresult.p2 is Device indev)
                        {
                            ClearInTaskInput();
                            if (indev.goods_id == 0)
                            {
                                Growl.Warning("请先设置砖机规格！");
                                return;
                            }
                            in_dev = indev;
                            In_Tilelifter_id = indev.id;
                            In_Goods_id = indev.goods_id;

                            if(indev.Type2 == DeviceType2E.单轨)
                            {
                                In_Take_track_id = indev.left_track_id;
                                Is_In_Double_Track = false;
                            }
                            else
                            {
                                In_Take_Left_TrackId = indev.left_track_id;
                                In_Take_Right_TrackId = indev.right_track_id;
                                Is_In_Double_Track = true;
                            }
                        }
                        #endregion
                        break;
                    case 2://选择取砖轨道

                        break;
                    case 3://选择放砖轨道
                        if (!CheckInDev()) return;
                        DialogResult ingivetrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                        .Initialize<TrackSelectViewModel>((vm) =>
                        {
                            vm.SetAreaFilter(0, false);
                            vm.QueryTileTrack(in_dev.area, in_dev.id);
                        }).GetResultAsync<DialogResult>();
                        if (ingivetrars.p1 is Track tra)
                        {
                            if(tra.StockStatus == TrackStockStatusE.满砖)
                            {
                                Growl.Warning("轨道满砖了，请选择其他轨道！");
                                return;
                            }
                            In_Give_track_id = tra.id;
                        }
                        break;
                    case 4://清空信息
                        ClearInTaskInput();
                        break;
                    case 5://添加入库任务
                        AddInTrans();
                        break;

                    #endregion

                    #region[添加手动出库]

                    case 6:
                        #region[选择上砖机]
                        DialogResult outresult = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                           .Initialize<DeviceSelectViewModel>((vm) =>
                           {
                               vm.FilterArea = false;
                               vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.上砖机 });
                           }).GetResultAsync<DialogResult>();
                        if (outresult.p1 is bool outrs && outresult.p2 is Device outdev)
                        {
                            ClearOutTaskInput();
                            if(outdev.goods_id == 0)
                            {
                                Growl.Warning("请先设置砖机规格！");
                                return;
                            }
                            out_dev = outdev;
                            Out_Tilelifter_id = outdev.id;
                            Out_Goods_id = outdev.goods_id;
                            if(outdev.Type2 == DeviceType2E.单轨)
                            {
                                Out_Give_track_id = outdev.left_track_id;
                                Is_Out_Double_Track = false;
                            }
                            else
                            {
                                Out_Give_Left_TrackId = outdev.left_track_id;
                                Out_Give_Right_TrackId = outdev.right_track_id;
                                Is_Out_Double_Track = true;
                            }
                        }
                        #endregion
                        break;
                    case 7://选择取砖轨道
                        if (!CheckOutDev()) return;
                        DialogResult outtaketrars = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                        .Initialize<TrackSelectViewModel>((vm) =>
                        {
                            vm.SetAreaFilter(0, false);
                            vm.QueryTileTrack(out_dev.area, out_dev.id);
                        }).GetResultAsync<DialogResult>();
                        if (outtaketrars.p1 is Track outtaketra)
                        {
                            Out_Take_track_id = outtaketra.id;
                        }
                        break;
                    case 8://选择放砖轨道

                        break;
                    case 9://清空信息
                        ClearOutTaskInput();
                        break;
                    case 10://添加出库任务
                        AddOutTrans();
                        break;

                    #endregion
                }
            }
        }

        private void AddInTrans()
        {
            if (!CheckInDev()) return;
            if(in_dev.Type2 == DeviceType2E.双轨)
            {
                if(!In_Left_Track_Check && !In_Right_Track_Check)
                {
                    Growl.Warning("请选择取砖轨道！");
                    return;
                }
            }

            uint taketracid = in_dev.left_track_id;
            if(in_dev.Type2 == DeviceType2E.双轨 && In_Right_Track_Check) 
            {
                taketracid = in_dev.right_track_id;
            }

            if (!PubTask.Trans.AddManualTrans(in_dev.area, in_dev.id, TransTypeE.手动入库, 
                in_dev.goods_id, taketracid, in_give_track_id, TransStatusE.调度设备, out string result))
            {
                Growl.Warning(result);
            }
            else
            {
                Growl.Success("添加成功！");
                ClearInTaskInput();
            }
        }

        private void AddOutTrans()
        {
            if (!CheckOutDev()) return;
            if (out_dev.Type2 == DeviceType2E.双轨)
            {
                if (!Out_Left_Track_Check && !Out_Right_Track_Check)
                {
                    Growl.Warning("请选择放砖轨道！");
                    return;
                }
            }

            uint givetrack = out_dev.left_track_id;
            if (out_dev.Type2 == DeviceType2E.双轨 && In_Right_Track_Check)
            {
                givetrack = out_dev.right_track_id;
            }

            if (!PubTask.Trans.AddManualTrans(out_dev.area, out_dev.id, TransTypeE.手动出库,
                out_dev.goods_id, out_take_track_id, givetrack, TransStatusE.调度设备, out string result))
            {
                Growl.Warning(result);
            }
            else
            {
                Growl.Success("添加成功！");
                ClearOutTaskInput();
            }
        }

        private bool CheckInDev()
        {
            if(in_dev == null)
            {
                Growl.Warning("请先选择下砖机!");
                return false;
            }
            return true;
        }

        private bool CheckOutDev()
        {
            if(out_dev == null)
            {
                Growl.Warning("请先选择上砖机!");
                return false;
            }
            return true;
        }

        private void ClearInTaskInput()
        {
            in_dev = null;
            In_Tilelifter_id = 0;
            In_Goods_id = 0;
            In_Take_track_id = 0;
            In_Give_track_id = 0;
            In_Take_Left_TrackId = 0;
            In_Take_Right_TrackId = 0;
            In_Left_Track_Check = false;
            In_Right_Track_Check = false;
        }

        private void ClearOutTaskInput()
        {
            out_dev = null;
            Out_Tilelifter_id = 0;
            Out_Goods_id = 0;
            Out_Take_track_id = 0;
            Out_Give_track_id = 0;
            Out_Give_Left_TrackId = 0;
            Out_Give_Right_TrackId = 0;
            Out_Left_Track_Check = false;
            Out_Right_Track_Check = false;
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
