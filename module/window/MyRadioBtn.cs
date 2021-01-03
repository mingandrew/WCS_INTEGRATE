using System.Windows;
using System.Windows.Controls.Primitives;

namespace module.window
{
    public class MyRadioBtn : ButtonBase
    {
        public uint AreaID { set; get; }
        public string AreaName { set; get; }
        public string AreaTag { set; get; }
        public CornerRadius BorderCorner { set; get; } = new CornerRadius(0);

    }
}
