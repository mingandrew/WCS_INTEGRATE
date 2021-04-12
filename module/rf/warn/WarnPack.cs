using System.Collections.Generic;

namespace module.rf
{
    public class WarnPack
    {
        public List<RfWarning> WarnList { set; get; }

        public void AddWarnList(List<Warning> list)
        {
            if (WarnList == null)
            {
                WarnList = new List<RfWarning>();
            }
            foreach (var item in list)
            {
                WarnList.Add(new RfWarning(item));
            }
        }
    }
}
