using enums;
using module.device;
using module.msg;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task.task;
using tool.appconfig;
using tool.mlog;

namespace task.device
{
    public class TileLifterNeedMaster
    {
        #region[字段]
        private MsgAction mMsg;
        private List<TileLifterNeed> NeedList { set; get; } // 需求列表
        private readonly object _obj;
        private bool Refreshing = true;
        private Thread _mRefresh;
        private Log mlog;
        #endregion

        #region[构造/初始化]
        public TileLifterNeedMaster()
        {
            _obj = new object();
            NeedList = new List<TileLifterNeed>();
            mMsg = new MsgAction();
            mlog = (Log)new LogFactory().GetLog("TileLifterNeed", false);

            TileNeedRefreshTime = GlobalWcsDataConfig.BigConifg.TileNeedRefreshTime;
        }

        private int TileNeedRefreshTime { set; get; }

        public void Start()
        {
            NeedList.Clear();
            NeedList.AddRange(PubMaster.Mod.TileLifterNeedSql.QueryTileLifterNeedList());

            SortNeedList();

            //开始 - 循环需求列表的 线程
            if (_mRefresh == null || !_mRefresh.IsAlive || _mRefresh.ThreadState == ThreadState.Aborted)
            {
                _mRefresh = new Thread(Refresh)
                {
                    IsBackground = true
                };
            }

            _mRefresh.Start();
        }

        private int uncreateneedcount;
        /// <summary>
        /// 循环需求列表
        /// </summary>
        private void Refresh()
        {
            Thread.Sleep(5000);
            while (Refreshing)
            {
                try
                {
                    //所有没有生成任务的需求，按时间升序排序
                    //List<TileLifterNeed> uncreate = NeedList.FindAll(c => !c.finish && c.trans_id == 0)?.OrderBy(c => c.create_time)?.ToList();
                    List<TileLifterNeed> uncreateDown = NeedList.FindAll(c => !c.finish && c.trans_id == 0 && c.need_type == DeviceTypeE.下砖机);
                    List<TileLifterNeed> uncreateUp = NeedList.FindAll(c => !c.finish && c.trans_id == 0 && c.need_type == DeviceTypeE.上砖机);
                    //分开上砖需求和下砖需求，互不干扰
                    CreateStockTrans(uncreateDown, out int downcount);
                    CreateStockTrans(uncreateUp, out int upcount);
                    uncreateneedcount = downcount + upcount;
                }
                catch (Exception e)
                {
                    mlog.Error(true, "Refresh() - " + e.Message, e);
                }

                if (uncreateneedcount > 0)
                {
                    Thread.Sleep(TileNeedRefreshTime);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// 尝试根据需求列表生成任务
        /// </summary>
        /// <param name="uncreate"></param>
        /// <param name="uncreateneedcount"></param>
        private void CreateStockTrans(List<TileLifterNeed> uncreate, out int uncreateneedcount)
        {
            uncreateneedcount = 0;
            if (uncreate != null && uncreate.Count != 0)
            {
                foreach (TileLifterNeed nd in uncreate)
                {
                    //如果需求所在的砖机离线/没需求了，删除需求
                    TileLifterTask needtask = PubTask.TileLifter.GetTileLifter(nd.device_id);
                    if (!PubTask.TileLifter.CheckTileLifterStatusWithNeed(needtask, nd.left, out string result))
                    {
                        RemoveTileLifterNeed(nd.device_id, nd.track_id);
                        continue;
                    }
                    //让需求尝试生成任务，可以生成就跳出当前循环
                    PubTask.TileLifter.CheckAndCreateStockTrans(needtask, nd);
                    if (nd.trans_id != 0)
                    {
                        break;
                    }
                }
                uncreateneedcount = uncreate.Count;
            }
        }

        public void Stop()
        {
            Refreshing = false;
            _mRefresh?.Abort(); //停止循环需求列表的线程
        }
        #endregion

        #region[增改删]
        /// <summary>
        /// 判断能否加需求
        /// </summary>
        /// <param name="task"></param>
        /// <param name="isneed"></param>
        /// <param name="isleft"></param>
        /// <param name="trackid"></param>
        private void CheckTileNeed(TileLifterTask task, bool isleft, uint trackid)
        {
            //判断当前工位的需求是否存在没有完成任务的需求
            List<TileLifterNeed> needs = NeedList.FindAll(c => c.device_id == task.ID && c.track_id == trackid && !c.finish 
                                                                && c.area_id == task.AreaId && c.need_type == task.Type);

            if(needs.Count == 0)
            {
                IsAddTileLifterNeed(task, isleft, trackid);
            }
        }

        /// <summary>
        /// 检查砖机需求
        /// </summary>
        /// <param name="task"></param>
        public void CheckTileLifterNeed(TileLifterTask task)
        {
            try
            {
                //判断砖机是否有需求，是否要插入/删除需求列表里
                if (task.IsNeed_1)
                {
                    CheckTileNeed(task, true, task.DevConfig.left_track_id);
                }

                if (task.IsTwoTrack && task.DevConfig.right_track_id != 0 && task.IsNeed_2)
                {
                    CheckTileNeed(task, false, task.DevConfig.right_track_id);
                }
            }
            catch (Exception e)
            {
                mlog.Error(true, task.Device.name + e.Message, e);
            }
        }

        /// <summary>
        /// 添加砖机需求
        /// </summary>
        /// <param name="task"></param>
        /// <param name="isleft"></param>
        /// <param name="track_id"></param>
        private void IsAddTileLifterNeed(TileLifterTask task, bool isleft, uint track_id)
        {
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    //第三优先：单需求的砖机（不分里外侧）
                    ushort pri = 99;
                    bool isUpdate = false;
                    uint ncount = (uint)NeedList.Count(c => c.device_id == task.ID && !c.finish && c.trans_id == 0);
                    if (ncount != 0 && task.Type == DeviceTypeE.下砖机)
                    {
                        //第一优先：双需求的远离摆渡的砖机
                        if (task.HaveBrother)
                        {
                            pri = 1;
                        }
                        //第二优先：双需求的靠近摆渡的砖机
                        else
                        {
                            pri = 2;
                        }
                        isUpdate = true;
                    }
                    
                    TileLifterNeed tileLifterNeed = new TileLifterNeed()
                    {
                        device_id = task.ID,
                        track_id = track_id,
                        create_time = DateTime.Now,
                        left = isleft,
                        need_type = task.Type,
                        area_id = task.AreaId,
                        prior = pri,
                    };

                    NeedList.Add(tileLifterNeed);
                    PubMaster.Mod.TileLifterNeedSql.AddTileLifterNeed(tileLifterNeed);

                    if (isUpdate)
                    {
                        UpdateTileNeedPrior(task.ID, pri);
                    }
                }
                catch (Exception e)
                {
                    mlog.Error(true, task.Device.name + e.Message, e);
                }
                finally { Monitor.Exit(_obj); }
            }
        }
        
        /// <summary>
        /// 更新需求的优先级
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="pri"></param>
        public void UpdateTileNeedPrior(uint devid, ushort pri)
        {
            try
            {
                List<TileLifterNeed> updateNeeds = NeedList.FindAll(c => c.device_id == devid && !c.finish && c.trans_id == 0);
                if (updateNeeds != null)
                {
                    foreach (TileLifterNeed need in updateNeeds)
                    {
                        need.prior = pri;
                        PubMaster.Mod.TileLifterNeedSql.EditTileLifterNeed(need, TileNeedStatusE.Prior);
                    }
                    SortNeedList();
                }
            }
            catch (Exception e)
            {
                mlog.Error(true, devid + "号砖机 - " + e.Message, e);
            }
        }
        
        /// <summary>
        /// 根据优先级-生成时间来排序需求列表
        /// </summary>
        private void SortNeedList()
        {
            if (NeedList != null && NeedList.Count > 0)
            {
                NeedList.Sort(
                    (x, y) =>
                    {
                        if (x.prior == y.prior)
                        {
                            if (x.create_time is DateTime xc && y.create_time is DateTime yc)
                            {
                                return xc.CompareTo(yc);
                            }
                        }
                        return x.prior.CompareTo(y.prior);
                    }
                );
            }
        }
        
        //生成任务时,更新需求的任务信息
        public void UpdateTileLifterNeedTrans(uint devid, uint trackid, DateTime? ctime, uint transid)
        {
            //if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            //{
            try
            {
                TileLifterNeed tileneed = NeedList.Find(c => c.device_id == devid && c.track_id == trackid && c.trans_id == 0);
                if (tileneed != null)
                {
                    tileneed.trans_id = transid;
                    tileneed.trans_create_time = ctime;
                    PubMaster.Mod.TileLifterNeedSql.EditTileLifterNeed(tileneed, TileNeedStatusE.Trans);
                }
            }
            catch (Exception e)
            {
                mlog.Error(true, devid + "号砖机 - " + e.Message, e);
            }
            //finally { Monitor.Exit(_obj); }
            //}
        }

        //完成任务时,更新需求的完成任务状态
        public void FinishTileLifterNeed(uint transid)
        {
            TileLifterNeed tileneed = NeedList.Find(c => c.trans_id == transid);
            if (tileneed != null)
            {
                tileneed.finish = true;
                PubMaster.Mod.TileLifterNeedSql.EditTileLifterNeed(tileneed, TileNeedStatusE.Finish);
            }
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    NeedList.RemoveAll(c => c.finish);
                }
                finally { Monitor.Exit(_obj); }
            }
        }
        
        //删除指定工位的需求
        public void RemoveTileLifterNeed(uint devid, uint trackid)
        {
            //删除
            if (Monitor.TryEnter(_obj, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    NeedList.RemoveAll(c => c.device_id == devid && c.track_id == trackid && c.trans_id == 0 && !c.finish);
                    PubMaster.Mod.TileLifterNeedSql.DeleteTileLifterNeed(devid, trackid);
                }
                catch (Exception e)
                {
                    //记录日志
                    mlog.Error(true, devid + "-" + trackid + "-" + e.Message, e);
                }
                finally { Monitor.Exit(_obj); }
            }
        }

        #endregion

        #region[判断/获取属性/排序]
        
        #endregion

        #region[获取对象]

        #endregion
    }
}
