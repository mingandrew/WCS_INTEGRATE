using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using System;

namespace wcs.ViewModel
{
    public class CheckRightDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public CheckRightDialogViewModel()
        {
            _result = new MsgAction();
        }

        public MsgAction Result 
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }


        #region[字段]
        private MsgAction _result;
        private string password;
        private string comparepwd;

        #endregion

        #region[属性]

        public string PASSWORD
        {
            get => password;
            set => Set(ref password, value);
        }

        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;


        #endregion

        #region[方法]
        public void SetComparePWD(string pwd)
        {
            PASSWORD = "";
            comparepwd = pwd;
        }

        private void Comfirm()
        {

            if (string.IsNullOrEmpty(PASSWORD))
            {
                Growl.Warning("请输入认证密码！");
                return;
            }

            if (!comparepwd.Equals(PASSWORD))
            {
                Growl.Success("密码错误！");
                return;
            }

            Growl.Success("密码正确！");
            Result.o1 = true;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.o1 = false;
            CloseAction?.Invoke();
        }

        #endregion
    }
}
