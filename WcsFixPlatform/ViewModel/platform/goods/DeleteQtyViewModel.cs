using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.window;
using System;
using System.Collections.Generic;

namespace wcs.ViewModel
{
    public class DeleteQtyViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DeleteQtyViewModel()
        {
            DelectQtyList = new List<uint>();
            _result = new DialogResult();
        }

        #region[字段]
        private DialogResult _result;
        private uint delectqty;
        private List<uint> delectqtylist;
        #endregion

        #region[属性]
        public uint DelectQty
        {
            get => delectqty;
            set => Set(ref delectqty, value);
        }

        public List<uint> DelectQtyList
        {
            get => delectqtylist;
            set => Set(ref delectqtylist, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }
        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]
        private void Comfirm()
        {
            Result.p1 = true;
            Result.p2 = DelectQty;
            if (DelectQty == 0)
            {
                Growl.Warning("请选择合适的数量");
                return;
            }
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }

        public void SetDelectQtyList(int qty)
        {
            DelectQtyList.Clear();
            for (uint i = 1; i <= qty; i++)
            {
                DelectQtyList.Add(i);
            }
        }
        #endregion
    }
}
