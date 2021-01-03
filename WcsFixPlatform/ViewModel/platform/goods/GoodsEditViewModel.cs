using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.goods;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using task;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class GoodsEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public GoodsEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            AreaList = PubMaster.Area.GetAreaList();
        }

        #region[字段]

        private DialogResult _result;
        private int id;
        private string name;
        private string color;
        private ushort length;
        private ushort width;
        private bool isoversize;
        private string memo;
        private byte stack;
        private ushort pieces;

        private string actionname;
        private bool mIsAdd;
        private Goods mEidtGood;
        private List<Area> areas;
        private Area selectarea;
        private CarrierTypeE carriertype;
        private bool areachange;
        #endregion

        #region[属性]
        public bool AreaChange
        {
            get => areachange;
            set => Set(ref areachange, value);
        }
        public Area SelectArea
        {
            get => selectarea;
            set
            {
                if (Set(ref selectarea, value))
                {
                    UpdateGoodName();
                }
            }
        }

        public List<Area> AreaList
        {
            get => areas;
            set => Set(ref areas, value);
        }

        public byte Stack
        {
            get => stack;
            set => Set(ref stack, value);
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
            set
            {
                if(Set(ref color, value))
                {
                    UpdateGoodName();
                }
            }
        }

        public ushort Length
        {
            get => length;
            set
            {
                if (Set(ref length, value))
                {
                    UpdateGoodName();
                }
            }
        }

        public ushort Width
        {
            get => width;
            set
            {
                if (Set(ref width, value))
                {
                    UpdateGoodName();
                }
            }
        }
        public bool Isoversize
        {
            get => isoversize;
            set => Set(ref isoversize, value);
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

        #endregion

        #region[方法]
        private void UpdateGoodName()
        {
            Name = SelectArea?.id + ":" + Width + "x" + Length + "-" + Color.Trim();
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

            if (Length <= 0 || Length > 5535)
            {
                Growl.Warning("请输入正确的长度！");
                return false;
            }

            if(Width <=0 || Width > 5000)
            {
                Growl.Warning("请输入正确的宽度！");
                return false;
            }

            if(Stack == 0)
            {
                Growl.Warning("请输入一车的垛数！");
                return false;
            }

            if(Pieces == 0)
            {
                Growl.Warning("请输入满砖数！");
                return false;
            }

            mEidtGood = new Goods()
            {
                name = Name,
                color = color,
                length = Length,
                width = Width,
                oversize = Isoversize,
                memo = Memo,
                area_id = SelectArea.id,
                stack = Stack,
                pieces = Pieces,
                GoodCarrierType = CarrierType
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
            if (isadd)
            {
                ACTIONNAME = "添加";
                Name = "";
                Color = "";
                Length = 0;
                Width = 0;
                Isoversize = false;
                Memo = "";
                SelectArea = AreaList.Find(c => c.id == area_id);
                Stack = 0;
                Pieces = 0;
            }
            else
            {
                ACTIONNAME = "修改";
                if (Param.p1 is GoodsView view)
                {
                    Name = view.Name;
                    Color = view.Color;
                    Length = view.Length;
                    Width = view.Width;
                    Isoversize = view.Isoversize;
                    Memo = view.Memo;
                    SelectArea = AreaList.Find(c => c.id == view.AreaId);
                    CarrierType = view.CarrierType;
                    Stack = view.Stack;
                    Pieces = view.Pieces;
                }
            }
        }
        #endregion
    }
}
