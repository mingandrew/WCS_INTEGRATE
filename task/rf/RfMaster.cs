using enums;
using enums.track;
using GalaSoft.MvvmLight.Messaging;
using module;
using module.device;
using module.diction;
using module.msg;
using module.rf;
using module.rf.carrier;
using module.rf.device;
using module.tiletrack;
using module.track;
using resource;
using socket.rf;
using System;
using System.Collections.Generic;
using System.Threading;
using task.device;
using task.task;
using tool.json;
using tool.mlog;

namespace task.rf
{
    public class RfMaster
    {
        #region[字段]
        private MsgAction mMsg;
        private List<RfClient> mClient;
        private RfServer mServer;
        private bool mIsServerStarting;
        private DictionPack mDicPack;
        internal Log _mLog;
        private object _obj;
        private bool isrunning = true;
        private Thread mTread;
        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public RfMaster()
        {
            _obj = new object();
            _mLog = (Log)new LogFactory().GetLog("RfAction", false);
            mMsg = new MsgAction();
            mClient = new List<RfClient>();
            Messenger.Default.Register<RfMsgMod>(this, MsgToken.RfMsgUpdate, RfMsgUpdate);
        }

        public void Start()
        {
            InitClient();
            if (!mIsServerStarting)
            {
                mServer = new RfServer(9007);
                mIsServerStarting = true;
                isrunning = true;
                mTread = new Thread(Refresh)
                {
                    IsBackground = true,
                    Name = "刷新警告"
                };

                mTread.Start();
            }
        }

        private void Refresh()
        {
            while (isrunning)
            {
                Thread.Sleep(5000);
                if (mServer.HaveClientOnline())
                {
                    List<Warning> warns = PubMaster.Warn.GetFatalError();
                    if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                    {
                        try
                        {
                            WarnPack pack = new WarnPack();
                            pack.AddWarnList(warns);
                            SendSuc2AllRf(FunTag.FatalErrorNotice, JsonTool.Serialize(pack));
                        }
                        catch (Exception e)
                        {
                            _mLog.Error(true, e.StackTrace, e);
                        }
                        finally
                        {
                            Monitor.Exit(_obj);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
			isrunning = false;
            mServer?.Stop();
        }

        public void ReStart()
        {
            if (mIsServerStarting)
            {
                mServer?.Stop();
                mIsServerStarting = false;
                new Thread(() => { Thread.Sleep(2000); Start(); })
                {
                    IsBackground = true
                }.Start();
            }
        }

        private void InitClient()
        {
            mClient.Clear();
            mClient.AddRange(PubMaster.Mod.DevSql.QueryRfClientList());
        }

        public void GetAllClient()
        {
            foreach (RfClient client in mClient)
            {
                SendMsg(client);
            }
        }

        #endregion

        #region[发送信息]

        private void SendMsg(RfClient client)
        {
            mMsg.Name = client.ip;
            mMsg.o1 = client;
            Messenger.Default.Send(mMsg, MsgToken.RfStatusUpdate);
        }

        #endregion

        #region[发送客户端信息]

        internal bool SendSucc2Rf(string meid, string func, string data)
        {
            RfPackage pack = new RfPackage()
            {
                Function = func,
                Result = "ok"
            };
            pack.Data = data;

            return mServer.SendMessage(meid, pack);
        }

        internal bool SendFail2Rf(string meid, string func, string errormsg)
        {
            RfPackage pack = new RfPackage()
            {
                Function = func,
                Result = "Fail"
            };
            pack.ErrorMsg = errormsg;

            return mServer.SendMessage(meid, pack);
        }

        public void SendSuc2AllRf(string func, string data)
        {
            RfPackage pack = new RfPackage()
            {
                Function = func,
                Result = "ok"
            };
            pack.Data = data;

            foreach (RfClient client in mClient)
            {
                if(client.rfid != null)
                {
                    mServer.SendMessage(client.rfid, pack);
                }
            }
        }
        #endregion

        #region[接收数据/连接/断开]

        private void RfMsgUpdate(RfMsgMod msg)
        {
            if(Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    switch (msg.Conn)
                    {
                        case RfConnectE.客户端连接:
                        case RfConnectE.客户端断开:
                            CheckClient(msg);
                            break;
                        case RfConnectE.客户端接收信息:
                            ProcessMsg(msg);
                            break;
                        default:
                            break;
                    }
                }catch(Exception e)
                {
                    _mLog.Error(true, e.StackTrace, e);
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
        }

        private void CheckClient(RfMsgMod msg)
        {
            if (msg.IP.Equals(msg.MEID)) return;
            RfClient rf = mClient.Find(c => c.rfid.Equals(msg.MEID));
            if (rf == null)
            {
                rf = new RfClient()
                {
                    rfid = msg.MEID,
                    ip = msg.IP,
                    conn_time =DateTime.Now
                };
                mClient.Add(rf);
                PubMaster.Mod.DevSql.AddRfClient(rf);
            }

            rf.Status = msg.Conn;
            switch (msg.Conn)
            {
                case RfConnectE.客户端连接:
                    rf.conn_time = DateTime.Now;
                    PubMaster.Mod.DevSql.EditRfDevice(rf);
                    break;
                case RfConnectE.客户端断开:
                    rf.disconn_time = DateTime.Now;
                    PubMaster.Mod.DevSql.EditRfDevice(rf);
                    break;
            }

            SendMsg(rf);
        }
        
        #endregion

        #region[处理信息]

        private void ProcessMsg(RfMsgMod msg)
        {
            if (msg.Pack == null) return;
            try
            {
                switch (msg.Pack.Function)
                {
                    #region[基础]
                    case FunTag.InitVersion:
                        GetInitVerion(msg);
                        break;
                    case FunTag.HeartBeat:
                        SendSucc2Rf(msg.MEID, FunTag.HeartBeat, "OK");
                        break;
                    case FunTag.Login:
                        SendSucc2Rf(msg.MEID, FunTag.Login, "OK");
                        CheckClient(msg);
                        break;
                    case FunTag.ModuleView:
                        GetPdaView(msg);
                        CheckClient(msg);
                        break;
                    case FunTag.QueryDicAll:
                        GetDicData(msg);
                        break;
                    case FunTag.UpdateGoodDic:
                        GetGoodDic(msg);
                        break;
                    case FunTag.QueryVersion:
                        GetGoodVerion(msg);
                        break;
                    case FunTag.UserCheck:
                        GetUserPdaView(msg);
                        break;
                    #endregion

                    #region[严重警告]
                    case FunTag.FatalErrorNotice:
                        NoticeFatalError();
                        break;
                    #endregion

                    #region[规格]

                    case FunTag.QueryGoods:
                        GetGoodsList(msg);
                        break;
                    case FunTag.UpdateGood:
                        UpdateGood(msg);
                        break;

                    #endregion

                    #region[库存]

                    case FunTag.QueryStockSum:
                        GetStockSum(msg);
                        break;
                    case FunTag.AddTrackStock:
                        AddTrackStock(msg);
                        break;
                    case FunTag.QueryTrackStock:
                        QueryTrackStock(msg);
                        break;
                    case FunTag.ShiftTrackStock:
                        ShiftTrackStock(msg);
                        break;
                    case FunTag.UpdateStockGood:
                        UpdateStockGood(msg);
                        break;
                    case FunTag.DeleteTrackStock:
                        DeleteTrackStock(msg);
                        break;
                    #endregion

                    #region[轨道]
                    case FunTag.QuerySingleTrack:
                        QuerySingleTrack(msg);
                        break;
                    case FunTag.QueryTrack:
                        GetTrack(msg);
                        break;
                    case FunTag.UpdateTrackStatus:
                        UpdateTrackStatus(msg);
                        break;
                    #endregion

                    #region[砖机]
                    case FunTag.QueryTileGood:
                        GetTileGood(msg);
                        break;
                    case FunTag.UpdateTileGood:
                        UpdateTileGood(msg);
                        break;
                    case FunTag.QueryTileStockGood:
                        QueryTileStockGood(msg);
                        break;
                    #endregion

                    #region[摆渡对位]
                    case FunTag.QueryFerryPos:
                        GetFerryPos(msg);
                        break;
                    case FunTag.StartFerryPos:
                        StartFerryPos(msg);
                        break;
                    case FunTag.StopFerryPos:
                        StopFerryPos(msg);
                        break;
                    case FunTag.UpdateFerryPos:
                        UpdateFerryPos(msg);
                        break;
                    case FunTag.TaskFerryToPos:
                        TaskFerryToPos(msg);
                        break;
                    case FunTag.TaskFerryStop:
                        TaskFerryStop(msg);
                        break;
                    case FunTag.TaskFerryReset:
                        TaskFerryReset(msg);
                        break;

                    #endregion

                    #region[设备警告]
                    case FunTag.QueryWarn:
                        GetWaring(msg);
                        break;
                    #endregion

                    #region[任务开关]

                    case FunTag.QueryTaskSwitch:
                        GetTaskSwitch(msg);
                        break;
                        
                    case FunTag.UpdateTaskSwitch:
                        UpdateTaskSwitch(msg);
                        break;
                    #endregion

                    #region[设备信息]

                    case FunTag.QueryDevice:
                        GetDevice(msg);
                        break;
                    case FunTag.UpdateDevWorking:
                        UpdateDevWorking(msg);
                        break;
                    case FunTag.QueryDevFerry:
                        GetDevFerry(msg);
                        break;
                    case FunTag.QueryDevCarrier:
                        GetDevCarrier(msg);
                        break;
                    case FunTag.QueryDevTileLifter:
                        GetDevTileLifter(msg);
                        break;
                    case FunTag.DoDevCarrierTask:
                        DoDevCarrierTask(msg);
                        break;
                    case FunTag.DoDevFerryTask:
                        DoDevFerryTask(msg);
                        break;
                    case FunTag.DoDevTileLifterTask:
                        DoDevTileLifterTask(msg);
                        break;
                    #endregion

                    #region[任务信息]
                    case FunTag.QueryTrans:
                        GetTrans(msg);
                        break;
                    case FunTag.ForseTransFinish:
                        DoForseTransFinish(msg);
                        break;
                    case FunTag.CancelTrans:
                        DoCancelTrans(msg);
                        break;
                    #endregion

                    #region[按轨出库]
                    case FunTag.QueryTileTrack:
                        GetTileTrack(msg);
                        break;
                    case FunTag.UpdateTileTrack:
                        UpdateTileTrack(msg);
                        break;
                    #endregion

                    #region[砖机转产]

                    case FunTag.QueryTileShift:
                        //查询砖机品种，转产状态
                        QueryTileShift(msg);
                        break;

                    case FunTag.UpdatePreGood:
                        //更新预设品种
                        UpdatePreGood(msg);

                        break;
                    case FunTag.ShiftTileGood:
                        //转品种
                        ShiftTileGood(msg);

                        break;
                    #endregion
                }

                _mLog?.Cmd(true, msg?.MEID + " : " + msg?.Pack?.Function);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                _mLog.Error(true, e.StackTrace, e);
            }
        }


        #region[致命警告]

        private void NoticeFatalError()
        {

        }

        #endregion

        private void GetGoodDic(RfMsgMod msg)
        {
            DictionPack dic = new DictionPack();
            dic.AddGood(PubMaster.Goods.GetGoodsList());
            dic.AddVersion(DicTag.PDA_GOOD_VERSION, PubMaster.Dic.GetDtlIntCode(DicTag.PDA_GOOD_VERSION));
            SendSucc2Rf(msg.MEID, FunTag.UpdateGoodDic, JsonTool.Serialize(dic));
        }


        #region[库存]

        private void GetStockSum(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                if (int.TryParse(msg.Pack.Data, out int areaid))
                {
                    StockSumPack pack = new StockSumPack();
                    pack.AddSumList(PubMaster.Goods.GetStockSums(areaid));
                    string data = JsonTool.Serialize(pack);

                    SendSucc2Rf(msg.MEID, FunTag.QueryStockSum, data);
                }
            }
        }

        private void AddTrackStock(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                TrackStockUpdatePack pack = JsonTool.Deserialize<TrackStockUpdatePack>(msg.Pack.Data);
                if (pack != null)
                {
                    byte picese = PubMaster.Goods.GetGoodsPieces(pack.GoodId);
                    if(PubMaster.Goods.AddTrackStocks(0, pack.TrackId, pack.GoodId,
                        picese, pack.ProduceTime, pack.AddQty, "平板添加库存"))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.AddTrackStock, "添加成功！");
                    }
                }
            }
        }

        private void QueryTrackStock(RfMsgMod msg)
        {
            if(uint.TryParse(msg.Pack.Data, out uint trackid))
            {
                TrackStockPack pack = new TrackStockPack();
                pack.TrackId = trackid;
                pack.AddStocks(PubMaster.Goods.GetStocks(trackid));

                SendSucc2Rf(msg.MEID, FunTag.QueryTrackStock, JsonTool.Serialize(pack));
            }
        }

        private void ShiftTrackStock(RfMsgMod msg)
        {
            if (uint.TryParse(msg.Pack.Data, out uint trackid))
            {
                Track track = PubMaster.Track.GetTrack(trackid);
                string result = null;
                if (track.Type != TrackTypeE.储砖_入)
                {
                    result = "不是储存入轨道";
                }

                if (track.StockStatus != TrackStockStatusE.满砖)
                {
                    result = track.name + "不是满砖状态";
                }

                Track btrack = PubMaster.Track.GetTrack(track.brother_track_id);
                if (btrack == null || btrack.StockStatus != TrackStockStatusE.空砖)
                {
                    result = "对应出轨道为空状态!";
                }

                if(result == null)
                {
                    if (PubMaster.Goods.ShiftStock(track.id, track.brother_track_id))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.ShiftTrackStock, "转移成功！");
                    }
                }
                else
                {
                    SendFail2Rf(msg.MEID, FunTag.ShiftTrackStock, result);
                }
            }
        }

        private void UpdateStockGood(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                TrackStockUpdatePack pack = JsonTool.Deserialize<TrackStockUpdatePack>(msg.Pack.Data);
                if (pack != null)
                {
                    if(!PubMaster.Goods.HaveStockInTrack(pack.TrackId, pack.GoodId, out uint stockid))
                    {
                        SendFail2Rf(msg.MEID, FunTag.UpdateStockGood, "没有原库信息，请刷新后再试！");
                        return;
                    }

                    if (PubMaster.Goods.ChangeStockGood(pack.TrackId, pack.NewGoodId, pack.ChangeDate, pack.ProduceTime))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.UpdateStockGood, "");
                    }
                }
            }
        }

        private void DeleteTrackStock(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                if(uint.TryParse(msg.Pack.Data, out uint stockid))
                {
                    if(!PubMaster.Goods.DeleteStock(stockid, out string result))
                    {
                        SendFail2Rf(msg.MEID, FunTag.DeleteTrackStock, result);
                    }
                    else
                    {
                        SendSucc2Rf(msg.MEID, FunTag.DeleteTrackStock, "");
                    }
                }
            }
        }


        #endregion

        #region[初始化]

        private void GetInitVerion(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                VersionDic pack = JsonTool.Deserialize<VersionDic>(msg.Pack.Data);
                if (pack != null)
                {
                    pack.Differ = PubMaster.Dic.IsVersionDiffer(pack);
                    SendSucc2Rf(msg.MEID, FunTag.InitVersion, JsonTool.Serialize(pack));
                }
            }
        }
        private void GetGoodVerion(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                VersionDic pack = JsonTool.Deserialize<VersionDic>(msg.Pack.Data);
                if (pack != null)
                {
                    pack.Differ = PubMaster.Dic.IsVersionDiffer(pack);
                    SendSucc2Rf(msg.MEID, FunTag.QueryVersion, JsonTool.Serialize(pack));
                }
            }
        }

        private void GetPdaView(RfMsgMod msg)
        {
            UserModelPack userModule = new UserModelPack()
            {
                UserId = msg.IP,
                UserName = "Public",
                UserModuleView = new List<ModuleView>()
                {
                    //new ModuleView()
                    //{
                    //    ModuleName="自定义分配",
                    //    ModuleId = "RFTRACKASSIGN",
                    //    ModulePic = "assignment.png",
                    //    ModuleEntry="com.keda.wcsfixplatformapp.screen.rftrackassignment.RfTrackAssignmentScreen"
                    //},
                    new ModuleView()
                    {
                        ModuleName = "摆渡车对位",
                        ModuleId = "RFARFTRACK",
                        ModulePic ="arttoposition.png",
                        ModuleEntry ="com.keda.wcsfixplatformapp.screen.rfferrypose.RfFerryPosScreen",
                    },

                    //new ModuleView()
                    //{
                    //    ModuleName="运输车设置",
                    //    ModuleId = "RFRGVSETTING",
                    //    ModulePic = "shiftcar.png",
                    //    ModuleEntry="com.keda.wcsfixplatformapp.screen.rfrgvsetting.RfRgvSettingScreen"
                    //},
                    //new ModuleView()
                    //{
                    //    ModuleName="上下砖机设置",
                    //    ModuleId = "RFUPDOWNSETTING",
                    //    ModulePic = "updowndev.png",
                    //    ModuleEntry="com.keda.wcsfixplatformapp.screen.rfupdownsetting.RfUpDownSettingScreen"
                    //},
                    new ModuleView()
                    {
                        ModuleName="品种设置",
                        ModuleId = "RFGOODTYPESETTING",
                        ModulePic = "goodstype.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rfgood.RfGoodMainScreen"
                    },
                    new ModuleView()
                    {
                        ModuleName="轨道设置",
                        ModuleId = "RFTRACK",
                        ModulePic = "assignment.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rftrack.RfTrackScreen"
                    },
                    new ModuleView()
                    {
                        ModuleName="砖机品种",
                        ModuleId = "RFTILEGOOD",
                        ModulePic = "updowndev.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rftilegood.RfTileGoodScreen"
                    }
                    //,
                    //new ModuleView()
                    //{
                    //    ModuleName="其他设置",
                    //    ModuleId = "RFOTHERSETTING",
                    //    ModulePic = "othersetting.png",
                    //    ModuleEntry="com.keda.adrf.screen.rfothersetting.RfOtherSettingScreen"
                    //}
                    ,
                    new ModuleView()
                    {
                        ModuleName="任务开关",
                        ModuleId = "RFTASKSWITCH",
                        ModulePic = "othersetting.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rfswitch.RfSwitchScreen"
                    }
                    ,
                    new ModuleView()
                    {
                        ModuleName="摆渡车状态",
                        ModuleId = "RFDEVFERRY",
                        ModulePic = "othersetting.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rfferry.RfFerryScreen"
                    }
                    ,
                    new ModuleView()
                    {
                        ModuleName="运输车状态",
                        ModuleId = "RFDEVCARRIER",
                        ModulePic = "shiftcar.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rfdevcarrier.RfDevCarrierScreen"
                    }
                    ,
                    new ModuleView()
                    {
                        ModuleName="砖机状态",
                        ModuleId = "RFDEVTILELIFTER",
                        ModulePic = "updowndev.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rftilelifter.RfTileLifterScreen"
                    }
                    ,
                    new ModuleView()
                    {
                        ModuleName="轨道库存",
                        ModuleId = "RFTRACKSTOCK",
                        ModulePic = "assignment.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rftrackstock.RfTrackStockScreen"
                    }
                    ,
                    new ModuleView()
                    {
                        ModuleName="任务信息",
                        ModuleId = "RFSTOCKTRANS",
                        ModulePic = "updowndev.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rftrans.RfTransScreen"
                    }
                    ,
                    new ModuleView()
                    {
                        ModuleName="按轨上砖",
                        ModuleId = "RFTILETRACK",
                        ModulePic = "updowndev.png",
                        ModuleEntry="com.keda.wcsfixplatformapp.screen.rftiletrack.RfTileTrackScreen"
                    }
                }
            };

            SendSucc2Rf(msg.MEID, FunTag.ModuleView, JsonTool.Serialize(userModule));
        }

        private void GetDicData(RfMsgMod msg)
        {
            if(mDicPack == null)
            {
                mDicPack = new DictionPack();

                #region[Enum]

                mDicPack.AddEnum(typeof(StockPosE), "库存位置", nameof(StockPosE));

                mDicPack.AddEnum(typeof(TransTypeE), "任务类型", nameof(TransTypeE));
                mDicPack.AddEnum(typeof(TransStatusE), "任务状态", nameof(TransStatusE));
                mDicPack.AddEnum(typeof(DevOperateModeE), "操作模式", nameof(DevOperateModeE));

                mDicPack.AddEnum(typeof(DeviceTypeE), "设备类型", nameof(DeviceTypeE));
                mDicPack.AddEnum(typeof(DeviceType2E), "设备类型2", nameof(DeviceType2E));
                mDicPack.AddEnum(typeof(StrategyInE), "入库策略", nameof(StrategyInE));
                mDicPack.AddEnum(typeof(StrategyOutE), "出库策略", nameof(StrategyOutE));
                mDicPack.AddEnum(typeof(DevWorkTypeE), "作业类型", nameof(DevWorkTypeE));

                mDicPack.AddEnum(typeof(TrackTypeE), "轨道类型", nameof(TrackTypeE));
                mDicPack.AddEnum(typeof(RfFilterTrackTypeE), "轨道过滤类型", nameof(RfFilterTrackTypeE));
                mDicPack.AddEnum(typeof(TrackStockStatusE), "轨道库存状态", nameof(TrackStockStatusE));
                mDicPack.AddEnum(typeof(TrackStatusE), "轨道状态", nameof(TrackStatusE));
                mDicPack.AddEnum(typeof(SocketConnectStatusE), "通信状态", nameof(SocketConnectStatusE));
                mDicPack.AddEnum(typeof(DevCarrierStatusE), "运输车状态", nameof(DevCarrierStatusE));
                mDicPack.AddEnum(typeof(DevCarrierTaskE), "运输车任务状态", nameof(DevCarrierTaskE));
                mDicPack.AddEnum(typeof(DevCarrierSizeE), "超限模式", nameof(DevCarrierSizeE));
                mDicPack.AddEnum(typeof(DevCarrierLoadE), "载货状态", nameof(DevCarrierLoadE));
                mDicPack.AddEnum(typeof(DevCarrierSignalE), "运输车作业结果信号", nameof(DevCarrierSignalE));
                mDicPack.AddEnum(typeof(CarrierTypeE), "运输车类型", nameof(CarrierTypeE));
                mDicPack.AddEnum(typeof(DevCarrierCmdE), "运输车指令", nameof(DevCarrierCmdE));

                mDicPack.AddEnum(typeof(DevFerryStatusE), "摆渡车状态", nameof(DevFerryStatusE));
                mDicPack.AddEnum(typeof(DevFerryLoadE), "摆渡车载车状态", nameof(DevFerryLoadE));
                mDicPack.AddEnum(typeof(DevFerryTaskE), "摆渡车载车状态", nameof(DevFerryTaskE));
                mDicPack.AddEnum(typeof(DevFerryCmdE), "摆渡车指令", nameof(DevFerryCmdE));

                mDicPack.AddEnum(typeof(DevLifterNeedE), "砖机需求状态", nameof(DevLifterNeedE));
                mDicPack.AddEnum(typeof(DevLifterLoadE), "砖机货物状态", nameof(DevLifterLoadE));
                mDicPack.AddEnum(typeof(DevLifterInvolE), "砖机货物状态", nameof(DevLifterInvolE));
                mDicPack.AddEnum(typeof(DevLifterCmdTypeE), "砖机指令", nameof(DevLifterCmdTypeE));
                mDicPack.AddEnum(typeof(TileShiftStatusE), "转产状态", nameof(TileShiftStatusE));

                #endregion

                #region[List]

                mDicPack.AddArea(PubMaster.Area.GetAreaList());
                mDicPack.AddTrack(PubMaster.Track.GetTrackList());
                mDicPack.AddDevice(PubMaster.Device.GetDeviceList());
                mDicPack.AddGood(PubMaster.Goods.GetGoodsList());
                //mDicPack.AddFerry(PubMaster.Device.GetFerrys());

                #endregion
            }
            mDicPack.AddVersion(DicTag.PDA_INIT_VERSION, PubMaster.Dic.GetDtlIntCode(DicTag.PDA_INIT_VERSION));
            mDicPack.UserLoginFunction = PubMaster.Dic.IsSwitchOnOff(DicTag.UserLoginFunction);
            SendSucc2Rf(msg.MEID, FunTag.QueryDicAll, JsonTool.Serialize(mDicPack));
        }

        private void GetUserPdaView(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                LoginMsg login = JsonTool.Deserialize<LoginMsg>(msg.Pack.Data);
                if (login != null)
                {
                    if(PubMaster.Role.CheckUserGetPdaView(login.username, login.password,out string result, out UserModelPack user))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.UserCheck, JsonTool.Serialize(user));
                    }
                    else
                    {
                        SendFail2Rf(msg.MEID, FunTag.UserCheck, result);
                    }
                }
            }
        }

        #endregion

        #region[心跳]
        private void HeartBeat(RfMsgMod msg)
        {

        }
        #endregion

        #region[摆渡车对位]

        private void GetFerryPos(RfMsgMod msg)
        {
            FerryPosPack gmsg = new FerryPosPack();
            if(uint.TryParse(""+msg.Pack.Data, out uint devid))
            {
                gmsg.Device = devid;
                gmsg.AddPosList(PubMaster.Track.GetFerryPos(devid));
            }

            SendSucc2Rf(msg.MEID, FunTag.QueryFerryPos, JsonTool.Serialize(gmsg));
        }

        private void StartFerryPos(RfMsgMod msg)
        {
            if(uint.TryParse(""+msg.Pack.Data, out uint ferryid))
            {
                PubTask.Ferry.StartRfPosSet(msg.MEID, ferryid);
            }
        }

        private void StopFerryPos(RfMsgMod msg)
        {
            PubTask.Ferry.StopRfPosSet(msg.MEID);
        }

        /// <summary>
        /// 发送摆渡车当前扫描的地标和 光电状态
        /// </summary>
        /// <param name="meid"></param>
        /// <param name="upsite"></param>
        /// <param name="downsite"></param>
        /// <param name="uplight"></param>
        /// <param name="downlight"></param>
        public bool SendFerryLightPos(string meid, ushort upsite, ushort downsite, bool uplight, bool downlight)
        {
            FerrySitePack pack = new FerrySitePack()
            {
                UpSite = upsite,
                DownSite = downsite,
                UpLight = uplight,
                DownLight = downlight
            };
            string data = JsonTool.Serialize(pack);
            return SendSucc2Rf(meid, FunTag.FerrySiteUpdate, data);
        }

        /// <summary>
        /// 发送摆渡车当前坐标地点信息
        /// </summary>
        /// <param name="meid"></param>
        /// <param name="ferrySite"></param>
        public void SendFerrySitePos(string meid, uint devid, DevFerrySite ferrySite)
        {
            FerryNewPosPack pack = new FerryNewPosPack()
            {
                NowTrackPos = ferrySite.NowTrackPos,
                TrackCode = ferrySite.TrackCode,
                TrackSite = ferrySite.TrackPos
            };
            string data = JsonTool.Serialize(pack);
            SendSucc2Rf(meid, FunTag.FerryPosUpdate, data);
        }

        internal void SendFerryPos(uint devid, string ip)
        {
            GetFerryPos(new RfMsgMod()
            {
                Pack = new RfPackage()
                {
                    Data = devid + ""
                },
                IP = ip
            });
        }

        /// <summary>
        /// 发送坐标对位信息给设备
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateFerryPos(RfMsgMod msg)
        {
            if(msg.IsPackHaveData())
            {
                FerryPosUpdatePack pack = JsonTool.Deserialize<FerryPosUpdatePack>(msg.Pack.Data);
                if (pack != null && pack.FerryId > 0 && pack.PosCode > 0 && pack.Position != 0)
                {
                    int trackcode = PubMaster.Track.GetTrackCode(pack.PosCode, pack.FerryId);
                    if (trackcode > 0 && !PubTask.Ferry.SetFerryPos(pack.FerryId, (ushort)trackcode, pack.Position, out string result))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.UpdateFerryPos, result);
                    }
                }
            }
        }

        private void TaskFerryToPos(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                FerryTaskPack pack = JsonTool.Deserialize<FerryTaskPack>(msg.Pack.Data);
                if (pack != null && pack.Id > 0 && pack.Value1 >0)
                {
                    bool isdownferry = PubMaster.Device.IsDevType(pack.Id, DeviceTypeE.下摆渡);
                    if (!PubTask.Ferry.DoManualLocate(pack.Id, pack.Value1, isdownferry, out string locateresult))
                    {
                        SendFail2Rf(msg.MEID, FunTag.TaskFerryToPos, locateresult);
                        return;
                    }
                    SendSucc2Rf(msg.MEID, FunTag.TaskFerryToPos, "");
                }
            }
        }

        private void TaskFerryStop(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                FerryTaskPack pack = JsonTool.Deserialize<FerryTaskPack>(msg.Pack.Data);
                if (pack != null && pack.Id > 0// && pack.Value1 > 0
                    )
                {
                    if(PubTask.Ferry.StopFerry(pack.Id,out string result))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.TaskFerryStop, result);
                    }
                    else
                    {
                        SendFail2Rf(msg.MEID, FunTag.TaskFerryStop, result);
                    }
                }
            }
        }
        
        private void TaskFerryReset(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                FerryTaskPack pack = JsonTool.Deserialize<FerryTaskPack>(msg.Pack.Data);
                if (pack != null && pack.Id > 0)
                {
                    DevFerryResetPosE type = DevFerryResetPosE.前进复位;
                    if(pack.Value1 == 1)
                    {
                        type = DevFerryResetPosE.后退复位;
                    }
                    if (PubTask.Ferry.ReSetFerry(pack.Id, type, out string result))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.TaskFerryReset, result);
                    }
                    else
                    {
                        SendFail2Rf(msg.MEID, FunTag.TaskFerryReset, result);
                    }
                }
            }
        }

        #endregion

        #region[设备状态]

        #endregion

        #region[规格]
        private void GetGoodsList(RfMsgMod msg)
        {
            GoodsPack gmsg = new GoodsPack();
            gmsg.AddGoodList(PubMaster.Goods.GetGoodsList());

            SendSucc2Rf(msg.MEID, FunTag.QueryGoods, JsonTool.Serialize(gmsg));
        }

        private void UpdateGood(RfMsgMod msg)
        {
            GoodUpdatePack pack = JsonTool.Deserialize<GoodUpdatePack>(msg.Pack.Data);
            if(pack != null)
            {
                if (pack.AddGood)
                {
                    if(!PubMaster.Goods.AddGoods(pack.EditGood, out string result))
                    {
                        SendFail2Rf(msg.MEID, FunTag.UpdateGood, result);
                    }
                    else
                    {
                        SendGoodDic2ToAll();
                        SendSucc2Rf(msg.MEID, FunTag.UpdateGood, "");
                    }
                }
            }
        }

        public void SendGoodDic2ToAll()
        {
            DictionPack dic = new DictionPack();
            dic.AddGood(PubMaster.Goods.GetGoodsList());
            dic.AddVersion(DicTag.PDA_GOOD_VERSION, PubMaster.Dic.GetDtlIntCode(DicTag.PDA_GOOD_VERSION));
            SendSuc2AllRf(FunTag.UpdateGoodDic, JsonTool.Serialize(dic));
        }
        #endregion

        #region[上下砖机品种设置]

        private void GetTileGood(RfMsgMod msg)
        {
            TilePack pack = new TilePack();
            pack.AddTileList(PubMaster.Device.GetTileLifters());

            SendSucc2Rf(msg.MEID, FunTag.QueryTileGood, JsonTool.Serialize(pack));
        }

        private void UpdateTileGood(RfMsgMod msg)
        {
            TileGoodUpdatePack pack = JsonTool.Deserialize<TileGoodUpdatePack>(msg.Pack.Data);
            if (pack != null)
            {
                if (PubMaster.Device.SetTileLifterGoods(pack.TileId, pack.GoodId))
                {
                    PubTask.TileLifter.UpdateTileLifterGoods(pack.TileId, pack.GoodId);
                    SendSucc2Rf(msg.MEID, FunTag.UpdateTileGood, pack.GoodId+"");
                }
            }
        }


        private void QueryTileStockGood(RfMsgMod msg)
        {
            StockGoodIdsPack pack = new StockGoodIdsPack();
            pack.AddIds(PubMaster.Goods.GetStockOutGoodsInsList());

            SendSucc2Rf(msg.MEID, FunTag.QueryTileStockGood, JsonTool.Serialize(pack));
        }
        #endregion

        #region[轨道]

        private void QuerySingleTrack(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                if(uint.TryParse(msg.Pack.Data, out uint trackid))
                {
                    Track track = PubMaster.Track.GetTrack(trackid);
                    if (track != null)
                    {
                        SendSucc2Rf(msg.MEID, FunTag.QuerySingleTrack, JsonTool.Serialize(track));
                    }
                    else
                    {
                        SendFail2Rf(msg.MEID, FunTag.QuerySingleTrack, "找不到轨道信息：" + trackid);
                    }
                }
            }
        }

        private void GetTrack(RfMsgMod msg)
        {
            TrackPack pack = new TrackPack();
            if (msg.IsPackHaveData())
            {
                List<TrackTypeE> tlist = new List<TrackTypeE>();
                if (msg.Pack.Data.Contains(":"))
                {
                    string[] types = msg.Pack.Data.Split(':');
                    foreach (string type in types)
                    {
                        if (byte.TryParse(type, out byte btype))
                        {
                            tlist.Add((TrackTypeE)btype);
                        }
                    }
                }
                else
                {
                    if (byte.TryParse(msg.Pack.Data, out byte btype))
                    {
                        tlist.Add((TrackTypeE)btype);
                    }
                }

                pack.AddTrackList(PubMaster.Track.GetTrackList(tlist));
            }
            else
            {
                pack.AddTrackList(PubMaster.Track.GetTrackList());
            }

            SendSucc2Rf(msg.MEID, FunTag.QueryTrack, JsonTool.Serialize(pack));
        }

        private void UpdateTrackStatus(RfMsgMod msg)
        {
            TrackUpdatePack pack = JsonTool.Deserialize<TrackUpdatePack>(msg.Pack.Data);
            if (pack != null)
            {
                if (PubMaster.Track.CheckAndUpateTrackStatus(pack, out string result))
                {
                    SendSucc2Rf(msg.MEID, FunTag.UpdateTrackStatus,"");
                }
                else
                {
                    SendFail2Rf(msg.MEID, FunTag.UpdateTrackStatus, result);
                }
            }
        }
        #endregion

        #region[设备警告]

        private void GetWaring(RfMsgMod msg)
        {
            WarnPack pack = new WarnPack();
            pack.AddWarnList(PubMaster.Warn.GetWarnings());

            SendSucc2Rf(msg.MEID, FunTag.QueryWarn, JsonTool.Serialize(pack));
        }
        #endregion

        #region[任务开关]

        private void GetTaskSwitch(RfMsgMod msg)
        {
            TaskSwitchPack pack = new TaskSwitchPack();
            pack.AddSwitch(PubMaster.Dic.GetSwitchDtl());
            SendSucc2Rf(msg.MEID, FunTag.QueryTaskSwitch, JsonTool.Serialize(pack));
        }

        private void UpdateTaskSwitch(RfMsgMod msg)
        {
            TaskSwitch pack = JsonTool.Deserialize<TaskSwitch>(msg.Pack.Data);
            if (pack != null)
            {
                if(PubMaster.Dic.UpdateSwitch(pack.code, pack.onoff, true))
                {
                    SendSucc2Rf(msg.MEID, FunTag.UpdateTaskSwitch, "ok");
                    //关闭开关，执行人工作业
                    if (!pack.onoff && pack.cleartask)
                    {
                        bool isdown = pack.code.Contains(DicSwitchTag.Down);
                        bool isup = pack.code.Contains(DicSwitchTag.Up);
                        bool issort = pack.code.Contains(DicSwitchTag.Sort);
                        uint areaid = 0;
                        if (pack.code.Contains("1"))
                        {
                            areaid = 1;
                        }
                        else if (pack.code.Contains("2"))
                        {
                            areaid = 2;
                        }
                        else if (pack.code.Contains("3"))
                        {
                            areaid = 3;
                        }
                        else if (pack.code.Contains("4"))
                        {
                            areaid = 4;
                        }
                        else if (pack.code.Contains("5"))
                        {
                            areaid = 5;
                        }
                        if(areaid!=0 && (isdown || isup || issort))
                        {
                            if(isup) PubTask.Trans.StopAreaUp(areaid);
                            if(isdown) PubTask.Trans.StopAreaDown(areaid);
                            if(issort) PubTask.Trans.StopAreaSort(areaid);
                        }
                    }
                }
            }
        }
        #endregion

        #region[设备信息]

        private void GetDevice(RfMsgMod msg)
        {
            if(msg.IsPackHaveData())
            {
                List<DeviceTypeE> tlist = new List<DeviceTypeE>();
                if (msg.Pack.Data.Contains(":"))
                {
                    string[] types = msg.Pack.Data.Split(':');
                    foreach (string type in types)
                    {
                        if (byte.TryParse(type, out byte btype))
                        {
                            tlist.Add((DeviceTypeE)btype);
                        }
                    }
                }
                else
                {
                    if (byte.TryParse(msg.Pack.Data, out byte btype))
                    {
                        tlist.Add((DeviceTypeE)btype);
                    }
                }
                
                RfDevicePack pack = new RfDevicePack();
                pack.AddDevs(PubMaster.Device.GetDevices(tlist)); ;

                SendSucc2Rf(msg.MEID, FunTag.QueryDevice, JsonTool.Serialize(pack));
            }
        }

        private void UpdateDevWorking(RfMsgMod msg)
        {
            RfDeviceWorkPack pack = JsonTool.Deserialize<RfDeviceWorkPack>(msg.Pack.Data);
            if (pack != null)
            {
                if (PubMaster.Device.SetDevWorking(pack.DevId, pack.Working, out DeviceTypeE type))
                {
                    switch (type)
                    {
                        case DeviceTypeE.上砖机:
                        case DeviceTypeE.下砖机:
                            PubTask.TileLifter.UpdateWorking(pack.DevId, pack.Working, pack.WorkType);
                            break;
                        case DeviceTypeE.上摆渡:
                        case DeviceTypeE.下摆渡:
                            PubTask.Ferry.UpdateWorking(pack.DevId, pack.Working);
                            break;
                        case DeviceTypeE.运输车:
                            PubTask.Carrier.UpdateWorking(pack.DevId, pack.Working);
                            break;
                    }

                    SendSucc2Rf(msg.MEID, FunTag.UpdateDevWorking, "");
                }
            }
        }
        private void GetDevFerry(RfMsgMod msg)
        {
            DevFerryPack pack = new DevFerryPack();
            foreach (FerryTask item in PubTask.Ferry.GetDevFerrys())
            {
                pack.AddDev(new RfDevFerry()
                {
                    Id = item.ID,
                    Area = item.AreaId,
                    Ferry = item.DevStatus,
                    Conn = item.ConnStatus
                });
            }

            SendSucc2Rf(msg.MEID, FunTag.QueryDevFerry, JsonTool.Serialize(pack));
        }

        private void GetDevCarrier(RfMsgMod msg)
        {
            DevCarrierPack pack = new DevCarrierPack();
            foreach (CarrierTask item in PubTask.Carrier.GetDevCarriers())
            {
                pack.AddDev(new RfDevCarrier()
                {
                    Id = item.ID,
                    Area = item.AreaId,
                    Carrier = item.DevStatus,
                    Conn = item.ConnStatus
                });
            }

            SendSucc2Rf(msg.MEID, FunTag.QueryDevCarrier, JsonTool.Serialize(pack));
        }
        
        private void GetDevTileLifter(RfMsgMod msg)
        {
            DevTileLifterPack pack = new DevTileLifterPack();
            foreach (TileLifterTask item in PubTask.TileLifter.GetDevTileLifters())
            {
                pack.AddDev(new RfDevTileLifter()
                {
                    Id = item.ID,
                    Area = item.AreaId,
                    TileLifter = item.DevStatus,
                    Conn = item.ConnStatus,
                    Working = item.IsWorking,
                    WorkType = item.WorkType
                });
            }

            SendSucc2Rf(msg.MEID, FunTag.QueryDevTileLifter, JsonTool.Serialize(pack));
        }

        private void DoDevCarrierTask(RfMsgMod msg)
        {
            DevCarrierTaskPack pack = JsonTool.Deserialize<DevCarrierTaskPack>(msg.Pack.Data);
            if (pack != null)
            {
                if (pack.CarrierTask == 128) return;
                DevCarrierTaskE type = (DevCarrierTaskE)pack.CarrierTask;
                if(!PubTask.Carrier.DoManualTask(pack.DevId, type, out string result))
                {
                    SendFail2Rf(msg.MEID, FunTag.DoDevCarrierTask, result);
                    return;
                }
                SendSucc2Rf(msg.MEID, FunTag.DoDevCarrierTask, "ok");
            }
        }

        private void DoDevFerryTask(RfMsgMod msg)
        {

        }

        private void DoDevTileLifterTask(RfMsgMod msg)
        {

        }
        #endregion

        #region[任务信息]

        private void GetTrans(RfMsgMod msg)
        {
            TransPack pack = new TransPack();
            pack.AddTransList(PubTask.Trans.GetTransList());
            string data = JsonTool.Serialize(pack);

            SendSucc2Rf(msg.MEID, FunTag.QueryTrans, data);
        }

        private void DoCancelTrans(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                if(uint.TryParse(msg.Pack.Data, out uint transid))
                {
                    if(!PubTask.Trans.CancelTask(transid, out string result))
                    {
                        SendFail2Rf(msg.MEID, FunTag.CancelTrans, result);
                    }
                    else
                    {
                        SendSucc2Rf(msg.MEID, FunTag.CancelTrans, "ok");
                    }
                }
            }
        }

        private void DoForseTransFinish(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                if (uint.TryParse(msg.Pack.Data, out uint transid))
                {
                    if (!PubTask.Trans.ForseFinish(transid, out string result))
                    {
                        SendFail2Rf(msg.MEID, FunTag.ForseTransFinish, result);
                    }
                    else
                    {
                        SendSucc2Rf(msg.MEID, FunTag.ForseTransFinish, "ok");
                    }
                }
            }
        }
        #endregion

        #region[按轨出库]

        private void GetTileTrack(RfMsgMod msg)
        {
            if (msg.IsPackHaveData() && uint.TryParse(msg.Pack.Data, out uint tileid))
            {
                RfTileTrackPack pack = new RfTileTrackPack();
                pack.TileId = tileid;
                pack.SetTrackList(PubMaster.TileTrack.GetTileTracks(tileid));
                pack.SetAreaTrackList(PubMaster.Area.GetDevTrackList(tileid));

                SendSucc2Rf(msg.MEID, FunTag.QueryTileTrack, JsonTool.Serialize(pack));
            }
        }

        private void UpdateTileTrack(RfMsgMod msg)
        {
            if (msg.IsPackHaveData())
            {
                RfTileTrackPack updatepack = JsonTool.Deserialize<RfTileTrackPack>(msg.Pack.Data);
                if(updatepack.DeleteList != null && updatepack.DeleteList.Count > 0)
                {
                    foreach (TileTrack track in updatepack.DeleteList)
                    {
                        PubMaster.TileTrack.DeleteTileTrack(track);
                    }
                }

                if(updatepack !=null && updatepack.TrackList != null)
                {
                    foreach (TileTrack track in updatepack.TrackList)
                    {
                        PubMaster.TileTrack.EditTileTrack(track, track.order);
                    }
                }

                PubMaster.TileTrack.SortTileTrackList();
                SendSucc2Rf(msg.MEID, FunTag.UpdateTileTrack, "");
            }
        }
        #endregion

        #region[砖机转规格]

        //查询砖机品种，转产状态
        private void QueryTileShift(RfMsgMod msg) 
        {
            RfTileShiftPack pack = new RfTileShiftPack();
            foreach (TileLifterTask item in PubTask.TileLifter.GetDevTileLifters())
            {
                pack.AddTileShift(item.Device, item.TileShiftStatus);
            }

            if(pack.TileShift != null)
            {
                SendSucc2Rf(msg.MEID, FunTag.QueryTileShift, JsonTool.Serialize(pack));
            }
        }


        //更新预设品种
        private void UpdatePreGood(RfMsgMod msg) 
        {
            if (msg.IsPackHaveData())
            {
                RfTileGoodPack pack = JsonTool.Deserialize<RfTileGoodPack>(msg.Pack.Data);
                if (pack != null)
                {
                    if(PubMaster.Device.UpdateTilePreGood(pack.tile_id, pack.good_id, pack.pregood_id, out string result))
                    {
                        SendSucc2Rf(msg.MEID, FunTag.UpdatePreGood, "ok");
                    }
                    else
                    {
                        SendFail2Rf(msg.MEID, FunTag.UpdatePreGood, result);
                    }
                }
            }
        }
        
        //转品种
        private void ShiftTileGood(RfMsgMod msg) 
        {
            RfTileGoodPack pack = JsonTool.Deserialize<RfTileGoodPack>(msg.Pack.Data);
            if (pack != null)
            {
                if (PubMaster.Device.UpdateShiftTileGood(pack.tile_id, pack.good_id, out string result))
                {
                    //发送砖机转产信号
                    SendSucc2Rf(msg.MEID, FunTag.ShiftTileGood, "ok");
                }
                else
                {
                    SendFail2Rf(msg.MEID, FunTag.ShiftTileGood, result);
                }
            }
        }
                    
        #endregion
        #endregion
    }
}
