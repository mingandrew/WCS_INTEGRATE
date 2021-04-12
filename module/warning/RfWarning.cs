using System;

namespace module
{
    public class RfWarning
    {
        public uint id { set; get; }
        public string content { set; get; }
        public DateTime? createtime { set; get; }

        public RfWarning(Warning item)
        {
            id = item.id;
            content = item.content;
            createtime = item.createtime;
        }
    }
}
