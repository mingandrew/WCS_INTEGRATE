using enums;
using resource;

namespace task.trans.diagnose
{
    /// <summary>
    /// 分析任务管理
    /// </summary>
    public class DiagnoseServer
    {
        #region[分析]
        SortTaskDiagnose SortDiagnose;
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="trans"></param>
        public DiagnoseServer(TransMaster trans)
        {
            SortDiagnose = new SortTaskDiagnose(trans);
        }

        /// <summary>
        /// 执行分析
        /// </summary>
        public void Diagnose()
        {
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.EnableDiagnose)) return;

            try
            {
                SortDiagnose?.Diagnose();
            }
            catch { }
        }
    }
}
