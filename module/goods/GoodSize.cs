namespace module.goods
{
    public class GoodSize
    {
        public uint id { set; get; }
        public string name { set; get; }
        public ushort length { set; get; }
        public ushort width { set; get; }
        public byte stack { set; get; }

        /// <summary>
        /// 一车砖的长度（脉冲）
        /// </summary>
        public ushort car_lenght { set; get; }

        /// <summary>
        /// 砖与砖最小间距（脉冲）
        /// </summary>
        public ushort car_space { set; get; }
        
    }
}
