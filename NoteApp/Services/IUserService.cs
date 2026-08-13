using System.Collections.Generic;
using NoteApp.Models;

namespace NoteApp.Services
{
    public interface IUserService
    {
        User Login(string accountName, string password);
        bool Register(User user, string password);
        List<User> GetAllUsers();
        User GetUserById(int id);
        void UpdateUser(User user, string newPassword = null);
        void DeleteUser(int userId);
        bool AccountExists(string accountName, int? excludeId = null);
    }
}