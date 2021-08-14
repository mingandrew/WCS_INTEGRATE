namespace wcs.Data.Model
{
    public class WcsComboBoxItem
    {
        public uint Id { set; get; }
        public string Name { set; get; }

        public WcsComboBoxItem()
        {

        }

        public WcsComboBoxItem(uint id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
