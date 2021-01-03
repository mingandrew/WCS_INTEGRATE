
namespace module.device
{
    public class IDevice
    {
        public int ID { set; get; }

        public bool IsUpdate { set; get; }

        public bool IsAlertUpdate { set; get; }

        protected void Set(ref bool obj, bool value)
        {
            if(obj != value)
            {
                IsUpdate = true;
                obj = value;
            }
        }

        protected void Set(ref int obj, int value)
        {
            if(obj != value && !obj.Equals(value))
            {
                IsUpdate = true;
                obj = value;
            }
        }

        protected void Set(ref byte obj, byte value)
        {
            if(obj != value && !obj.Equals(value))
            {
                IsUpdate = true;
                obj = value;
            }
        }

        protected bool Set(ref ushort obj, ushort value)
        {
            if(obj != value && !obj.Equals(value))
            {
                IsUpdate = true;
                obj = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 报警值更新
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        protected void SetAlert(ref byte obj, byte value)
        {
            if (obj != value && !obj.Equals(value))
            {
                IsUpdate = true;
                IsAlertUpdate = true;
                obj = value;
            }
        }

        public void ReSetUpdate()
        {
            IsUpdate = false;
            IsAlertUpdate = false;
        }
    }
}
