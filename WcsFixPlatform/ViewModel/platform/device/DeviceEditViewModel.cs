using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Tools.Extension;
using module.window;
using System;

namespace wcs.ViewModel
{
    public class DeviceEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DeviceEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
        }

        #region[字段]

        private DialogResult _result;
       
        #endregion

        #region[属性]
       
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
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            CloseAction?.Invoke();
        }
        #endregion
    }
}
