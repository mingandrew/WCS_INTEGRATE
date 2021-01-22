using enums;
using enums.track;
using GalaSoft.MvvmLight.Messaging;
using module.area;
using module.goods;
using module.msg;
using module.rf;
using module.track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace resource.goods
{
    public class GoodsMaster
    {
        #region[构造/初始化]

        public GoodsMaster()
        {
            _go = new object();
            _so = new object();
            GoodsList = new List<Goods>();
            StockList = new List<Stock>();
            StockSumList = new List<StockSum>();
            GoodSizeList = new List<GoodSize>();
            mMsg = new MsgAction();
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true, bool refr_2 = true, bool refr_3 = true, bool refr_4 = true)
        {

            if (refr_1)
            {
                GoodsList.Clear();
                GoodsList.AddRange(PubMaster.Mod.GoodSql.QueryGoodsList());
            }

            if (refr_2)
            {
                StockList.Clear();
                StockList.AddRange(PubMaster.Mod.GoodSql.QueryStockList());
            }

            if (refr_3)
            {
                StockSumList.Clear();
                StockSumList.AddRange(PubMaster.Mod.GoodSql.QueryStockSumList());
            }

            if (refr_4)
            {
                GoodSizeList.Clear();
                GoodSizeList.AddRange(PubMaster.Mod.GoodSql.QueryGoodSize());
            }
        }

        public void Stop()
        {

        }
        #endregion

        #region[字段]
        private readonly object _go, _so;
        private List<Goods> GoodsList { set; get; }
        private List<Stock> StockList { set; get; }
        private List<StockSum> StockSumList { set; get; }
        private List<GoodSize> GoodSizeList { set; get; }
        private MsgAction mMsg;
        #endregion

        #region[获取对象]

        #region[品种]
        public List<Goods> GetGoodsList()
        {
            return GoodsList;
        }
        public List<Goods> GetGoodsList(List<uint> areaids)
        {
            return GoodsList.FindAll(c=>areaids.Contains(c.area_id));
        }

        public Goods GetGoods(uint id)
        {
            return GoodsList.Find(c => c.id == id);
        }
        #endregion

        #region[库存]

        /// <summary>
        /// 获取单个库存信息
        /// </summary>
        /// <param name="id">库存ID</param>
        /// <returns></returns>
        public Stock GetStock(int id)
        {
            return StockList.Find(c => c.id == id);
        }

        /// <summary>
        /// 获取轨道上的所有库存信息
        /// </summary>
        /// <param name="traid">指定的轨道ID</param>
        /// <returns></returns>
        public List<Stock> GetStocks(uint traid)
        {
            return StockList.FindAll(c => c.track_id == traid);
        }

        /// <summary>
        /// 获取库存统计信息
        /// 1.重新排序
        /// 2.返回库存信息
        /// </summary>
        /// <returns></returns>
        public List<StockSum> GetStockSums()
        {
            SortSumList();
            return StockSumList;
        }

        /// <summary>
        /// 获取指定区域的库存信息
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <returns></returns>
        public List<StockSum> GetStockSums(int areaid)
        {
            if (areaid == 0) return StockSumList;
            return StockSumList.FindAll(c => c.area == areaid).ToList();
        }

        #endregion

        #region[规格]

        public GoodSize GetSize(uint id)
        {
            return GoodSizeList.Find(c => c.id == id);
        }

        public GoodSize GetGoodSize(uint gid)
        {
            return GetSize(GetGoods(gid)?.size_id ?? 0);
        }

        public List<GoodSize> GetGoodSizes()
        {
            return GoodSizeList;
        }

        #endregion
        #endregion

        #region[获取/判断属性]

        #region[品种]
        public string GetGoodsName(uint Goods_id)
        {
            return GoodsList.Find(c => c.id == Goods_id)?.info ?? "";
        }

        public byte GetGoodsLevel(uint Goods_id)
        {
            return GoodsList.Find(c => c.id == Goods_id)?.level ?? 0;
        }
        public bool IsGoodEmpty(uint goodsId)
        {
            return GoodsList.Exists(c => c.id == goodsId && c.empty);
        }

        public List<Goods> GetGoodsList(uint filterarea)
        {
            if (filterarea == 0) return GetGoodsList();
            return GoodsList.FindAll(c => c.area_id == filterarea);
        }

        public List<Goods> GetStockOutGoodsList(uint filterarea)
        {
            List<uint> goodsids = StockList.FindAll(c => c.area == filterarea
                && (c.TrackType == TrackTypeE.储砖_出 || c.TrackType == TrackTypeE.储砖_出入)).Select(t => t.goods_id).ToList();
            List<Goods> glist = new List<Goods>();
            glist.AddRange(GoodsList.FindAll(c => c.area_id == filterarea && (goodsids.Contains(c.id) || c.empty)));
            return glist;
            //return GoodsList.FindAll(c => c.area_id == filterarea && goodsids.Contains(c.id));
        }

        public string GetGoodSizeSimpleName(uint size_id, string prefix)
        {
            GoodSize size = GetSize(size_id);
            if (size != null)
            {
                return prefix + size.name;
            }
            return "";
        }

        public List<StockGoodPack> GetStockOutGoodsInsList()
        {
            List<StockGoodPack> list = new List<StockGoodPack>();
            foreach (Stock stock in StockList.FindAll(c => c.TrackType == TrackTypeE.储砖_出 || c.TrackType == TrackTypeE.储砖_出入))
            {
                if (!list.Exists(c => c.Area == stock.area && c.GoodsId == stock.goods_id))
                {
                    list.Add(new StockGoodPack()
                    {
                        Area = stock.area,
                        GoodsId = stock.goods_id
                    });
                }
            }
            List<Goods> goods = GoodsList.FindAll(c => c.empty);
            foreach (var item in goods)
            {
                list.Add(new StockGoodPack()
                {
                    Area = item.area_id,
                    GoodsId = item.id
                });
            }
            return list;
        }


        /// <summary>
        /// 获取品种安全距离
        /// </summary>
        /// <param name="goods_id"></param>
        /// <returns></returns>
        public ushort GetGoodsSafeDis(uint goods_id)
        {
            GoodSize size = GetSize(GetGoods(goods_id)?.size_id ?? 0);
            if (size != null)
            {
                return (ushort)(size.car_lenght + size.car_space);
            }
            return 0;
        }

        #endregion

        #region[库存]

        public uint GetTrackStock(uint trackid)
        {
            return StockSumList.Find(c => c.track_id == trackid)?.stack ?? 0;
        }

        public bool AddTrackStocks(uint tileid, uint trackid, uint goodsid, byte pieces, DateTime? produceTime, byte stockqty, string memo, out string rs)
        {
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    Goods addgood = GoodsList.Find(c => c.id == goodsid);
                    if (addgood == null || addgood.empty)
                    {
                        rs = "添加的品种不能为空品种！";
                        return false;
                    }

                    for (int i = 0; i < stockqty; i++)
                    {
                        AddStock(tileid, trackid, goodsid, pieces, produceTime);
                    }

                    CheckStockTop(trackid);
                    CheckTrackSum(trackid);
                    PubMaster.Track.UpdateStockStatus(trackid, TrackStockStatusE.有砖, memo);
                    rs = "";
                    return true;
                }
                finally
                {
                    Monitor.Exit(_so);
                }
            }
            rs = "";
            return false;
        }

        public uint GetTrackTopStockId(uint trackid)
        {
            uint stockid = StockList.Find(c => c.track_id == trackid && c.PosType == StockPosE.头部)?.id ?? 0;
            if (stockid == 0)
            {
                stockid = CheckStockTop(trackid);
            }
            return stockid;
        }

        public Stock GetTrackTopStock(uint trackid)
        {
            Stock stock = StockList.Find(c => c.track_id == trackid && c.PosType == StockPosE.头部);
            if (stock == null)
            {
                stock = CheckGetStockTop(trackid);
            }
            return stock;
        }

        public bool HaveStockInTrack(uint trackid, uint goodsid, out uint stockid)
        {
            stockid = StockList.Find(c => c.track_id == trackid && c.goods_id == goodsid)?.id ?? 0;
            return stockid != 0;
        }

        public bool HaveStockInTrack(uint trackid, out uint stockid)
        {
            stockid = StockList.Find(c => c.track_id == trackid)?.id ?? 0;
            return stockid != 0;
        }

        public bool ExistStockInTrack(uint trackid)
        {
            return StockList.Exists(c => c.track_id == trackid);
        }

        public bool ExistStockInTrack(uint trackid, uint goodid)
        {
            return StockList.Exists(c => c.track_id == trackid && c.goods_id == goodid);
        }

        public bool ExistStockInTrackByGid(uint goodid)
        {
            return StockList.Exists(c => c.goods_id == goodid && 
                (c.TrackType == TrackTypeE.储砖_入 || c.TrackType == TrackTypeE.储砖_出 || c.TrackType == TrackTypeE.储砖_出入));
        }

        /// <summary>
        /// 拿出对应库存的下一个库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public short GetNextStockPos(uint trackid, short pos)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid && c.pos > pos);
            if (stocks != null && stocks.Count != 0)
            {
                stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
                return stocks[0].pos;
            }

            return 0;
        }

        #endregion

        #endregion

        #region[增删改]

        #region[品种]
        public bool AddGoods(Goods good, out string result)
        {
            if (GoodsList.Exists(c => c.area_id == good.area_id
                                    && c.size_id == good.size_id
                                    && c.level == good.level
                                    && c.color.Equals(good.color) 
                                    && c.name.Equals(good.name)))
            {
                result = "已经存在一样的品种的信息了！";
                return false;
            }

            if (!Monitor.TryEnter(_go, TimeSpan.FromSeconds(2)))
            {
                result = "";
                return false;
            }

            try
            {
                uint goodid = PubMaster.Dic.GenerateID(DicTag.NewGoodId);
                good.id = goodid;
                good.GoodCarrierType = PubMaster.Area.GetCarrierType(good.area_id);
                good.createtime = DateTime.Now;
                good.updatetime = DateTime.Now;
                PubMaster.Mod.GoodSql.AddGoods(good);
                GoodsList.Add(good);
                GoodsList = GoodsList.OrderByDescending(c => c.updatetime).ToList();
                SendMsg(good, ActionTypeE.Add);
                PubMaster.Dic.UpdateVersion(DicTag.PDA_GOOD_VERSION);
                result = "";
                return true;
            }
            finally
            {
                Monitor.Exit(_go);
            }
        }

        public void UpdateStockProTime(uint id, DateTime? produceTime)
        {
            if (!Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    Stock stock = StockList.Find(c => c.id == id);
                    if (stock != null)
                    {
                        stock.produce_time = produceTime;
                        PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.ProduceTime);
                    }
                }
                finally { Monitor.Exit(_so); }
            }
        }

        public bool ChangeStockGood(uint trackid, uint goodid, bool changedate, DateTime? newdate)
        {
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
                    foreach (Stock stock in stocks)
                    {
                        if (changedate && newdate != null)
                        {
                            stock.produce_time = newdate;
                        }
                        stock.goods_id = goodid;
                        PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Goods);
                    }

                    StockSumChangeGood(trackid, goodid);
                    SortSumList();
                    return true;
                }
                finally { Monitor.Exit(_so); }
            }
            return false;
        }

        private void StockSumChangeGood(uint trackid, uint goodid)
        {
            List<StockSum> sums = StockSumList.FindAll(c => c.track_id == trackid);
            if (sums.Count == 1)
            {
                sums[0].goods_id = goodid;
                sums[0].produce_time = GetEarliestTime(trackid);
                SendSumMsg(sums[0], ActionTypeE.Update);
            }
            else if (sums.Count > 0)
            {
                StockSum newsum = new StockSum
                {
                    goods_id = goodid,
                    produce_time = GetEarliestTime(trackid),
                    track_id = trackid,
                    area = sums[0].area,
                    track_type = sums[0].track_type
                };

                foreach (StockSum sum in sums)
                {
                    newsum.count += sum.count;
                    newsum.pieces += sum.pieces;
                    newsum.stack += sum.stack;
                    SendSumMsg(sum, ActionTypeE.Delete);
                }
                StockSumList.RemoveAll(c => c.track_id == trackid);
                StockSumList.Add(newsum);

                SendSumMsg(newsum, ActionTypeE.Add);
            }
        }

        private DateTime? GetEarliestTime(uint trackid)
        {
            Stock stock = StockList.Find(c => c.track_id == trackid && c.PosType == StockPosE.头部);
            if (stock != null)
            {
                return stock.produce_time;
            }
            DateTime? earytime = null;
            List<Stock> list = StockList.FindAll(c => c.track_id == trackid);
            foreach (Stock item in list)
            {
                if (earytime == null)
                {
                    earytime = item.produce_time;
                }

                if (earytime is DateTime areat && item.produce_time is DateTime stime)
                {
                    if (stime.CompareTo(areat) < 0)
                    {
                        earytime = stime;
                    }
                }
            }

            return earytime;
        }

        public bool EditGood(Goods good, out string result)
        {
            if (!Monitor.TryEnter(_go, TimeSpan.FromSeconds(2)))
            {
                result = "";
                return false;
            }
            try
            {
                Goods g = GoodsList.Find(c => c.id == good.id);
                if (g != null)
                {
                    g.name = good.name;
                    g.color = good.color;
                    g.size_id = good.size_id;
                    g.level = good.level;
                    g.info = good.info;
                    g.memo = good.memo;
                    g.pieces = good.pieces;
                    g.carriertype = good.carriertype;
                    g.updatetime = DateTime.Now;
                    PubMaster.Mod.GoodSql.EditGoods(g);
                    SendMsg(g, ActionTypeE.Update);
                    PubMaster.Dic.UpdateVersion(DicTag.PDA_GOOD_VERSION);
                    result = "";
                    return true;
                }
            }
            finally { Monitor.Exit(_go); }

            result = "找不到该品种信息：" + good.name;
            return false;
        }

        public bool DeleteGood(uint goodid, out string result)
        {
            if (PubMaster.DevConfig.ExistTileLifterByGid(goodid))
            {
                result = "砖机配置了该品种！";
                return false;
            }

            if (PubMaster.Goods.ExistStockInTrackByGid(goodid))
            {
                result = "储砖库存内有该品种";
                return false;
            }

            if (!Monitor.TryEnter(_go, TimeSpan.FromSeconds(2)))
            {
                result = "";
                return false;
            }
            try
            {
                Goods gs = GetGoods(goodid);
                if (gs != null)
                {
                    DeleteStockByGid(goodid);
                    Thread.Sleep(500);

                    PubMaster.Mod.GoodSql.DeleteGoods(gs);
                    GoodsList.Remove(gs);
                    SendMsg(gs, ActionTypeE.Delete);
                    PubMaster.Dic.UpdateVersion(DicTag.PDA_GOOD_VERSION);
                    result = "删除成功：" + gs.name;
                    return true;
                }
            }
            finally { Monitor.Exit(_go); }

            result = "删除失败！";
            return false;
        }

        #endregion

        #region[库存]

        /// <summary>
        /// 添加库存在指定的轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodid"></param>
        /// <param name="transid"></param>
        public uint AddStock(uint tile_id, uint trackid, uint goodid, byte fullqty, DateTime? producetime = null)
        {
            if(Monitor.TryEnter(_go, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    uint newid = PubMaster.Dic.GenerateID(DicTag.NewStockId);
                    byte stack = PubMaster.Goods.GetGoodStack(goodid);
                    ushort allpieces = (ushort)(stack * fullqty);
                    Track track = PubMaster.Track.GetTrack(trackid);
                    Stock stock = new Stock()
                    {
                        id = newid,
                        track_id = trackid,
                        goods_id = goodid,
                        produce_time = producetime ?? DateTime.Now,
                        stack = stack,
                        pieces = allpieces, //总片数
                        tilelifter_id = tile_id,
                        area = track.area,
                        track_type = track.type
                    };

                    UpdateTrackPos(stock, track);
                    StockList.Add(stock);
                    PubMaster.Mod.GoodSql.AddStock(stock);
                    return newid;
                }
                finally
                {
                    Monitor.Exit(_go);
                }
            }
            return 0;
        }

        public bool DeleteStock(uint stockid, out string rs)
        {
            Stock stock = StockList.Find(c => c.id == stockid);
            if(stock == null)
            {
                rs = "找不到库存记录";
                return false;
            }

            StockList.Remove(stock);
            PubMaster.Mod.GoodSql.DeleteStock(stock);
            StockSumChange(stock, 0);
            if(stock.PosType == StockPosE.头部)
            {
                CheckStockTop(stock.track_id);
            }

            if (stock.PosType == StockPosE.尾部)
            {
                CheckStockBottom(stock.track_id);
            }
            rs = "";
            return true;
        }

        public void DeleteStockByGid(uint goodid)
        {
            List<Stock> stocks = StockList.FindAll(c => c.goods_id == goodid);
            if (stocks == null || stocks.Count == 0)
            {
                return;
            }

            foreach (Stock s in stocks)
            {
                StockList.Remove(s);
                PubMaster.Mod.GoodSql.DeleteStock(s);
                StockSumChange(s, 0);
            }
        }

        private byte GetGoodStack(uint goodid)
        {
            return GetGoodSize(goodid)?.stack ?? 1;
        }

        /// <summary>
        /// 检查并更新轨道库存为空
        /// </summary>
        /// <param name="trackid"></param>
        public void RemoveStock(uint trackid)
        {
            StockList.RemoveAll(c => c.track_id == trackid);
            PubMaster.Mod.GoodSql.DeleteStock(trackid);
        }

        /// <summary>
        /// 倒库/库存信息调整
        /// </summary>
        /// <param name="taketrackid"></param>
        public bool ShiftStock(uint taketrackid, uint givetrackid)
        {
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    if (!PubMaster.Track.IsBrotherTrack(taketrackid, givetrackid)) return false;

                    //if (PubMaster.Track.IsTrackFull(givetrackid)
                    //    && PubMaster.Track.IsTrackEmtpy(taketrackid))
                    //{
                    //    return true;
                    //}

                    Track givetrack = PubMaster.Track.GetTrack(givetrackid);
                    Track taketrack = PubMaster.Track.GetTrack(taketrackid);
                    if (givetrack != null && taketrack != null
                        && givetrack.StockStatus == TrackStockStatusE.空砖
                        //&& taketrack.StockStatus == TrackStockStatusE.满砖
                        )
                    {
                        List<Stock> stocks = StockList.FindAll(c => c.track_id == taketrack.id);
                        foreach (Stock stock in stocks)
                        {
                            stock.track_id = givetrack.id;
                            stock.area = givetrack.area;
                            stock.track_type = givetrack.type;
                            PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Track);
                        }

                        PubMaster.Track.ShiftTrack(taketrack.id, givetrack.id);
                        UpdateShiftStockSum(taketrackid, givetrackid, givetrack.type);
                        return true;
                    }
                }
                finally
                {
                    Monitor.Exit(_so);
                }
            }
            return false;
        }

        /// <summary>
        /// 获取轨道库存数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ushort GetTrackStockCount(uint id)
        {
            return (ushort)StockList.Count(c => c.track_id == id);
        }

        /// <summary>
        /// 根据上砖机,品种，分配库存
        /// 1.优先分配，非满砖轨道
        /// 2.在满砖轨道中，分配时间最早的轨道
        /// </summary>
        /// <param name="tilelifterid"></param>
        /// <param name="goodsid"></param>
        /// <param name="stockid"></param>
        /// <param name="taketrackid"></param>
        /// <returns></returns>
        public bool GetStock(uint areaid, uint tilelifterid, uint goodsid, out List<Stock> allocatstocks)
        {
            allocatstocks = new List<Stock>();

            //1.找到上砖机配置的轨道
            List<AreaDeviceTrack> devtrack = PubMaster.Area.GetAreaDevTraList(areaid, tilelifterid);

            //2.根据优先级查看非空且是需求的品种的轨道
            List<Stock> stocks = StockList.FindAll(c => c.goods_id == goodsid 
                                                    && c.PosType == StockPosE.头部 
                                                    &&  devtrack.Exists(d => d.track_id == c.track_id));

            if (stocks.Count == 0)
            {
                //找不到库存
                return false;
            }

            stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
            //foreach (Stock stock in stocks)
            //{
            //    //优先取非满的轨道
            //    if (stock.pos > 1 && PubMaster.Track.IsTrackHaveStock(stock.track_id)
            //        && PubMaster.Track.IsTrackEnable(stock.track_id))
            //    {
            //        allocatstocks.Add(stock);
            //    }
            //}

            //全部都是满砖，则找时间最早的库存
            stocks.Sort(
                (x, y) => 
                {
                    if(x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                    {
                        return xtime.CompareTo(ytime);
                    }
                    return 0;
                }
            );

            int minStockTime = PubMaster.Dic.GetDtlIntCode("MinStockTime");
            foreach (Stock stock1 in stocks)
            {
                if ((DateTime.Now - (DateTime)stock1.produce_time).TotalHours >= minStockTime)
                {
                    if (!PubMaster.Track.IsEmtpy(stock1.track_id) && 
                        PubMaster.Track.IsTrackEnable(stock1.track_id, TrackStatusE.仅上砖))
                    {
                        allocatstocks.Add(stock1);
                    }
                }
            }
            return allocatstocks.Count > 0;
        }

        /// <summary>
        /// 根据上砖机,品种，分配库存
        /// 1.优先分配，非满砖轨道
        /// 2.在满砖轨道中，分配时间最早的轨道
        /// </summary>
        /// <param name="tilelifterid"></param>
        /// <param name="goodsid"></param>
        /// <param name="stockid"></param>
        /// <param name="taketrackid"></param>
        /// <returns></returns>
        public List<Stock> GetStock(uint areaid, uint tilelifterid, uint goodsid)
        {
            //1.找到上砖机配置的轨道
            List<AreaDeviceTrack> devtrack = PubMaster.Area.GetAreaDevTraList(areaid, tilelifterid);

            //2.根据优先级查看非空且是需求的品种的轨道
            List<Stock> stocks = StockList.FindAll(c => c.goods_id == goodsid
                                                    && c.PosType == StockPosE.头部
                                                    && devtrack.Exists(d => d.track_id == c.track_id));

            if (stocks.Count == 0)
            {
                return new List<Stock>();
            }

            stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
            //foreach (Stock stock in stocks)
            //{
            //    //优先取非满的轨道
            //    if (stock.pos > 1 && PubMaster.Track.IsTrackHaveStock(stock.track_id)
            //        && PubMaster.Track.IsTrackEnable(stock.track_id))
            //    {
            //        return new List<Stock>() { stock };
            //    }
            //}

            //全部都是满砖，则找时间最早的库存
            stocks.Sort(
                (x, y) =>
                {
                    if (x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                    {
                        return xtime.CompareTo(ytime);
                    }
                    return 0;
                }
            );

            return stocks.FindAll(c => PubMaster.Track.IsTrackEnable(c.track_id, TrackStatusE.仅上砖));
        }

        /// <summary>
        /// 将库存转移到指定的轨道
        /// </summary>
        /// <param name="stock_id">库存ID</param>
        /// <param name="to_track_id">被转移到的轨道ID</param>
        public void MoveStock(uint stock_id, uint to_track_id)
        {
            Stock stock = StockList.Find(c => c.id == stock_id);
            if(stock != null && stock.track_id != to_track_id)
            {
                uint from_track_id = stock.track_id;

                bool istostore = PubMaster.Track.IsStoreTrack(to_track_id);
                bool isfromstore = PubMaster.Track.IsStoreTrack(from_track_id);

                //更新库存统计信息
                StockSumChange(stock, to_track_id);

                //更新轨道被转移后的轨道信息(区域，轨道ID，轨道类型)
                Track totrack = PubMaster.Track.GetTrack(to_track_id);
                stock.track_id = to_track_id;
                stock.area = totrack.area;
                stock.track_type = totrack.type;

                #region[更新储砖轨道]

                //将库存 移入 储砖轨道
                if (istostore)
                {
                    UpdateTrackPos(stock, totrack);
                    PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Pos);
                }

                //从储砖轨道 移出 库存
                if (isfromstore)
                {
                    CheckStockTop(from_track_id);

                    if (PubMaster.Track.IsTrackFull(from_track_id))
                    {
                        PubMaster.Track.UpdateStockStatus(from_track_id, TrackStockStatusE.有砖, "");
                    }
                }

                #endregion

                PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Track);

                PubMaster.Track.UpdateStockStatus(to_track_id, TrackStockStatusE.有砖, "");

                //由设备检查轨道没砖后才制空轨道
                //if(!ExistStockInTrack(from_track_id) 
                //    && !PubMaster.Track.IsStoreTrack(from_track_id))
                //{
                //    PubMaster.Track.UpdateStockStatus(from_track_id, TrackStockStatusE.空砖);
                //}

            }
        }

        /// <summary>
        /// 更新库存的位置信息(pos)
        /// </summary>
        /// <param name="stock">库存信息</param>
        /// <param name="track">库存所在轨道</param>
        public void UpdateTrackPos(Stock stock, Track track)
        {
            //轨道当前库存信息
            short storecount = (short)StockList.Count(c => c.track_id == stock.track_id && c.id != stock.id);

            if (storecount == 0)
            {
                stock.PosType = StockPosE.头部;
                stock.pos = (short)(track.same_side_inout ? 50 : 0);
            }
            else
            {
                //如轨道是同向出入，则将后面添加的库存放在第一位
                if (track.same_side_inout)
                {
                    Stock topStock = GetTrackTopStock(stock.track_id);
                    if (topStock != null)
                    {
                        SetStockPosType(topStock, StockPosE.中部);
                    }

                    short FinalStockPos = StockList.FindAll(c => c.track_id == stock.track_id && c.id != stock.id).Min(c => c.pos);
                    stock.pos = (short)(FinalStockPos - 1);
                    stock.PosType = StockPosE.头部;
                }
                else
                {
                    Stock Bottomstock = StockList.Find(c => c.track_id == stock.track_id && c.PosType == StockPosE.尾部);
                    if (Bottomstock != null)
                    {
                        SetStockPosType(Bottomstock, StockPosE.中部);
                    }

                    short FinalStockPos = StockList.FindAll(c => c.track_id == stock.track_id && c.id != stock.id).Max(c => c.pos);
                    stock.pos = (short)(FinalStockPos + 1);
                    stock.PosType = StockPosE.尾部;
                }
            }
        }

        /// <summary>
        /// 插入库存前将库存往后移
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="pos"></param>
        /// <param name="qty"></param>
        public void UpdateStockTrackPos(uint trackid, short pos, byte qty)
        {
            List<Stock> sl = StockList.FindAll(c => c.track_id == trackid && c.pos >= pos);
            sl.Sort((x, y) => x.pos.CompareTo(y.pos));
            if (sl != null && sl.Count != 0)
            {
                foreach (Stock s in sl)
                {
                    if (s.PosType == StockPosE.头部)
                    {
                        s.PosType = StockPosE.中部;
                    }
                    s.pos += qty;
                    PubMaster.Mod.GoodSql.EditStock(s, qty);
                }
            }

        }

        /// <summary>
        /// 在指定位置插入指定数量的库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodsid"></param>
        /// <param name="pieces"></param>
        /// <param name="produceTime"></param>
        /// <param name="stockqty"></param>
        /// <param name="memo"></param>
        /// <param name="rs"></param>
        /// <param name="startpos"></param>
        /// <returns></returns>
        public bool InsertStock(uint trackid, uint goodsid, byte pieces, DateTime? produceTime, byte stockqty, string memo, out string rs, short startpos)
        {
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    Goods addgood = GoodsList.Find(c => c.id == goodsid);
                    if (addgood == null || addgood.empty)
                    {
                        rs = "添加的品种不能为空品种！";
                        return false;
                    }

                    for (int i = 0; i < stockqty; i++)
                    {
                        uint newid = PubMaster.Dic.GenerateID(DicTag.NewStockId);
                        byte stack = PubMaster.Goods.GetGoodStack(goodsid);
                        ushort allpieces = (ushort)(stack * pieces);
                        Track track = PubMaster.Track.GetTrack(trackid);
                        Stock stock = new Stock()
                        {
                            id = newid,
                            track_id = trackid,
                            goods_id = goodsid,
                            produce_time = produceTime ?? DateTime.Now,
                            stack = stack,
                            pieces = allpieces, //总片数
                            tilelifter_id = 0,
                            area = track.area,
                            track_type = track.type,
                            pos = startpos,
                            PosType = StockPosE.中部,
                        };

                        StockList.Add(stock);
                        PubMaster.Mod.GoodSql.AddStock(stock);
                        startpos++;
                    }

                    CheckStockTop(trackid);
                    CheckTrackSum(trackid);
                    PubMaster.Track.UpdateStockStatus(trackid, TrackStockStatusE.有砖, memo);
                    rs = "";
                    return true;
                }
                finally
                {
                    Monitor.Exit(_so);
                }
            }
            rs = "";
            return false;
        }

        /// <summary>
        /// 添加下砖记录
        /// </summary>
        /// <param name="stockid"></param>
        public void AddStockInLog(uint stockid)
        {
            Stock stock = StockList.Find(c => c.id == stockid);
            if(stock != null)
            {
                PubMaster.Mod.GoodSql.AddStockInLog(stock);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="give_track_id"></param>
        /// <returns></returns>
        public bool IsTrackReachMinStackCount(uint goodid, uint trackid)
        {
            Goods goods = GetGoods(goodid);
            if (goods != null){

                return goods.minstack <= StockList.Count(c => c.track_id == trackid);
            }
            return true;
        }

        /// <summary>
        /// 生成上砖记录
        /// </summary>
        /// <param name="stockid"></param>
        /// <param name="iD"></param>
        /// <param name="leftTrackId"></param>
        public void AddStockOutLog(uint stockid, uint trackid, uint tileid)
        {
            Stock stock = StockList.Find(c => c.id == stockid);
            if (stock != null)
            {
                PubMaster.Mod.GoodSql.AddStockOutLog(stock, trackid, tileid);
            }
        }

        /// <summary>
        /// 判断轨道是否没有库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsTrackStockEmpty(uint trackid)
        {
            return !StockList.Exists(c => c.track_id == trackid);
        }

        /// <summary>
        /// 检查是否有Top,并且更新为顶部
        /// </summary>
        /// <param name="trackid"></param>
        public uint CheckStockTop(uint trackid)
        {
            if(!StockList.Exists(c=>c.track_id == trackid && c.PosType == StockPosE.头部))
            {
                List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
                if (stocks.Count > 0)
                {
                    stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
                    SetStockPosType(stocks[0], StockPosE.头部);
                    return stocks[0].id;
                }
            }
            return 0;
        }

        /// <summary>
        /// 检查是否有bottom,并且更新为尾部
        /// </summary>
        /// <param name="trackid"></param>
        public uint CheckStockBottom(uint trackid)
        {
            if (!StockList.Exists(c => c.track_id == trackid && c.PosType == StockPosE.尾部))
            {
                List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
                if (stocks.Count > 1)
                {
                    stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
                    SetStockPosType(stocks[stocks.Count -1], StockPosE.尾部);
                    return stocks[stocks.Count - 1].id;
                }
            }
            return 0;
        }

        public Stock CheckGetStockTop(uint trackid)
        {
            if (!StockList.Exists(c => c.track_id == trackid && c.PosType == StockPosE.头部))
            {
                List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
                if (stocks.Count > 0)
                {
                    stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
                    SetStockPosType(stocks[0], StockPosE.头部);
                    return stocks[0];
                }
            }
            return StockList.Find(c => c.track_id == trackid && c.PosType == StockPosE.头部);
        }

        public void SetStockPosType(Stock stock, StockPosE postype)
        {
            if (stock == null) return;
            stock.PosType = postype;
            PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.PosType);
        }

        #endregion

        #region[库存统计]

        /// <summary>
        /// 更新轨道的统计库存信息
        /// </summary>
        /// <param name="stock">单个库存信息</param>
        /// <param name="totrackid">库存的去向轨道ID</param>
        private void StockSumChange(Stock stock, uint totrackid)
        {
            //该库存是否去向储砖轨道
            bool istostore = PubMaster.Track.IsStoreTrack(totrackid);
            //该库存是否来源于储砖轨道
            bool isfromstore = PubMaster.Track.IsStoreTrack(stock.track_id);
            if (istostore || isfromstore)
            {
                StockSum sum;
                if (istostore)
                {   //库存放置 => 储砖轨道(加)
                    sum = StockSumList.Find(c => c.track_id == totrackid && c.goods_id == stock.goods_id);
                }
                else
                {   //库存 从 储砖轨道 => 出(减)
                    sum = StockSumList.Find(c => c.track_id == stock.track_id && c.goods_id == stock.goods_id);
                }

                if(sum != null)
                {
                    if (istostore)
                    {
                        sum.count += 1;
                        sum.stack += stock.stack;
                        sum.pieces += stock.pieces;
                    }
                    else
                    {
                        sum.count -= 1;
                        sum.stack -= stock.stack;
                        sum.pieces -= stock.pieces;
                    }

                    if (sum.count == 0)
                    {
                        StockSumList.Remove(sum);
                        SendSumMsg(sum, ActionTypeE.Delete);
                    }
                    else
                    {
                        if (isfromstore)
                        {
                            sum.produce_time = GetEarliestTime(sum.track_id);
                        }
                        SendSumMsg(sum, ActionTypeE.Update);
                    }
                }

                if (sum == null && istostore)
                {
                    Track track = PubMaster.Track.GetTrack(totrackid);
                    sum = new StockSum()
                    {
                        count = 1,
                        goods_id = stock.goods_id,
                        track_id = totrackid,
                        pieces = stock.pieces,
                        stack = stock.stack,
                        produce_time = stock.produce_time,
                        area = track.area,
                        track_type = track.type
                    };
                    StockSumList.Add(sum);
                    SendSumMsg(sum, ActionTypeE.Add);
                    SortSumList();
                }
            }
        }

        /// <summary>
        /// 判断库存是否是对应的品种
        /// </summary>
        /// <param name="stockid">库存ID</param>
        /// <param name="goodsId">品种ID</param>
        /// <returns></returns>
        public bool IsStockWithGood(uint stockid, uint goodsId)
        {
            return StockList.Exists(c => c.id == stockid && c.goods_id == goodsId);
        }

        /// <summary>
        /// 轨道转移库存，同时刷新界面统计数据
        /// </summary>
        /// <param name="taketrackid">原轨道ID</param>
        /// <param name="givetrackid">转移的轨道ID</param>
        /// <param name="type">转移后的轨道类型</param>
        private void UpdateShiftStockSum(uint taketrackid, uint givetrackid, byte type)
        {
            List<StockSum> takesums = StockSumList.FindAll(c => c.track_id == taketrackid);

            foreach (StockSum sum in takesums)
            {
                SendSumMsg(sum, ActionTypeE.Delete);
                sum.track_id = givetrackid;
                sum.track_type = type;
                SendSumMsg(sum, ActionTypeE.Add);
            }
        }
       
        /// <summary>
        /// 获取库存的砖机ID
        /// </summary>
        /// <param name="stock_id">库存ID</param>
        /// <returns></returns>
        public uint GetStockTileId(uint stock_id)
        {
            return StockList.Find(c => c.id == stock_id && c.tilelifter_id != 0)?.tilelifter_id ?? 0;
        }

        /// <summary>
        /// 获取品种车型
        /// </summary>
        /// <param name="goods_id">品种ID</param>
        /// <returns></returns>
        public CarrierTypeE GetGoodsCarrierType(uint goods_id)
        {
            return GoodsList.Find(c => c.id == goods_id).GoodCarrierType;
        }

        /// <summary>
        /// 清空轨道的所有统计信息
        /// </summary>
        /// <param name="trackid">被清空的轨道ID</param>
        private void RemoveTrackSum(uint trackid)
        {
            List<StockSum> sums = StockSumList.FindAll(c => c.track_id == trackid);
            if (sums.Count > 0)
            {
                foreach (StockSum sum in sums)
                {
                    StockSumList.Remove(sum);
                    SendSumMsg(sum, ActionTypeE.Delete);
                }
            }
        }

        /// <summary>
        /// 发送消息给界面更新统计信息
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="type"></param>
        private void SendSumMsg(StockSum sum , ActionTypeE type)
        {
            mMsg.o1 = sum;
            mMsg.o2 = type;
            Messenger.Default.Send(mMsg, MsgToken.StockSumeUpdate);
        }

        /// <summary>
        /// 刷新轨道的库存概况
        /// </summary>
        /// <param name="trackId"></param>
        public void CheckTrackSum(uint trackId)
        {
            List<uint> goodsids = StockList.FindAll(c => c.track_id == trackId && c.goods_id != 0)?.Select(c => c.goods_id).Distinct().ToList();

            if (goodsids != null && goodsids.Count != 0)
            {
                Track track = PubMaster.Track.GetTrack(trackId);
                foreach (uint gid in goodsids)
                {
                    Goods goods = GoodsList.Find(c => c.id == gid);
                    GoodSize size = GetSize(goods.size_id);
                    StockSum sum = StockSumList.Find(c => c.goods_id == gid && c.track_id == trackId);
                    if (sum == null)
                    {
                        sum = new StockSum()
                        {
                            track_id = trackId,
                            goods_id = gid,
                            produce_time = StockList.Find(c => c.goods_id == gid && c.track_id == trackId).produce_time,
                            area = track.area,
                            track_type = track.type
                        };
                        StockSumList.Add(sum);
                    }
                    sum.count = (uint)StockList.Count(c => c.goods_id == gid && c.track_id == trackId);
                    sum.stack = sum.count * (size?.stack ?? 1);
                    sum.pieces = sum.stack * goods.pieces;
                    SendSumMsg(sum, ActionTypeE.Update);
                }
                SortSumList();
            }
        }
        #endregion

        #region[排序]
        private void SortSumList()
        {
            StockSumList.Sort((x, y) =>
            {
                if (x.area == y.area)
                {
                    if (x.goods_id == y.goods_id)
                    {
                        return x.CompareProduceTime(y.produce_time);
                    }
                    else
                    {
                        return x.goods_id.CompareTo(y.goods_id);
                    }
                }
                return x.area.CompareTo(y.area);
            });
        }
        #endregion

        #endregion

        #region[任务逻辑]

        #region[分配轨道]

        /// <summary>
        /// 分配储砖轨道：根据区域/下砖设备/品种
        /// </summary>
        /// <param name="areaid">下砖区域</param>
        /// <param name="devid">分配设备</param>
        /// <param name="goodsid">品种</param>
        /// <param name="givetrackid">分配轨道</param>
        /// <returns></returns>
        public bool AllocateGiveTrack(uint areaid, uint devid, uint goodsid, out List<uint> traids)
        {
            List<AreaDeviceTrack> list = PubMaster.Area.GetAreaDevTraList(areaid, devid);
            traids = new List<uint>();
            List<uint> emptylist = new List<uint>();
            List<TrackStoreCount> trackstores = new List<TrackStoreCount>();
            uint storecount = 0;
            foreach (AreaDeviceTrack adt in list)
            {
                //是否是储砖轨道
                if (!PubMaster.Track.IsStoreGiveTrack(adt.track_id)) continue;

                //轨道是否启用
                if (!PubMaster.Track.IsTrackEnable(adt.track_id, TrackStatusE.仅下砖)) continue;

                //轨道满否
                if (PubMaster.Track.IsTrackFull(adt.track_id)) continue;

                //[可以放任何品种] 空轨道，轨道没有库存
                if (PubMaster.Track.IsEmtpy(adt.track_id)
                    && IsTrackStockEmpty(adt.track_id)
                    && IsTrackOkForGoods(adt.track_id, goodsid))
                {
                    emptylist.Add(adt.track_id);
                }

                //是否已存同品种并且未满
                if (IsTrackFineToStore(adt.track_id, goodsid, out storecount))
                {
                    trackstores.Add(new TrackStoreCount()
                    {
                        trackid = adt.track_id,
                        storecount = storecount
                    });
                }
            }

            if (trackstores.Count > 0)
            {
                trackstores.Sort((x, y) => y.storecount.CompareTo(x.storecount));
                foreach (TrackStoreCount item in trackstores)
                {
                    traids.Add(item.trackid);
                }
            }

            traids.AddRange(emptylist);
            return traids.Count > 0;
        }

        /// <summary>
        /// 判断轨道
        /// 1.已存同品种
        /// 2.轨道为到达满砖数量
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        private bool IsTrackFineToStore(uint trackid, uint goodsid, out uint storecount)
        {
            bool isok = StockList.Exists(c => c.track_id == trackid && c.goods_id == goodsid);
            if (isok)
            {
                int maxstore = PubMaster.Track.GetTrackMaxStore(trackid);
                storecount = StockSumList.Find(c => c.track_id == trackid)?.count ?? 0;
                if (storecount < maxstore)
                {
                    return true;
                }
            }
            storecount = 0;
            return false;
        }

        public bool IsHaveEmtpyTrackEnough(uint areaid, uint devid, out List<uint> traids)
        {
            List<AreaDeviceTrack> list = PubMaster.Area.GetAreaDevTraList(areaid, devid);
            traids = new List<uint>();
            if (list == null || list.Count == 0)
            {
                return false;
            }
            foreach (AreaDeviceTrack adt in list)
            {
                //是否是储砖轨道
                if (!PubMaster.Track.IsStoreGiveTrack(adt.track_id)) continue;

                //轨道是否启用
                if (!PubMaster.Track.IsTrackEnable(adt.track_id, TrackStatusE.仅下砖)) continue;

                //轨道空否
                if (!PubMaster.Track.IsTrackEmtpy(adt.track_id)) continue;

                traids.Add(adt.track_id);
            }
            return traids.Count > 0;
        }

        #endregion

        #region[轨道能否放该品种砖]

        public bool IsTrackOkForGoods(uint trackid, uint goodsid)
        {
            Goods goods = GoodsList.Find(c => c.id == goodsid);
            if (goods == null || goods.size_id == 0) return false;
            GoodSize size = GoodSizeList.Find(c => c.id == goods.size_id);
            if (size == null) return false;
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track == null) return false;

            bool isleftok = true, isrightok = true;

            #region[判断轨道能不能放砖]

            if (track.left_track_id == 0)
            {
                if (CheckTrackAndGood(track.width, size.width, track.left_distance))
                {
                    return false;
                }
            }

            if (track.right_track_id == 0)
            {
                if (CheckTrackAndGood(track.width, size.width, track.right_distance))
                {
                    return false;
                }
            }

            #endregion

            #region[判断隔壁轨道是否碰撞]

            Track ltrack = PubMaster.Track.GetTrack(track.left_track_id);
            Track rtrack = PubMaster.Track.GetTrack(track.right_track_id);

            if(ltrack != null)
            {
                //判断左兄弟轨道
                Goods leftgoods = GetTrackGoods(ltrack.id);
                if (leftgoods == null && ltrack.StockStatus == TrackStockStatusE.有砖)
                {
                    leftgoods = PubMaster.Goods.GetGoods(ltrack.recent_goodid);
                }
                if (leftgoods != null)
                {
                    isleftok = IsGoodsDistanceOk(ltrack, leftgoods, track, goods);
                }
            }

            if(rtrack != null)
            {
                Goods rightgoods = GetTrackGoods(rtrack.id);
                if (rightgoods == null && rtrack.StockStatus == TrackStockStatusE.有砖)
                {
                    rightgoods = PubMaster.Goods.GetGoods(rtrack.recent_goodid);
                }
                if (rightgoods != null)
                {
                    isrightok = IsGoodsDistanceOk(track, goods, rtrack, rightgoods);
                }
            }
            #endregion

            return isleftok && isrightok;
        }

        private bool CheckTrackAndGood(ushort trackwidth, ushort goodwidth, ushort trackdistance)
        {
            //(trackdistance - (goodwidth - trackwidth) / 2)
            return trackdistance-(Math.Abs((goodwidth - trackwidth)/2)) < 100;
        }

        public byte GetGoodsPieces(uint goodid)
        {
            return (byte)(GoodsList.Find(c => c.id == goodid)?.pieces ?? 0);
        }

        /// <summary>
        /// 获取轨道库存最大品种
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        private Goods GetTrackGoods(uint trackid)
        {
            uint goodsid = StockList.Find(c => c.track_id == trackid)?.goods_id ?? 0;
            if (goodsid != 0)
            {
                //是否存在不同品种的库存在同一个轨道
                if (StockList.Exists(c => c.track_id == trackid && c.goods_id != goodsid))
                {
                    var goodsids = StockList.FindAll(c => c.track_id == trackid)
                        .GroupBy(x => new { x.goods_id })
                        .Select(t => t.Key.goods_id);
                    foreach (var goodid in goodsids)
                    {
                        if (CampareGoodWidth(goodsid, goodid))
                        {
                            goodsid = goodid;
                        }
                    }
                }
                return GoodsList.Find(c => c.id == goodsid);
            }
            return null;
        }

        private bool CampareGoodWidth(uint one, uint two)
        {
            GoodSize sizeone = GetGoodSize(one);
            GoodSize sizetwo = GetGoodSize(two);
            if (sizeone != null && sizeone != null)
            {
                return sizeone.width < sizetwo.width;
            }
            return false;
        }

        /// <summary>
        /// 判断轨道
        /// </summary>
        /// <param name="track"></param>
        /// <param name="putgood"></param>
        /// <param name="havegood"></param>
        /// <returns></returns>
        public bool IsGoodsDistanceOk(Track lefttrack, Goods leftgoods, Track righttrack, Goods rightgoods)
        {
            GoodSize lsize = GetSize(leftgoods.size_id);
            GoodSize rsize = GetSize(rightgoods.size_id);

            int ld = 0;
            if (lefttrack.width <= lsize.width)
            {
                ld = Math.Abs(lsize.width - lefttrack.width) / 2;
            }
            int rd = 0;
            if (righttrack.width <= rsize.width)
            {
                rd = Math.Abs(rsize.width - righttrack.width) / 2;
            }

            if (lefttrack.right_distance == righttrack.left_distance)
            {
                return (righttrack.left_distance - ld - rd) >= 150;
            }
            int distance = lefttrack.right_distance < righttrack.left_distance ? lefttrack.right_distance : righttrack.left_distance;
            return (distance - ld - rd) >= 150;
        }

        public uint GetTrackStockId(uint trackid)
        {
            return StockList.Find(c => c.track_id == trackid)?.id ?? 0;
        }

        #endregion

        #region 计算轨道下一车作业坐标

        /// <summary>
        /// 计算下一车轨道坐标
        /// </summary>
        /// <param name="tt"></param>
        /// <param name="carrierid"></param>
        /// <param name="trackid"></param>
        /// <param name="stockcount"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool CalculateNextLocation(TransTypeE tt, uint carrierid, uint trackid, out ushort stockcount, out ushort location)
        {
            bool isOK = false;
            stockcount = 0;
            location = 0;
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
            if (stocks == null || stocks.Count == 0)
            {
                location = PubMaster.Track.GetTrackSplitPoint(trackid);
            }
            else
            {
                stockcount = (ushort)stocks.Count;
                switch (tt)
                {
                    case TransTypeE.下砖任务:
                    case TransTypeE.手动下砖:
                        Stock bottom = stocks.Find(c => c.PosType == StockPosE.尾部);
                        ushort car = PubMaster.DevConfig.GetCarrierLenght(carrierid);
                        ushort safe = GetGoodsSafeDis(bottom.goods_id);
                        // 当砖间距比小车顶板小，用顶板长度更安全
                        safe = car > safe ? car : safe;
                        ushort limit = PubMaster.Track.GetTrackLimitPoint(trackid);
                        location = (ushort)(bottom.location - safe);
                        if (location < limit)
                        {
                            location = 0;
                            break;
                        }
                        isOK = true;
                        break;

                    case TransTypeE.上砖任务:
                    case TransTypeE.手动上砖:
                        break;
                    case TransTypeE.倒库任务:
                        break;
                    case TransTypeE.移车任务:
                        break;
                    case TransTypeE.同向上砖:
                        break;
                    case TransTypeE.同向下砖:
                        break;
                    case TransTypeE.其他:
                        break;
                    default:
                        break;
                }
            }

            return isOK;
        }

        #endregion

        #endregion

        #region[发送信息]

        /// <summary>
        /// 发送品种更改消息-给品种界面用于更新数据
        /// </summary>
        /// <param name="goods"></param>
        /// <param name="type"></param>
        private void SendMsg(Goods goods, ActionTypeE type)
        {
            MsgAction msg = new MsgAction()
            {
                o1 = goods,
                o2 = type
            };

            Messenger.Default.Send(msg, MsgToken.GoodsUpdate);
        }

        /// <summary>
        /// 空砖信号后，清空轨道库存
        /// </summary>
        /// <param name="take_track_id"></param>
        public void ClearTrackEmtpy(uint take_track_id, bool isuptiletrack = false, uint tileid = 0)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == take_track_id);
            if (stocks.Count > 0)
            {
                PubMaster.Mod.GoodSql.DeleteStock(take_track_id);
                StockList.RemoveAll(c => c.track_id == take_track_id);
                RemoveTrackSum(take_track_id);

                if (isuptiletrack)
                {
                    AddTileConsumLog(stocks, tileid);
                }
            }
        }

        /// <summary>
        /// 获取库存上面的品种ID
        /// </summary>
        /// <param name="id">库存ID</param>
        /// <returns></returns>
        public uint GetGoodsId(uint id)
        {
            return StockList.Find(c => c.track_id == id)?.goods_id ?? 0;
        }

        #endregion

        #region[上砖消除库存记录]

        /// <summary>
        /// 添加砖机的消耗信息-用于给统计看板统计信息
        /// </summary>
        /// <param name="stocks">被消耗的库存</param>
        /// <param name="tileid">消耗库存的砖机ID</param>
        private void AddTileConsumLog(List<Stock> stocks, uint tileid)
        {
            foreach (var item in stocks)
            {
                PubMaster.Mod.GoodSql.AddConsumeLog(item, tileid);
            }
        }

        #endregion
    }
}
