using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using NoteApp.Models;

namespace NoteApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public User GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Users.Find(id);
        }

        public User GetByAccountName(string accountName)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Users.FirstOrDefault(u => u.AccountName == accountName);
        }

        public List<User> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Users.ToList();
        }

        public void Add(User user)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Users.Add(user);
            context.SaveChanges();
        }

        public void Update(User user)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Users.Update(user);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = context.Users.Find(id);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public bool AccountExists(string accountName, int? excludeId = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Users.Where(u => u.AccountName == accountName);
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }
            return query.Any();
        }
    }
}