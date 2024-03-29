﻿using enums;
using GalaSoft.MvvmLight.Messaging;
using module.goods;
using module.msg;
using module.track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tool.mlog;

namespace resource.goods
{
    public class GoodSumMaster
    {
        private List<StockSum> StockSumList { set; get; }
        private MsgAction mMsg;
        private Log _mlog;
        protected readonly string timeformat = "yyyy-MM-dd HH:mm:ss";



        #region[构造/初始化]

        public GoodSumMaster()
        {
            StockSumList = new List<StockSum>();
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
                StockSumList.Clear();
                StockSumList.AddRange(PubMaster.Mod.GoodSql.QueryStockSumList());
            }

        }

        public void Stop()
        {

        }
        #endregion


        #region[库存统计]



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
            return StockSumList.FindAll(c => c.area == areaid);
        }

        /// <summary>
        /// 修改轨道统计的品种为另外一个品种信息
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="goodid"></param>
        /// <param name="oldgoodid"></param>
        public void StockSumChangeGood(uint trackid, uint goodid, uint oldgoodid)
        {
            List<StockSum> sums;
            if (oldgoodid > 0)
            {
                sums = StockSumList.FindAll(c => c.track_id == trackid && c.goods_id == oldgoodid);
            }
            else
            {
                sums = StockSumList.FindAll(c => c.track_id == trackid);
            }

            if (sums.Count == 1)
            {
                SendSumMsg(sums[0], ActionTypeE.Delete);
                sums[0].goods_id = goodid;
                sums[0].produce_time = PubMaster.Goods.GetEarliestTime(trackid);
                SendSumMsg(sums[0], ActionTypeE.Update);

                SendTrackStockQtyChangeMsg(trackid);
            }
            else if (sums.Count > 0)
            {
                StockSum newsum = new StockSum
                {
                    goods_id = goodid,
                    produce_time = PubMaster.Goods.GetEarliestTime(trackid),
                    track_id = trackid,
                    area = sums[0].area,
                    track_type = sums[0].track_type
                };

                foreach (StockSum sum in sums)
                {
                    if (sum.goods_id == oldgoodid)
                    {
                        newsum.count += sum.count;
                        newsum.pieces += sum.pieces;
                        newsum.stack += sum.stack;
                        SendSumMsg(sum, ActionTypeE.Delete);
                    }

                }
                StockSumList.RemoveAll(c => c.track_id == trackid && c.goods_id == oldgoodid);
                StockSumList.Add(newsum);

                SendSumMsg(newsum, ActionTypeE.Add);

                SendTrackStockQtyChangeMsg(trackid);
            }
        }

        public List<StockSum> GetStockSumsByDevId(uint tileid)
        {
            if (tileid == 0) return StockSumList;
            uint areaid = PubMaster.Device.GetDeviceArea(tileid);
            return StockSumList.FindAll(c => c.area == areaid).ToList();
        }

        /// <summary>
        /// 清空轨道的所有统计信息
        /// </summary>
        /// <param name="trackid">被清空的轨道ID</param>
        public void RemoveTrackSum(uint trackid)
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
        private void SendSumMsg(StockSum sum, ActionTypeE type)
        {
            mMsg.o1 = sum;
            mMsg.o2 = type;
            Messenger.Default.Send(mMsg, MsgToken.StockSumeUpdate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterarea"></param>
        /// <param name="filterline"></param>
        /// <param name="filtertype"></param>
        public void GetGoodCountList(uint filterarea, ushort filterline, byte filtertype)
        {
            List<StockSum> filterlist = new List<StockSum>();
            if (filterarea == 0 && filtertype == 0)
            {
                filterlist = StockSumList;
            }
            else if (filterarea != 0 && filterline != 0 && filtertype == 0)
            {
                filterlist = StockSumList.FindAll(c => c.area == filterarea && c.line == filterline);
            }
            else if (filtertype != 0 && filterarea == 0)
            {
                filterlist = StockSumList.FindAll(c => c.track_type == filtertype);
            }
            else if (filterarea != 0 && filtertype != 0)
            {
                filterlist = StockSumList.FindAll(c => c.area == filterarea && c.line == filterline && c.track_type == filtertype);
            }
            if (filterlist == null) return;
            var list = filterlist.GroupBy(c => new { c.goods_id }).Select(c => new StockSum
            {
                goods_id = c.Key.goods_id,
                count = c.Sum(b => b.count),
                stack = c.Sum(b => b.stack),
                pieces = c.Sum(b => b.pieces),
                produce_time = c.Min(b => b.produce_time),
            });
            List<StockSum> goodcountlist = list.ToList();
            if (filterarea != 0 || filtertype != 0)
            {
                foreach (var item in goodcountlist)
                {
                    item.area = filterarea;
                    item.line = filterline;
                    item.track_type = filtertype;
                }
            }
            goodcountlist.RemoveAll(c => c.count <= 0 || c.count > 10000);
            Messenger.Default.Send(goodcountlist, MsgToken.GoodSumUpdate);
        }

        /// <summary>
        /// 刷新轨道的库存概况
        /// </summary>
        /// <param name="trackId"></param>
        public void CheckTrackSum(uint trackId)
        {
            List<uint> goodsids = PubMaster.Goods.TrackUnionGood(trackId);

            if (goodsids != null && goodsids.Count != 0)
            {
                Track track = PubMaster.Track.GetTrack(trackId);
                foreach (uint gid in goodsids)
                {
                    Goods goods = PubMaster.Goods.GetGoods(gid);// GoodsList.Find(c => c.id == gid);
                    GoodSize size = PubMaster.Goods.GetSize(goods.size_id);
                    StockSum sum = StockSumList.Find(c => c.goods_id == gid && c.track_id == trackId);
                    if (sum == null)
                    {
                        sum = new StockSum()
                        {
                            track_id = trackId,
                            goods_id = gid,
                            produce_time = PubMaster.Goods.GetEarliestTime(trackId,gid),
                            area = track.area,
                            track_type = track.type
                        };
                        StockSumList.Add(sum);
                    }
                    sum.count = PubMaster.Goods.GetTrackStockCount(trackId, gid);
                    sum.stack = sum.count * (size?.stack ?? 1);
                    sum.pieces = PubMaster.Goods.GetTrackStockPieseSum(trackId, gid);
                    SendSumMsg(sum, ActionTypeE.Update);
                }
                SortSumList();
            }

            SendTrackStockQtyChangeMsg(trackId);
        }

        /// <summary>
        /// 根据库存删除统计库存信息
        /// </summary>
        /// <param name="stock"></param>
        public void DelectSumUpdate(Stock stock)
        {
            StockSum sum = StockSumList.Find(c => c.track_id == stock.track_id && c.goods_id == stock.goods_id);
            if (sum != null)
            {
                sum.count -= 1;
                sum.stack -= stock.stack;
                sum.pieces -= stock.pieces;

                if (sum.count <= 0)
                {
                    StockSumList.Remove(sum);
                    SendSumMsg(sum, ActionTypeE.Delete);
                    return;
                }

                sum.produce_time = PubMaster.Goods.GetEarliestTime(stock.track_id, stock.goods_id);
                SendSumMsg(sum, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 更新轨道的统计库存信息
        /// </summary>
        /// <param name="stock">单个库存信息</param>
        /// <param name="totrackid">库存的去向轨道ID</param>
        public void StockSumChange(Stock stock, uint totrackid)
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
        {
            //库存 从 储砖轨道 => 出(减)
            StockSum sum = StockSumList.Find(c => c.track_id == stock.track_id && c.goods_id == stock.goods_id);
            if (sum != null)
            {
                sum.count -= 1;
                sum.stack -= stock.stack;
                sum.pieces -= stock.pieces;

                if (sum.count <= 0)
                {
                    StockSumList.Remove(sum);
                    SendSumMsg(sum, ActionTypeE.Delete);
                    return;
                }

                sum.produce_time = PubMaster.Goods.GetEarliestTime(sum.track_id);
                SendSumMsg(sum, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 轨道库存数量编号
        /// </summary>
        /// <param name="trackid"></param>
        public void SendTrackStockQtyChangeMsg(uint trackid)
        {
            Messenger.Default.Send(trackid, MsgToken.TrackStockQtyUpdate);
        }

        public void SortSumList()
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
    }
}
