using System.Collections.Generic;

namespace tool.timer
{

    /// <summary>
    /// 计算时间使用
    /// </summary>
    public class MTimer
    {
        #region[属性]

        private List<TimerMod> TimeList;

        #endregion

        public MTimer()
        {
            TimeList = new List<TimerMod>();
        }

        /// <summary>
        /// 检查是否达到超时条件
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool IsOver(string tag, int second, int overtime)
        {
            TimerMod md = TimeList.Find(c => tag.Equals(c.Tag));
            if (md == null)
            {
                md = new TimerMod()
                {
                    Tag = tag
                };
                TimeList.Add(md);
            }

            return md.IsOverTime(second, overtime);
        }

        public bool IsOver(int id, int second, int overtime)
        {
            TimerMod md = TimeList.Find(c => c.Id == id);
            if (md == null)
            {
                md = new TimerMod()
                {
                    Id = id
                };
                TimeList.Add(md);
            }

            return md.IsOverTime(second, overtime);
        }

        public bool IsTimeUp(string tag, int second)
        {
            TimerMod md = TimeList.Find(c => tag.Equals(c.Tag));
            if (md == null)
            {
                md = new TimerMod()
                {
                    Tag = tag
                };
                TimeList.Add(md);
            }

            return md.IsOverTime(second, second*2);
        }

        private void Reset(string tag)
        {
            TimeList.Find(c => c.Tag.Equals(tag))?.Reset();
        }

        public bool IsOver(string tag, uint id, int second, int overtime)
        {
            return IsOver(tag + id, second, overtime);
        }

        public bool IsOver(string tag, uint id, byte id2, int second, int overtime)
        {
            return IsOver(tag + id + id2, second, overtime);
        }

        public bool IsTimeOutAndReset(string tag, uint id, int second)
        {
            if (IsOver(tag + id, second, second * 3))
            {
                Reset(tag + id);
                return true;
            }
            return false;
        }

    }
}
