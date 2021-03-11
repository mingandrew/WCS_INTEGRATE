using enums;
using module.device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using tool.mlog;

namespace simserver.simsocket.rf
{
    public abstract class SimServerBase : IDisposable
    {

        #region [参数]
        protected const int P_HEAD = 0xAABB;
        protected const int P_TAIL = 0xCCDD;

        internal const int MESSAGE_RESEND_TIMEOUT = 5 * 1000;

        internal ManualResetEvent m_StopSignal;

        protected TcpListener listener;
        internal List<SimRfClientTcp> clients;
        private bool disposed = false;

        public bool IsRunning { get; protected set; }
        private int Port { get; set; }

        internal Log _mLog;
        internal object _obj;
        #endregion

        #region[抽象参数]

        public abstract void Stop();

        internal abstract void SendMsg(byte devid, SocketMsgTypeE type, SocketConnectStatusE connectE, IDevice dev);

        internal abstract void HandleClientData(ref byte[] data, SimRfClientTcp client);
        #endregion

        #region[释放资源]

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release 
        /// both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                    Stop();

                    listener = null;
                }

                disposed = true;
            }
        }

        #endregion
        
        #region[构造函数/启动]

        /// <summary>
        /// 异步TCP服务器
        /// </summary>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口</param>
        protected SimServerBase(int port)
        {
            _obj = new object();
            _mLog = (Log)new LogFactory().GetLog("模拟服务", true);

            Port = port;

            m_StopSignal = new ManualResetEvent(false);

            clients = new List<SimRfClientTcp>();

            listener = new TcpListener(IPAddress.Any, Port);
            listener.AllowNatTraversal(true);

        }


        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns>异步TCP服务器</returns>
        public void Start()
        {
            try
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    listener.Start();
                    listener.BeginAcceptTcpClient(
                      new AsyncCallback(HandleTcpClientAccepted), listener);
                }
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.Message, e);
            }
        }

        #endregion

        #region[客户处理/添加/断开]

        private void DoAddClient(SimRfClientTcp client)
        {
            lock (clients)
            {
                clients.Add(client);
                SendMsg(client.DevId,SocketMsgTypeE.Connection, SocketConnectStatusE.连接成功, null);
                _mLog.Status(true, "客户端连接：" + client.Id);
            }
        }

        /// <summary>
        /// 移除断开的客户端
        /// </summary>
        /// <param name="client"></param>
        internal void DoRemoveClient(SimRfClientTcp client)
        {
            if (client == null) return;
            lock (clients)
            {
                client.Close();
                clients.Remove(client);
                SendMsg(client.DevId, SocketMsgTypeE.Connection, SocketConnectStatusE.连接断开, null);
                _mLog.Status(true, "客户端断开：" + client.Id);
            }
        }

        #endregion

        #region[连接/接收信息]

        /// <summary>
        /// 连接客户端
        /// </summary>
        /// <param name="ar"></param>
        private void HandleTcpClientAccepted(IAsyncResult ar)
        {
            try
            {
                if (IsRunning)
                {
                    TcpListener tcpListener = (TcpListener)ar.AsyncState;

                    TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
                    byte[] buffer = new byte[4096];

                    SimRfClientTcp client = new SimRfClientTcp(tcpClient, buffer);

                    DoAddClient(client);

                    NetworkStream networkStream = client.NetworkStream;
                    networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, HandleDatagramReceived, client);

                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), ar.AsyncState);
                }
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.Message, e);
            }

        }

        /// <summary>
        /// 处理接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void HandleDatagramReceived(IAsyncResult ar)
        {
            try
            {
                if (IsRunning)
                {
                    SimRfClientTcp client = (SimRfClientTcp)ar.AsyncState;
                    NetworkStream networkStream = client.NetworkStream;

                    int numberOfReadBytes = 0;
                    try
                    {
                        numberOfReadBytes = networkStream.EndRead(ar);
                    }
                    catch
                    {
                        numberOfReadBytes = 0;
                    }

                    if (numberOfReadBytes == 0)
                    {
                        // connection has been closed
                        DoRemoveClient(client);
                        return;
                    }

                    // received byte and trigger event notification
                    byte[] readData = new byte[numberOfReadBytes];
                    Buffer.BlockCopy(client.Buffer, 0, readData, 0, numberOfReadBytes);

                    HandleClientData(ref readData, client);

                    try
                    {
                        if (readData.Any())
                        {
                            client.AddLeft(readData);
                            networkStream.BeginRead(client.Buffer, readData.Length, client.Buffer.Length - readData.Length, HandleDatagramReceived, client);
                        }
                        else
                        {
                            networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, HandleDatagramReceived, client);
                        }
                    }
                    catch (Exception e)
                    {
                        _mLog.Error(true, e.Message, e);
                        //信息不准确，重新连接
                        DoRemoveClient(client);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.Message, e);
            }
        }

        internal byte[] ShiftBytes(byte[] buffer, int offset, int size)
        {
            return buffer.Skip(offset).Take(size).Reverse().ToArray();
        }

        #endregion
    }
}
