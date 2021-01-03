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
    }
}
