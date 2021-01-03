using GalaSoft.MvvmLight;
using module.track;

namespace wcs.Data.View
{
    public class FerryPosView : ViewModelBase
    {
        #region[字段]
        private uint id;
        private uint track_id;
        private uint device_id;
        private ushort ferry_code;
        private int ferry_pos;

        #endregion

        #region[属性]
         public uint Id
        {
            get => id;
            set => Set(ref id, value);
        }
        public uint Track_Id
        {
            get => track_id;
            set => Set(ref track_id, value);
        }
        public uint Device_Id
        {
            get => device_id;
            set => Set(ref device_id, value);
        }
        public ushort Ferry_Code
        {
            get => ferry_code;
            set => Set(ref ferry_code, value);
        }
        public int Ferry_Pos
        {
            get => ferry_pos;
            set => Set(ref ferry_pos, value);
        }

        #endregion


        public FerryPosView(FerryPos pos)
        {
            Id = pos.id;
            Track_Id = pos.track_id;
            Device_Id = pos.device_id;
            Ferry_Code = pos.ferry_code;
            Ferry_Pos = pos.ferry_pos;
        }

    }
}
