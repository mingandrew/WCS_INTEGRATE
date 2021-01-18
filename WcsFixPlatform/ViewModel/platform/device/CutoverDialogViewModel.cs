using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using module.window;
using System;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel.platform.device
{
    public class CutoverDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public CutoverDialogViewModel()
        {
            _goodsname = "";
            _result = new MsgAction();
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        private uint _area, _goodsid;
        private string _devname, _goodsname;
        private TileWorkModeE _workmode;
        public TileWorkModeE WORKMODE
        {
            get => _workmode;
            set => Set(ref _workmode, value);
        }

        public uint AREA
        {
            get => _area;
            set => Set(ref _area, value);
        }

        public uint GOODSID
        {
            get => _goodsid;
            set => Set(ref _goodsid, value);
        }

        public string GOODSNAME
        {
            get => _goodsname;
            set => Set(ref _goodsname, value);
        }

        public string DEVNAME
        {
            get => _devname;
            set => Set(ref _devname, value);
        }

        #region[字段]
        private MsgAction _result;
        #endregion

        #region[属性]

        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        public RelayCommand ChooseCmd => new Lazy<RelayCommand>(() => new RelayCommand(ChooseGoods)).Value;

        #endregion

        #region[方法]

        private async void ChooseGoods()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
             .Initialize<GoodsSelectViewModel>((vm) =>
             {
                 vm.SetAreaFilter(AREA, true);
                 vm.QueryGood();
             }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && result.p2 is GoodsView good)
            {
                GOODSID = good.ID;
                GOODSNAME = good.Name;
            }
        }

        private void Comfirm()
        {
            if (WORKMODE == TileWorkModeE.无)
            {
                Growl.Info("请选择新模式！");
                return;
            }
            if (GOODSID == 0)
            {
                Growl.Info("请选择新品种！");
                return;
            }
            Result.o1 = true;
            Result.o2 = WORKMODE;
            Result.o3 = GOODSID;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.o1 = false;
            CloseAction?.Invoke();
        }

        public void SetArea(uint area, string devname)
        {
            AREA = area;
            DEVNAME = devname;
        }

        public void SetWorkMode(TileWorkModeE workmode)
        {
            switch (workmode)
            {
                case TileWorkModeE.上砖:
                    WORKMODE = TileWorkModeE.下砖;
                    break;
                case TileWorkModeE.下砖:
                    WORKMODE = TileWorkModeE.上砖;
                    break;
                default:
                    WORKMODE = TileWorkModeE.过砖;
                    break;
            }
        }

        #endregion

    }
}
