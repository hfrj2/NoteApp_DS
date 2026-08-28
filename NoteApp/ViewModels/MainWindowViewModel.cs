using System.Collections.ObjectModel;
using NoteApp.Services;
using Prism.Mvvm;
using Prism.Regions;

namespace NoteApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ISessionService _sessionService;

        public ObservableCollection<MenuItem> MenuItems { get; }

        private MenuItem _selectedMenuItem;
        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (SetProperty(ref _selectedMenuItem, value) && value != null)
                {
                    NavigateTo(value);
                }
            }
        }

        public MainWindowViewModel(IRegionManager regionManager, ISessionService sessionService)
        {
            _regionManager = regionManager;
            _sessionService = sessionService;
            MenuItems = new ObservableCollection<MenuItem>();
            BuildMenuItems();
        }

        // 根据当前登录用户的权限构建菜单：仅管理员可以访问"用户管理"
        private void BuildMenuItems()
        {
            MenuItems.Clear();
            MenuItems.Add(new MenuItem { Title = "💾  便签管理", NavigationTarget = "NoteManageView", IsEnabled = true });
            MenuItems.Add(new MenuItem
            {
                Title = "💾  用户管理",
                NavigationTarget = "UserManageView",
                IsEnabled = CanAccess("UserManageView")
            });
        }

        // 权限判断的唯一入口：非管理员不允许访问用户管理
        private bool CanAccess(string navigationTarget)
        {
            if (navigationTarget == "UserManageView")
                return _sessionService.CurrentUser?.IsAdmin == true;
            return true;
        }

        private void NavigateTo(MenuItem item)
        {
            // UI 禁用只是第一道防线，导航前再次校验，防止权限被绕过
            if (!item.IsEnabled || !CanAccess(item.NavigationTarget))
                return;
            _regionManager.RequestNavigate("ContentRegion", item.NavigationTarget);
        }

        // 供外部调用以触发初始导航
        public void InitializeNavigation()
        {
            // 主窗口 ViewModel 在登录前就被 Prism 创建（CreateShell），
            // 因此窗口加载（登录完成后）时按当前用户权限重建菜单
            BuildMenuItems();

            if (MenuItems.Count > 0 && SelectedMenuItem == null)
            {
                SelectedMenuItem = MenuItems[0];
            }
        }
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public string NavigationTarget { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
