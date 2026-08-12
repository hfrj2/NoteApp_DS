// Views/MainWindow.xaml.cs
using NoteApp.ViewModels;
using System.Windows;

namespace NoteApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 窗口完全加载后再执行初始导航，确保区域适配器已正确初始化
            Loaded += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.OnWindowLoaded();
                }
            };

            Closing += (s, e) => {
                if (SessionManager.IsLoggedIn)
                {
                    SessionManager.ClearSession();
                }
            };
        }
    }
}