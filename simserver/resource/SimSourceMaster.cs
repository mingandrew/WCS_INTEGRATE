using enums.track;
using module.track;
using resource;
using System.Collections.Generic;

namespace simtask.resource
{
    public class SimSourceMaster
    {
        #region[构造/初始化]

        public SimSourceMaster()
        {
            FerryPosList = new List<SimFerryPos>();
        }

        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true)
        {
            if (refr_1)
            {
                FerryPosList.Clear();
                FerryPosList.AddRange(PubMaster.Mod.TraSql.QuerySimFerryPosList());
            }
        }

        public void Stop()
        {

        }
        #endregion

        #region[字段]

        private List<Track> TrackList { set; get; }
        private List<SimFerryPos> FerryPosList { set; get; }

        #endregion

        #region[获取对象]

        public List<Track> GetTrackList()
        {
            return TrackList;
        }

        public Track GetTrack(uint trackid)
        {
            return TrackList.Find(c => c.id == trackid);
        }



        public Track GetTrackByCode(ushort trackcode)
        {
            return TrackList.Find(c => c.IsInTrack(trackcode));
        }

        #endregion

        #region[获取属性]

        public bool IsTargetFront(ushort targetSite, int nowPos)
        {
            SimFerryPos tpos = FerryPosList.Find(c => c.ferry_code == targetSite);
            if(tpos!=null)
            {
                return tpos.ferry_pos > nowPos;
            }
            return false;
        }


        /// <summary>
        /// 摆渡车当前是否到达点位
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="nowPos"></param>
        /// <param name="type"></param>
        /// <param name="upferrypose"></param>
        /// <returns></returns>
        public bool IsOnFerryPos(uint area_id, int nowPos, bool isdownferry, out SimFerryPos upferrypose, out SimFerryPos downferrypos)
        {
            List<SimFerryPos> list = FerryPosList.FindAll(c => c.area_id == area_id && c.isdownferry == isdownferry);
            list.Sort((x, y) => x.ferry_pos.CompareTo(y.ferry_pos));
            upferrypose = null;
            downferrypos = null;
            foreach (SimFerryPos pos in list)
            {
                if(nowPos == pos.ferry_pos)
                {
                    if (pos.isdownside)
                    {
                        downferrypos = pos;
                    }
                    else
                    {
                        upferrypose = pos;
                    }
                }
            }
            return upferrypose!=null || downferrypos !=null;
        }

        #endregion
    }
}