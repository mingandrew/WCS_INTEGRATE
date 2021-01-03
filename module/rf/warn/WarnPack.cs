using System.Collections.Generic;

namespace module.rf
{
    public class WarnPack
    {
        public List<Warning> WarnList { set; get; }

        public void AddWarnList(List<Warning> list)
        {
            if (WarnList == null)
            {
                WarnList = new List<Warning>();
            }
            WarnList.AddRange(list);
        }
    }
}
