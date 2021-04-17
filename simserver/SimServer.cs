using simtask.resource;
using simtask.master;
using tool.appconfig;

namespace simtask
{
    public static class SimServer
    {
        public static bool IsStartServer { set; get; }
        public static SimCarrierMaster Carrier { set; get; }
        public static SimFerryMaster Ferry { set; get; }
        public static SimTileLifterMaster TileLifter { set; get; }
        public static SimSourceMaster Source { set; get; }

        public static void Init()
        {
            Source = new SimSourceMaster();
            Carrier = new SimCarrierMaster();
            Ferry = new SimFerryMaster();
            TileLifter = new SimTileLifterMaster();
        }

        public static void Start()
        {
            Source?.Start();
            TileLifter?.Start();
            Ferry?.Start();
            Carrier?.Start();

            IsStartServer = true;
        }

        public static void Stop()
        {
            Source?.Stop();
            TileLifter?.Stop();
            Ferry?.Stop();
            Carrier?.Stop();

            IsStartServer = false;

            GlobalWcsDataConfig.SaveSimulateConfig();
        }
    }
}
