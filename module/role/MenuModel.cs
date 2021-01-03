using System.Collections.Generic;

namespace module.role
{
    public class MenuModel
    {
        public int Id { set; get; }
        public int FolderId { set; get; }
        public int ModuleId { set; get; }
        public int MenuId { set; get; }//菜单总表ID
        public string Name { set; get; }
        public string IKey { set; get; }
        public bool OpenPage { set; get; }
        public bool Rf { set; get; }

        public List<MenuModel> MenuList { get; set; }

        public bool ExistDtl(string ikey)
        {
            if (MenuList == null) return false;
            return MenuList.Exists(c => ikey.Equals(c.IKey));
        }


        public void AddMenu(MenuModel dtl)
        {
            if(MenuList == null)
            {
                MenuList = new List<MenuModel>();
            }

            if (dtl == null) return;

            MenuList.Add(dtl);
        }

        public bool MoveUp(MenuModel md, out string result)
        {
            if (MenuList == null || MenuList.Count <= 0)
            {
                result = "找不到对应的模块信息";
                return false;
            }
            int idx = MenuList.IndexOf(md);
            if(idx <= 0)
            {
                result = "已经在顶部！";
                return false;
            }
            MenuModel tmp = md;
            MenuList.RemoveAt(idx);
            MenuList.Insert(idx - 1, tmp);
            result = "";
            return true;
        }

        public bool MoveDown(MenuModel md, out string result)
        {
            if (MenuList == null || MenuList.Count <= 0)
            {
                result = "找不到对应的模块信息";
                return false;
            }
            int idx = MenuList.IndexOf(md);
            if(idx < 0 || idx >= MenuList.Count-1)
            {
                result = "已经在底部！";
                return false;
            }
            MenuModel tmp = md;
            MenuList.RemoveAt(idx);
            MenuList.Insert(idx + 1, tmp);
            result = "";
            return true;
        }

        public bool DeleteDtl(MenuModel md, out string result)
        {
            if (MenuList == null || MenuList.Count <= 0)
            {
                result = "找不到对应的模块信息";
                return false;
            }
            int idx = MenuList.IndexOf(md);
            if (idx == - 1)
            {
                result = "找不到功能信息！";
                return false;
            }
            MenuList.RemoveAt(idx);
            result = "";
            return true;
        }

        public bool UpdateDtl(MenuModel md, out string result)
        {
            if (MenuList == null || MenuList.Count <= 0)
            {
                result = "找不到对应的模块信息";
                return false;
            }
            int idx = MenuList.IndexOf(md);
            if (idx == -1)
            {
                result = "找不到功能信息！";
                return false;
            }
            MenuModel tmp = md;
            MenuList.RemoveAt(idx);
            MenuList.Insert(idx, tmp);
            result = "";
            return true;
        }
    }
}
