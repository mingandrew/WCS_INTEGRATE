using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using module.track;
using module.window;
using resource;
using System;
using task;
using wcs.Data.View;
using wcs.Dialog;
using wcs.Dialog.platform.track;

namespace wcs.ViewModel.platform.track
{
    public class LocationDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public LocationDialogViewModel()
        {
            _result = new MsgAction();
        }

        #region[字段]
        private MsgAction _result;

        private uint _area, _devid, _trackcode, _point;
        private string _devname, _trackname;
        private DeviceTypeE _devtype;
        private TrackTypeE _tracktype;
        private CarrierPosE _pointtype;
        private DevMoveDirectionE _movedir;
        private bool _iscarrier;
        #endregion

        #region[属性]

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        public uint AREA
        {
            get => _area;
            set => Set(ref _area, value);
        }

        public uint DEVID
        {
            get => _devid;
            set => Set(ref _devid, value);
        }

        public uint TRACKCODE
        {
            get => _trackcode;
            set => Set(ref _trackcode, value);
        }

        public uint POINT
        {
            get => _point;
            set => Set(ref _point, value);
        }

        public string DEVNAME
        {
            get => _devname;
            set => Set(ref _devname, value);
        }

        public string TRACKNAME
        {
            get => _trackname;
            set => Set(ref _trackname, value);
        }

        public DeviceTypeE DEVTYPE
        {
            get => _devtype;
            set => Set(ref _devtype, value);
        }

        public TrackTypeE TRACKTYPE
        {
            get => _tracktype;
            set => Set(ref _tracktype, value);
        }

        public CarrierPosE POINTTYPE
        {
            get => _pointtype;
            set => Set(ref _pointtype, value);
        }

        public DevMoveDirectionE MOVEDIR
        {
            get => _movedir;
            set => Set(ref _movedir, value);
        }

        public bool ISCARRIER
        {
            get => _iscarrier;
            set => Set(ref _iscarrier, value);
        }
        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        public RelayCommand ChooseTrackCmd => new Lazy<RelayCommand>(() => new RelayCommand(ChooseTrack)).Value;
        public RelayCommand ChoosePointCmd => new Lazy<RelayCommand>(() => new RelayCommand(ChoosePoint)).Value;

        #endregion

        #region[方法]

        private async void ChooseTrack()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
             .Initialize<TrackSelectViewModel>((vm) =>
             {
                 if (ISCARRIER)
                 {
                     vm.SetAreaFilter(AREA, true);
                     vm.QueryAreaTrack(AREA, TrackTypeE.上砖轨道, TrackTypeE.下砖轨道);
                 }
                 else
                 {
                     vm.SetAreaFilter(0, false);
                     vm.QueryFerryTrack(DEVID);
                 }
             }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                TRACKNAME = tra.name;
                TRACKTYPE = tra.Type;
                TRACKCODE = tra.ferry_up_code;

                // 清掉复位点
                POINT = 0;
                POINTTYPE = 0;
            }
        }

        private async void ChoosePoint()
        {
            if (TRACKCODE == 0)
            {
                Growl.Info("请先选择轨道！");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<CarrierPosSelectDialog>()
             .Initialize<CarrierPosSelectViewModel>((vm) =>
             {
                 vm.QueryCarrierPos(AREA, TRACKTYPE);
             }).GetResultAsync<DialogResult>();

            if (result.p1 is CarrierPos cp)
            {
                POINT = cp.track_point;
                POINTTYPE = cp.CarrierPosType;
            }
        }

        private void Comfirm()
        {
            if (string.IsNullOrEmpty(TRACKNAME))
            {
                Growl.Info("请选择轨道号！");
                return;
            }

            if (MOVEDIR == DevMoveDirectionE.无)
            {
                Growl.Info("请选择方向！");
                return;
            }

            // 初始化指令
            switch (DEVTYPE)
            {
                case DeviceTypeE.上摆渡:
                case DeviceTypeE.下摆渡:

                    // 复位指令
                    break;

                case DeviceTypeE.运输车:
                    if (POINT == 0)
                    {
                        Growl.Info("请选择复位点！");
                        return;
                    }

                    // 复位指令
                    break;

                default:
                    break;
            }

            Growl.Success("初始化指令发送成功");
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            CloseAction?.Invoke();
        }

        public void SetArea(uint area, uint devid, DeviceTypeE dt, string devname)
        {
            AREA = area;
            DEVID = devid;
            DEVTYPE = dt;
            DEVNAME = devname;
            ISCARRIER = DEVTYPE == DeviceTypeE.运输车;
        }

        #endregion

    }
}
