using enums;
using GalaSoft.MvvmLight;
using simtask;
using simtask.task;

namespace wcs.Data.View.sim
{
    public class SimDeviceView : ViewModelBase
    {
        private bool working;
        public uint area_id { set; get; }
        public string dev_name { set; get; }
        public uint dev_id { set; get; }
        public DeviceTypeE DevType { set; get; }
        public bool Working
        {
            get => working;
            set => Set(ref working, value);
        }
        public bool IsTwoTrackTile { set; get; }

        public SimDeviceView(SimTaskBase task)
        {
            area_id = task.Device.area;
            dev_id = task.Device.id;
            DevType = task.Device.Type;

            IsTwoTrackTile = task.Device.Type2 == DeviceType2E.双轨;
        }
    }
}
