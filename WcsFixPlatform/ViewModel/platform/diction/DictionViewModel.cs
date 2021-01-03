using enums;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.diction;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class DictionViewModel : MViewModel
    {
        public DictionViewModel() : base("Diction")
        {
            _dictionList = new ObservableCollection<Diction>();
            _dictionDtlList = new ObservableCollection<DictionDtl>();
            _dtlList = new List<DictionDtl>();
            ItemRefresh();
        }

        #region[字段]

        private List<DictionDtl> _dtlList;
        private ObservableCollection<Diction> _dictionList;
        private ObservableCollection<DictionDtl> _dictionDtlList;
        private Diction _dictionselected;
        private DictionDtl _dictiondtlselected;
        private bool isaddenable, isediteenable, isdeleteenable;
        private DictionTypeE type;
        private bool intvis, boolvis, stringvis, doublevis, uintvis;

        private DateTime refreshtime;
        #endregion

        #region[属性]
        public DictionTypeE TYPE
        {
            get => type;
            set
            {
                if (Set(ref type, value))
                {
                    ItemRefresh();
                }
            }
        }
        public ObservableCollection<Diction> DictionList
        {
            get => _dictionList;
            set => Set(ref _dictionList, value);
        }

        public ObservableCollection<DictionDtl> DictionDtlList
        {
            get => _dictionDtlList;
            set => Set(ref _dictionDtlList, value);
        }

        public Diction DictionSelected
        {
            get => _dictionselected;
            set => Set(ref _dictionselected, value);
        }

        public DictionDtl DictionDtlSelected
        {
            get => _dictiondtlselected;
            set => Set(ref _dictiondtlselected, value);
        }

        public bool ISADDENABLE
        {
            get => isaddenable;
            set => Set(ref isaddenable, value);
        }

        public bool ISEDITEENABLE
        {
            get => isediteenable;
            set => Set(ref isediteenable, value);
        }

        public bool ISDELETEENABLE
        {
            get => isdeleteenable;
            set => Set(ref isdeleteenable, value);
        }

        public bool IntVis
        {
            get => intvis;
            set => Set(ref intvis, value);
        }
        public bool BoolVis
        {
            get => boolvis;
            set => Set(ref boolvis, value);
        }
        public bool StringVis
        {
            get => stringvis;
            set => Set(ref stringvis, value);
        }
        public bool DoubleVis
        {
            get => doublevis;
            set => Set(ref doublevis, value);
        }
        public bool UIntVis
        {
            get => uintvis;
            set => Set(ref uintvis, value);
        }

        #endregion

        #region[命令]

        public RelayCommand<string> DictionEditCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DictionEdit)).Value;
        public RelayCommand<string> DictionDtlEditeCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DictionDtlEdit)).Value;

        public RelayCommand DictionSelectedChangeCmd => new Lazy<RelayCommand>(() => new RelayCommand(DictionSelectedChange)).Value;
        public RelayCommand DictionDtlSelectedChangeCmd => new Lazy<RelayCommand>(() => new RelayCommand(DictionDtlSelectedChange)).Value;

        #endregion

        #region[方法]

        private async void DictionEdit(string tag)
        {
            switch (tag)
            {
                case "refresh":
                    ItemRefresh();
                    break;
                case "add":
                    DialogResult result = await HandyControl.Controls.Dialog.Show<DictionEditDialog>()
                    .Initialize<DictionEditViewModel>((vm) =>
                    {
                        vm.Param.p1 = TYPE;
                        vm.SetActionType(true);
                    }).GetResultAsync<DialogResult>();
                    if (result.p1 is bool rs)
                    {
                        //MsgHelper.SendAction(SQLDataE.QueryDiction);
                        if (rs)
                        {
                            ItemRefresh();
                            Growl.Success("添加成功！");
                        }
                        else
                        {
                            Growl.Warning("添加失败！");
                        }
                    }
                    break;
                case "edit":
                    if(DictionSelected == null)
                    {
                        Growl.Warning("请选中后再修改！");
                        return;
                    }
                    result = await HandyControl.Controls.Dialog.Show<DictionEditDialog>()
                              .Initialize<DictionEditViewModel>((vm) =>
                              {
                                  vm.Param.p1 = DictionSelected;
                                  vm.SetActionType(false);
                              }).GetResultAsync<DialogResult>();
                    if (result.p1 is bool rs2)
                    {
                        //MsgHelper.SendAction(SQLDataE.QueryDiction);
                        if (rs2)
                        {
                            ItemRefresh();
                            Growl.Success("修改成功！");
                        }
                        else
                        {
                            Growl.Warning("修改失败！");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private async void DictionDtlEdit(string tag)
        {
            bool isadd = false;
            bool isdelete = false;
            switch (tag)
            {
                case "AddDicDtl":
                    isadd = true;
                    break;
                case "EditDicDtl":
                    isadd = false;
                    break;
                case "DeleteDicDtl":
                    isdelete = true;
                    break;
                default:
                    break;
            }

            if (isdelete)
            {
                Growl.Info("[TODO]删除子字典");
                return;
            }

            if (DictionSelected == null)
            {
                Growl.Info("请先选择项目的数据!");
                return;
            }

            if (!isadd && DictionDtlSelected == null)
            {
                Growl.Info("请先选择修改的数据!");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<DictionDtlEditDialog>()
               .Initialize<DictionDtlEditViewModel>((vm) =>
               {
                   vm.Param.p1 = DictionSelected;
                   vm.Param.p2 = DictionDtlSelected;
                   vm.SetActionType(isadd);
               }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && rs)
            {
                //MsgHelper.SendAction(SQLDataE.QueryDiction);
                ItemRefresh(true);
            }
        }

        private void ItemRefresh(bool isrefreshdtl = false)
        {
            if ((DateTime.Now - refreshtime).TotalSeconds < 2)
            {
                Growl.Warning("请不用频繁刷新!");
                return;
            }
            refreshtime = DateTime.Now;

            if (!isrefreshdtl)
            {
                DictionList.Clear();
                List<Diction> dics = PubMaster.Dic.GetDicList(TYPE);
                foreach (Diction dic in dics)
                {
                    DictionList.Add(dic);
                }

                SetActionBtn(null);
            }

            DictionDtlList.Clear();
            _dtlList = PubMaster.Dic.GetDicDtlList();

            DictionSelectedChange();
        }

        private void DictionSelectedChange()
        {
            DictionDtlSelected = null;
            DictionDtlList.Clear();

            if (DictionSelected != null)
            {
                foreach (DictionDtl dic in _dtlList.FindAll(c => c.diction_id == DictionSelected.id))
                {

                    DictionDtlList.Add(dic);
                }
                SetDataVisibilty(DictionSelected.ValueType);
            }
            SetActionBtn(DictionSelected);
        }

        /// <summary>
        /// 值选择改变
        /// </summary>
        private void DictionDtlSelectedChange()
        {
            //SetActionBtn(DictionDtlSelected);
        }

        /// <summary>
        /// 设置是否能编辑字典
        /// </summary>
        /// <param name="dic"></param>
        private void SetActionBtn(Diction dic)
        {
            ISADDENABLE = dic != null && dic.isadd;
            ISEDITEENABLE = dic != null && dic.isedit;
            ISDELETEENABLE = dic != null && dic.isdelete;
        }

        private void SetDataVisibilty(ValueTypeE type)
        {
            IntVis = type == ValueTypeE.Integer;
            BoolVis = type == ValueTypeE.Boolean;
            StringVis = type == ValueTypeE.String;
            DoubleVis = type == ValueTypeE.Double;
            UIntVis = type == ValueTypeE.UInteger;
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
