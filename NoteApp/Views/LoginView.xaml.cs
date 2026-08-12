// Views/LoginView.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoteApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            Loaded += (s, e) => {
                UsernameBox.Focus();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is ViewModels.LoginViewModel vm)
                {
                    if (vm.LoginCommand.CanExecute(null))
                        vm.LoginCommand.Execute(null);
                }
            }
        }
    }
}