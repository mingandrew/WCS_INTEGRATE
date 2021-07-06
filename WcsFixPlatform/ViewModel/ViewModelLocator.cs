using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using resource;
using System;
using System.Windows;
using task;
using tool.appconfig;
using wcs.Service;
using wcs.ViewModel.platform.device;

namespace wcs.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default); 
            SimpleIoc.Default.Register<DataService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<WaringViewModel>();
            SimpleIoc.Default.Register<WarnLogViewModel>();
            SimpleIoc.Default.Register<OperateGrandDialogViewModel>();
            SimpleIoc.Default.Register<CheckRightDialogViewModel>();

            SimpleIoc.Default.Register<DictionViewModel>();
            SimpleIoc.Default.Register<DictionDtlEditViewModel>();
            SimpleIoc.Default.Register<DictionEditViewModel>();
            SimpleIoc.Default.Register<DictionSelectViewModel>();

            SimpleIoc.Default.Register<AreaViewModel>();
            SimpleIoc.Default.Register<AreaLineSwitchModel>();
            SimpleIoc.Default.Register<AreaSwitchViewModel>();
            SimpleIoc.Default.Register<TileLifterViewModel>();
            SimpleIoc.Default.Register<FerryViewModel>();
            SimpleIoc.Default.Register<CarrierViewModel>();
            SimpleIoc.Default.Register<DeviceSelectViewModel>();
            SimpleIoc.Default.Register<DeviceBackupSelectViewModel>();
            SimpleIoc.Default.Register<FerryPosViewModel>();
            SimpleIoc.Default.Register<RfClientViewModel>();
            SimpleIoc.Default.Register<TrackViewModel>();
            SimpleIoc.Default.Register<TrackSelectViewModel>();
            SimpleIoc.Default.Register<ChangeStrategyDialogViewModel>();
            SimpleIoc.Default.Register<FerryAutoPosDialogViewModel>();
            SimpleIoc.Default.Register<FerryCopyPosDialogViewModel>();
            SimpleIoc.Default.Register<CutoverDialogViewModel>();
            SimpleIoc.Default.Register<GoodShiftDialogViewModel>();
            SimpleIoc.Default.Register<Carrier2TileLifterViewModel>();

            SimpleIoc.Default.Register<GoodsViewModel>();
            SimpleIoc.Default.Register<GoodsEditViewModel>();
            SimpleIoc.Default.Register<GoodsSelectViewModel>();
            SimpleIoc.Default.Register<StockSelectViewModel>();
            SimpleIoc.Default.Register<GoodSizeSelectViewModel>();
            SimpleIoc.Default.Register<StockViewModel>();
            SimpleIoc.Default.Register<StockSumViewModel>();
            SimpleIoc.Default.Register<StockEditViewModel>();
            SimpleIoc.Default.Register<StockGoodEditViewModel>();
            SimpleIoc.Default.Register<DeleteQtyViewModel>();
            SimpleIoc.Default.Register<TransViewModel>();
            SimpleIoc.Default.Register<TestGoodViewModel>();
            SimpleIoc.Default.Register<AddManualTransViewModel>();
            SimpleIoc.Default.Register<TrackAllocateViewModel>();
            SimpleIoc.Default.Register<TrackLogViewModel>();
            SimpleIoc.Default.Register<TileTrackViewModel>();
            SimpleIoc.Default.Register<TrackSetPointViewModel>();

            SimpleIoc.Default.Register<MenuViewModel>();
            SimpleIoc.Default.Register<UserViewModel>();
            SimpleIoc.Default.Register<ModuleSelectViewModel>();
            SimpleIoc.Default.Register<MenuSelectViewModel>();
            SimpleIoc.Default.Register<ToolBarViewModel>();
            SimpleIoc.Default.Register<UserEditViewModel>();
            SimpleIoc.Default.Register<DeviceEditViewModel>();
            SimpleIoc.Default.Register<DeviceViewModel>();
            SimpleIoc.Default.Register<CarrierPosViewModel>();

            SimpleIoc.Default.Register<SimulationViewModel>();
            SimpleIoc.Default.Register<OrganizeTrackViewModel>();

            GlobalWcsDataConfig.Init();
            PubMaster.Init();
            PubTask.Init();
        }

        public static ViewModelLocator Instance => new Lazy<ViewModelLocator>(() => Application.Current.TryFindResource("Locator") as ViewModelLocator).Value;

        public MainViewModel Main =>ServiceLocator.Current.GetInstance<MainViewModel>();
        public HomeViewModel Home => ServiceLocator.Current.GetInstance<HomeViewModel>();
        public WaringViewModel Warn => ServiceLocator.Current.GetInstance<WaringViewModel>();
        public WarnLogViewModel WarnLog => ServiceLocator.Current.GetInstance<WarnLogViewModel>();
        public OperateGrandDialogViewModel OperateGrand => ServiceLocator.Current.GetInstance<OperateGrandDialogViewModel>();
        public CheckRightDialogViewModel CheckRight => ServiceLocator.Current.GetInstance<CheckRightDialogViewModel>();

        #region[字典]
        public DictionViewModel Dic => ServiceLocator.Current.GetInstance<DictionViewModel>();
        public DictionEditViewModel DicEdit => ServiceLocator.Current.GetInstance<DictionEditViewModel>();
        public DictionDtlEditViewModel DicDtlEdit => ServiceLocator.Current.GetInstance<DictionDtlEditViewModel>();
        public DictionSelectViewModel DicSelect => ServiceLocator.Current.GetInstance<DictionSelectViewModel>();
        #endregion

        #region[设备/硬件]
        public AreaViewModel Area => ServiceLocator.Current.GetInstance<AreaViewModel>();
        public AreaLineSwitchModel AreaLineSwitch => ServiceLocator.Current.GetInstance<AreaLineSwitchModel>();
        public DeviceViewModel Device => ServiceLocator.Current.GetInstance<DeviceViewModel>();
        public DeviceEditViewModel DevEdit => ServiceLocator.Current.GetInstance<DeviceEditViewModel>();
        public AreaSwitchViewModel AreaSwitch => ServiceLocator.Current.GetInstance<AreaSwitchViewModel>();
        public TileLifterViewModel TileLifter => ServiceLocator.Current.GetInstance<TileLifterViewModel>();
        public FerryViewModel Ferry => ServiceLocator.Current.GetInstance<FerryViewModel>();
        public CarrierViewModel Carrier => ServiceLocator.Current.GetInstance<CarrierViewModel>();
        public DeviceSelectViewModel DeviceSelect => ServiceLocator.Current.GetInstance<DeviceSelectViewModel>();
        public DeviceBackupSelectViewModel DeviceBackupSelect => ServiceLocator.Current.GetInstance<DeviceBackupSelectViewModel>();
        public FerryPosViewModel FerryPos => ServiceLocator.Current.GetInstance<FerryPosViewModel>();
        public RfClientViewModel RfClient => ServiceLocator.Current.GetInstance<RfClientViewModel>();
        public TrackViewModel Track => ServiceLocator.Current.GetInstance<TrackViewModel>();
        public TrackLogViewModel TrackLog => ServiceLocator.Current.GetInstance<TrackLogViewModel>();
        public TrackSelectViewModel TrackSelect => ServiceLocator.Current.GetInstance<TrackSelectViewModel>();
        public TrackSetPointViewModel TrackSetPoint => ServiceLocator.Current.GetInstance<TrackSetPointViewModel>();
        public ChangeStrategyDialogViewModel StrategyChange => ServiceLocator.Current.GetInstance<ChangeStrategyDialogViewModel>();
        public FerryAutoPosDialogViewModel FerryAutoPos => ServiceLocator.Current.GetInstance<FerryAutoPosDialogViewModel>();
        public FerryCopyPosDialogViewModel FerryCopyPos => ServiceLocator.Current.GetInstance<FerryCopyPosDialogViewModel>();
        public CutoverDialogViewModel Cutover => ServiceLocator.Current.GetInstance<CutoverDialogViewModel>();
        public GoodShiftDialogViewModel GoodShift => ServiceLocator.Current.GetInstance<GoodShiftDialogViewModel>();
        public CarrierPosViewModel CarrierPos => ServiceLocator.Current.GetInstance<CarrierPosViewModel>();
        public Carrier2TileLifterViewModel Carrier2TileLifter => ServiceLocator.Current.GetInstance<Carrier2TileLifterViewModel>();
        #endregion

        #region[品种/库存/交易]

        public GoodsViewModel Goods => ServiceLocator.Current.GetInstance<GoodsViewModel>();
        public GoodsEditViewModel GoodsEdit => ServiceLocator.Current.GetInstance<GoodsEditViewModel>();
        public GoodsSelectViewModel GoodSelect => ServiceLocator.Current.GetInstance<GoodsSelectViewModel>(); 
        public StockSelectViewModel StockSelect => ServiceLocator.Current.GetInstance<StockSelectViewModel>();
        public GoodSizeSelectViewModel SizeSelect => ServiceLocator.Current.GetInstance<GoodSizeSelectViewModel>();
        public TransViewModel Trans => ServiceLocator.Current.GetInstance<TransViewModel>();
        public StockViewModel Stock => ServiceLocator.Current.GetInstance<StockViewModel>();
        public StockSumViewModel StockSum => ServiceLocator.Current.GetInstance<StockSumViewModel>();
        public StockEditViewModel StockEdit => ServiceLocator.Current.GetInstance<StockEditViewModel>();
        public StockGoodEditViewModel StockGoodEdit => ServiceLocator.Current.GetInstance<StockGoodEditViewModel>();
        public TestGoodViewModel TestGood => ServiceLocator.Current.GetInstance<TestGoodViewModel>();
        public AddManualTransViewModel AddManualTrans => ServiceLocator.Current.GetInstance<AddManualTransViewModel>();
        public TrackAllocateViewModel StockAllocate => ServiceLocator.Current.GetInstance<TrackAllocateViewModel>();
        public TileTrackViewModel TileTrack => ServiceLocator.Current.GetInstance<TileTrackViewModel>();
        public DeleteQtyViewModel DelectQty => ServiceLocator.Current.GetInstance<DeleteQtyViewModel>();
        #endregion

        #region[菜单-角色-用户]

        public MenuViewModel Menu => ServiceLocator.Current.GetInstance<MenuViewModel>();
        public UserViewModel User => ServiceLocator.Current.GetInstance<UserViewModel>();
        public ModuleSelectViewModel ModuleSelect => ServiceLocator.Current.GetInstance<ModuleSelectViewModel>();
        public MenuSelectViewModel MenuSelect => ServiceLocator.Current.GetInstance<MenuSelectViewModel>();
        public ToolBarViewModel ToolBar => ServiceLocator.Current.GetInstance<ToolBarViewModel>();
        public UserEditViewModel UserEdit => ServiceLocator.Current.GetInstance<UserEditViewModel>();

        #endregion

        #region[模拟系统]

        public SimulationViewModel Simulation => ServiceLocator.Current.GetInstance<SimulationViewModel>();
        #endregion

        #region[库存整理]

        public OrganizeTrackViewModel OrganizeTrack => ServiceLocator.Current.GetInstance<OrganizeTrackViewModel>();

        #endregion

        public static void Cleanup()
        {
                // TODO Clear the ViewModels
        }
    }
}