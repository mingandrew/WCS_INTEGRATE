    using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.goods;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class TrackAllocateViewModel : ViewModelBase
    {
        public TrackAllocateViewModel()
        {
            trackids = new ObservableCollection<Track>();
        }

        #region[字段]

        private string tilename;
        private uint goodsid;
        private Device device;
        private ObservableCollection<Track> trackids;
        #endregion

        #region[属性]
        public string TileName
        {
            get => tilename;
            set => Set(ref tilename, value);
        }

        public uint GoodsId 
        {
            get => goodsid;
            set => Set(ref goodsid, value);
        }

        public ObservableCollection<Track> TrackIdsList
        {
            get => trackids;
            set => Set(ref trackids, value);
        }
        #endregion

        #region[命令]
        public RelayCommand<string> StockAllocateCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(StockAllocateAsync)).Value;

        #endregion

        #region[方法]

        private async void StockAllocateAsync(string tag)
        {
            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1://选择砖机
                        DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                         .Initialize<DeviceSelectViewModel>((vm) =>
                         {
                             vm.FilterArea = false;
                             vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.上砖机, DeviceTypeE.下砖机 });
                         }).GetResultAsync<DialogResult>();
                        if (result.p1 is bool rs && result.p2 is Device dev)
                        {
                            device = dev;
                            TileName = dev.name;
                            GoodsId = dev.goods_id;
                            CheckUpdate(false);
                        }
                        break;
                    case 2://选择规格

                        if (device == null)
                        {
                            Growl.Warning("请选择设备");
                            return;
                        }
                        uint area = PubMaster.Device.GetDeviceArea(device.id);
                        result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                         .Initialize<GoodsSelectViewModel>((vm) =>
                         {
                             vm.SetAreaFilter(area, false);
                             vm.QueryGood();
                         }).GetResultAsync<DialogResult>();

                        if (result.p1 is bool rs2 && result.p2 is Goods good)
                        {
                            GoodsId = good.id;
                            CheckUpdate();
                        }
                        break;
                    case 3://刷新
                        CheckUpdate(true);

                        break;
                }
            }
        }

        private void CheckUpdate(bool isalertinfo = true)
        {
            if(device == null)
            {
                if (isalertinfo) Growl.Warning("请选择设备");
                return;
            }

            if(goodsid == 0)
            {
                if(isalertinfo) Growl.Warning("选择规格");
                return;
            }

            TrackIdsList.Clear();
            if (device.Type == DeviceTypeE.下砖机)
            {
                if (PubMaster.Goods.AllocateGiveTrack(device.area, device.id, GoodsId, out List<uint> traids))
                {
                    TrackIdsList.Clear();
                    foreach (uint id in traids)
                    {
                        TrackIdsList.Add(PubMaster.Track.GetTrack(id));
                    }
                }
            }
            else
            {
                if (PubMaster.Track.HaveTrackInGoodButNotStock(device.area, device.id, GoodsId, out List<uint> trackids))
                {
                    foreach (var trackid in trackids)
                    {
                        TrackIdsList.Add(PubMaster.Track.GetTrack(trackid));
                    }
                }
                //分配库存
                List<Stock> stocks = PubMaster.Goods.GetStock(device.area, device.id, GoodsId);
                foreach (Stock stock in stocks)
                {
                    if(TrackIdsList.FirstOrDefault(c => c.id == stock.track_id) is null)
                    {
                        TrackIdsList.Add(PubMaster.Track.GetTrack(stock.track_id));
                    }
                }
            }
        }

        #endregion
    }
}
