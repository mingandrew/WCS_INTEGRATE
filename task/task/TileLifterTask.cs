using enums;
using module.device;
using module.deviceconfig;
using socket.tcp;
using System;

namespace task.task
{
    public class TileLifterTask : TaskBase
    {
        #region[砖机配置信息]

        /// <summary>
        /// 需要根据兄弟砖机状态才能作业
        /// </summary>
        public bool HaveBrother
        {
            get => DevConfig?.HaveBrother ?? true;
        }

        public uint BrotherId
        {
            get => DevConfig.brother_dev_id;
        }

        public bool IsTwoTrack
        {
            get => Device.Type2 == DeviceType2E.双轨;
        }

        public StrategyInE InStrategy
        {
            get => DevConfig?.InStrategey ?? StrategyInE.同机同轨;
            set => DevConfig.InStrategey = value;
        }

        public StrategyOutE OutStrategy
        {
            get => DevConfig?.OutStrategey ?? StrategyOutE.同规同轨;
            set => DevConfig.OutStrategey = value;
        }

        /// <summary>
        /// 砖机类型
        /// </summary>
        public TileLifterTypeE TileLifterType
        {
            get => Device.TileLifterType;
        }
        
        public bool IsCanCutover
        {
            get => DevConfig?.can_cutover ?? false;
        }
        #endregion

        #region[属性]

        public bool IsNeed_1
        {
            get => DevStatus?.Need1 ?? false;
        }

        public bool IsNeed_2
        {
            get => DevStatus?.Need2 ?? false;
        }

        public bool IsLoad_1
        {
            get => DevStatus?.Load1 ?? false;
        }

        public bool IsEmpty_1
        {
            get => !DevStatus?.Load1 ?? false;
        }

        public bool IsLoad_2
        {
            get => DevStatus?.Load2 ?? false;
        }
        public bool IsEmpty_2
        {
            get => !DevStatus?.Load2 ?? false;
        }

        public bool IsInvo_1
        {
            get => DevStatus?.Involve1 ?? false;
        }
        
        public bool IsInvo_2
        {
            get => DevStatus?.Involve2 ?? false;
        }

        public byte FullQty
        {
            get => DevStatus?.FullQty ?? 66;
        }

        public byte Site1Qty
        {
            get => DevStatus?.Site1Qty ?? FullQty;
        }

        public byte Site2Qty
        {
            get => DevStatus?.Site2Qty ?? FullQty;
        }

        public TileShiftStatusE TileShiftStatus
        {
            get => DevStatus?.ShiftStatus ?? TileShiftStatusE.完成;
        }

        public bool StopOneTime { set; get; }

        public bool Ignore_1 = false;//忽略1

        public bool Ignore_2 = false;//忽略2

        #endregion

        #region[下砖/上砖策略]

        /// <summary>
        /// 入库策略
        /// </summary>
        public StrategyInE StrategyIn
        {
            get => DevConfig.InStrategey;
            set => DevConfig.InStrategey = value;
        }

        /// <summary>
        /// 出库策略
        /// </summary>
        public StrategyOutE StrategyOut
        {
            get => DevConfig.OutStrategey;
            set => DevConfig.OutStrategey = value;
        }

        public DevWorkTypeE WorkType
        {
            get => DevConfig?.WorkType ?? DevWorkTypeE.品种作业;
            set => DevConfig.WorkType = value;
        }

        public bool IsConnect
        {
            get => DevTcp?.IsConnected ?? false;
        }
        #endregion

        #region[构造/启动/停止]
        public TileLifterTcp DevTcp { set; get; }
        public DevTileLifter DevStatus { set; get; }
        public ConfigTileLifter DevConfig { set; get; }

        public TileLifterTask()
        {
            DevStatus = new DevTileLifter();
            DevConfig = new ConfigTileLifter();
        }

        public void Start(string memo = "开始连接")
        {
            if (!IsEnable) return;

            if (DevTcp == null)
            {
                DevTcp = new TileLifterTcp(Device);
            }

            if (!DevTcp.m_Working)
            {
                DevTcp.Start(memo);
            }
        }

        public void Stop(string memo = "连接断开")
        {
            DevTcp?.Stop(memo);
        }
        #endregion

        #region[发送指令]

        internal void DoQuery()
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.查询, 0, 0, 0);
        }

        internal void Do1Invo(DevLifterInvolE invo)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.介入1, (byte)invo, 0, 0);
        }

        internal void Do2Invo(DevLifterInvolE invo)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.介入2, (byte)invo, 0, 0);
        }

        internal void DoShift(TileShiftCmdE ts, byte count = 0, uint goods = 0)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.转产, (byte)ts,  count, goods);
        }

        internal void DoCutover(TileWorkModeE mode, TileFullE full)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.模式, (byte)mode, (byte)full, 0);
        }

        internal void DoUpdateLevel(byte level)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.等级, level, 0, 0);
        }

        internal void DoTileShiftSignal(TileAlertShiftE ts)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.复位转产, (byte)ts, 0, 0);
        }

        internal void SetInTaskStatus(bool status)
        {
            DevTcp.IsInTaskStatus = status;
        }

        internal bool IsSiteGoodSame()
        {
            return DevStatus.Goods1 == DevStatus.Goods2;
        }
        #endregion
    }
}
