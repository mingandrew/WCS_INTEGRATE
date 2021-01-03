using enums;
using module.rf;
using module.role;
using System.Collections.Generic;
using System.Linq;

namespace resource.role
{
    /// <summary>
    /// 规则更新
    /// </summary>
    public class RoleMaster
    {

        #region[字段]

        private List<WcsUser> UserList { set; get; }
        private List<WcsMenu> MenuList { set; get; }
        private List<WcsMenuDtl> MenuDtlList { set; get; }
        private List<WcsModule> ModuleList { set; get; }
        private List<WcsRole> RoleList { set; get; }

        private int MenuDtlNexId = -1;

        private WcsUser LoginUser;

        #endregion

        #region[属性]

        #endregion

        #region[构造/启动/停止/重连]

        public RoleMaster()
        {
            UserList = new List<WcsUser>();
            MenuList = new List<WcsMenu>();
            MenuDtlList = new List<WcsMenuDtl>();
            ModuleList = new List<WcsModule>();
            RoleList = new List<WcsRole>();
        }

        public void Start()
        {
            Refresh(true, true, true, true, true);
        }

        public void RefreshMenu()
        {
            Refresh(false, false, true, false, false);
        }

        private void Refresh(bool rmodule, bool rmenu, bool rmenudtl, bool rrole, bool ruser)
        {
            if (rmodule)
            {
                ModuleList.Clear();
                ModuleList.AddRange(PubMaster.Mod.RoleSql.QueryWcsModuleList());
            }

            if (rmenu)
            {
                MenuList.Clear();
                MenuList.AddRange(PubMaster.Mod.RoleSql.QueryWcsMenuList());
            }

            if (rmenudtl)
            {
                MenuDtlList.Clear();
                MenuDtlList.AddRange(PubMaster.Mod.RoleSql.QueryWcsMenuDtlList());
            }

            if (rrole)
            {
                RoleList.Clear();
                RoleList.AddRange(PubMaster.Mod.RoleSql.QueryWcsRoleList());
            }

            if (ruser)
            {
                UserList.Clear();
                UserList.AddRange(PubMaster.Mod.RoleSql.QueryWcsUserList());
            }
        }


        public void Stop()
        {

        }

        #endregion

        #region[用户管理]

        public bool IsUserMatch(string username, string password)
        {
            return UserList.Exists(c => username.Equals(c.username) && password.Equals(c.password));
        }

        public WcsUser GetUser(int id)
        {
            return UserList.Find(c => c.id == id);
        }

        public WcsUser GetUser(string username, string password)
        {
            return UserList.Find(c => username.Equals(c.username) && password.Equals(c.password));
        }

        private WcsRole GetUserRole(string username, string password)
        {
            if(IsUserMatch(username, password))
            {
                int roleid = UserList.Find(c => c.username.Equals(username))?.role_id ?? 0;
                return GetRole(roleid);
            }

            return null;
        }

        /// <summary>
        /// 获取默认的用户账号
        /// </summary>
        /// <returns></returns>
        public WcsUser GetGuestUser()
        {
            return UserList.Find(c => c.guest);
        }

        public WcsUser GetLoginUser()
        {
            return LoginUser;
        }

        public void SetLoginUser(int id)
        {
            LoginUser = GetUser(id);
        }

        /// <summary>
        /// 获取当前低于当前角色的用户列表
        /// </summary>
        /// <returns></returns>
        public List<WcsUser> GetUserList()
        {
            List<WcsUser> list = new List<WcsUser>();
            if (LoginUser == null)
            {
                return list;
            }
            List<WcsRole> controlrole = GetBelowLoginRoleList();
            return UserList.FindAll(c => c.id == LoginUser.id || controlrole.Exists(l => l.id == c.role_id));
        }

        public bool IsUserBelowLoginPrior(WcsUser user, out string result)
        {
            if(LoginUser == null)
            {
                result = "请先登陆！";
                return false;
            }

            if(LoginUser.id == user.id)
            {
                result = "";
                return true;
            }

            if(user == null)
            {
                result = "用户信息不能为空！";
                return false;
            }

            WcsRole loginuserrole = GetRole(LoginUser.role_id);
            WcsRole camparerole = GetRole(user.role_id);
            if(loginuserrole != null && camparerole != null)
            {
                if(loginuserrole.prior > camparerole.prior)
                {
                    result = "";
                    return true;
                }
                result = "没有权限修改该用户信息";
                return false;
            }
            result = "用户未配置角色!";
            return false;
        }


        public bool AddWcsUser(WcsUser user, out string result)
        {
            if (UserList.Exists(c => user.username.Equals(c.username)))
            {
                result = "已经存在该用户名！";
                return false;
            }

            int id = UserList.Max(c => c.id);
            user.id = ++id;
            result = "";
            UserList.Add(user);
            return PubMaster.Mod.RoleSql.AddWcsUser(user);
        }

        public bool EditWcsUser(WcsUser user, out string result)
        {
            if (!UserList.Exists(c => user.id == c.id))
            {
                result = "不存在该用户信息！";
                return false;
            }

            if (UserList.Exists(c => user.id != c.id && user.username.Equals(c.username)))
            {
                result = "已经存在该用户名！";
                return false;
            }

            result = "";
            WcsUser ouser = UserList.Find(c => c.id == user.id);
            ouser.name = user.name;
            ouser.username = user.username;
            ouser.password = user.password;
            ouser.exitwcs = user.exitwcs;
            ouser.memo = user.memo;
            ouser.role_id = user.role_id;
            return PubMaster.Mod.RoleSql.EditeWcsUser(ouser);
        }


        /// <summary>
        /// 用户登陆/获取用户授权模块
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="result"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckUserGetPdaView(string username, string password, out string result, out UserModelPack user)
        {
            user = null;
            if (username == null || string.IsNullOrEmpty(username))
            {
                result = "用户名不能为空！";
                return false;
            }

            if (password == null || string.IsNullOrEmpty(password))
            {
                result = "密码不能为空！";
                return false;
            }

            WcsUser wcsuser = GetUser(username, password);
            if (wcsuser == null)
            {
                result = "账号密码错误！";
                return false;
            }

            WcsRole userrole = GetUserRole(username, password);
            if(userrole == null)
            {
                result = "用户没有配置角色！";
                return false;
            }

            if(userrole.menu_id == 0)
            {
                result = "用户角色没有配置菜单！";
                return false;
            }

            user = GetPDAMenu(userrole);
            if(user != null)
            {
                user.UserId = wcsuser.id+"";
                user.UserName = wcsuser.name;
                result = "";
                return true;
            }

            result = "";
            return false;
        }
        #endregion

        #region[角色管理]

        public WcsRole GetRole(int id)
        {
            return RoleList.Find(c => c.id == id);
        }

        public string GetRoleName(int roleid)
        {
            return RoleList.Find(c => c.id == roleid)?.name ?? ""+roleid;
        }


        public List<WcsRole> GetBeLowRoleList(byte prior)
        {
            return RoleList.FindAll(c => c.prior < prior);
        }

        public List<WcsRole> GetBelowLoginRoleList()
        {
            if (LoginUser == null) return new List<WcsRole>();
            WcsRole role = GetRole(LoginUser.role_id);
            return GetBeLowRoleList(role.prior);
        }

        #endregion

        #region[模块管理]

        public WcsModule GetModule(int id)
        {
            return ModuleList.Find(c => c.id == id);
        }

        public WcsModule GetWcsModule(string key)
        {
            return ModuleList.Find(c => key.Equals(c.key));
        }

        public List<WcsModule> GetWcsModules()
        {
            return ModuleList;
        }

        public List<WcsModule> GetWcsModules(WcsModuleTypeE type)
        {
            return ModuleList.FindAll(c => c.ModuleType == type);
        }

        public string GetModuleInfo(int module_id)
        {
            WcsModule md = GetModule(module_id);
            if (md != null)
            {
                return md.name + " : " + md.memo;
            }

            return "找不到模块配置信息:" + module_id; ;
        }

        #endregion

        #region[菜单管理]

        public List<MenuModel> GetDefaultMenu()
        {
            List<MenuModel> menus = new List<MenuModel>();

            MenuModel home = new MenuModel()
            {
                Name = "主页",
                IKey = "Home",
                OpenPage = true
            };
            menus.Add(home);

            MenuModel task = new MenuModel()
            {
                Name = "任务",
                IKey = "",
                OpenPage = false,
                MenuList = new List<MenuModel>()
                {
                    new MenuModel()
                    {
                        Name = "开关",
                        IKey = "AreaSwitch",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "任务",
                        IKey = "Trans",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "按轨出库",
                        IKey = "TileTrack",
                        OpenPage = true
                    }

                }
            };
            menus.Add(task);

            MenuModel device = new MenuModel()
            {
                Name = "设备",
                IKey = "",
                OpenPage = false,
                MenuList = new List<MenuModel>()
                {
                    new MenuModel()
                    {
                        Name = "砖机",
                        IKey = "TileLifter",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "摆渡车",
                        IKey = "Ferry",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "运输车",
                        IKey = "Carrier",
                        OpenPage = true
                    },new MenuModel()
                    {
                        Name = "轨道",
                        IKey = "Track",
                        OpenPage = true,
                    }
                    //,
                    //new MenuModel()
                    //{
                    //    Name = "平板",
                    //    IKey = "RfClient",
                    //    OpenPage = true
                    //}
                }
            };
            menus.Add(device);

            MenuModel good = new MenuModel()
            {
                Name = "统计",
                IKey = "",
                OpenPage = false,
                MenuList = new List<MenuModel>()
                {
                    new MenuModel()
                    {
                        Name = "规格",
                        IKey = "Goods",
                        OpenPage = true
                    },new MenuModel()
                    {
                        Name = "库存",
                        IKey = "StockSum",
                        OpenPage = true
                    },new MenuModel()
                    {
                        Name = "轨道",
                        IKey = "Stock",
                        OpenPage = true
                    }
                }
            };
            menus.Add(good);

            MenuModel set = new MenuModel()
            {
                Name = "设置",
                IKey = "",
                OpenPage = false,
                MenuList = new List<MenuModel>()
                {
                   new MenuModel()
                    {
                        Name = "轨道分配",
                        IKey = "TrackAllocate",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "摆渡对位",
                        IKey = "FerryPos",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "区域配置",
                        IKey = "Area",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "字典",
                        IKey = "Diction",
                        OpenPage = true
                    },
                    new MenuModel()
                    {
                        Name = "测可入砖",
                        IKey = "TestGood",
                        OpenPage = true
                    },
                   new MenuModel()
                    {
                        Name = "添加任务",
                        IKey = "AddManualTrans",
                        OpenPage = true
                    },
                   new MenuModel()
                    {
                        Name = "菜单",
                        IKey = "Menu",
                        OpenPage = true
                    }
                }
            };
            menus.Add(set);

            MenuModel log = new MenuModel()
            {
                Name = "记录",
                IKey = "",
                OpenPage = false,
                MenuList = new List<MenuModel>()
                {
                    new MenuModel()
                    {
                        Name ="警告",
                        OpenPage = true,
                        IKey = "WarnLog"
                    },
                    new MenuModel()
                    {
                        Name ="空满轨道",
                        OpenPage = true,
                        IKey = "TrackLog"
                    },
                }
            };

            menus.Add(log);
            return menus;
        }

        public List<MenuModel> GetMenu(out string result, string username = "guest", string password = "guest")
        {
            //if (username.Equals("guest"))
            //{
            //    result = "";
            //    return GetDefaultMenu();
            //}

            List<MenuModel> menus = new List<MenuModel>();
            WcsRole role = GetUserRole(username, password);
            if(role == null)
            {
                result = "账号或密码不正确！";
                return menus;
            }

            menus.AddRange(GetWcsMenuDtl(role.menu_id));
            result = "";
            return menus;
        }

        public List<MenuModel> GetWcsMenuDtl(int menuid, bool getrf = false)
        {
            List<MenuModel> menus = new List<MenuModel>();
            List<WcsMenuDtl> usermenus = MenuDtlList.FindAll(c => c.menu_id == menuid);
            if (usermenus.Count > 0)
            {
                usermenus.Sort((x, y) => x.order.CompareTo(y.order));
                foreach (WcsMenuDtl item in usermenus)
                {
                    MenuModel menumd = GetMenu(item.id, getrf);
                    if (menumd != null)
                    {
                        menus.Add(menumd);
                    }
                }
            }
            return menus;
        }

        public List<WcsMenu> GetMenuList()
        {
            return MenuList;
        }

        public MenuModel GetMenu(int id, bool getrf = false)
        {
            MenuModel menu = new MenuModel();
            WcsMenuDtl dtl = GetMenuDtl(id);
            if (dtl != null && (!dtl.rf || getrf ))
            {
                menu.Id = dtl.id;
                menu.FolderId = dtl.folder_id;
                menu.Name = dtl.name;
                menu.OpenPage = !dtl.folder;
                menu.ModuleId = dtl.module_id;
                menu.MenuId = dtl.menu_id;
                menu.Rf = dtl.rf;
                if (!dtl.folder)
                {
                    WcsModule md = GetModule(dtl.module_id);
                    if (md != null)
                    {
                        menu.IKey = md.key;
                    }
                    else
                    {
                        menu.IKey = "没有配置菜单IKEY";
                    }
                }
                else
                {
                    List<WcsMenuDtl> dtls = GetMenuDtls(dtl.id);
                    if (dtls.Count > 0)
                    {
                        dtls.Sort((x, y) => x.order.CompareTo(y.order));
                    }

                    foreach (var item in dtls)
                    {
                        menu.AddMenu(GetMenu(item.id));
                    }
                }

                return menu;
            }
            return null;
        }

        public bool HavePriorInMenu(short prior)
        {
            if (LoginUser == null) return false;
            WcsRole role = GetRole(LoginUser.role_id);
            return (role?.prior ?? 0) >= prior;
        }

        public WcsMenuDtl GetMenuDtl(int id)
        {
            return MenuDtlList.Find(c => c.id == id);
        }

        public List<WcsMenuDtl> GetMenuDtls(int folderid)
        {
            return MenuDtlList.FindAll(c => c.folder_id == folderid);
        }

        public string GetMenuName(int id)
        {
            return MenuDtlList.Find(c => c.id == id)?.name ?? "找不到菜单数据:" + id;
        }

        public int GetMaxMenuDtlId()
        {
            if(MenuDtlNexId == -1)
            {
                try
                {
                    MenuDtlNexId = MenuDtlList.Max(c => c.id);
                }catch
                {
                    MenuDtlNexId = 1000;
                }
            }
            return ++MenuDtlNexId;
        }

        public void DeleteMenu(List<MenuModel> deletelist)
        {
            foreach (var item in deletelist)
            {
                WcsMenuDtl dtl = MenuDtlList.Find(c => c.id == item.Id);
                if (dtl != null)
                {
                    if (dtl.folder)
                    {
                        PubMaster.Mod.RoleSql.DeleteWcsMenuDtlsByFolderId(dtl.id);
                    }

                    PubMaster.Mod.RoleSql.DeleteWcsMenuDtl(dtl);
                }
            }
        }

        public void AddMenuDtl(MenuModel md, short order)
        {
            WcsMenuDtl dtl = MenuDtlList.Find(c => c.id == md.Id);
            if (dtl != null)//修改
            {
                EditMenuDtl(dtl, md, order);
            }
            else//添加
            {
                AddMenuDtlByModel(md, order);
            }

            if (md.MenuList != null)
            {
                short dtlorder = 1;
                List<WcsMenuDtl> dtllist = GetMenuDtls(md.Id);
                foreach (var item in md.MenuList)
                {
                    WcsMenuDtl mdtl = MenuDtlList.Find(c => c.id == item.Id);
                    //修改
                    if (mdtl != null)
                    {
                        EditMenuDtl(mdtl, item, dtlorder);
                    }else
                    {
                        //添加
                        AddMenuDtlByModel(item, dtlorder);
                    }
                    dtlorder++;
                }
            }
        }

        private void EditMenuDtl(WcsMenuDtl dtl, MenuModel md, short order)
        {
            dtl.name = md.Name;
            dtl.folder = !md.OpenPage;
            dtl.folder_id = md.FolderId;
            dtl.menu_id = md.MenuId;
            dtl.module_id = md.ModuleId;
            dtl.order = order;
            dtl.rf = md.Rf;
            PubMaster.Mod.RoleSql.EditeWcsMenuDtl(dtl);
        }

        private void AddMenuDtlByModel(MenuModel md, short order)
        {
            WcsMenuDtl dtl = new WcsMenuDtl();
            dtl.id = md.Id;
            dtl.name = md.Name;
            dtl.folder = !md.OpenPage;
            dtl.folder_id = md.FolderId;
            dtl.menu_id = md.MenuId;
            dtl.module_id = md.ModuleId;
            dtl.order = order;
            dtl.rf = md.Rf;
            PubMaster.Mod.RoleSql.AddWcsMenuDtl(dtl);
        }

        public List<WcsMenu> GetLoginPriorMenuList()
        {
            List<WcsMenu> list = new List<WcsMenu>();
            if(LoginUser == null)
            {
                return list;
            }
            WcsRole role = GetRole(LoginUser.role_id);
            return MenuList.FindAll(c => c.prior <= role.prior);
        }

        public UserModelPack GetPDAMenu(WcsRole role)
        {
            UserModelPack pack = new UserModelPack();
            List<ModuleView> menus = new List<ModuleView>();

            List<WcsMenuDtl> usermenus = MenuDtlList.FindAll(c =>c.rf && c.menu_id == role.menu_id);
            if (usermenus.Count > 0)
            {
                usermenus.Sort((x, y) => x.order.CompareTo(y.order));
                foreach (WcsMenuDtl item in usermenus)
                {
                    WcsModule md = GetModule(item.module_id);
                    if (md != null && md.ModuleType == WcsModuleTypeE.平板)
                    {
                        menus.Add(new ModuleView()
                        {
                            ModuleName = item.name,
                            ModuleId = md.key,
                            ModulePic = md.geometry,
                            ModuleEntry = md.entity
                        });
                    }
                }
            }
            pack.AddModule(menus);
            return pack;
        }

        public List<ModuleView> GetPDAMenuDtl(int menuid)
        {
            List<ModuleView> menus = new List<ModuleView>();
            
            return menus;
        }
        #endregion
    }
}
