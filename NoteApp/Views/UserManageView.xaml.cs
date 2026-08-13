using System.Windows;
using System.Windows.Controls;
using NoteApp.ViewModels;

namespace NoteApp.Views
{
    public partial class UserManageView : UserControl
    {
        public UserManageView()
        {
            InitializeComponent();
            // 密码框密码同步到 ViewModel
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is UserManageViewModel vm)
                {
                    vm.EditPassword = PasswordBox.Password;
                }
            };
        }
    }
}