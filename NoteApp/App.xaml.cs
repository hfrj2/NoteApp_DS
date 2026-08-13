using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using NoteApp.Data;
using NoteApp.Models;
using NoteApp.Repositories;
using NoteApp.Services;
using NoteApp.ViewModels;
using NoteApp.Views;
using NoteApp.Helpers;

namespace NoteApp
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            // 不使用默认 Shell，手动控制窗口显示
            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册数据库上下文工厂
            containerRegistry.RegisterSingleton<IDbContextFactory<AppDbContext>, AppDbContextFactory>();

            // 注册仓储
            containerRegistry.Register<IUserRepository, UserRepository>();
            containerRegistry.Register<INoteRepository, NoteRepository>();

            // 注册服务
            containerRegistry.RegisterSingleton<ISessionService, SessionService>();
            containerRegistry.Register<IUserService, UserService>();
            containerRegistry.Register<INoteService, NoteService>();

            // 注册窗口
            containerRegistry.Register<LoginView>();
            containerRegistry.Register<RegisterView>();
            containerRegistry.Register<MainWindow>();

            // 注册导航视图
            containerRegistry.RegisterForNavigation<NoteManageView, NoteManageViewModel>();
            containerRegistry.RegisterForNavigation<UserManageView, UserManageViewModel>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 初始化数据库
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                // 种子数据：创建默认管理员
                if (!context.Users.Any())
                {
                    var admin = new User
                    {
                        AccountName = "admin",
                        PasswordHash = PasswordHasher.HashPassword("admin123"),
                        Phone = "",
                        Address = "",
                        CreatedAt = DateTime.Now
                    };
                    context.Users.Add(admin);
                    context.SaveChanges();
                }
            }

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // 显示登录窗口
            var loginWindow = Container.Resolve<LoginView>();
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                // 登录成功，显示主窗口
                var mainWindow = Container.Resolve<MainWindow>();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                // 用户取消登录，关闭应用程序
                Application.Current.Shutdown();
            }
        }
    }
}