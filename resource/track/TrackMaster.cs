using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.area;
using module.device;
using module.goods;
using module.msg;
using module.rf;
using module.track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using tool.appconfig;
using tool.mlog;

namespace resource.track
{
    public class TrackMaster
    {

        #region[字段]
        private readonly object _obj;
        #endregion


        #region[构造/初始化]

        private Log mLog;

        public TrackMaster()
        {
            mLog = (Log)new LogFactory().GetLog("工位日志", false);
            TrackList = new List<Track>();
            CarrierPosList = new List<CarrierPos>();
            _obj = new object();

            TrackSortFrontCount = GlobalWcsDataConfig.BigConifg.TrackSortFrontCount;
            TrackSortMidCount = GlobalWcsDataConfig.BigConifg.TrackSortMidCount;
            TrackSortBackCount = GlobalWcsDataConfig.BigConifg.TrackSortBackCount;
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true, bool refr_2 = true, bool refr_3 = true)
        {
            if (refr_1)
            {
                TrackList.Clear();
                TrackList.AddRange(PubMaster.Mod.TraSql.QueryTrackList());
                if (TrackList != null && TrackList.Count != 0)
                {
                    foreach (Track item in TrackList)
                    {
                        item.GetAllRFID();
                    }
                }
            }

            if (refr_2)
            {
                List<Track> list = TrackList.FindAll(c => c.InType(TrackTypeE.储砖_出) && c.up_split_point > 0);
                foreach (Track track in list)
                {
                    GetAndRefreshUpCount(track.id);
                }
            }

            if (refr_3)
            {
                CarrierPosList.Clear();
                CarrierPosList.AddRange(PubMaster.Mod.TraSql.QueryCarrierPosList());
            }
        }


        public object GetTrackNameByCode(ushort trackcode)
        {
            return TrackList.Find(c => c.IsInTrack(trackcode))?.name ?? "";
        }

        public void Stop()
        {
        }
        #endregion

        #region[字段]

        private List<Track> TrackList { set; get; }

        private List<CarrierPos> CarrierPosList { set; get; }
        #endregion

        #region[获取对象]

        public List<Track> GetTrackList()
        {
            return TrackList;
        }

        public List<Track> GetTrackList(List<uint> areaids)
        {
            return TrackList.FindAll(c => areaids.Contains(c.area));
        }

        public List<Track> GetTrackList(List<TrackTypeE> types, List<uint> areaids)
        {
            return TrackList.FindAll(c => types.Contains(c.Type) && areaids.Contains(c.area));
        }

        public List<Track> GetTrackList(List<TrackTypeE> types)
        {
            return TrackList.FindAll(c => types.Contains(c.Type));
        }

        public List<Track> GetTrackList(uint areaid)
        {
            return TrackList.FindAll(c => c.area == areaid);
        }

        public Track GetTrack(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid);
        }

        /// <summary>
        /// 获取轨道位置相对顺序
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public short GetTrackOrder(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.order ?? 0;
        }

        public Track GetTrackBySite(ushort area, ushort trackrfid)
        {
            return TrackList.Find(c => c.area == area && c.IsInTrack(trackrfid));
        }

        public Track GetTrackBySite(ushort area, List<TrackTypeE> types, ushort trackrfid)
        {
            return TrackList.Find(c => c.area == area && types.Contains(c.Type) && c.IsInTrack(trackrfid));
        }

        public TrackTypeE GetTrackType(ushort trackrfid)
        {
            return TrackList.Find(c => c.IsInTrack(trackrfid)).Type;
        }

        /// <summary>
        /// 获取轨道类型
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public TrackTypeE GetTrackType(uint track_id)
        {
            return TrackList.Find(c => c.id == track_id).Type;
        }

        /// <summary>
        /// 获取轨道出入库类型
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public TrackType2E GetTrackType2(uint track_id)
        {
            return TrackList.Find(c => c.id == track_id).Type2;
        }

        /// <summary>
        /// 获取轨道出入库类型(byte)
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public byte GetTrackType2ForByte(uint track_id)
        {
            return TrackList.Find(c => c.id == track_id)?.type2 ?? 0;
        }

        public List<Track> GetTracksInTypes(uint areaid, List<TrackTypeE> types)
        {
            return TrackList.FindAll(c => c.area == areaid && types.Contains(c.Type));
        }

        public List<Track> GetTracksInTypes(List<TrackTypeE> types)
        {
            return TrackList.FindAll(c => types.Contains(c.Type));
        }

        public List<Track> GetAreaTracks(uint areaid)
        {
            return TrackList.FindAll(c => c.area == areaid);
        }

        /// <summary>
        /// 获取区域指定类型的单笔轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Track GetAreaTrack(uint areaid, ushort lineid, TrackTypeE type)
        {
            return TrackList.Find(c => c.area == areaid && c.line == lineid && c.Type == type);
        }

        /// <summary>
        /// 获取区域线所有的指定类型轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="line"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<uint> GetAreaLineTracks(uint areaid, uint lineid, TrackTypeE type)
        {
            return TrackList.FindAll(c => c.area == areaid && c.line == lineid && c.Type == type)?.Select(c => c.id).ToList() ?? new List<uint>();
        }

        /// <summary>
        /// 获取同区域内所有轨道
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<uint> GetAreaAllTrackIdList(uint area)
        {
            return TrackList.FindAll(c => c.area == area)?.Select(c => c.id).ToList() ?? new List<uint>();
        }

        /// <summary>
        /// 获取区域指定类型的轨道ID
        /// </summary>
        /// <param name="area"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<uint> GetAreaTrackIdList(uint area, params TrackTypeE[] types)
        {
            return TrackList.FindAll(c => c.area == area && c.InType(types))?.Select(c => c.id).ToList() ?? new List<uint>();
        }

        /// <summary>
        /// 获取区域线路轨道指定类型的轨道的ID
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="line">线路ID</param>
        /// <param name="types">轨道类型</param>
        /// <returns></returns>
        public List<uint> GetAreaSortOutTrack(uint areaid, uint line, params TrackTypeE[] types)
        {
            return TrackList.FindAll(c => c.area == areaid
                                    && c.line == line
                                    && c.InType(types))
                ?.Select(c => c.id).ToList() ?? new List<uint>();
        }

        /// <summary>
        /// 查找区域、线路、砖机配置的轨道
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="line">线路ID</param>
        /// <param name="tileid">砖机ID</param>
        /// <param name="types">轨道类型/param>
        /// <returns></returns>
        public List<uint> GetAreaLineAndTileTrack(uint areaid, uint line, uint tileid, bool isup, params TrackTypeE[] types)
        {
            List<uint> trackid = new List<uint>();
            //查找砖机配置的轨道
            if (tileid != 0)
            {
                //List<AreaDeviceTrack> list = PubMaster.Area.GetAreaDevTraList(areaid, tileid);
                List<AreaDeviceTrack> list = PubMaster.Area.GetTileWorkTraList(areaid, tileid, isup);
                List<uint> tiletraids = list?.Select(c => c.track_id).ToList() ?? new List<uint>();
                trackid.AddRange(tiletraids);
            }

            //查找区域线路的所有轨道
            foreach (var item in TrackList.FindAll(c => c.area == areaid && c.line == line && c.InType(types)))
            {
                if (!trackid.Contains(item.id))
                {
                    trackid.Add(item.id);
                }
            }

            return trackid;
        }

        /// <summary>
        /// 根据区域，线路获取指定轨道列表
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public List<Track> GetSortTrackList(uint area_id, ushort line, params TrackTypeE[] types)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == area_id 
                                                    && c.line == line 
                                                    && c.TrackStatus == TrackStatusE.启用
                                                    && c.sort_able 
                                                    && c.InType(types));

            tracks?.Sort((x, y) => x.sort_level.CompareTo(y.sort_level));

            return tracks;
        }
        #endregion

        #region[获取属性]

        /// <summary>
        /// 获取轨道线路信息
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public ushort GetTrackLine(uint track_id)
        {
            return GetTrack(track_id)?.line ?? 0;
        }

        public string GetTrackName(uint trackid, string defaultstr = "")
        {
            return TrackList.Find(c => c.id == trackid)?.name ?? defaultstr;
        }

        public uint GetTrackId(ushort area, ushort site)
        {
            if (site == 0) return 0;
            return GetTrackBySite(area, site)?.id ?? 0;
        }

        /// <summary>
        /// 获取设备分配的轨道ID
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="area"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public uint GetTrackId(uint devid, ushort area, ushort site)
        {
            if (site == 0) return 0;
            List<AreaDeviceTrack> list = PubMaster.Area.GetDevTrackList(devid);
            return TrackList.Find(c => c.area == area
                                    && c.IsInTrack(site)
                                    && list.Exists(d => d.track_id == c.id))?.id ?? 0;
        }

        /// <summary>
        /// 获取区域轨道
        /// </summary>
        /// <param name="devid">设备ID</param>
        /// <param name="area">区域ID</param>
        /// <param name="site">工位</param>
        /// <returns></returns>
        public uint GetAreaTrack(uint devid, ushort area, DeviceTypeE type, ushort site)
        {
            if (site == 0) return 0;
            if (type == DeviceTypeE.前摆渡)
            {
                return TrackList.Find(c => c.area == area
                                        && c.InType(TrackTypeE.上砖轨道, TrackTypeE.储砖_出, TrackTypeE.储砖_出入)
                                        && c.IsInTrack(site))?.id ?? 0;
            }
            return TrackList.Find(c => c.area == area
                                    && c.InType(TrackTypeE.下砖轨道, TrackTypeE.储砖_入, TrackTypeE.储砖_出入)
                                    && c.IsInTrack(site))?.id ?? 0;
        }

        /// <summary>
        /// 获取轨道名称
        /// </summary>
        /// <param name="devid">设备ID</param>
        /// <param name="site">轨道号</param>
        /// <returns></returns>
        public string GetTrackName(uint devid, ushort site)
        {
            if (site == 0) return "";
            uint areaid = PubMaster.Device.GetDeviceArea(devid);
            DeviceTypeE type = PubMaster.Device.GetDeviceType(devid);
            switch (type)
            {
                case DeviceTypeE.上砖机:
                case DeviceTypeE.前摆渡:
                    return TrackList.Find(c => c.area == areaid && c.IsUpAreaTrack() && c.IsInTrack(site))?.name ?? site + "";
                case DeviceTypeE.下砖机:
                case DeviceTypeE.后摆渡:
                    return TrackList.Find(c => c.area == areaid && c.IsDownAreaTrack() && c.IsInTrack(site))?.name ?? site + "";
                case DeviceTypeE.砖机:
                case DeviceTypeE.其他:
                case DeviceTypeE.运输车:
                default:
                    return TrackList.Find(c => c.area == areaid && c.IsInTrack(site))?.name ?? site + "";
            }
        }

        /// <summary>
        /// 获取轨道前进起点（默认入库端口）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackUpCode(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.ferry_up_code ?? 0;
        }

        /// <summary>
        /// 获取轨道后退起点（默认出库端口）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackDownCode(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.ferry_down_code ?? 0;
        }

        /// <summary>
        /// 判断是否有轨道配置了该地标
        /// </summary>
        /// <param name="trackSite">轨道地标</param>
        /// <returns></returns>
        public bool ExistSiteInTrack(ushort area, ushort trackSite)
        {
            return TrackList.Exists(c => c.area == area && c.IsInTrack(trackSite));
        }

        /// <summary>
        /// 获取小车位置对应轨道ID
        /// </summary>
        /// <param name="currentsite"></param>
        /// <param name="currentpoint"></param>
        /// <returns></returns>
        public uint GetTrackIdForCarrier(ushort area, ushort currentsite, ushort currentpoint)
        {
            uint traid = 0;
            Track t = GetTrackBySite(area, currentsite);
            if (t != null)
            {
                switch (t.Type)
                {
                    case TrackTypeE.储砖_入: // 读到入轨道地标，但是大于分段点距离，当做出轨道
                        if (currentpoint != 0
                            && currentpoint >= t.split_point
                            && IsBiggerSplitPoint(t.brother_track_id, (ushort)(currentpoint + 20))
                            )
                        {
                            traid = t.brother_track_id;
                        }
                        else
                        {
                            traid = t.id;
                        }
                        break;
                    case TrackTypeE.储砖_出:// 读到出轨道地标，但是小于分段点距离，当做入轨道
                        if (currentpoint != 0
                            && currentpoint <= t.split_point - 20
                            //&& IsSmallerSplitPoint(t.brother_track_id, (ushort)(site - 20))
                            )
                        {
                            traid = t.brother_track_id;
                        }
                        else
                        {
                            traid = t.id;
                        }
                        break;
                    default:
                        traid = t.id;
                        break;
                }
            }
            return traid;
        }

        /// <summary>
        /// 判断脉冲是否大于轨道的分割脉冲点
        /// </summary>
        /// <param name="track_id"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private bool IsBiggerSplitPoint(uint track_id, ushort site)
        {
            return site >= (GetTrack(track_id)?.split_point ?? 0);
        }

        /// <summary>
        /// 判断脉冲是否小于轨道的分割脉冲点
        /// </summary>
        /// <param name="track_id"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private bool IsSmallerSplitPoint(uint track_id, ushort site)
        {
            return site <= (GetTrack(track_id)?.split_point ?? 0);
        }

        /// <summary>
        /// 获取轨道RFID1（定位-默认最小地标）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackRFID1(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.rfid_1 ?? 0;
        }

        /// <summary>
        /// 获取轨道RFID2（定位-默认最大地标）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackRFID2(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.rfid_2 ?? 0;
        }

        /// <summary>
        /// 获取轨道RFID3（定位-反抛取砖地标）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackRFID3(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.rfid_3 ?? 0;
        }

        /// <summary>
        /// 获取轨道分割点脉冲
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackSplitPoint(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.split_point ?? 0;
        }

        /// <summary>
        /// 获取轨道下砖极限点脉冲（后侧）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackLimitPointIn(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.limit_point ?? 0;
        }

        /// <summary>
        /// 获取轨道上砖极限点脉冲（前侧）
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetTrackLimitPointOut(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.limit_point_up ?? 0;
        }

        /// <summary>
        /// 获取轨道的兄弟轨道ID
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public uint GetBrotherTrackId(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.brother_track_id ?? 0;
        }

        /// <summary>
        /// 判断轨道是否是储砖轨道
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsStoreTrack(uint track_id)
        {
            return TrackList.Exists(c => c.id == track_id
                                        && c.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入));
        }

        /// <summary>
        /// 判断轨道是放砖轨道
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsStoreGiveTrack(uint track_id)
        {
            return TrackList.Exists(c => c.id == track_id && c.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入));
        }

        /// <summary>
        /// 判断轨道是否同侧上下
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        internal bool IsSameSideTrack(uint track_id)
        {
            return TrackList.Exists(c => c.id == track_id && c.same_side_inout);
        }

        /// <summary>
        /// 判断轨道是否 后退存砖
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public bool IsGiveBackTrack(uint track_id)
        {
            return TrackList.Find(c => c.id == track_id)?.is_give_back ?? false;
        }

        /// <summary>
        /// 判断轨道是否 前进取砖
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public bool IsTakeForwardTrack(uint track_id)
        {
            return TrackList.Find(c => c.id == track_id)?.is_take_forward ?? false;
        }

        /// <summary>
        /// 获取摆渡车分配的轨道
        /// </summary>
        /// <param name="ferryid"></param>
        /// <returns></returns>
        public List<Track> GetFerryTracks(uint ferryid)
        {
            List<AreaDeviceTrack> deviceTrack = PubMaster.Area.GetDevTrackList(ferryid);
            return TrackList.FindAll(c => deviceTrack.Exists(d => d.track_id == c.id));
        }

        /// <summary>
        /// 获取轨道最大存砖数量
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal int GetTrackMaxStore(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid)?.max_store ?? 0;
        }

        /// <summary>
        /// 根据order的差值查找对应的轨道id
        /// </summary>
        /// <param name="track_id"></param>
        /// <param name="difference"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public uint GetTrackIdByDifference(uint track_id, int difference, bool isAdd = true)
        {
            Track t = TrackList.Find(c => c.id == track_id);
            int newOrder = 0;
            if (isAdd)
            {
                newOrder = t.order + difference;
            }
            else
            {
                newOrder = (t.order - difference) < 1 ? 1 : (t.order - difference);
            }
            // && c.area == t.area
            return TrackList.Find(c => c.order == newOrder
                        && c.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入))?.id ?? 0;
        }

        /// <summary>
        /// 将轨道按照order进行排序
        /// </summary>
        /// <param name="trackids">需要排序的轨道ID列表</param>
        /// <param name="tid">排序参照轨道</param>
        /// <param name="order">排序参照轨道order</param>
        /// <returns>返回一个以参考轨道为起点轨道id列表【越靠近参考轨道越前的】</returns>
        public List<uint> SortTrackIdsWithOrder(List<uint> trackids, uint tid, short order, bool checkType = true)
        {
            List<Track> tracks = TrackList.FindAll(c => c.id != tid && trackids.Contains(c.id));
            if (tracks.Count > 0)
            {
                // 按位置相对顺序排序
                tracks.Sort((x, y) =>
                {
                    int xorder = Math.Abs(x.order - order);
                    int yorder = Math.Abs(y.order - order);
                    return xorder.CompareTo(yorder);
                });

                // 再按轨道类型排序 ？
                if (checkType)
                {
                    TrackTypeE tt = GetTrackType(tid);
                    TrackType2E tt2 = GetTrackType2(tid);
                    // -1: x在前； 0:同序； 1: y在前；
                    tracks.Sort((x, y) =>
                    {
                        if (x.InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道))
                        {
                            if (y.InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道))
                            {
                                return 0;
                            }
                            return -1;
                        }

                        if (x.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                        {
                            if (y.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                            {
                                return 0;
                            }
                            return 1;
                        }

                        if (x.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入))
                        {
                            if (y.InType(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道))
                            {
                                return 1;
                            }

                            if (y.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道))
                            {
                                return -1;
                            }

                            if (x.Type == tt && y.Type != tt)
                            {
                                return -1;
                            }

                            if (x.Type != tt && y.Type == tt)
                            {
                                return 1;
                            }

                            if (tt == TrackTypeE.储砖_出入 && x.Type == tt && y.Type == tt)
                            {
                                switch (tt2)
                                {
                                    case TrackType2E.通用:
                                        if (x.Type2 == tt2 && y.Type2 != tt2)
                                        {
                                            return -1;
                                        }

                                        if (x.Type2 != tt2 && y.Type2 == tt2)
                                        {
                                            return 1;
                                        }
                                        break;

                                    case TrackType2E.入库:
                                        if (x.IsWorkIn() && !y.IsWorkIn())
                                        {
                                            return -1;
                                        }

                                        if (!x.IsWorkIn() && y.IsWorkIn())
                                        {
                                            return 1;
                                        }
                                        break;

                                    case TrackType2E.出库:
                                        if (x.IsWorkOut() && !y.IsWorkOut())
                                        {
                                            return -1;
                                        }

                                        if (!x.IsWorkOut() && y.IsWorkOut())
                                        {
                                            return 1;
                                        }
                                        break;
                                }
                            }

                        }

                        return 0;
                    });
                }

            }
            return tracks.Select(c => c.id).ToList();
        }

        /// <summary>
        /// 根据order查找对应的储砖轨道ID
        /// </summary>
        /// <returns></returns>
        public uint GetTrackIDByOrder(ushort area, DeviceTypeE type, int order)
        {
            List<Track> tracks;
            if (type == DeviceTypeE.前摆渡)
            {
                tracks = TrackList.FindAll(c => c.area == area && c.order == order
                           && c.InType(TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.上砖轨道));

            }
            else
            {
                tracks = TrackList.FindAll(c => c.area == area && c.order == order
                            && c.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入, TrackTypeE.下砖轨道));
            }

            if (tracks != null && tracks.Count > 1)
            {
                tracks.Sort((x, y) => y.type.CompareTo(x.type));
            }

            if (tracks != null && tracks.Count >= 1)
            {
                return tracks[0].id;
            }

            return 0;
        }

        /// <summary>
        /// 获取最小储砖轨道order
        /// </summary>
        /// <returns></returns>
        public int GetMinOrder(ushort area, DeviceTypeE dt)
        {
            switch (dt)
            {
                case DeviceTypeE.前摆渡:
                    return TrackList.FindAll(c => c.area == area
                            && c.InType(TrackTypeE.储砖_出入, TrackTypeE.上砖轨道, TrackTypeE.储砖_出)).Min(c => c.order);

                case DeviceTypeE.后摆渡:
                    return TrackList.FindAll(c => c.area == area
                            && c.InType(TrackTypeE.储砖_出入, TrackTypeE.下砖轨道, TrackTypeE.储砖_入)).Min(c => c.order);

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取最大储砖轨道order
        /// </summary>
        /// <returns></returns>
        public int GetMaxOrder(ushort area, DeviceTypeE dt)
        {
            switch (dt)
            {
                case DeviceTypeE.前摆渡:
                    return TrackList.FindAll(c => c.area == area
                            && c.InType(TrackTypeE.储砖_出入, TrackTypeE.上砖轨道, TrackTypeE.储砖_出)).Max(c => c.order);

                case DeviceTypeE.后摆渡:
                    return TrackList.FindAll(c => c.area == area
                            && c.InType(TrackTypeE.储砖_出入, TrackTypeE.下砖轨道, TrackTypeE.储砖_入)).Max(c => c.order);

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 根据摆渡车对位地标查找轨道信息
        /// </summary>
        /// <param name="poscode"></param>
        /// <returns></returns>
        private Track GetTrackByFerryCocde(ushort area, uint devid, ushort poscode)
        {
            List<AreaDeviceTrack> list = PubMaster.Area.GetDevTrackList(devid);
            return TrackList.Find(c => c.area == area && (c.ferry_down_code == poscode || c.ferry_up_code == poscode)
                                    && list.Exists(d => d.track_id == c.id));
        }

        /// <summary>
        /// 获取区域任意摆渡车定位地标
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ushort> GetFerryTrackCode(uint areaid, TrackTypeE type)
        {
            return TrackList.FindAll(c => c.area == areaid && c.Type == type)
                ?.Select(c => c.rfid_1).ToList() ?? new List<ushort>();
        }

        /// <summary>
        /// 判断是否存在该类型的轨道
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="types">轨道类型</param>
        /// <returns></returns>
        public bool ExistTrackInType(uint areaid, ushort lineid, params TrackTypeE[] types)
        {
            return TrackList.Exists(c => c.area == areaid && c.line == lineid && types.Contains(c.Type));
        }

        /// <summary>
        /// 判断指定的轨道列表是否存在指定的状态的轨道
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="statusEs"></param>
        /// <returns></returns>
        public bool ExistTracksStatus(List<uint> tracks, params TrackStatusE[] statusEs)
        {
            return TrackList.Exists(c => tracks.Contains(c.id) && c.InStatus(statusEs));
        }


        /// <summary>
        /// 获取出入库轨道的中间ID
        /// </summary>
        /// <returns></returns>
        public uint GetMidId(uint areaid, ushort lineid)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == areaid && c.line == lineid && c.InType(TrackTypeE.储砖_出入));
            if (tracks.Count > 0)
            {
                tracks.Sort((x, y) => x.id.CompareTo(y.id));
                int idx = (tracks.Count / 2) - 1;
                if (idx < tracks.Count)
                {
                    Track track = tracks[idx];
                    return track?.id ?? 10000;
                }
            }
            return 10000;
        }
        #endregion

        #region[更改]

        public bool SetTrackStatus(uint trackid, TrackStatusE trackstatus, out string result, string memo = "")
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track == null)
            {
                result = "找不到轨道的信息";
                return false;
            }

            if (track.TrackStatus == trackstatus)
            {
                result = "不用修改";
                return false;
            }

            if (track.NotInType(TrackTypeE.储砖_出入) && (trackstatus == TrackStatusE.仅上砖 || trackstatus == TrackStatusE.仅下砖))
            {
                result = "储砖_出入轨道才能修改！";
                return false;
            }

            //if (track.TrackStatus == TrackStatusE.倒库中)
            //{
            //    result = "倒库中不能修改";
            //    return false;
            //}
            UpdateTrackStatus(track, trackstatus, memo);

            result = "";
            return true;
        }

        public List<Track> GetTileTrack(uint tileid)
        {
            List<AreaDeviceTrack> areadevtras = PubMaster.Area.GetDevTrackList(tileid);
            return TrackList.FindAll(c => areadevtras.Exists(a => a.track_id == c.id));
        }

        public List<Track> GetTileTrack(uint areaid, uint tileid)
        {
            List<AreaDeviceTrack> areadevtras = PubMaster.Area.GetAreaDevTraList(areaid, tileid);
            return TrackList.FindAll(c => areadevtras.Exists(a => a.track_id == c.id));
        }

        public List<Track> GetFerryTracksInType(uint ferryid, TrackTypeE type)
        {
            List<AreaDeviceTrack> deviceTrack = PubMaster.Area.GetDevTrackList(ferryid);
            return TrackList.FindAll(c => c.Type == type && deviceTrack.Exists(d => d.track_id == c.id));
        }

        /// <summary>
        /// 更新轨道库存状态
        /// </summary>
        /// <param name="trackid">轨道ID</param>
        /// <param name="goodstatus">轨道库存状态</param>
        /// <param name="result">需改结果</param>
        /// <param name="memo">备注</param>
        /// <returns></returns>
        public bool SetStockStatus(uint trackid, TrackStockStatusE goodstatus, out string result, string memo = "")
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track == null)
            {
                result = "找不到轨道的信息";
                return false;
            }

            if (track.StockStatus == goodstatus)
            {
                result = "不用修改";
                return false;
            }

            if (goodstatus == TrackStockStatusE.空砖 && PubMaster.Goods.ExistStockInTrack(trackid))
            {
                result = "轨道有库存记录";
                return false;
            }

            if (goodstatus == TrackStockStatusE.满砖 && !PubMaster.Goods.ExistStockInTrack(trackid))
            {
                result = "轨道没有库存记录";
                return false;
            }

            UpdateStockStatus(track, goodstatus, memo, true);

            result = "";
            return true;
        }

        public void ResetTrackRecent(uint id)
        {
            Track track = TrackList.Find(c => c.id == id);
            if (track != null)
            {
                track.recent_goodid = 0;
                track.recent_tileid = 0;
            }
        }

        public void SetSortTrackStatus(uint taketrackid, uint givetrackid, TrackStatusE fromstatus, TrackStatusE tostatus)
        {
            Track taketrack = TrackList.Find(c => c.id == taketrackid);
            Track givetrack = TrackList.Find(c => c.id == givetrackid);

            if (taketrack != null && taketrack.TrackStatus == fromstatus)
            {
                UpdateTrackStatus(taketrack, tostatus, "倒库");
            }

            if (givetrack != null && givetrack.TrackStatus == fromstatus)
            {
                UpdateTrackStatus(givetrack, tostatus, "倒库");
            }
        }

        public void SetUpSortStatus(uint taketrackid, uint givetrackid, TrackStatusE fromstatus, TrackStatusE tostatus)
        {
            Track taketrack = TrackList.Find(c => c.id == taketrackid);
            Track givetrack = TrackList.Find(c => c.id == givetrackid);

            if (taketrack != null && taketrack.TrackStatus == fromstatus)
            {
                UpdateTrackStatus(taketrack, tostatus, "倒库");
            }
        }


        /// <summary>
        /// 倒库交换库存信息
        /// </summary>
        /// <param name="taketrackid">满砖轨道</param>
        /// <param name="givetrackid">空砖轨道</param>
        internal void ShiftTrack(uint taketrackid, uint givetrackid)
        {
            if (PubMaster.Goods.IsTrackStockEmpty(taketrackid))
            {
                UpdateStockStatus(taketrackid, TrackStockStatusE.空砖, "倒库");
            }

            if (PubMaster.Goods.ExistStockInTrack(givetrackid))
            {
                UpdateStockStatus(givetrackid, TrackStockStatusE.满砖, "倒库");
            }
        }

        public void UpdateStockStatus(uint trackid, TrackStockStatusE status, string memo)
        {
            Track track = TrackList.Find(c => c.id == trackid);
            UpdateStockStatus(track, status, memo, false);
        }

        /// <summary>
        /// 计算摆渡车自动对位，选中轨道后 剩余对位数据
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int GetFerryAutoPosLen(ushort area, uint devid, ushort poscode)
        {
            Track selectrack = GetTrackByFerryCocde(area, devid, poscode);
            if (selectrack != null)
            {
                List<Track> tracks = GetFerryTracksInType(devid, selectrack.Type);
                tracks.Sort((x, y) => x.rfid_1.CompareTo(y.rfid_1));
                return tracks.Count - tracks.IndexOf(selectrack);
            }
            return 1;
        }

        /// <summary>
        /// 修改轨道库存状态并更新到数据库
        /// </summary>
        /// <param name="track"></param>
        /// <param name="status"></param>
        /// <param name="memo"></param>
        /// <param name="isAllow"></param>
        internal void UpdateStockStatus(Track track, TrackStockStatusE status, string memo, bool isAllow)
        {
            if (track != null)
            {
                if (track.StockStatus == status) return;

                if (!isAllow && track.InType(TrackTypeE.储砖_出入) && track.StockStatus == TrackStockStatusE.满砖 && status == TrackStockStatusE.有砖)
                {
                    return;
                }

                #region 入库侧轨道满砖-下砖机标记不作业轨道
                if (track.InType(TrackTypeE.储砖_入) && status == TrackStockStatusE.满砖)
                {
                    PubMaster.DevConfig.SetDownTileNonWorkTrack(track.id);
                }
                #endregion

                try
                {
                    mLog.Status(true, string.Format("轨道[ {0} ], 货物[ {1} -> {2} ], 备注[ {3} ]", track.name, track.StockStatus, status, memo));
                }
                catch { }
                track.StockStatus = status;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.StockStatus);
                if (status == TrackStockStatusE.有砖 && track.early_full)
                {
                    SetTrackEaryFull(track.id, false, null);
                }
                else
                {
                    SendMsg(track);
                }
            }
        }

        internal void UpdateTrackStatus(Track track, TrackStatusE trackstatus, string memo)
        {
            if (track != null && track.TrackStatus != trackstatus)
            {
                mLog.Status(true, string.Format("轨道[ {0} ], 状态[ {1} -> {2} ], 备注[ {3} ]", track.name, track.TrackStatus, trackstatus, memo));
                track.TrackStatus = trackstatus;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.TrackStatus);
                if (trackstatus == TrackStatusE.停用 && track.early_full)
                {
                    //轨道提前满砖，停用轨道 -> 清除轨道提前满砖状态
                    SetTrackEaryFull(track.id, false, null);
                }
                SendMsg(track);
            }
        }

        public void UpdateRecentTile(uint trackid, uint recenttile)
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track != null && track.recent_tileid != recenttile)
            {
                track.recent_tileid = recenttile;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.RecentTileId);
            }
        }

        public void UpdateRecentGood(uint trackid, uint goodid)
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track != null && track.recent_goodid != goodid)
            {
                track.recent_goodid = goodid;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.RecentGoodId);
            }
        }


        public void UpdateUpCount(uint trackid, int count)
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track != null)
            {
                track.upcount = count;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.UpCount);
            }
        }


        /// <summary>
        /// 空砖/满砖/一般的取货卸货
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="signalTime"></param>
        /// <param name="takeTrackCode"></param>
        /// <param name="giveTrackCode"></param>
        /// <returns>处理结果</returns>
        public bool SetTrack(DevCarrierSignalE signal, ushort signalTime, ushort takeTrackCode, ushort giveTrackCode)
        {
            bool result = false;
            switch (signal)
            {
                #region[空砖信息]
                //小车取卸货后: 取砖轨道 -> 空
                case DevCarrierSignalE.空轨道:

                    break;
                #endregion
                case DevCarrierSignalE.满轨道:

                    break;
                case DevCarrierSignalE.非空非满:
                    break;
            }

            return result;
        }

        /// <summary>
        /// 空砖信息
        /// </summary>
        /// <param name="taketrackcode"></param>
        /// <returns></returns>
        public bool SetTrackEmtpy(ushort area, ushort taketrackcode)
        {
            Track track = GetTrackBySite(area, taketrackcode);
            if (track != null)
            {
                if (track.StockStatus != TrackStockStatusE.空砖)
                {
                    track.StockStatus = TrackStockStatusE.空砖;
                    PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.StockStatus);
                    PubMaster.Goods.RemoveStock(track.id);
                    SendMsg(track);
                }
                return true;
            }
            return false;
        }

        public List<FerryPos> AddFerryPos(uint areaid, uint devid)
        {
            List<AreaDeviceTrack> deviceTracks = PubMaster.Area.GetAreaDevTraList(areaid, devid);
            if (deviceTracks.Count == 0)
            {
                return new List<FerryPos>();
            }
            bool isdownferry = PubMaster.Device.IsDevType(devid, DeviceTypeE.后摆渡);
            Track track;
            FerryPos pos;
            foreach (AreaDeviceTrack item in deviceTracks)
            {
                track = GetTrack(item.track_id);
                if (track == null) continue;
                pos = new FerryPos
                {
                    track_id = track.id,
                    device_id = devid
                };
                switch (track.Type)
                {
                    case TrackTypeE.上砖轨道:
                    case TrackTypeE.储砖_入:
                        pos.ferry_code = track.ferry_up_code;
                        break;
                    case TrackTypeE.储砖_出入:
                        pos.ferry_code = isdownferry ? track.ferry_up_code : track.ferry_down_code;
                        break;
                    case TrackTypeE.储砖_出:
                    case TrackTypeE.下砖轨道:
                        pos.ferry_code = track.ferry_down_code;
                        break;
                    case TrackTypeE.后置摆渡轨道:
                    case TrackTypeE.前置摆渡轨道:
                        continue;
                }
                PubMaster.Mod.TraSql.AddFerryPos(pos);
            }
            return PubMaster.Mod.TraSql.QueryFerryPosList(areaid, devid);
        }

        public bool IsExistOldFerryPos()
        {
            return PubMaster.Mod.TraSql.IsHaveFerryOldAndAdd();
        }

        public List<FerryPos> GetFerryPos(uint areaid, uint devid)
        {
            return PubMaster.Mod.TraSql.QueryFerryPosList(areaid, devid);
        }

        public List<FerryPos> GetFerryPos(uint devid)
        {
            return PubMaster.Mod.TraSql.QueryFerryPosList(devid);
        }

        /// <summary>
        /// 满足信息
        /// </summary>
        /// <param name="givetrackcode"></param>
        /// <returns></returns>
        public bool SetTrackFull(ushort area, ushort givetrackcode)
        {
            Track track = GetTrackBySite(area, givetrackcode);
            if (track != null)
            {
                if (track.StockStatus != TrackStockStatusE.满砖)
                {
                    track.StockStatus = TrackStockStatusE.满砖;
                    PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.StockStatus);
                    SendMsg(track);
                }
                return true;
            }
            return false;
        }

        public void UpdateFerryPos(uint id, int ferrypos)
        {
            PubMaster.Mod.TraSql.EditFerryPos(id, ferrypos);
        }

        public void UpdateFerryPos(FerryPos pos, bool haveold)
        {
            PubMaster.Mod.TraSql.EditFerryPos(pos, haveold);
        }

        public void UpdateFerryPos(uint devid, int trackcode, int ferrypos)
        {
            PubMaster.Mod.TraSql.EditFerryPos(devid, trackcode, ferrypos);
        }

        public List<uint> GetFerryTrackId(uint area_id, TransTypeE transType)
        {
            List<uint> list = new List<uint>();
            switch (transType)
            {
                case TransTypeE.下砖任务:
                    list.AddRange(TrackList.FindAll(c => c.InType(TrackTypeE.后置摆渡轨道)).Select(c => c.id));
                    break;
                case TransTypeE.上砖任务:
                    list.AddRange(TrackList.FindAll(c => c.InType(TrackTypeE.前置摆渡轨道)).Select(c => c.id));
                    break;
                case TransTypeE.倒库任务:
                    list.AddRange(TrackList.FindAll(c => c.InType(TrackTypeE.前置摆渡轨道)).Select(c => c.id));
                    break;
                case TransTypeE.其他:
                    list.AddRange(TrackList.FindAll(c => c.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道)).Select(c => c.id));
                    break;
                default:
                    break;
            }
            return list;
        }

        public ushort AddTrackLog(ushort area, uint devid, uint trackid, TrackLogE logtype, string memo = "")
        {
            ushort storecount = PubMaster.Goods.GetTrackStockCount(trackid);
            TrackLog log = new TrackLog()
            {
                dev_id = devid,
                track_id = trackid,
                type = (byte)logtype,
                stock_count = storecount,
                log_time = DateTime.Now,
                memo = memo,
                area = area
            };

            PubMaster.Mod.TraSql.AddTrackLog(log);
            return storecount;
        }

        /// <summary>
        /// 判断轨道是否为兄弟轨道
        /// </summary>
        /// <param name="taketrackid"></param>
        /// <param name="givetrackid"></param>
        /// <returns></returns>
        public bool IsBrotherTrack(uint taketrackid, uint givetrackid)
        {
            return TrackList.Exists(c => c.id == taketrackid && c.brother_track_id == givetrackid);
        }

        public bool GetTrackFerryCode(uint take_track_id, DeviceTypeE ferrytype, out ushort trackferrycode, out string result)
        {
            result = "";
            trackferrycode = 0;
            Track track = TrackList.Find(c => c.id == take_track_id);
            if (track != null)
            {
                switch (track.Type)
                {
                    case TrackTypeE.上砖轨道:
                    case TrackTypeE.储砖_入:
                        trackferrycode = track.ferry_up_code;
                        break;
                    case TrackTypeE.下砖轨道:
                    case TrackTypeE.储砖_出:
                        trackferrycode = track.ferry_down_code;
                        break;
                    case TrackTypeE.储砖_出入:
                        if (ferrytype == DeviceTypeE.后摆渡)
                        {
                            trackferrycode = track.ferry_up_code;
                        }
                        else
                        {
                            trackferrycode = track.ferry_down_code;
                        }
                        break;
                    default:

                        break;
                }

                if (trackferrycode != 0)
                {
                    result = "执行指令定位到[ " + track?.name ?? "" + " ]";
                    return true;
                }
            }

            result = "找不到轨道信息[ " + track?.name ?? "" + " ]";
            return false;
        }

        public bool IsTrackHaveStock(uint track_id)
        {
            return TrackList.Exists(c => c.id == track_id && c.StockStatus == TrackStockStatusE.有砖);
        }

        public bool IsEmtpy(uint track_id)
        {
            return TrackList.Exists(c => c.id == track_id && c.StockStatus == TrackStockStatusE.空砖);
        }

        /// <summary>
        /// 判断是否空砖但非停用-上砖轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsEmtyp4Up(uint trackid)
        {
            return TrackList.Exists(c => c.id == trackid && c.InStatus(TrackStatusE.仅上砖, TrackStatusE.启用) && c.InStockStatus(TrackStockStatusE.空砖) && c.AlertStatus == TrackAlertE.正常);
        }

        /// <summary>
        /// 判断是否非空砖但非停用-上砖轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsNotEmtyp4Up(uint trackid)
        {
            return TrackList.Exists(c => c.id == trackid && c.InStatus(TrackStatusE.仅上砖, TrackStatusE.启用) && c.NotInStockStatus(TrackStockStatusE.空砖) && c.AlertStatus == TrackAlertE.正常);
        }

        /// <summary>
        /// 判断是否空砖但非停用-下砖轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsEmtyp4Down(uint trackid)
        {
            return TrackList.Exists(c => c.id == trackid && c.InStatus(TrackStatusE.仅下砖, TrackStatusE.启用) && c.InStockStatus(TrackStockStatusE.空砖) && c.AlertStatus == TrackAlertE.正常);
        }

        /// <summary>
        /// 判断是否非空砖但非停用-下砖轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsNotEmtyp4Down(uint trackid)
        {
            return TrackList.Exists(c => c.id == trackid && c.InStatus(TrackStatusE.仅下砖, TrackStatusE.启用) && c.NotInStockStatus(TrackStockStatusE.空砖) && c.AlertStatus == TrackAlertE.正常);
        }

        public bool IsStopUsing(uint track_id, TransTypeE transType)
        {
            switch (transType)
            {
                case TransTypeE.下砖任务:
                case TransTypeE.手动下砖:
                case TransTypeE.同向下砖:
                    return TrackList.Exists(c => c.id == track_id && c.InStatus(TrackStatusE.停用, TrackStatusE.仅上砖));

                case TransTypeE.上砖任务:
                case TransTypeE.手动上砖:
                case TransTypeE.同向上砖:
                    return TrackList.Exists(c => c.id == track_id && c.InStatus(TrackStatusE.停用, TrackStatusE.仅下砖));
                default:
                    return TrackList.Exists(c => c.id == track_id && c.InStatus(TrackStatusE.停用));
            }
        }

        internal bool CheckAndSetFull(uint track_id, int storecount)
        {
            Track track = TrackList.Find(c => c.id == track_id);
            if (track != null)
            {
                if (track.StockStatus != TrackStockStatusE.满砖 && track.max_store <= storecount)
                {
                    track.StockStatus = TrackStockStatusE.满砖;
                    PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.StockStatus);
                    SendMsg(track);
                }

                if (track.StockStatus == TrackStockStatusE.满砖)
                {
                    return true;
                }
            }
            return false;
        }

        internal List<uint> GetAreaTrackIds(uint areaid, TrackTypeE type)
        {
            return TrackList.FindAll(c => c.area == areaid && c.Type == type).Select(c => c.id).ToList();
        }

        /// <summary>
        /// 判断轨道是否满砖状态
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public bool IsTrackFull(uint track_id)
        {
            return TrackList.Exists(c => c.id == track_id && c.StockStatus == TrackStockStatusE.满砖);
        }

        public int GetTrackCode(ushort trackid, uint ferryid)
        {
            int code = 0;
            Track t = TrackList.Find(c => c.id == trackid);
            if (t != null)
            {
                switch (PubMaster.Device.GetDeviceType(ferryid))
                {
                    case DeviceTypeE.前摆渡:
                        code = t.ferry_down_code;
                        break;
                    case DeviceTypeE.后摆渡:
                        code = t.ferry_up_code;
                        break;
                }
            }
            return code;
        }

        /// <summary>
        /// 是否是储砖轨道
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        public bool IsStoreType(uint traid)
        {
            return TrackList.Exists(c => c.id == traid && c.IsStoreTrack());
        }

        /// <summary>
        /// 判断轨道是否符合状态
        /// </summary>
        /// <param name="traid"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool IsTrackType(uint traid, params TrackTypeE[] types)
        {
            return TrackList.Exists(c => c.id == traid && c.InType(types));
        }

        /// <summary>
        /// 判断是否是摆渡车类型
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        public bool IsFerryTrackType(uint traid)
        {
            return TrackList.Exists(c => c.id == traid && c.IsFerryTrack());
        }

        public bool IsUpAreaTrack(uint traid)
        {
            return TrackList.Exists(c => c.id == traid && c.IsUpAreaTrack());
        }
        public bool IsDownAreaTrack(uint traid)
        {
            return TrackList.Exists(c => c.id == traid && c.IsDownAreaTrack());
        }

        /// <summary>
        /// 更新轨道状态发送更新信息
        /// </summary>
        /// <param name="track"></param>
        private void SendMsg(Track track)
        {
            MsgAction msg = new MsgAction
            {
                o1 = track
            };
            Messenger.Default.Send(msg, MsgToken.TrackStatusUpdate);
        }

        /// <summary>
        /// 轨道是否符合放货条件
        /// </summary>
        /// <param name="give_track_id"></param>
        /// <returns></returns>
        public bool IsStatusOkToGive(uint give_track_id)
        {
            return TrackList.Exists(c => c.id == give_track_id
                                    && c.StockStatus != TrackStockStatusE.满砖
                                    && c.AlertStatus == TrackAlertE.正常
                                    && c.InStatus(TrackStatusE.启用, TrackStatusE.仅下砖));
        }

        /// <summary>
        /// 获取入库满砖轨道
        /// 1.优先砖机当前上砖品种
        /// 2.优先砖机当前预约品种
        /// 3.按入库时间最早
        /// </summary>
        /// <returns></returns>
        public List<Track> GetFullInTrackList()
        {
            List<Track> tracks = new List<Track>();
            List<Stock> stocks = new List<Stock>();
            // 获取区域
            List<Area> areas = PubMaster.Area.GetAreaList();
            foreach (Area area in areas)
            {
                // 获取所有可倒库的库存
                List<Stock> areastocks = PubMaster.Goods.GetStocksOrderByOut(area.id);
                if (areastocks != null && areastocks.Count > 0) stocks.AddRange(areastocks);
            }

            foreach (Stock item in stocks)
            {
                Track tra = GetTrack(item.track_id);
                if (tra.TrackStatus == TrackStatusE.启用 && tra.StockStatus == TrackStockStatusE.满砖)
                {
                    tracks.Add(tra);
                }
            }

            return tracks;
        }

        /// <summary>
        /// 判断是否是空轨道
        /// </summary>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public bool IsTrackEmtpy(uint track_id)
        {
            Track track = GetTrack(track_id);
            if (track != null)
            {
                return track.TrackStatus != TrackStatusE.停用
                        && track.StockStatus == TrackStockStatusE.空砖
                        && track.AlertStatus == TrackAlertE.正常
                        && !PubMaster.Goods.ExistStockInTrack(track_id);
            }
            return false;
        }

        /// <summary>
        /// 轨道是否启用
        /// </summary>
        /// <param name="track_id"></param>
        /// <param name="trackStatus"></param>
        /// <returns></returns>
        internal bool IsTrackEnable(uint track_id, TrackStatusE trackStatus)
        {
            return TrackList.Exists(c => c.id == track_id && c.AlertStatus == TrackAlertE.正常 && c.InStatus(TrackStatusE.启用, trackStatus));
        }

        /// <summary>
        /// 获取最近 下砖/上砖的轨道
        /// </summary>
        /// <param name="list"></param>
        /// <param name="devid"></param>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        internal List<Track> GetRecentUseTracks(List<AreaDeviceTrack> list, uint devid, uint goodsid)
        {
            return TrackList.FindAll(c => c.recent_goodid == goodsid  //&& c.recent_tileid == devid
                                        && list.Exists(l => l.track_id == c.id));
        }

        /// <summary>
        /// 获取轨道中最近存储品种轨道
        /// </summary>
        /// <param name="list"></param>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        internal List<Track> GetRecentGoodTracks(List<AreaDeviceTrack> list, uint goodsid)
        {
            return TrackList.FindAll(c => c.recent_goodid == goodsid && list.Exists(l => l.track_id == c.id));
        }

        internal List<Track> GetRecentTileTracks(List<AreaDeviceTrack> list, uint devid)
        {
            return TrackList.FindAll(c => c.recent_tileid == devid && list.Exists(l => l.track_id == c.id));
        }

        /// <summary>
        /// 获取区域指定类型的轨道
        /// </summary>
        /// <param name="area"></param>
        /// <param name="totracktype"></param>
        /// <returns></returns>
        public List<Track> GetTrackInTypeFree(ushort area, TrackTypeE totracktype)
        {
            List<Track> list = TrackList.FindAll(c => c.area == area
                                       && c.Type == totracktype
                                       && c.AlertStatus == TrackAlertE.正常
                                       && c.TrackStatus != TrackStatusE.停用);
            if (list.Count > 0)
            {
                list.Sort((x, y) => x.stock_status.CompareTo(y.stock_status));
            }
            return list;
        }

        /// <summary>
        /// 根据摆渡车类型获取可到轨道
        /// </summary>
        /// <param name="area"></param>
        /// <param name="type"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public Track GetTrackBySite(ushort area, DeviceTypeE type, ushort site)
        {
            if (site == 0) return null;
            if (type == DeviceTypeE.前摆渡)
            {
                return GetTrackBySite(area, new List<TrackTypeE>() { TrackTypeE.储砖_出, TrackTypeE.储砖_出入, TrackTypeE.上砖轨道 }, site);
            }

            return GetTrackBySite(area, new List<TrackTypeE>() { TrackTypeE.储砖_入, TrackTypeE.储砖_出入, TrackTypeE.下砖轨道 }, site);
        }

        public void SetTrackAlert(uint trackid, uint carrier, uint transid, TrackAlertE alert)
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track != null && track.AlertStatus != alert)
            {
                track.AlertStatus = alert;
                track.alert_carrier = carrier;
                track.alert_trans = transid;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.Alert);
                SendMsg(track);
            }
        }

        public ushort GetFerryTrackArea(uint devId, ushort startTrack)
        {
            Device dev = PubMaster.Device.GetDevice(devId);
            if (dev.Type == DeviceTypeE.前摆渡)
            {
                return TrackList.Find(c => c.IsUpAreaTrack() && (c.ferry_down_code == startTrack || c.ferry_up_code == startTrack))?.area ?? dev.area;
            }
            return TrackList.Find(c => c.IsDownAreaTrack() && (c.ferry_down_code == startTrack || c.ferry_up_code == startTrack))?.area ?? dev.area;
        }

        /// <summary>
        /// 轨道提前满砖
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="value"></param>
        /// <param name="earytime"></param>
        public void SetTrackEaryFull(uint trackid, bool value, DateTime? earytime)
        {
            Track track = TrackList.Find(c => c.id == trackid);
            if (track != null && track.early_full != value)
            {
                track.early_full = value;
                track.full_time = earytime;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.EarlyFull);
                if (value)
                {
                    PubMaster.Warn.AddTraWarn(track.area, track.line, WarningTypeE.TrackEarlyFull, (ushort)track.id, track.name);
                }
                else
                {
                    PubMaster.Warn.RemoveTraWarn(WarningTypeE.TrackEarlyFull, (ushort)track.id);
                }
                SendMsg(track);
            }
        }

        /// <summary>
        /// 获取上砖侧半满轨道
        /// </summary>
        public List<Track> GetUpSortTrack()
        {
            return TrackList.FindAll(c => c.TrackStatus == TrackStatusE.启用
                                                        && c.InType(TrackTypeE.储砖_出)
                                                        && c.InStockStatus(TrackStockStatusE.有砖)
                                                        && c.up_split_point != 0
                                                        && GetAndRefreshUpCount(c.id) == 0
                                                        && PubMaster.Goods.GetStocks(c.id).Count > 1);
        }


        #endregion

        #region[更新轨道状态]

        public bool CheckAndUpateTrackStatus(TrackUpdatePack pack, out string result)
        {
            if (pack == null)
            {
                result = "找不到轨道信息";
                return false;
            }
            if (pack.TrackId == 0)
            {
                result = "轨道ID不能为空";
                return false;
            }

            Track track = GetTrack(pack.TrackId);
            if (track.stock_status != pack.OldStockStatus || track.track_status != pack.OldTrackStatus)
            {
                result = "请刷新后再修改！";
                return false;
            }


            if (!pack.IsStockStatusChange() && !pack.IsStatusChange())
            {
                result = "不需要更改，请刷新！";
                return false;
            }

            if (pack.IsStatusChange() && !SetTrackStatus(pack.TrackId, (TrackStatusE)pack.TrackStatus, out result, "平板"))
            {
                return false;
            }

            if (pack.IsStockStatusChange() && !SetStockStatus(pack.TrackId, (TrackStockStatusE)pack.StockStatus, out result, "平板"))
            {
                return false;
            }

            result = "";
            return true;
        }

        #endregion

        #region[判断是否有非空轨道状态的轨道]

        public bool HaveTrackInGoodButNotStock(uint areaid, uint tilelifterid, uint goodsid, out List<uint> trackids)
        {
            trackids = new List<uint>();
            //List<AreaDeviceTrack> devtrack = PubMaster.Area.GetAreaDevTraList(areaid, tilelifterid);
            List<AreaDeviceTrack> devtrack = PubMaster.Area.GetTileWorkTraList(areaid, tilelifterid, true);

            #region[ 判断是否使用分割点后的库存做出库任务]
            bool isnotuseupsplitstock = false;
            //默认出库轨道库存是不管分割点的
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)
                && PubMaster.Dic.IsSwitchOnOff(DicTag.CannotUseUpSplitStock))
            {
                //开关打开后，分割点后的库存不能直接出库，需要倒库
                isnotuseupsplitstock = true;
            }
            #endregion

            //1.查看是否有最近下砖品种轨道
            List<Track> recentusetracks = GetRecentGoodTracks(devtrack, goodsid);
            foreach (Track track in recentusetracks)
            {
                if (track.StockStatus != TrackStockStatusE.有砖
                    || track.NotInStatus(TrackStatusE.启用, TrackStatusE.仅上砖)
                    || track.AlertStatus != TrackAlertE.正常
                    || (isnotuseupsplitstock && track.up_split_point != 0 && PubMaster.Track.GetAndRefreshUpCount(track.id) <= 0))
                {
                    UpdateRecentTile(track.id, 0);
                    UpdateRecentGood(track.id, 0);
                    PubMaster.DevConfig.SetLastTrackId(tilelifterid, 0);
                    continue;
                }
                if (!PubMaster.Goods.ExistStockInTrack(track.id))
                    trackids.Add(track.id);
            }
            return trackids.Count > 0;
        }

        public bool HaveTrackInGoodFrist(uint areaid, uint tilelifterid, uint goodsid, uint currentTake, out uint trackid)
        {
            if (currentTake == 0)
            {
                trackid = 0;
                return false;
            }

            #region[判断是否使用分割点后的库存做出库任务]
            bool isnotuseupsplitstock = false;
            //默认出库轨道库存是不管分割点的
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)
                && PubMaster.Dic.IsSwitchOnOff(DicTag.CannotUseUpSplitStock))
            {
                //开关打开后，分割点后的库存不能直接出库，需要倒库
                isnotuseupsplitstock = true;
            }
            #endregion

            Track track = PubMaster.Track.GetTrack(currentTake);
            if (track == null)
            {
                trackid = 0;
                return false;
            }

            if (track.StockStatus == TrackStockStatusE.空砖
                    || track.NotInStatus(TrackStatusE.启用, TrackStatusE.仅上砖)
                    || track.AlertStatus != TrackAlertE.正常
                    || (isnotuseupsplitstock && track.up_split_point != 0 && PubMaster.Track.GetAndRefreshUpCount(track.id) <= 0)
                    )
            {
                UpdateRecentTile(track.id, 0);
                UpdateRecentGood(track.id, 0);
                PubMaster.DevConfig.SetLastTrackId(tilelifterid, 0);
                trackid = 0;
                return false;
            }

            trackid = track.id;
            return true;
        }

        public bool HaveEmptyTrackInTile(ushort areaid, uint devid)
        {
            //List<AreaDeviceTrack> list = PubMaster.Area.GetAreaDevTraList(areaid, devid);
            List<AreaDeviceTrack> list = PubMaster.Area.GetTileWorkTraList(areaid, devid, false);
            return TrackList.Exists(c => c.StockStatus == TrackStockStatusE.空砖
                    && list.Exists(l => l.track_id == c.id));
        }

        public bool IsRecentGoodId(uint trackid, uint goods_id)
        {
            return TrackList.Exists(c => c.id == trackid && c.recent_goodid == goods_id);
        }

        public bool IsEarlyFullTimeOver(uint id)
        {
            Track track = GetTrack(id);
            if (track != null && track.early_full)
            {
                if (track.full_time is DateTime time)
                {
                    return (DateTime.Now - time).TotalMinutes > 5;
                }
                else
                {
                    track.full_time = DateTime.Now;
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取并刷新轨道上砖数量
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public int GetAndRefreshUpCount(uint trackid)
        {
            Track track = GetTrack(trackid);
            int count = -1;
            if (track != null)
            {
                if (track.up_split_point == 0) return count;
                count = PubMaster.Goods.GetUpStocks(trackid);
                if (count != track.upcount)
                {
                    PubMaster.Track.UpdateUpCount(trackid, count);
                    mLog.Status(true, "更新[" + track.name + "]可上砖数量为[" + count + "]");
                    return count;
                }
                return track.upcount;
            }
            return count;
        }


        /// <summary>
        /// 上砖数量是否为空
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsUpCountEmpty(uint trackid)
        {
            Track track = GetTrack(trackid);
            if (track != null && track.upcount == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 上砖侧是否分割
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsUpSplit(uint trackid)
        {
            Track track = GetTrack(trackid);
            if (track != null)
            {
                return track.up_split_point != 0;
            }
            return false;
        }

        /// <summary>
        /// 是否存在有上砖分割点的储砖出库轨道
        /// </summary>
        /// <returns></returns>
        public bool IsUpSplit()
        {
            return TrackList.Exists(c => c.up_split_point != 0 && c.Type == TrackTypeE.储砖_出);
        }

        /// <summary>
        /// 获取上砖侧分割点
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public int GetUpPoint(uint trackid)
        {
            Track track = GetTrack(trackid);
            if (track != null)
            {
                return track.up_split_point;
            }
            return 0;
        }

        /// <summary>
        /// 上砖分割检查
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool CheckStocksTrack(uint trackid)
        {
            if (IsUpSplit(trackid))
            {
                if (GetAndRefreshUpCount(trackid) > 0)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        #region[获取轨道状态用于记录]

        /// <summary>
        /// 获取日志用于记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTrackLogInfo(uint id)
        {
            return GetTrack(id)?.GetLog() ?? "";
        }

        /// <summary>
        /// 获取轨道库存/使用状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTrackStatusLogInfo(uint id)
        {
            return GetTrack(id)?.GetStatusLog() ?? "";
        }
        #endregion

        #region[更新轨道脉冲]

        /// <summary>
        /// 更新轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool UpdateTrackLimitOut(uint areaid, ushort lineid, ushort point)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == areaid && c.line == lineid && c.InType(TrackTypeE.储砖_出, TrackTypeE.储砖_出入));
            if (tracks.Count > 0)
            {
                foreach (var item in tracks)
                {
                    item.limit_point_up = point;
                    PubMaster.Mod.TraSql.EditTrack(item, TrackUpdateE.Point);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool UpdateTrackLimitIn(uint areaid, ushort lineid, ushort point)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == areaid && c.line == lineid && c.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入));
            if (tracks.Count > 0)
            {
                foreach (var item in tracks)
                {
                    item.limit_point = point;
                    PubMaster.Mod.TraSql.EditTrack(item, TrackUpdateE.Point);
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 更新轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool UpdateTrackFirstIn(uint areaid, ushort lineid, ushort point)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == areaid && c.line == lineid && c.InType(TrackTypeE.储砖_入));
            if (tracks.Count > 0)
            {
                foreach (var item in tracks)
                {
                    item.split_point = point;
                    PubMaster.Mod.TraSql.EditTrack(item, TrackUpdateE.Point);
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 更新轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool UpdateTrackLastOut(uint areaid, ushort lineid, ushort point)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == areaid && c.line == lineid && c.InType(TrackTypeE.储砖_出));
            if (tracks.Count > 0)
            {
                foreach (var item in tracks)
                {
                    item.split_point = point;
                    PubMaster.Mod.TraSql.EditTrack(item, TrackUpdateE.Point);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool UpdateTrackSortOut(uint areaid, ushort lineid, ushort point)
        {
            List<Track> tracks = TrackList.FindAll(c => c.area == areaid &&  c.line == lineid && c.InType(TrackTypeE.储砖_出, TrackTypeE.储砖_出入));
            if (tracks.Count > 0)
            {
                foreach (var item in tracks)
                {
                    item.up_split_point = point;
                    PubMaster.Mod.TraSql.EditTrack(item, TrackUpdateE.Point);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取区域上砖机的所有轨道
        /// </summary>
        /// <param name="area_id"></param>
        /// <returns></returns>
        public List<uint> GetUpTileTracks(uint area_id)
        {
            return TrackList.FindAll(c => c.area == area_id && c.Type == TrackTypeE.上砖轨道)?.Select(c => c.id).ToList();
        }

        #endregion

        #region [运输车复位脉冲]

        /// <summary>
        /// 是否能直接上摆渡车（复位脉冲判断）
        /// </summary>
        /// <param name="trackid">当前轨道ID</param>
        /// <param name="md">移动方向</param>
        /// <param name="point">当前脉冲</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CanMoveToFerryAboutPos(uint trackid, DevMoveDirectionE md, ushort point, out string result)
        {
            result = "";
            Track track = GetTrack(trackid);
            if (track == null)
            {
                result = "无轨道数据";
                return false;
            }

            ushort reset = 0;
            switch (md)
            {
                case DevMoveDirectionE.前进:
                    if (track.Type == TrackTypeE.下砖轨道 && track.ferry_up_code < 200)
                    {
                        reset = GetCarrierPos(track.area, CarrierPosE.下砖机复位点);
                    }
                    else
                    {
                        reset = GetCarrierPos(track.area, CarrierPosE.轨道前侧复位点);
                    }

                    if (reset == 0)
                    {
                        result = "未配置相关复位点脉冲";
                        return false;
                    }

                    if (reset > point)
                    {
                        result = "小车需超复位点，位于轨道头才可上摆渡，请先前进至点";
                        return false;
                    }
                    break;

                case DevMoveDirectionE.后退:
                    if (track.Type == TrackTypeE.上砖轨道 && track.ferry_up_code > 500)
                    {
                        reset = GetCarrierPos(track.area, CarrierPosE.上砖机复位点);
                    }
                    else
                    {
                        reset = GetCarrierPos(track.area, CarrierPosE.轨道后侧复位点);
                    }

                    if (reset == 0)
                    {
                        result = "未配置相关复位点脉冲";
                        return false;
                    }

                    if (reset < point)
                    {
                        result = "小车需超复位点，位于轨道头才可上摆渡，请先后进至点";
                        return false;
                    }
                    break;

                default:
                    result = "请给定移动至摆渡车的方向";
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 获取区域运输车复位点脉冲（单个）
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="cp"></param>
        /// <returns></returns>
        public ushort GetCarrierPos(uint area_id, CarrierPosE cp)
        {
            return CarrierPosList.Find(c => c.area_id == area_id && c.CarrierPosType == cp)?.track_pos ?? 0;
        }

        /// <summary>
        /// 获取区域运输车复位点脉冲（全部）
        /// </summary>
        /// <param name="area_id"></param>
        /// <returns></returns>
        public List<CarrierPos> GetCarrierPosList(uint area_id)
        {
            return CarrierPosList.FindAll(c=>c.area_id == area_id);
        }

        /// <summary>
        /// 新增区域运输车复位点
        /// </summary>
        /// <param name="pos"></param>
        public void AddCarrierPos(CarrierPos pos)
        {
            if (CarrierPosList.Exists(c => c.area_id == pos.area_id
                    && c.track_point == pos.track_point
                    && c.track_pos == pos.track_pos))
            {
                return;
            }
            pos.id = (CarrierPosList.Max(c => c.id) + 1);
            CarrierPosList.Add(pos);
            PubMaster.Mod.TraSql.AddCarrierPos(pos);
        }

        /// <summary>
        /// 修改区域运输车复位点
        /// </summary>
        /// <param name="pos"></param>
        public void EditCarrierPos(CarrierPos pos)
        {
            CarrierPos cp = CarrierPosList.Find(c => c.id == pos.id);
            if (cp == null) return;
            cp.track_point = pos.track_point;
            cp.track_pos = pos.track_pos;
            PubMaster.Mod.TraSql.EditCarrierPos(pos);
        }

        #endregion

        #region 更新轨道取放方向

        /// <summary>
        /// 根据上砖 砖机工位轨道吧变更类型及取放方向
        /// </summary>
        /// <param name="tileTrackID"></param>
        /// <param name="setTrackID"></param>
        public void UpdateDirByUp(uint tileTrackID, uint setTrackID)
        {
            // 获取砖机轨道
            Track tileTrack = TrackList.Find(c => c.id == tileTrackID);

            // 获取作业轨道
            Track setTrack = TrackList.Find(c => c.id == setTrackID);

            if (tileTrack == null || setTrack == null) return;

            // 先确认轨道取货方向 
            // (砖机code < 作业code [后退放砖 即前进取砖])
            // (砖机code > 作业code [前进放砖 即后退取砖])
            bool is_take_forward = (tileTrack.ferry_up_code / 100) < (setTrack.ferry_up_code / 100);

            // 再确认轨道放货方向（常规是相反，当同侧上下时为一致方向）
            bool is_give_forward = setTrack.same_side_inout ? is_take_forward : !is_take_forward;

            bool isupdate = false;
            if (!isupdate)
            {
                // 确认是否改为出库为主
                if (setTrack.Type2 == TrackType2E.入库)
                {
                    setTrack.Type2 = TrackType2E.出库;
                    isupdate = true;
                }

                // 更新取放方向
                if (setTrack.is_take_forward != is_take_forward)
                {
                    setTrack.is_take_forward = is_take_forward;
                    isupdate = true;
                }

                if (setTrack.is_give_back == is_give_forward)
                {
                    setTrack.is_give_back = !is_give_forward;
                    isupdate = true;
                }
            }

            if (isupdate) UpdateType2AndDir(setTrack);
        }

        /// <summary>
        /// 根据下砖 砖机工位轨道吧变更类型及取放方向
        /// </summary>
        /// <param name="tileTrackID"></param>
        /// <param name="setTrackID"></param>
        public void UpdateDirByDown(uint tileTrackID, uint setTrackID)
        {
            // 获取砖机轨道
            Track tileTrack = TrackList.Find(c => c.id == tileTrackID);

            // 获取作业轨道
            Track setTrack = TrackList.Find(c => c.id == setTrackID);

            if (tileTrack == null || setTrack == null) return;

            // 先确认轨道放货方向 
            // (砖机code > 作业code [前进取砖 即后退放砖])
            // (砖机code < 作业code [后退取砖 即前进放砖])
            bool is_give_back = (tileTrack.ferry_up_code / 100) > (setTrack.ferry_up_code / 100);

            // 再确认轨道取货方向（常规是相反，当同侧上下时为一致方向）
            bool is_take_back = setTrack.same_side_inout ? is_give_back : !is_give_back;

            bool isupdate = false;
            if (!isupdate)
            {
                // 确认是否改为入库为主
                if (setTrack.Type2 == TrackType2E.出库)
                {
                    setTrack.Type2 = TrackType2E.入库;
                    isupdate = true;
                }

                // 更新取放方向
                if (setTrack.is_give_back != is_give_back)
                {
                    setTrack.is_give_back = is_give_back;
                    isupdate = true;
                }

                if (setTrack.is_take_forward == is_take_back)
                {
                    setTrack.is_take_forward = !is_take_back;
                    isupdate = true;
                }
            }

            if (isupdate) UpdateType2AndDir(setTrack);
        }

        /// <summary>
        /// 更新轨道出入库类型及取放方向
        /// </summary>
        /// <param name="setTrack"></param>
        private void UpdateType2AndDir(Track setTrack)
        {
            if (setTrack.StockStatus != TrackStockStatusE.空砖)
            {
                // 更新库存统计-出入库类型
                PubMaster.Sums.CheckTrackSum(setTrack.id);
            }

            PubMaster.Mod.TraSql.EditTrack(setTrack, TrackUpdateE.TGtype);
        }

        #endregion


        /// <summary>
        /// 获取无任务，空闲轨道IDs
        /// </summary>
        /// <param name="id">指定轨道ID</param>
        /// <returns></returns>
        public List<uint> GetTrackFreeEmptyTrackIds(uint id)
        {
            Track track = GetTrack(id);
            List<Track> tracks = TrackList.FindAll(c => c.Type == track.Type && c.StockStatus == TrackStockStatusE.空砖 && c.TrackStatus == TrackStatusE.启用);

            List<TrackDis> tras = new List<TrackDis>();
            foreach (var item in tracks)
            {
                tras.Add(new TrackDis()
                {
                    trackid = item.id,
                    dis = Math.Abs((short)(item.id - id))
                });
            }

            tras.Sort((x, y) => x.dis.CompareTo(y.dis));

            return tras.Select(c => c.trackid)?.ToList() ?? new List<uint>();
        }


        #region[倒库轨道分析]
        /// <summary>
        /// 倒库等级：无倒库
        /// </summary>
        private byte SORT_LEVEL_NO = 0;
        /// <summary>
        /// 倒库等级1：最优先等级
        /// </summary>
        private byte SORT_LEVEL_1 = 1;
        /// <summary>
        /// 倒库等级：次优先等级
        /// </summary>
        private byte SORT_LEVEL_2 = 2;
        /// <summary>
        /// 倒库等级：低优先等级
        /// </summary>
        private byte SORT_LEVEL_3 = 3;

        /// <summary>
        /// 轨道前空出默认5个位置
        /// </summary>
        public int TrackSortFrontCount { set; get; } = 5;
        /// <summary>
        /// 轨道中间空出默认5个位置
        /// </summary>
        public int TrackSortMidCount { set; get; } = 5;
        /// <summary>
        /// 轨道后空出默认5个位置
        /// </summary>
        public int TrackSortBackCount { set; get; } = 5;
        /// <summary>
        /// 更新轨道倒库状态和等级
        /// </summary>
        /// <param name="track"></param>
        /// <param name="able"></param>
        /// <param name="sortlevel"></param>
        private void SetTrackSortable(Track track, bool able, byte sortlevel, string memo = null)
        {
            if(track.sort_able != able || track.sort_level != sortlevel)
            {
                try
                {
                    mLog.Status(true, string.Format("轨道[ {0} ], 倒库[ {1} -> {2} ], 等级[ {3} -> {4} ], 备注[ {5} ]",
                        track.name, track.sort_able, able, track.sort_level, sortlevel, memo));
                }
                catch { }
                track.sort_able = able;
                track.sort_level = sortlevel;
                PubMaster.Mod.TraSql.EditTrack(track, TrackUpdateE.SortAble);


                SendMsg(track);
            }
        }

        /// <summary>
        /// 分析倒库轨道状态
        /// </summary>
        public void DoSortTrackDiagnose(List<uint> tracids)
        {
            ushort safe = (ushort)PubMaster.Dic.GetDtlDouble(DicTag.StackPluse, 0);//217

            foreach (var item in TrackList)
            {
                if (item.TrackStatus == TrackStatusE.停用) continue;
                if (tracids.Contains(item.id))
                {
                    SetTrackSortable(item, false, SORT_LEVEL_NO, "任务中");
                    continue;
                }
                switch (item.Type)
                {
                    case TrackTypeE.储砖_入:
                        CheckInTrackStatus(item);
                        break;
                    case TrackTypeE.储砖_出:
                        CheckOutTrackStatus(item, safe);
                        break;

                    case TrackTypeE.储砖_出入:
                        switch (item.Type2)
                        {
                            case TrackType2E.通用:
                                CheckInOutTrackStatus(item, safe);
                                break;
                            case TrackType2E.入库:
                                CheckInTrackStatus(item);
                                break;
                            case TrackType2E.出库:
                                CheckOutTrackStatus(item, safe);
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 检查入轨道状态设置可倒库状态
        /// 入轨道改为可倒库状态
        ///  [等级1] 1.入轨道满砖
        ///  [等级2] 2.入轨道底部品种无砖机继续使用
        /// </summary>
        /// <param name="track"></param>
        private void CheckInTrackStatus(Track track)
        {
            //空砖->跳出
            if (track.StockStatus == TrackStockStatusE.空砖)
            {
                SetTrackSortable(track, false, SORT_LEVEL_NO, "轨道空砖");
                return;
            }
            
            //1.入轨道满砖
            if (track.StockStatus == TrackStockStatusE.满砖)
            {
                SetTrackSortable(track, true, SORT_LEVEL_1, "轨道满砖");
                return;
            }

            //2.入轨道底部品种无砖机继续使用
            Stock btmstock = PubMaster.Goods.GetStockForIn(track.id);
            if (btmstock != null)
            {
                if (!PubMaster.DevConfig.IsHaveSameTileNowGood(track.area, btmstock.goods_id, TileWorkModeE.下砖))
                {
                    SetTrackSortable(track, true, SORT_LEVEL_2, "无砖机正在下该品种");
                    return;
                }
            }
        }

        /// <summary>
        /// 检查出轨道状态设置可倒库状态
        /// 出轨道改为可倒库状态
        /// [等级1] 1.出轨道空
        /// [等级2] 2.出轨道前部大量空位，无上砖机上砖
        /// [等级3] 3.出轨道尾部大量空位，无上砖机上砖
        /// </summary>
        /// <param name="track"></param>
        private void CheckOutTrackStatus(Track track, ushort safe)
        {
            // 1.出轨道空
            if (track.StockStatus == TrackStockStatusE.空砖)
            {
                SetTrackSortable(track, true, SORT_LEVEL_1, "轨道空砖");
                return;
            }

            // 2.头部库存品种无砖机正在上砖
            Stock topstock = PubMaster.Goods.GetStockForOut(track.id);
            if (topstock != null)
            {
                if (!PubMaster.DevConfig.IsHaveSameTileNowGood(track.area, topstock.goods_id, TileWorkModeE.上砖))
                {
                    //第一车库存距离轨道头部有5个车的距离
                    int discount = GetPointCompareCount((track.is_take_forward ? track.limit_point : track.limit_point_up), topstock.location, safe);
                    if (discount >= TrackSortFrontCount)
                    {
                        SetTrackSortable(track, true, SORT_LEVEL_2, "轨道头部有空间");
                        return;
                    }

                    // 3.出轨道尾部大量空位，无上砖机上砖
                    Stock btmstock = PubMaster.Goods.GetStockForIn(track.id);
                    if (btmstock != null)
                    {
                        // 结束点
                        uint overPoint = 0;
                        if (track.Type == TrackTypeE.储砖_出入)
                        {
                            overPoint = track.is_give_back ? track.limit_point_up : track.limit_point;
                        }
                        else
                        {
                            overPoint = track.split_point;
                        }

                        //第一车库存距离轨道尾部有5个车的距离
                        discount = GetPointCompareCount(overPoint, btmstock.location, safe);
                        if (discount >= TrackSortBackCount)
                        {
                            SetTrackSortable(track, true, SORT_LEVEL_3, "轨道尾部有空间");
                            return;
                        }
                    }

                }
                else
                {
                    SetTrackSortable(track, false, SORT_LEVEL_NO, "有砖机正在上该品种");
                    return;
                }
            }

        }

        /// <summary>
        /// 检查出入轨道状态设置可倒库状态
        /// [等级1] 1.出库侧空
        /// [等级2] 2.出库侧头部大量空位，无上砖机上砖
        /// [等级3] 3.出库侧尾部大量空位，无上砖机上砖
        /// </summary>
        /// <param name="track"></param>
        private void CheckInOutTrackStatus(Track track, ushort safe)
        {
            // 1.出轨道空
            if (track.StockStatus == TrackStockStatusE.空砖)
            {
                SetTrackSortable(track, false, SORT_LEVEL_NO,"轨道空砖");
                return;
            }

            // 2.头部库存品种无砖机正在上砖
            Stock topstock = PubMaster.Goods.GetStockForOut(track.id);
            if (topstock != null)
            {
                if (!PubMaster.DevConfig.IsHaveSameTileNowGood(track.area, topstock.goods_id, TileWorkModeE.上砖))
                {
                    //如果未满，则需要判断尾部是否有砖机继续下砖
                    if (track.StockStatus == TrackStockStatusE.有砖)
                    {
                        Stock bottomstock = PubMaster.Goods.GetStockForIn(track.id);
                        if (bottomstock != null)
                        {
                            if (PubMaster.DevConfig.IsHaveSameTileNowGood(track.area, bottomstock.goods_id, TileWorkModeE.下砖))
                            {
                                SetTrackSortable(track, false, SORT_LEVEL_NO, "有砖机正在下该品种");
                                return;
                            }
                        }
                    }

                    //第一车库存距离轨道头部有5个车的距离
                    int discount = GetPointCompareCount((track.is_take_forward ? track.limit_point : track.limit_point_up), topstock.location, safe);
                    if (discount >= TrackSortFrontCount)
                    {
                        SetTrackSortable(track, true, SORT_LEVEL_2, "轨道头部有空间");
                        return;
                    }
                    else if (GlobalWcsDataConfig.BigConifg.TrackSortMid
                        && PubMaster.Goods.ExistCountEmptySpace(track.id, TrackSortMidCount, safe, out string rs))
                    {
                        SetTrackSortable(track, true, SORT_LEVEL_2, string.Format("中间存在[ {0} ]车的空位,备注[ {1} ]", TrackSortMidCount, rs));
                        return;
                    }
                    else
                    {
                        SetTrackSortable(track, false, SORT_LEVEL_NO, string.Format("不满足倒库, 头部空位[ {0} ], 头部设置空位[ {1} ]", discount, TrackSortFrontCount));
                        return;
                    }
                }
                else
                {
                    SetTrackSortable(track, false, SORT_LEVEL_NO, "有砖机正在上该品种");
                    return;
                }
            }
        }

        /// <summary>
        /// 指定空间内可存放车数
        /// </summary>
        /// <param name="loc1"></param>
        /// <param name="loc2"></param>
        /// <param name="carspace"></param>
        /// <returns></returns>
        public int GetPointCompareCount(uint loc1, uint loc2, ushort carspace)
        {
            return (int)Math.Abs(loc1 - loc2) / carspace;
        }
        #endregion
    }

    class TrackDis
    {
        public uint trackid { set; get; }
        public long dis { set; get; }
    }
}