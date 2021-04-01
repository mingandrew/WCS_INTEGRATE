using System;
using System.IO;
using Newtonsoft.Json;

namespace tool.appconfig
{
    public class GlobalWcsDataConfig
    {
        public static void Init()
        {
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
                Save();
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(MysqlConfig);
            if (!Directory.Exists(MysqlConfig.Path))
            {
                Directory.CreateDirectory(MysqlConfig.Path);
            }
            using (FileStream fs = new FileStream(MysqlConfig.SavePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Seek(fs.Length, SeekOrigin.Current);

                byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                fs.Write(data, 0, data.Length);

                fs.Close();
            }
        }

        public static MysqlConfig MysqlConfig { get; set; }
    }
}
