using System;
using System.Net;
using System.Net.Sockets;
using tool.mlog;

namespace socket.rf
{
    public class RfClientTcp
    {
        public string IP_PORT { get; private set; }
        public string MEID { get; private set; }

        public bool IsUpdateMEID { get; internal set; }

        private Log mlog;

        public RfClientTcp(TcpClient client, byte[] buffer)
        {
            TcpClient = client ?? throw new ArgumentNullException("client");
            Buffer = buffer ?? throw new ArgumentNullException("buffer");
            EndPoint romote = client.Client.RemoteEndPoint;
            IPEndPoint ip = (IPEndPoint)romote;
            IP_PORT = ip.Address.ToString() + ":" + ip.Port;
            MEID = IP_PORT;

            mlog = (Log)new LogFactory().GetLog(MEID.Replace(".","").Replace(":",""));
        }

        public TcpClient TcpClient { get; private set; }

        public bool IsConnect
        {
            get => TcpClient?.Connected ?? false;
        }

        public byte[] Buffer { get; private set; }
        public int Last_Count { set; get; }
        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }

        public void Close()
        {
            this.Buffer = new byte[4096];
            TcpClient?.Close();
        }

        public void AddLeft(byte[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                Buffer[i] = d[i];
            }

            Last_Count = d.Length;
        }

        internal void SetMEID(string meid)
        {
            if (MEID.Equals(meid))
            {
                IsUpdateMEID = true;
            }
            MEID = meid;

            if (!MEID.Equals(mlog.m_Name))
            {
                mlog = (Log)new LogFactory().GetLog(MEID);
            }
        }

        public void Log2File(string msg, byte[] data)
        {
            mlog.Cmd(true, msg, data);
        }
    }
}
