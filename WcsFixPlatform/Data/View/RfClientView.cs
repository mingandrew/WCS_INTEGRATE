using enums;
using GalaSoft.MvvmLight;
using module.rf;
using System;

namespace wcs.Data.View
{
    public class RfClientView : ViewModelBase
    {
        private string name ;
        private string rfid ;
        private string ip ;
        private DateTime? conn_time ;
        private DateTime? disconn_time ;
        private bool isconnect;
        private RfConnectE status ;

        public string Name
        {
            get => name;
            set => name = value;
        }
        public string Rfid
        {
            get => rfid;
            set => Set(ref rfid, value);
        }
        public string Ip
        {
            get => ip;
            set => Set(ref ip, value);
        }
        public DateTime? Conn_time
        {
            get => conn_time;
            set => Set(ref conn_time, value);
        }
        public DateTime? Disconn_time
        {
            get => disconn_time;
            set => Set(ref disconn_time, value);
        }

        public RfConnectE Status
        {
            get => status;
            set => Set(ref status, value);
        }
        public bool IsConnect
        {
            get => isconnect;
            set => Set(ref isconnect, value);
        }

        public RfClientView(RfClient rf)
        {
            Name = rf.name;
            Update(rf);
        }

        public void Update(RfClient rf)
        {
            Ip = rf.ip;
            Rfid = rf.rfid;
            Disconn_time = rf.disconn_time;
            Conn_time = rf.conn_time;
            Status = rf.Status;
            IsConnect = rf.Status != RfConnectE.客户端断开;
        }
    }
}
