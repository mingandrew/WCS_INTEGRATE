using System;

namespace tool.timer
{

    /// <summary>
    /// 计算
    /// </summary>
    public class TimerMod
    {
        public TimerMod()
        {
            Time = DateTime.Now;
            LastCheckTime = DateTime.Now;
        }
        public int Id { set; get; }
        /// <summary>
        /// 时间标识
        /// </summary>
        public string Tag { set; get; }
        /// <summary>
        /// 开始检查的时间
        /// </summary>
        public DateTime Time { set; get; }
        /// <summary>
        /// 最近一个检测时间，超过2秒未检查则重新计算
        /// </summary>
        private DateTime LastCheckTime { set; get; }

        /// <summary>
        /// 查看是否超时
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool IsOverTime(int second, int overtime)
        {
            //距离上一次刷新时间超过2秒则重新计算
            if ((DateTime.Now - LastCheckTime).TotalSeconds > overtime)
            {
                Time = DateTime.Now;
            }
            LastCheckTime = DateTime.Now;
            //Console.WriteLine("{0}, Name:{1}, second:{2}, result:{3}", DateTime.Now.ToLocalTime(), Tag, second, (DateTime.Now - Time).TotalSeconds > second);
            return (DateTime.Now - Time).TotalSeconds > second;
        }

        public bool IsOverTime(int second)
        {
            return (DateTime.Now - Time).TotalSeconds > second;
        }

        internal void Reset()
        {
            Time = DateTime.Now;
        }
    }
}
