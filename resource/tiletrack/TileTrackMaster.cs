using enums;
using module.area;
using module.tiletrack;
using System.Collections.Generic;

namespace resource.tiletrack
{
    public class TileTrackMaster
    {
        #region[构造函数]
        private List<TileTrack> List { set; get; }
        public TileTrackMaster()
        {
            List = new List<TileTrack>();
        }


        public void Start()
        {
            Refresh();
        }

        public void Refresh(bool refr_1 = true)
        {
            if (refr_1)
            {
                List.Clear();
                List.AddRange(PubMaster.Mod.TileTraSql.QueryTileTrackList());
            }
        }

        public void Stop()
        {

        }

        #endregion

        #region[砖机轨道信息]

        public List<TileTrack> GetTileTracks(uint tileid)
        {
            return List.FindAll(c => c.tile_id == tileid);
        }

        /// <summary>
        /// 添加砖机轨道信息
        /// </summary>
        /// <param name="tileid"></param>
        public void AddTileTrack(uint tileid)
        {
            if (List.Exists(c => c.tile_id == tileid)) return;
            List<AreaDeviceTrack> list = PubMaster.Area.GetDevTrackList(tileid);
            byte order = 1;
            foreach (AreaDeviceTrack track in list)
            {
                TileTrack tile = new TileTrack()
                {
                    tile_id = tileid,
                    track_id = track.track_id,
                    order = order++,
                };
                PubMaster.Mod.TileTraSql.AddTileTrack(tile);
                List.Add(tile);
            }
        }

        /// <summary>
        /// 保存砖机轨道信息
        /// </summary>
        /// <param name="leftTrackList"></param>
        /// <param name="rightTrackList"></param>
        public void EditTileTrack(TileTrack tiletrack, byte order)
        {
            TileTrack tile = List.Find(c => c.id == tiletrack.id && c.tile_id == tiletrack.tile_id && c.track_id == tiletrack.track_id);
            if (tile != null)
            {
                tile.order = order;
                PubMaster.Mod.TileTraSql.EditTileTrack(tile);
            }
            else
            {
                tiletrack.id = PubMaster.Dic.GenerateID(DicTag.NewTileTrackId);
                tiletrack.order = order;
                List.Add(tiletrack);
                PubMaster.Mod.TileTraSql.AddTileTrack(tiletrack);
            }
        }

        public void DeleteTileTrack(TileTrack tiletrack)
        {
            TileTrack tile = List.Find(c => c.id == tiletrack.id && c.tile_id == tiletrack.tile_id && c.track_id == tiletrack.track_id);
            if (tile != null)
            {
                List.Remove(tile);
                PubMaster.Mod.TileTraSql.DeleteTileTrack(tile);
            }
        }

        /// <summary>
        /// 砖机品种排序更新
        /// </summary>
        public void SortTileTrackList()
        {
            List.Sort((x, y) =>
            {
                if (x.tile_id == y.tile_id)
                {
                    return x.order.CompareTo(y.order);
                }
                return x.tile_id.CompareTo(y.tile_id);
            });
        }

        /// <summary>
        /// 获取砖机配置的品种进行出库
        /// </summary>
        /// <param name="tileid"></param>
        /// <returns></returns>
        public List<TileTrack> GetTileTrack2Out(uint tileid)
        {
            List<TileTrack> list = new List<TileTrack>();
            list.AddRange(List.FindAll(c => c.tile_id == tileid));
            if (list.Count > 0)
            {
                list.Sort((x, y) =>
                {
                    if (x.tile_id == y.tile_id)
                    {
                        return x.order.CompareTo(y.order);
                    }
                    return x.tile_id.CompareTo(y.tile_id);
                });
            }
            return list;
        }

        public void DeleteTileTrack(List<TileTrack> delelist)
        {
            foreach (TileTrack item in delelist)
            {
                TileTrack tile = List.Find(c => c.id == item.id && c.tile_id == item.tile_id && c.track_id == item.track_id);
                if (tile != null)
                {
                    List.Remove(tile);
                    PubMaster.Mod.TileTraSql.DeleteTileTrack(tile);
                }
            }
        }

        #endregion


    }
}
