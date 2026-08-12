// ViewModels/LoginViewModel.cs
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
    public class LoginViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private string _username;
        private string _password;
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

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ExitCommand { get; }

        public LoginViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            LoginCommand = new DelegateCommand(Login, CanLogin)
                .ObservesProperty(() => Username)
                .ObservesProperty(() => Password)
                .ObservesProperty(() => IsLoading);
            RegisterCommand = new DelegateCommand(Register);
            ExitCommand = new DelegateCommand(Exit);
        }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async void Login()
        {
            if (!CanLogin()) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _dataService.GetUserAsync(Username, Password);

                if (user != null)
                {
                    // 更新最后登录时间
                    await _dataService.UpdateLastLoginAsync(user.Id);

                    // 保存会话信息
                    SessionManager.CurrentUserId = user.Id;
                    SessionManager.CurrentUsername = user.Username;
                    SessionManager.CurrentUserRole = user.Role;

                    // 打开主窗口
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // 关闭登录窗口
                    Application.Current.Windows.OfType<LoginView>().FirstOrDefault()?.Close();
                }
                else
                {
                    ErrorMessage = "用户名或密码错误！";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"登录失败：{ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Register()
        {
            var registerWindow = new RegisterView();
            registerWindow.Owner = Application.Current.MainWindow;
            registerWindow.ShowDialog();
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}