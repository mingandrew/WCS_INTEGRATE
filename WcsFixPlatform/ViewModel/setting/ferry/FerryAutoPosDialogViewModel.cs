using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.msg;
using enums;
using task;
using System.Windows;

namespace wcs.ViewModel
{
    public class FerryAutoPosDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public FerryAutoPosDialogViewModel()
        {
            _result = new MsgAction();
        }

        #region[字段]
        private Device selectferry;
        private MsgAction _result;
        private ushort starttrack;
        private byte tracknumber;
        private string ferryname;
        private byte maxtracknumber;//根据选中的轨道设置最大轨道数
        private DevFerryAutoPosE _devferryautopos;

        #endregion

        #region[属性]
        public Device SELECTFERRY
        {
            get => selectferry;
            set => Set(ref selectferry, value);
        }
        public ushort STARTTRACKCODE
        {
            get => starttrack;
            set => Set(ref starttrack, value);
        }

        public byte TRACKNUMBER
        {
            get => tracknumber;
            set => Set(ref tracknumber, value);
        }

        public DevFerryAutoPosE AUTOPOSSIDE
        {
            get => _devferryautopos;
            set => Set(ref _devferryautopos, value);
        }

        public string FerryName
        {
            get => ferryname;
            set => Set(ref ferryname, value);
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
        public RelayCommand ClearCmd => new Lazy<RelayCommand>(() => new RelayCommand(ClearOtherTrack)).Value;

        #endregion

        #region[方法]
        private void CancelChange()
        {
            Result.o1 = null;
            Result.o2 = null;
            Result.o3 = null;
            CloseAction?.Invoke();
        }

        public void SetDialog(ushort ferrycode, int maxtracknum)
        {
            maxtracknumber = (byte)maxtracknum;
            FerryName = SELECTFERRY.name;
            STARTTRACKCODE = ferrycode;
            TRACKNUMBER = (byte)maxtracknum;

            if (((STARTTRACKCODE > 300 && STARTTRACKCODE < 400 && (DeviceTypeE)SELECTFERRY.type == DeviceTypeE.下摆渡)
                || STARTTRACKCODE > 500 && STARTTRACKCODE < 600))
            {
                AUTOPOSSIDE = DevFerryAutoPosE.前侧对位;
            }
            if ((STARTTRACKCODE > 300 && STARTTRACKCODE < 400 && (DeviceTypeE)SELECTFERRY.type == DeviceTypeE.上摆渡)
                || (STARTTRACKCODE > 100 && STARTTRACKCODE < 200))
            {
                AUTOPOSSIDE = DevFerryAutoPosE.后侧对位;
            }
        }
        private void Comfirm()
        {
            if (TRACKNUMBER <= 0)
            {
                Growl.Warning("请输入对位轨道数量！");
                return;
            }

            if (TRACKNUMBER > maxtracknumber)
            {
                Growl.Warning("对位数量不能超过设置的数量");
                return;
            }

            if (AUTOPOSSIDE == 0)
            {
                Growl.Warning("请选择对位侧！");
                return;
            }

            if (!PubTask.Ferry.IsOnline(SELECTFERRY.id))
            {
                Growl.Warning("设备离线！");
                return;
            }

            string tipmes = "是否确认开始自动对位？\n\n并请在摆渡车对位完毕后点击上方【刷新轨道坐标】！\n以便重新获取对位数据！";
            MessageBoxResult rs = HandyControl.Controls.MessageBox.Show(tipmes, "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (rs == MessageBoxResult.No)
            {
                return;
            }

            PubTask.Ferry.AutoPosMsgSend(SELECTFERRY.id, AUTOPOSSIDE, STARTTRACKCODE, TRACKNUMBER, "PC");
            CloseAction?.Invoke();
        }

        internal void SetPosSide(DevFerryAutoPosE autoPosSide)
        {
            AUTOPOSSIDE = autoPosSide;
        }

        /// <summary>
        /// 清空摆渡车未配置的其他轨道对位信息
        /// </summary>
        private void ClearOtherTrack()
        {
            if (!PubTask.Ferry.IsOnline(SELECTFERRY.id))
            {
                Growl.Warning("设备离线！");
                return;
            }

            if (!PubTask.Ferry.DoClearOtherTrackPos(SELECTFERRY.id, out string rs))
            {
                Growl.Warning(rs);
                return;
            }
            Growl.Success("开始清理中！");
        }

        #endregion
    }
}
