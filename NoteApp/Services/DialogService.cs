// Services/DialogService.cs
using System.Windows;

namespace NoteApp.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "提示")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirm(string message, string title = "确认")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public string ShowInputDialog(string message, string defaultText = "", string title = "输入")
        {
            var dialog = new Views.InputDialog(message, defaultText, title);
            dialog.Owner = Application.Current.MainWindow;
            var result = dialog.ShowDialog();

            if (result == true)
                return dialog.InputText;

            return null;
        }

        public void ShowError(string message, string title = "错误")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowWarning(string message, string title = "警告")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}