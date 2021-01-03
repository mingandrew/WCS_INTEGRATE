using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using task;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class StockViewModel : ViewModelBase
    {
        public StockViewModel()
        {
            List = new ObservableCollection<Stock>();
        }

        #region[字段]

        private Track _selecttrack;
        private string _selecttrackname;
        private ObservableCollection<Stock> _list;
        private Stock _selectstock;
        private DateTime? _refreshtime;
        #endregion

        #region[属性]

        public string SelectTrackName
        {
            get => _selecttrackname;
            set => Set(ref _selecttrackname, value);
        }

        public ObservableCollection<Stock> List
        {
            get => _list;
            set => Set(ref _list, value);
        }

        public Stock SelectStock
        {
            get => _selectstock;
            set => Set(ref _selectstock, value);
        }

        public DateTime? RefreshTime
        {
            get => _refreshtime;
            set => Set(ref _refreshtime, value);
        }

        #endregion

        #region[命令]
        public RelayCommand TrackSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(TrackSelected)).Value;
        public RelayCommand<string> ActionStockCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(ActionStock)).Value;
        public RelayCommand<string> StockEditCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(StockEdit)).Value;

        #endregion

        #region[方法]

        /// <summary>
        /// 操作选择
        /// </summary>
        /// <param name="tag"></param>
        private void StockEdit(string tag)
        {
            if (_selecttrack == null)
            {
                Growl.Warning("请先选择轨道");
                return;
            }

            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {
                    case 1://添加

                        break;
                    case 2://修改库存
                        EditStock();
                        break;
                    case 3://删除库存
                        DeleteStock();
                        break;
                    case 4://往前 + 库存
                        AddFrontStock();
                        break;
                    case 5://往后 + 库存
                        AddBackStock();
                        break;
                }
            }
        }

        /// <summary>
        /// 刷新轨道库存
        /// </summary>
        private void ShiftStock()
        {
            if (_selecttrack == null) return;

            if (_selecttrack.Type != TrackTypeE.储砖_入)
            {
                Growl.Warning("不是储存入轨道");
                return;
            }

            if (_selecttrack.StockStatus != TrackStockStatusE.满砖)
            {
                Growl.Warning(_selecttrack.name + "不是满砖状态");
                return;
            }

            Track btrack = PubMaster.Track.GetTrack(_selecttrack.brother_track_id);
            if (btrack == null || btrack.StockStatus != TrackStockStatusE.空砖)
            {
                Growl.Warning("对应出轨道为空状态!");
                return;
            }

            MessageBoxResult rs = HandyControl.Controls.MessageBox.Show("确定要转移库存吗？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (rs == MessageBoxResult.Yes)
            {
                if (PubMaster.Goods.ShiftStock(_selecttrack.id, _selecttrack.brother_track_id))
                {
                    Growl.Success("转移成功！");
                    ActionStock("0");
                }
            }
        }

        /// <summary>
        /// 刷新轨道库存
        /// </summary>
        private void ActionStock(string tag)
        {
            if (_selecttrack == null) return;
            if (int.TryParse(tag, out int type))
            {
                switch (type)
                {

                    case 0://刷新
                        List.Clear();
                        List<Stock> stocks = PubMaster.Goods.GetStocks(_selecttrack.id);
                        foreach (Stock stock in stocks)
                        {
                            List.Add(stock);
                        }
                        break;
                    case 1://添加
                        if (List.Count == 0)
                        {
                            TrackGoodsSelected();
                        }
                        else
                        {
                            Stock stock = List[0];
                            ushort pis = stock.stack > 0 ? (ushort)(stock.pieces / stock.stack) : (ushort)1;
                            TrackStockAdd(stock.goods_id, pis);
                        }
                        break;
                    case 2://更换规格
                        ChangeGoodAsync();
                        break;
                    case 3://转移库存
                        ShiftStock();
                        break;
                }
            }

        }


        private async void ChangeGoodAsync()
        {
            uint area = _selecttrack.area;
            if (!PubMaster.Goods.ExistStockInTrack(_selecttrack.id))
            {
                Growl.Warning("当前轨道没有库存！");
                return;
            }

            if (List.Count == 0)
            {
                Growl.Warning("当前没有库存记录！");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
             .Initialize<GoodsSelectViewModel>((vm) =>
             {
                 vm.SetAreaFilter(area, false);
                 vm.QueryGood();
             }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && result.p2 is Goods good)
            {
                Stock stock = List[0];
                if (stock.goods_id == good.id)
                {
                    Growl.Warning("库存规格相同，不用修改！");
                    return;
                }
                ShowStockGoodEditDialog(_selecttrack.id, stock.goods_id, good.id);
            }
        }

        private async void ShowStockGoodEditDialog(uint trackid, uint oldgoodid, uint newgoodid)
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<StockGoodEditDialog>()
                .Initialize<StockGoodEditViewModel>((vm) =>
                {
                    vm.SetInitValue(trackid, oldgoodid, newgoodid);
                }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && rs && result.p2 is bool changedate)
            {
                string msg = "确定要更改规格吗？";
                DateTime? nd = null;
                if (changedate && result.p3 is DateTime newdate)
                {
                    msg += "更新时间为:" + newdate.ToString();
                    nd = newdate;
                }
                MessageBoxResult ars = HandyControl.Controls.MessageBox.Show(msg, "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (ars == MessageBoxResult.Yes)
                {
                    if (PubMaster.Goods.ChangeStockGood(trackid, newgoodid, changedate, nd))
                    {
                        Growl.Success("更改成功！");
                        ActionStock("0");
                    }
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
                     vm.QueryTrack(new List<TrackTypeE>() { TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入 });
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                _selecttrack = tra;
                SelectTrackName = tra.name;
                ActionStock("0");
            }
        }

        /// <summary>
        /// 选择规格
        /// </summary>
        private async void TrackGoodsSelected()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                 .Initialize<GoodsSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(_selecttrack.area, false);
                     vm.QueryGood();
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Goods good)
            {
                TrackStockAdd(good.id, good.pieces);
            }
        }

        /// <summary>
        /// 添加库存
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="picese"></param>
        private async void TrackStockAdd(uint gid, ushort picese)
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<StockEditDialog>()
                 .Initialize<StockEditViewModel>((vm) =>
                 {
                     vm.SetAddInput(gid, _selecttrack.id, picese);
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && rs)
            {
                Growl.Success("添加成功！");
                ActionStock("0");
            }
        }

        private void DeleteStock()
        {
            if (SelectStock == null)
            {
                Growl.Warning("请先选择库存记录");
                return;
            }

            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否确认删除库存记录", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (PubTask.Trans.IsStockInTrans(SelectStock.id, out string rs))
                {
                    Growl.Warning(rs);
                    return;
                }

                if (!PubMaster.Goods.DeleteStock(SelectStock.id, out rs))
                {
                    Growl.Warning(rs);
                    return;
                }

                ActionStock("0");
                Growl.Success("删除成功!");
            }
        }

        private async void AddFrontStock()
        {
            if (!CheckSelectItem()) return;
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                 .Initialize<GoodsSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(_selecttrack.area, false);
                     vm.QueryGood();
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Goods good)
            {
                TrackStockAdd(good.id, good.pieces);
            }
        }

        private async void AddBackStock()
        {
            if (!CheckSelectItem()) return;
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                 .Initialize<GoodsSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(_selecttrack.area, false);
                     vm.QueryGood();
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Goods good)
            {
                TrackStockAdd(good.id, good.pieces);
            }
        }

        private async void EditStock()
        {
            if (!CheckSelectItem()) return;
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                 .Initialize<GoodsSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(_selecttrack.area, false);
                     vm.QueryGood();
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Goods good)
            {
                TrackStockAdd(good.id, good.pieces);
            }
        }

        private bool CheckSelectItem()
        {
            if (SelectStock == null)
            {
                Growl.Warning("请先选择数据！");
                return false;
            }

            return true;
        }
        #endregion


    }
}
