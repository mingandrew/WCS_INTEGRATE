namespace enums
{
    public enum DevWorkTypeE
    {
        品种作业,
        轨道作业,
        混砖作业
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceTypeE
    {
        上砖机,
        下砖机,
        前摆渡,
        后摆渡,
        运输车,
        其他,
        砖机,
    }

    public enum DeviceType2E
    {
        无 = 0,
        单轨 = 1,//上下砖机轨道数
        双轨 = 2,//上下砖机轨道数
    }



    /// <summary>
    /// 操作模式
    /// </summary>
    public enum DevOperateModeE
    {
        无,
        自动 = 1,
        手动 = 2
    }

    /// <summary>
    /// 入库策略
    /// </summary>
    public enum StrategyInE
    {
        无,
        优先下砖,//找空位放 派多个任务
        同规同轨,//不同砖机间，下同品种 放同轨道
        同机同轨,//同砖机同时指派一个任务
        同轨同轨,//砖机轨道相同的砖机 同时派一个任务
    }

    /// <summary>
    /// 出库策略
    /// </summary>
    public enum StrategyOutE
    {
        无,
        优先上砖,//找库存位取 派多个任务
        同规同轨,//默认去同样品种则优先取同一轨道
        同机同轨,//同砖机 派发一个取货任务
        同轨同轨,//砖机轨道相同的砖机 同时派一个任务
    }

    /// <summary>
    /// 交管类型
    /// </summary>
    public enum TrafficControlTypeE
    {
        运输车交管运输车,
        摆渡车交管摆渡车,
        运输车交管摆渡车,
        摆渡车交管运输车,
    }

    /// <summary>
    /// 交管状态
    /// </summary>
    public enum TrafficControlStatusE
    {
        初始化,
        交管中,
        已完成,
        取消
    }

    public enum TrafficControlUpdateE
    {
        Status,
        from,
        to
    }

    /// <summary>
    /// 设备移动方向
    /// </summary>
    public enum DevMoveDirectionE
    {
        无,
        前,
        后
    }

    public enum LevelTypeE
    {
        TileLevel,  //等级
        TileSite,   //窑位
    }

}
