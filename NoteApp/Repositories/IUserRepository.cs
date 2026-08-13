using System.Collections.Generic;
using NoteApp.Models;

namespace NoteApp.Repositories
{
    public interface IUserRepository
    {
        User GetById(int id);
        User GetByAccountName(string accountName);
        List<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        bool AccountExists(string accountName, int? excludeId = null);
    }
}