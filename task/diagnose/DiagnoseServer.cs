using enums;
using resource;
using System;
using task.diagnose.trans;
using task.trans;

namespace task.diagnose
{
    /// <summary>
    /// 分析任务管理
    /// </summary>
    public class DiagnoseServer
    {
        #region[分析]
        SortTaskDiagnose SortDiagnose;
        MoveCarDiagnose MoveDiagnose;
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="trans"></param>
        public DiagnoseServer(TransMaster trans)
        {
            SortDiagnose = new SortTaskDiagnose(trans);
            MoveDiagnose = new MoveCarDiagnose(trans);
        }

        /// <summary>
        /// 分析服务不用频繁执行（3s 检查一次）
        /// 1:过
        /// 2:过
        /// 3:检查
        /// </summary>
        private byte CountRun = 1;
        /// <summary>
        /// 执行分析
        /// </summary>
        public void Diagnose()
        {
            if (!PubMaster.Dic.IsSwitchOnOff(DicTag.EnableDiagnose)) return;

            if(CountRun < 3)
            {
                CountRun++;
                return;
            }
            CountRun = 1;
            try
            {
                //分析倒库：暂停倒库，恢复倒库
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableSortDiagnose)) SortDiagnose?.Diagnose();
                //分析分配运输车：移车任务
                if (PubMaster.Dic.IsSwitchOnOff(DicTag.EnableMoveCarDiagnose))  MoveDiagnose?.Diagnose();

            }
            catch { }
        }
    }
}
