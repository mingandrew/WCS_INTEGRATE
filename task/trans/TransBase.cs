using enums;
using module.goods;
using module.msg;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using tool.mlog;
using tool.timer;

namespace task.trans
{
    public abstract class TransBase
    {
        #region[字段]
        protected readonly object  _to,_for;
        private bool IsRunning = true;
        protected MsgAction mMsg;
        private DateTime inittime;
        private bool initwaitefinish;
        internal MTimer mTimer;
        internal Log mLog;
        protected List<StockTrans> TransList { set; get; }
        #endregion

        #region[构造函数/初始化/启动/停止]
        public TransBase()
        {
            mLog = (Log)new LogFactory().GetLog("任务日志", false);
            mTimer = new MTimer();
            mMsg = new MsgAction();
            _to = new object();
            _for = new object();
            TransList = new List<StockTrans>();
            InitTrans();
        }

        private void InitTrans()
        {
            TransList.Clear();
            TransList.AddRange(PubMaster.Mod.GoodSql.QueryStockTransList());
        }

        public void Start()
        {
            inittime = DateTime.Now;
            new Thread(Checking)
            {
                IsBackground = true
            }.Start();
        }

        internal void Stop()
        {
            IsRunning = false;
        }
        
        #endregion

        #region[操作交易]

        private void Checking()
        {
            while (IsRunning)
            {
                if (!initwaitefinish)
                {
                    //等待10秒再开始调度未完成的任务
                    if((DateTime.Now - inittime).TotalSeconds > 10)
                    {
                        initwaitefinish = true;
                    }
                    Thread.Sleep(1000);
                    continue;
                }
                if(Monitor.TryEnter(_for, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        TransList.RemoveAll(c => c.finish && c.TransStaus == TransStatusE.完成);

                        for (int i = 0; i < TransList.Count; i++)
                        {
                            try
                            {
                                StockTrans trans = TransList[i];
                                switch (trans.TransType)
                                {
                                    case TransTypeE.下砖任务:
                                        DoInTrans(trans);
                                        break;
                                    case TransTypeE.上砖任务:
                                        DoOutTrans(trans);
                                        break;
                                    case TransTypeE.倒库任务:
                                        DoSortTrans(trans);
                                        break;
                                    case TransTypeE.移车任务:
                                        DoMoveCarrier(trans);
                                        break;
                                    case TransTypeE.手动下砖:
                                        DoManualInTrans(trans);
                                        break;
                                    case TransTypeE.手动上砖:
                                        DoManualOutTrans(trans);
                                        break;
                                    case TransTypeE.同向上砖:
                                        DoSameSideOutTrans(trans);
                                        break;
                                    case TransTypeE.上砖侧倒库:
                                        DoUpSortTrans(trans);
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                mLog.Error(true, "[ID:" + TransList[i]?.id + "]", e);
                            }
                        }

                        CheckTrackSort();  //包装前无需倒库
                        CheckUpTrackSort(); //上砖侧倒库
                    }
                    catch (Exception e)
                    {
                        mLog.Error(true, e.Message, e);
                    }
                    finally
                    {
                        Monitor.Exit(_for);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public abstract void DoInTrans(StockTrans trans);//下砖任务
        public abstract void DoOutTrans(StockTrans trans);//上砖任务
        public abstract void DoSameSideOutTrans(StockTrans trans);//同向出库
        public abstract void DoSortTrans(StockTrans trans);//倒库
        public abstract void DoUpSortTrans(StockTrans trans);//上砖侧倒库
        public abstract void DoMoveCarrier(StockTrans trans);//移车
        public abstract void DoManualInTrans(StockTrans trans);//手动入库
        public abstract void DoManualOutTrans(StockTrans trans);//手动出库
        public abstract void CheckTrackSort();
        public abstract void CheckUpTrackSort();
        protected abstract void SendMsg(StockTrans trans);
        #endregion

        #region[交易信息资源管理]

        #region[增删改]

        public uint AddTrans(uint areaid, uint lifterid, TransTypeE type, uint goodsid, uint stocksid, uint taketrackid, uint givetrackid, uint carrierid = 0, ushort line = 0)
        {
            uint transid = 0;
            if (Monitor.TryEnter(_for, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    TransStatusE initstatus = TransStatusE.调度设备;
                    switch (type)
                    {
                        case TransTypeE.下砖任务:
                        case TransTypeE.手动下砖:
                        case TransTypeE.同向下砖:
                            initstatus = TransStatusE.检查轨道;
                            break;
                        case TransTypeE.上砖任务:
                        case TransTypeE.同向上砖:
                            if (PubMaster.Track.IsUpSplit(taketrackid))
                            {
                                PubMaster.DevConfig.SetLastTrackId(lifterid, taketrackid);
                            }
                            break;
                    }
                    transid = AddTransWithoutLock(areaid, lifterid, type, goodsid, stocksid, taketrackid, givetrackid, initstatus, carrierid, line);
                }
                finally
                {
                    Monitor.Exit(_for);
                }
            }
            return transid;
        }

        public uint AddTransWithoutLock(uint areaid, uint lifterid, TransTypeE type,
                                        uint goodsid, uint stocksid,
                                        uint taketrackid, uint givetrackid,
                                        TransStatusE initstatus = TransStatusE.调度设备,
                                        uint carrierid = 0, ushort line = 0)
        {
            uint newid = PubMaster.Dic.GenerateID(DicTag.NewTranId);
            StockTrans trans = new StockTrans()
            {
                id = newid,
                area_id = areaid,
                TransStaus = initstatus,
                TransType = type,
                tilelifter_id = lifterid,
                goods_id = goodsid,
                stock_id = stocksid,
                take_track_id = taketrackid,
                give_track_id = givetrackid,
                create_time = DateTime.Now,
                carrier_id = carrierid,
                line = line
            };
            bool isadd = PubMaster.Mod.GoodSql.AddStockTrans(trans);
            if (!isadd)
            {
                ReAddTransInNewID(trans);
            }
            TransList.Add(trans);

            SetLine(trans, line);

            //更新需求的任务id和时间 20210121
            if (type == TransTypeE.下砖任务 || type == TransTypeE.同向下砖)
            {
                PubTask.TileLifterNeed.UpdateTileLifterNeedTrans(lifterid, taketrackid, trans.create_time, trans.id);
            }
            else if (type == TransTypeE.上砖任务 || type == TransTypeE.同向上砖)
            {
                PubTask.TileLifterNeed.UpdateTileLifterNeedTrans(lifterid, givetrackid, trans.create_time, trans.id);
            }

            SendMsg(trans);

            try
            {
                string log = string.Empty;
                switch (type)
                {
                    case TransTypeE.下砖任务:
                    case TransTypeE.上砖任务:
                    case TransTypeE.手动下砖:
                    case TransTypeE.手动上砖:
                    case TransTypeE.同向上砖:
                    case TransTypeE.同向下砖:
                    case TransTypeE.其他:
                        log = string.Format("标识[ {0} ], 任务[ {1} ], 状态[ {2} ], 砖机[ {3} ], " +
                            "货物[ {4} ], 库存[ {5} ], 取轨[ {6} ], 卸轨[ {7} ]",
                            trans.id, type, initstatus,
                            PubMaster.Device.GetDeviceName(lifterid, lifterid + ""),
                            goodsid, stocksid,
                            PubMaster.Track.GetTrackName(taketrackid, taketrackid + ""),
                            PubMaster.Track.GetTrackName(givetrackid, givetrackid + ""));
                        break;
                    case TransTypeE.倒库任务:
                    case TransTypeE.上砖侧倒库:
                        log = string.Format("标识[ {0} ], 任务[ {1} ], 状态[ {2} ], " +
                            "货物[ {3} ], 取轨[ {4} ], 卸轨[ {5} ]",
                            trans.id, type, initstatus,
                            PubMaster.Goods.GetGoodsName(goodsid),
                            PubMaster.Track.GetTrackName(taketrackid, taketrackid + ""),
                            PubMaster.Track.GetTrackName(givetrackid, givetrackid + ""));
                        break;
                    case TransTypeE.移车任务:
                        log = string.Format("标识[ {0} ], 任务[ {1} ], 状态[ {2} ], " +
                            "取轨[ {3} ], 卸轨[ {4} ]",
                            trans.id, type, initstatus,
                            PubMaster.Track.GetTrackName(taketrackid, taketrackid + ""),
                            PubMaster.Track.GetTrackName(givetrackid, givetrackid + ""));
                        break;
                }
                
                if (carrierid > 0)
                {
                    log += string.Format(", 运输车[ {0} ]", PubMaster.Device.GetDeviceName(carrierid, carrierid + ""));
                }
                mLog.Status(true, log);
            }
            catch { }

            return newid;
        }

        /// <summary>
        /// ID冲突则重新添加
        /// </summary>
        /// <param name="trans"></param>
        private void ReAddTransInNewID(StockTrans trans)
        {
            ushort count = 0;
            while (true && count <= 10)
            {
                uint newid = PubMaster.Dic.UpdateGenerateID(DicTag.NewTranId, 1000);
                trans.id = newid;
                if (PubMaster.Mod.GoodSql.AddStockTrans(trans))
                {
                    return;
                }
                count++;
                mLog.Error(true, string.Format("第{0}次添加任务错误[{1}]", count, trans.ToString()));
            }
        }

        internal void SetStatus(StockTrans trans, TransStatusE status, string memo = "")
        {
            if (trans.TransStaus != status)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 状态[ {1} -> {2} ], 备注[ {3} ]", trans.id, trans.TransStaus, status, memo));
                trans.TransStaus = status;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Status);

                if(status == TransStatusE.取消)
                {
                    SetCancel(trans);
                }

                SendMsg(trans);
            }
        }

        internal void SetCarrier(StockTrans trans, uint carrierid)
        {
            if(trans.carrier_id != carrierid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 分配小车[ {1} ]", trans.id, PubMaster.Device.GetDeviceName(carrierid)));
                trans.carrier_id = carrierid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.CarrierId);
                SendMsg(trans);
            }
        }

        internal void SetTakeFerry(StockTrans trans, uint ferryid)
        {
            if (trans.take_ferry_id != ferryid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 分配T摆渡车[ {1} ]", trans.id, PubMaster.Device.GetDeviceName(ferryid)));
                trans.take_ferry_id = ferryid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TakeFerryId);
                SendMsg(trans);
            }
        }

        internal void SetGiveFerry(StockTrans trans, uint ferryid)
        {
            if (trans.give_ferry_id != ferryid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 分配G摆渡车[ {1} ]", trans.id, PubMaster.Device.GetDeviceName(ferryid)));
                trans.give_ferry_id = ferryid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.GiveFerryId);
                SendMsg(trans);
            }
        }

        /// <summary>
        /// 重新分配转机
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tileid"></param>
        internal void SetTile(StockTrans trans, uint tileid)
        {
            if (trans.tilelifter_id != tileid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 重新分配砖机[ {1} ]", trans.id, PubMaster.Device.GetDeviceName(tileid)));
                trans.tilelifter_id = tileid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TileId);
                SendMsg(trans);
            }
        }

        internal void SetLoadTime(StockTrans trans)
        {
            if (trans.load_time == null)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 取货时间[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.load_time = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.LoadTime);
                SendMsg(trans);
            }
        }

        internal void SetUnLoadTime(StockTrans trans)
        {
            if (trans.unload_time == null)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 卸货时间[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.unload_time = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.UnLoadTime);
                SendMsg(trans);
            }
        }
        
        internal void SetFinish(StockTrans trans)
        {
            if (trans.finish_time == null)
            {
                PubMaster.Warn.RemoveTaskAllWarn(trans.id);
                mLog.Status(true, string.Format("任务[ {0} ], 任务完成[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.finish = true;
                trans.finish_time = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Finish);
                SendMsg(trans);
            }
        }

        protected bool SetTakeSite(StockTrans trans, uint traid)
        {
            if (trans.take_track_id != traid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 取货轨道[ {1} ]", trans.id, PubMaster.Track.GetTrackName(traid)));
                trans.take_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TakeSite);
                SendMsg(trans);
                return true;
            }
            return false;
        }

        protected bool SetGiveSite(StockTrans trans, uint traid)
        {
            if (trans.give_track_id != traid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 卸货轨道[ {1} ]", trans.id, PubMaster.Track.GetTrackName(traid)));
                trans.give_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.GiveSite);
                SendMsg(trans);
                return true;
            }
            return false;
        }

        protected void SetStock(StockTrans trans, uint stockid)
        {
            if (trans.stock_id != stockid)
            {
                trans.stock_id = stockid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Stock);
            }
        }

        protected void SetCancel(StockTrans trans)
        {
            if (!trans.cancel)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 任务取消[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.cancel = true;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Cancel);
            }
        }

        protected void SetReTake(StockTrans trans, uint taketraid, uint stockid, uint carrierid, TransStatusE status)
        {
            trans.take_track_id = taketraid;
            trans.stock_id = stockid;
            trans.carrier_id = carrierid;
            trans.TransStaus = status;
            PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.ReTake);
        }

        protected void SetLine(StockTrans trans, ushort line)
        {
            if (line > 0)
            {
                trans.line = line;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Line);
            }
        }
        #endregion

        #region[获取交易]

        public List<StockTrans> GetTransList()
        {
            return TransList;
        }

        public List<StockTrans> GetTransList(List<uint> areaids)
        {
            return TransList.FindAll(c => areaids.Contains(c.area_id));
        }

        public StockTrans GetTrans(uint transid)
        {
            StockTrans stock = TransList.Find(c => c.id == transid);
            if (stock == null)
            {
                stock = PubMaster.Mod.GoodSql.QueryStockTransById(transid);
            }
            return stock;
        }

        public List<StockTrans> GetTransInType(TransTypeE type)
        {
            return TransList.FindAll(c => !c.finish && c.TransType == type);
        }

        #endregion

        #region[判断状态]

        /// <summary>
        /// 判断是否有交易在上下砖机工位
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public bool HaveInLifter(uint devid, uint taketrackid)
        {
            return TransList.Exists(c => !c.finish && c.tilelifter_id == devid && c.take_track_id == taketrackid);
        }

        /// <summary>
        /// 同下砖机不同轨道限制一个
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool HaveInLifter(uint devid)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    return TransList.Exists(c => !c.finish && c.tilelifter_id == devid);
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        public bool HaveTransWithGood(TransTypeE type, uint goodid, uint areaid)
        {
            return TransList.Exists(c =>c.TransType == type && c.goods_id == goodid && c.area_id == areaid);
        }

        /// <summary>
        /// 同轨道不同下砖机限制一个
        /// </summary>
        /// <param name="ltrack"></param>
        /// <param name="rtrack"></param>
        /// <returns></returns>
        public bool HaveInTileTrack(uint ltrack, uint rtrack)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    return TransList.Exists(c => !c.finish && (c.take_track_id == ltrack || c.take_track_id == rtrack));
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        /// <summary>
        /// 同轨道不同上砖机限制一个
        /// </summary>
        /// <param name="ltrack"></param>
        /// <param name="rtrack"></param>
        /// <returns></returns>
        public bool HaveOutTileTrack(uint ltrack, uint rtrack)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    return TransList.Exists(c => !c.finish && (c.give_track_id == ltrack || c.give_track_id == rtrack));
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        internal bool HaveInGoods(uint areaId, uint goodsId, TransTypeE tasktype)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    return TransList.Exists(c => !c.finish && c.area_id == areaId 
                                    && c.TransType == tasktype && c.goods_id == goodsId);
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        internal bool HaveInTileTrack(uint trackid)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    return TransList.Exists(c => !c.finish && (c.take_track_id == trackid || c.give_track_id == trackid || c.finish_track_id == trackid));
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
            return true;
        }

        internal bool ExistInTileTrack(uint devid, uint trackid)
        {
            return TransList.Exists(c => !c.finish
            && c.tilelifter_id == devid
            && (c.take_track_id == trackid || c.give_track_id == trackid));
        }

        #endregion

        #endregion

        #region[交易处理]

        /// <summary>
        /// 完成交易
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="taketrackcode"></param>
        /// <param name="givetrackcode"></param>
        public void FinishTrans(uint carrierid, ushort taketrackcode, ushort givetrackcode)
        {
            if (Monitor.TryEnter(_to, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    List<StockTrans> trans = TransList.FindAll(c => c.TransStaus != TransStatusE.完成 && c.carrier_id == carrierid);
                    if (trans.Count > 0)
                    {
                        //foreach (StockTrans tran in trans)
                        //{
                        //    if (PubMaster.Track.IsTrackWithCode(tran.take_track_id, taketrackcode)
                        //       && PubMaster.Track.IsTrackWithCode(tran.give_track_id, givetrackcode))
                        //    {

                        //        tran.TransStaus = TransStatusE.完成;
                        //        tran.finish = true;
                        //        tran.finish_time = DateTime.Now;
                        //        PubMaster.Mod.GoodSql.EditStockTrans(tran, TransUpdateE.Finish);
                        //    }
                        //}
                        Console.WriteLine("小车信息完成任务！");
                    }
                }
                finally
                {
                    Monitor.Exit(_to);
                }
            }
        }

        /// <summary>
        /// 倒库完成/库存调整
        /// </summary>
        /// <param name="carrid"></param>
        /// <param name="site"></param>
        public void ShiftTrans(uint carrid, uint trackid)
        {
            //StockTrans trans = TransList.Find(c => c.TransType == TransTypeE.倒库 && c.carrier_id == carrid
            //                                        && c.take_track_id == trackid && c.TransStaus != TransStatusE.完成);
            //if (trans != null)
            //{
            //    PubMaster.Goods.ShiftStock(trackid);
            //    trans.TransStaus = TransStatusE.完成;
            //    trans.finish_time = DateTime.Now;
            //    PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Finish);
            //}
        }

        #endregion

    }
}
