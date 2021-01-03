using System;

namespace tool.mlog
{
    internal class LogUtil
    {
        /// <summary>
        /// 格式式化Log信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="name"></param>
        /// <param name="logType"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private static string GetLogString(string name, string logType, string log)
        {
            return String.Format("[{0}]{1}-{2}: {3}", DateTime.Now.ToString("HH:mm:ss"), name, logType, log);
        }

        /// <summary>
        /// 获得日志要保存的路径
        /// </summary>
        /// <param name="name"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        private static string GetLogPath(string name, string logType)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            path += @"/" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            return System.IO.Path.Combine(path, string.Format("{0}_{1}.log", name, logType));
        }

        public static void WriteLogFile(string name, string logType, string log)
        {
            string logPath = GetLogPath(name, logType);

            FileUtil.WriteAppend(logPath, log);
        }
    }
}
