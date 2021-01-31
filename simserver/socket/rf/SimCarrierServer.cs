using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using module.msg;
using simserver.simsocket.process;
using System;
using System.Linq;
using System.Threading;

namespace simserver.simsocket.rf
{
    public class SimCarrierServer : SimServerBase
    {

        #region[定义]
        SocketMsgMod mMsg;
        SimCarrierProcesser mProcess;
        #endregion

        #region[构造函数/发送数据]

        public SimCarrierServer(int port) : base(port)
        {
            mMsg = new SocketMsgMod();
            mProcess = new SimCarrierProcesser();
            Start();
        }

        /// <summary>
        /// 主动端口客户端连接
        /// </summary>
        /// <param name="devid"></param>
        public void DisConnectClient(byte devid)
        {
            DoRemoveClient(FindClient(devid));
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

        private SimRfClientTcp FindClient(byte devid)
        {
            return clients.Find(c => c.DevId == devid);
        }

        private void Send(byte devid, byte[] data)
        {
            SimRfClientTcp client = FindClient(devid);
            if (client != null)
            {
                Send(client, data);
            }
        }

        /// <summary>
        /// 发送报文至指定的客户端
        /// </summary>
        /// <param name="tcpClient">客户端</param>
        /// <param name="datagram">报文</param>
        private void Send(SimRfClientTcp tcpClient, byte[] datagram)
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
                ((SimRfClientTcp)ar.AsyncState).TcpClient.GetStream().EndWrite(ar);
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
        public void SendMessage(byte devid, DevCarrier dev)
        {
            byte[] data = mProcess.GetStatus(dev);
            Send(devid, data);
        }

        #endregion

        #region[通知]

        internal override void SendMsg(byte devid, SocketMsgTypeE type, SocketConnectStatusE connectE, IDevice dev)
        {
            if (devid == 0) return ;

            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    mMsg.Devid = devid;
                    mMsg.MsgType = type;
                    mMsg.ConnStatus = connectE;
                    mMsg.Device = dev;
                    Messenger.Default.Send(mMsg, MsgToken.SimCarrierMsgUpdate);
                }
                finally
                {
                    Monitor.Exit(_obj);
                }
            }
        }

        internal override void HandleClientData(ref byte[] data, SimRfClientTcp client)
        {

            uint head = BitConverter.ToUInt16(ShiftBytes(data, 0, 2), 0);
            if (head != SimSocketConst.CARRIER_CMD_HEAD_KEY)
            {
                //信息不准确，重新连接
                DoRemoveClient(client);
                return;
            }

            if (data.Length >= SimSocketConst.CARRIER_CMD_SIZE)
            {
                ushort tail = BitConverter.ToUInt16(ShiftBytes(data, SimSocketConst.CARRIER_CMD_SIZE - 2, 2), 0);

                if (tail == SimSocketConst.TAIL_KEY)
                {
                    byte[] pdata = new byte[SimSocketConst.CARRIER_CMD_SIZE];
                    Array.Copy(data, 0, pdata, 0, SimSocketConst.CARRIER_CMD_SIZE);
                    CarrierCmd cmd = mProcess.GetCmd(pdata);
                    client.DevId = cmd.DeviceID;
                    SendMsg(cmd.DeviceID, SocketMsgTypeE.DataReiceive, SocketConnectStatusE.通信正常, cmd);
                    // remove from data array
                    data = data.Skip(SimSocketConst.CARRIER_CMD_SIZE).ToArray();
                }
                else
                {
                    //信息不准确，重新连接
                    DoRemoveClient(client);
                    return;
                }
            }
        }

        #endregion

        #region[通用消息反馈]


        #endregion
    }
}
