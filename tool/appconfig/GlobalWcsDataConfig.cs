using System;
using System.IO;
using Newtonsoft.Json;

namespace tool.appconfig
{
    public class GlobalWcsDataConfig
    {
        public static void Init()
        {
            #region[数据库配置文件读取/初始化]
            if (File.Exists(MysqlConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(MysqlConfig.SavePath);
                    MysqlConfig = (string.IsNullOrEmpty(json) ? new MysqlConfig() : JsonConvert.DeserializeObject<MysqlConfig>(json)) ?? new MysqlConfig();
                }
                catch
                {
                    MysqlConfig = new MysqlConfig();
                }
            }
            else
            {
                MysqlConfig = new MysqlConfig();
            }
            SaveMysqlConfig();
            #endregion

            #region[测试配置文件读取/初始化]
            if (File.Exists(DebugConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(DebugConfig.SavePath);
                    DebugConfig = (string.IsNullOrEmpty(json) ? new DebugConfig() : JsonConvert.DeserializeObject<DebugConfig>(json)) ?? new DebugConfig();
                }
                catch
                {
                    DebugConfig = new DebugConfig();
                }
            }
            else
            {
                DebugConfig = new DebugConfig();
            }
            SaveDebugConfig();
            #endregion
        }

        public static void SaveMysqlConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(MysqlConfig);
                if (!Directory.Exists(MysqlConfig.Path))
                {
                    Directory.CreateDirectory(MysqlConfig.Path);
                }
                using (FileStream fs = new FileStream(MysqlConfig.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }catch(Exception e)
            {

            }
            
        }

        public static void SaveDebugConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(DebugConfig);
                if (!Directory.Exists(DebugConfig.Path))
                {
                    Directory.CreateDirectory(DebugConfig.Path);
                }
                using (FileStream fs = new FileStream(DebugConfig.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }catch(Exception e)
            {

            }
            
        }

        public static MysqlConfig MysqlConfig { get; set; }
        public static DebugConfig DebugConfig { get; set; }
    }
}
