using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module;
using module.device;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using wcs.Dialog;

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
        private Device selectdevice;
        private string selectdevname;
        #endregion

        #region[属性]
        public string SelectDeviceName
        {
            get => selectdevname;
            set => Set(ref selectdevname, value);
        }

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
        public RelayCommand DevSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(DevSelected)).Value;

        #endregion

        #region[方法]


        private async void DevSelected()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                   .Initialize<DeviceSelectViewModel>((vm) =>
                   {
                       vm.FilterArea = false;
                       vm.AreaId = 0;
                       vm.LineId = 0;
                       vm.SetSelectType(DeviceTypeE.运输车);
                   }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                selectdevice = dev;
                SelectDeviceName = dev.name;
            }
            else
            {
                selectdevice = null;
                SelectDeviceName = "";
            }
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        private void SearchConsumelog(string tag)
        {
            if ((DateTime.Now - lastquerytime).TotalMilliseconds < 950)
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
                    case 2://半小时
                        DateTime today = DateTime.Now;
                        today = today.AddHours(-0.5);
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, today.Hour, today.Minute, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                        break;
                    case 3: //一个钟
                        today = DateTime.Now;
                        today = today.AddHours(-1);
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, today.Hour, today.Minute, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                        break;
                    case 4://两个钟
                        today = DateTime.Now;
                        today = today.AddHours(-2);
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, today.Hour, today.Minute, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                        break;

                    case 5://今天
                        today = DateTime.Now;
                        STARTDATE = new DateTime(today.Year, today.Month, today.Day, 0, 00, 00);
                        STOPDATE = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                        break;

                    case 6://昨天
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

                List<Warning> list = PubMaster.Mod.WarnSql.QueryWarningLog(wtype, start, stop, selectdevice?.id ?? 0);

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
                    Growl.Success("查询成功！");
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
