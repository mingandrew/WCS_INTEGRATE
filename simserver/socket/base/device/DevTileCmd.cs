using enums;
using module.device;

namespace simserver.simsocket
{
    public class DevTileCmd : IDevice
    {
        public byte DeviceID;      //设备号
        public DevLifterCmdTypeE Command;        //控制码
        public byte Value1;        //故障位2
        public byte Value2;        //值2
        public uint Value3;          //值3

        public DevLifterInvolE InVolType
        {
            get => (DevLifterInvolE)Value1;
        }
    }
}
