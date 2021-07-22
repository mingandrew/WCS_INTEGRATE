using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace wcs.ViewModel.platform.track
{
    public class CarrierPosSelectViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public CarrierPosSelectViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _carposlist = new ObservableCollection<CarrierPos>();

            CarPosView = System.Windows.Data.CollectionViewSource.GetDefaultView(CarPosList);

        }

        #region[字段]

        private DialogResult _result;
        private ObservableCollection<CarrierPos> _carposlist;
        private CarrierPos carposselected;
        #endregion

        #region[属性]
        public CarrierPos CarPosSelected
        {
            get => carposselected;
            set => Set(ref carposselected, value);
        }

        public ICollectionView CarPosView { set; get; }

        public ObservableCollection<CarrierPos> CarPosList
        {
            get => _carposlist;
            set => Set(ref _carposlist, value);
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

        #endregion

        #region[方法]

        public void QueryCarrierPos(uint area, TrackTypeE tt)
        {
            List<CarrierPos> cps = PubMaster.Track.GetCarrierPosList(area);
            Application.Current.Dispatcher.Invoke(() =>
            {
                CarPosList.Clear();
                foreach (CarrierPos cp in cps)
                {
                    switch (tt)
                    {
                        case TrackTypeE.储砖_入:
                        case TrackTypeE.储砖_出:
                        case TrackTypeE.储砖_出入:
                            if (!cp.InType(CarrierPosE.轨道前侧复位点, CarrierPosE.轨道后侧复位点))
                            {
                                continue;
                            }
                            break;

                        case TrackTypeE.后置摆渡轨道:
                            if (cp.CarrierPosType != CarrierPosE.后置摆渡复位点)
                            {
                                continue;
                            }
                            break;
                        case TrackTypeE.前置摆渡轨道:
                            if (cp.CarrierPosType != CarrierPosE.前置摆渡复位点)
                            {
                                continue;
                            }
                            break;
                        default:
                            break;
                    }

                    CarPosList.Add(cp);
                }
            });
        }

        private void Comfirm()
        {
            if (CarPosSelected == null)
            {
                Growl.Warning("请选择！");
                return;
            }
            Result.p1 = CarPosSelected;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = null;
            CloseAction?.Invoke();
        }
        #endregion
    }
}
