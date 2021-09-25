using enums;
using GalaSoft.MvvmLight;

namespace wcs.Data.View
{
    public class SynroTileView : ViewModelBase
    {
        #region[字段]

        private uint id;
        private string name;
        private byte type { set; get; }
        private bool isconnect;

        private bool isselected;
        #endregion

        #region[属性]
        public uint ID
        {
            get => id;
            set => Set(ref id, value);
        }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public bool IsSelected
        {
            get => isselected;
            set => Set(ref isselected, value);
        }

        public DeviceTypeE Type
        {
            get => (DeviceTypeE)type;
            set => type = (byte)value;
        }

        public bool IsConnect
        {
            get => isconnect;
            set => Set(ref isconnect, value);
        }

        public string label
        {
            get
            {
                if (isconnect)
                {
                    return "在线";
                }
                return "离线";
            }
        }
        #endregion

        #region[更新]

        #endregion
    }
}
