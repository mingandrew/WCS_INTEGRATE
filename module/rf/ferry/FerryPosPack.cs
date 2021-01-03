using module.track;
using System.Collections.Generic;

namespace module.rf
{
    public class FerryPosPack
    {
        public uint Device { set; get; }
        public List<FerryPos> PosList { set; get; }

        public void AddPosList(List<FerryPos> list)
        {
            if (PosList == null)
            {
                PosList = new List<FerryPos>();
            }

            PosList.AddRange(list);
        }
    }
}
