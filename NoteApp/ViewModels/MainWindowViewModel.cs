using System.Collections.ObjectModel;
using Prism.Commands;
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
                new MenuItem { Title = "便签管理", NavigationTarget = "NoteManageView" },
                new MenuItem { Title = "用户管理", NavigationTarget = "UserManageView" }
            };

            // 默认选中第一个菜单并导航
            SelectedMenuItem = MenuItems[0];
        }

        private void NavigateTo(string target)
        {
            _regionManager.RequestNavigate("ContentRegion", target);
        }
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public string NavigationTarget { get; set; }
    }
}