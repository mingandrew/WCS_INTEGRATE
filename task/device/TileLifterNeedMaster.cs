using enums;
using module.device;
using module.msg;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task.task;
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
        }

        public void Start()
        {
            NeedList.Clear();
            NeedList.AddRange(PubMaster.Mod.TileLifterNeedSql.QueryTileLifterNeedList());

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
                    List<TileLifterNeed> uncreate = NeedList.FindAll(c => !c.finish && c.trans_id == 0)?.OrderBy(c => c.create_time)?.ToList();
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
                    }
                }
                catch (Exception e)
                {
                    mlog.Error(true, "Refresh() - " + e.Message, e);
                }

                Thread.Sleep(2000);
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
                    TileLifterNeed tileLifterNeed = new TileLifterNeed()
                    {
                        device_id = task.ID,
                        track_id = track_id,
                        create_time = DateTime.Now,
                        left = isleft,
                        need_type = task.Type,
                        area_id = task.AreaId,
                    };

                    NeedList.Add(tileLifterNeed);
                    PubMaster.Mod.TileLifterNeedSql.AddTileLifterNeed(tileLifterNeed);
                }
                catch (Exception e)
                {
                    mlog.Error(true, task.Device.name + e.Message, e);
                }
                finally { Monitor.Exit(_obj); }
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
        public void FinishTileLifterNeed(uint devid, uint trackid)
        {
            TileLifterNeed tileneed = NeedList.Find(c => c.device_id == devid && c.track_id == trackid && !c.finish);
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
