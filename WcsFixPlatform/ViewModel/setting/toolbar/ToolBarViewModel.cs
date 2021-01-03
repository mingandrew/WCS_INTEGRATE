using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using module.role;
using resource;
using System;
using System.Windows;
using tool.mlog;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class ToolBarViewModel : ViewModelBase
    {

        private Log mLog;
        public ToolBarViewModel()
        {
            mLog = (Log)new LogFactory().GetLog("认证", false);
            btnname = "登陆";
            Primary = Application.Current.Resources["ButtonPrimary"] as Style;
            Danger = Application.Current.Resources["ButtonDanger"] as Style;

            btnstyle = Primary;
            LogOutOrInit();
        }

        #region[字段]
        private string btnname;
        private string username;
        private Style btnstyle;
        private bool islogin;
        private Style Danger, Primary;
        #endregion

        #region[属性]
        public string BtnName
        {
            get => btnname;
            set => Set(ref btnname, value);
        }

        public string UserName
        {
            get => username;
            set => Set(ref username, value);
        }

        public Style BtnStyle
        {
            get => btnstyle;
            set => Set(ref btnstyle, value);
        }

        #endregion

        #region[命令]
        public RelayCommand LoginOutCmd => new Lazy<RelayCommand>(() => new RelayCommand(DoLoginOut)).Value;
        #endregion

        #region[方法]
        private async void DoLoginOut()
        {
            //登陆
            if (!islogin)
            {
                MsgAction result = await HandyControl.Controls.Dialog.Show<OperateGrandDialog>()
                       .Initialize<OperateGrandDialogViewModel>((vm) => { vm.Clear(); vm.SetDialog(false); })
                       .GetResultAsync<MsgAction>();
                if (result.o1 is null)
                {
                    Growl.Error("用户密码错误，认证失败！");
                    if(result.o3 is string username)
                    {
                        mLog.Status(true, username + "：用户密码错误，认证失败！");
                    }
                    return;
                }

                //取消认证
                if (result.o1 is int cint)
                {
                    if (result.o3 is string username)
                    {
                        mLog.Status(true, username + "：取消认证！");
                    }
                    return;
                }

                if (result.o1 is WcsUser user)
                {
                    UserName = user.name;
                    MsgAction msg = new MsgAction()
                    {
                        o1 = user
                    };

                    Messenger.Default.Send(msg, MsgToken.OperateGrandUpdate);

                    mLog.Status(true, UserName + "：认证成功！");

                    PubMaster.Role.SetLoginUser(user.id);
                }

                BtnName = "登出";
                BtnStyle = Danger;
                islogin = true;
            }
            else//退出
            {
                LogOutOrInit();
            }
        }

        private void LogOutOrInit()
        {
            WcsUser guest = PubMaster.Role.GetGuestUser();
            if (guest != null)
            {
                MsgAction msg = new MsgAction()
                {
                    o1 = guest
                };
                Messenger.Default.Send(msg, MsgToken.OperateGrandUpdate);
                UserName = guest.name;
                PubMaster.Role.SetLoginUser(guest.id);
            }
            else
            {
                MsgAction msg = new MsgAction()
                {
                    o1 = true
                };
                Messenger.Default.Send(msg, MsgToken.OperateGrandUpdate);
                UserName = "普通用户";
            }
            BtnName = "登陆";
            BtnStyle = Primary;
            islogin = false;
        }
        #endregion


    }
}
