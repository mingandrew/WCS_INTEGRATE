using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.device;
using module.goods;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using wcs.Data.View;
using wcs.Dialog;
using module.msg;

namespace wcs.ViewModel
{
    public class PreStockGoodViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public PreStockGoodViewModel()
        {
            left_list = new ObservableCollection<StockGoodSumView>();
            right_list = new ObservableCollection<StockGoodSumView>();
            //delelist = new List<uint>();
            RightStockGoodList = new ObservableCollection<StockGoodSumView>();
            lefttemplist = new List<StockGoodSumView>();

            _result = new DialogResult();

            GoodListView = System.Windows.Data.CollectionViewSource.GetDefaultView(RightStockGoodList);
        }

        #region[字段]
        private StockGoodSumView left_good;
        private ObservableCollection<StockGoodSumView> left_list;
        private List<StockGoodSumView> lefttemplist { set; get; }

        private StockGoodSumView right_good;
        private ObservableCollection<StockGoodSumView> right_list;

        private Device tile;
        private string tilename;
        private uint addtempid = 1;
        private TilePreGoodType tilePreGoodType { get; set; }
        private uint nowgoodid { get; set; }
        //private List<uint> delelist;

        private bool isloop;   //是否循环
        private bool auto_shift_good_by_time; //预设品种按时间设置
        private bool auto_shift_good;//自动转产

        private DialogResult _result;

        private bool isChange { set; get; } = false; //左边列表是否有修改且没保存

        #endregion

        #region[属性]
        public ICollectionView GoodListView { set; get; }

        public string TileName
        {
            get => tilename;
            set => Set(ref tilename, value);
        }

        public StockGoodSumView LeftStockGood
        {
            get => left_good;
            set
            {
                left_good?.SetSelected(false);
                Set(ref left_good, value);
                left_good?.SetSelected(true);
            }
        }
        
        public ObservableCollection<StockGoodSumView> LeftStockGoodList
        {
            get => left_list;
            set => Set(ref left_list, value);
        }

        public StockGoodSumView RightStockGood
        {
            get => right_good;
            set
            {
                right_good?.SetSelected(false);
                Set(ref right_good, value);
                right_good?.SetSelected(true);
            }
        }
        
        public ObservableCollection<StockGoodSumView> RightStockGoodList
        {
            get => right_list;
            set => Set(ref right_list, value);
        }


        public bool IsLoop
        {
            get => isloop;
            set
            {
                Set(ref isloop, value);
                if (value)
                {
                    AutoShiftGoodByTime = false;
                    tilePreGoodType = TilePreGoodType.循环预设列表;
                }
            }
        }


        public bool AutoShiftGoodByTime
        {
            get => auto_shift_good_by_time;
            set
            {
                Set(ref auto_shift_good_by_time, value);
                if (value)
                {
                    IsLoop = false;
                    tilePreGoodType = TilePreGoodType.自动先进先出;
                    AllStockGood2Left();
                }
            }
        }


        public bool AutoShiftGood
        {
            get => auto_shift_good;
            set => Set(ref auto_shift_good, value);
        }

        public Action CloseAction { get; set; }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        #endregion

        #region[命令]
        public RelayCommand<string> ButtonCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DoButton)).Value;

        public RelayCommand<StockGoodSumView> QtyAddCmd => new Lazy<RelayCommand<StockGoodSumView>>(() => new RelayCommand<StockGoodSumView>(StockQtyAdd)).Value;
        public RelayCommand<StockGoodSumView> QtySubCmd => new Lazy<RelayCommand<StockGoodSumView>>(() => new RelayCommand<StockGoodSumView>(StockQtySub)).Value;
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
                case "RefreshPreStockGood"://刷新砖机轨道信息
                    RefreshStockGoods();
                    isChange = false;
                    break;
                case "SavePreStockGood"://保存
                    SavePreStockGood();
                    isChange = false;
                    break;
                case "TrackGoUp"://轨道上移
                    TrackGoUp();
                    isChange = true;
                    break;
                case "TrackGoDown"://轨道下移
                    TrackGoDown();
                    isChange = true;
                    break;
                case "Track2Left"://轨道添加到左边列表
                    StockGood2Left();
                    isChange = true;
                    break;
                case "Track2Right"://轨道添加到右边列表
                    StockGood2Right();
                    isChange = true;
                    break; 
                case "CloseStockGood"://关闭窗口
                    CloseStockGood();
                    break;
            }
        }

        private void SavePreStockGood()
        {
            uint nowgood = PubMaster.DevConfig.GetDeviceNowId(tile.id);
            byte order = 1;
            PubMaster.Goods.DeleteTilePreStockGoods(tile.id);
            PreStockGood firstgood = null;
            foreach (StockGoodSumView stockGoodSum in LeftStockGoodList)
            {
                PreStockGood preStock = new PreStockGood()
                {
                    tile_id = tile.id,
                    good_id = stockGoodSum.GoodId,
                    order = order++,
                    pre_good_qty = stockGoodSum.Count,
                    pre_good_all = stockGoodSum.IsUseAll(),
                    level = (byte)stockGoodSum.Level,
                };
                PubMaster.Goods.AddPreStockGood(preStock);
                if (preStock.order == 1)
                {
                    firstgood = preStock;
                }
            }

            //保存砖机的预设品种设置
            tilePreGoodType = TilePreGoodType.根据预设列表;
            if (IsLoop)
            {
                tilePreGoodType = TilePreGoodType.循环预设列表;
            }
            if (AutoShiftGoodByTime)
            {
                tilePreGoodType = TilePreGoodType.自动先进先出;
            }
            PubMaster.DevConfig.SetPreGoodSetting(tile.id, tilePreGoodType, AutoShiftGood);

            PubMaster.Goods.SortPreStockGoodList();
            Growl.Success("更新成功！");
            RefreshStockGoods();

            //如果上砖机没有预设品种,且预设品种列表第一个不为空
            if (firstgood != null)  //PubMaster.DevConfig.GetDevicePreId(tile.id) == 0 && 
            {
                //if (nowgood == 0)
                //{
                //    return;
                //}
                // 则将当前第一个预设品种设置上去
                PubMaster.DevConfig.UpdateTilePreGood(tile.id, nowgood, firstgood.good_id, (firstgood.pre_good_all ? 0 : firstgood.pre_good_qty), firstgood.level, out string msg);
            }
        }

        private async void SelectTile()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                .Initialize<DeviceSelectViewModel>((vm) =>
                {
                    vm.AreaId = 0;
                    vm.FilterArea = false;
                    vm.SetSelectType(DeviceTypeE.上砖机, DeviceTypeE.砖机);
                }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                tile = dev;
                TileName = dev.name;
                SetTilePreSetting();
                RefreshStockGoods();
            }
        }

        /// <summary>
        /// 获取砖机信息
        /// </summary>
        public void SelectTile(uint tileid)
        {
            Device dev = PubMaster.Device.GetDevice(tileid);
            tile = dev;
            TileName = dev.name;
            SetTilePreSetting();
            RefreshStockGoods();
        }

        /// <summary>
        /// 设置砖机的预设品种列表的配置
        /// </summary>
        private void SetTilePreSetting()
        {
            tilePreGoodType = PubMaster.DevConfig.GetTilePreGoodType(tile.id);
            IsLoop = tilePreGoodType == TilePreGoodType.循环预设列表;
            AutoShiftGoodByTime = tilePreGoodType == TilePreGoodType.自动先进先出;
            AutoShiftGood = PubMaster.DevConfig.GetTileAutoShiftGood(tile.id);
        }

        private void RefreshStockGoods()
        {
            if(tile == null)
            {
                Growl.Warning("请先选择砖机！");
                return;
            }
            LeftStockGoodList.Clear();
            RightStockGoodList.Clear();
            //delelist.Clear();
            lefttemplist.Clear();
            IsLoop = false;
            addtempid = 1;
            nowgoodid = PubMaster.DevConfig.GetDeviceNowId(tile.id);

            SetTilePreSetting();
            RefreshRightList();
            RefreshLeftList();
        }

        public void RefreshLeftList()
        {
            if (tilePreGoodType == TilePreGoodType.自动先进先出)
            {
                return;
            }
            LeftStockGoodList.Clear();
            List<PreStockGood> plist = PubMaster.Goods.GetPreStockGoodList(tile.id);
            List<StockGoodSumView> goodsums = new List<StockGoodSumView>();
            foreach (var item in plist)
            {
                StockGoodSumView sum = new StockGoodSumView(lefttemplist.Find(c => c.EqualGoodAndLevel(item.good_id, item.level)));
                sum.Count = item.pre_good_all ? (sum.OrgCount + 1) : item.pre_good_qty;
                sum.Order = item.order;
                sum.ShowLabel = item.pre_good_all;
                sum.ShowCount = !sum.ShowLabel;
                goodsums.Add(sum);
            }
            goodsums.Sort((x, y) =>
            {
                return x.Order.CompareTo(y.Order);
            });
            //if (goodsums != null && goodsums.Count != 0)
            //{
            //    if (tilePreGoodType != TilePreGoodType.循环预设列表 && goodsums[0].GoodId == nowgoodid)
            //    {
            //        goodsums.RemoveAt(0);
            //    }
            //}

            Application.Current.Dispatcher.Invoke(() =>
            {
                LeftStockGoodList.Clear();
                foreach (StockGoodSumView mod in goodsums)
                {
                    LeftStockGoodList.Add(mod);
                }
            });
        }

        public void RefreshRightList()
        {
            List<StockGoodSumView> goodsums = GetStockGoods();
            lefttemplist = goodsums;

            Application.Current.Dispatcher.Invoke(() =>
            {
                RightStockGoodList.Clear();
                foreach (StockGoodSumView mod in goodsums)
                {
                    mod.Count++;
                    RightStockGoodList.Add(mod);
                }
            });
        }

        /// <summary>
        /// 获取当前库存的品种
        /// </summary>
        /// <returns></returns>
        private List<StockGoodSumView> GetStockGoods()
        {
            List<StockSum> sums = PubMaster.Sums.GetStockSums();
            List<StockGoodSumView> goodsums = new List<StockGoodSumView>();
            foreach (var item in sums)
            {
                StockGoodSumView sum = goodsums.Find(c => c.GoodId == item.goods_id);
                if (sum != null)
                {
                    sum.AddToSum(item);
                }
                else
                {
                    if (item.TrackType != TrackTypeE.储砖_出
                        && item.TrackType != TrackTypeE.储砖_出入)
                    {
                        continue;
                    }
                    sum = new StockGoodSumView(item);
                    Goods goods = PubMaster.Goods.GetGoods(item.goods_id);
                    if (goods != null)
                    {
                        sum.GoodName = goods.name;
                        sum.Color = goods.color;
                        //sum.Level = goods.level;
                        sum.Width = PubMaster.Goods.GetSizeWidth(goods.size_id);
                    }
                    goodsums.Add(sum);
                }
            }
            goodsums.Sort((x, y) =>
            {
                if (x.produce_time is DateTime dtime && y.produce_time is DateTime ctime)
                {
                    return dtime.CompareTo(ctime);
                }
                return 0;
            });
            return goodsums;
        }

        private void TrackGoUp()
        {
            if (!CheckLeftStockGood()) return;
            int idx = LeftStockGoodList.IndexOf(LeftStockGood);
            if(idx == 0)
            {
                Growl.Warning("已经在第一位!");
                return;
            }
            StockGoodSumView good = LeftStockGood;
            LeftStockGoodList.RemoveAt(idx);
            LeftStockGoodList.Insert(idx - 1, good);
            LeftStockGood = good;
        }

        private void TrackGoDown()
        {
            if (!CheckLeftStockGood()) return;
            int idx = LeftStockGoodList.IndexOf(LeftStockGood);
            if (idx == LeftStockGoodList.Count-1)
            {
                Growl.Warning("已经在底部!");
                return;
            }
            StockGoodSumView good = LeftStockGood;
            LeftStockGoodList.Remove(LeftStockGood);
            LeftStockGoodList.Insert(idx + 1, good);
            LeftStockGood = good;
        }

        private void StockGood2Left()
        {
            if(RightStockGood == null)
            {
                Growl.Warning("请选择右边的列表数据!");
                return;
            }

            if (AutoShiftGoodByTime)
            {
                AutoShiftGoodByTime = false;
            }

            StockGoodSumView ntt = new StockGoodSumView(RightStockGood);
            LeftStockGoodList.Add(ntt);
        }
        
        /// <summary>
        /// 自动根据库存时间设置预设品种，将右边的列表全部加到左边
        /// </summary>
        private void AllStockGood2Left()
        {
            List<StockGoodSumView> goodsums = GetStockGoods();

            goodsums.Sort((x, y) =>
            {
                return x.Order.CompareTo(y.Order);
            });

            //if (goodsums != null && goodsums.Count != 0)
            //{
            //    if (goodsums[0].GoodId == nowgoodid)
            //    {
            //        goodsums.RemoveAt(0);
            //    }
            //}

            Application.Current.Dispatcher.Invoke(() =>
            {
                LeftStockGoodList.Clear();
                foreach (StockGoodSumView mod in goodsums)
                {
                    mod.Count++;
                    LeftStockGoodList.Add(mod);
                }
            });
        }

        private void StockGood2Right()
        {
            if (!CheckLeftStockGood()) return;

            if (AutoShiftGoodByTime)
            {
                AutoShiftGoodByTime = false;
            }

            //delelist.Add(LeftStockGood.GoodId);
            LeftStockGoodList.Remove(LeftStockGood);
        }

        private bool CheckLeftStockGood()
        {
            if (LeftStockGood == null)
            {
                Growl.Warning("请选择左边的列表!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 库存数+1
        /// </summary>
        /// <param name="view"></param>
        private void StockQtyAdd(StockGoodSumView view)
        {
            view.AddSubQty(true);
        }

        /// <summary>
        /// 库存数-1
        /// </summary>
        /// <param name="view"></param>
        private void StockQtySub(StockGoodSumView view)
        {
            view.AddSubQty(false);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void CloseStockGood()
        {
            Result.p1 = false;
            Result.p2 = null;
            RefreshLeftList();
            if (isChange)
            {
                string rr = string.Format("预设品种列表已修改，退出将不保存，是否退出？");
                MessageBoxResult box = HandyControl.Controls.MessageBox.Show(rr, "警告",
                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (box == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (LeftStockGoodList != null && LeftStockGood.Count != 0)
            {
                Result.p1 = true;
                Result.p2 = LeftStockGoodList[0];
            }
            CloseAction?.Invoke();
        }
        #endregion
    }
}
