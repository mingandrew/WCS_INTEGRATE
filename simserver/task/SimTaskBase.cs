using enums;
using module.device;
using tool.timer;

namespace simtask.task
{
    public abstract class SimTaskBase
    {
        #region[字段]

        public DeviceTypeE Type
        {
            get => Device?.Type ?? DeviceTypeE.其他;
        }

        public uint AreaId
        {
            get => Device?.area ?? 0;
        }

        public uint ID
        {
            get => Device?.id ?? 0;
        }

        public bool IsEnable
        {
            get => Device?.enable ?? false;
        }

        public Device Device { set; get; }
        public SocketConnectStatusE ConnStatus { set; get; }

        internal MTimer mTimer;
        #endregion

        public SimTaskBase()
        {
            mTimer = new MTimer();
        }
    }
}
