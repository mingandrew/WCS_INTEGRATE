namespace module.track
{
    /// <summary>
    /// 【模拟系统】计算摆渡车位置
    /// </summary>
    public class SimFerryPos
    {
        public uint id { set; get; }
        public uint area_id { set; get; }
        /// <summary>
        /// 是否是下砖的摆渡车模拟信息
        /// </summary>
        public bool isdownferry { set; get; }
        /// <summary>
        /// 摆渡车该脉冲值对应的轨道号
        /// </summary>
        public ushort ferry_code { set; get; }
        /// <summary>
        /// 摆渡车的模拟的脉冲值
        /// </summary>
        public int ferry_pos { set; get; }
        /// <summary>
        /// 是否是摆渡车的下砖测地标
        /// </summary>
        public bool isdownside { set; get; }
    }
}
