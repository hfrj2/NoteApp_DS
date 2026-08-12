// Views/InputDialog.xaml.cs
using System.Windows;

namespace NoteApp.Views
{
    public partial class InputDialog : Window
    {
        public string InputText { get; private set; }
        public string Message { get; set; }

        public InputDialog(string message, string defaultText = "", string title = "输入")
        {
            InitializeComponent();
            DataContext = this;

            Message = message;
            InputText = defaultText;
            Title = title;

            txtInput.Focus();
            txtInput.SelectAll();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = txtInput.Text;
            DialogResult = true;
            Close();
        }
    }
}