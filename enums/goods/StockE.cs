namespace enums
{
    public enum TransUpdateE
    {
        Status,
        CarrierId,
        TakeFerryId,
        GiveFerryId,
        LoadTime,
        UnLoadTime,
        TakeSite,
        GiveSite,
        Stock,
        Finish,
        Cancel,
        ReTake
    }

    public enum TransTypeE
    {
        入库,
        出库,
        倒库,
        移车,
        手动入库,
        手动出库,
        其他
    }

    public enum TransStatusE
    {
        调度设备,
        取砖流程,
        放砖流程,
        还车回轨,
        倒库中,
        移车中,
        小车回轨,
        完成,
        取消,
        检查轨道,
        其他
    }

    /// <summary>
    /// 移车类型
    /// </summary>
    public enum MoveTypeE
    {
        转移占用轨道,//轨道需要作业，转移小车到空闲轨道【无作业】
        释放摆渡车, // 小车没任务，回到轨道释放占用的小车
        离开砖机轨道, //下砖轨道/上砖轨道小车 没任务 回到储砖轨道
        切换区域,//去上砖区  去下砖区
    }

    public enum StockPosE
    {
        顶部,
        其他
    }

    public enum StockUpE
    {
        Goods,
        Track,
        Pos,
        PosType,
        ProduceTime
    }
}
