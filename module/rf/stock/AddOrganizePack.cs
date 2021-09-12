using System.Collections.Generic;

namespace module.rf
{
    public class AddOrganizePack
    {
        public uint TrackId { get; set; }
        public uint GoodId { get; set; }
        public uint DtlType { get; set; }
        public List<RfStockTransDtl> PreSetList { get; set; }

        public uint Level { set; get; }
    }
}
