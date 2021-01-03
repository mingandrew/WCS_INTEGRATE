using enums;

namespace module.role
{
    public class WcsModule
    {
        public int id { set; get; }
        public string name { set; get; }
        public byte type { set; get; }
        public string key { set; get; }
        public string entity { set; get; }
        public string brush { set; get; }
        public string geometry { set; get; }
        public string winctlname { set; get; }
        public string memo { set; get; }

        public WcsModuleTypeE ModuleType
        {
            get => (WcsModuleTypeE)type;
            set => type = (byte)value;
        }
    }
}
