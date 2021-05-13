using enums;
using module.device;
using module.deviceconfig;
using resource;
using System;
using tool.appconfig;

namespace simtask.task
{
    public class SimTileLifterTask : SimTaskBase
    {
        #region[配置信息]

        public byte FULL_QTY = 30;

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
        public bool IsOneTrack { set; get; } = false;

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

        /// <summary>
        /// 是否是有砖
        /// </summary>
        public bool IsFull_1
        {
            get => DevStatus.LoadStatus1 == DevLifterLoadE.满砖;
        }

        /// <summary>
        /// 是否是空
        /// </summary>
        public bool IsEmpty_1
        {
            get => DevStatus.LoadStatus1 == DevLifterLoadE.无砖;
        }

        /// <summary>
        /// 是否是空投
        /// </summary>
        public bool IsGood_1
        {
            get => DevStatus.LoadStatus1 == DevLifterLoadE.有砖;
        }

        /// <summary>
        /// 是否是有砖
        /// </summary>
        public bool IsFull_2
        {
            get => DevStatus.LoadStatus2 == DevLifterLoadE.满砖;
        }

        /// <summary>
        /// 是否是空
        /// </summary>
        public bool IsEmpty_2
        {
            get => DevStatus.LoadStatus2 == DevLifterLoadE.无砖;
        }

        /// <summary>
        /// 是否是空投
        /// </summary>
        public bool IsGood_2
        {
            get => DevStatus.LoadStatus2 == DevLifterLoadE.有砖;
        }


        public bool IsInvo_1
        {
            get => DevStatus.Involve1;
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

        public SimTileLifterTask() : base()
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

                bool usefull = PubMaster.Dic.IsSwitchOnOff(DicTag.UseTileFullSign);
                if (Type == DeviceTypeE.上砖机)
                {
                    if (DevConfig.left_track_id != 0 && DevStatus.Site1Qty == 0 && !IsInvo_1)
                    {
                        DevStatus.LoadStatus1 = DevLifterLoadE.无砖;
                        DevStatus.Need1 = true;
                    }

                    if (DevConfig.right_track_id != 0 && DevStatus.Site2Qty == 0 && !IsInvo_2)
                    {
                        DevStatus.LoadStatus2 = DevLifterLoadE.无砖;
                        DevStatus.Need2 = true;
                    }

                    if (IsOneTrack)
                    {
                        if ((IsGood_1 || IsFull_1 || IsGood_2 || IsFull_2) && !IsInvo_1 && !IsInvo_2)
                        {
                            CheckTimeToAct();
                        }

                        return;
                    }
                    else
                    {
                        if (IsInvo_1 && IsInvo_2) return;
                        if (IsLeftWork
                            && IsEmpty_1
                            && IsNeed_1
                            && (IsGood_2 || IsFull_2)
                            && !IsInvo_2)
                        {
                            IsLeftWork = false;
                            return;
                        }

                        if (!IsLeftWork
                            && IsEmpty_2
                            && IsNeed_2
                            && (IsGood_1 || IsFull_1)
                            && !IsInvo_1)
                        {
                            IsLeftWork = true;
                            return;
                        }

                        if (!IsLeftWork && !DevStatus.Involve1 && IsFull_1 && DevStatus.Involve2)
                        {
                            IsLeftWork = true;
                            return;
                        }

                        CheckTimeToAct();
                    }
                }
                else if (Type == DeviceTypeE.下砖机)
                {
                    DevStatus.Goods1 = DevStatus.Need1 ? DevStatus.SetGoods : 0;
                    DevStatus.Goods2 = DevStatus.Need2 ? DevStatus.SetGoods : 0;

                    if(DevStatus.LoadStatus1 == DevLifterLoadE.无砖 && DevStatus.Site1Qty > 0)
                    {
                        DevStatus.LoadStatus1 = DevLifterLoadE.有砖;
                    }

                    if (DevStatus.LoadStatus2 == DevLifterLoadE.无砖 && DevStatus.Site2Qty > 0)
                    {
                        DevStatus.LoadStatus2 = DevLifterLoadE.有砖;
                    }

                    if (DevStatus.FullQty == DevStatus.Site1Qty)
                    {
                        if (!DevStatus.Involve1)
                        {
                            DevStatus.Need1 = true;
                        }
                    }

                    if (DevStatus.FullQty == DevStatus.Site2Qty)
                    {
                        if (!DevStatus.Involve2)
                        {
                            DevStatus.Need2 = true;
                        }
                    }

                    if (IsOneTrack)
                    {
                        if ((!IsFull_1 || !IsFull_2) && !IsInvo_1 && !IsInvo_2)
                        {
                            CheckTimeToAct();
                        }
                        return;
                    }
                    else
                    {
                        if (IsInvo_1 && IsInvo_2) return;

                        if (IsLeftWork
                            && (IsGood_1 || IsFull_1)
                            && IsNeed_1
                            && !IsGood_2
                            && !IsInvo_2)
                        {
                            DevStatus.Site2Qty = 0;
                            IsLeftWork = false;
                            return;
                        }

                        if (!IsLeftWork
                            && (IsGood_2 || IsFull_2)
                            && IsNeed_2
                            && !IsGood_1
                            && !IsInvo_1)
                        {
                            DevStatus.Site1Qty = 0;
                            IsLeftWork = true;
                            return;
                        }

                        if (!IsLeftWork && !DevStatus.Involve1 && IsGood_1 && DevStatus.Involve2)
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
                if (!IsNeed_1 && DevStatus.Site1Qty > 0)
                {
                    DevStatus.Site1Qty--;
                }
            }
            else
            {
                if (!IsNeed_1 && DevStatus.Site1Qty < DevStatus.FullQty)
                {
                    DevStatus.Site1Qty++;
                }
            }
        }

        private void DoTileSite2CountRemove()
        {
            if (Type == DeviceTypeE.上砖机)
            {
                if (!IsNeed_2 && DevStatus.Site2Qty > 0)
                {
                    DevStatus.Site2Qty--;
                }
            }
            else
            {
                if (!IsNeed_2 && DevStatus.Site2Qty < DevStatus.FullQty)
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
            if (Type == DeviceTypeE.上砖机)
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

            if (DevConfig.left_track_id != 0)
            {
                IsLeftWork = true;
            }

            if (DevConfig.left_track_id == 0 && DevConfig.right_track_id != 0)
            {
                IsLeftWork = false;
            }

            if (DevConfig.left_track_id == 0 || DevConfig.right_track_id == 0)
            {
                IsOneTrack = true;
            }
        }


        internal bool IsWorkingTrack(uint trackid)
        {
            return DevConfig.left_track_id == trackid || DevConfig.right_track_id == trackid;
        }

        #endregion

        #region[模拟配置文件初始化]

        internal void SetUpSimulate(SimTileLifter sim)
        {
            if (sim == null) return;
            Working = sim.Working;
            LastPiecesTime = sim.LastPiecesTime;
            OnePiecesUsedTime = sim.OnePiecesUsedTime;
            IsLeftWork = sim.IsLeftWork;
            IsNeed_1 = sim.IsNeed_1;
            IsNeed_2 = sim.IsNeed_2;
            DevStatus.LoadStatus1 = sim.LoadStatus1;
            DevStatus.LoadStatus2 = sim.LoadStatus2;
            IsInvo_1 = sim.IsInvo_1;
            IsInvo_2 = sim.IsInvo_2;
            DevStatus.Site1Qty = sim.Site1Qty ;
            DevStatus.Site2Qty = sim.Site2Qty;
            DevStatus.Goods1 = sim.Good1;
            DevStatus.Goods2 = sim.Good2;
            DevStatus.ShiftStatus = sim.ShiftStatus;
            DevStatus.ShiftAccept = sim.ShiftAccept;
            DevStatus.WorkMode = sim.WorkMode;
            DevStatus.OperateMode = sim.OperateMode;
            DevStatus.NeedSytemShift = sim.NeedSystemShift;
            DevStatus.BackupShiftDev = sim.BackUpShiftDev;
        }

        internal SimTileLifter SaveSimulate()
        {
            SimTileLifter sim = new SimTileLifter();
            sim.DevId = ID;
            sim.Working = Working;
            sim.LastPiecesTime = LastPiecesTime;
            sim.OnePiecesUsedTime = OnePiecesUsedTime;
            sim.IsLeftWork = IsLeftWork;
            sim.IsNeed_1 = IsNeed_1;
            sim.IsNeed_2 = IsNeed_2;
            sim.LoadStatus1 = DevStatus.LoadStatus1;
            sim.LoadStatus2 = DevStatus.LoadStatus2;
            sim.IsInvo_1 = IsInvo_1;
            sim.IsInvo_2 = IsInvo_2;
            sim.Site1Qty = DevStatus.Site1Qty;
            sim.Site2Qty = DevStatus.Site2Qty;
            sim.Good1 = DevStatus.Goods1;
            sim.Good2 = DevStatus.Goods2;
            sim.ShiftStatus = DevStatus.ShiftStatus;
            sim.ShiftAccept = DevStatus.ShiftAccept;
            sim.WorkMode = DevStatus.WorkMode;
            sim.OperateMode = DevStatus.OperateMode;
            sim.NeedSystemShift = DevStatus.NeedSytemShift;
            sim.BackUpShiftDev = DevStatus.BackupShiftDev;
            
            return sim;
        }
        #endregion

        #region[其他方法]

        public void SetSite1Status(uint gid, byte qty, DevLifterLoadE status)
        {
            DevStatus.Goods1 = gid;
            DevStatus.Site1Qty = qty;
            DevStatus.LoadStatus1 = status;
        }

        public void SetSite2Status(uint gid, byte qty, DevLifterLoadE status)
        {
            DevStatus.Goods2 = gid;
            DevStatus.Site2Qty = qty;
            DevStatus.LoadStatus2 = status;
        }

        #endregion
    }
}
