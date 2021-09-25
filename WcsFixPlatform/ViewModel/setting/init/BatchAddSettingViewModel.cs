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
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class BatchAddSettingViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public BatchAddSettingViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _tracktypelist = new ObservableCollection<TrackView>();
            _devicetyelist = new ObservableCollection<Device>();
        }

        #region[字段]

        private DialogResult _result;
        private string title;

        private uint start_number;
        private string suffix_name;
        private uint add_count;
        private List<uint> typelist;
        private byte selecttype;
        private uint width;
        private uint leftdistance;
        private uint rightdistance;

        private bool showtrack;
        private ObservableCollection<TrackView> _tracktypelist;
        private TrackView selecttracktype;

        private bool showdevice;
        private ObservableCollection<Device> _devicetyelist;
        private Device selectdevicetype;
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

        public string SuffixName
        {
            get => suffix_name;
            set => Set(ref suffix_name, value);
        }

        public uint AddCount
        {
            get => add_count;
            set => Set(ref add_count, value);
        }

        public List<uint> TypeList
        {
            get => typelist;
            set => Set(ref typelist, value);
        }

        public DeviceTypeE SelectType
        {
            get => (DeviceTypeE)selecttype;
            set => selecttype = (byte)value;
        }

        public uint Width
        {
            get => width;
            set => Set(ref width, value);
        }
        public uint LeftDistance
        {
            get => leftdistance;
            set => Set(ref leftdistance, value);
        }
        public uint RightDistance
        {
            get => rightdistance;
            set => Set(ref rightdistance, value);
        }

        public bool ShowTrack
        {
            get => showtrack;
            set => Set(ref showtrack, value);
        }

        public bool ShowDevice
        {
            get => showdevice;
            set => Set(ref showdevice, value);
        }

        public ObservableCollection<TrackView> TrackTypeList
        {
            get => _tracktypelist;
            set => Set(ref _tracktypelist, value);
        }

        public ObservableCollection<Device> DeviceTypeList
        {
            get => _devicetyelist;
            set => Set(ref _devicetyelist, value);
        }

        public TrackView SelectTrackType
        {
            get => selecttracktype;
            set => Set(ref selecttracktype, value);
        }

        public Device SelectDeviceType
        {
            get => selectdevicetype;
            set => Set(ref selectdevicetype, value);
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
                case "Track":
                    #region[轨道类型]
                    ShowTrack = true;
                    ShowDevice = false;
                    Title = "轨道";
                    TrackTypeList.Clear();
                    foreach (TrackTypeE typeE in Enum.GetValues(typeof(TrackTypeE)))
                    {
                        TrackTypeList.Add(new TrackView()
                        {
                            Type = typeE,
                        });
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }


        private void Comfirm()
        {
            if (string.IsNullOrWhiteSpace(SuffixName))
            {
                Growl.WarningGlobal("请输入名称后缀！");
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

            if (ShowDevice && SelectDeviceType == null)
            {
                Growl.WarningGlobal("请选择设备类型！");
                return;
            }

            if (ShowTrack && SelectTrackType == null)
            {
                Growl.WarningGlobal("请选择轨道类型！");
                return;
            }

            Result.p1 = true;
            Result.p2 = StartNumber;
            Result.p3 = SuffixName;
            Result.p4 = AddCount;
            Result.p5 = SelectTrackType.Type;
            Result.p6 = Width;
            Result.p7 = LeftDistance;
            Result.p8 = RightDistance;
            if (ShowDevice)
            {
                Result.p5 = SelectDeviceType.Type;
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
