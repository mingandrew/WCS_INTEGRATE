using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using module.role;
using resource;
using System;
using System.ComponentModel;
using System.Windows;
using task;
using tool.mlog;
using wcs.Dialog;
using wcs.toolbar;
using wcs.ViewModel;
using wcs.window;

namespace wcs
{
    /// <summary>
    /// NewMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        private Log mLog;
        public MainWindow()
        {
            InitializeComponent();
            mLog = (Log)new LogFactory().GetLog("系统", false);
        }


        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            NonClientAreaContent = new MainToolBarCtl();
        }


        /// <summary>
        /// 关闭窗口前的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e.Cancel = true;
                    ShowQuitDialogAsync();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async void ShowQuitDialogAsync()
        {
            MsgAction result = await HandyControl.Controls.Dialog.Show<OperateGrandDialog>()
                    .Initialize<OperateGrandDialogViewModel>((vm) => { vm.Clear(); vm.SetDialog(true); }).GetResultAsync<MsgAction>();
            if(result.o1 is null)
            {
                Growl.Error("退出失败，认证错误！");
                return;
            }

            if(result.o1 is int cint)
            {
                return;
            }
            
            if (result.o1 is WcsUser user)
            {
                if (user.exitwcs)
                {
                    mLog.Status(true, "调度关闭：" + user.name);
                    PubMaster.Warn.Stop();
                    PubTask.Stop();
                    PubMaster.StopMaster();
                    Environment.Exit(0);
                    return;
                }
                else
                {
                    mLog.Status(true, user.name + "：没有退出调度的权限！");
                }
                Growl.Error(user.name + "：没有退出调度的权限！");
            }
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PubMaster.StartMaster();
            PubTask.Start();
            Sprite.Show(new WaringCtl());
            mLog.Status(true, "调度启动");
        }
    }
}
