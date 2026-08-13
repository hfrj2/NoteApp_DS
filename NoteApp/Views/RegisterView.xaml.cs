using NoteApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NoteApp.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is RegisterViewModel vm)
                {
                    vm.Password = PasswordBox.Password;
                }
            };
            ConfirmPasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is RegisterViewModel vm)
                {
                    vm.ConfirmPassword = ConfirmPasswordBox.Password;
                }
            };
        }
    }
}