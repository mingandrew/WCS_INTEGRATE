using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Tools.Extension;
using module.diction;
using module.msg;
using resource;
using System;
using System.Collections.Generic;

namespace wcs.ViewModel
{
    public class ChangeLevelTypeViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public ChangeLevelTypeViewModel()
        {
            _result = new MsgAction();
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        private List<DictionDtl> levels;
        private DictionDtl selectlevel;
        private string title;

        public List<DictionDtl> LevelList
        {
            get => levels;
            set => Set(ref levels, value);
        }


        public DictionDtl SelectLevel
        {
            get => selectlevel;
            set => Set(ref selectlevel, value);
        }

        public string Title
        {
            get => title;
            set => Set(ref title, value);
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
            Result.o2 = SelectLevel.int_value;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {

            Result.o1 = false;
            CloseAction?.Invoke();
        }


        internal void SetLevels(LevelTypeE lte, byte lel)
        {
            Title = "请选择";
            switch (lte)
            {
                case LevelTypeE.TileLevel:
                    Title += "等级";
                    LevelList = PubMaster.Dic.GetDicDtls(DicTag.TileLevel);
                    break;
                case LevelTypeE.TileSite:
                    Title += "窑位";
                    LevelList = PubMaster.Dic.GetDicDtls(DicTag.TileSite);
                    break;
                default:
                    break;
            }
            SelectLevel = GetLevelDtl(lel);
        }

        private DictionDtl GetLevelDtl(uint levelvalue)
        {
            return LevelList.Find(c => c.int_value == levelvalue);
        }

        #endregion
    }
}
