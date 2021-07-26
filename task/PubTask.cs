using resource;
using System.Threading;
using task.allocate;
using task.device;
using task.ping;
using task.rf;
using task.trans;

namespace task
{
    /// <summary>
    /// 任务资源管理公共类
    /// </summary>
    public static class PubTask
    {
        public static CarrierMaster Carrier { set; get; }
        public static FerryMaster Ferry { set; get; }
        public static TileLifterMaster TileLifter { set; get; }
        public static TransMaster Trans { set; get; }
        public static RfMaster Rf { set; get; }
        public static PingMaster Ping { set; get; }
        public static TrafficControlMaster TrafficControl { set; get; }
        public static TileLifterNeedMaster TileLifterNeed { set; get; }  //需求操作对象 20210121
        public static AllocateMaster Allocate { set; get; }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Carrier = new CarrierMaster();
            Ferry = new FerryMaster();
            TileLifter = new TileLifterMaster();
            Trans = new TransMaster();
            Rf = new RfMaster();
            Ping = new PingMaster();
            TrafficControl = new TrafficControlMaster();
            TileLifterNeed = new TileLifterNeedMaster();
            Allocate = new AllocateMaster();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public static void Start()
        {
            new Thread(CheckAndStart)
            {
                IsBackground = true,
                Name = "启动任务逻辑"
            }.Start();
        }

        /// <summary>
        /// 检测数据加载并启动各个服务
        /// </summary>
        private static void CheckAndStart()
        {
            //检测数据加载完成后启动
            while (!PubMaster.IsReady)
            {
                Thread.Sleep(2000);
            }
            TileLifterNeed?.Start();
            TileLifter?.Start();
            Ferry?.Start();
            Carrier?.Start();
            Trans?.Start();
            Rf?.Start();
            Ping?.Start();
            TrafficControl?.Start();
            Allocate?.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public static void Stop()
        {
            Trans?.Stop();
            TileLifter?.Stop();
            Ferry?.Stop();
            Carrier?.Stop();
            Rf?.Stop();
            Ping?.Stop();
            TileLifterNeed?.Stop();
            TrafficControl?.Stop();
            Allocate?.Stop();
        }

        /// <summary>
        /// 停止模拟的设备
        /// </summary>
        public static void StopSimDevice()
        {
            Ferry?.StockSimDevice();
            Carrier?.StockSimDevice();
            TileLifter?.StockSimDevice();
        }
    }
}
