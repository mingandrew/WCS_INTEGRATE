using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.diction;
using module.window;
using resource;
using System;
namespace wcs.ViewModel
{
    public class DictionEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DictionEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
        }

        #region[字段]

        private DialogResult _result;
        private DictionTypeE type;
        private ValueTypeE valuetype;
        private string name;
        private bool isadd;
        private bool isedit;
        private bool isdelete;
        private int authorizelevel;

        private string actionname;
        private bool mIsAdd;

        #endregion

        #region[属性]
        public string ACTIONNAME
        {
            get => actionname;
            set => Set(ref actionname, value);
        }

        public string NAME
        {
            get => name;
            set => Set(ref name, value);
        }

        public DictionTypeE TYPE
        {
            get => type;
            set => Set(ref type, value);
        }

        public ValueTypeE VALUETYPE
        {
            get => valuetype;
            set => Set(ref valuetype, value);
        }

        public bool ISADD
        {
            get => isadd;
            set => Set(ref isadd, value);
        }

        public bool ISEDIT
        {
            get => isedit;
            set => Set(ref isedit, value);
        }

        public bool ISDELETE
        {
            get => isdelete;
            set => Set(ref isdelete, value);
        }

        public int AUTHORIZELEVEL
        {
            get => authorizelevel;
            set => Set(ref authorizelevel, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public DialogResult Param
        {
            set;get;
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
            bool rs = CheckAndAdd();
            Result.p1 = rs;
            if(rs) CloseAction?.Invoke();
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
            if (string.IsNullOrEmpty(NAME))
            {
                Growl.InfoGlobal("名称不能为空！");
                return false;
            }

            if(AUTHORIZELEVEL< 0 || AUTHORIZELEVEL > 255)
            {
                Growl.Warning("权级范围[0,255]");
                return false;
            }


            Diction dic = new Diction()
            {
                name = NAME,
                type = (int)TYPE,
                valuetype = (int)VALUETYPE,
                isadd = ISADD,
                isedit = ISEDIT,
                isdelete = ISDELETE,
                authorizelevel = (byte)AUTHORIZELEVEL
            };

            if (mIsAdd)
            {
                if (!PubMaster.Dic.AddDiction(dic, out string result))
                {
                    Growl.Warning(result);
                    return false;
                }
            }
            else
            {

                if (Param.p1 is Diction di)
                {
                    dic.id = di.id;
                }
                if(!PubMaster.Dic.EditDiction(dic, out string result))
                {
                    Growl.Warning(result);
                    return false;
                }
            }

            return true;
        }

        internal void SetActionType(bool isadd)
        {
            mIsAdd = isadd;
            if (isadd)
            {
                ACTIONNAME = "添加";
                NAME = "";
                VALUETYPE = ValueTypeE.Integer;
                ISADD = false;
                ISEDIT = false;
                ISDELETE = false;
                AUTHORIZELEVEL = 10;
                if(Param.p1 is DictionTypeE type)
                {
                    TYPE = type;
                }
            }
            else
            {
                ACTIONNAME = "修改";
                if(Param.p1 is Diction dic)
                {
                    NAME = dic.name;
                    VALUETYPE = dic.ValueType;
                    ISADD = dic.isadd;
                    ISEDIT = dic.isedit;
                    ISDELETE = dic.isdelete;
                    AUTHORIZELEVEL = dic.authorizelevel;
                }
            }
        }
        #endregion
    }
}
