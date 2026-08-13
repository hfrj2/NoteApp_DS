using NoteApp.Models;
using NoteApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows;

namespace NoteApp.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly IUserService _userService;

        private string _accountName;
        public string AccountName
        {
            get => _accountName;
            set => SetProperty(ref _accountName, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterCommand = new DelegateCommand(Register, CanRegister);
            CancelCommand = new DelegateCommand(Cancel);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AccountName) || e.PropertyName == nameof(Password) || e.PropertyName == nameof(ConfirmPassword))
                    RegisterCommand.RaiseCanExecuteChanged();
            };
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(AccountName) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void Register()
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "两次输入的密码不一致";
                return;
            }

            var user = new User
            {
                AccountName = AccountName.Trim(),
                Phone = Phone?.Trim(),
                Address = Address?.Trim()
            };

            try
            {
                bool success = _userService.Register(user, Password);
                if (success)
                {
                    ErrorMessage = string.Empty;
                    MessageBox.Show("注册成功，请登录", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (Application.Current.Windows.OfType<Views.RegisterView>().FirstOrDefault() is Views.RegisterView registerWindow)
                    {
                        registerWindow.DialogResult = true;
                        registerWindow.Close();
                    }
                }
                else
                {
                    ErrorMessage = "注册失败，账户名可能已存在";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"注册失败：{ex.Message}";
            }
        }

        private void Cancel()
        {
            if (Application.Current.Windows.OfType<Views.RegisterView>().FirstOrDefault() is Views.RegisterView registerView)
            {
                registerView.DialogResult = false;
                registerView.Close();
            }
        }
    }
}