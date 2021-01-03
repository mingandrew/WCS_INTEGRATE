namespace enums
{
    public enum DevCarrierStatusE
    {
        停止 = 0,
        前进 = 1,
        后退 = 2,
        设备故障 = 0xFE
    }

    public enum DevCarrierTaskE
    {
        无 = 0,
        后退取砖 = 1,
        前进放砖 = 2,
        后退至摆渡车 = 3,
        前进至摆渡车 = 4,
        后退至轨道倒库 = 5,
        前进至点 = 6,
        后退至点 = 7,
        顶升取货 = 8,
        下降放货 = 9,
        终止 = 0x7F,
        其他
    }

    /// <summary>
    /// 超限模式
    /// </summary>
    public enum DevCarrierSizeE
    {
        未知,
        非超限 = 1,
        超限 = 2
    }

    /// <summary>
    /// 载货状态
    /// </summary>
    public enum DevCarrierLoadE
    {
        未知,
        无货 = 1,
        有货 = 2
    }

    /// <summary>
    /// 运行模式
    /// </summary>
    public enum DevCarrierWorkModeE
    {
        未知,
        调试 = 1,
        生产 = 2
    }

    
    /// <summary>
    /// 运输车取货放货后信号
    /// </summary>
    public enum DevCarrierSignalE
    {
        复位 = 0,
        空轨道 = 1,
        满轨道 = 2,
        非空非满 = 3,
        其他,
    }

    public enum DevCarrierCmdE
    {
        查询 = 0x00,
        执行任务 = 0x01,
        速度操作 = 0x02,
        模式调整 = 0x03,
        终止任务 = 0x7F
    }

    public enum DevCarrierResetE
    {
        无动作 = 0,
        复位 = 1
    }

    /// <summary>
    /// 小车类型
    /// </summary>
    public enum CarrierTypeE
    {
        窄车,
        宽车
    }

    public enum CarrierAlertE
    {
        PowerOff,
        GiveMissTrack,
        TakeMissTrack,

    }
    /// <summary>
    /// 小车职责
    /// </summary>
    public enum CarrierDutyE
    {
        未知,
        上砖,
        下砖
    }
}
