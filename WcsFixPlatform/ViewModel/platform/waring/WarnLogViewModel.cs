using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using module;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class WarnLogViewModel : ViewModelBase
    {
        public WarnLogViewModel()
        {
            STARTDATE = DateTime.Now;
            STOPDATE = DateTime.Now;

            lastquerytime = DateTime.Now.AddSeconds(-10);
        }

        #region[字段]
        private DateTime lastquerytime;
        private DateTime? startdate;
        private DateTime? stopdate;
        private ComboBoxItem warntypecb;

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

        public ComboBoxItem WarnTypeCB
        {
            get => warntypecb;
            set => Set(ref warntypecb, value);
        }

        public ObservableCollection<Warning> LogList { get; set; } = new ObservableCollection<Warning>();


        #endregion

        #region[命令]

        /// <summary>
        /// 查询生产记录
        /// </summary>
        public RelayCommand<string> SearchConsumelogCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SearchConsumelog)).Value;

        #endregion

        #region[方法]

        /// <summary>
        /// 查询数据库
        /// </summary>
        private void SearchConsumelog(string tag)
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

            if(int.TryParse(tag, out int type))
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

            if (STARTDATE is DateTime start && STOPDATE is DateTime stop)
            {
                if ((stop - start).TotalDays > 14)
                {
                    Growl.Warning("查询日期不能超过两周");
                    return;
                }
                byte wtype = 255;
                if(WarnTypeCB != null && byte.TryParse(WarnTypeCB.Tag.ToString(), out wtype))
                {

                }

                List<Warning> list = PubMaster.Mod.WarnSql.QueryWarningLog(wtype, start, stop);

                LogList.Clear();
                if (list.Count > 0)
                {
                    foreach (Warning md in list)
                    {
                        if (md.type >= 100 && md.track_id != 0)
                        {
                            md.track_id = 0;
                        }
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
