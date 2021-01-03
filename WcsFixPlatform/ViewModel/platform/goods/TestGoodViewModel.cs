using enums.track;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.goods;
using module.track;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class TestGoodViewModel : ViewModelBase
    {

        public TestGoodViewModel()
        {

        }

        #region[字段]

        private Track lefttrack, righttrack;
        private Goods leftgood, rightgood;

        private string lefttraname, righttraname;
        private string leftgoname, rightgoname;
        private bool issite1ok,issite2ok;
        private string result1, result2;
        #endregion

        #region[属性]
        public string LeftTraName
        {
            get => lefttraname;
            set => Set(ref lefttraname, value);
        }
        public string RightTraName
        {
            get => righttraname;
            set => Set(ref righttraname, value);
        }
        public string LeftGoName
        {
            get => leftgoname;
            set => Set(ref leftgoname, value);
        }
        public string RightGoName
        {
            get => rightgoname;
            set => Set(ref rightgoname, value);
        }

        public bool IsSite1Ok
        {
            get => issite1ok;
            set => Set(ref issite1ok, value);
        }
        public bool IsSite2Ok
        {
            get => issite2ok;
            set => Set(ref issite2ok, value);
        }

        public string Result1
        {
            get => result1;
            set => Set(ref result1, value);
        }

        public string Result2
        {
            get => result2;
            set => Set(ref result2, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<string> SelectGoodCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(SelectGood)).Value;


        #endregion

        #region[方法]

        private bool CheckTrackAndGood(ushort trackwidth, ushort goodwidth, ushort trackdistance)
        {
            return (trackdistance -(goodwidth - trackwidth)/2) < 100;
        }

        private void CheckIsOk4Good()
        {
            //检查左轨道是否能放砖
            if(lefttrack != null && leftgood != null)
            {
                if(lefttrack.left_track_id == 0)
                {
                    if (CheckTrackAndGood(lefttrack.width, leftgood.width, lefttrack.left_distance))
                    {
                        IsSite1Ok = false;
                        Result1 = "距离左轨道间距小于100";
                        return;
                    }
                    else
                    {
                        IsSite1Ok = true;
                        Result1 = "";
                    }
                }

                if(lefttrack.right_track_id == 0)
                {
                    if (CheckTrackAndGood(lefttrack.width, leftgood.width, lefttrack.right_distance))
                    {
                        IsSite1Ok = false;
                        Result1 = "距离右轨道间距小于100";
                        return;
                    }
                    else
                    {
                        IsSite1Ok = true;
                        Result1 = "";
                    }
                }
            }
            
            //检查右轨道是否能放砖
            if(righttrack != null && rightgood != null)
            {
                if (righttrack.left_track_id == 0)
                {
                    if (CheckTrackAndGood(righttrack.width, rightgood.width, righttrack.left_distance))
                    {
                        IsSite2Ok = false;
                        Result2 = "距离左轨道间距小于100";
                        return;
                    }
                }

                if (righttrack.right_track_id == 0)
                {
                    if (CheckTrackAndGood(righttrack.width, rightgood.width, righttrack.right_distance))
                    {
                        IsSite2Ok = false;
                        Result2 = "距离右轨道间距小于100";
                        return;
                    }
                }
            }

            if (lefttrack == null || righttrack == null
                || leftgood == null || rightgood == null) return;
            int ld = Math.Abs(leftgood.width - lefttrack.width) / 2;
            int rd = Math.Abs(rightgood.width - righttrack.width) / 2;

            if (lefttrack.right_distance == righttrack.left_distance)
            {
                IsSite1Ok = (righttrack.left_distance - ld - rd) >= 150;
                if (!IsSite1Ok)
                {
                    Result1 = string.Format("两边间距为:{0}", righttrack.left_distance - ld - rd);
                }
                else
                {
                    Result1 = "";
                }
                IsSite2Ok = IsSite1Ok;
                Result2 = Result1;
                return;
            }
            int distance = lefttrack.right_distance < righttrack.left_distance ? lefttrack.right_distance : righttrack.left_distance;
            IsSite1Ok = (distance - ld - rd) >= 150;
            if (!IsSite1Ok)
            {
                Result1 = string.Format("两边间距为:{0}", distance - ld - rd);
            }
            else
            {
                Result1 = "";
            }
            IsSite2Ok = IsSite1Ok;
            Result2 = Result1;
        }

        private async void SelectGood(string tag)
        {
            if (tag.Contains("track"))
            {
                DialogResult result = await HandyControl.Controls.Dialog.Show<TrackSelectDialog>()
                         .Initialize<TrackSelectViewModel>((vm) =>
                         {
                             vm.SetAreaFilter(0, true);
                             vm.QueryTrack(new List<TrackTypeE>() {TrackTypeE.储砖_入, TrackTypeE.储砖_出, TrackTypeE.储砖_出入});
                         }).GetResultAsync<DialogResult>();
                if (result.p1 is Track tra)
                {
                    if (tag.Contains("left"))
                    {
                        lefttrack = tra;
                        LeftTraName = tra.name;
                    }

                    Track trackr = PubMaster.Track.GetTrack(tra.right_track_id);
                    if(trackr != null)
                    {
                        righttrack = trackr;
                        RightTraName = trackr.name;
                    }
                    else{
                        righttrack = null;
                        RightTraName = "";
                        //Growl.Warning("请重新选择");
                        return;
                    }
                    CheckIsOk4Good();
                }
            }
            else
            {
                DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
                            .Initialize<GoodsSelectViewModel>((vm) =>
                            {
                                vm.QueryGood();
                            }).GetResultAsync<DialogResult>();

                if (result.p1 is bool rs && result.p2 is Goods good)
                {
                    if (tag.Contains("left"))
                    {
                        leftgood = good;
                        LeftGoName = good.name;
                    }
                    else
                    {
                        rightgood = good;
                        RightGoName = good.name;
                    }
                    CheckIsOk4Good();
                }
            }
        }
        #endregion
    }
}
