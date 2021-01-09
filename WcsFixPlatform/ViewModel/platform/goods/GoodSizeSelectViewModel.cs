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
        #endregion

        #region[属性]
        public ICollectionView ListView { set; get; }

        private ObservableCollection<GoodSize> SizeList { set; get; }

        public GoodSize SelectSize
        {
            get => selectsize;
            set => Set(ref selectsize, value);
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

        public void QuerySize()
        {
            List<GoodSize> list = PubMaster.Goods.GetGoodSizes();
            Application.Current.Dispatcher.Invoke(() =>
            {
                SizeList.Clear();
                foreach (GoodSize mod in list)
                {
                    SizeList.Add(mod);
                }
            });
        }

        private void Comfirm()
        {
            if (SelectSize == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = SelectSize;
            CloseAction?.Invoke();
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
