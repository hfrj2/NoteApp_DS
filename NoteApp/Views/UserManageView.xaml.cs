// Views/UserManageView.xaml.cs
using System.Windows.Controls;

namespace NoteApp.Views
{
    public partial class UserManageView : UserControl
    {
        public UserManageView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserManageViewModel vm && vm.EditingUser != null)
            {
                vm.EditingUser.Password = PasswordBox.Password;
            }
        }
    }
}