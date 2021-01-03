using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Tools.Extension;
using module.msg;
using System;

namespace wcs.ViewModel
{
    public class ChangeStrategyDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public ChangeStrategyDialogViewModel()
        {
            _result = new MsgAction();
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        private bool _showin;
        private StrategyInE _strategyin;
        private StrategyOutE _strategyout;
        private DevWorkTypeE _worktype;
        public StrategyInE STRATEGYIN
        {
            get => _strategyin;
            set => Set(ref _strategyin, value);
        }
        public StrategyOutE STRATEGYOUT
        {
            get => _strategyout;
            set => Set(ref _strategyout, value);
        }

        public DevWorkTypeE WORKTYPE
        {
            get => _worktype;
            set => Set(ref _worktype, value);
        }

        public bool SHOWIN
        {
            get => _showin;
            set => Set(ref _showin, value);
        }

        #region[字段]
        private MsgAction _result;
        #endregion

        #region[属性]

        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]

        private void Comfirm()
        {
            Result.o1 = true;
            if (SHOWIN)
            {
                Result.o2 = STRATEGYIN;
            }
            else
            {
                Result.o2 = STRATEGYOUT;
            }
            Result.o3 = WORKTYPE;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {

            Result.o1 = false;
            CloseAction?.Invoke();
        }

        public void SetShow(bool isdowntile)
        {
            SHOWIN = isdowntile;
        }

        internal void SetInStrategy(StrategyInE inStrategy, DevWorkTypeE worktype)
        {
            STRATEGYIN = inStrategy;
            WORKTYPE = worktype;
        }

        internal void SetOutStrategy(StrategyOutE outStrategy, DevWorkTypeE worktype)
        {
            STRATEGYOUT = outStrategy;
            WORKTYPE = worktype;
        }
        #endregion
    }
}
