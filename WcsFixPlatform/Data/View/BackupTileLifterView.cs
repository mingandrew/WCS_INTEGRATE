using enums;
using GalaSoft.MvvmLight;
using module.device;
using resource;
using System;

namespace wcs.Data.View
{
    public class BackupTileLifterView : ViewModelBase
    {
        public string Name { set; get; }
        private string goodidnfo;
        private string trackid;
        private DeviceTypeE devicetype;
        private string tracklist;



        #region[逻辑字段]
        private SocketConnectStatusE connstatus;
        private bool isconnect;

        public bool IsConnect
        {
            get => isconnect;
            set => Set(ref isconnect, value);
        }
        public SocketConnectStatusE ConnStatus
        {
            get => connstatus;
            set => Set(ref connstatus, value);
        }

        public string Goodidnfo
        {
            get => goodidnfo;
            set => Set(ref goodidnfo, value);
        }

        public string LastTrackName
        {
            get => trackid;
            set => Set(ref trackid, value);
        }
        public DeviceTypeE Type
        {
            get => devicetype;
            set => Set(ref devicetype, value);
        }

        public string TrackList
        {
            get => tracklist;
            set => Set(ref tracklist, value);
        }
        #endregion

        #region[字段]
        private uint deviceid;      //设备号
        #endregion

        #region[属性]
        public uint DeviceID//设备号
        {
            set => Set(ref deviceid, value);
            get => deviceid;
        }
        
        #endregion

        #region[更新]

        internal void Update(DevTileLifter st, SocketConnectStatusE conn, string  gid, string tid, DeviceTypeE type, string tlist)
        {
            DeviceID = st.DeviceID;
            Name = PubMaster.Device.GetDeviceName(st.DeviceID);
            Type = type;
            goodidnfo = gid;
            LastTrackName = tid;
            TrackList = tlist;
            ConnStatus = conn;
            IsConnect = ConnStatus == SocketConnectStatusE.通信正常;
        }
        #endregion
    }
}
