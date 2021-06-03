using enums;
using enums.warning;
using module.device;
using module.deviceconfig;
using module.track;
using resource;
using socket.tcp;
using System;
using System.Collections.Generic;
using System.Threading;

namespace task.task
{
    public class FerryTask : TaskBase
    {
        #region[逻辑属性]

        /// <summary>
        /// 摆渡车是否被任务锁定
        /// </summary>
        public bool IsLock { set; get; }

        /// <summary>
        /// 被锁定的任务ID
        /// </summary>
        public uint TransId { set; get; }

        /// <summary>
        /// 最近锁定刷新时间
        /// </summary>
        public DateTime? LockRefreshTime { set; get; }

        internal bool _cleaning;//清除其他轨道进行中

        /// <summary>
        /// 摆渡车暂记目标轨道ID
        /// </summary>
        public uint RecordTraId { set; get; }

        #endregion

        #region[属性]

        /// <summary>
        /// 设备状态
        /// </summary>
        public DevFerryStatusE Status
        {
            get => DevStatus?.DeviceStatus ?? DevFerryStatusE.设备故障;
        }

        /// <summary>
        /// 操作模式
        /// </summary>
        public DevOperateModeE OperateMode
        {
            get => DevStatus?.WorkMode ?? DevOperateModeE.无;
        }

        /// <summary>
        /// 有货状态
        /// </summary>
        public DevFerryLoadE Load
        {
            get => DevStatus?.LoadStatus ?? DevFerryLoadE.异常;
        }

        /// <summary>
        /// 下砖侧是否对上轨道
        /// </summary>
        public bool IsDownLight
        {
            get => DevStatus?.DownLight ?? false;
        }

        /// <summary>
        /// 上砖侧是否对上轨道
        /// </summary>
        public bool IsUpLight
        {
            get => DevStatus?.UpLight ?? false;
        }

        /// <summary>
        /// 上砖侧经过轨道号
        /// </summary>
        public ushort UpSite
        {
            get => DevStatus?.UpSite ?? 0;
        }

        /// <summary>
        /// 下砖侧经过轨道号
        /// </summary>
        public ushort DownSite
        {
            get => DevStatus?.DownSite ?? 0;
        }

        private uint uptraid, downtraid;

        /// <summary>
        /// 上砖侧经过轨道ID
        /// </summary>
        public uint UpTrackId
        {
            get => uptraid;
            set
            {
                if (uptraid != value)
                {
                    try
                    {
                        DevTcp.AddStatusLog(string.Format("上[ {0} ]", PubMaster.Track.GetTrackName(value)));
                    }
                    catch { }
                }
                uptraid = value;
            }
        }

        /// <summary>
        /// 下砖侧经过轨道ID
        /// </summary>
        public uint DownTrackId
        {
            get => downtraid;
            set
            {
                if (downtraid != value)
                {
                    try
                    {
                        DevTcp.AddStatusLog(string.Format("下[ {0} ]", PubMaster.Track.GetTrackName(value)));
                    }
                    catch { }
                }
                downtraid = value;
            }
        }


        /// <summary>
        /// 摆渡目的轨道ID
        /// </summary>
        public uint TargetTrackId
        {
            get => PubMaster.Track.GetAreaTrack(ID, (ushort)AreaId, Type, DevStatus.TargetSite);
        }


        /// <summary>
        /// 摆渡轨道ID
        /// </summary>
        public uint FerryTrackId
        {
            get => DevConfig?.track_id ?? 0;
        }

        /// <summary>
        /// 通讯状态
        /// </summary>
        public bool IsConnect
        {
            get => DevTcp?.IsConnected ?? false;
        }

        /// <summary>
        /// 判断摆渡车是否没有在执行任务
        /// </summary>
        public bool IsNotDoingTask
        {
            get => Status == DevFerryStatusE.停止 
                && (DevStatus.CurrentTask == DevStatus.FinishTask // 当前&完成 一致
                    || DevStatus.CurrentTask == DevFerryTaskE.无 // 当前无指令就当它没作业
                    || DevStatus.CurrentTask == DevFerryTaskE.终止// 当前终止就当它没作业
                    || (DevStatus.CurrentTask == DevFerryTaskE.定位
                        && DevStatus.FinishTask == DevFerryTaskE.无
                        && DevStatus.TargetSite == 0)// 手动后可能出现【目标-0，当前-定位，完成-无】
                    );
        }

        #endregion

        #region[构造/启动/停止]

        public FerryTcp DevTcp { set; get; }
        public DevFerry DevStatus { set; get; }
        public DevFerrySite DevSite { set; get; }
        public ConfigFerry DevConfig { set; get; }

        public FerryTask() : base()
        {
            DevStatus = new DevFerry();
            DevConfig = new ConfigFerry();
        }

        public void Start(string memo = "开始连接")
        {
            if (!IsEnable) return;

            if (DevTcp == null)
            {
                DevTcp = new FerryTcp(Device);
            }

            if (!DevTcp.m_Working)
            {
                DevTcp.Start(memo);
            }
        }

        public void Stop(string memo)
        {
            DevTcp?.Stop(memo);
        }
        #endregion

        #region[判断状态]
        public bool IsOnSite(ushort ferrycode)
        {
            return (UpSite == ferrycode && IsUpLight) || (DownSite == ferrycode && IsDownLight);
        }

        #endregion

        #region[发送指令]
        internal void DoQuery()
        {
            DevTcp?.SendCmd(DevFerryCmdE.查询, 0, 0, 0);
        }

        /// <summary>
        /// 摆渡车对位
        /// </summary>
        /// <param name="trackcode"></param>
        /// <param name="ltrack"></param>
        /// <param name="recodeTraid"></param>
        internal void DoLocate(ushort trackcode, uint ltrack, uint recodeTraid)
        {
            // 记录点未清零，不发其他定位
            if (RecordTraId > 0 && RecordTraId != recodeTraid) return;

            // 记录目标点
            RecordTraId = recodeTraid;
            DevTcp.AddStatusLog(string.Format("记录目标[ {0} ]", PubMaster.Track.GetTrackName(RecordTraId)));

            int speed = 2; // 快速移动

            if (DevStatus.LoadStatus != DevFerryLoadE.空)
            {
                if (PubTask.Carrier.IsLoadInFerry(ltrack))
                {
                    speed = 1; // 慢速移动
                }
            }

            byte[] b = BitConverter.GetBytes(trackcode);
            DevTcp?.SendCmd(DevFerryCmdE.定位, b[1], b[0], speed);
        }


        internal void DoSiteQuery(ushort trackcode)
        {
            byte[] b = BitConverter.GetBytes(trackcode);
            DevTcp?.SendCmd(DevFerryCmdE.查询轨道坐标, b[1], b[0], 0);
        }

        internal void DoSiteUpdate(ushort trackcode, int trackpos)
        {
            byte[] b = BitConverter.GetBytes(trackcode);
            DevTcp?.SendCmd(DevFerryCmdE.设置轨道坐标, b[1], b[0], trackpos);
            DevTcp.AddStatusLog(string.Format("设置轨道坐标, 轨道[ {0} ], 位置[ {1} ], 值[ {2} ]", Device.name, trackcode, trackpos));
        }

        internal void DoReSet(DevFerryResetPosE resetpos)
        {
            DevTcp?.SendCmd(DevFerryCmdE.原点复位, (byte)resetpos, 0, 0);
            RecordTraId = 0;
            DevTcp.AddStatusLog(string.Format("复位-清除记录目标"));

            //发送原点指令同时重新写入已经对位的数据
            if (!IsSendAll)
            {
                DoSendAllPose();
            }
        }

        /// <summary>
        /// 终止
        /// </summary>
        /// <param name="memo"></param>
        internal void DoStop(string memo, string purpose)
        {
            DevTcp?.SendCmd(DevFerryCmdE.终止任务, 0, 0, 0);
            // 清除 记录目标点
            RecordTraId = 0;
            DevTcp.AddStatusLog(string.Format("终止[ {0} ], 目的[ {1} ]", memo, purpose));
        }

        internal void DoAutoPos(DevFerryAutoPosE posside, ushort starttrack, byte tracknumber, string memo)
        {
            byte[] b = BitConverter.GetBytes(starttrack);
            DevTcp?.SendAutoPosCmd(DevFerryCmdE.自动对位, b[1], b[0], (byte)posside, tracknumber);
            DevTcp.AddStatusLog(string.Format("摆渡车[ {0} ], 开始自动对位, 对位测[ {1} ], 开始轨道[ {2} ], 对位数量[ {3} ], 备注[ {4} ]",
                Device.name, posside, starttrack, tracknumber, memo));
        }


        #endregion

        #region[更新轨道信息]

        internal void UpdateInfo()
        {
            if (UpSite == 0)
            {
                UpTrackId = 0;
            }

            if (UpSite != 0 && (DevStatus?.IsUpSiteChange ?? false))
            {
                UpTrackId = PubMaster.Track.GetAreaTrack(ID, (ushort)AreaId, Type, UpSite);
            }

            if (DownSite == 0)
            {
                DownTrackId = 0;
            }

            if (DownSite != 0 && (DevStatus?.IsDownSiteChange ?? false))
            {
                DownTrackId = PubMaster.Track.GetAreaTrack(ID, (ushort)AreaId, Type, DownSite);
            }

            //改为发送复位指令的时候就开始发送
            //if(DevStatus.CurrentTask == DevFerryTaskE.复位
            //    && DevStatus.FinishTask == DevFerryTaskE.复位)
            //{
            //    DoStop();
            //    if (!IsSendAll)
            //    {
            //        DoSendAllPose();
            //    }
            //}
        }
        #endregion

        #region[锁定摆渡车]

        /// <summary>
        /// 摆渡车是否空闲
        /// </summary>
        /// <returns></returns>
        public bool IsFerryFree()
        {
            if (Load == DevFerryLoadE.载车)
            {
                return false;
            }

            if (TransId == 0 && !IsLock)
            {
                return true;
            }

            if (IsLockOverTime() && IsLock)
            {
                IsLock = false;
                TransId = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 继续锁定摆渡车
        /// </summary>
        /// <param name="transid"></param>
        public bool IsStillLockInTrans(uint transid)
        {
            if (IsLock && TransId == transid)
            {
                LockRefreshTime = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 解锁摆渡车
        /// </summary>
        /// <param name="transid"></param>
        public void SetFerryUnlock(uint transid)
        {
            if (IsLock && TransId == transid)
            {
                TransId = 0;
                IsLock = false;
            }
        }

        /// <summary>
        /// 摆渡车是否被锁定
        /// </summary>
        /// <returns></returns>
        public bool IsFerryLock()
        {
            if (Load == DevFerryLoadE.载车)
            {
                return true;
            }

            if (IsLockOverTime() || TransId == 0)
            {
                return false;
            }

            return true;
        }

        internal void SetFerryLock(uint id)
        {
            TransId = id;
            IsLock = true;
            LockRefreshTime = DateTime.Now;
        }

        /// <summary>
        /// 是否锁定超时
        /// </summary>
        /// <returns></returns>
        public bool IsLockOverTime()
        {
            if (LockRefreshTime is null)
            {
                LockRefreshTime = DateTime.Now;
            }

            if (LockRefreshTime is DateTime time && (DateTime.Now - time).TotalSeconds > 20)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region  获取属性

        internal uint GetFerryCurrentTrackId()
        {
            uint trackId = 0;
            switch (Type)
            {
                case DeviceTypeE.上摆渡:
                    trackId = IsUpLight ? UpTrackId : DownTrackId;
                    break;
                case DeviceTypeE.下摆渡:
                    trackId = IsDownLight ? DownTrackId : UpTrackId;
                    break;
                default:
                    break;
            }
            return trackId;
        }

        /// <summary>
        /// 根据方向获取摆渡车所在轨道
        /// </summary>
        /// <param name="checkuplight"></param>
        /// <returns></returns>
        internal uint GetFerryCurrentTrackId(bool checkuplight)
        {
            if (checkuplight && IsUpLight)
            {
                return UpTrackId;
            }

            if (!checkuplight && IsDownLight)
            {
                return DownTrackId;
            }

            return 0;
        }

        /// <summary>
        /// 获取摆渡车当前所有对位轨道
        /// </summary>
        /// <returns></returns>
        internal List<uint> GetFerryCurrentTrackIds()
        {
            List<uint> trackIds = new List<uint>();
            if (IsUpLight)
            {
                trackIds.Add(UpTrackId);
            }
            if (IsDownLight)
            {
                trackIds.Add(DownTrackId);
            }

            if (trackIds == null || trackIds.Count == 0) // 都不亮，以储砖轨道为准
            {
                switch (Type)
                {
                    case DeviceTypeE.上摆渡:
                        trackIds.Add(DownTrackId);
                        break;
                    case DeviceTypeE.下摆渡:
                        trackIds.Add(UpTrackId);
                        break;
                    default:
                        break;
                }
            }

            return trackIds;
        }

        #endregion

        #region[清除摆渡车未配置的其他轨道对位信息]

        internal void StartClearOtherTrackPos(List<FerryPos> ferryPos)
        {
            _cleaning = true;
            ushort fpos = 0;
            new Thread(() =>
            {
                for (ushort i = 100; i <= 500; i += 200)
                {
                    for (ushort j = 1; j <= 99; j++)
                    {
                        fpos = (ushort)(i + j);
                        if (!ferryPos.Exists(c => c.ferry_code == fpos))
                        {
                            DoSiteUpdate(fpos, 0);
                            Thread.Sleep(100);
                        }
                    }
                }
                _cleaning = false;
            })
            {
                IsBackground = true
            }.Start();
        }

        #endregion

        #region[重新发送全部对位数据]

        public bool IsSendAll = false;
        internal void DoSendAllPose()
        {
            if (IsSendAll) return;
            IsSendAll = true;

            new Thread(SendAllPos)
            {
                IsBackground = true,
                Name = "发送全部对位信息"
            }.Start();
        }

        /// <summary>
        /// 重新发送所有已经对位的数据
        /// </summary>
        private void SendAllPos()
        {
            List<FerryPos> posList = PubMaster.Track.GetFerryPos(ID);
            try
            {
                foreach (var item in posList)
                {
                    try
                    {
                        if (item.ferry_pos != 0)
                        {
                            DoSiteUpdate(item.ferry_code, item.ferry_pos);
                            Thread.Sleep(500);
                        }
                    }
                    catch { }
                }
            }
            finally
            {
                IsSendAll = false;
            }
        }
        #endregion

        #region [检查报警]

        /// <summary>
        /// 检查报警
        /// </summary>
        public void CheckAlert()
        {
            // 断连停用 啥也不报警了
            if (!IsEnable && !IsWorking)
            {
                PubMaster.Warn.RemoveDevWarn((ushort)ID);
                return;
            }

            Alert1();
            RunWarningLogic();
        }

        /// <summary>
        /// 逻辑报警
        /// </summary>
        private void RunWarningLogic()
        {
            #region 失去位置信息-报警
            uint currentTraid = GetFerryCurrentTrackId();
            Track currentTrack = PubMaster.Track.GetTrack(currentTraid);
            if (currentTrack == null || currentTraid == 0 || currentTraid.Equals(0) || currentTraid.CompareTo(0) == 0)
            {
                PubMaster.Warn.AddDevWarn(WarningTypeE.FerryNoLocation, (ushort)ID);
            }
            else
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.FerryNoLocation, (ushort)ID);
            }
            #endregion

            #region 没有对位坐标值-报警
            if (Status == DevFerryStatusE.停止 && DevStatus.TargetSite > 0)
            {
                if (PubMaster.Track.GetFerryPos(ID).Exists(c => c.ferry_code == DevStatus.TargetSite && c.ferry_pos != 0))
                {
                    PubMaster.Warn.RemoveDevWarn(WarningTypeE.FerryTargetUnconfigured, (ushort)ID);
                }
                else
                {
                    PubMaster.Warn.AddDevWarn(WarningTypeE.FerryTargetUnconfigured, (ushort)ID);
                }
            }
            else
            {
                PubMaster.Warn.RemoveDevWarn(WarningTypeE.FerryTargetUnconfigured, (ushort)ID);
            }
            #endregion

        }

        private void Alert1()
        {
            if (DevStatus.Reserve == 0)
            {
                PubMaster.Warn.RemoveFerryWarn((ushort)ID, 1);
                return;
            }

            // 7 6 5 4   3 2 1 0 从零开始算
            if (On(DevStatus.Reserve, 0))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X0, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X0, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 1))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X1, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X1, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 2))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X2, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X2, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 3))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X3, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X3, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 4))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X4, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X4, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 5))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X5, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X5, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 6))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X6, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X6, (ushort)ID);
            }

            if (On(DevStatus.Reserve, 7))
            {
                PubMaster.Warn.AddFerryWarn(FerryWarnE.WarningF_A1X7, (ushort)ID, 1);
            }
            else
            {
                PubMaster.Warn.RemoveFerryWarn(FerryWarnE.WarningF_A1X7, (ushort)ID);
            }
        }

        private bool On(byte b, byte p)
        {
            return (b >> p) % 2 == 1;
        }

        #endregion

        #region [数据信息]

        /// <summary>
        /// 获取当前摆渡车信息
        /// 运动/指令/位置
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            return string.Format("摆渡车[ {0} ], 设备状态[ {1} ], 当前指令[{2} ], 完成指令[{3} ], 当前轨道[ {4} ], 目的轨道[ {5} ], 记录轨道[ {6} ]",
                Device.name, DevStatus.DeviceStatus, DevStatus.CurrentTask, DevStatus.FinishTask, 
                PubMaster.Track.GetTrackName(GetFerryCurrentTrackId()), 
                PubMaster.Track.GetTrackName(TargetTrackId), 
                PubMaster.Track.GetTrackName(RecordTraId));
        }

        #endregion
    }
}
