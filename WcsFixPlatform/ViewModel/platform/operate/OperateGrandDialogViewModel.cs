using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using resource;
using System;
using System.Windows;

namespace wcs.ViewModel
{
    public class OperateGrandDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public OperateGrandDialogViewModel()
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
        private string username;
        private string password;
        private string titlename = "请认证";
        private Visibility showicon = Visibility.Collapsed;

        #endregion

        #region[属性]
        public Visibility SHOWICON
        {
            get => showicon;
            set => Set(ref showicon, value);
        }

        public string TITLENAME
        {
            get => titlename;
            set => Set(ref titlename, value);
        }

        public string USERNAME
        {
            get => username;
            set => Set(ref username, value);
        }
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
        public void SetDialog(bool isexistshow)
        {
            if (isexistshow)
            {
                TITLENAME = "退出调度认证！"; ;
                SHOWICON = Visibility.Visible;
            }
            else
            {
                TITLENAME = "授权认证！"; ;
                SHOWICON = Visibility.Collapsed;
            }
            USERNAME = "";
            PASSWORD = "";
        }

        private void Comfirm()
        {
            if (string.IsNullOrEmpty(USERNAME))
            {
                Growl.Warning("请输入用户名！");
                return;
            }

            if (string.IsNullOrEmpty(PASSWORD))
            {
                Growl.Warning("请输入认证密码！");
                return;
            }

            Result.o1 = PubMaster.Role.GetUser(USERNAME, PASSWORD);
            Result.o3 = USERNAME;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.o1 = -1;
            Result.o3 = USERNAME;
            CloseAction?.Invoke();
        }

        public void Clear()
        {
            PASSWORD = "";
        }

        #endregion
    }
}
