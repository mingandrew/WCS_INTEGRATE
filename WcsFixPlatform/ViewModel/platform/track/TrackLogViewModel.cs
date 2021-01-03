using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using module.area;
using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class TrackLogViewModel : ViewModelBase
    {
        public TrackLogViewModel()
        {
            STARTDATE = DateTime.Now;
            STOPDATE = DateTime.Now;

            lastquerytime = DateTime.Now.AddSeconds(-10);
            AreaList = new List<Area>();
            Area all = new Area()
            {
                id = 0,
                name = "全部"
            };
            AreaList.Add(all);
            AreaList.AddRange(PubMaster.Area.GetAreaList());
            SelectArea = all;
        }

        #region[字段]
        private DateTime lastquerytime;
        private DateTime? startdate;
        private DateTime? stopdate;

        private Area selectarea;
        private List<Area> areas;
        private ComboBoxItem warntypecb;
        private string mincount;
        #endregion

        #region[属性]

        public DateTime? STARTDATE
        {
            get => startdate;
            set => Set(ref startdate, value);
        }

        public DateTime? STOPDATE
        {
            get => stopdate;
            set => Set(ref stopdate, value);
        }
        public List<Area> AreaList
        {
            get => areas;
            set => Set(ref areas, value);
        }
        public Area SelectArea
        {
            get => selectarea;
            set => Set(ref selectarea, value);
        }

        public ComboBoxItem WarnTypeCB
        {
            get => warntypecb;
            set => Set(ref warntypecb, value);
        }

        public string MaxCount
        {
            get => mincount;
            set => Set(ref mincount, value);
        }

        public ObservableCollection<TrackLog> LogList { get; set; } = new ObservableCollection<TrackLog>();


        #endregion

        #region[命令]

        /// <summary>
        /// 查询生产记录
        /// </summary>
        public RelayCommand<string> SearchlogCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(Searchloglog)).Value;

        #endregion

        #region[方法]

        /// <summary>
        /// 查询数据库
        /// </summary>
        private void Searchloglog(string tag)
        {
            if ((DateTime.Now - lastquerytime).TotalSeconds < 3)
            {
                Growl.Warning("请不要频繁刷新!");
                return;
            }

            lastquerytime = DateTime.Now;

            if (STARTDATE == null)
            {
                Growl.Warning("请选择开始时间");
                return;
            }

            if (STOPDATE == null)
            {
                Growl.Warning("请选择结束时间");
                return;
            }

            if (SelectArea == null)
            {
                Growl.Warning("请选择区域");
                return;
            }

            int wtype = 0;
            if (WarnTypeCB == null || !int.TryParse(WarnTypeCB.Tag.ToString(), out wtype))
            {
                Growl.Warning("请选择类型");
                return;
            }

            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1:
                        break;

                    case 2://今天
                        DateTime today = DateTime.Now;
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, 0, 00, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                        break;

                    case 3://昨天
                        today = DateTime.Now;
                        today = today.AddDays(-1);
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, 0, 00, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                        break;
                }
            }
            int countfilter = -1;
            if (MaxCount != null && MaxCount.Trim().Length > 0)
            {

                MaxCount = MaxCount.Trim();
                if (!int.TryParse(MaxCount, out countfilter))
                {
                    Growl.Warning("请输入正确的数字或空");
                    return;
                }
            }

            if (STARTDATE is DateTime start && STOPDATE is DateTime stop)
            {
                if ((stop - start).TotalDays > 14)
                {
                    Growl.Warning("查询日期不能超过两周");
                    return;
                }

                List<TrackLog> list = PubMaster.Mod.TraSql.QueryTrackLog(wtype, SelectArea.id, start, stop, countfilter);

                LogList.Clear();
                if (list.Count > 0)
                {
                    foreach (TrackLog md in list)
                    {
                        LogList.Add(md);
                    }
                }
                else
                {
                    Growl.Warning("查询不到信息！");
                }
            }
        }
        #endregion

    }
}
