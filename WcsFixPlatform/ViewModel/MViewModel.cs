using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

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
    }
}
