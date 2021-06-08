using enums;
using enums.track;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using tool.appconfig;

namespace wcs.ViewModel
{
    /// <summary>
    /// 更新轨道脉冲值
    /// </summary>
    public class TrackSetPointViewModel : MViewModel
    {
        public TrackSetPointViewModel() : base("TrackSetPoint")
        {
            InitAreaRadio();
            TabActivate();
            CheckIsSingle();
        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaLineRadioList(true);
        }

        #region[字段]
        private bool showareafilter = true;

        private IList<MyRadioBtn> _arearadio;
        private uint filterareaid = 0;
        private ushort filterlineid = 0;

        private bool isnotinouttrack;
        private ushort out_loc_point, out_sort_point, out_last_point, in_first_point, in_loc_point;

        private ushort set_in_loc_point, set_out_loc_point, set_in_first_point, set_out_last_point, set_out_sort_point, set_out_sort_qty, each_stack_point;
        private double point_to_cm, in_track_len, out_track_len, 
            inout_track_len, in_stack_qty, out_stack_qty, inout_stack_qty,
            middle_space_m, out_more_than_in;
        private GridLength onegridlen = new GridLength(1, GridUnitType.Pixel);
        private GridLength zerogridlen = new GridLength(0, GridUnitType.Pixel);
        private GridLength inoroutgridlen, inoutgridlen;

        #endregion

        #region[属性]
        public GridLength InOrOutGridLen
        {
            get => inoroutgridlen;
            set => Set(ref inoroutgridlen, value);
        }

        public GridLength InOutGridLen
        {
            get => inoutgridlen;
            set => Set(ref inoutgridlen, value);
        }

        public ushort Set_In_Loc_Point
        {
            get => set_in_loc_point;
            set => Set(ref set_in_loc_point, value);
        }
        public ushort Set_Out_Loc_Point
        {
            get => set_out_loc_point;
            set => Set(ref set_out_loc_point, value);
        }
        public ushort Set_In_First_Point
        {
            get => set_in_first_point;
            set => Set(ref set_in_first_point, value);
        }
        public ushort Set_Out_Last_Point
        {
            get => set_out_last_point;
            set => Set(ref set_out_last_point, value);
        }
        public ushort Set_Out_Sort_Point
        {
            get => set_out_sort_point;
            set => Set(ref set_out_sort_point, value);
        }
        public ushort Set_Out_Sort_Qty
        {
            get => set_out_sort_qty;
            set => Set(ref set_out_sort_qty, value);
        }
        public ushort Each_Stack_Point
        {
            get => each_stack_point;
            set => Set(ref each_stack_point, value);
        }
        public double Point_To_Cm
        {
            get => point_to_cm;
            set => Set(ref point_to_cm, value);
        }
        public double In_Track_Len
        {
            get => in_track_len;
            set => Set(ref in_track_len, value);
        }
        public double Out_Track_Len
        {
            get => out_track_len;
            set => Set(ref out_track_len, value);
        }
        public double Inout_Track_Len
        {
            get => inout_track_len;
            set => Set(ref inout_track_len, value);
        }
        public double In_Stack_Qty
        {
            get => in_stack_qty;
            set => Set(ref in_stack_qty, value);
        }
        public double Out_Stack_Qty
        {
            get => out_stack_qty;
            set => Set(ref out_stack_qty, value);
        }
        public double Inout_Stack_Qty
        {
            get => inout_stack_qty;
            set => Set(ref inout_stack_qty, value);
        }
        public double Middle_Space_M
        {
            get => middle_space_m;
            set =>Set(ref middle_space_m, value);
        }
        public double Out_More_Than_In
        {
            get => out_more_than_in;
            set =>Set(ref out_more_than_in, value);
        }
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

        public bool IsNotInOutTrack
        {
            get => isnotinouttrack;
            set => Set(ref isnotinouttrack, value);
        }

        public ushort Out_Loc_Point
        {
            get => out_loc_point;
            set => Set(ref out_loc_point, value);
        }

        public ushort Out_Sort_Point
        {
            get => out_sort_point;
            set => Set(ref out_sort_point, value);
        }

        public ushort Out_Last_Point
        {
            get => out_last_point;
            set => Set(ref out_last_point, value);
        }

        public ushort In_First_Point
        {
            get => in_first_point;
            set => Set(ref in_first_point, value);
        }

        public ushort In_Loc_Point
        {
            get => in_loc_point;
            set => Set(ref in_loc_point, value);
        }
        #endregion

        #region[命令]
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand<string> TrackPointUpdateCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(TrackPointUpdate)).Value;
        public RelayCommand CalculateFirstLastCmd => new Lazy<RelayCommand>(() => new RelayCommand(CalculateFirstLast)).Value;

        /// <summary>
        /// 轨道脉冲更新
        /// </summary>
        /// <param name="tag"></param>
        private void TrackPointUpdate(string tag)
        {
            switch (tag)
            {
                #region[出定位脉冲]
                case "out_loc_point":

                    if(Out_Loc_Point <= 0)
                    {
                        Growl.Warning("请输入正确的出定位脉冲！");
                        return;
                    }

                    if(Out_Last_Point > Set_Out_Loc_Point)
                    {
                        Growl.Warning("出定位脉冲不能比出最后一车脉冲小！");
                        return;
                    }

                    if(PubMaster.Track.UpdateTrackLimitOut(filterareaid, filterlineid, Set_Out_Loc_Point))
                    {
                        Growl.Success("更新成功！");
                    }
                    break;
                #endregion

                #region[入定位脉冲]
                case "in_loc_point":

                    if (Set_In_Loc_Point <= 0 || Set_In_Loc_Point <= 1000)
                    {
                        Growl.Warning("请输入正确的入定位脉冲！");
                        return;
                    }

                    if (Set_In_Loc_Point > In_First_Point)
                    {
                        Growl.Warning("入定位脉冲不能大于比入第一车脉冲！");
                        return;
                    }

                    if (PubMaster.Track.UpdateTrackLimitIn(filterareaid, filterlineid, Set_In_Loc_Point))
                    {
                        Growl.Success("更新成功！");
                    }
                    break;
                #endregion

                #region[入第一车脉冲]
                case "in_first_point":

                    if (Set_In_First_Point <= 0)
                    {
                        Growl.Warning("请输入正确的入定位脉冲！");
                        return;
                    }

                    if (Set_In_First_Point < In_Loc_Point )
                    {
                        Growl.Warning("第一车脉冲不能小于入定位脉冲！");
                        return;
                    }

                    if (PubMaster.Track.UpdateTrackFirstIn(filterareaid, filterlineid, Set_In_First_Point))
                    {
                        Growl.Success("更新成功！");
                    }
                    break;
                #endregion

                #region[出最后一车脉冲]
                case "out_last_point":

                    if (Set_Out_Last_Point <= 0)
                    {
                        Growl.Warning("请输入正确的出定位脉冲！");
                        return;
                    }

                    if (Set_Out_Last_Point > Out_Loc_Point)
                    {
                        Growl.Warning("出最后一车脉冲不能大于出定位脉冲！");
                        return;
                    }

                    if (PubMaster.Track.UpdateTrackLastOut(filterareaid, filterlineid, Set_Out_Last_Point))
                    {
                        Growl.Success("更新成功！");
                    }
                    break;
                #endregion

                #region[接力倒库脉冲]

                case "out_sort_point":

                    if(Set_Out_Sort_Qty == 0)
                    {
                        string tip3 = "确认重置接力脉冲为零吗？";

                        MessageBoxResult rs3 = HandyControl.Controls.MessageBox.Show(tip3, "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (rs3 == MessageBoxResult.OK || rs3 == MessageBoxResult.Yes)
                        {
                            PubMaster.Track.UpdateTrackSortOut(filterareaid, filterlineid, Set_Out_Sort_Point);
                        }
                    }
                    else
                    {
                        if (Set_Out_Sort_Point <= 0)
                        {
                            Growl.Warning("请输入正确的倒库脉冲！");
                            return;
                        }

                        if (Set_Out_Sort_Point <= Out_Last_Point || Set_Out_Sort_Point >= Out_Loc_Point)
                        {
                            Growl.Warning("倒库接力脉冲需要在出轨道范围内！");
                            return;
                        }

                        if (PubMaster.Track.UpdateTrackSortOut(filterareaid, filterlineid, Set_Out_Sort_Point))
                        {
                            Growl.Success("更新成功！");
                        }
                    }
                    GlobalWcsDataConfig.DefaultConfig.UpdateAreaPointSortQty(filterareaid, filterlineid, Set_Out_Sort_Qty);

                    break;
                    
                case "out_sort_point_calculate":

                    if(Set_Out_Sort_Qty <= 0)
                    {
                        Set_Out_Sort_Point = 0;
                        return;
                    }

                    if(Set_Out_Sort_Qty <= 1)
                    {
                        Growl.Warning("请输入正确的接力车数！");
                        return;
                    }
                    ushort sortpoint = (ushort)(Out_Loc_Point - ((Set_Out_Sort_Qty-1) * Each_Stack_Point) - 50);
                    if(sortpoint < Out_Last_Point)
                    {
                        Growl.Warning("接力位置已经超过最后一车,请重新设置！");
                        return;
                    }
                    Set_Out_Sort_Point = sortpoint;

                    break;

                #endregion

                #region[保存轨道中间空间]

                case "cal_mid_space_out_more":
                    GlobalWcsDataConfig.DefaultConfig.UpdateAreaPoint(filterareaid, filterlineid, Out_More_Than_In, Middle_Space_M);
                    CalculateFirstLast();
                    break;
                    #endregion
            }
            Console.WriteLine(tag);
            CheckAreaTrackPosSetShow();
        }


        #endregion

        #region[方法]
        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleAreaLine(out uint areaid, out ushort lineid))
            {
                filterareaid = areaid;
                filterlineid = lineid;
                ShowAreaFileter = false;
                CheckAreaTrackPosSetShow(true);
                GetAreaPointDefaultData();
            }
        }

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                if (btn.DataContext is MyRadioBtn radio)
                {
                    filterareaid = radio.AreaID;
                    filterlineid = radio.Line;
                    CheckAreaTrackPosSetShow(true);
                    GetAreaPointDefaultData();
                }
            }
        }

        private void CheckAreaTrackPosSetShow(bool refreshsetpoint = false)
        {
            if (filterareaid == 0)
            {
                ClearInput();
                return;
            }
            bool onlyinouttrack = PubMaster.Track.ExistTrackInType(filterareaid, filterlineid, TrackTypeE.储砖_出入);

            IsNotInOutTrack = !onlyinouttrack;
            if (!onlyinouttrack)
            {
                InOrOutGridLen = GridLength.Auto;
                InOutGridLen = onegridlen;
                Track outtrack = PubMaster.Track.GetAreaTrack(filterareaid, filterlineid, TrackTypeE.储砖_出);
                Track intrack = PubMaster.Track.GetAreaTrack(filterareaid, filterlineid, TrackTypeE.储砖_入);
                if (outtrack == null || intrack == null) return;

                In_Loc_Point = intrack.limit_point;
                In_First_Point = intrack.split_point;

                Out_Loc_Point = outtrack.limit_point_up;
                Out_Sort_Point = outtrack.up_split_point;
                Out_Last_Point = outtrack.split_point;

                CalInTrackLenQty();
                CalOutTrackLenQty();

                if (refreshsetpoint)
                {
                    Set_In_Loc_Point = intrack.limit_point;
                    Set_In_First_Point = intrack.split_point;

                    Set_Out_Loc_Point = outtrack.limit_point_up;
                    Set_Out_Last_Point = outtrack.split_point;
                    Set_Out_Sort_Point = outtrack.up_split_point;
                }
            }
            else
            {
                InOrOutGridLen = zerogridlen;
                InOutGridLen = GridLength.Auto;
                Track inouttrack = PubMaster.Track.GetAreaTrack(filterareaid, filterlineid, TrackTypeE.储砖_出入);
                if (inouttrack == null) return;

                In_Loc_Point = inouttrack.limit_point;
                Out_Loc_Point = inouttrack.limit_point_up;

                CalInOutTrackLenQty();

                if (refreshsetpoint)
                {
                    Set_In_Loc_Point = inouttrack.limit_point;
                    Set_Out_Loc_Point = inouttrack.limit_point_up;
                }
            }
        }

        private void CalculateFirstLast()
        {
            if (Middle_Space_M <= 0) return;
            if (In_Loc_Point <= 0) return;
            if (Out_Loc_Point <= 0) return;

            Set_Out_Last_Point = (ushort)((Out_Loc_Point + In_Loc_Point) / 2 - ((Out_More_Than_In - Middle_Space_M) / 2 * 100 / Point_To_Cm));
            Set_In_First_Point = (ushort)(Set_Out_Last_Point - Middle_Space_M * 100 / Point_To_Cm);
        }

        private void GetAreaPointDefaultData()
        {
            AreaPointSetData data = GlobalWcsDataConfig.DefaultConfig.GetAreaPoint(filterareaid, filterlineid);
            if(data != null)
            {
                Middle_Space_M = data.Middle_Space_M;
                Out_More_Than_In = data.Out_More_Than_In;
                Set_Out_Sort_Qty = data.Set_Out_Sort_Qty;
            }
        }

        private void CalInTrackLenQty()
        {
            if (In_Loc_Point <= 0) return;
            if (In_First_Point <= 0) return;
            if (Point_To_Cm <= 0) return;

            In_Track_Len = GetLenValue(In_First_Point, In_Loc_Point);
            In_Stack_Qty = GetQtyValue(In_First_Point, In_Loc_Point);
        }

        private void CalOutTrackLenQty()
        {
            if (Out_Loc_Point <= 0) return;
            if (Out_Last_Point <= 0) return;
            if (Point_To_Cm <= 0) return;

            Out_Track_Len = GetLenValue(Out_Loc_Point, Out_Last_Point);
            Out_Stack_Qty = GetQtyValue(Out_Loc_Point, Out_Last_Point);
        }

        private void CalInOutTrackLenQty()
        {
            if (In_Loc_Point <= 0) return;
            if (Out_Loc_Point <= 0) return;
            if (Point_To_Cm <= 0) return;

            Inout_Track_Len = GetLenValue(Out_Loc_Point, In_Loc_Point);
            Inout_Stack_Qty = GetQtyValue(Out_Loc_Point, In_Loc_Point);
        }

        /// <summary>
        /// 计算轨道长度
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private double GetLenValue(ushort v1, ushort v2)
        {
            return Math.Round((double)((v1 - v2) * Point_To_Cm / 100), 1);
        }

        /// <summary>
        /// 计算可放车数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private double GetQtyValue(ushort v1, ushort v2)
        {
            return Math.Round((double)((v1 - v2) / Each_Stack_Point), 1)+1;
        }

        private void ClearInput()
        {
            In_Loc_Point = 0;
            In_First_Point = 0;

            Out_Loc_Point = 0;
            Out_Last_Point = 0;
            Out_Sort_Point = 0;

            In_Stack_Qty = 0;
            In_Track_Len = 0;
            Out_Stack_Qty = 0;
            Out_Track_Len = 0;

            Inout_Stack_Qty = 0;
            Inout_Track_Len = 0;

            Set_In_Loc_Point = 0;
            Set_In_First_Point = 0;

            Set_Out_Loc_Point = 0;
            Set_Out_Last_Point = 0;

            Middle_Space_M = 0;
            Out_More_Than_In = 0;

            Set_Out_Sort_Point = 0;
            Set_Out_Sort_Qty = 0;
        }
        #endregion

        protected override void TabActivate()
        {
            Each_Stack_Point = PubMaster.Goods.GetStackSafe(0, 0);
            Point_To_Cm = PubMaster.Dic.GetDtlDouble(DicTag.Pulse2CM);
        }

        protected override void TabDisActivate()
        {

        }
    }
}
