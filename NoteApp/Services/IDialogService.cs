// Services/IDialogService.cs
namespace NoteApp.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "提示");
        bool ShowConfirm(string message, string title = "确认");
        string ShowInputDialog(string message, string defaultText = "", string title = "输入");
        void ShowError(string message, string title = "错误");
        void ShowWarning(string message, string title = "警告");
    }
}