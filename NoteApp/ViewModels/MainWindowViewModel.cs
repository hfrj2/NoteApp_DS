using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Regions;

namespace NoteApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public ObservableCollection<MenuItem> MenuItems { get; }

        private MenuItem _selectedMenuItem;
        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (SetProperty(ref _selectedMenuItem, value) && value != null)
                {
                    NavigateTo(value.NavigationTarget);
                }
            }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Title = "💾  便签管理", NavigationTarget = "NoteManageView" },
                new MenuItem { Title = "💾  用户管理", NavigationTarget = "UserManageView" }
            };

            // 不在构造函数中直接设置选中项，改为在窗口加载后触发
        }

        private void NavigateTo(string target)
        {
            // Shell 创建时 Prism 已将 ContentRegion 注册到全局 RegionManager，可直接导航
            _regionManager.RequestNavigate("ContentRegion", target);
        }

        // 供外部调用以触发初始导航
        public void InitializeNavigation()
        {
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
    }
}