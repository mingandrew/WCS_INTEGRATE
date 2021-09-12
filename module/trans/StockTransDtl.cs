using enums;

namespace module.goods
{
    public class StockTransDtl
    {
        public uint dtl_id { set; get; }// 主键

        /// <summary>
        /// 父任务ID
        /// </summary>
        public uint dtl_p_id { set; get; }

        /// <summary>
        /// 细表当前任务ID
        /// </summary>
        public uint dtl_trans_id { set; get; }

        public uint dtl_area_id { set; get; }// 区域ID
        public uint dtl_line_id { set; get; }// 线路ID

        public byte dtl_type { set; get; }// 配置类型

        /// <summary>
        /// 品种ID
        /// </summary>
        public uint dtl_good_id { set; get; }

        /// <summary>
        /// 库存等级
        /// </summary>
        public byte dtl_level { set; get; }

        /// <summary>
        ///  取货轨道
        /// </summary>
        public uint dtl_take_track_id { set; get; }

        /// <summary>
        /// 卸货轨道
        /// </summary>
        public uint dtl_give_track_id { set; get; }

        public byte dtl_status { set; get; }// 细单状态

        /// <summary>
        /// 全部库存数量
        /// </summary>
        public ushort dtl_all_qty { set; get; }

        /// <summary>
        /// 剩余库存数量
        /// </summary>
        public ushort dtl_left_qty { set; get; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool dtl_finish { set; get; }

        /// <summary>
        /// 细单类型
        /// </summary>
        public StockTransDtlTypeE DtlType
        {
            get => (StockTransDtlTypeE)dtl_type;
            set => dtl_type = (byte)value;
        }

        /// <summary>
        /// 细单状态
        /// </summary>
        public StockTransDtlStatusE DtlStatus
        {
            get => (StockTransDtlStatusE)dtl_status;
            set => dtl_status = (byte)value;
        }

        /// <summary>
        /// 判断品种/等级是否一致
        /// </summary>
        /// <param name="goodid"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool EqualGoodAndLevel(uint goodid, uint level)
        {
            return dtl_good_id == goodid && dtl_level == level;
        }
    }
}
