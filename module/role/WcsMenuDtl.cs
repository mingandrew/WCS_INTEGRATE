namespace module.role
{
    public class WcsMenuDtl
    {
        public int id { set; get; }
        public int menu_id { set; get; }
        public string name { set; get; }
        public bool folder { set; get; }
        public int folder_id { set; get; }
        public int module_id { set; get; }
        public short order { set; get; }
        public bool rf { set; get; }
    }
}
