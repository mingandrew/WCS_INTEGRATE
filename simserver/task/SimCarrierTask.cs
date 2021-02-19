using enums;
using module.device;
using module.deviceconfig;
using module.track;
using resource;
using simtask.task;
using System;

namespace simtask.master
{
    public class SimCarrierTask : SimTaskBase
    {
        #region[任务属性]
        public Track NowTrack { set; get; }
        public ushort TakeSiteCode { set; get; }
        public ushort GiveSiteCode { set; get; }
        public DevCarrierSignalE ActionSignal { set; get; }
        #endregion

        #region[属性]

        public byte DevId
        {
            get => DevStatus.DeviceID;
            set => DevStatus.DeviceID = value;
        }

        public uint TrackId { set; get; }

        #endregion

        #region[构造/启动/停止]
        public DevCarrier DevStatus { set; get; }
        public ConfigCarrier DevConfig { set; get; }

        public SimCarrierTask()
        {
            DevStatus = new DevCarrier();
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        #endregion

        /// <summary>
        /// 根据站点更新当前小车所在轨道
        /// </summary>
        /// <param name="poscode"></param>
        internal void UpdateCurrentSite(ushort poscode)
        {
            if (poscode == 999)
            {
                DevStatus.FinishTask = DevStatus.CurrentTask;
            }
            else
            {
                DevStatus.CurrentSite = poscode;
                Track track = PubMaster.Track.GetTrackByPoint(Device.area, poscode);
                if (track != null)
                {
                    SetNowTrack(track);
                }
            }
        }

        internal void UpdateCurrentSite(Track track)
        {
            if (track != null)
            {
                DevStatus.CurrentSite = track.rfid_1;
                SetNowTrack(track);
            }
        }

        private void SetNowTrack(Track track)
        {
            NowTrack = track;
        }


        #region[执行任务]

        internal void DoTask(DevCarrierTaskE task, DevCarrierSizeE oversize)
        {
            //if (DevStatus.CurrentTask == task) return;
            ////DevStatus.CurrentOverSize = oversize;
            //DevStatus.CurrentTask = task;
            //DevStatus.FinishTask = DevCarrierTaskE.无;
            //switch (task)
            //{
            //    case DevCarrierTaskE.后退取砖:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.后退;
            //        break;
            //    case DevCarrierTaskE.前进放砖:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.前进;
            //        break;

            //    case DevCarrierTaskE.后退至摆渡车:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.后退;
            //        break;
            //    case DevCarrierTaskE.前进至摆渡车:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.前进;
            //        break;

            //    case DevCarrierTaskE.倒库:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.后退;
            //        break;

            //    case DevCarrierTaskE.前进至点:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.前进;
            //        break;

            //    case DevCarrierTaskE.后退至点:
            //        DevStatus.DeviceStatus = DevCarrierStatusE.后退;
            //        break;
            //}
        }

        #endregion

        #region[检查任务]

        internal void CheckTask()
        {
            //if (NowTrack == null) return;
            //if (DevStatus.CurrentTask == DevStatus.FinishTask) return;
            ////DevStatus.ActionType = DevCarrierSignalE.非空非满;
            //switch (DevStatus.CurrentTask)
            //{
            //    #region[后退取砖]
            //    case DevCarrierTaskE.后退取砖:
            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.上砖轨道:
            //            case TrackTypeE.储砖_入:
            //                break;
            //            case TrackTypeE.摆渡车_入:
            //            case TrackTypeE.摆渡车_出:
            //                ushort poscode = PubTask.Ferry.GetFerryOnTrackPosCode(NowTrack.rfid_1, true);
            //                if (poscode != 0)
            //                {
            //                    UpdateCurrentSite(poscode);
            //                }
            //                break;
            //            case TrackTypeE.下砖轨道:
            //                //小车达到下砖轨道进行取砖
            //                //通知下砖机货物状态改为无货
            //                if (PubTask.TileLifter.DoUnload(NowTrack.id) || MTimer._.IsOver("LoadTimeReach",10))
            //                {
            //                    DevStatus.FinishTask = DevCarrierTaskE.后退取砖;
            //                    DevStatus.LoadStatus = DevCarrierLoadE.有货;
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;

            //                    TakeSiteCode = NowTrack.rfid_1;
            //                }
            //                break;
            //            case TrackTypeE.储砖_出:

            //                int piecesdis = PubMaster.Dic.GetDtlIntCode(DicTag.StockOutPiceseDis);
            //                int fulltime = PubMaster.Dic.GetDtlIntCode(DicTag.StockOutFullTime);
            //                int waitsecond = fulltime - (NowTrack.store_count * piecesdis);
            //                if (MTimer._.IsOver(TimerTag.CarrierLoading, ID, waitsecond))
            //                {
            //                    // 小车达到下砖轨道进行取砖
            //                    //通知下砖机货物状态改为无货
            //                    DevStatus.FinishTask = DevCarrierTaskE.后退取砖;
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;

            //                    TakeSiteCode = NowTrack.rfid_1;
            //                    if (PubMaster.Track.IsTrackEmpty(NowTrack.id))
            //                    {
            //                        //DevStatus.ActionType = DevCarrierSignalE.空轨道;
            //                        DevStatus.LoadStatus = DevCarrierLoadE.无货;
            //                    }
            //                    else
            //                    {
            //                        DevStatus.LoadStatus = DevCarrierLoadE.有货;
            //                        ActionSignal = DevCarrierSignalE.非空非满;
            //                    }

            //                    PubMaster.Track.RemoveStock(NowTrack.id);
            //                }
            //                break;
            //            default:
            //                break;
            //        }
            //        break;
            //    #endregion

            //    #region[前进放砖]
            //    case DevCarrierTaskE.前进放砖:
            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.上砖轨道:
            //                //小车达到储砖入轨道放砖
            //                if (MTimer._.IsOver(TimerTag.CarrierBeenUnLoad, DevId, 5))
            //                {
            //                    //小车达到上砖轨道进行放砖
            //                    //通知上砖机货物状态改为有货
            //                    if (PubTask.TileLifter.DoLoad(NowTrack.id))
            //                    {
            //                        DevStatus.FinishTask = DevCarrierTaskE.前进放砖;
            //                        DevStatus.LoadStatus = DevCarrierLoadE.无货;
            //                        DevStatus.DeviceStatus = DevCarrierStatusE.停止;

            //                        GiveSiteCode = NowTrack.rfid_1;

            //                        SetAction();
            //                    }
            //                }

            //                break;
            //            case TrackTypeE.储砖_入:

            //                int piecesdis = PubMaster.Dic.GetDtlIntCode(DicTag.StockInPiceseDis);
            //                int fulltime = PubMaster.Dic.GetDtlIntCode(DicTag.StockInFullTime);
            //                int waitsecond = fulltime - (NowTrack.LeftToGive() * piecesdis);
            //                //小车达到储砖入轨道放砖
            //                if (MTimer._.IsOver(TimerTag.CarrierUnloading, DevId, waitsecond))
            //                {
            //                    DevStatus.FinishTask = DevCarrierTaskE.前进放砖;
            //                    DevStatus.LoadStatus = DevCarrierLoadE.无货;
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;

            //                    GiveSiteCode = NowTrack.rfid_1;

            //                    PubMaster.Track.AddStock(NowTrack.id);

            //                    if (PubMaster.Track.IsTrackFull(NowTrack.id))
            //                    {
            //                        ActionSignal = DevCarrierSignalE.满轨道;
            //                    }
            //                    else
            //                    {
            //                        ActionSignal = DevCarrierSignalE.非空非满;
            //                    }
            //                    SetAction();
            //                }
            //                break;
            //            case TrackTypeE.摆渡车_入:
            //            case TrackTypeE.摆渡车_出:
            //                //DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                //return;
            //                ushort poscode = PubTask.Ferry.GetFerryOnTrackPosCode(NowTrack.rfid_1, false);
            //                if (poscode != 0)
            //                {
            //                    UpdateCurrentSite(poscode);
            //                }
            //                break;
            //            case TrackTypeE.下砖轨道:
            //                break;
            //            case TrackTypeE.储砖_出:
            //                break;
            //            default:
            //                break;
            //        }
            //        break;
            //    #endregion

            //    #region[后退至摆渡车]
            //    case DevCarrierTaskE.后退至摆渡车:
            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.上砖轨道:
            //                //获取摆渡车对应的轨道ID
            //                uint trackid = PubTask.Ferry.GetFerryTrackId(NowTrack.rfid_1);
            //                if (trackid != 0)
            //                {
            //                    NowTrack = PubMaster.Track.GetTrack(trackid);
            //                    DevStatus.CurrentSite = NowTrack.rfid_1;
            //                }

            //                break;
            //            case TrackTypeE.储砖_入:

            //                //获取摆渡车对应的轨道ID
            //                trackid =  PubTask.Ferry.GetFerryTrackId(NowTrack.rfid_1);
            //                if (trackid != 0)
            //                {
            //                    SetNowTrack(PubMaster.Track.GetTrack(trackid));
            //                    DevStatus.CurrentSite = NowTrack.rfid_1;
            //                }
            //                break;

            //            case TrackTypeE.摆渡车_入:
            //            case TrackTypeE.摆渡车_出:
            //                DevStatus.CurrentSite = NowTrack.rfid_1;
            //                DevStatus.FinishTask = DevStatus.CurrentTask;
            //                DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                break;
            //        }
            //        break;
            //    #endregion

            //    #region[前进至摆渡车]
            //    case DevCarrierTaskE.前进至摆渡车:
            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.下砖轨道:
            //            case TrackTypeE.储砖_出:
            //                if (MTimer._.IsOver("FrontToFerryOvertime", DevId, 10))
            //                {
            //                    uint postrack = PubTask.Ferry.GetFerryTrackId(NowTrack.rfid_1);
            //                    if (postrack != 0)
            //                    {
            //                        SetNowTrack(PubMaster.Track.GetTrack(postrack));
            //                    }
            //                }

            //                break;
            //            case TrackTypeE.摆渡车_入:
            //            case TrackTypeE.摆渡车_出:
            //                DevStatus.CurrentSite = NowTrack.rfid_1;
            //                DevStatus.FinishTask = DevStatus.CurrentTask;
            //                DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                break;
            //            default:
            //                break;
            //        }

            //        break;
            //    #endregion

            //    #region[后退至轨道倒库]
            //    case DevCarrierTaskE.后退至轨道倒库:

            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.储砖_出:

            //                int fulltime = NowTrack.max_store * PubMaster.Dic.GetDtlIntCode(DicTag.StockInFullTime);
            //                if (MTimer._.IsOver(TimerTag.CarrierSortting, fulltime))
            //                {
            //                    DevStatus.CurrentSite = NowTrack.rfid_1;
            //                    DevStatus.FinishTask = DevStatus.CurrentTask;
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;

            //                    PubMaster.Track.ShiftStock(NowTrack.id);
            //                }

            //                break;

            //            case TrackTypeE.摆渡车_出:
            //                //获取摆渡车对应的轨道ID
            //                ushort poscode = PubTask.Ferry.GetFerryOnTrackPosCode(NowTrack.rfid_1, true);
            //                if (poscode != 0)
            //                {
            //                    UpdateCurrentSite(poscode);
            //                    DevStatus.CurrentSite = NowTrack.rfid_1;
            //                }
            //                break;
            //        }
            //        break;
            //    #endregion

            //    #region[前进至点]

            //    case DevCarrierTaskE.前进至点:
            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.储砖_入:
            //                if (MTimer._.IsOver("BackCountTime", 10))
            //                {
            //                    uint trackid = PubMaster.Track.GetBrotherTrackId(NowTrack.id);

            //                    SetNowTrack(PubMaster.Track.GetTrack(trackid));
            //                    DevStatus.CurrentSite = NowTrack.rfid_1;
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                }
            //                break;
            //            case TrackTypeE.摆渡车_入:
            //            case TrackTypeE.摆渡车_出:
            //                ushort poscode = PubTask.Ferry.GetFerryOnTrackPosCode(NowTrack.rfid_1, false);
            //                if (poscode != 0)
            //                {
            //                    UpdateCurrentSite(poscode);
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                    DevStatus.FinishTask = DevCarrierTaskE.前进至点;
            //                }
            //                break;
            //            case TrackTypeE.下砖轨道:
            //                break;
            //            case TrackTypeE.储砖_出:
            //                DevStatus.FinishTask = DevCarrierTaskE.前进至点;
            //                DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                break;
            //            default:
            //                break;
            //        }
            //        break;
            //    #endregion

            //    #region[后退至点]

            //    case DevCarrierTaskE.后退至点:
            //        switch (NowTrack.Type)
            //        {
            //            case TrackTypeE.上砖轨道:
            //                break;
            //            case TrackTypeE.下砖轨道:
            //                break;
            //            case TrackTypeE.储砖_入:
            //                DevStatus.FinishTask = DevCarrierTaskE.后退至点;
            //                DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                break;
            //            case TrackTypeE.储砖_出:

            //                uint trackid = PubMaster.Track.GetBrotherTrackId(NowTrack.id);
            //                Track btrack = PubMaster.Track.GetTrack(trackid);
            //                UpdateCurrentSite(btrack);
            //                DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                DevStatus.FinishTask = DevCarrierTaskE.后退至点;

            //                break;
            //            case TrackTypeE.储砖_出入:
            //                break;
            //            case TrackTypeE.摆渡车_入:
            //                break;
            //            case TrackTypeE.摆渡车_出:
            //                ushort poscode = PubTask.Ferry.GetFerryOnTrackPosCode(NowTrack.rfid_1, true);
            //                if (poscode != 0)
            //                {
            //                    UpdateCurrentSite(poscode);
            //                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //                    DevStatus.FinishTask = DevCarrierTaskE.后退至点;
            //                }
            //                break;
            //            default:
            //                break;
            //        }
            //        break;
            //    #endregion

            //    #region[其他任务]

            //    case DevCarrierTaskE.顶升取货:
            //        DevStatus.LoadStatus = DevCarrierLoadE.有货;
            //        DevStatus.FinishTask = DevCarrierTaskE.顶升取货;
            //        break;
            //    case DevCarrierTaskE.下降放货:
            //        DevStatus.LoadStatus = DevCarrierLoadE.无货;
            //        DevStatus.FinishTask = DevCarrierTaskE.下降放货;
            //        break;
            //    case DevCarrierTaskE.终止:
            //        DevStatus.FinishTask = DevCarrierTaskE.终止;
            //        DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            //        break;
            //    case DevCarrierTaskE.其他:
            //        break;
            //        #endregion
            //}
        }

        private void SetAction()
        {
            ushort actiontime = (ushort)(DateTime.Now.Hour * 100);
            actiontime += (ushort)DateTime.Now.Minute;
            //DevStatus.ActionTime = actiontime;
            //DevStatus.TakeTrackCode = TakeSiteCode;
            //DevStatus.GiveTrackCode = GiveSiteCode;
            //DevStatus.ActionType = ActionSignal;
        }
        #endregion
    }
}
