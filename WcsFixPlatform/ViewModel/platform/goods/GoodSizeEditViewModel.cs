using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.window;
using resource;
using System;

namespace wcs.ViewModel
{
    public class GoodSizeEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public GoodSizeEditViewModel()
        {
            _result = new DialogResult();
        }

        #region[字段]

        private DialogResult _result;

        private string actionname;
        private bool doAdd;
        public GoodSize MGoodSize { set; get; }
        public uint id;
        public string name;
        public ushort length;
        public ushort width;
        public byte stack;
        public ushort car_lenght;
        public ushort car_space;
        #endregion

        #region[属性]

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public ushort Length
        {
            get => length;
            set
            {
                Set(ref length, value);
                SetName();
            }
        }

        public ushort Width
        {
            get => width;
            set
            {
                Set(ref width, value);
                SetName();
            }
        }

        public byte Stack
        {
            get => stack;
            set => Set(ref stack, value);
        }

        public ushort CarLength
        {
            get => car_lenght;
            set => Set(ref car_lenght, value);
        }

        public ushort CarSpace
        {
            get => car_space;
            set => Set(ref car_space, value);
        }

        public string ACTIONNAME
        {
            get => actionname;
            set => Set(ref actionname, value);
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
        private void SetName()
        {
            Name = Width + "x" + Length;
        }

        private bool CheckInput()
        {
            if (Width == 0 || Length == 0)
            {
                Growl.Warning("输入正确的规格信息!");
                return false;
            }

            if(Stack == 0 || Stack > 10)
            {
                Growl.Warning("请输入正确的垛数(1-10)");
                return false;
            }

            return true;
        }

        private void Comfirm()
        {
            if (!CheckInput()) return;

            MGoodSize = new GoodSize()
            {
                id = 0,
                name = Name,
                width = Width,
                length = Length,
                stack = Stack,
                car_lenght = CarLength,
                car_space = CarSpace
            };

            if (doAdd)
            {
                if(!PubMaster.Goods.AddGoodSize(MGoodSize, out string result))
                {
                    Growl.Warning(result);
                    return;
                }
            }
            else
            {
                MGoodSize.id = id;
                if(!PubMaster.Goods.EditGoodSize(MGoodSize, out string result))
                {
                    Growl.Warning(result);
                    return;
                }
            }
            Result.p1 = true;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }

        internal void SetAdd(bool v)
        {
            doAdd = v;
            ACTIONNAME = v ? "添加" : "修改";

        }

        internal void SetEditSize(GoodSize selectSize)
        {
            id = selectSize.id;
            Width = selectSize.width;
            Length = selectSize.length;
            Name = selectSize.name;
            Stack = selectSize.stack;
            CarLength = selectSize.car_lenght;
            CarSpace = selectSize.car_space;
        }
        #endregion
    }
}
