using enums;
using module.device;
using module.goods;
using System;
using System.Collections.Generic;
using System.Threading;

namespace resource.device
{
    public class DeviceMaster
    {
        #region[构造/初始化]

        public DeviceMaster()
        {
            _obj = new object();
            DeviceList = new List<Device>();
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true, bool refr_2 = true,bool refr_3 = true)
        {
            if (refr_1)
            {
                DeviceList.Clear();
                DeviceList.AddRange(PubMaster.Mod.DevSql.QueryDeviceList());
            }
        }

        public void Stop()
        {

        }
        #endregion

        #region[字段]
        private readonly object _obj;
        private List<Device> DeviceList { set; get; }

        #endregion

        #region[获取对象]

        public List<Device> GetDeviceList()
        {
            return DeviceList;
        } 

        public List<Device> GetDeviceList(DeviceTypeE type)
        {
            return DeviceList.FindAll(c => c.Type == type);
        }

        public List<Device> GetDevices(List<DeviceTypeE> types)
        {
            return DeviceList.FindAll(c => types.Contains(c.Type));
        }

        public List<Device> GetDevices(List<DeviceTypeE> types, uint areaid)
        {
            return DeviceList.FindAll(c => c.area == areaid && types.Contains(c.Type));
        }

        public List<Device> GetFerrys()
        {
            return DeviceList.FindAll(c => c.Type == DeviceTypeE.上摆渡 || c.Type == DeviceTypeE.下摆渡);
        }

        public List<Device> GetTileLifters()
        {
            return DeviceList.FindAll(c => c.Type == DeviceTypeE.上砖机 || c.Type == DeviceTypeE.下砖机);
        }

        public List<Device> GetTileLifters(uint areaid)
        {
            List<uint> devids = PubMaster.Area.GetAreaTileIds(areaid);
            return DeviceList.FindAll(c => devids.Contains(c.id));
        }

        public List<Device> GetFerrys(uint areaid)
        {
            List<uint> devids = PubMaster.Area.GetAreaFerryIds(areaid);
            return DeviceList.FindAll(c => devids.Contains(c.id));
        }

        public Device GetDevice(uint devid)
        {
            return DeviceList.Find(c => c.id == devid);
        }

        public bool IsDevType(uint devid, DeviceTypeE type)
        {
            return DeviceList.Exists(c => c.id == devid && c.Type == type);
        }

        #endregion

        #region[获取/判断属性]

        public string GetDeviceName(uint device_id)
        {
            return DeviceList.Find(c => c.id == device_id)?.name ?? "";
        }

        public void SetEnable(uint id, bool isenable)
        {
            Device device = GetDevice(id);
            if (device != null)
            {
                device.enable = isenable;
                PubMaster.Mod.DevSql.EditDevice(device);
            }
        }

        public uint GetDeviceArea(uint iD)
        {
            return DeviceList.Find(c => c.id == iD)?.area ?? 0;
        }

        public bool SetDevWorking(uint devid, bool working, out DeviceTypeE type)
        {
            Device dev = DeviceList.Find(c => c.id == devid);
            if (dev != null)
            {
                dev.do_work = working;
                PubMaster.Mod.DevSql.EditDevice(dev);
                type = dev.Type;
                return true;
            }
            type = DeviceTypeE.其他;
            return false;
        }

        public DeviceTypeE GetDeviceType(uint device_id)
        {
            return DeviceList.Find(c => c.id == device_id)?.Type ?? DeviceTypeE.其他;
        }

        #endregion

    }
}
