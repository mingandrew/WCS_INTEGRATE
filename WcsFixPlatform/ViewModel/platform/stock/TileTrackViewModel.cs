using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.device;
using module.tiletrack;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using wcs.Dialog;

namespace wcs.ViewModel.platform.stock
{
    public class TileTrackViewModel: ViewModelBase
    {
        public TileTrackViewModel()
        {
            left_list = new ObservableCollection<TileTrack>();
            right_list = new ObservableCollection<AreaDeviceTrack>();
            delelist = new List<TileTrack>();
        }

        #region[字段]
        private TileTrack left_track;
        private ObservableCollection<TileTrack> left_list;

        private AreaDeviceTrack right_track;
        private ObservableCollection<AreaDeviceTrack> right_list;

        private Device tile;
        private string tilename;
        private uint addtempid = 1;
        private List<TileTrack> delelist;
        #endregion

        #region[属性]
        public string TileName
        {
            get => tilename;
            set => Set(ref tilename, value);
        }

        public TileTrack LeftTrack
        {
            get => left_track;
            set => Set(ref left_track, value);
        }
        
        public ObservableCollection<TileTrack> LeftTrackList
        {
            get => left_list;
            set => Set(ref left_list, value);
        }

        public AreaDeviceTrack RightTrack
        {
            get => right_track;
            set => Set(ref right_track, value);
        }
        
        public ObservableCollection<AreaDeviceTrack> RightTrackList
        {
            get => right_list;
            set => Set(ref right_list, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<string> ButtonCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DoButton)).Value;

        #endregion

        #region[方法]
        /// <summary>
        /// 按钮方法
        /// </summary>
        /// <param name="tag"></param>
        private void DoButton(string tag)
        {
            switch (tag)
            {

                case "TileSelect"://选择砖机
                    SelectTile();
                    break;
                case "RefreshTileTrack"://刷新砖机轨道信息
                    RefreshTileTrack();
                    break;
                case "SaveTileTrack"://保存
                    SaveTileTrack();
                    break;
                case "TrackGoUp"://轨道上移
                    TrackGoUp();
                    break;
                case "TrackGoDown"://轨道下移
                    TrackGoDown();
                    break;
                case "Track2Left"://轨道添加到左边列表
                    Track2Left();
                    break;
                case "Track2Right"://轨道添加到右边列表
                    Track2Right();
                    break;
            }
        }

        private void SaveTileTrack()
        {
            byte order = 1;
            PubMaster.TileTrack.DeleteTileTrack(delelist);
            foreach (TileTrack tiletrack in LeftTrackList)
            {
                PubMaster.TileTrack.EditTileTrack(tiletrack, order++);
            }

            PubMaster.TileTrack.SortTileTrackList();
            Growl.Success("更新成功！");
            RefreshTileTrack();
        }

        private async void SelectTile()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                .Initialize<DeviceSelectViewModel>((vm) =>
                {
                    vm.AreaId = 0;
                    vm.FilterArea = false;
                    vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.上砖机});
                }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                tile = dev;
                TileName = dev.name;
                RefreshTileTrack();
            }
        }

        private void RefreshTileTrack()
        {
            if(tile == null)
            {
                Growl.Warning("请先选择砖机！");
                return;
            }
            LeftTrackList.Clear();
            RightTrackList.Clear();
            delelist.Clear();
            addtempid = 1;

            List<TileTrack> list = PubMaster.TileTrack.GetTileTracks(tile.id);
            foreach (TileTrack track in list)
            {
                LeftTrackList.Add(track);
            }

            List<AreaDeviceTrack> deviceTracks = PubMaster.Area.GetDevTrackList(tile.id);
            if (deviceTracks.Count > 0)
            {
                deviceTracks.Sort((x, y) => x.track_id.CompareTo(y.track_id));
                foreach (AreaDeviceTrack track in deviceTracks)
                {
                    RightTrackList.Add(track);
                }
            }
            else
            {
                Growl.Warning("请先在区域配置，配置砖机轨道！");
            }

        }

        private void TrackGoUp()
        {
            if (!CheckLeftTrack()) return;
            int idx = LeftTrackList.IndexOf(LeftTrack);
            if(idx == 0)
            {
                Growl.Warning("已经在第一位!");
                return;
            }
            TileTrack track = LeftTrack;
            LeftTrackList.RemoveAt(idx);
            LeftTrackList.Insert(idx - 1, track);
            LeftTrack = track;
        }

        private void TrackGoDown()
        {
            if (!CheckLeftTrack()) return;
            int idx = LeftTrackList.IndexOf(LeftTrack);
            if (idx == LeftTrackList.Count-1)
            {
                Growl.Warning("已经在底部!");
                return;
            }
            TileTrack track = LeftTrack;
            LeftTrackList.Remove(LeftTrack);
            LeftTrackList.Insert(idx+1, track);
            LeftTrack = track;
        }

        private void Track2Left()
        {
            if(RightTrack == null)
            {
                Growl.Warning("请选择左边的列表!");
                return;
            }

            TileTrack ntt = new TileTrack()
            {
                id = addtempid++,
                track_id = RightTrack.track_id,
                tile_id = RightTrack.device_id
            };
            LeftTrackList.Add(ntt);
        }

        private void Track2Right()
        {
            if (!CheckLeftTrack()) return;

            delelist.Add(LeftTrack);
            LeftTrackList.Remove(LeftTrack);
        }

        private bool CheckLeftTrack()
        {
            if (LeftTrack == null)
            {
                Growl.Warning("请选择右边的列表!");
                return false;
            }
            return true;
        }
        #endregion
    }
}
