// Views/MainWindow.xaml.cs
using System.Windows;

namespace NoteApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += (s, e) => {
                if (SessionManager.IsLoggedIn)
                {
                    SessionManager.ClearSession();
                }
            };
        }
    }
}