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

        #region[转产]
        public TileShiftCmdE ShiftType
        {
            get => (TileShiftCmdE)Value1;
        }
        public uint GoodId
        {
            get => Value3;
        }
        #endregion

        #region[转模式]
        public TileWorkModeE WorkMode
        {
            get => (TileWorkModeE)Value1;
        }

        public TileFullE SetFullType
        {
            get => (TileFullE)Value2;
        }
        #endregion

        #region[等级]

        public byte Level
        {
            get => Value1;
        }

        #endregion
    }
}
