// Views/RegisterView.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoteApp.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegisterViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegisterViewModel vm)
            {
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }

        private void LoginText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is ViewModels.RegisterViewModel vm)
                {
                    if (vm.RegisterCommand.CanExecute(null))
                        vm.RegisterCommand.Execute(null);
                }
            }
        }
    }
}