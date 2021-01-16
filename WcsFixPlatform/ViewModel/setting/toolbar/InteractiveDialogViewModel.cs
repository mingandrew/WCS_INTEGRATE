using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wcs.ViewModel.setting.toolbar
{
    public class InteractiveDialogViewModel : ViewModelBase, IDialogResultable<string>
    {
        public InteractiveDialogViewModel()
        {
            //Messenger.Default.Register<string>(this, MsgToken.DialogMsgShow, DialogMsgShow);
        }

        /// <summary>
        /// 设置内容及颜色
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isOK"></param>
        public void SetMsg(string msg, bool isOK)
        {
            Message = string.IsNullOrEmpty(msg) ? "请稍候" : msg;

            Color = isOK ? "Black" : "Red";
        }

        public Action CloseAction { get; set; }

        private string _result;
        private string _message;
        private string _color;

        public string Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public RelayCommand CloseCmd => new Lazy<RelayCommand>(() => new RelayCommand(() => CloseAction?.Invoke())).Value;
    }
}
