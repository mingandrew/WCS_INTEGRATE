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


            #region[模拟系统设备信息]
            if (DebugConfig.IsDebug)
            {
                if (File.Exists(SimulateConfig.SavePath))
                {
                    try
                    {
                        var json = File.ReadAllText(SimulateConfig.SavePath);
                        SimulateConfig = (string.IsNullOrEmpty(json) ? new SimulateConfig() : JsonConvert.DeserializeObject<SimulateConfig>(json)) ?? new SimulateConfig();
                    }
                    catch
                    {
                        SimulateConfig = new SimulateConfig();
                    }
                }
                else
                {
                    SimulateConfig = new SimulateConfig();
                }
                SaveSimulateConfig();
            }
            else
            {
                SimulateConfig = new SimulateConfig();
            }
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
            }catch(Exception)
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
            }catch(Exception)
            {

            }
        }

        public static void SaveSimulateConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(SimulateConfig);
                if (!Directory.Exists(SimulateConfig.Path))
                {
                    Directory.CreateDirectory(SimulateConfig.Path);
                }
                using (FileStream fs = new FileStream(SimulateConfig.SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }
            catch (Exception e)
            {

            }
        }

        public static MysqlConfig MysqlConfig { get; set; }
        public static DebugConfig DebugConfig { get; set; }
        public static SimulateConfig SimulateConfig { get; set; }
    }
}
