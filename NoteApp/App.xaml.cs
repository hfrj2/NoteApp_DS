using NoteApp.Services;
using NoteApp.Views;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace NoteApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<LoginView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDataService, SqliteDataService>();
            containerRegistry.RegisterSingleton<Services.IDialogService, Services.DialogService>();

            containerRegistry.RegisterForNavigation<NoteManageView>("NoteManage");
            containerRegistry.RegisterForNavigation<UserManageView>("UserManage");
            containerRegistry.RegisterForNavigation<RegisterView>("Register");
        }

        protected override async void OnInitialized()
        {
            base.OnInitialized();

            // 初始化数据库
            try
            {
                var dataService = Container.Resolve<IDataService>();
                var result = await dataService.InitializeDatabaseAsync();
                if (!result)
                {
                    MessageBox.Show("数据库初始化失败！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库初始化错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            SessionManager.ClearSession();
        }
    }
}

