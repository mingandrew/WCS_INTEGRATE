using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.goods;
using module.tiletrack;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task.device;
using tool.appconfig;

namespace task.trans
{

    /// <summary>
    /// 根据交易信息来调度整个系统
    /// </summary>
    public class TransMaster : TransBase
    {
        #region[构造函数]
        public TransMaster() : base()
        {
            InitDiagnore(this);
        }

        #endregion

        #region[检查满轨/添加倒库任务]

        /// <summary>
        /// 检查满砖轨道进行倒库
        /// 1.检查入库满砖轨道
        /// 2.生成倒库任务
        /// </summary>
        public override void CheckTrackSort()
        {
            /** 触发条件
             * 1. 获取可倒库的入库轨道
             *      1.1 优先砖机当前上砖品种
             *      1.2 优先砖机当前预约品种
             *      1.3 按入库时间最早
             * 2. 分配空砖出库轨道
             * 3. 生成任务
             */

            // 获取可倒库的入库轨道
            List<Track> inTracks = PubMaster.Track.GetFullInTrackList();


            #region old
            List<Track> tracks = PubMaster.Track.GetFullInTrackList();
            foreach (Track track in tracks)
            {
                if (!PubMaster.Area.IsLineSortOnoff(track.area, track.line)) continue;

                if (!PubMaster.Track.IsTrackEmtpy(track.brother_track_id)) continue;

                int count = GetAreaSortTaskCount(track.area, track.line);
                if (PubMaster.Area.IsSortTaskLimit(track.area, track.line, count)) continue;

                //同时判断入库
                //不判断出轨道，出现回轨分配出轨道，如果限制出轨道（回轨空轨道），很可能会进行了下一个轨道
                //但是分配车时判断出轨道是否有任务
                if (TransList.Exists(c => !c.finish && c.InTrack(track.id)))// || c.InTrack(track.brother_track_id)
                {
                    continue;
                }

                uint goodsid = PubMaster.Goods.GetGoodsId(track.id);

                if (goodsid != 0)
                {
                    if (!PubMaster.Goods.IsTrackOkForGoods(track.brother_track_id, goodsid))
                    {
                        continue;
                    }

                    uint stockid = PubMaster.Goods.GetTrackStockId(track.id);
                    if (stockid == 0) continue;
                    uint tileid = PubMaster.Goods.GetStockTileId(stockid);

                    uint tileareaid = PubMaster.Area.GetAreaDevAreaId(tileid);

                    if (!PubMaster.Track.IsEarlyFullTimeOver(track.id))
                    {
                        continue;
                    }

                    PubMaster.Track.SetTrackEaryFull(track.id, false, null);

                    AddTransWithoutLock(tileareaid > 0 ? tileareaid : track.area, 0, TransTypeE.倒库任务, goodsid, stockid, track.id, track.brother_track_id
                        , TransStatusE.检查轨道, 0, track.line);
                }
            }
            #endregion
        }

        /// <summary>
        /// 获取倒库数量
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetAreaSortTaskCount(uint area, ushort line)
        {
            return TransList.Count(c => !c.finish
                                        && c.area_id == area
                                        && c.line == line
                                        && c.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库));
        }

        /// <summary>
        /// 获取非等待倒库任务数量
        /// </summary>
        /// <param name="area"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetSortTaskNotWaitCount(uint area, ushort line)
        {
            return TransList.Count(c => !c.finish
                                        && c.area_id == area
                                        && c.line == line
                                        && c.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库)
                                        && c.NotInStatus(TransStatusE.倒库暂停));
        }

        #endregion

        #region[已不用]--[检查轨道/添加上砖侧倒库任务]
        /// <summary>
        /// 不用的原因如下：可能已接力倒库已生成，但运输车还没去take_track，上砖车就回轨了，停在摆渡车上，卡住了接力倒库的分配摆渡车
        /// 检查上砖轨道进行倒库
        /// 1.检查入库满砖轨道
        /// 2.生成倒库任务
        /// </summary>
        //public override void CheckUpTrackSort()
        //{
        //    if (!PubMaster.Track.IsUpSplit()) return;
        //
        //    if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)) return;
        //
        //    List<Track> tracks = PubMaster.Track.GetUpSortTrack();
        //    foreach (Track track in tracks)
        //    {
        //        if (!PubMaster.Dic.IsAreaTaskOnoff(track.area, DicAreaTaskE.上砖)) continue;
        //
        //        int count = GetAreaSortTaskCount(track.area, track.line);
        //        if (PubMaster.Area.IsSortTaskLimit(track.area, track.line, count)) continue;
        //
        //        if (TransList.Exists(c => !c.finish && (c.take_track_id == track.id
        //                                || c.give_track_id == track.id
        //                                || c.finish_track_id == track.id)))
        //        {
        //            continue;
        //        }
        //
        //        uint goodsid = PubMaster.Goods.GetGoodsId(track.id);
        //
        //        if (goodsid != 0)
        //        {
        //            uint stockid = PubMaster.Goods.GetTrackStockId(track.id);
        //            if (stockid == 0) continue;
        //            uint tileid = PubMaster.Goods.GetStockTileId(stockid);
        //
        //            uint tileareaid = PubMaster.Area.GetAreaDevAreaId(tileid);
        //
        //            if (!PubMaster.Track.IsEarlyFullTimeOver(track.id))
        //            {
        //                continue;
        //            }
        //
        //            AddTransWithoutLock(tileareaid > 0 ? tileareaid : track.area, 0, TransTypeE.上砖侧倒库, goodsid, stockid, track.id, track.id
        //                , TransStatusE.检查轨道, 0, track.line);
        //        }
        //    }
        //}
        #endregion

        #region[添加移车任务]
        public void AddMoveCarrierTask(uint trackid, uint carrierid, TrackTypeE totracktype, MoveTypeE movetype, DeviceTypeE ferrytype = DeviceTypeE.其他)
        {
            if (HaveCarrierInTrans(carrierid)) return;

            Track track = PubMaster.Track.GetTrack(trackid);
            if (PubTask.Carrier.IsCarrierFree(carrierid))
            {
                uint givetrackid = 0;
                switch (movetype)
                {
                    case MoveTypeE.转移占用轨道://优先到空轨道

                        // 优先移动到空轨道
                        //List<uint> trackids = PubMaster.Area.GetAreaTrackIds(track.area, totracktype);
                        List<uint> trackids = PubMaster.Track.GetAreaSortOutTrack(track.area, track.line, totracktype);

                        List<uint> tids = PubMaster.Track.SortTrackIdsWithOrder(trackids, trackid, track.order);

                        //能去这个取货/卸货轨道的所有配置的摆渡车信息
                        List<uint> ferryids;
                        if (ferrytype != DeviceTypeE.其他)
                        {
                            //如果指定摆渡车类型则查找选定的摆渡车类型
                            ferryids = PubMaster.Area.GetWithTracksFerryIds(ferrytype, trackid);
                        }
                        else
                        {
                            ferryids = PubMaster.Area.GetWithTracksFerryIds(trackid);
                        }
                        ferryids = PubTask.Ferry.GetWorkingAndEnable(ferryids);

                        foreach (uint t in tids)
                        {
                            if (!IsTraInTrans(t)
                                && !PubTask.Carrier.HaveInTrack(t, carrierid)
                                && PubMaster.Area.ExistFerryWithTrack(ferryids, t))
                            {
                                givetrackid = t;
                                break;
                            }
                        }
                        break;
                    case MoveTypeE.释放摆渡车:
                        break;
                    case MoveTypeE.离开砖机轨道:
                        break;
                    case MoveTypeE.切换区域:
                        break;
                }

                if (givetrackid != 0)
                {
                    AddTransWithoutLock(track.area, 0, TransTypeE.移车任务, 0, 0, trackid, givetrackid, TransStatusE.移车中, carrierid, track.line, ferrytype);
                }
            }
        }
        #endregion

        #region[添加手动任务]

        public bool AddManualTrans(ushort area, ushort line, uint devid, TransTypeE transtype, uint goods_id, uint taketrackid, uint givetrackid, TransStatusE transtatus, out string result)
        {
            result = "";
            if (transtype == TransTypeE.手动下砖)
            {
                if (HaveInTileTrack(taketrackid))
                {
                    result = "已经有该砖机轨道的任务";
                    return false;
                }

                //[已有库存]
                if (!PubMaster.Goods.HaveStockInTrack(taketrackid, goods_id, out uint stockid))
                {
                    byte fullqty = PubTask.TileLifter.GetTileFullQty(devid, goods_id);
                    ////[生成库存]
                    stockid = PubMaster.Goods.AddStock(devid, taketrackid, goods_id, fullqty);
                    if (stockid > 0)
                    {
                        PubMaster.Track.UpdateStockStatus(taketrackid, TrackStockStatusE.有砖, "手动任务");
                        PubMaster.Goods.AddStockInLog(stockid);
                    }
                }

                if (givetrackid != 0)
                {
                    if (!PubMaster.Goods.IsTrackOkForGoods(givetrackid, goods_id))
                    {
                        result = "该轨道放置该品种会碰撞！";
                        return false;
                    }

                    if (!PubMaster.Track.IsEmtpy(givetrackid)
                        && !PubMaster.Goods.ExistStockInTrack(givetrackid, goods_id))
                    {
                        if (PubMaster.Track.HaveEmptyTrackInTile(area, devid))
                        {
                            result = "还有空轨道，请不要混放";
                            return false;
                        }
                    }
                }

                //分配放货点
                if (stockid != 0
                    && givetrackid == 0
                    && PubMaster.Goods.AllocateGiveTrack(area, line, devid, goods_id, out List<uint> traids))
                {
                    foreach (uint traid in traids)
                    {
                        if (!PubTask.Trans.IsTraInTransWithLock(traid))
                        {
                            givetrackid = traid;
                            break;
                        }
                    }

                    if (givetrackid != 0)
                    {
                        PubMaster.Track.UpdateRecentGood(givetrackid, goods_id);
                        PubMaster.Track.UpdateRecentTile(givetrackid, devid);
                        //生成手动入库任务
                        AddTrans(area, devid, transtype, goods_id, stockid, taketrackid, givetrackid);
                        return true;
                    }
                }
            }

            if (transtype == TransTypeE.手动上砖)
            {
                if (taketrackid != 0)
                {
                    if (PubMaster.Track.IsEmtpy(taketrackid))
                    {
                        result = "轨道已经空！";
                        return false;
                    }

                    if (!PubMaster.Track.IsRecentGoodId(taketrackid, goods_id)
                        && !PubMaster.Goods.ExistStockInTrack(taketrackid, goods_id))
                    {
                        result = "该轨道中没有砖机需要的品种！";
                        return false;
                    }

                    if (!HaveInTileTrack(taketrackid))
                    {
                        uint stockid = PubMaster.Goods.GetTrackTopStockId(taketrackid);
                        //有库存但是不是砖机需要的品种
                        if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goods_id))
                        {
                            PubMaster.Track.UpdateRecentTile(taketrackid, 0);
                            PubMaster.Track.UpdateRecentGood(taketrackid, 0);
                            result = "请再次尝试添加";
                            return false;
                        }
                        //生成出库交易
                        AddTrans(area, devid, transtype, goods_id, stockid, taketrackid, givetrackid);
                        //PubMaster.Goods.AddStockOutLog(stockid, givetrackid, devid);

                        return true;
                    }
                    else
                    {
                        result = "取货轨道已有任务";
                        return false;
                    }
                }

                //1.查看是否有需要重新派发取货的空轨道
                if (PubMaster.Track.HaveTrackInGoodButNotStock(area, devid, goods_id, out List<uint> trackids))
                {
                    foreach (var trackid in trackids)
                    {
                        if (!HaveInTileTrack(trackid))
                        {
                            uint stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
                            //有库存但是不是砖机需要的品种
                            if (stockid != 0 && !PubMaster.Goods.IsStockWithGood(stockid, goods_id))
                            {
                                PubMaster.Track.UpdateRecentTile(trackid, 0);
                                PubMaster.Track.UpdateRecentGood(trackid, 0);
                                result = "请再次尝试添加";
                                return false;
                            }
                            //生成出库交易
                            AddTrans(area, devid, transtype, goods_id, stockid, trackid, givetrackid);
                            //PubMaster.Goods.AddStockOutLog(stockid, givetrackid, devid);
                            return true;
                        }
                    }

                }
                //分配库存
                else if (PubMaster.Goods.GetStock(area, line, devid, goods_id, out List<Stock> allocatestocks))
                {
                    foreach (Stock stock in allocatestocks)
                    {
                        if (!IsStockInTrans(stock.id, stock.track_id))
                        {
                            PubMaster.Track.UpdateRecentGood(stock.track_id, goods_id);
                            PubMaster.Track.UpdateRecentTile(stock.track_id, devid);
                            //生成出库交易
                            AddTrans(area, devid, transtype, goods_id, stock.id, stock.track_id, givetrackid);

                            //PubMaster.Goods.AddStockOutLog(stock.id, givetrackid, devid);
                            break;
                        }
                    }
                }
                else
                {
                    result = "找不到库存";
                }
            }
            return false;
        }

        #endregion

        #region[根据小车位置分配摆渡车]

        /// <summary>
        /// 分配摆渡车
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="track"></param>
        public string AllocateFerry(StockTrans trans, DeviceTypeE ferrytype, Track track, bool allotogiveferry)
        {
            uint ferryid = 0;
            string result = "";
            switch (track.Type)
            {
                #region[下砖区轨道]
                case TrackTypeE.下砖轨道://小车在下砖机轨道上(前往下砖机取砖中)
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.give_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;

                case TrackTypeE.储砖_入://小车在储砖轨道上(准备上摆渡车)
                    //调度摆渡车
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.take_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;

                case TrackTypeE.后置摆渡轨道://小车在摆渡车上(已经在摆渡车上)

                    uint tferryid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(track.id);
                    if (tferryid != 0)
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, tferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, tferryid);
                        }
                    }
                    break;

                #endregion

                #region[上砖区轨道]
                case TrackTypeE.上砖轨道:
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.give_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;
                case TrackTypeE.前置摆渡轨道:
                    uint outtferryid = PubMaster.DevConfig.GetFerryIdByFerryTrackId(track.id);
                    if (outtferryid != 0)
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, outtferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, outtferryid);
                        }
                    }
                    break;
                case TrackTypeE.储砖_出:
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.take_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;
                #endregion

                case TrackTypeE.储砖_出入:
                    if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.take_track_id, out ferryid, out result))
                    {
                        if (allotogiveferry)
                        {
                            SetGiveFerry(trans, ferryid);
                        }
                        else
                        {
                            SetTakeFerry(trans, ferryid);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (ferryid != 0)
            {
                PubMaster.Warn.RemoveTaskWarn(WarningTypeE.FailAllocateFerry, trans.id);
            }
            else if (ferryid == 0 && mTimer.IsOver(TimerTag.FailAllocateFerry, trans.take_track_id, 10, 5))
            {
                result = string.Format("{0},{1}货摆渡车", result, allotogiveferry ? "卸" : "取");
                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.FailAllocateFerry, (ushort)trans.tilelifter_id, trans.id, result);
            }

            return result;
        }

        /// <summary>
        /// 分配摆渡车给倒库
        /// </summary>
        /// <param name="trans"></param>
        public string AllocateFerryToCarrierSort(StockTrans trans, DeviceTypeE ferrytype)
        {
            if (PubTask.Ferry.AllocateFerry(trans, ferrytype, trans.give_track_id, out uint ferryid, out string result))
            {
                SetTakeFerry(trans, ferryid);
            }
            return result;
        }

        #endregion

        #region[其他判断方法]

        /// <summary>
        /// 库存是否被任务占用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool IsStockInTrans(uint id, out string rs)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    rs = "库存在任务中不能删除！";
                    return TransList.Exists(c => c.stock_id == id);
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            rs = "稍后再试！";
            return true;
        }

        /// <summary>
        /// 判断轨道是否有被占用
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        internal bool IsTraInTransWithLock(uint traid)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    return IsTraInTrans(traid);
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        /// <summary>
        /// 判断轨道是否有被占用
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        internal bool IsTraInTrans(uint traid)
        {
            return TransList.Exists(c => !c.finish && c.InTrack(traid));
        }

        /// <summary>
        /// 判断库存，作业轨道是否已经被任务占用
        /// </summary>
        /// <param name="stockid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool IsStockInTrans(uint stockid, uint trackid)
        {
            try
            {
                return TransList.Exists(c => !c.finish
                        && (c.stock_id == stockid
                        || c.take_track_id == trackid
                        || c.give_track_id == trackid));
            }
            catch (Exception)
            {

            }
            return true;
        }

        /// <summary>
        /// 判断库存，作业轨道是否已经被任务占用[但是忽略倒库任务]
        /// </summary>
        /// <param name="stockid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool IsStockInTransButSortTask(uint stockid, uint trackid, params TransTypeE[] types)
        {
            try
            {
                //是否开启【接力倒库轨道可以同时上砖】
                bool ignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

                //是否开启【出入倒库轨道可以同时上砖】
                bool inoutignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask);

                return TransList.Exists(c => !c.finish
                            && (c.stock_id == stockid || (c.InTrack(trackid) && c.NotInType(types)))
                            && (!ignoresort || c.NotInType(TransTypeE.上砖侧倒库))
                            && (!inoutignoresort || c.NotInType(TransTypeE.倒库任务)));
            }
            catch (Exception)
            {

            }
            return true;
        }

        public bool HaveGiveInTrackId(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && c.give_track_id == trans.give_track_id);
        }

        public bool HaveTaskInTrackId(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (c.take_track_id == trans.take_track_id || c.take_track_id == trans.give_track_id));
        }

        /// <summary>
        /// 判断任务占用轨道
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool HaveTaskInTrackButSort(StockTrans trans)
        {
            //是否开启【接力倒库轨道可以同时上砖】
            bool ignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

            //是否开启【出入倒库轨道可以同时上砖】
            bool inoutignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask);

            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && c.InTrack(trans.take_track_id, trans.give_track_id)
                                    && (!ignoresort
                                            || !(c.InType(TransTypeE.上砖侧倒库) && c.InStatus(TransStatusE.倒库中, TransStatusE.接力等待))
                                            || c.NotInType(TransTypeE.上砖侧倒库))
                                    && (!inoutignoresort
                                            || !(c.InType(TransTypeE.倒库任务) && c.InStatus(TransStatusE.倒库中, TransStatusE.接力等待))
                                            || c.NotInType(TransTypeE.倒库任务)));
        }

        /// <summary>
        /// 是否存在同卸货点的交易，如果有则等待该任务完成后，重新派送该车做新的任务
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool HaveTaskSortTrackId(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && !c.finish
                                    && c.TransStaus != TransStatusE.完成
                                    && c.InTrack(trans.take_track_id, trans.give_track_id));
        }

        public bool HaveCarrierInTrans(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && c.carrier_id == trans.carrier_id);
        }

        public bool HaveCarrierInTrans(uint carrrierid)
        {
            return TransList.Exists(c => c.TransStaus != TransStatusE.完成 && c.carrier_id == carrrierid);
        }

        private bool HaveTakeFerryInTrans(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (c.take_ferry_id == trans.take_ferry_id || c.give_ferry_id == trans.take_ferry_id));
        }

        private bool HaveGiveFerryInTrans(StockTrans trans)
        {
            return TransList.Exists(c => c.id != trans.id
                                    && c.TransStaus != TransStatusE.完成
                                    && (c.take_ferry_id == trans.give_ferry_id || c.give_ferry_id == trans.give_ferry_id));
        }

        /// <summary>
        /// 判断运输车是否在使用中
        /// </summary>
        /// <param name="carrierid"></param>
        /// <returns></returns>
        internal bool HaveInCarrier(uint carrierid)
        {
            return TransList.Exists(c => c.TransStaus != TransStatusE.完成 && c.carrier_id == carrierid);
        }

        /// <summary>
        /// 是否允许继续分配运输车
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowToHaveCarTask(uint area, ushort line, TransTypeE tt)
        {
            int count = TransList.Count(c => !c.finish && c.area_id == area && c.line == line && c.TransType == tt && c.carrier_id > 0);
            switch (tt)
            {
                case TransTypeE.手动下砖:
                case TransTypeE.下砖任务:
                case TransTypeE.同向下砖:
                    return !PubMaster.Area.IsDownTaskLimit(area, line, count);
                case TransTypeE.手动上砖:
                case TransTypeE.上砖任务:
                case TransTypeE.同向上砖:
                    return !PubMaster.Area.IsUpTaskLimit(area, line, count);
            }
            return !PubMaster.Area.IsSortTaskLimit(area, line, count);
        }

        /// <summary>
        /// 是否存在指定类型未分摆渡车任务
        /// </summary>
        /// <param name="area"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        internal bool IsExistsTask(uint area, TransTypeE tt)
        {
            return TransList.Exists(c => !c.finish && c.area_id == area && c.TransType == tt && (c.take_ferry_id == 0 || c.give_ferry_id == 0));
        }

        /// <summary>
        /// 是否存在区域线路类型的任务
        /// </summary>
        /// <param name="area">区域ID</param>
        /// <param name="line">线路ID</param>
        /// <param name="types">任务类型</param>
        /// <returns></returns>
        public bool ExistAreaLineType(uint area, uint line, params TransTypeE[] types)
        {
            return TransList.Exists(c => c.area_id == area && c.line == line && c.InType(types));
        }

        /// <summary>
        /// 流程运行前提判断（code-50~79）
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool RunPremise(StockTrans trans, out Track track)
        {
            //小车当前所在的轨道有数据
            track = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
            if (track == null)
            {
                #region 【任务步骤记录】
                SetStepLog(trans, false, 50, string.Format("运输车[ {0} ]当前位置信息异常，等待[ {0} ]恢复；",
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion
                return false;
            }

            //小车没有被其他任务占用
            if (HaveCarrierInTrans(trans))
            {
                #region 【任务步骤记录】
                SetStepLog(trans, false, 51, string.Format("有其他任务锁定了运输车[ {0} ]，等待[ {0} ]空闲；",
                    PubMaster.Device.GetDeviceName(trans.carrier_id)));
                #endregion
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判断是否需要加入流程超时报警
        /// 1.超时则添加报警Warning36
        /// 2.没超时则删除报警
        /// </summary>
        /// <param name="trans"></param>
        public void CheckAndAddTransStatusOverTimeWarn(StockTrans trans)
        {
            int overtime = PubMaster.Dic.GetDtlIntCode(DicTag.StepOverTime);
            // 倒库中的流程超时2小时，才报警
            if (trans.TransStaus == TransStatusE.倒库中)
            {
                overtime = PubMaster.Dic.GetDtlIntCode(DicTag.SortingStockStepOverTime);
                if (trans.IsInStatusOverTime(trans.TransStaus, overtime))
                {
                    PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.Warning36, 0, trans.id,
                        string.Format("{0}[ {1} ]-[ {2} ]流程已超过2小时，请检查任务相关的设备是否正常", trans.TransType, trans.id, trans.TransStaus));
                    return;
                }
            }
            //流程超过10分钟，就报警
            else if (trans.IsInStatusOverTime(trans.TransStaus, overtime))
            {
                PubMaster.Warn.AddTaskWarn(trans.area_id, trans.line, WarningTypeE.Warning36, 0, trans.id,
                    string.Format("{0}[ {1} ]-[ {2} ]流程已超过10分钟，请检查任务相关的设备是否正常", trans.TransType, trans.id, trans.TransStaus));
                return;
            }
            PubMaster.Warn.RemoveTaskWarn(WarningTypeE.Warning36, trans.id);
        }
        #endregion

        #region[更新界面数据]

        /// <summary>
        /// 通过任务ID更新任务界面
        /// </summary>
        /// <param name="transid"></param>
        public void ClueViewByTransID(uint transid)
        {
            StockTrans trans = GetTrans(transid);
            if (trans != null)
            {
                SendMsg(trans);
            }
        }

        protected override void SendMsg(StockTrans trans)
        {
            mMsg.o1 = trans;
            Messenger.Default.Send(mMsg, MsgToken.TransUpdate);
        }

        /// <summary>
        /// 获取所有交易信息
        /// </summary>
        public void GetAllTrans()
        {
            if (Monitor.TryEnter(_for, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    foreach (StockTrans trans in TransList)
                    {
                        SendMsg(trans);
                    }
                }
                finally
                {
                    Monitor.Exit(_for);
                }
            }
        }

        #endregion

        #region[取消任务]

        public bool CancelTask(uint transid, out string result)
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    StockTrans trans = TransList.Find(c => c.id == transid);
                    if (trans != null)
                    {
                        if (trans.TransStaus == TransStatusE.完成)
                        {
                            result = "任务已经完成";
                            return false;
                        }
                        if (trans.TransStaus == TransStatusE.取消)
                        {
                            result = "已经在取消中";
                            return false;
                        }
                        switch (trans.TransType)
                        {
                            case TransTypeE.下砖任务:
                            case TransTypeE.手动下砖:
                            case TransTypeE.同向下砖:
                                switch (trans.TransStaus)
                                {
                                    case TransStatusE.调度设备:
                                        if (trans.carrier_id == 0)
                                        {
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                            return true;
                                        }
                                        break;
                                    case TransStatusE.取砖流程:
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                                        if (PubTask.Carrier.IsNotLoad(trans.carrier_id)
                                            && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.取砖指令)
                                            && nowtrack.NotInType(TrackTypeE.下砖轨道, TrackTypeE.上砖轨道))
                                        {
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                            return true;
                                        }
                                        else
                                        {
                                            result = "小车正在取砖不能取消！";
                                        }
                                        break;
                                    case TransStatusE.放砖流程:
                                        result = "进入放砖流程，不能取消！";
                                        break;
                                }

                                break;
                            case TransTypeE.上砖任务:
                            case TransTypeE.手动上砖:
                            case TransTypeE.同向上砖:
                                switch (trans.TransStaus)
                                {
                                    case TransStatusE.调度设备:
                                        if (trans.carrier_id == 0)
                                        {
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                            return true;
                                        }
                                        break;
                                    case TransStatusE.取砖流程:
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(trans.carrier_id);
                                        if (PubTask.Carrier.IsNotLoad(trans.carrier_id)
                                            && !PubTask.Carrier.IsCarrierInTask(trans.carrier_id, DevCarrierOrderE.放砖指令)
                                            && nowtrack.NotInType(TrackTypeE.下砖轨道, TrackTypeE.上砖轨道))
                                        {
                                            SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                            return true;
                                        }
                                        else
                                        {
                                            result = "小车正在上砖！";
                                        }
                                        break;
                                    case TransStatusE.还车回轨:
                                        result = "正在调度小车回轨道";
                                        break;
                                }
                                break;
                            case TransTypeE.倒库任务:
                            case TransTypeE.上砖侧倒库:
                                SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                return true;
                            case TransTypeE.移车任务:
                                SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                return true;
                            case TransTypeE.其他:
                                SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                break;
                            default:
                                SetStatus(trans, TransStatusE.取消, "手动取消任务");
                                break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            result = "";
            return false;
        }

        /// <summary>
        /// 切换模式取消任务
        /// </summary>
        /// <param name="tilelifterid"></param>
        /// <param name="goodsid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CancelTaskForCutover(uint tilelifterid, uint goodsid, out string result)
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    List<StockTrans> trans = TransList.FindAll(c => !c.finish && !c.cancel && c.tilelifter_id == tilelifterid);
                    if (trans != null && trans.Count != 0)
                    {
                        foreach (StockTrans t in trans)
                        {
                            switch (t.TransType)
                            {
                                case TransTypeE.下砖任务:
                                case TransTypeE.手动下砖:
                                case TransTypeE.同向下砖:
                                    if (PubTask.Carrier.IsLoad(t.carrier_id))
                                    {
                                        result = "运输车已取砖，不能取消任务！";
                                        return false;
                                    }

                                    if (t.goods_id == goodsid)
                                    {
                                        SetStatus(t, TransStatusE.取消, "切换砖机模式取消任务");
                                    }
                                    break;

                                case TransTypeE.上砖任务:
                                case TransTypeE.手动上砖:
                                case TransTypeE.同向上砖:
                                    if (t.TransStaus == TransStatusE.取砖流程)
                                    {
                                        Track nowtrack = PubTask.Carrier.GetCarrierTrack(t.carrier_id);
                                        if (PubTask.Carrier.IsLoad(t.carrier_id)
                                            && (PubTask.Carrier.IsCarrierInTask(t.carrier_id, DevCarrierOrderE.放砖指令))
                                            && nowtrack.InType(TrackTypeE.后置摆渡轨道, TrackTypeE.前置摆渡轨道, TrackTypeE.上砖轨道))
                                        {
                                            result = "运输车正在上砖，不能取消任务！";
                                            return false;
                                        }
                                    }

                                    SetStatus(t, TransStatusE.取消, "切换砖机模式取消任务");
                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        #endregion

        #region[强制完成任务]
        public bool ForseFinish(uint id, out string result, string memo = "")
        {
            result = "";
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    StockTrans trans = TransList.Find(c => c.id == id);
                    if (trans != null)
                    {
                        if (trans.InType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库))
                        {
                            result = "倒库任务不可以强制完成，建议执行取消任务操作";
                            return false;
                        }

                        SetStatus(trans, TransStatusE.完成, memo);
                        return true;
                    }
                    else
                    {
                        result = "找不到任务";
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }

            result = "";
            return false;
        }
        #endregion

        #region[摆渡车锁定定位]

        /// <summary>
        /// 锁定摆渡车并且定位摆渡车
        /// </summary>
        /// <param name="trans">交易</param>
        /// <param name="ferryid">摆渡车ID</param>
        /// <param name="locatetrackid">摆渡车定位站点</param>
        /// <param name="carriertrackid">小车所在站点</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool LockFerryAndAction(StockTrans trans, uint ferryid, uint locatetrackid, uint carriertrackid,
            out uint ferryTraid, out string result, bool isnotload = false)
        {
            result = "";
            ferryTraid = PubTask.Ferry.GetFerryTrackId(ferryid);

            if (ferryid == 0) return false;

            if (ferryid != 0 && isnotload && PubTask.Ferry.IsLoad(ferryid))
            {
                result = "摆渡车载货状态未满足";
                return false;
            }

            //找摆渡车上的运输车
            //判断运输车是否在动
            //在动则返回false，不给摆渡车发任务
            if (PubTask.Ferry.IsLoad(ferryid))
            {
                //在摆渡车轨道上的运输车是否有状态不是停止的或者是手动的
                if (PubTask.Carrier.HaveTaskForFerry(ferryTraid))
                {
                    result = "存在运输车上下摆渡中";
                    return false;
                }
            }

            return ferryid != 0
                && PubTask.Ferry.TryLock(trans, ferryid, carriertrackid)
                && PubTask.Ferry.DoLocateFerry(trans.id, ferryid, locatetrackid, out result);
            //&& PubTask.Carrier.IsStopFTask(trans.carrier_id, track); 移出单独判断，考虑无缝上摆渡，不卡运输车
        }

        #endregion

        #region[开关联动-取消对应的任务]
        private void StopAreaTask(uint areaid, ushort lineid, TransTypeE[] types)
        {
            if (Monitor.TryEnter(_for, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    List<StockTrans> trans = TransList.FindAll(c => !c.finish
                                                && c.area_id == areaid
                                                && c.line == lineid
                                                && types.Contains(c.TransType));
                    if (trans != null)
                    {
                        foreach (StockTrans item in trans)
                        {
                            try
                            {
                                SetStatus(item, TransStatusE.完成, "平板任务开关-清除任务");
                                //    if (item.carrier_id > 0)
                                //    {
                                //        try
                                //        {
                                //            PubTask.Carrier.DoOrder(item.carrier_id, new CarrierActionOrder()
                                //            {
                                //                Order = DevCarrierOrderE.终止指令
                                //            }, "【平板任务开关】- 终止小车");

                                //        }
                                //        catch (Exception e)
                                //        {
                                //            Console.WriteLine(e.StackTrace);
                                //        }
                                //    }
                                //    if (item.take_ferry_id > 0)
                                //    {
                                //        try
                                //        {
                                //            PubTask.Ferry.StopFerry(item.take_ferry_id, "【平板任务开关】- 终止T摆渡车", "逻辑", out string result);
                                //        }
                                //        catch (Exception e)
                                //        {
                                //            Console.WriteLine(e.StackTrace);
                                //        }
                                //    }

                                //    if (item.give_ferry_id > 0)
                                //    {
                                //        try
                                //        {
                                //            PubTask.Ferry.StopFerry(item.give_ferry_id, "【平板任务开关】- 终止G摆渡车", "逻辑", out string result);
                                //        }
                                //        catch (Exception e)
                                //        {
                                //            Console.WriteLine(e.StackTrace);
                                //        }
                                //    }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }

                            //if (item.TransType == TransTypeE.倒库任务)
                            //{

                            //}
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_for);
                }
            }
        }

        internal void StopAreaUp(uint areaid, ushort lineid)
        {
            StopAreaTask(areaid, lineid, new TransTypeE[] { TransTypeE.上砖任务, TransTypeE.手动上砖, TransTypeE.同向上砖 });
        }

        internal void StopAreaDown(uint areaid, ushort lineid)
        {
            StopAreaTask(areaid, lineid, new TransTypeE[] { TransTypeE.下砖任务, TransTypeE.手动下砖, TransTypeE.同向下砖 });
        }

        internal void StopAreaSort(uint areaid, ushort lineid)
        {
            StopAreaTask(areaid, lineid, new TransTypeE[] { TransTypeE.倒库任务 });
        }

        #endregion

        #region[极限混砖]

        /// <summary>
        /// 判断是否存在同任务类型的并使用相同轨道的任务
        /// </summary>
        /// <param name="traid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsTrasInTransWithType(uint traid, params TransTypeE[] types)
        {
            return TransList.Exists(c => !c.finish && types.Contains(c.TransType) && c.InTrack(traid));
        }

        /// <summary>
        /// 判断轨道是否能发倒库任务
        /// 1.是否有下砖、上砖任务
        /// </summary>
        /// <param name="givetrackid"></param>
        /// <param name="taketrackid"></param>
        /// <param name="devid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool CheckTrackCanDoSort(uint givetrackid, uint taketrackid, uint devid, out string result)
        {
            StockTrans trans = TransList.Find(c => (c.TransType == TransTypeE.上砖侧倒库 || c.TransType == TransTypeE.倒库任务) && c.take_track_id == taketrackid
                                && c.give_track_id == givetrackid);
            if (trans != null)
            {
                if (trans.carrier_id > 0 && trans.carrier_id != devid)
                {
                    result = "非当前轨道任务分配的小车【" + PubMaster.Device.GetDeviceName(trans.carrier_id) + "】";
                    return false;
                }
                result = "";
                return true;
            }

            List<StockTrans> othertrans = TransList.FindAll(c => c.NotInType(TransTypeE.上砖侧倒库, TransTypeE.倒库任务)
                                                                && c.InTrack(givetrackid, taketrackid));

            if (othertrans.Count > 0)
            {
                foreach (var item in othertrans)
                {
                    result = string.Format("有任务[ {0} ], 不能执行倒库指令", item.TransType);
                    return false;
                }
            }

            //允许倒库的时候上砖
            //是否开启【出入倒库轨道可以同时上砖】
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask)
                || PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask))
            {
                //但前面不能允许有运输车
                if (PubTask.Carrier.ExistCarBehind(devid, givetrackid, out uint otherid))
                {
                    result = string.Format("倒库轨道有其他车[ {0} ]，不能执行倒库指令", PubMaster.Device.GetDeviceName(otherid));
                    return false;
                }
            }
            else
            { 
                //倒库的时候不允许上砖
                if (PubTask.Carrier.HaveInTrackButCarrier(taketrackid, givetrackid, devid, out uint othercarid))
                {
                    result = string.Format("倒库轨道有其他车[ {0} ]，不能执行倒库指令", PubMaster.Device.GetDeviceName(othercarid));
                    return false;
                }
            }

            result = "";
            return true;
        }

        /// <summary>
        /// 判断是否需要在库存在上砖分割点后，是否需要发送倒库任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public bool CheckTopStockAndSendSortTask(uint tranid, uint carrier_id, uint track_id)
        {
            //1.打开使用-开关(使用上砖侧分割点坐标)
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)) return false;

            //2.判断是否是出轨道
            Track track = PubMaster.Track.GetTrack(track_id);
            if (track == null || track.Type != TrackTypeE.储砖_出) return false;

            //3.如果已经有倒库任务则不发了
            if (TransList.Exists(c => !c.finish && c.take_track_id == track_id && (c.TransType == TransTypeE.倒库任务 || c.TransType == TransTypeE.上砖侧倒库)))
            {
                return false;
            }

            //4.判断轨道中是否已经有其他车在倒库了
            if (PubTask.Carrier.CheckHaveCarInTrack(TransTypeE.上砖侧倒库, track_id, carrier_id, out string result)) return false;

            //5.轨道的头部库存位置处于分割点后（分割点后不止一个库存）则给小车发送倒库任务
            if (PubMaster.Goods.IsTopStockBehindUpSplitPoint(track_id, out uint stockid)
                && !PubMaster.Goods.IsOnlyOneWithStock(stockid))
            {

                byte movecount = (byte)PubMaster.Goods.GetBehindPointStockCount(track.id, track.up_split_point);
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UpSortUseMaxNumber))
                {
                    byte line_max_move = PubMaster.Area.GetLineUpSortMaxNumber(track.area, track.line);

                    if (line_max_move > 0 && movecount > line_max_move)
                    {
                        movecount = line_max_move;
                    }
                }
                ushort stockqty = PubMaster.Goods.GetTrackStockCount(track.id);
                //后退至轨道倒库
                PubTask.Carrier.DoOrder(carrier_id, tranid, new CarrierActionOrder()
                {
                    Order = DevCarrierOrderE.往前倒库,
                    CheckTra = track.ferry_down_code,
                    ToPoint = (ushort)(track.split_point + 50), //倒库时，不能超过脉冲(出库轨道附件脉冲位置)
                    MoveCount = movecount,
                    ToTrackId = track.id
                }, string.Format("轨道有库存[ {0} ], 接力数量[ {1} ], 接力脉冲[ {2} ]", stockqty, movecount, track.up_split_point));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断小车是否在执行倒库指令
        /// 1.添加上砖倒库任务给该小车
        /// 2.返回true,完成上砖任务
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="track_id"></param>
        /// <returns></returns>
        public bool CheckCarrierInSortTaskAndAddTask(StockTrans trans, uint carrier_id, uint track_id)
        {
            if (PubTask.Carrier.IsCarrierInTask(carrier_id, DevCarrierOrderE.往前倒库))
            {
                Track track = PubMaster.Track.GetTrack(track_id);
                if (PubMaster.Goods.ExistStockInTrack(track_id))
                {
                    AddTransWithoutLock(trans.area_id, 0, TransTypeE.上砖侧倒库, trans.goods_id, 0, track_id, track_id, TransStatusE.倒库中, trans.carrier_id, track.line > 0 ? track.line : trans.line);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查轨道是否有其他车辆
        /// </summary>
        /// <param name="carrierid">运输车ID</param>
        /// <param name="trackid">轨道ID</param>
        /// <param name="result">检查结果</param>
        /// <returns></returns>
        public bool CheckHaveCarrierInOutTrack(uint carrierid, uint trackid, out string result)
        {
            result = "";
            //是否开启【接力倒库轨道可以同时上砖】
            bool isignoresorttask = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

            //是否开启【出入倒库轨道可以同时上砖】
            bool inoutignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask);

            //1.不允许，则不可以有车
            //2.允许，则不可以有非倒库车
            if (!isignoresorttask && !inoutignoresort && PubTask.Carrier.HaveInTrack(trackid, carrierid, out uint othercarid))
            {
                result = string.Format("存在运输车[ {0} ]", PubMaster.Device.GetDeviceName(othercarid));
                return true;
            }

            if ((isignoresorttask || inoutignoresort) && PubTask.Carrier.CheckHaveCarInTrack(TransTypeE.上砖任务, trackid, carrierid, out result))
            {
                return true;
            }

            if ((isignoresorttask || inoutignoresort) && ExistSortBackTask(trackid)) //&& PubMaster.Goods.GetTrackStockCount(trackid) > 1)
            {
                result = "轨道存在还车回轨的倒库任务";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否存在倒库任务状态为还车回轨的任务
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool ExistSortBackTask(uint trackid)
        {
            return TransList.Exists(c => c.give_track_id == trackid
                                && c.TransStaus == TransStatusE.小车回轨
                                && (c.TransType == TransTypeE.上砖侧倒库 || c.TransType == TransTypeE.倒库任务));
        }


        /// <summary>
        /// 是否存在倒库任务状态为还车回轨的任务
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool ExistSortTask(uint trackid)
        {
            return TransList.Exists(c => c.give_track_id == trackid
                                && (c.TransType == TransTypeE.上砖侧倒库 || c.TransType == TransTypeE.倒库任务));
        }



        /// <summary>
        /// 判断轨道的库存是否能够发送小车执行取砖指令
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool CheckStockIsableToTake(StockTrans trans, uint carrierid, uint trackid, uint stockid = 0)
        {
            if (stockid == 0)
            {
                stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
            }
            if (stockid == 0) return false;

            //是否开启【接力倒库轨道可以同时上砖】
            bool isignoresorttask = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

            //是否开启【出入倒库轨道可以同时上砖】
            bool inoutignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask);

            //判断运输车是否能进入轨道
            //1.允许倒库的过程中使用同轨道上砖
            if (isignoresorttask || inoutignoresort)
            {
                //【使用分割点、不限制使用分割点后的库存】
                //2.判断库存所在位置是否轨道分割点后面，并且库存不止一车
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)
                    && !PubMaster.Dic.IsSwitchOnOff(DicTag.CannotUseUpSplitStock))
                {
                    if (!GlobalWcsDataConfig.BigConifg.IsNotNeedSortToSplitUpPlace(trans.area_id, trans.line)
                        && PubMaster.Goods.IsStockBehindUpSplitPoint(trackid, stockid)
                        && !PubMaster.Goods.IsOnlyOneWithStock(stockid))
                    {
                        //在分割点后的库存需要（库存大于1）
                        return false;
                    }
                }

                //3.判断是否存在运输车绑定了该库存
                if (!GlobalWcsDataConfig.BigConifg.IsNotNeedSortToSplitUpPlace(trans.area_id, trans.line)
                    && PubTask.Carrier.ExistCarrierBindStock(carrierid, stockid))
                {
                    return false;
                }

                //4.判断是否存在运输车目标点是该轨道
                if (PubTask.Carrier.ExistLocateTrack(carrierid, trackid))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断轨道是否能继续作业
        /// </summary>
        /// <param name="carrierid">运输车ID</param>
        /// <param name="trackid">轨道ID</param>
        /// <param name="stockid">库存ID</param>
        /// <returns></returns>
        public bool CheckTrackStockStillCanUse(StockTrans trans, uint carrierid, uint trackid, uint stockid = 0)
        {
            if (stockid == 0)
            {
                stockid = PubMaster.Goods.GetTrackTopStockId(trackid);
            }

            if (stockid == 0) return false;

            //是否忽略接力倒库任务绑定的轨道
            bool isignoresorttask = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

            //是否开启【出入倒库轨道可以同时上砖】
            bool inoutignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask);

            if (!isignoresorttask && IsTrasInTransWithType(trackid, TransTypeE.上砖侧倒库))
            {
                return false;
            }


            if (!inoutignoresort && IsTrasInTransWithType(trackid, TransTypeE.倒库任务))
            {
                return false;
            }

            //判断运输车是否能进入轨道
            //1.允许倒库的过程中使用同轨道上砖
            if (isignoresorttask || inoutignoresort)
            {
                //【使用分割点、限制使用分割点后的库存】
                //2.判断库存所在位置是否轨道分割点后面
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.UseUpSplitPoint)
                    && PubMaster.Dic.IsSwitchOnOff(DicTag.CannotUseUpSplitStock)
                    && !GlobalWcsDataConfig.BigConifg.IsNotNeedSortToSplitUpPlace(trans.area_id, trans.line)
                    && PubMaster.Goods.IsStockBehindUpSplitPoint(trackid, stockid))
                {
                    //在分割点后的库存,则不能进行取货操作
                    return false;
                }
                ushort stockount = PubMaster.Goods.GetTrackStockCount(trackid);

                //3.库存大于1的同时，存在倒库完成的任务
                if (stockount > 1 
                    && ExistSortBackTask(trackid))
                {
                    return false;
                }

                //4.库存只剩1个，并且有任务在当前轨道 【就算只有一车也要接力】
                //if (stockount <= 1
                //    && ExistSortTask(trackid))
                //{
                //    return false;
                //}

                //5.存在空闲小车停在轨道头
                if (carrierid != 0 
                    && PubTask.Carrier.IsFreeCarrierInTrack(carrierid, trackid, out uint carid)
                    && !IsCarrierInTrans(carid, trackid, TransTypeE.上砖侧倒库, TransTypeE.倒库任务))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断小车是否在任务中
        /// </summary>
        /// <param name="carrier"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool IsCarrierInTrans(uint carrier, uint trackid,  params TransTypeE[] types)
        {
            return TransList.Exists(c => c.carrier_id == carrier && c.InTrack(trackid) && types.Contains(c.TransType));
        }

        /// <summary>
        /// 判断小车是否在任务中
        /// </summary>
        /// <param name="carrier"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool IsCarrierInTrans(uint carrier, params TransTypeE[] types)
        {
            return TransList.Exists(c => c.carrier_id == carrier && types.Contains(c.TransType));
        }
        #endregion
    }
}
