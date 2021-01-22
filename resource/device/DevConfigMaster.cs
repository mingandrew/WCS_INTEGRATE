using enums;
using module.deviceconfig;
using System.Collections.Generic;
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
            return ConfigCarrierList.Find(c => c.id == devid)?.length ?? 0;
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
        /// 设置砖机作业品种
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
                    mLog.Status(true, string.Format("[品种修改]砖机：{0}，原：{1}，新：{2}【{3} , {4}】",
                        PubMaster.Device.GetDeviceName(dev.id),
                        PubMaster.Goods.GetGoodsName(dev.goods_id),
                        PubMaster.Goods.GetGoodsName(goodid), dev.goods_id, goodid));
                }
                catch { }
                dev.goods_id = goodid;
                PubMaster.Mod.DevConfigSql.EditGoods(dev);
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
                    mLog.Status(true, string.Format("[入库逻辑]砖机：{0}，原:{1}，新:{2}", 
                        PubMaster.Device.GetDeviceName(dev.id),
                        dev.InStrategey, instrategy));
                }
                catch { }
                dev.InStrategey = instrategy;
                dev.WorkType = worktype;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev);
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
                    mLog.Status(true, string.Format("[出库逻辑]砖机：{0}，原:{1}，新:{2}",
                        PubMaster.Device.GetDeviceName(dev.id),
                        dev.OutStrategey, outstrategy));
                }
                catch { }
                dev.OutStrategey = outstrategy;
                dev.WorkType = worktype;
                PubMaster.Mod.DevConfigSql.EditConfigTileLifter(dev);
                return true;
            }
            return false;
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
                    mLog.Status(true, string.Format("[最后作业轨道]砖机：{0}，轨道:{1}",
                        PubMaster.Device.GetDeviceName(dev.id), trackid));
                }
                catch { }
                dev.last_track_id = trackid;
                PubMaster.Mod.DevConfigSql.EditLastTrackId(dev);
                return true;
            }
            return false;
        }


        #region [串联砖机]

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
        /// 获取内侧砖机的ID
        /// </summary>
        /// <param name="tilelifter_id"></param>
        /// <returns></returns>
        public uint GetBrotherId(uint tilelifter_id)
        {
            return ConfigTileLifterList.Find(c => c.brother_dev_id == tilelifter_id).id;
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
        public bool UpdateTilePreGood(uint devid, uint nowgoodid, uint pregoodid, out string result)
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
                    mLog.Status(true, string.Format("[预设品种]砖机：{0}，预设品种:{1}【{2}】",
                        PubMaster.Device.GetDeviceName(dev.id), 
                        PubMaster.Goods.GetGoodsName(pregoodid), pregoodid));
                }
                catch { }
                dev.pre_goodid = pregoodid;
                PubMaster.Mod.DevConfigSql.EditGoods(dev);
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
                    result = "切换模式中，无法转产！";
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
                dev.do_shift = true;
                PubMaster.Mod.DevConfigSql.EditGoods(dev);
                try
                {
                    mLog.Status(true, string.Format("[开始转产]砖机：{0}，原品种:{1}，新品种:{2}, 【{3} , {4}】",
                        PubMaster.Device.GetDeviceName(dev.id),
                        PubMaster.Goods.GetGoodsName(dev.old_goodid),
                        PubMaster.Goods.GetGoodsName(dev.goods_id),dev.old_goodid, dev.goods_id));
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
                    result = "转产中，无法切换模式！";
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
                    mLog.Status(true, string.Format("[开始切换模式]砖机：{0}，当前模式:{1}，切换模式:{2}",
                       PubMaster.Device.GetDeviceName(dev.id), dev.WorkMode, nextmode));
                }
                catch { }

                dev.WorkModeNext = nextmode;
                dev.pre_goodid = newgoodid;
                dev.do_cutover = true;
                PubMaster.Mod.DevConfigSql.EditWorkMode(dev); 
                
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
                    mLog.Status(true, string.Format("[完成切换模式]砖机：{0}，原模式:{1}，新模式:{2}",
                       PubMaster.Device.GetDeviceName(dev.id), dev.WorkMode, nextmode));
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
                PubMaster.Mod.DevConfigSql.EditWorkMode(dev);
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #endregion

    }
}
