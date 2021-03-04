using enums;
using GalaSoft.MvvmLight.Messaging;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using wcs.Data;
using wcs.Resources.Langs;
using wcs.Tools.Helper;

namespace wcs.toolbar
{
    public partial class MainToolBarCtl
    {
        public MainToolBarCtl()
        {
            InitializeComponent();
        }

        private void ButtonLangs_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Tag is string langName)
            {
                PopupConfig.IsOpen = false;
                if (langName.Equals(GlobalData.Config.Lang)) return;
                ConfigHelper.Instance.SetLang(langName);
                LangProvider.Culture = new CultureInfo(langName);
                Messenger.Default.Send<object>(null, MsgToken.LangUpdated);

                GlobalData.Config.Lang = langName;
                GlobalData.Save();
            }
        }

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

    }
}
