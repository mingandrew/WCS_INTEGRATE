using enums;
using GalaSoft.MvvmLight.Messaging;
using module.goods;
using module.msg;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task.diagnose;
using task.trans.transtask;
using tool.appconfig;
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
        internal Log mDtlLog;
        protected List<StockTrans> TransList { set; get; }
        protected List<StockTransDtl> TransDtlList { set; get; }
        private List<StockTrans> organizelist { set; get; }
        #endregion

        #region[分析]

        protected DiagnoseServer MDiagnoreServer;

        #endregion

        #region[任务】

        private InTaskTrans _InTrans;

        //private OutTaskTrans _outTrans;  // 停用
        private OutTaskTransV2 _outTransV2;

        //private In2OutSortTrans _in2outSortTrans;  // 停用
        private In2OutSortTrans_V2 _in2outSortTransV2;

        private Out2OutSortTrans _out2outSortTrans;
        private MoveTaskTrans _moveTrans;
        private SameSideOutTrans _sameSideOutTrans;
        private SameSideInTrans _sameSideInTrans;

        private SeperateStockTrans _seperatestocktrans;
        private MoveStockTrans _movestocktrans;
        private SecondUpTaskTrans _backUpTrans;
        private TransitStockTrans _transitstocktrans;

        #endregion

        #region[构造函数/初始化/启动/停止]
        public TransBase()
        {
            mLog = (Log)new LogFactory().GetLog("任务日志", false);
            mDtlLog = (Log)new LogFactory().GetLog("任务细单", false);
            mTimer = new MTimer();
            mMsg = new MsgAction();
            _to = new object();
            _for = new object();
            TransList = new List<StockTrans>();
            TransDtlList = new List<StockTransDtl>();
            organizelist = new List<StockTrans>();
            InitTrans();
        }

        protected void InitDiagnore(TransMaster trans)
        {
            MDiagnoreServer = new DiagnoseServer(trans);

            _InTrans = new InTaskTrans(trans);

            //_outTrans = new OutTaskTrans(trans);  // 停用
            _outTransV2 = new OutTaskTransV2(trans);

            //_in2outSortTrans = new In2OutSortTrans(trans);  // 停用
            _in2outSortTransV2 = new In2OutSortTrans_V2(trans);

            _out2outSortTrans = new Out2OutSortTrans(trans);
            _moveTrans = new MoveTaskTrans(trans);
            _sameSideOutTrans = new SameSideOutTrans(trans);
            _sameSideInTrans = new SameSideInTrans(trans);

            _backUpTrans = new SecondUpTaskTrans(trans);
            _seperatestocktrans = new SeperateStockTrans(trans);
            _movestocktrans = new MoveStockTrans(trans);
            _transitstocktrans = new TransitStockTrans(trans);
        }

        private void InitTrans()
        {
            TransList.Clear();
            TransList.AddRange(PubMaster.Mod.GoodSql.QueryStockTransList());

            TransDtlList.Clear();
            TransDtlList.AddRange(PubMaster.Mod.GoodSql.QueryTransDtlList());
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
                                        _InTrans.DoTrans(trans);
                                        break;
                                    case TransTypeE.上砖任务:
                                        _outTransV2.DoTrans(trans);
                                        //if (GlobalWcsDataConfig.BigConifg.IsUpTaskNewAllocate(trans.area_id, trans.line))
                                        //{
                                        //    _outTransV2.DoTrans(trans);
                                        //}
                                        //else
                                        //{
                                        //    _outTrans.DoTrans(trans);
                                        //}
                                        break;
                                    case TransTypeE.倒库任务:
                                        _in2outSortTransV2.DoTrans(trans);
                                        //if (GlobalWcsDataConfig.BigConifg.UseSortV2)
                                        //{
                                        //    _in2outSortTransV2.DoTrans(trans);
                                        //}
                                        //else
                                        //{
                                        //    _in2outSortTrans.DoTrans(trans);
                                        //}
                                        break;
                                    case TransTypeE.移车任务:
                                        _moveTrans.DoTrans(trans);
                                        break;
                                    case TransTypeE.手动下砖:
                                        break;
                                    case TransTypeE.手动上砖:
                                        break;
                                    case TransTypeE.同向上砖:
                                        _sameSideOutTrans.DoTrans(trans);
                                        break;
                                    case TransTypeE.同向下砖:
                                        _sameSideInTrans.DoTrans(trans);
                                        break;
                                    case TransTypeE.上砖侧倒库:
                                        _out2outSortTrans.DoTrans(trans);
                                        break;
                                    case TransTypeE.反抛任务:
                                        _backUpTrans.DoTrans(trans);
                                        break;
                                    case TransTypeE.库存整理:
                                    case TransTypeE.中转倒库:
                                        organizelist.Add(trans);
                                        break;
                                    case TransTypeE.库存转移:
                                        _movestocktrans.DoTrans(trans);
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                mLog.Error(true, "[ID:" + TransList[i]?.id + "]", e);
                            }
                        }

                        CheckTrackSort();  //包装前无需倒库
                        //CheckUpTrackSort(); //上砖侧倒库

                        #region[库存整理] 因为在检测的过程中会生成库存转移任务所以不能放在大循环里面
                        foreach (var item in organizelist)
                        {
                            switch (item.TransType)
                            {
                                case TransTypeE.库存整理:
                                    _seperatestocktrans.DoTrans(item);
                                    break;
                                case TransTypeE.中转倒库:
                                    _transitstocktrans.DoTrans(item);
                                    break;
                            }
                        }
                        organizelist.Clear();
                        #endregion
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

        public abstract void CheckTrackSort();
        protected abstract void SendMsg(StockTrans trans);
        #endregion

        #region[交易信息资源管理]

        #region[增删改]

        public uint AddTrans(uint areaid, uint lifterid, TransTypeE type, uint goodsid, byte level, uint stocksid, uint taketrackid, uint givetrackid, uint carrierid = 0, ushort line = 0)
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
                            //if (PubMaster.Track.IsUpSplit(taketrackid))
                            //{
                            //    PubMaster.DevConfig.SetLastTrackId(lifterid, taketrackid);
                            //}
                            break;
                    }
                    transid = AddTransWithoutLock(areaid, lifterid, type, goodsid, level, stocksid, taketrackid, givetrackid, initstatus, carrierid, line);
                }
                finally
                {
                    Monitor.Exit(_for);
                }
            }
            return transid;
        }

        public uint AddTransWithoutLock(uint areaid, uint lifterid, TransTypeE type,
                                        uint goodsid, byte level, uint stocksid,
                                        uint taketrackid, uint givetrackid,
                                        TransStatusE initstatus = TransStatusE.调度设备,
                                        uint carrierid = 0, ushort line = 0, DeviceTypeE ferrytype = DeviceTypeE.其他)
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
                line = line,
                AllocateFerryType = ferrytype,
                level = level
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
                    case TransTypeE.反抛任务:
                    case TransTypeE.库存转移:
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
                    case TransTypeE.库存整理:
                    case TransTypeE.中转倒库:
                        log = string.Format("标识[ {0} ], 任务[ {1} ], 状态[ {2} ], " +
                            "货物[ {3} ], 取轨[ {4} ], 卸轨[ {5} ]",
                            trans.id, type, initstatus,
                            PubMaster.Goods.GetGoodsName(goodsid),
                            PubMaster.Track.GetTrackName(taketrackid, taketrackid + ""),
                            PubMaster.Track.GetTrackName(givetrackid, givetrackid + ""));
                        break;
                    case TransTypeE.移车任务:
                        log = string.Format("标识[ {0} ], 任务[ {1} ], 状态[ {2} ], " +
                            "取轨[ {3} ], 卸轨[ {4} ], 摆渡车[ {5} ]",
                            trans.id, type, initstatus,
                            PubMaster.Track.GetTrackName(taketrackid, taketrackid + ""),
                            PubMaster.Track.GetTrackName(givetrackid, givetrackid + ""), ferrytype);
                        break;
                    default:
                        log = string.Format("标识[ {0} ], 任务[ {1} ], 状态[ {2} ], 砖机[ {3} ], " +
                            "货物[ {4} ], 库存[ {5} ], 取轨[ {6} ], 卸轨[ {7} ]",
                            trans.id, type, initstatus,
                            PubMaster.Device.GetDeviceName(lifterid, lifterid + ""),
                            goodsid, stocksid,
                            PubMaster.Track.GetTrackName(taketrackid, taketrackid + ""),
                            PubMaster.Track.GetTrackName(givetrackid, givetrackid + ""));
                        break;
                }

                if (carrierid > 0)
                {
                    log += string.Format(", 运输车[ {0} ]", PubMaster.Device.GetDeviceName(carrierid, carrierid + ""));
                }
                mLog.Status(true, log);

                if (type == TransTypeE.库存整理 || type == TransTypeE.中转倒库 || type == TransTypeE.库存转移)
                {
                    mDtlLog.Status(true, log);
                }
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

        /// <summary>
        /// 记录步骤日志到界面
        /// -自定义code4位数规则(随意不重复的2位数 + TransTypeE的2位数)
        /// -任务状态(code - TransStatusE)
        /// -流程前提(code - 100~199)
        /// -任务属性(code - 200~299)
        /// -摆渡步骤(code - 300~399)
        /// -运输步骤(code - 400~499)
        /// -轨道步骤(code - 500~599)
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="isOK"></param>
        /// <param name="code">规则(随意不重复的2位数 + TransTypeE的2位数)</param>
        /// <param name="info"></param>
        /// <param name="isRepeat">是否允许重复</param>
        internal void SetStepLog(StockTrans trans, bool isOK, uint code, string info, bool isRepeat = false)
        {
            if (!string.IsNullOrEmpty(info))
            {
                // 界面显示
                if (trans.LogStep(isOK, code, info, isRepeat))
                {
                    // log
                    mLog.Info(true, string.Format("任务[ {0} ], 步骤[ {1} ({2}) ]-[ {3} ]", trans.id, trans.TransStaus, code, info));

                    SendMsg(trans);
                }
            }
        }

        #region 设定任务属性（code-200~299）

        internal void SetStatus(uint transid, TransStatusE status, string memo = "")
        {
            StockTrans trans = GetTrans(transid);
            if (trans != null)
            {
                SetStatus(trans, status, memo);
            }
        }

        internal void SetStatus(StockTrans trans, TransStatusE status, string memo = "")
        {
            if (trans.TransStaus != status)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 状态[ {1} -> {2} ], 备注[ {3} ], 持续[ {4} ]",
                    trans.id, trans.TransStaus, status, memo, trans.GetStatusTimeStr()));
                trans.TransStaus = status;
                trans.TransStausStayTime = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Status);

                if (status == TransStatusE.取消)
                {
                    SetCancel(trans);
                }

                if (status == TransStatusE.完成)
                {
                    if (trans.InType(TransTypeE.上砖任务, TransTypeE.同向上砖, TransTypeE.手动上砖))
                    {
                        //完成需求
                        PubTask.TileLifterNeed.FinishTileLifterNeed(trans.id);
                    }

                    if (trans.InType(TransTypeE.下砖任务, TransTypeE.同向下砖, TransTypeE.手动下砖))
                    {
                        //完成需求
                        PubTask.TileLifterNeed.FinishTileLifterNeed(trans.id);
                    }
                }

                // 重置所有设备分配
                if (status == TransStatusE.调度设备)
                {
                    if (trans.take_ferry_id > 0)
                    {
                        trans.IsReleaseTakeFerry = true;
                        SetTakeFerry(trans, 0, "解锁分配设备，重新恢复任务");
                    }

                    if (trans.give_ferry_id > 0)
                    {
                        trans.IsReleaseGiveFerry = true;
                        SetGiveFerry(trans, 0, "解锁分配设备，重新恢复任务");
                    }

                    if (trans.carrier_id > 0)
                    {
                        SetCarrier(trans, 0, "解锁分配设备，重新恢复任务");
                    }
                }

                //SendMsg(trans);
                SetStepLog(trans, true, (uint)status, string.Format("切换流程[ {0} ]；{1}；", status, memo));

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
                SetStepLog(trans, true, 200, string.Format("锁定运输车[ {0} ]；{1}；", devname, memo));
            }
        }

        internal void SetTakeFerry(StockTrans trans, uint ferryid, string memo = "")
        {
            if (trans.take_ferry_id != ferryid)
            {
                string devname = PubMaster.Device.GetDeviceName(ferryid);
                mLog.Status(true, string.Format("任务[ {0} ], 分配T摆渡车[ {1} ], 备注[ {2} ]", trans.id, devname, memo));
                trans.take_ferry_id = ferryid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TakeFerryId);
                //SendMsg(trans);
                SetStepLog(trans, true, 201, string.Format("锁定接车摆渡车[ {0} ]；{1}；", devname, memo));
            }
        }

        internal void SetGiveFerry(StockTrans trans, uint ferryid, string memo = "")
        {
            if (trans.give_ferry_id != ferryid)
            {
                string devname = PubMaster.Device.GetDeviceName(ferryid);
                mLog.Status(true, string.Format("任务[ {0} ], 分配G摆渡车[ {1} ], 备注[ {2} ]", trans.id, devname, memo));
                trans.give_ferry_id = ferryid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.GiveFerryId);
                //SendMsg(trans);
                SetStepLog(trans, true, 202, string.Format("锁定送车摆渡车[ {0} ]；{1}；", devname, memo));
            }
        }

        internal void FreeTakeFerry(StockTrans trans, string memo = "")
        {
            string devname = PubMaster.Device.GetDeviceName(trans.take_ferry_id);
            mLog.Status(true, string.Format("任务[ {0} ], 解锁T摆渡车[ {1} ], 备注[ {2} ]", trans.id, devname, memo));
            SetStepLog(trans, true, 210, string.Format("解锁接车摆渡车[ {0} ]；{1}；", devname, memo));
        }

        internal void FreeGiveFerry(StockTrans trans, string memo = "")
        {
            string devname = PubMaster.Device.GetDeviceName(trans.give_ferry_id);
            mLog.Status(true, string.Format("任务[ {0} ], 解锁G摆渡车[ {1} ], 备注[ {2} ]", trans.id, devname, memo));
            SetStepLog(trans, true, 211, string.Format("解锁送车摆渡车[ {0} ]；{1}；", devname, memo));
        }

        /// <summary>
        /// 重新分配砖机
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
                SetStepLog(trans, true, 203, string.Format("重新锁定砖机[ {0} ]；{1}；", devname, memo));
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
                SetStepLog(trans, true, 204, string.Format("运输车[ {0} ], 取砖完毕；", PubMaster.Device.GetDeviceName(trans.carrier_id)));
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
                SetStepLog(trans, true, 205, string.Format("运输车[ {0} ], 放砖完毕；", PubMaster.Device.GetDeviceName(trans.carrier_id)));
            }
        }

        public void SetFinish(StockTrans trans)
        {
            if (trans.finish_time == null)
            {
                PubMaster.Warn.RemoveTaskAllWarn(trans.id);
                mLog.Status(true, string.Format("任务[ {0} ], 任务完成[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.finish = true;
                trans.finish_time = DateTime.Now;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Finish);
                //SendMsg(trans);
                SetStepLog(trans, true, 206, string.Format("任务流程结束；"));
            }
        }

        public bool SetTakeSite(StockTrans trans, uint traid)
        {
            if (trans.take_track_id != traid)
            {
                string traname = PubMaster.Track.GetTrackName(traid);
                mLog.Status(true, string.Format("任务[ {0} ], 取货轨道[ {1} ]", trans.id, traname));
                trans.take_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.TakeSite);
                //SendMsg(trans);
                SetStepLog(trans, true, 207, string.Format("重新分配取货轨道[ {0} ]；", traname));
                return true;
            }
            return false;
        }

        public bool SetGiveSite(StockTrans trans, uint traid)
        {
            if (trans.give_track_id != traid)
            {
                string traname = PubMaster.Track.GetTrackName(traid);
                mLog.Status(true, string.Format("任务[ {0} ], 卸货轨道[ {1} ]", trans.id, traname));
                trans.give_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.GiveSite);
                //SendMsg(trans);
                SetStepLog(trans, true, 208, string.Format("重新分配卸货轨道[ {0} ]；", traname));
                return true;
            }
            return false;
        }

        public bool SetFinishSite(StockTrans trans, uint traid, string memo)
        {
            if (trans.finish_track_id != traid)
            {
                string traname = PubMaster.Track.GetTrackName(traid);
                mLog.Status(true, string.Format("任务[ {0} ], 完成轨道[ {1} ], 备注[ {2} ]", trans.id, traname, memo));
                trans.finish_track_id = traid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.FinsihSite);
                //SendMsg(trans);
                SetStepLog(trans, true, 209, string.Format("重新分配结束回轨轨道[ {0} ]；{1}；", traname, memo));
                return true;
            }
            return false;
        }

        public void SetStock(StockTrans trans, uint stockid)
        {
            if (trans.stock_id != stockid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 更改库存[ {1} -> {2} ]]", trans.id, trans.stock_id, stockid));
                trans.stock_id = stockid;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Stock);
            }
        }

        public void SetCancel(StockTrans trans)
        {
            if (!trans.cancel)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 任务取消[ {1} ]", trans.id, DateTime.Now.ToString()));
                trans.cancel = true;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Cancel);
            }
        }

        public void SetReTake(StockTrans trans, uint taketraid, uint stockid, uint carrierid, TransStatusE status)
        {
            trans.take_track_id = taketraid;
            trans.stock_id = stockid;
            trans.carrier_id = carrierid;
            trans.TransStaus = status;
            PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.ReTake);
        }

        public void SetLine(StockTrans trans, ushort line)
        {
            if (line > 0)
            {
                trans.line = line;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Line);
            }
        }

        public void SetGoods(StockTrans trans, uint goodsid, byte level)
        {
            if (trans.goods_id != goodsid)
            {
                mLog.Status(true, string.Format("任务[ {0} ], 更改品种[ {1}^{2} -> {3}^{4} ]]",
                    trans.id, trans.goods_id, trans.level, goodsid, level));
                trans.goods_id = goodsid;
                trans.level = level;
                PubMaster.Mod.GoodSql.EditStockTrans(trans, TransUpdateE.Goods);
            }
        }

        #endregion

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
                return TransList.Exists(c => !c.finish && c.InTrack(ltrack, rtrack));
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
        internal bool HaveInGoods(uint areaId, uint goodsId, List<uint> tileids, params TransTypeE[] tasktype)
        {
            try
            {
                return TransList.Exists(c => !c.finish && c.area_id == areaId
                        && tileids.Contains(c.tilelifter_id)
                    && tasktype.Contains(c.TransType) && c.goods_id == goodsId);
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
            catch { }
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
                //是否开启【接力倒库轨道可以同时上砖】
                bool ignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreSortTask);

                //是否开启【出入倒库轨道可以同时上砖】
                bool inoutignoresort = PubMaster.Dic.IsSwitchOnOff(DicTag.UpTaskIgnoreInoutSortTask);

                return TransList.Exists(c => !c.finish
                            && c.InTrack(trackid)
                            && (!ignoresort || c.NotInType(TransTypeE.上砖侧倒库))
                            && (!inoutignoresort || c.NotInType(TransTypeE.倒库任务))
                            && c.NotInType(TransTypeE.库存整理));
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

        /// <summary>
        /// 判断是否任务使用了该轨道,除了反抛任务
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        //internal bool HaveInTrackButNotSecondUpTask(uint tile_id)
        //{
        //    try
        //    {
        //        if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableSecondUpTask))
        //        {
        //            return TransList.Exists(c => !c.finish && c.tilelifter_id == tile_id && c.NotInType(TransTypeE.反抛任务));
        //        }
        //    }
        //    catch { }
        //    return true;
        //}


        ///// <summary>
        ///// 判断是否有反抛任务使用了该轨道，且品种不一样
        ///// </summary>
        ///// <param name="trackid"></param>
        ///// <returns></returns>
        //internal bool HaveInTrackButSecondUpTask(uint tile_id, uint goodid)
        //{
        //    try
        //    {
        //        if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableSecondUpTask))
        //        {
        //            byte level = PubTask.TileLifter.GetTileLevel(tile_id);
        //            return TransList.Exists(c => !c.finish && c.tilelifter_id == tile_id && c.InType(TransTypeE.反抛任务) && c.goods_id != goodid && c.level == level);
        //        }
        //    }
        //    catch { }
        //    return false;
        //}
        #endregion

        #region [记录步骤信息]

        #region 摆渡车（code-300~399）

        /// <summary>
        /// 分配接车摆渡车失败记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal bool LogForTakeFerry(StockTrans trans, string memo = "")
        {
            if (trans.take_ferry_id > 0) return false;

            SetStepLog(trans, false, 300, string.Format("分配接车摆渡车失败，尝试继续分配；{0}；", memo), true);
            return true;
        }

        /// <summary>
        /// 分配送车摆渡车失败记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal bool LogForGiveFerry(StockTrans trans, string memo = "")
        {
            if (trans.give_ferry_id > 0) return false;

            SetStepLog(trans, false, 301, string.Format("分配送车摆渡车失败，尝试继续分配；{0}；", memo), true);
            return true;
        }

        /// <summary>
        /// 移动摆渡车记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForFerryMove(StockTrans trans, uint ferryid, uint trackid, string memo = "")
        {
            SetStepLog(trans, true, 302, string.Format("控制摆渡车[ {0} ]移至[ {1} ]；{2}；",
                PubMaster.Device.GetDeviceName(ferryid),
                PubMaster.Track.GetTrackName(trackid), memo), true);
        }

        #endregion

        #region 运输车（code-400~499）

        /// <summary>
        /// 分配运输车失败记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal bool LogForCarrier(StockTrans trans, string memo = "")
        {
            if (trans.carrier_id > 0) return false;

            SetStepLog(trans, false, 400, string.Format("分配运输车失败，尝试继续分配；{0}；", memo), true);
            return true;
        }

        /// <summary>
        /// 运输车原地放砖记录
        /// </summary>
        /// <param name="trans"></param>
        internal void LogForCarrierGiving(StockTrans trans)
        {
            SetStepLog(trans, true, 401, string.Format("待运输车[ {0} ]空闲，控制其放下砖；",
                PubMaster.Device.GetDeviceName(trans.carrier_id)));
        }

        /// <summary>
        /// 移动运输车记录（去轨道）
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierToTrack(StockTrans trans, uint trackid, string memo = "")
        {
            SetStepLog(trans, true, 402, string.Format("控制运输车[ {0} ]移至[ {1} ]；{2}",
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Track.GetTrackName(trackid), memo), true);
        }

        /// <summary>
        /// 移动运输车记录（去摆渡）
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierToFerry(StockTrans trans, uint trackid, uint ferryid)
        {
            SetStepLog(trans, true, 403, string.Format("两车都在[ {0} ]，控制运输车[ {1} ]移至摆渡车[ {2} ]；",
                PubMaster.Track.GetTrackName(trackid),
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Device.GetDeviceName(ferryid)));
        }

        /// <summary>
        /// 移动运输车记录（取砖）
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierTake(StockTrans trans, uint trackid, string memo = "")
        {
            SetStepLog(trans, true, 404, string.Format("控制运输车[ {0} ]移至[ {1} ]取砖；{2}",
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Track.GetTrackName(trackid),
                memo), true);
        }

        /// <summary>
        /// 移动运输车记录（放砖）
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierGive(StockTrans trans, uint trackid, string memo = "")
        {
            SetStepLog(trans, true, 405, string.Format("控制运输车[ {0} ]移至[ {1} ]放砖；{2}",
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Track.GetTrackName(trackid),
                memo), true);
        }

        /// <summary>
        /// 移动运输车记录（倒库）
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierSort(StockTrans trans, uint trackid, string memo = "")
        {
            SetStepLog(trans, true, 406, string.Format("控制运输车[ {0} ]移至[ {1} ]倒库-数量[ {2} ]；",
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Track.GetTrackName(trackid), memo));
        }

        /// <summary>
        /// 移动运输车记录（倒库接力）
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierSortRelay(StockTrans trans, uint trackid)
        {
            SetStepLog(trans, true, 407, string.Format("控制运输车[ {0} ]移至[ {1} ]倒库接力；",
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Track.GetTrackName(trackid)));
        }

        /// <summary>
        /// 运输车暂不能取砖记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierNoTake(StockTrans trans, uint trackid)
        {
            SetStepLog(trans, false, 408, string.Format("判断运输车[ {0} ]暂时不可移至[ {1} ]取砖；",
                PubMaster.Device.GetDeviceName(trans.carrier_id),
                PubMaster.Track.GetTrackName(trackid)));
        }

        /// <summary>
        /// 运输车取砖失败的记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="memo"></param>
        internal void LogForCarrierGetStockFalse(StockTrans trans)
        {
            SetStepLog(trans, false, 409, string.Format("[ {0} ]运输车取砖空砖，请检查取砖光电是否异常不亮，(如运输车在储砖轨道请核实轨道库存)，最后给运输车发终止指令；",
                PubMaster.Device.GetDeviceName(trans.carrier_id)));
        }

        #endregion

        #region 轨道（code-500~599）

        /// <summary>
        /// 轨道设为满砖记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        internal void LogForTrackFull(StockTrans trans, uint trackid)
        {
            SetStepLog(trans, true, 500, string.Format("检测到轨道[ {0} ]无法存入下一车，已设为满砖轨道；",
                PubMaster.Track.GetTrackName(trackid)));
        }

        /// <summary>
        /// 轨道设为空砖记录
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="trackid"></param>
        internal void LogForTrackNull(StockTrans trans, uint trackid)
        {
            SetStepLog(trans, true, 501, string.Format("系统已无库存,自动调整轨道[ {0} ]为空轨道；",
                PubMaster.Track.GetTrackName(trackid)));
        }

        #endregion

        #endregion

        #endregion

        #region[任务判断]

        /// <summary>
        /// 是否存在未完成的任务
        /// </summary>
        /// <param name="area"></param>
        /// <param name="库存整理"></param>
        /// <returns></returns>
        public bool ExistTransWithType(ushort area, params TransTypeE[] types)
        {
            return TransList.Exists(c => !c.finish && c.area_id == area && c.InType(types));
        }

        /// <summary>
        /// 是否存在任务使用轨道
        /// </summary>
        /// <param name="trackids"></param>
        /// <returns></returns>
        public bool ExistTransWithTracks(params uint[] trackids)
        {
            return TransList.Exists(c => !c.finish && c.InTrack(trackids) || ExistTrackInDtlUnFinish(trackids));
        }


        #endregion

        #region[细单操作]

        #region[获取细单]

        /// <summary>
        /// 获取总单下面包含的所有交易细单列表
        /// </summary>
        /// <param name="transid"></param>
        /// <returns></returns>
        public List<StockTransDtl> GetTransDtls(uint transid)
        {
            return TransDtlList.FindAll(c => c.dtl_p_id == transid);
        }

        /// <summary>
        /// 获取绑定了总单ID的交易细单
        /// </summary>
        /// <param name="transid"></param>
        /// <returns></returns>
        public StockTransDtl GetTransDtl(uint transid)
        {
            return TransDtlList.Find(c => c.dtl_trans_id == transid);
        }

        /// <summary>
        /// 获取任务对应的品种细单
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public StockTransDtl GetTransDtlInTransGood(StockTrans trans, uint gid)
        {
            return TransDtlList.Find(c => c.dtl_p_id == trans.id && c.dtl_good_id == gid);
        }

        #endregion

        #region[更新细单状态信息]
        /// <summary>
        /// 更新细单状态
        /// </summary>
        /// <param name="dtl"></param>
        /// <param name="status"></param>
        public void SetDtlStatus(StockTransDtl dtl, StockTransDtlStatusE status)
        {
            if (dtl.DtlStatus != status)
            {
                mDtlLog.Status(true, string.Format("任务[ {0} ], 状态[ {1} -> {2} ]", dtl.dtl_id, dtl.DtlStatus, status));

                dtl.DtlStatus = status;
                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.Status);


                SendDtlUpdateMsg(dtl, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 更新细单取货轨道ID
        /// </summary>
        /// <param name="dtl"></param>
        /// <param name="trackid"></param>
        public void SetDtlTakeTrack(StockTransDtl dtl, uint trackid)
        {
            if (dtl.dtl_take_track_id != trackid)
            {
                mDtlLog.Status(true, string.Format("任务[ {0} ], 取货轨道[ {1} -> {2} ]",
                    dtl.dtl_id, PubMaster.Track.GetTrackName(dtl.dtl_take_track_id),
                    PubMaster.Track.GetTrackName(trackid)));
                dtl.dtl_take_track_id = trackid;
                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.TakeTrack);

                SendDtlUpdateMsg(dtl, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 更新细单放货轨道ID
        /// </summary>
        /// <param name="dtl"></param>
        /// <param name="trackid"></param>
        public void SetDtlGiveTrack(StockTransDtl dtl, uint trackid)
        {
            if (dtl.dtl_give_track_id != trackid)
            {
                mDtlLog.Status(true, string.Format("任务[ {0} ], 放货轨道[ {1} -> {2} ]",
                    dtl.dtl_id, PubMaster.Track.GetTrackName(dtl.dtl_give_track_id),
                    PubMaster.Track.GetTrackName(trackid)));

                dtl.dtl_give_track_id = trackid;
                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.GiveTrack);

                SendDtlUpdateMsg(dtl, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 更新细单对应的任务ID
        /// </summary>
        /// <param name="dtl"></param>
        public void SetDtlTransId(StockTransDtl dtl, uint transid)
        {
            if (dtl.dtl_trans_id != transid)
            {
                mDtlLog.Status(true, string.Format("任务[ {0} ], 对应任务[ {1} -> {2} ]", dtl.dtl_id, dtl.dtl_trans_id, transid));

                dtl.dtl_trans_id = transid;
                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.TransId);

                SendDtlUpdateMsg(dtl, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 更新细单为完成状态
        /// </summary>
        /// <param name="dtl"></param>
        public void SetDtlFinish(StockTransDtl dtl)
        {
            if (!dtl.dtl_finish)
            {
                mDtlLog.Status(true, string.Format("任务[ {0} ], 细单完成, 所属主单[ {1} ]", dtl.dtl_id, dtl.dtl_p_id));

                dtl.dtl_finish = true;
                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.Finish);

                SendDtlUpdateMsg(dtl, ActionTypeE.Finish);
            }
        }


        /// <summary>
        /// 更新细单绑定的当前任务为空（任务完成）
        /// </summary>
        /// <param name="id"></param>
        public void SetTransDtlTransFinish(uint id)
        {
            StockTransDtl dtl = GetTransDtl(id);
            if (dtl != null && dtl.dtl_trans_id != 0)
            {
                mDtlLog.Status(true, string.Format("任务[ {0} ], 细单完成, 当前绑定指定任务完成[ {1} ]", dtl.dtl_id, dtl.dtl_trans_id));

                SetDtlTransId(dtl, 0);

                UpdateTransDtlLeftQty(dtl);
                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.TransId);

                SendDtlUpdateMsg(dtl, ActionTypeE.Update);
            }
        }

        /// <summary>
        /// 更新细单剩余数量
        /// </summary>
        /// <param name="id"></param>
        public void UpdateTransDtlLeftQty(StockTransDtl dtl)
        {
            if (dtl != null)
            {
                dtl.dtl_left_qty = PubMaster.Goods.GetTrackGoodCount(dtl.dtl_take_track_id, dtl.dtl_good_id);
                mDtlLog.Status(true, string.Format("任务[ {0} ], 全部数量[ {1} ], 更新剩余数量[ {2} ]", dtl.dtl_id, dtl.dtl_all_qty, dtl.dtl_left_qty));

                PubMaster.Mod.GoodSql.EditTransDtl(dtl, TransDtlUpdateE.Qty);

                SendDtlUpdateMsg(dtl, ActionTypeE.Update);
            }
        }


        /// <summary>
        /// 判断是否存在任务使用了该轨道同时不属于给定的类型内
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool ExistTransWithTrackButType(uint trackid, params TransTypeE[] types)
        {
            return TransList.Exists(c => !c.finish
                                                    && c.InTrack(trackid)
                                                    && c.NotInType(types)) || ExistTrackInDtlUnFinish(trackid);
        }


        /// <summary>
        /// 判断轨道是否被库存整理任务【卸货轨道】占用
        /// </summary>
        /// <param name="trackids"></param>
        /// <returns></returns>
        public bool ExistTrackInDtlUnFinish(params uint[] trackids)
        {
            return TransDtlList.Exists(c => //!c.dtl_finish
                                                        c.DtlStatus == StockTransDtlStatusE.整理中
                                                        && c.DtlType == StockTransDtlTypeE.转移品种
                                                        && trackids.Contains(c.dtl_give_track_id));
        }
        #endregion

        #region[判断细单状态]

        /// <summary>
        /// 判断是否存在同区域未完成的指定类型任务
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal bool ExistUnFinishTrans(uint area_id, uint trackid, TransTypeE type)
        {
            return TransList.Exists(c => !c.finish && c.area_id == area_id && c.InTrack(trackid) && c.InType(type));
        }

        /// <summary>
        /// 完成所有细单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool SetAllTransDtlFinish(uint id)
        {
            List<StockTransDtl> dtl = TransDtlList.FindAll(c => c.dtl_p_id == id && !c.dtl_finish);
            if (dtl == null || dtl.Count == 0)
            {
                return true;
            }

            foreach (StockTransDtl item in dtl)
            {
                SetDtlFinish(item);

                SendDtlUpdateMsg(item, ActionTypeE.Finish);
            }

            return false;
        }

        #endregion

        #endregion

        #region[添加库存整理任务]

        public bool CheckStockDtlGiveTrack(List<StockTransDtl> dtls)
        {
            foreach (var item in dtls)
            {
                if (item.dtl_give_track_id != 0 && ExistTransWithTracks(item.dtl_give_track_id))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加库存整理任务
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dtl"></param>
        public bool AddOrganizeTrans(uint id, List<StockTransDtl> dtl, out string result)
        {
            if (CheckStockDtlGiveTrack(dtl))
            {
                result = "轨道被占用, 请重新设定！";
                return false;
            }

            Track track = PubMaster.Track.GetTrack(id);
            uint transid = AddTransWithoutLock(track.area, 0, TransTypeE.库存整理, 0, 0, 0, track.id, track.id, TransStatusE.调度设备, 0, track.line);

            foreach (var item in dtl)
            {
                if (!AddStockTransDtl(transid, item))
                {

                }
            }

            SetStatus(transid, TransStatusE.整理中, "");

            result = "";
            return true;
        }

        /// <summary>
        /// 平板添加预设整理任务
        /// </summary>
        /// <param name="trackid"></param>
        /// <param name="dtls"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool RfPreSetOrganizeTrans(uint trackid, List<StockTransDtl> dtls, out string result)
        {
            Track track = PubMaster.Track.GetTrack(trackid);
            if (track == null)
            {
                result = "请先选择轨道！";
                return false;
            }

            if (PubTask.Trans.ExistTransWithType(track.area, TransTypeE.库存整理))
            {
                result = "当前已经有一个库存整理任务了！";
                return false;
            }

            if (dtls.Count == 0 || null == dtls.FirstOrDefault(c => c.DtlType == StockTransDtlTypeE.转移品种))
            {
                result = "当前轨道没有需要整理的库存信息";
                return false;
            }

            if (dtls.Count == 1)
            {
                result = "当前轨道只有一个品种，不需要整理";
                return false;
            }
            result = "ok";
            return true;
        }

        public bool UpdateTrackGoodOrganizeTrack(uint trackid, List<StockTransDtl> list, out string result)
        {
            List<uint> tracks = PubMaster.Track.GetTrackFreeEmptyTrackIds(trackid);
            List<uint> freeids = new List<uint>();
            foreach (var item in tracks)
            {
                if (!PubTask.Trans.ExistTransWithTracks(item))
                {
                    freeids.Add(item);
                }
            }

            if (freeids.Count < list.Count(c => c.DtlType == StockTransDtlTypeE.转移品种))
            {
                result = "当前空轨道数量不满足！";
                return false;
            }
            ushort idx = 0;
            foreach (var item in list)
            {
                if (item.DtlType == StockTransDtlTypeE.转移品种)
                {
                    item.dtl_give_track_id = freeids[idx];
                    idx++;
                }
            }
            result = "ok";
            return true;
        }

        public bool AddStockTransDtl(uint transid, StockTransDtl dtl)
        {
            uint dtlid = PubMaster.Dic.GenerateID(DicTag.NewTranDtlId);
            dtl.dtl_id = dtlid;
            dtl.dtl_p_id = transid;
            dtl.dtl_all_qty = PubMaster.Goods.GetTrackGoodCount(dtl.dtl_take_track_id, dtl.dtl_good_id);
            dtl.dtl_left_qty = dtl.dtl_all_qty;
            if (PubMaster.Mod.GoodSql.AddStockTransDtl(dtl))
            {
                TransDtlList.Add(dtl);
                SendDtlUpdateMsg(dtl, ActionTypeE.Add);
                mDtlLog.Status(true, string.Format("细任务[ {0} ], 主任务[ {1} ], 状态[ {2} ], 类型[ {3} ], 货物[ {4} & {8} ], 取货轨道[ {5} ], " +
                    "卸货轨道[ {6} ], 全部数量[ {7} ] ", dtl.dtl_id, dtl.dtl_p_id, dtl.DtlStatus, dtl.DtlType, dtl.dtl_good_id,
                    PubMaster.Track.GetTrackName(dtl.dtl_take_track_id),
                    PubMaster.Track.GetTrackName(dtl.dtl_give_track_id),
                    dtl.dtl_all_qty,
                    PubMaster.Goods.GetGoodsName(dtl.dtl_good_id)));
                return true;
            }
            return false;
        }

        public List<StockTransDtl> GetStockTransDtlsList()
        {
            return TransDtlList.FindAll(c => !c.dtl_finish);
        }

        public void GetAllStockTransDtl()
        {
            foreach (var item in TransDtlList)
            {
                SendDtlUpdateMsg(item, ActionTypeE.Add);
            }
        }

        public void SendDtlUpdateMsg(StockTransDtl dtl, ActionTypeE type)
        {
            MsgAction msg = new MsgAction()
            {
                o1 = dtl,
                o2 = type
            };
            Messenger.Default.Send(msg, MsgToken.StockTransDtlUpdate);
        }

        /// <summary>
        /// 判断任务是否完成
        /// </summary>
        /// <param name="trans_id"></param>
        /// <returns></returns>
        public bool IsTransFinish(uint trans_id)
        {
            return TransList.Exists(c => c.id == trans_id && c.finish);
        }
        #endregion
    }
}
