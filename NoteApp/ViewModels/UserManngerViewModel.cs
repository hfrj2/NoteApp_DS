// ViewModels/UserManageViewModel.cs
using NoteApp.Models;
using NoteApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NoteApp.ViewModels
{
    public class UserManageViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<User> _users;
        private User _selectedUser;
        private User _editingUser;
        private string _searchText;
        private bool _isEditing;
        private ObservableCollection<string> _roles;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                if (value != null)
                {
                    EditingUser = new User
                    {
                        Id = value.Id,
                        Username = value.Username,
                        Password = value.Password,
                        Phone = value.Phone,
                        Address = value.Address,
                        Role = value.Role,
                        CreateTime = value.CreateTime,
                        LastLogin = value.LastLogin
                    };
                }
                else
                {
                    EditingUser = null;
                }
                ((DelegateCommand)EditCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public User EditingUser
        {
            get => _editingUser;
            set => SetProperty(ref _editingUser, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                LoadUsersAsync();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public ObservableCollection<string> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }

        public UserManageViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            Users = new ObservableCollection<User>();
            Roles = new ObservableCollection<string> { "User", "Admin" };

            AddCommand = new DelegateCommand(AddUser);
            EditCommand = new DelegateCommand(EditUser, CanEditUser);
            DeleteCommand = new DelegateCommand(DeleteUser, CanDeleteUser);
            SaveCommand = new DelegateCommand(SaveUser, CanSaveUser);
            CancelCommand = new DelegateCommand(CancelEdit);
            RefreshCommand = new DelegateCommand(LoadUsersAsync);

            LoadUsersAsync();
        }

        private bool CanEditUser() => SelectedUser != null && !IsEditing && SelectedUser.Id != SessionManager.CurrentUserId;
        private bool CanDeleteUser() => SelectedUser != null && !IsEditing && SelectedUser.Id != SessionManager.CurrentUserId;
        private bool CanSaveUser() => IsEditing && EditingUser != null && !string.IsNullOrWhiteSpace(EditingUser.Username);

        private async void LoadUsersAsync()
        {
            try
            {
                var users = await _dataService.GetAllUsersAsync();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    users = users.Where(u => u.Username.Contains(SearchText) ||
                                             u.Phone.Contains(SearchText) ||
                                             u.Address.Contains(SearchText)).ToList();
                }

                Users.Clear();
                foreach (var user in users.OrderBy(u => u.Username))
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"加载用户失败：{ex.Message}");
            }
        }

        private void AddUser()
        {
            EditingUser = new User
            {
                Username = "",
                Password = "",
                Phone = "",
                Address = "",
                Role = "User",
                CreateTime = DateTime.Now
            };
            IsEditing = true;
            SelectedUser = null;
        }

        private void EditUser()
        {
            if (SelectedUser == null) return;
            IsEditing = true;
        }

        private async void DeleteUser()
        {
            if (SelectedUser == null) return;

            if (_dialogService.ShowConfirm($"确定要删除用户 \"{SelectedUser.Username}\" 吗？\n该用户的所有便签也会被删除。"))
            {
                try
                {
                    var success = await _dataService.DeleteUserAsync(SelectedUser.Id);
                    if (success)
                    {
                        Users.Remove(SelectedUser);
                        SelectedUser = null;
                        _dialogService.ShowMessage("删除成功");
                    }
                    else
                    {
                        _dialogService.ShowError("删除失败");
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"删除失败：{ex.Message}");
                }
            }
        }

        private async void SaveUser()
        {
            if (EditingUser == null) return;

            if (string.IsNullOrWhiteSpace(EditingUser.Username))
            {
                _dialogService.ShowWarning("请输入用户名");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingUser.Password) && EditingUser.Id == 0)
            {
                _dialogService.ShowWarning("请输入密码");
                return;
            }

            try
            {
                bool success;
                if (EditingUser.Id == 0)
                {
                    var existingUser = await _dataService.GetUserByUsernameAsync(EditingUser.Username);
                    if (existingUser != null)
                    {
                        _dialogService.ShowWarning("用户名已存在");
                        return;
                    }

                    success = await _dataService.AddUserAsync(EditingUser);
                }
                else
                {
                    success = await _dataService.UpdateUserAsync(EditingUser);
                }

                if (success)
                {
                    _dialogService.ShowMessage(EditingUser.Id == 0 ? "添加成功" : "更新成功");
                    IsEditing = false;
                    LoadUsersAsync();
                    if (EditingUser.Id > 0)
                    {
                        SelectedUser = Users.FirstOrDefault(u => u.Id == EditingUser.Id);
                    }
                }
                else
                {
                    _dialogService.ShowError("保存失败");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"保存失败：{ex.Message}");
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            EditingUser = null;
            if (SelectedUser != null)
            {
                EditingUser = new User
                {
                    Id = SelectedUser.Id,
                    Username = SelectedUser.Username,
                    Password = SelectedUser.Password,
                    Phone = SelectedUser.Phone,
                    Address = SelectedUser.Address,
                    Role = SelectedUser.Role,
                    CreateTime = SelectedUser.CreateTime,
                    LastLogin = SelectedUser.LastLogin
                };
            }
        }
    }
}