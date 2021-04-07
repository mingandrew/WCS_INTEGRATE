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
        前进倒库 = 4,
        后退倒库 = 5,
        终止指令 = 0x7F,
        异常
    }

    public enum DevCarrierTaskE
    {
        无 = 0,
        后退取砖 = 1,
        前进放砖 = 2,
        后退至摆渡车 = 3,
        前进至摆渡车 = 4,
        倒库 = 5,
        前进至点 = 6,
        后退至点 = 7,
        顶升取货 = 8,
        下降放货 = 9,
        //后退至外放砖 = 10,
        //后退至内放砖 = 11,
        前进取砖 = 12,
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
        设复位点 = 0x02,
        终止指令 = 0x7F
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
