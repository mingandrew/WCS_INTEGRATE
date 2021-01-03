namespace wcs.Data.Model
{
    public class WinCtlModel
    {
        public int Id { set; get; }
        public string Key { set; get; }
        public string Name { set; get; }
        public string Geometry { set; get; }
        public string Brush { set; get; }

        public string WinCtlName { set; get; }

        public override string ToString()
        {
            return string.Format("Key:{0},Name:{1},WinCtlName:{2}", Key, Name, WinCtlName);
        }
    }
}
