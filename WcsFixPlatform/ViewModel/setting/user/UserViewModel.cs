using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using module.role;
using module.window;
using resource;
using System;
using System.Collections.ObjectModel;
using wcs.Dialog;

namespace wcs.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        public UserViewModel()
        {
            _userlist = new ObservableCollection<WcsUser>();
        }

        #region[字段]
        private WcsUser _selectuser;
        private ObservableCollection<WcsUser> _userlist;
        #endregion

        #region[属性]

        public WcsUser SelectUser
        {
            get => _selectuser;
            set => Set(ref _selectuser, value);
        }

        public ObservableCollection<WcsUser> UserList
        {
            get => _userlist;
            set => Set(ref _userlist, value);
        }

        #endregion

        #region[命令]
        public RelayCommand<string> BtmCmd => new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(BtnAction)).Value;

        #endregion

        #region[方法]

        private void BtnAction(string tag)
        {
            switch (tag)
            {
                case "RefreshUser":
                    QueryUserList();
                    break;
                case "AddUser":
                    AddUser();
                    break;
                case "EditUser":
                    EditUser();
                    break;
                case "DeleteUser":
                    DeleteUser();
                    break;
            }
        }

        private void QueryUserList()
        {
            UserList.Clear();
            foreach (var item in PubMaster.Role.GetUserList())
            {
                UserList.Add(item);
            }

            if(UserList.Count == 0)
            {
                Growl.Info("当前没有用户数据！");
            }
        }


        private async void AddUser()
        {
            DialogResult result = await HandyControl.Controls.Dialog.Show<UserEditDialog>()
                              .Initialize<UserEditViewModel>((vm) =>
                              {
                                  vm.SetAdd();
                              }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && rs)
            {
                Growl.Success("添加成功！");
                QueryUserList();
            }
        }

        private async void EditUser()
        {
            if(SelectUser == null)
            {
                Growl.Warning("请先选择用户！");
                return;
            }

            if (!CheckHavePrior()) return;

            DialogResult result = await HandyControl.Controls.Dialog.Show<UserEditDialog>()
                              .Initialize<UserEditViewModel>((vm) =>
                              {
                                  vm.SetEditUser(SelectUser);
                              }).GetResultAsync<DialogResult>();
            if (result.p1 is bool rs && rs)
            {
                Growl.Success("修改成功！");
                QueryUserList();
            }
        }


        private void DeleteUser()
        {
            if(SelectUser == null)
            {
                Growl.Warning("请先选择用户！");
                return;
            }

            if (!CheckHavePrior()) return;

        }

        private bool CheckHavePrior()
        {
            if(!PubMaster.Role.IsUserBelowLoginPrior(SelectUser, out string reuslt))
            {
                Growl.Warning(reuslt);
                return false;
            }

            return true;
        }

        #endregion
    }
}
