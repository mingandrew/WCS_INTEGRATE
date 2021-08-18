using enums;
using enums.warning;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.deviceconfig;
using module.goods;
using module.msg;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using task;
using wcs.Data.View;
using wcs.Dialog;

namespace wcs.ViewModel
{
    /// <summary>
    /// 砖机转产操作面板
    /// </summary>
    public class GoodShiftDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public GoodShiftDialogViewModel()
        {
            _result = new MsgAction();
            syntileids = new List<uint>();
            syntilename = "";
            InitTimer();
        }

        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
        public Action CloseAction { get; set; }

        private uint _devid, _area, _goodsid, _pregoodsid, _level;
        private string _nowgname, _pregname, _nowcolor, _precolor;
        private int _nowlevel, _prelevel;
        private string _nowgqty, _pregqty;//全部 或确切数字
        private string _devname;
        TileShiftStatusE _shiftstatus;
        private bool shiftbtnenable;

        private bool showlevel = true;

        //需要同步转产的砖机id
        private List<uint> syntileids;
        public List<uint> SynTileIds
        {
            get => syntileids;
            set => Set(ref syntileids, value);
        }

        public uint AREA
        {
            get => _area;
            set => Set(ref _area, value);
        }

        public string DEVNAME
        {
            get => _devname;
            set => Set(ref _devname, value);
        }

        public TileShiftStatusE SHIFTSTATUS
        {
            get => _shiftstatus;
            set => Set(ref _shiftstatus, value);
        }

        public bool SHIFTBTNENABLE
        {
            get => shiftbtnenable;
            set => Set(ref shiftbtnenable, value);
        }

        public string NowGoodName
        {
            get => _nowgname;
            set => Set(ref _nowgname, value);
        }

        public string PreGoodName
        {
            get => _pregname;
            set => Set(ref _pregname, value);
        }

        public string NowGoodColor
        {
            get => _nowcolor;
            set => Set(ref _nowcolor, value);
        }

        public string PreGoodColor
        {
            get => _precolor;
            set => Set(ref _precolor, value);
        }

        public int NowLevel
        {
            get => _nowlevel;
            set => Set(ref _nowlevel, value);
        }

        public int PreLevel
        {
            get => _prelevel;
            set => Set(ref _prelevel, value);
        }

        public bool ShowLevel
        {
            get => showlevel;
            set => Set(ref showlevel, value);
        }

        public string NowGQty
        {
            get => _nowgqty;
            set => Set(ref _nowgqty, value);
        }

        public string PreGQty
        {
            get => _pregqty;
            set => Set(ref _pregqty, value);
        }

        private string syntilename;
        public string SynTileName
        {
            get => syntilename;
            set => Set(ref syntilename, value);
        }

        #region[字段]
        private MsgAction _result;
        #endregion

        #region[属性]

        #endregion

        #region[命令]

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        public RelayCommand<string> BtnCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(BtnAction)).Value;

        #endregion

        #region[方法]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        private void BtnAction(string tag)
        {
            switch (tag)
            {
                case "chosepregood":
                    DoSelectPreGood();
                    break;
                case "refreshstatus":
                    DoRefresh();
                    break;
                case "doshift":
                    DoShift();
                    break;
                case "clearnpregood":
                    ClearnPreGood();
                    break;
                case "chosesyntiles":
                    DoSelectSynroTileIds();
                    break;
            }

        }

        private void ClearnPreGood()
        {
            if (PubMaster.DevConfig.UpdateTilePreGood(_devid, _goodsid, 0, 0, 0, out string msg))
            {
                SetPreGood(0);
                SetGQty();
            }
        }

        /// <summary>
        /// 选择预设品种
        /// </summary>
        private async void DoSelectPreGood()
        {
            uint area = PubMaster.Device.GetDeviceArea(_devid);
            bool isuptilelifter = PubMaster.Device.IsDevType(_devid, DeviceTypeE.上砖机);
            if (!isuptilelifter)
            {
                DialogResult result = await HandyControl.Controls.Dialog.Show<GoodsSelectDialog>()
             .Initialize<GoodsSelectViewModel>((vm) =>
             {
                 vm.SetAreaFilter(area, false);
                 vm.QueryGood();
             }).GetResultAsync<DialogResult>();

                if (result.p1 is bool rs && result.p2 is GoodsView good)
                {
                    if (PubMaster.DevConfig.UpdateTilePreGood(_devid, _goodsid, good.ID, 0, 0, out string msg))
                    {
                        SetPreGood(good.ID);
                        SetGQty();
                    }
                    else
                    {
                        Growl.Warning(msg);
                    }
                }
            }
            else
            {
                DialogResult result = await HandyControl.Controls.Dialog.Show<PreStockGoodDialog>()
             .Initialize<PreStockGoodViewModel>((vm) =>
             {
                 vm.SelectTile(_devid);
                 //vm.SetAreaFilter(area, false);
                 //vm.QueryStockGood(true);
             }).GetResultAsync<DialogResult>();


                if (result.p1 is bool rs && result.p2 is StockGoodSumView good)
                {
                    int count = good.IsUseAll() ? 0 : good.Count;
                    if (PubMaster.DevConfig.UpdateTilePreGood(_devid, _goodsid, good.GoodId, count, good.Level, out string msg))
                    {
                        SetPreGood(good.GoodId, good.Level);
                        SetGQty();
                    }
                    else
                    {
                        Growl.Warning(msg);
                    }
                }
            }

        }

        /// <summary>
        /// 选择同步转产的砖机
        /// </summary>
        private async void DoSelectSynroTileIds()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<DeviceCheckComboSelectDialog>()
            .Initialize<DeviceCheckComboSelectViewModel>((vm) =>
            {
                vm.SetTileList(_devid);
            }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && result.p2 is List<uint> ids)
            {
                SynTileIds = ids;
                SynTileName = "";
                String name = string.Empty;
                foreach (uint item in ids)
                {
                    name += string.Format("[ {0} ]", PubMaster.Device.GetDeviceName(item));
                }
                SynTileName = name;

                SynTileIds.Add(_devid);
                PubMaster.DevConfig.SetSynchTileIds(SynTileIds);
            }
        }

        private void SetGQty()
        {
            ConfigTileLifter dev = PubMaster.DevConfig.GetTileLifter(_devid);
            if (dev != null)
            {
                if (PubMaster.Device.GetDeviceType(_devid) == DeviceTypeE.下砖机)
                {
                    NowGQty = "-";
                    PreGQty = "-";
                }
                else
                {
                    NowGQty = dev.now_good_all ? "不限" : dev.now_good_qty + "";
                    PreGQty = dev.pre_good_all ? "不限" : (dev.pre_good_qty > 0 ? (dev.pre_good_qty + "") : "-");
                }
                NowLevel = dev.level;
            }
        }

        private void SetNowGood(uint id)
        {
            _goodsid = id;
            Goods goods = PubMaster.Goods.GetGoods(id);
            if (goods != null)
            {
                NowGoodName = goods.name;
                //NowLevel = goods.level;
                NowGoodColor = goods.color;
            }
        }

        private void SetPreGood(uint id, int plevel = 0)
        {
            _pregoodsid = id;
            Goods goods = PubMaster.Goods.GetGoods(id);
            if (goods != null)
            {
                PreGoodName = goods.name;
                //PreLevel = goods.level;
                PreGoodColor = goods.color;
                ShowLevel = true;
            }
            else
            {
                ClearPreGood();
            }
            if (plevel != 0)
            {
                PreLevel = plevel;
            }
        }

        private void ClearPreGood()
        {
            PreGoodName = "";
            PreLevel = 0;
            ShowLevel = false;
            PreGoodColor = "";
            PreGQty = "-";
            _pregoodsid = 0;
        }


        /// <summary>
        /// 刷新转产状态
        /// </summary>
        private void DoRefresh()
        {
            SHIFTSTATUS = PubTask.TileLifter.GetTileShiftStatus(_devid);
            if (SHIFTSTATUS != TileShiftStatusE.转产中)
            {
                SHIFTBTNENABLE = true;
            }
        }

        /// <summary>
        /// 执行转产操作
        /// </summary>
        private void DoShift()
        {
            if (!PubTask.TileLifter.IsOnline(_devid))
            {
                Growl.Warning(string.Format("{0}砖机离线！不能执行转产操作！", _devname));
                return;
            }

            if (PubMaster.Device.IsDevType(_devid, DeviceTypeE.上砖机) && _pregoodsid == 0)
            {
                Growl.Info("请选择预设品种！");
                return;
            }
            
            if (!PubTask.TileLifter.IsSiteGoodSame(_devid))
            {
                Growl.Warning("砖机左右工位品种不一致！");
                return;
            }

            uint confgoodid = PubMaster.DevConfig.GetTileGood(_devid);
            if (confgoodid != _goodsid)
            {
                Growl.Warning("请先刷新设备信息，再执行转产操作！");
                return;
            }

            string confirmName = "";
            string askName = "";
            foreach (uint tid in SynTileIds)
            {
                confirmName += string.Format("【{0}】", PubMaster.Device.GetDeviceName(tid));
                if (!PubMaster.DevConfig.IsShiftInAllowTime(tid))
                {
                    askName += string.Format("【{0}】", PubMaster.Device.GetDeviceName(tid));
                }
            }

            if (!askName.Equals(""))
            {
                string rr = string.Format("{0}砖机在5分钟内已转产过，请确认是否需要再次转产", askName);
                MessageBoxResult box = HandyControl.Controls.MessageBox.Show(rr, "警告",
                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (box == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (PubMaster.Device.IsDevType(_devid, DeviceTypeE.下砖机))
            {
                if (_pregoodsid == 0 && !PubMaster.DevConfig.IsTileHavePreGood(_devid))
                {
                    //添加默认品种 A,B,C,D,E....
                    if (!PubMaster.Goods.AddDefaultGood(_devid, _goodsid, out string ad_rs, out uint pgoodid))
                    {
                        Growl.Warning(ad_rs);
                        return;
                    }
                    else
                    {
                        if (!PubMaster.DevConfig.UpdateTilePreGood(_devid, _goodsid, pgoodid, 0, 0, out string up_rs))
                        {
                            Growl.Info(up_rs);
                            return;
                        }
                    }
                }
            }
            else
            {
                if (_pregoodsid == 0)
                {
                    bool iswarn = true;
                    // 没有预设品种，则检查是否有预设品种列表数据，有则用列表数据，无则警告
                    if (PubMaster.Goods.IsHavePreStockGood(_devid))
                    {
                        PreStockGood firstgood = PubMaster.Goods.GetNextPreStockGood(_devid, _goodsid, (byte)_level);
                        if (firstgood == null)
                        {
                            iswarn = true;
                        }
                        else
                        {
                            iswarn = !PubMaster.DevConfig.UpdateTilePreGood(_devid, _goodsid, firstgood.good_id, (firstgood.pre_good_all ? 0 : firstgood.pre_good_qty), firstgood.level, out string mes);
                        }
                    }
                    if (iswarn)
                    {
                        Growl.Info("请选择预设品种！");
                        return;
                    }
                }
            }
            
            string warncontent = "";
            string succcontent = "";
            
            if (!PubMaster.DevConfig.UpdateShiftTileGood(_devid, _goodsid, out string msg))
            {
                warncontent += string.Format("【{0}】砖机{1} \r\n", PubMaster.Device.GetDeviceName(_devid), msg);
                return;
            }
            succcontent += string.Format("【{0}】砖机开始转产 \r\n", PubMaster.Device.GetDeviceName(_devid));

            foreach (uint tid in SynTileIds)
            {
                if (tid == _devid)
                {
                    continue;
                }
                if (!PubMaster.DevConfig.UpdateShiftTileGoodWithOtherTile(tid, _devid, out msg))
                {
                    warncontent += string.Format("【{0}】砖机{1} \r\n", PubMaster.Device.GetDeviceName(tid), msg);
                }
                else
                {
                    succcontent += string.Format("【{0}】砖机开始转产 \r\n", PubMaster.Device.GetDeviceName(tid));
                }
            }

            PubMaster.DevConfig.SetSynchTileIds(SynTileIds);

            //PubMaster.DevConfig.UpdateShiftTileGood

            //清除上砖数量为0的报警
            PubMaster.Warn.RemoveDevWarn(WarningTypeE.Warning37, (ushort)_devid);


            MessageBoxResult tip = HandyControl.Controls.MessageBox.Show(succcontent + warncontent, "警告",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);


            //Growl.Success("开始转产！");
            CloseAction?.Invoke();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void CancelChange()
        {
            StopTimer();
            CloseAction?.Invoke();
        }

        /// <summary>
        /// 设置设备名称和需要过滤的区域
        /// </summary>
        /// <param name="area"></param>
        /// <param name="devname"></param>
        public void SetArea(uint area, uint devid, string devname, uint goodid, uint level)
        {
            AREA = area;
            _devid = devid;
            _level = level;
            ConfigTileLifter confit = PubMaster.DevConfig.GetTileLifter(devid);
            if (confit == null)
            {
                Growl.Warning("获取不到砖机的配置信息！");
                CancelChange();
                return;
            }
            DEVNAME = devname;
            SetNowGood(goodid);
            SetGQty();
            SHIFTBTNENABLE = false;

            uint pregid = PubMaster.DevConfig.GetDevicePreId(devid);
            if (pregid > 0)
            {
                SetPreGood(pregid);
            }
            else
            {
                ClearPreGood();
            }

            StartTimer();

            SynTileIds.Clear();
            SynTileName = "";
            String name = string.Empty;
            foreach (ushort tid in confit.SynTileList)
            {
                if (tid == _devid)
                {
                    continue;
                }
                SynTileIds.Add(tid);
                name += string.Format("[ {0} ]", PubMaster.Device.GetDeviceName(tid));
            }
            SynTileIds.Add(_devid);
            SynTileName = name;
        }

        #endregion

        #region[刷新转产状态]

        //定义Timer类
        Timer threadTimer;
        private void InitTimer()
        {
            threadTimer = new Timer(new TimerCallback(TimesUp), null, Timeout.Infinite, 2000);
        }

        private void StartTimer()
        {
            //立即开始计时，时间间隔2000毫秒
            threadTimer.Change(0, 2000);
        }

        private void TimesUp(object value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DoRefresh();
            });
        }

        private void StopTimer()
        {
            //停止计时
            threadTimer.Change(Timeout.Infinite, 2000);
        }
        #endregion
    }
}
