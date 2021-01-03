namespace module.window
{
    public class DialogResult
    {
        public object p1 { set; get; }
        public object p2 { set; get; }
        public object p3 { set; get; }
        public object p4 { set; get; }
        public object p5 { set; get; }
        public object p6 { set; get; }
        public object p7 { set; get; }
        public object p8 { set; get; }
        public object p9 { set; get; }
        public object p10 { set; get; }

        public DialogResult() { }
        public DialogResult(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null,
            object o7 = null, object o8 = null, object o9 = null, object o10 = null)
        {
            p1 = o1;
            p2 = o2;
            p3 = o3;
            p4 = o4;
            p5 = o5;
            p6 = o6;
            p7 = o7;
            p8 = o8;
            p9 = o9;
            p10 = o10;
        }
    }
}
