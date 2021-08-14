using module.track;
using resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using task.trans;
using tool.mlog;

namespace task.diagnose.trans
{
    public class TrackDiagnose : TransBaseDiagnose
    {
        public TrackDiagnose(TransMaster master) : base(master)
        {
            _mLog = (Log)new LogFactory().GetLog("轨道分析", false);
        }
        bool dodiagnose;
        public override void Diagnose()
        {
            //dodiagnose = !dodiagnose;
            //if (dodiagnose) return;
            List<uint> transtrack = _M.GetTransTrackIds();
            PubMaster.Track.DoSortTrackDiagnose(transtrack);
        }
    }
}
