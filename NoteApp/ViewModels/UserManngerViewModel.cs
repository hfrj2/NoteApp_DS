using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using NoteApp.Models;
using NoteApp.Services;
using System.Windows;

namespace NoteApp.ViewModels
{
    public class UserManageViewModel : BindableBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value) && value != null)
                {
                    EditId = value.Id;
                    EditAccountName = value.AccountName;
                    EditPhone = value.Phone;
                    EditAddress = value.Address;
                    EditPassword = string.Empty; // 编辑时密码留空表示不修改
                }
            }
        }

        private int _editId;
        public int EditId
        {
            get => _editId;
            set => SetProperty(ref _editId, value);
        }

        private string _editAccountName;
        public string EditAccountName
        {
            get => _editAccountName;
            set => SetProperty(ref _editAccountName, value);
        }

        private string _editPassword;
        public string EditPassword
        {
            get => _editPassword;
            set => SetProperty(ref _editPassword, value);
        }

        private string _editPhone;
        public string EditPhone
        {
            get => _editPhone;
            set => SetProperty(ref _editPhone, value);
        }

        private string _editAddress;
        public string EditAddress
        {
            get => _editAddress;
            set => SetProperty(ref _editAddress, value);
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        public UserManageViewModel(IUserService userService, ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;

            AddCommand = new DelegateCommand(Add);
            SaveCommand = new DelegateCommand(Save, CanSave);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            RefreshCommand = new DelegateCommand(Refresh);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(EditAccountName) || e.PropertyName == nameof(EditPassword))
                    SaveCommand.RaiseCanExecuteChanged();
                if (e.PropertyName == nameof(SelectedUser))
                    DeleteCommand.RaiseCanExecuteChanged();
            };

            Refresh();
        }

        private void Add()
        {
            SelectedUser = null;
            EditId = 0;
            EditAccountName = string.Empty;
            EditPassword = string.Empty;
            EditPhone = string.Empty;
            EditAddress = string.Empty;
        }

        private bool CanSave()
        {
            if (EditId == 0)
            {
                // 新增时账户名和密码必填
                return !string.IsNullOrWhiteSpace(EditAccountName) && !string.IsNullOrWhiteSpace(EditPassword);
            }
            else
            {
                // 编辑时账户名必填，密码可选
                return !string.IsNullOrWhiteSpace(EditAccountName);
            }
        }

        private void Save()
        {
            try
            {
                if (EditId == 0)
                {
                    // 新增用户
                    if (_userService.AccountExists(EditAccountName.Trim()))
                    {
                        MessageBox.Show("账户名已存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var user = new User
                    {
                        AccountName = EditAccountName.Trim(),
                        Phone = EditPhone?.Trim(),
                        Address = EditAddress?.Trim()
                    };
                    _userService.Register(user, EditPassword);
                }
                else
                {
                    // 编辑用户
                    var user = _userService.GetUserById(EditId);
                    if (user == null) return;

                    // 检查账户名是否与其他用户重复
                    if (_userService.AccountExists(EditAccountName.Trim(), EditId))
                    {
                        MessageBox.Show("账户名已存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    user.AccountName = EditAccountName.Trim();
                    user.Phone = EditPhone?.Trim();
                    user.Address = EditAddress?.Trim();
                    _userService.UpdateUser(user, string.IsNullOrWhiteSpace(EditPassword) ? null : EditPassword);
                }
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDelete()
        {
            return SelectedUser != null && SelectedUser.Id != _sessionService.CurrentUser?.Id;
        }

        private void Delete()
        {
            if (SelectedUser == null) return;

            if (SelectedUser.Id == _sessionService.CurrentUser?.Id)
            {
                MessageBox.Show("不能删除当前登录用户", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"确定要删除用户“{SelectedUser.AccountName}”吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _userService.DeleteUser(SelectedUser.Id);
                Refresh();
            }
        }

        private void Refresh()
        {
            var list = _userService.GetAllUsers();
            Users = new ObservableCollection<User>(list);
            if (Users.Any())
            {
                SelectedUser = Users[0];
            }
            else
            {
                SelectedUser = null;
                Add();
            }
        }
    }
}