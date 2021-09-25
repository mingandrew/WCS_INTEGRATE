using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.device;
using module.deviceconfig;
using module.line;
using module.track;
using module.window;
using module.window.device;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using wcs.Data.View;
using wcs.Dialog;
using wcs.Tools;

namespace wcs.ViewModel
{
    public class InitSettingViewModel : ViewModelBase
    {
        public InitSettingViewModel()
        {
            _areamenulist = new ObservableCollection<SettingMenuView>();
            _arealist = new ObservableCollection<Area>();
            _deletearealist = new List<uint>();
            _currentarealist = new List<Area>();

            _linelist = new ObservableCollection<Line>();
            _currentlinelist = new List<Line>();
            _deletelinelist = new List<uint>();

            TrackList = new ObservableCollection<Track>();
            _deletetracklist = new List<uint>();
            _currenttracklist = new List<Track>();

            TileLifterSettingViewList = new ObservableCollection<TileLifterSettingView>();
            _currenttilelist = new List<TileLifterSettingView>();
            _deletetilelist = new List<uint>();

            CarrierSettingViewList = new ObservableCollection<CarrierSettingView>();
            _currentcarrierlist = new List<CarrierSettingView>();
            _deletecarrierlist = new List<uint>();

            FerrySettingViewList = new ObservableCollection<FerrySettingView>();
            _currentferrylist = new List<FerrySettingView>();
            _deleteferrylist = new List<uint>();

            _selectmenuview = new SettingMenuView();

            Refresh();
        }


        #region[字段]

        private bool updating { set; get; }
        private SettingMenuView _selectmenuview;

        #region[菜单]

        private ObservableCollection<SettingMenuView> _areamenulist;
        private ObservableCollection<Area> _arealist;
        private ObservableCollection<Line> _linelist;

        public bool showarea;
        public bool showline;
        public bool showtrack = true;
        public bool showcarrier = true;
        public bool showferry = true;
        public bool showtilelifter = true;
        public List<Line> lines;
        #endregion

        #region[区域]
        private string area_name;
        private uint up_car;
        private uint down_car;

        private Area _selectarea;

        /// <summary>
        /// 当前数据库里的区域列表
        /// </summary>
        private List<Area> _currentarealist { set; get; }
        /// <summary>
        /// 要删除的线id
        /// </summary>
        private List<uint> _deletearealist { set; get; }
        #endregion

        #region[线]

        private string line_name;
        private ushort line_num;
        private ushort sort_count;
        private ushort up_count;
        private ushort down_count;
        private LineTypeE line_type;
        private byte full_qty;

        private Line _selectline;
        /// <summary>
        /// 当前数据库里的线列表
        /// </summary>
        private List<Line> _currentlinelist { set; get; }
        /// <summary>
        /// 要删除的线id
        /// </summary>
        private List<uint> _deletelinelist { set; get; }
        #endregion

        #region[轨道]
        private Track _selecttrack;
        private List<Track> tracklist;
        private List<Track> _currenttracklist { set; get; }
        private List<uint> _deletetracklist { set; get; }

        private string uptilerfid1 = "{0}10";
        private string downtilerfid1 = "{0}80";
        private string inrfid1 = "{0}02";
        private string inrfid2 = "{0}48";
        private string outrfid1 = "{0}98";
        private string ferrfid1 = "{0}50";

        private string downtilerfids = "{0}00#{0}80#{0}99#{0}82#{0}#";
        private string uptilerfids = "{0}00#{0}08#{0}10#{0}99#{0}#";
        private string inrfids = "{0}00#{0}02#{0}04#{0}06#{0}44#{0}46#{0}48#{0}#";
        private string outrfids = "{0}54#{0}56#{0}94#{0}96#{0}98#{0}99#{0}";
        private string inoutrfids = "{0}00#{0}02#{0}04#{0}06#{0}94#{0}96#{0}98#{0}99#{0}#";
        private string ferrfids = "{0}48#{0}50#{0}52#{0}";
        #endregion

        #region [砖机]
        private TileLifterSettingView _selecttilelifter;
        private List<TileLifterSettingView> _currenttilelist { set; get; }
        private List<uint> _deletetilelist { set; get; }

        #endregion

        #region [运输车]
        private CarrierSettingView _selectcarrier;
        private List<CarrierSettingView> _currentcarrierlist { set; get; }
        private List<uint> _deletecarrierlist { set; get; }

        #endregion

        #region [摆渡车]
        private FerrySettingView _selectferry;
        private List<FerrySettingView> _currentferrylist { set; get; }
        private List<uint> _deleteferrylist { set; get; }

        #endregion

        #endregion

        #region[属性]
        public bool IsAdd { set; get; }

        public SettingMenuView SelectMenuView
        {
            get => _selectmenuview;
            set => Set(ref _selectmenuview, value);
        }

        #region[菜单]
        public ObservableCollection<SettingMenuView> SettingMenuList
        {
            get => _areamenulist;
            set => Set(ref _areamenulist, value);
        }

        public bool ShowArea
        {
            get => showarea;
            set => Set(ref showarea, value);
        }

        public bool ShowLine
        {
            get => showline;
            set => Set(ref showline, value);
        }

        public bool ShowTrack
        {
            get => showtrack;
            set => Set(ref showtrack, value);
        }

        public bool ShowCarrier
        {
            get => showcarrier;
            set => Set(ref showcarrier, value);
        }

        public bool ShowFerry
        {
            get => showferry;
            set => Set(ref showferry, value);
        }

        public bool ShowTilelifter
        {
            get => showtilelifter;
            set => Set(ref showtilelifter, value);
        }
        #endregion

        #region[区域]

        public ObservableCollection<Area> AreaList
        {
            get => _arealist;
            set => Set(ref _arealist, value);
        }

        public Area SelectArea
        {
            get => _selectarea;
            set
            {
                Set(ref _selectarea, value);
                if (value != null)
                {
                    SetAreaView(value);
                }
            }
        }

        public string AreaName
        {
            get => area_name;
            set => Set(ref area_name, value);
        }

        public uint UpCar
        {
            get => up_car;
            set => Set(ref up_car, value);
        }

        public uint DownCar
        {
            get => down_car;
            set => Set(ref down_car, value);
        }

        #endregion

        #region[线]
        public ObservableCollection<Line> LineList
        {
            get => _linelist;
            set => Set(ref _linelist, value);
        }

        public Line SelectLine
        {
            get => _selectline;
            set
            {
                Set(ref _selectline, value);
                SetLineView(_selectline);
            }
        }

        public string LineName
        {
            get => line_name;
            set => Set(ref line_name, value);
        }

        public ushort LineNum
        {
            get => line_num;
            set => Set(ref line_num, value);
        }

        public ushort SortCount
        {
            get => sort_count;
            set => Set(ref sort_count, value);
        }

        public ushort UpCount
        {
            get => up_count;
            set => Set(ref up_count, value);
        }

        public ushort DownCount
        {
            get => down_count;
            set => Set(ref down_count, value);
        }

        /// <summary>
        /// 线路类型
        /// </summary>
        public LineTypeE LineType
        {
            get => (LineTypeE)line_type;
            set => Set(ref line_type, value);
        }

        public byte FullQty
        {
            get => full_qty;
            set => Set(ref full_qty, value);
        }
        #endregion

        #region[轨道]

        public ObservableCollection<Track> TrackList { set; get; }

        public Track SelectTrack
        {
            get => _selecttrack;
            set => Set(ref _selecttrack, value);
        }
        #endregion

        #region[砖机]

        public ObservableCollection<TileLifterSettingView> TileLifterSettingViewList { set; get; }

        public TileLifterSettingView SelectTileLifter
        {
            get => _selecttilelifter;
            set => Set(ref _selecttilelifter, value);
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

        #region[摆渡车]

        public ObservableCollection<FerrySettingView> FerrySettingViewList { set; get; }

        public FerrySettingView SelectFerry
        {
            get => _selectferry;
            set => Set(ref _selectferry, value);
        }

        #endregion

        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> MenuTreeViewChangeCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(MenuTreeViewChange)).Value;
        public RelayCommand<string> ComfirmCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(ComfirmFunction)).Value;
        public RelayCommand<string> ActionCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(ActionFunction)).Value;
        public RelayCommand<string> DeviceSelectedCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DeviceSelected)).Value;
        public RelayCommand<string> DeviceCheckSelectedCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(DeviceCheckSelected)).Value;
        public RelayCommand<string> TrackSelectedCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TrackSelected)).Value;
        public RelayCommand FerryTrackSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(FerryTrackSelected)).Value;
        public RelayCommand GoodSizeSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(GoodSizeSelected)).Value;

        #endregion

        #region[方法]

        #region [通用]

        /// <summary>
        /// 初始化配置菜单
        /// </summary>
        private void Refresh()
        {
            RefreshAreaList();
            RefreshLineList();
            RefreshMenu();
            RefreshTrackList();
            RefreshCarrierList();
            RefreshFerryList();
        }

        /// <summary>
        /// 切换，选择树节点
        /// </summary>
        /// <param name="orgs"></param>
        private void MenuTreeViewChange(RoutedEventArgs orgs)
        {
            if (orgs != null)
            {
                if (orgs.OriginalSource is TreeView pro && pro.SelectedItem is SettingMenuView menu)
                {
                    SelectMenuView = menu;
                }
                else if (((System.Windows.FrameworkElement)orgs.OriginalSource).DataContext is SettingMenuView menu2)
                {
                    SelectMenuView = menu2;
                }
                else
                {
                    return;
                }
                Area _sarea = null;
                if (SelectMenuView.p_id != 0)
                {
                    _sarea = _currentarealist.Find(c => c.id == SelectMenuView.p_id);
                }
                else
                {
                    _sarea = _currentarealist.Find(c => c.id == SelectMenuView.id);
                }

                if (_sarea == null)
                {
                    Growl.Warning("出现区域错误，请重启调度系统！");
                    return;
                }
                Area temparea = new Area()
                {
                    id = _sarea.id,
                    name = _sarea.name,
                    memo = _sarea.memo,
                    up_car_count = _sarea.up_car_count,
                    down_car_count = _sarea.down_car_count,
                };
                AreaList.Clear();
                AreaList.Add(temparea);
                SelectArea = temparea;

                Line _sline = _currentlinelist.Find(c => c.area_id == _sarea.id && c.line == SelectMenuView.id);
                if (SelectMenuView.p_id != 0 && _sline != null)
                {
                    LineList.Clear();
                    Line ll = new Line()
                    {
                        id = _sline.id,
                        area_id = _sline.area_id,
                        line = _sline.line,
                        name = _sline.name,
                        sort_task_qty = _sline.sort_task_qty,
                        up_task_qty = _sline.up_task_qty,
                        down_task_qty = _sline.down_task_qty,
                        line_type = _sline.line_type,
                        full_qty = _sline.full_qty,
                    };
                    LineList.Add(ll);
                    SelectLine = ll;
                }
            }
        }


        /// <summary>
        /// 保存单个/删除单个
        /// </summary>
        /// <param name="tag"></param>
        private void ComfirmFunction(string tag)
        {
            switch (tag)
            {
                case "ComfirmArea":
                    #region[区域行保存]
                    if (SelectArea == null)
                    {
                        Growl.Warning("请选择需要保存的区域数据！");
                        return;
                    }
                    //新增/保存成功标志
                    bool success = SaveArea(false);
                    if (!success)
                    {
                        Growl.Warning("保存失败！");
                        return;
                    }
                    break;
                #endregion
                case "DeleteArea":
                    #region[区域行删除]
                    if (SelectArea == null)
                    {
                        return;
                    }
                    if (PubMaster.Area.IsAreaUsedOther(SelectArea.id, out string msg))
                    {
                        Growl.Warning(msg);
                        return;
                    }
                    if (SelectArea.id != 0)
                    {
                        _deletearealist.Add(SelectArea.id);
                    }
                    AreaList.Remove(SelectArea);
                    break;
                #endregion
                case "ComfirmLine":
                    #region[线行保存]
                    if (SelectLine == null)
                    {
                        Growl.Warning("请选择需要保存的线路数据！");
                        return;
                    }
                    //新增/保存成功标志
                    success = SaveLine(false);
                    if (!success)
                    {
                        Growl.Warning("保存失败！");
                        return;
                    }
                    break;
                #endregion
                case "DeleteLine":
                    #region[线行删除]
                    if (SelectLine == null)
                    {
                        return;
                    }
                    // 判断线是否有在使用
                    if (PubMaster.Area.IsLineUsedOther(SelectLine.area_id, SelectLine.line, out msg))
                    {
                        Growl.Warning(msg);
                        return;
                    }
                    if (SelectLine.id != 0)
                    {
                        _deletelinelist.Add(SelectLine.id);
                    }
                    LineList.Remove(SelectLine);
                    break;
                #endregion
                case "ComfirmTrack":
                    #region[轨道行保存]
                    if (SelectTrack == null)
                    {
                        Growl.Warning("请选择需要保存的轨道数据！");
                        return;
                    }
                    //新增/保存成功标志
                    success = SaveTrack(false);
                    if (!success)
                    {
                        Growl.Warning("保存失败！");
                        return;
                    }
                    break;
                #endregion
                case "DeleteTrack":
                    #region[轨道删除]
                    if (SelectTrack == null)
                    {
                        Growl.Warning("请选择轨道删除，删除失败！");
                        return;
                    }
                    // 判断轨道是否再使用中
                    if (PubMaster.Area.IsTrackUsedOther(SelectTrack.id, out msg))
                    {
                        Growl.Warning(msg);
                        return;
                    }
                    if (SelectTrack.id != 0)
                    {
                        _deletetracklist.Add(SelectTrack.id);
                    }
                    TrackList.Remove(SelectTrack);
                    break;
                #endregion
                case "ComfirmTile":
                    #region[砖机保存]
                    if (SelectTileLifter == null)
                    {
                        Growl.Warning("请选择需要保存的砖机数据！");
                        return;
                    }
                    success = SaveTile(false);
                    if (!success)
                    {
                        Growl.Warning("保存失败！");
                        return;
                    }
                    break;
                #endregion
                case "DeleteTile":
                    #region[砖机删除]
                    if (SelectTileLifter == null)
                    {
                        return;
                    }
                    // 判断砖机是否再使用中
                    if (PubMaster.Area.IsHaveInAreaDevTra(SelectTileLifter.area, SelectTileLifter.id))
                    {
                        Growl.Warning("砖机有分配能去的轨道，请先删除砖机所配置的轨道！");
                        return;
                    }
                    if (SelectTileLifter.id != 0)
                    {
                        _deletetilelist.Add(SelectTileLifter.id);
                    }
                    TileLifterSettingViewList.Remove(SelectTileLifter);
                    break;
                #endregion
                case "ComfirmCarrier":
                    #region[运输车保存]
                    if (SelectCarrier == null)
                    {
                        Growl.Warning("请选择需要保存的运输车数据！");
                        return;
                    }
                    success = SaveCarrier(false);
                    if (!success)
                    {
                        Growl.Warning("保存失败！");
                        return;
                    }
                    break;
                #endregion
                case "DeleteCarrier":
                    #region[运输车删除]
                    if (SelectCarrier == null)
                    {
                        return;
                    }
                    if (SelectCarrier.id != 0)
                    {
                        _deletecarrierlist.Add(SelectCarrier.id);
                    }
                    CarrierSettingViewList.Remove(SelectCarrier);
                    break;
                #endregion
                case "ComfirmFerry":
                    #region[摆渡车保存]
                    if (SelectFerry == null)
                    {
                        Growl.Warning("请选择需要保存的摆渡车数据！");
                        return;
                    }
                    success = SaveFerry(false);
                    if (!success)
                    {
                        Growl.Warning("保存失败！");
                        return;
                    }
                    break;
                #endregion
                case "DeleteFerry":
                    #region[摆渡车删除]
                    if (SelectFerry == null)
                    {
                        return;
                    }
                    // 判断摆渡车是否再使用中
                    if (PubMaster.Area.IsHaveInAreaDevTra(SelectFerry.area, SelectFerry.id))
                    {
                        Growl.Warning("摆渡车有分配能去的轨道，请先删除摆渡车所配置的轨道！");
                        return;
                    }
                    if (SelectFerry.id != 0)
                    {
                        _deleteferrylist.Add(SelectFerry.id);
                    }
                    FerrySettingViewList.Remove(SelectFerry);
                    break;
                #endregion
                default:
                    break;
            }
        }

        /// <summary>
        /// 新增/全部保存
        /// </summary>
        /// <param name="obj"></param>
        private void ActionFunction(string obj)
        {
            switch (obj)
            {
                case "RefreshArea":
                    #region[区域刷新]
                    Refresh();
                    break;
                #endregion
                case "AddArea":
                    #region[新增区域]
                    AreaList.Insert(0, new Area());
                    break;
                #endregion
                case "SaveArea":
                    #region[保存区域]
                    SaveArea(true);
                    break;
                #endregion
                case "RefreshLine":
                    #region[线刷新]
                    Refresh();
                    break;
                #endregion
                case "AddLine":
                    #region[新增线]
                    if (SelectArea == null)
                    {
                        Growl.Warning("请先选择区域信息!");
                        break;
                    }
                    if (SelectArea.id == 0)
                    {
                        Growl.Warning("请先保存区域信息!");
                        break;
                    }
                    LineList.Insert(0, new Line());
                    break;
                #endregion
                case "SaveLine":
                    #region[保存线]
                    SaveLine(true);
                    break;
                #endregion
                case "RefreshTrack":
                    #region[刷新轨道]
                    RefreshTrackList();
                    break;
                #endregion
                case "AddTrack":
                    #region[新增轨道]
                    if (SelectArea == null)
                    {
                        Growl.Warning("请先选择区域信息!");
                        break;
                    }
                    if (SelectArea.id == 0)
                    {
                        Growl.Warning("请先保存区域信息!");
                        break;
                    }
                    TrackList.Insert(0, new Track());
                    break;
                #endregion
                case "SaveTrack":
                    #region[保存轨道]
                    SaveTrack(true);
                    break;
                #endregion
                case "BatchAddTrack":
                    #region[批量新增轨道]
                    BatchAddTrackList();
                    break;
                #endregion
                case "RefreshTile":
                    #region[刷新砖机]
                    RefreshTileList();
                    break;
                #endregion
                case "AddTile":
                    #region[新增砖机]
                    if (SelectArea == null)
                    {
                        Growl.Warning("请先选择区域信息!");
                        break;
                    }
                    if (SelectArea.id == 0)
                    {
                        Growl.Warning("请先保存区域信息!");
                        break;
                    }
                    TileLifterSettingViewList.Insert(0, new TileLifterSettingView(new Device(), new ConfigTileLifter()));
                    break;
                #endregion
                case "SaveTile":
                    #region[保存轨道]
                    SaveTile(true);
                    break;
                #endregion
                case "BatchAddTile":
                    #region[批量新增砖机]
                    BatchAddTile();
                    break;
                #endregion
                case "RefreshCarrier":
                    #region[刷新运输车]
                    RefreshCarrierList();
                    break;
                #endregion
                case "AddCarrier":
                    #region[新增运输车]
                    if (SelectArea == null)
                    {
                        Growl.Warning("请先选择区域信息!");
                        break;
                    }
                    if (SelectArea.id == 0)
                    {
                        Growl.Warning("请先保存区域信息!");
                        break;
                    }
                    CarrierSettingViewList.Insert(0, new CarrierSettingView(new Device(), new ConfigCarrier()));
                    break;
                #endregion
                case "SaveCarrier":
                    #region[保存运输车]
                    SaveCarrier(true);
                    break;
                #endregion
                case "BatchAddCarrier":
                    #region[批量新增运输车]
                    BatchAddCarrier();
                    break;
                #endregion
                case "RefreshFerry":
                    #region[刷新摆渡车]
                    RefreshFerryList();
                    break;
                #endregion
                case "AddFerry":
                    #region[新增摆渡车]
                    if (SelectArea == null)
                    {
                        Growl.Warning("请先选择区域信息!");
                        break;
                    }
                    if (SelectArea.id == 0)
                    {
                        Growl.Warning("请先保存区域信息!");
                        break;
                    }
                    FerrySettingViewList.Insert(0, new FerrySettingView(new Device(), new ConfigFerry()));
                    break;
                #endregion
                case "SaveFerry":
                    #region[保存摆渡车]
                    SaveFerry(true);
                    break;
                #endregion
                case "BatchAddFerry":
                    #region[批量新增摆渡车]
                    BatchAddFerry();
                    break;
                #endregion
                default:
                    break;
            }
        }

        public void RefreshMenu()
        {
            SettingMenuList.Clear();

            List<Area> areas = PubMaster.Area.GetAreaList();
            //添加区域的菜单
            foreach (Area a in areas)
            {
                SettingMenuView s = new SettingMenuView()
                {
                    id = a.id,
                    name = a.name,
                    p_id = 0,
                    level = 1,
                    childlist = new List<SettingMenuView>(),
                };
                List<Line> line = PubMaster.Area.GetLineList(a.id);
                //获取对应区域的线路信息
                List<Line> ls = line.FindAll(c => c.area_id == a.id);
                if (ls == null || ls.Count == 0)
                {
                    SettingMenuList.Add(s);
                    continue;
                }
                //添加每一条线路的信息进区域的childlist
                foreach (Line l in ls)
                {
                    SettingMenuView temp = new SettingMenuView()
                    {
                        id = l.line,
                        name = l.name,
                        p_id = a.id,
                        level = 2,
                    };
                    s.childlist.Add(temp);
                }
                SettingMenuList.Add(s);
            }
        }

        public bool CheckAreaAndLine()
        {
            if (SelectArea == null)
            {
                Growl.Warning("请先选择区域信息!");
                return false;
            }
            if (SelectArea.id == 0)
            {
                Growl.Warning("请先保存区域信息!");
                return false;
            }
            if (SelectLine == null)
            {
                Growl.Warning("请先选择线路信息!");
                return false;
            }
            if (SelectLine.id == 0)
            {
                Growl.Warning("请先保存线路信息!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将字符串的IP转换成long类型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        /// <summary>
        /// 将long类型的IP转换成字符串
        /// </summary>
        /// <param name="ipInt"></param>
        /// <returns></returns>
        public string IntToIp(long ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append(ipInt & 0xFF);
            return sb.ToString();
        }

        #endregion

        #region[区域]

        public void RefreshAreaList()
        {
            //获取全部的区域和线的信息

            List<Area> areas = PubMaster.Area.GetAreaList();
            _deletearealist.Clear();
            _currentarealist.Clear();
            AreaList.Clear();
            foreach (var aa in areas)
            {
                Area temparea = new Area()
                {
                    id = aa.id,
                    name = aa.name,
                    memo = aa.memo,
                    up_car_count = aa.up_car_count,
                    down_car_count = aa.down_car_count,
                };
                AreaList.Add(temparea);
                _currentarealist.Add(temparea);
            }
        }

        /// <summary>
        /// 根据选择的区域，设置线、轨道、设备的信息
        /// </summary>
        /// <param name="id"></param>
        public void SetAreaView(Area area)
        {
            RefreshTrackList();
            RefreshLineList();
            RefreshTileList();
            RefreshCarrierList();
            RefreshFerryList();
            //RefreshMenu();
        }

        /// <summary>
        /// 保存区域
        /// </summary>
        /// <param name="isBatch">是否批量保存</param>
        /// <returns></returns>
        public bool SaveArea(bool isBatch)
        {
            bool success = true;
            //单个保存
            if (!isBatch)
            {
                success = SaveAreaSingle(SelectArea);
            }
            else
            {
                foreach (Area a in AreaList)
                {
                    SaveAreaSingle(a);
                }
                if (_deletearealist.Count > 0)
                {
                    PubMaster.Area.BatchDeleteArea(_deletearealist);
                }
            }
            Refresh();
            if (success)
            {
                Growl.Success("保存成功！");
            }
            return success;
        }

        /// <summary>
        /// 保存单个区域信息
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private bool SaveAreaSingle(Area area)
        {
            bool success = false;
            if (area.id == 0)
            {
                if (_currentarealist.Exists(c => c.name.Equals(area.name)))
                {
                    Growl.Warning(string.Format("已有相同名称{0}的区域，不可重复添加！", area.name));
                    return false;
                }
                if (string.IsNullOrWhiteSpace(area.name))
                {
                    Growl.Warning("区域名称不可为空白字符！");
                    return false;
                }
                // 添加区域
                success = PubMaster.Area.AddArea(area);
            }
            else
            {
                success = PubMaster.Area.UpdateArea(area);
            }
            return success;
        }

        #endregion

        #region[线]


        /// <summary>
        /// 根据选择的区域和线，设置轨道、设备的信息
        /// </summary>
        /// <param name="id"></param>
        public void SetLineView(Line line)
        {
            if (SelectArea == null)
            {
                SelectArea = AreaList.First(c => c.id == line.area_id);
            }
            RefreshTrackList();
            RefreshTileList();
            RefreshCarrierList();
            RefreshFerryList();
            //RefreshMenu();
        }

        public bool SaveLine(bool isBatch)
        {
            bool success = true;
            if (SelectArea == null || SelectArea.id == 0)
            {
                SelectArea = AreaList.First(c => c.id == SelectLine.area_id);
                if (SelectArea == null)
                {
                    Growl.Warning("请先选择区域信息!");
                    return false;
                }
            }
            //单个保存
            if (!isBatch)
            {
                if (SelectLine == null)
                {
                    Growl.Warning("请选择需要保存的线路数据！");
                    return false;
                }
                success = SaveLineSingle(SelectLine);
            }
            else
            {
                foreach (Line l in LineList)
                {
                    success = SaveLineSingle(l);
                }
                if (_deletelinelist.Count > 0)
                {
                    PubMaster.Area.BatchDeleteLine(_deletelinelist);
                }
            }
            Refresh();
            if (success)
            {
                Growl.Success("保存成功！");
            }
            return success;
        }

        public bool SaveLineSingle(Line line)
        {
            bool success = false;
            //新增
            if (line.id == 0)
            {
                line.area_id = SelectArea.id;
                if (_currentlinelist.Exists(c => c.area_id == line.area_id && (c.name.Equals(line.name) || c.line.Equals(line.line))))
                {
                    Growl.Warning(string.Format("同区域内已有相同的名称{0}/线号{1}， 不可重复添加", line.name, line.name));
                    return false;
                }
                if (line.line == 0 || string.IsNullOrWhiteSpace(line.name))
                {
                    Growl.Warning(string.Format("同区域内线号/线名不可为空白字符"));
                    return false;
                }
                success = PubMaster.Area.AddLine(line);
            }
            //编辑
            else
            {
                success = PubMaster.Area.UpdateLine(line);
            }
            return success;
        }

        public void RefreshLineList()
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                //Growl.Warning("请先选择区域");
                return;
            }
            List<Line> line = PubMaster.Area.GetLineList(SelectArea.id);
            LineList.Clear();
            _deletelinelist.Clear();
            _currentlinelist.Clear();
            if (line != null && line.Count != 0)
            {
                foreach (Line t in line)
                {
                    Line ll = new Line()
                    {
                        id = t.id,
                        area_id = t.area_id,
                        line = t.line,
                        name = t.name,
                        sort_task_qty = t.sort_task_qty,
                        up_task_qty = t.up_task_qty,
                        down_task_qty = t.down_task_qty,
                        line_type = t.line_type,
                        full_qty = t.full_qty,
                    };
                    LineList.Add(ll);
                    _currentlinelist.Add(ll);
                }
            }
        }

        #endregion

        #region[轨道]

        /// <summary>
        /// 刷新轨道信息
        /// </summary>
        public void RefreshTrackList()
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                return;
            }
            List<Track> tlist = PubMaster.Track.GetTrackList(SelectArea.id);
            if (SelectLine != null)
            {
                tlist = PubMaster.Track.GetLineTracks(SelectArea.id, SelectLine.line);
            }
            TrackList.Clear();
            _deletetracklist.Clear();
            _currenttracklist.Clear();
            if (tlist != null && tlist.Count != 0)
            {
                foreach (Track t in tlist)
                {
                    Track tra = new Track()
                    {
                        id = t.id,
                        name = t.name,
                        width = t.width,
                        left_distance = t.left_distance,
                        right_distance = t.right_distance,
                        ferry_up_code = t.ferry_up_code,
                        RFIDs = t.RFIDs,
                        order = t.order,
                        same_side_inout = t.same_side_inout,
                    };
                    TrackList.Add(t);
                    _currenttracklist.Add(t);
                }
            }

        }

        public bool SaveTrack(bool isBatch)
        {
            bool success = true;
            if (!isBatch)
            {
                if (!CheckAreaAndLine())
                {
                    return false;
                }
                success = SaveTrackSingle(SelectTrack);
            }
            else
            {
                foreach (Track t in TrackList)
                {
                    SaveTrackSingle(t);
                }
                if (_deletetracklist.Count > 0)
                {
                    PubMaster.Track.DeleteTrack(_deletetracklist);
                }
            }
            RefreshTrackList();
            if (success)
            {
                Growl.Success("保存轨道成功！");
            }
            return success;
        }

        public bool SaveTrackSingle(Track track)
        {
            bool success = false;
            //新增
            if (track.id == 0)
            {
                track.area = (ushort)SelectArea.id;
                track.line = (ushort)SelectLine.line;
                track.ferry_down_code = track.ferry_up_code;
                if (_currenttracklist.Exists(c => c.area == track.area && c.name == track.name))
                {
                    Growl.Warning("同区域已有相同名称的轨道！");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(track.name))
                {
                    Growl.Warning("轨道名称不能为空！");
                    return false;
                }
                success = PubMaster.Track.AddTrack(track, out string rs);
            }
            //编辑
            else
            {
                //编辑轨道
                success = PubMaster.Track.UpdateTrack(track);
            }
            return success;
        }

        private async void BatchAddTrackList()
        {
            if (!CheckAreaAndLine())
            {
                return;
            }
            DialogResult result = await HandyControl.Controls.Dialog.Show<BatchAddTrackDialog>()
                              .Initialize<BatchAddTrackViewModel>((vm) =>
                              {
                                  vm.SetAddType("Track");
                              }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && rs && result.p5 is TrackTypeE type)
            {
                uint startnum = (uint)result.p2;
                //string suffix = (string)result.p3;
                uint count = (uint)result.p4;
                uint width = (uint)result.p6;
                uint left = (uint)result.p7;
                uint right = (uint)result.p8;
                for (int i = 0; i < count; i++, startnum++)
                {
                    Track t = new Track()
                    {
                        //name = string.Format("{0}_{1}", startnum, suffix),
                        Type = type,
                        width = (ushort)width,
                        left_distance = (ushort)left,
                        right_distance = (ushort)right,
                        ferry_up_code = (ushort)startnum,
                    };
                    //设置轨道的地标
                    switch (t.Type)
                    {
                        case TrackTypeE.上砖轨道:
                            //t.ferry_up_code = (ushort)(startnum + 500);
                            SetTrackRfid(t, uptilerfid1, uptilerfid1, uptilerfids);
                            break;
                        case TrackTypeE.下砖轨道:
                            //t.ferry_up_code = (ushort)(startnum + 100);
                            SetTrackRfid(t, downtilerfid1, downtilerfid1, downtilerfids);
                            break;
                        case TrackTypeE.储砖_入:
                            //t.ferry_up_code = (ushort)(startnum + 300);
                            SetTrackRfid(t, inrfid1, inrfid2, inrfids);
                            t.order = (short)(startnum - 300);
                            break;
                        case TrackTypeE.储砖_出:
                            //t.ferry_up_code = (ushort)(startnum + 300);
                            SetTrackRfid(t, outrfid1, outrfid1, outrfids);
                            t.order = (short)(startnum - 300);
                            break;
                        case TrackTypeE.储砖_出入:
                            //t.ferry_up_code = (ushort)(startnum + 300);
                            SetTrackRfid(t, inrfid1, outrfid1, inoutrfids);
                            t.order = (short)(startnum - 300);
                            break;
                        case TrackTypeE.摆渡车_入:
                            //t.ferry_up_code = (ushort)(startnum + 200);
                            SetTrackRfid(t, ferrfid1, ferrfid1, ferrfids);
                            break;
                        case TrackTypeE.摆渡车_出:
                            //t.ferry_up_code = (ushort)(startnum + 400);
                            SetTrackRfid(t, ferrfid1, ferrfid1, ferrfids);
                            break;
                    }
                    t.name = string.Format("{0}_{1}", t.ferry_up_code, t.Type);

                    SaveTrackSingle(t);
                }

                RefreshTrackList();
            }
        }

        private void SetTrackRfid(Track t, string rfid_1, string rfid_2, string rfids)
        {
            t.rfid_1 = Convert.ToUInt16(string.Format(rfid_1, t.ferry_up_code));
            t.rfid_2 = Convert.ToUInt16(string.Format(rfid_2, t.ferry_up_code));
            t.rfids = string.Format(rfids, t.ferry_up_code);
            t.GetAllRFID();
        }



        #endregion

        #region[砖机]

        public void RefreshTileList()
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                return;
            }
            List<Device> devs = PubMaster.Device.GetTileLifters(SelectArea.id);
            if (SelectLine != null)
            {
                devs = PubMaster.Device.GetDevices(SelectArea.id, SelectLine.line, DeviceTypeE.上砖机, DeviceTypeE.下砖机, DeviceTypeE.砖机);
            }
            List<uint> devids = devs.Select(c => c.id).ToList();
            List<ConfigTileLifter> contiles = PubMaster.DevConfig.GetTileLifterList(devids);

            TileLifterSettingViewList.Clear();
            _deletetilelist.Clear();
            _currenttilelist.Clear();
            for (int i = 0; i < devs.Count; i++)
            {
                Device dev = devs[i];
                ConfigTileLifter config = contiles[i];
                TileLifterSettingView t = new TileLifterSettingView(dev, config);
                TileLifterSettingViewList.Add(t);
                _currenttilelist.Add(t);
            }
        }

        private async void BatchAddTile()
        {
            if (!CheckAreaAndLine())
            {
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<BatchAddTileDialog>()
                              .Initialize<BatchAddTileViewModel>((vm) =>
                              {
                                  vm.SetAddType("Tile");
                              }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && rs)
            {
                uint startnum = (uint)result.p2;
                string startip = (string)result.p3;
                uint count = (uint)result.p4;
                DeviceTypeE type = (DeviceTypeE)result.p5;
                DeviceType2E type2 = (DeviceType2E)result.p6;
                string middlenum = "A";
                uint ferrynum = 100;
                TileWorkModeE workModeE = TileWorkModeE.上砖;
                switch (type)
                {
                    case DeviceTypeE.上砖机:
                        middlenum = "D";
                        ferrynum = 500;
                        break;
                    case DeviceTypeE.下砖机:
                        workModeE = TileWorkModeE.下砖;
                        break;
                    case DeviceTypeE.砖机:
                        workModeE = TileWorkModeE.过砖;
                        middlenum = "E";
                        break;
                }
                string lastnum = SelectLine.LineType == LineTypeE.要倒库 ? "1" : "5";
                Track lefttrack = null;
                Track righttrack = null;
                long ip = IpToInt(startip);
                for (int i = 0; i < count; i++, startnum++)
                {
                    Device d = new Device()
                    {
                        ip = IntToIp(ip),
                        name = SelectLine.line + middlenum + lastnum + startnum,
                        port = 2000,
                        Type = type,
                        Type2 = type2,
                        enable = true,
                        memo = Convert.ToInt32(middlenum + startnum + "", 16).ToString(),
                        area = (ushort)SelectArea.id,
                        line = (ushort)SelectLine.line,
                        do_work = false,
                    };

                    switch (type2)
                    {
                        case DeviceType2E.单轨:
                            lefttrack = PubMaster.Track.GetTrackByFerryCode(startnum + ferrynum);
                            break;
                        case DeviceType2E.双轨:
                            lefttrack = PubMaster.Track.GetTrackByFerryCode(startnum * 2 - 1 + ferrynum);
                            righttrack = PubMaster.Track.GetTrackByFerryCode(startnum * 2 + ferrynum);
                            break;
                    }

                    ConfigTileLifter conf = new ConfigTileLifter()
                    {
                        left_track_id = lefttrack?.id ?? 0,
                        left_track_point = lefttrack?.rfid_1 ?? 0,
                        right_track_id = righttrack?.id ?? 0,
                        right_track_point = righttrack?.rfid_1 ?? 0,
                        WorkMode = workModeE,
                    };

                    TileLifterSettingView t = new TileLifterSettingView(d, conf);
                    bool suc = SaveTileSingle(t, out string msg);
                    if (!suc)
                    {
                        Growl.Warning(msg);
                    }
                    ip = ip + 5;
                }
                RefreshTileList();
            }
        }

        public bool SaveTile(bool isBatch)
        {
            bool success = true;
            string msg = "";
            if (!isBatch)
            {
                if (!CheckAreaAndLine())
                {
                    return false;
                }
                success = SaveTileSingle(SelectTileLifter, out msg);
            }
            else
            {
                foreach (TileLifterSettingView t in TileLifterSettingViewList)
                {
                    success = SaveTileSingle(t, out msg);
                }
                if (_deletetilelist.Count > 0)
                {
                    foreach (uint i in _deletetilelist)
                    {
                        TileLifterSettingView temp = _currenttilelist.Find(c => c.id == i);
                        DeleteTile(temp);
                    }
                }
            }
            RefreshTileList();
            if (success)
            {
                Growl.Success("保存砖机成功！");
            }
            else
            {
                Growl.Warning(msg);
            }
            return success;
        }

        public bool SaveTileSingle(TileLifterSettingView tile, out string msg)
        {
            bool success = false;
            msg = "";

            if (tile.Type != DeviceTypeE.上砖机 && tile.Type != DeviceTypeE.下砖机 && tile.Type != DeviceTypeE.砖机)
            {
                msg = "请选择砖机类型";
                return false;
            }
            //新增
            if (tile.id == 0)
            {
                tile.area = (ushort)SelectArea.id;
                tile.line = (ushort)SelectLine.line;
                if (_currenttilelist.Exists(c => c.area == tile.area && c.ip == tile.ip))
                {
                    Growl.Warning("同区域已有相同IP的砖机！");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(tile.ip))
                {
                    Growl.Warning("砖机的IP不能为空！");
                    return false;
                }
                //转换成Device保存
                Device dev = tile.TransformIntoDevice();
                success = PubMaster.Device.AddDevice(dev, out uint newid);
                tile.id = newid;

                //转换成Config保存
                ConfigTileLifter config = tile.TransformIntoConfigTileLifter();
                success = PubMaster.DevConfig.AddConfigTile(config);

                //添加进area_device表里
                success = PubMaster.Area.AddAreaDevice(tile.area, tile.id, out string rs);

            }
            //编辑
            else
            {
                //转换成Device保存
                Device dev = tile.TransformIntoDevice();
                success = PubMaster.Device.UpdateDevice(dev);

                //转换成Config保存
                ConfigTileLifter config = tile.TransformIntoConfigTileLifter();
                success = PubMaster.DevConfig.UpdateConfigTile(config);
            }
            return success;
        }

        public bool DeleteTile(TileLifterSettingView del)
        {
            bool success = false;
            //删除config表
            success = PubMaster.DevConfig.DeleteConfigTile(del.TransformIntoConfigTileLifter());
            //删除area_deice表
            success = PubMaster.Area.DeleteAreaDevice(del.area, del.id);
            //删除device表
            success = PubMaster.Device.DeleteDevice(del.TransformIntoDevice());
            return success;
        }

        /// <summary>
        /// 砖机选择
        /// </summary>
        /// <param name="tag"></param>
        private async void DeviceSelected(string tag)
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                Growl.Warning("请先选择区域!");
                return;
            }

            if (SelectLine == null || SelectLine.id == 0)
            {
                Growl.Warning("请先选择线路!");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                .Initialize<DeviceSelectViewModel>((vm) =>
                {
                    vm.AreaId = SelectArea.id;
                    vm.LineId = SelectLine.line;
                    vm.FilterArea = true;
                    vm.SetSelectTypeExceptSelf(SelectTileLifter.id, SelectTileLifter.Type);
                }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                TileLifterSettingView t = SelectTileLifter;
                t.brother_dev_id = dev.id;
                int num = TileLifterSettingViewList.IndexOf(SelectTileLifter);
                TileLifterSettingViewList.Remove(SelectTileLifter);
                TileLifterSettingViewList.Insert(num, t);
            }
            else if (result.p1 is bool rs2 && !rs2 && result.p2 is null)
            {
                TileLifterSettingView t = SelectTileLifter;
                t.brother_dev_id = 0;
                int num = TileLifterSettingViewList.IndexOf(SelectTileLifter);
                TileLifterSettingViewList.Remove(SelectTileLifter);
                TileLifterSettingViewList.Insert(num, t);
            }
        }

        /// <summary>
        /// 多砖机选择
        /// </summary>
        /// <param name="tag"></param>
        private async void DeviceCheckSelected(string tag)
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                Growl.Warning("请先选择区域!");
                return;
            }

            if (SelectLine == null || SelectLine.id == 0)
            {
                Growl.Warning("请先选择线路!");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceCheckComboSelectDialog>()
                .Initialize<DeviceCheckComboSelectViewModel>((vm) =>
                {
                    vm.SetTileList(SelectTileLifter.TransformIntoConfigTileLifter(), SelectTileLifter.area, SelectTileLifter.Type);
                }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && rs && result.p2 is List<uint> devids)
            {
                string ids = string.Join(",", devids);

                TileLifterSettingView t = SelectTileLifter;
                t.alter_ids = ids;
                t.GetAlterDevices();
                if (devids.Count > 0 && !t.can_alter)
                {
                    t.can_alter = true;
                }

                int num = TileLifterSettingViewList.IndexOf(SelectTileLifter);
                TileLifterSettingViewList.Remove(SelectTileLifter);
                TileLifterSettingViewList.Insert(num, t);
            }
            else if (result.p1 is bool rsfalse && !rsfalse && result.p2 is null)
            {
                TileLifterSettingView t = SelectTileLifter;
                t.alter_ids = "";
                t.GetAlterDevices();

                int num = TileLifterSettingViewList.IndexOf(SelectTileLifter);
                TileLifterSettingViewList.Remove(SelectTileLifter);
                TileLifterSettingViewList.Insert(num, t);
            }
        }


        /// <summary>
        /// 砖机选择轨道
        /// </summary>
        private async void TrackSelected(string tag)
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                 .Initialize<TrackSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(0, true);
                     if (tag == "Ferry")
                     {
                         switch (SelectFerry.Type)
                         {
                             case DeviceTypeE.上摆渡:
                                 vm.QueryTrack(TrackTypeE.摆渡车_出);
                                 break;
                             case DeviceTypeE.下摆渡:
                                 vm.QueryTrack(TrackTypeE.摆渡车_入);
                                 break;
                         }
                     }
                     else
                     {
                         switch (SelectTileLifter.Type)
                         {
                             case DeviceTypeE.上砖机:
                                 vm.QueryTrack(TrackTypeE.上砖轨道);
                                 break;
                             case DeviceTypeE.下砖机:
                                 vm.QueryTrack(TrackTypeE.下砖轨道);
                                 break;
                             case DeviceTypeE.砖机:
                                 vm.QueryTrack(TrackTypeE.上砖轨道, TrackTypeE.下砖轨道);
                                 break;
                         }
                     }
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                TileLifterSettingView t = SelectTileLifter;
                switch (tag)
                {
                    case "left":
                        t.left_track_id = tra.id;
                        t.left_track_point = tra.rfid_1;
                        break;
                    case "right":
                        t.right_track_id = tra.id;
                        t.right_track_point = tra.rfid_1;
                        break;
                }
                int num = TileLifterSettingViewList.IndexOf(SelectTileLifter);
                TileLifterSettingViewList.Remove(SelectTileLifter);
                TileLifterSettingViewList.Insert(num, t);
            }
            else if (result.p1 is null)
            {
                TileLifterSettingView t = SelectTileLifter;
                switch (tag)
                {
                    case "left":
                        t.left_track_id = 0;
                        t.left_track_point = 0;
                        break;
                    case "right":
                        t.right_track_id = 0;
                        t.right_track_point = 0;
                        break;
                }
                int num = TileLifterSettingViewList.IndexOf(SelectTileLifter);
                TileLifterSettingViewList.Remove(SelectTileLifter);
                TileLifterSettingViewList.Insert(num, t);
            }
        }
        #endregion

        #region[运输车]


        private async void BatchAddCarrier()
        {
            if (!CheckAreaAndLine())
            {
                return;
            }
            DialogResult result = await HandyControl.Controls.Dialog.Show<BatchAddTileDialog>()
                              .Initialize<BatchAddTileViewModel>((vm) =>
                              {
                                  vm.SetAddType("Carrier");
                              }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && rs)
            {
                uint startnum = (uint)result.p2;
                string startip = (string)result.p3;
                uint count = (uint)result.p4;
                uint length = (uint)result.p5;

                long ip = IpToInt(startip);
                string lastnum = SelectLine.LineType == LineTypeE.要倒库 ? "1" : "5";
                for (int i = 0; i < count; i++,startnum++)
                {
                    Device d = new Device()
                    {
                        ip = IntToIp(ip),
                        name = SelectLine.line + "C" + lastnum + startnum,
                        port = 2000,
                        Type = DeviceTypeE.运输车,
                        Type2 = DeviceType2E.无,
                        enable = true,
                        memo = Convert.ToInt32("C" + startnum, 16).ToString(),
                        area = (ushort)SelectArea.id,
                        line = (ushort)SelectLine.line,
                        do_work = false,
                    };

                    ConfigCarrier conf = new ConfigCarrier()
                    {
                        length = (ushort)length,
                    };

                    CarrierSettingView c = new CarrierSettingView(d, conf);
                    SaveCarrierSingle(c);
                    ip++;
                }
                RefreshCarrierList();
            }
        }


        public void RefreshCarrierList()
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                return;
            }
            List<Device> devs = PubMaster.Device.GetCarriers(SelectArea.id);
            if (SelectLine != null)
            {
                devs = PubMaster.Device.GetDevices(SelectArea.id, SelectLine.line, DeviceTypeE.运输车);
            }
            List<uint> devids = devs.Select(c => c.id).ToList();
            List<ConfigCarrier> contiles = PubMaster.DevConfig.GetCarrierList(devids);

            CarrierSettingViewList.Clear();
            _deletecarrierlist.Clear();
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

        public bool SaveCarrier(bool isBatch)
        {
            bool success = true;
            if (!isBatch)
            {
                if (!CheckAreaAndLine())
                {
                    return false;
                }
                success = SaveCarrierSingle(SelectCarrier);
            }
            else
            {
                foreach (CarrierSettingView t in CarrierSettingViewList)
                {
                    SaveCarrierSingle(t);
                }
                if (_deletecarrierlist.Count > 0)
                {
                    foreach (uint i in _deletecarrierlist)
                    {
                        CarrierSettingView temp = _currentcarrierlist.Find(c => c.id == i);
                        DeleteCarrier(temp);
                    }
                }
            }
            RefreshTileList();
            if (success)
            {
                Growl.Success("保存运输车成功！");
            }
            return success;
        }

        public bool SaveCarrierSingle(CarrierSettingView carrier)
        {
            bool success = false;

            //新增
            if (carrier.id == 0)
            {
                carrier.area = (ushort)SelectArea.id;
                carrier.line = (ushort)SelectLine.line;
                carrier.Type = DeviceTypeE.运输车;
                if (_currentcarrierlist.Exists(c => c.area == carrier.area && c.ip == carrier.ip))
                {
                    Growl.Warning("同区域已有相同IP的运输车！");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(carrier.ip))
                {
                    Growl.Warning("运输车的IP不能为空！");
                    return false;
                }
                //转换成Device保存
                Device dev = carrier.TransformIntoDevice();
                success = PubMaster.Device.AddDevice(dev, out uint newid);
                carrier.id = newid;

                //转换成Config保存
                ConfigCarrier config = carrier.TransformIntoConfigCarrier();
                success = PubMaster.DevConfig.AddConfigCarrier(config);

                //添加进area_device表里
                success = PubMaster.Area.AddAreaDevice(carrier.area, carrier.id, out string rs);

            }
            //编辑
            else
            {
                //转换成Device保存
                Device dev = carrier.TransformIntoDevice();
                success = PubMaster.Device.UpdateDevice(dev);

                //转换成Config保存
                ConfigCarrier config = carrier.TransformIntoConfigCarrier();
                success = PubMaster.DevConfig.UpdateConfigCarrier(config);
            }
            return success;
        }

        public bool DeleteCarrier(CarrierSettingView del)
        {
            bool success = false;
            //删除config表
            success = PubMaster.DevConfig.DeleteConfigCarrier(del.TransformIntoConfigCarrier());
            //删除area_deice表
            success = PubMaster.Area.DeleteAreaDevice(del.area, del.id);
            //删除device表
            success = PubMaster.Device.DeleteDevice(del.TransformIntoDevice());
            return success;
        }

        public async void GoodSizeSelected()
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                Growl.Warning("请先选择区域!");
                return;
            }

            if (SelectLine == null || SelectLine.id == 0)
            {
                Growl.Warning("请先选择线路!");
                return;
            }

            DialogResult result = await HandyControl.Controls.Dialog.Show<GoodSizeSelectDialog>()
                .Initialize<GoodSizeSelectViewModel>((vm) =>
                {
                    vm.QuerySize(SelectCarrier.goods_size);
                    vm.SetIsBatch(true);
                }).GetResultAsync<DialogResult>();

            if (result.p2 is List<string> goods_size_ids)
            {
                string ids = string.Join("#", goods_size_ids);

                CarrierSettingView c = SelectCarrier;
                c.goods_size = ids;
                c.GetSizeName();

                int num = CarrierSettingViewList.IndexOf(SelectCarrier);
                CarrierSettingViewList.Remove(SelectCarrier);
                CarrierSettingViewList.Insert(num, c);
            }
            //else
            //{
            //    CarrierSettingView c = SelectCarrier;
            //    c.goods_size = null;
            //    c.GetSizeName();

            //    int num = CarrierSettingViewList.IndexOf(SelectCarrier);
            //    CarrierSettingViewList.Remove(SelectCarrier);
            //    CarrierSettingViewList.Insert(num, c);
            //}
        }

        #endregion

        #region[摆渡车]


        private async void BatchAddFerry()
        {
            if (!CheckAreaAndLine())
            {
                return;
            }
            DialogResult result = await HandyControl.Controls.Dialog.Show<BatchAddTileDialog>()
                              .Initialize<BatchAddTileViewModel>((vm) =>
                              {
                                  vm.SetAddType("Ferry");
                              }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && rs)
            {
                uint startnum = (uint)result.p2;
                string startip = (string)result.p3;
                uint count = (uint)result.p4;
                DeviceTypeE type = (DeviceTypeE)result.p5;

                long ip = IpToInt(startip);
                string lastnum = SelectLine.LineType == LineTypeE.要倒库 ? "1" : "5";
                uint ferrynum = (uint)(type == DeviceTypeE.上摆渡 ? 400 : 200);
                for (int i = 0; i < count; i++, startnum++)
                {
                    Device d = new Device()
                    {
                        ip = IntToIp(ip),
                        name = SelectLine.line + "B" + lastnum + startnum,
                        port = 2000,
                        Type = type,
                        Type2 = DeviceType2E.无,
                        enable = true,
                        memo = Convert.ToInt32("B" + startnum, 16).ToString(),
                        area = (ushort)SelectArea.id,
                        line = (ushort)SelectLine.line,
                        do_work = false,
                    };

                    ConfigFerry conf = new ConfigFerry();
                    Track ferrytrack = PubMaster.Track.GetTrackByFerryCode(ferrynum + startnum);
                    if (type == DeviceTypeE.上摆渡)
                    {
                        ferrytrack = PubMaster.Track.GetTrackByFerryCode(ferrynum + startnum - 4);
                    }
                    if (ferrytrack != null)
                    {
                        conf.track_id = ferrytrack.id;
                        conf.track_point = ferrytrack.rfid_1;
                    }

                    FerrySettingView f = new FerrySettingView(d, conf);
                    SaveFerrySingle(f, out string msg);
                    ip++;
                }
                RefreshFerryList();
            }
        }

        public void RefreshFerryList()
        {
            if (SelectArea == null || SelectArea.id == 0)
            {
                return;
            }
            List<Device> devs = PubMaster.Device.GetFerrys(SelectArea.id);
            if (SelectLine != null)
            {
                devs = PubMaster.Device.GetDevices(SelectArea.id, SelectLine.line, DeviceTypeE.上摆渡, DeviceTypeE.下摆渡);
            }
            List<uint> devids = devs.Select(c => c.id).ToList();
            List<ConfigFerry> contiles = PubMaster.DevConfig.GetFerryList(devids);

            FerrySettingViewList.Clear();
            _deleteferrylist.Clear();
            _currentferrylist.Clear();
            for (int i = 0; i < devs.Count; i++)
            {
                Device dev = devs[i];
                ConfigFerry config = contiles[i];
                FerrySettingView t = new FerrySettingView(dev, config);
                FerrySettingViewList.Add(t);
                _currentferrylist.Add(t);
            }
        }

        public bool SaveFerry(bool isBatch)
        {
            bool success = true;
            string msg = "";
            if (!isBatch)
            {
                if (!CheckAreaAndLine())
                {
                    return false;
                }
                success = SaveFerrySingle(SelectFerry, out msg);
            }
            else
            {
                foreach (FerrySettingView t in FerrySettingViewList)
                {
                    success = SaveFerrySingle(t, out msg);
                }
                if (_deleteferrylist.Count > 0)
                {
                    foreach (uint i in _deleteferrylist)
                    {
                        FerrySettingView temp = _currentferrylist.Find(c => c.id == i);
                        DeleteFerry(temp);
                    }
                }
            }
            RefreshFerryList();
            if (success)
            {
                Growl.Success("保存摆渡车成功！");
            }
            else
            {
                Growl.Warning(msg);
            }
            return success;
        }

        public bool SaveFerrySingle(FerrySettingView ferry, out string msg)
        {
            bool success = false;
            msg = "";

            if (ferry.Type != DeviceTypeE.上摆渡 && ferry.Type != DeviceTypeE.下摆渡)
            {
                msg = "请选择摆渡车的类型";
                return false;
            }

            //新增
            if (ferry.id == 0)
            {
                ferry.area = (ushort)SelectArea.id;
                ferry.line = (ushort)SelectLine.line;
                if (_currentferrylist.Exists(c => c.area == ferry.area && c.ip == ferry.ip))
                {
                    Growl.Warning("同区域已有相同IP的摆渡车！");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(ferry.ip))
                {
                    Growl.Warning("摆渡车的IP不能为空！");
                    return false;
                }
                //转换成Device保存
                Device dev = ferry.TransformIntoDevice();
                success = PubMaster.Device.AddDevice(dev, out uint newid);
                ferry.id = newid;

                //转换成Config保存
                ConfigFerry config = ferry.TransformIntoConfigFerry();
                success = PubMaster.DevConfig.AddConfigFerry(config);

                //添加进area_device表里
                success = PubMaster.Area.AddAreaDevice(ferry.area, ferry.id, out string rs);

            }
            //编辑
            else
            {
                //转换成Device保存
                Device dev = ferry.TransformIntoDevice();
                success = PubMaster.Device.UpdateDevice(dev);

                //转换成Config保存
                ConfigFerry config = ferry.TransformIntoConfigFerry();
                success = PubMaster.DevConfig.UpdateConfigFerry(config);
            }
            return success;
        }

        public bool DeleteFerry(FerrySettingView del)
        {
            bool success = false;
            //删除config表
            success = PubMaster.DevConfig.DeleteConfigFerry(del.TransformIntoConfigFerry());
            //删除area_deice表
            success = PubMaster.Area.DeleteAreaDevice(del.area, del.id);
            //删除device表
            success = PubMaster.Device.DeleteDevice(del.TransformIntoDevice());
            return success;
        }

        /// <summary>
        /// 摆渡车选择轨道
        /// </summary>
        public async void FerryTrackSelected()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                 .Initialize<TrackSelectViewModel>((vm) =>
                 {
                     vm.SetAreaFilter(0, true);
                     switch (SelectFerry.Type)
                     {
                         case DeviceTypeE.上摆渡:
                             vm.QueryTrack(TrackTypeE.摆渡车_出);
                             break;
                         case DeviceTypeE.下摆渡:
                             vm.QueryTrack(TrackTypeE.摆渡车_入);
                             break;
                     }
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is Track tra)
            {
                FerrySettingView f = SelectFerry;
                f.track_id = tra.id;
                f.track_point = tra.rfid_1;
                int num = FerrySettingViewList.IndexOf(SelectFerry);
                FerrySettingViewList.Remove(SelectFerry);
                FerrySettingViewList.Insert(num, f);
            }
            else if (result.p1 is null)
            {
                FerrySettingView f = SelectFerry;
                f.track_id = 0;
                f.track_point = 0;
                int num = FerrySettingViewList.IndexOf(SelectFerry);
                FerrySettingViewList.Remove(SelectFerry);
                FerrySettingViewList.Insert(num, f);
            }
        }

        #endregion

        #endregion

    }
}
