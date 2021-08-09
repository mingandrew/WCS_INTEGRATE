using enums;
using System.Linq;

namespace module.goods
{
    public class StockTransDtl
    {
        public uint dtl_id { set; get; }// 主键
        public uint dtl_p_id { set; get; }// 父任务ID
        public uint dtl_trans_id { set; get; }// 细表当前任务
        public uint dtl_area_id { set; get; }// 区域ID
        public uint dtl_line_id { set; get; }// 线路ID
        public byte dtl_type { set; get; }// 配置类型
        public uint dtl_good_id { set; get; }// 品种ID
        public uint dtl_take_track_id { set; get; }// 取货轨道
        public uint dtl_give_track_id { set; get; }// 卸货轨道
        public byte dtl_status { set; get; }// 细单状态
        public ushort dtl_all_qty { set; get; }// 全部库存数量
        public ushort dtl_left_qty { set; get; }// 剩余库存数量
        public bool dtl_finish { set; get; }// 完成

        //细单类型
        public StockTransDtlTypeE DtlType
        {
            get => (StockTransDtlTypeE)dtl_type;
            set => dtl_type = (byte)value;
        }

        //西单状态
        public StockTransDtlStatusE DtlStatus
        {
            get => (StockTransDtlStatusE)dtl_status;
            set => dtl_status = (byte)value;
        }

        /// <summary>
        /// 判断是否符合类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InDtlType(params StockTransDtlTypeE[] types)
        {
            return types.Contains(DtlType);
        }

        /// <summary>
        /// 判断是否符合状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool InDtlStatus(params StockTransDtlStatusE[] status)
        {
            return status.Contains(DtlStatus);
        }
    }
}
