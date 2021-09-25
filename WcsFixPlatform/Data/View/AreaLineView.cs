using enums;
using GalaSoft.MvvmLight;
using module.line;
using resource;
using System;

namespace wcs.Data.View
{
    public class AreaLineView : ViewModelBase
    {
        public ushort id { set; get; }
        public uint area_id { set; get; }
        public ushort line { set; get; }
        public string name { set; get; }
        public byte line_type { set; get; }
        public byte max_upsort_num { get; set; }

        private bool? onoff_up;
        private bool? onoff_down;
        private bool? onoff_sort;

        public bool? Onoff_Up
        {
            get => onoff_up;
            set 
            {
                if(Set(ref onoff_up, value))
                {
                    PubMaster.Area.UpdateLineSwitch(id, OnOffTaskE.上砖, (bool)value, "PC", out string _);
                    //Console.WriteLine("上砖:" + name+ value);
                }
            }
        }
        public bool? Onoff_Down
        {
            get => onoff_down;
            set
            {
                if (Set(ref onoff_down, value))
                {
                    PubMaster.Area.UpdateLineSwitch(id, OnOffTaskE.下砖, (bool)value, "PC", out string _);
                    //Console.WriteLine("下砖:" + name + value);
                }
            }
        }

        public bool? Onoff_Sort
        {
            get => onoff_sort;
            set
            {
                if (Set(ref onoff_sort, value))
                {
                    PubMaster.Area.UpdateLineSwitch(id, OnOffTaskE.倒库, (bool)value, "PC", out string _);
                    //Console.WriteLine("倒库:" + name + value);
                }
            }
        }

        /// <summary>
        /// 线路类型
        /// </summary>
        public LineTypeE LineType
        {
            get => (LineTypeE)line_type;
        }

        public bool HaveSort { set; get; }

        public AreaLineView(Line item)
        {
            id = item.id;
            area_id = item.area_id;
            line = item.line;
            name = item.name;
            line_type = item.line_type;
            HaveSort = LineType == LineTypeE.要倒库;
            Update(item);   
        }

        public void Update(Line item)
        {
            Onoff_Down = item.onoff_down;
            Onoff_Sort = item.onoff_sort;
            Onoff_Up = item.onoff_up;
        }
    }
}
