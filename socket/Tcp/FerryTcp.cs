using enums;
using GalaSoft.MvvmLight.Messaging;
using module.device;
using socket.process;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using tool.appconfig;
using tool.timer;

namespace socket.tcp
{
    public class FerryTcp : TcpBase
    {
        #region[字段]

        private FerryProcesser mProcess;

        #endregion
        public FerryTcp(Device dev) : base(dev)
        {
            mProcess = new FerryProcesser();
            mMinProtLength = SocketConst.FERRY_SPEED_SIZE;
        }

        #region[发送信息]
        public void SendCmd(DevFerryCmdE type, byte b1, byte b2, int int3, byte mark = 0)
        {
            if (Monitor.TryEnter(_senobj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    byte[] data = mProcess.GetCmd(mDev.memo, type, b1, b2, int3, mark);
                    SendMessage(data);
                }
                finally
                {
                    Monitor.Exit(_senobj);
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
                    Messenger.Default.Send(mMsgMod, MsgToken.FerryMsgUpdate);
                }
                finally
                {
                    Monitor.Exit(mMsgMod);
                }
            }
        }

        public void SendAutoPosCmd(DevFerryCmdE type, ushort b1, byte b3, byte b4)
        {
            if (Monitor.TryEnter(mMsgMod, TimeSpan.FromMilliseconds(500)))
            {

                try
                {
                    byte[] data = mProcess.GetAutoPosCmd(mDev.memo, type, b1, b3, b4);
                    SendMessage(data);
                }
                finally
                {
                    Monitor.Exit(mMsgMod);
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
                    Reconnect();
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
                        continue;
                        //Reconnect();
                        //break;
                    }

                    byte[] readData = buffer.Take(bytesRead).ToArray();

                    if (bufferData != null && bufferData.Length > 0)
                    {
                        readData = bufferData.Concat(readData).ToArray();
                    }

                    if (!ProcessData(ref readData))
                    {
                        Reconnect();
                        break;
                    }

                    // save until next round
                    bufferData = readData;
                }
                catch (IOException)
                {
                    // unclean disconnect from service
                    Reconnect();
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
            int orglength = data.Length;
            while (data.Length >= mMinProtLength)
            {
                MatchWithProtocol(ref data);
                if (data.Length == orglength)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断尾部是否符合
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tialoffset"></param>
        /// <returns></returns>
        private bool IsTailMatch(byte[] data, int tialoffset)
        {
            return SocketConst.TAIL_KEY == BitConverter.ToUInt16(ShiftBytes(data, tialoffset - 2, 2), 0);
        }

        /// <summary>
        /// 区分协议更新数据
        /// </summary>
        /// <param name="data"></param>
        private void MatchWithProtocol(ref byte[] data)
        {
            if (GlobalWcsDataConfig.DebugConfig.LogDeviceReceiver) _mLog.Cmd(true, "接收：", data);

            ushort head = BitConverter.ToUInt16(ShiftBytes(data, 0, 2), 0);
            switch (head)
            {
                case SocketConst.FEERY_STATUS_HEAD_KEY:
                    if (IsTailMatch(data, SocketConst.FERRY_STATUS_SIZE))
                    {
                        ProcessStatus(ref data);
                    }
                    break;
                case SocketConst.FEERY_SPEED_HEAD_KEY:
                    if (IsTailMatch(data, SocketConst.FERRY_SPEED_SIZE))
                    {
                        ProcessSpeed(ref data);
                    }
                    break;
                case SocketConst.FEERY_SITE_HEAD_KEY:
                    if (IsTailMatch(data, SocketConst.FERRY_SITE_SIZE))
                    {
                        ProcessSite(ref data);
                    }
                    break;
            }
        }

        #region[状态/速度/站点]
        private void ProcessStatus(ref byte[] data)
        {
            byte[] pdata = new byte[SocketConst.FERRY_STATUS_SIZE];
            Array.Copy(data, 0, pdata, 0, SocketConst.FERRY_STATUS_SIZE);
            IDevice device = mProcess.GetStatus(pdata);
            if (device.IsUpdate || mTimer.IsTimeOutAndReset(TimerTag.DevTcpDateRefresh, DevID, 5))
            {
                SendMsg(SocketMsgTypeE.DataReiceive, SocketConnectStatusE.通信正常, device);
                if (device.IsUpdate) _mLog.Status(true, device.ToString());
            }

            // remove from data array
            data = data.Skip(SocketConst.FERRY_STATUS_SIZE).ToArray();
        }

        private void ProcessSpeed(ref byte[] data)
        {
            byte[] pdata = new byte[SocketConst.FERRY_SPEED_SIZE];
            Array.Copy(data, 0, pdata, 0, SocketConst.FERRY_SPEED_SIZE);
            IDevice device = mProcess.GetSpeed(pdata);
            if (device.IsUpdate)
            {
                SendMsg(SocketMsgTypeE.DataReiceive, SocketConnectStatusE.通信正常, device);
                _mLog.Status(true, device.ToString());
            }

            // remove from data array
            data = data.Skip(SocketConst.FERRY_SPEED_SIZE).ToArray();
        }

        private void ProcessSite(ref byte[] data)
        {
            byte[] pdata = new byte[SocketConst.FERRY_SITE_SIZE];
            Array.Copy(data, 0, pdata, 0, SocketConst.FERRY_SITE_SIZE);
            IDevice device = mProcess.GetSite(pdata);
            if (device.IsUpdate)
            {
                SendMsg(SocketMsgTypeE.DataReiceive, SocketConnectStatusE.通信正常, device);
                _mLog.Status(true, device.ToString());
            }

            // remove from data array
            data = data.Skip(SocketConst.FERRY_SITE_SIZE).ToArray();
        }

        #endregion

        #endregion
    }
}
