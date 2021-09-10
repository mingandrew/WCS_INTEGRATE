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
        FinsihSite,
        Stock,
        Goods,
        Finish,
        Cancel,
        ReTake,
        TileId,
        Line,
        FerryType
    }

    public enum TransTypeE
    {
        下砖任务,
        上砖任务,
        倒库任务,
        移车任务,
        手动下砖,
        手动上砖,
        同向上砖,
        同向下砖,
        上砖接力,
        反抛任务,
        库存整理,
        库存转移,
        中转倒库,
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
        倒库暂停,
        接力等待, 
        整理中,
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
        首车,
        中部,
        尾车
    }

    public enum StockUpE
    {
        Goods,
        Track,
        Pos,
        PosType,
        ProduceTime,
        Location,
        PriorNum
    }


    public enum StockTransDtlTypeE
    {
        上砖品种,
        转移品种,
        保留品种,
        中转库存
    }

    public enum StockTransDtlStatusE
    {
        整理中,
        完成
    }


    public enum TransDtlUpdateE
    {
        Status,
        TakeTrack,
        GiveTrack,
        TransId,
        Qty,
        Finish
    }
}
