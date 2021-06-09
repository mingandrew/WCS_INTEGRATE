using enums;
using GalaSoft.MvvmLight;
using module.goods;
using System;
using System.Text;
using task;

namespace wcs.Data.View
{
    public class TransView : ViewModelBase
    {

        #region[属性]

        private uint id;
        private TransTypeE trans_type;
        private TransStatusE trans_status;
        private uint goods_id;
        private uint stock_id;
        private uint take_track_id;
        private uint give_track_id;
        private uint finish_track_id;
        private uint tilelifter_id;
        private uint take_ferry_id;//取货摆渡车
        private uint give_ferry_id;//卸货摆渡车
        private uint carrier_id;
        private DateTime? create_time;
        private DateTime? load_time;
        private DateTime? unload_time;
        private bool finish;
        private DateTime? finish_time;

        private string tcmsg;//交管信息

        private StringBuilder stepinfo = new StringBuilder();//当前步骤信息

        public uint Id
        {
            get => id;
            set => Set(ref id, value);
        }

        public uint Area_id { set; get; }
        public ushort Line_id { set; get; }
        public uint Goods_id
        {
            get => goods_id;
            set => Set(ref goods_id, value);
        }

        public uint Stock_id
        {
            get => stock_id;
            set => Set(ref stock_id, value);
        }

        public TransTypeE TransType
        {
            get => trans_type;
            set => Set(ref trans_type, value);
        }

        public TransStatusE TransStaus
        {
            get => trans_status;
            set => Set(ref trans_status, value);
        }

        public uint Take_track_id
        {
            get => take_track_id;
            set => Set(ref take_track_id, value);
        }
        public uint Give_track_id
        {
            get => give_track_id;
            set => Set(ref give_track_id, value);
        }
        public uint Finish_track_id
        {
            get => finish_track_id;
            set => Set(ref finish_track_id, value);
        }
        public uint Tilelifter_id
        {
            get => tilelifter_id;
            set => Set(ref tilelifter_id, value);
        }
        public uint Take_ferry_id
        {
            get => take_ferry_id;
            set => Set(ref take_ferry_id, value);
        }
        public uint Give_ferry_id
        {
            get => give_ferry_id;
            set => Set(ref give_ferry_id, value);
        }
        public uint Carrier_id
        {
            get => carrier_id;
            set => Set(ref carrier_id, value);
        }

        public DateTime? Create_time
        {
            get => create_time;
            set => Set(ref create_time, value);
        }

        public DateTime? Load_time
        {
            get => load_time;
            set => Set(ref load_time, value);
        }

        public DateTime? Unload_time
        {
            get => unload_time;
            set => Set(ref unload_time, value);
        }

        public DateTime? Finish_time
        {
            get => finish_time;
            set => Set(ref finish_time, value);
        }

        public string TCmsg
        {
            get => tcmsg;
            set => tcmsg = value;
        }

        public string StepInfo
        {
            get => stepinfo.ToString();
            set => stepinfo = stepinfo.Insert(0, value);
        }

        #endregion
        public TransView(StockTrans trans)
        {
            Id = trans.id;
            Area_id = trans.area_id;
            Line_id = trans.line;
            TransType = trans.TransType;
            Goods_id = trans.goods_id;
            Stock_id = trans.stock_id;
            Create_time = trans.create_time;
            Update(trans);
        }

        public void Update(StockTrans trans)
        {
            TransStaus = trans.TransStaus;
            Take_ferry_id = trans.take_ferry_id;
            Give_ferry_id = trans.give_ferry_id;
            Tilelifter_id = trans.tilelifter_id;
            Carrier_id = trans.carrier_id;
            Take_track_id = trans.take_track_id;
            Give_track_id = trans.give_track_id;
            Finish_track_id = trans.finish_track_id;
            Load_time = trans.load_time;
            Unload_time = trans.unload_time;
            finish = trans.finish;
            Finish_time = trans.finish_time;

            if (trans.StepLog != null)
            {
                StepInfo = trans.StepLog.StepInfo;
            }

            // 交管信息
            if (trans.give_ferry_id > 0) // 卸货摆渡车
            {
                if (!trans.IsReleaseGiveFerry)
                {
                    TCmsg = PubTask.TrafficControl.GetTrafficCtlInfoForFerry(trans.give_ferry_id);
                }
            }
            else if (trans.take_ferry_id > 0) // 取货摆渡车
            {
                if (!trans.IsReleaseTakeFerry)
                {
                    TCmsg = PubTask.TrafficControl.GetTrafficCtlInfoForFerry(trans.take_ferry_id);
                }
            }
        }

        /// <summary>
        /// 在界面展示超过1h，然后清掉该任务
        /// </summary>
        /// <returns></returns>
        public bool IsOver1Hours()
        {
            if(finish_time is DateTime time)
            {
                return (DateTime.Now - time).TotalMinutes > 60;
            }
            return false;
        }
    }
}
