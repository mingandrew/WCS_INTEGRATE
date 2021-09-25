﻿using enums;
using module.device;
using module.deviceconfig;
using module.goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using tool.mlog;

namespace resource.device
{
    public class DeviceMaster
    {
        #region[构造/初始化]

        public DeviceMaster()
        {
            _obj = new object();
            DeviceList = new List<Device>();
            mLog = (Log)new LogFactory().GetLog("设备配置", false);
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
        private Log mLog;
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

        public List<Device> GetDevices(params DeviceTypeE[] types)
        {
            return DeviceList.FindAll(c => types.Contains(c.Type));
        }

        public List<Device> GetDevices(List<DeviceTypeE> types, List<uint> areaids)
        {
            return DeviceList.FindAll(c => types.Contains(c.Type) && areaids.Contains(c.area));
        }

        public List<Device> GetDevices(uint areaid, params DeviceTypeE[] types)
        {
            return DeviceList.FindAll(c => c.area == areaid && types.Contains(c.Type));
        }

        public List<Device> GetDevices(uint areaid, ushort lineid, params DeviceTypeE[] types)
        {
            return DeviceList.FindAll(c => c.area == areaid && c.line == lineid && types.Contains(c.Type));
        }
        public List<uint> GetDevIds(params DeviceTypeE[] types)
        {
            return DeviceList.FindAll(c => c.InType(types))?.Select(c => c.id).ToList();
        }

        public List<Device> GetFerrys()
        {
            return DeviceList.FindAll(c => c.Type == DeviceTypeE.上摆渡 || c.Type == DeviceTypeE.下摆渡);
        }

        public List<Device> GetTileLifters()
        {
            return DeviceList.FindAll(c => c.Type == DeviceTypeE.上砖机 || c.Type == DeviceTypeE.下砖机 || c.Type == DeviceTypeE.砖机);
        }

        public List<Device> GetTileLifters(List<uint> areaids)
        {
            return DeviceList.FindAll(c => areaids.Contains(c.area) && (c.Type == DeviceTypeE.上砖机 || c.Type == DeviceTypeE.下砖机 || c.Type == DeviceTypeE.砖机));
        }

        public List<Device> GetTileLifters(uint areaid)
        {
            List<uint> devids = PubMaster.Area.GetAreaTileIds(areaid);
            return DeviceList.FindAll(c => devids.Contains(c.id));
        }

        public List<Device> GetCarriers(uint areaid)
        {
            List<uint> devids = PubMaster.Area.GetAreaCarrierIds(areaid);
            return DeviceList.FindAll(c => devids.Contains(c.id));
        }

        public List<Device> GetTileLifters(uint areaid, DeviceTypeE type)
        {
            return DeviceList.FindAll(c => c.area == areaid && c.Type == type) ;
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

        public Device GetDeviceByMemo(string memo)
        {
            return DeviceList.Find(c => memo.Equals(c.memo));
        }

        public uint GetDevIdByMemo(string memo)
        {
            return DeviceList.Find(c => memo.Equals(c.memo))?.id ?? 0;
        }

        public bool IsDevType(uint devid, DeviceTypeE type)
        {
            return DeviceList.Exists(c => c.id == devid && c.Type == type);
        }

        /// <summary>
        /// 根据区域获取设备
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<Device> GetDeviceList(uint area)
        {
            List<Device> d = DeviceList.FindAll(c => c.area == area);
            if (d != null)
            {
                return d;
            }
            return new List<Device>();
        }

        #endregion

        #region[获取/判断属性]

        public string GetDeviceName(uint device_id, string defaultstr = "")
        {
            return DeviceList.Find(c => c.id == device_id)?.name ?? defaultstr;
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

        public bool SetDevWorking(uint devid, bool working, out DeviceTypeE type, string memo = "")
        {
            Device dev = DeviceList.Find(c => c.id == devid);
            if (dev != null)
            {
                try
                {
                    PubMaster.DevConfig.AddLog(string.Format("【启停状态】设备[ {0} ], 类型[ {1} ], 状态[ {2} -> {3} ], 备注[ {4} ]",
                        dev.name, dev.type, dev.do_work, working, memo));
                }
                catch { }
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

        /// <summary>
        /// 更新备用机的线
        /// </summary>
        /// <param name="backup_id"></param>
        /// <param name="need_id"></param>
        internal void SetBackUpLine(uint backup_id, uint need_id)
        {
            Device backdev = GetDevice(backup_id);
            Device needdev = GetDevice(need_id);
            if(backdev != null && needdev != null)
            {
                backdev.line = needdev.line;
                if (backdev.line > 0)
                {
                    PubMaster.Mod.DevSql.EditDeviceLine(backdev);
                }
            }
        }

        /// <summary>
        /// 获取设备配置的区域和线路
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        public void GetDeviceAreaLine(uint devid, out uint areaid, out ushort lineid)
        {
            Device dev = GetDevice(devid);
            if (dev != null)
            {
                areaid = dev.area;
                lineid = dev.line;
                return;
            }

            areaid = 0;
            lineid = 0;
        }

        public ushort GetDeviceLIne(uint devid)
        {
            return GetDevice(devid)?.line ?? 0;
        }

        /// <summary>
        /// 判断是否有设备是这条线的
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool HaveDeviceInLine(uint area, uint line)
        {
            return DeviceList.Exists(c => c.area == area && c.line == line);
        }
        #endregion

        #region[新增]
        public bool AddDevice(Device dev, out uint id)
        {
            id = 0;
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    if (DeviceList.Exists(c => c.area == dev.area && c.ip == dev.ip))
                    {
                        return false;
                    }
                    List<Device> areadevs = DeviceList.FindAll(c => c.area == dev.area);
                    uint newid = 1;
                    if (areadevs.Count > 0)
                    {
                        newid = DeviceList.Max(c => c.id) + 1;
                    }
                    dev.id = newid;
                    id = newid;
                    PubMaster.Mod.DevSql.AddDevice(dev);
                    DeviceList.Add(dev);
                    return true;
                }
                catch (Exception e)
                {

                    mLog.Error(true, e.Message + "新增失败：" + dev.name);
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
            return false;
        }
        #endregion

        #region[修改]

        public bool UpdateDevice(Device dev)
        {
            Device d = DeviceList.Find(c => c.id == dev.id);
            if (d == null)
            {
                return false;
            }
            PubMaster.Mod.DevSql.EditDevice(dev);
            d.Update(dev);
            return true;
        }

        #endregion

        #region[删除]
        public bool DeleteDevice(Device dev)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    if (!DeviceList.Exists(c => c.area == dev.area && c.ip == dev.ip))
                    {
                        return false;
                    }
                    PubMaster.Mod.DevSql.DeleteDevice(dev);
                    DeviceList.RemoveAll(c => c.area == dev.area && c.ip == dev.ip && c.id == dev.id);
                    return true;
                }
                catch (Exception e)
                {

                    mLog.Error(true, e.Message + "删除失败：" + dev.name);
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
            return false;
        }
        #endregion
    }
}
