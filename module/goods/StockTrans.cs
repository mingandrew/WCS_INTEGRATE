using enums;
using System;
using System.Linq;

namespace module.goods
{
    public class StockTrans
    {
        public StockTrans()
        {
            TransStausStayTime = DateTime.Now;
        }
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
        /// 任务处于状态的时间
        /// </summary>
        public DateTime TransStausStayTime { set; get; }

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

        /// <summary>
        /// 任务步骤信息
        /// </summary>
        public TransStep StepLog { set; get; } = new TransStep();

        /// <summary>
        /// 是否记录步骤信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="info"></param>
        /// <param name="isOk"></param>
        /// <returns></returns>
        public bool LogStep(bool isOk, uint code, string info, bool isRepeat)
        {
            // 重复code 且 不允许重复
            if (StepLog.StepCode == code && !isRepeat)
            {
                return false;
            }

            // 允许重复，但内容没变
            if (isRepeat && !string.IsNullOrEmpty(StepLog.StepInfo) && StepLog.StepInfo.Contains(info))
            {
                return false;
            }

            // 信息格式
            string msg = string.Format(@"[{0}]>>{1}:[{2}]-({3})-{4}
", DateTime.Now, TransStaus, isOk ? "✔" : "❌", code, info);

            StepLog.StepCode = code;
            StepLog.StepInfo = msg;
            return true;
        }


        /// 任务使用了该轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool InTrack(params uint[] trackid)
        {
            return trackid.Contains(take_track_id) || trackid.Contains(give_track_id) || trackid.Contains(finish_track_id);
        }

        /// <summary>
        /// 任务没有使用该轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool NotInTrack(params uint[] trackid)
        {
            return !trackid.Contains(take_track_id) && !trackid.Contains(give_track_id) && !trackid.Contains(finish_track_id);
        }

        /// <summary>
        /// 是否需要该任务类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InType(params TransTypeE[] types)
        {
            return types.Contains(TransType);
        }

        /// <summary>
        /// 不符合任务类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool NotInType(params TransTypeE[] types)
        {
            return !types.Contains(TransType);
        }

        /// <summary>
        /// 是否需要该任务状态
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InStatus(params TransStatusE[] status)
        {
            return status.Contains(TransStaus);
        }

        /// <summary>
        /// 不符合任务状态
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool NotInStatus(params TransStatusE[] status)
        {
            return !status.Contains(TransStaus);
        }

        /// <summary>
        /// 判断任务处于
        /// 1.状态是否处于该状态
        /// 2.持续时间是否已经超过指定时间
        /// </summary>
        /// <param name="status"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool IsInStatusOverTime(TransStatusE status, int second)
        {
            return TransStaus == status && DateTime.Now.Subtract(TransStausStayTime).TotalSeconds > second;
        }

        public override string ToString()
        {
            return string.Format("标识[ {0} ], 类型[ {1} ], 小车[ {2} ], 取货[ {3} ], 卸货[ {4} ]", id, TransType, carrier_id, take_track_id, give_track_id);
        }

        public string GetStatusTimeStr()
        {
            TimeSpan span = DateTime.Now.Subtract(TransStausStayTime);
            if(span.TotalHours >= 1)
            {
                return string.Format("{0}时 {1}分 {2}秒", span.Hours, span.Minutes, span.Seconds);
            }

            if(span.TotalMinutes >= 1)
            {
                return string.Format("{0}分 {1}秒", span.Minutes, span.Seconds);
            }

            return string.Format("{0}秒", span.Seconds);
        }
    }

    /// <summary>
    /// 任务步骤
    /// </summary>
    public class TransStep
    {
        /// <summary>
        /// 步骤编码
        /// </summary>
        public uint StepCode { set; get; }

        /// <summary>
        /// 步骤信息
        /// </summary>
        public string StepInfo { set; get; }
    }

}
