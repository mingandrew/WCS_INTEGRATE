using enums;
using enums.track;
using module.device;
using module.deviceconfig;
using module.goods;
using module.track;
using resource;
using simserver.simsocket;
using simtask.task;
using System;
using System.Collections.Generic;

namespace simtask
{
    public class SimCarrierTask : SimTaskBase
    {
        #region[任务属性]
        public Track NowTrack { set; get; }
        public Track TargetTrack { set; get; }
        public Track EndTrack { set; get; }
        #endregion

        #region[属性]

        public byte DevId
        {
            get => DevStatus.DeviceID;
            set => DevStatus.DeviceID = value;
        }

        public uint TrackId { set; get; }

        private const ushort ZERO_SITE = 0;
        private const ushort ZERO_POINT = 0;

        /// <summary>
        /// 正在取货
        /// </summary>
        private bool OnLoading { set; get; }
        /// <summary>
        /// 正在卸货
        /// </summary>
        private bool OnUnloading { set; get; }
        /// <summary>
        /// 取货完成
        /// </summary>
        private bool LoadFinish { set; get; }
        /// <summary>
        /// 卸货完成
        /// </summary>
        private bool UnloadFinish { set; get; }
        /// <summary>
        /// 定位站点
        /// </summary>
        private ushort TO_SITE { set; get; }
        /// <summary>
        /// 定位脉冲
        /// </summary>
        private ushort TO_POINT { set; get; }
        /// <summary>
        /// 结束站点
        /// </summary>
        private ushort END_SITE { set; get; }
        /// <summary>
        /// 结束脉冲
        /// </summary>
        private ushort END_POINT { set; get; }
        /// <summary>
        /// 倒库数量
        /// </summary>
        private ushort SORT_QTY { set; get; }
        /// <summary>
        /// 取砖脉冲点
        /// </summary>
        private ushort TAKE_STOCK_POINT { set; get; }
        /// <summary>
        /// 卸货脉冲点
        /// </summary>
        private ushort GIVE_STOCK_POINT { set; get; }

        /// <summary>
        /// 倒库任务
        /// </summary>
        private const byte IN_2_OUT_SORT = 1;
        /// <summary>
        /// 上砖侧倒库
        /// </summary>
        private const byte OUT_2_OUT_SORT = 2;
        /// <summary>
        /// 运输车倒库任务类型
        /// </summary>
        private byte SORT_TYPE;
        private SimCarrierSortStepE SORT_STEP;
        #endregion

        #region[构造/启动/停止]
        public DevCarrier DevStatus { set; get; }
        public ConfigCarrier DevConfig { set; get; }

        public SimCarrierTask() : base()
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
        internal void UpdateCurrentSite(ushort initsite, ushort initpoint)
        {
            DevStatus.CurrentSite = initsite;
            DevStatus.CurrentPoint = initpoint;
            Track track = PubMaster.Track.GetTrackBySite(Device.area, initsite);
            if (track != null)
            {
                SetNowTrack(track);
            }
        }

        internal void UpdateCurrentSite(ushort site)
        {
            DevStatus.CurrentSite = site;
            Track track = PubMaster.Track.GetTrackBySite(Device.area, site);
            if (track != null)
            {
                SetNowTrack(track);
            }
        }

        private void SetNowTrack(Track track)
        {
            NowTrack = track;
        }

        private void SetNowTrack(Track track, ushort site)
        {
            DevStatus.CurrentSite = site;
            NowTrack = track;
        }

        private void SetTargetTrack(Track track)
        {
            TargetTrack = track;
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

        /// <summary>
        /// 初始化设置
        /// </summary>
        internal void SetUpInit()
        {
            DevStatus.OperateMode = DevOperateModeE.自动;
            DevStatus.LoadStatus = DevCarrierLoadE.无货;
            DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            DevStatus.CurrentOrder = DevCarrierOrderE.终止指令;
            DevStatus.FinishOrder = DevCarrierOrderE.终止指令;
            DevStatus.Position = DevCarrierPositionE.在轨道上;
        }

        #endregion

        #region[检查任务]

        internal void CheckTask()
        {
            if (NowTrack == null) return;
            if (DevStatus.CurrentOrder == DevStatus.FinishOrder) return;

            UpdateTrackSite();
            UpdateTrackPoint();
            switch (DevStatus.CurrentOrder)
            {
                #region[无]
                case DevCarrierOrderE.无:
                    break;
                #endregion

                #region[定位指令]
                case DevCarrierOrderE.定位指令:

                    switch (TargetTrack.Type)
                    {
                        case TrackTypeE.上砖轨道:

                            break;
                        case TrackTypeE.下砖轨道:

                            break;
                        case TrackTypeE.储砖_入:
                            if (TO_SITE == TargetTrack.rfid_1)
                            {
                                SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                                DevStatus.CurrentPoint = TargetTrack.limit_point;
                            }
                            else if (TO_SITE == TargetTrack.rfid_2)
                            {
                                SetNowTrack(TargetTrack, TargetTrack.rfid_2);
                                DevStatus.CurrentPoint = TargetTrack.limit_point_up;
                            }
                            break;
                        case TrackTypeE.储砖_出:

                            if (TO_SITE == TargetTrack.rfid_1)
                            {
                                SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                                DevStatus.CurrentPoint = TargetTrack.limit_point_up;
                            }
                            break;
                        case TrackTypeE.储砖_出入:
                            if(TO_SITE == TargetTrack.rfid_1)
                            {
                                SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                                DevStatus.CurrentPoint = TargetTrack.limit_point;
                            }else if(TO_SITE == TargetTrack.rfid_2)
                            {
                                SetNowTrack(TargetTrack, TargetTrack.rfid_2);
                                DevStatus.CurrentPoint = TargetTrack.limit_point_up;
                            }
                            break;
                        case TrackTypeE.摆渡车_入:
                            DevStatus.CurrentPoint = SimServer.Carrier.GetFerryTrackPos(TargetTrack.rfid_1);
                            SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                            break;
                        case TrackTypeE.摆渡车_出:
                            DevStatus.CurrentPoint = SimServer.Carrier.GetFerryTrackPos(TargetTrack.rfid_1);
                            SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                            break;
                        default:
                            break;
                    }

                    if(DevStatus.CurrentSite == DevStatus.TargetSite)
                    {
                        DevStatus.TargetSite = ZERO_SITE;
                        FinishAndStop(DevCarrierOrderE.定位指令);
                    }

                    break;
                #endregion

                #region[取砖指令]
                case DevCarrierOrderE.取砖指令:

                    #region[顶升取货]
                    if (TO_SITE == ZERO_SITE
                        && TO_POINT == ZERO_POINT)
                    {
                        DevStatus.LoadStatus = DevCarrierLoadE.有货;
                        SetLoadSitePoint();
                        FinishAndStop(DevCarrierOrderE.取砖指令);
                    }
                    #endregion

                    #region[靠地标取货【下砖轨道取砖】]
                    if (TO_SITE != ZERO_SITE)
                    {
                        if(DevStatus.CurrentSite == TO_SITE)
                        {
                            DevStatus.TargetSite = 0;
                            OnLoading = true;
                        }

                        if (TO_SITE == TargetTrack.rfid_1)
                        {
                            int dif = NowTrack.Type == TrackTypeE.摆渡车_入 ? -270 : 270;
                            DevStatus.CurrentPoint = (ushort)(SimServer.Carrier.GetFerryTrackPos(NowTrack.rfid_1) + dif);
                            SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                            OnLoading = true;
                        }
                    }
                    #endregion

                    #region[靠脉冲取货【储砖轨道取砖】]
                    if (TO_POINT != ZERO_POINT)
                    {
                        //从摆渡车进入轨道的过程
                        if(NowTrack.id != EndTrack.id
                            && DevStatus.CurrentPoint <= EndTrack.limit_point_up)
                        {
                            SetNowTrack(EndTrack, EndTrack.rfid_1);
                        }

                        if (!OnLoading && !LoadFinish)
                        {
                            if (TAKE_STOCK_POINT != 0)
                            {
                                if (DevStatus.CurrentPoint == TAKE_STOCK_POINT)
                                {
                                    DevStatus.TargetPoint = 0;
                                    OnLoading = true;
                                }
                            }
                            else if(DevStatus.CurrentPoint == TO_POINT)
                            {
                                DevStatus.TargetSite = 0;
                                OnLoading = true;
                            }
                        }
                    }
                    #endregion

                    #region[执行取货]

                    if (OnLoading && !LoadFinish)
                    {
                        switch (DevStatus.LoadStatus)
                        {
                            case DevCarrierLoadE.异常:
                                if (mTimer.IsTimeUp("ToLoad", 2))
                                {
                                    SetLoadSitePoint();
                                    DevStatus.LoadStatus = DevCarrierLoadE.有货;
                                }
                                break;
                            case DevCarrierLoadE.无货:
                                if (mTimer.IsTimeUp("ToErrorLoad", 1))
                                {
                                    DevStatus.LoadStatus = DevCarrierLoadE.异常;
                                }
                                break;
                            case DevCarrierLoadE.有货:
                                OnLoading = false;
                                if (END_SITE == ZERO_SITE)
                                {
                                    FinishAndStop(DevCarrierOrderE.取砖指令);
                                }
                                else
                                {
                                    LoadFinish = true;
                                }
                                break;
                        }
                    }
                    #endregion

                    #region[卸货完成,回到结束地标]
                    if (EndTrack != null && LoadFinish && !OnLoading)
                    {
                        if (EndTrack.Type == TrackTypeE.储砖_出 || EndTrack.Type == TrackTypeE.储砖_出入)
                        {
                            if (DevStatus.CurrentPoint == EndTrack.limit_point_up)
                            {
                                FinishAndStop(DevCarrierOrderE.取砖指令);
                            }
                        }
                    }
                    #endregion

                    #region[取完回轨]

                    if (LoadFinish)
                    {
                        if (EndTrack != null
                            && EndTrack.rfid_1 == DevStatus.CurrentSite
                            && TO_POINT != ZERO_POINT
                            && TAKE_STOCK_POINT != ZERO_POINT
                            && END_SITE != ZERO_SITE)
                        {
                            TAKE_STOCK_POINT = ZERO_POINT;
                            TO_POINT = EndTrack.limit_point_up;
                            DevStatus.TargetSite = EndTrack.rfid_1;
                        }

                        if (DevStatus.CurrentPoint == TO_POINT)
                        {
                            FinishAndStop(DevCarrierOrderE.取砖指令);
                            OnLoading = false;
                        }
                    }

                    #endregion
                    break;
                #endregion

                #region[放砖指令]
                case DevCarrierOrderE.放砖指令:

                    #region[下降放货]
                    if (TO_SITE == ZERO_SITE
                        && TO_POINT == ZERO_POINT)
                    {
                        DevStatus.LoadStatus = DevCarrierLoadE.无货;
                        SetUnLoadSitePoint();
                        DevStatus.FinishOrder = DevCarrierOrderE.放砖指令;
                    }
                    #endregion

                    #region[靠地标放货【上砖轨道放砖】]
                    if (TO_SITE != ZERO_SITE)
                    {
                        if (DevStatus.CurrentSite == TO_SITE)
                        {
                            DevStatus.TargetSite = 0;
                            OnUnloading = true;
                        }

                        if (TO_SITE == TargetTrack.rfid_1)
                        {
                            int dif = NowTrack.Type == TrackTypeE.摆渡车_入 ? -270 : 270;
                            DevStatus.CurrentPoint = (ushort)(SimServer.Carrier.GetFerryTrackPos(NowTrack.rfid_1) + dif);
                            SetNowTrack(TargetTrack, TargetTrack.rfid_1);
                            OnUnloading = true;
                        }
                    }
                    #endregion

                    #region[靠脉冲放货【储砖轨道放货】]
                    if (TO_POINT != ZERO_POINT)
                    {
                        if(NowTrack.id != EndTrack.id
                            && DevStatus.CurrentPoint >= EndTrack.limit_point)
                        {
                            SetNowTrack(EndTrack, EndTrack.rfid_1);
                        }

                        if (GIVE_STOCK_POINT != 0)
                        {
                            if (DevStatus.CurrentPoint == GIVE_STOCK_POINT)
                            {
                                DevStatus.TargetPoint = 0;
                                OnUnloading = true;
                            }
                        }
                        else
                        {
                            if(DevStatus.CurrentPoint == TO_POINT)
                            {
                                DevStatus.TargetPoint = 0;
                                OnUnloading = true;
                            }

                            if(EndTrack != null && DevStatus.CurrentPoint >= EndTrack.limit_point)
                            {
                                DevStatus.CurrentSite = EndTrack.rfid_1;
                                SetNowTrack(EndTrack);
                            }
                        }
                    }
                    #endregion

                    #region[执行卸货]
                    if (OnUnloading && !UnloadFinish)
                    {
                        switch (DevStatus.LoadStatus)
                        {
                            case DevCarrierLoadE.异常:
                                if (mTimer.IsTimeUp("ToUnLoad", 2))
                                {
                                    SetUnLoadSitePoint();
                                    DevStatus.LoadStatus = DevCarrierLoadE.无货;
                                }
                                break;
                            case DevCarrierLoadE.无货:
                                if(END_SITE == ZERO_SITE)
                                {
                                    FinishAndStop(DevCarrierOrderE.放砖指令);
                                }
                                else
                                {
                                    UnloadFinish = true;
                                }
                                break;
                            case DevCarrierLoadE.有货:
                                if (mTimer.IsTimeUp("ToErrorUnload", 1))
                                {
                                    DevStatus.LoadStatus = DevCarrierLoadE.异常;
                                }
                                break;
                        }
                    }
                    #endregion

                    #region[卸完回轨]

                    if (UnloadFinish)
                    {
                        if (EndTrack.rfid_1 == DevStatus.CurrentSite
                            && TO_POINT != ZERO_POINT
                            && GIVE_STOCK_POINT != ZERO_POINT
                            && END_SITE != ZERO_SITE)
                        {
                            GIVE_STOCK_POINT = ZERO_POINT;
                            TO_POINT = EndTrack.limit_point;
                        }

                        if(DevStatus.CurrentPoint == TO_POINT)
                        {
                            FinishAndStop(DevCarrierOrderE.放砖指令);
                            OnUnloading = false;
                        }
                    }

                    #endregion

                    break;
                #endregion

                #region[前进倒库]
                case DevCarrierOrderE.前进倒库:
                    //从摆渡车进入轨道的过程
                    if (NowTrack.id != EndTrack.id
                        && DevStatus.CurrentPoint <= EndTrack.limit_point_up)
                    {
                        SetNowTrack(EndTrack, EndTrack.rfid_1);
                    }

                    #region[上砖侧倒库]
                    if (SORT_TYPE == OUT_2_OUT_SORT)
                    {
                        switch (SORT_STEP)
                        {
                            case SimCarrierSortStepE.获取取货库存位置:
                                Stock behindstock = PubMaster.Goods.GetBehindUpSplitTopStock(EndTrack.id);
                                TAKE_STOCK_POINT = behindstock.location;
                                SORT_STEP = SimCarrierSortStepE.前往取货库存位置;
                                break;
                            case SimCarrierSortStepE.前往取货库存位置:
                                if (DevStatus.CurrentPoint == TAKE_STOCK_POINT)
                                {
                                    SORT_STEP = SimCarrierSortStepE.取货中;
                                }
                                break;
                            case SimCarrierSortStepE.取货中:
                                switch (DevStatus.LoadStatus)
                                {
                                    case DevCarrierLoadE.异常:
                                        if (mTimer.IsTimeUp("ToLoad", 2))
                                        {
                                            SetLoadSitePoint();
                                            DevStatus.LoadStatus = DevCarrierLoadE.有货;
                                        }
                                        break;
                                    case DevCarrierLoadE.无货:
                                        if (mTimer.IsTimeUp("ToErrorLoad", 1))
                                        {
                                            DevStatus.LoadStatus = DevCarrierLoadE.异常;
                                        }
                                        break;
                                    case DevCarrierLoadE.有货:
                                        TAKE_STOCK_POINT = ZERO_POINT;
                                        SORT_STEP = SimCarrierSortStepE.取货完成获取卸货位置;
                                        break;
                                }
                                break;
                            case SimCarrierSortStepE.取货完成获取卸货位置:
                                Stock infrontstock = PubMaster.Goods.GetInfrontUpSplitButtonStock(EndTrack.id);
                                if (infrontstock != null)
                                {
                                    ushort safe = PubMaster.Goods.GetStackSafe(0, 0);
                                    GIVE_STOCK_POINT = (ushort)(infrontstock.location - safe);
                                }
                                else
                                {
                                    GIVE_STOCK_POINT = EndTrack.limit_point_up;
                                }
                                SORT_STEP = SimCarrierSortStepE.前往卸货位置;
                                break;
                            case SimCarrierSortStepE.前往卸货位置:
                                if (DevStatus.CurrentPoint == GIVE_STOCK_POINT)
                                {
                                    SORT_STEP = SimCarrierSortStepE.卸货中;
                                }
                                break;
                            case SimCarrierSortStepE.卸货中:
                                switch (DevStatus.LoadStatus)
                                {
                                    case DevCarrierLoadE.异常:
                                        if (mTimer.IsTimeUp("ToUnLoad", 2))
                                        {
                                            SetUnLoadSitePoint();
                                            DevStatus.LoadStatus = DevCarrierLoadE.无货;
                                        }
                                        break;
                                    case DevCarrierLoadE.无货:
                                        GIVE_STOCK_POINT = ZERO_POINT;
                                        SORT_STEP = SimCarrierSortStepE.卸货完成;
                                        break;
                                    case DevCarrierLoadE.有货:
                                        if (mTimer.IsTimeUp("ToErrorUnload", 1))
                                        {
                                            DevStatus.LoadStatus = DevCarrierLoadE.异常;
                                        }
                                        break;
                                }
                                break;
                            case SimCarrierSortStepE.卸货完成:
                                DevStatus.MoveCount++;
                                if(DevStatus.MoveCount < SORT_QTY)
                                {
                                    SORT_STEP = SimCarrierSortStepE.获取取货库存位置;
                                }
                                else
                                {
                                    FinishAndStop(DevCarrierOrderE.前进倒库);
                                }
                                break;
                        }
                    }
                    #endregion

                    #region[倒库]
                    if (SORT_TYPE == IN_2_OUT_SORT)
                    {
                        switch (SORT_STEP)
                        {
                            case SimCarrierSortStepE.获取取货库存位置:
                                Stock intopstock = PubMaster.Goods.GetTrackTopStock(TargetTrack.id);
                                if (intopstock != null)
                                {
                                    TAKE_STOCK_POINT = intopstock.location;
                                    SORT_STEP = SimCarrierSortStepE.前往取货库存位置;
                                }
                                break;
                            case SimCarrierSortStepE.前往取货库存位置:
                                if (DevStatus.CurrentPoint == TAKE_STOCK_POINT)
                                {
                                    SORT_STEP = SimCarrierSortStepE.取货中;
                                }
                                break;
                            case SimCarrierSortStepE.取货中:
                                switch (DevStatus.LoadStatus)
                                {
                                    case DevCarrierLoadE.异常:
                                        if (mTimer.IsTimeUp("ToLoad", 2))
                                        {
                                            SetLoadSitePoint();
                                            DevStatus.LoadStatus = DevCarrierLoadE.有货;
                                        }
                                        break;
                                    case DevCarrierLoadE.无货:
                                        if (mTimer.IsTimeUp("ToErrorLoad", 1))
                                        {
                                            DevStatus.LoadStatus = DevCarrierLoadE.异常;
                                        }
                                        break;
                                    case DevCarrierLoadE.有货:
                                        TAKE_STOCK_POINT = ZERO_POINT;
                                        SORT_STEP = SimCarrierSortStepE.取货完成获取卸货位置;
                                        break;
                                }
                                break;
                            case SimCarrierSortStepE.取货完成获取卸货位置:
                                Stock outbuttomstock = PubMaster.Goods.GetTrackButtomStock(EndTrack.id);
                                if (outbuttomstock != null)
                                {
                                    ushort safe = PubMaster.Goods.GetStackSafe(0, 0);
                                    GIVE_STOCK_POINT = (ushort)(outbuttomstock.location - safe);
                                }
                                else
                                {
                                    GIVE_STOCK_POINT = EndTrack.limit_point_up;
                                }
                                SORT_STEP = SimCarrierSortStepE.前往卸货位置;
                                break;
                            case SimCarrierSortStepE.前往卸货位置:
                                if (DevStatus.CurrentPoint == GIVE_STOCK_POINT)
                                {
                                    SORT_STEP = SimCarrierSortStepE.卸货中;
                                }
                                break;
                            case SimCarrierSortStepE.卸货中:
                                switch (DevStatus.LoadStatus)
                                {
                                    case DevCarrierLoadE.异常:
                                        if (mTimer.IsTimeUp("ToUnLoad", 2))
                                        {
                                            SetUnLoadSitePoint();
                                            DevStatus.LoadStatus = DevCarrierLoadE.无货;
                                        }
                                        break;
                                    case DevCarrierLoadE.无货:
                                        GIVE_STOCK_POINT = ZERO_POINT;
                                        SORT_STEP = SimCarrierSortStepE.卸货完成;
                                        break;
                                    case DevCarrierLoadE.有货:
                                        if (mTimer.IsTimeUp("ToErrorUnload", 1))
                                        {
                                            DevStatus.LoadStatus = DevCarrierLoadE.异常;
                                        }
                                        break;
                                }
                                break;
                            case SimCarrierSortStepE.卸货完成:
                                DevStatus.MoveCount++;
                                if (DevStatus.MoveCount < SORT_QTY)
                                {
                                    SORT_STEP = SimCarrierSortStepE.获取取货库存位置;
                                }
                                else
                                {
                                    FinishAndStop(DevCarrierOrderE.前进倒库);
                                }
                                break;
                        }
                    }
                    #endregion

                    break;
                #endregion

                #region[后退倒库]
                case DevCarrierOrderE.后退倒库:
                    break;
                #endregion

                #region[终止指令]
                case DevCarrierOrderE.终止指令:
                    break;
                #endregion

                #region[异常]
                case DevCarrierOrderE.异常:
                    break;
                #endregion

            }

            #region
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
            #endregion
        }

        #endregion

        #region[更新任务信息]

        /// <summary>
        /// 设置任务信息
        /// </summary>
        /// <param name="cmd"></param>
        internal void SetTaskInfo(CarrierCmd cmd)
        {
            DevStatus.TargetSite = cmd.TargetSite;
            DevStatus.TargetPoint = cmd.TargetPoint;

            OnLoading = false;
            OnUnloading = false;
            LoadFinish = false;
            UnloadFinish = false;

            TO_SITE = cmd.TargetSite;
            TO_POINT = cmd.TargetPoint;
            END_SITE = cmd.FinishSite;
            END_POINT = cmd.FinishPoint;

            SORT_QTY = cmd.SortQty;
            //目标站点
            if (TO_SITE > 0)
            {
                TargetTrack = PubMaster.Track.GetTrackBySite(Device.area, TO_SITE);
            }

            //结束站点
            if (END_SITE > 0)
            {
                if (TargetTrack != null && TargetTrack.IsInTrack(END_SITE))
                {
                    EndTrack = TargetTrack;
                }
                else
                {
                    EndTrack = PubMaster.Track.GetTrackBySite(Device.area, END_SITE);
                }

                //在摆渡车出上执行取货指令
                if (cmd.CarrierOrder == DevCarrierOrderE.取砖指令
                    && TO_POINT != 0
                    && EndTrack != null
                    && (EndTrack.Type == TrackTypeE.储砖_出 || EndTrack.Type == TrackTypeE.储砖_出入))
                {
                    GIVE_STOCK_POINT = 0;
                    Stock stock = PubMaster.Goods.GetTrackTopStock(EndTrack.id);
                    if (stock != null)
                    {
                        TAKE_STOCK_POINT = stock.location;
                    }
                }

                //在摆渡入上执行放砖指令
                if (cmd.CarrierOrder == DevCarrierOrderE.放砖指令
                    && TO_POINT != 0
                    && EndTrack != null
                    && (EndTrack.Type == TrackTypeE.储砖_入 || EndTrack.Type == TrackTypeE.储砖_出入))
                {
                    TAKE_STOCK_POINT = 0;
                    if (PubMaster.Goods.CalculateNextLocation(TransTypeE.下砖任务, 0, EndTrack.id, out ushort stockcount, out ushort location))
                    {
                        GIVE_STOCK_POINT = location;
                    }
                }
            }

            if (cmd.CarrierOrder == DevCarrierOrderE.前进倒库
                || cmd.CarrierOrder == DevCarrierOrderE.后退倒库)
            {
                if(TO_POINT != ZERO_POINT)
                {
                    SORT_TYPE = OUT_2_OUT_SORT;
                    EndTrack = PubMaster.Track.GetTrackBySite((ushort)AreaId, new List<TrackTypeE> { TrackTypeE.储砖_出 }, cmd.CheckTrackCode);
                }
                else
                {
                    SORT_TYPE = IN_2_OUT_SORT;
                    TargetTrack = PubMaster.Track.GetTrackBySite((ushort)AreaId, new List<TrackTypeE> { TrackTypeE.储砖_入 }, cmd.CheckTrackCode);
                    EndTrack = PubMaster.Track.GetTrackBySite((ushort)AreaId, new List<TrackTypeE> { TrackTypeE.储砖_出 }, cmd.CheckTrackCode);
                }
            }
            DevStatus.MoveCount = 0;
            DevStatus.CurrentOrder = cmd.CarrierOrder;
            DevStatus.FinishOrder = DevCarrierOrderE.无;
        }
        #endregion

        #region[取货卸货站点设置]

        /// <summary>
        /// 设置取货站点脉冲
        /// </summary>
        private void SetLoadSitePoint()
        {
            DevStatus.GivePoint = 0;
            DevStatus.GiveSite = 0;
            DevStatus.TakePoint = DevStatus.CurrentPoint;
            DevStatus.TakeSite = DevStatus.CurrentSite;
        }

        /// <summary>
        /// 设置切换站点脉冲
        /// </summary>
        private void SetUnLoadSitePoint()
        {
            DevStatus.TakePoint = 0;
            DevStatus.TakeSite = 0;
            DevStatus.GivePoint = DevStatus.CurrentPoint;
            DevStatus.GiveSite = DevStatus.CurrentSite;
        }

        #endregion

        #region[轨道地标变化/轨道脉冲变化]

        /// <summary>
        /// 更新轨道站点
        /// </summary>
        private void UpdateTrackSite()
        {
            if (TO_SITE == ZERO_SITE) return;
            switch (DevStatus.CurrentSite.CompareTo(TO_SITE))
            {
                case -1:
                    DevStatus.DeviceStatus = DevCarrierStatusE.前进;
                    break;
                case 0:
                    DevStatus.DeviceStatus = DevCarrierStatusE.停止;
                    break;
                case 1:
                    DevStatus.DeviceStatus = DevCarrierStatusE.后退;
                    break;
            }
        }

        /// <summary>
        /// 更新轨道脉冲
        /// </summary>
        private void UpdateTrackPoint()
        {
            if (TO_POINT == ZERO_POINT
                && TAKE_STOCK_POINT == ZERO_POINT
                && GIVE_STOCK_POINT == ZERO_POINT) return;
            int rs = 0;
            if(TAKE_STOCK_POINT != ZERO_POINT)
            {
                rs = DevStatus.CurrentPoint.CompareTo(TAKE_STOCK_POINT);
            }else if(GIVE_STOCK_POINT != ZERO_POINT)
            {
                rs = DevStatus.CurrentPoint.CompareTo(GIVE_STOCK_POINT);
            }else if(TO_POINT != ZERO_POINT)
            {
                rs = DevStatus.CurrentPoint.CompareTo(TO_POINT);
            }

            ushort dif = (ushort)Math.Abs(rs);
            if (rs > 0)
            {
                DevStatus.CurrentPoint -= (ushort)(dif > 50 ? 50 : dif);
                DevStatus.DeviceStatus = DevCarrierStatusE.后退;
            }
            
            if(rs < 0)
            {
                DevStatus.CurrentPoint += (ushort)(dif > 50 ? 50 : dif);
                DevStatus.DeviceStatus = DevCarrierStatusE.前进;
            }
            
            if(rs == 0)
            {
                DevStatus.DeviceStatus = DevCarrierStatusE.停止;
            }
        }

        /// <summary>
        /// 完成任务并且清空信息
        /// </summary>
        /// <param name="finishorder"></param>
        private void FinishAndStop(DevCarrierOrderE finishorder)
        {
            TargetTrack = null;
            EndTrack = null;
            TO_POINT = ZERO_POINT;
            TO_SITE = ZERO_SITE;
            END_POINT = ZERO_POINT;
            END_SITE = ZERO_SITE;
            DevStatus.FinishOrder = finishorder;
            DevStatus.TargetSite = ZERO_SITE;
            DevStatus.TargetPoint = ZERO_POINT;
            DevStatus.DeviceStatus = DevCarrierStatusE.停止;
        }
        #endregion

    }
}
