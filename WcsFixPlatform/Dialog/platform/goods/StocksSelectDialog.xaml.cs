using System.Text.RegularExpressions;

namespace wcs.Dialog
{
    /// <summary>
    /// DictionSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class StocksSelectDialog
    {
        public StocksSelectDialog()
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
