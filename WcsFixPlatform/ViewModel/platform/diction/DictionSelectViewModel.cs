using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.diction;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class DictionSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DictionSelectViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _diclist = new ObservableCollection<Diction>();
        }

        #region[字段]

        private DialogResult _result;
        private ObservableCollection<Diction> _diclist;
        private Diction selectdic;

        #endregion

        #region[属性]
        public ObservableCollection<Diction> DicList
        {
            get => _diclist;
            set => Set(ref _diclist, value);
        }

        public Diction SelectDic
        {
            get => selectdic;
            set => Set(ref selectdic, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public DialogResult Param
        {
            set; get;
        }

        public Action CloseAction { get; set; }

        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]        
        private void CheckRadioBtn(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is RadioButton rbtn)
            {
                List<Diction> list = new List<Diction>();
                switch (rbtn.Tag.ToString())
                {
                    case "device":
                        list.AddRange(PubMaster.Dic.GetDicList(DictionTypeE.设备));
                        break;
                    case "task":
                        list.AddRange(PubMaster.Dic.GetDicList(DictionTypeE.任务));
                        break;
                    case "switch":
                        list.AddRange(PubMaster.Dic.GetDicList(DictionTypeE.开关));
                        break;
                    case "user":
                        list.AddRange(PubMaster.Dic.GetDicList(DictionTypeE.用户));
                        break;
                }
                DicList.Clear();
                foreach (Diction diction in list)
                {
                    DicList.Add(diction);
                }
}
        }

        private void Comfirm()
        {
            if (SelectDic == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = true;
            Result.p2 = SelectDic;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }
        #endregion
    }
}
