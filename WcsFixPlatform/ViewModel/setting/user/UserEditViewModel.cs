using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.role;
using module.window;
using resource;
using System;
using System.Collections.ObjectModel;

namespace wcs.ViewModel
{
    public class UserEditViewModel : ViewModelBase, IDialogResultable<DialogResult>
    {
        public UserEditViewModel()
        {
            _result = new DialogResult();
            Param = new DialogResult();
            _rolelist = new ObservableCollection<WcsRole>();
        }

        #region[字段]

        private DialogResult _result;
        private string name;
        private string username;
        private string password;
        private string memo;
        private bool exitwcs;

        private WcsUser edituser;
        private WcsRole userrole;
        private string actionname;
        private bool mIsAdd;

        private bool exitwcsshow;
        private bool roleenable;
        private ObservableCollection<WcsRole> _rolelist;

        #endregion

        #region[属性]

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        
        public string UserName
        {
            get => username;
            set => Set(ref username, value);
        }
        
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }

        public string Memo
        {
            get => memo;
            set => Set(ref memo, value);
        }

        public bool ExitWcs
        {
            get => exitwcs;
            set => Set(ref exitwcs, value);
        }
        
        public bool ExitWcsShow
        {
            get => exitwcsshow;
            set => Set(ref exitwcsshow, value);
        }

        public string ACTIONNAME
        {
            get => actionname;
            set => Set(ref actionname, value);
        }

        public DialogResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public DialogResult Param
        {
            set; get;
        }

        public Action CloseAction { get; set; }


        public WcsRole SelectRole
        {
            get => userrole;
            set => Set(ref userrole, value);
        }

        public ObservableCollection<WcsRole> RoleList
        {
            get => _rolelist;
            set => Set(ref _rolelist, value);
        }

        public bool RoleEnable
        {
            get => roleenable;
            set => Set(ref roleenable, value);
        }

        #endregion

        #region[命令]
        public RelayCommand ComfirmCmd => new Lazy<RelayCommand>(() => new RelayCommand(Comfirm)).Value;

        public RelayCommand CancelCmd => new Lazy<RelayCommand>(() => new RelayCommand(CancelChange)).Value;

        #endregion

        #region[方法]

        private void LoadLoginCanSelectRole()
        {
            RoleList.Clear();
            SelectRole = null;
            foreach (var item in PubMaster.Role.GetBelowLoginRoleList())
            {
                RoleList.Add(item);
            }
        }

        public void SetEditUser(WcsUser user)
        {
            edituser = user;
            ACTIONNAME = "修改";
            mIsAdd = false;

            Name = user.name;
            UserName = user.username;
            Password = user.password;
            Memo = user.memo;
            ExitWcs = user.exitwcs;

            WcsUser loginuser = PubMaster.Role.GetLoginUser();
            if(loginuser!=null && "supervisor".Equals(loginuser.username) && user.id != loginuser.id)
            {
                ExitWcsShow = true;
            }
            else
            {
                ExitWcsShow = false;
            }

            if(loginuser != null && user.id == loginuser.id)
            {
                RoleEnable = false;
                RoleList.Clear(); 
                WcsRole role = PubMaster.Role.GetRole(user.role_id);
                RoleList.Add(role);
                SelectRole = role;
            }
            else
            {
                RoleEnable = true;
                LoadLoginCanSelectRole();
                WcsRole role = PubMaster.Role.GetRole(user.role_id);
                if (role != null)
                {
                    SelectRole = role;
                }
            }
        }

        public void SetAdd()
        {
            edituser = null;
            ACTIONNAME = "添加";
            mIsAdd = true;
            Name = "";
            UserName = "";
            Password = "";
            Memo = "";
            ExitWcs = false;
            RoleEnable = true;
            LoadLoginCanSelectRole();
        }

        private void Comfirm()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Growl.WarningGlobal("请输入名称！");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(UserName))
            {
                Growl.WarningGlobal("请输入用户名！");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                Growl.WarningGlobal("请输入密码！");
                return;
            }

            if(SelectRole == null)
            {
                Growl.WarningGlobal("请选择角色！");
                return;
            }

            if (mIsAdd)
            {   //添加
                WcsUser user = new WcsUser();
                user.name = Name;
                user.username = UserName;
                user.password = Password;
                user.role_id = SelectRole.id;
                user.exitwcs = ExitWcs;

                if(!PubMaster.Role.AddWcsUser(user, out string result))
                {
                    Growl.WarningGlobal(result);
                    return;
                }
            }
            else
            {
                //修改
                edituser.name = Name;
                edituser.username = UserName;
                edituser.password = Password;
                edituser.role_id = SelectRole.id;
                edituser.exitwcs = ExitWcs;

                if (!PubMaster.Role.EditWcsUser(edituser, out string result))
                {
                    Growl.WarningGlobal(result);
                    return;
                }
            }

            Result.p1 = true;
            CloseAction?.Invoke();
        }

        private void CancelChange()
        {
            Result.p1 = false;
            CloseAction?.Invoke();
        }

        #endregion
    }
}
