using task.trans;
using tool.mlog;

namespace task.diagnose
{
    /// <summary>
    /// 任务分析类
    /// </summary>
    public abstract class TransBaseDiagnose
    {
        protected TransMaster _M {private set; get; }
        internal Log _mLog;
        public TransBaseDiagnose(TransMaster master)
        {
            _M = master;
        }

        /// <summary>
        /// 分析任务
        /// </summary>
        public abstract void Diagnose();
    }
}
