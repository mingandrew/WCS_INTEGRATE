using resource.area;
using resource.device;
using resource.diction;
using resource.goods;
using resource.module;
using resource.role;
using resource.tiletrack;
using resource.track;
using task;

namespace resource
{
    public static class PubMaster
    {
        public static bool _isready = false;
        public static ModuleMaster Mod { set; get; }
        public static DictionMaster Dic { set; get; }
        public static AreaMaster Area { set; get; }
        public static TrackMaster Track { set; get; }
        public static DeviceMaster Device { set; get; }
        public static GoodsMaster Goods { set; get; }
        public static WarningMaster Warn { set; get; }
        public static TileTrackMaster TileTrack { set; get; }
        public static RoleMaster Role { set; get; }

        public static void Init()
        {
            Mod = new ModuleMaster();
            Dic = new DictionMaster();
            Area = new AreaMaster();
            Track = new TrackMaster();
            Device = new DeviceMaster();
            Goods = new GoodsMaster();
            Warn = new WarningMaster();
            TileTrack = new TileTrackMaster();
            Role = new RoleMaster();
            PreStart();
        }

        public static void PreStart()
        {
            Role?.Start();
        }

        public static void StartMaster()
        {
            Warn.Start();
            Mod.Start();
            Dic.Start();
            Area.Start();
            Track.Start();
            Device.Start();
            Goods.Start();
            TileTrack.Start();
            _isready = true;
        }

        public static void StopMaster()
        {
            Warn.Stop();
            Mod.Stop();
            Role?.Stop();
            Dic.Stop();
            Device.Stop();
            Track.Stop();
            Area.Stop();
            Goods.Stop();
            TileTrack.Stop();
        }

        public static bool IsReady
        {
            get=>_isready;
        }
    }
}
