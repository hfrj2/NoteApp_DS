// ViewModels/RegisterViewModel.cs
using NoteApp.Models;
using NoteApp.Services;
using NoteApp.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NoteApp.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _phone;
        private string _address;
        private string _errorMessage;
        private bool _isLoading;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            RegisterCommand = new DelegateCommand(Register, CanRegister);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private bool CanRegister()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   Password == ConfirmPassword &&
                   Username.Length >= 3 &&
                   Password.Length >= 6;
        }

        private async void Register()
        {
            if (!CanRegister()) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // 检查用户名是否已存在
                var existingUser = await _dataService.GetUserByUsernameAsync(Username);
                if (existingUser != null)
                {
                    ErrorMessage = "用户名已存在！";
                    return;
                }

                // 创建新用户
                var newUser = new User
                {
                    Username = Username,
                    Password = Password,
                    Phone = Phone,
                    Address = Address,
                    Role = "User",
                    CreateTime = DateTime.Now
                };

                var success = await _dataService.AddUserAsync(newUser);

                if (success)
                {
                    _dialogService.ShowMessage("注册成功！请登录。", "注册完成");
                    Application.Current.Windows.OfType<RegisterView>().FirstOrDefault()?.Close();
                }
                else
                {
                    ErrorMessage = "注册失败，请稍后重试。";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"注册失败：{ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Cancel()
        {
            Application.Current.Windows.OfType<RegisterView>().FirstOrDefault()?.Close();
        }
    }
}