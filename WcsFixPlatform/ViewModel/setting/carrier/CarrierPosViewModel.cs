using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using task;

namespace wcs.ViewModel
{
    public class CarrierPosViewModel : MViewModel
    {
        public CarrierPosViewModel() : base("CarrierPos")
        {
            List = new ObservableCollection<CarrierPos>();
            InitAreaRadio();
            CheckIsSingle();
        }
        #region[字段]

        private ObservableCollection<CarrierPos> _list;
        private CarrierPos _selectpos;
        private ushort _trackpoint, _trackpos;
        private IList<MyRadioBtn> _arearadio;
        private uint areaid;
        private bool showareafilter = true;
        private string btn_name = "修改";
        private bool _track_point_enable;
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

        public ObservableCollection<CarrierPos> List
        {
            get => _list;
            set => Set(ref _list, value);
        }
        public CarrierPos SelectPos
        {
            get => _selectpos;
            set => Set(ref _selectpos, value);
        }

        public ushort TrackPoint
        {
            get => _trackpoint;
            set => Set(ref _trackpoint, value);
        }

        public ushort TrackPos
        {
            get => _trackpos;
            set => Set(ref _trackpos, value);
        }

        public string BtnName
        {
            get => btn_name;
            set => Set(ref btn_name, value);
        }

        public bool TrackPointEnable
        {
            set => Set(ref _track_point_enable, value);
            get => _track_point_enable;
        }
        #endregion

        #region[命令]
        public RelayCommand PosSelectedChangeCmd => new Lazy<RelayCommand>(() => new RelayCommand(PosSelectedChange)).Value;
        public RelayCommand<string> BtnSelectCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(BtnSelect)).Value;
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;

        #endregion

        #region[方法]


        private void PosSelectedChange()
        {
            if (SelectPos != null)
            {
                TrackPoint = SelectPos.track_point;
                TrackPos = SelectPos.track_pos;
                BtnName = "修改";
                TrackPointEnable = false;
            }
        }

        private void BtnSelect(string tag)
        {
            switch (tag)
            {
                case "addpos":
                    if (areaid == 0)
                    {
                        Growl.Warning("请先选择区域！");
                        return;
                    }
                    BtnName = "添加";
                    TrackPointEnable = true;
                    ClearInput();
                    break;

                case "actionbtn":
                    if (areaid == 0)
                    {
                        Growl.Warning("请先选择区域！");
                        return;
                    }
                    if (TrackPoint <= 0)
                    {
                        Growl.Warning("请输入正确的复位地标！");
                        return;
                    }
                    if (TrackPos < 0)
                    {
                        Growl.Warning("请输入正确的复位脉冲！");
                        return;
                    }
                    CarrierPos pos = new CarrierPos
                    {
                        area_id = areaid,
                        track_point = TrackPoint,
                        track_pos = TrackPos
                    };

                    if (SelectPos == null)//添加
                    {
                        if (TrackPointEnable)
                        {
                            CarrierPos ishavepos = List.FirstOrDefault(c => c.track_point == TrackPoint);
                            if (ishavepos == null)
                            {
                                if (!PubMaster.Track.ExistSiteInTrack((ushort)areaid, TrackPoint))
                                {
                                    Growl.Warning("找不到配置该地标的轨道信息，请检查地标是否准确！");
                                    return;
                                }
                                PubMaster.Track.AddCarrierPos(pos);
                            }
                            else
                            {
                                Growl.Warning("已经存在对应的复位地标！");
                            }
                        }
                    }
                    else//修改
                    {
                        if (SelectPos == null)
                        {
                            Growl.Warning("请先选择数据！");
                            return;
                        }
                        pos.id = SelectPos.id;
                        PubMaster.Track.EditCarrierPos(pos);
                    }
                    QueryPointPosData();

                    break;

                case "update2allcar":
                    if (SelectPos == null)
                    {
                        Growl.Warning("请先选择复位信息！");
                        return;
                    }
                    PubTask.Carrier.DoAreaResetSite(areaid, SelectPos.track_point, SelectPos.track_pos);
                    Growl.Info("发送数据！");
                    break;

            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {

            if (args.OriginalSource is RadioButton btn)
            {
                string area = btn.Tag.ToString();

                if (uint.TryParse(area, out uint aid))
                {
                    areaid = aid;
                    QueryPointPosData();
                }
            }
        }

        private void QueryPointPosData()
        {
            List<CarrierPos> list = PubMaster.Track.GetCarrierPosList(areaid);
            List.Clear();
            foreach (var item in list)
            {
                List.Add(item);
            }
        }


        private void ClearInput()
        {
            SelectPos = null;
            TrackPoint = 0;
            TrackPos = 0;
        }
        #endregion

        #region[界面激活]
        protected override void TabActivate()
        {

        }

        protected override void TabDisActivate()
        {

        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaRadioList();
        }
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint aid))
            {
                ShowAreaFileter = false;
                areaid = aid;
                QueryPointPosData();
            }
        }
        #endregion
    }
}
