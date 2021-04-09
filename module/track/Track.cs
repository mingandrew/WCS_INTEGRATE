using enums.track;
using System;
using System.Collections.Generic;

namespace module.track
{
    public class Track
    {
        public uint id { set; get; }
        public string name { set; get; }
        public ushort area { set; get; }
        public byte type { set; get; }
        public byte stock_status { set; get; }//库存状态
        public byte track_status { set; get; }//轨道使用状态
        public ushort width { set; get; }
        public ushort left_distance { set; get; }
        public ushort right_distance { set; get; }

        /// <summary>
        /// 摆渡对位点位，也作为轨道尽头 前进起始点位
        /// </summary>
        public ushort ferry_up_code{set;get; }
        /// <summary>
        /// 摆渡对位点位，也作为轨道尽头 后退起始点位
        /// </summary>
        public ushort ferry_down_code{set;get; } 

        public int max_store { set; get; }
        public uint brother_track_id { set; get; }
        public uint left_track_id { set; get; }
        public uint right_track_id { set; get; }
        public string memo { set; get; }
        public ushort rfid_1 { set; get; }
        public ushort rfid_2 { set; get; }
        public ushort rfid_3 { set; get; }
        public ushort rfid_4 { set; get; }
        public ushort rfid_5 { set; get; }
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
        public bool same_side_inout { set; get; }//是否同侧出入库
        public int upcount { get; set; }//上砖车数
        public bool isupsort { get; set; }//是否在上砖侧倒库

        /// <summary>
        /// 轨道分段点坐标
        /// </summary>
        public ushort split_point { set; get; }

        /// <summary>
        /// 轨道下砖极限点坐标
        /// </summary>
        public ushort limit_point { set; get; }

        /// <summary>
        /// 轨道上砖极限点坐标
        /// </summary>
        public ushort limit_point_up { set; get; }

        /// <summary>
        /// 上砖侧分割点坐标
        /// </summary>
        public int up_split_point { get; set; }

        public ushort line { set; get; }//线
        /// <summary>
        /// 轨道类型
        /// </summary>
        public TrackTypeE Type
        {
            get => (TrackTypeE)type;
            set => type = (byte)value;
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

        public int TrackCode
        {
            get => ferry_down_code + ferry_up_code;
        }

        public TrackAlertE AlertStatus
        {
            get => (TrackAlertE)alert_status;
            set => alert_status = (ushort)value;
        }

        /// <summary>
        /// 获取轨道状态信息
        /// </summary>
        /// <returns></returns>
        public string GetLog()
        {
            string log = string.Empty;
            log += string.Format("名称[ {0} ]", name);
            if (split_point > 0)
            {
                log+= string.Format("，分割[ {0} ]", split_point);
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
        /// 判断轨道是否是能放砖类型
        /// 用于库存切换轨道
        /// </summary>
        /// <returns></returns>
        public bool IsNotFerryTrack()
        {
            return Type != TrackTypeE.摆渡车_入 && Type != TrackTypeE.摆渡车_出;
        }

        /// <summary>
        /// 是否是上砖区域的轨道
        /// </summary>
        /// <returns></returns>
        public bool IsUpAreaTrack()
        {
            return Type == TrackTypeE.上砖轨道 || Type == TrackTypeE.储砖_出 || Type == TrackTypeE.储砖_出入 || Type == TrackTypeE.摆渡车_出;
        }
        
        /// <summary>
        /// 是否是下砖区域的轨道
        /// </summary>
        /// <returns></returns>
        public bool IsDownAreaTrack()
        {
            return Type == TrackTypeE.下砖轨道 || Type == TrackTypeE.储砖_入 || Type == TrackTypeE.储砖_出入 || Type == TrackTypeE.摆渡车_入;
        }


        public bool IsStoreTrack()
        {
            return Type == TrackTypeE.储砖_入 || Type == TrackTypeE.储砖_出 || Type == TrackTypeE.储砖_出入;
        }
    }
}
