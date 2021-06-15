using System;

namespace module
{
    public class Warning
    {
        public uint id { set; get; }
        public ushort area_id { set; get; }
        public ushort line_id { set; get; }
        public byte type { set; get; }
        public bool resolve { set; get; }
        public ushort dev_id { set; get; }
        public uint trans_id { set; get; }
        public ushort track_id { set; get; }
        public string content { set; get; }
        public DateTime? createtime { set; get; }
        public DateTime? resolvetime { set; get; }
        public byte level { set; get; }
    }
}
