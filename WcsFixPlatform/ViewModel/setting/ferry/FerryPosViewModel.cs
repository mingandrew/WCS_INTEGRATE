using enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.msg;
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
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class FerryPosViewModel : MViewModel
    {
        public FerryPosViewModel() : base("FerryPos")
        {
            List = new ObservableCollection<FerryPosView>();
            Messenger.Default.Register<MsgAction>(this, MsgToken.FerrySiteUpdate, FerrySiteUpdate);
            InitAreaRadio();
            CheckIsSingle();
        }

        #region[单区域]

        private bool showareafilter = true;
        private uint areaid;

        public bool ShowAreaFileter
        {
            get => showareafilter;
            set => Set(ref showareafilter, value);
        }

        private void CheckIsSingle()
        {
            if (PubMaster.Area.IsSingleArea(out uint aid))
            {
                ShowAreaFileter = false;
                areaid = aid;
            }
        }
        #endregion

        #region[字段]
        private Device _selectferry;
        private string _selectferryname;

        private bool _uplightcheck, _downlightcheck;

        private ObservableCollection<FerryPosView> _list;
        private FerryPosView _selectpos;
        private ushort _ferrycode;
        private DateTime? _refreshtime;
        private int _nowpos;
        private string _setpos;
        private bool _issetting;
        private IList<MyRadioBtn> _arearadio;

        private bool showferrypos;
        private DateTime showtime;
        #endregion

        #region[属性]
        public bool SHOWFERRYPOS
        {
            get => showferrypos;
            set => Set(ref showferrypos, value);
        }
        public IList<MyRadioBtn> AreaRadio
        {
            get => _arearadio;
            set => Set(ref _arearadio, value);
        }
        public string SelectFerryName
        {
            get => _selectferryname;
            set => Set(ref _selectferryname, value);
        }
        public bool UpLightCheck
        {
            get => _uplightcheck;
            set => Set(ref _uplightcheck, value);
        }
        public bool DownLightCheck
        {
            get => _downlightcheck;
            set => Set(ref _downlightcheck, value);
        }
        public ObservableCollection<FerryPosView> List
        {
            get => _list;
            set => Set(ref _list, value);
        }
        public FerryPosView SelectPos
        {
            get => _selectpos;
            set => Set(ref _selectpos, value);
        }
        public ushort FerryCode
        {
            get => _ferrycode;
            set => Set(ref _ferrycode, value);
        }
        public DateTime? RefreshTime
        {
            get => _refreshtime;
            set => Set(ref _refreshtime, value);
        }
        public int NowPos
        {
            get => _nowpos;
            set => Set(ref _nowpos, value);
        }
        public string SetPos
        {
            get => _setpos;
            set => Set(ref _setpos, value);
        }

        #endregion

        #region[命令]
        public RelayCommand DeviceSelectedCmd => new Lazy<RelayCommand>(() => new RelayCommand(DeviceSelected)).Value;
        public RelayCommand PosSelectedChangeCmd => new Lazy<RelayCommand>(() => new RelayCommand(PosSelectedChange)).Value;
        public RelayCommand<string> BtnSelectCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(BtnSelect)).Value;
        public RelayCommand<RoutedEventArgs> CheckRadioBtnCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(CheckRadioBtn)).Value;
        public RelayCommand AutoPosCmd => new Lazy<RelayCommand>(() => new RelayCommand(AutoPos)).Value;
        public RelayCommand QueryPosCmd => new Lazy<RelayCommand>(() => new RelayCommand(QueryPosList)).Value;

        #endregion

        #region[方法]

        private void CheckRadioBtn(RoutedEventArgs args)
        {
            if (args.OriginalSource is RadioButton btn)
            {
                string area = btn.Tag.ToString();

                if (uint.TryParse(area, out uint aid))
                {
                    this.areaid = aid;

                    if (_selectferry == null)
                    {
                        Growl.Warning("请先选择摆渡车！");
                        return;
                    }

                    CheckAreaHaveTrackAndAdd();
                }
            }
        }

        private void CheckAreaHaveTrackAndAdd()
        {
            if (_selectferry == null)
            {
                Growl.Warning("请先选择摆渡车！");
                return;
            }

            if (areaid == 0) return;
            List.Clear();
            List<FerryPos> pos = PubMaster.Track.GetFerryPos(areaid, _selectferry.id);
            if (pos == null || pos.Count == 0)
            {
                bool havetrack = PubMaster.Area.ExistAreaDevTrack(areaid, _selectferry.id);
                if (havetrack)
                {
                    Growl.Ask("是否添加轨道的信息！", isConfirmed =>
                    {
                        if (isConfirmed)
                        {
                            pos = PubMaster.Track.AddFerryPos(areaid, _selectferry.id);
                            foreach (FerryPos p in pos)
                            {
                                List.Add(new FerryPosView(p));
                            }
                        }
                        return true;
                    });
                }
            }
            else
            {
                foreach (FerryPos p in pos)
                {
                    List.Add(new FerryPosView(p));
                }
            }
        }

        private async void DeviceSelected()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceSelectDialog>()
                 .Initialize<DeviceSelectViewModel>((vm) =>
                 {
                     vm.FilterArea = false;
                     vm.AreaId = 0;
                     vm.SetSelectType(new List<DeviceTypeE>() { DeviceTypeE.上摆渡, DeviceTypeE.下摆渡 });
                 }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && result.p2 is Device dev)
            {
                _selectferry = dev;
                SelectFerryName = dev.name;
                PubTask.Ferry.StartFerryPosSetting(dev.id, 201);
                CheckAreaHaveTrackAndAdd();
            }
        }
        private void PosSelectedChange()
        {
            if (SelectPos != null)
            {
                FerryCode = SelectPos.Ferry_Code;
                //SetPos = SelectPos.Ferry_Pos + "";
                SetPos = "";
            }
        }
        private void BtnSelect(string tag)
        {
            if (_selectferry == null)
            {
                Growl.Warning("请先选择摆渡车！");
                return;
            }

            switch (tag)
            {
                case "cleardev":
                    PubTask.Ferry.StopFerryPosSetting();
                    ClearInput();
                    break;
                case "stopdev":
                    if (!PubTask.Ferry.StopFerry(_selectferry.id, "对位界面终止", "逻辑", out string result))
                    {
                        Growl.Info(result);
                        return;
                    }
                    Growl.Success("发送成功！");
                    break;
                case "devtocode":
                    if (SelectPos != null && SelectPos.Ferry_Code > 0)
                    {
                        string tipwaring = "确认发送对位：" + SelectPos.Ferry_Code + " 给" + _selectferry.name + "吗？";
                        MessageBoxResult rs = HandyControl.Controls.MessageBox.Show(tipwaring, "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (rs == MessageBoxResult.Yes)
                        {
                            bool isdownferry = _selectferry.Type == DeviceTypeE.下摆渡;
                            if (!PubTask.Ferry.DoManualLocate(_selectferry.id, SelectPos.Track_Id, isdownferry, out result))
                            {
                                Growl.Info(result);
                                return;
                            }
                            Growl.Success("发送成功！");
                        }
                    }
                    break;
                case "updatenowpos":
                    SetPos = NowPos + "";
                    SetFerryPos();
                    break;
                case "updatesetpos":
                    SetFerryPos();
                    break;
                case "leftreset":
                    ReSetFerry(DevFerryResetPosE.前进复位);
                    break;
                case "rightreset":
                    ReSetFerry(DevFerryResetPosE.后退复位);
                    break;
                case "showferry":
                    ShowFerryPosPanal();
                    break;

                #region[复制选定砖机对位数据或者重新发送一遍]
                case "resendall":
                    ResendAll();
                    break;
                case "copypose":
                    OpenCopyPose();
                    break;
                #endregion
            }
        }

        private async void AutoPos()
        {
            if (_selectferry == null)
            {
                Growl.Warning("请先选择摆渡车！");
                return;
            }
            if (_selectpos == null)
            {
                Growl.Warning("请先选择轨道！");
                return;
            }

            int autolen = PubMaster.Track.GetFerryAutoPosLen(_selectferry.area, _selectferry.id, _selectpos.Ferry_Code);
            MsgAction autopos = await HandyControl.Controls.Dialog.Show<FerryAutoPosDialog>()
                .Initialize<FerryAutoPosDialogViewModel>((vm) =>
                {
                    vm.SELECTFERRY = _selectferry;
                    vm.SetDialog(_selectpos.Ferry_Code, autolen);
                }).GetResultAsync<MsgAction>();
        }

        private void SetFerryPos()
        {
            if (SelectPos == null)
            {
                Growl.Warning("请先选择轨道！");
                return;
            }

            bool isint = int.TryParse(SetPos, out int intpos);
            if (string.IsNullOrEmpty(SetPos) || !isint)
            {
                Growl.Warning("请输入正确格式的坐标!");
                return;
            }

            if (!PubTask.Ferry.SetFerryPos(_selectferry.id, SelectPos.Ferry_Code, intpos, out string result))
            {
                Growl.Warning(result);
                return;
            }
            Growl.Success("发送成功！");
        }

        private void ReSetFerry(DevFerryResetPosE type)
        {
            if (!PubTask.Ferry.ReSetFerry(_selectferry.id, type, "PC", out string result))
            {
                Growl.Warning(result);
                return;
            }
            Growl.Success("发送成功！");
        }

        private void ClearInput()
        {
            _selectferry = null;
            SelectFerryName = "";
            SetPos = "";
            UpLightCheck = false;
            DownLightCheck = false;
            NowPos = 0;
            RefreshTime = null;
            List.Clear();
            SHOWFERRYPOS = false;

        }

        private void QueryPosList()
        {
            if (_selectferry == null)
            {
                Growl.Warning("请选择摆渡车");
                return;
            }

            if (!PubTask.Ferry.IsOnline(_selectferry.id))
            {
                Growl.Warning("设备离线！");
                return;
            }
            PubTask.Ferry.RefreshPosList(_selectferry.id);
        }

        private void ResendAll()
        {
            if (!PubTask.Ferry.ReSendAllFerryPose(_selectferry.id, out string result))
            {
                Growl.Warning(result);
                return;
            }

            Growl.Success("正在发送！");
        }

        /// <summary>
        /// 打开复制摆渡车
        /// </summary>
        private async void OpenCopyPose()
        {
            if (_selectferry == null)
            {
                Growl.Warning("请先选择摆渡车！");
                return;
            }

            MsgAction result = await HandyControl.Controls.Dialog.Show<FerryCopyPosDialog>()
                 .Initialize<FerryCopyPosDialogViewModel>((vm) =>
                 {
                     vm.InitVar();
                     vm.BeforeOpenDialog(_selectferry);
                 }).GetResultAsync<MsgAction>();
            if (result.o1 is bool rs && rs)
            {
                Growl.Success("复制成功！");
                CheckAreaHaveTrackAndAdd();
            }
        }


        #endregion

        #region[刷新]

        private void FerrySiteUpdate(MsgAction msg)
        {
            if (msg.o1 is DevFerry ferry)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpLightCheck = ferry.UpLight;
                    DownLightCheck = ferry.DownLight;
                });
            }
            if (msg.o1 is DevFerrySite site)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RefreshTime = DateTime.Now;
                    NowPos = site.NowTrackPos;

                    FerryPosView view = List.FirstOrDefault(c => c.Ferry_Code == site.TrackCode);
                    if (view != null && view.Ferry_Pos != site.TrackPos //&& site.TrackPos != 0
                    )
                    {
                        view.Ferry_Pos = site.TrackPos;
                        PubMaster.Track.UpdateFerryPos(view.Id, view.Ferry_Pos);
                    }
                });
            }
        }

        private async void ShowFerryPosPanal()
        {
            MsgAction checkright = await HandyControl.Controls.Dialog.Show<CheckRightDialog>()
                .Initialize<CheckRightDialogViewModel>((vm) =>
                {
                    vm.SetComparePWD("keda");
                }).GetResultAsync<MsgAction>();
            if (checkright.o1 is bool r && r)
            {
                SHOWFERRYPOS = true;
                showtime = DateTime.Now;
            }
        }

        #endregion

        #region[界面激活]
        protected override void TabActivate()
        {
            _issetting = true;
            if (_selectferry != null)
            {
                PubTask.Ferry.StartFerryPosSetting(_selectferry.id, FerryCode);
            }
        }

        protected override void TabDisActivate()
        {
            if (_issetting)
            {
                PubTask.Ferry.StopFerryPosSetting();
                _issetting = false;
            }

            if (SHOWFERRYPOS)
            {
                //对位功能开启十分钟后自动关闭
                if (DateTime.Now.Subtract(showtime).TotalMinutes > 10)
                {
                    SHOWFERRYPOS = false;
                }
            }
        }

        private void InitAreaRadio()
        {
            AreaRadio = PubMaster.Area.GetAreaRadioList();
        }
        #endregion
    }
}
