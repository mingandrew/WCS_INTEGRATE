using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.msg;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using task;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class OrganizeTrackViewModel : ViewModelBase
    {
        public OrganizeTrackViewModel()
        {
            List = new ObservableCollection<OrgGoodTrackView>();
            DtlList = new ObservableCollection<OrgGoodTrackView>();

            Messenger.Default.Register<MsgAction>(this, MsgToken.AllowShow, AllowUserShow);

            //权限配置 20210118
            Admin = PubMaster.Role.MatchRolePrior(WcsRolePrior.管理员, null);
            SuperVisor = PubMaster.Role.MatchRolePrior(WcsRolePrior.超级管理员, null);

            Messenger.Default.Register<MsgAction>(this, MsgToken.StockTransDtlUpdate, StockTransDtlUpdate);

            PubTask.Trans.GetAllStockTransDtl();
        }

        #region[字段]

        private Track _selecttrack;
        private string _selecttrackname;
        private ObservableCollection<OrgGoodTrackView> _list;
        private ObservableCollection<OrgGoodTrackView> _dtllist;
        private OrgGoodTrackView _selectitem;


        private bool admin; //管理员功能授权，是否显示
        private bool supervisor; //超级管理员功能授权，是否显示

        #endregion

        #region[属性]

        public string SelectTrackName
        {
            get => _selecttrackname;
            set => Set(ref _selecttrackname, value);
        }

        public ObservableCollection<OrgGoodTrackView> List
        {
            get => _list;
            set => Set(ref _list, value);
        }

        public ObservableCollection<OrgGoodTrackView> DtlList
        {
            get => _dtllist;
            set => Set(ref _dtllist, value);
        }

        public OrgGoodTrackView SelectItem
        {
            get => _selectitem;
            set => Set(ref _selectitem, value);
        }


        //管理员功能授权，是否显示
        public bool Admin
        {
            get => admin;
            set => Set(ref admin, value);
        }
        //超级管理员功能授权，是否显示
        public bool SuperVisor
        {
            get => supervisor;
            set => Set(ref supervisor, value);
        }
        #endregion

        #region[命令]
        public RelayCommand<string> BtnCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(ButtonClick)).Value;
        public RelayCommand<OrgGoodTrackView> GoodTrackUpCmd => new Lazy<RelayCommand<OrgGoodTrackView>>(() => new RelayCommand<OrgGoodTrackView>(TrackGoodUp)).Value;
        public RelayCommand<OrgGoodTrackView> GoodTrackMoveCmd => new Lazy<RelayCommand<OrgGoodTrackView>>(() => new RelayCommand<OrgGoodTrackView>(TrackGoodMove)).Value;
        public RelayCommand<OrgGoodTrackView> GoodTrackStayCmd => new Lazy<RelayCommand<OrgGoodTrackView>>(() => new RelayCommand<OrgGoodTrackView>(TrackGoodStay)).Value;

        #endregion

        #region[方法]

        /// <summary>
        /// 操作选择
        /// </summary>
        /// <param name="tag"></param>
        private void ButtonClick(string tag)
        {
            switch (tag)
            {
                case "selecttrack"://选择轨道
                    TrackSelected();
                    break;
                case "clearinfo"://清除信息
                    ClearInputInfo();
                    break;
                case "addtrans"://添加任务
                    if (_selecttrack == null)
                    {
                        Growl.Warning("请先选择轨道");
                        return;
                    }
                    AddOrganizeTrans();
                    break;
            }
        }

        private void TrackGoodUp(OrgGoodTrackView view)
        {
            view.SetDtlType(StockTransDtlTypeE.上砖品种);
            view.Give_Track_Id = 0;
        }

        private void TrackGoodMove(OrgGoodTrackView view)
        {
            view.SetDtlType(StockTransDtlTypeE.转移品种);
            UpdateTrackGoodOrganizeTrack();
        }

        public void TrackGoodStay(OrgGoodTrackView view)
        {
            int idx = List.IndexOf(view);
            bool inlast = idx == List.Count - 1;
            if (!inlast)
            {
                List<uint> gids = new List<uint>();
                for(int i=idx+1; i< List.Count; i++)
                {
                    OrgGoodTrackView v =  List[i];
                    if(v != null && v.DtlType != StockTransDtlTypeE.保留品种)
                    {
                        Growl.Warning("该品种后面存在非保留品种");
                        return;
                    }

                    if (v != null)
                    {
                        gids.Add(v.Good_Id);
                    }
                }
                gids.Add(view.Good_Id);
                if (!PubMaster.Goods.IsGoodAllInButton(view.Take_Track_Id, view.Good_Id, gids))
                {
                    Growl.Warning("该品种并没有集中在轨道后面！");
                    return;
                }
            }
            else
            {
                if (!PubMaster.Goods.IsGoodAllInButton(view.Take_Track_Id, view.Good_Id))
                {
                    Growl.Warning("该品种并没有集中在轨道后面！");
                    return;
                }
            }

            view.SetDtlType(StockTransDtlTypeE.保留品种);
            view.Give_Track_Id = 0;
        }

        private void UpdateTrackGoodOrganizeTrack()
        {
            List<uint> tracks = PubMaster.Track.GetTrackFreeEmptyTrackIds(_selecttrack.id);
            List<uint> freeids = new List<uint>();
            foreach (var item in tracks)
            {
                if (!PubTask.Trans.ExistTransWithTracks(item))
                {
                    freeids.Add(item);
                }
            }
            
            if(freeids.Count < List.Count(c=>c.DtlType == StockTransDtlTypeE.转移品种))
            {
                Growl.Warning("当前空轨道数量不满足！");
                return;
            }
            ushort idx = 0;
            foreach (var item in List)
            {
                if(item.DtlType == StockTransDtlTypeE.转移品种)
                {
                    item.Give_Track_Id = freeids[idx];
                    idx++;
                }
            }
        }

        /// <summary>
        /// 选择轨道
        /// </summary>
        private async void TrackSelected()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                 .Initialize<TrackSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(0, true);
                     vm.QueryTrack(TrackTypeE.储砖_出, TrackTypeE.储砖_出入);
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                _selecttrack = tra;
                SelectTrackName = tra.name;
                ShowTrackGood(tra);
            }
        }

        private void ClearInputInfo()
        {
            SelectTrackName = null;
            SelectItem = null;
            List.Clear();
        }

        /// <summary>
        /// 添加库存整理任务
        /// </summary>
        private void AddOrganizeTrans()
        {
            if (_selecttrack == null)
            {
                Growl.Warning("请先选择轨道！");
                return;
            }

            if(PubTask.Trans.ExistTransWithType(_selecttrack.area, TransTypeE.库存整理))
            {
                Growl.Warning("当前已经有一个库存整理任务了！");
                return;
            }

            if (List.Count == 0 || null == List.FirstOrDefault(c => c.DtlType == StockTransDtlTypeE.转移品种))
            {
                Growl.Warning("当前轨道没有需要整理的库存信息");
                return;
            }

            if (List.Count == 1)
            {
                Growl.Warning("当前轨道只有一个品种，不需要整理");
                return;
            }
            bool havestay = false;
            foreach (var item in List)
            {
                if(item.DtlType == StockTransDtlTypeE.保留品种)
                {
                    havestay = true;
                }

                if(item.DtlType == StockTransDtlTypeE.转移品种 && havestay)
                {
                    Growl.Warning("转移品种前面有保留品种，请重新设置！");
                    return;
                }

                if (item.Give_Track_Id == 0 && item.DtlType == StockTransDtlTypeE.转移品种)
                {
                    Growl.Warning("存在转移轨道为空的整理品种类型");
                    return;
                }
            }


            List<StockTransDtl> dtl = new List<StockTransDtl>();
            foreach (var item in List)
            {
                dtl.Add(new StockTransDtl()
                {
                    dtl_good_id = item.Good_Id,
                    dtl_take_track_id = item.Take_Track_Id,
                    dtl_give_track_id = item.Give_Track_Id,
                    dtl_all_qty = item.All_Qty,
                    dtl_area_id = item.Area_Id,
                    DtlType = item.DtlType,
                    DtlStatus = item.DtlType == StockTransDtlTypeE.转移品种 ? StockTransDtlStatusE.整理中 : StockTransDtlStatusE.完成,
                });
            }

            if (!PubTask.Trans.AddOrganizeTrans(_selecttrack.id, dtl,out string result))
            {
                Growl.Warning(result);
                UpdateTrackGoodOrganizeTrack();
                return;
            }

            Growl.Success("添加库存整理任务成功！");
            ClearInputInfo();
        }

        /// <summary>
        /// 展示选择的轨道品种数据
        /// </summary>
        /// <param name="track"></param>
        private void ShowTrackGood(Track track)
        {
            List.Clear();
             List<StockTransDtl> dtls = PubMaster.Goods.GetTrackGood2Organize(track.id);
            foreach (var item in dtls)
            {
                List.Add(new OrgGoodTrackView(item));
            }
            UpdateTrackGoodOrganizeTrack();
        }

        private void StockTransDtlUpdate(MsgAction msg)
        {
            if (msg == null) return;
            Application.Current.Dispatcher.Invoke(() => 
            { 
                if(msg.o1 is StockTransDtl dtl && msg.o2 is ActionTypeE type)
                {
                    OrgGoodTrackView view = DtlList.FirstOrDefault(c => c.Dtl_ID == dtl.dtl_id);
                    if(view == null && type == ActionTypeE.Add)
                    {
                        view = new OrgGoodTrackView(dtl);
                        DtlList.Add(view);
                    }

                    if(view != null)
                    {
                        switch (type)
                        {
                            case ActionTypeE.Add:
                                break;
                            case ActionTypeE.Update:
                                view.Update(dtl);
                                break;
                            case ActionTypeE.Delete:
                                DtlList.Remove(view);
                                break;
                            case ActionTypeE.Finish:
                                DtlList.Remove(view);
                                break;
                        }
                    }
                }
            });
        }

        ////更新功能展示权限
        private void AllowUserShow(MsgAction msg)
        {
            if (msg != null && msg.o1 is bool ad && msg.o2 is bool super)
            {
                Admin = ad;
                SuperVisor = super;
            }
        }
        
        #endregion


    }
}
