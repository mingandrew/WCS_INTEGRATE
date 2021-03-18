using enums;
using module.device;
using module.deviceconfig;
using System;

namespace simtask.task
{
    public class SimTileLifterTask : SimTaskBase
    {
        #region[配置信息]

        private byte FULL_QTY = 50;

        #endregion

        #region[作业逻辑]
        /// <summary>
        /// 上砖机/下砖机 是否作业：上砖/下砖
        /// </summary>
        public bool Working { set; get; }

        /// <summary>
        /// 上一片作业时间
        /// </summary>
        public DateTime? LastPiecesTime { set; get; }
        public byte OnePiecesUsedTime { set; get; }
        public bool IsLeftWork { set; get; } = true;

        #endregion

        #region[属性]
        public byte DevId
        {
            get => DevStatus.DeviceID;
            set => DevStatus.DeviceID = value;
        }
        public bool IsNeed_1
        {
            get => DevStatus.Need1;
            set => DevStatus.Need1 = value;
        }

        public bool IsNeed_2
        {
            get => DevStatus.Need2;
            set => DevStatus.Need2 = value;
        }

        public bool IsLoad_1
        {
            get => DevStatus.Load1;
            set => DevStatus.Load1 = value;
        }

        public bool IsLoad_2
        {
            get => DevStatus.Load2;
            set => DevStatus.Load2 = value;
        }

        public bool IsInvo_1
        {
            get => DevStatus.Involve1 ;
            set => DevStatus.Involve1 = value;
        }
        
        public bool IsInvo_2
        {
            get => DevStatus.Involve2;
            set => DevStatus.Involve2 = value;
        }


        #endregion

        #region[构造/启动/停止]
        public DevTileLifter DevStatus { set; get; }
        public ConfigTileLifter DevConfig { set; get; }

        public SimTileLifterTask()
        {
            DevStatus = new DevTileLifter();
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
        #endregion

        #region[计算满砖/空砖]
        public void StartWorking()
        {
            if (!Working)
            {
                LastPiecesTime = DateTime.Now;
                Working = true;
            }
        }

        public void StopWorking()
        {
            Working = false;
        }

        public void CheckFullOrEmpty()
        {
            if (Working)
            {
                if(Type == DeviceTypeE.上砖机)
                {
                    if(DevStatus.Site1Qty == 0)
                    {
                        DevStatus.Load1 = false;
                        DevStatus.Need1 = true;
                    }

                    if(Device.Type2 == DeviceType2E.双轨 && DevStatus.Site2Qty == 0)
                    {
                        DevStatus.Load2 = false;
                        DevStatus.Need2 = true;
                    }

                    if (Device.Type2 == DeviceType2E.单轨 
                            && DevStatus.Load1 
                            && !DevStatus.Involve1)
                    {
                        CheckTimeToAct();
                    }else
                    {
                        if (IsInvo_1 && IsInvo_2) return;

                        if (IsLeftWork
                            && !DevStatus.Load1
                            && DevStatus.Need1
                            && DevStatus.Load2
                            && !DevStatus.Involve2)
                        {
                            DevStatus.Site2Qty = FULL_QTY;
                            IsLeftWork = false;
                            return;
                        }

                        if (!IsLeftWork
                            && !DevStatus.Load2
                            && DevStatus.Need2
                            && DevStatus.Load1
                            && !DevStatus.Involve1)
                        {
                            DevStatus.Site1Qty = FULL_QTY;
                            IsLeftWork = true;
                            return;
                        }

                        if (!IsLeftWork && !DevStatus.Involve1 && DevStatus.Load1 && DevStatus.Involve2)
                        {
                            DevStatus.Site1Qty = FULL_QTY;
                            IsLeftWork = true;
                            return;
                        }

                        CheckTimeToAct();
                    }
                }
                else if(Type == DeviceTypeE.下砖机)
                {
                    DevStatus.Goods1 = DevStatus.Need1 ? DevStatus.SetGoods : 0;
                    DevStatus.Goods2 = DevStatus.Need2 ? DevStatus.SetGoods : 0;

                    DevStatus.Load1 = DevStatus.Site1Qty > 0;
                    DevStatus.Load2 = DevStatus.Site2Qty > 0;

                    if (DevStatus.FullQty == DevStatus.Site1Qty)
                    {
                        if (!DevStatus.Involve1)
                        {
                            DevStatus.Load1 = true;
                            DevStatus.Need1 = true;
                        }
                    }

                    if (DevStatus.FullQty == DevStatus.Site2Qty)
                    {
                        if (!DevStatus.Involve2)
                        {
                            DevStatus.Goods2 = DevStatus.SetGoods;
                            DevStatus.Load2 = true;
                            DevStatus.Need2 = true;
                        }
                    }

                    if (Device.Type2 == DeviceType2E.单轨 && !DevStatus.Load1 && !DevStatus.Involve1)
                    {
                        CheckTimeToAct();
                    }else
                    {
                        if (IsInvo_1 && IsInvo_2) return;

                        if (IsLeftWork 
                            && DevStatus.Load1 
                            && DevStatus.Need1
                            && !DevStatus.Load2
                            && !DevStatus.Involve2)
                        {
                            DevStatus.Site2Qty = 0;
                            IsLeftWork = false;
                            return;
                        }

                        if(!IsLeftWork 
                            && DevStatus.Load2
                            && DevStatus.Need2
                            && !DevStatus.Load1
                            && !DevStatus.Involve1)
                        {
                            DevStatus.Site1Qty = 0;
                            IsLeftWork = true;
                            return;
                        }

                        if(!IsLeftWork && !DevStatus.Involve1 && !DevStatus.Load1 && DevStatus.Involve2)
                        {
                            DevStatus.Site1Qty = 0;
                            IsLeftWork = true;
                            return;
                        }

                        CheckTimeToAct();
                    }
                }
            }
        }
        private void CheckTimeToAct()
        {
            if (LastPiecesTime is DateTime lasttime)
            {
                if ((DateTime.Now - lasttime).TotalSeconds > OnePiecesUsedTime)
                {
                    LastPiecesTime = DateTime.Now;
                    if (IsLeftWork)
                    {
                        DoTileSite1CountRemove();
                    }
                    else
                    {
                        DoTileSite2CountRemove();
                    }
                }
            }
        }

        private void DoTileSite1CountRemove()
        {
            if (Type == DeviceTypeE.上砖机)
            {
                if (DevStatus.Site1Qty > 0)
                {
                    DevStatus.Site1Qty--;
                }
            }
            else
            {
                if (DevStatus.Site1Qty < DevStatus.FullQty)
                {
                    DevStatus.Site1Qty++;
                }
            }
        }

        private void DoTileSite2CountRemove()
        {
            if (Type == DeviceTypeE.上砖机)
            {
                if (DevStatus.Site2Qty > 0)
                {
                    DevStatus.Site2Qty--;
                }
            }
            else
            {
                if (DevStatus.Site2Qty < DevStatus.FullQty)
                {
                    DevStatus.Site2Qty++;
                }
            }
        }

        /// <summary>
        /// 初始化后配置砖机
        /// </summary>
        internal void SetupTile()
        {
            if(Type == DeviceTypeE.上砖机)
            {
                DevStatus.FullQty = 30;
                OnePiecesUsedTime = 3;
            }
            else
            {
                DevStatus.FullQty = 20;
                OnePiecesUsedTime = 3;
            }
            DevStatus.OperateMode = DevOperateModeE.自动;
            DevStatus.WorkMode = DevConfig.WorkMode;
            DevStatus.ShiftStatus = TileShiftStatusE.复位;
            DevStatus.SetGoods = DevConfig.goods_id;
            DevStatus.SetLevel = DevConfig.level;
        }

        internal bool IsTrackLoad(uint trackid)
        {
            if (DevConfig.left_track_id == trackid)
            {
                return IsLoad_1 && IsNeed_1;
            }

            if (DevConfig.right_track_id == trackid)
            {
                return IsLoad_2 && IsNeed_2;
            }
            return false;
        }

        internal bool IsTrackUnLoad(uint trackid)
        {
            if (DevConfig.left_track_id == trackid)
            {
                return !IsLoad_1 && IsNeed_1;
            }

            if (DevConfig.right_track_id == trackid)
            {
                return !IsLoad_2 && IsNeed_2;
            }
            return false;
        }

        internal bool IsWorkingTrack(uint trackid)
        {
            return DevConfig.left_track_id == trackid || DevConfig.right_track_id == trackid;
        }

        internal void DoTrackUnload(uint trackid)
        {
            //左轨道
            if(DevConfig.left_track_id == trackid)
            {
                DevStatus.Load1 = false;
                DevStatus.Need1 = false;
                if (Device.Type2 == DeviceType2E.单轨)
                {
                    DevStatus.Site1Qty = 0;
                }
            }

            //右轨道
            if(DevConfig.right_track_id == trackid)
            {
                DevStatus.Load2 = false;
                DevStatus.Need2 = false;
            }
        }

        internal void DoTrackLoad(uint trackid)
        {
            //左轨道
            if (DevConfig.left_track_id == trackid)
            {
                DevStatus.Load1 = true;
                DevStatus.Need1 = false;
                if (Device.Type2 == DeviceType2E.单轨)
                {
                    DevStatus.Site1Qty = FULL_QTY;
                }
            }

            //右轨道
            if (DevConfig.right_track_id == trackid)
            {
                DevStatus.Load2 = true;
                DevStatus.Need2 = false;
            }
        }

        #endregion
    }
}
