using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using module.device;
using module.msg;
using enums;
using wcs.Dialog;
using System.Collections.Generic;
using module.window;
using HandyControl.Tools.Extension;
using HandyControl.Controls;
using resource;
using module.track;
using System.Windows;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class FerryCopyPosDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public FerryCopyPosDialogViewModel()
        {
            _result = new MsgAction();
        }

        #region[字段]
        private Device to_ferry, from_ferry;
        private MsgAction _result;

        private string to_ferry_name, from_ferry_name;

        /// <summary>
        /// 复制同轨道
        /// </summary>
        const byte COPY_SAME_CODE = 0;
        /// <summary>
        /// 删除并添加
        /// </summary>
        const byte DEL_AND_COPY = 1;

        private byte Copy_Type = COPY_SAME_CODE;//默认复制同轨道

        #endregion

        #region[属性]
        public string ToFerryName
        {
            get => to_ferry_name;
            set => Set(ref to_ferry_name, value);
        }

        public string FromFerryName
        {
            get => from_ferry_name;
            set => Set(ref from_ferry_name, value);
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public Action CloseAction { get; set; }
        #endregion

        #region[命令]
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand SelectFerryCmd => new Lazy<RelayCommand>(() => new RelayCommand(SelectFromFerry)).Value;
        public RelayCommand<RoutedEventArgs> CheckTypeRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckTypeRadioBtn)).Value;

        #endregion

        #region[方法]
        public void InitVar()
        {
            from_ferry = null;
            to_ferry = null;
            FromFerryName = "";
            ToFerryName = "";
        }

        public void BeforeOpenDialog(Device toferry)
        {
            to_ferry = toferry;
            ToFerryName = to_ferry.name;
        }

        private async void SelectFromFerry()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                    .Initialize<DeviceSelectViewModel>((vm) =>
                    {
                        vm.FilterArea = false;
                        vm.AreaId = 0;
                        if(to_ferry.Type == DeviceTypeE.上摆渡)
                        {
                            vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.上摆渡 });
                        }
                        else
                        {
                            vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.下摆渡 });
                        }
                    }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                if(dev.id == to_ferry.id)
                {
                    Growl.Warning("复制给自己就没意思了吧！");
                    return;
                }
                from_ferry = dev;
                FromFerryName = from_ferry.name;
            }
        }

        private void CancelChange()
        {
            Result.o1 = null;
            Result.o2 = null;
            Result.o3 = null;
            CloseAction?.Invoke();
        }

        private void Comfirm()
        {
            if(from_ferry == null)
            {
                Growl.Warning("请先选择摆渡车");
                return;
            }

            List<FerryPos> topos = PubMaster.Track.GetFerryPos(to_ferry.area, to_ferry.id);

            if(topos.Count == 0)
            {
                if(Copy_Type == COPY_SAME_CODE)
                {
                    Growl.Warning("当前摆渡车没有轨道配置信息");
                    return;
                }
            }
            List<FerryPos> frompos = PubMaster.Track.GetFerryPos(from_ferry.area, from_ferry.id);

            if (frompos.Count == 0)
            {
                Growl.Warning("指定复制的摆渡车没有配置信息");
                return;
            }

            bool ishaveoldcolumn = PubMaster.Track.IsExistOldFerryPos();
            if(Copy_Type == COPY_SAME_CODE)
            {
                FerryPos pos = null;
                foreach (var item in topos)
                {
                    pos = frompos.Find(c => c.track_id == item.track_id && c.ferry_code == item.ferry_code);
                    if (pos != null)
                    {
                        if (item.ferry_pos != pos.ferry_pos)
                        {
                            if (item.ferry_pos > 0)
                            {
                                item.old_ferry_pos = item.ferry_pos;
                            }
                            item.ferry_pos = pos.ferry_pos;
                            PubMaster.Track.UpdateFerryPos(item, ishaveoldcolumn);
                        }
                    }
                }
            }

            Result.o1 = true;
            CloseAction?.Invoke();
        }


        private void CheckTypeRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (byte.TryParse(btn.Tag.ToString(), out byte type))
                {
                    Copy_Type = type;
                }
            }
        }
        #endregion
    }
}
