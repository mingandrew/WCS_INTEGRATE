using enums;
using System;
using System.Collections.Generic;

namespace tool.appconfig
{
    public class SimulateConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\SimulateConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";

        public List<SimCarrier> SimCarrierList { set; get; }
        public List<SimFerry> SimFerryList { set; get; }
        public List<SimTileLifter> SimTileLifterList { set; get; }

        public SimulateConfig()
        {
            SimCarrierList = new List<SimCarrier>();
            SimFerryList = new List<SimFerry>();
            SimTileLifterList = new List<SimTileLifter>();
        }

        public SimCarrier GetSimCarrier(uint devid)
        {
            return SimCarrierList.Find(c => c.DevId == devid);
        }

        public SimFerry GetSimFerry(uint devid)
        {
            return SimFerryList.Find(c => c.DevId == devid);
        }

        public SimTileLifter GetSimTileLifter(uint devid)
        {
            return SimTileLifterList.Find(c => c.DevId == devid);
        }

        public void UpdateSim(SimCarrier sim)
        {
            SimCarrier md = SimCarrierList.Find(c => c.DevId == sim.DevId);
            if (md != null)
            {
                SimCarrierList.Remove(md);
            }
            SimCarrierList.Add(sim);
        }


        public void UpdateSim(SimFerry sim)
        {
            SimFerry md = SimFerryList.Find(c => c.DevId == sim.DevId);
            if (md != null)
            {
                SimFerryList.Remove(md);
            }
            SimFerryList.Add(sim);
        }

        public void UpdateSim(SimTileLifter sim)
        {
            SimTileLifter md = SimTileLifterList.Find(c => c.DevId == sim.DevId);
            if (md != null)
            {
                SimTileLifterList.Remove(md);
            }
            SimTileLifterList.Add(sim);
        }
    }

    public class SimCarrier
    {
        public uint DevId { set; get; }
        public uint NowTrack { set; get; }
        public uint TargetTrack { set; get; }
        public uint EndTrack { set; get; }
        public bool OnLoading { set; get; }
        public bool OnUnloading { set; get; }
        public bool LoadFinish { set; get; }
        public bool UnloadFinish { set; get; }
        public ushort TO_SITE { set; get; }
        public ushort TO_POINT { set; get; }
        public ushort END_SITE { set; get; }
        public ushort END_POINT { set; get; }
        public ushort SORT_QTY { set; get; }
        public ushort TAKE_STOCK_POINT { set; get; }
        public ushort GIVE_STOCK_POINT { set; get; }
        public byte SORT_TYPE { set; get; }
        public SimCarrierSortStepE SORT_STEP { set; get; }

        public DevCarrierStatusE DeviceStatus { set; get; }
        public ushort CurrentSite { set; get; }
        public ushort CurrentPoint { set; get; }
        public ushort TargetSite { set; get; }
        public ushort TargetPoint { set; get; }
        public DevCarrierOrderE CurrentOrder { set; get; }
        public DevCarrierOrderE FinishOrder { set; get; }
        public DevCarrierLoadE LoadStatus { set; get; }
        public DevCarrierPositionE Position { set; get; }
        public DevOperateModeE OperateMode { set; get; }
        public ushort TakeSite { set; get; }
        public ushort TakePoint { set; get; }
        public ushort GiveSite { set; get; }
        public ushort GivePoint { set; get; }
        public byte MoveCount { set; get; }
    }

    public class SimFerry
    {
        public uint DevId { set; get; }
        public bool IsLocating { set; get; }
        public int TargetPos { set; get; }
        public ushort NowPosCode { set; get; }
        public int NowPos { set; get; }
        public DevFerryStatusE DeviceStatus { set; get; }
        public ushort TargetSite { set; get; }
        public DevFerryTaskE CurrentTask { set; get; }
        public ushort UpSite { set; get; }
        public ushort DownSite { set; get; }
        public DevFerryTaskE FinishTask { set; get; }
        public DevFerryLoadE LoadStatus { set; get; }
        public DevOperateModeE WorkMode { set; get; }
        public bool DownLight { set; get; }
        public bool UpLight { set; get; }
    }

    public class SimTileLifter
    {
        public uint DevId { set; get; }
        public bool Working { set; get; }
        public DateTime? LastPiecesTime { set; get; }
        public byte OnePiecesUsedTime { set; get; }
        public bool IsLeftWork { set; get; } = true;
        public bool IsNeed_1 { set; get; }
        public bool IsNeed_2 { set; get; }
        public bool IsLoad_1 { set; get; }
        public bool IsLoad_2 { set; get; }
        public bool IsInvo_1 { set; get; }
        public bool IsInvo_2 { set; get; }
    }
}
