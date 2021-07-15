

namespace module.rf
{
    public class RfStockTransDtl
    {

        public uint areaid { set; get; }// 区域ID
        public uint lineid { set; get; }// 线路ID
        public uint dtltype { set; get; }// 配置类型
        public uint goodid { set; get; }// 品种ID
        public uint taketrackid { set; get; }// 取货轨道
        public uint givetrackid { set; get; }// 卸货轨道
        public uint allqty { set; get; }// 全部库存数量
    }
}
