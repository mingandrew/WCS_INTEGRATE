using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.role;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class BatchAddTileViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public BatchAddTileViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _devicetyelist = new ObservableCollection<Device>();
        }

        #region[字段]

        private DialogResult _result;
        private string title;

        private uint start_number;
        private string start_ip;
        private uint add_count;
        private uint car_length;

        private bool showcarlength;
        private bool showtype;
        private bool showtype2;
        private ObservableCollection<Device> _devicetyelist;
        private Device selectdevicetype;
        private DeviceType2E selecttype2;
        #endregion

        #region[属性]
        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public uint StartNumber
        {
            get => start_number;
            set => Set(ref start_number, value);
        }

        public string StartIP
        {
            get => start_ip;
            set => Set(ref start_ip, value);
        }

        public uint AddCount
        {
            get => add_count;
            set => Set(ref add_count, value);
        }
        

        public uint CarLength
        {
            get => car_length;
            set => Set(ref car_length, value);
        }

        public Device SelectDeviceType
        {
            get => (Device)selectdevicetype;
            set => Set(ref selectdevicetype, value);
        }

        public DeviceType2E SelectType2
        {
            get => selecttype2;
            set => Set(ref selecttype2, value);
        }

        public bool ShowCarLength
        {
            get => showcarlength;
            set => Set(ref showcarlength, value);
        }

        public bool ShowType
        {
            get => showtype;
            set => Set(ref showtype, value);
        }

        public bool ShowType2
        {
            get => showtype2;
            set => Set(ref showtype2, value);
        }

        public ObservableCollection<Device> DeviceTypeList
        {
            get => _devicetyelist;
            set => Set(ref _devicetyelist, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public DialogResult Param
        {
            set; get;
        }

        public Action CloseAction { get; set; }


        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]
        public void SetAddType(string tag)
        {
            switch (tag)
            {
                case "Tile":
                    #region[砖机]
                    ShowCarLength = false;
                    ShowType = true;
                    ShowType2 = true;
                    Title = "砖机";
                    DeviceTypeList.Clear();
                    DeviceTypeList.Add(new Device() { Type = DeviceTypeE.上砖机 });
                    DeviceTypeList.Add(new Device() { Type = DeviceTypeE.下砖机 });
                    DeviceTypeList.Add(new Device() { Type = DeviceTypeE.砖机 });
                    break;
                #endregion
                case "Carrier":
                    #region[运输车]
                    Title = "运输车";
                    ShowCarLength = true;
                    ShowType = false;
                    ShowType2 = false;
                    break;
                #endregion
                case "Ferry":
                    #region[摆渡车]
                    Title = "摆渡车";
                    ShowCarLength = false;
                    ShowType = true;
                    ShowType2 = false;
                    DeviceTypeList.Clear();
                    DeviceTypeList.Add(new Device() { Type = DeviceTypeE.上摆渡 });
                    DeviceTypeList.Add(new Device() { Type = DeviceTypeE.下摆渡 });
                    break;
                #endregion
                default:
                    break;
            }
        }


        private void Comfirm()
        {
            if (string.IsNullOrWhiteSpace(StartIP))
            {
                Growl.WarningGlobal("请输入起始IP！");
                return;
            }

            if (StartNumber <= 0)
            {
                Growl.WarningGlobal("请输入正确的起始编号！");
                return;
            }

            if (AddCount <= 0)
            {
                Growl.WarningGlobal("请输入新增数量！");
                return;
            }

            if (ShowType && SelectDeviceType == null)
            {
                Growl.WarningGlobal("请选择设备类型！");
                return;
            }

            Result.p1 = true;
            Result.p2 = StartNumber;
            Result.p3 = StartIP;
            Result.p4 = AddCount;
            if (ShowType)
            {
                Result.p5 = SelectDeviceType.Type;
            }
            if (ShowCarLength)
            {
                Result.p5 = CarLength;
            }
            if (ShowType2)
            {
                Result.p6 = SelectType2;
            }
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }
        #endregion
    }
}
