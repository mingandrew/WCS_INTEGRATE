using enums;
using module.goods;
using module.msg;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using task.trans.diagnose;
using tool.mlog;
using tool.timer;

namespace task.trans
{
    public abstract class TransBase
    {
        #region[字段]
        protected readonly object _to, _for;
        private bool IsRunning = true;
        protected MsgAction mMsg;
        private DateTime inittime;
        private bool initwaitefinish;
        internal MTimer mTimer;
        internal Log mLog;
        protected List<StockTrans> TransList { set; get; }
        #endregion

        #region[分析]

        DiagnoseServer MDiagnoreServer;

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

        protected void InitDiagnore(TransMaster trans)
        {
            MDiagnoreServer = new DiagnoseServer(trans);
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
                    if ((DateTime.Now - inittime).TotalSeconds > 10)
                    {
                        initwaitefinish = true;
                    }
                    Thread.Sleep(1000);
                    continue;
                }
                if (Monitor.TryEnter(_for, TimeSpan.FromSeconds(2)))
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

                try
                {
                    MDiagnoreServer.Diagnose();
                }
                catch { }

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
                trans.TransStausStayTime = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Status);

                if (status == TransStatusE.取消)
                {
                    SetCancel(trans);
                }

                //SendMsg(trans);
                SetStepLog(trans, true, (uint)status, string.Format("切换流程[ {0} ]；{1}；", status, memo));

                if (status == TransStatusE.完成)
                {
                    if (trans.InType(TransTypeE.上砖任务, TransTypeE.同向上砖, TransTypeE.手动上砖))
                    {
                        //完成需求
                        PubTask.TileLifterNeed.FinishTileLifterNeed(trans.tilelifter_id, trans.give_track_id);
                    }

                    if (trans.InType(TransTypeE.下砖任务, TransTypeE.同向下砖, TransTypeE.手动下砖))
                    {
                        //完成需求
                        PubTask.TileLifterNeed.FinishTileLifterNeed(trans.tilelifter_id, trans.take_track_id);
                    }
                }
            }
        }

        /// <summary>
        /// 记录步骤日志到界面
        /// -code规则(随意不重复的2位数+TransStatusE + TransTypeE)
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="isOK"></param>
        /// <param name="code">规则(随意不重复的2位数+TransStatusE + TransTypeE)</param>
        /// <param name="info"></param>
        internal void SetStepLog(StockTrans trans, bool isOK, uint code, string info)
        {
            if (!string.IsNullOrEmpty(info))
            {
                // log
                mLog.Info(true, string.Format("任务[ {0} ], 步骤[ {1} ({2}) ]-[ {3} ]", trans.id, trans.TransStaus, code, info));

                // 界面显示
                if (trans.LogStep(isOK, code, info))
                {
                    SendMsg(trans);
                }
            }
        }

        internal void SetCarrier(StockTrans trans, uint carrierid, string memo = "")
        {
            if (trans.carrier_id != carrierid)
            {
                string devname = PubMaster.Device.GetDeviceName(carrierid);
                mLog.Status(true, string.Format("任务[ {0} ], 分配小车[ {1} ], 备注[ {2} ]", trans.id, devname, memo));
                trans.carrier_id = carrierid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.CarrierId);
                //SendMsg(trans);
                SetStepLog(trans, true, 99, string.Format("锁定运输车[ {0} ]；", devname));
            }
        }

        internal void SetTakeFerry(StockTrans trans, uint ferryid)
        {
            if (trans.take_ferry_id != ferryid)
            {
                string devname = PubMaster.Device.GetDeviceName(ferryid);
                mLog.Status(true, string.Format("任务[ {0} ], 分配T摆渡车[ {1} ]", trans.id, devname));
                trans.take_ferry_id = ferryid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TakeFerryId);
                //SendMsg(trans);
                SetStepLog(trans, true, 98, string.Format("锁定接车摆渡车[ {0} ]；", devname));
            }
        }

        internal void SetGiveFerry(StockTrans trans, uint ferryid)
        {
            if (trans.give_ferry_id != ferryid)
            {
                string devname = PubMaster.Device.GetDeviceName(ferryid);
                mLog.Status(true, string.Format("任务[ {0} ], 分配G摆渡车[ {1} ]", trans.id, devname));
                trans.give_ferry_id = ferryid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.GiveFerryId);
                //SendMsg(trans);
                SetStepLog(trans, true, 97, string.Format("锁定送车摆渡车[ {0} ]；", devname));
            }
        }

        /// <summary>
        /// 重新分配转机
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tileid"></param>
        internal void SetTile(StockTrans trans, uint tileid, string memo = "")
        {
            if (trans.tilelifter_id != tileid)
            {
                string devname = PubMaster.Device.GetDeviceName(tileid);
                mLog.Status(true, string.Format("任务[ {0} ], 重新分配砖机[ {1} ], 备注[ {2} ]", trans.id, devname, memo));
                trans.tilelifter_id = tileid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TileId);
                //SendMsg(trans);
                SetStepLog(trans, true, 96, string.Format("重新锁定砖机[ {0} ]；", devname));
            }
        }

        internal void SetLoadTime(StockTrans trans)
        {
            if (trans.load_time == null)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 取货时间[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.load_time = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.LoadTime);
                //SendMsg(trans);
                SetStepLog(trans, true, 95, string.Format("任务运输车取货完成；"));
            }
        }

        internal void SetUnLoadTime(StockTrans trans)
        {
            if (trans.unload_time == null)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 卸货时间[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.unload_time = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.UnLoadTime);
                //SendMsg(trans);
                SetStepLog(trans, true, 94, string.Format("任务运输车卸货完成；"));
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
                //SendMsg(trans);
                SetStepLog(trans, true, 93, string.Format("任务结束；"));
            }
        }

        protected bool SetTakeSite(StockTrans trans, uint traid)
        {
            if (trans.take_track_id != traid)
            {
                string traname = PubMaster.Track.GetTrackName(traid);
                mLog.Status(true, string.Format("任务[ {0} ], 取货轨道[ {1} ]", trans.id, traname));
                trans.take_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TakeSite);
                //SendMsg(trans);
                SetStepLog(trans, true, 92, string.Format("重新分配取货轨道[ {0} ]；", traname));
                return true;
            }
            return false;
        }

        protected bool SetGiveSite(StockTrans trans, uint traid)
        {
            if (trans.give_track_id != traid)
            {
                string traname = PubMaster.Track.GetTrackName(traid);
                mLog.Status(true, string.Format("任务[ {0} ], 卸货轨道[ {1} ]", trans.id, traname));
                trans.give_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.GiveSite);
                //SendMsg(trans);
                SetStepLog(trans, true, 91, string.Format("重新分配卸货轨道[ {0} ]；", traname));
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
        /// 同下砖机不同轨道限制一个
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool HaveInLifter(uint devid)
        {
            try
            {
                return TransList.Exists(c => !c.finish && c.tilelifter_id == devid);
            }
            catch { }
            return true;
        }
        
        /// <summary>
        /// 同轨道不同下砖机限制一个
        /// </summary>
        /// <param name="ltrack"></param>
        /// <param name="rtrack"></param>
        /// <returns></returns>
        public bool HaveInTileTrack(uint ltrack, uint rtrack)
        {
            try
            {
                return TransList.Exists(c => !c.finish && c.InTrack(ltrack,rtrack));
            }
            catch { }
            return true;
        }

        /// <summary>
        /// 判断任务是否使用了该品种
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="goodsId"></param>
        /// <param name="tasktype"></param>
        /// <returns></returns>
        internal bool HaveInGoods(uint areaId, uint goodsId, TransTypeE tasktype)
        {
            try
            {
                return TransList.Exists(c => !c.finish && c.area_id == areaId
                                && c.TransType == tasktype && c.goods_id == goodsId);
            }
            catch { }
            return true;
        }

        /// <summary>
        /// 判断是否有任务使用了该轨道
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTileTrack(uint trackid)
        {
            try
            {
                return TransList.Exists(c => !c.finish && c.InTrack(trackid));
            }
            catch{ }
            return true;
        }

        /// <summary>
        /// 判断是否有任务使用了该轨道(但是不管倒库任务)
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool HaveInTrackButSortTask(uint trackid)
        {
            try
            {
                //是否忽略倒库任务绑定的轨道
                bool ignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);
                return TransList.Exists(c => !c.finish
                            && (!ignoresort || c.NotInType(TransTypeE.倒库任务, TransTypeE.上砖侧倒库))
                            && c.InTrack(trackid));
            }
            catch { }
            return true;
        }

        /// <summary>
        /// 是否存在砖机使用了该轨道
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="trackid"></param>
        /// <returns></returns>
        internal bool ExistInTileTrack(uint devid, uint trackid)
        {
            return TransList.Exists(c => !c.finish && c.tilelifter_id == devid && c.InTrack(trackid));
        }

        #endregion

        #endregion
    }
}
