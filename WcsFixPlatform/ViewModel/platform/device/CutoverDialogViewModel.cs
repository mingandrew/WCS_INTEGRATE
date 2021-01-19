using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.msg;
using module.window;
using resource;
using System;
using task;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class CutoverDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public CutoverDialogViewModel()
        {
            _pregoodsname = "";
            _result = new MsgAction();
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        private uint _area, _devid, _goodsid, _pregoodid;
        private string _devname, _pregoodsname;
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

        public uint PREGOODSID
        {
            get => _pregoodid;
            set => Set(ref _pregoodid, value);
        }

        public string PREGOODSNAME
        {
            get => _pregoodsname;
            set => Set(ref _pregoodsname, value);
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
                PREGOODSID = good.ID;
                PREGOODSNAME = good.Name;
            }
        }

        private void Comfirm()
        {
            if (WORKMODE == TileWorkModeE.无)
            {
                Growl.Info("请选择新模式！");
                return;
            }
            if (PREGOODSID == 0)
            {
                Growl.Info("请选择新品种！");
                return;
            }

            if (!PubTask.TileLifter.IsOnline(_devid))
            {
                Growl.Warning("砖机离线！不能执行切换操作！");
                return;
            }

            if (!PubMaster.DevConfig.DoCutover(_devid, _goodsid, WORKMODE, PREGOODSID, out string msg))
            {
                Growl.Warning(msg);
                return;
            }
            Growl.Success("开始切换~");

            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            CloseAction?.Invoke();
        }

        public void SetArea(uint area, uint devid, string devname, uint goodid)
        {
            AREA = area;
            DEVNAME = devname;
            _devid = devid;
            _goodsid = goodid;
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
