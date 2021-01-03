using System;
using System.Linq;
using System.Text;
using tool.json;
using module.rf;
using enums;
using GalaSoft.MvvmLight.Messaging;
using module.msg;
using System.Collections.Generic;

namespace socket.rf
{
    public class RfServer : RfServerBase
    {
        #region[定义]
        RfMsgMod mMsg;
        #endregion

        #region[构造函数/发送数据]

        public RfServer(int port) : base(port)
        {
            mMsg = new RfMsgMod();
            Start();
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        /// <returns>异步TCP服务器</returns>
        public override void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                listener.Stop();

                lock (clients)
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        clients[i].TcpClient.Client.Disconnect(false);
                    }
                    clients.Clear();
                }

            }
        }

        /// <summary>
        /// 发送给所有客户端
        /// </summary>
        /// <param name="datagram"></param>
        private void SendToAll(byte[] datagram)
        {
            if (!IsRunning)
                throw new InvalidProgramException("This TCP server has not been started.");

            for (int i = 0; i < clients.Count; i++)
            {
                Send(clients[i], datagram);
            }
        }

        private List<RfClientTcp> FindSameClient(string clientid)
        {
            return clients.FindAll(c => clientid.Contains(c.MEID));
        }

        private bool SendSameClient(string clientid, byte[] data)
        {
            List<RfClientTcp> rfClients = FindSameClient(clientid);
            if (rfClients.Count > 0)
            {
                foreach (RfClientTcp item in rfClients)
                {
                    Send(item, data);
                }
                return true;
            }
            return false;
        }

        public bool HaveClientOnline()
        {
            return clients.Exists(c => c.IsConnect);
        }


        /// <summary>
        /// 发送报文至指定的客户端
        /// </summary>
        /// <param name="tcpClient">客户端</param>
        /// <param name="datagram">报文</param>
        private void Send(RfClientTcp tcpClient, byte[] datagram)
        {
            try
            {
                if (!IsRunning)
                    throw new InvalidProgramException("This TCP server has not been started.");

                if (tcpClient == null)
                    throw new ArgumentNullException("tcpClient");

                if (tcpClient.TcpClient == null)
                    throw new ArgumentNullException("tcpClient.TcpClient");

                if (datagram == null)
                    throw new ArgumentNullException("datagram");
                if (tcpClient.TcpClient.Connected)
                {
                    tcpClient.TcpClient.GetStream().BeginWrite(
                      datagram, 0, datagram.Length, HandleDatagramWritten, tcpClient);
                }
                else
                {
                    //信息不准确，重新连接
                    DoRemoveClient(tcpClient);
                }
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.Message, e);
                //信息不准确，重新连接
                DoRemoveClient(tcpClient);
            }
        }

        private void HandleDatagramWritten(IAsyncResult ar)
        {
            try
            {
                ((RfClientTcp)ar.AsyncState).TcpClient.GetStream().EndWrite(ar);
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.Message, e);
            }
        }

        #endregion

        #region[接收数据/发送数据]


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        public bool SendMessage(string clientid, RfPackage pack)
        {
            if (pack == null)
            {
                string logMessage = "Cannot send empty message";
                throw new ArgumentNullException(logMessage);
            }
            string dstr = JsonTool.Serialize(pack);
            byte[] data = Encoding.UTF8.GetBytes(dstr);
            int msgsize = data.Length + 8;
            byte[] msg = ShiftBytes(BitConverter.GetBytes(P_HEAD), 0, 2)
                               .Concat(ShiftBytes(BitConverter.GetBytes(msgsize), 0, 4))
                               .Concat(data)                                            // message data
                               .Concat(ShiftBytes(BitConverter.GetBytes(P_TAIL), 0, 2))   // tailKey
                               .ToArray();

            //_mLog.Status(true, string.Format("[Send]-Client:{0}\n" + "Msg:{1}", clientid, dstr)); 
            return SendSameClient(clientid, msg);
        }

        #endregion

        #region[通知]

        internal override void NoticeDataReceive(byte[] data, RfClientTcp client)
        {
            try
            {
                string dstr = Encoding.UTF8.GetString(data);

                RfPackage package = JsonTool.Deserialize<RfPackage>(dstr);
                if (!client.IsUpdateMEID) client.SetMEID(package.Meid);
                package.ClientId = client.MEID;

                SendMsg(RfConnectE.客户端接收信息, client.IP_PORT, client.MEID, package);
                _mLog.Status(true, string.Format("[Send]-Client:{0}\n" + "Msg:{1}", client.MEID, dstr));
            }
            catch (Exception e)
            {
                _mLog.Error(true, e.StackTrace);
            }
        }

        internal override void SendMsg(RfConnectE connectE, string ip, string meid, RfPackage pack)
        {
            mMsg.IP = ip;
            mMsg.MEID = meid;
            mMsg.Conn = connectE;
            mMsg.Pack = pack;
            Messenger.Default.Send(mMsg, MsgToken.RfMsgUpdate);
        }

        #endregion

        #region[通用消息反馈]


        #endregion
    }
}
