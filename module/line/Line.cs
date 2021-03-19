namespace module.line
{
    /// <summary>
    /// 线的概念
    /// 同一区域内多条线的概念
    /// </summary>
    public class Line
    {
        public ushort id { set; get; }
        public uint area_id { set; get; }
        public ushort line { set; get; }
        public string name { set; get; }
        public ushort sort_task_qty { set; get; }
        public ushort up_task_qty { set; get; }
        public ushort down_task_qty { set; get; }
    }
}
