using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using tool.mlog;

namespace task.ping
{
    public class PingMaster
    {

        #region[字段]

        private List<PingItem> PingList { set; get; }
        private Thread mRefresh;
        private bool IsRuning;
        private Log mLog;
        private object _obj;
        #endregion


        #region[构造/启动/停用]
        public PingMaster()
        {
            _obj = new object();
            mLog = (Log)new LogFactory().GetLog("Ping", false);
            PingList = new List<PingItem>();

        }
        public void Start()
        {
            if (mRefresh != null)
            {
                if (mRefresh.IsAlive)
                {
                    mRefresh.Join(TimeSpan.FromSeconds(1));
                }
            }

            mRefresh = new Thread(Refresh)
            {
                Name = "Ping设备网络",
                IsBackground = true
            };

            mRefresh.Start();
        }

        public void Stop()
        {
            IsRuning = false;
            if (mRefresh != null)
            {
                if (mRefresh.IsAlive)
                {
                    mRefresh.Join(TimeSpan.FromSeconds(1));
                }
            }
        }

        private void Refresh()
        {
            while (!IsRuning)
            {
                Thread.Sleep(5000);

                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        foreach (var item in PingList)
                        {
                            item.Ping();
                        }
                    }
                    finally
                    {
                        Monitor.Exit(_obj);
                    }
                }
            }
        }


        #endregion

        #region[添加/删除Ping]

        public void AddPing(string who, string name)
        {
            if (!PingList.Exists(c => c.host.Equals(who)))
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        Ping pingSender = new Ping();

                        // When the PingCompleted event is raised,
                        // the PingCompletedCallback method is called.
                        pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                        PingList.Add(new PingItem(name, who, mLog)
                        {
                            pingSender = pingSender
                        });
                    }
                    catch (Exception e)
                    {
                        mLog.Error(true, e.Message, e);
                    }
                    finally
                    {
                        Monitor.Exit(_obj);
                    }
                }
            }
        }


        public void RemovePing(string who)
        {
            PingItem item = PingList.Find(c => c.host.Equals(who));
            if (item != null)
            {
                if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        PingList.Remove(item);
                    }
                    catch (Exception e)
                    {
                        mLog.Error(true, e.Message, e);
                    }
                    finally
                    {
                        Monitor.Exit(_obj);
                    }
                }
            }
        }

        #endregion

        #region[Ping网络]


        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            string host = e.UserState.ToString();
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                //Console.WriteLine("Ping canceled.");
                mLog.Status(true, host + ":Ping canceled.");
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                //Console.WriteLine("Ping failed:");
                //Console.WriteLine(e.Error.ToString());
                mLog.Error(true, host + ":Ping failed:", e.Error);
            }

            DisplayReply(e.Reply, host);

        }

        public void DisplayReply(PingReply reply, string host)
        {
            if (reply == null)
                return;

            //Console.WriteLine("ping status: {0}", reply.Status);
            if (reply.Status == IPStatus.Success)
            {
                mLog.Status(true, string.Format("{0},OK", host));
                //Console.WriteLine("Address: {0}", reply.Address.ToString());
                //Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                //Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                //Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                //Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
            else
            {
                mLog.Status(true, string.Format("{0},Error：{1}", host, reply.Status));
            }
        }
        #endregion
    }


    public class PingItem
    {
        private string name { set; get; }
        public string host { set; get; }
        public Ping pingSender { set; get; }
        private Log mLog;

        // Set options for transmission:
        // The data can go through 64 gateways or routers
        // before it is destroyed, and the data packet
        // cannot be fragmented.
        PingOptions options = new PingOptions(64, true);

        public PingItem(string n, string who, Log log)
        {
            name = string.Format("({0}:{1})", n, who);
            host = who;
            mLog = log;
        }

        public void Ping()
        {
            try
            {
                // Send the ping asynchronously.
                // Use the waiter as the user token.
                // When the callback completes, it can wake up this thread.
                pingSender.SendAsync(host, 500, new byte[0], options, name);
            }
            catch (Exception e)
            {
                mLog.Error(true, e.Message, e);
            }
        }
    }
}
