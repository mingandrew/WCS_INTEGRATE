using System;

namespace module.diction
{
    public class DictionDtl
    {
        public int id { set; get; }
        public int diction_id { set; get; }
        public string code { set; get; }
        public string name { set; get; }
        public int int_value { set; get; }
        public bool bool_value { set; get; }
        public string string_value { set; get; }
        public double double_value { set; get; }
        public uint uint_value { set; get; }
        public int order { set; get; }
        public DateTime? updatetime { set; get; }
        public byte level { set; get; }

        public override string ToString()
        {
            return string.Format("父ID[ {0} ], ID[ {1} ], 名称[ {2} ], int[ {3} ], bool[ {4} ], str[ {5} ], dou[ {6} ], uin[ {7} ], order[ {8} ], level[ {9} ]",
                diction_id, id, name, int_value, bool_value, string_value, double_value, uint_value, order, level);
        }

        public string Tostr()
        {
            return string.Format(@"【{5}】- 整型：{0}，布尔：{1}，字符串：{2}，双精度{3}，无符号整型{4}", int_value, bool_value, string_value, double_value, uint_value, name);
        }
    }
}
