using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.role;
using module.window;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public MenuViewModel()
        {
            MenuList = new ObservableCollection<MenuModel>();
            deletelist = new List<MenuModel>();
        }

        #region[字段]
        private string actionname;

        private ObservableCollection<MenuModel> _menuList;

        private string menunmae;
        private bool isfolder;
        private string foldername;
        private string modulename;
        private int module_id;
        private bool ismoduleeenable;
        private bool isrf;

        private MenuModel selectmodel;
        private MenuModel foldermd;

        private List<MenuModel> deletelist;
        private int actionType;

        private WcsMenu selectMenu { set; get; }
        private string selectmenuname;
        #endregion

        #region[属性]

        public string ActionName
        {
            get => actionname;
            set => Set(ref actionname, value);
        }

        public ObservableCollection<MenuModel> MenuList
        {
            get => _menuList;
            set => Set(ref _menuList, value);
        }

        public string MenuName
        {
            get => menunmae;
            set => Set(ref menunmae, value);
        }

        public bool IsFolder
        {
            get => isfolder;
            set => Set(ref isfolder, value);
        }

        public string FolderName
        {
            get => foldername;
            set => Set(ref foldername, value);
        }

        public string ModuleName
        {
            get => modulename;
            set => Set(ref modulename, value);
        }

        public bool IsModuleEnable
        {
            get => ismoduleeenable;
            set => Set(ref ismoduleeenable, value);
        }

        public string SelectMenuName
        {
            get => selectmenuname;
            set => Set(ref selectmenuname, value);
        }

        public bool IsRf
        {
            get => isrf;
            set => Set(ref isrf, value);
        }
        #endregion

        #region[命令]        
        public RelayCommand<RoutedEventArgs> MenuTreeViewChangeCmd => new Lazy<RelayCommand<RoutedEventArgs>>(() => new RelayCommand<RoutedEventArgs>(MenuTreeViewChange)).Value;
        public RelayCommand<string> MenuBtnCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(MenuBtnFunction)).Value;

        #endregion

        #region[方法]

        private void MenuTreeViewChange(RoutedEventArgs orgs)
        {
            if (orgs != null && orgs.OriginalSource is TreeView pro && pro.SelectedItem is MenuModel menu)
            {
                selectmodel = menu;
                if (!menu.OpenPage)
                {
                    foldermd = menu;
                    FolderName = "";
                }
                else
                {
                    foldermd = MenuList.FirstOrDefault(c => c.Id == menu.FolderId);
                }

                MenuName = selectmodel.Name;
                IsFolder = !selectmodel.OpenPage;
                IsModuleEnable = selectmodel.OpenPage;
                if (!IsFolder && foldermd != null)
                {
                    FolderName = foldermd.Name;
                }

                if (!IsFolder)
                {
                    ModuleName = PubMaster.Role.GetModuleInfo(selectmodel.ModuleId);
                    module_id = selectmodel.ModuleId;
                }
                else
                {
                    ModuleName = "";
                    module_id = 0;
                }

                ActionName = "修改";
                actionType = 4;
            }
        }

        private void MenuBtnFunction(string tag)
        {
            switch (tag)
            {
                case "SelectMenu":
                    SelectMenu();
                    break;
                case "Refresh":
                    Refresh();
                    break;
                case "MenuMoveUp":
                    MenuMoveUp();
                    break;
                case "MenuMoveDown":
                    MenuMoveDown();
                    break;
                case "DeleteMenu":
                    DeleteMenu();
                    break;
                case "ClearInput":
                    ClearInput();
                    break;
                case "EditMenu": //添加/修改确认按钮
                    EditMenu();
                    break;
                case "AddFolder":
                    AddFolder();
                    ActionName = "添加目录";
                    actionType = 1;
                    break;
                case "AddModule":
                    AddModule();
                    ActionName = "添加目录功能";
                    actionType = 2;
                    break;
                case "AddNoneFolderModule":
                    AddNoneFolderModule();
                    ActionName = "添加无目录功能";
                    actionType = 3;
                    break;
                case "AddPDAModule":
                    AddPDAModule();
                    ActionName = "添加平板功能";
                    actionType = 5;
                    break;
                case "SelectModule":
                    SelectModule();
                    break;
                case "SaveChange"://保存修改
                    SaveChange();
                    break;
            }
        }

        private async void SelectModule()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<ModuleSelectDialog>()
                           .Initialize<ModuleSelectViewModel>((vm) =>{
                               vm.QueryModule(IsRf ? WcsModuleTypeE.平板 : WcsModuleTypeE.电脑);
                           }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && result.p2 is WcsModule md)
            {
                ModuleName = md.name + (md.memo.Length <= 0 ? "" : " : "+md.memo);
                module_id = md.id;
            }
        }

        private async void SelectMenu()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<MenuSelectDialog>()
                           .Initialize<MenuSelectViewModel>((vm) =>{
                               vm.QueryMenu();
                           }).GetResultAsync<DialogResult>();

            if (result.p1 is bool rs && result.p2 is WcsMenu menu)
            {
                SelectMenuName = menu.name;
                selectMenu = menu;
                Refresh();
            }
        }

        private void MenuMoveUp()
        {
            if (!CheckSelectMenuFolder()) return;
            if (!selectmodel.OpenPage || selectmodel.FolderId == 0)
            {
                int index = MenuList.IndexOf(selectmodel);
                if (index == 0)
                {
                    Growl.Info("已经在顶部了");
                    return;
                }
                MenuModel md = selectmodel;
                MenuList.RemoveAt(index);
                MenuList.Insert(index - 1, md);
            }
            else
            {
                if(selectmodel.MenuId > 0)
                {
                    int index = MenuList.IndexOf(selectmodel);
                    MenuModel md = selectmodel;
                    MenuList.RemoveAt(index);
                    MenuList.Insert(index - 1, md);
                }
                else
                {
                    if (!CheckMenuFolder()) return;
                    int index = MenuList.IndexOf(foldermd);
                    if (!foldermd.MoveUp(selectmodel, out string reuslt))
                    {
                        Growl.Warning(reuslt);
                        return;
                    }
                    MenuModel md = foldermd;
                    MenuList.RemoveAt(index);
                    MenuList.Insert(index, md);
                }
                
            }
            selectmodel = null;
            foldermd = null;
        }

        private void MenuMoveDown()
        {
            if (!CheckSelectMenuFolder()) return;
            if (!selectmodel.OpenPage || selectmodel.FolderId == 0)
            {
                int index = MenuList.IndexOf(selectmodel);
                if (index == MenuList.Count - 1)
                {
                    Growl.Info("已经在底部了");
                    return;
                }
                MenuModel md = selectmodel;
                MenuList.RemoveAt(index);
                MenuList.Insert(index + 1, md);
            }
            else
            {
                if (!CheckMenuFolder()) return;
                int index = MenuList.IndexOf(foldermd);
                if (!foldermd.MoveDown(selectmodel, out string result))
                {
                    Growl.Warning(result);
                    return;
                }
                MenuModel md = foldermd;
                MenuList.RemoveAt(index);
                MenuList.Insert(index, md);
            }
            selectmodel = null;
            foldermd = null;
        }

        private void AddFolder()
        {
            ClearInput();
            ActionName = "添加";
        }

        private void Refresh()
        {
            if(selectMenu == null)
            {
                Growl.Warning("请先选择菜单！");
                return;
            }
            deletelist.Clear();
            ClearInput();

            List<MenuModel> list = PubMaster.Role.GetWcsMenuDtl(selectMenu.id, true) ;
            MenuList.Clear();
            foreach (MenuModel item in list)
            {
                MenuList.Add(item);
            }
        }

        /// <summary>
        /// 添加目录下的功能
        /// </summary>
        private void AddModule()
        {
            if(selectmodel == null || selectmodel.OpenPage)
            {
                Growl.Warning("请先选择目录！");
                return;
            }
            ActionName = "添加";
            ClearInput(false);
            FolderName = foldermd.Name;
            IsModuleEnable = true;
        }
        
        /// <summary>
        /// 添加目录级别的功能
        /// </summary>
        private void AddNoneFolderModule()
        {
            ActionName = "添加";
            ClearInput();
            IsModuleEnable = true;

        }

        /// <summary>
        /// 添加平板功能
        /// </summary>
        private void AddPDAModule()
        {
            ActionName = "添加";
            ClearInput();
            IsModuleEnable = true;
            IsRf = true;
            IsFolder = false;
        }

        private void DeleteMenu()
        {
            if (!CheckSelectMenuFolder()) return;
            deletelist.Add(selectmodel);
            if (!selectmodel.OpenPage || selectmodel.FolderId == 0)
            {
                MenuList.Remove(selectmodel);
            }
            else
            {
                if(foldermd == null || foldermd.Id != selectmodel.FolderId)
                {
                    Growl.Warning("功能的目录信息找不到");
                    return;
                }

                if(!foldermd.DeleteDtl(selectmodel, out string reuslt))
                {
                    Growl.Warning(reuslt);
                    return;
                }
                int index = MenuList.IndexOf(foldermd);
                MenuModel md = foldermd;
                MenuList.RemoveAt(index);
                MenuList.Insert(index, md);
            }
        }

        /// <summary>
        /// 添加/修改
        /// </summary>
        private void EditMenu()
        {
            switch (actionType)
            {
                //1.添加目录  
                case 1:
                    if (!CheckInput()) return;
                    if (!CheckSelectMenu()) return;
                    AddMenu();
                    break;
                case 2://2.添加功能 
                    if (!CheckInput()) return;
                    if (!CheckModule()) return;
                    AddMenuModule();
                    break;
                case 3:// 3.添加无目录功能
                    if (!CheckInput()) return;
                    if (!CheckModule()) return;
                    AddNoneMenuModule();
                    break;
                case 4://4.修改目录、功能
                    if (!CheckSelectMenuFolder()) return;
                    if (!selectmodel.OpenPage || selectmodel.FolderId == 0)
                    {
                        int idx = MenuList.IndexOf(selectmodel);
                        selectmodel.Name = MenuName;
                        selectmodel.ModuleId = module_id;

                        MenuModel md = selectmodel;
                        MenuList.RemoveAt(idx);
                        MenuList.Insert(idx, md);
                        Growl.Success("修改成功!");
                    }
                    else
                    {
                        selectmodel.Name = MenuName;
                        selectmodel.ModuleId = module_id;
                        if (!foldermd.UpdateDtl(selectmodel, out string result))
                        {
                            Growl.Warning(result);
                            return;
                        }
                        int idx = MenuList.IndexOf(foldermd);
                        MenuModel md = foldermd;
                        MenuList.RemoveAt(idx);
                        MenuList.Insert(idx, md);
                        Growl.Success("修改成功!");
                    }
                    break;
                case 5://5.添加平板功能
                    if (!CheckInput()) return;
                    if (!CheckModule()) return;
                    AddPDAMenuModule();
                    break;
            }
        }

        private bool CheckSelectMenuFolder()
        {
            if (selectmodel == null)
            {
                Growl.Warning("请先选择目录！");
                return false;
            }
            return true;
        }

        private bool CheckMenuFolder()
        {
            if (foldermd == null || foldermd.Id != selectmodel.FolderId)
            {
                Growl.Warning("找不到目录的数据!");
                return false;
            }

            return true;
        }

        private bool CheckInput()
        {
            if(MenuName.Trim().Length <= 0)
            {
                Growl.Warning("请输入名称！");
                return false;
            }

            return true;
        }

        private bool CheckModule()
        {
            if (module_id <= 0)
            {
                Growl.Warning("请选择功能！");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 清空输入内容
        /// </summary>
        /// <param name="clearfolder"></param>
        private void ClearInput(bool clearfolder = true)
        {
            selectmodel = null;
            MenuName = "";
            IsFolder = false;
            IsRf = false;
            IsModuleEnable = false;
            if (clearfolder)
            {
                foldermd = null;
                FolderName = "";
            }
            ModuleName = "";
            module_id = 0;
        }

        private bool CheckSelectMenu()
        {
            if(selectMenu == null)
            {
                Growl.Warning("请选择菜单!");
                return false;
            }

            return true;
        }
        #endregion


        #region[添加菜单/修改/保存]

        private void AddMenu()
        {
            MenuModel menu = new MenuModel();
            menu.Id = PubMaster.Role.GetMaxMenuDtlId();
            menu.Name = MenuName;
            menu.OpenPage = false;
            menu.MenuId = selectMenu.id;

            MenuList.Add(menu);
        }

        private void AddMenuModule()
        {

            MenuModel menu = new MenuModel();
            menu.Id = PubMaster.Role.GetMaxMenuDtlId();
            menu.Name = MenuName;
            menu.FolderId = foldermd.Id;
            menu.ModuleId = module_id;
            menu.OpenPage = true;

            int idx = MenuList.IndexOf(foldermd);
            foldermd.AddMenu(menu);

            MenuModel temp = foldermd;
            MenuList.RemoveAt(idx);
            MenuList.Insert(idx, temp);
            ClearInput();
        }

        private void AddNoneMenuModule()
        {
            MenuModel menu = new MenuModel();
            menu.Id = PubMaster.Role.GetMaxMenuDtlId();
            menu.Name = MenuName;
            menu.FolderId = 0;
            menu.MenuId = selectMenu.id;
            menu.ModuleId = module_id;
            menu.OpenPage = true;

            MenuList.Add(menu);
        }

        //添加平板功能
        private void AddPDAMenuModule()
        {
            MenuModel menu = new MenuModel();
            menu.Id = PubMaster.Role.GetMaxMenuDtlId();
            menu.Name = MenuName;
            menu.FolderId = 0;
            menu.MenuId = selectMenu.id;
            menu.ModuleId = module_id;
            menu.OpenPage = true;
            menu.Rf = true;

            MenuList.Add(menu);
        }

        private void SaveChange()
        {
            if (!CheckSelectMenu()) return;
            if (!PubMaster.Role.HavePriorInMenu(selectMenu.prior))
            {
                Growl.Warning("当前账号没有权限操作该菜单！");
                ClearInput();
                selectMenu = null;
                SelectMenuName = "";
                MenuList.Clear();
                return;
            }

            if (deletelist.Count > 0)
            {
                PubMaster.Role.DeleteMenu(deletelist);
            }
            short order = 1;
            foreach (var item in MenuList)
            {
                PubMaster.Role.AddMenuDtl(item, order);
                order++;
            }
            PubMaster.Role.RefreshMenu();
            Refresh();
        }
        #endregion

    }
}
