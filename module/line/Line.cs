using enums;

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
        /// <summary>
        /// 接力倒库最大倒库数量
        /// </summary>
        public byte max_upsort_num { get; set; }
        public bool onoff_up { set; get; }
        public bool onoff_down { set; get; }
        public bool onoff_sort { set; get; }
        public byte line_type { set; get; }

        //入库轨道满砖数上限
        public byte full_qty { set; get; }

        /// <summary>
        /// 线路类型
        /// </summary>
        public LineTypeE LineType
        {
            get => (LineTypeE)line_type;
            set => line_type = (byte)value;
        }

        public void Update(Line l)
        {
            line = l.line;
            name = l.name;
            sort_task_qty = l.sort_task_qty;
            up_task_qty = l.up_task_qty;
            down_task_qty = l.down_task_qty;
            full_qty = l.full_qty;
        }
    }
}
