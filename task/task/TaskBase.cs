using enums;
using module.device;
using resource;
using System;
using tool.timer;

namespace task.task
{
    public abstract class TaskBase
    {
        #region[字段]
        public uint AreaId
        {
            get => Device.area;
        }

        public DeviceTypeE Type
        {
            get => Device?.Type ?? DeviceTypeE.其他;
        }

        public uint ID
        {
            get => Device?.id ?? 0;
        }

        public bool IsEnable
        {
            get => Device?.enable ?? false;
        }

        public bool IsWorking
        {
            get => Device?.do_work ?? false;
            set => Device.do_work = value;
        }

        public Device Device { set; get; }
        private SocketConnectStatusE mconn;
        internal bool MConChange;
        public MTimer mTimer;

        public SocketConnectStatusE ConnStatus
        {
            get => mconn;
            set
            {
                MConChange = value != mconn;
                mconn = value;
            }
        
        }
        /// <summary>
        /// 设备通讯刷新时间
        /// </summary>
        private DateTime SockRefreshTime { set; get;  }
        private bool HaveRefreshToSurface { set; get; }
        #endregion

        public TaskBase()
        {
            mTimer = new MTimer();
            OfflineTime = DateTime.Now;
        }


        internal void SetEnable(bool isenable)
        {
            Device.enable = isenable;
            PubMaster.Device.SetEnable(Device.id, isenable);
        }


        #region[离线自断开重连]

        /// <summary>
        /// 正在停止休息时间中
        /// </summary>
        private bool IsOfflineInBreak;
        /// <summary>
        /// 离线停止休息开始时间
        /// </summary>
        private DateTime? OfflineBreakTime;
        /// <summary>
        /// 离线时间
        /// </summary>
        private DateTime? OfflineTime;
        private DateTime ConnRefreshTime = DateTime.Now;

        internal bool IsDevOfflineInBreak
        {
            get => IsOfflineInBreak;
            set => IsOfflineInBreak = value;
        }

        internal void ReSetRefreshTime()
        {
            ConnRefreshTime = DateTime.Now;
        }

        internal void ReSetOffLineTime()
        {
            OfflineTime = DateTime.Now;
        }

        internal void ReSetOfflineBreakTime()
        {
            OfflineTime = null;
            OfflineBreakTime = null;
            IsOfflineInBreak = false;
        }

        /// <summary>
        /// 设置开始休息
        /// </summary>
        /// <param name="value">True:开始离线，False:离线结束</param>
        internal void SetDevConnOnBreak(bool value)
        {
            IsOfflineInBreak = value;
            if (value)
            {
                OfflineBreakTime = DateTime.Now;
            }
            else
            {
                OfflineBreakTime = null;
                OfflineTime = DateTime.Now;
            }
        }

        internal bool IsInBreakOver()
        {
            if (IsOfflineInBreak
                && OfflineBreakTime is DateTime time
                && DateTime.Now.Subtract(time).TotalSeconds > 3)
            {
                IsOfflineInBreak = false;
                return true;
            }
            return false;
        }

        internal bool IsOfflineTimeOver()
        {
            if (OfflineTime is DateTime time
                && DateTime.Now.Subtract(time).TotalSeconds > 10
                && IsRefreshTimeOver())
            {
                OfflineTime = DateTime.Now;
                return true;
            }
            return false;
        }

        internal bool IsRefreshTimeOver()
        {
            return DateTime.Now.Subtract(ConnRefreshTime).TotalSeconds > 5;
        }
        #endregion
    }
}
