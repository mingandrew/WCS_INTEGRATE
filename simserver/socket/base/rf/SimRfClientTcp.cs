using System;
using System.Net;
using System.Net.Sockets;

namespace simserver.simsocket.rf
{
    public class SimRfClientTcp
    {
        public byte DevId { set; get; }
        public string Id { get; private set; }

        public SimRfClientTcp(TcpClient client, byte[] buffer)
        {
            TcpClient = client ?? throw new ArgumentNullException("client");
            Buffer = buffer ?? throw new ArgumentNullException("buffer");
            EndPoint romote = client.Client.RemoteEndPoint;
            IPEndPoint ip = (IPEndPoint)romote;
            Id = ip.Address.ToString() + ":" + ip.Port;

        }

        public TcpClient TcpClient { get; private set; }

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
    }
}
