using enums;
using enums.warning;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using module.diction;
using resource;
using System.Windows;

namespace wcs.ViewModel
{
    public class AreaSwitchViewModel : ViewModelBase
    {
        public AreaSwitchViewModel()
        {
            _up_1 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area1Up);
            
            _down_1 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area1Down);
            _sort_1 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area1Sort, out bool sort_1);
            HAVE_SORT_1 = sort_1;

            _up_2 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area2Up);
            _down_2 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area2Down);
            _sort_2 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area2Sort, out bool sort_2);
            HAVE_SORT_2 = sort_2;

            _up_3 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area3Up);
            _down_3 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area3Down);
            _sort_3 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area3Sort, out bool sort_3);
            HAVE_SORT_3 = sort_3;

            _up_4 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area4Up);
            _down_4 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area4Down);
            _sort_4 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area4Sort, out bool sort_4);
            HAVE_SORT_4 = sort_4;

            _up_5 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area5Up);
            _down_5 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area5Down);
            _sort_5 = PubMaster.Dic.IsSwitchOnOff(DicSwitchTag.Area5Sort, out bool sort_5);
            HAVE_SORT_5 = sort_5;

            Messenger.Default.Register<DictionDtl>(this, MsgToken.TaskSwitchUpdate, TaskSwitchUpdate);

            CheckIsSingle();

            Area1Name = PubMaster.Area.GetName(1);
            Area2Name = PubMaster.Area.GetName(2);
            Area3Name = PubMaster.Area.GetName(3);
            Area4Name = PubMaster.Area.GetName(4);
            Area5Name = PubMaster.Area.GetName(5);
        }

        #region[字段]
        private bool _up_1, _down_1, _sort_1;
        private bool _up_2, _down_2, _sort_2;
        private bool _up_3, _down_3, _sort_3;
        private bool _up_4, _down_4, _sort_4;
        private bool _up_5, _down_5, _sort_5;

        public bool HAVE_SORT_1 { set; get; }
        public bool HAVE_SORT_2 { set; get; }
        public bool HAVE_SORT_3 { set; get; }
        public bool HAVE_SORT_4 { set; get; }
        public bool HAVE_SORT_5 { set; get; }
        private bool show2area, show3area, show4area, show5area;

        private string area1name, area2name, area3name, area4name, area5name;
        #endregion

        #region[属性]

        public string Area1Name
        {
            get => area1name;
            set => Set(ref area1name, value);
        }
        public string Area2Name
        {
            get => area2name;
            set => Set(ref area2name, value);
        }
        public string Area3Name
        {
            get => area3name;
            set => Set(ref area3name, value);
        }

        public string Area4Name
        {
            get => area4name;
            set => Set(ref area4name, value);
        }

        public string Area5Name
        {
            get => area5name;
            set => Set(ref area5name, value);
        }

        public bool Show2Area
        {
            get => show2area;
            set => Set(ref show2area, value);
        }

        public bool Show3Area
        {
            get => show3area;
            set => Set(ref show3area, value);
        }

        public bool Show4Area
        {
            get => show4area;
            set => Set(ref show4area, value);
        }

        public bool Show5Area
        {
            get => show5area;
            set => Set(ref show5area, value);
        }

        public bool Up_1
        {
            get => _up_1;
            set
            {
                if (Set(ref _up_1, value))
                {
                    ChangeSwitch(DicSwitchTag.Area1Up, value);
                }
            }
        }
        public bool Down_1
        {
            get => _down_1;
            set
            {
                if (Set(ref _down_1, value))
                {
                    ChangeSwitch(DicSwitchTag.Area1Down, value);
                }
            }
        }
        public bool Sort_1
        {
            get => _sort_1;
            set
            {
                if (Set(ref _sort_1, value))
                {
                    ChangeSwitch(DicSwitchTag.Area1Sort, value);
                }
            }
        }
        public bool Up_2
        {
            get => _up_2;
            set
            {
                if (Set(ref _up_2, value))
                {
                    ChangeSwitch(DicSwitchTag.Area2Up, value);
                }
            }
        }
        public bool Down_2
        {
            get => _down_2;
            set
            {
                if (Set(ref _down_2, value))
                {
                    ChangeSwitch(DicSwitchTag.Area2Down, value);
                }
            }
        }
        public bool Sort_2
        {
            get => _sort_2;
            set
            {
                if (Set(ref _sort_2, value))
                {
                    ChangeSwitch(DicSwitchTag.Area2Sort, value);
                }
            }
        }
        public bool Up_3
        {
            get => _up_3;
            set
            {
                if (Set(ref _up_3, value))
                {
                    ChangeSwitch(DicSwitchTag.Area3Up, value);
                }
            }
        }
        public bool Down_3
        {
            get => _down_3;
            set
            {
                if (Set(ref _down_3, value))
                {
                    ChangeSwitch(DicSwitchTag.Area3Down, value);
                }
            }
        }
        public bool Sort_3
        {
            get => _sort_3;
            set
            {
                if (Set(ref _sort_3, value))
                {
                    ChangeSwitch(DicSwitchTag.Area3Sort, value);
                }
            }
        }
        public bool Up_4
        {
            get => _up_4;
            set
            {
                if (Set(ref _up_4, value))
                {
                    ChangeSwitch(DicSwitchTag.Area4Up, value);
                }
            }
        }
        public bool Down_4
        {
            get => _down_4;
            set
            {
                if (Set(ref _down_4, value))
                {
                    ChangeSwitch(DicSwitchTag.Area4Down, value);
                }
            }
        }
        public bool Sort_4
        {
            get => _sort_4;
            set
            {
                if (Set(ref _sort_4, value))
                {
                    ChangeSwitch(DicSwitchTag.Area4Sort, value);
                }
            }
        }
        public bool Up_5
        {
            get => _up_5;
            set
            {
                if (Set(ref _up_5, value))
                {
                    ChangeSwitch(DicSwitchTag.Area5Up, value);
                }
            }
        }
        public bool Down_5
        {
            get => _down_5;
            set
            {
                if (Set(ref _down_5, value))
                {
                    ChangeSwitch(DicSwitchTag.Area5Down, value);
                }
            }
        }
        public bool Sort_5
        {
            get => _sort_5;
            set
            {
                if (Set(ref _sort_5, value))
                {
                    ChangeSwitch(DicSwitchTag.Area5Sort, value);
                }
            }
        }


        #endregion

        #region[命令]

        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint areaid))
            {
                Show2Area = false;
                Show3Area = false;
                Show4Area = false;
                Show5Area = false;
            }
            else
            {
                Show2Area = PubMaster.Area.HaveArea(2);
                Show3Area = PubMaster.Area.HaveArea(3);
                Show4Area = PubMaster.Area.HaveArea(4);
                Show5Area = PubMaster.Area.HaveArea(5);
            }
        }

        private void ChangeSwitch(string tag, bool onoff)
        {
            PubMaster.Dic.UpdateSwitch(tag, onoff, false);

            #region[开关的报警]
            string areaid = System.Text.RegularExpressions.Regex.Replace(tag, @"[^0-9]+", "");
            bool f = uint.TryParse(areaid, out uint aid);

            if (aid != 0)
            {
                if (tag.Contains("Down"))
                {
                    if (onoff)
                    {
                        PubMaster.Warn.RemoveAreaWarn(WarningTypeE.DownTaskSwitchClosed, (ushort)aid);
                    }
                    else
                        PubMaster.Warn.AddAreaWarn(WarningTypeE.DownTaskSwitchClosed, (ushort)aid);
                }
                else if (tag.Contains("Up"))
                {
                    if (onoff)
                    {
                        PubMaster.Warn.RemoveAreaWarn(WarningTypeE.UpTaskSwitchClosed, (ushort)aid);
                    }
                    else
                        PubMaster.Warn.AddAreaWarn(WarningTypeE.UpTaskSwitchClosed, (ushort)aid);
                }
                else if (tag.Contains("Sort"))
                {
                    if (onoff)
                    {
                        PubMaster.Warn.RemoveAreaWarn(WarningTypeE.SortTaskSwitchClosed, (ushort)aid);
                    }
                    else
                        PubMaster.Warn.AddAreaWarn(WarningTypeE.SortTaskSwitchClosed, (ushort)aid);
                }
            }

            #endregion
        }

        private void TaskSwitchUpdate(DictionDtl dtl)
        {
            if (dtl != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (dtl.code)
                    {
                        case DicSwitchTag.Area1Down:
                            Down_1 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area1Sort:
                            Sort_1 = dtl.bool_value; 
                            break;
                        case DicSwitchTag.Area1Up:
                            Up_1 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area2Down:
                            Down_2 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area2Sort:
                            Sort_2 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area2Up:
                            Up_2 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area3Down:
                            Down_3 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area3Sort:
                            Sort_3 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area3Up:
                            Up_3 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area4Down:
                            Down_4 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area4Sort:
                            Sort_4 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area4Up:
                            Up_4 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area5Down:
                            Down_5 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area5Sort:
                            Sort_5 = dtl.bool_value;
                            break;
                        case DicSwitchTag.Area5Up:
                            Up_5 = dtl.bool_value;
                            break;
                    }
                });
            }
        }
        #endregion
    }
}
