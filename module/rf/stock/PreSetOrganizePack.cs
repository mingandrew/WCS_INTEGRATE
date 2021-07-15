using module.goods;
using System.Collections.Generic;

namespace module.rf
{
    public class PreSetOrganizePack
    {
        public uint TrackId { get; set; }
        public List<StockTransDtl> PreSetList { get; set; }

        public void AddList(List<StockTransDtl> list)
        {
            if (PreSetList == null)
            {
                PreSetList = new List<StockTransDtl>();
            }
            PreSetList.AddRange(list);
        }
    }
}
