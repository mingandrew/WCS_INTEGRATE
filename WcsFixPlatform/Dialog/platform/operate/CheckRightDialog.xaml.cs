using System.Windows.Controls;
using wcs.ViewModel;

namespace wcs.Dialog
{
    /// <summary>
    /// OperateGrand.xaml 的交互逻辑
    /// </summary>
    public partial class CheckRightDialog
    {
        public CheckRightDialog()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if(sender is PasswordBox box)
            {
                ViewModelLocator.Instance.CheckRight.PASSWORD = box.Password;
            }
        }
    }
}
