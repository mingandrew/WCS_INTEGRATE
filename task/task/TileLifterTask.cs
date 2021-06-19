using enums;
using module.device;
using module.deviceconfig;
using socket.tcp;
using System;
using tool.appconfig;

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

        /// <summary>
        /// 兄弟砖机ID
        /// </summary>
        public uint BrotherId
        {
            get => DevConfig.brother_dev_id;
        }

        /// <summary>
        /// 是否双工位
        /// </summary>
        public bool IsTwoTrack
        {
            get => Device.Type2 == DeviceType2E.双轨;
        }

        /// <summary>
        /// 入库策略
        /// </summary>
        public StrategyInE InStrategy
        {
            get => DevConfig?.InStrategey ?? StrategyInE.同机同轨;
            set => DevConfig.InStrategey = value;
        }

        /// <summary>
        /// 出库策略
        /// </summary>
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

        /// <summary>
        /// 是否允许切换作业模式
        /// </summary>
        public bool IsCanCutover
        {
            get => DevConfig?.can_cutover ?? false;
        }
        #endregion

        #region[属性]

        /// <summary>
        /// 工位1-左-需求信号
        /// </summary>
        public bool IsNeed_1
        {
            get => DevStatus?.Need1 ?? false;
        }

        /// <summary>
        /// 工位2-右-需求信号
        /// </summary>
        public bool IsNeed_2
        {
            get => DevStatus?.Need2 ?? false;
        }

        /// <summary>
        /// 工位1-左-是否有货
        /// </summary>
        public bool IsLoad_1
        {
            get => DevStatus?.Load1 ?? false;
        }

        /// <summary>
        /// 工位1-左-是否无货
        /// </summary>
        public bool IsEmpty_1
        {
            get => !DevStatus?.Load1 ?? false;
        }

        /// <summary>
        /// 工位1-左-载货状态
        /// </summary>
        public DevLifterLoadE LoadStatus1
        {
            get => DevStatus?.LoadStatus1 ?? DevLifterLoadE.无砖;
        }

        /// <summary>
        /// 工位2-右-是否有货
        /// </summary>
        public bool IsLoad_2
        {
            get => DevStatus?.Load2 ?? false;
        }
        /// <summary>
        /// 工位2-右-是否无货
        /// </summary>
        public bool IsEmpty_2
        {
            get => !DevStatus?.Load2 ?? false;
        }

        /// <summary>
        /// 工位2-右-载货状态
        /// </summary>
        public DevLifterLoadE LoadStatus2
        {
            get => DevStatus?.LoadStatus2 ?? DevLifterLoadE.无砖;
        }

        /// <summary>
        /// 工位1-左-是否介入
        /// </summary>
        public bool IsInvo_1
        {
            get => DevStatus?.Involve1 ?? false;
        }

        /// <summary>
        /// 工位2-右-是否介入
        /// </summary>
        public bool IsInvo_2
        {
            get => DevStatus?.Involve2 ?? false;
        }

        /// <summary>
        /// 工位满砖层数
        /// </summary>
        public byte FullQty
        {
            get => DevStatus?.FullQty ?? 66;
        }

        /// <summary>
        /// 工位1-左-当前层数
        /// </summary>
        public byte Site1Qty
        {
            get => DevStatus?.Site1Qty ?? FullQty;
        }

        /// <summary>
        /// 工位2-右-当前层数
        /// </summary>
        public byte Site2Qty
        {
            get => DevStatus?.Site2Qty ?? FullQty;
        }

        /// <summary>
        /// 转产状态
        /// </summary>
        public TileShiftStatusE TileShiftStatus
        {
            get => DevStatus?.ShiftStatus ?? TileShiftStatusE.完成;
        }

        /// <summary>
        /// 工位1-左-是否忽略
        /// </summary>
        public bool Ignore_1 = false;

        /// <summary>
        /// 工位2-右-是否忽略
        /// </summary>
        public bool Ignore_2 = false;

        /// <summary>
        /// 作业依据
        /// </summary>
        public DevWorkTypeE WorkType
        {
            get => DevConfig?.WorkType ?? DevWorkTypeE.品种作业;
            set => DevConfig.WorkType = value;
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnect
        {
            get => DevTcp?.IsConnected ?? false;
        }

        #endregion

        #region[报警灯]

        /// <summary>
        /// 是否有报警灯信息
        /// </summary>
        public DevLight Config_Light { set; get; }

        /// <summary>
        /// 灯亮
        /// </summary>
        public bool LightOn { get => DevStatus.AlertLightStatus == 1; }

        /// <summary>
        /// 灯灭
        /// </summary>
        public bool LightOff { get => DevStatus.AlertLightStatus == 0; }

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
                DoQuery(); // 开始连接查询一次
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
            DevTcp?.SendCmd(DevLifterCmdTypeE.转产, (byte)ts, count, goods);
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

        internal void DoLight(TileLightShiftE light)
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.开关灯, (byte)light, 0, 0);
        }

        internal void SetInTaskStatus(bool status)
        {
            DevTcp.IsInTaskStatus = status;
        }

        internal bool IsSiteGoodSame()
        {
            return DevStatus.Goods1 == DevStatus.Goods2;
        }

        /// <summary>
        /// 接收-回复
        /// </summary>
        internal void DoReply()
        {
            DevTcp?.SendCmd(DevLifterCmdTypeE.接收回复, 0, 0, 0, DevStatus.MarkCode);
        }

        #endregion
    }
}
