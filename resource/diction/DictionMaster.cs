using enums;
using GalaSoft.MvvmLight.Messaging;
using module.diction;
using module.rf;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

        public List<DictionDtl> GetDicDtls(string code)
        {
            return DicDtlList.FindAll(c => c.code.Equals(code));
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

        public double GetDtlDouble(string code, double defaultvalue = 0.0)
        {
            return DicDtlList.Find(c => code.Equals(c.code))?.double_value ?? defaultvalue;
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

        public string GetDtlStrCode(string code, out  byte level)
        {
            DictionDtl dtl = DicDtlList.Find(c => c.code.Equals(code));
            if (dtl != null)
            {
                level = dtl.level;
                return dtl?.string_value ?? code;
            }
            level = 0;
            return code;
        }

        public string GetDtlStrCode(string code, int int_v)
        {
            return DicDtlList.Find(c => c.code.Equals(code) && c.int_value == int_v)?.name ?? "字典未配置";
        }

        public string GetDtlStrCode(string code1, string code2, int int_v)
        {
            return DicDtlList.Find(c => (c.code.Equals(code1) || c.code.Equals(code2)) && c.int_value == int_v)?.name ?? "字典未配置";
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

        public uint UpdateGenerateID(string dictag, ushort addqty)
        {
            DictionDtl dtl = DicDtlList.Find(c => dictag.Equals(c.code));
            if (dtl != null)
            {
                uint uvalue = dtl.uint_value + addqty;
                dtl.uint_value = uvalue + 1;
                PubMaster.Mod.DicSql.EditDicDtlValue(dtl, ValueTypeE.UInteger);
                return uvalue;
            }

            return (uint)(addqty * 2);
        }

        #endregion

        #region[增改字典]

        public bool AddDiction(Diction dic, out string result)
        {
            result = "";
            if (PubMaster.Mod.DicSql.AddDiction(dic))
            {
                mLog.Info(true, string.Format(@"新增字典[ {0} ]", dic.ToString()));
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
            mLog.Info(true, string.Format(@"添加子字典[ {0} ]", dtl.ToString()));
            Refresh(false, true);
            return true;
        }

        public bool EditDiction(Diction dic, out string result)
        {
            result = "";
            Diction d = GetDiction(dic.id);
            string log = SetValue(d, dic);
            if (!PubMaster.Mod.DicSql.EditDiction(d))
            {
                result = "数据没有更改！";
                return false;
            }
            mLog.Info(true, string.Format(@"修改字典[ {0} & {1} ], {2}", dic.id, d.name, log));
            Refresh(true, false);
            return true;
        }

        public bool EditDictionDtl(DictionDtl dtl, ValueTypeE dicType, out string result)
        {
            result = "";
            DictionDtl d = GetDictionDtl(dtl.id);
            string log = SetValue(d, dtl, dicType);
            if (!PubMaster.Mod.DicSql.EditDicDtlValue(d, dicType))
            {
                result = "添加失败！";
                return false;
            }
            mLog.Info(true, string.Format(@"修改子字典[ {0} & {1} ], {2}", d.diction_id, d.name, log));
            Refresh(false, true);
            return true;
        }

        public string SetValue(Diction odic, Diction ndic)
        {
            StringBuilder builder = new StringBuilder();
            if (odic.name != ndic.name)
            {
                builder.Append(string.Format("名称[ {0} -> {1} ], ", odic.name, ndic.name));
            }
            odic.name = ndic.name;

            if (odic.isadd != ndic.isadd)
            {
                builder.Append(string.Format("添加[ {0} -> {1} ], ", odic.isadd, ndic.isadd));
            }
            odic.isadd = ndic.isadd;

            if (odic.isedit != ndic.isedit)
            {
                builder.Append(string.Format("编辑[ {0} -> {1} ], ", odic.isedit, ndic.isedit));
            }
            odic.isedit = ndic.isedit;

            if (odic.isdelete != ndic.isdelete)
            {
                builder.Append(string.Format("删除[ {0} -> {1} ], ", odic.isdelete, ndic.isdelete));
            }
            odic.isdelete = ndic.isdelete;

            if (odic.type != ndic.type)
            {
                builder.Append(string.Format("类型[ {0} -> {1} ], ", odic.type, ndic.type));
            }
            odic.type = ndic.type;

            if (odic.valuetype != ndic.valuetype)
            {
                builder.Append(string.Format("值类型[ {0} -> {1} ], ", odic.valuetype, ndic.valuetype));
            }
            odic.valuetype = ndic.valuetype;

            if (odic.authorizelevel != ndic.authorizelevel)
            {
                builder.Append(string.Format("等级[ {0} -> {1} ], ", odic.authorizelevel, ndic.authorizelevel));
            }
            odic.authorizelevel = ndic.authorizelevel;

            return builder.ToString();
        }

        public string SetValue(DictionDtl oldv, DictionDtl newv, ValueTypeE type)
        {
            StringBuilder builder = new StringBuilder();
            if(oldv.order != newv.order)
            {
                builder.Append(string.Format("顺序[ {0} -> {1} ], ", oldv.order, newv.order));
            }
            oldv.order = newv.order;

            if (!oldv.name.Equals(newv.name))
            {
                builder.Append(string.Format("名称[ {0} -> {1} ], ", oldv.name, newv.name));
            }
            oldv.name = newv.name;
            switch (type)
            {
                case ValueTypeE.Integer:
                    builder.Append(string.Format("int值[ {0} -> {1} ]", oldv.int_value, newv.int_value));
                    oldv.int_value = newv.int_value;
                    break;
                case ValueTypeE.Boolean:
                    builder.Append(string.Format("bool值[ {0} -> {1} ]", oldv.bool_value, newv.bool_value));
                    oldv.bool_value = newv.bool_value;
                    break;
                case ValueTypeE.String:
                    builder.Append(string.Format("str值[ {0} -> {1} ]", oldv.string_value, newv.string_value));
                    oldv.string_value = newv.string_value;
                    break;
                case ValueTypeE.Double:
                    builder.Append(string.Format("doub值[ {0} -> {1} ]", oldv.double_value, newv.double_value));
                    oldv.double_value = newv.double_value;
                    break;
                case ValueTypeE.UInteger:
                    builder.Append(string.Format("uint值[ {0} -> {1} ]", oldv.uint_value, newv.uint_value));
                    oldv.uint_value = newv.uint_value;
                    break;
                default:
                    break;
            }
            return builder.ToString();
        }

        #endregion

        #region[开关修改]

        public bool UpdateSwitch(string tag, bool onoff, bool fromrf)
        {
            DictionDtl dtl = GetDtlInCode(tag);
            if (dtl != null && dtl.bool_value != onoff)
            {
                bool before = dtl.bool_value;
                dtl.bool_value = onoff;
                PubMaster.Mod.DicSql.EditDicDtlValue(dtl, ValueTypeE.Boolean);
                mLog.Info(true, string.Format(@"在开关【{0}】里,编辑【{1}】 -> 【{2}】,备注：【{3}】", dtl.name, before, onoff, fromrf ? "平板" : "电脑"));
                if (fromrf)
                {
                    Messenger.Default.Send(dtl, MsgToken.TaskSwitchUpdate);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否打开开关
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="defvalue"></param>
        /// <returns></returns>
        public bool IsSwitchOnOff(string tag, bool defvalue = false)
        {
            return GetDtlInCode(tag)?.bool_value ?? defvalue;
        }

        public bool IsSwitchOnOff(string tag,out bool havedtl, bool defvalue = false)
        {
            DictionDtl dtl = GetDtlInCode(tag);
            if (dtl != null)
            {
                havedtl = true;
                return dtl.bool_value;
            }
            havedtl = false;
            return defvalue;
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

        public List<TaskSwitch> GetSwitchDtl(List<uint> areaids)
        {
            List<TaskSwitch> list = new List<TaskSwitch>();
            foreach (DictionDtl dtl in DicDtlList.FindAll(c => c.diction_id == TaskSwitchId))
            {
                if (areaids.Contains(dtl.uint_value))
                {
                    list.Add(new TaskSwitch()
                    {
                        id = dtl.id,
                        code = dtl.code,
                        name = dtl.name,
                        onoff = dtl.bool_value
                    });
                }
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
