using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tool.mlog;

namespace task.trans
{
    /// <summary>
    /// 任务分析类
    /// </summary>
    public abstract class BaseDiagnose
    {
        protected TransMaster _M {private set; get; }
        internal Log _mLog;
        public BaseDiagnose(TransMaster master)
        {
            _M = master;
        }

        /// <summary>
        /// 分析任务
        /// </summary>
        public abstract void Diagnose();
    }
}
