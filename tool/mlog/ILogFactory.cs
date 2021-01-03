namespace tool.mlog
{
    public interface ILogFactory
    {
        ILog GetLog(string name);
    }
}
