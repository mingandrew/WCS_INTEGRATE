using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace module.device
{
    /// <summary>
    /// 报警状态记录
    /// </summary>
    public class DevCarrierAlert
    {
        public bool[,] Warning { set; get; }
        public DevCarrierAlert()
        {
            Warning = new bool[10,8];
        }

        public void RetEmptyWarn(int v1)
        {
            if (v1 < 0 || v1 > 9) return;
            Warning[v1, 0] = false ;
            Warning[v1, 1] = false ;
            Warning[v1, 2] = false ;
            Warning[v1, 3] = false ;
            Warning[v1, 4] = false ;
            Warning[v1, 5] = false ;
            Warning[v1, 6] = false ;
            Warning[v1, 7] = false ;
        }

        public bool GetWarn(int v1, int v2)
        {
            if (v1 < 0 || v1 > 9 || v2 < 0 || v2 > 7) return false;
            return Warning[v1, v2];
        }

        public void SwitchAlert(int v1, int v2)
        {
            if (v1 < 0 || v1 > 9 || v2 < 0 || v2 > 7) return;
            bool v = Warning[v1, v2];
            Warning[v1, v2] = !v;
        }

        public void SetAlert(int v1, int v2, bool value)
        {
            if (v1 < 0 || v1 > 9 || v2 < 0 || v2 > 7) return;
            Warning[v1, v2] = value;
        }

        /// <summary>
        /// 获取对应位的值
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public byte GetWarnByte(int idx)
        {
            byte value = 0;
            if (Warning[idx, 0]) value += 1;
            if (Warning[idx, 1]) value += 2;
            if (Warning[idx, 2]) value += 4;
            if (Warning[idx, 3]) value += 8;
            if (Warning[idx, 4]) value += 16;
            if (Warning[idx, 5]) value += 32;
            if (Warning[idx, 6]) value += 64;
            if (Warning[idx, 7]) value += 128;
            return value;
        }
    }
}
