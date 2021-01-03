using enums;
using GalaSoft.MvvmLight.Messaging;
using module.diction;
using module.rf;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using tool.mlog;

namespace resource.diction
{
    public class DictionMaster
    {
        #region[构造/初始化]
        private int TaskSwitchId = 2;
        private Log mLog;
        public DictionMaster()
        {
            DicList = new List<Diction>();
            DicDtlList = new List<DictionDtl>();
            mLog = (Log)new LogFactory().GetLog("字典", false);
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refdic = true, bool refdtl = true)
        {
            if (refdic)
            {
                DicList.Clear();
                DicList.AddRange(PubMaster.Mod.DicSql.QueryDictionList());
            }

            if (refdtl)
            {
                DicDtlList.Clear();
                DicDtlList.AddRange(PubMaster.Mod.DicSql.QueryDictionDtlList());
            }
        }

        public void Stop()
        {

        }
        #endregion

        #region[字段]
        private List<Diction> DicList { set; get; }
        private List<DictionDtl> DicDtlList { set; get; }


        #endregion

        #region[获取对象]
        public List<Diction> GetDicList()
        {
            return DicList;
        }

        public List<DictionDtl> GetDicDtlList()
        {
            return DicDtlList;
        }

        public List<Diction> GetDicList(DictionTypeE type)
        {
            return DicList.FindAll(c => c.Type == type);
        }

        public DictionDtl GetDtlInCode(string code)
        {
            if (code == null) return null;
            return DicDtlList.Find(c => c.code.Equals(code));
        }
         
        public Diction GetDiction(int dicid)
        {
            return DicList.Find(c => c.id == dicid);
        }

        public DictionDtl GetDictionDtl(int dicid, int dicdtlid)
        {
            return DicDtlList.Find(c => c.diction_id == dicid && c.id == dicdtlid);
        }

        public DictionDtl GetDictionDtl(int dicdtlid)
        {
            return DicDtlList.Find(c => c.id == dicdtlid);
        }

        public List<DictionDtl> GetDictionDtlList(int dicid)
        {
            return DicDtlList.FindAll(c => c.diction_id == dicid);
        }

        #endregion

        #region[获取/判断属性]

        public int GetDtlInt(int dicdtlid, int defaultvalue = -1)
        {
            return DicDtlList.Find(c => c.id == dicdtlid)?.int_value ?? defaultvalue;
        }


        public bool GetDtlBool(int dicdtlid, bool defaultvalue = false)
        {
            return DicDtlList.Find(c => c.id == dicdtlid)?.bool_value ?? defaultvalue;
        }

        public string GetDtlString(int dicdtlid, string defaultvalue = "")
        {
            return DicDtlList.Find(c => c.id == dicdtlid)?.string_value ?? defaultvalue;
        }

        public double GetDtlDouble(int dicdtlid, double defaultvalue = 0.0)
        {
            return DicDtlList.Find(c => c.id == dicdtlid)?.double_value ?? defaultvalue;
        }

        public string GetDicName(int diction_id)
        {
            return DicList.Find(c => c.id == diction_id)?.name ?? "";
        }

        public string GetDtlIntName(int dicid, int intvalue)
        {
            return DicDtlList.Find(c => c.diction_id == dicid && c.int_value == intvalue)?.name ?? ""+intvalue;
        }

        public string GetDtlStringName(int dicid, string stringvalue)
        {
            return DicDtlList.Find(c => c.diction_id == dicid && stringvalue.Equals(c.string_value))?.name ?? "" + stringvalue;
        }


        public object GetDictionName(ValueDiction valuedic)
        {
            return GetDtlIntName(valuedic.DictionID, (int)valuedic.Value);
        }

        public int GetDtlIntCode(string code)
        {
            return DicDtlList.Find(c => c.code.Equals(code))?.int_value ?? 0;
        }

        public string GetDtlStrCode(string code)
        {
            return DicDtlList.Find(c => c.code.Equals(code))?.string_value ?? code;
        }

        public uint GetDtlUIntCode(string code)
        {
            return DicDtlList.Find(c => c.code.Equals(code))?.uint_value ?? 0;
        }



        public uint GenerateID(string dictag)
        {
            DictionDtl dtl = DicDtlList.Find(c => dictag.Equals(c.code));
            if (dtl != null)
            {
                uint uvalue = dtl.uint_value;
                dtl.uint_value = uvalue + 1;
                PubMaster.Mod.DicSql.EditDicDtlValue(dtl, ValueTypeE.UInteger);
                return uvalue;
            }

            return 0;
        }

        #endregion

        #region[增改字典]

        public bool AddDiction(Diction dic, out string result)
        {
            result = "";
            if (PubMaster.Mod.DicSql.AddDiction(dic))
            {
                mLog.Info(true, string.Format(@"新增字典名【{0}】", dic.name));
                Refresh(true, false);
                return true;
            }
            return false;
        }

        public bool AddDictionDtl(DictionDtl dtl, ValueTypeE dicType, out string result)
        {
            result = "";
            if(!PubMaster.Mod.DicSql.AddDictionDtl(dtl, dicType))
            {
                result = "添加失败！";
                return false;
            }
            mLog.Info(true, string.Format(@"在字典ID【{0}】里,添加字典细节{1}", dtl.diction_id, dtl.ToString()));
            Refresh(false, true);
            return true;
        }

        public bool EditDiction(Diction dic, out string result)
        {
            result = "";
            Diction d = GetDiction(dic.id);
            SetValue(d, dic);
            if (!PubMaster.Mod.DicSql.EditDiction(d))
            {
                result = "数据没有更改！";
                return false;
            }
            mLog.Info(true, string.Format(@"编辑字典名【{0}】", dic.name));
            Refresh(true, false);
            return true;
        }

        public bool EditDictionDtl(DictionDtl dtl, ValueTypeE dicType, out string result)
        {
            result = "";
            DictionDtl d = GetDictionDtl(dtl.id);
            SetValue(d, dtl, dicType);
            if (!PubMaster.Mod.DicSql.EditDicDtlValue(d, dicType))
            {
                result = "添加失败！";
                return false;
            }
            mLog.Info(true, string.Format(@"在字典ID【{0}】里,编辑字典细节{1}", d.diction_id, d.ToString()));
            Refresh(false, true);
            return true;
        }

        public void SetValue(Diction odic, Diction ndic)
        {
            odic.name = ndic.name;
            odic.isadd = ndic.isadd;
            odic.isedit = ndic.isedit;
            odic.isdelete = ndic.isdelete;
            odic.type = ndic.type;
            odic.valuetype = ndic.valuetype;
            odic.authorizelevel = ndic.authorizelevel;
        }

        public void SetValue(DictionDtl oldv, DictionDtl newv, ValueTypeE type)
        {
            oldv.order = newv.order;
            oldv.name = newv.name;
            switch (type)
            {
                case ValueTypeE.Integer:
                    oldv.int_value = newv.int_value;
                    break;
                case ValueTypeE.Boolean:
                    oldv.bool_value = newv.bool_value;
                    break;
                case ValueTypeE.String:
                    oldv.string_value = newv.string_value;
                    break;
                case ValueTypeE.Double:
                    oldv.double_value = newv.double_value;
                    break;
                case ValueTypeE.UInteger:
                    oldv.uint_value = newv.uint_value;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region[开关修改]

        public bool UpdateSwitch(string tag, bool onoff, bool fromrf)
        {
            DictionDtl dtl = GetDtlInCode(tag);
            if (dtl != null && dtl.bool_value != onoff)
            {
                dtl.bool_value = onoff;
                PubMaster.Mod.DicSql.EditDicDtlValue(dtl, ValueTypeE.Boolean);
                if (fromrf)
                {
                    Messenger.Default.Send(dtl, MsgToken.TaskSwitchUpdate);
                }
                return true;
            }
            return false;
        }

        public bool IsSwitchOnOff(string tag, bool defvalue = false)
        {
            return GetDtlInCode(tag)?.bool_value ?? defvalue;
        }


        public bool IsAreaTaskOnoff(uint areaid, DicAreaTaskE type)
        {
            switch (type)
            {
                case DicAreaTaskE.上砖:
                    return IsSwitchOnOff(DicSwitchTag.Area + areaid + DicSwitchTag.Up);
                case DicAreaTaskE.下砖:
                    return IsSwitchOnOff(DicSwitchTag.Area + areaid + DicSwitchTag.Down);
                case DicAreaTaskE.倒库:
                    return IsSwitchOnOff(DicSwitchTag.Area + areaid + DicSwitchTag.Sort);
                default:
                    break;
            }
            return false;
        }

        public List<TaskSwitch> GetSwitchDtl()
        {
            List<TaskSwitch> list = new List<TaskSwitch>();
            foreach (DictionDtl dtl in DicDtlList.FindAll(c => c.diction_id == TaskSwitchId))
            {
                list.Add(new TaskSwitch()
                {
                    id = dtl.id,
                    code = dtl.code,
                    name = dtl.name,
                    onoff = dtl.bool_value
                });
            }
            return list;
        }

        #endregion

        #region[版本更新]

        public void UpdateVersion(string dictag)
        {
            DictionDtl dtl = GetDtlInCode(dictag);
            if (dtl != null)
            {
                dtl.int_value++;
                PubMaster.Mod.DicSql.EditDicDtlValue(dtl, ValueTypeE.Integer);
            }
        }

        public bool IsVersionDiffer(VersionDic pack)
        {
            DictionDtl dtl = GetDtlInCode(pack.Name);

            if (dtl != null)
            {
                return dtl.int_value != pack.Version;
            }

            return false;
        }

        #endregion
    }
}
