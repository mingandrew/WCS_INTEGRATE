using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using task;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class DeviceBackupSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public DeviceBackupSelectViewModel()
        {
            _result = new DialogResult();
            _devbackuplist = new ObservableCollection<BackupTileLifterView>();
            BackupDeviceView = System.Windows.Data.CollectionViewSource.GetDefaultView(DevBackupList);

            //PubTask.TileLifter.GetBackupTileLifter()
        }

        #region[字段]

        private DialogResult _result;
        private ObservableCollection<BackupTileLifterView> _devbackuplist;
        private BackupTileLifterView selectdic;

        #endregion

        #region[属性]
        
        public ObservableCollection<BackupTileLifterView> DevBackupList
        {
            get => _devbackuplist;
            set => Set(ref _devbackuplist, value);
        }

        public BackupTileLifterView SelectBackupDev
        {
            get => selectdic;
            set => Set(ref selectdic, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public ICollectionView BackupDeviceView { set; get; }

        public Action CloseAction { get; set; }
        
        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]

        public void GetBackupTilelifterInfo(uint dev_id)
        {
            List<List<object>> altertiles = PubTask.TileLifter.GetBackupTileLifter(dev_id);
            DevBackupList.Clear();
            foreach(var item in altertiles)
            {
                BackupTileLifterView backup = new BackupTileLifterView();
                backup.DeviceID = (uint)item[0];
                backup.Name = (string)item[1];
                backup.Type = (DeviceTypeE)item[2];
                backup.Goodidnfo = (string)item[3];
                backup.LastTrackName = PubMaster.Track.GetTrackName((uint)item[4]);
                backup.TrackList = (string)item[5];

                DevBackupList.Add(backup);
            }

            BackupDeviceView.Refresh();
        }
        
        private void CheckRadioBtn(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is RadioButton rbtn)
            {
                //switch (rbtn.Tag.ToString())
                //{
                //    case "tilelifter":
                //        GetDevce(DeviceTypeE.上砖机);
                //        break;
                //    case "ferry":
                //        GetDevce(DeviceTypeE.上摆渡);
                //        break;
                //    case "carrier":
                //        GetDevce(DeviceTypeE.运输车);
                //        break;
                //}
            }
        }

        private void Comfirm()
        {
            if (SelectBackupDev == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = true;
            Result.p2 = SelectBackupDev.DeviceID;
            Result.p3 = SelectBackupDev.Name;
            Result.p4 = SelectBackupDev.Type;
            Result.p5 = SelectBackupDev.Goodidnfo;
            Result.p6 = SelectBackupDev.LastTrackName.Equals("") ? "无" : SelectBackupDev.LastTrackName;
            Result.p7 = SelectBackupDev.TrackList;
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
