using enums;
using GalaSoft.MvvmLight;
using module.goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcs.Data.View
{
    public class OrgGoodTrackView : ViewModelBase
    {
        public OrgGoodTrackView()
        {

        }

        public OrgGoodTrackView(StockTransDtl dtl)
        {
            Dtl_ID = dtl.dtl_id;
            Dtl_P_ID = dtl.dtl_p_id;
            good_id = dtl.dtl_good_id;
            area_id = dtl.dtl_area_id;
            trans_id = dtl.dtl_trans_id;
            take_track_id = dtl.dtl_take_track_id;

            Update(dtl);
        }

        #region[属性]
        public uint Dtl_ID { set; get; }
        public uint Dtl_P_ID { set; get; }

        private uint area_id;
        private uint trans_id;
        private uint take_track_id;
        private uint give_track_id;
        private uint good_id;
        private int level;
        private ushort all_qty;
        private ushort left_qty;
        private StockTransDtlTypeE dtl_type;
        private StockTransDtlStatusE dtl_status;
        public uint Area_Id
        {
            get => area_id;
            set => Set(ref area_id, value);
        }

        public uint Trans_Id
        {
            get => trans_id;
            set => Set(ref trans_id, value);
        }

        public uint Take_Track_Id
        {
            get => take_track_id;
            set => Set(ref take_track_id, value);
        }

        public uint Give_Track_Id
        {
            get => give_track_id;
            set => Set(ref give_track_id, value);
        }

        public uint Good_Id
        {
            get => good_id;
            set => Set(ref good_id, value);
        }

        public int Level
        {
            get => level;
            set => Set(ref level, value);
        }

        public ushort All_Qty
        {
            get => all_qty;
            set => Set(ref all_qty, value);
        }

        public ushort Left_Qty
        {
            get => left_qty;
            set => Set(ref left_qty, value);
        }

        public StockTransDtlTypeE DtlType
        {
            get => dtl_type;
            set => Set(ref dtl_type, value);
        }

        public StockTransDtlStatusE DtlStatus
        {
            get => dtl_status;
            set => Set(ref dtl_status, value);
        }

        #endregion


        public void Update(StockTransDtl dtl)
        {
            Trans_Id = dtl.dtl_trans_id;
            All_Qty = dtl.dtl_all_qty;
            Left_Qty = dtl.dtl_left_qty;
            DtlType = dtl.DtlType;
            Give_Track_Id = dtl.dtl_give_track_id;
            DtlStatus = dtl.DtlStatus;
            Level = (int)dtl.dtl_level;
        }

        public void SetDtlType(StockTransDtlTypeE type)
        {
            DtlType = type;
        }
    }
}
