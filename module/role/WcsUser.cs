namespace module.role
{
    public class WcsUser
    {
        public int id { set; get; }
        public string username { set; get; }
        public string password { set; get; }
        public string name { set; get; }
        public string memo { set; get; }
        public int role_id { set; get; }
        public bool exitwcs { set; get; }
        public bool guest { set; get; }
    }
}
