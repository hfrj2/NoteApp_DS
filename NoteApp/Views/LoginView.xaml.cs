using NoteApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NoteApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            // 将密码框的内容同步到 ViewModel
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is LoginViewModel vm)
                {
                    vm.Password = PasswordBox.Password;
                }
            };
        }
    }
}