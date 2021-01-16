using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Tools.Extension;
using HandyControlDemo.UserControl;
using wcs.ViewModel.setting.toolbar;

namespace wcs.ViewModel
{
    public abstract class MViewModel : ViewModelBase
    {
        //        private readonly string timeformat = "yyyy-MM-dd HH:mm:ss:ffff";

        #region[字段]
        #endregion

        #region[属性]
        #endregion

        #region[命令]
        #endregion

        #region[方法]
        #endregion

        public MViewModel()
        {

        }

        public string ModelName { set; get; }

        /// <summary>
        /// 默认激活
        /// </summary>
        public bool IsViewActive { set; get; } = true;


        protected MViewModel(string name) : base()
        {
            ModelName = name;
            Messenger.Default.Register<string>(this, MsgToken.TabItemSelected, TabItemSelected);
            //Messenger.Default.Register<string>(this, MsgToken.TabItemClosed, TabItemClosed);
        }


        protected MViewModel(IMessenger messenger) : base(messenger) { }

        /// <summary>
        /// 所在Tab页被激活
        /// </summary>
        protected abstract void TabActivate();
        protected abstract void TabDisActivate();


        public void TabItemSelected(string tagname)
        {
            if (ModelName.Equals(tagname))
            {
                IsViewActive = true;
                TabActivate();
            }
            else
            {
                IsViewActive = false;
                TabDisActivate();
            }
        }
        private void TabItemClosed(string tagname)
        {
            if (ModelName.Equals(tagname))
            {
                IsViewActive = false;
            }
        }
        private bool IsShowingDialog = false;
        /// <summary>
        /// 弹框提示 [ 1s ]
        /// </summary>
        /// <param name="msg"></param>
        public async void ShowMsg(string msg, bool isOK)
        {
            // Messenger.Default.Send(msg, MsgToken.DialogMsgShow);
            if (IsShowingDialog) return;
            IsShowingDialog = true;
            await HandyControl.Controls.Dialog.Show<TextDialogWithTimer>()
                    .Initialize<InteractiveDialogViewModel>((vm) => 
                    {
                        vm.SetMsg(msg, isOK);
                    }).GetResultAsync<string>();
            IsShowingDialog = false;
            //await HandyControl.Controls.Dialog.Show<TextDialogWithTimer>(msg).GetResultAsync<string>();
        }

    }
}
