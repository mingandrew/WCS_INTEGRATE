namespace enums
{
    public enum SimCarrierSortStepE
    {
        获取取货库存位置,
        前往取货库存位置,
        取货中,
        取货完成获取卸货位置,
        前往卸货位置,
        卸货中,
        卸货完成,
    }

    public enum SimCarrierTakeStepE
    {
        砖机工位取货,
        前往砖机工位,
        工位取货中,
        工位取货完成,
        获取储砖轨道取货脉冲,
        前往取货脉冲点,
        脉冲取货中,
        脉冲卸货完成,
        前往结束地标
    }
}
