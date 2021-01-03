using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.role;
using module.window;
using resource;
using System;
using System.Collections.ObjectModel;

namespace wcs.ViewModel
{
    public class MenuSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public MenuSelectViewModel()
        {
            _result = new DialogResult();
            MenuList = new ObservableCollection<WcsMenu>();
        }

        #region[字段]

        private DialogResult _result;
        private WcsMenu selectmd;

        #endregion

        #region[属性]

        public ObservableCollection<WcsMenu> MenuList { set; get; }

        public WcsMenu SelectMenu
        {
            get => selectmd;
            set => Set(ref selectmd, value);
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
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        #endregion

        #region[方法]

        public void QueryMenu()
        {
            MenuList.Clear();
            foreach (var item in PubMaster.Role.GetLoginPriorMenuList())
            {
                MenuList.Add(item);
            }
        }

        private void Comfirm()
        {
            if (SelectMenu == null)
            {
                Growl.Warning("请选择模块！");
                return;
            }
            Result.p1 = true;
            Result.p2 = SelectMenu;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            Result.p2 = null;
            CloseAction?.Invoke();
        }

        #endregion
    }
}
