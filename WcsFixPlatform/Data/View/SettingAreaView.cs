using enums;
using GalaSoft.MvvmLight;
using module.line;
using resource;
using System;
using System.Collections.Generic;

namespace wcs.Data.View
{
    public class SettingAreaView : ViewModelBase
    {
        public uint id { set; get; }
        public string name { set; get; }

        public uint up_car_count { set; get; }//上砖侧设定的运输车数量
        public uint down_car_count { set; get; }//下砖侧设定的运输车数量
    }
}
