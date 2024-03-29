﻿using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.deviceconfig;
using module.goods;
using module.msg;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            _sizelist = new ObservableCollection<GoodSize>();
            AreaRadio = PubMaster.Area.GetAreaRadioList(true);

            Messenger.Default.Register<MsgAction>(this, MsgToken.GoodsUpdate, GoodsUpdate);

            InitView();

            GoodListView = System.Windows.Data.CollectionViewSource.GetDefaultView(List);
            GoodListView.Filter = new Predicate<object>(OnFilterMovie);
            CheckIsSingle();

            #region [运输车]

            CarrierSettingViewList = new ObservableCollection<CarrierSettingView>();
            _currentcarrierlist = new List<CarrierSettingView>();

            RefreshCarrierList();
            #endregion

            InitSizeView();
        }


        #region[字段]
        private List<GoodsView> _list;
        private GoodsView _selectgood;
        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private bool showareafilter = true;

        private ObservableCollection<GoodSize> _sizelist;
        private GoodSize _selectsize;

        #region [运输车]
        private CarrierSettingView _selectcarrier;
        private List<CarrierSettingView> _currentcarrierlist { set; get; }

        #endregion

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

        #region【品种】
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

        #region【规格】
        public ObservableCollection<GoodSize> SizeList
        {
            get => _sizelist;
            set => Set(ref _sizelist, value);
        }

        public GoodSize SelectSize
        {
            get => _selectsize;
            set => Set(ref _selectsize, value);
        }
        #endregion

        #region[运输车]

        public ObservableCollection<CarrierSettingView> CarrierSettingViewList { set; get; }

        public CarrierSettingView SelectCarrier
        {
            get => _selectcarrier;
            set => Set(ref _selectcarrier, value);
        }

        #endregion

        #endregion

        #region[命令]
        public RelayCommand<string> GoodsEditeCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(GoodsEdite)).Value;
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand GoodSizeSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(GoodSizeSelected)).Value;

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint areaid))
            {
                ShowAreaFileter = false;
                filterareaid = areaid;
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
                        List.Sort((x, y) =>
                        {
                            return (x.UpdateTime is DateTime xtime && y.UpdateTime is DateTime ytime) ? ytime.CompareTo(xtime) : y.ID.CompareTo(x.ID);
                        });
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

        private void InitView(string str = null)
        {
            List.Clear();
            List<Goods> list = PubMaster.Goods.GetGoodsList();
            foreach (Goods goods in list)
            {
                List.Add(new GoodsView(goods));
            }
        }

        private void RefreshGoodList(string str)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                List.Clear();
                List<Goods> list = PubMaster.Goods.GetGoodsList();
                foreach (Goods goods in list)
                {
                    List.Add(new GoodsView(goods));
                }
                GoodListView.Refresh();
            });
        }

        bool OnFilterMovie(object item)
        {
            //if (filterareaid == 0) return true;
            //if (item is GoodsView view)
            //{
            //    return view.AreaId == filterareaid;
            //}
            return true;
        }

        private async void GoodsEdite(string tag)
        {
            switch (tag)
            {
                #region[刷新品种]
                case "refresh":
                    InitView();
                    GoodListView.Refresh();
                    break;
                #endregion

                #region[添加品种]
                case "add":
                    DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsEditDialog>()
                              .Initialize<GoodsEditViewModel>((vm) =>
                              {
                                  vm.SetActionType(true, filterareaid);
                              }).GetResultAsync<DialogResult>();
                    if (result.p1 is bool rs && rs)
                    {
                        Growl.Success("添加成功！");
                        InitView();
                    }
                    break;
                #endregion

                #region[编辑品种]
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
                        InitView();
                    }
                    break;
                #endregion

                #region[删除品种]
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

                    InitView();
                    break;
                #endregion

                #region[刷新规格]
                case "refreshsize":
                    InitSizeView();
                    break;
                #endregion

                #region[添加规格]
                case "addsize":

                    result = await HandyControl.Controls.Dialog.Show<GoodSizeEditDialog>()
                              .Initialize<GoodSizeEditViewModel>((vm) => { 
                                  vm.SetAdd(true); 
                              })
                              .GetResultAsync<DialogResult>();
                    if (result.p1 is bool addrs && addrs)
                    {
                        Growl.Success("添加成功！");
                        InitSizeView();
                    }

                    break;
                    
                 #endregion

                #region[修改规格]
                case "editesize":
                    if(SelectSize == null)
                    {
                        Growl.Warning("请先选择规格后进行修改！");
                        return;
                    }
                    result = await HandyControl.Controls.Dialog.Show<GoodSizeEditDialog>()
                              .Initialize<GoodSizeEditViewModel>((vm) => {
                                  vm.SetAdd(false);
                                  vm.SetEditSize(SelectSize);
                              })
                              .GetResultAsync<DialogResult>();
                    if (result.p1 is bool isedit && isedit)
                    {
                        Growl.Success("修改成功！");
                        InitSizeView();
                    }



                    break;

                #endregion

                #region[删除规格]
                case "deletesize":

                    if (SelectSize == null)
                    {
                        Growl.Warning("请先选择规格！");
                        return;
                    }

                    if (!PubMaster.Goods.DeleteSize(SelectSize, out res))
                    {
                        Growl.Warning(res);
                        return;
                    }

                    Growl.Success("删除成功！");
                    InitSizeView();

                    break;

                    #endregion
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

        #region[规格]

        private void InitSizeView()
        {
            List<GoodSize> sizes = PubMaster.Goods.GetGoodSizes();
            SizeList.Clear();
            foreach (var item in sizes)
            {
                SizeList.Add(item);
            }
        }

        #endregion

        #region [运输车规格绑定]

        /// <summary>
        /// 刷新运输车及绑定的规格
        /// </summary>
        public void RefreshCarrierList()
        {
            List<Device> devs = PubMaster.Device.GetDevices(DeviceTypeE.运输车);
            List<uint> devids = PubMaster.Device.GetDevIds(DeviceTypeE.运输车);

            
            List<ConfigCarrier> contiles = PubMaster.DevConfig.GetCarrierList(devids);

            CarrierSettingViewList.Clear();
            _currentcarrierlist.Clear();
            for (int i = 0; i < devs.Count; i++)
            {
                Device dev = devs[i];
                ConfigCarrier config = contiles[i];
                CarrierSettingView t = new CarrierSettingView(dev, config);
                CarrierSettingViewList.Add(t);
                _currentcarrierlist.Add(t);
            }
        }

        public async void GoodSizeSelected()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodSizeSelectDialog>()
                .Initialize<GoodSizeSelectViewModel>((vm) =>
                {
                    vm.QuerySize(SelectCarrier.goods_size);
                    vm.SetIsBatch(true);
                }).GetResultAsync<DialogResult>();

            if (result.p2 is List<string> goods_size_ids)
            {
                CarrierSettingView c = SelectCarrier;
                
                if (goods_size_ids.Count == 0)
                {
                    c.goods_size = "";
                    c.GetSizeName();
                }
                else
                {
                    string ids = string.Join("#", goods_size_ids);
                    c.goods_size = ids;
                    c.GetSizeName();
                }

                int num = CarrierSettingViewList.IndexOf(SelectCarrier);
                CarrierSettingViewList.Remove(SelectCarrier);
                CarrierSettingViewList.Insert(num, c);
                SaveCarrierSingle(c);
            }
        }

        public bool SaveCarrierSingle(CarrierSettingView carrier)
        {
            bool success = false;

            //转换成Device保存
            Device dev = carrier.TransformIntoDevice();
            success = PubMaster.Device.UpdateDevice(dev);

            //转换成Config保存
            ConfigCarrier config = carrier.TransformIntoConfigCarrier();
            success = PubMaster.DevConfig.UpdateConfigCarrier(config);

            return success;
        }
        #endregion
    }
}
