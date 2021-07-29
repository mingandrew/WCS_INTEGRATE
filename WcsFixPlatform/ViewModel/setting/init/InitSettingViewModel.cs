using enums;
using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.area;
using module.device;
using module.line;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            _linelist = new ObservableCollection<Line>();
            _devicemenulist = new List<SettingMenuView>();
            _devicemenulist.Add(new SettingMenuView()
            {
                id = 1,
                name = "轨道",
                level = 3,
            });
            _devicemenulist.Add(new SettingMenuView()
            {
                id = 2,
                name = "运输车",
                level = 3,
            });
            _devicemenulist.Add(new SettingMenuView()
            {
                id = 3,
                name = "摆渡车",
                level = 3,
            });
            _devicemenulist.Add(new SettingMenuView()
            {
                id = 4,
                name = "砖机",
                level = 3,
            });

            _selectmenuview = new SettingMenuView();


            Refresh();

            ShowArea = false;
        }


        #region[字段]

        #region[菜单]

        private ObservableCollection<SettingMenuView> _areamenulist;
        private ObservableCollection<Area> _arealist;
        private ObservableCollection<Line> _linelist;
        private List<SettingMenuView> _devicemenulist;

        private SettingMenuView _selectmenuview;

        public bool showarea;
        public bool showline;
        public bool showtrack;
        public bool showcarrier;
        public bool showferry;
        public bool showtilelifter;
        public List<Area> areas;
        public List<Line> lines;
        #endregion

        #region[区域]
        private string area_name;
        private uint up_car;
        private uint down_car;

        #endregion

        #region[线]

        private string line_name;
        private ushort line_num;
        private ushort sort_count;
        private ushort up_count;
        private ushort down_count;
        private LineTypeE line_type;
        private byte full_qty;

        #endregion

        #endregion

        #region[属性]

        public string AREAACTIONNAME { set; get; }
        public bool IsAdd { set; get; }

        #region[菜单]
        public ObservableCollection<SettingMenuView> SettingMenuList
        {
            get => _areamenulist;
            set => Set(ref _areamenulist, value);
        }

        public ObservableCollection<Area> AreaList
        {
            get => _arealist;
            set => Set(ref _arealist, value);
        }

        public ObservableCollection<Line> LineList
        {
            get => _linelist;
            set => Set(ref _linelist, value);
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


        public SettingMenuView SelectMenuView
        {
            get => _selectmenuview;
            set => Set(ref _selectmenuview, value);
        }

        #endregion

        #region[区域]
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

        #endregion

        #region[命令]

        public RelayCommand<string> SettingBtnCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SettingBtnFunction)).Value;
        public RelayCommand<RoutedEventArgs> MenuTreeViewChangeCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(MenuTreeViewChange)).Value;
        public RelayCommand<string> ComfirmCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(ComfirmFunction)).Value;
        public RelayCommand<string> CancelCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(CancelFunction)).Value;

        #endregion

        #region[方法]
        /// <summary>
        /// 初始化配置菜单
        /// </summary>
        private void Refresh()
        {
            SettingMenuList.Clear();
            //获取全部的区域和线的信息
            areas = PubMaster.Area.GetAreaList();
            lines = PubMaster.Area.GetLineList();
            AreaList.Clear();
            LineList.Clear();
            foreach (var aa in areas)
            {
                AreaList.Add(aa);
            }
            foreach (var ll in lines)
            {
                LineList.Add(ll);
            }

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

                //获取对应区域的线路信息
                List<Line> ls = lines.FindAll(c => c.area_id == a.id);
                if (ls == null || ls.Count == 0)
                {
                    SettingMenuList.Add(s);
                    continue;
                }

                //添加每一条线路的信息进区域的childlist
                foreach (Line l in ls)
                {
                    _devicemenulist.ForEach(c => c.p_id = l.line);
                    SettingMenuView temp = new SettingMenuView()
                    {
                        id = l.line,
                        name = l.name,
                        p_id = a.id,
                        level = 2,
                        childlist = _devicemenulist,
                    };
                    s.childlist.Add(temp);
                }

                SettingMenuList.Add(s);
            }

        }

        /// <summary>
        /// 切换，选择树节点
        /// </summary>
        /// <param name="orgs"></param>
        private void MenuTreeViewChange(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is TreeView pro && pro.SelectedItem is SettingMenuView menu)
            {
                SelectMenuView = menu;
            }
        }

        /// <summary>
        /// 菜单功能
        /// </summary>
        /// <param name="tag"></param>
        private void SettingBtnFunction(string tag)
        {
            switch (tag)
            {
                case "AddArea":
                    AREAACTIONNAME = "新增";
                    ShowTab("Area");
                    IsAdd = true;
                    AreaName = "";
                    UpCar = 0;
                    DownCar = 0;
                    break;
                case "EditArea":
                    if (SelectMenuView != null && SelectMenuView.level == 1)
                    {
                        IsAdd = false;
                        SetAreaView(SelectMenuView.id);
                        AREAACTIONNAME = "编辑";
                        ShowTab("Area");
                        break;
                    }
                    Growl.Warning("请先选择区域!");
                    break;
                case "AddLine":
                    if (SelectMenuView == null)
                    {
                        Growl.Warning("请先选择区域,再新增线路!");
                        return;
                    }
                    AREAACTIONNAME = "新增";
                    ShowTab("Line");
                    IsAdd = true;
                    LineName = "";
                    LineNum = 0;
                    SortCount = 0;
                    UpCount = 0;
                    DownCount = 0;
                    LineType = 0;
                    FullQty = 0;
                    break;
                case "EditLine":
                    ShowLine = false;
                    if (SelectMenuView != null && SelectMenuView.level == 2)
                    {
                        IsAdd = false;
                        SetLineView(SelectMenuView.p_id, SelectMenuView.id);
                        AREAACTIONNAME = "编辑";
                        ShowTab("Line");
                        break;
                    }
                    Growl.Warning("请先选择线路!");
                    break;
                default:
                    ShowArea = false;
                    break;
            }
        }

        /// <summary>
        /// 展示一个Tab页
        /// </summary>
        /// <param name="tag"></param>
        public void ShowTab(string tag)
        {
            ShowArea = false;
            ShowLine = false;
            ShowTrack = false;
            ShowCarrier = false;
            ShowFerry = false;
            ShowTilelifter = false;
            switch (tag)
            {
                case "Area":
                    ShowArea = true;
                    break;
                case "Line":
                    ShowLine = true;
                    break;
                case "Track":
                    ShowTrack = true;
                    break;
                case "Carrier":
                    ShowCarrier = true;
                    break;
                case "Ferry":
                    ShowFerry = true;
                    break;
                case "Tile":
                    ShowTilelifter = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="tag"></param>
        private void ComfirmFunction(string tag)
        {
            switch (tag)
            {
                case "ComfirmArea":
                    if (IsAdd)
                    {
                        Area a = new Area()
                        {
                            name = AreaName,
                            up_car_count = UpCar,
                            down_car_count = DownCar,
                            enable = true,
                            devautorun = true,
                            memo = AreaName,
                        };
                        PubMaster.Mod.AreaSql.AddArea(a);
                        PubMaster.Area.Refresh(true, false, false, false, false);
                        Refresh();
                    }
                    else
                    {
                        Area aa = PubMaster.Area.GetArea(SelectMenuView.id);
                        if (aa == null)
                        {
                            Growl.Warning("请先选择区域，再执行编辑保存");
                        }
                        aa.name = AreaName;
                        aa.up_car_count = UpCar;
                        aa.down_car_count = DownCar;
                        PubMaster.Mod.AreaSql.EditArea(aa);
                        Refresh();
                    }
                    break;
                case "ComfirmLine":
                    if (IsAdd)
                    {
                        Line l = new Line()
                        {
                            area_id = SelectMenuView.id,
                            line = LineNum,
                            name = LineName,
                            sort_task_qty = SortCount,
                            up_task_qty = UpCount,
                            down_task_qty = DownCount,
                            line_type = (byte)LineType,
                            full_qty = FullQty,
                        };
                        PubMaster.Mod.AreaSql.AddLine(l);
                        PubMaster.Area.Refresh(false, false, false, false, true);
                        Refresh();
                    }
                    else
                    {

                        Line ll = PubMaster.Area.GetLine(SelectMenuView.p_id, (ushort)SelectMenuView.id);
                        if (ll == null)
                        {
                            Growl.Warning("请先选择线路，再执行编辑保存");
                        }
                        ll.line = LineNum;
                        ll.name = LineName;
                        ll.sort_task_qty = SortCount;
                        ll.up_task_qty = UpCount;
                        ll.down_task_qty = DownCount;
                        ll.line_type = (byte)LineType;
                        ll.full_qty = FullQty;
                        PubMaster.Mod.AreaSql.EditAreaLine(ll);
                        Refresh();
                    }
                    break;
                default:
                    break;
            }
        }

        private void CancelFunction(string obj)
        {
            ShowTab("");
        }


        /// <summary>
        /// 设置区域信息
        /// </summary>
        /// <param name="id"></param>
        public void SetAreaView(uint id)
        {
            Area area = PubMaster.Area.GetArea(id);
            if (area == null)
            {
                Growl.Warning("区域数据错误，请重新启动调度系统!");
                return;
            }
            AreaName = area.name;
            UpCar = area.up_car_count;
            DownCar = area.down_car_count;
        }

        /// <summary>
        /// 设置线信息
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="lineid"></param>
        public void SetLineView(uint areaid, uint lineid)
        {
            Line line = PubMaster.Area.GetLine(areaid, (ushort)lineid);
            LineName = line.name;
            LineNum = line.line;
            SortCount = line.sort_task_qty;
            UpCount = line.up_task_qty;
            DownCount = line.down_task_qty;
            LineType = line.LineType;
            FullQty = line.full_qty;
        }

        /// <summary>
        /// 获取区域id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        public uint GetAreaId(SettingMenuView setting)
        {
            switch (setting.level)
            {
                case 1:
                    return setting.id;
                case 2:
                    return setting.p_id;
                case 3:
                    Line l = lines.Find(c => c.line == setting.p_id);
                    return l.area_id;
                default:
                    break;
            }
            return 0;
        }
        #endregion
        
    }
}
