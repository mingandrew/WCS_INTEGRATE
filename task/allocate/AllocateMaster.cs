using enums;
using enums.track;
using module.goods;
using resource;
using System.Collections.Generic;

namespace task.allocate
{
    /// <summary>
    /// 分配逻辑管理类
    /// 1.入库轨道分配
    /// 2.出库轨道分配
    /// </summary>
    public class AllocateMaster
    {
        public AllocateMaster()
        {

        }

        internal void Stop()
        {

        }

        internal void Start()
        {

        }

        /// <summary>
        ///  判断轨道是否满足入库轨道的要求
        /// [前置条件]启用，空砖|有砖
        /// b.没有入库任务占用
        /// c.轨道库存数 x(大于零)
        /// x >= 2 可以同时出库入库
        /// x = 1 时有出库任务，则切换轨道
        /// </summary>
        /// <param name="trackid">进行判断的轨道</param>
        /// <returns></returns>
        public bool IsTrackOk4InTrans(uint trackid)
        {
            if (PubTask.Trans.IsTraInTransWithLock(trackid))
            {
                return false;
            }

            //if (PubTask.Trans.IsTrasInTransWithType(trackid, TransTypeE.下砖任务))
            //{
            //    return false;
            //}

            //同时上下砖
            //轨道库存数量
            //ushort stockqty = PubMaster.Goods.GetTrackStockCount(trackid);
            ////x =  1 时有出库任务，则切换轨道
            //if (stockqty > 0 && stockqty <= 2 
            //    && (PubTask.Trans.IsTrasInTransWithType(trackid, TransTypeE.上砖任务)
            //        || PubTask.Carrier.CheckHaveCarInTrack( TransTypeE.上砖任务, trackid,0)))
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 入库分配轨道
        /// 1、找同品种未满入轨道
        /// 2、优先找出轨道同品种空的入轨道（不能连续两次下同一条轨道）
        /// 3、找空的出轨道对应空的入轨道
        /// 4、找出轨道入库时间最早时间的空入轨道
        /// 5、不存在空轨道的情况下，极限混砖【有开关】
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="tileid">砖机ID</param>
        /// <param name="goodid">品种ID</param>
        /// <param name="givetrackid">入库分配轨道</param>
        /// <param name="lastgoodid">极限满砖分配时，轨道的上个品种</param>
        /// <param name="islimitallocate">是否使用了极限满砖分配</param>
        public void AllocateInGiveTrack(uint areaid, ushort lineid, uint tileid, uint goodid, 
            out uint givetrackid, out uint lastgoodid, out bool islimitallocate)
        {
            givetrackid = 0;
            lastgoodid = 0;
            islimitallocate = false;

            //【常规分配轨道 1-4】
            if (PubMaster.Goods.AllocateGiveTrack(areaid, lineid, tileid, goodid, out List<uint> traids))
            {
                foreach (uint traid in traids)
                {
                    if (!IsTrackOk4InTrans(traid)) continue;

                    givetrackid = traid;
                    return;
                }
            }
            else//【极限混砖 5】
            {
                if (!PubMaster.Dic.IsSwitchOnOff(DicTag.EnableLimitAllocate))
                {
                    return;
                }

                /*【极限混砖】常规分配不到符合的轨道
                 * 1.没空轨道
                 * 2.砖机品种没有对应品种的未满轨道
                 * 3.分配其他砖机品种轨道以外的非满轨道 
                 * 4.优先放库存数量多的轨道
                 */
                if (PubMaster.Goods.AllocateLimitGiveTrack(areaid, tileid, goodid, out List<uint> stocktraids))
                {
                    foreach (var traid in stocktraids)
                    {
                        if (!IsTrackOk4InTrans(traid)) continue;

                        lastgoodid = PubMaster.Goods.GetLastStockGid(traid);
                        if (lastgoodid > 0 && PubMaster.DevConfig.IsHaveSameTileNowGood(lastgoodid, TileWorkModeE.下砖))
                        {
                            continue;
                        }

                        givetrackid = traid;
                        islimitallocate = true;

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 分配摆渡车上的库存小车
        /// </summary>
        /// <param name="areaid">区域ID</param>
        /// <param name="goodid">品种ID</param>
        /// <param name="stockcarrierid">在摆渡车的运输车ID</param>
        /// <param name="stock">库存信息</param>
        /// <returns>Ture:分配成功 False:找不到摆渡车上的有货运输车</returns>
        public bool AllocateOutInFerry(uint areaid, uint goodid, out uint stockcarrierid,out Stock stock)
        {
            if (PubTask.Carrier.GetInFerryAndLoad(areaid, out List<uint> carrierids, TrackTypeE.摆渡车_出))
            {
                foreach (var carrid in carrierids)
                {
                    if (!PubTask.Trans.HaveInCarrier(carrid))
                    {
                        uint stockid = PubMaster.DevConfig.GetCarrierStockId(carrid);
                        if (stockid > 0)
                        {
                            stock = PubMaster.Goods.GetStock(stockid);
                            if (stock != null && stock.goods_id == goodid)
                            {
                                stockcarrierid = carrid;
                                return true;
                            }
                        }
                    }
                }
            }
            stock = null;
            stockcarrierid = 0;
            return false;
        }
    }
}
