using enums;
using enums.track;
using enums.warning;
using GalaSoft.MvvmLight.Messaging;
using module.area;
using module.deviceconfig;
using module.goods;
using module.msg;
using module.other;
using module.rf;
using module.track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using tool.mlog;

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
            _mlog = (Log)new LogFactory().GetLog("库存信息", false);
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
        private Log _mlog;
        protected readonly string timeformat = "yyyy-MM-dd HH:mm:ss";
        #endregion

        #region[获取对象]

        #region[品种]
        public List<Goods> GetGoodsList()
        {
            return GoodsList;
        }
        public List<Goods> GetGoodsList(List<uint> areaids)
        {
            return GoodsList.FindAll(c => areaids.Contains(c.area_id));
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
        public Stock GetStock(uint id)
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
        /// 获取指定类型轨道上的所有头部库存（按时间从早到晚）
        /// </summary>
        /// <param name="tt"></param>
        /// <returns></returns>
        public List<Stock> GetStocksOrderByTop(TrackTypeE tt)
        {
            List<Stock> stocks = StockList.FindAll(c => c.TrackType == tt && c.PosType == StockPosE.头部);

            if (stocks.Count == 0)
            {
                //找不到库存
                return stocks;
            }

            // 按时间从早到晚
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

            List<Stock> afterlist = new List<Stock>();

            if (stocks.Count > 0)
            {
                List<uint> nowuptilegood = PubMaster.DevConfig.GetUpTileGood();
                List<uint> preuptilegood = PubMaster.DevConfig.GetUpTilePreGood();

                if (nowuptilegood != null && nowuptilegood.Count > 0)
                {
                    List<Stock> nowlist = stocks.FindAll(c => nowuptilegood.Exists(n => n == c.goods_id));
                    afterlist.AddRange(nowlist);
                    stocks.RemoveAll(c => nowuptilegood.Exists(n => n == c.goods_id));
                }

                if (preuptilegood != null && preuptilegood.Count > 0 && stocks.Count > 0)
                {
                    List<Stock> prelist = stocks.FindAll(c => preuptilegood.Exists(n => n == c.goods_id));
                    afterlist.AddRange(prelist);
                    stocks.RemoveAll(c => preuptilegood.Exists(n => n == c.goods_id));
                }
            }

            afterlist.AddRange(stocks);
            return afterlist;
        }

        /// <summary>
        /// 检查轨道是否能够添加对应数量的库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="stockqty"></param>
        /// <param name="ableqty"></param>
        /// <returns></returns>
        public bool CheckCanAddStockQty(uint trackid, uint goodid, byte stockqty, out int ableqty, out string result)
        {
            result = null;
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track != null)
            {
                ushort loc = 0;
                //计算可存放位置
                Stock buttomStock = GetTrackButtomStock(track.id);
                if (buttomStock != null)
                {
                    loc = buttomStock.location;
                }
                else
                {
                    //如果入轨道没有库存，则分割点为起点进行计算
                    if (track.InType(TrackTypeE.储砖_入))
                    {
                        loc = track.is_give_back ? track.limit_point : track.split_point;
                    }
                    else
                    {
                        loc = track.is_give_back ? track.limit_point : track.limit_point_up;
                    }
                }

                if (loc > 0)
                {
                    //ushort car = PubMaster.DevConfig.GetCarrierLenghtByArea(track.area);
                    //ushort safe = GetGoodsSafeDis(buttomStock.goods_id);
                    //// 当砖间距比小车顶板小，用顶板长度更安全
                    //safe = car > safe ? car : safe;
                    ushort safe = GetStackSafe(goodid);
                    if (safe > 0)
                    {
                        int count;

                        //出库轨道的分割点为放砖极限点
                        if (track.InType(TrackTypeE.储砖_出))
                        {
                            count = (track.is_give_back ? (track.limit_point_up - loc) : (loc - track.split_point)) / safe;
                        }
                        else
                        {
                            count =(int)Math.Truncate(((double)(track.is_give_back ? (track.limit_point_up - loc) : (loc - track.limit_point)) / safe)); 
                        }
                        ableqty = count;
                        if (count < stockqty)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }

            ableqty = -1; //找不到轨道
            return false;
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
        /// <param name="isonlyup">是否是上砖库存</param>
        /// <returns></returns>
        public List<StockSum> GetStockSums(int areaid)
        {
            if (areaid == 0) return StockSumList;
            return StockSumList.FindAll(c => c.area == areaid).ToList();
        }

        public int GetUpStocks(uint trackid)
        {
            int uppoint = PubMaster.Track.GetUpPoint(trackid);
            return StockList.Count(c => c.track_id == trackid && c.location >= uppoint);
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
                && (c.TrackType == TrackTypeE.储砖_出 || c.TrackType == TrackTypeE.储砖_出入) && c.PosType == StockPosE.头部).Select(t => t.goods_id).ToList();
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

        public int GetSizeWidth(uint size_id)
        {
            return GoodSizeList.Find(c => c.id == size_id)?.width ?? 0;
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
            if (goods_id == 0)
            {
                size = GoodSizeList.Find(c => c.car_lenght > 0 || c.car_space > 0);
            }

            if (size != null)
            {
                return (ushort)(size.car_lenght + size.car_space);
            }
            return 0;
        }

        /// <summary>
        /// 获取品种的规格ID
        /// </summary>
        /// <param name="Goods_id"></param>
        /// <returns></returns>
        public uint GetGoodsSizeID(uint Goods_id)
        {
            return GoodsList.Find(c => c.id == Goods_id)?.size_id ?? 0;
        }

        private bool IsHaveGoodInName(string name)
        {
            return GoodsList.Exists(c => name.Equals(c.name));
        }

        public string GetGoodsSizeName(uint Goods_id)
        {
            GoodSize size = GetGoodSize(Goods_id);
            if (size != null)
            {
                return size.name;
            }
            return "";
        }
        #endregion

        #region[库存]

        public uint GetTrackStock(uint trackid)
        {
            return StockSumList.Find(c => c.track_id == trackid)?.stack ?? 0;
        }

        /// <summary>
        /// 手动添加轨道库存
        /// </summary>
        /// <param name="tileid">砖机ID</param>
        /// <param name="trackid">轨道ID</param>
        /// <param name="goodsid">品种ID</param>
        /// <param name="pieces">片数</param>
        /// <param name="produceTime">生产时间</param>
        /// <param name="stockqty">车数</param>
        /// <param name="memo">备注</param>
        /// <param name="rs">添加结果</param>
        /// <returns></returns>
        public bool AddTrackStocks(uint tileid, uint trackid, uint goodsid, byte pieces, DateTime? produceTime, byte stockqty, string memo, out string rs)
        {
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    // 时间判断
                    if (produceTime != null && !PubMaster.Goods.IsAllowToOperateStock(trackid, goodsid, (DateTime)produceTime, out rs))
                    {
                        return false;
                    }

                    Goods addgood = GetGoods(goodsid);
                    if (addgood == null || addgood.empty)
                    {
                        rs = "添加的品种不能为空品种！";
                        return false;
                    }

                    #region[计算模拟库存位置]
                    uint StockId = 0;//库存ID
                    int maxaddcount = 0;//最大可添加库存数量
                    ushort nextstockloc = 0; //下一个库存计算存放位置
                    ushort safe = (ushort)PubMaster.Dic.GetDtlDouble(DicTag.StackPluse, 217);//统计出来的(实际库存位置差平均值)

                    Track track = PubMaster.Track.GetTrack(trackid);
                    if (track != null)
                    {
                        //计算可存放位置
                        Stock buttomStock = GetTrackButtomStock(trackid);

                        // (默认)前进存砖 下一车脉冲变小；后退存砖 下一车脉冲变大；
                        if (buttomStock != null)
                        {
                            int loc = 0;
                            if (track.InType(TrackTypeE.储砖_出))
                            {
                                loc = track.is_give_back ? (track.limit_point_up - buttomStock.location) : (buttomStock.location - track.split_point);
                            }
                            else
                            {
                                loc = track.is_give_back ? (track.limit_point_up - buttomStock.location) : (buttomStock.location - track.limit_point);
                            }

                            maxaddcount = loc / safe;
                            if (maxaddcount > 0)
                            {
                                nextstockloc = (ushort)(track.is_give_back ? (buttomStock.location + safe) : (buttomStock.location - safe));
                            }
                        }
                        else
                        {
                            if (track.InType(TrackTypeE.储砖_入))
                            {
                                nextstockloc = track.is_give_back ? track.limit_point : track.split_point;
                            }
                            else
                            {
                                nextstockloc = track.is_give_back ? track.limit_point : track.limit_point_up;
                            }
                        }
                    }
                    #endregion

                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("【添加库存-{0}】轨道[ {1} ], 数量[ {2} ], 品种[ {3} ], 片数[ {4} ], 时间[ {5} ], ID们:",
                        memo, track.name, stockqty, addgood.info, pieces, produceTime?.ToString(timeformat)));

                    //手动添加库存并计算脉冲
                    for (int i = 0; i < stockqty; i++)
                    {
                        StockId = AddStock(tileid, trackid, goodsid, pieces, produceTime);
                        builder.Append(StockId + ", ");
                        if (nextstockloc > 0)
                        {
                            UpdateStockLocation(StockId, nextstockloc);
                            if (track.is_give_back)
                            {
                                nextstockloc += safe;
                            }
                            else
                            {
                                nextstockloc -= safe;
                            }
                            
                        }
                    }

                    CheckStockTop(trackid);
                    CheckTrackSum(trackid);

                    //判断是否能继续放砖
                    if (IsTrackFull(trackid, (track.is_give_back ? track.limit_point_up : track.limit_point), safe, track.is_give_back))
                    {
                        PubMaster.Track.UpdateStockStatus(trackid, TrackStockStatusE.满砖, memo);
                    }
                    else
                    {
                        PubMaster.Track.UpdateStockStatus(trackid, TrackStockStatusE.有砖, memo);
                    }
                    _mlog.Status(true, builder.ToString());
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

        /// <summary>
        /// 获取入库端最后的库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public Stock GetTrackButtomStock(uint trackid)
        {
            Stock stock = null;
            // 是否是同侧上下轨道
            bool isSameSide = PubMaster.Track.IsSameSideTrack(trackid);
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid && c.PosType == (isSameSide ? StockPosE.头部 : StockPosE.尾部));
            if (stocks == null || stocks.Count ==0)
            {
                List<Stock> list = StockList.FindAll(c => c.track_id == trackid);
                if (list.Count > 0)
                {
                    list.Sort((x, y) => x.pos.CompareTo(y.pos));
                    stock = list[0];
                }
            }
            else
            {
                if(stocks.Count >1)
                    stocks.Sort((x, y) => x.location.CompareTo(y.location));
                stock = stocks[stocks.Count - 1];
            }
            return stock;
        }

        /// <summary>
        /// 获取轨道最小脉冲库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public Stock GetStockInLocMin(uint trackid)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
            if (stocks == null || stocks.Count == 0)
            {
                return null;
            }
            else
            {
                if (stocks.Count > 1) stocks.Sort((x, y) => x.location.CompareTo(y.location));
                return stocks[0];
            }
        }

        /// <summary>
        /// 获取轨道最大脉冲库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public Stock GetStockInLocMax(uint trackid)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
            if (stocks == null || stocks.Count == 0)
            {
                return null;
            }
            else
            {
                if (stocks.Count > 1) stocks.Sort((x, y) => y.location.CompareTo(x.location));
                return stocks[0];
            }
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

        /// <summary>
        /// 获取指定砖机轨道等待库存ID
        /// </summary>
        /// <param name="trackid">砖机轨道</param>
        /// <param name="tileid">砖机ID</para>
        /// <param name="gid">品种ID</para>
        /// <param name="givemeone">对地要求没有就早一个</para>
        /// <returns></returns>
        public uint GetStockInTileTrack(uint trackid, uint tileid, uint gid, bool givemeone = false)
        {
            Stock stock = null;
            if (tileid > 0)
            {
                if(gid > 0)
                {
                    stock = StockList.Find(c => c.track_id == trackid && c.tilelifter_id == tileid && c.goods_id == gid);
                }
                else
                {
                    stock = StockList.Find(c => c.track_id == trackid && c.tilelifter_id == tileid);
                }
            }
            else
            {
                stock = StockList.Find(c => c.track_id == trackid);
            }

            if(givemeone && stock == null)
            {
                stock = StockList.Find(c => c.track_id == trackid);
            }
            return stock?.id ?? 0;
        }

        /// <summary>
        /// 【手动添加库存用】根据库存位置判断轨道是否满
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public bool IsTrackFull(uint trackid, uint limitpoint, ushort safe, bool isgiveback)
        {
            // 计算可存放位置
            Stock buttomStock = GetTrackButtomStock(trackid);
            if (buttomStock != null)
            {
                uint count = (isgiveback ? (limitpoint - buttomStock.location) : (buttomStock.location - limitpoint)) / safe;
                if (count <= 0)
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 获取最早的品种库存
        /// </summary>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public uint GetTheEarliestStock(uint goodid)
        {
            uint trackid = 0;
            List<Stock> stklist = StockList.FindAll(c => c.goods_id == goodid);
            if (stklist != null && stklist.Count > 0)
            {
                stklist.Sort(
                    (x, y) =>
                    {
                        if (x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                        {
                            return xtime.CompareTo(ytime);
                        }
                        return 0;
                    }
                );
                trackid = stklist[0].track_id;
            }

            return trackid;
        }

        /// <summary>
        /// 获取最晚的品种库存
        /// </summary>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public uint GetTheLatestStock(uint goodid)
        {
            uint trackid = 0;
            List<Stock> stklist = StockList.FindAll(c => c.goods_id == goodid);
            if (stklist != null && stklist.Count > 0)
            {
                stklist.Sort(
                    (x, y) =>
                    {
                        if (x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                        {
                            return ytime.CompareTo(xtime);
                        }
                        return 0;
                    }
                );
                trackid = stklist[0].track_id;
            }

            return trackid;
        }

        /// <summary>
        /// 获取指定轨道类型最早的品种库存
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="goodid"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public Stock GetTheEarliestStock(uint goodid, params TrackTypeE[] types)
        {
            Stock stk = null;
            List<Stock> stklist = StockList.FindAll(c => c.goods_id == goodid && types.Contains(c.TrackType));
            if (stklist != null && stklist.Count > 0)
            {
                stklist.Sort(
                    (x, y) =>
                    {
                        if (x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                        {
                            return xtime.CompareTo(ytime);
                        }
                        return 0;
                    }
                );
                stk = stklist[0];
            }

            return stk;
        }

        /// <summary>
        /// 获取指定轨道类型最晚的品种库存
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="goodid"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public Stock GetTheLatestStock(uint goodid, params TrackTypeE[] types)
        {
            Stock stk = null;
            List<Stock> stklist = StockList.FindAll(c => c.goods_id == goodid && types.Contains(c.TrackType));
            if (stklist != null && stklist.Count > 0)
            {
                stklist.Sort(
                    (x, y) =>
                    {
                        if (x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                        {
                            return ytime.CompareTo(xtime);
                        }
                        return 0;
                    }
                );
                stk = stklist[0];
            }

            return stk;
        }

        /// <summary>
        /// 是否允许操作库存（时间判断）
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodid"></param>
        /// <param name="producetime"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool IsAllowToOperateStock(uint trackid, uint goodid, DateTime producetime, out string result)
        {
            result = "";
            Track tra = PubMaster.Track.GetTrack(trackid);
            Stock stk = null;
            switch (tra.Type)
            {
                case TrackTypeE.储砖_入:
                    // 时间要比出库侧都晚
                    stk = GetTheLatestStock(goodid, TrackTypeE.储砖_出);
                    if (stk != null && stk.produce_time != null && producetime != null
                        && DateTime.Compare((DateTime)stk.produce_time, (DateTime)producetime) > 0)
                    {
                        result = "生产时间要比所有出库侧轨道同品种库存都晚";
                        return false;
                    }
                    break;
                case TrackTypeE.储砖_出:
                    // 时间要比入库侧都早
                    stk = GetTheEarliestStock(goodid, TrackTypeE.储砖_入);
                    if (stk != null && stk.produce_time != null && producetime != null
                        && DateTime.Compare((DateTime)stk.produce_time, (DateTime)producetime) < 0)
                    {
                        result = "生产时间要比所有入库侧轨道同品种库存都早";
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        #endregion

        #endregion

        #region[增删改]

        #region[品种]
        public bool AddGoods(Goods good, out string result, out uint gid)
        {
            gid = 0;
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
                gid = goodid;
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

        public bool ChangeStockGood(uint trackid, uint goodid, bool changedate, DateTime? newdate, out string res)
        {
            res = "";
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    // 时间判断
                    if (newdate != null && !PubMaster.Goods.IsAllowToOperateStock(trackid, goodid, (DateTime)newdate, out res))
                    {
                        return false;
                    }

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
                    SendTrackStockQtyChangeMsg(trackid);
                    return true;
                }
                finally { Monitor.Exit(_so); }
            }
            return false;
        }

        /// <summary>
        /// 修改一个库存的品种（和生产时间）
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodid"></param>
        /// <param name="changedate"></param>
        /// <param name="newdate"></param>
        /// <returns></returns>
        public bool ChangeOneStock(uint stockid, uint trackid, uint goodid, bool changedate , DateTime? newdate)
        {
            if (Monitor.TryEnter(_so, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    Stock stock = StockList.Find(c => c.id == stockid);
                    if (stock == null)
                    {
                        return false;
                    }
                    if (changedate && newdate != null)
                    {
                        stock.produce_time = newdate;
                    }
                    stock.goods_id = goodid;
                    PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Goods);

                    RemoveTrackSum(trackid);
                    CheckTrackSum(trackid);
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
                SendSumMsg(sums[0], ActionTypeE.Delete);
                sums[0].goods_id = goodid;
                sums[0].produce_time = GetEarliestTime(trackid);
                SendSumMsg(sums[0], ActionTypeE.Update);

                SendTrackStockQtyChangeMsg(trackid);
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

                SendTrackStockQtyChangeMsg(trackid);
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

        /// <summary>
        /// 获取库存状态信息
        /// </summary>
        /// <param name="stock_id"></param>
        /// <returns></returns>
        public string GetStockInfo(uint stock_id)
        {
            return StockList.Find(c => c.id == stock_id)?.ToString() ?? stock_id + "";
        }

        /// <summary>
        /// 获取库存简易信息
        /// </summary>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public string GetStockSmallInfo(uint stockid)
        {
            return StockList.Find(c => c.id == stockid)?.ToSmalString() ?? stockid + "";
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
            if (Monitor.TryEnter(_go, TimeSpan.FromSeconds(2)))
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

        /// <summary>
        /// 手动删除库存
        /// </summary>
        /// <param name="stockid"></param>
        /// <param name="rs"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public bool DeleteStock(uint stockid, out string rs, string memo = "删除库存")
        {
            Stock stock = StockList.Find(c => c.id == stockid);
            if (stock == null)
            {
                rs = "找不到库存记录";
                return false;
            }

            if (PubMaster.DevConfig.GetCarrierByStockid(stock.id, out string carname))
            {
                rs = string.Format("序号{0}的库存绑定在{1}运输车上，请让运输车放下砖后，再来删除库存！", stock.pos, carname);
                return false;
            }

            StockList.Remove(stock);
            PubMaster.Mod.GoodSql.DeleteStock(stock);
            try
            {
                AddStockLog(string.Format("【删除库存】库存[ {0} ], 轨道[ {1} ], 备注[ {2} ]", stock.ToString()
                    , PubMaster.Track.GetTrackName(stock.track_id)
                    , memo));
            }
            catch { }
            StockSumChange(stock, 0);
            SendTrackStockQtyChangeMsg(stock.track_id);

            if (stock.PosType == StockPosE.头部)
            {
                CheckStockTop(stock.track_id);
            }

            if (stock.PosType == StockPosE.尾部)
            {
                CheckStockBottom(stock.track_id);
            }

            if (!ExistStockInTrack(stock.track_id))
            {
                PubMaster.Track.UpdateStockStatus(stock.track_id, TrackStockStatusE.空砖, memo);
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
                SendTrackStockQtyChangeMsg(s.track_id);
            }

        }

        /// <summary>
        /// 根据运输车报警删除多余的库存信息
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="site"></param>
        /// <param name="memo"></param>
        public void DeleteStockBySite(uint trackid, ushort site, string memo)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
            if (stocks == null || stocks.Count == 0)
            {
                return;
            }

            foreach (Stock s in stocks)
            {
                if (s.location < site)
                {
                    continue;
                }
                if (s.location == 0)
                {
                    if (s.location_cal != 0 && s.location_cal < site)
                    {
                        continue;
                    }
                }

                if (PubMaster.DevConfig.GetCarrierByStockid(s.id, out string carname))
                {
                    try
                    {
                        AddStockLog(string.Format("【删除库存】【删除失败，库存已绑定{3}运输车】- 库存[ {0} ], 轨道[ {1} ], 备注[ {2} ]", s.ToString()
                            , PubMaster.Track.GetTrackName(s.track_id)
                            , memo, carname));
                    }
                    catch { }
                    continue;
                }

                StockList.Remove(s);
                PubMaster.Mod.GoodSql.DeleteStock(s);
                try
                {
                    AddStockLog(string.Format("【删除库存】库存[ {0} ], 轨道[ {1} ], 备注[ {2} ]", s.ToString()
                        , PubMaster.Track.GetTrackName(s.track_id)
                        , memo));
                }
                catch { }
                StockSumChange(s, 0);
                SendTrackStockQtyChangeMsg(s.track_id);
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

                    Track givetrack = PubMaster.Track.GetTrack(givetrackid);
                    Track taketrack = PubMaster.Track.GetTrack(taketrackid);
                    if (givetrack != null && taketrack != null)
                    {
                        PubMaster.Track.ShiftTrack(taketrack.id, givetrack.id);
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
        public bool GetStock(uint areaid, ushort lineid, uint tilelifterid, uint goodsid, out List<Stock> allocatstocks)
        {
            allocatstocks = new List<Stock>();

            #region 时间判断
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableStockTimeForUp))
            {
                // 当最早的库存在入库侧，停止作业且报警
                Stock stk = GetTheEarliestStock(goodsid, TrackTypeE.储砖_入, TrackTypeE.储砖_出);
                if (stk != null && stk.TrackType == TrackTypeE.储砖_入)
                {
                    PubMaster.Warn.AddTaskWarn(areaid, lineid, WarningTypeE.TheEarliestStockInDown, (ushort)tilelifterid, tilelifterid, 
                        string.Format("[ {0} ]最早的库存在[ {1} ]", 
                            GetGoodsName(stk.goods_id), PubMaster.Track.GetTrackName(stk.track_id)));
                    return false;
                }
            }
            PubMaster.Warn.RemoveTaskWarn(WarningTypeE.TheEarliestStockInDown, tilelifterid);
            #endregion

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

            //1.找到上砖机配置的轨道
            List<AreaDeviceTrack> devtrack = PubMaster.Area.GetAreaDevTraList(areaid, tilelifterid);


            //2.根据优先级查看非空且是需求的品种的轨道
            List<Stock> stocks = StockList.FindAll(c => c.goods_id == goodsid
                                                    && c.PosType == StockPosE.头部
                                                    && devtrack.Exists(d => d.track_id == c.track_id)
                                                    && (!isnotuseupsplitstock 
                                                        || (isnotuseupsplitstock && PubMaster.Track.CheckStocksTrack(c.track_id))));

            if (stocks.Count == 0)
            {
                //找不到库存
                return false;
            }

            stocks.Sort((x, y) => x.pos.CompareTo(y.pos));

            // 找时间最早的库存,   同侧找最晚的
            bool isSameSide = PubMaster.Track.IsSameSideTrack(stocks[0].track_id);
            stocks.Sort(
                (x, y) => 
                {
                    if(x.produce_time is DateTime xtime && y.produce_time is DateTime ytime)
                    {
                        if (isSameSide)
                        {
                            return ytime.CompareTo(xtime);
                        }
                        else
                        {
                            return xtime.CompareTo(ytime);
                        }
                    }
                    return 0;
                }
            );

            int minStockTime = PubMaster.Dic.GetDtlIntCode(DicTag.MinStockTime);
            foreach (Stock stock1 in stocks)
            {
                double dirhours = (DateTime.Now - (DateTime)stock1.produce_time).TotalHours;
                if (minStockTime == 0 || dirhours < 0 || dirhours >= minStockTime)
                {
                    if (PubMaster.Track.IsNotEmtyp4Up(stock1.track_id))
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

            return stocks.FindAll(c => PubMaster.Track.IsNotEmtyp4Up(c.track_id));
        }

        /// <summary>
        /// 将库存转移到指定的轨道
        /// </summary>
        /// <param name="stock_id">库存ID</param>
        /// <param name="to_track_id">被转移到的轨道ID</param>
        /// <param name="fromtrans">调用方法来自任务逻辑</param>
        public void MoveStock(uint stock_id, uint to_track_id, bool fromtrans = true, string memo = "", uint devid = 0)
        {
            //屏蔽任务逻辑里面的调用
            if (fromtrans) return;

            Stock stock = StockList.Find(c => c.id == stock_id);
            if (stock != null && stock.track_id != to_track_id && to_track_id > 0)
            {
                uint from_track_id = stock.track_id;

                //更新库存统计信息
                StockSumChange(stock, to_track_id);

                //更新轨道被转移后的轨道信息(区域，轨道ID，轨道类型)
                Track totrack = PubMaster.Track.GetTrack(to_track_id);
                Track fromtrack = PubMaster.Track.GetTrack(from_track_id);

                #region[更新库存位置]

                if (totrack != null)
                {
                    stock.last_track_id = stock.track_id;
                    stock.track_id = to_track_id;
                    stock.area = totrack.area;
                    stock.track_type = totrack.type;
                    PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Track);
                }

                #endregion

                #region[更新储砖轨道]

                //将库存 移入 储砖轨道
                if (totrack != null && totrack.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入))
                {
                    if (fromtrack.InType(TrackTypeE.摆渡车_出))
                    {
                        UpdateTrackPos(stock, totrack, false);
                    }
                    else
                    {
                        UpdateTrackPos(stock, totrack);
                    }
                    PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Pos);

                    if (totrack.StockStatus == TrackStockStatusE.空砖)
                    {
                        PubMaster.Track.UpdateStockStatus(to_track_id, TrackStockStatusE.有砖, memo);
                    }

                    //if (!CheckCanAddStockQty(totrack.id, stock.goods_id, 1, out int _, out string _))
                    //{
                    //    PubMaster.Track.UpdateStockStatus(to_track_id, TrackStockStatusE.满砖, memo);
                    //}
                }

                //从储砖轨道 移出 库存
                if (fromtrack != null && fromtrack.IsStoreTrack())
                {
                    CheckStockTop(from_track_id);

                    if (fromtrack.StockStatus == TrackStockStatusE.满砖
                        && fromtrack.Type == TrackTypeE.储砖_出)
                    {
                        PubMaster.Track.UpdateStockStatus(from_track_id, TrackStockStatusE.有砖, memo);
                    }

                    if (!ExistStockInTrack(from_track_id))
                    {
                        PubMaster.Track.UpdateStockStatus(from_track_id, TrackStockStatusE.空砖, memo);
                    }
                }
                #endregion

                try
                {
                    AddStockLog(string.Format("【转移】轨道[ {0} -> {1} ], 库存[ {2} ], 备注[ {3} ]",
                        fromtrack?.name ?? from_track_id + "", totrack?.name ?? to_track_id + "", stock.ToString(), memo));
                }
                catch { }

                #region[清理摆渡车遗留库存信息]

                //【先不启用】
                //if (fromtrack.Type == TrackTypeE.摆渡车_出 
                //    || fromtrack.Type == TrackTypeE.摆渡车_入)
                //{
                //    ClearFerryTrackStocks(fromtrack.id);
                //}

                #endregion


                SendTrackStockQtyChangeMsg(from_track_id);
                SendTrackStockQtyChangeMsg(to_track_id);
            }
        }

        public bool IsStockQtyCompare(uint trackid, ushort campareqty)
        {
            return StockList.Count(c => c.track_id == trackid) >= campareqty;
        }


        /// <summary>
        /// 查找是否存在极限位的库存信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fullpos">满砖数、下砖轨道极限库存位置</param>
        /// <returns></returns>
        public bool ExistStockInTrackPos(uint id, ushort fullpos)
        {
            if (StockList.Exists(c => c.track_id == id && c.pos >= fullpos))
            {
                return true;
            }

            ushort stockqty = (ushort)StockList.Count(c => c.track_id == id);
            return stockqty == fullpos;
        }

        /// <summary>
        /// 更新库存的位置信息(pos)
        /// </summary>
        /// <param name="stock">库存信息</param>
        /// <param name="track">库存所在轨道</param>
        /// <param name="addinbottom">默认添加在尾部</param>
        public void UpdateTrackPos(Stock stock, Track track, bool addinbottom = true)
        {
            //轨道当前库存信息
            short storecount = (short)StockList.Count(c => c.track_id == stock.track_id && c.id != stock.id);

            if (addinbottom)
            {
                if (storecount == 0)
                {
                    stock.PosType = StockPosE.头部;
                    stock.pos = (short)(track.is_give_back ? 50 : 0);
                }
                else
                {
                    //如轨道是同向出入，则将后面添加的库存放在第一位
                    if (track.is_give_back)
                    {
                        Stock topStock = GetTrackTopStock(stock.track_id);
                        if (topStock != null)
                        {
                            SetStockPosType(topStock, StockPosE.中部);
                        }

                        short FinalStockPos = StockList.FindAll(c => c.track_id == stock.track_id && c.id != stock.id)?.Min(c => c.pos) ?? 0;
                        stock.pos = (short)(FinalStockPos - 1);
                        stock.PosType = StockPosE.头部;
                    }
                    else
                    {
                        List<Stock> Bottomstock = StockList.FindAll(c => c.track_id == stock.track_id && c.PosType == StockPosE.尾部);
                        if (Bottomstock != null && Bottomstock.Count > 0)
                        {
                            foreach (var item in Bottomstock)
                            {
                                SetStockPosType(item, StockPosE.中部);
                            }
                        }

                        short FinalStockPos = StockList.FindAll(c => c.track_id == stock.track_id && c.id != stock.id)?.Max(c => c.pos) ?? 0;
                        stock.pos = (short)(FinalStockPos + 1);
                        stock.PosType = StockPosE.尾部;
                    }
                }
            }
            else
            {
                //加在头部
                Stock top = StockList.Find(c => c.track_id == stock.track_id && c.PosType == StockPosE.头部
                                                && c.id != stock.id);

                stock.PosType = StockPosE.头部;
                if (top != null && top.id != stock.id)
                {
                    stock.pos = (short)(top.pos - 1);

                    top.PosType = StockPosE.中部;
                    PubMaster.Mod.GoodSql.EditStock(top, StockUpE.PosType);
                }
                else
                {
                    stock.pos = (short)(track.is_give_back ? 50 : 1);
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
        /// 判断小车绑定的库存品种是不是与任务需要的品种一致
        /// </summary>
        /// <param name="carrier_id"></param>
        /// <param name="goods_id"></param>
        /// <returns></returns>
        public bool IsStockGoodDif(uint carrier_id, uint goods_id)
        {
            uint stockid = PubMaster.DevConfig.GetCarrierStockId(carrier_id);
            if(stockid !=0 )
            {
                uint stockgid = GetStockGoodId(stockid);
                if(stockgid != 0 && stockgid != goods_id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取库存的品种信息
        /// </summary>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public uint GetStockGoodId(uint stockid)
        {
            return StockList.Find(c => c.id == stockid)?.goods_id ?? 0;
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
        /// 删除砖机轨道多余的库存信息
        /// </summary>
        /// <param name="tileid">砖机ID</param>
        /// <param name="tiletrackid">砖机轨道</param>
        /// <param name="goodid">品种ID</param>
        public void RemoveTileTrackOtherStock(uint tileid, uint tiletrackid, uint goodid)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == tiletrackid && c.tilelifter_id == tileid && c.goods_id != goodid);
            if (stocks == null || stocks.Count == 0) return;
            string trackname = PubMaster.Track.GetTrackName(tiletrackid);
            foreach (Stock item in stocks)
            {
                PubMaster.Mod.GoodSql.DeleteStock(item);
                AddStockLog(string.Format("【删除砖机多余库存】轨道[ {0} ], 库存[ {1} ]", trackname, item.ToString()));
            }
            StockList.RemoveAll(c => c.track_id == tiletrackid && c.tilelifter_id == tileid && c.goods_id != goodid);
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
                    stocks.Sort((x, y) => y.pos.CompareTo(x.pos));
                    SetStockPosType(stocks[0], StockPosE.尾部);
                    return stocks[0].id;
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

        /// <summary>
        /// 更新库存实际卸货坐标
        /// </summary>
        /// <param name="stock_id"></param>
        /// <param name="giveSite"></param>
        public void UpdateStockLocation(uint stock_id, ushort loc)
        {
            Stock stock = GetStock(stock_id);
            if (stock != null && stock.location != loc)
            {
                stock.location = loc;
                PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Location);
            }
        }

        /// <summary>
        /// 更新库存计算卸货坐标
        /// </summary>
        /// <param name="stock_id"></param>
        /// <param name="giveSite"></param>
        public void UpdateStockLocationCal(uint stock_id, ushort loc)
        {
            Stock stock = GetStock(stock_id);
            if (stock != null && stock.location_cal != loc)
            {
                stock.location_cal = loc;
                PubMaster.Mod.GoodSql.EditStock(stock, StockUpE.Location);
            }
        }


        /// <summary>
        /// 在储砖轨道获取库存信息返回给小车绑定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="takePoint"></param>
        /// <param name="takeSite"></param>
        /// <returns></returns>
        public uint GetStockInStoreTrack(Track track, ushort point)
        {
            //查找脉冲范围内的库存信息
            List<Stock> stocks = StockList.FindAll(c => c.track_id == track.id && c.IsInLocation(point, 250));//250脉冲 ≈ 250*(1.736) = 434.8cm
            if (stocks.Count == 1)
            {
                //找到范围内唯一的库存
                return stocks[0].id;
            }
            else if(stocks.Count > 1)
            {
                stocks.Sort((x, y) => y.location.CompareTo(x.location));
                return stocks[0].id;
            }

            //如果在储砖出、或者入的轨道找不到库存则在兄弟轨道查找该脉冲范围内有没有库存的信息
            if(track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出))
            {
                stocks = StockList.FindAll(c => c.track_id == track.brother_track_id && c.IsInLocation(point, 250));//150脉冲 ≈ 150*(1.736)=86.8cm
                if (stocks.Count == 1)
                {
                    //找到范围内唯一的库存
                    return stocks[0].id;
                }
                else if (stocks.Count > 1)
                {
                    stocks.Sort((x, y) => y.location.CompareTo(x.location));
                    return stocks[0].id;
                }
            }

            if(stocks.Count == 0)
            {
                //找不到范围内的库存信息
                stocks = StockList.FindAll(c => c.track_id == track.id);//找到范围内多个的库存 排序取最近的库存
            }

            if (stocks.Count > 0)
            {
                List<CalData> callist = new List<CalData>();
                foreach (var item in stocks)
                {
                    callist.Add(new CalData() { id = item.id, s_location = item.location,  s_data = (ushort)Math.Abs(item.location - point) });
                }
                callist.Sort((x, y) =>
                {
                    if(x.s_location > y.s_location && x.s_data > y.s_data && Math.Abs(x.s_data-y.s_data) < 250)
                    {
                        return -1;
                    }

                    return x.s_data.CompareTo(y.s_data);
                });
                ///callist.Sort((x, y) => x.s_data.CompareTo(y.s_data));

                return callist[0].id;
            }
            return 0;
        }

        /// <summary>
        /// 获取摆渡车上的库存信息返回给小车绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetStockInFerryTrack(uint trackid)
        {
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
            if(stocks.Count == 1)
            {
                return stocks[0].id;
            }
            else if(stocks.Count > 1)
            {
                stocks.Sort((x, y) => x.pos.CompareTo(y.pos));
                uint stockid = stocks[0].id;
                string trackname = PubMaster.Track.GetTrackName(trackid);
                for(int i =1; i< stocks.Count; i++)
                {
                    PubMaster.Mod.GoodSql.DeleteStock(stocks[i]);
                    AddStockLog(string.Format("【删除摆渡库存】轨道[ {0} ], 库存[ {1} ]", trackname, stocks[i].ToString()));
                }
                StockList.RemoveAll(c =>c.track_id == trackid && c.id != stockid && stocks.Exists(s => s.id == c.id));
            }

            return 0;
        }

        /// <summary>
        /// 是否存在库存使用该品种
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool HaveStockUseGood(uint id)
        {
            return StockList.Exists(c => c.goods_id == id);
        }

        /// <summary>
        /// 根据脉冲获取轨道库存列表
        /// </summary>
        /// <param name="traid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public List<Stock> GetStockListInPoint(uint traid, ushort point)
        {
            List<Stock> list = new List<Stock>();
            //查找脉冲范围内的库存信息
            List<Stock> stocks = StockList.FindAll(c => c.track_id == traid && c.IsInLocation(point, 250));//50脉冲 ≈ 50*(1.736)=86.8cm
            if (stocks.Count > 0)
            {
                stocks.Sort((x, y) => y.location.CompareTo(x.location));
                list.AddRange(stocks);
            }
            Track track = PubMaster.Track.GetTrack(traid);
            //如果在储砖出、或者入的轨道找不到库存则在兄弟轨道查找该脉冲范围内有没有库存的信息
            if (track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出))
            {
                stocks = StockList.FindAll(c => c.track_id == track.brother_track_id && c.IsInLocation(point, 250));//50脉冲 ≈ 50*(1.736)=86.8cm
                if (stocks.Count > 0)
                {
                    stocks.Sort((x, y) => y.location.CompareTo(x.location));
                    list.AddRange(stocks);
                }
            }

            if (stocks.Count == 0)
            {
                //找不到范围内的库存信息
                stocks = StockList.FindAll(c => c.track_id == track.id);//找到范围内多个的库存 排序取最近的库存
            }

            if (stocks.Count > 0)
            {
                List<CalData> callist = new List<CalData>();
                foreach (var item in stocks)
                {
                    callist.Add(new CalData() { id = item.id, s_location = item.location, s_data = (ushort)Math.Abs(item.location - point) });
                }
                callist.Sort((x, y) =>
                {
                    if (x.s_location > y.s_location && x.s_data > y.s_data && Math.Abs(x.s_data - y.s_data) < 120)
                    {
                        return -1;
                    }

                    return x.s_data.CompareTo(y.s_data);
                });
                ///callist.Sort((x, y) => x.s_data.CompareTo(y.s_data));

                foreach (var item in callist)
                {
                    list.Add(stocks.Find(c => c.id == item.id));
                }
            }
            return list;
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
            //1.摆渡车 -> 储砖轨道
            //2.入轨道 -> 出轨道
            if (PubMaster.Track.IsStoreTrack(totrackid))
            {
                ToStoreSumUpdate(totrackid, stock);
            }

            //该库存是否来源于储砖轨道
            //1.储砖轨道 -> 摆渡车
            //2.入轨道 -> 出轨道
            if (PubMaster.Track.IsStoreTrack(stock.track_id))
            {
                FromStoreSumUpdate(stock);
            }
        }

        /// <summary>
        /// 该库存是否去向储砖轨道
        /// 1.摆渡车 -> 储砖轨道
        /// 2.入轨道 -> 出轨道
        /// </summary>
        /// <param name="totrackid"></param>
        /// <param name="stock"></param>
        private void ToStoreSumUpdate(uint totrackid, Stock stock)
        {
            //库存放置 => 储砖轨道(加)
            StockSum sum = StockSumList.Find(c => c.track_id == totrackid && c.goods_id == stock.goods_id);
            if (sum == null)
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
            else
            {
                sum.count += 1;
                sum.stack += stock.stack;
                sum.pieces += stock.pieces;
                SendSumMsg(sum, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 该库存是否来源于储砖轨道
        /// 1.储砖轨道 -> 摆渡车
        /// 2.入轨道 -> 出轨道
        /// </summary>
        /// <param name="stock"></param>
        private void FromStoreSumUpdate(Stock stock)
        {   //库存 从 储砖轨道 => 出(减)
            StockSum sum = StockSumList.Find(c => c.track_id == stock.track_id && c.goods_id == stock.goods_id);
            if (sum != null)
            {
                sum.count -= 1;
                sum.stack -= stock.stack;
                sum.pieces -= stock.pieces;

                if (sum.count == 0)
                {
                    StockSumList.Remove(sum);
                    SendSumMsg(sum, ActionTypeE.Delete);
                    return;
                }

                sum.produce_time = GetEarliestTime(sum.track_id);
                SendSumMsg(sum, ActionTypeE.Update);
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
        /// 轨道库存数量编号
        /// </summary>
        /// <param name="trackid"></param>
        private void SendTrackStockQtyChangeMsg(uint trackid)
        {
            Messenger.Default.Send(trackid, MsgToken.TrackStockQtyUpdate);
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

            SendTrackStockQtyChangeMsg(trackId);
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
        /// 1、找同品种未满入轨道
        /// 2、优先找出轨道同品种空的入轨道（不能连续两次下同一条轨道）
        /// 3、找空的出轨道对应空的入轨道
        /// 4、找出轨道入库时间最早时间的空入轨道
        /// </summary>
        /// <param name="areaid">下砖区域</param>
        /// <param name="devid">分配设备</param>
        /// <param name="goodsid">品种</param>
        /// <param name="traids">符合的轨道列表</param>
        /// <returns></returns>
        public bool AllocateGiveTrack(uint areaid, ushort lineid, uint devid, uint goodsid, out List<uint> traids)
        {
            List<AreaDeviceTrack> list = PubMaster.Area.GetAreaDevTraList(areaid, devid);
            traids = new List<uint>();
            ///
            List<TrackStoreCount> trackstores = new List<TrackStoreCount>();//1.[同品种,未满] 入
            List<uint> same_out_good_in = new List<uint>(); //[同品种] 出 -   [空] 入
            List<uint> empty_out_and_in = new List<uint>(); //[空] 出 -   [空] 入
            List<uint> empty_in = new List<uint>();                 //[有] 出 - [空] 入

            List<uint> emptylist = new List<uint>();//所有空轨道列表

            uint storecount = 0;
            foreach (AreaDeviceTrack adt in list)
            {
                Track track = PubMaster.Track.GetTrack(adt.track_id);
                //是否是储砖轨道
                if (!track.InType(TrackTypeE.储砖_入, TrackTypeE.储砖_出入)) continue;

                //轨道是否启用
                if (!(track.AlertStatus == TrackAlertE.正常 
                        && (track.TrackStatus == TrackStatusE.启用 || track.TrackStatus == TrackStatusE.仅下砖))) continue;

                //轨道满否
                if (track.StockStatus == TrackStockStatusE.满砖) continue;

                //是否已存同品种并且未满
                if (IsTrackFineToStore(adt.track_id, goodsid, out storecount))
                {
                    /// 1、找同品种未满入轨道
                    trackstores.Add(new TrackStoreCount()
                    {
                        trackid = adt.track_id,
                        storecount = storecount
                    });
                }

                //[可以放任何品种] 空轨道，轨道没有库存
                if (track.StockStatus == TrackStockStatusE.空砖
                    && IsTrackStockEmpty(adt.track_id)
                    && IsTrackOkForGoods(adt.track_id, goodsid))
                {
                    #region[出入轨道]
                    if (track.Type == TrackTypeE.储砖_出入)
                    {
                        // 3、找空的出轨道对应空的入轨道
                        empty_in.Add(adt.track_id);
                        continue;
                    }
                    #endregion

                    #region[入库轨道】
                    // 2、优先找出轨道同品种空的入轨道（不能连续两次下同一条轨道）
                    if (PubMaster.Goods.HaveGoodInTrack(track.brother_track_id, goodsid))
                    {
                        // [同品种] 出 -   [空] 入
                        same_out_good_in.Add(adt.track_id);
                    }
                    else if (PubMaster.Track.IsEmtpy(track.brother_track_id))
                    {
                        empty_out_and_in.Add(adt.track_id);
                    }
                    else
                    {
                        //[有] 出 - [空] 入
                        empty_in.Add(adt.track_id);
                    }
                    #endregion
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

            emptylist.AddRange(same_out_good_in);
            emptylist.AddRange(empty_out_and_in);
            emptylist.AddRange(empty_in);

            //排序空轨道
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableDownTrackOrder) 
                && emptylist.Count > 0)
            {
                emptylist = OrderEmptyTrackList(devid, list, emptylist);
            }

            traids.AddRange(emptylist);

            #region 下砖机不作业轨道报警判断
            // 同品种的空轨道不作业
            if (emptylist != null && emptylist.Count > 0)
            {
                // 是否停止作业且报警提示
                bool isopen = PubMaster.Dic.IsSwitchOnOff(DicTag.EnableStockTimeForDown);
                int count = emptylist.Count;

                uint NonWorkTrackid = PubMaster.DevConfig.GetNonWorkTrackId(devid);

                if (NonWorkTrackid > 0 && emptylist.Contains(NonWorkTrackid))
                {
                    Track nwTrack = PubMaster.Track.GetTrack(NonWorkTrackid);
                    if (nwTrack.Type == TrackTypeE.储砖_入
                        && nwTrack.recent_goodid == goodsid
                        && (count > 1 || (count == 1 && isopen))
                        )
                    {
                        traids.Remove(NonWorkTrackid);
                    }
                }

                if (isopen && traids.Count == 0)
                {
                    PubMaster.Warn.AddTaskWarn(areaid, lineid, WarningTypeE.PreventTimeConflict, (ushort)devid, devid,
                        string.Format("[ {0} ]不能连续下满[ {1} ]",
                            GetGoodsName(goodsid), PubMaster.Track.GetTrackName(NonWorkTrackid)));
                }
                else
                {
                    PubMaster.Warn.RemoveTaskWarn(WarningTypeE.PreventTimeConflict, devid);
                }
            }
            #endregion

            return traids.Count > 0;
        }

        /// <summary>
        /// 判断轨道是否存在该品种的库存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        private bool HaveGoodInTrack(uint id, uint goodsid)
        {
            return StockList.Exists(c => c.track_id == id && c.goods_id == goodsid);
        }

        private List<uint> OrderEmptyTrackList(uint devid, List<AreaDeviceTrack> list, List<uint> emptylist)
        {
            //是否按顺序放砖1->2->3->4的顺序
            //获取砖机最近的下砖轨道的优先级
            uint lasttrack = PubMaster.DevConfig.GetLastTrackId(devid);
            ushort lasttrackprior = PubMaster.Area.GetAreaDevTrackPrior(devid, lasttrack);

            if (lasttrackprior != 0)
            {
                //获取空轨道的优先级
                List<AreaDeviceTrack> emptyADTList = list.FindAll(c => emptylist.Contains(c.track_id));
                //按顺序保存轨道信息
                List<AreaDeviceTrack> emptyListLargeOrder = new List<AreaDeviceTrack>();
                List<AreaDeviceTrack> emptyListLessOrder = new List<AreaDeviceTrack>();
                List<uint> emptyListOrder = new List<uint>();

                if (emptylist.Contains(lasttrack))
                {
                    emptyListOrder.Add(lasttrack);
                }

                emptyListLargeOrder = emptyADTList.FindAll(c => c.prior > lasttrackprior);
                if (emptyListLargeOrder != null && emptyListLargeOrder.Count > 0)
                {
                    emptyListLargeOrder.Sort((x, y) => x.prior.CompareTo(y.prior));
                    emptyListOrder.AddRange(emptyListLargeOrder.Select(c => c.track_id));
                }

                emptyListLessOrder = emptyADTList.FindAll(c => c.prior < lasttrackprior);
                if (emptyListLessOrder != null && emptyListLessOrder.Count > 0)
                {
                    emptyListLessOrder.Sort((x, y) => x.prior.CompareTo(y.prior));
                    emptyListOrder.AddRange(emptyListLessOrder.Select(c => c.track_id));
                }

                emptylist = emptyListOrder;
            }

            return emptylist;
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
            //bool isok = StockList.Exists(c => c.track_id == trackid && c.goods_id == goodsid);
            //存在但不一定在轨道最后存放的库存
            uint gooid = PubMaster.Goods.GetLastStockGid(trackid);
            if (gooid == goodsid)
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

        public List<StockSum> GetStockSumsByDevId(uint tileid)
        {
            if (tileid == 0) return StockSumList;
            uint areaid = PubMaster.Device.GetDeviceArea(tileid);
            return StockSumList.FindAll(c => c.area == areaid).ToList();
        }
        #endregion

        #region [计算轨道存取坐标]

        /// <summary>
        /// 计算下一车轨道坐标
        /// </summary>
        /// <param name="tt">任务类型</param>
        /// <param name="carrierid">小车用于计算间距</param>
        /// <param name="trackid">放砖轨道</param>
        /// <param name="stockcount">库存数量</param>
        /// <param name="location">库存存放计算地标</param>
        /// <returns></returns>
        public bool CalculateNextLocation(TransTypeE tt, uint carrierid, uint trackid, out ushort stockcount, out ushort location)
        {
            bool isOK = false;
            stockcount = 0;
            location = 0;
            List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid);
            Track track = PubMaster.Track.GetTrack(trackid);
            if (stocks == null || stocks.Count == 0)
            {
                if (tt == TransTypeE.同向下砖)
                {
                    location = (track.Type == TrackTypeE.储砖_出 ? track.split_point : track.limit_point);
                }
                else
                {
                    location = (track.Type == TrackTypeE.储砖_入 ? track.split_point : track.limit_point);
                }
                isOK = true;
            }
            else
            {
                stockcount = (ushort)stocks.Count;
                Stock bottom = GetTrackButtomStock(trackid); ;
                ushort safe = GetStackSafe(bottom.goods_id, carrierid);
                ushort limit = 0;

                switch (tt)
                {
                    case TransTypeE.下砖任务:
                    case TransTypeE.手动下砖:
                        limit = track.limit_point;
                        location = (ushort)(bottom.location - safe);
                        if (location < limit)
                        {
                            location = 0;
                            break;
                        }
                        isOK = true;
                        break;

                    case TransTypeE.同向下砖:
                        limit = track.limit_point_up;
                        location = (ushort)(bottom.location + safe);
                        if (location > limit)
                        {
                            location = 0;
                            break;
                        }
                        isOK = true;
                        break;

                    default:
                        break;
                }
            }

            return isOK;
        }

        /// <summary>
        /// 获取前进/后退 存砖下一车坐标
        /// </summary>
        /// <param name="md"></param>
        /// <param name="carrierid"></param>
        /// <param name="trackid"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool CalculateNextLocByDir(DevMoveDirectionE md, uint carrierid, uint trackid, out ushort location)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            Stock stk; // 计算库存
            ushort safe = 0; // 安全间隔
            ushort limit = 0; // 轨道尽头
            switch (md)
            {
                case DevMoveDirectionE.前进:
                    stk = GetStockInLocMin(trackid);
                    if (stk == null)
                    {
                        location = (track.Type == TrackTypeE.储砖_入 ? track.split_point : track.limit_point_up);
                        return true;
                    }
                    safe = GetStackSafe(stk.goods_id, carrierid);
                    limit = (track.Type == TrackTypeE.储砖_出 ? track.split_point : track.limit_point);
                    location = (ushort)(stk.location - safe);
                    if (location < limit)
                    {
                        location = 0;
                        return false;
                    }
                    return true;

                case DevMoveDirectionE.后退:
                    stk = GetStockInLocMax(trackid);
                    if (stk == null)
                    {
                        location = (track.Type == TrackTypeE.储砖_出 ? track.split_point : track.limit_point);
                        return true;
                    }
                    safe = GetStackSafe(stk.goods_id, carrierid);
                    location = (track.Type == TrackTypeE.储砖_入 ? track.split_point : track.limit_point_up);
                    location = (ushort)(stk.location - safe);
                    if (location > limit)
                    {
                        location = 0;
                        return false;
                    }
                    return true;

                default:
                    location = 0;
                    return false;
            }

        }

        /// <summary>
        /// 获取库存间距脉冲，用于计算
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="goodid"></param>
        /// <returns></returns>
        public ushort GetStackSafe(uint goodid, uint carrierid = 0)
        {
            ushort safe = (ushort)PubMaster.Dic.GetDtlDouble(DicTag.StackPluse, 0);//217
            if(safe == 0)
            {
                ushort car = PubMaster.DevConfig.GetCarrierLenght(carrierid);
                safe = GetGoodsSafeDis(goodid);
                // 当砖间距比小车顶板小，用顶板长度更安全
                safe = car > safe ? car : safe;
            }

            return safe;
        }

        /// <summary>
        /// 获取前进/后退 取砖库存坐标
        /// </summary>
        /// <param name="md"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public ushort GetStockLocByDir(DevMoveDirectionE md, uint trackid)
        {
            ushort location = 0;
            Track track = PubMaster.Track.GetTrack(trackid);
            Stock stk; // 计算库存
            switch (md)
            {
                case DevMoveDirectionE.前进:
                    stk = GetStockInLocMin(trackid);
                    if (stk == null)
                    {
                        location = 0;
                        break;
                    }
                    location = stk.location;
                    break;

                case DevMoveDirectionE.后退:
                    stk = GetStockInLocMax(trackid);
                    if (stk == null)
                    {
                        location = 0;
                        break;
                    }
                    location = stk.location;
                    break;

            }
            return location;
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
		
		#region[添加日志|清空摆渡车轨道库存]
        /// <summary>
        /// 清空摆渡车轨道库存
        /// </summary>
        /// <param name="trackid"></param>
        private void ClearFerryTrackStocks(uint trackid)
        { 
            List<Stock> ferrystocks = StockList.FindAll(c => c.track_id == trackid);
            foreach (var item in ferrystocks)
            {
                AddStockLog(string.Format("【删除摆渡库存】品种[ {0} ], 库存[ {1} ]", PubMaster.Goods.GetGoodsName(item.goods_id), item.ToString()));
                PubMaster.Mod.GoodSql.DeleteStock(item);
            }
        }

        /// <summary>
        /// 添加库存日志
        /// </summary>
        /// <param name="log"></param>
        public void AddStockLog(string log)
        {
            try
            {
                _mlog.Status(true, log);
            }
            catch { }
        }
        #endregion

        #region[自动添加品种]


        /// <summary>
        /// 添加默认品种
        /// </summary>
        /// <param name="basegid"></param>
        /// <param name="ad_rs"></param>
        /// <param name="pgoodid"></param>
        /// <returns></returns>
        public bool AddDefaultGood(uint basegid, out string ad_rs, out uint pgoodid)
        {
            Goods ngood = GetGoods(basegid);
            Goods notusegood = GetNotUseGood(ngood);
            if (notusegood != null)
            {
                ad_rs = "";
                pgoodid = notusegood.id;
                return true;
            }
            string naddgname = GetPreAddName();

            string levelname = PubMaster.Dic.GetDtlStrCode(DicTag.GoodLevel, ngood.level);
            Goods pgood = new Goods()
            {
                name = naddgname,
                color = naddgname,
                memo = "自动生成",
                area_id = ngood.area_id,
                pieces = ngood.pieces,
                GoodCarrierType = ngood.GoodCarrierType,
                size_id = ngood.size_id,
                level = ngood.level,
                info = naddgname + "/" + naddgname + PubMaster.Goods.GetGoodSizeSimpleName(ngood.size_id, "/") + "/" + levelname
            };

            return AddGoods(pgood, out ad_rs, out pgoodid);
        }


        /// <summary>
        /// 查找库存中未被使用的品种
        /// </summary>
        /// <param name="ngood"></param>
        /// <returns></returns>
        public Goods GetNotUseGood(Goods ngood)
        {
            if (ngood == null) return null;
            for (int v = 65; v < 90; v++)
            {
                string vn = "" + (char)v;
                if (IsGoodNotUse(vn, ngood.size_id, ngood.level, out Goods good))
                {
                    return good;
                }
            }
            return null;
        }

        private bool IsGoodNotUse(string name, uint sizeid, byte level, out Goods good)
        {
            good = GoodsList.Find(c => name.Equals(c.name) && c.size_id == sizeid && c.level == level);
            if (good != null)
            {
                if (!PubMaster.DevConfig.HaveTileSetGood(good.id)
                    && !HaveStockUseGood(good.id))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetPreAddName()
        {
            for (int v = 65; v < 90; v++)
            {
                string vn = "" + (char)v;
                if (!IsHaveGoodInName(vn))
                {
                    return vn;
                }
            }

            return DateTime.Now.ToString("MM-dd:HH");
        }

        /// <summary>
        /// 判断是否储砖库存在轨道的出库首位置
        /// </summary>
        /// <param name="finish_track_id"></param>
        /// <returns></returns>
        public bool IsTrackHaveStockInTopPosition(uint track_id)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            return StockList.Exists(c => c.track_id == track_id && c.IsInLocation(track.limit_point_up, 500));
        }

        #endregion

        #region[极限混砖]

        /// <summary>
        /// 极限混砖：获取所有可放轨道
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="devid"></param>
        /// <param name="traids"></param>
        /// <returns></returns>
        public bool AllocateLimitGiveTrack(uint areaid, uint devid, uint goodsid, out List<uint> traids)
        {
            List<AreaDeviceTrack> list = PubMaster.Area.GetAreaDevTraList(areaid, devid);
            traids = new List<uint>();

            uint storecount = 0;
            List<TrackStoreCount> trackstores = new List<TrackStoreCount>();
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
                    traids.Add(adt.track_id);
                }
                else if (IsTrackOkForGoods(adt.track_id, goodsid))//未满能放
                {
                    storecount = (uint)StockList.Count(c => c.track_id == adt.track_id);
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
            return traids.Count > 0;
        }

        /// <summary>
        /// 获取底部最近的库存品种信息
        /// </summary>
        /// <param name="traid"></param>
        /// <returns></returns>
        public uint GetLastStockGid(uint traid)
        {
            List<Stock> list = StockList.FindAll(c => c.track_id == traid);
            if (list.Count > 0)
            {
                list.Sort((x, y) => x.pos.CompareTo(y.pos));
                return list[list.Count - 1].goods_id;
            }
            return 0;
        }

        #endregion

        #region[出库分割点]

        /// <summary>
        /// 判断轨道的第一托顶部库存是否处于出库分段点后面
        /// 是：脉冲小于分段点
        /// 否：脉冲大于分段点
        /// </summary>
        /// <param name="track_id">检查轨道</param>
        /// <returns></returns>
        public bool IsTopStockBehindUpSplitPoint(uint track_id, out uint stockid)
        {
            Track track = PubMaster.Track.GetTrack(track_id);
            if (track != null)
            {
                Stock topstock = GetTrackTopStock(track_id);
                if (topstock != null)
                {
                    stockid = topstock.id;
                    return track.up_split_point > topstock.location;
                }
            }
            stockid = 0;
            return false;
        }

        /// <summary>
        /// 判断库存所在位置是否轨道分割点后面
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool IsStockBehindUpSplitPoint(uint trackid, uint stockid)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track != null)
            {
                Stock topstock = GetStock(stockid);
                if (topstock != null)
                {
                    return topstock.location < track.up_split_point;
                }
            }

            return false;
        }


        /// <summary>
        /// 判断库存所在位置是否轨道分割点后面
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool ExistBehindUpSplitPoint(uint trackid, uint point)
        {
            return StockList.Exists(c => c.track_id == trackid && c.location >= point);
        }

        /// <summary>
        /// 判断库存所在位置是否轨道分割点前面
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool ExistInfrontUpSplitPoint(uint trackid, uint point)
        {
            return StockList.Exists(c => c.track_id == trackid && c.location <= point);
        }


        /// <summary>
        /// 判断库存是否是头部库存信息
        /// </summary>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool IsTopStock(uint stockid)
        {
            return StockList.Exists(c => c.id == stockid && c.PosType == StockPosE.头部);
        }

        /// <summary>
        /// 取上砖分割点后的第一车库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public Stock GetBehindUpSplitTopStock(uint trackid)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track != null)
            {
                List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid && c.location < track.up_split_point);
                if (stocks.Count > 0)
                {
                    stocks.Sort((x, y) => y.location.CompareTo(x.location));
                    return stocks[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取分割点前面最后的一车库存
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public Stock GetInfrontUpSplitButtonStock(uint trackid)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track != null)
            {
                List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid && c.location > track.up_split_point);
                if (stocks.Count > 0)
                {
                    stocks.Sort((x, y) => y.location.CompareTo(x.location));
                    return stocks[stocks.Count-1];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定点后的库存位置
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public Stock GetStockBehindStockPoint(uint trackid, ushort point)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track != null)
            {
                List<Stock> stocks = StockList.FindAll(c => c.track_id == trackid && c.location < point);
                if (stocks.Count > 0)
                {
                    //从小到大
                    stocks.Sort((x, y) => x.location.CompareTo(y.location));
                    return stocks[stocks.Count - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// 判断顶部库存是否是该品种
        /// </summary>
        /// <param name="finish_track_id"></param>
        /// <param name="goods_id"></param>
        /// <returns></returns>
        public bool IsTopStockIsGood(uint track_id, uint goods_id)
        {
            return (GetTrackTopStock(track_id)?.goods_id ?? 0) == goods_id;
        }

        /// <summary>
        /// 获取指定脉冲-后的库存数量
        /// </summary>
        /// <param name="trackid">轨道ID</param>
        /// <param name="point">脉冲</param>
        /// <returns></returns>
        public int GetBehindPointStockCount(uint trackid, int point)
        {
            return StockList.Count(c => c.track_id == trackid && c.location < point);
        }

        /// <summary>
        /// 获取指定脉冲-前的库存数量
        /// </summary>
        /// <param name="trackid">轨道ID</param>
        /// <param name="point">脉冲</param>
        /// <returns></returns>
        public int GetInfrontPointStockCount(uint trackid, int point)
        {
            return StockList.Count(c => c.track_id == trackid && c.location > point);
        }

        /// <summary>
        /// 获取指定脉冲前面最大的库存位置脉冲
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public ushort GetInfrontPointStockMaxLocation(uint trackid, int point)
        {
            List<Stock> list = StockList.FindAll(c => c.track_id == trackid && c.location > point);
            if(list.Count > 1)
            {
                return list.Max(c => c.location);
            }

            return 0;
        }

        /// <summary>
        /// 判断是否只存在最后一个库存（脉冲小于顶部库存）
        /// </summary>
        /// <param name="stockid"></param>
        /// <returns></returns>
        public bool IsOnlyOneWithStock(uint stockid)
        {
            Stock stock = GetStock(stockid);
            if(stock != null)
            {
                return StockList.Count(c => c.track_id == stock.track_id && c.location <= stock.location) == 1;
            }
            return false;
        }
        #endregion
    }
}
