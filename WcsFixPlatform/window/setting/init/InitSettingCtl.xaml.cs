using System;
using System.Windows.Controls;

namespace wcs.window
{
    /// <summary>
    /// DeviceCtl.xaml 的交互逻辑
    /// </summary>
    public partial class InitSettingCtl
    {
        public InitSettingCtl()
        {
            InitializeComponent();
        }

        private void V_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine(1);
        }
    }
}
