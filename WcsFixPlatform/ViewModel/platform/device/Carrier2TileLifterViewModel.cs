using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcs.ViewModel.platform.device
{
    public class Carrier2TileLifterViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public  Carrier2TileLifterViewModel(){
            _result = new DialogResult();           
        }

        #region[字段]

        private DialogResult _result;
        private Track track;
        private List<Device> deviceList;
        public Device _selectdevice;
        #endregion

        #region[属性]

        public Track TRACK
        {
            get => track;
            set => Set(ref track, value);
        }

        public List<Device> DeviceList
        {
            get => deviceList;
            set => Set(ref deviceList, value);
        }

        public Device SelectDevice
        {
            get => _selectdevice;
            set => Set(ref _selectdevice, value);
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

        private void Comfirm()
        {
            if (SelectDevice == null)
            {
                Growl.Warning("请选择设备");
                return;
            }
            Result.p1 = PubMaster.DevConfig.GetTileSite(SelectDevice.id, TRACK.id);
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = null;
            CloseAction?.Invoke();
        }
        #endregion

    }
}
