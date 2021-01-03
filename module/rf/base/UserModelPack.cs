using System.Collections.Generic;

namespace module.rf
{
    public class UserModelPack
    {
        public string UserId { set; get; }
        public string UserName { set; get; }
        public List<ModuleView> UserModuleView { set; get; }

        public void AddModule(List<ModuleView> menus)
        {
            if(UserModuleView == null)
            {
                UserModuleView = new List<ModuleView>();
            }

            UserModuleView.AddRange(menus);
        }
    }
}
