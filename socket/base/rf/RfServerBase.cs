using enums;
using module.rf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using tool.mlog;

namespace socket.rf
{
    public abstract class RfServerBase : IDisposable
    {

        #region [参数]
        protected const int P_HEAD = 0xAABB;
        protected const int P_TAIL = 0xCCDD;

        internal const int MESSAGE_RESEND_TIMEOUT = 5 * 1000;

        internal ManualResetEvent m_StopSignal;

        protected TcpListener listener;
        internal List<RfClientTcp> clients;
        private bool disposed = false;

        public bool IsRunning { get; protected set; }
        private int Port { get; set; }

        internal Log _mLog;
        #endregion

        #region[抽象参数]

        public abstract void Stop();

        internal abstract void NoticeDataReceive(byte[] data, RfClientTcp client);

        internal abstract void SendMsg(RfConnectE connectE, string ip, string meid, RfPackage pack);

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
        protected RfServerBase(int port)
        {
            _mLog = (Log)new LogFactory().GetLog("PDA服务", false);

            Port = port;

            m_StopSignal = new ManualResetEvent(false);

            clients = new List<RfClientTcp>();

            listener = new TcpListener(IPAddress.Any, Port);
            listener.AllowNatTraversal(true);
        }


        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns>异步TCP服务器</returns>
        public RfServerBase Start()
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
            return this;
        }

        #endregion

        #region[客户处理/添加/断开]

        private void DoAddClient(RfClientTcp client)
        {
            lock (clients)
            {
                clients.Add(client);
                SendMsg(RfConnectE.客户端连接, client.IP_PORT, client.MEID, null);
                _mLog.Status(true, "客户端连接：" + client.MEID);
            }
        }

        /// <summary>
        /// 移除断开的客户端
        /// </summary>
        /// <param name="client"></param>
        internal void DoRemoveClient(RfClientTcp client)
        {
            if (client == null) return;
            lock (clients)
            {
                client.Close();
                clients.Remove(client);
                SendMsg(RfConnectE.客户端断开, client.IP_PORT, client.MEID, null);
                _mLog.Status(true, "客户端断开：" + client.MEID);
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

                    RfClientTcp client = new RfClientTcp(tcpClient, buffer);

                    DoAddClient(client);

                    NetworkStream networkStream = client.NetworkStream;
                    networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, HandleDatagramReceived, client);

                    //tcpListener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), ar.AsyncState);
                }
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.Message, e);
            }
            finally
            {
                if(listener != null && IsRunning)
                {
                    listener.BeginAcceptTcpClient(
                      new AsyncCallback(HandleTcpClientAccepted), listener);
                }
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
                    RfClientTcp client = (RfClientTcp)ar.AsyncState;
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

                    uint headerKey = BitConverter.ToUInt16(ShiftBytes(readData, 0, 2), 0);
                    if (headerKey != P_HEAD) //43707
                    {
                        //信息不准确，重新连接
                        DoRemoveClient(client);
                        return;
                    }


                    if (readData.Length > 8)
                    {
                        uint messageSize = BitConverter.ToUInt32(ShiftBytes(readData, 2, 4), 0);
                        
                        if(readData.Count() *10 < messageSize)
                        { 
                            //信息不准确，重新连接
                            //DoRemoveClient(client);
                            //return;
                        }

                        client.Log2File(string.Format("数据长度：{0}, 总长度：{1}，", messageSize, readData.Count()), readData);

                        if (readData.Count() >= messageSize)
                        {
                            //2 + 4 + msgsize + 2
                            int tailKeyIndex = (int)messageSize - 2;
                            uint tailKey = BitConverter.ToUInt16(ShiftBytes(readData, tailKeyIndex, 2), 0);
                            if (tailKey == P_TAIL)//52445
                            {
                                byte[] data = new byte[messageSize - 8];
                                Array.Copy(readData, 6, data, 0, messageSize - 8);

                                NoticeDataReceive(data, client);

                                int skiplen = tailKeyIndex + 4;
                                // remove from data array
                                readData = readData.Skip(skiplen).ToArray();
                            }
                            else
                            {
                                //信息不准确，重新连接
                                DoRemoveClient(client);
                                return;
                            }
                        }
                    }
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
