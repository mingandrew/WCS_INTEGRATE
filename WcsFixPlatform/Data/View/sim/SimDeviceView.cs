using enums;
using GalaSoft.MvvmLight;
using simtask;
using simtask.task;

namespace wcs.Data.View.sim
{
    public class SimDeviceView : ViewModelBase
    {
        private bool? working;
        public uint area_id { set; get; }
        public ushort line_id { set; get; }
        public string dev_name { set; get; }
        public uint dev_id { set; get; }
        public DeviceTypeE DevType { set; get; }
        public bool? Working
        {
            get => working;
            set
            {
                if(Set(ref working, value))
                {
                    SimServer.TileLifter.StartOrStopWork(dev_id, (bool)value);
                }
            }
        }
        public bool IsTwoTrackTile { set; get; }

        public SimDeviceView(SimTaskBase task)
        {
            area_id = task.Device.area;
            line_id = task.LineId;
            dev_id = task.Device.id;
            DevType = task.Device.Type;

            IsTwoTrackTile = task.Device.Type2 == DeviceType2E.双轨;
        }
    }
}
