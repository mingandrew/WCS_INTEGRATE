using Microsoft.Win32;
using module.area;
using module.line;
using System;
using System.Collections.Generic;
using System.Data;
using tool;
using tool.mysql.extend;

namespace resource.module.modulesql
{
    public class ExcelConfigModSql : BaseModSql
    {
        public ExcelConfigModSql(MySql sql) : base(sql)
        {
        }


        #region[导出]

        public void SaveToExcel(SaveFileDialog saveFileDialog, string sql)
        {
            DataTable dt = mSql.ExecuteQuery(@sql);
            DataTableExtend.SaveToExcel(saveFileDialog, dt);
        }


        public void SaveToExcel<T>(SaveFileDialog saveFileDialog, List<T> TList)
        {
            DataTable dt = DataTableExtend.ToDataTable(TList);
            DataTableExtend.SaveToExcel(saveFileDialog, dt);
        }
        #endregion
    }
}
