using module.role;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class RoleModSql : BaseModSql
    {
        public RoleModSql(MySql mysql) : base(mysql)
        {

        }

        #region[查询]


        public List<WcsModule> QueryWcsModuleList()
        {
            List<WcsModule> list = new List<WcsModule>();
            string sql = string.Format("SELECT t.id, t.`name`, t.type, t.`key`, t.entity, t.brush," +
                " t.geometry, t.winctlname, t.memo FROM wcs_module AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<WcsModule>();
            }
            return list;
        }

        public List<WcsMenu> QueryWcsMenuList()
        {
            List<WcsMenu> list = new List<WcsMenu>();
            string sql = string.Format("SELECT t.id, t.`name`, t.prior, t.memo FROM wcs_menu AS t ORDER BY t.prior ASC ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<WcsMenu>();
            }
            return list;
        }

        public List<WcsMenuDtl> QueryWcsMenuDtlList()
        {
            List<WcsMenuDtl> list = new List<WcsMenuDtl>();
            string sql = string.Format("SELECT t.id, t.menu_id, t.`name`, t.folder, t.folder_id, t.module_id, " +
                "t.`order`, t.`rf` FROM wcs_menu_dtl AS t ORDER BY t.`order` ASC ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<WcsMenuDtl>();
            }
            return list;
        }
        
        public List<WcsRole> QueryWcsRoleList()
        {
            List<WcsRole> list = new List<WcsRole>();
            string sql = string.Format("SELECT t.id, t.`name`, t.admin, t.menu_id, t.prior FROM wcs_role AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<WcsRole>();
            }
            return list;
        }
        
        public List<WcsUser> QueryWcsUserList()
        {
            List<WcsUser> list = new List<WcsUser>();
            string sql = string.Format("SELECT t.id, t.username, t.`password`, t.`name`," +
                " t.memo, t.role_id, t.exitwcs, t.guest FROM wcs_user AS t ");
            DataTable dt = mSql.ExecuteQuery(@sql);
            if (!mSql.IsNoData(dt))
            {
                list = dt.ToDataList<WcsUser>();
            }
            return list;
        }


        #endregion

        #region[添加]

        internal bool AddWcsUser(WcsUser user)
        {
            string str = "INSERT INTO `wcs_user`(`id`, `username`, `password`, `name`, `memo`, `role_id`, `exitwcs`)"+
                " VALUES ({0}, '{1}', '{2}', '{3}', '{4}', {5}, {6})";
            string sql = string.Format(@str, user.id, user.username, user.password, user.name, user.memo, user.role_id, user.exitwcs);
            int row = mSql.ExcuteSql(sql);
            return row > 0;
        }

        internal bool AddWcsMenu(WcsMenu menu)
        {
            string str = "INSERT INTO `wcs_menu`(`id`, `name`, `prior`, `memo`) VALUES ({0}, '{1}', {2}, '{3}')";
            string sql = string.Format(@str, menu.id, menu.name, menu.prior, menu.memo);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal void AddWcsMenuDtl(WcsMenuDtl dtl)
        {
            string str = "INSERT INTO `wcs_menu_dtl`(`id`, `menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`)" +
                " VALUES ({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7})";
            string sql = string.Format(str, dtl.id, dtl.menu_id, dtl.name,
                dtl.folder, dtl.folder_id, dtl.module_id, dtl.order, dtl.rf);
            int row = mSql.ExcuteSql(sql);
        }

        #endregion

        #region[修改]


        internal bool EditeWcsUser(WcsUser user)
        {
            string sql = "UPDATE `wcs_user` SET `username` = '{0}', `password` = '{1}', " +
                "`name` = '{2}', `memo` = '{3}', `role_id` = {4}, `exitwcs` = {5}  WHERE `id` = {6}";
            sql = string.Format(sql, user.username, user.password, user.name, user.memo, user.role_id, user.exitwcs, user.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditWcsMenu(WcsMenu menu)
        {
            string sql = "UPDATE `goods` SET `name` = '{0}'  WHERE `id` = '{1}'";
            sql = string.Format(sql);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool EditeWcsMenuDtl(WcsMenuDtl dtl)
        {
            string sql = "UPDATE `wcs_menu_dtl` SET `menu_id` = {0}, `name` = '{1}'," +
                " `folder` = {2}, `folder_id` = {3}, `module_id` = {4}, `order` = {5}, `rf` = {6} WHERE `id` = {7}";
            sql = string.Format(sql, dtl.menu_id, dtl.name, dtl.folder, dtl.folder_id, dtl.module_id, dtl.order, dtl.rf, dtl.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion

        #region[删除]

        internal bool DeleteWcsMenu(WcsMenu menu)
        {
            string sql = string.Format("DELETE FROM wcs_menu where id = '{0}'", menu.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }
        internal bool DeleteWcsMenuDtlsByFolderId(int folderid)
        {
            string sql = string.Format("DELETE FROM wcs_menu_dtl where folder_id = '{0}'", folderid);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        internal bool DeleteWcsMenuDtl(WcsMenuDtl dtl)
        {
            string sql = string.Format("DELETE FROM wcs_menu_dtl where id = '{0}'", dtl.id);
            int row = mSql.ExcuteSql(sql);
            return row >= 1;
        }

        #endregion
    }
}
