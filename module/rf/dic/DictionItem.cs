using System.Collections.Generic;

namespace module.rf
{
    public class RfDiction
    {
        public string DicName { set; get; }
        public string DicCode { set; get; }
        public List<RfDictionDtl> DtlList { set; get; }

        public void AddDtl(RfDictionDtl dtl)
        {
            if(DtlList == null)
            {
                DtlList = new List<RfDictionDtl>();
            }

            DtlList.Add(dtl);
        }
    }

    public class RfDictionDtl
    {
        public int DtlOrder { set; get; }
        public int DtlValue { set; get; }
        public string DtlName { set; get; }
    }
}
