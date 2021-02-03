using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.msg;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class GoodsViewModel : ViewModelBase
    {
        public GoodsViewModel()
        {
            _list = new List<GoodsView>();
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);

            Messenger.Default.Register<MsgAction>(this, MsgToken.GoodsUpdate, GoodsUpdate);
            InitView();

            GoodListView = System.Windows.Data.CollectionViewSource.GetDefaultView(List);
            GoodListView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();
        }


        #region[字段]
        private List<GoodsView> _list;
        private GoodsView _selectgood;
        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private bool showareafilter = true;
        #endregion

        #region[属性]
        public bool ShowAreaFileter
        {
            get => showareafilter;
            set => Set(ref showareafilter, value);
        }
        public IList<MyRadioBtn> AreaRadio
        {
            get => _arearadio;
            set => Set(ref _arearadio, value);
        }
        public ICollectionView GoodListView { set; get; }

        public List<GoodsView> List
        {
            get => _list;
            set => Set(ref _list, value);
        }

        public GoodsView SelectGood
        {
            get => _selectgood;
            set => Set(ref _selectgood, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<string> GoodsEditeCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(GoodsEdite)).Value;
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint aid))
            {
                ShowAreaFileter = false;
            }
        }
        private void GoodsUpdate(MsgAction msg)
        {
            if (msg.o1 is Goods goods && msg.o2 is ActionTypeE type)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    GoodsView view = List.Find(c => c.ID == goods.id);
                    if (view == null)
                    {
                        view = new GoodsView(goods);
                        List.Add(view);
                    }
                    switch (type)
                    {
                        case ActionTypeE.Add:
                        case ActionTypeE.Update:
                            view.Update(goods);
                            break;
                        case ActionTypeE.Delete:
                            List.Remove(view);
                            break;
                    }
                    GoodListView.Refresh();
                });
            }
        }

        private void InitView()
        {
            List.Clear();
            List<Goods> list = PubMaster.Goods.GetGoodsList();
            foreach (Goods goods in list)
            {
                List.Add(new GoodsView(goods));
            }
        }

        bool OnFilterMovie(object item)
        {
            if (filterareaid == 0) return true;
            if (item is GoodsView view)
            {
                return view.AreaId == filterareaid;
            }
            return true;
        }

        private async void GoodsEdite(string tag)
        {
            switch (tag)
            {
                case "add":
                    DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsEditDialog>()
                              .Initialize<GoodsEditViewModel>((vm) =>
                              {
                                  vm.SetActionType(true, filterareaid);
                              }).GetResultAsync<DialogResult>();
                    if (result.p1 is bool rs && rs)
                    {
                        Growl.Success("添加成功！");
                    }
                    break;
                case "edite":
                    if (SelectGood == null)
                    {
                        Growl.Warning("请先选择品种！");
                        return;
                    }

                    if (SelectGood.empty)
                    {
                        Growl.Warning("不能编辑空品种！");
                        return;
                    }

                    result = await HandyControl.Controls.Dialog.Show<GoodsEditDialog>()
                              .Initialize<GoodsEditViewModel>((vm) =>
                              {
                                  vm.Param.p1 = SelectGood;
                                  vm.SetActionType(false, filterareaid);
                              }).GetResultAsync<DialogResult>();
                    if (result.p1 is bool rs2 && rs2)
                    {
                        Growl.Success("修改成功！");
                    }
                    break;
                case "delete":
                    if (SelectGood == null)
                    {
                        Growl.Warning("请先选择品种！");
                        return;
                    }

                    if (SelectGood.empty)
                    {
                        Growl.Warning("不能删除空品种！");
                        return;
                    }

                    if (PubMaster.Goods.DeleteGood(SelectGood.ID, out string res))
                    {
                        Growl.Success(res);
                    }
                    else
                    {
                        Growl.Warning(res);
                    }

                    break;
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint areaid))
                {
                    filterareaid = areaid;
                    GoodListView.Refresh();
                }
            }
        }
        #endregion
    }
}
