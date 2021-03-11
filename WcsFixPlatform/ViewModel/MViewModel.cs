using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using module.msg;
using resource;

namespace wcs.ViewModel
{
    public abstract class MViewModel : ViewModelBase
    {
        //        private readonly string timeformat = "yyyy-MM-dd HH:mm:ss:ffff";

        #region[字段]
        private bool admin; //管理员功能授权，是否显示
        private bool supervisor; //超级管理员功能授权，是否显示
        #endregion

        #region[属性]
        //管理员功能授权，是否显示
        public bool Admin
        {
            get => admin;
            set => Set(ref admin, value);
        }
        //超级管理员功能授权，是否显示
        public bool SuperVisor
        {
            get => supervisor;
            set => Set(ref supervisor, value);
        }
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
            Messenger.Default.Register<MsgAction>(this, MsgToken.AllowShow, AllowUserShow);  //注册 限制功能 的通知


            Admin = PubMaster.Role.MatchRolePrior(WcsRolePrior.管理员, null);
            SuperVisor = PubMaster.Role.MatchRolePrior(WcsRolePrior.超级管理员, null);
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


        //更新功能展示权限
        private void AllowUserShow(MsgAction msg)
        {
            if (msg != null && msg.o1 is bool ad && msg.o2 is bool super)
            {
                Admin = ad;
                SuperVisor = super;
            }
        }
    }
}
