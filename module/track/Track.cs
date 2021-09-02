using enums.track;
using System;
using System.Collections.Generic;
using System.Linq;

namespace module.track
{
    public class Track
    {
        #region[字段属性]

        public uint id { set; get; }
        public string name { set; get; }
        public ushort area { set; get; }
        public byte type { set; get; }
        public byte type2 { set; get; }
        public byte stock_status { set; get; }//库存状态
        public byte track_status { set; get; }//轨道使用状态
        public ushort width { set; get; }
        public ushort left_distance { set; get; }
        public ushort right_distance { set; get; }

        /// <summary>
        /// 轨道编号（摆渡车前侧，轨道后端）
        /// </summary>
        public ushort ferry_up_code{set;get; }
        /// <summary>
        /// 轨道编号（摆渡车后侧，轨道前端）
        /// </summary>
        public ushort ferry_down_code{set;get; } 

        public int max_store { set; get; }
        public uint brother_track_id { set; get; }
        public uint left_track_id { set; get; }
        public uint right_track_id { set; get; }
        public string memo { set; get; }
        public ushort rfid_1 { set; get; }
        public ushort rfid_2 { set; get; }

        /// <summary>
        /// 反抛点
        /// </summary>
        public ushort rfid_3 { set; get; }
        public ushort rfid_4 { set; get; }
        public ushort rfid_5 { set; get; }

        /// <summary>
        /// 接力结束点
        /// </summary>
        public ushort rfid_6 { set; get; }

        public string rfids { set; get; }

        public short order { set; get; }
        public uint recent_goodid { set; get; }
        public uint recent_tileid { set; get; }
        public ushort alert_status { set; get; }//故障状态
        public uint alert_carrier { set; get; }//故障小车
        public uint alert_trans { set; get; }//故障任务
        public bool early_full { set; get; }//提前满砖
        public DateTime? full_time { set; get; }//满砖时间

        /// <summary>
        /// 是否同侧出入库
        /// </summary>
        public bool same_side_inout { set; get; }

        public int upcount { get; set; }//上砖车数

        /// <summary>
        /// 轨道分段点坐标（中间点）
        /// </summary>
        public ushort split_point { set; get; }

        /// <summary>
        /// 轨道下砖极限点坐标【定位点脉冲】（后侧点）
        /// </summary>
        public ushort limit_point { set; get; }

        /// <summary>
        /// 轨道上砖极限点坐标【定位点脉冲】（前侧点）
        /// </summary>
        public ushort limit_point_up { set; get; }

        /// <summary>
        /// 上砖侧分割点坐标【上砖接力倒库点】
        /// </summary>
        public ushort up_split_point { get; set; }

        public ushort line { set; get; }//线

        /// <summary>
        /// 是否入库 后退存砖
        /// </summary>
        public bool is_give_back { set; get; }

        /// <summary>
        /// 是否出库 前进取砖
        /// </summary>
        public bool is_take_forward { set; get; } 

        public bool sort_able { set; get; }//倒库状态
        public byte sort_level { set; get; }//倒库等级
        #endregion

        #region [获取属性]

        /// <summary>
        /// 轨道类型
        /// </summary>
        public TrackTypeE Type
        {
            get => (TrackTypeE)type;
            set => type = (byte)value;
        }

        /// <summary>
        /// 轨道出入库类型
        /// </summary>
        public TrackType2E Type2
        {
            get => (TrackType2E)type2;
            set => type2 = (byte)value;
        }

        /// <summary>
        /// 轨道库存状态
        /// </summary>
        public TrackStockStatusE StockStatus
        {
            get => (TrackStockStatusE)stock_status;
            set => stock_status = (byte)value;
        }

        /// <summary>
        /// 轨道状态
        /// </summary>
        public TrackStatusE TrackStatus
        {
            get => (TrackStatusE)track_status;
            set => track_status = (byte)value;
        }

        public List<ushort> RFIDs;
        public void GetAllRFID()
        {
            RFIDs = new List<ushort>();

            if (string.IsNullOrEmpty(rfids)) return;

            string[] s = rfids.Split('#');
            foreach (var item in s)
            {
                if (ushort.TryParse(item, out ushort rfid))
                {
                    RFIDs.Add(rfid);
                }
            }
        }

        public TrackAlertE AlertStatus
        {
            get => (TrackAlertE)alert_status;
            set => alert_status = (ushort)value;
        }

        #endregion

        #region [数据信息]

        /// <summary>
        /// 获取轨道点位信息
        /// </summary>
        /// <returns></returns>
        public string GetLog()
        {
            string log = string.Empty;
            log += string.Format("名称[ {0} ]", name);
            if (split_point > 0)
            {
                log += string.Format("，分割[ {0} ]", split_point);
            }

            if (limit_point_up > 0)
            {
                log += string.Format("，上极[ {0} ]", limit_point_up);
            }

            if (limit_point > 0)
            {
                log += string.Format("，下极[ {0} ]", limit_point);
            }

            return log;
        }

        /// <summary>
        /// 获取轨道状态信息
        /// </summary>
        /// <returns></returns>
        public string GetStatusLog()
        {
            return string.Format("名称[ {0} ], 状态[ {1} ], 货物[ {2} ]", name, TrackStatus, StockStatus);
        }

        #endregion

        #region [条件判断]

        /// <summary>
        /// 是否是轨道内地标
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        public bool IsInTrack(ushort rfid)
        {
            if (rfid == 0) return false;
            //return rfid == rfid_1 || rfid == rfid_2 || rfid == rfid_3 || rfid == rfid_4 || rfid == rfid_5 || rfid == rfid_6;

            if (RFIDs == null || RFIDs.Count == 0) return false;
            return RFIDs.Contains(rfid);
        }

        /// <summary>
        /// 是否是上砖区域的轨道
        /// </summary>
        /// <returns></returns>
        public bool IsUpAreaTrack()
        {
            return InType(TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.前置摆渡轨道);
        }
        
        /// <summary>
        /// 是否是下砖区域的轨道
        /// </summary>
        /// <returns></returns>
        public bool IsDownAreaTrack()
        {
            return InType(TrackTypeE.下砖轨道, TrackTypeE.储砖_入, TrackTypeE.储砖_出入, TrackTypeE.后置摆渡轨道);
        }


        /// <summary>
        /// 是否储砖类型轨道
        /// </summary>
        /// <returns></returns>
        public bool IsStoreTrack()
        {
            return InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
        }

        /// <summary>
        /// 是否摆渡类型轨道
        /// </summary>
        /// <returns></returns>
        public bool IsFerryTrack()
        {
            return InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道);
        }

        /// <summary>
        /// 是否砖机类型轨道
        /// </summary>
        /// <returns></returns>
        public bool IsTileTrack()
        {
            return InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道);
        }


        /// <summary>
        /// 检查是否符合类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InType(params TrackTypeE[] types)
        {
            return types.Contains(Type);
        }

        /// <summary>
        /// 检查是否不符合类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool NotInType(params TrackTypeE[] types)
        {
            return !types.Contains(Type);
        }


        /// <summary>
        /// 检查是否符合状态
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InStatus(params TrackStatusE[] status)
        {
            return status.Contains(TrackStatus);
        }

        /// <summary>
        /// 检查是否不符合状态
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool NotInStatus(params TrackStatusE[] status)
        {
            return !status.Contains(TrackStatus);
        }


        /// <summary>
        /// 检查是否符合库存状态
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool InStockStatus(params TrackStockStatusE[] status)
        {
            return status.Contains(StockStatus);
        }

        /// <summary>
        /// 检查是否不符合库存状态
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool NotInStockStatus(params TrackStockStatusE[] status)
        {
            return !status.Contains(StockStatus);
        }


        /// <summary>
        /// 是否入库作业轨道
        /// </summary>
        /// <returns></returns>
        public bool IsWorkIn()
        {
            return Type == TrackTypeE.储砖_入
                || (Type == TrackTypeE.储砖_出入 && (Type2 == TrackType2E.通用 || Type2 == TrackType2E.入库));
        }

        /// <summary>
        /// 是否出库作业轨道
        /// </summary>
        /// <returns></returns>
        public bool IsWorkOut()
        {
            return Type == TrackTypeE.储砖_出
                || (Type == TrackTypeE.储砖_出入 && (Type2 == TrackType2E.通用 || Type2 == TrackType2E.出库));
        }

        #endregion

    }
}
