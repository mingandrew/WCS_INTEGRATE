using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using module;
using resource;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using wcs.Data.View;

namespace wcs.ViewModel
{
    /// <summary>
    /// 警告信息类
    /// </summary>
    public class WaringViewModel : ViewModelBase
    {
        public WaringViewModel()
        {
            Messenger.Default.Register<Warning>(this, MsgToken.WarningUpdate, WarningUpdate);
            PubMaster.Warn.GetWarns();
        }

        #region[字段]
        private Visibility showstatusok = Visibility.Visible, showstatuserror = Visibility.Hidden;
        private Visibility showerrorlist = Visibility.Hidden;
        private int errorcount;
        private WarningView selectwarn;
        #endregion

        #region[属性]
        public ObservableCollection<WarningView> WarnList { get; set; } = new ObservableCollection<WarningView>();
        public int ERRORCOUNT
        {
            get => errorcount;
            set => Set(ref errorcount, value);
        }

        public Visibility SHOWSTATUSOK
        {
            get => showstatusok;
            set => Set(ref showstatusok, value);
        }

        public Visibility SHOWSTATUSERROR
        {
            get => showstatuserror;
            set => Set(ref showstatuserror, value);
        }

        public Visibility SHOWERRORLIST
        {
            get => showerrorlist;
            set => Set(ref showerrorlist, value);
        }

        public WarningView SelectedWarn
        {
            get => selectwarn;
            set => Set(ref selectwarn, value);
        }
        #endregion

        #region[命令]

        public RelayCommand ShowErrorListCmd => new Lazy<RelayCommand>(() => new RelayCommand(ShowErrorList)).Value;
        public RelayCommand ClearnALertCmd => new Lazy<RelayCommand>(() => new RelayCommand(ClearnALert)).Value;
        public RelayCommand<string> WarnActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(WarnAction)).Value;


        #endregion

        #region[方法]


        private void WarnAction(string tag)
        {
            if (SelectedWarn == null) return;
            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1://取消任务
                        PubMaster.Warn.RemoveWarning(SelectedWarn.ID);
                        break;
                }
            }
        }


        private void ShowErrorList()
        {
            SHOWERRORLIST = SHOWERRORLIST == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void WarningUpdate(Warning md)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WarningView warn = WarnList.FirstOrDefault(c => c.ID == md.id);
                if (warn != null)
                {
                    if(md.resolve)
                    {
                        WarnList.Remove(warn);
                    }
                }
                else
                {
                    WarnList.Add(new WarningView(md));
                }
                ERRORCOUNT = WarnList.Count;

                CheckErrorPanelShow();
            });
        }

        private void CheckErrorPanelShow()
        {
            if (WarnList.Count > 0)
            {
                SHOWSTATUSERROR = Visibility.Visible;
                SHOWSTATUSOK = Visibility.Hidden;
            }
            else
            {
                SHOWSTATUSERROR = Visibility.Hidden;
                SHOWERRORLIST = Visibility.Hidden;
                SHOWSTATUSOK = Visibility.Visible;
            }
        }


        private void ClearnALert()
        {
            WarnList.Clear();
            CheckErrorPanelShow();
            PubMaster.Warn.GetWarns();
        }
        #endregion

    }
}
