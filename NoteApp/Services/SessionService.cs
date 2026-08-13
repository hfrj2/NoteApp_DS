using NoteApp.Models;

namespace NoteApp.Services
{
    public class SessionService : ISessionService
    {
        public User CurrentUser { get; set; }
        public bool IsLoggedIn => CurrentUser != null;
    }
}