using enums;
using GalaSoft.MvvmLight;
using module.goods;
using System;

namespace wcs.Data.View
{
    public class TransView : ViewModelBase
    {

        #region[属性]

        private uint id;
        private TransTypeE trans_type;
        private TransStatusE trans_status;
        private uint area_id;
        private uint goods_id;
        private uint stock_id;
        private uint take_track_id;
        private uint give_track_id;
        private uint tilelifter_id;
        private uint take_ferry_id;//取货摆渡车
        private uint give_ferry_id;//卸货摆渡车
        private uint carrier_id;
        private DateTime? create_time;
        private DateTime? load_time;
        private DateTime? unload_time;
        private bool finish;
        private DateTime? finish_time;

        public uint Id
        {
            get => id;
            set => id = value;
        }

        public uint Area_id
        {
            get => area_id;
            set => area_id = value;
        }

        public uint Goods_id
        {
            get => goods_id;
            set => goods_id = value;
        }

        public uint Stock_id
        {
            get => stock_id;
            set => stock_id = value;
        }

        public TransTypeE TransType
        {
            get => trans_type;
            set => trans_type = value;
        }

        public TransStatusE TransStaus
        {
            get => trans_status;
            set => Set(ref trans_status, value);
        }

        public uint Take_track_id
        {
            get => take_track_id;
            set => take_track_id = value;
        }
        public uint Give_track_id
        {
            get => give_track_id;
            set => give_track_id = value;
        }
        public uint Tilelifter_id
        {
            get => tilelifter_id;
            set => tilelifter_id = value;
        }
        public uint Take_ferry_id
        {
            get => take_ferry_id;
            set => take_ferry_id = value;
        }
        public uint Give_ferry_id
        {
            get => give_ferry_id;
            set => give_ferry_id = value;
        }
        public uint Carrier_id
        {
            get => carrier_id;
            set => carrier_id = value;
        }

        public DateTime? Create_time
        {
            get => create_time;
            set => create_time = value;
        }

        public DateTime? Load_time
        {
            get => load_time;
            set => load_time = value;
        }

        public DateTime? Unload_time
        {
            get => unload_time;
            set => unload_time = value;
        }


        #endregion
        public TransView(StockTrans trans)
        {
            Id = trans.id;
            Area_id = trans.area_id;
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
            Load_time = trans.load_time;
            Unload_time = trans.unload_time;
            finish = trans.finish;
            finish_time = trans.finish_time;
        }

        /// <summary>
        /// 在界面展示超过1消失，然后清掉该任务
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
