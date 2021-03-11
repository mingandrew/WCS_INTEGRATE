using enums;
using GalaSoft.MvvmLight;
using simtask;
using simtask.task;

namespace wcs.Data.View.sim
{
    public class SimDeviceView : ViewModelBase
    {
        public uint area_id { set; get; }
        public string dev_name { set; get; }
        public uint dev_id { set; get; }
        public DeviceTypeE DevType { set; get; }
        private bool working;
        public bool Working
        {
            get => working;
            set
            {
                if (Set(ref working, value))
                {
                    switch (DevType)
                    {
                        case DeviceTypeE.砖机:
                        case DeviceTypeE.上砖机:
                        case DeviceTypeE.下砖机:
                            SimServer.TileLifter.StartOrStopWork(dev_id, value);
                            break;
                        case DeviceTypeE.上摆渡:
                        case DeviceTypeE.下摆渡:
                            break;
                        case DeviceTypeE.运输车:

                            break;
                        case DeviceTypeE.其他:
                            break;
                    }
                }
            }
        }

        public SimDeviceView(SimTaskBase task)
        {
            area_id = task.Device.area;
            dev_id = task.Device.id;
            DevType = task.Device.Type;
        }
    }
}
