using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace wcs.ViewModel
{
    public class GoodSizeSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public GoodSizeSelectViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            SizeList = new ObservableCollection<GoodSize>();

            ListView = System.Windows.Data.CollectionViewSource.GetDefaultView(SizeList);
            ListView.Filter = new Predicate<object>(OnFilterMovie);

            QuerySize();
        }

        #region[字段]

        private DialogResult _result;
        private GoodSize selectsize;

        private uint filterwidth = 0;

        //是否要能多选
        private bool isbatch;
        private bool isselectall;
        #endregion

        #region[属性]
        public ICollectionView ListView { set; get; }

        private ObservableCollection<GoodSize> SizeList { set; get; }

        public GoodSize SelectSize
        {
            get => selectsize;
            set
            {
                Set(ref selectsize, value);
            }
        }

        /// <summary>
        /// 多选
        /// </summary>
        public bool IsBatch
        {
            get => isbatch;
            set => Set(ref isbatch, value);
        }

        public bool IsSelectAll
        {
            get => isselectall;
            set => Set(ref isselectall, value);
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

        public Action CloseAction { get; set; }
        #endregion

        #region[命令]        
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        public RelayCommand<RoutedEventArgs> CheckWidthRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckWidthRadioBtn)).Value;
        public RelayCommand CheckAllCmd => new Lazy<RelayCommand>(() => new RelayCommand(SetAllCheck)).Value;

        #endregion

        #region[方法]

        bool OnFilterMovie(object item)
        {
            if (filterwidth == 0) return true;

            if (item is GoodSize view)
            {
                return filterwidth == view.width;
            }
            return true;
        }

        public void QuerySize(string ids = null)
        {
            List<GoodSize> list = PubMaster.Goods.GetGoodSizes();
            Application.Current.Dispatcher.Invoke(() =>
            {
                SizeList.Clear();

                #region [根据运输车的规格id集合判断是否被选中]

                uint[] sizeidList = null;
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    sizeidList = Array.ConvertAll(ids.Split('#'), uint.Parse);
                }

                #endregion

                foreach (GoodSize mod in list)
                {
                    #region [运输车规格选中则IsSelected为true]

                    if (sizeidList != null && Array.IndexOf(sizeidList, mod.id) != -1)
                    {
                        mod.IsSelected = true;
                    }
                    else
                    {
                        mod.IsSelected = false;
                    }

                    #endregion

                    SizeList.Add(mod);
                }
            });
        }

        /// <summary>
        /// 设置是否多选
        /// </summary>
        /// <param name="isbat"></param>
        public void SetIsBatch(bool isbat)
        {
            IsBatch = isbat;
        }

        private void Comfirm()
        {
            if (SelectSize == null && !IsBatch)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = SelectSize;
            Result.p2 = null;

            #region [传送被选中的规格id]

            List<string> gsids = new List<string>();
            foreach (GoodSize gs in SizeList)
            {
                if (gs.IsSelected)
                {
                    gsids.Add(gs.id.ToString());
                    gs.IsSelected = false;
                }
            }
            if (IsBatch)
            {
                Result.p2 = gsids;
            }

            #endregion

            CloseAction?.Invoke();
        }


        private void SetCheck(GoodSize view)
        {
            if (view != null)
            {
                view.IsSelected = !view.IsSelected;
            }
        }

        private void SetUnCheck()
        {
            foreach (GoodSize item in SizeList.Where(c => c.IsSelected == true))
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
                foreach (GoodSize item in SizeList.Where(c => c.IsSelected == false))
                {
                    item.IsSelected = true;

                }
                IsSelectAll = true;
            }
        }

        private void CancelChange()
        {
            Result.p1 = null;
            CloseAction?.Invoke();
        }

        private void CheckWidthRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (uint.TryParse(btn.Tag.ToString(), out uint width))
                {
                    filterwidth = width;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ListView.Refresh();
                    });
                }
            }
        }

        #endregion
    }
}
