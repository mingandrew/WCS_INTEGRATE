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
    public class DictionDtlEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DictionDtlEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
        }

        #region[字段]

        private bool _isadd = false;
        private ValueTypeE _valueType;
        private DialogResult _result;
        private string actionname;

        #region[子字典]

        private string name;
        private int int_value;
        private bool bool_value;
        private string string_value;
        private double double_value;
        private uint uint_value;
        private int order;

        #endregion

        private bool showbool, showint, showstring, showdouble, showuint;

        #endregion

        #region[属性]

        #region[子字典属性]

        public string NAME
        {
            get => name;
            set => Set(ref name, value);
        }
        public int INTVALUE
        {
            get => int_value;
            set => Set(ref int_value, value);
        }
        
        public bool BOOLVALUE
        {
            get => bool_value;
            set => Set(ref bool_value, value);
        }

        public string STRINGVALUE
        {
            get => string_value;
            set => Set(ref string_value, value);
        }

        public double DOUBLEVALUE
        {
            get => double_value;
            set => Set(ref double_value, value);
        }
        public uint UINTVALUE
        {
            get => uint_value;
            set => Set(ref uint_value, value);
        }

        public int ORDER
        {
            get => order;
            set => Set(ref order, value);
        }

        #endregion

        #region[显示]

        public bool SHOWBOOL
        {
            get => showbool;
            set => Set(ref showbool, value);
        }
        public bool SHOWINT
        {
            get => showint;
            set => Set(ref showint, value);
        }
        public bool SHOWSTRING
        {
            get => showstring;
            set => Set(ref showstring, value);
        }
        public bool SHOWDOUBLE
        {
            get => showdouble;
            set => Set(ref showdouble, value);
        }
        public bool SHOWUINT
        {
            get => showuint;
            set => Set(ref showuint, value);
        }
        #endregion

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public string ACTIONNAME
        {
            get => actionname;
            set => Set(ref actionname, value);
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
            bool rs = CheckInput();
            if (!rs) return;
            DictionDtl dtl = new DictionDtl();
            if (Param.p1 is Diction dic)
            {
                dtl.diction_id = dic.id;
                dtl.name = NAME;
                dtl.order = ORDER;
                switch (_valueType)
                {
                    case ValueTypeE.Integer:
                        dtl.int_value = INTVALUE;
                        break;
                    case ValueTypeE.Boolean:
                        dtl.bool_value = BOOLVALUE;
                        break;
                    case ValueTypeE.String:
                        dtl.string_value = STRINGVALUE;
                        break;
                    case ValueTypeE.Double:
                        dtl.double_value = DOUBLEVALUE;
                        break;
                    case ValueTypeE.UInteger:
                        dtl.uint_value = UINTVALUE;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Growl.Warning("请先选择字典后再添加!");
                return;
            }
            string result = "";
            if (_isadd)
            {
                rs = PubMaster.Dic.AddDictionDtl(dtl, _valueType, out result);
            }
            else
            {
                if(Param.p2 is DictionDtl dicdtl)
                {
                    dtl.id = dicdtl.id;
                }
                rs = PubMaster.Dic.EditDictionDtl(dtl, _valueType, out result);
            }
            if (!rs)
            {
                Growl.Warning(result);
            }
            Result.p1 = rs;
            if(rs) CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="isadd"></param>
        public void SetActionType(bool isadd)
        {
            _isadd = isadd;
            ACTIONNAME = isadd ? "添加" : "修改";
            if (isadd)
            {
                NAME = "";
                ORDER = 0;
                INTVALUE = 0;
                STRINGVALUE = "";
                BOOLVALUE = false;
                DOUBLEVALUE = 0.0;
            }
            if(Param.p1 is Diction dic)
            {
                _valueType = dic.ValueType;
                SHOWBOOL = _valueType == ValueTypeE.Boolean;
                SHOWSTRING = _valueType == ValueTypeE.String;
                SHOWINT = _valueType == ValueTypeE.Integer;
                SHOWDOUBLE = _valueType == ValueTypeE.Double;
                SHOWUINT = _valueType == ValueTypeE.UInteger;
            }
            else
            {
                Growl.Warning("请选择字典后再操作！");
                Result.p1 = false;
                CloseAction?.Invoke();
            }

            if (!isadd)
            {
                if (Param.p2 is DictionDtl dtl)
                {
                    NAME = dtl.name;
                    ORDER = dtl.order;
                    switch (_valueType)
                    {
                        case ValueTypeE.Integer:
                            INTVALUE = dtl.int_value;
                            break;
                        case ValueTypeE.Boolean:
                            BOOLVALUE = dtl.bool_value;
                            break;
                        case ValueTypeE.String:
                            STRINGVALUE = dtl.string_value;
                            break;
                        case ValueTypeE.Double:
                            DOUBLEVALUE = dtl.double_value;
                            break;
                        case ValueTypeE.UInteger:
                            UINTVALUE = dtl.uint_value;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Growl.Warning("请选择子字典后再修改！");
                    Result.p1 = false;
                    CloseAction?.Invoke();
                }

            }
        }

        /// <summary>
        /// 检测并添加数字字典值
        /// </summary>
        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(NAME))
            {
                Growl.InfoGlobal("名称不能为空！");
                return false;
            }

            switch (_valueType)
            {
                case ValueTypeE.Integer:
                    break;
                case ValueTypeE.Boolean:
                    break;
                case ValueTypeE.String:
                    break;
                case ValueTypeE.Double:
                    break;
                default:
                    break;
            }

            return true;
        }
        #endregion
    }
}
