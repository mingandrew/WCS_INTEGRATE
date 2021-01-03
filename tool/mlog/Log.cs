using System;
using System.Text;

namespace tool.mlog
{
    /// <summary>
    /// Console Log
    /// </summary>
    public class Log : ILog
    {
        private string m_Name;

        private const string m_MessageTemplate = "{0}-{1}: {2}";

        private const string m_Debug = "DEBUG";

        private const string m_Error = "ERROR";

        private const string m_Fatal = "FATAL";

        private const string m_Info = "INFO";

        private const string m_Warn = "WARN";

        public const string m_Status = "状态";

        public const string m_ALert = "报警";

        public const string m_Cmd = "指令";

        private bool m_isconsole = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Log(string name)
        {
            m_Name = name;
        }

        public Log(string name, bool iscon)
        {
            m_Name = name;
            m_isconsole = iscon;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is error enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is error enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsErrorEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fatal enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is fatal enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFatalEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is info enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is info enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInfoEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is warn enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public string GetDataTimeLog(string log)
        {
            return string.Format("[{0}]>>{1}", DateTime.Now.ToString("yy-MM-dd HH:mm:ss"), log);
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Debug(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Debug, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(bool isWriteFile, object message, Exception exception)
        {
            string log = GetDataTimeLog(message + Environment.NewLine + exception.Message + exception.StackTrace);
            WriteLine(m_MessageTemplate, m_Name, m_Debug, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void DebugFormat(bool isWriteFile, string format, object arg0)
        {
            string log = GetDataTimeLog(string.Format(format, arg0));
            WriteLine(m_MessageTemplate, m_Name, m_Debug, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(bool isWriteFile, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Debug, string.Format(format, args));
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Debug, string.Format(provider, format, args));
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void DebugFormat(bool isWriteFile, string format, object arg0, object arg1)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1));
            WriteLine(m_MessageTemplate, m_Name, m_Debug, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the debug message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void DebugFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1, arg2));
            WriteLine(m_MessageTemplate, m_Name, m_Debug, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Debug, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Error(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(bool isWriteFile, object message, Exception exception)
        {
            string log = GetDataTimeLog(message + Environment.NewLine + exception.Message + exception.StackTrace);
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void ErrorFormat(bool isWriteFile, string format, object arg0)
        {
            string log = GetDataTimeLog(string.Format(format, arg0));
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(bool isWriteFile, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(provider, format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void ErrorFormat(bool isWriteFile, string format, object arg0, object arg1)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1));
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void ErrorFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg2));
            WriteLine(m_MessageTemplate, m_Name, m_Error, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Error, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Fatal(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(bool isWriteFile, object message, Exception exception)
        {
            string log = GetDataTimeLog(message + Environment.NewLine + exception.Message + exception.StackTrace);
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void FatalFormat(bool isWriteFile, string format, object arg0)
        {
            string log = GetDataTimeLog(string.Format(format, arg0));
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(bool isWriteFile, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(provider, format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void FatalFormat(bool isWriteFile, string format, object arg0, object arg1)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1));
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the fatal error message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void FatalFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1, arg2));
            WriteLine(m_MessageTemplate, m_Name, m_Fatal, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Fatal, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Info(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(bool isWriteFile, object message, Exception exception)
        {
            string log = GetDataTimeLog(message + Environment.NewLine + exception.Message + exception.StackTrace);
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void Info(bool isWriteFile, object message, byte[] data)
        {
            string msg = ByteToString(data);
            Info(isWriteFile, message + msg);
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void InfoFormat(bool isWriteFile, string format, object arg0)
        {
            string log = GetDataTimeLog(string.Format(format, arg0));
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(bool isWriteFile, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(provider, format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void InfoFormat(bool isWriteFile, string format, object arg0, object arg1)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1));
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void InfoFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1, arg2));
            WriteLine(m_MessageTemplate, m_Name, m_Info, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Info, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Warn(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(bool isWriteFile, object message, Exception exception)
        {
            string log = GetDataTimeLog(message + Environment.NewLine + exception.Message + exception.StackTrace);
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public void WarnFormat(bool isWriteFile, string format, object arg0)
        {
            string log = GetDataTimeLog(string.Format(format, arg0));
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(bool isWriteFile, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(bool isWriteFile, IFormatProvider provider, string format, params object[] args)
        {
            string log = GetDataTimeLog(string.Format(provider, format, args));
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public void WarnFormat(bool isWriteFile, string format, object arg0, object arg1)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1));
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }

        /// <summary>
        /// Logs the warning message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public void WarnFormat(bool isWriteFile, string format, object arg0, object arg1, object arg2)
        {
            string log = GetDataTimeLog(string.Format(format, arg0, arg1, arg2));
            WriteLine(m_MessageTemplate, m_Name, m_Warn, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Warn, log);
            }
        }


        /// <summary>
        /// 转化为字符串
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="isReverse"></param>
        /// <returns></returns>
        private string ByteToString(byte[] arr)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                str.Append(arr[i].ToString("X2") + " ");
            }
            return str.ToString();
        }
        private void WriteLine(string format, object arg0, object arg1, object arg2)
        {
#if DEBUG
            if (m_isconsole) Console.WriteLine(format, arg0, arg1, arg2);
#endif
        }

        #region[添加]


        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Status(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Status, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Status, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Alert(bool isWriteFile, object message)
        {
            string log = GetDataTimeLog(message.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_ALert, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_ALert, log);
            }
        }

        /// <summary>
        /// Logs the info message.
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message">The message.</param>
        public void Cmd(bool isWriteFile, object message)
        {
            if (message == null) return;
            string log = GetDataTimeLog(message?.ToString());
            WriteLine(m_MessageTemplate, m_Name, m_Cmd, log);
            if (isWriteFile)
            {
                LogUtil.WriteLogFile(m_Name, m_Cmd, log);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isWriteFile"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void Cmd(bool isWriteFile, object message, byte[] data)
        {
            string msg = ByteToString(data);
            if (oldcmd.Equals(msg)) return;
            oldcmd = msg;
            Cmd(isWriteFile, message + msg);
        }
        private string oldcmd = "";
        #endregion
    }
}
