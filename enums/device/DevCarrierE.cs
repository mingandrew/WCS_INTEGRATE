namespace enums
{
    public enum DevCarrierStatusE
    {
        停止 = 0,
        前进 = 1,
        后退 = 2,
        异常
    }

    /// <summary>
    /// 运输车指令
    /// </summary>
    public enum DevCarrierOrderE
    {
        无 = 0,
        定位指令 = 1,
        取砖指令 = 2,
        放砖指令 = 3,
        往前倒库 = 4,
        往后倒库 = 5,
        初始化 = 0x7E,
        终止指令 = 0x7F,
        置位,
        异常
    }

    /// <summary>
    /// 手动操作（固定指令）
    /// </summary>
    public enum DevCarrierTaskE
    {
        置位 = 1,
        终止,
        倒库,
        上升取砖,
        下降放砖,

        前进取砖 = 10,      // 前进- 1x
        前进放砖,
        前进至点,
        前进至轨道头,
        前进至摆渡车,

        后退取砖 = 20,     // 后退- 2x
        后退放砖,
        后退至点,
        后退至轨道头,
        后退至摆渡车,
    }

    /// <summary>
    /// 手动操作（灵活指令）
    /// </summary>
    public enum DevCarrierTask2E
    {
        脉冲定位,
        初始化
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
        异常,
        无货 = 1,
        有货 = 2
    }

    /// <summary>
    /// 运输车位置
    /// </summary>
    public enum DevCarrierPositionE
    {
        异常,
        在轨道上 = 1,
        在摆渡上 = 2,
        上下摆渡中 = 3
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
        执行指令 = 0x01,
        复位操作 = 0x02,
        置位指令 = 0x7F, 

        接收回复 = 0x88
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
        TakeMissTrack,
        GiveMissTrack,
    }

}
