using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.area;
using module.device;
using module.goods;
using module.line;
using module.rf;
using module.window;
using System;
using System.Collections.Generic;
using System.Linq;
using tool.mlog;

namespace resource.area
{
    public class AreaMaster
    {
        #region[构造/初始化]

        public AreaMaster()
        {
            AreaList = new List<Area>();
            AreaDevList = new List<AreaDevice>();
            AreaTraList = new List<AreaTrack>();
            AreaDevTraList = new List<AreaDeviceTrack>();
            LineList = new List<Line>();

            mLog = (Log)new LogFactory().GetLog("线路信息", false);
            mConfig = (Log)new LogFactory().GetLog("区域配置", false);

        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true, bool refr_2 = true, bool refr_3 = true, bool refr_4 = true, bool refr_5 = true)
        {
            if (refr_1)
            {
                AreaList.Clear();
                AreaList.AddRange(PubMaster.Mod.AreaSql.QueryAreaList());
            }

            if (refr_2)
            {
                AreaDevList.Clear();
                AreaDevList.AddRange(PubMaster.Mod.AreaSql.QueryAreaDeviceList());
            }

            if (refr_3)
            {
                AreaTraList.Clear();
                AreaTraList.AddRange(PubMaster.Mod.AreaSql.QueryAreaTrackList());
            }

            if (refr_4)
            {
                AreaDevTraList.Clear();
                AreaDevTraList.AddRange(PubMaster.Mod.AreaSql.QueryAreaDeviceTrackList());
            }

            if (refr_5)
            {
                LineList.Clear();
                LineList.AddRange(PubMaster.Mod.AreaSql.QueryLineList());
                LineList.Sort((x, y) => { return x.area_id == y.area_id ? x.line.CompareTo(y.line) : x.area_id.CompareTo(y.area_id); });
                foreach (Line line in LineList)
                {
                    UpdateLineSwitchWarn(line);
                }
            }
        }

        public List<Line> GetLineList()
        {
            return LineList;
        }

        public uint GetAreaId(uint id)
        {
            return AreaDevList.Find(c => c.device_id == id)?.area_id ?? 0;
        }

        public IList<MyRadioBtn> GetAreaRadioList(bool iswithall = false)
        {
            List<MyRadioBtn> myradios = new List<MyRadioBtn>();

            if (iswithall)
            {
                myradios.Add(new MyRadioBtn()
                {
                    AreaID = 0,
                    AreaName = "全部",
                    AreaTag = "0",
                });
            }

            foreach (Area area in AreaList)
            {
                myradios.Add(new MyRadioBtn()
                {
                    AreaID = area.id,
                    AreaName = area.name,
                    AreaTag = area.id + ""
                }); ;
            }

            if (myradios.Count >= 2)
            {
                myradios[0].BorderCorner = new System.Windows.CornerRadius(5, 0, 0, 5);
                myradios[myradios.Count - 1].BorderCorner = new System.Windows.CornerRadius(0, 5, 5, 0);
            }

            return myradios;
        }

        /// <summary>
        /// 使用线路信息构造区域过滤信息
        /// </summary>
        /// <param name="iswithall"></param>
        /// <returns></returns>
        public IList<MyRadioBtn> GetAreaLineRadioList(bool iswithall = false)
        {
            List<MyRadioBtn> myradios = new List<MyRadioBtn>();

            if (iswithall)
            {
                myradios.Add(new MyRadioBtn()
                {
                    AreaID = 0,
                    AreaName = "全部",
                    AreaTag = "0",
                    Line = 0
                });
            }

            foreach (Line line in LineList)
            {
                myradios.Add(new MyRadioBtn()
                {
                    AreaID = line.area_id,
                    AreaName = line.name,
                    AreaTag = line.area_id + "",
                    Line = line.line
                }); ;
            }

            if (myradios.Count >= 2)
            {
                myradios[0].BorderCorner = new System.Windows.CornerRadius(5, 0, 0, 5);
                myradios[myradios.Count - 1].BorderCorner = new System.Windows.CornerRadius(0, 5, 5, 0);
            }

            return myradios;
        }


        internal List<uint> GetAreaTileIds(uint areaid)
        {
            return AreaDevList.FindAll(c => c.area_id == areaid
                                && (c.DevType == DeviceTypeE.上砖机 || c.DevType == DeviceTypeE.下砖机 || c.DevType == DeviceTypeE.砖机))
                                    .Select(c => c.device_id).ToList();
        }

        internal List<uint> GetAreaFerryIds(uint areaid)
        {
            return AreaDevList.FindAll(c => c.area_id == areaid
                                && (c.DevType == DeviceTypeE.前摆渡 || c.DevType == DeviceTypeE.后摆渡))
                                    .Select(c => c.device_id).ToList();
        }

        public bool IsDeviceInArea(uint filterareaid, uint iD)
        {
            return AreaDevList.Exists(c => c.area_id == filterareaid && c.device_id == iD);
        }

        public void Stop()
        {

        }
        #endregion

        #region[字段]

        private List<Area> AreaList { set; get; }
        private List<AreaDevice> AreaDevList { set; get; }
        private List<AreaTrack> AreaTraList { set; get; }
        private List<AreaDeviceTrack> AreaDevTraList { set; get; }
        private List<Line> LineList { set; get; }

        private Log mLog, mConfig;
        #endregion

        #region[获取对象]

        public List<Area> GetAreaList()
        {
            return AreaList;
        }

        public List<Area> GetAreaList(List<uint> ids)
        {
            return AreaList.FindAll(c => ids.Contains(c.id));
        }

        public bool HaveArea(int id)
        {
            return AreaList.Exists(c => c.id == id);
        }

        public Area GetArea(uint devid)
        {
            return AreaList.Find(c => c.id == devid);
        }

        public List<AreaDevice> GetAreaDevList()
        {
            return AreaDevList;
        }

        public List<AreaDevice> GetAreaDevList(uint area_id)
        {
            return AreaDevList.FindAll(c => c.area_id == area_id);
        }

        public List<AreaDevice> GetAreaDevList(uint area_id, DeviceTypeE devtype)
        {
            return AreaDevList.FindAll(c => c.area_id == area_id && c.DevType == devtype);
        }

        public List<AreaDevice> GetAreaDevListWithType(DeviceTypeE devtype)
        {
            return AreaDevList.FindAll(c => c.DevType == devtype);
        }

        public List<AreaDeviceTrack> GetDevTrackList(uint devid)
        {
            return AreaDevTraList.FindAll(c => c.device_id == devid);
        }

        public List<AreaDeviceTrack> GetAreaDevTraList(uint areaid, uint devid)
        {
            List<AreaDeviceTrack> list = AreaDevTraList.FindAll(c => c.area_id == areaid && c.device_id == devid);
            if (list.Count > 0)
            {
                list.Sort((x, y) => x.prior.CompareTo(y.prior));
            }
            return list;
        }

        /// <summary>
        /// 获取砖机作业对应储砖轨道
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="devid">设备ID</param>
        /// <param name="isup">是否上砖作业</param>
        /// <returns></returns>
        public List<AreaDeviceTrack> GetTileWorkTraList(uint areaid, uint devid, bool isup)
        {
            List<AreaDeviceTrack> list = new List<AreaDeviceTrack>();
            if (isup)
            {
                list = AreaDevTraList.FindAll(c => c.area_id == areaid && c.device_id == devid && c.can_up);
            }
            else
            {
                list = AreaDevTraList.FindAll(c => c.area_id == areaid && c.device_id == devid && c.can_down);
            }

            if (list.Count > 0)
            {
                list.Sort((x, y) => x.prior.CompareTo(y.prior));
            }
            return list;
        }

        public bool ExistAreaDevTrack(uint areaid, uint devid)
        {
            return AreaDevTraList.Exists(c => c.area_id == areaid && c.device_id == devid);
        }

        #endregion

        #region[获取/判断属性]
        /// <summary>
        /// 根据设备id查找对应的轨道id
        /// </summary>
        /// <param name="tileid"></param>
        /// <returns></returns>
        public List<uint> GetAreaDevTrackWithTrackIds(uint tileid)
        {
            List<uint> trackids = null;
            List<AreaDeviceTrack> areaDeviceTracks = GetDevTrackList(tileid);
            if (areaDeviceTracks != null && areaDeviceTracks.Count != 0)
            {
                trackids = areaDeviceTracks.Select(c => c.track_id).ToList();
            }
            return trackids;

        }

        public string GetAreaName(uint Area_id)
        {
            return AreaList.Find(c => c.id == Area_id)?.name ?? "";
        }

        public string GetName(uint id)
        {
            return AreaList.Find(c => c.id == id)?.name ?? id + "";
        }

        /// <summary>
        /// 倒库专用：找到上砖摆渡轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public List<uint> GetFerryTrackSortIds(StockTrans trans, bool istrackid)
        {
            List<uint> list = new List<uint>();
            List<AreaDevice> ferrys = AreaDevList.FindAll(c => c.area_id == trans.area_id && c.DevType == DeviceTypeE.前摆渡);

            if (ferrys != null && ferrys.Count > 0)
            {
                foreach (AreaDevice device in ferrys)
                {
                    if (AreaDevTraList.Exists(c => c.device_id == device.device_id
                                                && c.area_id == device.area_id
                                                && c.track_id == trans.give_track_id))
                    {
                        if (istrackid)
                        {
                            uint trackid = PubMaster.DevConfig.GetFerryTrackId(device.device_id);
                            if (trackid > 0)
                                list.Add(trackid);
                        }
                        else
                        {
                            list.Add(device.device_id);
                        }
                    }
                }
            }

            return list;
        }

        public CarrierTypeE GetCarrierType(uint area_id)
        {
            return AreaList.Find(c => c.id == area_id)?.CarrierType ?? CarrierTypeE.窄车;
        }

        /// <summary>
        /// 摆渡车是否允许定位轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="ferryid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsFerryWithTrack(uint areaid, uint ferryid, uint trackid)
        {
            return AreaDevTraList.Exists(c => c.area_id == areaid && c.device_id == ferryid && c.track_id == trackid);
        }

        /// <summary>
        /// 获取能够同时到达指定轨道的摆渡车ID
        /// </summary>
        /// <param name="ferrytype">摆渡车类型</param>
        /// <param name="traids">轨道可变列表（多个轨道号）</param>
        /// <returns></returns>
        public List<uint> GetWithTracksFerryIds(DeviceTypeE ferrytype, params uint[] traids)
        {
            List<uint> ferryids = PubMaster.Device.GetDevIds(ferrytype);
            return GetWithTracksFerryIds(ferryids, traids);
        }

        /// <summary>
        /// 获取能够同时到达指定轨道的摆渡车ID
        /// </summary>
        /// <param name="traids">轨道可变列表（多个轨道号）</param>
        /// <returns></returns>
        public List<uint> GetWithTracksFerryIds(params uint[] traids)
        {
            return GetWithTracksFerryIds(null, traids);
        }

        /// <summary>
        /// 获取能够同时到达指定轨道的摆渡车ID
        /// </summary>
        /// <param name="ferryids">需要判断的摆渡车</param>
        /// <param name="traids">轨道可变列表（多个轨道号）</param>
        /// <returns></returns>
        public List<uint> GetWithTracksFerryIds(List<uint> ferryids, params uint[] traids)
        {
            List<uint> filterferryids = new List<uint>();

            if (ferryids == null)
            {
                ferryids = PubMaster.Device.GetDevIds(DeviceTypeE.前摆渡, DeviceTypeE.后摆渡);
            }
            //查找能满砖所有轨道的摆渡车
            bool havealltrack;
            foreach (var ferryid in ferryids)
            {
                havealltrack = true;
                foreach (uint traid in traids)
                {
                    if (!AreaDevTraList.Exists(c => c.track_id == traid && ferryid == c.device_id))
                    {
                        havealltrack = false;
                    }
                }

                if (havealltrack)
                {
                    filterferryids.Add(ferryid);
                }
            }
            return filterferryids;
        }

        /// <summary>
        /// 查看是否摆渡车分配的轨道里面是否配置了另外一个轨道
        /// </summary>
        /// <param name="ferryids">已经配置了A轨道的摆渡车信息</param>
        /// <param name="trackid">需要检查是否配了的B轨道</param>
        /// <returns></returns>
        public bool ExistFerryWithTrack(List<uint> ferryids, uint trackid)
        {
            return AreaDevTraList.Exists(c => ferryids.Contains(c.device_id) && c.track_id == trackid);
        }

        /// <summary>
        /// 查看是否摆渡车分配的轨道里面是否配置了另外一个轨道
        /// </summary>
        /// <param name="ferryid">已经配置了A轨道的摆渡车信息</param>
        /// <param name="trackid">需要检查是否配了的B轨道</param>
        /// <returns></returns>
        public bool ExistFerryWithTrack(uint ferryid, uint trackid)
        {
            return AreaDevTraList.Exists(c => ferryid == c.device_id && c.track_id == trackid);
        }

        /// <summary>
        /// 判断是否存在摆渡车同时满砖配置的轨道信息
        /// </summary>
        /// <param name="trackids"></param>
        /// <returns></returns>
        public bool ExistFerryWithTracks(DeviceTypeE type, params uint[] trackids)
        {
            List<uint> ferryids = new List<uint>();
            if (type == DeviceTypeE.其他)
            {
                ferryids = PubMaster.Device.GetDevIds(DeviceTypeE.前摆渡, DeviceTypeE.后摆渡);
            }
            else
            {
                ferryids = PubMaster.Device.GetDevIds(type);
            }

            foreach (var ferryid in ferryids)
            {
                // 获取摆渡所有轨道
                List<AreaDeviceTrack>  ferryTracks = AreaDevTraList.FindAll(c => c.device_id == ferryid);
                bool isAll = true;
                foreach (var trackid in trackids)
                {
                    if(!ferryTracks.Exists(c => c.track_id == trackid))
                    {
                        isAll = false;
                        break;
                    }
                }

                if (isAll) return true;
            }

            return false;
        }

        /// <summary>
        /// 获取摆渡车ID或摆渡车轨道的ID
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="istrackid"></param>
        /// <returns></returns>
        public List<uint> GetFerryWithTrackInOut(DeviceTypeE ferrytype, uint areaid, uint taketrackid, uint givetrackid, uint cartrackid, bool istrackid)
        {
            List<uint> list = new List<uint>();
            List<AreaDevice> ferrys = AreaDevList.FindAll(c => c.area_id == areaid && c.DevType == ferrytype);

            if (ferrys != null && ferrys.Count > 0)
            {
                bool takematch, givematch, carmatch;
                foreach (AreaDevice device in ferrys)
                {
                    takematch = taketrackid == 0 || AreaDevTraList.Exists(c => c.device_id == device.device_id
                                                  && c.area_id == device.area_id
                                                  && c.track_id == taketrackid);
                    givematch = givetrackid == 0 || AreaDevTraList.Exists(c => c.device_id == device.device_id
                                                && c.area_id == device.area_id
                                                && c.track_id == givetrackid);
                    carmatch = cartrackid == 0 || AreaDevTraList.Exists(c => c.device_id == device.device_id
                                                && c.area_id == device.area_id
                                                && c.track_id == cartrackid);

                    if (takematch && givematch && carmatch)
                    {
                        if (istrackid)
                        {
                            uint trackid = PubMaster.DevConfig.GetFerryTrackId(device.device_id);
                            if (trackid > 0)
                                list.Add(trackid);
                        }
                        else
                        {
                            list.Add(device.device_id);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 保存区域对应的设备配置信息到数据库
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="devid"></param>
        public void SaveToDb(uint areaid, Device dev)
        {
            foreach (AreaDeviceTrack item in AreaDevTraList.FindAll(c => c.area_id == areaid && c.device_id == dev.id))
            {
                // 更新轨道取放方向
                switch (dev.Type)
                {
                    case DeviceTypeE.上砖机:
                        PubMaster.Track.UpdateDirByUp(PubMaster.DevConfig.GetLeftTrackId(dev.id), item.track_id);
                        break;

                    case DeviceTypeE.下砖机:
                        PubMaster.Track.UpdateDirByDown(PubMaster.DevConfig.GetLeftTrackId(dev.id), item.track_id);
                        break;
                }

                // 保存到数据库
                PubMaster.Mod.AreaSql.EditAreaDeviceTrack(item);
            }
        }

        /// <summary>
        /// 添加区域砖机轨道信息
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="toareaid"></param>
        /// <param name="tile"></param>
        public void AddAreaTileTrackList(uint areaid, uint toareaid, Device tile)
        {
            TrackTypeE tracktype = TrackTypeE.储砖_入;

            if (tile.Type == DeviceTypeE.上砖机)
            {
                tracktype = TrackTypeE.储砖_出;
            }
            List<uint> trackids = PubMaster.Track.GetAreaTrackIds(areaid, tracktype);
            trackids.AddRange(PubMaster.Track.GetAreaTrackIds(areaid, TrackTypeE.储砖_出入));
            ushort prior = (ushort)AreaDevTraList.FindAll(c => c.area_id == toareaid && c.device_id == tile.id).Max(c => c.prior);
            prior++;
            AreaDeviceTrack areatradev;

            uint maxid = PubMaster.Mod.AreaSql.GetAreaDevTraMaxId();
            foreach (uint trackid in trackids)
            {
                if (AreaDevTraList.Exists(c => c.area_id == toareaid && c.device_id == tile.id && c.track_id == trackid))
                {
                    continue;
                }

                areatradev = new AreaDeviceTrack()
                {
                    id = ++maxid,
                    area_id = toareaid,
                    device_id = tile.id,
                    track_id = trackid,
                    prior = prior,
                };
                PubMaster.Mod.AreaSql.AddAreaDeviceTrack(areatradev);
                prior += 1;
            }

            Refresh(false, false, false, true, false);
        }

        /// <summary>
        /// 删除区域设备配置轨道信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteAreaDevTrack(uint id)
        {
            AreaDeviceTrack track = AreaDevTraList.Find(c => c.id == id);
            if (track != null)
            {
                AreaDevTraList.Remove(track);
                PubMaster.Mod.AreaSql.DeleteAreaDeviceTrack(track);

                mConfig.Status(true, string.Format("[删除轨道], 设备[ {0} ], 轨道[ {1} ], 优先级[ {2} ]", PubMaster.Device.GetDeviceName(track.device_id),
                    PubMaster.Track.GetTrackName(track.track_id), track.prior));

                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加摆渡车配置的轨道信息
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="toareaid"></param>
        /// <param name="ferry"></param>
        public void AddAreaFerryTrackList(uint areaid, uint toareaid, Device ferry)
        {
            TrackTypeE tracktype = TrackTypeE.储砖_入;
            TrackTypeE tiletrtype = TrackTypeE.下砖轨道;

            if (ferry.Type == DeviceTypeE.前摆渡)
            {
                tracktype = TrackTypeE.储砖_出;
                tiletrtype = TrackTypeE.上砖轨道;
            }
            List<uint> trackids = PubMaster.Track.GetAreaTrackIds(areaid, tracktype);
            trackids.AddRange(PubMaster.Track.GetAreaTrackIds(areaid, TrackTypeE.储砖_出入));
            trackids.AddRange(PubMaster.Track.GetAreaTrackIds(areaid, tiletrtype));
            ushort prior = (ushort)AreaDevTraList.FindAll(c => c.area_id == toareaid && c.device_id == ferry.id).Max(c => (uint?)c.prior).GetValueOrDefault(0);
            prior++;
            AreaDeviceTrack areatradev;

            uint maxid = PubMaster.Mod.AreaSql.GetAreaDevTraMaxId();
            foreach (uint trackid in trackids)
            {
                if (AreaDevTraList.Exists(c => c.area_id == toareaid && c.device_id == ferry.id && c.track_id == trackid))
                {
                    continue;
                }

                areatradev = new AreaDeviceTrack()
                {
                    id = ++maxid,
                    area_id = toareaid,
                    device_id = ferry.id,
                    track_id = trackid,
                    prior = prior,
                };

                PubMaster.Mod.AreaSql.AddAreaDeviceTrack(areatradev);
                prior += 1;
            }

            Refresh(false, false, false, true, false);
        }

        /// <summary>
        /// 获取区域轨道信息
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public List<AreaTrack> GetAreaTracks(uint areaid)
        {
            List<AreaTrack> list = AreaTraList.FindAll(c => c.area_id == areaid);
            if (list.Count > 0)
            {
                list.Sort((x, y) => x.id.CompareTo(y.id));
            }
            return list;
        }

        public List<AreaTrack> GetAreaTracks(uint areaid, TrackTypeE type)
        {
            List<AreaTrack> list = AreaTraList.FindAll(c => c.area_id == areaid && c.TrackType == type);
            return list;
        }

        public bool IsInDevTrack(uint trackid, uint toareaid, uint devid)
        {
            return AreaDevTraList.Exists(c => c.area_id == toareaid && c.device_id == devid && c.track_id == trackid);
        }

        public void AddAreaTrack(uint trackid, uint toareaid, Device dev)
        {
            ushort prior = 0;
            try
            {
                prior = (ushort)AreaDevTraList.FindAll(c => c.area_id == toareaid && c.device_id == dev.id).Max(c => c.prior);
            }
            catch
            {
                prior++;
            }

            if (AreaDevTraList.Exists(c => c.area_id == toareaid && c.device_id == dev.id && c.track_id == trackid))
            {
                return;
            }

            uint maxid = PubMaster.Mod.AreaSql.GetAreaDevTraMaxId();
            AreaDeviceTrack areatradev = new AreaDeviceTrack()
            {
                id = ++maxid,
                area_id = toareaid,
                device_id = dev.id,
                track_id = trackid,
                prior = prior,
            };

            switch (dev.Type)
            {
                case DeviceTypeE.上砖机:
                    areatradev.can_up = true;
                    PubMaster.Track.UpdateDirByUp(PubMaster.DevConfig.GetLeftTrackId(dev.id), trackid);
                    break;
                case DeviceTypeE.下砖机:
                    areatradev.can_down = true;
                    PubMaster.Track.UpdateDirByDown(PubMaster.DevConfig.GetLeftTrackId(dev.id), trackid);
                    break;
                case DeviceTypeE.砖机:
                    areatradev.can_up = true;
                    areatradev.can_down = true;
                    break;
                default:
                    break;
            }

            PubMaster.Mod.AreaSql.AddAreaDeviceTrack(areatradev);
            Refresh(false, false, false, true, false);

            mConfig.Status(true, string.Format("[添加轨道], 设备[ {0} ], 轨道[ {1} ], 优先级[ {2} ]", dev.name, PubMaster.Track.GetTrackName(trackid), prior - 1));
        }

        public bool IsFerrySetTrack(uint ferryid, uint trackid)
        {
            return AreaDevTraList.Exists(c => c.device_id == ferryid && c.track_id == trackid);
        }

        /// <summary>
        /// 获取下砖机所在的区域ID
        /// </summary>
        /// <param name="tileid"></param>
        /// <returns></returns>
        public uint GetAreaDevAreaId(uint tileid)
        {
            return AreaDevList.Find(c => c.device_id == tileid)?.area_id ?? 0;
        }

        /// <summary>
        /// 根据线路信息ID获取线路
        /// </summary>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public Line GetLineById(ushort lineid)
        {
            return LineList.Find(c => c.id == lineid);
        }

        public Line GetLine(uint areaid, ushort line)
        {
            return LineList.Find(c => c.area_id == areaid && c.line == line);
        }

        public string GetLineName(uint areaid, ushort line)
        {
            Line l = LineList.Find(c => c.area_id == areaid && c.line == line);
            if (l != null)
            {
                return l.name;
            }
            return "";
        }

        /// <summary>
        /// 获取区域线路类型
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public LineTypeE GetLineType(uint areaid, ushort line)
        {
            return LineList.Find(c => c.area_id == areaid && c.line == line)?.LineType ?? LineTypeE.窑后;
        }

        public bool IsSortTaskLimit(uint area, ushort line, int count)
        {
            if (line == 0)
            {
                return AreaList.Exists(c => c.id == area && count >= c.c_sorttask);
            }

            return LineList.Exists(c => c.area_id == area && c.line == line && count >= c.sort_task_qty);
        }

        public bool IsUpTaskLimit(uint area, ushort line, int count)
        {
            if (line == 0)
            {
                return AreaList.Exists(c => c.id == area && count >= c.c_sorttask);
            }

            return LineList.Exists(c => c.area_id == area && c.line == line && c.up_task_qty > 0 && count >= c.up_task_qty);
        }

        public bool IsDownTaskLimit(uint area, ushort line, int count)
        {
            if (line == 0)
            {
                return AreaList.Exists(c => c.id == area && count >= c.c_sorttask);
            }

            return LineList.Exists(c => c.area_id == area && c.line == line && c.down_task_qty > 0 && count >= c.down_task_qty);
        }

        /// <summary>
        /// 判断入库轨道库存是否已到上限
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool IsDownStockFullLimit(uint area, ushort line, uint count)
        {
            if (line == 0)
            {
                return AreaList.Exists(c => c.id == area && count >= c.full_qty);
            }

            return LineList.Exists(c => c.area_id == area && c.line == line && c.full_qty > 0 && count >= c.full_qty);
        }

        public ushort GetAreaFullQty(uint id)
        {
            return AreaList.Find(c => c.id == id)?.full_qty ?? 0;
        }

        /// <summary>
        /// 获取对应砖机的指定轨道的优先级
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetAreaDevTrackPrior(uint devid, uint trackid)
        {
            return AreaDevTraList.Find(c => c.device_id == devid && c.track_id == trackid)?.prior ?? 0;
        }

        /// <summary>
        /// 获取区域的上砖侧设定的运输车数量
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public uint GetAreaUpCarCount(uint area)
        {
            return AreaList.Find(c => c.id == area)?.up_car_count ?? 0;
        }

        /// <summary>
        /// 获取区域的下砖侧设定的运输车数量
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public uint GetAreaDownCarCount(uint area)
        {
            return AreaList.Find(c => c.id == area)?.down_car_count ?? 0;
        }

        /// <summary>
        /// 判断轨道是否存在于同类型设备的配置中
        /// </summary>
        /// <param name="area"></param>
        /// <param name="dt"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsTrackInTiles(uint area, DeviceTypeE dt, uint trackid)
        {
            List<uint> tileids = PubMaster.Device.GetDevIds(area, dt);
            return AreaDevTraList.Exists(c => tileids.Contains(c.device_id) && c.track_id == trackid);
        }

        #endregion

        #region[更改]

        #endregion

        #region[单一区域]

        public bool IsSingleArea(out uint areaid)
        {
            if (AreaList.Count == 1)
            {
                areaid = AreaList[0].id;
                return true;
            }
            areaid = 0;
            return false;
        }

        public bool IsSingleAreaLine(out uint areaid, out ushort lineid)
        {
            if (LineList.Count == 1)
            {
                areaid = LineList[0].area_id;
                lineid = LineList[0].line;
                return true;
            }
            areaid = 0;
            lineid = 0;
            return false;
        }


        #endregion

        #region[获取最大值]

        private uint _areadevnextid = 1;
        private uint GetAreaDevMaxId()
        {
            if (_areadevnextid == 1)
            {
                _areadevnextid = AreaDevList.Max(c => c.id);
            }
            return ++_areadevnextid;
        }

        #endregion

        #region[区域设备添加/修改/删除]

        public bool AddAreaDevice(uint areaid, uint devid, out string result)
        {
            if (AreaDevList.Exists(c => c.area_id == areaid && c.device_id == devid))
            {
                result = "区域里面已经存在该设备！";
                return false;
            }

            Device device = PubMaster.Device.GetDevice(devid);
            if (device != null)
            {
                AreaDevice areadev = new AreaDevice();
                areadev.id = GetAreaDevMaxId();
                areadev.area_id = areaid;
                areadev.device_id = device.id;
                areadev.DevType = device.Type;
                bool isadd = PubMaster.Mod.AreaSql.AddAreaDevice(areadev);
                if (isadd)
                {
                    AreaDevList.Add(areadev);
                    result = "";
                    return true;
                }
                else
                {
                    Refresh(false, true, false, false, false);
                    result = "请刷新数据!再试！";
                    return false;
                }
            }

            result = "找不到设备信息";
            return false;
        }

        public bool DeleteAreaDevice(uint areadevid)
        {
            AreaDevice areadev = AreaDevList.Find(c => c.id == areadevid);
            if (areadev != null)
            {
                bool isdelete = PubMaster.Mod.AreaSql.DeleteAreaDevice(areadev);
                AreaDevList.Remove(areadev);
            }
            return true;
        }
        #endregion

        #region[线管理]

        /// <summary>
        /// 获取线配置的区域线路接力倒库最大倒库数量
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public byte GetLineUpSortMaxNumber(uint areaid, ushort line)
        {
            return LineList.Find(c => c.area_id == areaid && c.line == line)?.max_upsort_num ?? 0;
        }


        /// <summary>
        /// 更新线路倒库每次发送数量
        /// </summary>
        /// <param name="filterareaid"></param>
        /// <param name="filterlineid"></param>
        /// <param name="set_Each_Sort_Qty"></param>
        public void SetLineEachSortQty(uint filterareaid, ushort filterlineid, byte eachsortqty)
        {
            Line line = GetLine(filterareaid, filterlineid);
            if (line != null)
            {
                line.max_upsort_num = eachsortqty;
                PubMaster.Mod.AreaSql.EditAreaLine(line);
            }
        }

        private void GetTaskSwitch(Line line, ref List<TaskSwitch> list)
        {
            list.Add(new TaskSwitch()
            {
                id = line.id * 1000 + 1,
                code = line.id + ":" + ((byte)OnOffTaskE.上砖),
                name = line.name + "上砖开关",
                onoff = line.onoff_up
            });

            list.Add(new TaskSwitch()
            {
                id = line.id * 1000 + 2,
                code = line.id + ":" + ((byte)OnOffTaskE.下砖),
                name = line.name + "下砖开关",
                onoff = line.onoff_down
            });

            if (line.LineType == LineTypeE.窑后)
            {
                list.Add(new TaskSwitch()
                {
                    id = line.id * 1000 + 3,
                    code = line.id + ":" + ((byte)OnOffTaskE.倒库),
                    name = line.name + "倒库开关",
                    onoff = line.onoff_sort
                });
            }
        }

        public List<TaskSwitch> GetAreaLineSwitch()
        {
            List<TaskSwitch> list = new List<TaskSwitch>();
            foreach (Line line in LineList)
            {
                GetTaskSwitch(line, ref list);
            }
            return list;
        }

        public List<TaskSwitch> GetAreaLineSwitch(List<uint> areaids)
        {
            List<TaskSwitch> list = new List<TaskSwitch>();
            foreach (Line line in LineList.FindAll(c => areaids.Contains(c.area_id)))
            {
                GetTaskSwitch(line, ref list);
            }
            return list;
        }

        public bool UpdateLineSwitch(ushort lineid, OnOffTaskE onofftype, bool onoff, string memo, out string result, bool isfromrf = false)
        {
            Line line = GetLineById(lineid);
            if (line != null)
            {
                result = "开关没有改变";
                switch (onofftype)
                {
                    case OnOffTaskE.上砖:
                        if (line.onoff_up == onoff) return false;
                        line.onoff_up = onoff;
                        break;
                    case OnOffTaskE.下砖:
                        if (line.onoff_down == onoff) return false;
                        line.onoff_down = onoff;
                        break;
                    case OnOffTaskE.倒库:
                        if (line.onoff_sort == onoff) return false;
                        line.onoff_sort = onoff;
                        break;
                }

                PubMaster.Mod.AreaSql.EditAreaLineOnoff(line);
                Refresh(false, false, false, false, true);//从数据库刷新开关状态

                if (isfromrf)
                {
                    Messenger.Default.Send(line, MsgToken.LineSwitchUpdate);
                }
                mLog.Status(true, string.Format("[任务开关], 线路[ {0} ], 类型[ {1} ], 开关[ {2} ], 备注[ {3} ]", line.name, onofftype, onoff, memo));
                return true;
            }

            result = "找不到开关信息";
            return false;
        }


        /// <summary>
        /// 判断区域线路倒库开关
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsLineSortOnoff(uint area, ushort line)
        {
            return GetLine(area, line)?.onoff_sort ?? false;
        }


        /// <summary>
        /// 判断区域线路下砖开关
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsLineDownOnoff(uint area, ushort line)
        {
            return GetLine(area, line)?.onoff_down ?? false;
        }

        /// <summary>
        /// 判断区域线路上砖开关
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsLineUpOnoff(uint area, ushort line)
        {
            return GetLine(area, line)?.onoff_up ?? false;
        }

        public void UpdateLineSwitchWarn(Line line)
        {
            if (line != null)
            {
                //下砖开关报警/消除
                if (!line.onoff_down)
                {
                    PubMaster.Warn.AddLineWarn(WarningTypeE.DownTaskSwitchClosed, (ushort)line.area_id, line.line);
                }
                else
                {
                    PubMaster.Warn.RemoveLineWarn(WarningTypeE.DownTaskSwitchClosed, (ushort)line.area_id, line.line);
                }

                //上砖开关报警/消除
                if (!line.onoff_up)
                {
                    PubMaster.Warn.AddLineWarn(WarningTypeE.UpTaskSwitchClosed, (ushort)line.area_id, line.line);
                }
                else
                {
                    PubMaster.Warn.RemoveLineWarn(WarningTypeE.UpTaskSwitchClosed, (ushort)line.area_id, line.line);
                }

                //倒库开关报警/消除
                if (!line.onoff_sort && line.LineType == LineTypeE.窑后)
                {
                    PubMaster.Warn.AddLineWarn(WarningTypeE.SortTaskSwitchClosed, (ushort)line.area_id, line.line);
                }
                else
                {
                    PubMaster.Warn.RemoveLineWarn(WarningTypeE.SortTaskSwitchClosed, (ushort)line.area_id, line.line);
                }
            }
        }
        #endregion
    }
}