using GalaSoft.MvvmLight;
using module;
using System;

namespace wcs.Data.View
{
    public class WarningView : ViewModelBase
    {

        #region[参数变量]
        public uint id;
        public string content;
        public DateTime? createtime;
        public DateTime? resolvetime;
        //public double remaintime;
        #endregion

        public uint ID
        {
            get => id;
            set => Set(ref id, value);
        }

        public string CONTENT
        {
            get => content;
            set => Set(ref content, value);
        }

        public DateTime? CREATETIME
        {
            get => createtime;
            set => Set(ref createtime, value);
        }

        //public double REMAINTIME
        //{
        //    get => remaintime;
        //    set => Set(ref remaintime, value);
        //}

        public WarningView(Warning md)
        {
            ID = md.id;
            CONTENT = md.content;
            CREATETIME = md.createtime;
            //Update();
        }

        //public void Update()
        //{
        //    if (CREATETIME is DateTime date)
        //    {
        //        remaintime = (DateTime.Now - date).TotalSeconds;
        //    }
        //}
    }
}
