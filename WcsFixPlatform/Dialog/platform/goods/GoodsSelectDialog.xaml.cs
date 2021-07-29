using enums;
using GalaSoft.MvvmLight.Messaging;
using module.msg;
using System;

namespace wcs.Dialog
{
    /// <summary>
    /// DictionSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsSelectDialog
    {
        public GoodsSelectDialog()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string fname = ((System.Windows.Controls.TextBox)e.Source).Text;
            Messenger.Default.Send(fname, MsgToken.AutoSearchGood);
        }
    }
}
