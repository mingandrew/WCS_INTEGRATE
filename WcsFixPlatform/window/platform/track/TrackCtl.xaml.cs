using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wcs.window
{
    /// <summary>
    /// TrackCtl.xaml 的交互逻辑
    /// </summary>
    public partial class TrackCtl
    {
        public TrackCtl()
        {
            InitializeComponent();
        }

        ScrollViewer sv1, sv2;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //分别获取两个DataGrid的ScrollViewer
            sv1 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.InDataGrid, 0), 0) as ScrollViewer;
            sv2 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.OutDataGrid, 0), 0) as ScrollViewer;

            //关联ScrollChanged事件
            sv1.ScrollChanged += new ScrollChangedEventHandler(sv1_ScrollChanged);
            sv2.ScrollChanged += new ScrollChangedEventHandler(sv2_ScrollChanged);
        }

        void sv1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sv2.ScrollToVerticalOffset(sv1.VerticalOffset);
        }

        void sv2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sv1.ScrollToVerticalOffset(sv2.VerticalOffset);
        }
    }
}
