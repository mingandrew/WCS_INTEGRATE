namespace enums
{
    public enum DevFerryStatusE
    {
        停止 = 0,
        前进 = 1,
        后退 = 2,
        设备故障 = 0xFE
    }

    public enum DevFerryLoadE
    {
        空,
        载车,
        非空,
        异常
    }

    public enum DevFerryTaskE
    {
        无 = 0,
        定位 = 1,
        回原点 = 5,
        自动对位 = 7,
        终止 = 0x7F
    }


    public enum DevFerryCmdE
    {
        查询 = 0,
        定位 = 1,
        速度操作 = 2,
        查询轨道坐标 = 3,
        设置轨道坐标 = 4,
        原点复位 = 5,
        自动对位 = 7,
        终止任务 = 0x7F,

        接收回复 = 0x88
    }

    public enum DevFerrySpeedCmdE
    {
        查询,
        手动快速设置,
        手动慢速设置,
        自动快速设置,
        自动慢速设置,
        重载提前停止设置,
        空载提前停止设置
    }

    public enum DevFerryResetPosE
    {
        前进回原点 = 1,
        后退回原点 = 2
    }

    public enum DevFerryAutoPosE
    {
        前侧对位 = 1,
        后侧对位 = 2
    }
}
