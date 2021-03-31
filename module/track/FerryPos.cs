namespace module.track
{
    public class FerryPos
    {
        public uint id { set; get; }
        public uint track_id { set; get; }
        public uint device_id { set; get; }
        public ushort ferry_code { set; get; }
        public int ferry_pos { set; get; }
        public int old_ferry_pos { set; get; }

        #region[模拟使用方法]
        public int sim_ferry_pos { set; get; }

        public bool SimIsInArea(int pos, int distance)
        {
            if(pos < sim_ferry_pos)
            {
                return pos >= (sim_ferry_pos - distance);
            }

            return pos <= (sim_ferry_pos + distance);
        }

        #endregion
    }
}
