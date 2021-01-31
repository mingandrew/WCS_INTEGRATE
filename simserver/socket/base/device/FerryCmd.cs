using enums;
using module.device;
using System;

namespace simserver.simsocket
{

    public class FerryCmd : IDevice
{
    public byte DeviceID;      //设备号
    public DevFerryCmdE Commond; //控制码
    public byte Value1;  //值1
    public byte Value2;  //值2
    public int Value3;//值3

    public ushort DesCode
    {
        get => BitConverter.ToUInt16(new byte[] { Value2, Value1 }, 0);
    }
}
}
