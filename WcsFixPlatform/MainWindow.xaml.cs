using enums;
using HandyControl.Controls;
using HandyControl.Tools;
using HandyControl.Tools.Extension;
using module.msg;
using module.role;
using resource;
using simtask;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using task;
using tool.appconfig;
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
        private bool IsShowCloseDialog = false;
        public MainWindow()
        {
            InitializeComponent();
            mLog = (Log)new LogFactory().GetLog("系统", false); 

            HandyControl.Controls.Dialog.SetToken(this, MsgToken.MainDialog);
        }


        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            NonClientAreaContent = new MainToolBarCtl();

            GlobalShortcut.Init(new List<KeyBinding>
            {
                new KeyBinding(ViewModelLocator.Instance.Main.GlobalShortcutCmd, Key.Back, ModifierKeys.Control | ModifierKeys.Alt)
            });

            WindowAttach.SetIgnoreAltF4(this, true);
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
            if (GlobalWcsDataConfig.DebugConfig.IsDebug)
            {
                mLog.Status(true, "调度关闭[调试模式]");
                PubMaster.Warn.Stop();
                PubTask.Stop();
                PubMaster.StopMaster();
                SimServer.Stop();
                Environment.Exit(0);
                return;
            }
            if (OperateGrandDialogConst.IsOprerateDialogOpen) return;
            if (IsShowCloseDialog) return;
            IsShowCloseDialog = true;
            OperateGrandDialogConst.IsOprerateDialogOpen = true;
            MsgAction result = await HandyControl.Controls.Dialog.Show<OperateGrandDialog>(MsgToken.MainDialog)
                    .Initialize<OperateGrandDialogViewModel>((vm) => { vm.Clear(); vm.SetDialog(true); }).GetResultAsync<MsgAction>();

            IsShowCloseDialog = false;
            OperateGrandDialogConst.IsOprerateDialogOpen = false;
            if (result.o1 is null)
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
