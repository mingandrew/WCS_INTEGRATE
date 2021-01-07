using enums;
using module.device;
using module.deviceconfig;
using socket.tcp;

namespace task.task
{
    public class TileLifterTask : TaskBase
    {
        #region[兄弟砖机]

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

        public bool InInvoStatus
        {
            get => IsNeed_1 || IsNeed_2;
        }


        public TileShiftStatusE TileShiftStatus
        {
            get => DevStatus?.ShiftStatus ?? TileShiftStatusE.完成;
        }

        public bool StopOneTime { set; get; }

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

        /// <summary>
        /// 当前优先取砖轨道
        /// </summary>
        public uint CurrentTakeId
        {
            get => Device.CurrentTakeId;
        }

        public DevWorkTypeE WorkType
        {
            get => DevConfig.WorkType;
            set => DevConfig.WorkType = value;
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

        internal void DoChangeModel(TileWorkModeE mode, TileFullE full)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.模式, (byte)mode, (byte)full, 0);
        }

        internal void DoUpdateLevel(byte level)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.等级, (byte)level, 0, 0);
        }

        internal void SetInTaskStatus(bool status)
        {
            DevTcp.IsInTaskStatus = status;
        }
        #endregion
    }
}
