namespace tool.mlog
{
    public class LogFactory : ILogFactory
    {
        /// <summary>
        /// 创建日志实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILog GetLog(string name)
        {
            return new Log(name);
        }

        public ILog GetLog(string name,bool isconsole)
        {
            return new Log(name, isconsole);
        }
    }
}
