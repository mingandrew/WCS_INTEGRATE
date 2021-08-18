using enums;
using GalaSoft.MvvmLight.Messaging;
using System.Text.RegularExpressions;

namespace wcs.Dialog
{
    /// <summary>
    /// PreStockGoodDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PreStockGoodDialog
    {
        public PreStockGoodDialog()
        {
            InitializeComponent();
        }

        private void tb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");

            e.Handled = re.IsMatch(e.Text);
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string fname = ((System.Windows.Controls.TextBox)e.Source).Text;
            Messenger.Default.Send(fname, MsgToken.AutoSearchStockGood);
        }
    }
}
