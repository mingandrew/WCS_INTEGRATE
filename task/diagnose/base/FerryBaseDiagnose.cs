using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using task.device;
using tool.mlog;

namespace task.diagnose
{
    public abstract class FerryBaseDiagnose
    {
        protected FerryMaster _M { private set; get; }
        internal Log _mLog;
        public FerryBaseDiagnose(FerryMaster master)
        {
            _M = master;
        }

        /// <summary>
        /// 分析任务
        /// </summary>
        public abstract void Diagnose();
    }
}
