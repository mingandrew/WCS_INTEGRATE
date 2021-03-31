using enums;
using System;

namespace module.goods
{
    public class StockTrans
    {
        public uint id { set; get; }
        public byte trans_type { set; get; }
        public byte trans_status { set; get; }
        public uint area_id { set; get; }
        public uint goods_id { set; get; }
        public uint stock_id { set; get; }
        public uint take_track_id { set; get; }
        public uint give_track_id { set; get; }
        public uint tilelifter_id { set; get; }
        public uint take_ferry_id { set; get; }//取货摆渡车
        public uint give_ferry_id { set; get; }//卸货摆渡车
        public uint carrier_id { set; get; }
        public DateTime? create_time { set; get; }
        public DateTime? load_time { set; get; }
        public DateTime? unload_time { set; get; }
        public bool finish { set; get; }
        public DateTime? finish_time { set; get; }
        public bool cancel { set; get; }
        public uint finish_track_id { set; get; }
        public ushort line { set; get; }//线

        /// <summary>
        /// 记录摆渡车当下前往的任务轨道
        /// </summary>
        public uint RecordTraID { set; get; }

        public TransTypeE TransType
        {
            get => (TransTypeE)trans_type;
            set => trans_type = (byte)value;
        }

        public TransStatusE TransStaus
        {
            get => (TransStatusE)trans_status;
            set => trans_status = (byte)value;
        }

        /// <summary>
        /// 是否已经发送离开上下砖机
        /// </summary>
        public bool IsLeaveTileLifter { get; set; }

        /// <summary>
        /// 是否已经释放取货摆渡车
        /// </summary>
        public bool IsReleaseTakeFerry { set; get; }

        /// <summary>
        /// 是否已经释放放货摆渡车
        /// </summary>
        public bool IsReleaseGiveFerry { set; get; }
        /// <summary>
        /// 作业的取砖放砖信号
        /// </summary>
        public bool IsSignalProcess { set; get; }

        public bool IsSiteSame(StockTrans trans)
        {
            return take_track_id == trans.take_track_id || give_track_id == trans.give_track_id
                || take_track_id == trans.give_track_id || give_track_id == trans.take_track_id;
        }

        public bool HaveTrack(uint tra1_id, uint tra2_id)
        {
            return take_track_id == tra1_id || give_track_id == tra1_id
                || take_track_id == tra2_id || give_track_id == tra2_id
                || finish_track_id == tra2_id || finish_track_id == tra1_id;
        }
    }
}
