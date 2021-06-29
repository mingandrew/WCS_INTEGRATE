using enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using module.goods;
using module.msg;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using task;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class TransViewModel : MViewModel
    {
        public TransViewModel() : base("Trans")
        {
            MList = new ObservableCollection<TransView>();
            MFList = new ObservableCollection<TransView>();

            Messenger.Default.Register<MsgAction>(this, MsgToken.TransUpdate, TransUpdate);

            PubTask.Trans.GetAllTrans();

            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);

            TListView = System.Windows.Data.CollectionViewSource.GetDefaultView(MList);
            TListView.Filter = new Predicate<object>(OnFilterMovie);

            FTListView = System.Windows.Data.CollectionViewSource.GetDefaultView(MFList);
            FTListView.Filter = new Predicate<object>(OnFilterMovie);

            CheckIsSingle();
        }

        #region[字段]      
        private bool showareafilter = true;
        private uint filterareaid = 0;
        private ushort filterlineid = 0;
        private TransView selectedtask, selectedftask;
        private TransView recentTask, finishTask;
        private bool m_finish_tab_show;

        private string tcmsg;//交管信息
        private string stepinfo;//步骤信息

        private bool showsecondupbutton = false;//展示反抛按钮

        #endregion

        #region[属性]

        public bool ShowAreaFileter
        {
            get => showareafilter;
            set => Set(ref showareafilter, value);
        }

        private IList<MyRadioBtn> _arearadio;

        public IList<MyRadioBtn> AreaRadio
        {
            get => _arearadio;
            set => Set(ref _arearadio, value);
        }

        public TransView SelectedTask
        {
            get => selectedtask;
            set => Set(ref selectedtask, value);

        }
        public TransView SelectedFTask
        {
            get => selectedftask;
            set => Set(ref selectedftask, value);

        }

        /// <summary>
        /// 交管消息
        /// </summary>
        public string TCmsg
        {
            get => tcmsg;
            set => Set(ref tcmsg, value);
        }

        /// <summary>
        /// 步骤信息
        /// </summary>
        public string StepInfo
        {
            get => stepinfo;
            set => Set(ref stepinfo, value);
        }

        public ICollectionView TListView { set; get; }
        public ICollectionView FTListView { set; get; }

        /// <summary>
        /// 当前任务列表
        /// </summary>
        public ObservableCollection<TransView> MList { get; set; }

        /// <summary>
        /// 完成任务列表
        /// </summary>
        public ObservableCollection<TransView> MFList { get; set; }

        public bool ShowSecondUpButton
        {
            get => showsecondupbutton;
            set => Set(ref showsecondupbutton, value);
        }
        #endregion

        #region[命令]            
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<RoutedEventArgs> TaskItemSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TaskItemSelected)).Value;
        public RelayCommand<RoutedEventArgs> TabSelectedCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(TabSelected)).Value;
        public RelayCommand<string> TaskActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TaskAction)).Value;
        
        public RelayCommand<string> ActionTaskCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(ActionTask)).Value;

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                ShowAreaFileter = false;
                filterareaid = areaid;
                filterlineid = lineid;
            }
            if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableSecondUpTask))
            {
                ShowAreaFileter = true;
                ShowSecondUpButton = true;
            }
        }
        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0) return true;
            if (item is TransView trans)
            {
                return trans.Area_id == filterareaid && trans.Line_id == filterlineid ;
            }
            return true;
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn && btn.DataContext is MyRadioBtn radio)
            {
                filterareaid = radio.AreaID;
                filterlineid = radio.Line;
                TListView.Refresh();
                FTListView.Refresh();
            }
        }

        private void TaskAction(string tag)
        {
            if (SelectedTask == null) return;
            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1://取消任务
                        if (!PubTask.Trans.CancelTask(SelectedTask.Id, out string result))
                        {
                            Growl.Warning(result);
                        }
                        else
                        {
                            Growl.Success("正在取消！");
                        }
                        break;
                    case 2://完成任务
                        if (!PubTask.Trans.ForseFinish(SelectedTask.Id, out result, "PC手动"))
                        {
                            Growl.Warning(result);
                        }
                        else
                        {
                            Growl.Success("正在完成！");
                        }
                        break;
                    case 3://修改卸货点
                        break;
                    case 4://刷新数据
                        break;
                    case 5:
                        break;

                    default:
                        break;
                }
            }
        }
        private void TaskItemSelected(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is DataGrid pro
                && pro.SelectedItem is TransView md)
            {
                if (m_finish_tab_show)
                {
                    finishTask = md;
                }
                else
                {
                    recentTask = md;
                }
                Update(md);
            }
        }
        private void TabSelected(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is System.Windows.Controls.TabControl pro && pro.SelectedItem is System.Windows.Controls.TabItem tab)
            {
                //Growl.Info(tab.Header.ToString());
                m_finish_tab_show = "FINISH".Equals(tab.Tag.ToString());
                Update(m_finish_tab_show ? finishTask : recentTask);

            }
        }


        /// <summary>
        /// 任务按钮功能
        /// </summary>
        private void ActionTask(string tag)
        {
            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {

                    case 0://反抛
                        uint tile_id = 4, track_id = 5;

                        //如果当前上砖机轨道已有任务
                        if (PubTask.Trans.HaveInTileTrack(track_id, TransTypeE.反抛任务))
                        {
                            Growl.Warning("已有反抛任务，不能继续生成!");
                            return;
                        }

                        string rr = string.Format("确认是否生成反抛任务？");

                        MessageBoxResult box = HandyControl.Controls.MessageBox.Show(rr, "警告",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (box == MessageBoxResult.Yes)
                        {
                            PubTask.Trans.CheckAndAddBackUpTask(tile_id, track_id);
                        }
                        break;
                }
            }

        }

        /// <summary>
        /// 选择任务后，更新详细信息
        /// </summary>
        /// <param name="module"></param>
        private void Update(TransView module)
        {
            if (module != null)
            {
                TCmsg = module.TCmsg;
                StepInfo = module.StepInfo;
            }
            else
            {
                TCmsg = "";
                StepInfo = "";
            }
        }

        /// <summary>
        /// 更新任务列表信息
        /// </summary>
        /// <param name="item"></param>
        public void TransUpdate(MsgAction item)
        {
            if (item != null && item.o1 is StockTrans trans)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TransView grid = MList.FirstOrDefault(c => c.Id == trans.id);
                    if (grid == null)
                    {
                        if (trans.finish)
                        {
                            MFList.Add(new TransView(trans));
                        }
                        else
                        {
                            MList.Add(new TransView(trans));
                        }
                    }
                    else
                    {
                        grid.Update(trans);
                        if (trans.finish)
                        {
                            FinishUpdateTask(grid);
                        }
                        else
                        {
                            UpdateNoFinishTask(grid);
                        }
                    }
                    ClearOver1HoursTask();
                });
            }
        }

        /// <summary>
        /// 更新完成任务的信息
        /// </summary>
        /// <param name="grid"></param>
        private void FinishUpdateTask(TransView grid)
        {
            MList.Remove(grid);
            if (MFList.Count > 20)
            {
                MFList.RemoveAt(0);
            }
            MFList.Add(grid);
            if (recentTask != null && recentTask.Id == grid.Id)
            {
                recentTask = null;
                if (!m_finish_tab_show)
                {
                    Update(recentTask);
                }
            }
        }

        /// <summary>
        /// 更新未完成的状态
        /// </summary>
        /// <param name="grid"></param>
        private void UpdateNoFinishTask(TransView grid)
        {
            if (!m_finish_tab_show && recentTask != null && recentTask.Id == grid.Id)
            {
                recentTask = grid;
                Update(recentTask);
            }
            else if (m_finish_tab_show && finishTask != null && finishTask.Id == grid.Id)
            {
                finishTask = grid;
                Update(finishTask);
            }
        }

        private void ClearOver1HoursTask()
        {
            TransView md = MFList.FirstOrDefault(c => c.IsOver1Hours());
            if (md != null)
            {
                MFList.Remove(md);
            }
        }

        #endregion

        protected override void TabActivate()
        {

        }

        protected override void TabDisActivate()
        {

        }
    }

}
