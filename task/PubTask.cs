using resource;
using System.Threading;
using task.device;
using task.ping;
using task.rf;
using task.trans;

namespace task
{
    public static class PubTask
    {
        public static CarrierMaster Carrier { set; get; }
        public static FerryMaster Ferry { set; get; }
        public static TileLifterMaster TileLifter { set; get; }
        public static TransMaster Trans { set; get; }
        public static RfMaster Rf { set; get; }
        public static PingMaster Ping { set; get; }
        public static TrafficControlMaster TrafficControl { set; get; }
        public static void Init()
        {
            Carrier = new CarrierMaster();
            Ferry = new FerryMaster();
            TileLifter = new TileLifterMaster();
            Trans = new TransMaster();
            Rf = new RfMaster();
            Ping = new PingMaster();
            TrafficControl = new TrafficControlMaster();
        }

        public static void Start()
        {
            new Thread(CheckAndStart)
            {
                IsBackground = true,
                Name = "启动任务逻辑"
            }.Start();
        }

        private static void CheckAndStart()
        {
            while (!PubMaster.IsReady)
            {
                Thread.Sleep(2000);
            }
            TileLifter?.Start();
            Ferry?.Start();
            Carrier?.Start();
            Trans?.Start();
            Rf?.Start();
            Ping?.Start();
            //TrafficControl?.Start();   // 暂时停用 - 取消注释即可用
        }

        public static void Stop()
        {
            Trans?.Stop();
            TileLifter?.Stop();
            Ferry?.Stop();
            Carrier?.Stop();
            Rf?.Stop();
            Ping?.Stop();
            //TrafficControl?.Stop();   // 暂时停用 - 取消注释即可用
        }
    }
}
