namespace module.track
{
    public class FerryPos
    {
        public uint id { set; get; }
        public uint track_id { set; get; }
        public uint device_id { set; get; }
        public ushort ferry_code { set; get; }
        public int ferry_pos { set; get; }
    }
}
