using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.device;
using module.msg;
using enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using module.window;
using task;

namespace wcs.ViewModel
{
    public class FerryAutoPosDialogViewModel : ViewModelBase, IDialogResultable<MsgAction>
    {
        public FerryAutoPosDialogViewModel()
        {
            _result = new MsgAction();
        }

        #region[字段]
        private Device selectferry;
        private MsgAction _result;
        private int starttrack;
        private string tracknumber;
        private string ferryname;
        private DevFerryAutoPosE _devferryautopos;

        #endregion


        #region[属性]
        public Device SELECTFERRY
        {
            get => selectferry;
            set => Set(ref selectferry, value);
        }
        public int STARTTRACKCODE
        {
            get => starttrack;
            set => Set(ref starttrack, value);
        }

        public string TRACKNUMBER
        {
            get => tracknumber;
            set => Set(ref tracknumber, value);
        }

        public DevFerryAutoPosE AUTOPOSSIDE
        {
            get => _devferryautopos;
            set => Set(ref _devferryautopos, value);
        }
        public string FerryName
        {
            get => ferryname;
            set => Set(ref ferryname, value);
        }




        public MsgAction Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public Action CloseAction { get; set; }
        #endregion



        #region[命令]
        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;


        #endregion


        #region[方法]
        private void CancelChange()
        {
            Result.o1 = null;
            Result.o2 = null;
            Result.o3 = null;
            CloseAction?.Invoke();
        }



        public void SetDialog(string ferryname, int ferrycode)
        {
            FerryName = ferryname;
            STARTTRACKCODE = ferrycode;
            TRACKNUMBER = "";
            if (((STARTTRACKCODE > 200 && STARTTRACKCODE < 300) || STARTTRACKCODE > 600 && STARTTRACKCODE < 700))
            {
                AUTOPOSSIDE = DevFerryAutoPosE.上砖侧对位;
            }
            if (((STARTTRACKCODE > 500 && STARTTRACKCODE < 600) || (STARTTRACKCODE > 100 && STARTTRACKCODE < 200)))
            {
                AUTOPOSSIDE = DevFerryAutoPosE.下砖侧对位;
            }
        }
        private void Comfirm()
        {
            //if (string.IsNullOrEmpty(STARTTRACKCODE))
            //{
            //    Growl.Warning("请输入起始轨道号！");
            //    return;
            //}

            if (string.IsNullOrEmpty(TRACKNUMBER))
            {
                Growl.Warning("请输入对位轨道数量！");
                return;
            }
            if (AUTOPOSSIDE == 0)
            {
                Growl.Warning("请选择对位侧！");
                return;
            }
           
            Result.o1 = STARTTRACKCODE;
            Result.o2 = TRACKNUMBER;
            Result.o3 = AUTOPOSSIDE;
            if (Result.o1 != null && Result.o2 != null && Result.o3 != null)
            {
                try
                {
                    int starttrack = STARTTRACKCODE;//Convert.ToInt32(STARTTRACKCODE);
                    byte tracknumber = Convert.ToByte(TRACKNUMBER);
                    PubTask.Ferry.AutoPosMsgSend(SELECTFERRY.id, (DevFerryAutoPosE)Result.o3, starttrack, tracknumber);
                }
                finally { }
            }
        }

        internal void SetPosSide(DevFerryAutoPosE autoPosSide)
        {
            AUTOPOSSIDE = autoPosSide;
        }



        #endregion
    }
}
