namespace module.rf
{
    public class FerryNewPosPack
    {
        public ushort TrackCode { set; get; }//轨道号
        public int TrackSite { set; get; }//已设坐标值
        public int NowTrackPos { set; get; }//当前坐标值
    }
}
