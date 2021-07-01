using enums;
using module.device;
using module.deviceconfig;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using tool.appconfig;

namespace simtask.task
{
    public class SimFerryTask : SimTaskBase
    {
        #region[属性]       
        public byte DevId
        {
            get => DevStatus.DeviceID;
            set => DevStatus.DeviceID = value;
        }

        public uint FerryTrackId
        {
            get => DevConfig.track_id;
        }

        public bool IsLocating { set; get; } = false;//是否对位中
        public int TargetPos { set; get; }//目标站点脉冲
        public ushort NowPosCode { set; get; }//当前站点
        public int NowPos { set; get; }//当前坐标

        private List<FerryPos> FerryPosList { set; get; }
        #endregion

        #region[构造/启动/停止]

        public DevFerry DevStatus { set; get; }
        public ConfigFerry DevConfig { set; get; }
        public DevFerrySite DevSite { set; get; }

        public SimFerryTask() : base()
        {
            DevStatus = new DevFerry();
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        #endregion

        #region[定位任务]

        internal void StartLocate(ushort desCode)
        {
            if (IsLocating && DevStatus.TargetSite == desCode) return;
            if (DevStatus.TargetSite != desCode || !(DevStatus.UpLight || DevStatus.DownLight))
            {
                DevStatus.TargetSite = desCode;
                DevStatus.CurrentTask = DevFerryTaskE.定位;
                DevStatus.FinishTask = DevFerryTaskE.终止;
                IsLocating = true;
                TargetPos = FerryPosList.Find(c => c.ferry_code == desCode)?.sim_ferry_pos ?? 0;//目标脉冲
            }
        }
        int dir;
        internal void CheckLoaction()
        {
            if (IsLocating)
            {
                dir = TargetPos - NowPos;
                if(dir > 0)
                {
                    NowPos += dir > 200 ? 200 : dir ;
                    DevStatus.DeviceStatus = DevFerryStatusE.后退;
                }
                else
                {
                    NowPos += dir < -200 ? -200 : dir;
                    DevStatus.DeviceStatus = DevFerryStatusE.后退;
                }

                CheckOnFerryPos();

                if((DevStatus.TargetSite == DevStatus.UpSite && DevStatus.UpLight)  ||
                    (DevStatus.TargetSite == DevStatus.DownSite && DevStatus.DownLight))
                {
                    IsLocating = false;
                    DevStatus.DeviceStatus = DevFerryStatusE.停止;
                    DevStatus.FinishTask = DevFerryTaskE.定位;
                }
            }
        }

        internal void HaveLoad()
        {
            if (SimServer.Carrier.ExistOnFerry(FerryTrackId))
            {
                DevStatus.LoadStatus = DevFerryLoadE.载车;
            }
            else
            {
                DevStatus.LoadStatus = DevFerryLoadE.空;
            }
        }

        internal void SetUpFerry()
        {
            DevStatus.DeviceStatus = DevFerryStatusE.停止;
            DevStatus.WorkMode = DevOperateModeE.自动;
            DevStatus.LoadStatus = DevFerryLoadE.空;
            DevStatus.CurrentTask = DevFerryTaskE.终止;
            DevStatus.FinishTask = DevFerryTaskE.终止;

            FerryPosList = new List<FerryPos>();
            FerryPosList.AddRange(PubMaster.Track.GetFerryPos(AreaId, ID));
        }

        public void SetInitSiteAndPos(bool setdown, bool setup)
        {
            FerryPos downferry = FerryPosList.Find(c => c.ferry_code == DevStatus.DownSite);
            if (setdown && DevStatus.DownSite != 0)
            {
                DevStatus.DownLight = true;
                NowPos = downferry?.sim_ferry_pos ?? 0;
            }

            FerryPos upferry = FerryPosList.Find(c => c.ferry_code == DevStatus.UpSite);
            if (setup && DevStatus.UpSite != 0)
            {
                DevStatus.UpLight = true;
                NowPos = upferry?.sim_ferry_pos ?? 0;
            }

            if(downferry != null && Math.Abs(downferry.sim_ferry_pos - NowPos) > 10)
            {
                DevStatus.DownLight = false;
            }

            if(upferry != null && Math.Abs(upferry.sim_ferry_pos - NowPos) > 10)
            {
                DevStatus.UpLight = false;
            }

            DevStatus.CurrentTask = DevFerryTaskE.定位;
            DevStatus.FinishTask = DevFerryTaskE.定位;
            DevStatus.DeviceStatus = DevFerryStatusE.停止;
            DevStatus.TargetSite = 0;
        }
        #endregion

        #region[模拟摆渡车定位]

        /// <summary>
        /// 摆渡车当前是否到达点位
        /// </summary>
        private void CheckOnFerryPos()
        {
            List<FerryPos> tracks = FerryPosList.FindAll(c => c.SimIsInArea(NowPos, 50));
            foreach (var item in tracks)
            {
                if (Device.Type == DeviceTypeE.上摆渡)
                {
                    if(item.ferry_code < 500)
                    {
                        DevStatus.DownSite = item.ferry_code;
                    }
                    else
                    {
                        DevStatus.UpSite = item.ferry_code;
                    }
                }
                else
                {
                    if (item.ferry_code < 300)
                    {
                        DevStatus.DownSite = item.ferry_code;
                    }
                    else
                    {
                        DevStatus.UpSite = item.ferry_code;
                    }
                }

                CheckOnTrackAnd(item);
            }
        }

        /// <summary>
        /// 判断是否对上轨道
        /// </summary>
        /// <param name="pos"></param>
        private void CheckOnTrackAnd(FerryPos pos)
        {
            if(IsOnTrack(pos.ferry_code, pos.sim_ferry_pos))
            {
                if(DevStatus.TargetSite == pos.ferry_code)
                {
                    if(DevStatus.UpSite == DevStatus.TargetSite)
                    {
                        DevStatus.UpLight = true;
                    }

                    if (DevStatus.DownSite == DevStatus.TargetSite)
                    {
                        DevStatus.DownLight = true;
                    }
                }
            }
            else
            {
                if(DevStatus.UpSite == pos.ferry_code)
                {
                    DevStatus.UpLight = false;
                }


                if (DevStatus.DownSite == pos.ferry_code)
                {
                    DevStatus.DownLight = false;
                }
            }
        }

        private bool IsOnTrack(ushort poscode, int pos)
        {
            return DevStatus.TargetSite == poscode && Math.Abs(NowPos - pos) <= 10;
        }

        #endregion
        #region[模拟配置文件初始化]

        internal void SetUpSimulate(SimFerry sim)
        {
            if (sim == null) return;
            IsLocating = sim.IsLocating;
            TargetPos = sim.TargetPos;
            NowPosCode = sim.NowPosCode;
            NowPos = sim.NowPos;
            DevStatus.DeviceStatus = sim.DeviceStatus;
            DevStatus.TargetSite = sim.TargetSite;
            DevStatus.CurrentTask = sim.CurrentTask;
            DevStatus.UpSite = sim.UpSite;
            DevStatus.DownSite = sim.DownSite;
            DevStatus.FinishTask = sim.FinishTask;
            DevStatus.LoadStatus = sim.LoadStatus;
            DevStatus.WorkMode = sim.WorkMode;
            DevStatus.DownLight = sim.DownLight;
            DevStatus.UpLight = sim.UpLight;
        }

        internal SimFerry SaveSimulate()
        {
            SimFerry sim = new SimFerry();
            sim.DevId = ID;
            sim.IsLocating = IsLocating;
            sim.TargetPos = TargetPos;
            sim.NowPosCode = NowPosCode;
            sim.NowPos = NowPos;
            sim.DeviceStatus = DevStatus.DeviceStatus;
            sim.TargetSite = DevStatus.TargetSite;
            sim.CurrentTask = DevStatus.CurrentTask;
            sim.UpSite = DevStatus.UpSite;
            sim.DownSite = DevStatus.DownSite;
            sim.FinishTask = DevStatus.FinishTask;
            sim.LoadStatus = DevStatus.LoadStatus;
            sim.WorkMode = DevStatus.WorkMode;
            sim.DownLight = DevStatus.DownLight;
            sim.UpLight = DevStatus.UpLight;
            return sim;
        }
        #endregion
    }
}
