using module.goods;
using tool.timer;

namespace task.trans.transtask
{
    public abstract class BaseTaskTrans
    {
        internal TransMaster _M { set; get; }
        internal MTimer mTimer;
        public BaseTaskTrans(TransMaster trans)
        {
            _M = trans;
            mTimer = new MTimer();
        }
        public abstract void DoTrans(StockTrans trans);
    }
}
