using System.Windows.Controls;
using wcs.ViewModel;

namespace wcs.Dialog
{
    /// <summary>
    /// OperateGrand.xaml 的交互逻辑
    /// </summary>
    public partial class OperateGrandDialog
    {
        public OperateGrandDialog()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if(sender is PasswordBox box)
            {
                ViewModelLocator.Instance.OperateGrand.PASSWORD = box.Password;
            }
        }
    }
}
