using System;
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

        private ScrollViewer GetDataGridScrollViewer(DependencyObject obj)
        {
            try
            {
                DependencyObject c1 = VisualTreeHelper.GetChild(obj, 0);
                DependencyObject c2 = VisualTreeHelper.GetChild(c1, 0);
                DependencyObject c3 = VisualTreeHelper.GetChild(c2, 0);
                DependencyObject c4 = VisualTreeHelper.GetChild(c3, 0);
                DependencyObject c5 = VisualTreeHelper.GetChild(c4, 0);
                DependencyObject c6 = VisualTreeHelper.GetChild(c5, 0);

                return c6 as ScrollViewer;
            }catch(Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            return null;
        }
        bool initview = false;
        ScrollViewer sv1, sv2;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (initview) return;
            //分别获取两个DataGrid的ScrollViewer
            sv1 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.InDataGrid, 0), 0) as ScrollViewer;
            sv2 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.OutDataGrid, 0), 0) as ScrollViewer;

            if(sv1 == null)
                sv1 = GetDataGridScrollViewer(this.InDataGrid);

            if(sv2 == null)
                sv2 = GetDataGridScrollViewer(this.OutDataGrid);

            //关联ScrollChanged事件
            if (sv1 != null)
            {
                sv1.ScrollChanged += new ScrollChangedEventHandler(sv1_ScrollChanged);
            }

            if (sv2 != null)
            {
                sv2.ScrollChanged += new ScrollChangedEventHandler(sv2_ScrollChanged);
            }

            initview = true;
        }

        void sv1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sv2 == null) return;
            sv2.ScrollToVerticalOffset(sv1.VerticalOffset);
        }

        void sv2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sv1.ScrollToVerticalOffset(sv2.VerticalOffset);
        }
    }
}
