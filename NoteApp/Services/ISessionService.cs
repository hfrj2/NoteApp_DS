using NoteApp.Models;

namespace NoteApp.Services
{
    public interface ISessionService
    {
        User CurrentUser { get; set; }
        bool IsLoggedIn { get; }
    }
}