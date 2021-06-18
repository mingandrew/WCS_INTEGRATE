using enums;

namespace module.diction
{
    public class Diction
    {
        public int id { set; get; }
        public int type { set; get; }
        public int valuetype { set; get; }
        public string name { set; get; }
        public bool isadd { set; get; }
        public bool isedit { set; get; }
        public bool isdelete { set; get; }
        public byte authorizelevel { set; get; }
        public DictionTypeE Type
        {
            get => (DictionTypeE)type;
        }
        public ValueTypeE ValueType
        {
            get => (ValueTypeE)valuetype;
        }

        public override string ToString()
        {
            return string.Format("Id[ {0} ], 名称[ {1} ], 类型[ {2} & {3} ], 可加[ {4} ], 可编辑[ {5} ], 可删除[ {6} ], 等级[ {7} ], 值类型[ {8} & {9} ]",
                 id, name, Type, type, isadd, isedit, isdelete, authorizelevel, ValueType, valuetype);
        }
    }
}
