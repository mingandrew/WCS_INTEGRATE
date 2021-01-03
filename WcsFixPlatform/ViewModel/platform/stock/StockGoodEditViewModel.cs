using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.track;
using module.window;
using System;

namespace wcs.ViewModel
{
    public class StockGoodEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public StockGoodEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
        }

        #region[字段]

        private DialogResult _result;

        private uint trackid;
        private uint oldgoodid, newgoodid;
        private bool isupdatedate;
        private DateTime? newdate;

        #endregion

        #region[属性]
        
        public uint TrackId
        {
            get => trackid;
            set => Set(ref trackid, value);
        }
        public uint OldGoodsId
        {
            get => oldgoodid;
            set => Set(ref oldgoodid, value);
        }

        public uint NewGoodsId
        {
            get => newgoodid;
            set => Set(ref newgoodid, value);
        }
        
        public bool IsUpdateDate
        {
            get => isupdatedate;
            set => Set(ref isupdatedate, value);
        }

        public DateTime? NewDate
        {
            get => newdate;
            set => Set(ref newdate, value);
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
        public void SetInitValue(uint trackid, uint ogoodid, uint ngoodid)
        {
            TrackId = trackid;
            OldGoodsId = ogoodid;
            NewGoodsId = ngoodid;
            IsUpdateDate = false;
            NewDate = null;
        }


        private void Comfirm()
        {
            Result.p2 = false;
            if (IsUpdateDate)
            {
                if(NewDate== null)
                {
                    Growl.Warning("请选择更改后的日期");
                    return;
                }
                Result.p2 = true;
                Result.p3 = NewDate;
            }
            Result.p1 = true;
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
