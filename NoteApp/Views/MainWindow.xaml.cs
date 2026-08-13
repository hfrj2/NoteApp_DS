using NoteApp.ViewModels;
using System.Windows;

namespace NoteApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 窗口加载完成后触发初始导航，默认显示第一个页面
            Loaded += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.InitializeNavigation();
                }
            };
        }
    }
}