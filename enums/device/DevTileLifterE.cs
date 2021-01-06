namespace enums
{
    public enum DevLifterNeedE
    {
        无,
        有
    }

    public enum DevLifterLoadE
    {
        无砖,
        有砖
    }

    public enum DevLifterInvolE
    {
        离开,
        介入
    }

    public enum DevLifterCmdTypeE
    {
        查询,
        介入1,
        介入2,
        转产,
        模式,
        等级
    }

    public enum TileShiftCmdE
    {
        复位,
        变更品种,
        执行转产
    }

    public enum TileShiftStatusE
    {
        复位,
        转产中,
        完成
    }

    public enum TileWorkModeE
    {
        过砖,
        上砖,
        下砖
    }

    public enum TileFullE
    {
        忽略,
        设为满砖
    }

}
