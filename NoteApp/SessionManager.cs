// SessionManager.cs
namespace NoteApp
{
    public static class SessionManager
    {
        private static int _currentUserId;
        private static string _currentUsername;
        private static string _currentUserRole;

        public static int CurrentUserId
        {
            get => _currentUserId;
            set => _currentUserId = value;
        }

        public static string CurrentUsername
        {
            get => _currentUsername;
            set => _currentUsername = value;
        }

        public static string CurrentUserRole
        {
            get => _currentUserRole;
            set => _currentUserRole = value;
        }

        public static bool IsLoggedIn => CurrentUserId > 0;

        public static void ClearSession()
        {
            CurrentUserId = 0;
            CurrentUsername = string.Empty;
            CurrentUserRole = string.Empty;
        }
    }
}