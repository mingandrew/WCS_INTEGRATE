using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.deviceconfig;
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
    public class DeviceCheckComboSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DeviceCheckComboSelectViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            SynchroTileLifterList = System.Windows.Data.CollectionViewSource.GetDefaultView(synchro_tilelifter_list);
        }

        #region[字段]
        private uint _devid;

        private SynroTileView select_syntiles;

        private DialogResult _result;

        private List<uint> syntileids = new List<uint>();

        private bool isselectall;
        #endregion

        #region[属性]

        public ICollectionView SynchroTileLifterList { set; get; }

        /// <summary>
        /// 砖机数据
        /// </summary>
        private ObservableCollection<SynroTileView> synchro_tilelifter_list { get; set; } = new ObservableCollection<SynroTileView>();

        public SynroTileView SelectSynTiles
        {
            get => select_syntiles;
            set
            {
                Set(ref select_syntiles, value);
                if (value != null)
                {
                    value.IsSelected = !value.IsSelected;
                }
            }
        }
        
        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public DialogResult Param
        {
            set; get;
        }

        public List<uint> SynTileIds
        {
            get => syntileids;
            set => Set(ref syntileids, value);
        }

        public bool IsSelectAll
        {
            get => isselectall;
            set => Set(ref isselectall, value);
        }

        public Action CloseAction { get; set; }
        #endregion

        #region[命令]        
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        public RelayCommand CheckAllCmd => new Lazy<RelayCommand>(() => new RelayCommand(SetAllCheck)).Value;

        #endregion

        #region[方法]

        public void SetTileList(uint devid)
        {
            _devid = devid;
            ConfigTileLifter confit = PubMaster.DevConfig.GetTileLifter(devid);
            if (confit == null)
            {
                Growl.Warning("获取不到砖机的配置信息！");
                CancelChange();
                return;
            }
            List<Device> tiles = new List<Device>();
            if (confit.WorkMode == TileWorkModeE.上砖)
            {
                //找在线的砖机
                tiles = PubMaster.Device.GetTileLiftersWithoutSelf(_devid, DeviceTypeE.上砖机);
            }

            if (confit.WorkMode == TileWorkModeE.下砖)
            {
                tiles = PubMaster.Device.GetTileLiftersWithoutSelf(_devid, DeviceTypeE.下砖机);
            }

            synchro_tilelifter_list.Clear();
            foreach (Device item in tiles)
            {
                SynroTileView temp = new SynroTileView()
                {
                    ID = item.id,
                    Name = item.name,
                    Type = item.Type,
                    IsConnect = true
                };
                if (!PubTask.TileLifter.IsOnline(item.id))
                {
                    temp.IsConnect = false;
                }
                if (confit.SynTileList.Contains((ushort)item.id))
                {
                    temp.IsSelected = true;
                }
                synchro_tilelifter_list.Add(temp);
            }
        }

        private void SetCheck(SynroTileView view)
        {
            if (view != null)
            {
                view.IsSelected = !view.IsSelected;
                //MSiteListView.Refresh();
            }
        }

        private void SetUnCheck()
        {
            foreach (SynroTileView item in synchro_tilelifter_list.Where(c => c.IsSelected == true))
            {
                item.IsSelected = false;
            }
        }

        private void SetAllCheck()
        {
            if (IsSelectAll)
            {
                SetUnCheck();
                IsSelectAll = false;
            }
            else
            {
                foreach (SynroTileView item in synchro_tilelifter_list.Where(c => c.IsSelected == false))
                {
                    item.IsSelected = true;

                }
                IsSelectAll = true;
            }
        }

        private void Comfirm()
        {
            Result.p1 = true;
            SynTileIds.Clear();
            foreach (SynroTileView tileView in synchro_tilelifter_list)
            {
                if (tileView.IsSelected)
                {
                    SynTileIds.Add(tileView.ID);
                }
            }
            if (SynTileIds.Count > 0)
            {
                Result.p2 = SynTileIds;
            }
            else
            {
                Result.p2 = null;
            }
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            Result.p2 = null;
            CloseAction?.Invoke();
        }
        #endregion
    }
}
