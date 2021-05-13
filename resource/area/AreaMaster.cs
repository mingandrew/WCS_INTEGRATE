using enums;
using enums.track;
using module.area;
using module.device;
using module.goods;
using module.line;
using module.window;
using System;
using System.Collections.Generic;
using System.Linq;

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
            }
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
                    AreaTag = area.id+""
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
                                    .Select(c=>c.device_id).ToList();
        }

        internal List<uint> GetAreaFerryIds(uint areaid)
        {
            return AreaDevList.FindAll(c => c.area_id == areaid 
                                && (c.DevType == DeviceTypeE.上摆渡 || c.DevType == DeviceTypeE.下摆渡))
                                    .Select(c=>c.device_id).ToList();
        }

        public bool IsFerryInArea(uint filterareaid, uint iD)
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
        #endregion

        #region[获取对象]

        public List<Area> GetAreaList()
        {
            return AreaList;
        }

        public List<Area> GetAreaList(List<uint> ids)
        {
            return AreaList.FindAll(c=>ids.Contains(c.id));
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
            return AreaList.Find(c => c.id == id)?.name ?? id+"";
        }

        /// <summary>
        /// 倒库专用：找到上砖摆渡轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public List<uint> GetFerryTrackSortIds(StockTrans trans, bool istrackid)
        {
            List<uint> list = new List<uint>();
            List<AreaDevice> ferrys = AreaDevList.FindAll(c => c.area_id == trans.area_id && c.DevType == DeviceTypeE.上摆渡);

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
                ferryids = PubMaster.Device.GetDevIds(DeviceTypeE.上摆渡, DeviceTypeE.下摆渡);
            }
            //查找能满砖所有轨道的摆渡车
            bool havealltrack;
            foreach (var ferryid in ferryids)
            {
                havealltrack = true;
                foreach (uint traid in traids)
                {
                    if(!AreaDevTraList.Exists(c => c.track_id == traid && ferryid == c.device_id))
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
                bool takematch, givematch,carmatch;
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
                            if(trackid > 0)
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
        public void SaveToDb(uint areaid, uint devid)
        {
            foreach (AreaDeviceTrack item in AreaDevTraList.FindAll(c => c.area_id == areaid && c.device_id == devid))
            {
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
            ushort prior = (ushort)AreaDevTraList.Count(c => c.area_id == toareaid && c.device_id == tile.id);
            prior++;
            AreaDeviceTrack areatradev;
            foreach (uint trackid in trackids)
            {
                if (AreaDevTraList.Exists(c => c.area_id == toareaid && c.device_id == tile.id && c.track_id == trackid))
                {
                    continue;
                }

                areatradev = new AreaDeviceTrack()
                {
                    area_id = toareaid,
                    device_id = tile.id,
                    track_id = trackid,
                    prior = prior,
                };
                PubMaster.Mod.AreaSql.AddAreaDeviceTrack(areatradev);
                prior += 1;
            }

            Refresh(false, false, false, true);
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

            if (ferry.Type == DeviceTypeE.上摆渡)
            {
                tracktype = TrackTypeE.储砖_出; 
                tiletrtype = TrackTypeE.上砖轨道;
            }
            List<uint> trackids = PubMaster.Track.GetAreaTrackIds(areaid, tracktype);
            trackids.AddRange(PubMaster.Track.GetAreaTrackIds(areaid, TrackTypeE.储砖_出入));
            trackids.AddRange(PubMaster.Track.GetAreaTrackIds(areaid, tiletrtype));
            ushort prior = (ushort)AreaDevTraList.Count(c => c.area_id == toareaid && c.device_id == ferry.id);
            prior++;
            AreaDeviceTrack areatradev;
            foreach (uint trackid in trackids)
            {
                if (AreaDevTraList.Exists(c=>c.area_id == toareaid && c.device_id == ferry.id && c.track_id == trackid))
                {
                    continue;
                }

                areatradev = new AreaDeviceTrack()
                {
                    area_id = toareaid,
                    device_id = ferry.id,
                    track_id = trackid,
                    prior = prior,
                };
                PubMaster.Mod.AreaSql.AddAreaDeviceTrack(areatradev);
                prior += 1;
            }

            Refresh(false, false, false, true);
        }

        /// <summary>
        /// 添加区域轨道信息
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
            ushort prior = (ushort)AreaDevTraList.Count(c => c.area_id == toareaid && c.device_id == dev.id);
            prior++;

            if (AreaDevTraList.Exists(c => c.area_id == toareaid && c.device_id == dev.id && c.track_id == trackid))
            {
                return;
            }

            AreaDeviceTrack areatradev = new AreaDeviceTrack()
            {
                area_id = toareaid,
                device_id = dev.id,
                track_id = trackid,
                prior = prior,
            };

            PubMaster.Mod.AreaSql.AddAreaDeviceTrack(areatradev);
            prior += 1;

            Refresh(false, false, false, true);
        }

        public bool IsFerrySetTrack(uint ferryid, uint trackid)
        {
            return AreaDevTraList.Exists(c => c.device_id == ferryid && c.track_id == trackid);
        }

        public List<uint> GetTileTrackIds(StockTrans trans)
        {
            List<AreaDeviceTrack> list = GetAreaDevTraList(trans.area_id, trans.tilelifter_id);
            return list?.Select(c => c.track_id).ToList() ?? new List<uint>();
        }

        public List<AreaTrack> GetAreaTrackIds(uint areaid)
        {
            return AreaTraList.FindAll(c => c.area_id == areaid);
        }

        /// <summary>
        /// 获取区域内可用轨道（已含出入轨道）
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="typeE"></param>
        /// <returns></returns>
        public List<uint> GetAreaTrackIds(uint areaid, TrackTypeE typeE)
        {
            List<AreaTrack> areaTracks = AreaTraList.FindAll(c => c.area_id == areaid && (c.TrackType == typeE || c.TrackType == TrackTypeE.储砖_出入));
            return areaTracks.Select(c => c.track_id).ToList();
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

        public Line GetLine(uint areaid, ushort line)
        {
            return LineList.Find(c => c.area_id == areaid && c.line == line);
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

        public ushort GetAreaFullQty(uint id)
        {
            return AreaList.Find(c => c.id == id)?.full_qty ?? 0;
        }

        #endregion

        #region[更改]

        #endregion

        #region[单一区域]

        public bool IsSingleArea(out uint areaid)
        {
            if(AreaList.Count == 1)
            {
                areaid = AreaList[0].id;
                return true;
            }
            areaid = 0;
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
                    Refresh(false, true, false, false);
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

        #endregion
    }
}
