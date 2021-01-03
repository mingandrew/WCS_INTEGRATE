using enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using module.msg;
using module.rf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using task;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class RfClientViewModel : MViewModel
    {
        public RfClientViewModel() : base("RfClient")
        {
            list = new ObservableCollection<RfClientView>();
            Messenger.Default.Register<MsgAction>(this, MsgToken.RfStatusUpdate, RfStatusUpdate);

            PubTask.Rf.GetAllClient();
        }


        #region[字段]

        private ObservableCollection<RfClientView> list;

        #endregion

        #region[属性]

        public ObservableCollection<RfClientView> List
        {
            get => list;
            set => Set(ref list, value);
        }
        #endregion

        #region[命令]        

        public RelayCommand<string> RfClientEditCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(RfClientEdit)).Value;


        #endregion

        #region[方法]
        private void RfClientEdit(string obj)
        {

        }
        private void RfStatusUpdate(MsgAction msg)
        {
            if (msg.o1 is RfClient client)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RfClientView view = List.FirstOrDefault(c => c.Ip.Equals(msg.Name));

                    if (view == null)
                    {
                        view = new RfClientView(client);
                        List.Add(view);
                    }
                    view.Update(client);
                });
            }
        }
        #endregion

        protected override void TabActivate()
        {

        }

        protected override void TabDisActivate()
        {

        }
    }
}
