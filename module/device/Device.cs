using enums;
using System;
using System.Net.Configuration;

namespace module.device
{
    public class Device
    {
        public uint id { set; get; }
        public string name { set; get; }
        public string ip { set; get; }
        public ushort port { set; get; }
        public byte type { set; get; }
        public byte type2 { set; get; }
        public bool enable { set; get; }
        public byte att1 { set; get; }//用于区分运输车类型  窄 宽
        public byte att2 { set; get; }//用于区分运输车职责  上砖 下砖 ;用于上砖机优先清空轨道使用
        public uint goods_id { set; get; }
        public uint left_track_id { set; get; }
        public uint right_track_id { set; get; }
        public uint brother_dev_id { set; get; }
        public byte strategy_in { set; get; }
        public byte strategy_out { set; get; }
        public string memo { set; get; }
        public ushort area { set; get; }
        public DateTime? offlinetime { set; get; }//离线时间
        public bool a_givemisstrack { set; get; }//前进放货没扫到地标
        public bool a_takemisstrack { set; get; }//后退取砖没扫到地标
        public bool a_poweroff { set; get; }//设备轨道掉电
        public uint a_alert_track { set; get; }//故障轨道
        public bool do_work { set; get; }//是否作业
        public byte work_type { set; get; }//作业类型
        public bool ignorearea { set; get; }//忽略区域
        public uint last_track_id { set; get; }//砖机上次作业轨道（混砖下砖使用）

        public uint old_goodid { set; get; }//上一个品种
        public uint pre_goodid { set; get; }//预设品种
        public bool do_shift { set; get; }//是否转产
        public byte left_goods { set; get; }//工位品种 左
        public byte right_goods { set; get; }//工位品种 右

        public DevLifterGoodsE LeftGoods
        {
            get => (DevLifterGoodsE)left_goods;
            set => left_goods = (byte)value;
        }

        public DevLifterGoodsE RightGoods
        {
            get => (DevLifterGoodsE)right_goods;
            set => right_goods = (byte)value;
        }

        public DevWorkTypeE WorkType
        {
            get => (DevWorkTypeE)work_type;
            set => work_type = (byte)value;
        }

        public DeviceTypeE Type
        {
            get => (DeviceTypeE)type;
            set => type = (byte)value;
        }

        public DeviceType2E Type2
        {
            get => (DeviceType2E)type2;
            set => type2 = (byte)value;
        }

        /// <summary>
        /// 是否有干预设备
        /// </summary>
        public bool HaveBrother
        {
            get => brother_dev_id != 0;
        }

        public StrategyInE InStrategey
        {
            get => (StrategyInE)strategy_in;
            set => strategy_in = (byte)value;
        }

        public StrategyOutE OutStrategey
        {
            get => (StrategyOutE)strategy_out;
            set => strategy_out = (byte)value;
        }

        public CarrierTypeE CarrierType
        {
            get => (CarrierTypeE)att1;
            set => att1 = (byte)value;
        }

        /// <summary>
        /// 运输车职责
        /// </summary>
        public CarrierDutyE CarrierDuty
        {
            get => (CarrierDutyE)att2;
            set => att2 = (byte)value;
        }

        /// <summary>
        /// 上砖机取轨
        /// </summary>
        public uint CurrentTakeId
        {
            get => att2;
            set => att2 = (byte)value;
        }
    }
}
