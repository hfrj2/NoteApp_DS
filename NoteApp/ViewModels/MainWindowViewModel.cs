// ViewModels/MainWindowViewModel.cs
using NoteApp.Services;
using NoteApp.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NoteApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;

        private string _currentUsername;
        private string _currentRole;

        public string CurrentUsername
        {
            get => _currentUsername;
            set => SetProperty(ref _currentUsername, value);
        }

        public string CurrentRole
        {
            get => _currentRole;
            set => SetProperty(ref _currentRole, value);
        }

        public ICommand NavigateToNotesCommand { get; }
        public ICommand NavigateToUsersCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ExitCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            CurrentUsername = SessionManager.CurrentUsername;
            CurrentRole = SessionManager.CurrentUserRole;

            NavigateToNotesCommand = new DelegateCommand(() => NavigateTo("NoteManage"));
            NavigateToUsersCommand = new DelegateCommand(() => NavigateTo("UserManage"), CanNavigateToUsers);
            LogoutCommand = new DelegateCommand(Logout);
            ExitCommand = new DelegateCommand(Exit);

            // 默认加载便签管理
            NavigateTo("NoteManage");
        }

        private void NavigateTo(string viewName)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }

        private bool CanNavigateToUsers()
        {
            return SessionManager.CurrentUserRole == "Admin";
        }

        private void Logout()
        {
            if (_dialogService.ShowConfirm("确定要退出登录吗？", "确认退出"))
            {
                SessionManager.ClearSession();

                var loginView = new LoginView();
                loginView.Show();

                Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
            }
        }

        private void Exit()
        {
            if (_dialogService.ShowConfirm("确定要退出程序吗？", "确认退出"))
            {
                Application.Current.Shutdown();
            }
        }
    }
}