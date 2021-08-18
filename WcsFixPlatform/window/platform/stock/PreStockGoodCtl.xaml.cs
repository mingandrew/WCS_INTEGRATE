using GalaSoft.MvvmLight.Messaging;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace wcs.window
{
    /// <summary>
    /// TileTrackCtl.xaml 的交互逻辑
    /// </summary>
    public partial class PreStockGoodCtl : UserControl
    {
        public PreStockGoodCtl()
        {
            InitializeComponent();
        }

        private void tb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");

            e.Handled = re.IsMatch(e.Text);
        }
    }
}
