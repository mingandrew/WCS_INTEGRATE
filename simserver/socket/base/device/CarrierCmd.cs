using enums;
using module.device;

namespace simserver.simsocket
{
    public class CarrierCmd : IDevice
    {
        public byte DeviceID;      //设备号
        public DevCarrierCmdE Command { set; get; }       //控制码
        public ushort Value1_2;        //值1-2
        public ushort Value3_4;        //值3-4
        public ushort Value5_6;        //值5-6
        public DevCarrierOrderE Value7;        //值7
        public ushort Value8_9;        //值8-9
        public ushort Value10_11;        //值10-11
        public byte Value12;        //值12

        public DevCarrierTaskE Task
        {
            get => (DevCarrierTaskE)Value12;
        }

        public DevCarrierSizeE OverSize
        {
            get => (DevCarrierSizeE)Value12;
        }
    }
}
