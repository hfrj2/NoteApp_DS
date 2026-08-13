using NoteApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows;

namespace NoteApp.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

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

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand GoToRegisterCommand { get; }

        public LoginViewModel(IUserService userService, ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;
            LoginCommand = new DelegateCommand(Login, CanLogin);
            GoToRegisterCommand = new DelegateCommand(GoToRegister);

            // 当属性变化时重新检查登录命令是否可执行
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AccountName) || e.PropertyName == nameof(Password))
                    LoginCommand.RaiseCanExecuteChanged();
            };
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(AccountName) && !string.IsNullOrWhiteSpace(Password);
        }

        private void Login()
        {
            try
            {
                var user = _userService.Login(AccountName.Trim(), Password);
                if (user != null)
                {
                    _sessionService.CurrentUser = user;
                    ErrorMessage = string.Empty;
                    // 请求关闭登录窗口，并返回成功
                    if (Application.Current.Windows.OfType<Views.LoginView>().FirstOrDefault() is Views.LoginView loginView)
                    {
                        loginView.DialogResult = true;
                        loginView.Close();
                    }
                }
                else
                {
                    ErrorMessage = "账号或密码错误";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"登录失败：{ex.Message}";
            }
        }

        private void GoToRegister()
        {
            var registerView = new Views.RegisterView();
            registerView.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is Views.LoginView);
            registerView.ShowDialog();
        }
    }
}