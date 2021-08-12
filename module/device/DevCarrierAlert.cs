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

        public void ResetAler1()
        {
            Warning[0, 0] = false;
            Warning[0, 1] = false;
            Warning[0, 2] = false;
            Warning[0, 3] = false;
            Warning[0, 4] = false;
            Warning[0, 5] = false;
            Warning[0, 6] = false;
        }

        public void ResetAler2()
        {
            Warning[1, 0] = false;
            Warning[1, 1] = false;
            Warning[1, 2] = false;
            Warning[1, 3] = false;
            Warning[1, 4] = false;
            Warning[1, 5] = false;
            Warning[1, 6] = false;
        }

        public void ResetAler3()
        {
            Warning[1, 0] = false;
            Warning[1, 1] = false;
            Warning[1, 2] = false;
            Warning[1, 3] = false;
            Warning[1, 4] = false;
            Warning[1, 5] = false;
            Warning[1, 6] = false;
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
