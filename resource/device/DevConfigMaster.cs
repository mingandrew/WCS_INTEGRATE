using enums;
using module.device;
using module.deviceconfig;
using System;
using System.Collections.Generic;
using System.Linq;
using tool.mlog;

namespace resource.device
{
    public class DevConfigMaster
    {
        #region[字段]
        private Log mLog;
        private readonly object _obj;

        /// <summary>
        /// 运输车 配置LIST
        /// </summary>
        private List<ConfigCarrier> ConfigCarrierList { set; get; }

        /// <summary>
        /// 摆渡车 配置LIST
        /// </summary>
        private List<ConfigFerry> ConfigFerryList { set; get; }

        /// <summary>
        /// 砖机 配置LIST
        /// </summary>
        private List<ConfigTileLifter> ConfigTileLifterList { set; get; }

        #endregion

        #region[构造/初始化]

        public DevConfigMaster()
        {
            _obj = new object();
            ConfigCarrierList = new List<ConfigCarrier>();
            ConfigFerryList = new List<ConfigFerry>();
            ConfigTileLifterList = new List<ConfigTileLifter>();
            mLog = (Log)new LogFactory().GetLog("砖机配置", false);
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true, bool refr_2 = true, bool refr_3 = true)
        {
            if (refr_1)
            {
                ConfigCarrierList.Clear();
                ConfigCarrierList.AddRange(PubMaster.Mod.DevConfigSql.QueryConfigCarrier());
            }

            if (refr_2)
            {
                ConfigFerryList.Clear();
                ConfigFerryList.AddRange(PubMaster.Mod.DevConfigSql.QueryConfigFerry());
            }

            if (refr_3)
            {
                ConfigTileLifterList.Clear();
                ConfigTileLifterList.AddRange(PubMaster.Mod.DevConfigSql.QueryConfigTileLifter());
            }
        }

        public void Stop()
        {

        }

        #endregion

        #region[获取对象]

        #region 运输车

        /// <summary>
        /// 获取所有运输车配置信息
        /// </summary>
        /// <returns></returns>
        public List<ConfigCarrier> GetConfigCarrier()
        {
            return ConfigCarrierList;
        }

        /// <summary>
        /// 获取单个运输车配置信息
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public ConfigCarrier GetCarrier(uint devid)
        {
            return ConfigCarrierList.Find(c => c.id == devid) ?? new ConfigCarrier();
        }

        #endregion

        #region 摆渡车

        /// <summary>
        /// 获取所有摆渡车配置信息
        /// </summary>
        /// <returns></returns>
        public List<ConfigFerry> GetConfigFerry()
        {
            return ConfigFerryList;
        }

        /// <summary>
        /// 获取单个摆渡车配置信息
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public ConfigFerry GetFerry(uint devid)
        {
            return ConfigFerryList.Find(c => c.id == devid) ?? new ConfigFerry();
        }


        #endregion

        #region 砖机

        /// <summary>
        /// 获取所有砖机配置信息
        /// </summary>
        /// <returns></returns>
        public List<ConfigTileLifter> GetConfigTileLifter()
        {
            return ConfigTileLifterList;
        }

        /// <summary>
        /// 获取单个砖机配置信息
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public ConfigTileLifter GetTileLifter(uint devid)
        {
            return ConfigTileLifterList.Find(c => c.id == devid) ?? new ConfigTileLifter();
        }


        /// <summary>
        /// 判断品种是否与砖机当前品种相同
        /// </summary>
        /// <param name="goodid">判断是否相同的品种</param>
        /// <param name="type">进行判断的砖机类型</param>
        /// <returns>找到砖机返回：true, 反之则返回：false</returns>
        public bool IsHaveSameTileNowGood(uint goodid, TileWorkModeE type)
        {
            return ConfigTileLifterList.Exists(c => c.WorkMode == type && c.goods_id == goodid);
        }



        /// <summary>
        /// 判断区域上砖机品种都是一样的
        /// </summary>
        /// <param name="area_id"></param>
        /// <returns></returns>
        public bool IsAreaUpTileGoodNotSame(uint area_id, uint tileid)
        {
            ConfigTileLifter conf = GetTileLifter(tileid);
           List<Device> devs = PubMaster.Device.GetTileLifters(area_id, DeviceTypeE.上砖机);
            return ConfigTileLifterList.Exists(c => devs.Exists(d => d.id == c.id) && conf.goods_id != c.goods_id);
        }
        #endregion

        #endregion

        #region[获取/判断属性]

        #region 运输车

        /// <summary>
        /// 获取运输车顶板长度
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public ushort GetCarrierLenght(uint devid)
        {
            if(devid == 0)
            {
                return ConfigCarrierList.Find(c => c.length > 0)?.length ?? 0;
            }
            return ConfigCarrierList.Find(c => c.id == devid)?.length ?? 0;
        }

        /// <summary>
        /// 获取区域里面的小车长
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <returns></returns>
        public ushort GetCarrierLenghtByArea(uint areaid)
        {
            List<Device> list = PubMaster.Device.GetDevices(areaid,DeviceTypeE.运输车);
            return ConfigCarrierList.Find(c => c.length > 0 && list.Exists(d=>d.id == c.id))?.length ?? 0;
        }

        /// <summary>
        /// 获取绑定的库存id的运输车名字
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool GetCarrierByStockid(uint stockid, out string name)
        {
            ConfigCarrier car = ConfigCarrierList.Find(c => c.stock_id == stockid);
            if (car != null)
            {
                name = PubMaster.Device.GetDeviceName(car.id);
                return true;
            }
            name = "";
            return false;
        }

        #endregion

        #region 摆渡车

        /// <summary>
        /// 获取摆渡车轨道ID
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetFerryTrackId(uint devid)
        {
            return ConfigFerryList.Find(c => c.id == devid)?.track_id ?? 0;
        }

        /// <summary>
        /// 获取该轨道ID对应的摆渡车设备ID
        /// </summary>
        /// <param name="ferrytrackid"></param>
        /// <returns></returns>
        public uint GetFerryIdByFerryTrackId(uint ferrytrackid)
        {
            return ConfigFerryList.Find(c => c.track_id == ferrytrackid)?.id ?? 0;
        }

        #endregion

        #region 砖机

        /// <summary>
        /// 获取砖机预设品种
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetDevicePreId(uint devid)
        {
            return ConfigTileLifterList.Find(c => c.id == devid)?.pre_goodid ?? 0;
        }

        /// <summary>
        /// 获取砖机作业类型
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public DevWorkTypeE GetTileWorkType(uint devid)
        {
            return GetTileLifter(devid)?.WorkType ?? DevWorkTypeE.品种作业;
        }

        /// <summary>
        /// 获取砖机作业模式
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public TileWorkModeE GetTileWorkMode(uint devid)
        {
            return GetTileLifter(devid)?.WorkMode ?? TileWorkModeE.无;
        }

        /// <summary>
        /// 获取砖机最后作业轨道ID
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetLastTrackId(uint devid)
        {
            return GetTileLifter(devid)?.last_track_id ?? 0;
        }

        /// <summary>
        /// 是否符合砖机作业模式
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool IsTileWorkMod(uint devid, TileWorkModeE mode)
        {
            return ConfigTileLifterList.Exists(c => c.id == devid && c.WorkMode == mode);
        }

        /// <summary>
        /// 是否存在该品种的砖机
        /// </summary>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public bool ExistTileLifterByGid(uint goodid)
        {
            return ConfigTileLifterList.Exists(c => c.goods_id == goodid || c.old_goodid == goodid || c.pre_goodid == goodid);
        }

        /// <summary>
        /// 转产完成后，下砖机设置砖机作业品种
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public bool SetTileLifterGoods(uint devid, uint goodid)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                try
                {
                    mLog.Status(true, string.Format("【品种修改】砖机[ {0} ], 品种[ {1} -> {2} ], 标识[ {3} -> {4} ], 数量[ {5} ]",
                        PubMaster.Device.GetDeviceName(dev.id),
                        PubMaster.Goods.GetGoodsName(dev.goods_id),
                        PubMaster.Goods.GetGoodsName(goodid), dev.goods_id, goodid,
                        dev.now_good_all ? "不限" : (dev.now_good_qty + "")));
                }
                catch { }
                dev.goods_id = goodid;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Goods);
                return true;
            }
            return false;
        }
        

        /// <summary>
        /// 转产完成后，下砖机设置砖机作业品种
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public bool SetTileLifterGoodsAllCount(uint devid, uint goodid)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                try
                {
                    mLog.Status(true, string.Format("【品种修改2】砖机[ {0} ], 品种[ {1} -> {2} ], 标识[ {3} -> {4} ], 数量[ 不限 ]",
                        PubMaster.Device.GetDeviceName(dev.id),
                        PubMaster.Goods.GetGoodsName(dev.goods_id),
                        PubMaster.Goods.GetGoodsName(goodid), dev.goods_id, goodid));
                }
                catch { }
                dev.goods_id = goodid;
                dev.now_good_all = true;
                dev.now_good_qty = 0;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Goods);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置砖机入库策略
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="instrategy"></param>
        /// <param name="worktype"></param>
        /// <returns></returns>
        public bool SetInStrategy(uint devid, StrategyInE instrategy, DevWorkTypeE worktype)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null && (dev.InStrategey != instrategy || dev.WorkType != worktype))
            {
                try
                {
                    mLog.Status(true, string.Format("【入库逻辑】砖机[ {0} ], 策略[ {1} -> {2} ], 作业类型[ {3} -> {4} ]", 
                        PubMaster.Device.GetDeviceName(dev.id),
                        dev.InStrategey, instrategy,
                        dev.WorkType, worktype));
                }
                catch { }
                dev.InStrategey = instrategy;
                dev.WorkType = worktype;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Strategey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置砖机出库策略
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="outstrategy"></param>
        /// <param name="worktype"></param>
        /// <returns></returns>
        public bool SetOutStrategy(uint devid, StrategyOutE outstrategy, DevWorkTypeE worktype)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null && (dev.OutStrategey != outstrategy || dev.WorkType != worktype))
            {
                try
                {
                    mLog.Status(true, string.Format("【出库逻辑】砖机[ {0} ], 策略[ {1} -> {2} ]",
                        PubMaster.Device.GetDeviceName(dev.id),
                        dev.OutStrategey, outstrategy));
                }
                catch { }
                dev.OutStrategey = outstrategy;
                dev.WorkType = worktype;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Strategey);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取砖机配置的工位取货地标点
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <param name="take_track_id"></param>
        /// <returns></returns>
        public ushort GetTileSite(uint tilelifter_id, uint take_track_id)
        {
            ConfigTileLifter dev = GetTileLifter(tilelifter_id);
            if (dev != null)
            {
                if(dev.left_track_id == take_track_id)
                {
                    return dev.left_track_point;
                }

                if(dev.right_track_id == take_track_id)
                {
                    return dev.right_track_point;
                }
            }
            return 0;
        }

        /// <summary>
        /// 设置砖机最后作业轨道
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool SetLastTrackId(uint devid, uint trackid)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null && dev.last_track_id != trackid)
            {
                try
                {
                    mLog.Status(true, string.Format("【最后作业轨道】砖机[ {0} ], 轨道[ {1} ]",
                        PubMaster.Device.GetDeviceName(dev.id), PubMaster.Track.GetTrackName(trackid, trackid+"")));
                }
                catch { }
                dev.last_track_id = trackid;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.LastTrack);
                return true;
            }
            return false;
        }

        #region 不作业轨道

        /// <summary>
        /// 获取砖机不作业轨道ID
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public uint GetNonWorkTrackId(uint devid)
        {
            return GetTileLifter(devid)?.non_work_track_id ?? 0;
        }

        /// <summary>
        /// 设置下砖机不作业轨道
        /// </summary>
        /// <param name="takeid"></param>
        public void SetDownTileNonWorkTrack(uint tarckid)
        {
            // 根据当前作业轨道获取所有下砖机
            List<ConfigTileLifter> ctls = ConfigTileLifterList.FindAll(c => c.last_track_id == tarckid && c.WorkMode == TileWorkModeE.下砖);

            if (ctls != null && ctls.Count > 0)
            {
                foreach (ConfigTileLifter item in ctls)
                {
                    SetNonWorkTrackId(item.goods_id, item.non_work_track_id, tarckid);
                }
            }
        }

        /// <summary>
        /// 更新不作业轨道
        /// </summary>
        /// <param name="goodsid"></param>
        /// <param name="oldtarckid"></param>
        /// <param name="newtrackid"></param>
        private void SetNonWorkTrackId(uint goodsid, uint oldtarckid, uint newtrackid)
        {
            foreach (ConfigTileLifter ctl in ConfigTileLifterList.FindAll(c => c.goods_id == goodsid && c.non_work_track_id == oldtarckid && c.WorkMode == TileWorkModeE.下砖))
            {
                if (ctl.non_work_track_id != newtrackid)
                {
                    try
                    {
                        mLog.Status(true, string.Format("【更新不作业轨道】砖机[ {0} ], 轨道[ {1} -> {2} ]",
                            PubMaster.Device.GetDeviceName(ctl.id),
                            PubMaster.Track.GetTrackName(oldtarckid, oldtarckid + ""),
                            PubMaster.Track.GetTrackName(newtrackid, newtrackid + "")));
                    }
                    catch { }
                    ctl.non_work_track_id = newtrackid;
                    PubMaster.Mod.DevConfigSql.EditConfigTileLifter(ctl, TileConfigUpdateE.NoWorkTrack);
                }
            }
        }

        #endregion

        #region [串联砖机]

        /// <summary>
        /// 根据轨道ID获取配置了该轨道的砖机ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<uint> GetTileInTrack(uint id)
        {
            return ConfigTileLifterList.FindAll(c => c.InTrack(id))?.Select(c => c.id).ToList() ?? new List<uint>();
        }

        /// <summary>
        /// 判断是否有兄弟砖机
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool HaveBrother(uint devid)
        {
            return ConfigTileLifterList.Exists(c => c.id == devid && c.HaveBrother);
        }

        /// <summary>
        /// 判断是不是外面的砖机
        /// </summary>
        /// <param name="tileid"></param>
        /// <returns></returns>
        public bool IsBrother(uint tileid)
        {
            return ConfigTileLifterList.Exists(c => c.brother_dev_id == tileid);
        }

        /// <summary>
        /// 获取砖机对应的地标
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public uint GetTileInPoint(uint trackid, ushort site)
        {
            return ConfigTileLifterList.Find(c => (c.left_track_id == trackid && c.left_track_point == site)
                    || (c.right_track_id == trackid && c.right_track_point == site))?.id ?? 0 ;
        }

        /// <summary>
        /// 获取内侧砖机的ID
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <returns></returns>
        public uint GetBrotherIdInside(uint tilelifter_id)
        {
            return ConfigTileLifterList.Find(c => c.brother_dev_id == tilelifter_id)?.id ?? 0;
        }

        /// <summary>
        /// 获取外侧砖机的ID
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <returns></returns>
        public uint GetBrotherIdOutside(uint tilelifter_id)
        {
            return ConfigTileLifterList.Find(c => c.id == tilelifter_id)?.brother_dev_id ?? 0;
        }

        /// <summary>
        /// 当前是否有砖机是否该品种
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool HaveTileSetGood(uint id)
        {
            return ConfigTileLifterList.Exists(c => c.goods_id == id);
        }

        /// <summary>
        /// 通过轨道获取砖机设备表
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public List<Device> GetDevices(uint trackid)
        {
            List<ConfigTileLifter> clist = new List<ConfigTileLifter>();
            List<Device> dlist = new List<Device>();
            clist = ConfigTileLifterList.FindAll(c => c.left_track_id == trackid || c.right_track_id == trackid);
            foreach (ConfigTileLifter item in clist)
            {
                dlist.Add(PubMaster.Device.GetDevice(item.id));
            }
            return dlist;
        }

        #endregion


        /// <summary>
        /// 是否允许处理转换类的指令作业
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="cmd"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool IsAllowToDo(uint devid, DevLifterCmdTypeE cmd, out string result)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                switch (cmd)
                {
                    case DevLifterCmdTypeE.转产:
                        if (dev.do_cutover)
                        {
                            result = "切换模式中，无法转产！";
                            return false;
                        }

                        if (dev.do_shift)
                        {
                            result = "正在转产中！";
                            return false;
                        }
                        break;

                    case DevLifterCmdTypeE.模式:
                        if (!dev.can_cutover)
                        {
                            result = "该砖机不具备切换功能！";
                            return false;
                        }

                        if (dev.do_shift)
                        {
                            result = "转产中，无法切换模式！";
                            return false;
                        }

                        if (dev.do_cutover)
                        {
                            result = "正在切换模式中！";
                            return false;
                        }
                        break;

                    default:
                        break;
                }
                result = "";
                return true;
            }
            result = "找不到砖机配置信息！";
            return false;
        }

        #region[砖机转产]

        /// <summary>
        /// 预设砖机品种
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="nowgoodid"></param>
        /// <param name="pregoodid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool UpdateTilePreGood(uint devid, uint nowgoodid, uint pregoodid, int count, out string result)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                if (dev.goods_id != nowgoodid)
                {
                    result = "请刷新设备信息！";
                    return false;
                }

                if (dev.do_shift)
                {
                    result = "正在转产不能修改！";
                    return false;
                }

                if (dev.do_cutover)
                {
                    result = "正在切换模式不能修改！";
                    return false;
                }

                try
                {
                    mLog.Status(true, string.Format("【预设品种】砖机[ {0} ], 预设品种[ {1} ], 标识[ {2} ]",
                        PubMaster.Device.GetDeviceName(dev.id), 
                        PubMaster.Goods.GetGoodsName(pregoodid), pregoodid));
                }
                catch { }

                dev.pre_goodid = pregoodid;
                dev.pre_good_qty = count;
                dev.pre_good_all = false;
                if (count == 0 && pregoodid != 0)
                {
                    dev.pre_good_all = true;
                }

                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Goods);
                result = "";
                return true;
            }
            result = "找不到砖机配置信息！";
            return false;
        }

        /// <summary>
        /// 开始砖机转产作业
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="nowgoodid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool UpdateShiftTileGood(uint devid, uint nowgoodid, out string result)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                if (dev.do_cutover)
                {
                    result = "切换模式中, 无法转产！";
                    return false;
                }

                if (dev.goods_id != nowgoodid)
                {
                    result = "请刷新设备信息！";
                    return false;
                }
                if (dev.do_shift)
                {
                    result = "正在转产中！";
                    return false;
                }

                dev.old_goodid = dev.goods_id;
                dev.goods_id = dev.pre_goodid;
                dev.pre_goodid = 0;

                dev.now_good_all = dev.pre_good_all;
                dev.now_good_qty = dev.pre_good_qty;

                dev.pre_good_qty = 0;
                dev.pre_good_all = false;

                dev.do_shift = true;
                dev.last_shift_time = DateTime.Now;//更新转产时间
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Goods);
                try
                {
                    mLog.Status(true, string.Format("【开始转产】砖机[ {0} ], 品种[ {1} -> {2} ], 标识[ {3} -> {4} ], 数量[ {5} ]",
                        PubMaster.Device.GetDeviceName(dev.id),
                        PubMaster.Goods.GetGoodsName(dev.old_goodid),
                        PubMaster.Goods.GetGoodsName(dev.goods_id),dev.old_goodid, dev.goods_id, (dev.now_good_all ? "不限" : (dev.now_good_qty + ""))));
                }
                catch { }
                result = "";
                return true;
            }
            result = "找不到砖机配置信息！";
            return false;
        }

        #endregion

        #region [切换模式]

        /// <summary>
        /// 开始切换
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="nextmode"></param>
        /// <param name="newgoodid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool DoCutover(uint devid, uint goodid, TileWorkModeE nextmode, uint newgoodid, out string result)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                if(dev.goods_id != goodid)
                {
                    result = "请刷新后再试！";
                    return false;
                }

                if (!dev.can_cutover)
                {
                    result = "该砖机不允许切换模式！";
                    return false;
                }

                if (dev.do_shift)
                {
                    result = "转产中, 无法切换模式！";
                    return false;
                }

                if (dev.do_cutover)
                {
                    result = "正在切换模式中！";
                    return false;
                }

                if (dev.WorkMode == nextmode)
                {
                    result = "请刷新设备信息！";
                    return false;
                }
                try
                {
                    mLog.Status(true, string.Format("【开始切换模式】砖机[ {0} ], 模式[ {1} -> {2} ], 品种[ {3} -> {4}], 标识[ {5} -> {6} ]",
                      PubMaster.Device.GetDeviceName(dev.id), dev.WorkMode, nextmode,
                      PubMaster.Goods.GetGoodsName(dev.goods_id),
                      PubMaster.Goods.GetGoodsName(newgoodid), dev.goods_id, newgoodid));
                }
                catch { }

                dev.WorkModeNext = nextmode;
                dev.pre_goodid = newgoodid;
                dev.do_cutover = true;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.WorkMode); 
                
                result = "";
                return true;
            }
            result = "找不到砖机配置信息！";
            return false;
        }

        /// <summary>
        /// 完成切换
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool FinishCutover(uint devid, TileWorkModeE nextmode)
        {
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                try
                {
                    mLog.Status(true, string.Format("【完成切换模式】砖机[ {0} ], 模式[ {1} -> {2} ], 品种[ {3} -> {4} ], 标识[ {5} -> {6} ]",
                       PubMaster.Device.GetDeviceName(dev.id), dev.WorkMode, nextmode,
                       PubMaster.Goods.GetGoodsName(dev.goods_id),
                       PubMaster.Goods.GetGoodsName(dev.pre_goodid),
                       dev.goods_id, dev.pre_goodid));
                }
                catch { }
                dev.WorkMode = nextmode;
                dev.WorkModeNext = TileWorkModeE.无;
                if (dev.pre_goodid != 0)
                {
                    dev.goods_id = dev.pre_goodid;
                    dev.pre_goodid = 0;
                }
                dev.do_cutover = false;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.WorkMode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 取消切换
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CancelCutover(uint devid, out string result)
        {
            result = "";
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == devid);
            if (dev != null)
            {
                if (!dev.do_cutover)
                {
                    result = "当前该砖机并没有切换！";
                    return false;
                }

                try
                {
                    mLog.Status(true, string.Format("【取消切换模式】砖机[ {0} ], 模式[ {1} -> {2} ], 品种[ {3} -> {4} ], 标识[ {5} -> {6} ]",
                       PubMaster.Device.GetDeviceName(dev.id), dev.WorkMode, dev.WorkModeNext,
                       PubMaster.Goods.GetGoodsName(dev.goods_id),
                       PubMaster.Goods.GetGoodsName(dev.pre_goodid),
                       dev.goods_id, dev.pre_goodid));
                }
                catch { }
                dev.WorkModeNext = TileWorkModeE.无;
                dev.pre_goodid = 0;
                dev.do_cutover = false;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.WorkMode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断库存是否被绑定
        /// </summary>
        /// <param name="carid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool IsCarrierBindStock(uint carid, uint stockid)
        {
            return ConfigCarrierList.Exists(c => c.id == carid && c.stock_id == stockid);
        }

        #endregion

        #region[设置备用砖机的信息]

        /// <summary>
        /// 将备用砖机的信息设置成需要备用的砖机的信息
        /// </summary>
        /// <param name="need_id">需要备用的砖机id</param>
        /// <param name="backup_id">备用砖机id</param>
        /// <param name="frombacktile">是否是来自备用砖机请求</param>
        /// <returns></returns>
        public bool SetBackupTileLifter(uint need_id, uint backup_id, bool frombacktile = true)
        {
            //要备用的砖机
            ConfigTileLifter need_dev = ConfigTileLifterList.Find(c => c.id == need_id);
            //备用砖机
            ConfigTileLifter backup_dev = ConfigTileLifterList.Find(c => c.id == backup_id);
            if (need_dev != null && backup_dev != null 
                && backup_dev.can_alter
                // 直接转备用    自动转备用，由普通砖机转，需要结束后才能继续转备用
                && (frombacktile || backup_dev.alter_dev_id ==0))
            {
                try
                {
                    //if (backup_dev.alter_dev_id == need_dev.id)
                    //{
                    //    return true;
                    //}

                    if (!backup_dev.IsInBackUpList(need_id))
                    {
                        mLog.Status(true, string.Format("【切换备用砖机】备用砖机[ {0} ], 没有配置需要备用的砖机[ {1} ]",
                            PubMaster.Device.GetDeviceName(backup_id), PubMaster.Device.GetDeviceName(need_id)));
                        return false;
                    }

                    mLog.Status(true, string.Format("【启用备用砖机】备用砖机[ {0} ], 需要备用的砖机[ {1} ]",
                        PubMaster.Device.GetDeviceName(backup_id), PubMaster.Device.GetDeviceName(need_id)));
                }
                catch { }

                #region[设置备用砖机的config表-品种、策略、当前工作轨道、工作模式]

                backup_dev.InStrategey = need_dev.InStrategey;
                backup_dev.OutStrategey = need_dev.OutStrategey;
                backup_dev.WorkType = need_dev.WorkType;
                backup_dev.last_track_id = need_dev.last_track_id;
                backup_dev.non_work_track_id = need_dev.non_work_track_id;
                backup_dev.old_goodid = need_dev.old_goodid;
                backup_dev.goods_id = need_dev.goods_id;
                backup_dev.pre_goodid = need_dev.pre_goodid;
                backup_dev.alter_dev_id = need_dev.id;

                if (PubMaster.Mod.DevConfigSql.EditConfigTileLifter(backup_dev))
                {
                    //修改备用机的线
                    PubMaster.Device.SetBackUpLine(backup_dev.id, need_dev.id);
                }

                #endregion

                #region[设置备用砖机的area_device_track表-备用砖机能去的轨道和摆渡车对应到达砖机轨道]

                //先将backup砖机的对应轨道删除
                PubMaster.Mod.AreaSql.DeleteAreaDeviceTrackByDevId(backup_dev.id);

                //将need砖机的对应储砖轨道 复制一份给backup砖机，注意修改信息中设备id为backup砖机的id
                PubMaster.Mod.AreaSql.CopyOtherDeviceTrackByDevId(need_id, backup_id);


                #region[设置摆渡车对应到达砖机轨道--已停用，不能修改摆渡车的配置]
                //// 找出backup砖机的左右轨道id，然后删除这些轨道id的信息
                //PubMaster.Mod.AreaSql.DeleteAreaDeviceTrackByTrackId(backup_dev.left_track_id);
                //if (backup_dev.right_track_id != 0)
                //{
                //    PubMaster.Mod.AreaSql.DeleteAreaDeviceTrackByTrackId(backup_dev.right_track_id);
                //}

                //// 找出need砖机的左右轨道id，然后将那些信息复制一份给backup砖机，注意修改信息中轨道id为backup砖机的左右轨道id
                //PubMaster.Mod.AreaSql.CopyOtherDeviceTrackByTrackId(need_dev.left_track_id, backup_dev.left_track_id);
                //if (need_dev.right_track_id != 0 && backup_dev.right_track_id != 0)
                //{
                //    PubMaster.Mod.AreaSql.CopyOtherDeviceTrackByTrackId(need_dev.right_track_id, backup_dev.right_track_id);
                //}
                //else if (backup_dev.right_track_id != 0)
                //{
                //    PubMaster.Mod.AreaSql.CopyOtherDeviceTrackByTrackId(need_dev.left_track_id, backup_dev.right_track_id);
                //}
                #endregion

                PubMaster.Area.Refresh(false, false, false, true);
                #endregion

                return true;
            }
            return false;
        }

        /// <summary>
        /// 第一种方式：备用砖机传设备号过来进行设备转换
        /// </summary>
        /// <param name="need_id">备用砖机ID</param>
        /// <param name="devcode">转产砖机设备号</param>
        public void SetBackupTileLifterCode(uint backup_id, byte devcode)
        {
            uint need_id = PubMaster.Device.GetDevIdByMemo(devcode + "");
            if(need_id != 0)
            {
                SetBackupTileLifter(need_id, backup_id);
            }
        }


        /// <summary>
        /// 备用结束,备用砖机执行转产操作
        /// </summary>
        /// <param name="backup_id">备用砖机ID</param>
        /// <param name="doshift">是否设置满砖</param>
        public bool StopBackupTileLifter(uint backup_id, bool doshift =false)
        {
            //备用砖机
            ConfigTileLifter dev = ConfigTileLifterList.Find(c => c.id == backup_id);
            if (dev != null && dev.can_alter && dev.alter_dev_id != 0)
            {
                try
                {
                    mLog.Status(true, string.Format("【备用结束】砖机[ {0} ], 备用砖机[ {1} ], 是否设满砖[ {2} ]",
                        PubMaster.Device.GetDeviceName(dev.id),
                        PubMaster.Device.GetDeviceName(dev.alter_dev_id),
                        doshift));
                }
                catch { }

                //结束备用，不能转产，设满砖
                //if (doshift)
                //{
                //    if (dev.pre_goodid == 0)
                //    {
                //        //同品种转产
                //        dev.pre_goodid = dev.goods_id;
                //    }
                //    UpdateShiftTileGood(backup_id, dev.goods_id, out string _);
                //}

                dev.alter_dev_id = 0;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev, TileConfigUpdateE.Alert_Dev_Id);

                return true;
            }

            return false;
        }

        public bool IsTileHavePreGood(uint tile_id)
        {
            return ConfigTileLifterList.Exists(c => c.id == tile_id && c.pre_goodid != 0);
        }

        public bool IsShiftInAllowTime(uint devid)
        {
            ConfigTileLifter device = GetTileLifter(devid);
            if (device != null)
            {
                return device.IsLastShiftTimeOk();
            }
            return false;
        }

        /// <summary>
        /// 设置普通砖机的当前备用砖机
        /// </summary>
        /// <param name="normaltileid"></param>
        /// <param name="backuptileid"></param>
        public void SetNormalTileBackTileId(uint normaltileid, uint backuptileid)
        {
            ConfigTileLifter device = GetTileLifter(normaltileid);
            if (device != null && device.alter_dev_id != backuptileid)
            {
                try
                {
                    if(backuptileid == 0)
                    {
                        mLog.Status(true, string.Format("【普通砖机转备用-结束更新】普通砖机[ {0} ], 备用砖机[ {1} ]",
                            PubMaster.Device.GetDeviceName(normaltileid, normaltileid + ""),
                            PubMaster.Device.GetDeviceName(backuptileid, backuptileid + "")));
                    }
                    else
                    {
                        mLog.Status(true, string.Format("【普通砖机转备用-备用开始】普通砖机[ {0} ], 备用砖机[ {1} ]",
                            PubMaster.Device.GetDeviceName(normaltileid, normaltileid + ""),
                            PubMaster.Device.GetDeviceName(backuptileid, backuptileid + "")));
                    }
                }
                catch { }
                device.alter_dev_id = backuptileid;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(device, TileConfigUpdateE.Alert_Dev_Id);
            }
        }
        #endregion

        #region[获取品种]

        /// <summary>
        /// 获取上砖机当前品种列表
        /// </summary>
        /// <returns></returns>
        public List<uint> GetUpTileGood()
        {
            return ConfigTileLifterList.FindAll(c => c.WorkMode == TileWorkModeE.上砖)?.Select(c => c.goods_id)?.ToList() ?? new List<uint>() ;
        }

        /// <summary>
        /// 获取下砖机当前预设品种列表
        /// </summary>
        /// <returns></returns>
        public List<uint> GetUpTilePreGood()
        {
            return ConfigTileLifterList.FindAll(c => c.WorkMode == TileWorkModeE.上砖)?.Select(c => c.pre_goodid)?.ToList() ?? new List<uint>();
        }

        #endregion

        /// <summary>
        /// 判断当前品种设定的上砖数量是否大于0
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public bool IsTileNowGoodQtyOk(uint tileid, uint goodid)
        {
            return ConfigTileLifterList.Exists(c => c.id == tileid && c.goods_id == goodid && (c.now_good_all || c.now_good_qty > 0));
        }

        /// <summary>
        /// 减少一车当前品种设定的上砖数量
        /// </summary>
        /// <param name="tileid"></param>
        /// <param name="goods_id"></param>
        public void SubTileNowGoodQty(uint tileid, uint goods_id)
        {
            ConfigTileLifter tile = GetTileLifter(tileid);
            if (tile != null)
            {
                if (!tile.now_good_all)
                {
                    tile.now_good_qty--;
                    if (tile.now_good_qty < 0)
                    {
                        tile.now_good_qty = 0;
                    }
                    PubMaster.Mod.DevConfigSql.EditConfigTileLifter(tile, TileConfigUpdateE.Goods);
                    AddLog(string.Format("【上砖剩余数量】砖机 [ {0} ],品种[ {1} ]，剩余上砖数量[ {2} ]", PubMaster.Device.GetDeviceName(tileid), PubMaster.Goods.GetGoodsName(goods_id), tile.now_good_qty));
                }
            }
        }

        public bool CheckBroTileLifters(uint trackid,out List<uint> devids)
        {
            devids = null;
            List<ConfigTileLifter> list =  ConfigTileLifterList.FindAll(c => c.left_track_id == trackid || c.right_track_id == trackid);
            if (list.Count > 1)
            {
                devids = list.Select(c => c.id).ToList();
                return true;
            }
            else return false;
        }
        
        #endregion

        #endregion

        #region[日志]
        public void AddLog(string msg)
        {
            try
            {
                mLog.Status(true, msg);
            }
            catch { }
        }


        public void UpdateShiftTime(uint tilelifter_id)
        {
            ConfigTileLifter tile = GetTileLifter(tilelifter_id);
            if (tile != null)
            {
                if (tile.do_shift)
                {
                    tile.last_shift_time = DateTime.Now;
                }
            }
        }

        public uint GetCarrierStockId(uint carrier_id)
        {
            return ConfigCarrierList.Find(c => c.id == carrier_id)?.stock_id ?? 0;
        }
        #endregion
    }
}
