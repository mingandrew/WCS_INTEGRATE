using System;

namespace tool.appconfig
{
    public class MysqlConfig
    {
        public static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public static readonly string FileName = $"\\MysqlConfig.json";
        public static readonly string SavePath = $"{Path}{FileName}";
        private string SslMode { set; get; } = "None";
        public string Server { set; get; } = "localhost";
        public string Database { set; get; } = "wcs_lsf_yh";
        public string UserName { set; get; } = "root";
        public string Password { set; get; } = "root";

        //"SslMode=None;server=localhost;database=wcs_lsf_yh;uid=root;pwd=root;
        //AllowPublicKeyRetrieval=true";

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        public string MySqlConn()
        {
            return string.Format("SslMode={0};server={1};database={2};" +
                "uid={3};pwd={4};AllowPublicKeyRetrieval=true",SslMode, Server, Database, UserName, Password);
        }
    }
}
