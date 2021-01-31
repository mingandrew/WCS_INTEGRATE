using enums;
using module.device;
using module.deviceconfig;
using module.track;

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


        public bool IsLocating { set; get; } = false;
        public ushort NowPosCode { set; get; }//当前站点
        public int NowPos { set; get; }//当前坐标

        #endregion

        #region[构造/启动/停止]

        public DevFerry DevStatus { set; get; }
        public ConfigFerry DevConfig { set; get; }
        public DevFerrySite DevSite { set; get; }

        public SimFerryTask()
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
                SetBackFront();
                DevStatus.CurrentTask = DevFerryTaskE.定位;
                DevStatus.FinishTask = DevFerryTaskE.终止;
                IsLocating = true;
            }
        }
        private void SetBackFront()
        {
            bool isfront = SimServer.Source.IsTargetFront(DevStatus.TargetSite, NowPos);
            DevStatus.DeviceStatus = isfront ? DevFerryStatusE.前进 : DevFerryStatusE.后退;
        }


        internal void CheckLoaction()
        {
            if (IsLocating)
            {
                bool isdownferry = Type == DeviceTypeE.下摆渡;
                bool isfront = SimServer.Source.IsTargetFront(DevStatus.TargetSite, NowPos);
                NowPos += (isfront ? 50 : -50);
                //NowPos = 1100;
                DevStatus.DownLight = false;
                DevStatus.UpLight = false;
                if (SimServer.Source.IsOnFerryPos(Device.area, NowPos, isdownferry, out SimFerryPos upferrypose, out SimFerryPos downferrypose))
                {
                    if (downferrypose != null)
                    {
                        DevStatus.DownSite = downferrypose.ferry_code;
                        DevStatus.DownLight = true;
                        NowPosCode = downferrypose.ferry_code;
                    }

                    if(upferrypose != null)
                    {
                        DevStatus.UpSite = upferrypose.ferry_code;
                        DevStatus.UpLight = true;
                        NowPosCode = upferrypose.ferry_code;
                    }

                    if ((DevStatus.UpSite == DevStatus.TargetSite && DevStatus.UpLight)
                        || (DevStatus.DownSite == DevStatus.TargetSite && DevStatus.DownLight))
                    {
                        IsLocating = false;
                        DevStatus.DeviceStatus = DevFerryStatusE.停止;
                        DevStatus.FinishTask = DevFerryTaskE.定位;
                    }
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
        #endregion

    }
}
