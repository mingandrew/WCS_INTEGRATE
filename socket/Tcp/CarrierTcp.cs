using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using socket.process;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace socket.tcp
{
    public class CarrierTcp : TcpBase
    {
        #region[字段]

        private CarrierProcesser mProcess;

        #endregion
        public CarrierTcp(Device dev) : base(dev)
        {
            mProcess = new CarrierProcesser();
            mMinProtLength = SocketConst.CARRIER_STATUS_SIZE;
        }


        #region[发送信息]        
        public void SendCmd(DevCarrierCmdE type)
        {
            if (type == DevCarrierCmdE.查询 && DateTime.Now.Subtract(lastfunctime).TotalMilliseconds <= 1000)
            {
                return;
            }

            if (Monitor.TryEnter(_senobj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    byte[] data = mProcess.GetCmd(mDev.memo, type);
                    SendMessage(data);
                }
                finally
                {
                    Monitor.Exit(_senobj);
                }
            }
        }

        public void SendCmdNow(DevCarrierCmdE type)
        {
            try
            {
                byte[] data = mProcess.GetCmd(mDev.memo, type);
                SendMessage(data);
            }
            catch { }
        }

        public void SendCmd(DevCarrierCmdE type, DevCarrierOrderE order,
            ushort v1, ushort v2, ushort v3, ushort v4, ushort v5, byte v6)
        {
            lastfunctime = DateTime.Now;

            if (Monitor.TryEnter(_senobj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    byte[] data = mProcess.GetCmd(mDev.memo, type, order, v1, v2, v3, v4, v5, v6);
                    SendMessage(data);
                }
                finally
                {
                    Monitor.Exit(_senobj);
                }
            }
        }

        public void SendMessage(byte[] data)
        {
            if (!IsConnected)
            {
                return;
            }

            if (data != null && data.Any())
            {
                try
                {
                    m_Stream.Write(data, 0, data.Count());
                    m_Stream.Flush();

                    _mLog.Cmd(true, "发送：", data);
                }
                catch (Exception e)
                {
                    _mLog.Error(true, e.StackTrace);
                }
            }
        }

        internal override void SendMsg(SocketMsgTypeE type, SocketConnectStatusE status, IDevice device)
        {
            if (Monitor.TryEnter(mMsgMod, TimeSpan.FromMilliseconds(500)))
            {
                try
                {
                    mMsgMod.MsgType = type;
                    mMsgMod.ConnStatus = status;
                    mMsgMod.Device = device;
                    Messenger.Default.Send(mMsgMod, MsgToken.CarrierMsgUpdate);
                }
                finally
                {
                    Monitor.Exit(mMsgMod);
                }
            }
        }

        #endregion

        #region[连接成功和数据处理]

        /// <summary>
        /// 成功连接
        ///     1.接收数据
        ///     2.处理数据
        ///     3.发送数据
        /// </summary>
        /// <param name="ar"></param>
        internal override void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (m_Client != null)
                {
                    m_Client.EndConnect(ar);

                    m_Stream = m_Client.GetStream();

                    m_StopSignal = new ManualResetEvent(false);
                    m_Connected = true;

                    m_ReaderThread = new Thread(new ThreadStart(ReceiverHandler));
                    m_ReaderThread.Name = "ClientBaseReceiver";
                    m_ReaderThread.Start();

                    SendMsg(SocketMsgTypeE.Connection, SocketConnectStatusE.连接成功, null);
                    //Console.WriteLine("连接成功:" + DateTime.Now.ToString());

                    _mLog.Status(true, "连接成功");
                }
                else
                {
                    m_RetryTimer = new Timer(delegate (object state)
                    {
                        m_RetryTimer = null;
                        Connect();
                        //Console.WriteLine("连接失败重连:" + DateTime.Now.ToString());
                    }, null, CONNECTION_RETRY_TIMEOUT, 0);
                }
            }
            catch
            {
                m_RetryTimer = new Timer(delegate (object state)
                {
                    m_RetryTimer = null;
                    Connect();
                    //Console.WriteLine("连接失败重连:" + DateTime.Now.ToString());
                }, null, CONNECTION_RETRY_TIMEOUT, 0);
            }
        }

        /// <summary>
        /// 接收到数据
        /// </summary>
        private void ReceiverHandler()
        {
            if (!IsConnected)
            {
                string logMessage = "Cannot start receiver - client not started";
                //throw new InvalidOperationException(logMessage);
                _mLog.Error(true, logMessage);
                Reconnect();
                return;
            }

            byte[] bufferData = null;

            byte[] buffer = new byte[SocketConst.BUFFER_SIZE];

            while (!m_StopSignal.WaitOne(0, false))
            {
                try
                {

                    int bytesRead = m_Stream.Read(buffer, 0, SocketConst.BUFFER_SIZE);
                    if (bytesRead == 0)
                    {
                        //Reconnect();
                        continue;
                    }

                    byte[] readData = buffer.Take(bytesRead).ToArray();

                    if (bufferData != null && bufferData.Length > 0)
                    {
                        readData = bufferData.Concat(readData).ToArray();
                    }


                    if (!ProcessData(ref readData))
                    {
                        Reconnect();

                        //Console.WriteLine("处理数据重连:" + DateTime.Now.ToString());
                        break;
                    }

                    // save until next round
                    bufferData = readData;
                }
                catch (IOException)
                {
                    // unclean disconnect from service
                    Reconnect();
                    //Console.WriteLine("处理数据报错:" + DateTime.Now.ToString()+e.StackTrace);
                    break;
                }
                catch
                {
                    // don't handle error, just wait for end signal
                }
            }
        }

        #endregion

        #region[处理信息]

        internal bool ProcessData(ref byte[] data)
        {
            while (data.Length >= mMinProtLength)
            {
                if (!MatchWithProtocol(ref data))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 循环匹配所有协议
        /// 数据符合
        ///     1.找到合适的协议并更新
        /// 数据不符合
        ///     1.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool MatchWithProtocol(ref byte[] data)
        {
            ushort head = BitConverter.ToUInt16(ShiftBytes(data, 0, 2), 0);
            ushort tail = BitConverter.ToUInt16(ShiftBytes(data, mMinProtLength - 2, 2), 0);

            if (head == SocketConst.CARRIER_HEAD_KEY && tail == SocketConst.TAIL_KEY)
            {
                byte[] pdata = new byte[SocketConst.CARRIER_STATUS_SIZE];
                Array.Copy(data, 0, pdata, 0, SocketConst.CARRIER_STATUS_SIZE);
                DevCarrier device = mProcess.GetStatus(pdata);
                if (device.IsUpdate
                    || device.IsCurrentSiteUpdate
                    || mTimer.IsTimeOutAndReset(TimerTag.DevTcpDateRefresh, DevID, 2))
                {
                    SendMsg(SocketMsgTypeE.DataReiceive, SocketConnectStatusE.通信正常, device);
                    if (device.IsUpdate) _mLog.Status(true, device.ToString());
                }
                if (device.IsAlertUpdate)
                {
                    _mLog.Alert(true, device.AlertToString());
                }
                // remove from data array
                data = data.Skip(SocketConst.CARRIER_STATUS_SIZE).ToArray();
                return true;
            }
            return false;
        }

        #endregion
    }
}
