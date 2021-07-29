using enums;
using GalaSoft.MvvmLight;
using module.line;
using resource;
using System;
using System.Collections.Generic;

namespace wcs.Data.View
{
    public class SettingMenuView : ViewModelBase
    {
        public uint id { set; get; }
        public string name { set; get; }
        public uint p_id { set; get; }
        public uint level { set; get; }
        public List<SettingMenuView> childlist { set; get; }//区域中线的集合
    }
}
