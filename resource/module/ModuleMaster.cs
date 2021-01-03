using resource.module.modulesql;
using tool;
using tool.mlog;

namespace resource.module
{
    public class ModuleMaster
    {
        private readonly MySql mySQL;
        private Log mLog;

        #region[字段]

        public DictionModSql DicSql { set; get; }
        public DeviceModSql DevSql { set; get; }
        public TrackModSql TraSql { set; get; }
        public AreaModSql AreaSql { set; get; }
        public GoodsModSql GoodSql { set; get; }
        public WarnModSql WarnSql { set; get; }
        public TileTrackModSql TileTraSql { set; get; }

        public RoleModSql RoleSql { set; get; }
        #endregion

        #region[构造/初始化]
        public ModuleMaster()
        {
            mLog = (Log)new LogFactory().GetLog("Mods", false);
            mySQL = new MySql(mLog);

            DicSql = new DictionModSql(mySQL);
            DevSql = new DeviceModSql(mySQL);
            TraSql = new TrackModSql(mySQL);
            AreaSql = new AreaModSql(mySQL);
            GoodSql = new GoodsModSql(mySQL);
            WarnSql = new WarnModSql(mySQL);
            TileTraSql = new TileTrackModSql(mySQL);
            RoleSql = new RoleModSql(mySQL);
        }

        public void Start()
        {

        }

        public void Refresh()
        {

        }

        public void Stop()
        {

        }
        #endregion

    }
}
