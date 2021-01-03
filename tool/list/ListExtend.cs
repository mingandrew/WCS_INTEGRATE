using System.Collections.Generic;

namespace tool.list
{
    public static class ListExtend
    {
        /// <summary>
        /// DataTable转成List
        /// </summary>
        public static List<T> CloneList<T>(this List<T> orglist)
        {
            List<T> list = new List<T>();
            foreach (var item in orglist)
            {
                if(item is IClone clone)
                {
                    list.Add((T)clone.Clone());
                }
            }
            return list;
        }
    }
}
