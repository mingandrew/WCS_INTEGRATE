using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HandyControl.Tools;

namespace wcs.Resources.Langs
{
    public class LangProvider : INotifyPropertyChanged
    {
        internal static LangProvider Instance => ResourceHelper.GetResource<LangProvider>("WcsLangs");

        private static string CultureInfoStr;

        public static CultureInfo Culture
        {
            get => Lang.Culture;
            set
            {
                if (value == null) return;
                if (Equals(CultureInfoStr, value.EnglishName)) return;
                Lang.Culture = value;
                CultureInfoStr = value.EnglishName;

                Instance.UpdateLangs();
            }
        }

        public static string GetLang(string key) => Lang.ResourceManager.GetString(key, Culture);

        public static void SetLang(DependencyObject dependencyObject, DependencyProperty dependencyProperty, string key) =>
            BindingOperations.SetBinding(dependencyObject, dependencyProperty, new Binding(key)
            {
                Source = Instance,
                Mode = BindingMode.OneWay
            });

		private void UpdateLangs()
        {
			OnPropertyChanged(nameof(Accordingtoproduct));
			OnPropertyChanged(nameof(Accordingtotrack));
			OnPropertyChanged(nameof(Addininventory));
			OnPropertyChanged(nameof(Addinventorysignal));
			OnPropertyChanged(nameof(Adduser));
			OnPropertyChanged(nameof(Administratormenu));
			OnPropertyChanged(nameof(All));
			OnPropertyChanged(nameof(Am));
			OnPropertyChanged(nameof(AppTitle));
			OnPropertyChanged(nameof(Authorizationmanagement));
			OnPropertyChanged(nameof(Autoaligning));
			OnPropertyChanged(nameof(Automode));
			OnPropertyChanged(nameof(Backposition));
			OnPropertyChanged(nameof(Backsensor));
			OnPropertyChanged(nameof(Basicmenu));
			OnPropertyChanged(nameof(BWD));
			OnPropertyChanged(nameof(BWDreset));
			OnPropertyChanged(nameof(BWDtoexternaltilerelease));
			OnPropertyChanged(nameof(BWDtoferrytrolley));
			OnPropertyChanged(nameof(BWDtointernaltilerelease));
			OnPropertyChanged(nameof(BWDtopointedposition));
			OnPropertyChanged(nameof(Cancel));
			OnPropertyChanged(nameof(Changeproduct));
			OnPropertyChanged(nameof(Clear));
			OnPropertyChanged(nameof(Clear1));
			OnPropertyChanged(nameof(Close));
			OnPropertyChanged(nameof(CloseAll));
			OnPropertyChanged(nameof(CloseOther));
			OnPropertyChanged(nameof(Colornumber));
			OnPropertyChanged(nameof(Communicationconnected));
			OnPropertyChanged(nameof(Communicationdisconnected));
			OnPropertyChanged(nameof(Communicationnormal));
			OnPropertyChanged(nameof(Communicationstatus));
			OnPropertyChanged(nameof(Communicationsuccessful));
			OnPropertyChanged(nameof(Completedinstruction));
			OnPropertyChanged(nameof(Configurationmanagement));
			OnPropertyChanged(nameof(Confirm));
			OnPropertyChanged(nameof(Connected));
			OnPropertyChanged(nameof(Conveyingtrolley));
			OnPropertyChanged(nameof(Coordinatesset));
			OnPropertyChanged(nameof(Curentcoordinates));
			OnPropertyChanged(nameof(Currentcoordinatesrenew));
			OnPropertyChanged(nameof(Currentinstruction));
			OnPropertyChanged(nameof(Currentlayer));
			OnPropertyChanged(nameof(Currentposition));
			OnPropertyChanged(nameof(Currentpositioncode));
			OnPropertyChanged(nameof(Currentproduct));
			OnPropertyChanged(nameof(Currenttrack));
			OnPropertyChanged(nameof(Deleteinventory));
			OnPropertyChanged(nameof(Deleteuser));
			OnPropertyChanged(nameof(Destinationcoordinates));
			OnPropertyChanged(nameof(Destinationposition));
			OnPropertyChanged(nameof(Destinationposition1));
			OnPropertyChanged(nameof(Destinationstation));
			OnPropertyChanged(nameof(Directory));
			OnPropertyChanged(nameof(Disconnected));
			OnPropertyChanged(nameof(Drivedisconnected));
			OnPropertyChanged(nameof(Emptytrack));
			OnPropertyChanged(nameof(Entering));
			OnPropertyChanged(nameof(Enterwarehousetrack));
			OnPropertyChanged(nameof(Equipment));
			OnPropertyChanged(nameof(Equipmenttype));
			OnPropertyChanged(nameof(Excellentproduct));
			OnPropertyChanged(nameof(Exitingwarehousetrack));
			OnPropertyChanged(nameof(Fault));
			OnPropertyChanged(nameof(Ferrytrolleymoving));
			OnPropertyChanged(nameof(Ferrytrolleytotrack));
			OnPropertyChanged(nameof(Ferrytrolleytracktransferring));
			OnPropertyChanged(nameof(Find));
			OnPropertyChanged(nameof(First_gradeproduct));
			OnPropertyChanged(nameof(Front_backedgelength));
			OnPropertyChanged(nameof(Frontposition));
			OnPropertyChanged(nameof(Frontsensor));
			OnPropertyChanged(nameof(Fulltrack));
			OnPropertyChanged(nameof(Functiondescription));
			OnPropertyChanged(nameof(FWD));
			OnPropertyChanged(nameof(FWDreset));
			OnPropertyChanged(nameof(FWDtoferrytrolley));
			OnPropertyChanged(nameof(FWDtopointedposition));
			OnPropertyChanged(nameof(Grade));
			OnPropertyChanged(nameof(Gradeset));
			OnPropertyChanged(nameof(Inconnecting));
			OnPropertyChanged(nameof(Initializing));
			OnPropertyChanged(nameof(Insertininvertorybackward));
			OnPropertyChanged(nameof(Insertininvertoryforward));
			OnPropertyChanged(nameof(Instructionabnormal));
			OnPropertyChanged(nameof(Inventorynumber));
			OnPropertyChanged(nameof(Inventoryposition));
			OnPropertyChanged(nameof(Inventoryproducts));
			OnPropertyChanged(nameof(Inventoryrenew));
			OnPropertyChanged(nameof(Inventorytransferring));
			OnPropertyChanged(nameof(Jump));
			OnPropertyChanged(nameof(L_Redgelength));
			OnPropertyChanged(nameof(Leaving));
			OnPropertyChanged(nameof(Loading_unloadingmachine));
			OnPropertyChanged(nameof(Loadingcoordinates));
			OnPropertyChanged(nameof(Loadingferrytrack));
			OnPropertyChanged(nameof(Loadingferrytrolley));
			OnPropertyChanged(nameof(Loadingmachine));
			OnPropertyChanged(nameof(LoadingONLY));
			OnPropertyChanged(nameof(Loadingposition));
			OnPropertyChanged(nameof(Loadingstatus));
			OnPropertyChanged(nameof(Loadingstatus1));
			OnPropertyChanged(nameof(Loadingstatus2));
			OnPropertyChanged(nameof(Loadingtask));
			OnPropertyChanged(nameof(Loadingtrack));
			OnPropertyChanged(nameof(Loadingtrolley));
			OnPropertyChanged(nameof(Login));
			OnPropertyChanged(nameof(LoginUser));
			OnPropertyChanged(nameof(Manualloading));
			OnPropertyChanged(nameof(Manualmode));
			OnPropertyChanged(nameof(Manualunloading));
			OnPropertyChanged(nameof(Max_tilelayer));
			OnPropertyChanged(nameof(Menu));
			OnPropertyChanged(nameof(Modify));
			OnPropertyChanged(nameof(Modifyuser));
			OnPropertyChanged(nameof(Modifyworkingcondition));
			OnPropertyChanged(nameof(Narrowtrolley));
			OnPropertyChanged(nameof(NextPage));
			OnPropertyChanged(nameof(No));
			OnPropertyChanged(nameof(Noinstruction));
			OnPropertyChanged(nameof(Notempty));
			OnPropertyChanged(nameof(Notempty1));
			OnPropertyChanged(nameof(Notemptytrack));
			OnPropertyChanged(nameof(Off_linewarning));
			OnPropertyChanged(nameof(Operatingmode));
			OnPropertyChanged(nameof(Origindownunloading));
			OnPropertyChanged(nameof(Originuploading));
			OnPropertyChanged(nameof(Parentdirectory));
			OnPropertyChanged(nameof(Password));
			OnPropertyChanged(nameof(Passwordcheck));
			OnPropertyChanged(nameof(Pm));
			OnPropertyChanged(nameof(Positioninstruction));
			OnPropertyChanged(nameof(PreviousPage));
			OnPropertyChanged(nameof(Prioritygrade));
			OnPropertyChanged(nameof(Producingtime));
			OnPropertyChanged(nameof(Productchanged));
			OnPropertyChanged(nameof(Productchangestatus));
			OnPropertyChanged(nameof(Productchanging));
			OnPropertyChanged(nameof(Productiondescription));
			OnPropertyChanged(nameof(Productset));
			OnPropertyChanged(nameof(Pulsevalue));
			OnPropertyChanged(nameof(Qualifiedproduct));
			OnPropertyChanged(nameof(Readerdisconnectwarning));
			OnPropertyChanged(nameof(Realinventorycoordinates));
			OnPropertyChanged(nameof(Realposition));
			OnPropertyChanged(nameof(Requirementstatus));
			OnPropertyChanged(nameof(Reset));
			OnPropertyChanged(nameof(Resetinstruction));
			OnPropertyChanged(nameof(ReturingwarehouseBWD));
			OnPropertyChanged(nameof(ReturingwarehouseFWD));
			OnPropertyChanged(nameof(Returningtowarehouse));
			OnPropertyChanged(nameof(Returningtowarehouse1));
			OnPropertyChanged(nameof(Returntowarehouse));
			OnPropertyChanged(nameof(Role));
			OnPropertyChanged(nameof(Runningstatus));
			OnPropertyChanged(nameof(Samedirectionloading));
			OnPropertyChanged(nameof(Samedirectionunloading));
			OnPropertyChanged(nameof(Save));
			OnPropertyChanged(nameof(Saverenew));
			OnPropertyChanged(nameof(Schedulingequipment));
			OnPropertyChanged(nameof(Second_gradeproduct));
			OnPropertyChanged(nameof(Specification));
			OnPropertyChanged(nameof(Stackqty));
			OnPropertyChanged(nameof(Start));
			OnPropertyChanged(nameof(Start1));
			OnPropertyChanged(nameof(Starttime));
			OnPropertyChanged(nameof(Stop));
			OnPropertyChanged(nameof(Stop1));
			OnPropertyChanged(nameof(Stopinstruction));
			OnPropertyChanged(nameof(Stoptime));
			OnPropertyChanged(nameof(Strategymodify));
			OnPropertyChanged(nameof(String1));
			OnPropertyChanged(nameof(Taskcancelled));
			OnPropertyChanged(nameof(Taskcompleted));
			OnPropertyChanged(nameof(Taskswitch));
			OnPropertyChanged(nameof(Tasktype));
			OnPropertyChanged(nameof(Tasktype1));
			OnPropertyChanged(nameof(Tilecatching));
			OnPropertyChanged(nameof(Tilelack));
			OnPropertyChanged(nameof(TileloadingBWD));
			OnPropertyChanged(nameof(TileloadingFWD));
			OnPropertyChanged(nameof(Tileloadinginstruction));
			OnPropertyChanged(nameof(Tileqty));
			OnPropertyChanged(nameof(Tileqtyontrack));
			OnPropertyChanged(nameof(Tileready));
			OnPropertyChanged(nameof(Tilereleasing));
			OnPropertyChanged(nameof(TilereleasingFWD));
			OnPropertyChanged(nameof(Tilereleasinginstruction));
			OnPropertyChanged(nameof(Tilestoragetrack));
			OnPropertyChanged(nameof(Tip));
			OnPropertyChanged(nameof(TooLarge));
			OnPropertyChanged(nameof(Trackcheck));
			OnPropertyChanged(nameof(Trackdescription));
			OnPropertyChanged(nameof(Trackemptysignal));
			OnPropertyChanged(nameof(Trackend));
			OnPropertyChanged(nameof(Trackfullsignal));
			OnPropertyChanged(nameof(Trackhead));
			OnPropertyChanged(nameof(Trackmid));
			OnPropertyChanged(nameof(Trackreadingwarning));
			OnPropertyChanged(nameof(Trackstarted));
			OnPropertyChanged(nameof(Trackstatus));
			OnPropertyChanged(nameof(Trackstatussignal));
			OnPropertyChanged(nameof(Trackstopped));
			OnPropertyChanged(nameof(Tracktype));
			OnPropertyChanged(nameof(Trolleyempty));
			OnPropertyChanged(nameof(Trolleyqty));
			OnPropertyChanged(nameof(Truckmovement));
			OnPropertyChanged(nameof(Unknown));
			OnPropertyChanged(nameof(UnknownSize));
			OnPropertyChanged(nameof(Unloadingcoordinates));
			OnPropertyChanged(nameof(Unloadingferrytrack));
			OnPropertyChanged(nameof(Unloadingferrytrolley));
			OnPropertyChanged(nameof(Unloadingmachine));
			OnPropertyChanged(nameof(UnloadingONLY));
			OnPropertyChanged(nameof(Unloadingposition));
			OnPropertyChanged(nameof(Unloadingtask));
			OnPropertyChanged(nameof(Unloadingtrack));
			OnPropertyChanged(nameof(User));
			OnPropertyChanged(nameof(Warehouseinstrategy));
			OnPropertyChanged(nameof(Warehouseoutstrategy));
			OnPropertyChanged(nameof(Warehousereturningqty));
			OnPropertyChanged(nameof(Warningtype));
			OnPropertyChanged(nameof(Widetrolley));
			OnPropertyChanged(nameof(Workingcondition));
			OnPropertyChanged(nameof(Workingmodeshift));
			OnPropertyChanged(nameof(Workstationproduct));
			OnPropertyChanged(nameof(Yes));
			OnPropertyChanged(nameof(ZoomIn));
			OnPropertyChanged(nameof(ZoomOut));
        }

        /// <summary>
        ///   查找类似 依据品种作业 的本地化字符串。
        /// </summary>
		public string Accordingtoproduct => Lang.Accordingtoproduct;

        /// <summary>
        ///   查找类似 依据轨道作业 的本地化字符串。
        /// </summary>
		public string Accordingtotrack => Lang.Accordingtotrack;

        /// <summary>
        ///   查找类似 添加库存 的本地化字符串。
        /// </summary>
		public string Addininventory => Lang.Addininventory;

        /// <summary>
        ///   查找类似 添加库存信息 的本地化字符串。
        /// </summary>
		public string Addinventorysignal => Lang.Addinventorysignal;

        /// <summary>
        ///   查找类似 添加用户 的本地化字符串。
        /// </summary>
		public string Adduser => Lang.Adduser;

        /// <summary>
        ///   查找类似 管理员菜单 的本地化字符串。
        /// </summary>
		public string Administratormenu => Lang.Administratormenu;

        /// <summary>
        ///   查找类似 全部 的本地化字符串。
        /// </summary>
		public string All => Lang.All;

        /// <summary>
        ///   查找类似 上午 的本地化字符串。
        /// </summary>
		public string Am => Lang.Am;

        /// <summary>
        ///   查找类似 储砖调度系统 的本地化字符串。
        /// </summary>
		public string AppTitle => Lang.AppTitle;

        /// <summary>
        ///   查找类似 授权管理 的本地化字符串。
        /// </summary>
		public string Authorizationmanagement => Lang.Authorizationmanagement;

        /// <summary>
        ///   查找类似 自动对位 的本地化字符串。
        /// </summary>
		public string Autoaligning => Lang.Autoaligning;

        /// <summary>
        ///   查找类似 自动模式 的本地化字符串。
        /// </summary>
		public string Automode => Lang.Automode;

        /// <summary>
        ///   查找类似 后侧位置 的本地化字符串。
        /// </summary>
		public string Backposition => Lang.Backposition;

        /// <summary>
        ///   查找类似 后侧光电 的本地化字符串。
        /// </summary>
		public string Backsensor => Lang.Backsensor;

        /// <summary>
        ///   查找类似 基础菜单 的本地化字符串。
        /// </summary>
		public string Basicmenu => Lang.Basicmenu;

        /// <summary>
        ///   查找类似 后退 的本地化字符串。
        /// </summary>
		public string BWD => Lang.BWD;

        /// <summary>
        ///   查找类似 后退复位 的本地化字符串。
        /// </summary>
		public string BWDreset => Lang.BWDreset;

        /// <summary>
        ///   查找类似 后退至外放砖 的本地化字符串。
        /// </summary>
		public string BWDtoexternaltilerelease => Lang.BWDtoexternaltilerelease;

        /// <summary>
        ///   查找类似 后退至摆渡车 的本地化字符串。
        /// </summary>
		public string BWDtoferrytrolley => Lang.BWDtoferrytrolley;

        /// <summary>
        ///   查找类似 后退至内放砖 的本地化字符串。
        /// </summary>
		public string BWDtointernaltilerelease => Lang.BWDtointernaltilerelease;

        /// <summary>
        ///   查找类似 后退至点 的本地化字符串。
        /// </summary>
		public string BWDtopointedposition => Lang.BWDtopointedposition;

        /// <summary>
        ///   查找类似 取消 的本地化字符串。
        /// </summary>
		public string Cancel => Lang.Cancel;

        /// <summary>
        ///   查找类似 变更品种 的本地化字符串。
        /// </summary>
		public string Changeproduct => Lang.Changeproduct;

        /// <summary>
        ///   查找类似 清空 的本地化字符串。
        /// </summary>
		public string Clear => Lang.Clear;

        /// <summary>
        ///   查找类似 清空 的本地化字符串。
        /// </summary>
		public string Clear1 => Lang.Clear1;

        /// <summary>
        ///   查找类似 关闭 的本地化字符串。
        /// </summary>
		public string Close => Lang.Close;

        /// <summary>
        ///   查找类似 关闭所有 的本地化字符串。
        /// </summary>
		public string CloseAll => Lang.CloseAll;

        /// <summary>
        ///   查找类似 关闭其他 的本地化字符串。
        /// </summary>
		public string CloseOther => Lang.CloseOther;

        /// <summary>
        ///   查找类似 色号 的本地化字符串。
        /// </summary>
		public string Colornumber => Lang.Colornumber;

        /// <summary>
        ///   查找类似 连接通讯 的本地化字符串。
        /// </summary>
		public string Communicationconnected => Lang.Communicationconnected;

        /// <summary>
        ///   查找类似 中断通讯 的本地化字符串。
        /// </summary>
		public string Communicationdisconnected => Lang.Communicationdisconnected;

        /// <summary>
        ///   查找类似 通信正常 的本地化字符串。
        /// </summary>
		public string Communicationnormal => Lang.Communicationnormal;

        /// <summary>
        ///   查找类似 通讯状态 的本地化字符串。
        /// </summary>
		public string Communicationstatus => Lang.Communicationstatus;

        /// <summary>
        ///   查找类似 握手成功 的本地化字符串。
        /// </summary>
		public string Communicationsuccessful => Lang.Communicationsuccessful;

        /// <summary>
        ///   查找类似 完成指令 的本地化字符串。
        /// </summary>
		public string Completedinstruction => Lang.Completedinstruction;

        /// <summary>
        ///   查找类似 配置管理 的本地化字符串。
        /// </summary>
		public string Configurationmanagement => Lang.Configurationmanagement;

        /// <summary>
        ///   查找类似 确定 的本地化字符串。
        /// </summary>
		public string Confirm => Lang.Confirm;

        /// <summary>
        ///   查找类似 连接成功 的本地化字符串。
        /// </summary>
		public string Connected => Lang.Connected;

        /// <summary>
        ///   查找类似 运输车 的本地化字符串。
        /// </summary>
		public string Conveyingtrolley => Lang.Conveyingtrolley;

        /// <summary>
        ///   查找类似 设置坐标 的本地化字符串。
        /// </summary>
		public string Coordinatesset => Lang.Coordinatesset;

        /// <summary>
        ///   查找类似 当前坐标 的本地化字符串。
        /// </summary>
		public string Curentcoordinates => Lang.Curentcoordinates;

        /// <summary>
        ///   查找类似 更新为当前坐标 的本地化字符串。
        /// </summary>
		public string Currentcoordinatesrenew => Lang.Currentcoordinatesrenew;

        /// <summary>
        ///   查找类似 当前指令 的本地化字符串。
        /// </summary>
		public string Currentinstruction => Lang.Currentinstruction;

        /// <summary>
        ///   查找类似 当前层数 的本地化字符串。
        /// </summary>
		public string Currentlayer => Lang.Currentlayer;

        /// <summary>
        ///   查找类似 当前位置 的本地化字符串。
        /// </summary>
		public string Currentposition => Lang.Currentposition;

        /// <summary>
        ///   查找类似 当前站点 的本地化字符串。
        /// </summary>
		public string Currentpositioncode => Lang.Currentpositioncode;

        /// <summary>
        ///   查找类似 当前品种 的本地化字符串。
        /// </summary>
		public string Currentproduct => Lang.Currentproduct;

        /// <summary>
        ///   查找类似 当前作业轨道 的本地化字符串。
        /// </summary>
		public string Currenttrack => Lang.Currenttrack;

        /// <summary>
        ///   查找类似 删除库存 的本地化字符串。
        /// </summary>
		public string Deleteinventory => Lang.Deleteinventory;

        /// <summary>
        ///   查找类似 删除用户 的本地化字符串。
        /// </summary>
		public string Deleteuser => Lang.Deleteuser;

        /// <summary>
        ///   查找类似 目的坐标 的本地化字符串。
        /// </summary>
		public string Destinationcoordinates => Lang.Destinationcoordinates;

        /// <summary>
        ///   查找类似 目标位置 的本地化字符串。
        /// </summary>
		public string Destinationposition => Lang.Destinationposition;

        /// <summary>
        ///   查找类似 目的位置 的本地化字符串。
        /// </summary>
		public string Destinationposition1 => Lang.Destinationposition1;

        /// <summary>
        ///   查找类似 目的站点 的本地化字符串。
        /// </summary>
		public string Destinationstation => Lang.Destinationstation;

        /// <summary>
        ///   查找类似 目录 的本地化字符串。
        /// </summary>
		public string Directory => Lang.Directory;

        /// <summary>
        ///   查找类似 连接断开 的本地化字符串。
        /// </summary>
		public string Disconnected => Lang.Disconnected;

        /// <summary>
        ///   查找类似 主动断开 的本地化字符串。
        /// </summary>
		public string Drivedisconnected => Lang.Drivedisconnected;

        /// <summary>
        ///   查找类似 空砖轨道 的本地化字符串。
        /// </summary>
		public string Emptytrack => Lang.Emptytrack;

        /// <summary>
        ///   查找类似 介入 的本地化字符串。
        /// </summary>
		public string Entering => Lang.Entering;

        /// <summary>
        ///   查找类似 入库轨道 的本地化字符串。
        /// </summary>
		public string Enterwarehousetrack => Lang.Enterwarehousetrack;

        /// <summary>
        ///   查找类似 设备名称 的本地化字符串。
        /// </summary>
		public string Equipment => Lang.Equipment;

        /// <summary>
        ///   查找类似 设备类型 的本地化字符串。
        /// </summary>
		public string Equipmenttype => Lang.Equipmenttype;

        /// <summary>
        ///   查找类似 优等品 的本地化字符串。
        /// </summary>
		public string Excellentproduct => Lang.Excellentproduct;

        /// <summary>
        ///   查找类似 出库轨道 的本地化字符串。
        /// </summary>
		public string Exitingwarehousetrack => Lang.Exitingwarehousetrack;

        /// <summary>
        ///   查找类似 故障 的本地化字符串。
        /// </summary>
		public string Fault => Lang.Fault;

        /// <summary>
        ///   查找类似 移车中 的本地化字符串。
        /// </summary>
		public string Ferrytrolleymoving => Lang.Ferrytrolleymoving;

        /// <summary>
        ///   查找类似 还车回轨 的本地化字符串。
        /// </summary>
		public string Ferrytrolleytotrack => Lang.Ferrytrolleytotrack;

        /// <summary>
        ///   查找类似 小车回轨 的本地化字符串。
        /// </summary>
		public string Ferrytrolleytracktransferring => Lang.Ferrytrolleytracktransferring;

        /// <summary>
        ///   查找类似 查找 的本地化字符串。
        /// </summary>
		public string Find => Lang.Find;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string First_gradeproduct => Lang.First_gradeproduct;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string Front_backedgelength => Lang.Front_backedgelength;

        /// <summary>
        ///   查找类似 前侧位置 的本地化字符串。
        /// </summary>
		public string Frontposition => Lang.Frontposition;

        /// <summary>
        ///   查找类似 前侧光电 的本地化字符串。
        /// </summary>
		public string Frontsensor => Lang.Frontsensor;

        /// <summary>
        ///   查找类似 满砖轨道 的本地化字符串。
        /// </summary>
		public string Fulltrack => Lang.Fulltrack;

        /// <summary>
        ///   查找类似 功能名称 的本地化字符串。
        /// </summary>
		public string Functiondescription => Lang.Functiondescription;

        /// <summary>
        ///   查找类似 前进 的本地化字符串。
        /// </summary>
		public string FWD => Lang.FWD;

        /// <summary>
        ///   查找类似 前进复位 的本地化字符串。
        /// </summary>
		public string FWDreset => Lang.FWDreset;

        /// <summary>
        ///   查找类似 前进至摆渡车 的本地化字符串。
        /// </summary>
		public string FWDtoferrytrolley => Lang.FWDtoferrytrolley;

        /// <summary>
        ///   查找类似 前进至点 的本地化字符串。
        /// </summary>
		public string FWDtopointedposition => Lang.FWDtopointedposition;

        /// <summary>
        ///   查找类似 等级 的本地化字符串。
        /// </summary>
		public string Grade => Lang.Grade;

        /// <summary>
        ///   查找类似 设定等级 的本地化字符串。
        /// </summary>
		public string Gradeset => Lang.Gradeset;

        /// <summary>
        ///   查找类似 连接中 的本地化字符串。
        /// </summary>
		public string Inconnecting => Lang.Inconnecting;

        /// <summary>
        ///   查找类似 初始化 的本地化字符串。
        /// </summary>
		public string Initializing => Lang.Initializing;

        /// <summary>
        ///   查找类似 向后插入库存 的本地化字符串。
        /// </summary>
		public string Insertininvertorybackward => Lang.Insertininvertorybackward;

        /// <summary>
        ///   查找类似 向前插入库存 的本地化字符串。
        /// </summary>
		public string Insertininvertoryforward => Lang.Insertininvertoryforward;

        /// <summary>
        ///   查找类似 指令异常 的本地化字符串。
        /// </summary>
		public string Instructionabnormal => Lang.Instructionabnormal;

        /// <summary>
        ///   查找类似 库存序号 的本地化字符串。
        /// </summary>
		public string Inventorynumber => Lang.Inventorynumber;

        /// <summary>
        ///   查找类似 库存位置 的本地化字符串。
        /// </summary>
		public string Inventoryposition => Lang.Inventoryposition;

        /// <summary>
        ///   查找类似 库存品种 的本地化字符串。
        /// </summary>
		public string Inventoryproducts => Lang.Inventoryproducts;

        /// <summary>
        ///   查找类似 刷新库存信息 的本地化字符串。
        /// </summary>
		public string Inventoryrenew => Lang.Inventoryrenew;

        /// <summary>
        ///   查找类似 转移库存 的本地化字符串。
        /// </summary>
		public string Inventorytransferring => Lang.Inventorytransferring;

        /// <summary>
        ///   查找类似 跳转 的本地化字符串。
        /// </summary>
		public string Jump => Lang.Jump;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string L_Redgelength => Lang.L_Redgelength;

        /// <summary>
        ///   查找类似 离开 的本地化字符串。
        /// </summary>
		public string Leaving => Lang.Leaving;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string Loading_unloadingmachine => Lang.Loading_unloadingmachine;

        /// <summary>
        ///   查找类似 取货坐标 的本地化字符串。
        /// </summary>
		public string Loadingcoordinates => Lang.Loadingcoordinates;

        /// <summary>
        ///   查找类似 上砖摆渡轨道 的本地化字符串。
        /// </summary>
		public string Loadingferrytrack => Lang.Loadingferrytrack;

        /// <summary>
        ///   查找类似 上砖摆渡车 的本地化字符串。
        /// </summary>
		public string Loadingferrytrolley => Lang.Loadingferrytrolley;

        /// <summary>
        ///   查找类似 上砖机 的本地化字符串。
        /// </summary>
		public string Loadingmachine => Lang.Loadingmachine;

        /// <summary>
        ///   查找类似 仅上砖 的本地化字符串。
        /// </summary>
		public string LoadingONLY => Lang.LoadingONLY;

        /// <summary>
        ///   查找类似 取货站点 的本地化字符串。
        /// </summary>
		public string Loadingposition => Lang.Loadingposition;

        /// <summary>
        ///   查找类似 载货状态 的本地化字符串。
        /// </summary>
		public string Loadingstatus => Lang.Loadingstatus;

        /// <summary>
        ///   查找类似 载车状态 的本地化字符串。
        /// </summary>
		public string Loadingstatus1 => Lang.Loadingstatus1;

        /// <summary>
        ///   查找类似 载砖状态 的本地化字符串。
        /// </summary>
		public string Loadingstatus2 => Lang.Loadingstatus2;

        /// <summary>
        ///   查找类似 上砖任务 的本地化字符串。
        /// </summary>
		public string Loadingtask => Lang.Loadingtask;

        /// <summary>
        ///   查找类似 上砖轨道 的本地化字符串。
        /// </summary>
		public string Loadingtrack => Lang.Loadingtrack;

        /// <summary>
        ///   查找类似 载车 的本地化字符串。
        /// </summary>
		public string Loadingtrolley => Lang.Loadingtrolley;

        /// <summary>
        ///   查找类似 登陆 的本地化字符串。
        /// </summary>
		public string Login => Lang.Login;

        /// <summary>
        ///   查找类似 当前用户： 的本地化字符串。
        /// </summary>
		public string LoginUser => Lang.LoginUser;

        /// <summary>
        ///   查找类似 手动上砖 的本地化字符串。
        /// </summary>
		public string Manualloading => Lang.Manualloading;

        /// <summary>
        ///   查找类似 手动模式 的本地化字符串。
        /// </summary>
		public string Manualmode => Lang.Manualmode;

        /// <summary>
        ///   查找类似 手动下砖 的本地化字符串。
        /// </summary>
		public string Manualunloading => Lang.Manualunloading;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string Max_tilelayer => Lang.Max_tilelayer;

        /// <summary>
        ///   查找类似 菜单 的本地化字符串。
        /// </summary>
		public string Menu => Lang.Menu;

        /// <summary>
        ///   查找类似 修改 的本地化字符串。
        /// </summary>
		public string Modify => Lang.Modify;

        /// <summary>
        ///   查找类似 修改用户 的本地化字符串。
        /// </summary>
		public string Modifyuser => Lang.Modifyuser;

        /// <summary>
        ///   查找类似 改变作业依据 的本地化字符串。
        /// </summary>
		public string Modifyworkingcondition => Lang.Modifyworkingcondition;

        /// <summary>
        ///   查找类似 窄车 的本地化字符串。
        /// </summary>
		public string Narrowtrolley => Lang.Narrowtrolley;

        /// <summary>
        ///   查找类似 下一页 的本地化字符串。
        /// </summary>
		public string NextPage => Lang.NextPage;

        /// <summary>
        ///   查找类似 否 的本地化字符串。
        /// </summary>
		public string No => Lang.No;

        /// <summary>
        ///   查找类似 没有指令 的本地化字符串。
        /// </summary>
		public string Noinstruction => Lang.Noinstruction;

        /// <summary>
        ///   查找类似 非空 的本地化字符串。
        /// </summary>
		public string Notempty => Lang.Notempty;

        /// <summary>
        ///   查找类似 非空非满 的本地化字符串。
        /// </summary>
		public string Notempty1 => Lang.Notempty1;

        /// <summary>
        ///   查找类似 有砖轨道 的本地化字符串。
        /// </summary>
		public string Notemptytrack => Lang.Notemptytrack;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string Off_linewarning => Lang.Off_linewarning;

        /// <summary>
        ///   查找类似 操作模式 的本地化字符串。
        /// </summary>
		public string Operatingmode => Lang.Operatingmode;

        /// <summary>
        ///   查找类似 下降放货 的本地化字符串。
        /// </summary>
		public string Origindownunloading => Lang.Origindownunloading;

        /// <summary>
        ///   查找类似 顶升取货 的本地化字符串。
        /// </summary>
		public string Originuploading => Lang.Originuploading;

        /// <summary>
        ///   查找类似 上级目录 的本地化字符串。
        /// </summary>
		public string Parentdirectory => Lang.Parentdirectory;

        /// <summary>
        ///   查找类似 密码 的本地化字符串。
        /// </summary>
		public string Password => Lang.Password;

        /// <summary>
        ///   查找类似 校验密码 的本地化字符串。
        /// </summary>
		public string Passwordcheck => Lang.Passwordcheck;

        /// <summary>
        ///   查找类似 下午 的本地化字符串。
        /// </summary>
		public string Pm => Lang.Pm;

        /// <summary>
        ///   查找类似 定位指令 的本地化字符串。
        /// </summary>
		public string Positioninstruction => Lang.Positioninstruction;

        /// <summary>
        ///   查找类似 上一页 的本地化字符串。
        /// </summary>
		public string PreviousPage => Lang.PreviousPage;

        /// <summary>
        ///   查找类似 优先级 的本地化字符串。
        /// </summary>
		public string Prioritygrade => Lang.Prioritygrade;

        /// <summary>
        ///   查找类似 生产时间 的本地化字符串。
        /// </summary>
		public string Producingtime => Lang.Producingtime;

        /// <summary>
        ///   查找类似 转产完成 的本地化字符串。
        /// </summary>
		public string Productchanged => Lang.Productchanged;

        /// <summary>
        ///   查找类似 转产状态 的本地化字符串。
        /// </summary>
		public string Productchangestatus => Lang.Productchangestatus;

        /// <summary>
        ///   查找类似 转产中 的本地化字符串。
        /// </summary>
		public string Productchanging => Lang.Productchanging;

        /// <summary>
        ///   查找类似 品种名称 的本地化字符串。
        /// </summary>
		public string Productiondescription => Lang.Productiondescription;

        /// <summary>
        ///   查找类似 设定品种 的本地化字符串。
        /// </summary>
		public string Productset => Lang.Productset;

        /// <summary>
        ///   查找类似 脉冲值 的本地化字符串。
        /// </summary>
		public string Pulsevalue => Lang.Pulsevalue;

        /// <summary>
        ///   查找类似 合格品 的本地化字符串。
        /// </summary>
		public string Qualifiedproduct => Lang.Qualifiedproduct;

        /// <summary>
        ///   查找类似 阅读器断开报警 的本地化字符串。
        /// </summary>
		public string Readerdisconnectwarning => Lang.Readerdisconnectwarning;

        /// <summary>
        ///   查找类似 库存实际坐标 的本地化字符串。
        /// </summary>
		public string Realinventorycoordinates => Lang.Realinventorycoordinates;

        /// <summary>
        ///   查找类似 实际点位 的本地化字符串。
        /// </summary>
		public string Realposition => Lang.Realposition;

        /// <summary>
        ///   查找类似 需求状态 的本地化字符串。
        /// </summary>
		public string Requirementstatus => Lang.Requirementstatus;

        /// <summary>
        ///   查找类似 复位 的本地化字符串。
        /// </summary>
		public string Reset => Lang.Reset;

        /// <summary>
        ///   查找类似 复位指令 的本地化字符串。
        /// </summary>
		public string Resetinstruction => Lang.Resetinstruction;

        /// <summary>
        ///   查找类似 后退倒库 的本地化字符串。
        /// </summary>
		public string ReturingwarehouseBWD => Lang.ReturingwarehouseBWD;

        /// <summary>
        ///   查找类似 前进倒库 的本地化字符串。
        /// </summary>
		public string ReturingwarehouseFWD => Lang.ReturingwarehouseFWD;

        /// <summary>
        ///   查找类似 倒库中 的本地化字符串。
        /// </summary>
		public string Returningtowarehouse => Lang.Returningtowarehouse;

        /// <summary>
        ///   查找类似 倒库 的本地化字符串。
        /// </summary>
		public string Returningtowarehouse1 => Lang.Returningtowarehouse1;

        /// <summary>
        ///   查找类似 倒库任务 的本地化字符串。
        /// </summary>
		public string Returntowarehouse => Lang.Returntowarehouse;

        /// <summary>
        ///   查找类似 角色 的本地化字符串。
        /// </summary>
		public string Role => Lang.Role;

        /// <summary>
        ///   查找类似 运动状态 的本地化字符串。
        /// </summary>
		public string Runningstatus => Lang.Runningstatus;

        /// <summary>
        ///   查找类似 同向上砖 的本地化字符串。
        /// </summary>
		public string Samedirectionloading => Lang.Samedirectionloading;

        /// <summary>
        ///   查找类似 同向下砖 的本地化字符串。
        /// </summary>
		public string Samedirectionunloading => Lang.Samedirectionunloading;

        /// <summary>
        ///   查找类似 保存 的本地化字符串。
        /// </summary>
		public string Save => Lang.Save;

        /// <summary>
        ///   查找类似 保存更新至后台 的本地化字符串。
        /// </summary>
		public string Saverenew => Lang.Saverenew;

        /// <summary>
        ///   查找类似 调度设备 的本地化字符串。
        /// </summary>
		public string Schedulingequipment => Lang.Schedulingequipment;

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public string Second_gradeproduct => Lang.Second_gradeproduct;

        /// <summary>
        ///   查找类似 规格 的本地化字符串。
        /// </summary>
		public string Specification => Lang.Specification;

        /// <summary>
        ///   查找类似 存放垛数 的本地化字符串。
        /// </summary>
		public string Stackqty => Lang.Stackqty;

        /// <summary>
        ///   查找类似 启用 的本地化字符串。
        /// </summary>
		public string Start => Lang.Start;

        /// <summary>
        ///   查找类似 启用状态 的本地化字符串。
        /// </summary>
		public string Start1 => Lang.Start1;

        /// <summary>
        ///   查找类似 开始时间 的本地化字符串。
        /// </summary>
		public string Starttime => Lang.Starttime;

        /// <summary>
        ///   查找类似 停用 的本地化字符串。
        /// </summary>
		public string Stop => Lang.Stop;

        /// <summary>
        ///   查找类似 停止 的本地化字符串。
        /// </summary>
		public string Stop1 => Lang.Stop1;

        /// <summary>
        ///   查找类似 终止指令 的本地化字符串。
        /// </summary>
		public string Stopinstruction => Lang.Stopinstruction;

        /// <summary>
        ///   查找类似 结束时间 的本地化字符串。
        /// </summary>
		public string Stoptime => Lang.Stoptime;

        /// <summary>
        ///   查找类似 修改策略 的本地化字符串。
        /// </summary>
		public string Strategymodify => Lang.Strategymodify;

        /// <summary>
        ///   查找类似 区域 的本地化字符串。
        /// </summary>
		public string String1 => Lang.String1;

        /// <summary>
        ///   查找类似 取消任务 的本地化字符串。
        /// </summary>
		public string Taskcancelled => Lang.Taskcancelled;

        /// <summary>
        ///   查找类似 完成任务 的本地化字符串。
        /// </summary>
		public string Taskcompleted => Lang.Taskcompleted;

        /// <summary>
        ///   查找类似 任务开关 的本地化字符串。
        /// </summary>
		public string Taskswitch => Lang.Taskswitch;

        /// <summary>
        ///   查找类似 任务类型 的本地化字符串。
        /// </summary>
		public string Tasktype => Lang.Tasktype;

        /// <summary>
        ///   查找类似 任务类型 的本地化字符串。
        /// </summary>
		public string Tasktype1 => Lang.Tasktype1;

        /// <summary>
        ///   查找类似 取砖流程 的本地化字符串。
        /// </summary>
		public string Tilecatching => Lang.Tilecatching;

        /// <summary>
        ///   查找类似 无砖 的本地化字符串。
        /// </summary>
		public string Tilelack => Lang.Tilelack;

        /// <summary>
        ///   查找类似 后退取砖 的本地化字符串。
        /// </summary>
		public string TileloadingBWD => Lang.TileloadingBWD;

        /// <summary>
        ///   查找类似 前进取砖 的本地化字符串。
        /// </summary>
		public string TileloadingFWD => Lang.TileloadingFWD;

        /// <summary>
        ///   查找类似 取砖指令 的本地化字符串。
        /// </summary>
		public string Tileloadinginstruction => Lang.Tileloadinginstruction;

        /// <summary>
        ///   查找类似 存放片数 的本地化字符串。
        /// </summary>
		public string Tileqty => Lang.Tileqty;

        /// <summary>
        ///   查找类似 轨道库存 的本地化字符串。
        /// </summary>
		public string Tileqtyontrack => Lang.Tileqtyontrack;

        /// <summary>
        ///   查找类似 有砖 的本地化字符串。
        /// </summary>
		public string Tileready => Lang.Tileready;

        /// <summary>
        ///   查找类似 放砖流程 的本地化字符串。
        /// </summary>
		public string Tilereleasing => Lang.Tilereleasing;

        /// <summary>
        ///   查找类似 前进放砖 的本地化字符串。
        /// </summary>
		public string TilereleasingFWD => Lang.TilereleasingFWD;

        /// <summary>
        ///   查找类似 放砖指令 的本地化字符串。
        /// </summary>
		public string Tilereleasinginstruction => Lang.Tilereleasinginstruction;

        /// <summary>
        ///   查找类似 储砖轨道 的本地化字符串。
        /// </summary>
		public string Tilestoragetrack => Lang.Tilestoragetrack;

        /// <summary>
        ///   查找类似 提示 的本地化字符串。
        /// </summary>
		public string Tip => Lang.Tip;

        /// <summary>
        ///   查找类似 过大 的本地化字符串。
        /// </summary>
		public string TooLarge => Lang.TooLarge;

        /// <summary>
        ///   查找类似 检查轨道 的本地化字符串。
        /// </summary>
		public string Trackcheck => Lang.Trackcheck;

        /// <summary>
        ///   查找类似 轨道名称 的本地化字符串。
        /// </summary>
		public string Trackdescription => Lang.Trackdescription;

        /// <summary>
        ///   查找类似 空轨信号 的本地化字符串。
        /// </summary>
		public string Trackemptysignal => Lang.Trackemptysignal;

        /// <summary>
        ///   查找类似 轨道尾部 的本地化字符串。
        /// </summary>
		public string Trackend => Lang.Trackend;

        /// <summary>
        ///   查找类似 满轨信号 的本地化字符串。
        /// </summary>
		public string Trackfullsignal => Lang.Trackfullsignal;

        /// <summary>
        ///   查找类似 轨道头部 的本地化字符串。
        /// </summary>
		public string Trackhead => Lang.Trackhead;

        /// <summary>
        ///   查找类似 轨道中部 的本地化字符串。
        /// </summary>
		public string Trackmid => Lang.Trackmid;

        /// <summary>
        ///   查找类似 轨道读点报警 的本地化字符串。
        /// </summary>
		public string Trackreadingwarning => Lang.Trackreadingwarning;

        /// <summary>
        ///   查找类似 启用轨道 的本地化字符串。
        /// </summary>
		public string Trackstarted => Lang.Trackstarted;

        /// <summary>
        ///   查找类似 轨道状态 的本地化字符串。
        /// </summary>
		public string Trackstatus => Lang.Trackstatus;

        /// <summary>
        ///   查找类似 空满信号 的本地化字符串。
        /// </summary>
		public string Trackstatussignal => Lang.Trackstatussignal;

        /// <summary>
        ///   查找类似 停用轨道 的本地化字符串。
        /// </summary>
		public string Trackstopped => Lang.Trackstopped;

        /// <summary>
        ///   查找类似 轨道类型 的本地化字符串。
        /// </summary>
		public string Tracktype => Lang.Tracktype;

        /// <summary>
        ///   查找类似 空车 的本地化字符串。
        /// </summary>
		public string Trolleyempty => Lang.Trolleyempty;

        /// <summary>
        ///   查找类似 存放车数 的本地化字符串。
        /// </summary>
		public string Trolleyqty => Lang.Trolleyqty;

        /// <summary>
        ///   查找类似 移车任务 的本地化字符串。
        /// </summary>
		public string Truckmovement => Lang.Truckmovement;

        /// <summary>
        ///   查找类似 未知 的本地化字符串。
        /// </summary>
		public string Unknown => Lang.Unknown;

        /// <summary>
        ///   查找类似 未知大小 的本地化字符串。
        /// </summary>
		public string UnknownSize => Lang.UnknownSize;

        /// <summary>
        ///   查找类似 卸货坐标 的本地化字符串。
        /// </summary>
		public string Unloadingcoordinates => Lang.Unloadingcoordinates;

        /// <summary>
        ///   查找类似 下砖摆渡轨道 的本地化字符串。
        /// </summary>
		public string Unloadingferrytrack => Lang.Unloadingferrytrack;

        /// <summary>
        ///   查找类似 下砖摆渡车 的本地化字符串。
        /// </summary>
		public string Unloadingferrytrolley => Lang.Unloadingferrytrolley;

        /// <summary>
        ///   查找类似 下砖机 的本地化字符串。
        /// </summary>
		public string Unloadingmachine => Lang.Unloadingmachine;

        /// <summary>
        ///   查找类似 仅下砖 的本地化字符串。
        /// </summary>
		public string UnloadingONLY => Lang.UnloadingONLY;

        /// <summary>
        ///   查找类似 卸货站点 的本地化字符串。
        /// </summary>
		public string Unloadingposition => Lang.Unloadingposition;

        /// <summary>
        ///   查找类似 下砖任务 的本地化字符串。
        /// </summary>
		public string Unloadingtask => Lang.Unloadingtask;

        /// <summary>
        ///   查找类似 下砖轨道 的本地化字符串。
        /// </summary>
		public string Unloadingtrack => Lang.Unloadingtrack;

        /// <summary>
        ///   查找类似 用户 的本地化字符串。
        /// </summary>
		public string User => Lang.User;

        /// <summary>
        ///   查找类似 入库策略 的本地化字符串。
        /// </summary>
		public string Warehouseinstrategy => Lang.Warehouseinstrategy;

        /// <summary>
        ///   查找类似 出库策略 的本地化字符串。
        /// </summary>
		public string Warehouseoutstrategy => Lang.Warehouseoutstrategy;

        /// <summary>
        ///   查找类似 倒库数量 的本地化字符串。
        /// </summary>
		public string Warehousereturningqty => Lang.Warehousereturningqty;

        /// <summary>
        ///   查找类似 警告类型 的本地化字符串。
        /// </summary>
		public string Warningtype => Lang.Warningtype;

        /// <summary>
        ///   查找类似 宽车 的本地化字符串。
        /// </summary>
		public string Widetrolley => Lang.Widetrolley;

        /// <summary>
        ///   查找类似 作业依据 的本地化字符串。
        /// </summary>
		public string Workingcondition => Lang.Workingcondition;

        /// <summary>
        ///   查找类似 切换工作模式 的本地化字符串。
        /// </summary>
		public string Workingmodeshift => Lang.Workingmodeshift;

        /// <summary>
        ///   查找类似 工位品种 的本地化字符串。
        /// </summary>
		public string Workstationproduct => Lang.Workstationproduct;

        /// <summary>
        ///   查找类似 是 的本地化字符串。
        /// </summary>
		public string Yes => Lang.Yes;

        /// <summary>
        ///   查找类似 放大 的本地化字符串。
        /// </summary>
		public string ZoomIn => Lang.ZoomIn;

        /// <summary>
        ///   查找类似 缩小 的本地化字符串。
        /// </summary>
		public string ZoomOut => Lang.ZoomOut;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class LangKeys
    {
        /// <summary>
        ///   查找类似 依据品种作业 的本地化字符串。
        /// </summary>
		public static string Accordingtoproduct = nameof(Accordingtoproduct);

        /// <summary>
        ///   查找类似 依据轨道作业 的本地化字符串。
        /// </summary>
		public static string Accordingtotrack = nameof(Accordingtotrack);

        /// <summary>
        ///   查找类似 添加库存 的本地化字符串。
        /// </summary>
		public static string Addininventory = nameof(Addininventory);

        /// <summary>
        ///   查找类似 添加库存信息 的本地化字符串。
        /// </summary>
		public static string Addinventorysignal = nameof(Addinventorysignal);

        /// <summary>
        ///   查找类似 添加用户 的本地化字符串。
        /// </summary>
		public static string Adduser = nameof(Adduser);

        /// <summary>
        ///   查找类似 管理员菜单 的本地化字符串。
        /// </summary>
		public static string Administratormenu = nameof(Administratormenu);

        /// <summary>
        ///   查找类似 全部 的本地化字符串。
        /// </summary>
		public static string All = nameof(All);

        /// <summary>
        ///   查找类似 上午 的本地化字符串。
        /// </summary>
		public static string Am = nameof(Am);

        /// <summary>
        ///   查找类似 储砖调度系统 的本地化字符串。
        /// </summary>
		public static string AppTitle = nameof(AppTitle);

        /// <summary>
        ///   查找类似 授权管理 的本地化字符串。
        /// </summary>
		public static string Authorizationmanagement = nameof(Authorizationmanagement);

        /// <summary>
        ///   查找类似 自动对位 的本地化字符串。
        /// </summary>
		public static string Autoaligning = nameof(Autoaligning);

        /// <summary>
        ///   查找类似 自动模式 的本地化字符串。
        /// </summary>
		public static string Automode = nameof(Automode);

        /// <summary>
        ///   查找类似 后侧位置 的本地化字符串。
        /// </summary>
		public static string Backposition = nameof(Backposition);

        /// <summary>
        ///   查找类似 后侧光电 的本地化字符串。
        /// </summary>
		public static string Backsensor = nameof(Backsensor);

        /// <summary>
        ///   查找类似 基础菜单 的本地化字符串。
        /// </summary>
		public static string Basicmenu = nameof(Basicmenu);

        /// <summary>
        ///   查找类似 后退 的本地化字符串。
        /// </summary>
		public static string BWD = nameof(BWD);

        /// <summary>
        ///   查找类似 后退复位 的本地化字符串。
        /// </summary>
		public static string BWDreset = nameof(BWDreset);

        /// <summary>
        ///   查找类似 后退至外放砖 的本地化字符串。
        /// </summary>
		public static string BWDtoexternaltilerelease = nameof(BWDtoexternaltilerelease);

        /// <summary>
        ///   查找类似 后退至摆渡车 的本地化字符串。
        /// </summary>
		public static string BWDtoferrytrolley = nameof(BWDtoferrytrolley);

        /// <summary>
        ///   查找类似 后退至内放砖 的本地化字符串。
        /// </summary>
		public static string BWDtointernaltilerelease = nameof(BWDtointernaltilerelease);

        /// <summary>
        ///   查找类似 后退至点 的本地化字符串。
        /// </summary>
		public static string BWDtopointedposition = nameof(BWDtopointedposition);

        /// <summary>
        ///   查找类似 取消 的本地化字符串。
        /// </summary>
		public static string Cancel = nameof(Cancel);

        /// <summary>
        ///   查找类似 变更品种 的本地化字符串。
        /// </summary>
		public static string Changeproduct = nameof(Changeproduct);

        /// <summary>
        ///   查找类似 清空 的本地化字符串。
        /// </summary>
		public static string Clear = nameof(Clear);

        /// <summary>
        ///   查找类似 清空 的本地化字符串。
        /// </summary>
		public static string Clear1 = nameof(Clear1);

        /// <summary>
        ///   查找类似 关闭 的本地化字符串。
        /// </summary>
		public static string Close = nameof(Close);

        /// <summary>
        ///   查找类似 关闭所有 的本地化字符串。
        /// </summary>
		public static string CloseAll = nameof(CloseAll);

        /// <summary>
        ///   查找类似 关闭其他 的本地化字符串。
        /// </summary>
		public static string CloseOther = nameof(CloseOther);

        /// <summary>
        ///   查找类似 色号 的本地化字符串。
        /// </summary>
		public static string Colornumber = nameof(Colornumber);

        /// <summary>
        ///   查找类似 连接通讯 的本地化字符串。
        /// </summary>
		public static string Communicationconnected = nameof(Communicationconnected);

        /// <summary>
        ///   查找类似 中断通讯 的本地化字符串。
        /// </summary>
		public static string Communicationdisconnected = nameof(Communicationdisconnected);

        /// <summary>
        ///   查找类似 通信正常 的本地化字符串。
        /// </summary>
		public static string Communicationnormal = nameof(Communicationnormal);

        /// <summary>
        ///   查找类似 通讯状态 的本地化字符串。
        /// </summary>
		public static string Communicationstatus = nameof(Communicationstatus);

        /// <summary>
        ///   查找类似 握手成功 的本地化字符串。
        /// </summary>
		public static string Communicationsuccessful = nameof(Communicationsuccessful);

        /// <summary>
        ///   查找类似 完成指令 的本地化字符串。
        /// </summary>
		public static string Completedinstruction = nameof(Completedinstruction);

        /// <summary>
        ///   查找类似 配置管理 的本地化字符串。
        /// </summary>
		public static string Configurationmanagement = nameof(Configurationmanagement);

        /// <summary>
        ///   查找类似 确定 的本地化字符串。
        /// </summary>
		public static string Confirm = nameof(Confirm);

        /// <summary>
        ///   查找类似 连接成功 的本地化字符串。
        /// </summary>
		public static string Connected = nameof(Connected);

        /// <summary>
        ///   查找类似 运输车 的本地化字符串。
        /// </summary>
		public static string Conveyingtrolley = nameof(Conveyingtrolley);

        /// <summary>
        ///   查找类似 设置坐标 的本地化字符串。
        /// </summary>
		public static string Coordinatesset = nameof(Coordinatesset);

        /// <summary>
        ///   查找类似 当前坐标 的本地化字符串。
        /// </summary>
		public static string Curentcoordinates = nameof(Curentcoordinates);

        /// <summary>
        ///   查找类似 更新为当前坐标 的本地化字符串。
        /// </summary>
		public static string Currentcoordinatesrenew = nameof(Currentcoordinatesrenew);

        /// <summary>
        ///   查找类似 当前指令 的本地化字符串。
        /// </summary>
		public static string Currentinstruction = nameof(Currentinstruction);

        /// <summary>
        ///   查找类似 当前层数 的本地化字符串。
        /// </summary>
		public static string Currentlayer = nameof(Currentlayer);

        /// <summary>
        ///   查找类似 当前位置 的本地化字符串。
        /// </summary>
		public static string Currentposition = nameof(Currentposition);

        /// <summary>
        ///   查找类似 当前站点 的本地化字符串。
        /// </summary>
		public static string Currentpositioncode = nameof(Currentpositioncode);

        /// <summary>
        ///   查找类似 当前品种 的本地化字符串。
        /// </summary>
		public static string Currentproduct = nameof(Currentproduct);

        /// <summary>
        ///   查找类似 当前作业轨道 的本地化字符串。
        /// </summary>
		public static string Currenttrack = nameof(Currenttrack);

        /// <summary>
        ///   查找类似 删除库存 的本地化字符串。
        /// </summary>
		public static string Deleteinventory = nameof(Deleteinventory);

        /// <summary>
        ///   查找类似 删除用户 的本地化字符串。
        /// </summary>
		public static string Deleteuser = nameof(Deleteuser);

        /// <summary>
        ///   查找类似 目的坐标 的本地化字符串。
        /// </summary>
		public static string Destinationcoordinates = nameof(Destinationcoordinates);

        /// <summary>
        ///   查找类似 目标位置 的本地化字符串。
        /// </summary>
		public static string Destinationposition = nameof(Destinationposition);

        /// <summary>
        ///   查找类似 目的位置 的本地化字符串。
        /// </summary>
		public static string Destinationposition1 = nameof(Destinationposition1);

        /// <summary>
        ///   查找类似 目的站点 的本地化字符串。
        /// </summary>
		public static string Destinationstation = nameof(Destinationstation);

        /// <summary>
        ///   查找类似 目录 的本地化字符串。
        /// </summary>
		public static string Directory = nameof(Directory);

        /// <summary>
        ///   查找类似 连接断开 的本地化字符串。
        /// </summary>
		public static string Disconnected = nameof(Disconnected);

        /// <summary>
        ///   查找类似 主动断开 的本地化字符串。
        /// </summary>
		public static string Drivedisconnected = nameof(Drivedisconnected);

        /// <summary>
        ///   查找类似 空砖轨道 的本地化字符串。
        /// </summary>
		public static string Emptytrack = nameof(Emptytrack);

        /// <summary>
        ///   查找类似 介入 的本地化字符串。
        /// </summary>
		public static string Entering = nameof(Entering);

        /// <summary>
        ///   查找类似 入库轨道 的本地化字符串。
        /// </summary>
		public static string Enterwarehousetrack = nameof(Enterwarehousetrack);

        /// <summary>
        ///   查找类似 设备名称 的本地化字符串。
        /// </summary>
		public static string Equipment = nameof(Equipment);

        /// <summary>
        ///   查找类似 设备类型 的本地化字符串。
        /// </summary>
		public static string Equipmenttype = nameof(Equipmenttype);

        /// <summary>
        ///   查找类似 优等品 的本地化字符串。
        /// </summary>
		public static string Excellentproduct = nameof(Excellentproduct);

        /// <summary>
        ///   查找类似 出库轨道 的本地化字符串。
        /// </summary>
		public static string Exitingwarehousetrack = nameof(Exitingwarehousetrack);

        /// <summary>
        ///   查找类似 故障 的本地化字符串。
        /// </summary>
		public static string Fault = nameof(Fault);

        /// <summary>
        ///   查找类似 移车中 的本地化字符串。
        /// </summary>
		public static string Ferrytrolleymoving = nameof(Ferrytrolleymoving);

        /// <summary>
        ///   查找类似 还车回轨 的本地化字符串。
        /// </summary>
		public static string Ferrytrolleytotrack = nameof(Ferrytrolleytotrack);

        /// <summary>
        ///   查找类似 小车回轨 的本地化字符串。
        /// </summary>
		public static string Ferrytrolleytracktransferring = nameof(Ferrytrolleytracktransferring);

        /// <summary>
        ///   查找类似 查找 的本地化字符串。
        /// </summary>
		public static string Find = nameof(Find);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string First_gradeproduct = nameof(First_gradeproduct);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string Front_backedgelength = nameof(Front_backedgelength);

        /// <summary>
        ///   查找类似 前侧位置 的本地化字符串。
        /// </summary>
		public static string Frontposition = nameof(Frontposition);

        /// <summary>
        ///   查找类似 前侧光电 的本地化字符串。
        /// </summary>
		public static string Frontsensor = nameof(Frontsensor);

        /// <summary>
        ///   查找类似 满砖轨道 的本地化字符串。
        /// </summary>
		public static string Fulltrack = nameof(Fulltrack);

        /// <summary>
        ///   查找类似 功能名称 的本地化字符串。
        /// </summary>
		public static string Functiondescription = nameof(Functiondescription);

        /// <summary>
        ///   查找类似 前进 的本地化字符串。
        /// </summary>
		public static string FWD = nameof(FWD);

        /// <summary>
        ///   查找类似 前进复位 的本地化字符串。
        /// </summary>
		public static string FWDreset = nameof(FWDreset);

        /// <summary>
        ///   查找类似 前进至摆渡车 的本地化字符串。
        /// </summary>
		public static string FWDtoferrytrolley = nameof(FWDtoferrytrolley);

        /// <summary>
        ///   查找类似 前进至点 的本地化字符串。
        /// </summary>
		public static string FWDtopointedposition = nameof(FWDtopointedposition);

        /// <summary>
        ///   查找类似 等级 的本地化字符串。
        /// </summary>
		public static string Grade = nameof(Grade);

        /// <summary>
        ///   查找类似 设定等级 的本地化字符串。
        /// </summary>
		public static string Gradeset = nameof(Gradeset);

        /// <summary>
        ///   查找类似 连接中 的本地化字符串。
        /// </summary>
		public static string Inconnecting = nameof(Inconnecting);

        /// <summary>
        ///   查找类似 初始化 的本地化字符串。
        /// </summary>
		public static string Initializing = nameof(Initializing);

        /// <summary>
        ///   查找类似 向后插入库存 的本地化字符串。
        /// </summary>
		public static string Insertininvertorybackward = nameof(Insertininvertorybackward);

        /// <summary>
        ///   查找类似 向前插入库存 的本地化字符串。
        /// </summary>
		public static string Insertininvertoryforward = nameof(Insertininvertoryforward);

        /// <summary>
        ///   查找类似 指令异常 的本地化字符串。
        /// </summary>
		public static string Instructionabnormal = nameof(Instructionabnormal);

        /// <summary>
        ///   查找类似 库存序号 的本地化字符串。
        /// </summary>
		public static string Inventorynumber = nameof(Inventorynumber);

        /// <summary>
        ///   查找类似 库存位置 的本地化字符串。
        /// </summary>
		public static string Inventoryposition = nameof(Inventoryposition);

        /// <summary>
        ///   查找类似 库存品种 的本地化字符串。
        /// </summary>
		public static string Inventoryproducts = nameof(Inventoryproducts);

        /// <summary>
        ///   查找类似 刷新库存信息 的本地化字符串。
        /// </summary>
		public static string Inventoryrenew = nameof(Inventoryrenew);

        /// <summary>
        ///   查找类似 转移库存 的本地化字符串。
        /// </summary>
		public static string Inventorytransferring = nameof(Inventorytransferring);

        /// <summary>
        ///   查找类似 跳转 的本地化字符串。
        /// </summary>
		public static string Jump = nameof(Jump);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string L_Redgelength = nameof(L_Redgelength);

        /// <summary>
        ///   查找类似 离开 的本地化字符串。
        /// </summary>
		public static string Leaving = nameof(Leaving);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string Loading_unloadingmachine = nameof(Loading_unloadingmachine);

        /// <summary>
        ///   查找类似 取货坐标 的本地化字符串。
        /// </summary>
		public static string Loadingcoordinates = nameof(Loadingcoordinates);

        /// <summary>
        ///   查找类似 上砖摆渡轨道 的本地化字符串。
        /// </summary>
		public static string Loadingferrytrack = nameof(Loadingferrytrack);

        /// <summary>
        ///   查找类似 上砖摆渡车 的本地化字符串。
        /// </summary>
		public static string Loadingferrytrolley = nameof(Loadingferrytrolley);

        /// <summary>
        ///   查找类似 上砖机 的本地化字符串。
        /// </summary>
		public static string Loadingmachine = nameof(Loadingmachine);

        /// <summary>
        ///   查找类似 仅上砖 的本地化字符串。
        /// </summary>
		public static string LoadingONLY = nameof(LoadingONLY);

        /// <summary>
        ///   查找类似 取货站点 的本地化字符串。
        /// </summary>
		public static string Loadingposition = nameof(Loadingposition);

        /// <summary>
        ///   查找类似 载货状态 的本地化字符串。
        /// </summary>
		public static string Loadingstatus = nameof(Loadingstatus);

        /// <summary>
        ///   查找类似 载车状态 的本地化字符串。
        /// </summary>
		public static string Loadingstatus1 = nameof(Loadingstatus1);

        /// <summary>
        ///   查找类似 载砖状态 的本地化字符串。
        /// </summary>
		public static string Loadingstatus2 = nameof(Loadingstatus2);

        /// <summary>
        ///   查找类似 上砖任务 的本地化字符串。
        /// </summary>
		public static string Loadingtask = nameof(Loadingtask);

        /// <summary>
        ///   查找类似 上砖轨道 的本地化字符串。
        /// </summary>
		public static string Loadingtrack = nameof(Loadingtrack);

        /// <summary>
        ///   查找类似 载车 的本地化字符串。
        /// </summary>
		public static string Loadingtrolley = nameof(Loadingtrolley);

        /// <summary>
        ///   查找类似 登陆 的本地化字符串。
        /// </summary>
		public static string Login = nameof(Login);

        /// <summary>
        ///   查找类似 当前用户： 的本地化字符串。
        /// </summary>
		public static string LoginUser = nameof(LoginUser);

        /// <summary>
        ///   查找类似 手动上砖 的本地化字符串。
        /// </summary>
		public static string Manualloading = nameof(Manualloading);

        /// <summary>
        ///   查找类似 手动模式 的本地化字符串。
        /// </summary>
		public static string Manualmode = nameof(Manualmode);

        /// <summary>
        ///   查找类似 手动下砖 的本地化字符串。
        /// </summary>
		public static string Manualunloading = nameof(Manualunloading);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string Max_tilelayer = nameof(Max_tilelayer);

        /// <summary>
        ///   查找类似 菜单 的本地化字符串。
        /// </summary>
		public static string Menu = nameof(Menu);

        /// <summary>
        ///   查找类似 修改 的本地化字符串。
        /// </summary>
		public static string Modify = nameof(Modify);

        /// <summary>
        ///   查找类似 修改用户 的本地化字符串。
        /// </summary>
		public static string Modifyuser = nameof(Modifyuser);

        /// <summary>
        ///   查找类似 改变作业依据 的本地化字符串。
        /// </summary>
		public static string Modifyworkingcondition = nameof(Modifyworkingcondition);

        /// <summary>
        ///   查找类似 窄车 的本地化字符串。
        /// </summary>
		public static string Narrowtrolley = nameof(Narrowtrolley);

        /// <summary>
        ///   查找类似 下一页 的本地化字符串。
        /// </summary>
		public static string NextPage = nameof(NextPage);

        /// <summary>
        ///   查找类似 否 的本地化字符串。
        /// </summary>
		public static string No = nameof(No);

        /// <summary>
        ///   查找类似 没有指令 的本地化字符串。
        /// </summary>
		public static string Noinstruction = nameof(Noinstruction);

        /// <summary>
        ///   查找类似 非空 的本地化字符串。
        /// </summary>
		public static string Notempty = nameof(Notempty);

        /// <summary>
        ///   查找类似 非空非满 的本地化字符串。
        /// </summary>
		public static string Notempty1 = nameof(Notempty1);

        /// <summary>
        ///   查找类似 有砖轨道 的本地化字符串。
        /// </summary>
		public static string Notemptytrack = nameof(Notemptytrack);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string Off_linewarning = nameof(Off_linewarning);

        /// <summary>
        ///   查找类似 操作模式 的本地化字符串。
        /// </summary>
		public static string Operatingmode = nameof(Operatingmode);

        /// <summary>
        ///   查找类似 下降放货 的本地化字符串。
        /// </summary>
		public static string Origindownunloading = nameof(Origindownunloading);

        /// <summary>
        ///   查找类似 顶升取货 的本地化字符串。
        /// </summary>
		public static string Originuploading = nameof(Originuploading);

        /// <summary>
        ///   查找类似 上级目录 的本地化字符串。
        /// </summary>
		public static string Parentdirectory = nameof(Parentdirectory);

        /// <summary>
        ///   查找类似 密码 的本地化字符串。
        /// </summary>
		public static string Password = nameof(Password);

        /// <summary>
        ///   查找类似 校验密码 的本地化字符串。
        /// </summary>
		public static string Passwordcheck = nameof(Passwordcheck);

        /// <summary>
        ///   查找类似 下午 的本地化字符串。
        /// </summary>
		public static string Pm = nameof(Pm);

        /// <summary>
        ///   查找类似 定位指令 的本地化字符串。
        /// </summary>
		public static string Positioninstruction = nameof(Positioninstruction);

        /// <summary>
        ///   查找类似 上一页 的本地化字符串。
        /// </summary>
		public static string PreviousPage = nameof(PreviousPage);

        /// <summary>
        ///   查找类似 优先级 的本地化字符串。
        /// </summary>
		public static string Prioritygrade = nameof(Prioritygrade);

        /// <summary>
        ///   查找类似 生产时间 的本地化字符串。
        /// </summary>
		public static string Producingtime = nameof(Producingtime);

        /// <summary>
        ///   查找类似 转产完成 的本地化字符串。
        /// </summary>
		public static string Productchanged = nameof(Productchanged);

        /// <summary>
        ///   查找类似 转产状态 的本地化字符串。
        /// </summary>
		public static string Productchangestatus = nameof(Productchangestatus);

        /// <summary>
        ///   查找类似 转产中 的本地化字符串。
        /// </summary>
		public static string Productchanging = nameof(Productchanging);

        /// <summary>
        ///   查找类似 品种名称 的本地化字符串。
        /// </summary>
		public static string Productiondescription = nameof(Productiondescription);

        /// <summary>
        ///   查找类似 设定品种 的本地化字符串。
        /// </summary>
		public static string Productset = nameof(Productset);

        /// <summary>
        ///   查找类似 脉冲值 的本地化字符串。
        /// </summary>
		public static string Pulsevalue = nameof(Pulsevalue);

        /// <summary>
        ///   查找类似 合格品 的本地化字符串。
        /// </summary>
		public static string Qualifiedproduct = nameof(Qualifiedproduct);

        /// <summary>
        ///   查找类似 阅读器断开报警 的本地化字符串。
        /// </summary>
		public static string Readerdisconnectwarning = nameof(Readerdisconnectwarning);

        /// <summary>
        ///   查找类似 库存实际坐标 的本地化字符串。
        /// </summary>
		public static string Realinventorycoordinates = nameof(Realinventorycoordinates);

        /// <summary>
        ///   查找类似 实际点位 的本地化字符串。
        /// </summary>
		public static string Realposition = nameof(Realposition);

        /// <summary>
        ///   查找类似 需求状态 的本地化字符串。
        /// </summary>
		public static string Requirementstatus = nameof(Requirementstatus);

        /// <summary>
        ///   查找类似 复位 的本地化字符串。
        /// </summary>
		public static string Reset = nameof(Reset);

        /// <summary>
        ///   查找类似 复位指令 的本地化字符串。
        /// </summary>
		public static string Resetinstruction = nameof(Resetinstruction);

        /// <summary>
        ///   查找类似 后退倒库 的本地化字符串。
        /// </summary>
		public static string ReturingwarehouseBWD = nameof(ReturingwarehouseBWD);

        /// <summary>
        ///   查找类似 前进倒库 的本地化字符串。
        /// </summary>
		public static string ReturingwarehouseFWD = nameof(ReturingwarehouseFWD);

        /// <summary>
        ///   查找类似 倒库中 的本地化字符串。
        /// </summary>
		public static string Returningtowarehouse = nameof(Returningtowarehouse);

        /// <summary>
        ///   查找类似 倒库 的本地化字符串。
        /// </summary>
		public static string Returningtowarehouse1 = nameof(Returningtowarehouse1);

        /// <summary>
        ///   查找类似 倒库任务 的本地化字符串。
        /// </summary>
		public static string Returntowarehouse = nameof(Returntowarehouse);

        /// <summary>
        ///   查找类似 角色 的本地化字符串。
        /// </summary>
		public static string Role = nameof(Role);

        /// <summary>
        ///   查找类似 运动状态 的本地化字符串。
        /// </summary>
		public static string Runningstatus = nameof(Runningstatus);

        /// <summary>
        ///   查找类似 同向上砖 的本地化字符串。
        /// </summary>
		public static string Samedirectionloading = nameof(Samedirectionloading);

        /// <summary>
        ///   查找类似 同向下砖 的本地化字符串。
        /// </summary>
		public static string Samedirectionunloading = nameof(Samedirectionunloading);

        /// <summary>
        ///   查找类似 保存 的本地化字符串。
        /// </summary>
		public static string Save = nameof(Save);

        /// <summary>
        ///   查找类似 保存更新至后台 的本地化字符串。
        /// </summary>
		public static string Saverenew = nameof(Saverenew);

        /// <summary>
        ///   查找类似 调度设备 的本地化字符串。
        /// </summary>
		public static string Schedulingequipment = nameof(Schedulingequipment);

        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
		public static string Second_gradeproduct = nameof(Second_gradeproduct);

        /// <summary>
        ///   查找类似 规格 的本地化字符串。
        /// </summary>
		public static string Specification = nameof(Specification);

        /// <summary>
        ///   查找类似 存放垛数 的本地化字符串。
        /// </summary>
		public static string Stackqty = nameof(Stackqty);

        /// <summary>
        ///   查找类似 启用 的本地化字符串。
        /// </summary>
		public static string Start = nameof(Start);

        /// <summary>
        ///   查找类似 启用状态 的本地化字符串。
        /// </summary>
		public static string Start1 = nameof(Start1);

        /// <summary>
        ///   查找类似 开始时间 的本地化字符串。
        /// </summary>
		public static string Starttime = nameof(Starttime);

        /// <summary>
        ///   查找类似 停用 的本地化字符串。
        /// </summary>
		public static string Stop = nameof(Stop);

        /// <summary>
        ///   查找类似 停止 的本地化字符串。
        /// </summary>
		public static string Stop1 = nameof(Stop1);

        /// <summary>
        ///   查找类似 终止指令 的本地化字符串。
        /// </summary>
		public static string Stopinstruction = nameof(Stopinstruction);

        /// <summary>
        ///   查找类似 结束时间 的本地化字符串。
        /// </summary>
		public static string Stoptime = nameof(Stoptime);

        /// <summary>
        ///   查找类似 修改策略 的本地化字符串。
        /// </summary>
		public static string Strategymodify = nameof(Strategymodify);

        /// <summary>
        ///   查找类似 区域 的本地化字符串。
        /// </summary>
		public static string String1 = nameof(String1);

        /// <summary>
        ///   查找类似 取消任务 的本地化字符串。
        /// </summary>
		public static string Taskcancelled = nameof(Taskcancelled);

        /// <summary>
        ///   查找类似 完成任务 的本地化字符串。
        /// </summary>
		public static string Taskcompleted = nameof(Taskcompleted);

        /// <summary>
        ///   查找类似 任务开关 的本地化字符串。
        /// </summary>
		public static string Taskswitch = nameof(Taskswitch);

        /// <summary>
        ///   查找类似 任务类型 的本地化字符串。
        /// </summary>
		public static string Tasktype = nameof(Tasktype);

        /// <summary>
        ///   查找类似 任务类型 的本地化字符串。
        /// </summary>
		public static string Tasktype1 = nameof(Tasktype1);

        /// <summary>
        ///   查找类似 取砖流程 的本地化字符串。
        /// </summary>
		public static string Tilecatching = nameof(Tilecatching);

        /// <summary>
        ///   查找类似 无砖 的本地化字符串。
        /// </summary>
		public static string Tilelack = nameof(Tilelack);

        /// <summary>
        ///   查找类似 后退取砖 的本地化字符串。
        /// </summary>
		public static string TileloadingBWD = nameof(TileloadingBWD);

        /// <summary>
        ///   查找类似 前进取砖 的本地化字符串。
        /// </summary>
		public static string TileloadingFWD = nameof(TileloadingFWD);

        /// <summary>
        ///   查找类似 取砖指令 的本地化字符串。
        /// </summary>
		public static string Tileloadinginstruction = nameof(Tileloadinginstruction);

        /// <summary>
        ///   查找类似 存放片数 的本地化字符串。
        /// </summary>
		public static string Tileqty = nameof(Tileqty);

        /// <summary>
        ///   查找类似 轨道库存 的本地化字符串。
        /// </summary>
		public static string Tileqtyontrack = nameof(Tileqtyontrack);

        /// <summary>
        ///   查找类似 有砖 的本地化字符串。
        /// </summary>
		public static string Tileready = nameof(Tileready);

        /// <summary>
        ///   查找类似 放砖流程 的本地化字符串。
        /// </summary>
		public static string Tilereleasing = nameof(Tilereleasing);

        /// <summary>
        ///   查找类似 前进放砖 的本地化字符串。
        /// </summary>
		public static string TilereleasingFWD = nameof(TilereleasingFWD);

        /// <summary>
        ///   查找类似 放砖指令 的本地化字符串。
        /// </summary>
		public static string Tilereleasinginstruction = nameof(Tilereleasinginstruction);

        /// <summary>
        ///   查找类似 储砖轨道 的本地化字符串。
        /// </summary>
		public static string Tilestoragetrack = nameof(Tilestoragetrack);

        /// <summary>
        ///   查找类似 提示 的本地化字符串。
        /// </summary>
		public static string Tip = nameof(Tip);

        /// <summary>
        ///   查找类似 过大 的本地化字符串。
        /// </summary>
		public static string TooLarge = nameof(TooLarge);

        /// <summary>
        ///   查找类似 检查轨道 的本地化字符串。
        /// </summary>
		public static string Trackcheck = nameof(Trackcheck);

        /// <summary>
        ///   查找类似 轨道名称 的本地化字符串。
        /// </summary>
		public static string Trackdescription = nameof(Trackdescription);

        /// <summary>
        ///   查找类似 空轨信号 的本地化字符串。
        /// </summary>
		public static string Trackemptysignal = nameof(Trackemptysignal);

        /// <summary>
        ///   查找类似 轨道尾部 的本地化字符串。
        /// </summary>
		public static string Trackend = nameof(Trackend);

        /// <summary>
        ///   查找类似 满轨信号 的本地化字符串。
        /// </summary>
		public static string Trackfullsignal = nameof(Trackfullsignal);

        /// <summary>
        ///   查找类似 轨道头部 的本地化字符串。
        /// </summary>
		public static string Trackhead = nameof(Trackhead);

        /// <summary>
        ///   查找类似 轨道中部 的本地化字符串。
        /// </summary>
		public static string Trackmid = nameof(Trackmid);

        /// <summary>
        ///   查找类似 轨道读点报警 的本地化字符串。
        /// </summary>
		public static string Trackreadingwarning = nameof(Trackreadingwarning);

        /// <summary>
        ///   查找类似 启用轨道 的本地化字符串。
        /// </summary>
		public static string Trackstarted = nameof(Trackstarted);

        /// <summary>
        ///   查找类似 轨道状态 的本地化字符串。
        /// </summary>
		public static string Trackstatus = nameof(Trackstatus);

        /// <summary>
        ///   查找类似 空满信号 的本地化字符串。
        /// </summary>
		public static string Trackstatussignal = nameof(Trackstatussignal);

        /// <summary>
        ///   查找类似 停用轨道 的本地化字符串。
        /// </summary>
		public static string Trackstopped = nameof(Trackstopped);

        /// <summary>
        ///   查找类似 轨道类型 的本地化字符串。
        /// </summary>
		public static string Tracktype = nameof(Tracktype);

        /// <summary>
        ///   查找类似 空车 的本地化字符串。
        /// </summary>
		public static string Trolleyempty = nameof(Trolleyempty);

        /// <summary>
        ///   查找类似 存放车数 的本地化字符串。
        /// </summary>
		public static string Trolleyqty = nameof(Trolleyqty);

        /// <summary>
        ///   查找类似 移车任务 的本地化字符串。
        /// </summary>
		public static string Truckmovement = nameof(Truckmovement);

        /// <summary>
        ///   查找类似 未知 的本地化字符串。
        /// </summary>
		public static string Unknown = nameof(Unknown);

        /// <summary>
        ///   查找类似 未知大小 的本地化字符串。
        /// </summary>
		public static string UnknownSize = nameof(UnknownSize);

        /// <summary>
        ///   查找类似 卸货坐标 的本地化字符串。
        /// </summary>
		public static string Unloadingcoordinates = nameof(Unloadingcoordinates);

        /// <summary>
        ///   查找类似 下砖摆渡轨道 的本地化字符串。
        /// </summary>
		public static string Unloadingferrytrack = nameof(Unloadingferrytrack);

        /// <summary>
        ///   查找类似 下砖摆渡车 的本地化字符串。
        /// </summary>
		public static string Unloadingferrytrolley = nameof(Unloadingferrytrolley);

        /// <summary>
        ///   查找类似 下砖机 的本地化字符串。
        /// </summary>
		public static string Unloadingmachine = nameof(Unloadingmachine);

        /// <summary>
        ///   查找类似 仅下砖 的本地化字符串。
        /// </summary>
		public static string UnloadingONLY = nameof(UnloadingONLY);

        /// <summary>
        ///   查找类似 卸货站点 的本地化字符串。
        /// </summary>
		public static string Unloadingposition = nameof(Unloadingposition);

        /// <summary>
        ///   查找类似 下砖任务 的本地化字符串。
        /// </summary>
		public static string Unloadingtask = nameof(Unloadingtask);

        /// <summary>
        ///   查找类似 下砖轨道 的本地化字符串。
        /// </summary>
		public static string Unloadingtrack = nameof(Unloadingtrack);

        /// <summary>
        ///   查找类似 用户 的本地化字符串。
        /// </summary>
		public static string User = nameof(User);

        /// <summary>
        ///   查找类似 入库策略 的本地化字符串。
        /// </summary>
		public static string Warehouseinstrategy = nameof(Warehouseinstrategy);

        /// <summary>
        ///   查找类似 出库策略 的本地化字符串。
        /// </summary>
		public static string Warehouseoutstrategy = nameof(Warehouseoutstrategy);

        /// <summary>
        ///   查找类似 倒库数量 的本地化字符串。
        /// </summary>
		public static string Warehousereturningqty = nameof(Warehousereturningqty);

        /// <summary>
        ///   查找类似 警告类型 的本地化字符串。
        /// </summary>
		public static string Warningtype = nameof(Warningtype);

        /// <summary>
        ///   查找类似 宽车 的本地化字符串。
        /// </summary>
		public static string Widetrolley = nameof(Widetrolley);

        /// <summary>
        ///   查找类似 作业依据 的本地化字符串。
        /// </summary>
		public static string Workingcondition = nameof(Workingcondition);

        /// <summary>
        ///   查找类似 切换工作模式 的本地化字符串。
        /// </summary>
		public static string Workingmodeshift = nameof(Workingmodeshift);

        /// <summary>
        ///   查找类似 工位品种 的本地化字符串。
        /// </summary>
		public static string Workstationproduct = nameof(Workstationproduct);

        /// <summary>
        ///   查找类似 是 的本地化字符串。
        /// </summary>
		public static string Yes = nameof(Yes);

        /// <summary>
        ///   查找类似 放大 的本地化字符串。
        /// </summary>
		public static string ZoomIn = nameof(ZoomIn);

        /// <summary>
        ///   查找类似 缩小 的本地化字符串。
        /// </summary>
		public static string ZoomOut = nameof(ZoomOut);

    }
}