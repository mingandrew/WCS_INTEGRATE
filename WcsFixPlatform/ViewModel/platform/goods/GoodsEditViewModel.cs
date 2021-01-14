using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.diction;
using module.goods;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using task;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class GoodsEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public GoodsEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            AreaList = PubMaster.Area.GetAreaList();
            LevelList = PubMaster.Dic.GetDicDtls(DicTag.GoodLevel);
        }

        #region[字段]

        private DialogResult _result;
        private int id;
        private string name;
        private string color;
        private string memo;
        private ushort pieces;
        private uint size_id;
        private string size_info;
        private byte level;

        private string actionname;
        private bool mIsAdd;
        private Goods mEidtGood;
        private List<Area> areas;
        private List<DictionDtl> levels;
        private Area selectarea;
        private DictionDtl selectlevel;
        private CarrierTypeE carriertype;
        private bool areachange;
        #endregion

        #region[属性]
        public string SizeInfo
        {
            get => size_info;
            set => Set(ref size_info, value);
        }
        public bool AreaChange
        {
            get => areachange;
            set => Set(ref areachange, value);
        }
        public Area SelectArea
        {
            get => selectarea;
            set => Set(ref selectarea, value);
        }
        
        public DictionDtl SelectLevel
        {
            get => selectlevel;
            set => Set(ref selectlevel, value);
        }

        public List<Area> AreaList
        {
            get => areas;
            set => Set(ref areas, value);
        }


        public List<DictionDtl> LevelList
        {
            get => levels;
            set => Set(ref levels, value);
        }

        public ushort Pieces
        {
            get => pieces;
            set => Set(ref pieces, value);
        }
        public int ID
        {
            get => id;
            set => Set(ref id, value);
        }
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        public string Color
        {
            get => color;
            set => Set(ref color,value);
        }

        public string Memo
        {
            get => memo;
            set => Set(ref memo, value);
        }

        public string ACTIONNAME
        {
            get => actionname;
            set => Set(ref actionname, value);
        }

        public CarrierTypeE CarrierType
        {
            get => carriertype;
            set => Set(ref carriertype, value);
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
        public RelayCommand SelectSizeCmd => new Lazy<RelayCommand>(() => new RelayCommand(SelectGoodSize)).Value;

        #endregion

        #region[方法]
        private async void SelectGoodSize()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodSizeSelectDialog>()
                        .Initialize<GoodSizeSelectViewModel>((vm) =>{}).GetResultAsync<DialogResult>();

            if (result.p1 is GoodSize size)
            {
                SetSizeInfo(size);
            }
        }

        private void SetSizeInfo(GoodSize size)
        {
            if (size == null) return;
            SizeInfo = "宽:[ " + size.width + " ]   长:[ " + size.length + " ]   垛:[ " + size.stack+" ]";
            size_id = size.id;
        }

        private void Comfirm()
        {
            bool rs = CheckAndAdd();
            Result.p1 = rs;
            if (rs) CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }


        /// <summary>
        /// 检测并添加数字字典值
        /// </summary>
        private bool CheckAndAdd()
        {
            if(SelectArea == null)
            {
                Growl.Warning("请选择区域");
                return false;
            }

            if (string.IsNullOrEmpty(Name))
            {
                Growl.Info("名称不能为空！");
                return false;
            }

            if (string.IsNullOrEmpty(Color))
            {
                Growl.Info("色号不能为空！");
                return false;
            }

            if(size_id == 0)
            {
                Growl.Info("请选择规格！");
                return false;
            }

            if(Pieces == 0)
            {
                Growl.Warning("请输入满砖数！");
                return false;
            }

            if(SelectLevel == null)
            {
                Growl.Warning("请选择品种等级");
                return false;
            }
            

            mEidtGood = new Goods()
            {
                name = Name,
                color = color,
                memo = Memo,
                area_id = SelectArea.id,
                pieces = Pieces,
                GoodCarrierType = CarrierType,
                size_id = size_id,
                level = (byte)SelectLevel.int_value,
                info = name + "/" + color + PubMaster.Goods.GetGoodSizeSimpleName(size_id, "/") + "/" + selectlevel.name
            };

            if (mIsAdd)
            {
                mEidtGood.GoodCarrierType = PubMaster.Area.GetCarrierType(mEidtGood.area_id);
                if (!PubMaster.Goods.AddGoods(mEidtGood, out string result))
                {
                    Growl.Warning(result);
                    return false;
                }
                else
                {
                    PubTask.Rf.SendGoodDic2ToAll();
                }
            }
            else
            {
                if (Param.p1 is GoodsView view)
                {
                    mEidtGood.id = view.ID;
                }
                if (!PubMaster.Goods.EditGood(mEidtGood, out string result))
                {
                    Growl.Warning(result);
                    return false;
                }
                else
                {
                    PubTask.Rf.SendGoodDic2ToAll();
                }
            }
            return true;
        }

        internal void SetActionType(bool isadd, uint area_id)
        {
            mIsAdd = isadd;
            AreaChange = isadd;
            SizeInfo = "";
            size_id = 0;
            level = 0;
            SelectLevel = null;
            if (isadd)
            {
                ACTIONNAME = "添加";
                Name = "";
                Color = "";
                Memo = "";
                SelectArea = AreaList.Find(c => c.id == area_id);
                Pieces = 0;
            }
            else
            {
                ACTIONNAME = "修改";
                if (Param.p1 is GoodsView view)
                {
                    Name = view.Name;
                    Color = view.Color;
                    Memo = view.Memo;
                    SelectArea = AreaList.Find(c => c.id == view.AreaId);
                    CarrierType = view.CarrierType;
                    Pieces = view.Pieces;

                    SetSizeInfo(PubMaster.Goods.GetSize(view.SizeId));
                    SelectLevel = GetLevelDtl(view.Level);
                }
            }
        }

        private DictionDtl GetLevelDtl(uint levelvalue)
        {
            return LevelList.Find(c => c.int_value == levelvalue);
        }
        #endregion
    }
}
