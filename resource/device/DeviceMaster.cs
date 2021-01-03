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
            return DeviceList.FindAll(c => c.Type == type); ;
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

        public uint GetFerryTrackId(uint device_id)
        {
            return DeviceList.Find(c => c.id == device_id)?.left_track_id ?? 0;
        }

        public bool SetTileLifterGoods(uint devid, uint goodid)
        {
            Device dev = DeviceList.Find(c => c.id == devid);
            if(dev != null)
            {
                dev.goods_id = goodid;
                PubMaster.Mod.DevSql.EditDevice(dev);
                return true;
            }
            return false;
        }

        public bool SetTileLifterGoods(uint devid, DevLifterGoodsE goods1, DevLifterGoodsE goods2)
        {
            Device dev = DeviceList.Find(c => c.id == devid);
            if (dev != null)
            {
                dev.old_goodid = 0;
                dev.LeftGoods = goods1;
                dev.RightGoods = goods2;
                PubMaster.Mod.DevSql.EditeTileGood(dev);
                return true;
            }
            return false;
        }



        public uint GetFerryIdByFerryTrackId(uint ferrytrackid)
        {
            return DeviceList.Find(c => (c.Type == DeviceTypeE.下摆渡 || c.Type == DeviceTypeE.上摆渡) 
                                    && c.left_track_id == ferrytrackid)?.id ?? 0;
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

        public void SetCurrentTake(uint id, uint trackid)
        {
            Device device = GetDevice(id);
            if (device != null && device.Type == DeviceTypeE.上砖机)
            {
                device.CurrentTakeId = trackid;
                PubMaster.Mod.DevSql.EditDevice(device);
            }
        }

        public bool SetInStrategy(uint id, StrategyInE instrategy, DevWorkTypeE worktype)
        {
            Device device = GetDevice(id);
            if (device != null && (device.InStrategey != instrategy || device.WorkType != worktype))
            {
                device.InStrategey = instrategy;
                device.WorkType = worktype;
                PubMaster.Mod.DevSql.EditDevice(device);
                return true;
            }
            return false;
        }

        public bool SetOutStrategy(uint id, StrategyOutE outstrategy, DevWorkTypeE worktype)
        {
            Device device = GetDevice(id);
            if (device != null && (device.OutStrategey != outstrategy || device.WorkType != worktype))
            {
                device.OutStrategey = outstrategy;
                device.WorkType = worktype;
                PubMaster.Mod.DevSql.EditDevice(device);
                return true;
            }
            return false;
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

        public DevWorkTypeE GetDeviceWorkType(uint id)
        {
            Device device = GetDevice(id);
            if (device != null)
            {
                return device.WorkType;
            }
            return DevWorkTypeE.规格作业;
        }

        public uint GetLastWorkTrack(uint tileid)
        {
            return GetDevice(tileid)?.last_track_id ?? 0;
        }

        public void SetDeviceLastTrack(uint tileid, uint trackid)
        {
            Device device = GetDevice(tileid);
            if (device != null && device.last_track_id != trackid)
            {
                device.last_track_id = trackid;
                PubMaster.Mod.DevSql.EditDeviceLastTrack(device);
            }
        }

        public DeviceTypeE GetDeviceType(uint device_id)
        {
            return DeviceList.Find(c => c.id == device_id)?.Type ?? DeviceTypeE.其他;
        }

        public bool ExistDevByGid(uint goodid)
        {
            return DeviceList.Exists(c => c.goods_id == goodid || c.old_goodid == goodid || c.pre_goodid == goodid);
        }

        #endregion



        #region[砖机转产]

        public bool UpdateTilePreGood(uint tileid, uint nowgoodid, uint pregoodid, out string result)
        {
            Device device = GetDevice(tileid);
            if (device != null)
            {
                if(device.goods_id != nowgoodid)
                {
                    result = "请刷新设备信息！";
                    return false;
                }

                device.pre_goodid = pregoodid;
                PubMaster.Mod.DevSql.EditeTileGood(device);
                result = "";
                return true;
            }
            result = "找不到设备信息！";
            return false;
        }

        public bool UpdateShiftTileGood(uint tileid, uint nowgoodid, out string result)
        {
            Device device = GetDevice(tileid);
            if (device != null)
            {
                if(device.goods_id != nowgoodid)
                {
                    result = "请刷新设备信息！";
                    return false;
                }
                if (device.Type == DeviceTypeE.下砖机 && device.do_shift)
                {
                    result = "下砖机转产中！";
                    return false;
                }
                device.old_goodid = device.goods_id;
                device.goods_id = device.pre_goodid;
                device.pre_goodid = 0;
                device.do_shift = true;
                PubMaster.Mod.DevSql.EditeTileGood(device);
                result = "";
                return true;
            }
            result = "找不到设备信息！";
            return false;
        }

        #endregion
    }
}
