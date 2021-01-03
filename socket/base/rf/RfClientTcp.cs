using System;
using System.Net;
using System.Net.Sockets;

namespace socket.rf
{
    public class RfClientTcp
    {
        public string IP_PORT { get; private set; }
        public string MEID { get; private set; }

        public bool IsUpdateMEID { get; internal set; }

        public RfClientTcp(TcpClient client, byte[] buffer)
        {
            TcpClient = client ?? throw new ArgumentNullException("client");
            Buffer = buffer ?? throw new ArgumentNullException("buffer");
            EndPoint romote = client.Client.RemoteEndPoint;
            IPEndPoint ip = (IPEndPoint)romote;
            IP_PORT = ip.Address.ToString() + ":" + ip.Port;
            MEID = IP_PORT;
        }

        public TcpClient TcpClient { get; private set; }

        public bool IsConnect
        {
            get => TcpClient?.Connected ?? false;
        }

        public byte[] Buffer { get; private set; }
        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }

        public void Close()
        {
            TcpClient?.Close();
        }

        public void AddLeft(byte[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                Buffer[i] = d[i];
            }
        }

        internal void SetMEID(string meid)
        {
            if (MEID.Equals(meid))
            {
                IsUpdateMEID = true;
            }
            MEID = meid;
        }
    }
}
