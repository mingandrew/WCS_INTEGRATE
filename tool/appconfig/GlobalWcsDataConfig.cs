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
                if (File.Exists(string.Format(SimulateConfig.SavePath, MysqlConfig.Database)))
                {
                    try
                    {
                        var json = File.ReadAllText(string.Format(SimulateConfig.SavePath, MysqlConfig.Database));
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

            #region[默认配置信息]
            if (File.Exists(DefaultConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(DefaultConfig.SavePath);
                    DefaultConfig = (string.IsNullOrEmpty(json) ? new DefaultConfig() : JsonConvert.DeserializeObject<DefaultConfig>(json)) ?? new DefaultConfig();
                }
                catch
                {
                    DefaultConfig = new DefaultConfig();
                }
            }
            else
            {
                DefaultConfig = new DefaultConfig();
            }
            SaveDefaultConfig();
            #endregion

            #region[大配置]

            if (File.Exists(BigConifg.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(BigConifg.SavePath);
                    BigConifg = (string.IsNullOrEmpty(json) ? new BigConifg() : JsonConvert.DeserializeObject<BigConifg>(json)) ?? new BigConifg();
                }
                catch
                {
                    BigConifg = new BigConifg();
                }
            }
            else
            {
                BigConifg = new BigConifg();
            }
            SaveBigConifg();

            #endregion

            #region[报警灯配置信息]

            if (File.Exists(DevLightConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(DevLightConfig.SavePath);
                    AlertLightConfig = (string.IsNullOrEmpty(json) ? new DevLightConfig() : JsonConvert.DeserializeObject<DevLightConfig>(json)) ?? new DevLightConfig();
                }
                catch
                {
                    AlertLightConfig = new DevLightConfig();
                }
            }
            else
            {
                AlertLightConfig = new DevLightConfig();
            }
            SaveAlertLightConfig();

            #endregion
        }

        public static void SaveMysqlConfig()
        {
            SaveJsonObj(MysqlConfig, MysqlConfig.Path, MysqlConfig.SavePath);
        }

        public static void SaveDebugConfig()
        {
            SaveJsonObj(DebugConfig, DebugConfig.Path, DebugConfig.SavePath);
        }

        public static void SaveSimulateConfig()
        {
            SaveJsonObj(SimulateConfig, SimulateConfig.Path, SimulateConfig.SavePath);
        }
        public static void SaveDefaultConfig()
        {
            SaveJsonObj(DefaultConfig, DefaultConfig.Path, DefaultConfig.SavePath);
        }
        public static void SaveBigConifg()
        {
            SaveJsonObj(BigConifg, BigConifg.Path, BigConifg.SavePath);
        }

        public static void SaveAlertLightConfig()
        {
            SaveJsonObj(AlertLightConfig, DevLightConfig.Path, DevLightConfig.SavePath);
        }

        #region[保存配置文件]
        public static void SaveJsonObj(object obj, string dirpath, string savepath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                using (FileStream fs = new FileStream(savepath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

                    fs.Write(data, 0, data.Length);

                    fs.Close();
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        public static MysqlConfig MysqlConfig { get; set; }
        public static DebugConfig DebugConfig { get; set; }
        public static SimulateConfig SimulateConfig { get; set; }
        public static DefaultConfig DefaultConfig { get; set; }
        public static BigConifg BigConifg { get; set; }
        public static DevLightConfig AlertLightConfig { get; set; }
    }
}
